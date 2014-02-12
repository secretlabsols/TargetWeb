var clientSelectorID, pctSelectorID, olaSelectorID, orgSelectorID;

function Init() {
    optType_Click();
}
function optType_Click() {
    var optCouncil, optPCT, optClient, optOther, optOrganization;
    var divClient, divPCT, divOLA, divORG;
    optCouncil = GetElement("optCouncil");
    optPCT = GetElement("optPCT");
    optClient = GetElement("optClient");
    optOther = GetElement("optOLA");
    optOrganization = GetElement("optOther");
    divClient = GetElement("divClient");
    divPCT = GetElement("divPCT");
    divOLA = GetElement("divOLA");
    divORG = GetElement("divORG");
    
//    ValidatorEnable(GetElement(pctSelectorID + "_valRequired"), false);
//    ValidatorEnable(GetElement(clientSelectorID + "_valRequired"), false);
//    ValidatorEnable(GetElement(clientSelectorID + "_clientSelector_valRequired"), false);
//    ValidatorEnable(GetElement(olaSelectorID + "_valRequired"), false);
//    ValidatorEnable(GetElement(orgSelectorID + "_valRequired"), false);
    
    divClient.style.display = "none";
    divPCT.style.display = "none";
    divOLA.style.display = "none";
    divORG.style.display = "none";
    
    if (optPCT.checked == true) {
        divPCT.style.display = "block";
        //ValidatorEnable(GetElement(pctSelectorID + "_valRequired"), true);
    }else if (optClient.checked == true) {
        divClient.style.display = "block";
        //ValidatorEnable(GetElement(clientSelectorID + "_valRequired"), true);
        //ValidatorEnable(GetElement(clientSelectorID + "_clientSelector_valRequired"), true);
    } else if (optOther.checked == true){
        divOLA.style.display = "block";
        //ValidatorEnable(GetElement(olaSelectorID + "_valRequired"), true);
    } else if (optOrganization.checked == true){
        divORG.style.display = "block";
        //ValidatorEnable(GetElement(orgSelectorID + "_valRequired"), true);
    }
    
}

addEvent(window, "load", Init);