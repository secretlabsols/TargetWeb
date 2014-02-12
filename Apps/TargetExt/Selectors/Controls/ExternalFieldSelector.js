// enum for External Field parameters
Selectors.ExternalFieldSelectorParameters = {};

Selectors.ExternalFieldSelector = function(initSettings) {

    var me = this;
    var selectorType = 24;

    // set the type to ExternalField always
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

Selectors.ExternalFieldSelector.prototype = new Selectors.SelectorControl();

