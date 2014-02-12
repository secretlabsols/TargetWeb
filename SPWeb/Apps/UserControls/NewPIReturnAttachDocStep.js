
function NewPIReturnAttachDocStep_BeforeNavigate() {
	var uploadFile = GetElement("SelectorWizard1_uploadPIReturn").getElementsByTagName("DIV")[0].getElementsByTagName("INPUT")[0];
	var result;
	
	if(uploadFile.value.trim().length == 0) {
		window.alert("You have not selected a PI Workbook to submit. " + 
					"Please select a PI Workbook and then click the Finish button to Submit.");
	} else {
		result = true;
	}
	if(result) {
	    OpenFileUploadProgress(FileUploader_UploadID);
		window.setTimeout('document.forms[0].submit();', 1000);
	}
	return false;
}