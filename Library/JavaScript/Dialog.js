
//*********************************************************************************************************
// DIALOG BASE CLASS
//*********************************************************************************************************
addNamespace("Target.Web.Dialog");

Target.Web.Dialog.Current = null;

Target.Web.Dialog = function() {
    var doc = GetDocForModalDIV();
    // private variables
    this._box = doc.createElement("DIV");
    this._title = doc.createElement("DIV");
    this._titleText = doc.createElement("DIV");
    this._content = doc.createElement("DIV");
    this._buttons = doc.createElement("DIV");
    this._closeImg = doc.createElement("IMG");
    this._closeLink = null;
    // default callback function	
    this._callback = function(evt, args) {
        var dialog = args[0];
        var dialogAnswer = args[1];
        dialog.Hide();
    }

    // setup dialog
    this._width = "30";
    this._height = "8";
    this._box.className = "dialogBox";
    // title
    this._title.id = "dialogTitle";
    SetInnerText(this._titleText, "[Dialog Title]");
    this._titleText.id = "titleText";
    this._title.appendChild(this._titleText);
    // close button
    this._closeImg.src = SITE_VIRTUAL_ROOT + "images/close.gif";
    this._closeLink = AddLink(this._title, "", "javascript:void(0);", "Close", doc);
    this._closeLink.id = "closeLink";
    this._closeLink.className = "transBg";
    //AddOnClick(this._closeLink, this._callback, new Array(this));
    this._closeLink.appendChild(this._closeImg);
    this._box.appendChild(this._title);
    // content
    this._content.id = "dialogContent";
    this._content.innerHTML = "[Dialog Content]";
    this._box.appendChild(this._content);
    // buttons
    this._buttons.id = "dialogButtons"
    this._box.appendChild(this._buttons);
    this._defaultButton = null;
}
// public property setters
Target.Web.Dialog.prototype.SetHeight = function(height) {
	this._height = height;
}
Target.Web.Dialog.prototype.SetWidth = function(width) {
	this._width = width;
}
Target.Web.Dialog.prototype.SetTitle = function(title) {
	SetInnerText(this._titleText, title);
}
Target.Web.Dialog.prototype.SetCallback = function(callback) {
	this._callback = callback;
}
Target.Web.Dialog.prototype.SetDefaultButton = function(dialog, textBox, button) {
	dialog._defaultButton = button;
	// 13 = ENTER key
	addEvent(textBox, "keydown", function go(evt) { if(CatchKeyPress(evt, 13)) { cancelEvent(evt, true); dialog._defaultButton.click(); } });
}
Target.Web.Dialog.prototype.ShowCloseLink = function(show) {
	this._closeLink.style.display = show ? "visible" : "none";
}

// public methods
Target.Web.Dialog.prototype.Hide = function() {
    if (Target.Web.Dialog.Current != null) {
        var doc = GetDocForModalDIV();
        removeEvent(doc, "keydown", Target.Web.Dialog.Current.HideViaKeyPress);
        doc.forms[0].removeChild(Target.Web.Dialog.Current._box);
        HideModalDIV();
        Target.Web.Dialog.Current = null;
    }
}
Target.Web.Dialog.prototype.HideViaKeyPress = function(evt) {
	if(CatchKeyPress(evt, 27)) {	// 27 = ESC key
		cancelEvent(evt, true);
		Target.Web.Dialog.prototype.Hide.call();
	}
}
Target.Web.Dialog.prototype.Show = function() {
    var doc = GetDocForModalDIV();
    // add to document
    ShowModalDIV();
    doc.forms[0].appendChild(this._box);
    // set position
    var scrollTop = GetScrollY(doc.parentWindow);
    var widthPx = ConvertEmToPx(this._width);
    var heightPx = ConvertEmToPx(this._height);
    this._box.style.width = widthPx + "px";
    this._box.style.height = heightPx + "px";
    var left = Math.round((GetViewportWidth(window.top) - this._content.offsetWidth) / 2);
    var top = Math.round((GetViewportHeight(window.top) - this._content.offsetHeight / 2 - (this._box.offsetHeight / 2)) / 2) + scrollTop;
    this._box.style.top = top + "px";
    this._box.style.left = left + "px";
    this._box.style.zIndex = 10000;
    Target.Web.Dialog.Current = this;
    // handle ESC key to close the dialog
    addEvent(doc, "keydown", Target.Web.Dialog.Current.HideViaKeyPress);
    AddOnClick(this._closeLink, this._callback, new Array(this));
}
Target.Web.Dialog.prototype.ClearButtons = function() {
	this._buttons.innerHTML = "";
}
Target.Web.Dialog.prototype.AddButton = function(text, title, onclick, args) {
    // adds a new button to the specified object
    var doc = GetDocForModalDIV();
    var button = doc.createElement("INPUT");
	button.type = "button";
	button.value = text;
	button.title = title;
	addEvent(button, "click", function go(evt) { onclick(evt, args); });
	this._buttons.appendChild(button);
	return button;
}
Target.Web.Dialog.prototype.ClearContent = function() {
	this._content.innerHTML = "";
}
Target.Web.Dialog.prototype.AddContent = function(content) {
	this._content.appendChild(content);
}

//*********************************************************************************************************
// MESSAGE DIALOG
//*********************************************************************************************************
addNamespace("Target.Web.Dialog.Msg");

Target.Web.Dialog.Msg = function() { 
	// private variables
	this._type = 1;	// alert	
}
// inherit from base
Target.Web.Dialog.Msg.prototype = new Target.Web.Dialog();

// property setters
Target.Web.Dialog.prototype.SetType = function(type) {
	this._type = type;
}
Target.Web.Dialog.prototype.SetContentText = function(contentText) {
	this._content.innerHTML = contentText;
}

// override show method
Target.Web.Dialog.Msg.prototype.Show = function() {
	this.ClearButtons();
	switch(this._type) {
		case 1:
			// Alert
			this.AddButton("OK", "", this._callback, new Array(this, 1));	// 1 = OK
			break;
		case 2:
			// Yes/No
			this.AddButton("Yes", "", this._callback, new Array(this, 2));	// 2 = Yes
			this.AddButton("No", "", this._callback, new Array(this, 3));	// 3 = No
			break;
		case 3:
			// Yes/No/Cancel
			this.AddButton("Yes", "", this._callback, new Array(this, 2));	// 2 = Yes
			this.AddButton("No", "", this._callback, new Array(this, 3));	// 3 = No
			this.AddButton("Cancel", "", this._callback, new Array(this, 4));	// 4 = Cancel
			break;
		case 4:
		    // OK/Cancel
		    this.AddButton("OK", "", this._callback, new Array(this, 1));	// 1 = OK
		    this.AddButton("Cancel", "", this._callback, new Array(this, 4));	// 4 = Cancel
			break;
		case 5:
		    // Save/Cancel
		    this.AddButton("Save", "", this._callback, new Array(this, 5));	// 5 = Save
		    this.AddButton("Cancel", "", this._callback, new Array(this, 4));	// 4 = Cancel
			break;
		default:
			alert("Target.Web.Dialog.Msg: unknown dialog type specified.");
			break;
	}
	Target.Web.Dialog.prototype.Show.call(this);
}
