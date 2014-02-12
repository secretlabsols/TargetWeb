// enum for Contract Number Mapping parameters
Selectors.ContractNumberMappingSelectorParameters = {};

Selectors.ContractNumberMappingSelector = function(initSettings) {

    var me = this;
    var selectorType = 29;

    // set the type to ContractNumberMapping always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

}

Selectors.ContractNumberMappingSelector.prototype = new Selectors.SelectorControl();

