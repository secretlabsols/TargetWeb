// enum for Spend Plan parameters
Selectors.SpendPlanSelectorParameters = {
    ColumnFilterServiceUserName: '@strListFilterServiceUserName',
    ColumnFilterServiceUserReference: '@strListFilterServiceUserReference',
    ColumnFilterSpendPlanReference: '@strListFilterSpendPlanReference',
    DateFrom: '@dtDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dtDateTo',
    ServiceUserID: '@intClientID',
    ColumnFilterNHSNumber: '@strListFilterNHSNumber'
};

Selectors.SpendPlanSelector = function(initSettings) {

    var me = this;
    var selectorType = 11;

    // set the type to SpendPlan always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to spend plan always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // get the date from
    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.SpendPlanSelectorParameters.DateFrom);
    }

    // get the date period type
    me.GetDatePeriodType = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.SpendPlanSelectorParameters.DatePeriodType));
    }

    // get the date to
    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.SpendPlanSelectorParameters.DateTo);
    }

    // set the svc user id
    me.GetServiceUserID = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.SpendPlanSelectorParameters.ServiceUserID));
    }

    // get the service user name column filter
    me.GetColumnFilterServiceUserName = function() {
        return me.GetParameterValue(Selectors.SpendPlanSelectorParameters.ColumnFilterServiceUserName);
    }

    // get the service user ref column filter
    me.GetColumnFilterServiceUserReference = function() {
        return me.GetParameterValue(Selectors.SpendPlanSelectorParameters.ColumnFilterServiceUserReference);
    }

    // get the spend plan ref column filter
    me.GetColumnFilterSpendPlanReference = function() {
        return me.GetParameterValue(Selectors.SpendPlanSelectorParameters.ColumnFilterSpendPlanReference);
    }
    // get the spend plan ref column filter
    me.GetColumnFilterNHSNumber = function() {
        return me.GetParameterValue(Selectors.SpendPlanSelectorParameters.ColumnFilterNHSNumber);
    }
    
    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.SpendPlanSelectorParameters.DateFrom, value);
    }

    // set the date period type
    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.SpendPlanSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.SpendPlanSelectorParameters.DateTo, value);
    }

    // set the svc user id
    me.SetServiceUserID = function(value) {
        me.AddParameter(Selectors.SpendPlanSelectorParameters.ServiceUserID, Selectors.Helpers.ToInt(value));
    }

}

Selectors.SpendPlanSelector.prototype = new Selectors.SelectorControl();

