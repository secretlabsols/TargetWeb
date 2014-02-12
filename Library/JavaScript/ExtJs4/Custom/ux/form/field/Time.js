Ext.define('Target.ux.form.field.Time', {
    extend: 'Ext.form.field.Trigger',
    requires: ['Target.ux.form.field.ClearButton'],
    triggerCls: 'ux-form-time-trigger',
    editable: false,
    emptyTime: '00',
    config: {
        maxTime: null,
        minTime: null
    },
    constructor: function (config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    emptyText: 'Select a time...',
    initComponent: function () {
        var me = this, cfg = me.config;
        me.addListener('render', function (cmp) {
            me.btnCancel = Ext.create('Ext.Button', {
                handler: function () {
                    me.cancel();
                },
                text: 'Cancel',
                tooltip: 'Cancel changes to the time?'
            });
            me.btnClear = Ext.create('Ext.Button', {
                handler: function () {
                    me.setValue(null);
                    me.toggleShow(true);
                },
                hidden: !me.allowBlank,
                text: 'Clear',
                tooltip: 'Clear the time?'
            });
            me.btnNow = Ext.create('Ext.Button', {
                handler: function () {
                    me.setValueToNow();
                    me.toggleShow(false);
                },
                text: 'Now',
                tooltip: 'Set the time to now?'
            });
            me.btnOk = Ext.create('Ext.Button', {
                handler: function () {
                    me.toggleShow(true);
                },
                text: 'OK',
                tooltip: 'Accept changes to the time?'
            });
            me.tbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    me.btnNow,
                    me.btnClear,
                    '-',
                    '->',
                    '-',
                    me.btnOk,
                    me.btnCancel
                ]
            });
            me.sliderHours = me.createSliderField(23, me.getHours(me.getValue()));
            me.sliderMins = me.createSliderField(59, me.getMins(me.getValue()));
            me.numHours = me.createNumberField(me.sliderHours, 23, me.getHours(me.getValue()));
            me.numMins = me.createNumberField(me.sliderMins, 59, me.getMins(me.getValue()));
            me.hoursCon = Ext.create('Ext.form.FieldContainer', {
                anchor: '100%',
                fieldLabel: 'Hours',
                items: [
                    me.sliderHours,
                    me.numHours
                ],
                labelWidth: 50,
                layout: 'hbox'
            });
            me.minsCon = Ext.create('Ext.form.FieldContainer', {
                anchor: '100%',
                fieldLabel: 'Minutes',
                items: [
                    me.sliderMins,
                    me.numMins
                ],
                labelWidth: 50,
                layout: 'hbox'
            });
            me.minLabel = Ext.create('Ext.form.Label', {
                text: '',
                padding: '0 5 0 0'
            });
            me.maxLabel = Ext.create('Ext.form.Label', {
                text: ''
            });
            me.pnl = Ext.create('Ext.form.Panel', {
                bbar: me.tbar,
                bodyPadding: 5,
                hidden: true,
                floating: true,
                listeners: {
                    show: {
                        fn: function (cmp) {
                            me.valueOnLastShow = me.getValue();
                            cmp.alignTo(me.inputEl);
                            if (!cfg.hasBeenShown) {
                                me.mon(Ext.getDoc(), {
                                    mousedown: function (e) {
                                        if (me.pnl && me.pnl.el && !me.pnl.isHidden() && !me.isInMe(e)) {
                                            me.toggleShow(true);
                                        }
                                    },
                                    scope: me
                                });
                                cfg.hasBeenShown = true;
                            }
                            me.numHours.focus(true, 10);
                        },
                        scope: me
                    }
                },
                items: [
                    me.hoursCon,
                    me.minsCon,
                    me.minLabel,
                    me.maxLabel
                ],
                renderTo: Ext.getBody(),
                width: 300
            });
            cmp.inputEl.addListener('click', function () {
                if (me.isDisabled() == false) {
                    me.onTriggerClick();
                }
            }, cmp);
            cmp.inputEl.addListener('dblclick', function () {
                me.setValueToNow();
                me.toggleShow(true);
            }, cmp);
        }, me);
        me.plugins = ['clearbutton'];
        me.callParent(arguments);
    },
    cancel: function () {
        var me = this;
        me.setValue(me.valueOnLastShow);
        me.toggleShow(true);
    },
    createNumberField: function (slider, maxValue, value) {
        var me = this;
        return Ext.create('Ext.form.field.Number', {
            listeners: {
                change: {
                    fn: function (cmp, newVal) {
                        me.handleNumberFieldChange(cmp, slider, newVal);
                    },
                    scope: me
                },
                keydown: {
                    fn: function (cmp, e) {
                        var pressedKey = e.getKey();
                        if (pressedKey == e.ENTER) {
                            me.toggleShow(true);
                        } else if (pressedKey == e.ESC) {
                            me.cancel();
                        }
                    },
                    scope: me
                }
            },
            enableKeyEvents: true,
            maxValue: maxValue,
            minValue: 0,
            value: value,
            width: 45
        });
    },
    createSliderField: function (maxValue, value) {
        var me = this;
        return Ext.create('Ext.slider.Single', {
            flex: 1,
            increment: 1,
            listeners: {
                change: {
                    fn: function () {
                        me.syncSlidersToInput();
                    },
                    scope: me
                }
            },
            margin: '0 5 0 5',
            maxValue: maxValue,
            minValue: 0,
            tipText: function (thumb) {
                return me.getTimeString(thumb.value);
            },
            value: value
        });
    },
    getHours: function (val) {
        var me = this, hrs = me.emptyTime;
        if (Ext.isNumber(val)) {
            val = val.toString();
        }
        if (val) {
            var colonIdx = val.indexOf(':');
            hrs = val.substring(0, colonIdx);
            hrs = me.getTimeString(hrs);
        }
        return hrs;
    },
    getMins: function (val) {
        var me = this, mins = me.emptyTime;
        if (Ext.isNumber(val)) {
            val = val.toString();
        }
        if (val) {
            var colonIdx = val.indexOf(':');
            mins = val.substring(colonIdx + 1);
            mins = me.getTimeString(mins);
        }
        return mins;
    },
    getTimeString: function (val) {
        var tVal = parseInt(val, 10);
        if (!isNaN(tVal)) {
            return ((tVal > 9) ? tVal.toString() : '0' + tVal.toString())
        } else {
            return me.emptyTime;
        }
    },
    getValueAsDate: function () {
        var me = this, curVal = me.getValue(), valAsDate = new Date();
        if (curVal) {
            valAsDate.setHours(me.getHours(curVal), me.getMins(curVal), 0, 0);
        } else {
            valAsDate.setHours(0, 0, 0, 0);
        }
        return valAsDate;
    },
    handleNumberFieldChange: function (cmp, slider, newVal) {
        if (newVal > cmp.maxValue) {
            cmp.suspendEvents(false);
            cmp.setValue(cmp.maxValue);
            cmp.resumeEvents();
            newVal = cmp.maxValue;
        }
        slider.setValue(newVal);
    },
    isInMe: function (e) {
        var me = this;
        if (!me.isDestroyed && !e.within(me.bodyEl, false, true) && !e.within(me.pnl.el, false, true)) {
            return false;
        } else {
            return true;
        }
    },
    onTriggerClick: function () {
        var me = this;
        me.toggleShow();
    },
    syncSlidersToInput: function () {
        var me = this, hoursVal = me.sliderHours.getValue(), hoursValStr = me.getTimeString(hoursVal), minsVal = me.sliderMins.getValue(), minsValStr = me.getTimeString(minsVal);
        me.setValueWithoutEvents((hoursValStr + ':' + minsValStr), false);
    },
    setValueWithoutEvents: function (value, setSliders) {
        var me = this, cfg = me.config;
        me.suspendEvents(false);
        me.setValue(value, setSliders);
        me.resumeEvents();
    },
    setValue: function (value, setSliders) {
        var me = this, cfg = me.config;
        value = arguments[0];
        if (value) {
            if (Ext.isDate(value)) {
                value = me.getTimeString(value.getHours()) + ':' + me.getTimeString(value.getMinutes());
            }
            value = me.getHours(value) + ':' + me.getMins(value);
        }
        if (cfg.maxTime != null && value > me.config.maxTime) {
            value = cfg.maxTime;
            setSliders = true;
        }
        if (cfg.minTime != null && value < cfg.minTime) {
            value = cfg.minTime;
            setSliders = true;
        }
        if (me.inputEl) {
            if (setSliders !== false) {
                me.sliderHours.suspendEvents(false);
                me.sliderMins.suspendEvents(false);
                me.sliderHours.setValue(me.getHours(value));
                me.sliderMins.setValue(me.getMins(value));
                me.sliderHours.resumeEvents();
                me.sliderMins.resumeEvents();
            }
            me.numHours.suspendEvents(false);
            me.numMins.suspendEvents(false);
            me.numHours.setValue(me.getHours(value));
            me.numMins.setValue(me.getMins(value));
            me.numHours.resumeEvents();
            me.numMins.resumeEvents();
        }
        return me.callParent(arguments);
    },
    setValueToNow: function () {
        var me = this, now = new Date();
        me.setValue(now);
    },
    toggleShow: function (hide) {
        var me = this, cfg = me.config, shouldHide = !me.pnl.hidden;
        if (hide) {
            shouldHide = hide;
        }
        if (me.isDisabled()) {
            shouldHide = true;
        }
        if (shouldHide) {
            me.pnl.hide();
            me.fireEvent('change', me, me.getValue());
        } else {
            if (cfg.minTime != null) {
                me.minLabel.setVisible(true);
                me.minLabel.setText('Minimum Time: ' + cfg.minTime);
            } else {
                me.minLabel.setVisible(false);
            }
            if (cfg.maxTime != null) {
                me.maxLabel.setVisible(true);
                me.maxLabel.setText('Maximum Time: ' + cfg.maxTime);
            } else {
                me.maxLabel.setVisible(false);
            }
            me.pnl.show();
        }
        me.isPickerHidden = shouldHide;
    },
    setMinTime: function (value) {
        var me = this, cfg = me.config;
        cfg.minTime = value;
    },
    setMaxTime: function (value) {
        var me = this, cfg = me.config;
        cfg.maxTime = value;
    }
});