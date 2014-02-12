// enum for generic service order parameters
Selectors.GenericServiceOrderSelectorParameters = {
    ColumnFilterContractNumber: '@strListFilterContractNumber',
    ColumnFilterOrderReference: '@strListFilterOrderReference',
    ColumnFilterProviderName: '@strListFilterProviderName',
    ColumnFilterServiceGroupDescription: '@strListFilterServiceGroupDescription',
    ColumnFilterServiceUserName: '@strListFilterServiceUserName',
    ColumnFilterServiceUserReference: '@strListFilterServiceUserReference',
    ColumnFilterNHSNumber: '@strListFilterNHSNumber',
    DateFrom: '@dtDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dtDateTo',
    GenericContractID: '@intGenericContractID',
    ProviderID: '@intProviderID',
    ServiceGroupID: '@intServiceGroupID',
    ServiceUserID: '@intClientDetailID',
    GenericCreditorPaymentID: '@GenericCreditorPaymentID'
};

Selectors.GenericServiceOrderSelector = function(initSettings) {

    var me = this;
    var selectorType = 4;

    // set the type to GenericServiceOrder always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // get the contract number column filter
    me.GetColumnFilterContractNumber = function() {
        return me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ColumnFilterContractNumber);
    }

    // get the order reference column filter
    me.GetColumnFilterOrderReference = function() {
        return me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ColumnFilterOrderReference);
    }

    // get the provider name column filter
    me.GetColumnFilterProviderName = function() {
        return me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ColumnFilterProviderName);
    }

    // get the service group column filter
    me.GetColumnFilterServiceGroupDescription = function() {
        return me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ColumnFilterServiceGroupDescription);
    }

    // get the service user name column filter
    me.GetColumnFilterServiceUserName = function() {
        return me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ColumnFilterServiceUserName);
    }

    // get the service user ref column filter
    me.GetColumnFilterServiceUserReference = function() {
        return me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ColumnFilterServiceUserReference);
    }

    // get the NHS number column filter
    me.GetColumnFilterServiceNHSNumber = function() {
        return me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ColumnFilterNHSNumber);
    }
    
    // get the date from
    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.DateFrom);
    }

    // get the date period type
    me.GetDatePeriodType = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.DatePeriodType));
    }

    // get the date to
    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.DateTo);
    }

    // get the generic contract id
    me.GetGenericContractID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.GenericContractID));
    }

    // get the provider id
    me.GetProviderID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ProviderID));
    }

    // get the svc group id
    me.GetServiceGroupID = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ServiceGroupID));
    }

    // set the svc user id
    me.GetServiceUserID = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericServiceOrderSelectorParameters.ServiceUserID));
    }

    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.GenericServiceOrderSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.GenericServiceOrderSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }
    
    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.GenericServiceOrderSelectorParameters.DateTo, value);
    }

    // set the generic contract id
    me.SetGenericContractID = function(value) {
        me.AddParameter(Selectors.GenericServiceOrderSelectorParameters.GenericContractID, Selectors.Helpers.ToInt(value));
    }

    // set the provider id
    me.SetProviderID = function(value) {
        me.AddParameter(Selectors.GenericServiceOrderSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    // set the svc group id
    me.SetServiceGroupID = function(value) {
        me.AddParameter(Selectors.GenericServiceOrderSelectorParameters.ServiceGroupID, Selectors.Helpers.ToInt(value));
    }

    // set the svc user id
    me.SetServiceUserID = function(value) {
        me.AddParameter(Selectors.GenericServiceOrderSelectorParameters.ServiceUserID, Selectors.Helpers.ToInt(value));
    }

    me.SetGenericCreditorPaymentID = function (value) {
        me.AddParameter(Selectors.GenericServiceOrderSelectorParameters.GenericCreditorPaymentID, Selectors.Helpers.ToInt(value));
    }
    
}

Selectors.GenericServiceOrderSelector.prototype = new Selectors.SelectorControl();

