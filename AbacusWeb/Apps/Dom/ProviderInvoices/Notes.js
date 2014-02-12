var invoiceID, lookupSvc, tblInvNotes, domProviderInvoiceID, txtNoteID, contractSvc, noteID, noteText;
var btnView, btnNew, btnEdit, btnDelete;
var popUpTitle;

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	tblInvNotes = GetElement("tblInvNotes");
	btnView = GetElement("btnView", true);
	btnNew = GetElement("btnNew", true);
	btnEdit = GetElement("btnEdit", true);
	btnDelete = GetElement("btnDelete", true);
    
	if(btnView) btnView.disabled = true;
	if(btnEdit) btnEdit.disabled = true;
	if(btnDelete) btnDelete.disabled = true;
	
	// populate table
	invoiceID = GetQSParam(document.location.search, "ID");

	FetchDPINotesList();
}


function FetchDPINotesList() {
	
	DisplayLoading(true);
	if(invoiceID == undefined || invoiceID == null) invoiceID = 0;
	

	lookupSvc.FetchDomProviderInvoiceNotesList(invoiceID, FetchDPINotesList_Callback)
}

function FetchDPINotesList_Callback(response) {
	var notes, index;
	var tr, td, radioButton;
	var str;
	var link;
	if(invoiceID == 0) {
		if(btnView) btnView.disabled = true;
	    if(btnEdit) btnEdit.disabled = true;
	    if(btnDelete) btnDelete.disabled = true;
	}
	
	if(CheckAjaxResponse(response, lookupSvc.url)) {
		
		// populate the table
		notes = response.value.Notes;
		
		// remove existing rows
		ClearTable(tblInvNotes);
		for(index=0; index<notes.length; index++) {
		
			tr = AddRow(tblInvNotes);
			td = AddCell(tr, "");
			radioButton = AddRadio(td, "", "DPINotesSelect", notes[index].ID, RadioButton_Click);


			td = AddCell(tr, notes[index].isProviderInvoiceNote);
			td.style.display = "none";
			
			AddCell(tr, Date.strftime("%d/%m/%Y", notes[index].Date));
		
			AddCell(tr, Date.strftime("%H:%M", notes[index].Time));
			
			td = AddCell(tr, notes[index].EnteredBy);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			td = AddCell(tr, notes[index].Notes.substring(0,60));
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			AddCell(tr, Date.strftime("%d/%m/%Y %H:%M", notes[index].DateAmended));
			
			str = notes[index].AmendedBy
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
			
			if(notes[index].ID == invoiceID || notes.length == 1)
			    radioButton.click();
			
		}

	}
	DisplayLoading(false);
}

function RadioButton_Click() {
	var index, rdo;
	
	for (index = 0; index < tblInvNotes.tBodies[0].rows.length; index++){
		rdo = tblInvNotes.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			tblInvNotes.tBodies[0].rows[index].className = "highlightedRow";
			noteID = rdo.value;			
			if(btnView) btnView.disabled = false;
	        if(btnEdit) btnEdit.disabled = false;
	        if (btnDelete) btnDelete.disabled = false;

	        // disable edit and delete button
	        if (tblInvNotes.tBodies[0].rows[index].cells[1].innerText == "true") {
	            if (btnEdit) btnEdit.disabled = true;
	            if (btnDelete) btnDelete.disabled = true;
	        }
	        
		} else {
			tblInvNotes.tBodies[0].rows[index].className = "";
        }
       
	}
}

function btnView_Click() {
    var d = new Target.Web.Dialog.Msg();
    var emptyDialogContent, noteDialogContentContainer, noteDialogContent;
    var txtTextbox, response, noteText;
    
    // populate node text
    txtTextbox = GetElement(txtNoteID + "_txtTextBox");
    txtTextbox.readOnly = true;
    response = contractSvc.DomProviderInvoiceNotes_fetchNoteText(noteID).value;
    if(response.ErrMsg.Success == true) {
        txtTextbox.value = response.Value;
        txtTextbox.style.width = "50em";
        txtTextbox.style.height = "17em";
        
        d.SetType(1);   // OK
        d.SetCallback(NoteDialog_Callback);
        d.SetTitle("View an existing note");
        d.SetWidth("60");
        d.SetHeight("40");

        d.ClearContent();    
        emptyDialogContent = document.createElement("DIV");
        d.AddContent(emptyDialogContent);
        
        noteDialogContentContainer = GetElement("divCopyDialogContentContainer");
        noteDialogContent = noteDialogContentContainer.getElementsByTagName("DIV")[0];
        
        // swap nodes
        emptyDialogContent = d._content.removeChild(emptyDialogContent);
        noteDialogContent = noteDialogContentContainer.removeChild(noteDialogContent);
        noteDialogContentContainer.appendChild(emptyDialogContent);
        d.AddContent(noteDialogContent);
        
        d.ShowCloseLink(false);
        d.Show();
    
    } else {
        alert(response.ErrMsg.Message);
        return;
    } 
}

function btnEdit_Click() {
    var d = new Target.Web.Dialog.Msg();
    var emptyDialogContent, noteDialogContentContainer, noteDialogContent;
    var txtTextbox, response, noteText;
    
    // populate node text
    txtTextbox = GetElement(txtNoteID + "_txtTextBox");
    txtTextbox.readOnly = false;
    response = contractSvc.DomProviderInvoiceNotes_fetchNoteText(noteID).value;
    if(response.ErrMsg.Success == true) {
        txtTextbox.value = response.Value;
        txtTextbox.style.width = "50em";
        txtTextbox.style.height = "17em";
        
        d.SetType(5);   // Save/Cancel
        d.SetCallback(NoteDialog_Callback);
        d.SetTitle("Edit an existing note");
        d.SetWidth("60");
        d.SetHeight("40");

        d.ClearContent();    
        emptyDialogContent = document.createElement("DIV");
        d.AddContent(emptyDialogContent);
        
        noteDialogContentContainer = GetElement("divCopyDialogContentContainer");
        noteDialogContent = noteDialogContentContainer.getElementsByTagName("DIV")[0];
        
        // swap nodes
        emptyDialogContent = d._content.removeChild(emptyDialogContent);
        noteDialogContent = noteDialogContentContainer.removeChild(noteDialogContent);
        noteDialogContentContainer.appendChild(emptyDialogContent);
        d.AddContent(noteDialogContent);
        
        d.ShowCloseLink(false);
        d.Show();
    } else {
        alert(response.ErrMsg.Message);
        return;
    } 
}

function btnDelete_Click() {
    if(window.confirm('Are you sure you wish to Delete this note?')) {
        response = contractSvc.DomProviderInvoiceNotes_deleteNote(noteID).value;
        if(response.Success == false) {
            alert(response.Message);
            return;
        } else {
            document.location.href = "Notes.aspx" + document.location.search;
        }
    }

}

function btnNew_Click() {
    var d = new Target.Web.Dialog.Msg();
    var emptyDialogContent, noteDialogContentContainer, noteDialogContent;

    noteID = 0;
    UnSelectNote();
    
    txtTextbox = GetElement(txtNoteID + "_txtTextBox");
    txtTextbox.readOnly = false;
    txtTextbox.value = "";
    txtTextbox.style.width = "50em";
    txtTextbox.style.height = "17em";
    
    d.SetType(5);   // Save/Cancel
    d.SetCallback(NoteDialog_Callback);
    d.SetTitle("Enter a new note");
    d.SetWidth("60");
    d.SetHeight("40");

    d.ClearContent();    
    emptyDialogContent = document.createElement("DIV");
    d.AddContent(emptyDialogContent);
    
    noteDialogContentContainer = GetElement("divCopyDialogContentContainer");
    noteDialogContent = noteDialogContentContainer.getElementsByTagName("DIV")[0];
    
    // swap nodes
    emptyDialogContent = d._content.removeChild(emptyDialogContent);
    noteDialogContent = noteDialogContentContainer.removeChild(noteDialogContent);
    noteDialogContentContainer.appendChild(emptyDialogContent);
    d.AddContent(noteDialogContent);
    
    d.ShowCloseLink(false);
    d.Show();
}

function UnSelectNote() {
        var index, rdo;
        for (index = 0; index < tblInvNotes.tBodies[0].rows.length; index++) {
            rdo = tblInvNotes.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
            if (rdo.checked) {
                tblInvNotes.tBodies[0].rows[index].className = "";
                rdo.checked = false;
                if (btnView) btnView.disabled = true;
                if (btnEdit) btnEdit.disabled = true;
                if (btnDelete) btnDelete.disabled = true;
            } 
        }
}

function GetBackUrl() {
    //var url = document.location.href;
    //url = AddQSParam(RemoveQSParam(url, "ID"), "ID", invoiceID);
    //return escape(url);
    return GetQSParam(document.location.search, "backUrl")
}

function NoteDialog_Callback(evt, args) {
    var d = args[0];
    var answer = args[1];
    var emptyDialogContent, noteDialogContentContainer, noteDialogContent;
    var valRequired, noteText, response;
    
    if(noteID == undefined || noteID == null) noteID = 0;
    
    // answer == 5 means Save
    if(answer == 5) {
        if(Page_ClientValidate("Copy")) {
            noteText = GetElement(txtNoteID + "_txtTextBox").value;
            //valRequired = GetElement(dteCopyWeekEndingID + "_valRequired");
            response = contractSvc.DomProviderInvoiceNotes_CreateNew(invoiceID, noteID, noteText);
            if(response.Success == false) {
                alert(response.Message);
                return;
            } else {
                document.location.href = "Notes.aspx" + document.location.search
            }
        } else {
            return;
        }
      }
//    } else if (answer == 4) {
//        document.location.href = "Notes.aspx?ID=" + invoiceID + 
//                                    "&backUrl=" + GetBackUrl();
//    }
    noteDialogContentContainer = GetElement("divCopyDialogContentContainer");
    emptyDialogContent = noteDialogContentContainer.getElementsByTagName("DIV")[0];
    noteDialogContent = d._content.getElementsByTagName("DIV")[0];
    
    // swap nodes
    emptyDialogContent = noteDialogContentContainer.removeChild(emptyDialogContent);
    noteDialogContent = d._content.removeChild(noteDialogContent);
    noteDialogContentContainer.appendChild(noteDialogContent);
    d.AddContent(emptyDialogContent);
    
    d.Hide();
}



addEvent(window, "load", Init);