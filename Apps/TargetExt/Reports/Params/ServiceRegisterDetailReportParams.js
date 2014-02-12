Ext.define('Reports.Params.ServiceRegisterDetailReportParams', {
    extend: 'Reports.ReportParameterControl',
    initComponent: function() {
        var me = this, cfg = me.config, mdl, mdlName = 'ServiceRegisterDetailReportParamsModel';
        // setup and create instance of model
        Ext.define(mdlName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'ServiceUser', mapping: 'ServiceUser' },
                { name: 'ServiceType', mapping: 'ServiceType' },
                { name: 'RegisterGroup', mapping: 'RegisterGroup' }
            ]
        });
        mdl = Ext.create(mdlName, { ServiceUserID: null, ServiceType: null, RegisterGroup: null });
        // setup form, load and add to ctrl
        me.setupFrm();
        cfg.frm.loadRecord(mdl);
        me.items = [
            cfg.frm
        ];
        // call parent init
        me.callParent(arguments);
    },
    getParameters: function() {
        var me = this, cfg = me.config, frm = cfg.frm.getForm(), rcd = frm.getRecord();
        frm.updateRecord(rcd);
        return {
            success: true,
            params: [
                {
                    key: 'suid',
                    value: rcd.data.ServiceUser.ID
                },
                {
                    key: 'stid',
                    value: rcd.data.ServiceType
                }
            ]
        };
    },
    setupFrm: function() {
        var me = this, cfg = me.config;
        if (!cfg.frm) {
            // get data from server
            var frmData = me.getData();
            // create service user in place selector
            cfg.serviceUserSelector = Ext.create('InPlaceSelectors.Controls.ServiceUserInPlaceSelector', {
                fieldLabel: 'Service User',
                name: 'ServiceUser',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            // create service types
            cfg.storeServiceTypes = Ext.create('Ext.data.Store', {
                fields: ['Description', 'ID'],
                data: frmData.ServiceTypes,
                remoteSort: false
            });
            cfg.cmbServiceTypes = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Service Type',
                store: cfg.storeServiceTypes,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                labelWidth: 100,
                anchor: '100%',
                name: 'ServiceType',
                plugins: ['clearbutton']
            });
            // create form containing controls
            cfg.frm = Ext.create('Ext.form.Panel', {
                border: 0,
                items: [
                    cfg.serviceUserSelector,
                    cfg.cmbServiceTypes
                ]
            });
        }
    }
});
