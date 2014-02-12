Ext.define('Target.ux.form.field.DateFilter', {
    extend: 'Ext.form.FieldContainer',
    requires: ['Target.ux.form.field.ClearButton',
                'Target.ux.form.field.DateTime',
                'Target.ux.form.field.Time'
              ],
    anchor: '100%',
    fieldLabel: '',
    margin: 0,
    padding: '0 0 0 0',
    labelWidth: 95,
    layout: 'fit',
    config: {
        filterOptionStore: null
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config, isReadOnly = ((cfg.filterOptionStoreData || []).length <= 1);
        cfg.filterOptionStoreData = (cfg.filterOptionStoreData || []);
        if (cfg.filterOptionStoreData.length == 0) {
            cfg.filterOptionStoreData = [
                { value: 1, description: 'Between' },
                { value: 2, description: 'Equals' }
            ]
        }
        cfg.filterOptionStore = Ext.create('Ext.data.Store', {
            fields: ['value', 'description'],
            data: cfg.filterOptionStoreData
        });
        cfg.dateFilterDropDownControl = Ext.create('Ext.form.field.ComboBox',
            Ext.merge(
            {
                store: cfg.filterOptionStore,
                queryMode: 'local',
                displayField: 'description',
                emptyText: 'Select criteria...',
                valueField: 'value',
                editable: false,
                padding: '0 0 0 0',
                width: 150,
                listeners: {
                    select: function () {
                        me.setControlVisibility();
                    },
                    change: function () {
                        me.setControlVisibility();
                    }
                }
            },
            cfg.dateFilterDropDownControlConfig
        ));
            cfg.dateFrom = Ext.create('Target.ux.form.field.DateTime',
            Ext.merge({
                blankTime: '23:59',
                dateControlConfig: {
                    name: 'RangeDatefrom',
                    fieldLabel: ' ',
                    labelSeparator:'',
                    labelWidth: 0,
                    padding: '5 0 0 100',
                    width: 127
                },
                timeControlConfig: {
                    name: 'RangeTimeTo',
                    value: '23:59',
                    padding: '0 0 0 0'
                },
                fieldLabel: '',
                labelWidth: 0
            },
            cfg.dateFromControlConfig
        ));
        cfg.dateFrom.addListener('dateChange', function (fld, newVal) {
            cfg.dateToControl.setMinDateValue(newVal);
            me.fireEvent('change');
        }, me);
        cfg.dateToControl = Ext.create('Target.ux.form.field.DateTime',
            Ext.merge({
                blankTime: '23:59',
                dateControlConfig: {
                    name: 'RangeDateTo',
                    fieldLabel: 'and',
                    labelWidth: 23,
                    padding: '5 0 0 5',
                    width: 150
                },
                timeControlConfig: {
                    name: 'RangeTimeTo',
                    value: '23:59',
                    padding: '0 0 0 0'
                },
                fieldLabel: '',
                labelWidth: 0
            },
            cfg.dateToControlConfig
        ));
        cfg.dateToControl.addListener('dateChange', function (fld, newVal) {
            cfg.dateFrom.setMaxDateValue(newVal);
            me.fireEvent('change');
        }, me);

        cfg.TextFilterContainert = Ext.create('Ext.container.Container', {
            xtype: 'container',
            items: [
                cfg.dateFrom,
                cfg.dateToControl
            ],
            layout: {
                type: 'hbox',
                align: 'stretch'
            },
            padding: '0 0 0 0'
        });
        me.items = [
            cfg.dateFilterDropDownControl,
            cfg.TextFilterContainert
        ];
        me.callParent(arguments);
    },
    setControlVisibility: function () {
        var me = this, cfg = me.config
        me.setVisibility(cfg.dateFilterDropDownControl.getValue());
    },
    setVisibility: function (value) {
        var me = this, cfg = me.config;
        me.showDateTo((value == 1));
    },
    showDateTo: function (value) {
        var me = this, cfg = me.config;
        cfg.dateToControl.setVisible(value);
    },
    setSearchType: function (value) {
        var me = this, cfg = me.config;
        cfg.setSearchType.setValue(value);
    },
    setDateTo: function () {
        var me = this, cfg = me.config;
        cfg.dateToControl.setValue();
    },
    setDateFrom: function () {
        var me = this, cfg = me.config;
        cfg.dateFrom.setValue();
    },
    setValue: function (value) {
        var me = this, cfg = me.config;
        cfg.dateFilterDropDownControl.setValue(value);
    } 
});