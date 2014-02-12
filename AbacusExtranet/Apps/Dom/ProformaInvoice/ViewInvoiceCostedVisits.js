
var contractSvc, currentPage, invoiceID, batchID, dteCopyWeekEndingID;
var selectedVisitID;
var tblInvoiceVisits, divPagingLinks, btnVisitDetails, btnVisitComponents;
var populating = false;
var rdbAllId = "rdbAll", rdbDifferentId = "rdbdifferent" , rdbSameId = "rdbSame";
var rdbAll, rdbDifferent, rdbSame, rdbSelectedValue;
var statusAwait, statusVerified;
var selectedInvoiceID, contractHasRoundingRules;

function Init() {
	contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
	tblInvoiceVisits = GetElement("tblInvoiceVisits");
	divPagingLinks = GetElement("Invoice_PagingLinks");
	btnVisitDetails = GetElement("btnVisitDetails");
	btnVisitComponents = GetElement("btnVisitComponents");
	
	rdbAll = GetElement(rdbAllId);
	rdbDifferent = GetElement(rdbDifferentId);
	rdbSame = GetElement(rdbSameId);

	rdbSelectedValue = GetQSParam(document.location.search, "dcr");
	if (rdbSelectedValue == "all") {
	    rdbAll.checked = true;
	    rdbSelectedValue = '';
	} else if (rdbSelectedValue == "different") {
	    rdbDifferent.checked = true;
	} else if (rdbSelectedValue == "same") {
	    rdbSame.checked = true;
	}
	
	selectedVisitID = GetQSParam(document.location.search, "visitID");
	if (!selectedVisitID) selectedVisitID = 0;
	
	statusAwait = GetQSParam(document.location.search, "await");
	statusVerified = GetQSParam(document.location.search, "ver");
	selectedInvoiceID = GetQSParam(document.location.search, "ver");
	
    FetchInvoiceVisitList(currentPage, selectedVisitID);
}
function FetchInvoiceVisitList(page, visitID) {
    currentPage = page;
	DisplayLoading(true);
	
	btnVisitDetails.disabled = true;
	btnVisitComponents.disabled = true;
	
	contractSvc.FetchDomProformaInvoiceCostedVisits(currentPage, invoiceID, visitID, rdbSelectedValue, FetchInvoiceVisitList_Callback);
}
function FetchInvoiceVisitList_Callback(response) {
    var index, visits, str, roundingUsed, img, imgSrc, rounding;
    var defaultDate = "Fri Dec 31 23:59:59 UTC 9999";
    
    if(CheckAjaxResponse(response, contractSvc.url)) {
        visits = response.value.InvoiceCostedVisits;
        ClearTable(tblInvoiceVisits);
        
        populating = true;
        
        for(index=0; index<visits.length; index++) {
        
            tr = AddRow(tblInvoiceVisits);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "VisitSelect", visits[index].InvoiceVisitID, RadioButton_Click);
			
			AddCell(tr, Date.strftime("%d/%m/%Y", visits[index].VisitDate));
			AddCell(tr, Date.strftime("%H:%M", visits[index].StartTime));
            // service type
			AddCell(tr, visits[index].ServiceType);
			//SecondaryVisit
			
			if (visits[index].SecondaryVisit == true) {
			    AddCell(tr, "Yes");
			}
			else {
			    AddCell(tr, "No");
			}
			//SecondaryVisitAutoSet
			if (visits[index].SecondaryVisitAutoSet == true) {
			    AddCell(tr, "Yes");
			}
			else{
			    AddCell(tr, "No");
			}

			// NumberOfCarers
			AddCell(tr, visits[index].NumberOfCarers);
			//VisitCode
			AddCell(tr, visits[index].VisitCode);

			str = "<div style='float:left;'>";
            str = str + visits[index].DurationClaimed.getHours() + "h " + visits[index].DurationClaimed.getMinutes() + "m";
            roundingUsed = visits[index].PreRoundedDurationClaimed;
            if (roundingUsed == defaultDate) {
                roundingUsed = ""
            } 
             else {
                roundingUsed =
             visits[index].PreRoundedDurationClaimed.getHours() + "h " + visits[index].PreRoundedDurationClaimed.getMinutes() + "m"
            }
            if (roundingUsed.length != 0) {
                str = str + " <span class='warningText'>[" + roundingUsed + "]</span>";
            }
            str += "</div>";
            str += "<div  style='float:left;margin-top:0px;padding-top:0px; '>";
            if (visits[index].IgnoreRounding == true && contractHasRoundingRules == true) {
                imgSrc = SITE_VIRTUAL_ROOT + "Images/IgnoreRounding.png";
                str = str + "<img  src='" + imgSrc + "' alt='Duration Claimed Rounding rules are ignored' style='margin-left:5px;'  />";
            }
            str += "</div>";
            
            td = AddCell(tr, "");
            td.innerHTML = str;
			
			
			AddCell(tr, Math.floor(visits[index].DurationPaid / 60) + "h " +(visits[index].DurationPaid % 60) + "m" );
							
			td = AddCell(tr, "");
			td.innerHTML = visits[index].Payment.toString().formatCurrency();
			
			if(visits[index].InvoiceVisitID == selectedVisitID || ( currentPage == 1 && visits.length == 1))
			    radioButton.click();

			

        }
        
        populating = false;
        
        // load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}

function RadioButton_Click() {
    var index, rdo, selectedRow, batchTypeID, batchStatusID, currentUrl, newUrl;
	for (index = 0; index < tblInvoiceVisits.tBodies[0].rows.length; index++){
		rdo = tblInvoiceVisits.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblInvoiceVisits.tBodies[0].rows[index];
			tblInvoiceVisits.tBodies[0].rows[index].className = "highlightedRow";
			selectedVisitID = rdo.value;

			//Update Url In Session State, this is used by the back command
			currentUrl = document.location.href;
			currentUrl = currentUrl.substr(currentUrl.indexOf(SITE_VIRTUAL_ROOT));
			
			newUrl = AddQSParam(RemoveQSParam(currentUrl, "visitID"), "visitID", selectedVisitID);
			var $response = contractSvc.UpdateSessionBackURL(currentUrl, newUrl)
			if (!CheckAjaxResponse($response, contractSvc.url)) {
                return false;
            }
			
			btnVisitDetails.disabled = false;
			btnVisitComponents.disabled = false;
		} else {
			tblInvoiceVisits.tBodies[0].rows[index].className = ""
		}
	}

}


function btnVisitComponents_Click() {
    var pScheduleId = GetQSParam(document.location.search, "pScheduleId");
    document.location.href = "ViewInvoiceVisitComponents.aspx?id=" + selectedVisitID +
                             "&batchID=" + batchID +
                             "&invoiceID=" + invoiceID +
                             "&pScheduleId=" + pScheduleId +
                             "&await=" + statusAwait +
                             "&ver=" + statusVerified 
}

function btnVisitDetails_Click() {
    var pScheduleId = GetQSParam(document.location.search, "pScheduleId");
               
 
    document.location.href = "ViewInvoiceVisitDetails.aspx?id=" + selectedVisitID +
                             "&batchID=" + batchID +
                             "&invoiceID=" + invoiceID +
                             "&pScheduleId=" + pScheduleId +
                             "&await=" + statusAwait +
                             "&ver=" + statusVerified 
}

function checkedChanged(option) {
    var id = GetQSParam(document.location.search, "id");
    var pScheduleId = GetQSParam(document.location.search, "pScheduleId");
    rdbSelectedValue = GetQSParam(document.location.search, "dcr");
    if (option == "all") {
        rdbAll.checked = true;
        rdbSelectedValue = "all"
    } 
    else if (option == "different") {
        rdbDifferent.checked = true;
        rdbSelectedValue = "different"
    } 
    else if (option == "same") {
        rdbSame.checked = true;
        rdbSelectedValue = "same"
    }
    document.location.href = "ViewInvoiceCostedVisits.aspx?id=" + id + "&pScheduleId=" + pScheduleId + "&dcr=" + rdbSelectedValue;
}

//function GetBackUrl() {
//    var url = document.location.href;
//    url = AddQSParam(RemoveQSParam(url, "selectedInvoiceID"), "selectedInvoiceID", selectedInvoiceID);
//    return escape(url);
//}

//function btnBack_Click() {
//        var url = GetQSParam(document.location.search, "backUrl");
//        url = unescape(url);
//        document.location.href = url;
//}
/*
// Have commented out this block as the copy functionality is not required at present, but may be reinstated at a later date.
function btnCopy_Click() {
    var d = new Target.Web.Dialog.Msg();
    var emptyDialogContent, copyDialogContentContainer, copyDialogContent;
    
    d.SetType(4);   // OK/Cancel
    d.SetCallback(CopyDialog_Callback);
    d.SetTitle("Copy Invoice Batch");

    d.ClearContent();    
    emptyDialogContent = document.createElement("DIV");
    d.AddContent(emptyDialogContent);
    
    copyDialogContentContainer = GetElement("divCopyDialogContentContainer");
    copyDialogContent = copyDialogContentContainer.getElementsByTagName("DIV")[0];
    
    // swap nodes
    emptyDialogContent = d._content.removeChild(emptyDialogContent);
    copyDialogContent = copyDialogContentContainer.removeChild(copyDialogContent);
    copyDialogContentContainer.appendChild(emptyDialogContent);
    d.AddContent(copyDialogContent);
    
    d.ShowCloseLink(false);
    d.Show();
}
function CopyDialog_Callback(evt, args) {
    var d = args[0];
    var answer = args[1];
    var emptyDialogContent, copyDialogContentContainer, copyDialogContent;
    var valRequired, weekEndingDate, response;
    
    // answer == 1 means OK
    if(answer == 1) {
        if(Page_ClientValidate("Copy")) {
            weekEndingDate = GetElement(dteCopyWeekEndingID + "_txtTextBox").value;
            valRequired = GetElement(dteCopyWeekEndingID + "_valRequired");
            
            response = contractSvc.WeekendingDateValid(weekEndingDate.toDate()).value;
            if(response.Success == false) {
                alert(response.Message);
                return;
            } else {
                document.location.href = "ManualEnter.aspx?copyFromID=" + batchID + 
                                    "&copyFromWE=" + weekEndingDate + 
                                    "&backUrl=" + escape(document.location.href);
            }
        } else {
            return;
        }
    }
    copyDialogContentContainer = GetElement("divCopyDialogContentContainer");
    emptyDialogContent = copyDialogContentContainer.getElementsByTagName("DIV")[0];
    copyDialogContent = d._content.getElementsByTagName("DIV")[0];
    
    // swap nodes
    emptyDialogContent = copyDialogContentContainer.removeChild(emptyDialogContent);
    copyDialogContent = d._content.removeChild(copyDialogContent);
    copyDialogContentContainer.appendChild(copyDialogContent);
    d.AddContent(emptyDialogContent);
    
    d.Hide();
}
*/

addEvent(window, "load", Init);
