
var msgSvc, selectRecipientParams;;
var cboDistListsID = "cboDistLists";
var txtNameID = "txtName_txtTextBox";
var spnRecipientsID = "spnRecipients";
var txtRecipientsID = "txtRecipients";
var txtRecipientNamesID = "txtRecipientNames";
var btnDeleteID = "btnDelete";

var cboDistLists, txtName, spnRecipients, txtRecipients, txtRecipientNames;
var originalName = "", originalList = "", currentListIndex = 0;
var onLoadDistListID = 0;

function Init() {
	distListID = 0;
	msgSvc = new Target.Web.Apps.Msg.WebSvc.Messaging_class();
	selectRecipientParams = new Target.Web.Apps.Msg.RecipientCollection();
	cboDistLists = GetElement(cboDistListsID);
	txtName = GetElement(txtNameID);
	spnRecipients = GetElement(spnRecipientsID);
	txtRecipients = GetElement(txtRecipientsID);
	txtRecipientNames = GetElement(txtRecipientNamesID);
	btnDelete = GetElement(btnDeleteID);
	// load the dropdown of dist lists
	FetchDistributionLists();
	// if we already have recipients in the list, reload the UI
	if(txtRecipientNames.value.length > 0)
		spnRecipients.innerHTML = txtRecipientNames.value;
}

function cboDistLists_OnChange() {
	if(ConfirmChange()) {
		// fetch the selected list
		var listID = parseInt(cboDistLists.value, 10);
		if(listID > 0) {
			FetchSingleDistList(cboDistLists.value);
		} else {
			DisplayList(0, "", null);
		}
		currentListIndex = cboDistLists.selectedIndex;
	} else {
		cboDistLists.selectedIndex = currentListIndex;
	}	
}
function btnNew_OnClick() {
	cboDistLists.selectedIndex = 0;
	cboDistLists_OnChange();
}
function btnRecipients_OnClick() {			
	OpenDialog("ModalDialogWrapper.axd?../SelectRecipients.aspx", 47, 30, window);
}
function btnDelete_OnClick() {
	if(window.confirm("Are you sure you wish to delete this distribution list?")) {
		document.location.href = AddQSParam(RemoveQSParam("ManageDistLists.aspx", "deleteID"), "deleteID", cboDistLists.value);
	}
}

function DisplayList(id, name, members) {
	var memberCounter, recipient;
	// load the name
	txtName.value = name;
	originalName = txtName.value;
	// load the members into the recipients collection
	selectRecipientParams = new Target.Web.Apps.Msg.RecipientCollection();
	if(members != null) {
		for(memberCounter=0; memberCounter<members.length; memberCounter++) {
			recipient = new Target.Web.Apps.Msg.Recipient();
			recipient.Type = members[memberCounter].Type;
			recipient.ID = members[memberCounter].ID;
			recipient.Name = members[memberCounter].Name;
			selectRecipientParams.AddRecipient(recipient);
		}
		spnRecipients.innerHTML = selectRecipientParams.GetRecipientList();
		txtRecipients.value = selectRecipientParams.Serialize();
		originalList = txtRecipients.value;
	} else {
		spnRecipients.innerHTML = "";
	}
	btnDelete.disabled = (id <= 0);
}
function ConfirmChange() {
	if(HasChanged()) {
		if(window.confirm("You have made changes to this distribution list. Do you wish to continue and lose these changes?"))
			return true;
		else
			return false;
	} else {
		return true;
	}
}
function HasChanged() {
	if(txtName.value != originalName || txtRecipients.value != originalList) {
		return true;
	} else {
		return false;
	}
}

/* SELECT RECIPIENT METHODS */
function SelectRecipients_GetParams() {
	return selectRecipientParams;
}
function SelectRecipients_Changed(params) {
	selectRecipientParams = params
	spnRecipients.innerHTML = selectRecipientParams.GetRecipientList();
	txtRecipientNames.value = spnRecipients.innerHTML;
	txtRecipients.value = selectRecipientParams.Serialize();
}
function SelectRecipients_ShowDistList() {
	return false;
}
function SelectRecipients_OverviewText() {
	return "Please select who you wish to be part of this distribution list.";
}

/* FETCH SINGLE DIST LIST */
function FetchSingleDistList(distListID) {
	DisplayLoading(true);
	msgSvc.FetchSingleDistributionList(distListID, FetchSingleDistList_Callback);
}
function FetchSingleDistList_Callback(response) {
	var list;
	if(CheckAjaxResponse(response, msgSvc.url)) {
		list = response.value;
		DisplayList(list.ID, list.Name, list.Members);
	}
	DisplayLoading(false);
}

/* FETCH ALL DIST LISTS */
function FetchDistributionLists() {
	DisplayLoading(true);
	msgSvc.FetchDistributionLists(FetchDistributionLists_Callback);
}
function FetchDistributionLists_Callback(response) {
	var opt, item, keys;
	if(CheckAjaxResponse(response, msgSvc.url)) {
		// clear the dropdown and add a blank item
		cboDistLists.options.length = 0;
		opt = new Option("", 0);
		cboDistLists.options.add(opt);
		// add the returned dist lists
		items = response.value.List;
		keys = items.getKeys();
		for(keyCounter=0; keyCounter<keys.length; keyCounter++) {
			opt = new Option(keys[keyCounter], items.getValue(keys[keyCounter]));
			cboDistLists.options.add(opt);
		}
	}
	DisplayLoading(false);
	
	// auto load a dist list?
	if(onLoadDistListID > 0) {
		cboDistLists.value = onLoadDistListID;
		FetchSingleDistList(cboDistLists.value);
	}
}

addEvent(window, "load", Init);
