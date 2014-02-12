
/* NEW SU NOTIF SELECTOR WIZARD STEPS */
function NewSUNotifEndDetailsStep_BeforeNavigate() {
	if(typeof(Page_ClientValidate) == "function") {
		clientValidate = Page_ClientValidate();
	} else {
		clientValidate = true;
	}
	if (clientValidate) {
		document.forms[0].submit();
	}	
	return false;
}
function NewSUNotifAttachDocStep_BeforeNavigate() {
	var scannedFile = GetElement("SelectorWizard1_scannedNotifForm").getElementsByTagName("DIV")[0].getElementsByTagName("INPUT")[0];
	var result;
	
	if(scannedFile.value.trim().length == 0) {
		result = window.confirm("You have not selected a scanned notification document to upload. " + 
					"If you do not have access to a scanner you may be permitted to post or fax the signed document instead.\n\n" +
					"NOTE: failure to provide a signed notification document may result in a subsidy not being paid.\n\n"  +
					"Are you sure you wish to continue?"
					);
	} else {
		result = true;
	}
	if(result) document.forms[0].submit();
	return false;
}
function NewSUNotif_BeforeNavigate() {
	// default before nav for NewSUNotif steps that do not have a specific implementation
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	SelectorWizard_newUrl = url;
	return true;
}
function NewSUNotifPrintDocStep_ViewNotif() {
	var url = "ViewSUNotif.aspx?suNotifID=" + GetQSParam(document.location.search, "suNotifID");
	window.open(url)
}