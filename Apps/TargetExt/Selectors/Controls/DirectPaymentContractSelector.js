// enum for Direct Payment Contract parameters
Selectors.DirectPaymentContractSelectorParameters = {
    FilterServiceUserID: '@pintClientID',
    FilterSetBudgetHolderID: '@pintBudgetHolderID',
    FilterSetDatePeriodType: '@intDatePeriodType',
    FilterSetDateFrom: '@pdtDateFrom',
    FilterSetDateTo: '@pdtDateTo',
    FilterSetIsSDS: '@pstrFilterSDS'
};

Selectors.DirectPaymentContractSelector = function(initSettings) {

    var me = this;
    var selectorType = 16;

    // set the type to DirectPaymentContract always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // set filter by user
    me.SetServiceUserID = function(value) {
        me.AddParameter(Selectors.ServiceGroupSelectorParameters.ServiceUserID, value);
    }
    // set filter by user
    me.SetBudgetHolderID = function(value) {
        me.AddParameter(Selectors.ServiceGroupSelectorParameters.BudgetHolderID, value);
    }
    // set filter by user
    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.ServiceGroupSelectorParameters.DatePeriodType, value);
    }
    // set filter by user
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.ServiceGroupSelectorParameters.DateFrom, value);
    }
    // set filter by user
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.ServiceGroupSelectorParameters.DateTo, value);
    }
    // set filter by user
    me.SetIsSDS = function(value) {
        me.AddParameter(Selectors.ServiceGroupSelectorParameters.IsSDS, value);
    }                    
}

Selectors.DirectPaymentContractSelector.prototype = new Selectors.SelectorControl();

