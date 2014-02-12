Ext.define('Target.ux.form.field.NumberRange', {
    extend: 'Ext.form.FieldContainer',
    requires: ['Target.ux.form.field.Number'],
    anchor: '100%',
    fieldLabel: 'Number Range',
    labelWidth: 100,
    layout: 'hbox',
    config: {
        maxValue: null,
        minValue: null,
        numberAppendChars: null,
        numberFromControl: null,
        numberPrependChars: null,
        numberStep: 1,
        numberToControl: null,
        numberType: 'decimal',
        numberTypePrecision: 2
    },
    constructor: function(config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        // create a default cfg for ctrls
        var defaultNumberConfig = {
            allowBlank: false,
            allowDecimals: (cfg.numberType === 'decimal'),
            appendChars: cfg.numberAppendChars,
            checkChangeBuffer: 750,
            decimalPrecision: cfg.numberTypePrecision,
            emptyText: 'Open...',
            labelWidth: 30,
            maxValue: cfg.maxValue,
            minValue: cfg.minValue,
            numberType: cfg.numberType,
            numberTypePrecision: cfg.numberType,
            padding: '0 5 5 0',
            prependChars: cfg.numberPrependChars,
            step: cfg.numberStep,
            width: 150
        };
        // setup from ctrl
        var defaultNumberFromConfig = Ext.merge(Ext.merge(Ext.clone(defaultNumberConfig), {
            fieldLabel: 'From',
            value: cfg.minValue
        }), cfg.numberFromControlConfig);
        if (!defaultNumberFromConfig.allowBlank && (!cfg.minValue && cfg.minValue !== 0)) {
            cfg.minValue = 0;
            defaultNumberFromConfig.minValue = cfg.minValue;
            defaultNumberFromConfig.value = cfg.minValue;
        }
        cfg.numberFromControl = Ext.create('Target.ux.form.field.Number', defaultNumberFromConfig);
        cfg.numberFromControl.addListener('change', function(fld, newVal, oldValue) {
            me.handleNumberChanged(true);
        }, me);
        // setup to ctrl
        var defaultNumberToConfig = Ext.merge(Ext.merge(Ext.clone(defaultNumberConfig), {
            fieldLabel: 'To',
            labelWidth: 25,
            value: cfg.maxValue
        }), cfg.numberToControlConfig);
        if (!defaultNumberToConfig.allowBlank && (!cfg.maxValue && cfg.maxValue !== 0)) {
            cfg.maxValue = 99999999;
            defaultNumberToConfig.maxValue = cfg.maxValue;
            defaultNumberToConfig.value = cfg.maxValue;
        }
        cfg.numberToControl = Ext.create('Target.ux.form.field.Number', defaultNumberToConfig);
        cfg.numberToControl.addListener('change', function(fld, newVal, oldValue) {
            me.handleNumberChanged(false);
        }, me);
        // add items
        me.items = [
            cfg.numberFromControl,
            cfg.numberToControl
        ];
        // init the parent
        me.callParent(arguments);
    },
    getNumberFromValue: function() {
        var me = this, cfg = me.config, value = cfg.numberFromControl.getValue();
        return ((!Ext.isNumber(value) && !cfg.numberFromControl.allowBlank) ? cfg.minValue : value);
    },
    getNumberToValue: function() {
        var me = this, cfg = me.config, value = cfg.numberToControl.getValue();
        var t = ((!Ext.isNumber(value) && !cfg.numberToControl.allowBlank) ? cfg.maxValue : value);
        return ((!Ext.isNumber(value) && !cfg.numberToControl.allowBlank) ? cfg.maxValue : value);
    },
    getValue: function() {
        var me = this, cfg = me.config, item = me.getValueItem(null);
        item.from = me.getNumberFromValue();
        item.to = me.getNumberToValue();
        return item;
    },
    getValueItem: function(item) {
        return Ext.merge({
            from: null,
            to: null
        }, item);
    },
    handleNumberChanged: function(isFrom) {
        var me = this, cfg = me.config, numberFromVal = me.getNumberFromValue(), numberToVal = me.getNumberToValue(),
        numberFromValMax, numberFromValMin, numberToValMax, numberToValMin;
        if ((cfg.numberFromControl.allowBlank && Ext.isNumber(numberFromVal)) || (!cfg.numberFromControl.allowBlank)) {
            numberFromValMax = ((numberToVal < cfg.maxValue) ? numberToVal : cfg.maxValue);
        }
        if ((cfg.numberFromControl.allowBlank && Ext.isNumber(numberFromVal)) || (!cfg.numberFromControl.allowBlank)) {
            numberFromValMin = cfg.minValue;
        }
        if ((cfg.numberToControl.allowBlank && Ext.isNumber(numberToVal)) || (!cfg.numberToControl.allowBlank)) {
            numberToValMax = cfg.maxValue;
        }
        if ((cfg.numberToControl.allowBlank && Ext.isNumber(numberToVal)) || (!cfg.numberToControl.allowBlank)) {
            numberToValMin = ((numberFromVal > cfg.minValue) ? numberFromVal : cfg.minValue);
        }
        cfg.numberFromControl.setMaxValue(numberFromValMax);
        cfg.numberFromControl.setMinValue(numberFromValMin);
        cfg.numberToControl.setMaxValue(numberToValMax);
        cfg.numberToControl.setMinValue(numberToValMin);
        if (isFrom) {
            cfg.numberFromControl.ensureValuesInBounds(true);
        } else {
            cfg.numberToControl.ensureValuesInBounds(false);
        }
    },
    setNumberFromValue: function(val) {
        var me = this, cfg = me.config;
        cfg.numberFromControl.setValue(val);
    },
    setNumberToValue: function(val) {
        var me = this, cfg = me.config;
        cfg.numberToControl.setValue(val);
    },
    setValue: function(val) {
        var me = this, cfg = me.config, item = me.getValueItem(val);
        me.setNumberToValue(item.to);
        me.setNumberFromValue(item.from);
    },
    setValueOnControlNoEvents: function(fld, val) {
        var me = this, cfg = me.config;
        fld.suspendEvents(false);
        fld.setValue(val);
        fld.resumeEvents();
    }
});