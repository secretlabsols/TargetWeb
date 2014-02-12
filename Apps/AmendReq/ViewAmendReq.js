
var amendReqSvc, requestID;

function Init() {
	amendReqSvc = new Target.Web.Apps.AmendReq.WebSvc.AmendmentRequests_class();
	requestID = parseInt(GetQSParam(document.location.search, "id"), 10);
	FetchRequest();
}

function btnStartConv_OnClick() {
	document.location.href = "../Msg/NewConv.aspx";
}
function btnProcess_OnClick() {
	document.location.href = "Admin/ProcessAmendReq.aspx?id=" + requestID;
}

/* FETCH REQUESTS METHODS */
function FetchRequest() {
	DisplayLoading(true);
	amendReqSvc.FetchAmendmentRequestList(1, requestID, null, null, 0, 0, false, FetchRequest_Callback)
}
function FetchRequest_Callback(response) {
	var request;
	if(CheckAjaxResponse(response, amendReqSvc.url)) {
		// populate the screen
		request = response.value.Requests[0];
		SetInnerText(GetElement("spnReference"), request.Reference);
		SetInnerText(GetElement("spnRequested"), Date.strftime("%a %d %b %Y %H:%M:%S", request.CreatedDate));
		SetInnerText(GetElement("spnRequestedBy"), request.RequestedByUser + " (External User: " + request.RequestedByExternalUser + ")");
		SetInnerText(GetElement("spnRequest"), request.RequestDescription);
		GetElement("spnNewValue").innerHTML = request.NewValueDescription.replace(/\r\n/g, "<br />\r\n");
		SetInnerText(GetElement("spnStatus"), request.StatusDesc);
		if(request.ProcessedByUser != null) 
			SetInnerText(GetElement("spnProcessed"), request.ProcessedByUser + " (" + Date.strftime("%a %d %b %Y %H:%M:%S", request.ProcessedDate) + ")");
		else
			SetInnerText(GetElement("spnProcessed"), "[This request has not been processed.]");
		if(request.Comment != null) 
			SetInnerText(GetElement("spnComment"), request.Comment);		
	}
	DisplayLoading(false);
}

addEvent(window, "load", Init);

