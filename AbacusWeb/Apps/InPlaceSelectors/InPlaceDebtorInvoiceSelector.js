
var InPlaceDebtorInvoiceSelector_currentID;

function InPlaceDebtorInvoiceSelector_btnFind_Click(id) {
	var txtName, hidID, txtRef;
	
	InPlaceDebtorInvoiceSelector_currentID = id;
	
    txtName = GetElement(id + "_txtName");
    hidID = GetElement(id + "_hidID");
    txtRef = GetElement(id + "_txtRef");
    	
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/DebtorInvoices.aspx?&id=" + hidID.value;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceDebtorInvoice_ItemSelected(id, name, ref) {
    var txtName, hidID, txtRef;

    txtName = GetElement(InPlaceDebtorInvoiceSelector_currentID + "_txtName");
    hidID = GetElement(InPlaceDebtorInvoiceSelector_currentID + "_hidID");
    txtRef = GetElement(InPlaceDebtorInvoiceSelector_currentID + "_txtRef");

	hidID.value = id;
	txtName.value = name;
	txtRef.value = ref;
	
	if(typeof(InPlaceDebtorInvoice_Changed) == "function") 
	{
	    InPlaceDebtorInvoice_Changed(hidID.value);
    }
}
function InPlaceDebtorInvoiceSelector_ClearStoredID(id) {
	var hidID;
	hidID = GetElement(id + "_hidID");
	hidID.value = "";
	
	if(typeof(InPlaceDebtorInvoice_Changed) == "function") 
	{
	    InPlaceDebtorInvoice_Changed(hidID.value);
    }	
}