


//*********************************************************************************************************
// Comment/Query/ Notes DIALOG
//*********************************************************************************************************
addNamespace("Apps.Dom.ProformaInvoice.Comment.Dialog");

Apps.Dom.ProformaInvoice.Comment.Dialog = function(title, caption, textValue, isPrivate) {

    this.SetTitle(title);
    this.clearContent;
    this._type = 1;
    this.SetContentText(caption);
    this.SetWidth("70");


    // filter box	
    this._content.appendChild(document.createElement("BR"));
    this.FilterBox = AddTextArea(this._content, "txtValue", 20, 70, "", "", textValue, this.onKeyPress);
    this.FilterBox.style.width = "65em"
    // workaround for FireFox bug (see https://bugzilla.mozilla.org/show_bug.cgi?id=236791)
    this.FilterBox.setAttribute("autocomplete", "off");
    this._content.appendChild(document.createElement("BR"));

    this.rdoPrivateNotes = AddRadio(this._content, "", "notes", "PrivateNote", Notes_Click, new Array(this, 1));
    AddLabel(this._content, "PrivateNote", "Private Note");

    this.rdoPublicNotes = AddRadio(this._content, "", "notes", "PublicNote", Notes_Click, new Array(this, 2));
    AddLabel(this._content, "PublicNote", "Note viewable by the council on the Provider Invoice");

    if (isPrivate == 0 || isPrivate == 1) {
        this.rdoPrivateNotes.checked = true;
        this.rdoPrivateNotes.defaultChecked = true;
    } else {
        this.rdoPublicNotes.checked = true;
        this.rdoPublicNotes.defaultChecked = true;
    }
}

// inherit from base
Apps.Dom.ProformaInvoice.Comment.Dialog.prototype = new Target.Web.Dialog();

// override show method
Apps.Dom.ProformaInvoice.Comment.Dialog.prototype.Show = function() {
    this.ClearButtons();
    switch (this._type) {
        case 1:
            this.FilterBox.disabled = true;
            // Alert
            this.AddButton("OK", "", this._callback, new Array(this, 1)); // 1 = OK
            break;
        case 2:

            // Save/Clear/Cancel
            this.AddButton("Save", "", this._callback, new Array(this, 2)); // 2 = Save
            this.AddButton("Clear", "", this._callback, new Array(this, 3)); // 3 = Clear
            this.AddButton("Cancel", "", this._callback, new Array(this, 4)); // 4 = Cancel
            break;
        default:
            alert("Apps.Dom.ProformaInvoice.Comment.Dialog: unknown dialog type specified.");
            break;
    }
    Target.Web.Dialog.prototype.Show.call(this);
}

//*********************************************************************************************************
// Help DIALOG
//*********************************************************************************************************
addNamespace("Apps.Dom.ProformaInvoice.Comment.HelpDialog");

Apps.Dom.ProformaInvoice.Comment.HelpDialog = function(title, caption) { 
	this.SetTitle(title);
	this.clearContent;
	this._type = 1;
	this.SetContentText(caption);
	this.SetWidth("39");
	
}

// inherit from base
Apps.Dom.ProformaInvoice.Comment.HelpDialog.prototype = new Target.Web.Dialog();

// override show method
Apps.Dom.ProformaInvoice.Comment.HelpDialog.prototype.Show = function() {
	this.ClearButtons();
	switch(this._type) {
		case 1:
			// Alert
			this.AddButton("OK", "", this._callback, new Array(this, 1));	// 1 = OK
			break;
		default:
			alert("Apps.Dom.ProformaInvoice.Comment.HelpDialog: unknown dialog type specified.");
			break;
	}
	Target.Web.Dialog.prototype.Show.call(this);
}