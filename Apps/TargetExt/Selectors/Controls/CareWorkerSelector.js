// enum for Care Worker parameters
Selectors.CareWorkerSelectorParameters = {
    ProviderID: '@intProviderID'
};

Selectors.CareWorkerSelector = function(initSettings) {

    var me = this;
    var selectorType = 40;

    // set the type to CareWorker always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);


    me.SetProviderID = function (value) {
        me.AddParameter(Selectors.CareWorkerSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }
}

Selectors.CareWorkerSelector.prototype = new Selectors.SelectorControl();

