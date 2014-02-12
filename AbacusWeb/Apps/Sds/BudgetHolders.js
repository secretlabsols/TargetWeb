var Edit_bhID, documentsTabLoaded = false, tabContainer, tabContainerID, chkIsGlobalID, chkIsGlobal;

function Init() {
    chkIsGlobal = GetElement(chkIsGlobalID + '_chkCheckbox');
    tabContainer = $find(tabContainerID);
    resetFormAvailability();
}

function resizeIframe(newHeight, iFrameName) {
    document.getElementById(iFrameName).style.height = parseInt(newHeight) + 15 + 'px';
    currentFrame = iFrameName;
}

function chkIsGlobal_OnChange(id) {
    resetFormAvailability();
}

function resetFormAvailability() {
    // timeout required for ie purposes...
    setTimeout(function () {
        tabContainer = $find(tabContainerID);
        tabContainer.get_tabs()[1].set_enabled(!chkIsGlobal.checked);
    }, 10);
}

function tabStrip_ActiveTabChanged(sender, args) {
    var hidSelectedTab = GetElement("hidSelectedTab");
    hidSelectedTab.value = sender.get_activeTab().get_headerText();

    switch (hidSelectedTab.value) {
        case "Documents":
            if (!documentsTabLoaded) {
                if (document.getElementById('ifrDocuments')) {
                    document.getElementById('ifrDocuments').src = "BudgetHoldersDocumentsTab.aspx?bhid=" + Edit_bhID + "&iframeid=ifrDocuments";
                    documentsTabLoaded = true;
                }
            }
            break;
    }
}

addEvent(window, "load", Init);