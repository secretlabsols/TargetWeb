var btnNext;
function Init() {
    var uploadFile = GetElement("flServiceDelFileUpload").getElementsByTagName("DIV")[0].getElementsByTagName("INPUT")[0];

    if (uploadFile.value.trim().length == 0) {
        btnNext = GetElement("btnNext");
        btnNext.disabled = true;
    }
}

function btnNext_click() {
	var uploadFile = GetElement("flServiceDelFileUpload").getElementsByTagName("DIV")[0].getElementsByTagName("INPUT")[0];
	var result;
	
	if(uploadFile.value.trim().length == 0) {
		window.alert("You have not selected a service delivery file to upload. " + 
					"Please select a service delivery file and then click the Next button to upload.");
	} else {
		result = true;
	}
	if(result) {
		OpenFileUploadProgress(FileUploader_UploadID);
		window.setTimeout('document.forms[0].submit();', 1000);
	}
	return false;
}

function btnBack_click() {
    var url;
    url = unescape(GetQSParam(document.location.search, 'backUrl'));
    document.location.href = url;
}

function FileUploader_UploadBox_Changed_Custom(inputBox) {

    btnNext.disabled = false;
}

addEvent(window, "load", Init);