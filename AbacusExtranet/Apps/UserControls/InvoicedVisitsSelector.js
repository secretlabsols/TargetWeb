var providerInvoiceSvc, currentPage;
var providerID, contractID, svcUsrID, careWorkerID, dateFrom, DateTo, selectedVisitID;
var listFilter, listFilterName = "";
var tblVisits, divPagingLinks, btnView, btnViewID;

function Init() {
 	providerInvoiceSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomProviderInvoice_class();
	tblVisits = GetElement("tblVisits");
	divPagingLinks = GetElement("Visits_PagingLinks");
	btnView = GetElement(btnViewID);
	btnView.disabled = true;
	// populate table
	FetchVisitsList(currentPage)
}

/* FETCH CLIENT LIST METHODS */
function FetchVisitsList(page) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedVisitID == undefined) selectedVisitID = 0;
	if(careWorkerID == undefined) careWorkerID = 0;

	providerInvoiceSvc.FetchDomProviderInvoiceVisitsEnquiryResults(page, providerID, svcUsrID, contractID, careWorkerID, dateFrom, dateTo, FetchVisitList_Callback);
}


function FetchVisitList_Callback(response) {
	var visits, index;
	var tr, td, radioButton;
	var str, roundingUsed;
	var defaultDate = "Fri Dec 31 23:59:59 UTC 9999"; ;
	var link;

	btnView.disabled = true;
    
	// populate the care worker table
	visits = response.value.Visits;
	// remove existing rows
	ClearTable(tblVisits);
	for(index=0; index<visits.length; index++) {
	
		tr = AddRow(tblVisits);
		td = AddCell(tr, "");			
		radioButton = AddRadio(td, "", "VisitSelect", visits[index].VisitID, RadioButton_Click);
		
		AddCell(tr, Date.strftime("%d/%m/%Y", visits[index].VisitDate));
		td.style.textOverflow = "ellipsis";
		td.style.overflow = "hidden";

		AddCell(tr, Date.strftime("%H:%M", visits[index].StartTimeClaimed));
		td.style.textOverflow = "ellipsis";
		td.style.overflow = "hidden";


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

		AddCell(tr, Date.strftime("%H:%M", visits[index].ActualDuration));
		td.style.textOverflow = "ellipsis";
		td.style.overflow = "hidden";
		
		if (svcUsrID == 0 && careWorkerID != 0) {
		    td = AddCell(tr, visits[index].ServiceUserName);
		}
		if (svcUsrID != 0 && careWorkerID == 0) {
		    td = AddCell(tr, visits[index].CareWorker);
		}
		td.style.textOverflow = "ellipsis";
		td.style.overflow = "hidden";
		
		// select the vists?
		if(selectedVisitID == visits[index].VisitID || (currentPage == 1 && visits.length == 1)){
			radioButton.click();
		}			
	}
	// load the paging link HTML
	divPagingLinks.innerHTML = response.value.PagingLinks;
		
	DisplayLoading(false);
}

function RadioButton_Click() {
	var x
	var Radio
	for (x = 0; x < tblVisits.tBodies[0].rows.length; x++){
		Radio = tblVisits.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblVisits.tBodies[0].rows[x].className = "highlightedRow"
			selectedVisitID = Radio.value;
			btnView.disabled = false;
		} else {
			tblVisits.tBodies[0].rows[x].className = ""
		}
	}
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

function btnView_Click() {
   document.location.href = "ViewInvoicedVisit.aspx?id=" + selectedVisitID + "&backUrl=" + GetBackUrl(); 
}

addEvent(window, "load", Init);
