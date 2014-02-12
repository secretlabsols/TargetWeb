// enum for Dom Service Order Provider Unit Cost Override parameters
Selectors.DomServiceOrderProviderUnitCostOverrideSelectorParameters = { 
    DomServiceOrderID: '@intDomServiceOrderID'
};

Selectors.DomServiceOrderProviderUnitCostOverrideSelector = function (initSettings) {

    var me = this;
    var selectorType = 31;

    // set the type to DomServiceOrderProviderUnitCostOverride always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;
    // call the parent constructor
    me.constructor(initSettings);

    // set service order id
    me.SetDomServiceOrderID = function (value) {
        me.AddParameter(Selectors.DomServiceOrderProviderUnitCostOverrideSelectorParameters.DomServiceOrderID, Selectors.Helpers.ToInt(value));
    }
}

Selectors.DomServiceOrderProviderUnitCostOverrideSelector.prototype = new Selectors.SelectorControl();

