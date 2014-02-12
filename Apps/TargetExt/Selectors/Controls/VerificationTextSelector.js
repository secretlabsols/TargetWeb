// enum for Verification Text parameters
Selectors.VerificationTextSelectorParameters = {};

Selectors.VerificationTextSelector = function(initSettings) {

    var me = this;
    var selectorType = 43;

    // set the type to VerificationText always
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

Selectors.VerificationTextSelector.prototype = new Selectors.SelectorControl();

