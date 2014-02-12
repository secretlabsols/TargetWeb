Ext.define('Target.ux.file.Downloader', {
    statics: {
        instanceCount: 0
    },
    config: {
        downloaderBody: null,
        downloaderFrame: null,
        downloaderForm: null,
        downloaderCancelButton: null,
        downloaderCancellable: false,   // configurable
        downloaderCheckInterval: 250,   // configurable - millisecs
        downloaderCheckIntervalMax: 60000,   // configurable - millisecs
        downloaderIsInited: false,
        downloaderProgress: null,
        downloaderTarget: null, // configurable - string or element
        downloaderWindow: null
    },
    constructor: function (config) {
        var me = this, cfg = me.config;
        me.initConfig(config);
        // increment number of instances
        me.self.instanceCount++;
        // set parent config
        me.callParent([config]);
        return me;
    },
    downloadInit: function () {
        var me = this, cfg = me.config;
        if (!cfg.downloaderIsInited) {
            // set ref to the body
            cfg.downloaderBody = Ext.getBody();
            // set ref to the iframe we will use when downloading
            cfg.downloaderFrame = cfg.downloaderBody.createChild({
                tag: 'iframe'
                , cls: 'x-hidden'
                , id: 'iframe' + me.self.instanceCount.toString()
                , name: 'iframe' + me.self.instanceCount.toString()
            });
            // set ref to the form we will use when downloading
            cfg.downloaderForm = cfg.downloaderBody.createChild({
                tag: 'form'
                , cls: 'x-hidden'
                , id: 'form' + me.self.instanceCount.toString()
                , target: cfg.downloaderFrame.dom.id
                , method: 'post'
            });
            // set the cancel button
            cfg.downloaderCancelButton = Ext.create('Ext.Button', {
                text: 'Cancel',
                handler: function () {
                    me.stopDownload();
                },
                tooltip: 'Cancel Download?',
                hidden: !me.getDownloaderCancellable()
            });
            // set the progress bar
            cfg.downloaderProgress = Ext.create('Ext.ProgressBar', {
                margin: 5,
                border: 1
            });
            // set the downloader window
            cfg.downloaderWindow = Ext.create('Ext.window.Window', {
                animateTarget: me.getDownloaderTarget(),
                bbar: [
                    { xtype: 'tbfill' },
                    cfg.downloaderCancelButton
                ],
                bodyStyle: 'background-color: #FFFFFF',
                border: 0,
                closable: me.getDownloaderCancellable(),
                closeAction: 'hide',
                constrain: true,
                draggable: false,
                items: [
                    cfg.downloaderProgress
                ],
                listeners: {
                    hide: {
                        fn: function () {
                            me.stopDownload();
                        }
                    }
                },
                loadMask: true,
                resizable: false,
                title: 'Downloading File...',
                width: 400,
                modal: true
            });
            cfg.downloaderIsInited = true;
        }
    },
    download: function (config) {
        var me = this, cfg = me.config;
        // create any controls we may need for the download
        me.downloadInit();
        if (!cfg.currentDownloadID) {
            // create an id to correlate download with server
            cfg.currentDownloadID = 'Downloader' + new Date().getTime().toString();
            // block input whilst downloading
            cfg.downloaderProgress.updateProgress(1, 'Initialising...');
            cfg.downloaderWindow.show();
            // extend config
            Ext.apply(
                {
                    url: null,
                    data: []
                },
                config
            );
            // set the url of the item to download
            cfg.downloaderForm.dom.action = config.url;
            // remove any existing inputs
            cfg.downloaderForm.dom.innerHTML = "";
            // loop each data item and add as a form input
            Ext.Array.each(config.data, function (itm) {
                Ext.apply(
                    {
                        key: null,
                        value: null
                    },
                    itm
                );
                if (itm.key) {
                    cfg.downloaderForm.createChild({
                        tag: 'input'
                        , cls: 'x-hidden'
                        , id: itm.key
                        , name: itm.key
                        , value: itm.value
                    });
                }
            });
            // create an id to correlate download with server
            cfg.downloaderForm.createChild({
                tag: 'input'
                , cls: 'x-hidden'
                , id: 'DownloaderCookieID'
                , name: 'DownloaderCookieID'
                , value: cfg.currentDownloadID
            });
            // submit the form to send values to server
            cfg.downloaderForm.dom.submit();
            // query cookies until find correlating
            cfg.downloaderTask = {
                run: function (iterations) {
                    var cookie = Ext.util.Cookies.get(cfg.currentDownloadID),
                    cookieFound = false,
                    checkInterval = me.getDownloaderCheckInterval(),
                    checkIntervalMax = me.getDownloaderCheckIntervalMax(),
                    totalTimeUsed = (iterations * checkInterval),
                    totalTimeUsedPercent = (((100 / checkIntervalMax) * totalTimeUsed) / 100)
                    timedOut = (totalTimeUsed >= checkIntervalMax);
                    cfg.downloaderProgress.updateProgress((1 - totalTimeUsedPercent), ((checkIntervalMax - totalTimeUsed) / 1000) + ' seconds until timeout');
                    if (cookie === 'true') {
                        cookieFound = true;
                    } else if (cookie === 'false') {
                        var errMsg = cfg.downloaderFrame.dom.contentWindow.document.body.innerHTML;
                        if (errMsg) {
                            cookieFound = true;
                            Ext.Msg.alert('Error', errMsg);
                        } else {
                            cookieFound = false;
                        }
                    }
                    if (cookieFound || timedOut) {
                        me.stopDownload();
                    }
                    if (timedOut) {
                        Ext.Msg.alert('Timed Out', 'Your download has timed out as it has taken longer than '
                                            + (checkIntervalMax / 1000).toString()
                                            + ' seconds. Please try again.');
                    }
                },
                interval: me.getDownloaderCheckInterval()
            }
            // start a task to monitor if download has completed
            Ext.TaskManager.start(cfg.downloaderTask);
        } else {
            Ext.Msg.alert('Error', 'A file is already being downloaded. Please wait until the download has been completed and try again.');
        }
    },
    stopDownload: function () {
        var me = this, cfg = me.config;
        if (cfg.currentDownloadID) {
            cfg.currentDownloadID = null;
            Ext.TaskManager.stop(cfg.downloaderTask);
            Ext.util.Cookies.clear(cfg.currentDownloadID);
            cfg.downloaderWindow.hide();
            setTimeout(function () {
                cfg.downloaderFrame.dom.src = 'javascript:\'<html></html>\';'
            }, 100);
        }
    }
});