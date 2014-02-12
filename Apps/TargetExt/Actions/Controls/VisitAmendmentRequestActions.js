Ext.define('Actions.Controls.VisitAmendmentRequestActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.NewButton'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        
        // add the reports button
        cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', {
            autoRegisterEventsControls: [me],
            reportIds: me.resultSettings.ReportIdsForActionPanel
        });
        me.items = [
            cfg.reportsButton
        ];
        // init the parent
        me.callParent(arguments);
    }
});

