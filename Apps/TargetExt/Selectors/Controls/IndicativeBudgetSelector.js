// enum for Indicative Budget parameters
Selectors.IndicativeBudgetSelectorParameters = {
    DateFrom: '@dtDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dtDateTo',
    ServiceUserID: '@intClientID',
    FilterServiceUser: '@strListFilterServiceUserName',
    FilterServiceUserRef: '@strListFilterServiceUserReference',
    FilterNHSNumber: '@strListFilterNHSNumber'
};

Selectors.IndicativeBudgetSelector = function(initSettings) {

    var me = this;
    var selectorType = 8;

    // set the type to IndicativeBudget always
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
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.IndicativeBudgetSelectorParameters.DatePeriodType));
    }
    
    // get the date from
    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.IndicativeBudgetSelectorParameters.DateFrom);
    }

    // get the date to
    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.IndicativeBudgetSelectorParameters.DateTo);
    }

    // get the svc user id
    me.GetClientID = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.IndicativeBudgetSelectorParameters.ServiceUserID));
    }
    
    // get service user name
    me.GetListFilterServiceUser = function() {
        return me.GetParameterValue(Selectors.IndicativeBudgetSelectorParameters.FilterServiceUser);
    }

    // get service user ref
    me.GetListFilterServiceUserRef = function() {
        return me.GetParameterValue(Selectors.IndicativeBudgetSelectorParameters.FilterServiceUserRef);
    }

    // get nhs number
    me.GetListFilterNHSNumber = function() {
        return me.GetParameterValue(Selectors.IndicativeBudgetSelectorParameters.FilterNHSNumber);
    }
        
    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.IndicativeBudgetSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.IndicativeBudgetSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.IndicativeBudgetSelectorParameters.DateTo, value);
    }

    // set the svc user id
    me.SetServiceUserID = function(value) {
        me.AddParameter(Selectors.IndicativeBudgetSelectorParameters.ServiceUserID, Selectors.Helpers.ToInt(value));
    }

}

Selectors.IndicativeBudgetSelector.prototype = new Selectors.SelectorControl();

