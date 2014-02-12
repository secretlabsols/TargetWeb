
//$(function () {

//    var financeCodeMatrixSelectorControl = 'Selectors.FinanceCodeMatrixSelector';

//    Ext.require(financeCodeMatrixSelectorControl)

//    Ext.onReady(function () {

//        var fcmSelectorControl = Ext.create(financeCodeMatrixSelectorControl, {
//            request: {
//                PageSize: 0
//            }
//        });

//        var footerContainer = $('#footerContainer'), bottom = footerContainer.position().top + footerContainer.outerHeight();
//        var selectorHeight = ((document.documentElement.clientHeight - bottom) - 1);

//        var extPanel = Ext.create('Ext.panel.Panel', {
//            layout: 'fit',
//            border: 1,
//            height: selectorHeight,
//            renderTo: 'extContent'
//        });

//        fcmSelectorControl.Load(extPanel);
//   
//    });
//});

var fcmResultsPanel, fcmResultSettings;

$(function () {

    var financeCodeMatrixResultsControl = 'Results.Controls.FinanceCodeMatrixResults';
    Ext.require(financeCodeMatrixResultsControl)

    Ext.onReady(function () {

        Ext.suspendLayouts();

        var fcmResultsPanel = Ext.create(financeCodeMatrixResultsControl, {
            resultSettings: fcmResultSettings
        });

        Ext.resumeLayouts(true);

    });

});

