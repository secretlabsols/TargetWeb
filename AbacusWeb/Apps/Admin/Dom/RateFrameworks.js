
var tblDetails, cboFrameworkType, chkUseEnhancedRateDays, stdButtonsMode;

function Init() {
    tblDetails = GetElement("tblDetails");
    cboFrameworkType = GetElement("cboFrameworkType_cboDropDownList");
    chkUseEnhancedRateDays = GetElement("chkUseEnhancedRateDays_chkCheckbox");
    cboFrameworkType_OnChange();
    chkUseEnhancedRateDays_Click();    
}
function chkUseEnhancedRateDays_Click() {
    
    var rows = tblDetails.tBodies[0].rows;
    var row, cell, cbo, val;
    
    for(index=0; index<rows.length; index++) {
        row = rows[index];
        if(row.cells.length > 2) {
            cell = row.cells[2];
            cbo = cell.getElementsByTagName("SELECT")[0];
            cbo.disabled = !chkUseEnhancedRateDays.checked;
            
            if(cbo.disabled) cbo.value = "";
            val = GetElement(cbo.id.replace("cboDropDownList", "valRequired"));
            ValidatorEnable(val, chkUseEnhancedRateDays.checked);
        } 
    }
}
function cboFrameworkType_OnChange() {
    if(cboFrameworkType.value != "1") {
        // 1 = Visit
        chkUseEnhancedRateDays.checked = false;
        chkUseEnhancedRateDays.disabled = true;
        chkUseEnhancedRateDays_Click();
    } else {
        if(stdButtonsMode == 2 || stdButtonsMode == 3) {
            // AddNew or Edit mode
            chkUseEnhancedRateDays.disabled = false;
        }
    }
}

addEvent(window, "load", Init);