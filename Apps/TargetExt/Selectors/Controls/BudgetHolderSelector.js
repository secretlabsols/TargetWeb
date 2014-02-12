// enum for Budget Holder parameters
Selectors.BudgetHolderSelectorParameters = {};

Selectors.BudgetHolderSelector = function(initSettings) {

    var me = this;
    var selectorType = 13;

    // set the type to BudgetHolder always
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

Selectors.BudgetHolderSelector.prototype = new Selectors.SelectorControl();

