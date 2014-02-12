//===========================================================================
// Provides a PaymentTolerance Functionality
// Created: 09/09/2011 Motahir D11766 - e-Invoicing Provider Invoice Tolerances
// Prerequisites:
// 1)ensure 'UseJQuery = True' in code behind of page this script is included in
// include "Library/JavaScript/Utils.js" script in code behind
// include "Library/JavaScript/ToleranceCalculator.js" script in code behind
// 2) add following enums in code beind:-
// AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.ToleranceType))
// AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.ToleranceCombinationMethod))
// AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.PaymentToleranceGroupSystemTypes))
// AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomContractedExceeded))
// 3) Call the function ToleranceCalculator.processProviderInvoiceTolerances() 
// ensuring you pass in a collection of detaillines, paymenttolerances, and paymentToleranceGroups.
// The function will return one of the enum values of DomContractedExceeded.No/DomContractedExceeded.Yes_Units/DomContractedExceeded.Yes_Cost
// or -1 if there are no payment tolerances to process
// 4) Ref Target.Abacus.Web.Apps.Dom.ProviderInvoices.Edit.js for an example of how this class is used
//===========================================================================

var paymentTolerance_WeekEndings;
var dictmDomServiceOrderPaymentToleranceAndGroupIDs = new Dictionary();
var dictmContractPeriodPaymentToleranceAndGroupIDs = new Dictionary();
var result;

function processProviderInvoiceTolerances(detailLines, paymentTolerances, paymentToleranceGroups) {
    paymentTolerance_WeekEndings = new Array();
    //iterate through detail lines objects to collect information required for calculating payment tolerances
    $.each(detailLines, function(index, dlItem) {
        $.each(paymentTolerances, function(index, ptItem) {
            if (dlItem.toleranceID == ptItem.PaymentToleranceID && ptItem.PaymentToleranceType == dlItem.toleranceType
					&& FormatHelpers.formatDate(Date.getFormatedJSONDate(ptItem.WeekEnding)) == dlItem.weekEnding) {
                //if we have a matching payment tolerance object update the financial values
                ptItem.ActualUnits += dlItem.totalUnits;
                ptItem.ActualCost += dlItem.totalCost;
                ptItem.PlannedUnits += dlItem.plannedUnits;
                ptItem.PlannedCost += dlItem.plannedCost;
                //check if the weekending exists in the Payment Tolerance Weekending array
                //if weekending does not already exist then add it
                if ($.inArray(FormatHelpers.formatDate(Date.getFormatedJSONDate(ptItem.WeekEnding)), paymentTolerance_WeekEndings) == -1)
                    paymentTolerance_WeekEndings[paymentTolerance_WeekEndings.length] = FormatHelpers.formatDate(Date.getFormatedJSONDate(ptItem.WeekEnding));
                //add toleranceid and tolerance type to the service order payment tolerances dictionary
                if (ptItem.PaymentToleranceType == Target.Abacus.Library.ToleranceType.ServiceOrder) {
                    //check it doesn't already exist in dictionary
                    if (dictmDomServiceOrderPaymentToleranceAndGroupIDs.Lookup(ptItem.PaymentToleranceID) == undefined)
                        dictmDomServiceOrderPaymentToleranceAndGroupIDs.Add(ptItem.PaymentToleranceID, ptItem.ToleranceGroupID);
                }
                //add toleranceid and tolerance type to the contract period payment tolerances dictionary
                if (ptItem.PaymentToleranceType == Target.Abacus.Library.ToleranceType.Contract) {
                    //check it doesn't already exist in dictionary
                    if (dictmContractPeriodPaymentToleranceAndGroupIDs.Lookup(ptItem.PaymentToleranceID) == undefined)
                        dictmContractPeriodPaymentToleranceAndGroupIDs.Add(ptItem.PaymentToleranceID, ptItem.ToleranceGroupID);
                }
            }
        });
    });
    //if we have payment tolerances process them
    if (paymentTolerance_WeekEndings.length > 0) {
        calculateTolerances();
    }
    else {
        return -1;
    }
    //reset the financial values
    resetFinancialValuesOnPaymentToleranceObjects();
    return result;
//    //reset the result variable
//    result = undefined;
}
//reset the planned units, actual units, planned cost, and actual cost to 0
function resetFinancialValuesOnPaymentToleranceObjects() {
    //iterate through payment tolerance objects
    $.each(paymentTolerances, function(index, item) {
        //reset the financial values
        item.ActualUnits = 0;
        item.ActualCost = 0;
        item.PlannedUnits = 0;
        item.PlannedCost = 0;
    });
}
//function used to calculate tolerances
function calculateTolerances() {
    //prime local variable service order tolerances found
    var serviceOrderTolerancesFound = false;
    //prime the result variable
    result = Target.Abacus.Library.DomContractedExceeded.No
    //iterate through the weekendings
    $.each(paymentTolerance_WeekEndings, function(weIndex, weValue) {
        //iterate through payment tolerance groups
        $.each(paymentToleranceGroups, function(ptgIndex, ptgItem) {
            //iterate through dictionary of service order payment tolerances
            $.each(dictmDomServiceOrderPaymentToleranceAndGroupIDs, function(ptID, ptgID) {
                if (ptgItem.ID == ptgID) {
                    $.each(paymentTolerances, function(ptolID, ptItem) {
                        if (ptItem.PaymentToleranceID == ptID && ptItem.PaymentToleranceType ==
                        Target.Abacus.Library.ToleranceType.ServiceOrder &&
                        FormatHelpers.formatDate(Date.getFormatedJSONDate(ptItem.WeekEnding)) == weValue) {
                            processPaymentToleranceGroupLogic(ptItem.SuspendWhenUnitsExceeded, ptID,
                                                      Target.Abacus.Library.ToleranceType.ServiceOrder, weValue,
                                                      ptgItem.SystemType);
                            if (result != Target.Abacus.Library.DomContractedExceeded.No)
                                return false;
                        }
                    });
                }
                if (result != Target.Abacus.Library.DomContractedExceeded.No)
                    return false;
            });
            //iterate through dictionary of contract period payment tolerances only
            //if service order tolerances were not found for the current payment tolerance group
            if (!serviceOrderTolerancesFound) {
                $.each(dictmContractPeriodPaymentToleranceAndGroupIDs, function(ptID, ptgID) {
                    if (ptgItem.ID == ptgID) {
                        $.each(paymentTolerances, function(ptolID, ptItem) {
                            if (ptItem.PaymentToleranceID == ptID && ptItem.PaymentToleranceType ==
                                Target.Abacus.Library.ToleranceType.Contract &&
                                FormatHelpers.formatDate(Date.getFormatedJSONDate(ptItem.WeekEnding)) == weValue) {
                                processPaymentToleranceGroupLogic(ptItem.SuspendWhenUnitsExceeded, ptID,
                                                               Target.Abacus.Library.ToleranceType.Contract, weValue,
                                                               ptgItem.SystemType);
                                if (result != Target.Abacus.Library.DomContractedExceeded.No)
                                    return false;
                            }
                        });
                    }
                    if (result != Target.Abacus.Library.DomContractedExceeded.No)
                        return false;
                });
            }
            if (result != Target.Abacus.Library.DomContractedExceeded.No)
                return false;
        });
        //reset
        if (result != Target.Abacus.Library.DomContractedExceeded.No)
            return false;
        serviceOrderTolerancesFound = false;
    });
    return
}
//process the payment tolerance group
function processPaymentToleranceGroupLogic(suspendWhenUnitsExceeded, ptID, ptType, ptWeekEnding, ptgSystemType) {
    //get the payment tolerance object
    $.each(paymentTolerances, function(ptIndex, ptItem) {
        if (ptItem.PaymentToleranceID == ptID && ptItem.PaymentToleranceType == ptType
        && FormatHelpers.formatDate(Date.getFormatedJSONDate(ptItem.WeekEnding)) == ptWeekEnding) {
            if (ptItem.SuspendWhenUnitsExceeded) {
                result = unitsExceededCalculationLogic(ptItem);
                if (result != Target.Abacus.Library.DomContractedExceeded.No)
                    return false;
            }
            if (!ptItem.SuspendWhenUnitsExceeded) {
                if (ptgSystemType == Target.Abacus.Library.PaymentToleranceGroupSystemTypes.UserEnteredPaymentToleranceGroup) {
                    result = costExceededCalculationLogic(ptItem);
                    if (result != Target.Abacus.Library.DomContractedExceeded.No)
                        return false;
                }
                if (ptgSystemType == Target.Abacus.Library.PaymentToleranceGroupSystemTypes.VisitBasedServiceToleranceGroup) {
                    result = unitsExceededCalculationLogic(ptItem);
                    if (result == Target.Abacus.Library.DomContractedExceeded.Yes_Units &&
                        ptItem.PaymentToleranceCombinationMethod == Target.Abacus.Library.ToleranceCombinationMethod.And)
                        return false;
                    if (result == Target.Abacus.Library.DomContractedExceeded.No &&
                        ptItem.PaymentToleranceCombinationMethod == Target.Abacus.Library.ToleranceCombinationMethod.Or)
                        return false;
                    result = costExceededCalculationLogic(ptItem);
                    if (result != Target.Abacus.Library.DomContractedExceeded.No)
                        return false;
                }
            }
        }
        if (result != Target.Abacus.Library.DomContractedExceeded.No)
            return false;
    });
    return
}
//units exceeded calculation logic
function unitsExceededCalculationLogic(pTol) {
    //suspend Invoice when planned units exceeded option is ticked
    if (pTol.SuspendWhenUnitsExceeded) {
        if (pTol.PlannedUnits < pTol.ActualUnits) {
            return Target.Abacus.Library.DomContractedExceeded.Yes_Units;
        }
    }
    //check acceptable additional units
    if (!((pTol.ActualUnits) <= (pTol.PlannedUnits + pTol.AdditionalHours))) {
        //check acceptable additional units as percentage only if acceptable additional units is exceeded
        if (pTol.ActualUnits <=
        Math.min(pTol.PlannedUnits * ((pTol.AdditionalHoursPercentage / 100) + 1),
        pTol.PlannedUnits + pTol.AdditionalHoursPercentageCap)) {
            //reset the variables if acceptable additional unit percentage is not exceeded
            return Target.Abacus.Library.DomContractedExceeded.No;
        }
        return Target.Abacus.Library.DomContractedExceeded.Yes_Units;
    }
    return Target.Abacus.Library.DomContractedExceeded.No;
}
//cost exceeded calculation logic
function costExceededCalculationLogic(pTol) {
    if (!((pTol.ActualCost) <= (pTol.PlannedCost + pTol.AdditionalPayment))) {
        if (pTol.ActualCost <=
        Math.min(pTol.PlannedCost * ((pTol.AdditionalPaymentPercentage / 100) + 1),
        pTol.PlannedCost + pTol.AdditionalPaymentPercentageCap)) {
            return Target.Abacus.Library.DomContractedExceeded.No;
        }
        return Target.Abacus.Library.DomContractedExceeded.Yes_Cost;
    }
    return Target.Abacus.Library.DomContractedExceeded.No;
}
//class tolerance calculator
function ToleranceCalculator() {
    this.costExceededCalculationLogic = costExceededCalculationLogic;
    this.unitsExceededCalculationLogic = unitsExceededCalculationLogic;
    this.processPaymentToleranceGroupLogic = processPaymentToleranceGroupLogic;
    this.calculateTolerances = calculateTolerances;
    this.resetFinancialValuesOnPaymentToleranceObjects = resetFinancialValuesOnPaymentToleranceObjects;
    this.processProviderInvoiceTolerances = processProviderInvoiceTolerances;
}