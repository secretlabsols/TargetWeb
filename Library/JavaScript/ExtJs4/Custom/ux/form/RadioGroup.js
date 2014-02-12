Ext.define('Target.ux.form.RadioGroup', {
    extend: 'Ext.form.RadioGroup',
    constructor: function(config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    setValue: function(value) {
        if (!Ext.isObject(value)) {
            var obj = new Object();
            obj[this.name] = value;
            value = obj;
        }
        Ext.form.RadioGroup.prototype.setValue.call(this, value);
    }
});