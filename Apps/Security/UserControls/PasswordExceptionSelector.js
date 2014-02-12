
var securitySvc, currentPage;
var PasswordException_selectedWordID;
var listFilter, listFilterWord = "", listFilterList = "";
var tblPEs, divPagingLinks;

function Init() {
	securitySvc = new Target.Web.Apps.Security.WebSvc.Security_class();
	tblPEs = GetElement("tblPEs");
	divPagingLinks = GetElement("PasswordException_PagingLinks");
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Word", GetElement("thWord"));
	listFilter.AddColumn("List", GetElement("thList"));
	
	// populate table
	FetchPeList(currentPage, PasswordException_selectedWordID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Word":
			listFilterWord = column.Filter;
			break;
		case "List":
			listFilterList = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchPeList(1, 0);
}

/* FETCH PE LIST METHODS */
function FetchPeList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
			
	securitySvc.FetchPasswordExceptionList(page, 
                            selectedID, 
                            listFilterWord,
                            listFilterList,
                            FetchPeList_Callback);
}

function FetchPeList_Callback(response) {
	var pes, peCounter;
	var tr, td, btn;
		
	if(CheckAjaxResponse(response, securitySvc.url)) {
	   
		pes = response.value.Words;

		// remove existing rows
		ClearTable(tblPEs);
		for(peCounter=0; peCounter<pes.length; peCounter++) {
			tr = AddRow(tblPEs);
			
			// word
			AddCell(tr, pes[peCounter].Second);
			
		    // list
			AddCell(tr, pes[peCounter].Third);
			
			// remove cell
			td = AddCell(tr, "");
			td.style.textAlign = "right";
			btn = AddButton(td, "Remove", "Click here to remove this word", btnRemove_Click, pes[peCounter].First);
			
			if(PasswordException_selectedWordID == pes[peCounter].First) tr.className = "highlightedRow";
			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function btnRemove_Click(evt, args) {
    if(window.confirm("Are you sure you wish to remove this word?")) {
        var wordID = args;
        DeletePasswordException(wordID);
    }
}

function DeletePasswordException(wordID) {
    DisplayLoading(true);
    securitySvc.DeletePasswordException(wordID, DeletePasswordException_Callback);
}
function DeletePasswordException_Callback(response) {
    if(CheckAjaxResponse(response, securitySvc.url)) {
        FetchPeList(currentPage, 0);
    }
    DisplayLoading(false);
}

function btnAdd_Click() {
    var dialog = new Target.Web.App.Security.Admin.PasswordExceptionDialog(btnAdd_Callback);
    dialog.Show();
}
function btnAdd_Callback(evt, args) {

    var dialog = args[0];
    var result = args[1];
    var newWord;
    
    if (result == 1) {
        // OK was clicked
        newWord = dialog.Word.value.trim();
        if(newWord.length == 0) {
            alert("Please enter a new password exception.");
            return;
        } else {
            AddPasswordException(newWord);
        }
    }
    dialog.Hide();
}

function AddPasswordException(newWord) {
    DisplayLoading(true);
    securitySvc.AddPasswordException(newWord, AddPasswordException_Callback);
}
function AddPasswordException_Callback(response) {
    if(CheckAjaxResponse(response, securitySvc.url)) {
        PasswordException_selectedWordID = response.value.Result;
        FetchPeList(currentPage, PasswordException_selectedWordID);
    }
    DisplayLoading(false);
}

/************************************/
/* NEW PASSWORD EXCEPTION DIALOG    */
/************************************/

addNamespace("Target.Web.App.Security.Admin.PasswordExceptionDialog");

Target.Web.App.Security.Admin.PasswordExceptionDialog = function(callback) {
 
	this.SetTitle("Add New Password Exception");
	this.SetWidth("39");
	
	this.ClearContent();
    this.AddContent(document.createTextNode("New Password Exception: "));    
    this.Word = AddInput(this._content, "txtWord", "text", "", "", "");

	// buttons
	this.ClearButtons();
	this.AddButton("OK", "", callback, new Array(this, 1));     // OK = 1
	this.AddButton("Cancel", "", callback, new Array(this, 2)); // Cancel = 2
}

// inherit from base
Target.Web.App.Security.Admin.PasswordExceptionDialog.prototype = new Target.Web.Dialog();

addEvent(window, "load", Init);
