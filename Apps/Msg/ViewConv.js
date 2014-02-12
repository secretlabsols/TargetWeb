
var msgSvc, tblMessages, divPagingLinks, convID, currentPage, messages, grpMessages, btnBack, btnReply, cboActions, divActions, btnMarkMessageAsUnRead;
var divMsgView, spnMsgViewFrom, spnMsgViewTo, spnMsgViewSent, grpMsgViewAttachments, spnMsgViewAttachments, grpMsgViewMsg, currentMsg, newMsgReadStatus;
var tblMessagesID = "Messages_Table";
var divPagingLinksID = "Messages_PagingLinks";
var grpMessagesID = "grpMessages";
var btnBackID = "btnBack";
var btnReplyID = "btnReply";
var btnMarkMessageAsUnReadID = "btnMarkMessageAsUnRead";
var cboActionsID = "cboActions";
var divMsgViewID = "divMsgView";
var divActionsID = "divActions";
var spnMsgViewFromID = "spnMsgViewFrom";
var spnMsgViewToID = "spnMsgViewTo";
var spnMsgViewSentID = "spnMsgViewSent";
var grpMsgViewAttachmentsID = "grpMsgViewAttachments";
var spnMsgViewAttachmentsID = "spnMsgViewAttachments";
var grpMsgViewMsgID = "grpMsgViewMsg";
var selectedMsgID;

function Init() {
	msgSvc = new Target.Web.Apps.Msg.WebSvc.Messaging_class();
	btnBack = GetElement(btnBackID);
	btnReply = GetElement(btnReplyID);
	btnMarkMessageAsUnRead = GetElement(btnMarkMessageAsUnReadID);
	tblMessages = GetElement(tblMessagesID);
	divPagingLinks = GetElement(divPagingLinksID);
	grpMessages = GetElement(grpMessagesID);
	
	divActions = GetElement(divActionsID);
	divMsgView = GetElement(divMsgViewID);
	spnMsgViewFrom = GetElement(spnMsgViewFromID);
	spnMsgViewTo = GetElement(spnMsgViewToID);
	spnMsgViewSent = GetElement(spnMsgViewSentID);
	grpMsgViewAttachments = GetElement(grpMsgViewAttachmentsID);
	spnMsgViewAttachments = GetElement(spnMsgViewAttachmentsID);
	grpMsgViewMsg = GetElement(grpMsgViewMsgID);
	cboActions = GetElement(cboActionsID);
		
	convID = parseInt(GetQSParam(document.location.search, "id"), 10);
	currentPage = 1;
	// fetch the messages
	FetchMessageList(currentPage);
}
function btnBack_OnClick() {
	var url = unescape(GetQSParam(document.location.search, "backUrl"));
	document.location.href = url;
}
function btnReply_OnClick() {
	document.location.href = "ReplyTo.aspx?id=" + currentMsg;
}
function btnMarkMessageAsUnRead_OnClick() {
	msgSvc.SetMessageReadStatus(selectedMsgID, false);
	//ShowMessages();
	//btnBack_OnClick();
	FetchMessageList(currentPage);
	ShowMessages();
}

function cboActions_OnChange() {
	var url = document.location.href;
	var labelID = cboActions.value;
	var newLabelText = null;
	var rowCounter;
	var newMsgReadStatus;
	
	// remove any existing params
	url = RemoveQSParam(url, "alID");
	url = RemoveQSParam(url, "rlID");
	url = RemoveQSParam(url, "nl");
	
	if (labelID == "Read") {
		for(rowCounter=0; rowCounter<tblMessages.rows.length - 1; rowCounter++) {
			//SetMessageReadStatus(tblMessages.tBodies[0].rows[rowCounter].getAttribute("label"), true)
			//alert(tblMessages.tBodies[0].rows[rowCounter].getAttribute("label"))
			msgSvc.SetMessageReadStatus(tblMessages.tBodies[0].rows[rowCounter].getAttribute("label"), true)
		}
	} else if (labelID == "UnRead"){
		for(rowCounter=0; rowCounter<tblMessages.rows.length - 1; rowCounter++) {
			//SetMessageReadStatus(tblMessages.tBodies[0].rows[rowCounter].getAttribute("label"), false)
			//alert(tblMessages.tBodies[0].rows[rowCounter].getAttribute("label"))
			msgSvc.SetMessageReadStatus(tblMessages.tBodies[0].rows[rowCounter].getAttribute("label"), false)
		}
	}else if(parseInt(labelID) == -1) {
		// new label
		newLabelText = window.prompt("Please enter the name of the new label", "New label");
		if(newLabelText == null) {
			cboActions.value = 0;
			return;
		}
		url = AddQSParam(url, "nl", escape(newLabelText));
	} else {
		url = AddQSParam(url, cboActions.options[cboActions.selectedIndex].getAttribute("tag"), labelID);
	}
	document.location.href = url;
}
function ViewMsg(id) {
	var msgCounter, attachments, attachmentKeys, keyCounter, link;
	
	for(msgCounter=0; msgCounter<messages.length; msgCounter++) {
		if(messages[msgCounter].ID == id) {
			// load the content
			selectedMsgID = messages[msgCounter].ID;
			spnMsgViewFrom.innerHTML = messages[msgCounter].FromName;
			spnMsgViewTo.innerHTML = messages[msgCounter].ToName;
			spnMsgViewSent.innerHTML = Date.strftime("%a %d %b %Y %H:%M:%S", messages[msgCounter].Sent);
			
			// attachments
			attachments = messages[msgCounter].Attachments
			attachmentKeys = attachments.getKeys();
			spnMsgViewAttachments.innerHTML = "";
			if(attachmentKeys.length == 0) {
				grpMsgViewAttachments.style.display = "none";
			} else {
				grpMsgViewAttachments.style.display = "block";
			}
			for(keyCounter=0; keyCounter<attachmentKeys.length; keyCounter++) {
				link = AddLink(spnMsgViewAttachments, attachmentKeys[keyCounter], 
					"../FileStore/FileStoreGetFile.axd?fileDataID=" + attachments.getValue(attachmentKeys[keyCounter]), 
					"Click here to open/download this attachment");
				link.setAttribute("target", "_blank");
				spnMsgViewAttachments.appendChild(document.createTextNode("; "));
			}
			grpMsgViewMsg.innerHTML = messages[msgCounter].Message.replace(/\r\n/g, "<br />\r\n");
			
			// show/hide content blocks
			grpMessages.style.display = "none";
			btnReply.style.display = "inline";
			//btnReply.style.margin-right = "0.5em";
			btnMarkMessageAsUnRead.style.display = "inline";
			divActions.style.display = "none";
			divMsgView.style.display = "block";
			btnBack.onclick = ShowMessages;
			btnBack.title = "Click here to view all of the message in this conversation";
			
			currentMsg = id;
			
			// set the message read status
			if(!messages[msgCounter].ReadStatus) {
				newMsgReadStatus = true;
				SetMessageReadStatus(currentMsg, newMsgReadStatus);
			}			
			
			break;
		}
	}
}
function ShowMessages() {
	grpMessages.style.display = "block";
	btnReply.style.display = "none";
	btnMarkMessageAsUnRead.style.display = "none";
	divActions.style.display = "block";
	divMsgView.style.display = "none";
	btnBack.onclick = btnBack_OnClick;
	btnBack.title = "Click here to return to the previous page";
	currentMsg = 0;
}

/* SET READ STATUS */
function SetMessageReadStatus(msgID, readStatus) {
	DisplayLoading(true);
	msgSvc.SetMessageReadStatus(msgID, readStatus, SetMessageReadStatus_Callback)
}
function SetMessageReadStatus_Callback(response) {
	var msgCounter, rowCounter;
	if(CheckAjaxResponse(response, msgSvc.url)) {
		for(msgCounter=0; msgCounter<messages.length; msgCounter++) {
			// locate the message
			if(messages[msgCounter].ID == currentMsg) {
				// update the local copy of its read status
				messages[msgCounter].ReadStatus = newMsgReadStatus;
				// update the UI
				// find the relevant message row
				for(rowCounter=0; rowCounter<tblMessages.rows.length - 1; rowCounter++) {
					if(parseInt(tblMessages.tBodies[0].rows[rowCounter].getAttribute("label"), 10) == messages[msgCounter].ID) {
						SetUIReadStatus(tblMessages.tBodies[0].rows[rowCounter], tblMessages.tBodies[0].rows[rowCounter].cells[0], newMsgReadStatus);
						break;
					}
				}
				break;
			}
		}
	}
	DisplayLoading(false);
}

/* FETCH MESSAGES */
function FetchMessageList(page){
	DisplayLoading(true);
	msgSvc.FetchMessageList(page, convID, FetchMessageList_Callback)
}
function FetchMessageList_Callback(response) {
	var msgCounter;
	if(CheckAjaxResponse(response, msgSvc.url)) {
		// populate the messages table
		messages = response.value.Messages;
		// remove existing rows
		ClearTable(tblMessages);
		for(msgCounter=0; msgCounter<messages.length; msgCounter++) {
			tr = AddRow(tblMessages);
			tr.setAttribute("label", messages[msgCounter].ID);
			td = AddCell(tr, " ");
			SetUIReadStatus(tr, td, messages[msgCounter].ReadStatus);
			td = AddCell(tr, messages[msgCounter].FromName);
			td = AddCell(tr, messages[msgCounter].ToName);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			td = AddCell(tr, "");
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			AddLink(td, messages[msgCounter].Message, "javascript:ViewMsg(" + messages[msgCounter].ID + ");", "Click here to view this message");
			AddCell(tr, Date.strftime("%a %d %b %Y %H:%M:%S", messages[msgCounter].Sent));
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

addEvent(window, "load", Init);