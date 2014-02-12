
var InPlaceExpenditureAccountSelector_currentID;
var InPlaceExpenditureAccountSelector_serviceType, InPlaceExpenditureAccountSelector_accountType;


function InPlaceExpenditureAccountSelector_btnFind_Click(id) {
	var txtReference, txtName, hidID;

	InPlaceExpenditureAccountSelector_currentID = id;

	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");

    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/ExpenditureAccounts.aspx?description=" + 
		txtName.value + "&aID=" + hidID.value + "&serviceType=" + InPlaceExpenditureAccountSelector_serviceType + "&accountType=" + InPlaceExpenditureAccountSelector_accountType;
    var dialog = OpenDialog(url, 70, 32, window);

}
function InPlaceExpenditureAccountSelector_ItemSelected(id, desc, type) {
	var txtName, hidID, accountType;
	txtName = GetElement(InPlaceExpenditureAccountSelector_currentID + "_txtName");
	hidID = GetElement(InPlaceExpenditureAccountSelector_currentID + "_hidID");
	accountType = GetElement(InPlaceExpenditureAccountSelector_currentID + "_expAccountType");
	
	hidID.value = id;	
	txtName.value = desc;
	accountType.value = accountType;

	if(typeof(InPlaceExpenditureAccountSelector_Changed) == "function") {
	    InPlaceExpenditureAccountSelector_Changed(InPlaceExpenditureAccountSelector_currentID, hidID.value);
	}

	if (typeof (FinanceCodeTable_SetFundedBy) == "function") {
	    var test = document.getElementById(InPlaceExpenditureAccountSelector_currentID + "_txtName");
	    FinanceCodeTable_SetFundedBy(hidID.value, type, test);
	}

}

function InPlaceExpenditureAccountSelector_ClearStoredID(id) {
	var  txtName, hidID;
	
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
	txtName.value = "";
	hidID.value = "";
}

function InPlaceExpenditureAccountSelector_Enabled(id, enable) {
	var txtReference, txtName, hidID, btnFind;
	
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	btnFind = GetElement(id + "_btnFind");
	
	txtName.disabled = !enable;
	hidID.disabled = !enable;
	btnFind.disabled = !enable;
}