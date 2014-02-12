// enum for Non Residential Service User Payment parameters
Selectors.NonResidentialServiceUserPaymentSelectorParameters = 
{
    ProviderID: '@intProviderID',
    GenericContractID: '@intGenericContractID',
    ServiceUserID: '@intClientID',
    PeriodFrom: '@dteDateFrom',
    PeriodTo: '@dteDateTo',
    WeekendingFrom: '@dteWeekendingFrom',
    WeekendingTo: '@dteWeekendingTo',
    InvoiceNumber: '@varInvoiceNo',
    Unpaid: '@blnUnpaid',
    Paid: '@blnPaid',
    Authorised: '@blnAuthorised',
    Suspended: '@blnSuspended',
    HideRetracted: '@blnHideRetraction',

    ColumnFilterServiceUserReference: '@strListFilterSUReference',
    ColumnFilterServiceUserName: '@strListFilterSUName',
    ColumnFilterInvoiceNumber: '@strListFilterInvNumber',
    ColumnFilterContractNumber: '@strListFilterContractNumber',
    ColumnFilterPaymentReference: '@strListFilterPaymentRef'
};

Selectors.NonResidentialServiceUserPaymentSelector = function (initSettings) {

    var me = this;
    var selectorType = 35;

    // set the type to NonResidentialServiceUserPayment always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        },
        showSettings: true
    }, initSettings);

    // set the type to non-res service user payments
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // Get the column filters
    me.GetColumnFilterServiceUserReference = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.ColumnFilterServiceUserReference);
    }

    me.GetColumnFilterServiceUserName = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.ColumnFilterServiceUserName);
    }

    me.GetColumnFilterInvoiceNumber = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.ColumnFilterInvoiceNumber);
    }

    me.GetColumnFilterContractNumber = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.ColumnFilterContractNumber);
    }

    me.GetColumnFilterPaymentReference = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.ColumnFilterPaymentReference);
    }

    // get the provider id
    me.GetProviderID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.ProviderID));
    }

    // get the generic contract id
    me.GetGenericContractID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.GenericContractID));
    }

    // set the svc user id
    me.GetClientID = function (value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.ServiceUserID));
    }

    // get the invoice number
    me.GetInvoiceNumber = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.InvoiceNumber);
    }

    // get the date from
    me.GetPeriodFrom = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.PeriodFrom);
    }

    // get the date to
    me.GetPeriodTo = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.PeriodTo);
    }

    // get the weekending date from
    me.GetWeekendingFrom = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.WeekendingFrom);
    }

    // get the weekending date to
    me.GetWeekendingTo = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.WeekendingTo);
    }

    // get the unpaid checkbox
    me.GetUnpaid = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.Unpaid);
    }

    // get the paid checkbox
    me.GetPaid = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.Paid);
    }

    // get the authorised checkbox
    me.GetAuthorised = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.Authorised);
    }

    // get the suspended checkbox
    me.GetSuspended = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.Suspended);
    }

    // get the hide retracted / retraction checkbox
    me.GetHideRetracted = function () {
        return me.GetParameterValue(Selectors.NonResidentialServiceUserPaymentSelectorParameters.HideRetracted);
    }

    // get payment params to be passed about to web services etc
    me.GetWebServiceParameters = function () {
        var params = {
            ProviderID: me.GetProviderID(),
            GenericContractID: me.GetGenericContractID(),
            ClientID: me.GetClientID(),
            InvoiceNumber: me.GetInvoiceNumber(),
            WeekendingFrom: me.GetWeekendingFrom(),
            WeekendingTo: me.GetWeekendingTo(),
            Unpaid: me.GetUnpaid(),
            Paid: me.GetPaid(),
            Authorised: me.GetAuthorised(),
            Suspended: me.GetSuspended(),
            HideRetracted: me.GetHideRetracted()
        };
        return params
    }

    me.GetUserID = function () {
        return me.GetWebSecurityUserID();
    }

    // set the generic contract id
    me.SetGenericContractID = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.GenericContractID, Selectors.Helpers.ToInt(value));
    }

    // set the provider id
    me.SetProviderID = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    // set the svc user id
    me.SetClientID = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.ServiceUserID, Selectors.Helpers.ToInt(value));
    }

    // set the invoice number
    me.SetInvoiceNumber = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.InvoiceNumber, value);
    }

    // set the date from
    me.SetPeriodFrom = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.PeriodFrom, value);
    }

    // set the date to
    me.SetPeriodTo = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.PeriodTo, value);
    }

    // set the date from
    me.SetWeekendingFrom = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.WeekendingFrom, value);
    }

    // set the date to
    me.SetWeekendingTo = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.WeekendingTo, value);
    }

    // set unpaid
    me.SetUnpaid = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.Unpaid, value);
    }
    // set paid
    me.SetPaid = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.Paid, value);
    }
    // set authorised
    me.SetAuthorised = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.Authorised, value);
    }
    // set suspended
    me.SetSuspended = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.Suspended, value);
    }
    // set hide retracted
    me.SetHideRetracted = function (value) {
        me.AddParameter(Selectors.NonResidentialServiceUserPaymentSelectorParameters.HideRetracted, value);
    }
}

Selectors.NonResidentialServiceUserPaymentSelector.prototype = new Selectors.SelectorControl();

