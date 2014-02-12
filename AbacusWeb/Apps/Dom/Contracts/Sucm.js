var rdoUseSystem, rdoUseLocal, divUseLocalSettings;

function Init() {
    optUseSettings_Click();
}

function optUseSettings_Click() {
    rdoUseSystem = GetElement("optUseSystemSettings");
    rdoUseLocal = GetElement("optUseLocalSettings");
    divUseLocalSettings = GetElement("divUseLocalSettings");
    
    if (rdoUseSystem.checked == true) {
        divUseLocalSettings.disabled = true;
    } else if (rdoUseLocal.checked == true) {
        divUseLocalSettings.disabled = false;
    }
}

addEvent(window, "load", Init);