// enum for Sanity Checks parameters
Selectors.SanityChecksSelectorParameters = {};

Selectors.SanityChecksSelector = function(initSettings) {

    var me = this;
    var selectorType = 25;

    // set the type to SanityChecks always
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

Selectors.SanityChecksSelector.prototype = new Selectors.SelectorControl();

