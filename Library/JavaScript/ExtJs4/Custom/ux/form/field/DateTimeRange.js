Ext.define('Target.ux.form.field.DateTimeRange', {
    extend: 'Ext.form.FieldContainer',
    requires: ['Target.ux.form.field.DateTime', 'Target.ux.form.field.Time'],
    anchor: '100%',
    fieldLabel: 'Date Period',
    labelWidth: 100,
    layout: 'fit',
    config: {
        rangeOptionStore: null,
        dateRangeDropDownControl: null,
        dateRangeFromControl: null,
        dateRangeToControl: null,
        showTime: false,
        daysRestricted: false
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config, isReadOnly = ((cfg.rangeOptionStoreData || []).length <= 1);
        cfg.rangeOptionStoreData = (cfg.rangeOptionStoreData || []);
        if (cfg.rangeOptionStoreData.length == 0) {
            cfg.rangeOptionStoreData = [{ value: 0, description: 'Covering part or all of the period'}]
        }
        cfg.rangeOptionStore = Ext.create('Ext.data.Store', {
            fields: ['value', 'description'],
            data: cfg.rangeOptionStoreData
        });
        cfg.dateRangeDropDownControl = Ext.create('Ext.form.ComboBox',
            Ext.merge(
            {
                anchor: '100%',
                store: cfg.rangeOptionStore,
                queryMode: 'local',
                displayField: 'description',
                emptyText: 'Select an item...',
                valueField: 'value',
                editable: false,
                padding: '0 0 5 0',
                listeners: {
                    render: {
                        fn: function (cmp) {
                            //if there is only one item in the combo, select it.
                            if (cfg.rangeOptionStore.count() == 1) {
                                cmp.select(cmp.getStore().data.items[0]);
                            }
                        }
                    }
                },
                readOnly: isReadOnly,
                fieldCls: (isReadOnly ? 'x-form-field-readonly-aslabel' : 'x-form-field'),
                readOnlyCls: 'x-form-readonly-aslabel'
            },
            cfg.dateRangeDropDownControlConfig
        ));
        cfg.dateRangeDropDownControl.addListener('change', function (fld, newVal, oldValue) {
            me.fireEvent('change', fld, newVal, oldValue);
        }, me);
        cfg.dateRangeDateTimeFromControl = Ext.create('Target.ux.form.field.DateTime',
            Ext.merge({
                blankTime: '00:00',
                showTime: cfg.showTime,
                dateControlConfig: {
                    name: 'RangeDateFrom',
                    fieldLabel: 'From',
                    labelWidth: 30
                },
                timeControlConfig: {
                    name: 'RangeTimeFrom',
                    value: '00:00'
                },
                fieldLabel: ''
            },
            cfg.dateRangeDateTimeFromControlConfig)
         );
        cfg.dateRangeDateTimeFromControl.addListener('dateChange', function (fld, newVal) {
            cfg.dateRangeDateTimeToControl.setMinDateValue(newVal);
            me.setMinAndMaxTime();
        }, me);
        cfg.dateRangeDateTimeFromControl.addListener('timeChange', function (fld, newVal) {
            me.setMinAndMaxTime();
        }, me);
        cfg.dateRangeDateTimeToControl = Ext.create('Target.ux.form.field.DateTime',
            Ext.merge({
                blankTime: '23:59',
                showTime: cfg.showTime,
                dateControlConfig: {
                    name: 'RangeDateTo',
                    fieldLabel: 'To',
                    labelWidth: 30,
                    padding: '0 5 0 0'
                },
                timeControlConfig: {
                    name: 'RangeTimeTo',
                    value: '23:59',
                    padding: '0 5 0 0'
                },
                fieldLabel: '',
                labelWidth: 0
            },
            cfg.dateRangeDateTimeToControlConfig)
         );
        cfg.dateRangeDateTimeToControl.addListener('dateChange', function (fld, newVal) {
            cfg.dateRangeDateTimeFromControl.setMaxDateValue(newVal);
            me.setMinAndMaxTime();
        }, me);
        cfg.dateRangeDateTimeToControl.addListener('timeChange', function (fld, newVal) {
            me.setMinAndMaxTime();
        }, me);
        me.items = [
            cfg.dateRangeDropDownControl,
            cfg.dateRangeDateTimeFromControl,
            cfg.dateRangeDateTimeToControl
        ];
        me.callParent(arguments);
    },
    getDateFrom: function () {
        var me = this, cfg = me.config;
        return cfg.dateRangeDateTimeFromControl.getDate();
    },
    getDateTo: function () {
        var me = this, cfg = me.config;
        return cfg.dateRangeDateTimeToControl.getDate();
    },
    getTimeFrom: function () {
        var me = this, cfg = me.config;
        return cfg.dateRangeDateTimeFromControl.getTime();
    },
    getTimeTo: function () {
        var me = this, cfg = me.config;
        return cfg.dateRangeDateTimeToControl.getTime();
    },
    getDateTimeFrom: function () {
        var me = this, cfg = me.config;
        return cfg.dateRangeDateTimeFromControl.getDateTime();
    },
    getDateTimeTo: function () {
        var me = this, cfg = me.config;
        return cfg.dateRangeDateTimeToControl.getDateTime();
    },
    getDateRangeOption: function () {
        var me = this, cfg = me.config;
        return cfg.dateRangeDropDownControl.getValue();
    },
    restrictSelectableDays: function (weekCommencingDay, weekEndingDay) {
        var me = this, cfg = me.config;
        cfg.dateRangeDateTimeToControl.showOnlyDay(weekEndingDay);
        cfg.dateRangeDateTimeFromControl.showOnlyDay(weekCommencingDay);
        me.config.daysRestricted = true;
    },
    restrictSelectionToWeekRange: function (weekEndingDay) {
        var me = this, weekCommencingDay;
        if (weekEndingDay === 6) {
            weekCommencingDay = 0
        } else {
            weekCommencingDay = weekEndingDay + 1
        }
        me.restrictSelectableDays(weekCommencingDay, weekEndingDay);
    },
    setMinTime: function () {
        var me = this, cfg = me.config, dateFrom = '', dateTo = '', timeFrom = me.getTimeFrom();
        if (me.getDateFrom() != null) {
            dateFrom = me.getDateFrom().toString();
        }
        if (me.getDateTo() != null) {
            dateTo = me.getDateTo().toString();
        }
        if (dateFrom === dateTo && timeFrom != '') {
            cfg.dateRangeDateTimeToControl.setMinTime(timeFrom);
        } else {
            cfg.dateRangeDateTimeToControl.setMinTime(null);
        }
        cfg.dateRangeDateTimeToControl.setTime(me.getTimeTo());
    },
    setMaxTime: function () {
        var me = this, cfg = me.config, dateFrom = '', dateTo = '', timeTo = me.getTimeTo();
        if (me.getDateFrom() != null) {
            dateFrom = me.getDateFrom().toString();
        }
        if (me.getDateTo() != null) {
            dateTo = me.getDateTo().toString();
        }
        if (dateFrom === dateTo && timeTo != '') {
            cfg.dateRangeDateTimeFromControl.setMaxTime(timeTo);
        } else {
            cfg.dateRangeDateTimeFromControl.setMaxTime(null);
        }
        cfg.dateRangeDateTimeFromControl.setTime(me.getTimeFrom());
    },
    setMinDate: function (value) {
        var me = this, cfg = me.config;
        cfg.dateRangeDateTimeFromControl.setMinDateValue(value);
        cfg.dateRangeDateTimeToControl.setMinDateValue(value);
    },
    setMinAndMaxTime: function () {
        var me = this;
        me.setMinTime();
        me.setMaxTime();
        me.fireEvent('change');
    },
    setMaxDate: function (value) {
        var me = this, cfg = me.config;
        cfg.dateRangeDateTimeFromControl.setMaxDateValue(value);
        cfg.dateRangeDateTimeToControl.setMaxDateValue(value);
    },
    setDateFrom: function (value) {
        var me = this, cfg = me.config;
        cfg.dateRangeDateTimeFromControl.setDate(value);
        cfg.dateRangeDateTimeToControl.setMinDateValue(me.getDateFrom());
        me.setMinAndMaxTime();
    },
    setDateTo: function (value) {
        var me = this, cfg = me.config;
        cfg.dateRangeDateTimeToControl.setDate(value);
        cfg.dateRangeDateTimeFromControl.setMaxDateValue(me.getDateTo());
        me.setMinAndMaxTime();
    },
    setTimeFrom: function (value) {
        var me = this, cfg = me.config;
        cfg.dateRangeDateTimeFromControl.setTime(value);
    },
    setTimeTo: function (value) {
        var me = this, cfg = me.config;
        cfg.dateRangeDateTimeToControl.setTime(value);
    },
    showTime: function (show) {
        var me = this, cfg = me.config;
        if (show === true && me.config.daysRestricted === true) {
            alert('Incorrect Configuration, Time control can not be shown when restricting days.');
        } else {
            cfg.dateRangeDateTimeFromControl.showTimeElement(show);
            cfg.dateRangeDateTimeToControl.showTimeElement(show);
        }
    },
    showDateFrom: function (show) {
        var me = this, cfg = me.config;
        if (show) {
            cfg.dateRangeDateTimeFromControl.show();
        } else {
            cfg.dateRangeDateTimeFromControl.hide();
        }
    },
    setDatetypeValue: function (value) {
        var me = this, cfg = me.config;
        cfg.dateRangeDropDownControl.setValue(value);
    }
});