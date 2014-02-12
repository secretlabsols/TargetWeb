var JobResultsUrl = 'JobResults.aspx';
var RecreateUrl = 'ReCreate.aspx';
var ReportsUrl = 'Reports.aspx';
var RemittancesUrl = 'Remittances.aspx';

function Init() {

}

function btnRecreateFiles_Click() {

    OpenUrlForSelectedItem(RecreateUrl);

}

function btnReports_Click() {

    OpenUrlForSelectedItem(ReportsUrl);

}

function btnRemittances_Click() {

    OpenUrlForSelectedItem(RemittancesUrl);

}

function GetBackUrl(optSelectedId) {

    var url = document.location.href;
    var selectedId = 0;

    if (optSelectedId) {

        selectedId = optSelectedId;

    } else {
    
        var selectedObject = InterfaceLogSelector_GetSelectedObject(InterfaceLogSelector_SelectedID);

        if (selectedObject) {

            selectedId = selectedObject.ID;
            
        }
        
    }

    url = AddQSParam(RemoveQSParam(url, QsInterfaceLogID), QsInterfaceLogID, selectedId);
    
    return escape(url);

}

function InterfaceLogSelector_JobStatusClicked(selectedItem) {

    OpenUrlForSelectedItem(JobResultsUrl, selectedItem);

}

function InterfaceLogSelector_SelectedItemChanged(selectedItem) {

    ResetButtonAvailability();
    
    if (selectedItem) {

        var disableRecreate = true;
        var disableReport = true;

        if (selectedItem.Entries > 0) {
            disableReport = false;            
            if (selectedItem.JobStatus != 'Queued') {
                disableRecreate = false;
            }
        }

        if (btnRecreateFiles) {
            btnRecreateFiles.disabled = disableRecreate;
        }

        if (btnReports) {
            btnReports.disabled = disableReport;
        }
        if (btnRemittances) {
            btnRemittances.disabled = disableReport;
        }
    }

}

function OpenUrlForSelectedItem(url, optSelectedItem) {

    var selectedItem = InterfaceLogSelector_GetSelectedObject(InterfaceLogSelector_SelectedID);

    if (optSelectedItem) {
        selectedItem = optSelectedItem;
    }

    if (selectedItem) {
        url = url + '?bid=' + selectedItem.ID + '&jid=' + selectedItem.JobID + '&backUrl=' + GetBackUrl(selectedItem.ID);
        document.location.href = url;
    } else {
        alert('An item must be selected from the list to complete this action.');
    }
    
}

function ResetButtonAvailability() {

    if (btnRecreateFiles) {
        btnRecreateFiles.disabled = true;
    }

    if (btnReports) {
        btnReports.disabled = true;
    }

    if (btnRemittances) {
        btnRemittances.disabled = true;
    }
}

addEvent(window, "load", Init);

