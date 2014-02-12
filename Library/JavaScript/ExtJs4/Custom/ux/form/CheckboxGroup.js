Ext.define('Target.ux.form.CheckboxGroup', {
    extend: 'Ext.form.CheckboxGroup',
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
        Ext.form.CheckboxGroup.prototype.setValue.call(this, value);
    }
});