var contractSvc, providerID, rdbtnSelectExistingID, rdbtnEnterNewCareWorkerID, rdbtnCareWorkerNotSpecifiedID, txtReferenceID, txtNameID, tabindex;
var tblCareWorkers, rdbtnSelectExisting, rdbtnEnterNewCareWorker, rdbtnCareWorkerNotSpecified, btnSelect, btnCancel, txtReference, txtName;
var listFilter, listFilterName = "";
var careWorkerID, careWorkerName, careWorkerReference;
var existingIds;
var editMode;

function btnCancel_Click() {

    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    //if (selectedDcrDomContractId != 0)
    parentWindow.InPlaceCareWorkerSelector_ItemCancelled(tabindex);
    window.parent.close();
}

function btnSelectCareWorker_Click() {
    var isCwSelected = false;
    if (rdbtnCareWorkerNotSpecified.checked) {
        careWorkerID = 0;
        careWorkerName = "Not Specified";
        careWorkerReference = "";
        isCwSelected = true;
    }
    if (rdbtnEnterNewCareWorker.checked) {
        careWorkerID = 0;
        careWorkerName = txtName.value;
        careWorkerReference = txtReference.value;
        if (!isCareWorkerReferenceUnique(careWorkerReference)) {
            alert("Reference number already used.");
            return;
        }
        if (careWorkerName.length == 0) {
            alert("Name is required.");
            return;
        }
        if (careWorkerReference.length == 0) {
            alert("Reference number is required");
            return;
        }
        var response = contractSvc.CreateCareWorker(careWorkerReference, careWorkerName, providerID);
        if (response.value.ErrMsg.Success) {
            careWorkerID = response.value.CareWorkerID;
        }
        else {
            alert(response.value.ErrMsg.Message);
        }
        isCwSelected = true;
    }
    if (rdbtnSelectExisting.checked) {
        var x
        var Radio
        for (x = 0; x < tblCareWorkers.tBodies[0].rows.length; x++) {
            Radio = tblCareWorkers.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
            if (Radio.checked) {
                isCwSelected = true;
                tblCareWorkers.tBodies[0].rows[x].className = "highlightedRow"
                careWorkerID = Radio.value;
                careWorkerName = tblCareWorkers.tBodies[0].rows[x].cells[2].innerHTML;
                careWorkerReference = tblCareWorkers.tBodies[0].rows[x].cells[1].innerHTML;
            }
        }
    }
    if (!isCwSelected) {
        alert("Care worker not selected");
        return;
    }
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    //if (selectedDcrDomContractId != 0)

    parentWindow.InPlaceCareWorkerSelector_ItemSelected(careWorkerID, careWorkerReference, careWorkerName, editMode);
    window.parent.close();
}

function isCareWorkerReferenceUnique(reference) {
    var x;
    var Radio;
    var isReferenceUnique = true;
    reference = reference.toLowerCase();
    for (x = 0; x < tblCareWorkers.tBodies[0].rows.length; x++) {
        Radio = tblCareWorkers.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
        var existingReference = tblCareWorkers.tBodies[0].rows[x].cells[1].innerHTML.toLowerCase();
        if (reference == existingReference) {
            isReferenceUnique = false;
        }
    }
    return isReferenceUnique;
}

function RdbCheckedChanged() {
    if (rdbtnCareWorkerNotSpecified.checked) {
        btnSelect.disabled = false;
    }
    if (rdbtnEnterNewCareWorker.checked) {
        btnSelect.disabled = false;
    }
    if (rdbtnSelectExisting.checked) {
        btnSelect.disabled = false;
    }

}

function Init() {
    contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
    tblCareWorkers = GetElement("tblCareWorkers");
    divPagingLinks = GetElement("CareWorker_PagingLinks");
    rdbtnSelectExisting = GetElement(rdbtnSelectExistingID);
    rdbtnCareWorkerNotSpecified = GetElement(rdbtnCareWorkerNotSpecifiedID);
    rdbtnEnterNewCareWorker = GetElement(rdbtnEnterNewCareWorkerID);

    btnSelect = GetElement("btnSelectContract");
    btnSelect.disabled = true;
    btnCancel = GetElement("btnCancel");
    txtReference = GetElement(txtReferenceID);
    txtName = GetElement(txtNameID);
    // setup list filters
    listFilter = new Target.Web.ListFilter(ListFilter_Callback);
    listFilter.AddColumn("Name", GetElement("thName"));

    // populate table
    FetchCareWorkerListByProviderID()
}

/* FETCH CLIENT LIST METHODS */
function FetchCareWorkerListByProviderID() {
    contractSvc.FetchCareWorkerListByProviderID(providerID, listFilterName,existingIds, FetchCareWorkerListByProviderID_Callback)
}

function ListFilter_Callback(column) {
    switch (column.Name) {
        case "Name":
            listFilterName = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    FetchCareWorkerListByProviderID();
}

function FetchCareWorkerListByProviderID_Callback(response) {
    var careWorkers, index;
    var tr, td, radioButton;
    var str;
    var link;

    if (CheckAjaxResponse(response, contractSvc.url)) {
        // populate the care worker table
        careWorkers = response.value.CareWorkers;
        // remove existing rows
        ClearTable(tblCareWorkers);
        for (index = 0; index < careWorkers.length; index++) {

            tr = AddRow(tblCareWorkers);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "CareWorkerSelect", careWorkers[index].CareWorkerID, RadioButton_Click);


            td = AddCell(tr, careWorkers[index].Reference);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, careWorkers[index].Name);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";
        }
    }

    if (rdbtnCareWorkerNotSpecified.checked) {
        btnSelect.disabled = false;
    }
    
    DisplayLoading(false);
}

function RadioButton_Click() {
    var x
    var Radio
    for (x = 0; x < tblCareWorkers.tBodies[0].rows.length; x++) {
        Radio = tblCareWorkers.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
        if (Radio.checked) {
            tblCareWorkers.tBodies[0].rows[x].className = "highlightedRow"
            careWorkerID = Radio.value;
            careWorkerName = tblCareWorkers.tBodies[0].rows[x].cells[2].innerHTML;
            careWorkerReference = tblCareWorkers.tBodies[0].rows[x].cells[1].innerHTML;
            rdbtnSelectExisting.checked = true;
            btnSelect.disabled = false;
        } else {
            tblCareWorkers.tBodies[0].rows[x].className = ""
        }
    }
}


function CareWorkerStep_BeforeNavigate() {
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
    if (clientID == 0 && careWorkerID == 0) {
        alert("Either a Service User or a Care Worker must be selected.");
        return false;
    } else {
        url = AddQSParam(RemoveQSParam(url, "careWorkerID"), "careWorkerID", careWorkerID);
        SelectorWizard_newUrl = url;
        return true;
    }
}


    
    


addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);