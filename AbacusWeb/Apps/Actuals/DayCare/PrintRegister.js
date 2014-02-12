var BLANK_ROWS;
BLANK_ROWS = 4;

function init() 
{
    setHeaders();
    var content = self.opener.document.getElementById('dvRegister').innerHTML;
    var reg = document.getElementById('register');
    reg.innerHTML = content;
    setDisabled(reg, "");
    
    var viewType = getURLParameter('printmode');
       
    if (viewType == 2)
    {   
        addRow('tbRegister');
        clearCheckBoxes('tbRegister');
        clearTextBoxes('tbRegister');
        clearCellBackGroundColour('tbRegister');
    }
    else
    {
        clearCheckBoxesOnClick('tbRegister');
    }
}

function setDisabled(el, val) 
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

function clearCheckBoxes(tblName)
{
    table = document.getElementById(tblName);
    inputs = table.getElementsByTagName('input');
    var checkboxes = new Array();
    for (i = 0, inputlength = inputs.length; i < inputlength; i++)
    {
        if (!inputs[i].length)
        {
            if (inputs[i].type == 'checkbox')
            checkboxes[checkboxes.length] = inputs[i];
        }     
        else
        {
            for(k = 0, inputilength=inputs[i].length; k < inputilength; k++)
            {
              if (inputs[i][k].type == 'checkbox')
                checkboxes[checkboxes.length] = inputs[i];
            }
        }
    }

    for (i = 0, checkboxlength=checkboxes.length; i < checkboxlength; i++)
    {
      checkboxes[i].checked = false;
      checkboxes[i].onclick = stopcheck;
    }
}

function clearTextBoxes(tblName)
{
    table = document.getElementById(tblName);
    inputs = table.getElementsByTagName('input');
    var txtboxes = new Array();
    for (i = 0, inputlength=inputs.length; i < inputlength; i++)
    {
        if (!inputs[i].length)
        {
            if (inputs[i].type == 'text')
            txtboxes[txtboxes.length] = inputs[i];
        }     
        else
        {
            for(k = 0, inputilength=inputs[i].length; k < inputilength; k++)
            {
              if (inputs[i][k].type == 'text')        
                txtboxes[txtboxes.length] = inputs[i];
            }
        }
    }

    for (i = 0, textboxlength=txtboxes.length; i < textboxlength; i++)
    {
      if( txtboxes[i].id.indexOf("totalactual") != -1 || txtboxes[i].id.indexOf("outcome") != -1 ) 
      {
            txtboxes[i].value="";
      }
      txtboxes[i].style.backgroundColor='#ffffff';
    }
}

function clearCellBackGroundColour(tblName) 
{
    table = document.getElementById(tblName);
    cells = table.getElementsByTagName('TD');
    for (var i=0, cellslength=cells.length; i<cellslength; i++)  
    {
      cells[i].style.backgroundColor='#ffffff';
    } 
}

function setHeaders()
{
     var regProviderVal = self.opener.document.getElementById('hidProvider').value;
     var regContractVal = self.opener.document.getElementById('hidContract').value;
     var regDayVal = self.opener.document.getElementById('hidDay').value;
     var regWeekEndingVal = self.opener.document.getElementById('hidWeekEnding').value;
     
     var regProvider = document.getElementById('txtProvider');
     var regContract = document.getElementById('txtContract');
     var regDay = document.getElementById('txtDay');
     var regWeekEnding = document.getElementById('txtWeekEnding');
     
     regProvider.value=regProviderVal;
     regContract.value=regContractVal;
     regDay.value=regDayVal;  
     regWeekEnding.value=regWeekEndingVal;
}

function stopcheck() 
{ 
   return false; 
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

function clearCheckBoxesOnClick(tblName)
{
    table = document.getElementById(tblName);
    inputs = table.getElementsByTagName('input');
    var checkboxes = new Array();
    for (i = 0, inputlength=inputs.length; i < inputlength; i++)
    {
        if (!inputs[i].length)
        {
            if (inputs[i].type == 'checkbox')
            checkboxes[checkboxes.length] = inputs[i];
        }     
        else
        {
            for(k = 0, inputilength=inputs[i].length; k < inputilength; k++)
            {
              if (inputs[i][k].type == 'checkbox')
                checkboxes[checkboxes.length] = inputs[i];
            }
        }
    }

    for (i = 0, checkboxlength=checkboxes.length; i < checkboxlength; i++)
    {
      checkboxes[i].onclick = stopcheck;
    }
}

function addRow(tableID) 
{  
	var table = document.getElementById(tableID);  
	var rowCount = table.tBodies[0].rows.length;  
		
	if (rowCount >= 1)
	{
	    var cellCount = table.tBodies[0].rows[0].cells.length;	    
	    
	    if (cellCount == 13)
	    {
	        for( var row = 0; row < BLANK_ROWS; row++ ) 
	        {
	              var newRow = table.tBodies[0].insertRow(rowCount);
    	      
                  for( var cell = 0; cell < cellCount; cell++ ) 
                  {
                        if (cell < 4)
                        {
                            var newCell = newRow.insertCell(cell); 
                            newCell.innerHTML = "&nbsp"; 
                            newCell.className = "a";
                        }
                        else if (cell < cellCount - 2)
                        {
                            var newCell = newRow.insertCell(cell); 
                            newCell.className = "a";
                            var element1 = document.createElement("input");  
                            element1.type = "checkbox";  
                            newCell.appendChild(element1);
                        }
                        else if (cell > cellCount - 3)
                        {
                            var newCell = newRow.insertCell(cell); 
                            newCell.innerHTML = "&nbsp"; 
                            newCell.className = "a";
                        }
                  }
            }    
        }
        else
	    {
	        var cellCount = table.tBodies[0].rows[0].cells.length;	    
	    
            for( var row = 0; row < BLANK_ROWS; row++ ) 
            {
                  var newRow = table.tBodies[0].insertRow(rowCount);
    	      
                  for( var cell = 0; cell < cellCount; cell++ ) 
                  {
                        if (cell < 3)
                        {
                            var newCell = newRow.insertCell(cell); 
                            newCell.innerHTML = "&nbsp"; 
                            newCell.className = "a";
                        }
                        else if (cell < cellCount - 2)
                        {
                            var newCell = newRow.insertCell(cell); 
                            newCell.className = "a";
                            var element1 = document.createElement("input");  
                            element1.type = "checkbox";  
                            newCell.appendChild(element1);
                        }
                        else if (cell > cellCount - 3)
                        {
                            var newCell = newRow.insertCell(cell); 
                            newCell.innerHTML = "&nbsp"; 
                            newCell.className = "a";
                        }
                  }
            }    
	    } 
	}	
}  

addEvent(window, "load", init);

