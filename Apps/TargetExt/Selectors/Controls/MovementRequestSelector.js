// enum for Movement Request parameters
Selectors.MovementRequestSelectorParameters = {
    MovementDateFrom: '@dtBedsMovementDateFrom',
    MovementDatePeriodType: '@intBedsMovementDatePeriodType',
    MovementDateTo: '@dtBedsMovementDateTo',
    ProviderID: '@intProviderID',
    ServiceUserID: '@intServiceUserID',
    Reasons: '@xmlReasons',
    ProcessStatuses: '@xmlProcessStatuses',
    ProcessStatusBy: '@xmlProcessStatusBy',
    ProcessSubStatuses: '@xmlProcessSubStatuses',
    AssignedTo: '@intAssignedTo',
    ReceivedDateFrom: '@dtReceivedDateDateFrom',
    ReceivedDatePeriodType: '@intReceivedDatePeriodType',
    ReceivedDateTo: '@dtReceivedDateDateTo',
    ProcessStatusDateFrom: '@dtProcessStatusDateDateFrom',
    ProcessStatusDatePeriodType: '@intProcessStatusDatePeriodType',
    ProcessStatusDateTo: '@dtProcessStatusDateDateTo'
};

Selectors.MovementProcessStatuses = {
    Pending: 1,
    Approved: 2,
    ApprovedSentToBeds: 3,
    Rejected: 4,
    RejectedSentToBeds: 5,
    FailedTransmission: 6
};

Selectors.MovementRequestSelector = function (initSettings) {

    var me = this;
    var selectorType = 21;

    // set the type to MovementRequest always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // construct the value of the warnings column
    me.RendererWarnings = function (val, md, rcd) {
        var warnings = '', cellHtml = '', cellCssClass = '', cellToolTip = '', isSvcUsrFound = true;
        if (!rcd.raw.IsBedsServiceUserRefMatched) {
            warnings += '* BEDS Service User Ref does not match an ABACUS Service User.<br /><br />';
            isSvcUsrFound = false;
        }
        if (isSvcUsrFound) {
            if (!rcd.raw.IsBedsServiceUserSurnameMatched) {
                warnings += '* BEDS Surname does not match ABACUS Service User Surname.<br /><br />';
            }
            if (!rcd.raw.IsBedsServiceUserDateOfBirthMatched) {
                warnings += '* BEDS Date of Birth does not match ABACUS Service User Date of Birth.<br /><br />';
            }
        }
        if (warnings != '') {
            cellCssClass = 'imgExclamation';
            cellToolTip = '<b>Warnings</b><br /><br />' + warnings + ''
        }
        cellHtml = '<div class=\'Selectors ' + cellCssClass + '\' style=\'height: 15px; width: 16px;\' data-qtip=\'' + cellToolTip + '\' />';
        return cellHtml;
    }

    // set the assigned to
    me.SetAssignedToID = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.AssignedTo, Selectors.Helpers.ToInt(value));
    }

    // set the movement date from
    me.SetMovementDateFrom = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.MovementDateFrom, value);
    }

    // set the movement date period type
    me.SetMovementDatePeriodType = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.MovementDatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the movement date to
    me.SetMovementDateTo = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.MovementDateTo, value);
    }

    // set the provider id
    me.SetProviderID = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    // set the process status by
    me.SetProcessStatusesBy = function (types) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ProcessStatusBy, types);
    }

    // set the process statuses
    me.SetProcessStatuses = function (types) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ProcessStatuses, types);
    }

    // set the process sub statuses
    me.SetProcessSubStatuses = function (types) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ProcessSubStatuses, types);
    }

    // set the reasons
    me.SetReasons = function (types) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.Reasons, types);
    }

    // set the received date from
    me.SetReceivedDateFrom = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ReceivedDateFrom, value);
    }

    // set the received date period type
    me.SetReceivedDatePeriodType = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ReceivedDatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the received date to
    me.SetReceivedDateTo = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ReceivedDateTo, value);
    }

    // set the svc user id
    me.SetServiceUserID = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ServiceUserID, Selectors.Helpers.ToInt(value));
    }

    // set the process status date from
    me.SetProcessStatusDateFrom = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ProcessStatusDateFrom, value);
    }

    // set the process status date period type
    me.SetProcessStatusDatePeriodType = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ProcessStatusDatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the process status date to
    me.SetProcessStatusDateTo = function (value) {
        me.AddParameter(Selectors.MovementRequestSelectorParameters.ProcessStatusDateTo, value);
    }

}

Selectors.MovementRequestSelector.prototype = new Selectors.SelectorControl();

