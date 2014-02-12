
var contractSvc, currentPage, invoiceID, dteCopyWeekEndingID;
var selectedVisitID;
var tblInvoiceVisits, divPagingLinks, btnVisitDetails, btnVisitComponents;
var populating = false;
var pScheduleId;
var rdbAllId = "rdbAll", rdbDifferentId = "rdbdifferent", rdbSameId = "rdbSame";
var rdbAll, rdbDifferent, rdbSame, rdbSelectedValue;
var InvoiceHasNotes;
var customBackUrl;
var showPaymentSchedule;

function Init() {
    if (InvoiceHasNotes == 'true') {
        AddNotesLink();
    }
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
	if(!selectedVisitID) selectedVisitID = 0;

	FetchInvoiceVisitList(currentPage, selectedVisitID);

	pScheduleId = GetQSParam(document.location.search, "pScheduleId");

}
function FetchInvoiceVisitList(page, visitID) {
    currentPage = page;
	DisplayLoading(true);
	btnVisitDetails.disabled = true;
	btnVisitComponents.disabled = true;
	
	contractSvc.FetchDomProviderInvoiceCostedVisits(currentPage, invoiceID, visitID,rdbSelectedValue, FetchInvoiceVisitList_Callback);
}
function FetchInvoiceVisitList_Callback(response) {
    var index, visits, str, roundingUsed;
    var defaultDate = "Fri Dec 31 23:59:59 UTC 9999";
    
    if(CheckAjaxResponse(response, contractSvc.url)) {
        visits = response.value.InvoiceCostedVisits;
        ClearTable(tblInvoiceVisits);

        populating = true;
                
        for(index=0; index<visits.length; index++) {

            var absDurationPaid = Math.abs(visits[index].DurationPaid);
        
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
			else {
			    AddCell(tr, "No");
			}

			// NumberOfCarers
			AddCell(tr, visits[index].NumberOfCarers);
			//VisitCode
			AddCell(tr, visits[index].VisitCode);
			
			str = visits[index].DurationClaimed.getHours() + "h " + visits[index].DurationClaimed.getMinutes() + "m";
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

			td = AddCell(tr, "");
			td.innerHTML = str;  
			
			AddCell(tr, ((visits[index].DurationPaid < 0) ? "-" : "") + Math.floor(absDurationPaid / 60) + "h " + (absDurationPaid % 60) + "m");
							
			td = AddCell(tr, "");
			td.innerHTML = visits[index].Payment.toString().formatCurrency();
			
			if(visits[index].InvoiceVisitID == selectedVisitID || visits.length == 1)
			    radioButton.click();

        }
        
        populating = false;
        
        // load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}

function RadioButton_Click() {
    var index, rdo, selectedRow, batchTypeID, batchStatusID;
    for (index = 0; index < tblInvoiceVisits.tBodies[0].rows.length; index++) {
        rdo = tblInvoiceVisits.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = tblInvoiceVisits.tBodies[0].rows[index];
            tblInvoiceVisits.tBodies[0].rows[index].className = "highlightedRow";
            selectedVisitID = rdo.value;
            btnVisitDetails.disabled = false;
            btnVisitComponents.disabled = false;

            //Update Url In Session State, this is used by the back command
            currentUrl = document.location.href;
            currentUrl = currentUrl.substr(currentUrl.indexOf(SITE_VIRTUAL_ROOT));

            newUrl = AddQSParam(RemoveQSParam(currentUrl, "visitID"), "visitID", selectedVisitID);
            var $response = contractSvc.UpdateSessionBackURL(currentUrl, newUrl)
            if (!CheckAjaxResponse($response, contractSvc.url)) {
                return false;
            }
            
            
        } else {
            tblInvoiceVisits.tBodies[0].rows[index].className = ""
        }
    }

}

function btnVisitComponents_Click() {
    document.location.href = "ViewInvoiceVisitComponents.aspx?psview=" + showPaymentSchedule +"&pScheduleId=" + pScheduleId + "&id=" + selectedVisitID;
}

function btnVisitDetails_Click() {
    document.location.href = "ViewInvoicedVisit.aspx?psview=" + showPaymentSchedule + "&pScheduleId=" + pScheduleId + "&id=" + selectedVisitID;
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

function AddNotesLink() {
    $('#imgNotes').prepend('<img alt="View provider-entered note" style="cursor:pointer;" onclick="DisplayNotes();" id="theImg" src="../../../../images/Notes.png" />');
}

function btnBack_Click() {
        var url = GetQSParam(document.location.search, "backUrl");
        url = unescape(url);
        document.location.href = url;
}


addEvent(window, "load", Init);
