
function Init() {
    var backurl = GetQSParam(document.location.search, "backUrl");
    var backButton = GetElement("btnBack");
    var autoPopup = GetQSParam(document.location.search, "autopopup");

    if (!backurl && !autoPopup) {
        autoPopup = 0;
        backButton.style.display = 'none';
    }
}

function btnBack_Click() {

    var autoPopup = GetQSParam(document.location.search, "autopopup");

    if (!autoPopup)
        autoPopup = 0;

    if (autoPopup == 1)
        window.close();
    else
        document.location.href = unescape(GetQSParam(document.location.search, 'backUrl'));

}

addEvent(window, "load", Init);