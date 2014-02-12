// enum for Payment Schedule parameters
Selectors.PaymentScheduleSelectorParameters = {
    GenericContractID: '@intContractID',
    ProviderID: '@intProviderID',
    DateFrom: '@dtePeriodFrom',
    DateTo: '@dtePeriodTo',
    NoProforma: '@blnNoProformaInvoice',
    AwaitingVerification: '@blnAwaitingVerificationProformaInvoice',
    Verified: '@blnVerifiedProformaInvoice',
    UnpaidInv: '@blnUnPaidProviderInvoice',
    SuspendedInv: '@blnSuspendedProviderInvoice',
    AuthorisedInv: '@blnAuthorisedProviderInvoice',
    PaidInv: '@blnPaidProviderInvoice',
    AMAwaitingVerification: '@blnAwaitingVerificationAmendments',
    ARVerified: '@blnVerifiedAmendments',
    ARDeclined: '@blnDeclinedVerificationAmendments'
};


Selectors.PaymentScheduleSelector = function(initSettings) {

    var me = this;
    var selectorType = 36;

    // set the type to PaymentSchedule always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // get the generic contract id
    me.GetGenericContractID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.GenericContractID));
    }

    // get the provider id
    me.GetProviderID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.ProviderID));
    }

    // get the date from
    me.GetDateFrom = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.DateFrom);
    }

    // get the date to
    me.GetDateTo = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.DateTo);
    }
    me.GetNoProforma = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.NoProforma);
    }

    me.GetAwaitingVerification = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.AwaitingVerification);
    }

    me.GetVerified = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.Verified);
    }

    me.GetUnpaidInv = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.UnpaidInv);
    }

    me.GetSuspendedInv = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.SuspendedInv);
    }

    me.GetAuthorisedInv = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.AuthorisedInv);
    }

    me.GetPaidInv = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.PaidInv);
    }

    me.GetAMAwaitingVerification = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.AMAwaitingVerification);
    }

    me.GetARVerified = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.ARVerified);
    }

    me.GetARDeclined = function () {
        return me.GetParameterValue(Selectors.PaymentScheduleSelectorParameters.ARDeclined);
    }







    // set the provider id
    me.SetProviderID = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }
    // set the generic contract id
    me.SetGenericContractID = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.GenericContractID, Selectors.Helpers.ToInt(value));
    }

    // set the date from
    me.SetDateFrom = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.DateFrom, value);
    }

    // set the date to
    me.SetDateTo = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.DateTo, value);
    }
    
    me.SetNoProforma = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.NoProforma, value);
    }

    me.SetAwaitingVerification = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.AwaitingVerification, value);
    }

    me.SetVerified = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.Verified, value);
    }

    me.SetUnpaidInv = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.UnpaidInv, value);
    }

    me.SetSuspendedInv = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.SuspendedInv, value);
    }

    me.SetAuthorisedInv = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.AuthorisedInv, value);
    }

    me.SetPaidInv = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.PaidInv, value);
    }

    me.SetAMAwaitingVerification = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.AMAwaitingVerification, value);
    }

    me.SetARVerified = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.ARVerified, value);
    }

    me.SetARDeclined = function (value) {
        me.AddParameter(Selectors.PaymentScheduleSelectorParameters.ARDeclined, value);
    }

}

Selectors.PaymentScheduleSelector.prototype = new Selectors.SelectorControl();

