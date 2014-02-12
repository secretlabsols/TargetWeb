var DateRange_btnViewID, DateRange_btnView;
var optShowAll, optPermResOnly, optExcludePermRes;
var dateFromValue, dateToValue, dateToChanged, dateFromChanged;
function Init() {
    DateRange_btnView = GetElement(DateRange_btnViewID + '_btnReports', true);
    DateRange_btnView.disabled = true;
    optShowAll = GetElement("optShowAll");
    optPermResOnly = GetElement("optPermResOnly");
    optExcludePermRes = GetElement("optExcludePermRes");
    optShowAll.checked = true;

    ReportsButton_AddParam(DateRange_btnView.id, "DateFrom", dateFromValue);
    ReportsButton_AddParam(DateRange_btnView.id, "DateTo", dateToValue);
    DateRange_btnView.disabled = false;
}

function dteDateFrom_Changed(id) {
    var dteDateFrom = GetElement(id + "_txtTextBox");

    dateFromChanged = true;
    dateFromValue = dteDateFrom.value.toDate();
    var dateToValueLimit = new Date(dateFromValue.getYear() + 1, dateFromValue.getMonth(), dateFromValue.getDate());

    ReportsButton_AddParam(DateRange_btnView.id, "DateFrom", dteDateFrom.value);

    //validate the date and make sure the range is not more than a year
    if (dateToChanged == true) {
        if (dateToValue > dateToValueLimit) {
            DateRange_btnView.disabled = true;
            alert("The date range must not exceed 1 year.");
        } else {
            DateRange_btnView.disabled = false;
        }
    }

}

function dteDateTo_Changed(id) {
    var dteDateTo = GetElement(id + "_txtTextBox");
    var dateToValueLimit = new Date(dateFromValue.getYear() + 1, dateFromValue.getMonth(), dateFromValue.getDate());

    dateToChanged = true;
    dateToValue = dteDateTo.value.toDate();

    ReportsButton_AddParam(DateRange_btnView.id, "DateTo", dteDateTo.value);

    //validate the date and make sure the range is not more than a year
    if (dateFromChanged == true) {
        if (dateToValue > dateToValueLimit) {
            DateRange_btnView.disabled = true;
            alert("The date range must not exceed 1 year.");
        } else {
            DateRange_btnView.disabled = false;
        }
    }

}

function optType_Click() {
    if (optShowAll.checked == true) {
        ReportsButton_RemoveParam(DateRange_btnView.id, "ShowPermRes");
    } else if (optPermResOnly.checked == true) {
        ReportsButton_AddParam(DateRange_btnView.id, "ShowPermRes", true);
    } else if (optExcludePermRes.checked == true) {
        ReportsButton_AddParam(DateRange_btnView.id, "ShowPermRes", false);
    }
}


addEvent(window, "load", Init);