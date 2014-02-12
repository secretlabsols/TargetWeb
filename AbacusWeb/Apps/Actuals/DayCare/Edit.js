
var registerID, cboServiceOutcomesID, statusID, errorLabelID, weekEnding, weekEndDay, editClick, pageTitle, maxWeekEnding;
var contractSvc, checkboxId, cboServiceOutcomes, planned, cellID, chkBoxSelected, lblError;
var cell, hidAttendanceID, hidOutcomeID, txtOutcomeID, hidChangedID;
var InPlaceClientSelector_currentID;
var edited = false;

//constants
TOTAL_ACTUAL_PREFIX = "ctl00_MPContent_totalactualU_";
TEXT_SUFFIX_ZERO = "_0";
TEXT_SUFFIX_ONE = "_1";
TOTAL_PLANNED_PREFIX = "ctl00_MPContent_totalplannedU_";
TOTAL_PLANNED_COLUMN_PREFIX = "ctl00_MPContent_totalplanned_";
TOTAL_ACTUAL_COLUMN_PREFIX = "ctl00_MPContent_totalactual_";
TOTAL_COLUMN_SUFFIX = "_column";

HIDDEN_OUTCOME_PREFIX = "ctl00_MPContent_houtcomeU_";
TEXT_OUTCOME_PREFIX = "ctl00_MPContent_outcomeU_";
HIDDEN_FIELD_SUFFIX = "_hidField";
TEXT_FIELD_SUFFIX = "_txtTextBox";


function Init() 
{
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	cboServiceOutcomes = GetElement(cboServiceOutcomesID + "_cboDropDownList",true);
	lblError = GetElement(errorLabelID,true);
	var txtStatus = GetElement(statusID);

	setWeekViewTotalColumnColours();

	if (txtStatus.value == "In Progress")
	{
	    if (document.getElementById('ctl00_MPContent_btnSubmit') != null)
	    {
	        document.getElementById('ctl00_MPContent_btnSubmit').disabled = false;
	    }
	}
	
	if (txtStatus.value == "Submitted")
	{
	    document.getElementById('ctl00_MPContent_stdButtons1_btnEdit').disabled = true;
	    document.getElementById('ctl00_MPContent_stdButtons1_btnDelete').disabled = true;
	    if (document.getElementById('ctl00_MPContent_btnUnSubmit') != null)
	    {
	        document.getElementById('ctl00_MPContent_btnUnSubmit').disabled = false;
	    }
	}
	
	if (txtStatus.value == "Processed")
	{   
	    if (document.getElementById('ctl00_MPContent_stdButtons1_btnDelete') != null)
	    {
	        document.getElementById('ctl00_MPContent_stdButtons1_btnDelete').disabled = true;
	    }
	}
	
	if (txtStatus.value == "Amended")
	{
	    if (document.getElementById('ctl00_MPContent_stdButtons1_btnDelete') != null)
	    {
	        document.getElementById('ctl00_MPContent_stdButtons1_btnDelete').disabled = true;
	    }
	    if (document.getElementById('ctl00_MPContent_btnSubmit') != null)
	    {
	        document.getElementById('ctl00_MPContent_btnSubmit').disabled = false;
	    }
	}
	
	if(editClick != undefined)
	{
	    if(editClick == "True")
	    {
	        document.getElementById('ctl00_MPContent_stdButtons1_ddView').disabled = true;
	        document.getElementById('ctl00_MPContent_stdButtons1_btnRefresh').disabled = true;
	        document.getElementById('ctl00_MPContent_stdButtons1_btnBack').disabled = true;
	         if (document.getElementById('ctl00_MPContent_btnSubmit') != null)
	        {
	            document.getElementById('ctl00_MPContent_btnSubmit').disabled = true;
	        }
	        document.getElementById('ctl00_MPContent_btnPrintRegister').disabled = true;
	        document.getElementById('ctl00_MPContent_btnPrintReport').disabled = true;
	    }
	}
	
    if (document.getElementById('ctl00_MPContent_stdButtons1_btnEdit') != null)
    {
        document.getElementById('ctl00_MPContent_stdButtons1_btnEdit').onclick = confirmation;
    }
    
    if (document.getElementById('ctl00_MPContent_stdButtons1_btnDelete') != null)
    {
        document.getElementById('ctl00_MPContent_stdButtons1_btnDelete').onclick = deleteConfirmation;
    }  
    
    if (document.getElementById('ctl00_MPContent_stdButtons1_btnCancel') != null)
    {
        document.getElementById('ctl00_MPContent_stdButtons1_btnCancel').onclick = cancelConfirmation;
    }

    if (ie6) {
        var tbl = GetElement("tbRegister");
        var firstRow = tbl.tBodies[0].rows[0];
        var cells = firstRow.getElementsByTagName("td");
        for (index = 0; index < cells.length; index++) {
            cells[index].style.paddingTop = "49px";
            cells[index].style.paddingRight = "4px";
        }
    }
    
}   

//object to hold in cell properties
cell = new Object();

//function used for the print view
function btnPrint_Click(buttonID) 
{
    var url
    
    if (buttonID.match("btnPrintReport") == "btnPrintReport")
    {       
        url = "PrintRegister.aspx?printmode=1";
    }
    else if (buttonID.match("btnPrintRegister") == "btnPrintRegister")
    {
        url = "PrintRegister.aspx?printmode=2";
    }    
    OpenPopup(url, 75, 50, 1);
}

//function used for updating txtStatus
function btnSubmit_Click(buttonID) 
{
    var button;
      
    if (buttonID.match("btnSubmit") == "btnSubmit")
    {       
        submitConfirmation();
    }
    if (buttonID.match("btnUnSubmit") == "btnUnSubmit")
    {
        unsubmitConfirmation();
    }  
}

function getCellInfoFromControlID(controlID, cellObject)
{
    var positionOfPrefixUnderScore, positionOfSuffixUnderScore;
    //strip out id for a row in vwRegisterWeek from the checkbox id
    positionOfPrefixUnderScore = controlID.indexOf('_')
    positionOfPrefixUnderScore = controlID.indexOf('_', positionOfPrefixUnderScore + 1);
    positionOfPrefixUnderScore = controlID.indexOf('_', positionOfPrefixUnderScore + 1);
    
    positionOfSuffixUnderScore = positionOfPrefixUnderScore;
    
    positionOfSuffixUnderScore = controlID.indexOf('_', positionOfSuffixUnderScore + 1);
    positionOfSuffixUnderScore = controlID.indexOf('_', positionOfSuffixUnderScore + 1);
    positionOfSuffixUnderScore = controlID.indexOf('_', positionOfSuffixUnderScore + 1);
    positionOfSuffixUnderScore = controlID.indexOf('_', positionOfSuffixUnderScore + 1);
    positionOfSuffixUnderScore = controlID.indexOf('_', positionOfSuffixUnderScore + 1);
    positionOfSuffixUnderScore = controlID.indexOf('_', positionOfSuffixUnderScore + 1);
    positionOfSuffixUnderScore = controlID.indexOf('_', positionOfSuffixUnderScore + 1);
    
    //get the id of the row from vwRegisterWeek
    cellObject.vwRegisterWeekID = controlID.substring(positionOfPrefixUnderScore + 1, positionOfSuffixUnderScore);
        
    /*get planned flag to identify if the checkbox was planned or if data exists for cells, 
    unplanned cells that have been created previously */
    cellObject.Planned = controlID.substring(positionOfSuffixUnderScore - 1, positionOfSuffixUnderScore);
        
    //get the cellID
    positionOfSuffixUnderScore = controlID.indexOf('_', positionOfSuffixUnderScore+1);
    cellObject.CellID = controlID.substring(positionOfPrefixUnderScore + 1, positionOfSuffixUnderScore);
      
    //get contract id

    positionOfPrefixUnderScore = cellObject.CellID.indexOf('_')
    positionOfSuffixUnderScore = cellObject.CellID.indexOf('_', positionOfPrefixUnderScore+1);
    cellObject.DomContractID = parseInt(cellObject.CellID.substring(positionOfPrefixUnderScore + 1, positionOfSuffixUnderScore));
    
	positionOfPrefixUnderScore = cellObject.CellID.indexOf('_');
    cellObject.ClientID = parseInt(cellObject.CellID.substring(0, positionOfPrefixUnderScore));
        
    //get the rate category
    positionOfPrefixUnderScore = cellObject.CellID.indexOf('_', positionOfPrefixUnderScore + 1);
    positionOfSuffixUnderScore = cellObject.CellID.indexOf('_', positionOfPrefixUnderScore + 1);
    cellObject.DomRateCategoryID = parseInt(cellObject.CellID.substring(positionOfPrefixUnderScore + 1, positionOfSuffixUnderScore));
      
    //get register id
    positionOfPrefixUnderScore = cellObject.CellID.indexOf('_', positionOfPrefixUnderScore + 1);
    positionOfSuffixUnderScore = cellObject.CellID.indexOf('_', positionOfPrefixUnderScore + 1);
    cellObject.RegisterID = parseInt(cellObject.CellID.substring(positionOfPrefixUnderScore + 1, positionOfSuffixUnderScore));
        
    //get the registerrow id
    positionOfPrefixUnderScore = cellObject.CellID.indexOf('_', positionOfPrefixUnderScore + 1);
    positionOfSuffixUnderScore = cellObject.CellID.indexOf("_", positionOfSuffixUnderScore + 1);
    cellObject.RegisterRowID = parseInt(cellObject.CellID.substring(positionOfPrefixUnderScore + 1, positionOfSuffixUnderScore)); 
        
    //get the registercolumn id
    positionOfPrefixUnderScore = cellObject.CellID.indexOf('_', positionOfPrefixUnderScore + 1);
    positionOfSuffixUnderScore = cellObject.CellID.indexOf("_", positionOfSuffixUnderScore + 1);
    cellObject.RegisterColumnID = parseInt(cellObject.CellID.substring(positionOfPrefixUnderScore + 1, positionOfSuffixUnderScore)); 
      
    //get the day ID
    positionOfSuffixUnderScore = cellObject.CellID.indexOf("_", positionOfSuffixUnderScore + 1);
    cellObject.DayOfWeek = parseInt(cellObject.CellID.substring(positionOfSuffixUnderScore + 1));

}

function getClientFromRowID(controlID) {
    var start, end, clientID;
    // controlID format for rows is: ctl00_MPContent_U_XXXXXX_235_76_2_2_6_1
    // XXXXXX is the clientID
    start = controlID.indexOf("_");
    start = controlID.indexOf("_", start + 1);
    start = controlID.indexOf("_", start + 1);
    start++;
    end = controlID.indexOf("_", start);
    clientID = parseInt(controlID.substring(start, end));
    return clientID;
}

function checkBox_Clicked(ckbID, cellID) {
    checkboxId = ckbID;
    hidOutcomeID = HIDDEN_OUTCOME_PREFIX + cellID + HIDDEN_FIELD_SUFFIX;
    txtOutcomeID = TEXT_OUTCOME_PREFIX + cellID;
        
    var serviceDaySpecified;
    var precluded;            
	var d = new Target.Web.Dialog.Msg();
    var emptyDialogContent, serviceOutcomeDialogContentContainer, serviceOutcomeDialogContent, response, opt;
    var chkBoxDay, contractID, myDate, serviceOutcomesRows, ratePreclusionList;
    
    chkBoxDay = GetElement(checkboxId);
    chkBoxSelected = (GetElement(checkboxId).checked ? "true" : "false");
    
    //prime the cell object
    getCellInfoFromControlID(ckbID, cell);
       
    if(cell.Planned == true && chkBoxSelected == "false" || chkBoxSelected == "true")
    {                
    
        //get date of cell
        myDate = new Date();
        myDate = contractSvc.GetDateFromWeekDayNumber(cell.DayOfWeek, weekEndDay, weekEnding).value;
        
        //Check if service specified on chosen day
        response = contractSvc.CheckServiceDay(cell.DomContractID, myDate, cell.DayOfWeek).value;
        
        serviceDaySpecified = true;
        
        if(response.Success == false) 
        {
           serviceDaySpecified = checkServiceDayConfirmation(cell.DayOfWeek);
           if (serviceDaySpecified == false  && chkBoxSelected == "false")
           {
                chkBoxDay.checked = true;  
           }
           if (serviceDaySpecified == false  && chkBoxSelected == "true")
           {
                chkBoxDay.checked = false;  
           }
        }    
        
        precluded = false;
        
        //get rate category preclusions
        response = contractSvc.FetchRatePreclusionsList(cell.DomContractID);
        ratePreclusionList = response.value.RatePreclusions;
        
        //check rate category to see if it is precluded
        if (ratePreclusionList != undefined)
        {
            precluded = checkPreclusions(ratePreclusionList, cell.DomRateCategoryID, cell.ClientID, cell.DayOfWeek);
            if (precluded == true  && chkBoxSelected == "true")
            {
                chkBoxDay.checked = false;  
            }
        }
                            
        if(serviceDaySpecified == true && precluded == false)
        {
            //get list of service outcome for cboServiceOutcomes
            response = contractSvc.FetchServiceOutcomeList(cell.DomContractID, myDate, chkBoxSelected);
            serviceOutcomesRows = response.value.ServiceOutcomes;
            
            if(serviceOutcomesRows.length == 0 && chkBoxSelected == "true")
            {
                alert("No attendance type service outcomes found");
                chkBoxDay.checked = false;
                return false;
            }
            
            if(serviceOutcomesRows.length == 0 && chkBoxSelected == "false")
            {
                alert("No non-attendance type service outcomes found");
                chkBoxDay.checked = true;
                return false;
            }
                                
            // clear
            cboServiceOutcomes.options.length = 0;
            // add blank		
            opt = document.createElement("OPTION");
            //populate cboServiceOucomes
            cboServiceOutcomes.options.add(opt);
            SetInnerText(opt, "");
            opt.value = "";
    	
	        for(index=0, serviceoutcomerowlength = serviceOutcomesRows.length; index<serviceoutcomerowlength; index++) 
	        {
	            opt = document.createElement("OPTION");
	            cboServiceOutcomes.options.add(opt);
	            SetInnerText(opt, serviceOutcomesRows[index].Description);
	            opt.value = serviceOutcomesRows[index].ID;
	        }
    	    
            d.SetType(4);   // OK/Cancel
            d.SetCallback(ServiceOutcomeDialog_Callback);
            d.SetTitle("Service Outcome");

            d.ClearContent();    
            emptyDialogContent = document.createElement("DIV");
            d.AddContent(emptyDialogContent);

            serviceOutcomeDialogContentContainer = GetElement("divServiceOutcomesDialogContentContainer");
            serviceOutcomeDialogContent = serviceOutcomeDialogContentContainer.getElementsByTagName("DIV")[0];

            // swap nodes
            emptyDialogContent = d._content.removeChild(emptyDialogContent);
            serviceOutcomeDialogContent = serviceOutcomeDialogContentContainer.removeChild(serviceOutcomeDialogContent);
            serviceOutcomeDialogContentContainer.appendChild(emptyDialogContent);
            d.AddContent(serviceOutcomeDialogContent);

            d.ShowCloseLink(false);
            d.Show();
        } 
    }
    else
    {
        updatehidOutcome(hidOutcomeID,0);
        updatetxtOutcome(txtOutcomeID,"");
        updateTotalActual(chkBoxSelected, cell);        
        edited = true;
    }
}

function updatehidOutcome(hidattendanceControlId, serviceOutcomeID)
{
    var attendControl = GetElement(hidattendanceControlId);
    attendControl.value = serviceOutcomeID;
}

function updatetxtOutcome(txtoutcomeControlId, serviceOutcomeID)
{
    var outControl = GetElement(txtoutcomeControlId);
    
    if(serviceOutcomeID != "")
    {
        var serviceCode;
        serviceCode = contractSvc.GetServiceOutcomeCode(serviceOutcomeID).value;
        SetInnerText(outControl, serviceCode.Value);
    }
    else
    {
        outControl = GetElement(txtoutcomeControlId);
        SetInnerText(outControl, "");
    }
}

function ServiceOutcomeDialog_Callback(evt, args) 
{
    var d = args[0];
    var answer = args[1];
    var emptyDialogContent, serviceOutcomeDialogContentContainer, serviceOutcomeDialogContent;
    var newServiceOutcome;   
    var chkBoxDay, ckBoxChecked;  
    
    // answer == 1 means OK
    if(answer == 1) 
    {
        if(Page_ClientValidate("ServiceOutcomes")) 
        {
            newServiceOutcome = cboServiceOutcomes.value;
            if (cell.Planned == 1 || cell.Planned == 0)
            {     
                updatehidOutcome(hidOutcomeID, newServiceOutcome);
                updatetxtOutcome(txtOutcomeID, newServiceOutcome);
                edited = true;
            }
        } 
        else 
        {   
            if (cell.Planned != 1 && cell.Planned != 0) alert("Planned Variable Not Set");
            return;
        }
         //update total actual
          ckBoxChecked = (GetElement(checkboxId).checked ? "true" : "false"); 
          updateTotalActual(ckBoxChecked, cell);
    }
    else
    {
        chkBoxDay = GetElement(checkboxId);
        ckBoxChecked = (GetElement(checkboxId).checked ? "true" : "false");  
        if (ckBoxChecked == "false") chkBoxDay.checked = true;
        if (ckBoxChecked == "true") chkBoxDay.checked = false;     
        edited = true;
    }
    serviceOutcomeDialogContentContainer = GetElement("divServiceOutcomesDialogContentContainer");
    emptyDialogContent = serviceOutcomeDialogContentContainer.getElementsByTagName("DIV")[0];
    serviceOutcomeDialogContent = d._content.getElementsByTagName("DIV")[0];
    
    // swap nodes
    emptyDialogContent = serviceOutcomeDialogContentContainer.removeChild(emptyDialogContent);
    serviceOutcomeDialogContent = d._content.removeChild(serviceOutcomeDialogContent);
    serviceOutcomeDialogContentContainer.appendChild(serviceOutcomeDialogContent);
    d.AddContent(emptyDialogContent);
    
    d.Hide();
}

function NewEnquiry() 
{
 document.location.href = "List.aspx?datefrom=";
}

function InPlaceClientSelector_btnFind_Click(id, mode) 
{
    if (addNewServiceUserConfirmation() == true)
    {
	    var txtReference, txtName, hidID;
    	
	    InPlaceClientSelector_currentID = id;
    	
	    txtReference = "";
	    txtName = "";
	    hidID = "";
    	
        var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/Clients.aspx?ref=" + 
		    txtReference.value + "&name=" + txtName.value + "&clientID=" + hidID.value + "&mode=" + mode;
        var dialog = OpenDialog(url, 60, 32, window);
    }
}

function InPlaceClient_ItemSelected(id, reference, name) 
{   
   var response;
   response = contractSvc.PrimeRegisterByUser(id, registerID, pageTitle).value;
   if(response.Success == false) 
    {
        alert("Unable to add service user to register, the service user may already exist in this register. Please check and uncheck the relevant rate categories in the register for this service user.");
        return;
    }
    else
    {
        GetParentWindow().document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Actuals/DayCare/Edit.aspx?mode=1&ID=" + registerID + "&contractid=0" + "&backUrl=" + getURLParameter('backUrl');
        
    }
}

function setDisabled(el,val) 
{
    try
    {
        el.disabled = val;
    }
    catch(e)
    {
    }

    if (el.childNodes && el.childNodes.length > 0)
    {
        for (var x = 0, childnodeslength=el.childNodes.length; x < childnodeslength; x++)
        {
            setDisabled(el.childNodes[x],val);
        }
    }
}

function updateTotalActual(ckbValue, cellObj)
{   
    var totalActual1 = TOTAL_ACTUAL_PREFIX + cellObj.ClientID + "_" + cellObj.DomContractID + "_" +  cellObj.DomRateCategoryID + "_" +
                        cellObj.RegisterID + "_" + cellObj.RegisterRowID + "_" + cellObj.RegisterColumnID + TEXT_SUFFIX_ZERO;       
    var totalActual2 = TOTAL_ACTUAL_PREFIX + cellObj.ClientID + "_" + cellObj.DomContractID + "_" +  cellObj.DomRateCategoryID + "_" +
                        cellObj.RegisterID + "_" + cellObj.RegisterRowID + "_" + cellObj.RegisterColumnID + TEXT_SUFFIX_ONE;
    var totalPlanned1 = TOTAL_PLANNED_PREFIX + cellObj.ClientID + "_" + cellObj.DomContractID + "_" +  cellObj.DomRateCategoryID + "_" +
                        cellObj.RegisterID + "_" + cellObj.RegisterRowID + "_" + cellObj.RegisterColumnID + TEXT_SUFFIX_ZERO; 
    var totalPlanned2 = TOTAL_PLANNED_PREFIX + cellObj.ClientID + "_" + cellObj.DomContractID + "_" +  cellObj.DomRateCategoryID + "_" +
                        cellObj.RegisterID + "_" + cellObj.RegisterRowID + "_" + cellObj.RegisterColumnID + TEXT_SUFFIX_ONE;
                        
    var totalPlannedDayView = TOTAL_PLANNED_PREFIX + cellObj.ClientID + "_" + cellObj.DomContractID + "_" + 0 + "_" +
                        cellObj.RegisterID + "_" + cellObj.RegisterRowID + "_" + 0 + TEXT_SUFFIX_ZERO;
    
    var totalActualDayView = TOTAL_ACTUAL_PREFIX + cellObj.ClientID + "_" + cellObj.DomContractID + "_" +  0 + "_" +
                        cellObj.RegisterID + "_" + cellObj.RegisterRowID + "_" + 0 + TEXT_SUFFIX_ZERO;
                        
    var totalPlannedColumn = TOTAL_PLANNED_COLUMN_PREFIX + getDayOfWeek(cellObj.DayOfWeek) + TOTAL_COLUMN_SUFFIX;
    
    var totalActualColumn = TOTAL_ACTUAL_COLUMN_PREFIX + getDayOfWeek(cellObj.DayOfWeek) + TOTAL_COLUMN_SUFFIX; 
    
    var totalPlannedColumnDayView = TOTAL_PLANNED_COLUMN_PREFIX + cellObj.DomRateCategoryID + TOTAL_COLUMN_SUFFIX;
    var totalActualColumnDayView = TOTAL_ACTUAL_COLUMN_PREFIX + cellObj.DomRateCategoryID + TOTAL_COLUMN_SUFFIX; 
                                                  
    if (checkObjectExists(totalActual1))
    {        
        checkTotals(totalActual1, totalPlanned1, ckbValue);     
    }
    else if (checkObjectExists(totalActual2))
    {       
        checkTotals(totalActual2, totalPlanned2, ckbValue);
    }   
	
	if (checkObjectExists(totalActualDayView))
    {   
        checkTotals(totalActualDayView, totalPlannedDayView, ckbValue);       
    }	
    
    if (checkObjectExists(totalPlannedColumn))
    {        
        checkTotals(totalActualColumn, totalPlannedColumn, ckbValue); 
    }
    
    if (checkObjectExists(totalPlannedColumnDayView))
    {        
        checkTotals(totalActualColumnDayView, totalPlannedColumnDayView, ckbValue); 
    }
}

function setWeekViewTotalColumnColours()
{ 
   var totalPlannedColumnCheck = TOTAL_PLANNED_COLUMN_PREFIX + getDayOfWeek(0) + TOTAL_COLUMN_SUFFIX;
 
   if (checkObjectExists(totalPlannedColumnCheck))
   { 
      var cellReference, totalPlannedColumn, totalActualColumn, totalActualControl, totalPlannedControl, totalActual, totalPlanned;
     
       for(var i = 0; i <= 6; i++)
       {
          totalPlannedColumn = TOTAL_PLANNED_COLUMN_PREFIX + getDayOfWeek(i) + TOTAL_COLUMN_SUFFIX;
          totalActualColumn = TOTAL_ACTUAL_COLUMN_PREFIX + getDayOfWeek(i) + TOTAL_COLUMN_SUFFIX;
           
          totalActualControl = GetElement(totalActualColumn);
          totalPlannedControl = GetElement(totalPlannedColumn);
          
          totalActual = totalActualControl.value;
          totalActual = parseInt(totalActual, 10);
          totalPlanned = totalPlannedControl.value;
          totalPlanned = parseInt(totalPlanned, 10);
          
          setActualCellColour(totalActual, totalPlanned, cellReference, totalActualControl, totalActualColumn); 
       }
   }
}

function checkTotals(actual, planned, ckbValue)
{
    var totalActualControl;
    var totalPlannedControl;
    var totalActual, totalPlanned;
    var cellReference;
    
    totalActualControl = GetElement(actual);
    totalPlannedControl = GetElement(planned); 
        
    if ((ckbValue == "false") && (GetInnerText(totalActualControl) != "0"))
    {
        totalActual = GetInnerText(totalActualControl);
        totalActual = parseInt(totalActual, 10) - 1; 
        SetInnerText(totalActualControl, totalActual);
    }
    else if (ckbValue == "true")
    {
       totalActual = GetInnerText(totalActualControl);
       totalActual = parseInt(totalActual, 10) + 1;
       SetInnerText(totalActualControl, totalActual);
    }
    totalPlanned = GetInnerText(totalPlannedControl); 
    setActualCellColour(totalActual, totalPlanned, cellReference, totalActualControl, actual);   
}

function getDayOfWeek(dayOfWeek)
{
    switch (dayOfWeek) {
        case "0" : return "sun"; 
        case "1" : return "mon"; 
        case "2" : return "tue"; 
        case "3" : return "wed"; 
        case "4" : return "thu"; 
        case "5" : return "fri"; 
        case "6" : return "sat";
        default: break; 
        }
        
    switch (dayOfWeek) {
        case 0 : return "sun"; 
        case 1 : return "mon"; 
        case 2 : return "tue"; 
        case 3 : return "wed"; 
        case 4 : return "thu"; 
        case 5 : return "fri"; 
        case 6 : return "sat"; 
        default: return "unknown";
        }
}

function setActualCellColour(totActual, totPlanned, cellRef, totActControl, totalActualID)
{
    if ((totActual != undefined) && (totPlanned != undefined))
	{
		if (parseInt(totActual, 10) > parseInt(totPlanned, 10))
		{
			cellRef = document.getElementById(totalActualID).parentNode;
			cellRef.style.backgroundColor = "#ffddac";
			totActControl.style.backgroundColor = "#ffddac";
		}  
		else if (parseInt(totActual, 10) <= parseInt(totPlanned, 10))
		{
			cellRef = document.getElementById(totalActualID).parentNode;
			cellRef.style.backgroundColor = "#FFFFFF";
			totActControl.style.backgroundColor = "#FFFFFF";
		}
	}
}

function checkObjectExists(obj) 
{ 
    if (document.getElementById(obj)) 
    { 
        return true; 
    } 
    else 
    { 
        return false; 
    } 
} 

function getURLParameter(name)
{  
	name = name.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");  
	var regexS = "[\\?&]"+name+"=([^&#]*)";  
	var regex = new RegExp( regexS );  
	var results = regex.exec( window.location.href );  

	if( results == null )   
		 return "";  
	else    
		return results[1];
}

function confirmation()
{
    if (document.getElementById('ctl00_MPContent_stdButtons1_status_txtTextBox').value == "Processed") 
    {
        if (confirm("This register has already been processed. Click OK if your sure you want to amend it?") == true)
            {
                response = contractSvc.AmendRegister(registerID, document.getElementById('ctl00_MPContent_stdButtons1_status_txtTextBox').value, pageTitle).value;
                               
                if(response.Success == false) 
                {
                    alert(response.Message);
                    if (response.Message == "The register status has been ammended and must be re-read before it is edited")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        else
            return false;
    }
    else
    {
        response = contractSvc.AmendRegister(registerID, document.getElementById('ctl00_MPContent_stdButtons1_status_txtTextBox').value, pageTitle).value;
                               
        if(response.Success == false) 
        {
            if (response.Message != document.getElementById('ctl00_MPContent_stdButtons1_status_txtTextBox').value)
            {
                alert("The register has been changed and must be re-read before it can be edited");
                document.location.href = document.location.href;
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}

function deleteConfirmation()
{
    if(confirm("Are you sure you want to delete this register?") == true)
        {
            response = contractSvc.DeleteRegister(registerID, document.getElementById('ctl00_MPContent_stdButtons1_status_txtTextBox').value).value;
                           
            if(response.Success == false) 
            {
                alert(response.Message);
                if (response.Message == "Unable to retrieve register record. Please check if the register exists")
                    document.location.href = unescape(GetQSParam(document.location.search, "backUrl"));
                else
                    document.location.href = document.location.href;
                return false;
            }
            else
            {
                document.location.href = unescape(GetQSParam(document.location.search, "backUrl"));
                return false;
            }
        }
    else
        return false;
}

function saveConfirmation()
{
   alert("Unable to save changes, registers with the status of submitted or processed can not be amended");
   document.location.href = document.location.href;
   return false;

}

function ValidateWeekEndingDate() {
    if (maxWeekEnding != '') {       
        var dtWeekEnding = new Date(weekEnding.substring(6, 10), Number(weekEnding.substring(3, 5)) - 1, weekEnding.substring(0, 2));
        var dtMaxWeekEnding = new Date(maxWeekEnding.substring(6, 10), Number(maxWeekEnding.substring(3, 5)) - 1, maxWeekEnding.substring(0, 2));

        if (dtWeekEnding > dtMaxWeekEnding) {
            alert('Actual service for future periods may not be entered:\nThe current week ending date is ' + weekEnding + ', only registrations upto and including ' + maxWeekEnding + ' can be submitted.');
            return false;
        }
        else {
            return true;
        }        
    }
    else {
        return true;
    }
}

function submitConfirmation() {

    if (ValidateWeekEndingDate() && confirm("Are you sure you want to submit this register?") == true) {
    
        response = contractSvc.SubmitRegister(registerID, "Submitted", pageTitle).value;

        if (response.Success == false) {
            alert(response.Message);
            if (response.Message == "Unable to retrieve register record. Please check if the register exists")
                document.location.href = unescape(GetQSParam(document.location.search, "backUrl"));
            else
                document.location.href = document.location.href;
            return false;
        }
        else {
            document.location.href = document.location.href;
            return false;
        }
        
    }
    else {
    
        return false;
        
    }    
}

function unsubmitConfirmation()
{
    if(confirm("Are you sure you want to unsubmit this register?")== true)
        {
            response = contractSvc.SubmitRegister(registerID, "In Progress", pageTitle).value;
                           
            if(response.Success == false) 
            {
                alert(response.Message);
                if (response.Message == "Unable to retrieve register record. Please check if the register exists")
                    document.location.href = unescape(GetQSParam(document.location.search, "backUrl"));
                else
                    document.location.href = document.location.href;
                return false;
            }
            else
            {
                document.location.href = document.location.href;
                return false;
            }
        }
    else
        return false;
}

function cancelConfirmation()
{
    if(edited == true)
    {
        if(confirm("Are you sure you want to Cancel, any changes you have made to the register will be lost")== true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

function addNewServiceUserConfirmation() 
{
    if(edited == true)
    {
        alert("Changes made to the register must be saved before you can add a new service user")
        {
            return false;
        }
    }
    else
    {
        return true;
    }
}

function checkServiceDayConfirmation(dayOfWeek)
{
    if (chkBoxSelected == "true")
    {
        if(confirm("Are you sure you want to enter service on " + getFullDayOfWeek(dayOfWeek) + ", as it has not been specified on the service days node, within the relavant period for the contract specified on this register")== true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    else
    {
        return true;
    }
}

function getFullDayOfWeek(dayOfWeek)
{
    switch (dayOfWeek) {
        case "0" : return "Sunday"; 
        case "1" : return "Monday"; 
        case "2" : return "Tuesday"; 
        case "3" : return "Wednesday"; 
        case "4" : return "Thursday"; 
        case "5" : return "Friday"; 
        case "6" : return "Saturday";
        default: break; 
        }
        
    switch (dayOfWeek) {
        case 0 : return "Sunday"; 
        case 1 : return "Monday"; 
        case 2 : return "Tuesday"; 
        case 3 : return "Wednesday"; 
        case 4 : return "Thursday"; 
        case 5 : return "Friday"; 
        case 6 : return "Saturday"; 
        default: return "Unknown";
        }
}

function checkPreclusions(preclusionList, selectedRateCategory, clientID, day)
{

    var table = document.getElementById('tbRegister');
    var inputs;
    var checkboxes = new Array();
    var cellObj;
    cellObj = new Object();

    var preclusionCounter = 0;
    var preclusions = new Array();
    var preclusionDescriptions = new Array();
 
    for(index = 0, preclusionlistlength = preclusionList.length; index < preclusionlistlength; index++) 
    {
         if (selectedRateCategory == preclusionList[index].DomRateCategoryID)
         {
            preclusions[preclusionCounter] = preclusionList[index].PrecludedDomRateCategoryID;
            preclusionDescriptions[preclusionCounter] = preclusionList[index].pDescription;
            preclusionCounter ++;
         }
         if(selectedRateCategory == preclusionList[index].PrecludedDomRateCategoryID)
         {
            preclusions[preclusionCounter] = preclusionList[index].DomRateCategoryID;
            preclusionDescriptions[preclusionCounter] = preclusionList[index].rDescription;
            preclusionCounter ++;
         }
    }

    // loop through rows in the table
    for (index = 0, tablerowslength=table.rows.length; index < tablerowslength; index++) {
        // if we have a row for the correct client
        if (getClientFromRowID(table.rows[index].id) == clientID) {
            // grab the checkboxes for this row
            inputs = table.rows[index].getElementsByTagName('input');
            for (inputIndex = 0, inputlength=inputs.length; inputIndex < inputlength; inputIndex++) {
                if (inputs[inputIndex].type == 'checkbox')
                    checkboxes[checkboxes.length] = inputs[inputIndex];
            }
        }
    }

    // loop through checkboxes for the client
    for (i = 0, checkboxlength=checkboxes.length; i < checkboxlength; i++)
    {
       getCellInfoFromControlID(checkboxes[i].id, cellObj);
       
        if (cellObj.ClientID == clientID && cellObj.DomRateCategoryID != selectedRateCategory && cellObj.DayOfWeek == day)
        {
            if (checkboxes[i].checked == true)
            {
                for(index = 0, preclusionlength=preclusions.length; index < preclusionlength; index++) 
                {
                     if (cellObj.DomRateCategoryID == preclusions[index])
                     {
                        alert("You cannot tick this cell because " + preclusionDescriptions[index] + " is already ticked for this user on " + getFullDayOfWeek(day) +  ", and is precluded");
                        return true
                     }
                }
            }
        }
    }

    return false;
    
}

function unCheckAllClicked(dayOfWeek, weekEndDay, weekEnding, contractID)
{           
	var d = new Target.Web.Dialog.Msg();
    var emptyDialogContent, serviceOutcomeDialogContentContainer, serviceOutcomeDialogContent, response, opt;
    var myDate, serviceOutcomesRows;
    
    //get date of cell
    myDate = new Date();
    myDate = contractSvc.GetDateFromWeekDayNumber(dayOfWeek, weekEndDay, weekEnding).value;

    //get list of service outcome for cboServiceOutcomes
    response = contractSvc.FetchServiceOutcomeList(contractID, myDate, "false");
    serviceOutcomesRows = response.value.ServiceOutcomes;
    
    //
    if (serviceOutcomesRows == null || serviceOutcomesRows.length == 0)
    {
        alert("No non-attendance type service outcomes specified");
        return false;
    }
                        
    // clear
    cboServiceOutcomes.options.length = 0;
    // add blank		
    opt = document.createElement("OPTION");
    //populate cboServiceOucomes
    cboServiceOutcomes.options.add(opt);
    SetInnerText(opt, "");
    opt.value = "";

    for(index = 0, serviceoutcomerowslength=serviceOutcomesRows.length; index<serviceoutcomerowslength; index++) 
    {
        opt = document.createElement("OPTION");
        cboServiceOutcomes.options.add(opt);
        SetInnerText(opt, serviceOutcomesRows[index].Description);
        opt.value = serviceOutcomesRows[index].ID;
    }
    
    d.SetType(4);   // OK/Cancel
    d.SetCallback(unCheckAllDialog_Callback);
    d.SetTitle("Service Outcome");

    d.ClearContent();    
    emptyDialogContent = document.createElement("DIV");
    d.AddContent(emptyDialogContent);

    serviceOutcomeDialogContentContainer = GetElement("divServiceOutcomesDialogContentContainer");
    serviceOutcomeDialogContent = serviceOutcomeDialogContentContainer.getElementsByTagName("DIV")[0];

    // swap nodes
    emptyDialogContent = d._content.removeChild(emptyDialogContent);
    serviceOutcomeDialogContent = serviceOutcomeDialogContentContainer.removeChild(serviceOutcomeDialogContent);
    serviceOutcomeDialogContentContainer.appendChild(emptyDialogContent);
    d.AddContent(serviceOutcomeDialogContent);

    d.ShowCloseLink(false);
    d.Show();
}

function unCheckAllDialog_Callback(evt, args) 
{
    var d = args[0];
    var answer = args[1];
    var emptyDialogContent, serviceOutcomeDialogContentContainer, serviceOutcomeDialogContent;
    var newServiceOutcome;      
    var chkBoxDay, ckBoxChecked, checkboxId;  
    var hidOutcomeID, txtOutcomeID;
    
    table = document.getElementById('tbRegister');
    inputs = table.getElementsByTagName('input');
    var checkboxes = new Array();
    var cellObj;
    cellObj = new Object();
    
    // answer == 1 means OK
    if(answer == 1) 
    {
        if(Page_ClientValidate("ServiceOutcomes")) 
        {
            for (i = 0, inputlength=inputs.length; i < inputlength; i++)
            {
                if (!inputs[i].length)
                {
                    if (inputs[i].type == 'checkbox')
                    checkboxes[checkboxes.length] = inputs[i];
                }     
                else
                {
                    for(k = 0, inputsilength=inputs[i].length; k < inputsilength; k++)
                    {
                      if (inputs[i][k].type == 'checkbox')
                        checkboxes[checkboxes.length] = inputs[i];
                    }
                }
            }

            for (i = 0, checkboxlength=checkboxes.length; i < checkboxlength; i++)
            {
               getCellInfoFromControlID(checkboxes[i].id, cellObj);
               newServiceOutcome = cboServiceOutcomes.value;
            
                if (cellObj.Planned == 1 || cellObj.Planned == 0)
                {     
                    checkboxes[i].checked = false
                    
                    hidOutcomeID = HIDDEN_OUTCOME_PREFIX + cellObj.ClientID + "_" + cellObj.DomContractID + "_" +  cellObj.DomRateCategoryID + "_" +
                        cellObj.RegisterID + "_" + cellObj.RegisterRowID + "_" + cellObj.RegisterColumnID + "_" +
                        cellObj.Planned + "_" + cellObj.DayOfWeek +  HIDDEN_FIELD_SUFFIX;
                    txtOutcomeID = TEXT_OUTCOME_PREFIX + cellObj.ClientID + "_" + cellObj.DomContractID + "_" +  cellObj.DomRateCategoryID + "_" +
                        cellObj.RegisterID + "_" + cellObj.RegisterRowID + "_" + cellObj.RegisterColumnID + "_" +
                        cellObj.Planned + "_" + cellObj.DayOfWeek;
                                        
                    if (cellObj.Planned == true)
                    {
                        updatehidOutcome(hidOutcomeID, newServiceOutcome);
                        updatetxtOutcome(txtOutcomeID, newServiceOutcome);
                    }
                    else
                    {
                        updatehidOutcome(hidOutcomeID,0);
                        updatetxtOutcome(txtOutcomeID,"");
                    }
                    
                    //update total actual
                    ckBoxChecked = (GetElement(checkboxes[i].id).checked ? "true" : "false"); 
                    updateTotalActual(ckBoxChecked, cellObj);
                    edited = true;
                }
            }
        } 
        else 
        {   
            return;
        }
    }

    serviceOutcomeDialogContentContainer = GetElement("divServiceOutcomesDialogContentContainer");
    emptyDialogContent = serviceOutcomeDialogContentContainer.getElementsByTagName("DIV")[0];
    serviceOutcomeDialogContent = d._content.getElementsByTagName("DIV")[0];
    
    // swap nodes
    emptyDialogContent = serviceOutcomeDialogContentContainer.removeChild(emptyDialogContent);
    serviceOutcomeDialogContent = d._content.removeChild(serviceOutcomeDialogContent);
    serviceOutcomeDialogContentContainer.appendChild(serviceOutcomeDialogContent);
    d.AddContent(emptyDialogContent);
    
    d.Hide();
}

function Save_Validation(){
    var checkboxes = new Array();
    var cellObj = new Target.Abacus.Library.ViewableRegisterCell_class();
    var cellObjects = new Array();
    var index = 0;
    table = document.getElementById('tbRegister');
    inputs = table.getElementsByTagName('input');
    
        
    for (i = 0, inputlength=inputs.length; i < inputlength; i++)
    {
        if (!inputs[i].length)
        {
            if (inputs[i].type == 'checkbox')
            checkboxes[checkboxes.length] = inputs[i];
        }     
        else
        {
            for(k = 0, inputslength=inputs[i].length; k < inputslength; k++)
            {
              if (inputs[i][k].type == 'checkbox')
                checkboxes[checkboxes.length] = inputs[i];
            }
        }
    }

    //Loop around the checkboxes
    for (i = 0, checkboxlength=checkboxes.length; i < checkboxlength; i++)
    {
       cellObj = new Target.Abacus.Library.ViewableRegisterCell_class();
       getCellInfoFromControlID(checkboxes[i].id, cellObj);
       cellObj.Attended = checkboxes[i].checked;
       cellObj.Planned = cellObj.Planned == "1" ? true : false;
       if (cellObj.Attended == true){
            cellObjects[index] = cellObj;
            index++
       }
    }
    
    response = contractSvc.ValidateRegisterPriorToSave(cellObjects).value;

    if (response.length == 0)
    {
        return true;
    } else {
        lblError.innerText = response;
        return false;
    }

}

addEvent(window, "load", Init);