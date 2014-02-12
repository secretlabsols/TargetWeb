// enum for Rate Framework parameters
Selectors.RateFrameworkSelectorParameters = { 
    ColumnFilterName: '@strListFilterDescription',
    ColumnFilterReference: '@strListFilterFrameworkType',
    IncludeRedundant: '@blnIncludeRedundant'
};

Selectors.RateFrameworkSelector = function(initSettings) {

    var me = this;
    var selectorType = 6;

    // set the type to RateFramework always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // get include redundant
    me.GetIncludeRedundant = function() {
        return me.GetParameterValue(Selectors.RateFrameworkSelectorParameters.IncludeRedundant);
    }

    // set include redundant
    me.SetIncludeRedundant = function(value) {
        me.AddParameter(Selectors.RateFrameworkSelectorParameters.IncludeRedundant, value);
    }

}

Selectors.RateFrameworkSelector.prototype = new Selectors.SelectorControl();

