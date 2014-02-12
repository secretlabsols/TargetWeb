
function Init() {
    var dlgDiv = $('#invNotesPopup');
    dlgDiv.dialog({
        autoOpen: false,
        draggable: true,
        modal: true,
        resizable: false,
        closeOnEscape: true,
        zIndex: 9999,
        minHeight: 400,
        minWidth: 800,
        title: "Provider-entered Invoice Note",
        buttons: {
            "Close": function() {
                cancelClicked($(this));
            }
        }
    });

}

function DisplayNotes() {
    var dlgDiv = $('#invNotesPopup');
    dlgDiv.dialog('open');
}

function cancelClicked() {
    var dlgDiv = $('#invNotesPopup');
    dlgDiv.dialog('close');
}


addEvent(window, "load", Init);