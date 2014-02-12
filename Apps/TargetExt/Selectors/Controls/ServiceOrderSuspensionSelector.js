// enum for Service Order Suspension parameters
Selectors.ServiceOrderSuspensionSelectorParameters = {
    DateFrom: '@dtDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dtDateTo',
    ServiceUserID: '@intClientID',
    FilterServiceUser: '@strListFilterServiceUserName',
    FilterServiceUserRef: '@strListFilterServiceUserReference',
    FilterNHSNumber: '@strListFilterNHSNumber'
};

Selectors.ServiceOrderSuspensionSelector = function(initSettings) {

    var me = this;
    var selectorType = 9;

    // set the type to ServiceOrderSuspension always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // get the date period type
    me.GetDatePeriodType = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ServiceOrderSuspensionSelectorParameters.DatePeriodType));
    }
    
    // get the date from
    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.ServiceOrderSuspensionSelectorParameters.DateFrom);
    }

    // get the date to
    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.ServiceOrderSuspensionSelectorParameters.DateTo);
    }

    // set the svc user id
    me.GetClientID = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ServiceOrderSuspensionSelectorParameters.ServiceUserID));
    }

    // get service user name
    me.GetListFilterServiceUser = function() {
        return me.GetParameterValue(Selectors.ServiceOrderSuspensionSelectorParameters.FilterServiceUser);
    }
    // get nhs number
    me.GetListFilterNHSNumber = function() {
        return me.GetParameterValue(Selectors.ServiceOrderSuspensionSelectorParameters.FilterNHSNumber);
    }    

    // get service user ref
    me.GetListFilterServiceUserRef = function() {
        return me.GetParameterValue(Selectors.ServiceOrderSuspensionSelectorParameters.FilterServiceUserRef);
    }
    
    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.ServiceOrderSuspensionSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.ServiceOrderSuspensionSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.ServiceOrderSuspensionSelectorParameters.DateTo, value);
    }

    // set the svc user id
    me.SetServiceUserID = function(value) {
        me.AddParameter(Selectors.ServiceOrderSuspensionSelectorParameters.ServiceUserID, Selectors.Helpers.ToInt(value));
    }
}

Selectors.ServiceOrderSuspensionSelector.prototype = new Selectors.SelectorControl();

