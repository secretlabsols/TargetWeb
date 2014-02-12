
var STATUS_INITIALIZING = 0;
var STATUS_UPLOADING = 1;
var STATUS_COMPLETE = 2;

var FileUploadProgressDialog_UploadID;
var FileUploadProgressDialog_ProgressBar;
var FileUploadProgress_CurrentUploadStatus;
var FileUploadProgress_UploadComplete;
var FileUploadProgress_XmlDoc;

function FileUploadProgressDialog_Init() {
	// create progress bar
	FileUploadProgressDialog_ProgressBar = new ProgressBar(GetElement("fileUploadProgress"), 290, 12);
	// initial setup
	FileUploadProgress_UploadComplete = false;
	FileUploadProgress_CurrentUploadStatus = STATUS_INITIALIZING;
	FileUploadProgress_XmlDoc = Sarissa.getDomDocument();
	FileUploadProgress_XmlDoc.async = false;
	// start polling upload status
	window.setTimeout("FileUploadProgressDialog_GetProgress()", 500);
}
function FileUploadProgressDialog_UpdateProgress(status, size, time, percent) {
	GetElement("status").innerHTML = status;
	GetElement("size").innerHTML = size;
	GetElement("time").innerHTML = time;
	FileUploadProgressDialog_ProgressBar.setPercent(percent);
}
function FileUploadProgressDialog_GetProgress() {				

	FileUploadProgress_XmlDoc.load("FileStoreGetFileUploadProgress.axd?uploadId=" + FileUploadProgressDialog_UploadID);
	
	var isError = false;
	var title = GetElement("title");
			
	if (FileUploadProgress_XmlDoc != null && FileUploadProgress_XmlDoc.documentElement != null && FileUploadProgress_XmlDoc.documentElement.attributes.length > 0)	{
		
		var state = FileUploadProgress_XmlDoc.documentElement.getAttribute("state");
		var size = FileUploadProgress_XmlDoc.documentElement.getAttribute("contentLengthText");
		var remainingSize = FileUploadProgress_XmlDoc.documentElement.getAttribute("transferredLengthText");
		var time = FileUploadProgress_XmlDoc.documentElement.getAttribute("elapsedTimeText");
		var remainingTime = FileUploadProgress_XmlDoc.documentElement.getAttribute("remainingTimeText");
		var positionRaw = FileUploadProgress_XmlDoc.documentElement.getAttribute("positionRaw")
		var contentLengthRaw = FileUploadProgress_XmlDoc.documentElement.getAttribute("contentLengthRaw")
		var percent = (1/(contentLengthRaw/positionRaw))
	
		FileUploadProgressDialog_UpdateProgress(state.splitOnCapitals(), remainingSize + " / " + size, time + " / " + remainingTime, percent);
		
		if (state == "ReceivingData") {						
			if(FileUploadProgress_CurrentUploadStatus == STATUS_INITIALIZING) FileUploadProgress_CurrentUploadStatus = STATUS_UPLOADING;
		} else {			
			switch(state) {
				case "Error":
					SetInnerText(title, "Upload Error!");
					title.className = "titleError";
					isError = true;
					break;
					
				case "ErrorMaxRequestLengthExceeded":
					SetInnerText(title, "Error uploading file. Maximum request length exceeded.");
					title.className = "titleError";									
					isError = true;
					break;
			}
			FileUploadProgress_UploadComplete = true;
		}
	}
	if (FileUploadProgress_UploadComplete) {
		if(!isError) {
			SetInnerText(title, "Upload Complete");
			GetElement("status").innerHTML = "Upload Complete";
			window.setTimeout("window.close();", 1000);
		}
		FileUploadProgress_CurrentUploadStatus = STATUS_COMPLETE;
		GetElement("btnClose").disabled = false;
	}
	if (FileUploadProgress_CurrentUploadStatus != STATUS_COMPLETE) window.setTimeout("FileUploadProgressDialog_GetProgress()", 1000);
}

addEvent(window, "load", FileUploadProgressDialog_Init);

