Ext.define('Actions.Controls.RateCategoryActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.NewButton',
        'Actions.Buttons.ReportsButton'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // use me.resultSettings.HasPermission to determine if the current users has a permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('AddNew');
        // add any controls into the config object so they are accessible else where
        cfg.addButton = Ext.create('Actions.Buttons.NewButton', {
            autoRegisterEventsControls: [me]
        });
        cfg.sortOrderButton = Ext.create('Ext.Button', {
            text: 'Sort Order',
            cls: 'ActionPanels',
            disabled: true,
            iconCls: 'SortOrderImage',
            handler: function () {
                me.fireEvent('onSortOrderButtonClicked');
            }
        });
        cfg.enhancedRateButton = Ext.create('Ext.Button', {
            text: 'Enhanced Rates',
            cls: 'ActionPanels',
            disabled: true,
            iconCls: 'EnhancedRatesImage',
            handler: function () {
                me.fireEvent('onEnhancedRatesButtonClicked');
            }
        });
        // add the reports button
        cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', {
            autoRegisterEventsControls: [me]
        });
        // use cfg.permissionCanAddNew to determine if this control
        // is disabled or not...or directly use me.resultSettings.HasPermission(
        cfg.addButton.setDisabled(!cfg.permissionCanAddNew);
        // add the reports button
        cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', {
            autoRegisterEventsControls: [me],
            reportIds: me.resultSettings.ReportIdsForActionPanel
        });
        // add controls created above into the controls items 
        // in the order they are expected on screen
        me.items = [
            cfg.addButton,
            cfg.sortOrderButton,
            cfg.enhancedRateButton,
            '->',
            cfg.reportsButton
        ];
        // init the parent
        me.callParent(arguments);
    },
    disableNewButton: function (disable) {
        var me = this, cfg = me.config;
        if (!disable && !cfg.permissionCanAddNew) {
            disable = true;
        }
        cfg.addButton.setDisabled(disable);
    },
    disableSortOrderButton: function (disable) {
        var me = this, cfg = me.config;
        cfg.sortOrderButton.setDisabled(disable);
    },
    disableEnhancedRatesButton: function (disable) {
        var me = this, cfg = me.config;
        cfg.enhancedRateButton.setDisabled(disable);
    }
});