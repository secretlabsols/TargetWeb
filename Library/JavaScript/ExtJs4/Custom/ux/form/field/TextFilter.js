Ext.define('Target.ux.form.field.TextFilter', {
    extend: 'Ext.form.FieldContainer',
    anchor: '100%',
    fieldLabel: '',
    comboWidth: 100,
    textboxWidth: 100,
    layout: 'fit',
    config: {
        filterOptionStore: null
    },
    constructor: function(config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function() {
        var me = this, cfg = me.config, isReadOnly = ((cfg.filterOptionStoreData || []).length <= 1);
        cfg.filterOptionStoreData = (cfg.filterOptionStoreData || []);
        if (cfg.filterOptionStoreData.length == 0) {
            cfg.filterOptionStoreData = [{ value: 1, description: 'Contains'}]
        }
        cfg.filterOptionStore = Ext.create('Ext.data.Store', {
            fields: ['value', 'description'],
            data: cfg.filterOptionStoreData
        });
        cfg.TextFilterDropDownControl = Ext.create('Ext.form.ComboBox',
            Ext.merge(
            {
                width: 220,
                store: cfg.filterOptionStore,
                queryMode: 'local',
                displayField: 'description',
                emptyText: 'Select an item...',
                valueField: 'value',
                editable: false,
                padding: '0 0 0 0',
                readOnly: isReadOnly
            },
            cfg.textFilterDropDownControlConfig
        ));
        cfg.TextFilterTextBox = Ext.create('Ext.form.TextField',
            Ext.merge(
            {
                width: 157,
                padding: '0 0 0 5'
            },
            cfg.textFilterTextboxControlConfig
        ));
        cfg.TextFilterContainert = Ext.create('Ext.container.Container', {
            xtype: 'panel',
            layout: { type: 'hbox'},
            items: [cfg.TextFilterDropDownControl, cfg.TextFilterTextBox]
        });
        me.margin = 0;
        me.items = [
                cfg.TextFilterContainert
            ];
        me.callParent(arguments);
    },
    setValue: function(value) {
        var me = this, cfg = me.config;
        cfg.TextFilterDropDownControl.setValue(value);
    },
    resetSearchCriteria: function() {
        var me = this, cfg = me.config;
        cfg.TextFilterTextBox.setValue('');
    }
});