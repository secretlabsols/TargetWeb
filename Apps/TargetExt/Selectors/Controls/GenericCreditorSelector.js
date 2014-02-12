// enum for Generic Creditor parameters
Selectors.GenericCreditorSelectorParameters = {};

Selectors.GenericCreditorSelector = function(initSettings) {

    var me = this;
    var selectorType = 17;

    // set the type to GenericCreditor always
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

Selectors.GenericCreditorSelector.prototype = new Selectors.SelectorControl();

