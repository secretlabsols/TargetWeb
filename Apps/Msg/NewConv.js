
var selectRecipientParams;

function Init() {
	selectRecipientParams = new Target.Web.Apps.Msg.RecipientCollection();
}
		
function SelectRecipients_GetParams() {
	return selectRecipientParams;
}
function SelectRecipients_Changed(params) {
	selectRecipientParams = params
	GetElement("spnRecipients").innerHTML = selectRecipientParams.GetRecipientList();
	GetElement("txtRecipients").value = selectRecipientParams.Serialize();
}

function btnTo_OnClick() {			
	OpenDialog("ModalDialogWrapper.axd?SelectRecipients.aspx", 47, 30, window);
}
function btnCancel_OnClick() {
	var url = unescape(GetQSParam(document.location.search, "backUrl"));
	if(url == "null") {
		history.go(-1);
	} else {
		document.location.href = url;
	}
}
function btnSend_OnClick(evt) {
	
	var results = "";
	var clientValidate = false;
	
	// Page_ClientValidate() is IE only - problem with firefox in that if user chooses a file but doesn't fill in 
	// the other fields, the file will be submitted to SlickUpload during the round-trip to do the server-side
	// validation.  Hence, we need custom validation instead.
	if(typeof(Page_ClientValidate) == "function") {
		clientValidate = Page_ClientValidate();
	} else {
		clientValidate = true;
		var subject = GetElement("txtSubject_txtTextBox");
		var message = GetElement("txtMessage_txtTextBox");
		if(subject.value.trim().length == 0) results += "Please enter the subject.\n";
		if(message.value.trim().length == 0) results += "Please enter the message.\n";
	}
	var recipients = GetElement("txtRecipients");
	if(recipients.value.trim().length == 0) results += "Please select at least one recipient.\n";
	if(results.length > 0) {
		alert(results);
	} else if (clientValidate) {
		Go();
	}
}
function Go() {
	if(FileUploader_SelectedFiles > 0) {
		OpenFileUploadProgress(FileUploader_UploadID);
		window.setTimeout('document.forms[0].submit();', 1000);
	} else {
		document.forms[0].submit();
	}
}

addEvent(window, "load", Init);
