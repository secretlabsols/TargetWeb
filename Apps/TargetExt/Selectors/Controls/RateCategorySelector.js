// enum for generic service order parameters
Selectors.RateCategorySelectorParameters = {
    RateFrameworkID: '@intRateFrameworkID'
};

Selectors.RateCategorySelector = function(initSettings) {

    var me = this;
    var myType = 5;

    // set the type always
    initSettings = $.extend(true, {
        request: {
            Type: myType
        }
    }, initSettings);

    // set the type always
    initSettings.request.Type = myType;

    // call the parent constructor
    me.constructor(initSettings);

    // get the Rate Framework ID
    me.GetRateFrameworkID = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.RateCategorySelectorParameters.RateFrameworkID));
    }

    // set the Rate Framework ID
    me.SetRateFrameworkID = function(value) {
        me.AddParameter(Selectors.RateCategorySelectorParameters.RateFrameworkID, value);
    }

    // get rate framework id
    me.GetDomRateFrameWorkID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.RateCategorySelectorParameters.RateFrameworkID));
    }
}

// set the prototype of this selector to the base selector control i.e. inherit functions etc
Selectors.RateCategorySelector.prototype = new Selectors.SelectorControl();