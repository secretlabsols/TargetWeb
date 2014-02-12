var resultsPanel, resultsSettings;

$(function () {

    var resultsControl = 'Results.Controls.ContractNumberMappingResults';

    Ext.require(resultsControl)

    Ext.onReady(function () {

        Ext.suspendLayouts();

        var resultsPanel = Ext.create(resultsControl, {
            resultSettings: resultsSettings
        });

        Ext.resumeLayouts(true);

    });

});