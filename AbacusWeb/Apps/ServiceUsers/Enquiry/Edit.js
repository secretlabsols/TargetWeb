var clientID, administrationTabLoaded, addressesTabLoaded, financeTabLoaded, servicesTabLoaded, sdsContribsTabLoaded, documentsTabLoaded, expandChildrenPanels, selectedTabIndex;
var firedActiveTabChanged = false;
var notesTabLoaded;

function Init() 
{
    document.getElementById('ifrAdministration').src =  "Administration.aspx?clientid=" + clientID;
    administrationTabLoaded = true;
}

function GetBackUrl() {
    var url = document.location.href;
    url = RemoveQSParam(url, "clientid");
    url = AddQSParam(url, "clientid", clientID);
    
    return escape(url);
}

function resizeIframe(newHeight, iFrameName) 
{ 
  document.getElementById(iFrameName).style.height = parseInt(newHeight) + 15 + 'px'; 
  currentFrame = iFrameName;
} 

function ActiveTabChanged(sender, e) {
    var tabIndex = sender.get_activeTabIndex() + 1;
    var btnName;
    switch(tabIndex) {
        case 1:
            if (!administrationTabLoaded)
            { 
                document.getElementById('ifrAdministration').src =  "Administration.aspx?clientid=" + clientID;
                administrationTabLoaded = true;
            }
            break;
        case 2:
            if (!addressesTabLoaded) 
            {
                document.getElementById('ifrAddresses').src =  "Addresses.aspx?clientid=" + clientID;
                addressesTabLoaded = true;
            }
            break;
        case 3:
            if (!servicesTabLoaded) {
                document.getElementById('ifrServices').src = "Services.aspx?clientid=" + clientID;
                servicesTabLoaded = true;
            }
            break;
        case 4:
            if (!financeTabLoaded) 
            {
                document.getElementById('ifrFinance').src =  "Finance.aspx?clientid=" + clientID;
                financeTabLoaded =  true;
            }
            break;
        case 5:
            LoadSdsContribsTab();
            break;
        case 6:
            if (!notesTabLoaded) {
                document.getElementById('ifrNotes').src = "Notes.aspx?clientid=" + clientID + "&iframeid=ifrDocuments" + "&notetype=1";
                notesTabLoaded = true;
            }
            break;
        case 7:
            if (!documentsTabLoaded) {
                if (document.getElementById('ifrDocuments')) {
                    document.getElementById('ifrDocuments').src = "Documents.aspx?clientid=" + clientID + "&iframeid=ifrDocuments";
                    documentsTabLoaded = true;
                }
            }
            break;

    }
    if(!ie) 
        {
           if(firedActiveTabChanged) {
            firedActiveTabChanged = false;
            return;
        }
            firedActiveTabChanged = true;
        }
    }

    function LoadSdsContribsTab() {
        if (!sdsContribsTabLoaded) {
            document.getElementById('ifrSdsContribs').src = "SdsContributions.aspx?clientid=" + clientID + "&expanded=" + expandChildrenPanels;
            sdsContribsTabLoaded = true;
        }
    }

addEvent(window, "load", Init);