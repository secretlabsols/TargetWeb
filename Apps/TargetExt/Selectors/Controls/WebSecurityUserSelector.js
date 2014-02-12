// enum for Web Security User parameters
Selectors.WebSecurityUserSelectorParameters = {};

Selectors.WebSecurityUserSelector = function(initSettings) {

    var me = this;
    var selectorType = 22;

    // set the type to WebSecurityUser always
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

Selectors.WebSecurityUserSelector.prototype = new Selectors.SelectorControl();

