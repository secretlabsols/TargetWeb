
var FileUploadProgressDialog_UploadID;
var FileUploadProgressDialog_ProgressBar;
var FileUploadProgressDialog_WebSvc;
//var FileUploadProgressDialog_IntervalID;

function FileUploadProgressDialog_UpdateProgress(status, size, time, percent) {
	GetElement("status").innerHTML = status;
	GetElement("size").innerHTML = size;
	GetElement("time").innerHTML = time;
	FileUploadProgressDialog_ProgressBar.setPercent(percent);
}

function FileUploadProgressDialog_GetProgress() {
	FileUploadProgressDialog_WebSvc.GetProgress(FileUploadProgressDialog_UploadID, FileUploadProgressDialog_GetProgress_Callback);
}

function FileUploadProgressDialog_GetProgress_Callback(response) {
	var title = GetElement("title");
	
	if(CheckAjaxResponse(response, FileUploadProgressDialog_WebSvc.url)) {
		if(response.value.State != null) {
			FileUploadProgressDialog_UpdateProgress(response.value.State.splitOnCapitals(), response.value.TransferredLengthText + " / " + response.value.ContentLengthText, 
				response.value.ElapsedTimeText + " / " + response.value.RemainingTimeText, (1/(response.value.ContentLengthRaw/response.value.PositionRaw)));
			if(response.value.State.substr(0, 5) == "Error") {
				SetInnerText(title, "Upload Error!");
				title.className = "titleError";
			}
			if(response.value.State == "Complete") {
				SetInnerText(title, "Upload Complete");
				window.setTimeout("window.close();", 1000);
			}
			if(response.value.State != "ReceivingData") {
				// if we are done, stop polling and enable close button
				//window.clearInterval(FileUploadProgressDialog_IntervalID);
				GetElement("btnClose").disabled = false;
			}
		}
	}
	window.setTimeout("FileUploadProgressDialog_GetProgress()", 100);
}

function FileUploadProgressDialog_Init() {
	// create progress bar
	FileUploadProgressDialog_ProgressBar = new ProgressBar(GetElement("fileUploadProgress"), 290, 12);
	// create web svc utility class
	FileUploadProgressDialog_WebSvc = new Target.Web.Apps.FileStore.FileUploadProgress_class();
	// start polling upload status
	//FileUploadProgressDialog_IntervalID = window.setInterval("FileUploadProgressDialog_GetProgress()", 1000);
	window.setTimeout("FileUploadProgressDialog_GetProgress()", 1000);
}

addEvent(window, "load", FileUploadProgressDialog_Init);
