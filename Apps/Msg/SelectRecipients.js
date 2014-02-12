
var securitySvc, msgSvc, srcList, destList, cboSource, originalParams, title, msgPassingType;
var srcListID = "dlRecipients_lstSrc";
var destListID = "dlRecipients_lstDest";
var cboSourceID = "cboSource";
var titleID = "title";
var parentWindow = GetParentWindow();

function Init() {

	addEvent(window, "unload", DialogUnload);

	var selectedCounter, opt, showDistList;
	securitySvc = new Target.Web.Apps.Security.WebSvc.Security_class();
	msgSvc = new Target.Web.Apps.Msg.WebSvc.Messaging_class();
	srcList = GetElement(srcListID);
	destList = GetElement(destListID);
	cboSource = GetElement(cboSourceID);
	title = GetElement(titleID);
	params = parentWindow.SelectRecipients_GetParams();
	
	// override the default overview text?
	if(typeof(parentWindow.SelectRecipients_OverviewText) == "function") {
		title.innerHTML = parentWindow.SelectRecipients_OverviewText();
	}
	// load source list
	opt = new Option("", 0);
	cboSource.options.add(opt);
	if(typeof(parentWindow.SelectRecipients_ShowDistList) == "function") {
		showDistList = parentWindow.SelectRecipients_ShowDistList();
	} else {
		showDistList= true;
	}
	if(showDistList) {
		opt = new Option("Distributions Lists", 3);
		cboSource.options.add(opt);
	}
	if(msgPassingType == 1) {
		opt = new Option("Users", 1);
		cboSource.options.add(opt);
	}
	if(msgPassingType == 2) {
		opt = new Option("Companies", 2);
		cboSource.options.add(opt);
	}
	// populate the dest list with any passed in items
	for(selectedCounter=0; selectedCounter<params.Recipients.length; selectedCounter++) {
		opt = new Option(params.Recipients[selectedCounter].Name, params.Recipients[selectedCounter].ID);
		opt.setAttribute("tag", params.Recipients[selectedCounter].Type);
		destList.options[destList.options.length] = opt
	}
}
function cboSource_OnChange() {
	switch(cboSource.value) {
		case "0":
			srcList.options.length = 0;
			break;
		case "1":
			FetchUserList();
			break;
		case "2":
			FetchCompanyList();
			break;
		case "3":
			FetchDistributionLists();
			break;
	}
}
function btnOK_OnClick() {
	var selectedCounter, r;
	
	// check we have selected some recipients
	if(destList.options.length == 0) {
		alert("Please select at least one recipient.");
		return;
	}
	
	// load the params with the selected recipients
	params.ClearRecipients();
	for(selectedCounter=0; selectedCounter<destList.options.length; selectedCounter++) {
		r = new Target.Web.Apps.Msg.Recipient();
		r.Type = destList.options[selectedCounter].getAttribute("tag");
		r.ID = destList.options[selectedCounter].value;
		r.Name = destList.options[selectedCounter].text;
		params.AddRecipient(r);
	}
	// send the params back to the caller
	parentWindow.SelectRecipients_Changed(params);
	CloseWindow();
}
function btnCancel_OnClick() {
	CloseWindow();	
}
function CloseWindow() {
	parentWindow.HideModalDIV();
	window.parent.close();
}

/* USER LIST METHODS */
function FetchUserList() {
	DisplayLoading(true);
	securitySvc.FetchSimpleUserList(0, FetchUserList_Callback)
}
function FetchUserList_Callback(response) {
	if(CheckAjaxResponse(response, securitySvc.url)) {
		RePopulateLists(response.value.List);
	}
	DisplayLoading(false);
}

/* COMPANY LIST METHODS */
function FetchCompanyList() {
	DisplayLoading(true);
	securitySvc.FetchSimpleCompanyList(FetchCompanyList_Callback)
}
function FetchCompanyList_Callback(response) {
	if(CheckAjaxResponse(response, securitySvc.url)) {
		RePopulateLists(response.value.List);
	}
	DisplayLoading(false);
}

/* FETCH ALL DIST LISTS */
function FetchDistributionLists() {
	DisplayLoading(true);
	msgSvc.FetchDistributionLists(FetchDistributionLists_Callback);
}
function FetchDistributionLists_Callback(response) {
	var opt, listCounter, distLists;
	if(CheckAjaxResponse(response, msgSvc.url)) {
		RePopulateLists(response.value.List);
	}
	DisplayLoading(false);
}

function RePopulateLists(items) {
	var keyCounter, selectedCounter;
	var keys = items.getKeys();
	var deletedKeys = new Array();
	var currentType = parseInt(cboSource.value, 10);
		
	// clear src list
	srcList.options.length = 0;
		
	// filter the new source list based on what we already have selected
	for(selectedCounter=0; selectedCounter<destList.options.length; selectedCounter++) {
		for(keyCounter=0; keyCounter<keys.length; keyCounter++) {
			// if the ID and Type match then we already have the item selected so we remove it from the new source list
			var newItemID = parseInt(items.getValue(keys[keyCounter]), 10);
			var existingItemID = parseInt(destList.options[selectedCounter].value, 10);
			var existingType = parseInt(destList.options[selectedCounter].getAttribute("tag"), 10);
			if (newItemID == existingItemID && existingType == currentType) {
				deletedKeys[deletedKeys.length] = newItemID;
			}
		}
	}
	
	// loop through response and create items in src list
	for(i=0; i<keys.length; i++) {
		var add = true;
		var deletedCounter;
		for(deletedCounter=0; deletedCounter<deletedKeys.length; deletedCounter++) {
			if(items.getValue(keys[i]) == deletedKeys[deletedCounter]) {
				add = false;
				break;
			}
		}
		if(add) srcList.options[srcList.options.length] = new Option(keys[i], items.getValue(keys[i]));
	}
}
function DualList_ItemAdded(newItem) {
	// store the type
	newItem.setAttribute("tag", cboSource.value);
}
function DualList_ItemRemoved(newItem) {
	// if the removed item is not the same type as the items currently in the available list, delete it from the available list
	var currentType = parseInt(cboSource.value, 10);
	var itemType = parseInt(newItem.getAttribute("tag"), 10);
	if(itemType != currentType) {
		srcList.remove(newItem.index);
	}
}

addEvent(window, "load", Init);
