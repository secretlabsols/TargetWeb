
var contractSvc, currentPage, establishmentID, contractType, contractGroupID, dateFrom, dateTo, contractEndReasonID, serviceGroupID, serviceGroupClassificationID, frameworkTypeID;
var DomContractSelector_selectedContractID, DomContractSelector_btnViewID, DomContractSelector_btnCopyID;
var DomContractSelector_btnTerminateID, DomContractSelector_btnReinstateID;
var DomContractStep_required, DomContractSelector_showServiceUserColumn;
var listFilter, listFilterNumber = "", listFilterTitle = "", listFilterSU = "";
var listFilterGroup = "";
var tblContracts, divPagingLinks, btnView, btnCopy, btnNext, btnFinish, btnTerminate, btnReinstate;


function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	tblContracts = GetElement("tblContracts");
	divPagingLinks = GetElement("DomContract_PagingLinks");
	btnView = GetElement(DomContractSelector_btnViewID, true);
	btnCopy = GetElement(DomContractSelector_btnCopyID, true);
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	btnTerminate = GetElement(DomContractSelector_btnTerminateID, true);
	btnReinstate = GetElement(DomContractSelector_btnReinstateID, true);
	
	if(btnView) btnView.disabled = true;
	if(btnCopy) btnCopy.disabled = true;
	if(btnTerminate) btnTerminate.disabled = true;
	if(btnReinstate) btnReinstate.disabled = true;
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Number", GetElement("thNumber"));
	listFilter.AddColumn("Title", GetElement("thTitle"));
	if(DomContractSelector_showServiceUserColumn) listFilter.AddColumn("Service User", GetElement("thServiceUser"));
	listFilter.AddColumn("Group", GetElement("thGroup"));
		
	// populate table
	FetchDomContractList(currentPage, DomContractSelector_selectedContractID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Number":
			listFilterNumber = column.Filter;
			break;
		case "Title":
			listFilterTitle = column.Filter;
			break;
		case "Service User":
			listFilterSU = column.Filter;
			break;
		case "Group":
			listFilterGroup = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchDomContractList(1);
}

function FetchDomContractList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	contractSvc.FetchDomContractList(page, selectedID, establishmentID, contractType, contractGroupID, dateFrom, dateTo, listFilterNumber, listFilterTitle, listFilterSU, listFilterGroup, contractEndReasonID, serviceGroupID, serviceGroupClassificationID, frameworkTypeID, FetchDomContractList_Callback)
}

function FetchDomContractList_Callback(response) {
	var contract, index;
	var tr, td, radioButton;
	var str;
	var link;
	
	// disable next/finish buttons by default if the step is mandatory
	if(DomContractStep_required) {
		if(btnNext) btnNext.disabled = true;
		if(btnFinish) btnFinish.disabled = true;
    }
    if (btnView) btnView.disabled = true;
    if (btnCopy) btnCopy.disabled = true;
    if (btnTerminate) btnTerminate.disabled = true;
    if (btnReinstate) btnReinstate.disabled = true;
	
	if(CheckAjaxResponse(response, contractSvc.url)) {
		
		// populate the table
		contracts = response.value.Contracts;
		
		// remove existing rows
		ClearTable(tblContracts);
		for(index=0; index<contracts.length; index++) {
		
			tr = AddRow(tblContracts);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "DomContractSelect", contracts[index].ID, RadioButton_Click);
			
			td = AddCell(tr, contracts[index].Number);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			td = AddCell(tr, contracts[index].Title);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, contracts[index].ProviderName);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			AddCell(tr, Date.strftime("%d/%m/%Y", contracts[index].StartDate));

			AddCell(tr, Date.strftime("%d/%m/%Y", contracts[index].EndDate));
			
			td = AddCell(tr, " ");
			if(contracts[index].ContractGroup) SetInnerText(td, contracts[index].ContractGroup);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
//			str = contracts[index].EndReason;
//			if(!str || str.length == 0) str = " ";
//			td = AddCell(tr, str);
//			td.style.textOverflow = "ellipsis";
//			td.style.overflow = "hidden";
			
			if(DomContractSelector_showServiceUserColumn) {
			    str = contracts[index].ClientName;
			    if(!str || str.length == 0) str = " ";
			    AddCell(tr, str);
			}
			
			if(contracts[index].ID == DomContractSelector_selectedContractID || ( currentPage == 1 && contracts.length == 1))
			    radioButton.click();
			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
function RadioButton_Click() {
	var index, rdo, selectedRow;
	
	for (index = 0; index < tblContracts.tBodies[0].rows.length; index++){
		rdo = tblContracts.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblContracts.tBodies[0].rows[index];
			tblContracts.tBodies[0].rows[index].className = "highlightedRow"
			DomContractSelector_selectedContractID = rdo.value;
			if(btnView) btnView.disabled = false;
			if(btnCopy) btnCopy.disabled = false;
			if (GetInnerText(selectedRow.cells[5]) == " ") {
			    if(btnTerminate) btnTerminate.disabled = false; 
			    if(btnReinstate) btnReinstate.disabled = true;
			} else { 
			    if(btnTerminate) btnTerminate.disabled = true;
			    if(btnReinstate) btnReinstate.disabled = false;
			}
		} else {
			tblContracts.tBodies[0].rows[index].className = ""
		}
	}
	if(DomContractStep_required) {
		if(btnNext) btnNext.disabled = false;
		if(btnFinish) btnFinish.disabled = false;
	}
	if(typeof DomContractSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = DomContractSelector_selectedContractID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);
		DomContractSelector_SelectedItemChange(args);
	}
}
function btnNew_Click() {
	var response, serviceGroupID;
		
	response = contractSvc.NoServiceGroupsAvailableToUser(true).value;
    if(response == 0) {
        alert("You do not have permission to set up a contract for a Service Group.");
    }else if (response == 1) {
        response = contractSvc.FetchOnlyAvailableServiceGroupToUser().value;
        if (response > 0){
            InPlaceServiceGroup_ItemSelected(response, "");
        }
    } else {
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/ServiceGroups.aspx?&fUser=true&fNonBlankFK=true";
        var dialog = OpenDialog(url, 60, 32, window);
    }
	
}

function InPlaceServiceGroup_ItemSelected(id, name) {
	var qs = document.location.search;
	qs = RemoveQSParam(qs, "currentStep");
	qs = RemoveQSParam(qs, "dateFrom");
	qs = RemoveQSParam(qs, "dateTo");
	qs = RemoveQSParam(qs, "svcGroupID");

	document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/Contracts/Edit.aspx" + qs + "&svcGroupID=" + id + "&backUrl=" + GetBackUrl();
}

function btnTerminate_Click() {
	var qs = document.location.search;
	qs = RemoveQSParam(qs, "currentStep");
	qs = RemoveQSParam(qs, "dateFrom");
	qs = RemoveQSParam(qs, "dateTo");
	document.location.href = "Terminate.aspx?id=" + DomContractSelector_selectedContractID + "&backUrl=" + GetBackUrl();
}
function btnReinstate_Click() {
	var qs = document.location.search;
	qs = RemoveQSParam(qs, "currentStep");
	qs = RemoveQSParam(qs, "dateFrom");
	qs = RemoveQSParam(qs, "dateTo");
	document.location.href = "Reinstate.aspx?id=" + DomContractSelector_selectedContractID + "&backUrl=" + GetBackUrl();
}
function btnView_Click() {
	document.location.href = "Edit.aspx?id=" + DomContractSelector_selectedContractID + "&backUrl=" + GetBackUrl();
}
function btnCopy_Click() {
	document.location.href = "Edit.aspx?copyFromID=" + DomContractSelector_selectedContractID + "&backUrl=" + GetBackUrl();
}
function GetBackUrl() {
    var url = document.location.href;
    url = AddQSParam(RemoveQSParam(url, "contractID"), "contractID", DomContractSelector_selectedContractID);
	return escape(url);
}

function DomContractStep_BeforeNavigate() {
	var originalContractID = GetQSParam(document.location.search, "contractID");
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	// contract is required?
	if(DomContractStep_required && DomContractSelector_selectedContractID == 0) {
		alert("Please select a contract.");
		return false;
	}
	
	url = AddQSParam(RemoveQSParam(url, "contractID"), "contractID", DomContractSelector_selectedContractID);
	SelectorWizard_newUrl = url;
	return true;
}

function DomContractSelector_MruOnChange(mruListKey, selectedValue) {
    if(selectedValue.length > 0) {
        var url = document.location.href;
        url = RemoveQSParam(url, "contractID");
        url = AddQSParam(url, "contractID", selectedValue);
        document.location.href = url;
    }
}

addEvent(window, "load", Init);
