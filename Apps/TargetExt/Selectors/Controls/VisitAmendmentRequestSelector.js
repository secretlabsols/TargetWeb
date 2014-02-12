// enum for Visitamendment Request parameters
Selectors.VisitAmendmentRequestSelectorParameters = {
    ProviderID : '@intProviderId',
    GenericContractID: '@intContractId',
    ClientID : '@intClientID',
    OrigCouncil: '@blnOrigCouncil',
    OrigProvider: '@blnOrigProvider',
    RequestByID : '@intReqByID',
    RequestDateFrom : '@dteRequestDateFrom',
    RequestDateTo : '@dteRequestDateTo',
    StatusDateFrom : '@dteStatusDateFrom',
    StatusDateTo : '@dteStatusDateTo',
    StatusAwaitingVerification: '@blnStatusAwaitingVerification',
	StatusDeclined: '@blnStatusDeclined' ,
	StatusProcessed: '@blnStatusProcessed' ,
	StatusInvoiced: '@blnStatusInvoiced' ,
	StatusVerified: '@blnStatusVerified' 
};

Selectors.VisitAmendmentRequestSelector = function (initSettings) {

    var me = this;
    var selectorType = 42;

    // set the type to VisitamendmentRequest always
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
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    me.SetGenericContractID = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.GenericContractID, Selectors.Helpers.ToInt(value));
    }

    me.SetClientID = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.ClientID, Selectors.Helpers.ToInt(value));
    }

    me.SetOrigCouncil = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.OrigCouncil, value);
    }

    me.SetOrigProvider = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.OrigProvider, value);
    }

    me.SetRequestByID = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.RequestByID, Selectors.Helpers.ToInt(value));
    }

    me.SetRequestDateFrom = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.RequestDateFrom, value);
    }

    // set the date to
    me.SetRequestDateTo = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.RequestDateTo, value);
    }

    me.SetStatusDateFrom = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.StatusDateFrom, value);
    }

    // set the date to
    me.SetStatusDateTo = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.StatusDateTo, value);
    }

    me.SetStatusAwaiting = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.StatusAwaitingVerification, value);
    }

    me.SetStatusDeclined = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.StatusDeclined, value);
    }

    me.SetStatusProcessed = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.StatusProcessed, value);
    }

    me.SetStatusInvoiced = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.StatusInvoiced, value);
    }

    me.SetStatusVerified = function (value) {
        me.AddParameter(Selectors.VisitAmendmentRequestSelectorParameters.StatusVerified, value);
    }






    me.GetProviderID = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.ProviderID);
    }

    me.GetGenericContractID = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.GenericContractID);
    }

    me.GetClientID = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.ClientID);
    }

    me.GetOrigCouncil = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.OrigCouncil);
    }

    me.GetOrigProvider = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.OrigProvider);
    }

    me.GetRequestByID = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.RequestByID);
    }

    me.GetRequestDateFrom = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.RequestDateFrom);
    }

    // set the date to
    me.GetRequestDateTo = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.RequestDateTo);
    }

    me.GetStatusDateFrom = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.StatusDateFrom);
    }

    // set the date to
    me.GetStatusDateTo = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.StatusDateTo);
    }

    me.GetStatusAwaiting = function () {
        var test =   me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.StatusAwaitingVerification);
        return test;
    }

    me.GetStatusDeclined = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.StatusDeclined);
    } 

    me.GetStatusProcessed = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.StatusProcessed);
    } 

    me.GetStatusInvoiced = function () {
      return  me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.StatusInvoiced);
    }

    me.GetStatusVerified = function () {
        return me.GetParameterValue(Selectors.VisitAmendmentRequestSelectorParameters.StatusVerified);
    } 


}

Selectors.VisitAmendmentRequestSelector.prototype = new Selectors.SelectorControl();

