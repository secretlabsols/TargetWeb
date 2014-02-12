// enum for Service Delivery File parameters
Selectors.ServiceDeliveryFileSelectorParameters = { 
    SubmittedByUserID: '@intSubmittedByUserID',
    DateFrom: '@dteDateFrom',
    DateTo: '@dteDateTo',
    AwaitingProc: '@blnAwaiting',
    WorkProgress: '@blnWorkInProgress',
    Processed: '@blnProcessed',
    Deleted: '@blnDeleted',
    Failed: '@blnFailed'
};

Selectors.ServiceDeliveryFileSelector = function(initSettings) {

    var me = this;
    var selectorType = 38;

    // set the type to ServiceDeliveryFile always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);


    // get the date from
    me.SubmittedByUserID = function () {
        return me.GetParameterValue(Selectors.ServiceDeliveryFileSelectorParameters.SubmittedByUserID);
    }

    // get the date from
    me.GetDateFrom = function () {
        return me.GetParameterValue(Selectors.ServiceDeliveryFileSelectorParameters.DateFrom);
    }

    // get the date to
    me.GetDateTo = function () {
        return me.GetParameterValue(Selectors.ServiceDeliveryFileSelectorParameters.DateTo);
    }

    // get the Awaiting Proc
    me.GetAwaitingProc = function () {
        return me.GetParameterValue(Selectors.ServiceDeliveryFileSelectorParameters.AwaitingProc);
    }

    // get the WorkProgress
    me.GetWorkProgress = function () {
        return me.GetParameterValue(Selectors.ServiceDeliveryFileSelectorParameters.WorkProgress);
    }

    // get the Processed
    me.GetProcessed = function () {
        return me.GetParameterValue(Selectors.ServiceDeliveryFileSelectorParameters.Processed);
    }

    // get the Deleted
    me.GetDeleted = function () {
        return me.GetParameterValue(Selectors.ServiceDeliveryFileSelectorParameters.Deleted);
    }

    // get the Failed
    me.GetFailed = function () {
        return me.GetParameterValue(Selectors.ServiceDeliveryFileSelectorParameters.Failed);
    }


    // set the SubmittedByUserID
    me.SetSubmittedByUserID = function (value) {
        me.AddParameter(Selectors.ServiceDeliveryFileSelectorParameters.SubmittedByUserID, value);
    }

    // set the date from
    me.SetDateFrom = function (value) {
        me.AddParameter(Selectors.ServiceDeliveryFileSelectorParameters.DateFrom, value);
    }

    // set the date to
    me.SetDateTo = function (value) {
        me.AddParameter(Selectors.ServiceDeliveryFileSelectorParameters.DateTo, value);
    }

    // set the Awaiting Proc
    me.SetAwaitingProc = function (value) {
        me.AddParameter(Selectors.ServiceDeliveryFileSelectorParameters.AwaitingProc, value);
    }

    // Set the WorkProgress
    me.SetWorkProgress = function (value) {
        me.AddParameter(Selectors.ServiceDeliveryFileSelectorParameters.WorkProgress, value);
    }

    // Set the Processed
    me.SetProcessed = function (value) {
        me.AddParameter(Selectors.ServiceDeliveryFileSelectorParameters.Processed, value);
    }

    // Set the Deleted
    me.SetDeleted = function (value) {
        me.AddParameter(Selectors.ServiceDeliveryFileSelectorParameters.Deleted, value);
    }

    // get the Failed
    me.SetFailed = function (value) {
        me.AddParameter(Selectors.ServiceDeliveryFileSelectorParameters.Failed, value);
    }

}

Selectors.ServiceDeliveryFileSelector.prototype = new Selectors.SelectorControl();

