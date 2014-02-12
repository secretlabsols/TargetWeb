
var dteCommitmentDateID, chkResidentialID, chkDPID, chkIncludeLastRunID;
var dteCommitmentDate, chkResidential, chkDP, chkIncludeLastRun;

function Init() {
    chkResidential = GetElement(chkResidentialID + "_chkCheckbox");
    chkDP = GetElement(chkDPID + "_chkCheckbox");
    chkIncludeLastRun = GetElement(chkIncludeLastRunID + "_chkCheckbox");
}

function chkIncludeLastRun_Click() {
    var sMsg;
    var change = "true";
        
    if (chkIncludeLastRun.checked == false) {
        sMsg = "WARNING"
        sMsg = sMsg + "\n\nUnticking this option will remove any commitment from the last run.";
        sMsg = sMsg + " This cannot be reversed once done. ";
        sMsg = sMsg + "You should only do this for the first run in the financial year.";
        sMsg = sMsg + "\n\nDo you still wish to remove the previous commitment?";
        
        change = confirm(sMsg);
        if (!change) {
            chkIncludeLastRun.checked = true;
        }
    }
}

addEvent(window, "load", Init);
