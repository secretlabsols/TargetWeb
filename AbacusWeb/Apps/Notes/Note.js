//variable to hold the selected id passed down to selector control
var selectedID, mode;

function Init() {
    //format the note textbox so that it is scrollable
    if (mode == Target.Library.Web.UserControls.StdButtonsMode.Fetched) {
        jQuery('[id*=txtNote]').removeAttr("disabled");  
        jQuery('[id*=txtNote]').attr("readonly") == true;
        jQuery('[id*=txtNote]').addClass("disabled");
    }
}

addEvent(window, "load", Init);
addEvent(window, "unload", NotesDialogUnload);

function NotesDialogUnload() {
    var parentWindow = GetParentWindow();
    GetParentWindow().HideModalDIV();
    parentWindow.NotesSelector_SelectedID = selectedID;
    parentWindow.NotesSelector_Init();
}

