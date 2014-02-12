// enum for Generic Creditor Payment Suspension History parameters
Selectors.GenericCreditorPaymentSuspensionHistorySelectorParameters = {
    GenericCreditorPaymentID: '@intGenericCreditorPaymentID'
};

Selectors.GenericCreditorPaymentSuspensionHistorySelector = function(initSettings) {

    var me = this;
    var selectorType = 20;

    // set the type to GenericCreditorPaymentSuspensionHistory always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // set generic creditor payment id
    me.SetGenericCreditorPaymentID = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSuspensionHistorySelectorParameters.GenericCreditorPaymentID, Selectors.Helpers.ToInt(value));
    }

}

Selectors.GenericCreditorPaymentSuspensionHistorySelector.prototype = new Selectors.SelectorControl();

