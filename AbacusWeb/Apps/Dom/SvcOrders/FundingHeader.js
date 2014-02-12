var tblProportions;

function Init() {
    tblProportions = GetElement("tblProportions");
    optOption_Click();
}

function optOption_Click() {
    
    var optApportion = GetElement("optApportion");
    var rows = tblProportions.tBodies[0].rows;
    var row, cell, cbo, val;
    
    for(index=0; index<rows.length; index++) {
        row = rows[index];
        if(row.cells.length > 3) {
            cell = row.cells[3];
            cbo = cell.getElementsByTagName("SELECT")[0];
            cbo.disabled = optApportion.checked;
            if(cbo.disabled) cbo.value = "";
        }
    }
}


addEvent(window, "load", Init);