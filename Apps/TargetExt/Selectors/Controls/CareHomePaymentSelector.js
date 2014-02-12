// enum for Care Home Payment parameters
Selectors.CareHomePaymentSelectorParameters = {
    DateFrom: '@DateFrom',
    DatePeriodType: '@intDateSearchTypeID',
    DateTo: '@DateTo',
    SDateFrom: '@SDateFrom',
    SDatePeriodType: '@intSDateSearchTypeID',
    SDateTo: '@SDateTo',
    WebSecurityUserID: '@webSecurityUserID',
    ProviderID: '@intEstablishmentID',
    RemittanceNumber: '@strListFilterRemittanceNumber',
    ProvisionalCreatedCount: '@intProvisionalCreatedCount',
    ProcessStatuses: '@xmlProcessStatuses',
    ProcessStatusBy: '@xmlProcessStatusBy',
    ProcessSubStatuses: '@xmlProcessSubStatuses',
    PublicationMediums: '@xmlPublicationMediums'
};

Selectors.CareHomePaymentSelectorProcessStatuses = {
    ProvisionalCreated: 1,
    ProvisionalApproved: 2,
    ProvisionalPublished: 3,
    ProvisionalRejected: 5
};

Selectors.CareHomePaymentSelector = function (initSettings) {

    var me = this;
    var selectorType = 23;

    // set the type to CareHomePayment always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);


    // set the date from
    me.SetDateFrom = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.DateTo, value);
    }

    // set the status date from
    me.SetSDateFrom = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.SDateFrom, value);
    }

    me.SetSDatePeriodType = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.SDatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the status date to
    me.SetSDateTo = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.SDateTo, value);
    }

    // set the websecurity user id
    me.SetWebSecurityUserID = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.WebSecurityUserID, Selectors.Helpers.ToInt(value));
    }

    // set the provider id
    me.SetProviderID = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    // set the process status by
    me.SetProcessStatusesBy = function (types) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.ProcessStatusBy, types);
    }

    // set the publication medium
    me.SetPublicationMedium = function (types) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.PublicationMediums, types);
    }

    // set the process statuses
    me.SetProcessStatuses = function (types) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.ProcessStatuses, types);
    }

    // set the process sub statuses
    me.SetProcessSubStatuses = function (types) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.ProcessSubStatuses, types);
    }

    // set remittance number 
    me.SetRemittanceNumber = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.RemittanceNumber, value);
    }

    me.SetProvisionalCreatedCount = function (value) {
        me.AddParameter(Selectors.CareHomePaymentSelectorParameters.ProvisionalCreatedCount, value);
    }

    // set the date from
    me.GetDateFrom = function () {
        return me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.DateFrom);
    }

    me.GetDatePeriodType = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.DatePeriodType));
    }

    // get the date to
    me.GetDateTo = function () {
        return me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.DateTo);
    }

    me.GetSDateFrom = function () {
        return me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.SDateFrom);
    }

    me.GetSDatePeriodType = function () {
        return me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.SDatePeriodType);
    }

    // get the date to
    me.GetSDateTo = function () {
        return me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.SDateTo);
    }

    me.GetStatusBy = function (value) {
        return me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.ProcessStatusBy);
    }

    me.GetWebSecurityUserID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.WebSecurityUserID));
    }

    // get the provider id
    me.GetProviderID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.ProviderID));
    }

    // get the status
    me.GetStatus = function () {
        return me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.ProcessStatuses);
    }

    // get the sub status
    me.GetSubStatus = function () {
        return me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.ProcessSubStatuses);
    }

    // get the sub status
    me.GetRemittanceNumber = function () {
        return me.GetParameterValue(Selectors.CareHomePaymentSelectorParameters.RemittanceNumber);
    }

    // get number of authorised payments
    me.GetProvisionalCreatedCount = function () {
        return me.GetAdditionalItemAsInt(Selectors.CareHomePaymentSelectorParameters.ProvisionalCreatedCount);
    }

    // get additional item as an integer
    me.GetAdditionalItemAsInt = function (key) {
        var addItem = me.GetAdditionalItem(key);
        return (addItem ? Selectors.Helpers.ToInt(addItem.Value) : 0);
    }

    // get params to be passed about to web services etc
    me.GetWebServiceParameters = function () {
        var params = {
            DateFrom: me.GetDateFrom(),
            DatePeriodType: me.GetDatePeriodType(),
            DateTo: me.GetDateTo(),
            ProviderID: me.GetProviderID(),
            ProcessStatuses: me.GetStatus(),
            ProcessStatusesBy: me.GetStatusBy(),
            ProcessSubStatuses: me.GetSubStatus(),
            ProcessStatusDateFrom: me.GetSDateFrom(),
            ProcessStatusDatePeriodType: me.GetSDatePeriodType(),
            ProcessStatusDateTo: me.GetSDateTo(),
            WebSecurityUserID: me.GetWebSecurityUserID()
        };
        return params
    }

}


Selectors.CareHomePaymentSelector.prototype = new Selectors.SelectorControl();

