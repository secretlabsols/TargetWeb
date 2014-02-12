Ext.define('Actions.Buttons.AuditLogButton', {
    extend: 'Actions.ActionButton',
    config: {
        auditLogAutoShow: true,
        auditLogHeight: 600,
        auditLogID: 0,
        auditLogTables: [],
        auditLogUseApplicationFilter: true,
        auditLogWebNavMenuItemID: 0,
        auditLogWidth: 800,
        autoRegisterEventsControls: [],
        eventNameOnAuditLogButtonClicked: 'onAuditLogButtonClicked'
    },
    constructor: function (config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup button
        me.cls = 'ActionPanels';
        me.iconCls = 'AuditLogImage';
        me.handler = function () {
            if (cfg.auditLogAutoShow) {
                // if auto show then show the window
                me.showAuditLogWindow();
            } else {
                // otherwise raise an event and others will show window
                me.raiseAuditLogButtonClicked();
            }
        };
        me.text = 'Audit Log';
        me.tooltip = 'View Audit Log';
        // init the parent
        me.callParent(arguments);
    },
    // advise observers that the button has been clicked
    raiseAuditLogButtonClicked: function () {
        var me = this, cfg = me.config;
        me.raiseEventsToObservers(cfg.eventNameOnAuditLogButtonClicked, cfg.autoRegisterEventsControls);
    },
    // set the config settings using a result settings object
    setConfigFromResultSettings: function (args) {
        var me = this, cfg = me.config;
        if (args && args.AuditDetails) {
            cfg.auditLogID = args.AuditDetails.ID;
            cfg.auditLogTables = args.AuditDetails.TableNames;
            cfg.auditLogUseApplicationFilter = args.AuditDetails.UseApplicationFilter;
            cfg.auditLogWebNavMenuItemID = args.WebNavMenuItemID;
        }
    },
    setConfigSettings: function (args) {
        var me = this, cfg = me.config;
        if (args) {
            cfg.auditLogID = args.auditLogID;
            cfg.auditLogTables = args.auditLogTables;
            cfg.auditLogUseApplicationFilter = args.auditLogUseApplicationFilter;
            cfg.auditLogWebNavMenuItemID = args.auditLogWebNavMenuItemID;
        }
    },
    // show the audit log window
    showAuditLogWindow: function () {
        var me = this, cfg = me.config, tables = (cfg.auditLogTables || []).join('|'), url, ifrAuditPopup = 'ifrAuditPopup';
        // validate the id is set
        cfg.auditLogID = parseInt(cfg.auditLogID);
        if (isNaN(cfg.auditLogID) || cfg.auditLogID <= 0) {
            Ext.Msg.alert('auditLogID Required', 'Please specify the auditLogID parameter.');
            return false;
        }
        // validate the menu id is set
        cfg.auditLogWebNavMenuItemID = parseInt(cfg.auditLogWebNavMenuItemID);
        if (isNaN(cfg.auditLogWebNavMenuItemID) || cfg.auditLogWebNavMenuItemID <= 0) {
            Ext.Msg.alert('auditLogWebNavMenuItemID Required', 'Please specify the auditLogWebNavMenuItemID parameter.');
            return false;
        }
        // validate at least one table is set
        if (!tables || tables.length == 0) {
            Ext.Msg.alert('auditLogTables Required', 'Please specify the auditLogTables parameter.');
            return false;
        }
        // create the url
        url = SITE_VIRTUAL_ROOT + 'Apps/AuditLog/Popup.aspx?sc=0&tbl=|' + tables + '|&id=' + cfg.auditLogID.toString() + '&wnmi=' + cfg.auditLogWebNavMenuItemID.toString() + '&UseApplicationFilter=' + cfg.auditLogUseApplicationFilter;
        // open the window in a popup
        new Ext.Window({
            title: 'Audit Log',
            width: cfg.auditLogWidth,
            height: cfg.auditLogHeight,
            layout: 'fit',
            items: [{
                xtype: 'component',
                id: ifrAuditPopup,
                name: ifrAuditPopup,
                autoEl: {
                    tag: 'iframe',
                    src: url
                }
            }],
            maximizable: true,
            modal: true,
            bbar: [
                { xtype: 'tbfill' },
                {
                    text: 'Copy',
                    cls: 'ActionPanels',
                    iconCls: 'CopyImage',
                    tooltip: 'Copy?',
                    handler: function (cmp) {
                        Ext.getDom(ifrAuditPopup).contentWindow.btnCopy_Click();
                    }
                },
                {
                    text: 'Print',
                    cls: 'ActionPanels',
                    iconCls: 'PrintEdit',
                    tooltip: 'Print?',
                    handler: function (cmp) {
                        Ext.getDom(ifrAuditPopup).contentWindow.print();
                    }
                }
            ],
            resizable: false,
            constrain: true
        }).show();
    }
});
