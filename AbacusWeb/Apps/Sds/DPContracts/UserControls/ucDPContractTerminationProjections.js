var DpContractProjectedTerminations = {};
var DpContractProjectedTerminationsUpdateManager;

var FormatHelpers = {
    formatCurrency: function(val) {
        val = parseFloat(val);
        return String((isNaN(val) ? 0.00 : val).toFixed(2)).formatCurrency()
    },
    formatDate: function(val) {
        if (val) {
            return Date.strftime("%d/%m/%Y", val);
        } else {
            return '&nbsp;';
        }
    }
};

DpContractProjectedTerminations.Updater = function(args) {

    var cellTemplateWeeksPaid = '{{if DPContractDetailFrequency === 0 }} N/A - {{if !DPContractDetailPaidUpTo }}Not {{/if}} Paid {{else DPContractDetailWeeksPaidUnderOver == 0 }} Paid in full {{else DPContractDetailWeeksPaidUnderOver < 0 }} ${DPContractDetailWeeksPaidUnderOver * -1} week(s) under {{else DPContractDetailWeeksPaidUnderOver > 0 }} ${DPContractDetailWeeksPaidUnderOver} week(s) over {{/if}}';    
    var cellTemplateWeeksPaidName = 'DpContractProjectedTerminations.Updater.Cell.WeeksPaidUnderOver';
    var lastTerminations = [];
    var rowTemplate = '<tr><td>${FormatHelpers.formatDate(DPContractDetailDateFrom)}</td><td>${FormatHelpers.formatCurrency(DPContractDetailAmount)}</td><td>${DPContractDetailFrequencyDescription}</td><td>${FormatHelpers.formatCurrency(DPContractDetailWeeklyAmount)}</td><td>${FormatHelpers.formatDate(DPContractDetailPaidUpTo)}</td><td>${FormatHelpers.formatDate(DPContractDetailProjectedEndDate)}</td><td>{{tmpl "' + cellTemplateWeeksPaidName + '"}}</td><td>${FormatHelpers.formatCurrency(DPContractDetailWeeklyPaidUnderOverAmount)}</td></tr>';
    var rowTemplateName = 'DpContractProjectedTerminations.Updater.Row';
    var settings = {
        service: null,
        table: null
    };

    Init(args);

    function Init(args) {
        if (args) {
            $.extend(settings, args);
        }
        $.template(rowTemplateName, rowTemplate);
        $.template(cellTemplateWeeksPaidName, cellTemplateWeeksPaid);
    }


    var Update = function(args) {
        DisplayLoading(true);
        var updateSettings = {
            dpContractID: 0,
            dpContractEndDate: null
        };
        if (args) {
            $.extend(updateSettings, args);
        }
        if (!settings.service) {
            alert('Please specify the \'service\' on init.');
            return false;
        }
        if (!updateSettings.dpContractID > 0) {
            alert('Please specify the \'dpContractID\' on update.');
            return false;
        }
        if (!updateSettings.dpContractEndDate) {
            alert('Please specify the \'dpContractEndDate\' on update.');
            return false;
        }
        lastTerminations = [];
        settings.service.GetProjectedTerminationDetailsForContract(updateSettings.dpContractID, args.dpContractEndDate, UpdateCallBack);
    };

    var UpdateCallBack = function(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, settings.service.url)) {
            var tableBody = $('table > tbody');
            tableBody.empty();
            lastTerminations = serviceResponse.value.Items;
            $.tmpl(rowTemplateName, lastTerminations).appendTo(tableBody);
        }
        DisplayLoading(false);
    };

    return {
        Update: function(args) {
            Update(args);
        },
        GetItems: function() {
            return lastTerminations;
        },
        GetItemsBalancingAmount: function() {
            var amount = 0;
            $.each(this.GetItems(), function(itemIdx, itemVal) {
                amount += itemVal.DPContractDetailWeeklyPaidUnderOverAmount.toFixed(2);
            });
            return parseFloat(amount).toFixed(2);
        }
    }
}
