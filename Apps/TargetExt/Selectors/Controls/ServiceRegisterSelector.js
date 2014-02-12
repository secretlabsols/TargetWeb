// enum for Service Register parameters
Selectors.ServiceRegisterSelectorParameters = {
    DateFrom: '@dteDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dteDateTo',
    GenericContractID: '@intContractID',
    ProviderID: '@intEstablishmentID',
    Status: '@status',
    ListFilterProviderName:  "@strProviderName",
    ListFilterContractNumber: "@strContractNumber",
    ListFilterContractTitle: "@strContractTitle",
    ListFilterRegisterstatus: "@strRegisterStatus" ,
    StatusInProgress: "@blnInProgress",
    StatusSubmitted : "@blnSubmitted",
    StatusAmended : "@blnAmended" ,
    StatusProcessed : "@blnProcessed"
};

Selectors.ServiceRegisterSelector = function(initSettings) {

    var me = this;
    var selectorType = 7;

    // set the type to ServiceRegister always
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
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.ServiceRegisterSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.ServiceRegisterSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.ServiceRegisterSelectorParameters.DateTo, value);
    }

    // set the generic contract id
    me.SetGenericContractID = function(value) {
        me.AddParameter(Selectors.ServiceRegisterSelectorParameters.GenericContractID, Selectors.Helpers.ToInt(value));
    }

    // set the provider id
    me.SetProviderID = function(value) {
        me.AddParameter(Selectors.ServiceRegisterSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    // set the status
    me.SetStatus = function(value) {
        me.AddParameter(Selectors.ServiceRegisterSelectorParameters.Status, value);
    }

    // set the date from
    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.DateFrom);
    }

    me.GetDatePeriodType = function() {
        return me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.DatePeriodType);
    }

    // get the date to
    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.DateTo);
    }

    // get the generic contract id
    me.GetGenericContractID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.GenericContractID));
    }

    // get the provider id
    me.GetProviderID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.ProviderID));
    }

    // get the status
    me.GetStatus = function() {
        return me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.Status);
    }

    me.GetListFilterProviderName = function() {
        return me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.ListFilterProviderName);
    }

    me.GetListFilterContractNumber = function() {
        return me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.ListFilterContractNumber);
    }

    me.GetListFilterContractTitle = function() {
        return me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.ListFilterContractTitle);
    }

    me.GetListFilterRegisterstatus = function() {
        return me.GetParameterValue(Selectors.ServiceRegisterSelectorParameters.ListFilterRegisterstatus);
    }

    me.GetStatusContainsKey = function(key) {
        return Ext.Array.contains(me.GetStatus(), key);
    }

    me.GetStatusInProgress = function() {
        return me.GetStatusContainsKey('1');
    }

    me.GetStatusSubmitted = function() {
        return me.GetStatusContainsKey('2');
    }

    me.GetStatusAmended = function() {
        return me.GetStatusContainsKey('3');
    }

    me.GetStatusProcessed = function() {
        return me.GetStatusContainsKey('4');
    }

}

Selectors.ServiceRegisterSelector.prototype = new Selectors.SelectorControl();

