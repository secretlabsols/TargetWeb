Ext.define('Actions.Controls.NonResidentialServiceUserPaymentActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.ReportsButton'
    ],
    config: {
        authoriseButton: null,
        createBatchButton: null,
        suspensionsButton: null
    },
    constructor: function(config) {
        var me = this, cfg = me.config;
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', {
            autoRegisterEventsControls: [me],
            reportIds: me.resultSettings.ReportIdsForActionPanel
        });
        // add buttons
        me.items = [
            '->',
            cfg.reportsButton
        ];
        // init the parent
        me.callParent(arguments);
    }
});

