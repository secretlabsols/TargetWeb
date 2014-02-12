// enum for Provider Invoice Notes parameters
Selectors.ProviderInvoiceNoteSelectorParameters = {
    ProviderInvoiceID: '@intProviderInvoiceID'
};

Selectors.ProviderInvoiceNoteSelector = function(initSettings) {

    var me = this;
    var selectorType = 19;

    // set the type to ProviderInvoiceNotes always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // set provider invoice id
    me.SetProviderInvoiceID = function(value) {
        me.AddParameter(Selectors.ProviderInvoiceNoteSelectorParameters.ProviderInvoiceID, Selectors.Helpers.ToInt(value));
    }

}

Selectors.ProviderInvoiceNoteSelector.prototype = new Selectors.SelectorControl();

