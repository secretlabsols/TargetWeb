Selectors.ProviderSelectorParameters = {
    ColumnFilterName: '@strListFilterName',
    ColumnFilterReference: '@strListFilterReference',
    IncludeRedundant: '@blnIncludeRedundant',
    IncludeBedsHomes: '@blnIsBedsHome',
    IncludeResidential: '@blnIsResidential'    
};

Selectors.ProviderSelector = function(initSettings) {

    var me = this;
    var selectorType = 3;

    // set the type to provider always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType // provider
        }
    }, initSettings);

    // set the type to provider always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // get include redundant
    me.GetIncludeRedundant = function() {
        return me.GetParameterValue(Selectors.ProviderSelectorParameters.IncludeRedundant);
    }

    // set include beds home -- true/false or null
    me.SetIncludeBedsHomes = function (value) {
        me.AddParameter(Selectors.ProviderSelectorParameters.IncludeBedsHomes, value);
    }

    // set include redundant
    me.SetIncludeRedundant = function(value) {
        me.AddParameter(Selectors.ProviderSelectorParameters.IncludeRedundant, value);
    }

    // set include residential -- true/false or null
    me.SetIncludeResidential = function (value) {
        me.AddParameter(Selectors.ProviderSelectorParameters.IncludeResidential, value);
    }

}

Selectors.ProviderSelector.prototype = new Selectors.SelectorControl();

