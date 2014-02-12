// enum for Finance Code Matrix parameters
Selectors.FinanceCodeMatrixSelectorParameters = {};

Selectors.FinanceCodeMatrixSelector = function(initSettings) {

    var me = this;
    var selectorType = 32;

    // set the type to FinanceCodeMatrix always
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

Selectors.FinanceCodeMatrixSelector.prototype = new Selectors.SelectorControl();

