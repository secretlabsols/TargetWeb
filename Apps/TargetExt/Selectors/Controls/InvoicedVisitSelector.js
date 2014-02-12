// enum for invoiced visit parameters
Selectors.InvoicedVisitSelectorParameters = {
    DateFrom: '@dtDateTimeFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dtDateTimeTo',
    GenericContractID: '@intGenericContractID',
    ProviderID: '@intProviderID',
    ServiceUserID: '@intClientID',
    CareWorkerID: '@intCareWorkerID'
};

Selectors.InvoicedVisitSelector = function(initSettings) {

    var me = this;
    var selectorType = 15;

    // set the type to InvoicedVisit always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to invoiced visit always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // get the date from
    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.InvoicedVisitSelectorParameters.DateFrom);
    }

    // get the date period type
    me.GetDatePeriodType = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.InvoicedVisitSelectorParameters.DatePeriodType));
    }

    // get the date to
    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.InvoicedVisitSelectorParameters.DateTo);
    }

    // get the generic contract id
    me.GetGenericContractID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.InvoicedVisitSelectorParameters.GenericContractID));
    }

    // get the provider id
    me.GetProviderID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.InvoicedVisitSelectorParameters.ProviderID));
    }

    // set the svc user id
    me.GetServiceUserID = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.InvoicedVisitSelectorParameters.ServiceUserID));
    }

    // set the care worker id
    me.GetCareWorkerID = function (value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.InvoicedVisitSelectorParameters.CareWorkerID));
    }

    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.InvoicedVisitSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.InvoicedVisitSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }
    
    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.InvoicedVisitSelectorParameters.DateTo, value);
    }

    // set the generic contract id
    me.SetGenericContractID = function(value) {
        me.AddParameter(Selectors.InvoicedVisitSelectorParameters.GenericContractID, Selectors.Helpers.ToInt(value));
    }

    // set the provider id
    me.SetProviderID = function(value) {
        me.AddParameter(Selectors.InvoicedVisitSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    // set the svc user id
    me.SetServiceUserID = function(value) {
        me.AddParameter(Selectors.InvoicedVisitSelectorParameters.ServiceUserID, Selectors.Helpers.ToInt(value));
    }

    // set the care worker id
    me.SetCareWorkerID = function (value) {
        me.AddParameter(Selectors.InvoicedVisitSelectorParameters.CareWorkerID, Selectors.Helpers.ToInt(value));
    }
}

Selectors.InvoicedVisitSelector.prototype = new Selectors.SelectorControl();

