Ext.define('Target.ux.form.field.Number', {
    extend: 'Ext.form.field.Number',
    requires: ['Target.ux.form.field.ClearButton'],
    appendChars: null,
    prependChars: null,
    useThousandSeparator: true,
    thousandSeparator: ',',
    selectOnFocus: false,
    constructor: function(config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        me.plugins = ['clearbutton'];
        me.addListener('blur', function() {
            me.setValue(me.removeFormat(me.getValue()));
        }, me);
        me.addListener('focus', function() {
            me.setRawValue(me.removeFormat(me.getRawValue()));
            if (me.selectOnFocus) {
                me.selectText();
            }
        }, me);
        // init the parent
        me.callParent(arguments);
    },
    ensureValuesInBounds: function(useMin) {
        var me = this, v = me.getValue();
        if ((!me.allowBlank && v < me.minValue) || (me.allowBlank && Ext.isNumber(me.minValue) && Ext.isNumber(v) && v < me.minValue)) {
            me.setValue((useMin ? me.minValue : me.maxValue));
            return false;
        }
        if ((!me.allowBlank && v > me.maxValue) || (me.allowBlank && Ext.isNumber(me.maxValue) && Ext.isNumber(v) && v > me.maxValue)) {
            me.setValue((useMin ? me.minValue : me.maxValue));
            return false;
        }
        return true;
    },
    getErrors: function(v) {
        var me = this;
        return me.callParent([me.removeFormat(v)]);
    },
    getFormattedValue: function(v) {
        var me = this;
        if (Ext.isEmpty(v) || !me.hasFormat()) {
            return v;
        }
        else {
            var numberFormat = (me.useThousandSeparator ? '0,000' : '0');
            if (me.useThousandSeparator) {
                if (Ext.isEmpty(me.thousandSeparator)) {
                    throw ('NumberFormatException: invalid thousandSeparator, property must has a valid character.');
                }
                if (me.thousandSeparator == me.decimalSeparator) {
                    throw ('NumberFormatException: invalid thousandSeparator, thousand separator must be different from decimalSeparator.');
                }
            }
            if (me.allowDecimals && me.decimalPrecision > 0) {
                v = Ext.util.Format.number(v, numberFormat + '.' + Ext.String.leftPad('', me.decimalPrecision, '0'));
            } else {
                v = Ext.util.Format.number(v, numberFormat);
            }
            return String.format('{0}{1}{2}', (Ext.isEmpty(me.prependChars) ? '' : me.prependChars), v, (Ext.isEmpty(me.appendChars) ? '' : me.appendChars));
        }
    },
    hasFormat: function() {
        var me = this;
        return me.decimalSeparator != '.' || me.useThousandSeparator == true || !Ext.isEmpty(me.appendChars) || !Ext.isEmpty(me.prependChars) || me.alwaysDisplayDecimals;
    },
    parseValue: function(v) {
        var me = this;
        return me.callParent([me.removeFormat(v)]);
    },
    removeFormat: function(v) {
        var me = this;
        if (Ext.isEmpty(v) || !me.hasFormat()) {
            return v;
        }
        else {
            v = v.toString();
            v = v.replace(me.appendChars, '');
            v = v.replace(me.prependChars, '');
            v = me.useThousandSeparator ? v.replace(new RegExp('[' + me.thousandSeparator + ']', 'g'), '') : v;
            v = me.allowDecimals && me.decimalPrecision > 0 ? v.replace(me.decimalSeparator, '.') : v;
            return v;
        }
    },
    setValue: function(v) {
        var me = this;
        me.callParent([v]);
        me.setRawValue(me.getFormattedValue(v));
    }
});