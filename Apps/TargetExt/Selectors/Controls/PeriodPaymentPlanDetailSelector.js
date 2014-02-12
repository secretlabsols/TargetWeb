// enum for Period Payment Plan Detail parameters
Selectors.PeriodPaymentPlanDetailSelectorParameters = {
    DomContractPeriodID: '@intDomContractPeriodID',
    WebSecurityUserID: '@intWebSecurityUserID'
};

Selectors.PeriodPaymentPlanDetailSelector = function (initSettings) {

    var me = this;
    var selectorType = 33;

    // set the type to PeriodPaymentPlanDetail always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // set the dom contract period id
    me.SetDomContractPeriodID = function (value) {
        me.AddParameter(Selectors.PeriodPaymentPlanDetailSelectorParameters.DomContractPeriodID, Selectors.Helpers.ToInt(value));
    }

    // set the web security user id
    me.SetWebSecurityUserID = function (value) {
        me.AddParameter(Selectors.PeriodPaymentPlanDetailSelectorParameters.WebSecurityUserID, Selectors.Helpers.ToInt(value));
    }

}

Selectors.PeriodPaymentPlanDetailSelector.prototype = new Selectors.SelectorControl();

