Selectors.FinanceCodeSelectorParameters = {
};

Selectors.FinanceCodeSelector = function(initSettings) {

    var me = this;
    var selectorType = 45;

    // set the type to provider always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType // finance code
        }
    }, initSettings);

    // set the type to provider always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);
}

Selectors.FinanceCodeSelector.prototype = new Selectors.SelectorControl();

