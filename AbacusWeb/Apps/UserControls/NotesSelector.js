var NotesSelector_LookupService;
var NotesSelector_ResultsTable;
var NotesSelector_PagingLinks;
var NotesSelector_CurrentPage;
var NotesSelector_SelectedID;
var NotesSelector_Notes;
var NotesSelector_QsNotesID = "ntID";
var NotesSelector_NoteCategoryID;
var NotesSelector_cboNoteCategoriesID = "DropDownListFilter";
var NotesSelector_cboNoteCategories;
var NotesSelector_NoteTypeID;
var NotesSelector_NoteTypeChildID;
var CONST_DROP_DOWN_LIST_SUFFIX = "_cboDropDownList";
var CONST_PAGE_SIZE = 10;
var NotesSelector_cboNoteCategories_SelectedIndex;
var NotesSelector_ViewNoteInNewWindow;
var CONST_NOTE_LENGTH = 100;
var NotesSelector_btnList;
var CONST_REPORT_BUTTON_SUFFIX = "_btnReports";
var NotesSelector_ItemWidth = '50em';
var NotesSelector_ItemHeight = '30em';

function NotesSelector_Init() {

    //get the web service
    NotesSelector_LookupService = new Target.Abacus.Web.Apps.WebSvc.Notes_class();

    //get screen elements in to javascript
    NotesSelector_ResultsTable = GetElement("tblNotes");
    NotesSelector_PagingLinks = GetElement("Notes_PagingLinks");
    NotesSelector_cboNoteCategories = GetElement(NotesSelector_cboNoteCategoriesID, true);
    NotesSelector_btnList = GetElement(NotesSelector_btnListID + CONST_REPORT_BUTTON_SUFFIX, true);
    
    //wire up the event handler for onchange events
    NotesSelector_cboNoteCategories.onchange = NotesSelector_cboNoteCategories_OnSelectedIndexChanged;

    //disable buttons
    if (NotesSelector_btnView) NotesSelector_btnView.disabled = true;
    if (NotesSelector_btnEdit) NotesSelector_btnEdit.disabled = true;

    //populate note categories drop down
    Populate_cboNoteCategories(NotesSelector_NoteTypeID, NotesSelector_NoteTypeChildID);

    //set the note categories drop down selected index
    NotesSelector_cboNoteCategories.selectedIndex = (NotesSelector_cboNoteCategories_SelectedIndex > 0) ? NotesSelector_cboNoteCategories_SelectedIndex : 0;

    //fetch notes
    NotesSelector_FetchNotes(NotesSelector_CurrentPage, NotesSelector_SelectedID);

    //add report buttons parameters
    if (NotesSelector_btnList) {
        ReportsButton_AddParam(NotesSelector_btnList.id, "NoteTypeChildID", NotesSelector_NoteTypeChildID);
        ReportsButton_AddParam(NotesSelector_btnList.id, "NoteTypeID", NotesSelector_NoteTypeID);
    }
}


function NotesSelector_FetchNotes(page, selectedID) {
    DisplayLoading(true);
    if (page == undefined) page = 0;
    if (selectedID == undefined) selectedID = 0;
    if (NotesSelector_NoteCategoryID == undefined) NotesSelector_NoteCategoryID = 0;
    NotesSelector_CurrentPage = page;
    NotesSelector_LookupService.GetPagedNotes(page, CONST_PAGE_SIZE, NotesSelector_NoteCategoryID, NotesSelector_NoteTypeID, NotesSelector_NoteTypeChildID, selectedID, NotesSelector_FetchNotes_Callback);
}

function NotesSelector_FetchNotes_Callback(response) {

    var itemSelected = false;

    NotesSelector_Notes = null;

    if (CheckAjaxResponse(response, NotesSelector_LookupService.url)) {

        var index, tr, td, radioButton, str, link, sCount, currentNote;

        NotesSelector_Notes = response.value.Items;

        if (NotesSelector_Notes) {
            sCount = NotesSelector_Notes.length;
        } else {
            sCount = 0;
        }

        ClearTable(NotesSelector_ResultsTable);

        if (sCount > 0) {

            for (index = 0; index < sCount; index++) {

                // set the current record
                currentNote = NotesSelector_Notes[index];

                // create row and add radio button
                tr = AddRow(NotesSelector_ResultsTable);
                td = AddCell(tr, "");
                radioButton = AddRadio(td, "", "NotesSelect", currentNote.ID, NotesSelector_RadioButton_Click);

                // add other items
                NotesSelector_AddCell(tr, Date.strftime("%d/%m/%Y", currentNote.CreatedDate));
                NotesSelector_AddCell(tr, currentNote.CreatedBy);
                NotesSelector_AddCell(tr, currentNote.NoteCategoryDescription);                
                NotesSelector_AddLink(tr, currentNote.Notes.substring(0,100), currentNote.ID);

                // select the item?
                if (NotesSelector_SelectedID == currentNote.ID || NotesSelector_Notes.length == 1) {
                    radioButton.click();
                    itemSelected = true;
                    if (NotesSelector_btnNew) NotesSelector_btnNew.disabled = false;
                    if (NotesSelector_btnView) NotesSelector_btnView.disabled = false;
                    if (NotesSelector_btnEdit) NotesSelector_btnEdit.disabled = false;
                }

            }

        }

        // load the paging link HTML
        NotesSelector_PagingLinks.innerHTML = response.value.PagingLinks;
    }

    if (itemSelected == false) {

        if (typeof NotesSelector_SelectedItemChanged == "function") {
            NotesSelector_SelectedItemChanged(null);
        }

    }

    DisplayLoading(false);
    //resize the parent frame
    resizeIframeIfControlInIframe();
}

function NotesSelector_AddLink(tr, str, NotesID) {

    if (NotesSelector_btnView) {
        var td;

        td = AddCell(tr, "");
        link = AddLink(td, str, NotesSelector_GetPlanURL(NotesID), "Click here to view this note.");
        link.className = "transBg";

        return td;
    }
    else {
        NotesSelector_AddCell(tr, str)
    }
}

function NotesSelector_AddCell(tr, str) {

    var td;
    if (str.length == 0) str = " ";
    td = AddCell(tr, str);
    td.style.textOverflow = "ellipsis";
    td.style.overflow = "hidden";
    return td;

}

function NotesSelector_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = NotesSelector_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = NotesSelector_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = NotesSelector_ResultsTable.tBodies[0].rows[index];
            NotesSelector_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            NotesSelector_SelectedID = rdo.value;
            if (NotesSelector_btnView) NotesSelector_btnView.disabled = false;
            if (NotesSelector_btnEdit) NotesSelector_btnEdit.disabled = false;
            if (NotesSelector_btnList) {
                ReportsButton_AddParam(NotesSelector_btnList.id, "NoteTypeChildID", NotesSelector_NoteTypeChildID);
                ReportsButton_AddParam(NotesSelector_btnList.id, "NoteTypeID", NotesSelector_NoteTypeID);
            } 
        } 
        else {
            NotesSelector_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof NotesSelector_SelectedItemChanged == "function") {
        NotesSelector_SelectedItemChanged(NotesSelector_GetSelectedObject(NotesSelector_SelectedID));
    }
}

function NotesSelector_GetSelectedObject(id) {
    if (NotesSelector_Notes != null) {
        var collectionLength = NotesSelector_Notes.length;
        for (var j = 0; j < collectionLength; j++) {
            if (NotesSelector_Notes[j].ID == id) {
                return NotesSelector_Notes[j];
            }
        }
    }
}

function NotesSelector_BeforeNavigate() {

    var qsNotesToken = NotesSelector_QsNotesID;
    var originalID = GetQSParam(document.location.search, qsNotesToken);
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

    url = AddQSParam(RemoveQSParam(url, qsNotesToken), qsNotesToken, NotesSelector_SelectedID);
    SelectorWizard_newUrl = url;

    return true;

}

function NotesSelector_GetBackUrl() {
    var url = document.location.href;

    if (NotesSelector_SelectedID != 0)
        url = AddQSParam(RemoveQSParam(url, "id"), "id", NotesSelector_SelectedID);
    return escape(url);
}


function NotesSelector_GetPlanURL(id) {
    return "javascript:NotesSelector_ViewNote(" + id + ");";
}

function Populate_cboNoteCategories(noteTypeID, noteTypeChildID) {

    var response, noteCategoryRows, opt;

    response = NotesSelector_LookupService.FetchNoteCategoriesList(noteTypeID, noteTypeChildID);
    noteCategoryRows = response.value.Items;

    // clear
    NotesSelector_cboNoteCategories.options.length = 0;
    // add blank		
    opt = document.createElement("OPTION");
    //populate cboServiceOucomes
    NotesSelector_cboNoteCategories.options.add(opt);
    SetInnerText(opt, "All");
    opt.value = "0";

    if (noteCategoryRows != null) {
        for (index = 0, noteCategoryRowsLength = noteCategoryRows.length; index < noteCategoryRowsLength; index++) {
            opt = document.createElement("OPTION");
            NotesSelector_cboNoteCategories.options.add(opt);
            SetInnerText(opt, noteCategoryRows[index].Description);
            opt.value = noteCategoryRows[index].ID;
        }
    }
}

function NotesSelector_cboNoteCategories_OnSelectedIndexChanged() {
    NotesSelector_cboNoteCategories_SelectedIndex = NotesSelector_cboNoteCategories.selectedIndex;
    NotesSelector_NoteCategoryID = NotesSelector_cboNoteCategories.options[NotesSelector_cboNoteCategories_SelectedIndex].value;
    NotesSelector_Init();
}


function NotesSelector_btnView_Click() {

    if (NotesSelector_ViewNoteInNewWindow) {

        var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Notes/Note.aspx?id=" + NotesSelector_SelectedID + "&mode=1" + "&notetype=" + NotesSelector_NoteTypeID + "&notetypechildid=" + NotesSelector_NoteTypeChildID + "&backUrl=" + NotesSelector_GetBackUrl();
        var dialog = OpenDialog(url, NotesSelector_ItemWidth, NotesSelector_ItemHeight, window);
    }
    else {
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Notes/Note.aspx?id=" + NotesSelector_SelectedID + "&mode=1" + "&notetype=" + NotesSelector_NoteTypeID + "&notetypechildid=" + NotesSelector_NoteTypeChildID + "&backUrl=" + NotesSelector_GetBackUrl();
    }    
}

function NotesSelector_btnNew_Click() {

    if (NotesSelector_ViewNoteInNewWindow) {

        var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Notes/Note.aspx?id=" + NotesSelector_SelectedID + "&mode=2" + "&notetype=" + NotesSelector_NoteTypeID + "&notetypechildid=" + NotesSelector_NoteTypeChildID + "&backUrl=" + NotesSelector_GetBackUrl();
        var dialog = OpenDialog(url, NotesSelector_ItemWidth, NotesSelector_ItemHeight, window);
    }
    else {
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Notes/Note.aspx?id=" + NotesSelector_SelectedID + "&mode=2" + "&notetype=" + NotesSelector_NoteTypeID + "&notetypechildid=" + NotesSelector_NoteTypeChildID + "&backUrl=" + NotesSelector_GetBackUrl();
    }
}

function NotesSelector_btnEdit_Click() {

    if (NotesSelector_ViewNoteInNewWindow) {

        var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Notes/Note.aspx?id=" + NotesSelector_SelectedID + "&mode=3" + "&notetype=" + NotesSelector_NoteTypeID + "&notetypechildid=" + NotesSelector_NoteTypeChildID + "&backUrl=" + NotesSelector_GetBackUrl();
        var dialog = OpenDialog(url, NotesSelector_ItemWidth, NotesSelector_ItemHeight, window);
    }
    else {
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Notes/Note.aspx?id=" + NotesSelector_SelectedID + "&mode=3" + "&notetype=" + NotesSelector_NoteTypeID + "&notetypechildid=" + NotesSelector_NoteTypeChildID + "&backUrl=" + NotesSelector_GetBackUrl();
    }
}

function NotesSelector_ViewNote(id) {

    if (id == undefined || id <= 0) {
        id = NotesSelector_SelectedID;
    }

    if (id > 0) {
        if (NotesSelector_ViewNoteInNewWindow) {
            var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Notes/Note.aspx?id=" + id + "&mode=1" + "&notetype=" + NotesSelector_NoteTypeID + "&notetypechildid=" + NotesSelector_NoteTypeChildID + "&backUrl=" + NotesSelector_GetBackUrl();
            var dialog = OpenDialog(url, NotesSelector_ItemWidth, NotesSelector_ItemHeight, window);
        } else {
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Notes/Note.aspx?id=" + id + "&mode=1" + "&backUrl=" + NotesSelector_GetBackUrl();
        }
    } else {
        alert("Please select a Note");
    }
}

function NotesSelector_btnList_Click() {

    resizeIframeIfControlInIframe();
}

function resizeIframeIfControlInIframe() {
    //resize the parent frame if the name of the frame is ifrNotes
    if (parent.ifrNotes) {
        parent.resizeIframe(document.body.scrollHeight, 'ifrNotes');
    }
}

addEvent(window, "load", NotesSelector_Init);