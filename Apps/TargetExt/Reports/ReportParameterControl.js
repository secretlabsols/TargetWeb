Ext.define('Reports.ReportParameterControl', {
    extend: 'Ext.panel.Panel',
    bodyPadding: 5,
    border: 1,
    width: 500,
    constructor: function (config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    getParameters: function () {
        alert('getParameters not implemented.');
        return null;
    },
    getData: function () {
        var me = this, cfg = me.config, response;
        cfg.service = Target.Abacus.Library.Reports.Services.ReportsService;
        response = cfg.service.FetchAdditionalParameterData(me.report.ID);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return null;
        }
        return response.value.Item;
    }
});
