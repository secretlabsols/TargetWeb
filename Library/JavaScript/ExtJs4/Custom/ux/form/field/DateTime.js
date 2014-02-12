Ext.define('Target.ux.form.field.DateTime', {
	extend: 'Ext.form.FieldContainer',
	requires: ['Target.ux.form.field.ClearButton'],
	padding: '0 0 5 0',
	layout: 'hbox',
	config: {
		blankTime: null,
		periodOptionStore: null,
		dateControl: null,
		timeControl: null,
		maxDate: null,
		minDate: null,
		showTime: false
	},
	constructor: function (config) {
		this.initConfig(config);
		this.callParent([config]);
		return this;
	},
	initComponent: function () {
		var me = this, cfg = me.config, controls = [];
		cfg.dateControl = Ext.create('Ext.form.field.Date',
			Ext.merge(
				{
					validateOnChange: true,
					checkChangeBuffer: 500,
					maxLength: 10,
					enforceMaxLength: true,
					maskRe: /[0-9\/]/,
					editable: true,
					fieldLabel: 'fieldLabel',
					format: 'd/m/Y',
					altFormats: "d/m/y|d/m|j/n|j/n/Y|j/n/y",
					labelWidth: 100,
					name: 'ts_date',
					plugins: ['clearbutton'],
					width: 150,
					padding: '0 5 5 0'
				},
				cfg.dateControlConfig
			)
		);
		cfg.dateControl.on('change', function (fld, value) {
			if (cfg.showTime) {
				cfg.timeControl.setDisabled(false);
			}
			if (!Ext.isDate(me.getDate())) {
			   me.setTime(null);
			}
			me.fireEvent('dateChange', me, value);
		}, me);
		controls.push(cfg.dateControl);
		if (cfg.showTime) {
			cfg.timeControl = Ext.create('Target.ux.form.field.Time',
				Ext.merge({
					name: 'ts_time',
					disabled: true,
					width: 80
				},
				cfg.timeControlConfig)
			);
			cfg.timeControl.on('change', function (fld, newVal, oldVal) {
				me.fireEvent('dateChange', me, me.getDate());
				me.fireEvent('timeChange', me, newVal);
			}, me);
			controls.push(cfg.timeControl);
		};
		me.items = controls;
		me.callParent(arguments);
	},
	showTimeElement: function (show) {
		var me = this, cfg = me.config;
		if (cfg.showTime) {
			cfg.timeControl.setVisible(show);
			if (show == true) {
				cfg.dateControl.setDisabledDays("");
			}
		}
	},
	setMinDateValue: function (minDate) {
		var me = this, cfg = me.config;
		cfg.minDate = minDate;
		cfg.dateControl.setMinValue(minDate);
	},
	setMaxDateValue: function (maxDate) {
		var me = this, cfg = me.config;
		cfg.maxDate = maxDate;
		cfg.dateControl.setMaxValue(maxDate);
	},
	showOnlyDay: function (day) {
		var me = this, cfg = me.config, days = me.getDisabledDays(day);
		cfg.dateControl.setDisabledDays(days);
		cfg.dateControl.showToday = false;
		me.showTimeElement(false);
	},
	setTime: function (timeValue) {
		var me = this, cfg = me.config;
		if (cfg.showTime) {
			cfg.timeControl.setValue(timeValue, true);
		}
	},
	setDate: function (dateValue) {
		var me = this, cfg = me.config, tmpDate = dateValue;
		if (cfg.minDate != null && dateValue != null && dateValue < cfg.minDate) {
			tmpDate = cfg.minDate;
		}
		if (cfg.maxDate != null && dateValue != null && tmpDate > cfg.maxDate) {
			tmpDate = cfg.maxDate;
		}
		if (cfg.showTime) {
			cfg.timeControl.setDisabled(false);
		};
		cfg.dateControl.setValue(tmpDate);
	},
	setMinTime: function (minTime) {
		var me = this, cfg = me.config;
		if (cfg.showTime) {
			cfg.timeControl.setMinTime(minTime);
		}
	},
	setMaxTime: function (maxTime) {
		var me = this, cfg = me.config;
		if (cfg.showTime) {
			cfg.timeControl.setMaxTime(maxTime);
		}
	},
	getMins: function () {
		var me = this, cfg = me.config;
		if (cfg.showTime) {
			return cfg.timeControl.getMins(me.config.timeControl.getValue());
		} else {
			return null;
		}
	},
	getHours: function () {
		var me = this, cfg = me.config;
		if (cfg.showTime) {
			return cfg.timeControl.getHours(me.config.timeControl.getValue());
		} else {
			return null;
		}
	},
	getTime: function () {
		var me = this, cfg = me.config, time;
		if (cfg.showTime) {
			return cfg.timeControl.getValue();
		} else {
			return null;
		}
	},
	getDate: function () {
		var me = this, cfg = me.config;
		if (me.isValid()) {
			return cfg.dateControl.getValue();
		} else {
			return null;
		}
	},
	getDateTime: function () {
		var me = this, cfg = me.config, date, sDate, time;
		if (me.getDate() != null) {
			date = cfg.dateControl.getValue().toDateString();
			if (cfg.showTime) {
				time = cfg.timeControl.getValue();
				if (date && cfg.blankTime && !time) {
					time = cfg.blankTime;
				}
				sDate = new Date(Date.parse(date + ' ' + time));
			} else {
				sDate = new Date(date);
			}
		}
		return sDate;
	},
	getDisabledDays: function (day) {
		var i, days = [];
		for (i = 0; i <= 6; i++) {
			if (i != day) {
				days.push(i);
			}
		}
		return days;
	},
	getModelData: function () {
		var me = this, data = null, val;
		val = me.getValue();
		if (val !== null) {
			data = {};
			data[me.getName()] = val;
		}
		return data;
	},
	getName: function () {
		return this.name;
	},
	getSubmitData: function () {
		return this.getValue();
	},
	getValue: function () {
		return this.getDateTime();
	},
	isFormField: true,
	isDirty: function () {
		return false;
	},
	isValid: function () {
		var me = this, cfg = me.config, dateValid = false, timeValid = true;
		dateValid = cfg.dateControl.isValid();
		if (cfg.showTime) {
			timeValid = cfg.timeControl.isValid();
		}
		return (dateValid && timeValid);
	},
	setValue: function (val) {
		var me = this;
		//if (val != null) {
		me.setDate(val);
		me.setTime(val);
		// }
	},
	setEnabled: function (val) {
		var me = this, cfg = me.config;
		if (val == true) {
			cfg.dateControl.enable(true);
			if (cfg.showTime) {
				cfg.timeControl.enable(true);
			}
		}
		if (val == false) {
			cfg.dateControl.disable(true);
			if (cfg.showTime) {
				cfg.timeControl.disable(true);
			}
		}
	},
	validate: function () {
		var me = this;
		return me.isValid();
	}
});