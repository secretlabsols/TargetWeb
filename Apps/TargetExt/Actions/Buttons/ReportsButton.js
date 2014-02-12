Ext.define('Actions.Buttons.ReportsButton', {
    extend: 'Actions.ActionButton',
    iconCls: 'ReportImage',
    text: 'Reports',
    tooltip: 'View Reports?',
    statics: {
        instanceCount: 0
    },
    config: {
        autoRegisterEventsControls: [],
        autoSort: false,
        btnExport: null,
        btnView: null,
        divExport: null,
        eventNameOnReportsButtonClicked: 'onReportsButtonClicked',
        gridReports: null,
        initCallBackFunc: null,
        isInited: false,
        reports: [],
        reportIds: [],
        reportParameters: [],
        reportParameterClasses: {},
        reportParameterWindow: null,
        reportParameterWindowCurrent: null,
        selectedReport: null,
        service: null,
        store: null,
        window: null,
        createUsingJob: false
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
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup button
        me.handler = function () {
            me.raiseReportsButtonClicked();
        };
        cfg.service = Target.Abacus.Library.Reports.Services.ReportsService;
        // init the parent
        me.callParent(arguments);
    },
    initControls: function (show, initCallBackFunc) {
        var me = this, cfg = me.config, apcls = 'ActionPanels';
        show = !(show === false);
        cfg.initCallBackFunc = initCallBackFunc;
        if (!cfg.isInited) {
            if (cfg.reportIds && cfg.reports.length == 0) {
                Ext.define('ReportsButtonItem', {
                    extend: 'Ext.data.Model',
                    fields: [
                        { name: 'ID' },
                        { name: 'Description' },
                        { name: 'RowLimit' },
                        { name: 'ParametersUiClass' }
                    ],
                    idProperty: 'ID'
                });
                cfg.store = Ext.create('Ext.data.Store', {
                    autoLoad: true,
                    data: [],
                    model: 'ReportsButtonItem',
                    proxy: {
                        type: 'memory',
                        reader: {
                            type: 'json',
                            root: 'Items'
                        }
                    }
                });
                cfg.btnView = Ext.create('Ext.button.Button', {
                    cls: apcls,
                    iconCls: 'PreviewImage',
                    text: 'View',
                    handler: function () {
                        me.viewReport();
                    },
                    tooltip: 'View Report?'
                });
                cfg.btnExport = Ext.create('Ext.button.Split', {
                    handler: function (cmp) {
                        cmp.showMenu();
                    },
                    cls: apcls, iconCls: 'ExportExcelImage',
                    text: 'Export',
                    tooltip: 'View Export Options?',
                    menu: new Ext.menu.Menu({
                        defaults: {
                            cls: apcls
                        },
                        items: [
                            { text: 'CSV (Unlimited)', iconCls: 'ExportCsvImage', tooltip: 'Export upto the first million records via Job Service in CSV format?', handler: function () { me.exportReportViaJobService(); } },
                            { text: 'Excel', iconCls: 'ExportExcelImage', tooltip: 'Export to Excel?', handler: function () { me.viewReport({ format: 'EXCEL' }); } },
                            { text: 'PDF', iconCls: 'ExportPdfImage', tooltip: 'Export to PDF?', handler: function () { me.viewReport({ format: 'PDF' }); } }
                        ]
                    })
                });
                cfg.gridReports = Ext.create('Ext.grid.Panel', {
                    border: 0,
                    columnLines: true,
                    columns: [
                        { header: 'Description', dataIndex: 'Description', flex: 6 },
                        { header: 'Row Limit', dataIndex: 'RowLimit', flex: 1.5, renderer: function (value) { return ((value > 0) ? value : 'No Limit'); } },
                        { header: 'Additional Parameters?', dataIndex: 'ParametersUiClass', flex: 2.5, renderer: function (value) { return ((Ext.isEmpty(value)) ? 'No' : 'Yes'); } }
                    ],
                    enableColumnHide: false,
                    enableColumnMove: false,
                    enableColumnResize: false,
                    flex: 10,
                    stateful: false,
                    sortableColumns: true,
                    store: cfg.store,
                    viewConfig: {
                        stripeRows: true
                    },
                    listeners: {
                        select: {
                            fn: function (vw, rcd) {
                                cfg.selectedReport = rcd.data;
                                cfg.btnView.setTooltip('View the first ' + cfg.selectedReport.RowLimit + ' records of the report <br /> \'' + cfg.selectedReport.Description + '\'');
                                cfg.btnExport.setTooltip('View export options for the report <br /> \'' + cfg.selectedReport.Description + '\'');
                                cfg.btnExport.menu.items.items[0].setTooltip('Export all records of the report <br /> \'' + cfg.selectedReport.Description + '\' via job service in CSV format');
                                cfg.btnExport.menu.items.items[1].setTooltip('Export the first ' + cfg.selectedReport.RowLimit + ' records of the report <br /> \'' + cfg.selectedReport.Description + '\' to Excel format');
                                cfg.btnExport.menu.items.items[2].setTooltip('Export the first ' + cfg.selectedReport.RowLimit + ' records of the report <br /> \'' + cfg.selectedReport.Description + '\' to PDF format');
                            },
                            scope: me
                        },
                        itemdblclick: {
                            fn: function (vw, rcd) {
                                me.viewReport();
                            },
                            scope: me
                        }
                    }
                });
                cfg.window = Ext.create('Ext.window.Window', {
                    animateTarget: me,
                    bbar: [
                        { xtype: 'tbfill' },
                        cfg.btnView,
                        cfg.btnExport
                    ],
                    border: 0,
                    closeAction: 'hide',
                    constrain: true,
                    items: [cfg.gridReports],
                    layout: {
                        type: 'fit',
                        align: 'stretch'
                    },
                    loadMask: true,
                    maxHeight: 400,
                    minHeight: 100,
                    modal: true,
                    resizable: false,
                    title: 'Reports',
                    width: 600
                });
            }
        }
        if (show) {
            cfg.window.show();
        }
        cfg.service.FetchReportsByIDS(cfg.reportIds, me.initControlsCallback, me);
    },
    initControlsCallback: function (response) {
        var me = response.context, cfg = me.config;
        cfg.isInited = true;
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.reports = response.value.Items;
        cfg.store.loadData(cfg.reports);
        if (cfg.autoSort) {
            cfg.store.sort({ property: 'Description', direction: 'ASC' });
        }
        cfg.window.setLoading(false);
        cfg.gridReports.getView().select(0);
        cfg.btnExport.menu.items.items[0].setDisabled(!response.value.CanExportReportsAsCsv);
        if (cfg.initCallBackFunc) {
            cfg.initCallBackFunc();
            cfg.initCallBackFunc = null;
        }
    },
    exportReportViaJobService: function () {
        var me = this, cfg = me.config, report = cfg.selectedReport, exportRequest = { ID: report.ID, Parameters: [] };
        cfg.window.setLoading('Creating Job...');
        cfg.lastArgs = me.getViewReportArgs();


        cfg.createUsingJob = true;

        Ext.Array.each(me.getReportParameters(cfg.lastArgs).params, function (param) {
            exportRequest.Parameters.push({ key: param.key, value: param.value });
        });

        if (Ext.isEmpty(report.ParametersUiClass)) {
            cfg.service.CreateExportReportJob(exportRequest, me.exportReportViaJobServiceCallback, me);
        } else {
            me.getAdditionalReportParameters(report.ParametersUiClass);
        }

    },
    exportReportViaJobServiceCallback: function (response) {
        var me = response.context, cfg = me.config;
        cfg.window.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        Ext.MessageBox.show({
            animateTarget: cfg.btnExport,
            buttons: Ext.MessageBox.OK,
            icon: Ext.MessageBox.INFO,
            msg: 'Your report has been successfully queued for export and you will be notified by email when it has completed.',
            title: 'Export Queued Successfully'
        });
    },
    getAdditionalReportParameters: function (className) {
        var me = this, cfg = me.config, parameterPanel, parameterPanelComp, report = me.getReport(cfg.lastArgs.id);
        if (!cfg.reportParameterWindow) {
            cfg.reportParameterWindow = Ext.create('Ext.window.Window', {
                bbar: [
                            { xtype: 'tbfill' },
                            {
                                handler: function (cmp) {
                                    me.handleAdditionalReportParametersSet();
                                },
                                text: 'Continue',
                                tooltip: 'Continue to Report?'
                            }
                        ],
                border: 0,
                closeAction: 'hide',
                constrain: true,
                layout: {
                    type: 'fit',
                    align: 'stretch'
                },
                loadMask: true,
                modal: true,
                resizable: false,
                title: 'Additional Parameters'
            });
        }
        //me.createParamWindow(className);

        if (cfg.reportParameterWindowCurrent) {
            cfg.reportParameterWindowCurrent.hide();
        }
        parameterPanel = cfg.reportParameterClasses[className];
        //if (!parameterPanel) {
        parameterPanel = Ext.create('Reports.Params.' + className, { report: report });
        cfg.reportParameterClasses[className] = parameterPanel;
        cfg.reportParameterWindow.add(parameterPanel);
        //}
        cfg.reportParameterWindowCurrent = parameterPanel;
        parameterPanel.report = report;
        //parameterPanel.show();
        cfg.reportParameterWindow.setTitle('Additional Parameters: ' + report.Description);
        cfg.reportParameterWindow.show();
    },
    getReport: function (id) {
        var me = this, cfg = me.config, report;
        Ext.Array.each(cfg.reports, function (rpt) {
            if (rpt.ID === id) {
                report = rpt;
                return false;
            }
        });
        return report;
    },
    getReportParameters: function (args) {
        var me = this, cfg = me.config, report = me.getReport(args.id), args = (args || me.getViewReportArgs(args)), reportParams = report.Parameters;
        Ext.Array.each(args.params, function (param) {
            if (param.ids.length == 0) {
                reportParams = param;
            } else {
                Ext.Array.each(param.ids, function (id) {
                    if (id == args.id) {
                        reportParams = param;
                        return false;
                    }
                });
            }
            if (reportParams) {
                return false;
            }
        });
        report.Parameters = Ext.clone(reportParams);
        return report.Parameters;
    },
    getReportParametersForDirectView: function (rptId, rptFormat, key, value) {
        return {
            id: rptId,
            format: rptFormat,
            direct: true,
            params: [
                {
                    ids: [rptId],
                    params: [
                        {
                            key: key,
                            value: value
                        }
                    ]
                }
            ]
        };
    },
    getReportUrl: function (args, params) {
        var me = this, cfg = me.config, args = (args || me.getViewReportArgs(args)), url = SITE_VIRTUAL_ROOT + 'Apps/Reports/Viewer.aspx?rID=' + args.id.toString();
        params = (params || me.getReportParameters(args).params);
        Ext.Array.each(params, function (param) {
            url += '&' + param.key + '=' + ((param.value) ? param.value : '');
        });
        if (args.format) {
            url += '&rc:Command=Render';
            url += '&rc:Format=' + args.format;
        }
        return url;
    },
    getViewReportArgs: function (args) {
        var me = this, cfg = me.config;
        args = Ext.merge({
            id: 0,
            format: null,
            params: []
        }, args);
        if (args.id === 0 && cfg.selectedReport) {
            args.id = cfg.selectedReport.ID
        }
        if (!args.params || args.params.length === 0) {
            args.params = cfg.reportParameters;
        }
        return args;
    },
    handleAdditionalReportParametersSet: function () {
        var me = this, cfg = me.config, addParams = cfg.reportParameterWindowCurrent.getParameters(), report = cfg.selectedReport, exportRequest = { ID: report.ID, Parameters: [] };
        if (addParams && addParams.success) {
            var report = me.getReport(cfg.lastArgs.id), reportParams = me.getReportParameters(cfg.lastArgs).params, reportParamMatched = false;
            cfg.reportParameterWindow.hide();
            Ext.Array.each(addParams.params, function (arparam) {
                reportParamMatched = false
                Ext.Array.each(reportParams, function (rparam) {
                    if (arparam.key === rparam.key) {
                        rparam.value = arparam.value;
                        reportParamMatched = true;
                        return false;
                    }
                });
                if (!reportParamMatched) {
                    reportParams.push({ key: arparam.key, value: arparam.value });
                }
            });

            if (cfg.createUsingJob == true) {
                exportRequest.Parameters = reportParams;
                cfg.service.CreateExportReportJob(exportRequest, me.exportReportViaJobServiceCallback, me);
                //cfg.service.CreateExportReportJob(report.ID, reportParams, me.exportReportViaJobServiceCallback, me);
                report.ID
            } else {
                me.openReport(reportParams);
            }


        }
    },
    handleReportViewed: function () {
        var me = this, cfg = me.config, args = cfg.lastArgs;
        if (cfg.window && !args.direct) {
            cfg.window.setLoading(false);
        } else {
            Ext.getBody().unmask();
        }
    },
    openReport: function (params) {
        var me = this, cfg = me.config;
        OpenReport(me.getReportUrl(cfg.lastArgs, params), ((cfg.divExport) ? cfg.divExport.id : ''), function () { me.handleReportViewed() });
    },
    raiseReportsButtonClicked: function () {
        var me = this, cfg = me.config;
        me.raiseEventsToObservers(cfg.eventNameOnReportsButtonClicked, cfg.autoRegisterEventsControls);
    },
    viewReport: function (args) {
        var me = this, cfg = me.config, args = me.getViewReportArgs(args), exportingMsg = 'Exporting...',
        report = me.getReport(args.id), reportsDiv, reportsDivName = 'divReportsButtonExporter';

        cfg.createUsingJob = false;

        if (args.direct) {
            cfg.reportParameters = args.params;
        }
        if (args.format) {
            if (cfg.window && !args.direct) {
                cfg.window.setLoading(exportingMsg);
            } else {
                Ext.getBody().mask(exportingMsg);
            }
        }
        if (!cfg.divExport) {
            reportsDiv = Ext.get(reportsDivName);
            if (!reportsDiv) {
                cfg.divExport = Ext.getBody().createChild({
                    tag: 'div'
                    , cls: 'x-hidden'
                    , id: reportsDivName
                    , name: reportsDivName
                });
            } else {
                cfg.divExport = reportsDiv;
            }
        }
        cfg.lastArgs = args;
        if (Ext.isEmpty(report.ParametersUiClass)) {
            me.openReport();
        } else {
            me.getAdditionalReportParameters(report.ParametersUiClass);
        }
    },
    viewReports: function (params) {
        var me = this, cfg = me.config;
        cfg.reportParameters = params;
        me.initControls();
        if (cfg.isInited) {
            cfg.window.show();
        }
    }
});

Ext.define('Actions.Buttons.ReportsButtonMenuItem', {
    extend: 'Ext.menu.Item',
    cls: 'ActionPanels',
    iconCls: 'ReportImage',
    text: 'Reports',
    tooltip: 'View Reports for the Selected Item?',
    requires: [
        'Actions.Buttons.ReportsButton'
    ],
    config: {
        reportsButton: null,
        reportsButtonConfig: null
    },
    constructor: function(config) {
        var me = this, cfg = me.config;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        me.handler = function() {
            me.initControls();
            cfg.reportsButton.raiseReportsButtonClicked();
        };
        this.callParent(arguments);
    },
    initControls: function() {
        var me = this, cfg = me.config;
        if (!cfg.reportsButton) {
            cfg.reportsButtonConfig.renderTo = Ext.getBody();
            cfg.reportsButtonConfig.hidden = true;
            cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', cfg.reportsButtonConfig);
        }
    }
});
