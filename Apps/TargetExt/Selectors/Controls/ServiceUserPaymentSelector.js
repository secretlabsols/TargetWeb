// enum for residential payment parameters
Selectors.ServiceUserPaymentSelectorParameters = {
    ProviderID: '@intProviderID',
    ServiceUserID: '@intServiceUserID',
    DateFrom: '@dtDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dtDateTo'
};

Selectors.ServiceUserPaymentSelector = function(initSettings) {

    var me = this;
    var selectorType = 41;

    // set the type to contract always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to residential payment always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    me.SetProviderID = function(value) {
        me.AddParameter(Selectors.ServiceUserPaymentSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    me.SetServiceUserID = function (value) {
        me.AddParameter(Selectors.ServiceUserPaymentSelectorParameters.ServiceUserID, Selectors.Helpers.ToInt(value));
    }

    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.ServiceUserPaymentSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.ServiceUserPaymentSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.ServiceUserPaymentSelectorParameters.DateTo, value);
    }

    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.ServiceUserPaymentSelectorParameters.DateFrom);
    }

    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.ServiceUserPaymentSelectorParameters.DateTo);
    }

    me.GetDatePeriodType = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ServiceUserPaymentSelectorParameters.DatePeriodType));
    }

    me.GetProviderID = function() {
        return me.GetParameterValue(Selectors.ServiceUserPaymentSelectorParameters.ProviderID);
    }

    me.GetServiceUserID = function (value) {
        return me.GetParameterValue(Selectors.ServiceUserPaymentSelectorParameters.ServiceUserID);
    }
}

Selectors.ServiceUserPaymentSelector.prototype = new Selectors.SelectorControl();
