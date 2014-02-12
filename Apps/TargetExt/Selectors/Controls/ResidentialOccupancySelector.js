// enum for residential payment parameters
Selectors.ResidentialOccupancySelectorParameters = {
    ProviderID: '@intProviderID',
    DateFrom: '@dtDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dtDateTo'
};

Selectors.ResidentialOccupancySelector = function(initSettings) {

    var me = this;
    var selectorType = 39;

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
        me.AddParameter(Selectors.ResidentialOccupancySelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.ResidentialOccupancySelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.ResidentialOccupancySelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.ResidentialOccupancySelectorParameters.DateTo, value);
    }

    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.ResidentialOccupancySelectorParameters.DateFrom);
    }

    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.ResidentialOccupancySelectorParameters.DateTo);
    }

    me.GetDatePeriodType = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ResidentialOccupancySelectorParameters.DatePeriodType));
    }

    me.GetProviderID = function() {
        return me.GetParameterValue(Selectors.ResidentialOccupancySelectorParameters.ProviderID);
    }

}

Selectors.ResidentialOccupancySelector.prototype = new Selectors.SelectorControl();
