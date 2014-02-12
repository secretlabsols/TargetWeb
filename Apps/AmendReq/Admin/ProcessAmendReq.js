
var amendReqSvc, requestID, oldValuesDifferFromCurrent, txtComment, btnAccept, btnDecline;
var txtCommentID = "txtComment_txtTextBox";
var btnAcceptID = "btnAccept";
var btnDeclineID = "btnDecline";

function Init() {
	amendReqSvc = new Target.Web.Apps.AmendReq.WebSvc.AmendmentRequests_class();
	requestID = parseInt(GetQSParam(document.location.search, "id"), 10);
	txtComment = GetElement(txtCommentID);
	btnAccept = GetElement(btnAcceptID);
	btnDecline = GetElement(btnDeclineID);
	FetchRequest();
}

function btnAccept_OnClick() {
	var result;
	if(oldValuesDifferFromCurrent) {
		result = window.confirm("The value of the field(s) has changed since this amendment request was made. Are you sure you wish to overwrite these changes with the new value(s) specified in this request?");
		if (!result) return;
	}
	// start the accept process
	FetchBeforeAcceptQuestion();
}
function btnDecline_OnClick() {
	if(txtComment.value.trim().length == 0) {
		alert("Please enter a comment to explain your decision to decline the changes in this request.");
		return;
	}
	// decline the request
	ProcessRequest(false, 0);	// 0 = No Answer
}
function btnCancel_OnClick() {
	history.go(-1);
}

/* BEFORE ACCEPT QUESTION METHODS */
function FetchBeforeAcceptQuestion() {
	DisplaySaving(true);
	amendReqSvc.AskBeforeAcceptQuestion(requestID, FetchBeforeAcceptQuestion_Callback);
}
function FetchBeforeAcceptQuestion_Callback(response) {
	var question;
	if(CheckAjaxResponse(response, amendReqSvc.url)) {
		question = response.value.Question;
		// ask the question, if there is one 
		if(question.Question.trim().length > 0 && question.Question.QuestionType != 0) {	// 0 = None
			var d = new Target.Web.Dialog.Msg();
			if (question.DialogHeight > 0) d.SetHeight(question.DialogHeight);
			if (question.DialogWidth > 0) d.SetWidth(question.DialogWidth);
			d.SetType(question.QuestionType);
			d.SetTitle("Confirmation");
			d.SetContentText(question.Question.replace(/\r\n/g, "<br />\r\n"));
			d.SetCallback(DialogCallback);
			d.Show();
			DisplaySaving(false);
		} else {
			ProcessRequest(true, 0);	// 0 = No Answer
		}
	}
}
function DialogCallback(evt, args) {
	var dialog = args[0];
	var dialogAnswer = args[1];
	dialog.Hide();
	ProcessRequest(true, dialogAnswer);
}

/* PROCESS REQUEST METHODS */
function ProcessRequest(accept, answer) {
	amendReqSvc.ProcessRequest(requestID, accept, answer, txtComment.value, ProcessRequest_Callback);
}
function ProcessRequest_Callback(response) {
	var cancelled;
	if(CheckAjaxResponse(response, amendReqSvc.url)) {
		cancelled = response.value.BooleanValue;
		if(!cancelled) {
			document.location.href = "../ListAmendReq.aspx";
		}
	}
	DisplaySaving(false);
}

/* FETCH REQUESTS METHODS */
function FetchRequest() {
	DisplayLoading(true);
	amendReqSvc.FetchAmendmentRequestList(1, requestID, null, null, 0, 0, true, FetchRequest_Callback)
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
		
		GetElement("spnOldValue").innerHTML = request.OldValueDescription.replace(/\r\n/g, "<br />\r\n");
		GetElement("spnCurrentValue").innerHTML = request.CurrentValueDescription.replace(/\r\n/g, "<br />\r\n");
		GetElement("spnNewValue").innerHTML = request.NewValueDescription.replace(/\r\n/g, "<br />\r\n");
		oldValuesDifferFromCurrent = request.OldValuesDifferFromCurrent;
		
		btnAccept.disabled = false;
		btnDecline.disabled = false;
		
	}
	DisplayLoading(false);
}

addEvent(window, "load", Init);

