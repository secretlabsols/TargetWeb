
var InPlaceFinanceCodeSelector_currentID;
//var expenditureAccount_SelectedID = 0;

function InPlaceFinanceCodeSelector_btnFind_Click(id) {
	var txtName, hidID, hidCategoryID;
	var hidExpAccID;

	InPlaceFinanceCodeSelector_currentID = id;
	
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	hidCategoryID = GetElement(id + "_hidCategoryID");

	hidExpAccID = GetElement(id + "_hidExpenditureAccountID");

	var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/FinanceCodes.aspx?&id=" + hidID.value + '&catId=' + hidCategoryID.value + '&expAccID=' + hidExpAccID.value;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceFinanceCode_ItemSelected(id, name) {
	var txtName, hidID;
	txtName = GetElement(InPlaceFinanceCodeSelector_currentID + "_txtName");
    hidID = GetElement(InPlaceFinanceCodeSelector_currentID + "_hidID");

	hidID.value = id;
	txtName.value = name;

	if (typeof (InPlaceFinanceCode_Changed) == "function") {
	    InPlaceFinanceCode_Changed({ id: parseInt(id), name: name });
	}
}
function InPlaceFinanceCodeSelector_ClearStoredID(id) {
	var hidID;
	hidID = GetElement(id + "_hidID");
	hidID.value = "";
}

function InPlaceExpenditureAccountSelector_Changed(callersControlID, expAccID) {
    var hidExpAccID;
    $("[id*=" + callersControlID.replace('expenditureAccount', 'financeCode') + "_hidExpenditureAccountID]").val(expAccID);

    if (typeof (FlagControlsToBeRecreated) == "function") {
        FlagControlsToBeRecreated();
    }


}