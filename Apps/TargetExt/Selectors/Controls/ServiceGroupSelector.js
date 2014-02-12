// enum for Service Group parameters
Selectors.ServiceGroupSelectorParameters = {
    FilterbyUser: '@blnFilterByUser'
};

Selectors.ServiceGroupSelector = function(initSettings) {

    var me = this;
    var selectorType = 12;

    // set the type to ServiceGroup always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // set filter by user
    me.SetFilterbyUser = function(value) {
        me.AddParameter(Selectors.ServiceGroupSelectorParameters.FilterbyUser, value);
    }  
}

Selectors.ServiceGroupSelector.prototype = new Selectors.SelectorControl();

