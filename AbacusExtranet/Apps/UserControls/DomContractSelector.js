
var contractSvc, currentPage, establishmentID, contractType, contractGroupID, dateFrom, dateTo, btnViewID, isAsInPlaceContractor;
var DomContractSelector_selectedContractID, DomContractStep_required;
var listFilter, listFilterNumber = "", listFilterTitle = "";
var tblContracts, divPagingLinks, btnView, btnNext, btnFinish;
var DomContractSelector_CurrentItems = [];

function Init() {
	contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
	tblContracts = GetElement("tblContracts");
	divPagingLinks = GetElement("DomContract_PagingLinks");
	btnView = GetElement(btnViewID, true);
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	
	if(btnView) btnView.disabled = true;
	if(btnNext) btnNext.disabled = DomContractStep_required;
	if(btnFinish) btnFinish.disabled = DomContractStep_required;
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Number", GetElement("thNumber"));
	listFilter.AddColumn("Title", GetElement("thTitle"));
		
	// populate table
	FetchDomContractList(currentPage, DomContractSelector_selectedContractID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Contract No.":
			listFilterNumber = column.Filter;
			break;
		case "Contract Title":
			listFilterTitle = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchDomContractList(1, 0);
}

function FetchDomContractList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if (selectedID == undefined) selectedID = 0;

	contractSvc.FetchDomContractList(page, selectedID, establishmentID, contractType, contractGroupID, dateFrom, dateTo, listFilterNumber, listFilterTitle, FetchDomContractList_Callback)
}

function FetchDomContractList_Callback(response) {
	var contract, index;
	var tr, td, radioButton;
	var str;
	var link;
	
	if(DomContractSelector_selectedContractID == 0) {
		if(btnView) btnView.disabled = true;
	}
	
	if(CheckAjaxResponse(response, contractSvc.url)) {
		
		// populate the table
		contracts = response.value.Contracts;
		DomContractSelector_CurrentItems = contracts;
		
		// remove existing rows
		ClearTable(tblContracts);
		for(index=0; index<contracts.length; index++) {
		
			tr = AddRow(tblContracts);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "DomContractSelect", contracts[index].ID, RadioButton_Click);
			
			td = AddCell(tr, "");
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			str = contracts[index].Number;
			if(btnView) {
			    link = AddLink(td, str, "javascript:DomContractSelector_selectedContractID=" + contracts[index].ID + ";btnView_Click();", 
			        "Click here to view this contract");
			    link.className = "transBg";
			} else {
			    SetInnerText(td, str);	
            }
		
			td = AddCell(tr, "");
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			str = contracts[index].Title;
			if(btnView) {
			    link = AddLink(td, str, "javascript:DomContractSelector_selectedContractID=" + contracts[index].ID + ";btnView_Click();", 
			        "Click here to view this contract");
                link.className = "transBg";
			} else {
			    SetInnerText(td, str);
            }

            td = AddCell(tr, "");
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";
            SetInnerText(td, contracts[index].ProviderName);

            td = AddCell(tr, "");
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";
            SetInnerText(td, contracts[index].ContractType);

            td = AddCell(tr, "");
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";
            
            str = contracts[index].DcrReference;
            if (!str || str.length == 0) str = "None";
            SetInnerText(td, str);
		
			AddCell(tr, Date.strftime("%d/%m/%Y", contracts[index].StartDate));

			AddCell(tr, Date.strftime("%d/%m/%Y", contracts[index].EndDate));
			
			// select the contract?
			if(DomContractSelector_selectedContractID == contracts[index].ID || ( currentPage == 1 && contracts.length == 1)) {
				radioButton.click();
			}
			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
function RadioButton_Click() {
	var index, rdo, selectedRow, selectedItem;
	
	for (index = 0; index < tblContracts.tBodies[0].rows.length; index++){
		rdo = tblContracts.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblContracts.tBodies[0].rows[index];
			tblContracts.tBodies[0].rows[index].className = "highlightedRow"
			DomContractSelector_selectedContractID = rdo.value;
			if(btnView) btnView.disabled = false;
			if(btnNext) btnNext.disabled = false;
			if (btnFinish) btnFinish.disabled = false;
			selectedItem = DomContractSelector_CurrentItems[index];
		} else {
			tblContracts.tBodies[0].rows[index].className = ""
		}
	}
	if(typeof DomContractSelector_SelectedItemChange == "function") {
		var args = new Array(3);
		args[0] = DomContractSelector_selectedContractID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);
		args[3] = selectedItem; 
		DomContractSelector_SelectedItemChange(args);
	}
}
function btnView_Click() {
	document.location.href = SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/Dom/Contracts/ViewDomiciliaryContract.aspx?id=" + DomContractSelector_selectedContractID + "&backUrl=" + GetBackUrl();
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

addEvent(window, "load", Init);
