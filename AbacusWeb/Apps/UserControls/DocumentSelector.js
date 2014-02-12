
var DocumentSelector_currentPage, 
DocumentSelector_selectedDocumentID, DocumentSelector_selectedClientID, 
DocumentSelector_viewDocumentInNewWindow;

function Init() 
{}


function DocumentSelector_btnNew_Click() {
    
    if (DocumentSelector_viewDocumentInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/Enquiry/NewStatement.aspx?clientid=" + DocumentSelector_selectedClientID + "&mode=2", 75, 50, 1);
//        window.open(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/Enquiry/NewStatement.aspx?clientid=" + DocumentSelector_selectedClientID + "&mode=2", "mywindow");
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/Enquiry/NewStatement.aspx?clientid=" + DocumentSelector_selectedClientID + "&mode=2";
}

addEvent(window, "load", Init);
