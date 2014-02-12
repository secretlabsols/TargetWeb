Ext.define('Reports.Params.ServiceOrderDetailReportParams', {
    extend: 'Reports.ReportParameterControl',
    initComponent: function () {
        var me = this, cfg = me.config; 

        me.setupFrm();
        me.items = [
            cfg.frm
        ];
        // call parent init
        me.callParent(arguments);
    },
    getParameters: function () {
        var me = this, cfg = me.config, frm = cfg.frm.getForm(), rcd = frm.getRecord();
        //frm.updateRecord(rcd);
        var filterdate = cfg.postingDateControl.getDate();
        var filterValue;
        if (filterdate == null) {
            filterValue = null
        } else {
            filterValue = Date.strftime("%d/%b/%Y", filterdate)
        }

        return {
            success: true,
            params: [
                {
                    key: 'dtePlanInForceOn',
                    value: filterValue
                }
            ]
        };
    },
    setupFrm: function () {
        var me = this, cfg = me.config, response;
        if (!cfg.frm) {

            var frmData = me.getData();

            cfg.postingDateControl = Ext.create('Target.ux.form.field.DateTime', {
                dateControlConfig: {
                    name: 'PostingDateFrom',
                    fieldLabel: 'Show service plan in force on ',
                    labelWidth: 220,
                    width: 350,
                    disabledDays: frmData.DisableDays
                },
                fieldLabel: '',
                labelWidth: 0
            });
            cfg.postingDateControl.setDate(frmData.CurrentWeekending);

            // create form containing controls
            cfg.frm = Ext.create('Ext.form.Panel', {
                border: 0,
                items: [cfg.postingDateControl]
            });
        }
    }
});
