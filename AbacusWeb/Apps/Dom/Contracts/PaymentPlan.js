var periodPaymentDetailPanel, pppdResultSettings, domContractPeriodID, periodFrom, periodTo, txtContractValue, userID;
var viewportHeight = document.documentElement.clientHeight;
var actionPanelHeight = 30;
var periodPaymentPanelDetailHeight = 0;
var lblTotal, stdButtonMode, hidTotal, lblError, dteEndDate, paymentConfigPresent, lastPaymentToDate;
var errTotalSum = "Contract Value and Total sum of Period Payment Plan Detail lines must be the same";

$(document).ready(function () {

    $(function () {

        var periodPaymentPlanControl = 'TargetExt.PeriodPaymentPlanDetail.PeriodPaymentPlanDetailControl';
        Ext.require(periodPaymentPlanControl);

        periodPaymentPanelDetailHeight = 500;

        Ext.onReady(function () {

            Ext.suspendLayouts();

            var periodPaymentDetailPanel = Ext.create(periodPaymentPlanControl, {
                renderTo: 'extContent'
            });

            periodPaymentDetailPanel.initComponent();

            Ext.resumeLayouts(true);

        });

        validateTotal();
    });
});

function txtContractValue_Changed()
{
    txtContractValue.value = Number(txtContractValue.value).toFixed(2);
    validateTotal();
}

function validateTotal() {

    if (lblTotal.innerHTML != "" || paymentConfigPresent) {
        if (Number(lblTotal.innerHTML).toFixed(2) != Number(txtContractValue.value).toFixed(2)) {
            lblTotal.className = "errorText";
            lblError.innerHTML = errTotalSum;
        }
        else {
            if (lblError.innerHTML == errTotalSum) {
                lblTotal.className = "";
                lblError.innerHTML = "";
            }
        }
    }
}

//$(function () {

//    var periodPaymentPlanDetailSelectorControl = 'Selectors.PeriodPaymentPlanDetailSelector';
//    var periodPaymentPlanDetailActionPanel = 'Actions.Controls.PeriodPaymentDetailActions'

//    Ext.require(periodPaymentPlanDetailSelectorControl)
//    Ext.require(periodPaymentPlanDetailActionPanel) 

//    Ext.onReady(function () {

//        var pppdSelectorControl = Ext.create(periodPaymentPlanDetailSelectorControl, {
//            request: {
//                PageSize: 0
//            }
//        });

//        var pppdActionPanel = Ext.create(periodPaymentPlanDetailActionPanel, {
//            resultSettings: pppdResultSettings
//        });

//        var selectorHeight = (viewportHeight - $("#HeaderDiv").height() - $("#ButtonsDiv").height() - $("#errorText").height() - $("#pageLoading").height() - actionPanelHeight);

//        var extPanel = Ext.create('Ext.panel.Panel', {
//            layout: 'fit',
//            border: 1,
//            height: selectorHeight,
//            renderTo: 'extContent',
//            bbar: pppdActionPanel
//        });

//        pppdSelectorControl.Load(extPanel);

//    });

//});


