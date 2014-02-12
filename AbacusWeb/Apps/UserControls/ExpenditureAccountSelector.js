
var financeCodeSvc, currentPage;
var ExpenditureAccountSelector_selectedAccountID, ExpenditureAccountSelector_selectedServiceType, ExpenditureAccountSelector_selectedExpenditureType;
var ExpenditureAccountSelector_cboTypeID, ExpenditureAccountSelector_Description;
var listFilter, listFilterReference = "", listFilterName = "";
var tblExpenditureAccounts, divPagingLinks, btnNext, btnFinish;

function Init() {
    cboExpAcc = GetElement(ExpenditureAccountSelector_cboTypeID + "_cboDropDownList");
    cboExpAcc.value = ExpenditureAccountSelector_selectedExpenditureType;
    
	financeCodeSvc = new Target.Abacus.Web.Apps.WebSvc.FinanceCodes_class();
	tblExpenditureAccounts = GetElement("tblExpenditureAccounts");
	divPagingLinks = GetElement("ExpAccount_PagingLinks");
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Description", GetElement("thDescription"));
	
	// populate table
	FetchAccountList(currentPage, ExpenditureAccountSelector_selectedAccountID, ExpenditureAccountSelector_selectedServiceType, ExpenditureAccountSelector_selectedExpenditureType);

}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Description":
			listFilterName = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchAccountList(1, 0, ExpenditureAccountSelector_selectedServiceType, ExpenditureAccountSelector_selectedExpenditureType);
}

/* FETCH CLIENT LIST METHODS */
function FetchAccountList(page, selectedID, serviceType, accountType) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	if(serviceType == undefined) serviceType = ExpenditureAccountSelector_selectedServiceType;
	if(accountType == undefined) accountType = ExpenditureAccountSelector_selectedExpenditureType;
	
	financeCodeSvc.FetchExpenditureAccountList(page, selectedID, serviceType, accountType, listFilterName, FetchAccountList_Callback)
}

function FetchAccountList_Callback(response) {
	var accounts, accCounter;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, financeCodeSvc.url)) {
	    
		// populate the Expenditure Accounts table
		accounts = response.value.Accounts;

		// remove existing rows
		ClearTable(tblExpenditureAccounts);
		for(accCounter=0; accCounter<accounts.length; accCounter++) {
		
			tr = AddRow(tblExpenditureAccounts);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "AccsSelect", accounts[accCounter].ID, RadioButton_Click);
			
			str = accounts[accCounter].Description;
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
			
			str = accounts[accCounter].Type;
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
						
			// select the Account?
			if(ExpenditureAccountSelector_selectedAccountID == accounts[accCounter].ID || ( currentPage == 1 && accounts.length == 1)) {
				radioButton.click();
			}			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function RadioButton_Click() {
	var index, rdo, selectedRow;
	for (index = 0; index < tblExpenditureAccounts.tBodies[0].rows.length; index++){
		rdo = tblExpenditureAccounts.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblExpenditureAccounts.tBodies[0].rows[index];
			tblExpenditureAccounts.tBodies[0].rows[index].className = "highlightedRow"
			ExpenditureAccountSelector_selectedAccountID = rdo.value;
		} else {
			tblExpenditureAccounts.tBodies[0].rows[index].className = ""
		}
	}

	if(typeof InPlaceExpenditureAccountSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = ExpenditureAccountSelector_selectedAccountID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);
		InPlaceExpenditureAccountSelector_SelectedItemChange(args);
	}
}

function expenditureAccountSelector_cboExpaccountType_Click(){
    cboExpAcc = GetElement(ExpenditureAccountSelector_cboTypeID + "_cboDropDownList");
    ExpenditureAccountSelector_selectedExpenditureType = cboExpAcc.value;
    Init();
}

addEvent(window, "load", Init);
