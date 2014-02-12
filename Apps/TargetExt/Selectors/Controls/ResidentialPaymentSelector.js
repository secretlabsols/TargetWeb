// enum for residential payment parameters
Selectors.ResidentialPaymentSelectorParameters = {
    ListFilterNumber: '@strListFilterRemittanceNumber',
    ProviderID: '@intProviderID',
    DateFrom: '@dtDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dtDateTo'
};

Selectors.ResidentialPaymentSelector = function(initSettings) {

    var me = this;
    var selectorType = 37;

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
        me.AddParameter(Selectors.ResidentialPaymentSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.ResidentialPaymentSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.ResidentialPaymentSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.ResidentialPaymentSelectorParameters.DateTo, value);
    }

    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.ResidentialPaymentSelectorParameters.DateFrom);
    }

    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.ResidentialPaymentSelectorParameters.DateTo);
    }

    me.GetDatePeriodType = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ResidentialPaymentSelectorParameters.DatePeriodType));
    }

    me.GetListFilterNumber = function() {
        return me.GetParameterValue(Selectors.ResidentialPaymentSelectorParameters.ListFilterNumber);
    }

    me.GetProviderID = function() {
        return me.GetParameterValue(Selectors.ResidentialPaymentSelectorParameters.ProviderID);
    }

}

Selectors.ResidentialPaymentSelector.prototype = new Selectors.SelectorControl();
