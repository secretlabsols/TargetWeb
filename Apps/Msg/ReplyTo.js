
function btnSend_OnClick() {
	var results = "";
	var clientValidate = false;
	
	// Page_ClientValidate() is IE only - problem with firefox in that if user chooses a file but doesn't fill in 
	// the other fields, the file will be submitted to SlickUpload during the round-trip to do the server-side
	// validation.  Hence, we need custom validation instead.
	if(typeof(Page_ClientValidate) == "function") {
		clientValidate = Page_ClientValidate();
	} else {
		clientValidate = true;
		var message = GetElement("txtMessage_txtTextBox");
		if(message.value.trim().length == 0) results += "Please enter the message.\n";
	}
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
function btnCancel_OnClick() {
	history.go(-1);
}

