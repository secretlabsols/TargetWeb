// enum for Commitment Reporting Management parameters
Selectors.CommitmentReportingManagementSelectorParameters = 
{
    BudgetYearId: "@BudgetYearId",
    BudgetPeriodId: "@BudgetPeriodId",
    Period: "@Period",
    ReportStatusId: "@ReportStatusId"
};

Selectors.CommitmentReportingManagementSelector = function (initSettings) {

    var me = this;
    var selectorType = 44;

    // set the type to CommitmentReportingManagement always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    me.SetCommitmentReportId = function (value) {
    }

    me.SetBudgetYearId = function (value) {
        me.AddParameter(Selectors.CommitmentReportingManagementSelectorParameters.BudgetYearId, value);
    }

    me.SetBudgetPeriodId = function (value) {
        me.AddParameter(Selectors.CommitmentReportingManagementSelectorParameters.BudgetPeriodId, value);
    }

    me.SetPeriod = function (value) {
        me.AddParameter(Selectors.CommitmentReportingManagementSelectorParameters.Period, value);
    }

    me.SetReportStatusId = function (value) {
        me.AddParameter(Selectors.CommitmentReportingManagementSelectorParameters.ReportStatusId, value);
    }

    me.SetIsLockedForRetention = function (value) {
        me.AddParameter(Selectors.CommitmentReportingManagementSelectorParameters.IsLockedForRetention, value);
    }

    me.GetCommitmentReportId = function (value) {
        return 0; // me.extSettings.response.GetSelectedID();
    }

    me.GetBudgetYearId = function (value) {
        return me.GetParameterValue(Selectors.CommitmentReportingManagementSelectorParameters.BudgetYearId);
    }

    me.GetBudgetPeriodId = function (value) {
        return me.GetParameterValue(Selectors.CommitmentReportingManagementSelectorParameters.BudgetPeriodId);
    }

    me.GetPeriod = function (value) {
        return me.GetParameterValue(Selectors.CommitmentReportingManagementSelectorParameters.Period);
    }

    me.GetReportStatusId = function (value) {
        return me.GetParameterValue(Selectors.CommitmentReportingManagementSelectorParameters.ReportStatusId);
    }

    me.GetIsLockedForRetention = function (value) {
        return me.GetIsLockedForRetention(Selectors.CommitmentReportingManagementSelectorParameters.IsLockedForRetention);
    }

    me.GetWebServiceParameters = function () {
        var params = {
            BudgetYearId: me.GetBudgetYearId(),
            BudgetPeriodId: me.GetBudgetPeriodId(),
            Period: me.GetPeriod(),
            ReportStatusId: me.GetReportStatusId()
        };

        return params
    }
}

Selectors.CommitmentReportingManagementSelector.prototype = new Selectors.SelectorControl();

