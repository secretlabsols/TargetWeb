var pageView = null;

function DisplayExtErrorMessage(message) {
    message = message.replace(/\n/g, '<br />')
    Ext.create('Ext.window.Window', {
        bodyPadding: 5,
        bodyStyle: 'padding:10 10 0 10px;background:#FFFFFF;background-color:#FFFFFF',
        constrain: true,
        height: 400,
        html: message,
        minHeight: 200,
        minWidth: 600,
        modal: true,
        title: 'Error',
        width: 600,
        maximizable: true,
        overflowY: 'auto'        
    }).show();
}

function DisplayError(err, url, isExt) {
	var message = "An error has occurred in the operation of this application. Please try the operation again." +
		"\nIf the error persists, please contact the system administrator and quote the information given below." +
		"\n\nError: " + err.Number + " " + err.Message.replace(/<br \/>/g, "\n").replace(/<br\/>/g, "\n") +
		"\n\nDate/time: " + new Date() +
		"\nUrl: " + url +
		"\n\nSystem Exception: " + err.ExName +
		"\nSystem Message: " + err.ExMessage +
		"\nSource: " + err.ExSource +
		"\nTarget: " + err.ExTarget +
		"\nInner Exceptions:\n" + err.ExInnerExceptions +
		"\nStack Trace:\n" + err.ExStackTrace +
		"\nExtra Info:\n" + err.ExExtraInfo;
	if (!GetBoolean(isExt)) {
	    alert(message);
	} else {
	    DisplayExtErrorMessage(message);
	}
}
function DisplayAjaxError(err, url, isExt) {
	var message = "An error has occurred in the operation of this application. Please try the operation again." +
		"\nIf the error persists, please contact the system administrator and quote the information given below." +
		"\n\nAjax Error: " + err.Type + 
		"\nMessage: " + err.Message.replace(/<br \/>/g, "\n").replace(/<br\/>/g, "\n") + 
		"\nStatus: " + err.Status +
		"\n\nDate/time: " + new Date() +
		"\nUrl: " + url;
	if (!GetBoolean(isExt)) {
	    alert(message);
	} else {
	    DisplayExtErrorMessage(message);
	}
}
function DisplayBusinessLogicError(err, isExt) {
    var message = err.Message.replace(/<br \/>/g, "\n").replace(/<br\/>/g, "\n");

    // if error number exists then append it at the end
    if (err.Number && err.Number.length > 0) message += " (" + err.Number + ")";

    if (!GetBoolean(isExt)) {
        alert(message);
    } else {
        DisplayExtErrorMessage(message);
    }
}
function CheckAjaxResponse(response, url, isExt) {
	var errMsg = null;
	if(response.error != null) {
	    DisplayAjaxError(response.error, url, isExt);
		return false;
	}
	// the returned ErrorMessage object can be the response itself
	if (typeof (response.value.__type) != "undefined" && response.value.__type.indexOf("Target.Library.ErrorMessage") > -1) errMsg = response.value;
	// but is usually in the ErrMsg property of the result
	if(typeof(response.value.ErrMsg) != "undefined") errMsg = response.value.ErrMsg;
	// otherwise, some other error message passing mechanism is used
	if(errMsg != null) {	
		if(!errMsg.Success) {
		    if (errMsg.Number == "E0001" || errMsg.Number == "E0501" || errMsg.Number == "E0502")
		        DisplayError(errMsg, url, isExt);
			else
			    DisplayBusinessLogicError(errMsg, isExt);
			return false;
		}
	}
	return true;
}
function DisplayLoading(loading) {
	DisplayStatus("Loading...", loading, !loading);
}
function DisplaySaving(saving) {
	DisplayStatus("Saving...", saving, !saving);
}
function DisplayStatus(text, show, resetTimeout) {
	var status = GetElement("pageLoading");
	status.innerHTML = text;
	status.style.top = GetScrollY() + "px";
	status.style.visibility = show ? "visible" : "hidden";
	if(resetTimeout) ResetTimeout();
}
function ChangePageView(view) {
	if(pageView != null) GetElement(pageView + "_Content").style.display = "none";
    if(GetElement("subHeaderPageTitle", true)) SetInnerText(GetElement("subHeaderPageTitle"), GetInnerText(GetElement(view + "_PageTitle")));
    if(GetElement("pageOverviewContainer", true)) SetInnerText(GetElement("pageOverviewContainer"), GetInnerText(GetElement(view + "_PageOverview")));
	GetElement(view + "_Content").style.display = "block";
	pageView = view
}
// GENERAL HELPER FUNCTIONS
function AddLink(obj, text, href, title, doc) {
    // adds a A HREF tag to the specified object
    if (!doc) doc = document;
	var link = doc.createElement("A");
	link.innerHTML = text;
	link.href = href;
	link.title = title;
	obj.appendChild(link);
	return link;
}
function AddButton(obj, text, title, onclick, args, doc) {
    // adds a new button to the specified object
    if (!doc) doc = document;
	var button = AddInput(obj, "" ,"Button", title, "", text, onclick, args, doc);
	return button;
}

function AddLabel(obj, forid, text) {
    var lbl = document.createElement("label");
    lbl.setAttribute("for", forid);
    SetInnerText(lbl, text);
    obj.appendChild(lbl);
}

function AddRadio(obj, text, name, value, onclick, args, doc) {

    var elems, radio, html, label;
    if (!doc) doc = document;
    if (ie6 || ie7) {
		radio = doc.createElement('<input name="' + name + '"/>');
		radio.setAttribute("type", "radio");
		radio.setAttribute("value", value);
		radio.setAttribute("id", value);
		obj.appendChild(radio);
		AddOnClick(radio, onclick, args);
	} else {
	    radio = AddInput(obj, value, "radio", "", name, value, onclick, args, doc);
    }
	return radio;
}

function AddHidden(obj, text, name, value, doc) {
    var elems, hidden, html;
    if (!doc) doc = document;
    if (ie6 || ie7) {
		hidden = doc.createElement('<input name="' + name + '"/>');
		hidden.setAttribute("type", "hidden");
		hidden.setAttribute("value", value);
		obj.appendChild(hidden);
		AddOnClick(hidden);
	} else {
	    hidden = AddInput(obj, "", "hidden", "", name, value, doc);
    }
	return hidden;
}

function AddInput(obj, id, type, title, name, value, onclick, args, doc) {
    // adds a new button to the specified object
    if (!doc) doc = document;
	var input = doc.createElement("input");
	input.id = id;
	input.type = type;
	input.title = title;
	input.name = name;
	input.value = value;
	
	AddOnClick(input, onclick, args);
	obj.appendChild(input);
	return input;
}
function AddTextArea(obj, id, rows, cols, title, name, value, onKeyPress, args, doc) {
    // adds a new button to the specified object
    if (!doc) doc = document;
	var input = doc.createElement("textArea");
	input.id = id;
    input.rows = rows;
    input.cols = cols;
	input.title = title;
	input.name = name;
	input.value = value;
	
	//AddOnClick(input, onclick, args);
	AddOnKeypress(input, onKeyPress, args);
	obj.appendChild(input);
	return input;
}
function AddImg(obj, src, alt, cursor, onclick, args, doc) {
    if (!doc) doc = document;
    var img = doc.createElement("img");
	img.src = src;
	img.alt = alt;
	if(cursor) img.style.cursor = cursor;
	
	AddOnClick(img, onclick, args);
	obj.appendChild(img);
	return img;
}

function AddOnKeypress(obj, onKeyPress, args) {
	if (onKeyPress != undefined) {
	    var fnGo = function go(evt) { onKeyPress(evt, args); }
		addEvent(obj, "onKeyPress", fnGo);
		return fnGo;
	}
}

function AddOnClick(obj, onclick, args) {
	if (onclick != undefined) {
	    var fnGo = function go(evt) { onclick(evt, args); }
		addEvent(obj, "click", fnGo);
		return fnGo;
	}
}
function RemoveOnClick(obj, onclick) {
	if (onclick != undefined) {
		removeEvent(obj, "click", onclick);
	}
}
// TABLE HELPER FUNCTIONS
function ClearTable(table) {
	// deletes all rows from the table tbody
	var tbody = table.tBodies[0];
	if(tbody) {
		for(i=tbody.rows.length-1; i>=0; i--) {
			tbody.deleteRow(i);
		}
	}
}
function AddRow(table) {
	// appends a new row to the last table tbody
	var tbody = table.tBodies[0];
	var tr = document.createElement("TR");
	if(tbody)
		tbody.appendChild(tr);
	else
		table.appendChild(tr);
	return tr;
}
function AddCell(tr, text) {
	// adds a new cell with text to a table row
	var td = document.createElement("TD");
	var textNode = document.createTextNode(text);
	td.appendChild(textNode);
	tr.appendChild(td);
	return td;
}
/* AjaxPro Init */
function AjaxPro_Init() {
	//AjaxPro.timeoutPeriod = 10000;
	if(typeof(AjaxPro) != "undefined") {
		AjaxPro.onTimeout = AjaxPro_OnTimeout;
	}
}
function AjaxPro_OnTimeout() {
	alert("The web site is taking a long time to return the requested data.\n\n" +
			"You may continue to wait or, alternatively, click 'OK' on this message and refresh the page to try again.\n\n" +
			"If the problem persists, please contact the system administrator.");
}
//addEvent(window, "load", AjaxPro_Init);

function GetBoolean(val) {
    if (val === null || val === undefined) {
        return false;
    } else {
        return val;
    }
}