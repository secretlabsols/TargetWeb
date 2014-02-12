Ext.define('Target.ux.grid.filter.AdvancedStringFilter', {
    extend: 'Ext.ux.grid.filter.Filter',
    alias: 'gridfilter.advancedString',
    iconCls: 'ux-gridfilter-text-icon',
    emptyText: 'Enter Filter Text...',
    operators: {
        Contains: 'Contains',
        EndsWith: 'EndsWith',
        Equals: 'Equals',
        StartsWith: 'StartsWith'
    },
    width: 250,
    wildCardChar: '%',
    init: function (config) {
        var me = this;
        Ext.applyIf(config, {
            enableKeyEvents: true,
            iconCls: me.iconCls,
            hideLabel: true,
            listeners: {
                scope: me,
                keyup: me.onInputKeyUp,
                keydown: me.onInputKeyDown,
                el: {
                    click: function (e) {
                        e.stopPropagation();
                    }
                }
            },
            selectOnFocus: true
        });
        me.currentFilterValue = '';
        me.inputItemOperator = Ext.create('Ext.form.ComboBox', {
            name: 'filterOperatorCombo',
            allowBlank: false,
            editable: false,
            value: me.operators.Contains,
            displayField: 'text',
            queryMode: 'local',
            triggerAction: 'all',
            valueField: 'value',
            store: new Ext.data.SimpleStore({
                fields: ['text', 'value'],
                data: [
					['Contains', me.operators.Contains],
					['Ends With', me.operators.EndsWith],
					['Equals', me.operators.Equals],
					['Starts With', me.operators.StartsWith]
				]
            }),
            submitValue: true
        });
        me.inputItem = Ext.create('Ext.form.field.Text', config);
        me.inputForm = Ext.create('Ext.form.Panel', {
            bodyPadding: 5,
            width: 250,
            layout: 'anchor',
            defaults: {
                anchor: '100%'
            },
            defaultType: 'textfield',
            items: [
				me.inputItemOperator,
                me.inputItem,
				{
				    xtype: 'label',
				    anchor: '54%'
				},
				{
				    xtype: 'button',
				    anchor: '23%',
				    text: 'Filter',
				    listeners: {
				        click: {
				            scope: me,
				            fn: function () {
				                me.setValue(me.getValue());
				                me.hideMenus();
				            }
				        }
				    }
				},
				{
				    xtype: 'button',
				    anchor: '23%',
				    margin: '0 0 0 2',
				    text: 'Clear',
				    listeners: {
				        click: {
				            scope: me,
				            fn: function () {
				                me.inputItem.setValue('');
				                if (me.active) {
				                    me.setValue('');
				                    me.setActive(false);
				                }
				                me.hideMenus();
				            }
				        }
				    }
				}
			]
        });
        me.menu.add(me.inputForm);
        me.menu.addListener('show', function (cmp) {
            cmp.setWidth(255);
            me.inputItem.focus(true, true);
        }, me);
        me.menu.canActivateItem = function (item) {
            return false;
        };
        me.updateTask = Ext.create('Ext.util.DelayedTask', me.fireUpdate, me);
    },
    getValue: function () {
        var me = this, returnValue = me.inputItem.getValue().replace(/%/g, '');
        switch (me.inputItemOperator.getValue()) {
            case me.operators.Contains:
                returnValue = me.wildCardChar + returnValue + me.wildCardChar;
                break;
            case me.operators.EndsWith:
                returnValue = me.wildCardChar + returnValue;
                break;
            case me.operators.StartsWith:
                returnValue = returnValue + me.wildCardChar;
                break;
        }
        return $.trim(returnValue);
    },
    setValue: function (value) {
        var me = this, valueOperator = me.operators.Contains;
        if (value && value != '') {
            value = $.trim(value);
            var startMatch = ((value.lastIndexOf(me.wildCardChar) + 1) == value.length);
            var endMatch = (value.indexOf(me.wildCardChar) == 0);
            if (endMatch && startMatch) {
                valueOperator = me.operators.Contains;
            } else if (startMatch) {
                valueOperator = me.operators.StartsWith;
            } else if (endMatch) {
                valueOperator = me.operators.EndsWith;
            } else {
                valueOperator = me.operators.Equals;
            }
            value = value.replace(/%/g, '');
        } else {
            value = '';
        }
        me.inputItemOperator.setValue(valueOperator);
        me.inputItem.setValue(value);
        if (value != '') {
            me.setActive(true);
        }
        me.fireEvent('update', me);
    },
    isActivatable: function () {
        return this.inputItem.getValue().length > 0;
    },
    getSerialArgs: function () {
        return { type: 'string', value: this.getValue() };
    },
    validateRecord: function (record) {
        var me = this, val = record.get(me.dataIndex);
        if (typeof val != 'string') {
            return (me.getValue().length === 0);
        }
        return val.toLowerCase().indexOf(me.getValue().toLowerCase()) > -1;
    },
    onInputKeyUp: function (field, e) {
        var me = this, key = e.getKey();
        if (field.isValid()) {
            if (key == e.RETURN && field.isValid()) {
                me.hideMenus();
                me.fireUpdate();
            }
            if (me.updateBuffer > 0) {
                me.updateTask.delay(me.updateBuffer);
            }
        }
    },
    onInputKeyDown: function (field, e) {
        var input, selStart, selEnd, value;

        if (e.getCharCode() === e.SPACE) {
            input = field.getEl().query('input')[0];
            selStart = input.value.length;
            selEnd = input.value.length;
            value = field.getValue();
            value = value.substring(0, selStart) + ' ' + value.substring(selEnd, value.length);
            field.setValue(value);
        }
    },
    hideMenus: function () {
        var me = this;
        me.menu.hide();
        me.menu.parentMenu.hide();
    }
});

