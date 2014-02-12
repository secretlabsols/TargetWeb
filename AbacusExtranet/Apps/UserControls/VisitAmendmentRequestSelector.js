var providerInvoiceSvc, currentPage;
var reqByUserID, reqByCompanyID;
var providerID, contractID, reqDateFrom, reqDateTo, statusDateFrom, statusDateTo, status, selectedVisitID, clientID, pScheduleId;
var tblAmendments, divPagingLinks, btnView, btnViewID;
var listFilter, listFilterServiceUser = "", listFilterReference = "";

function Init() {
 	providerInvoiceSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomProviderInvoice_class();
	tblAmendments = GetElement("tblAmendments");
	divPagingLinks = GetElement("Amendments_PagingLinks");
	btnView = GetElement(btnViewID);
	btnView.disabled = true;


	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Service User", GetElement("thServiceUser"), ((listFilterServiceUser.length > 0) ? listFilterServiceUser : null));
	listFilter.AddColumn("S/U Reference", GetElement("thServiceUserReference"), ((listFilterReference.length > 0) ? listFilterReference : null));

	// populate table
	FetchAmendmentList(currentPage)
}

function ListFilter_Callback(column) {
    switch (column.Name) {
        case "Service User":
            listFilterServiceUser = column.Filter;
            break;
        case "S/U Reference":
            listFilterReference = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    FetchAmendmentList(1);
}


/* FETCH CLIENT LIST METHODS */
function FetchAmendmentList(page) {
	currentPage = page;
	DisplayLoading(true);

	providerInvoiceSvc.FetchVisitAmendmentRequestEnquiryResults(page, providerID, contractID, reqDateFrom, reqDateTo, status, statusDateFrom, statusDateTo, reqByCompanyID, reqByUserID, clientID, pScheduleId,listFilterServiceUser, listFilterReference,  FetchAmendmentList_Callback);
}

function FetchAmendmentList_Callback(response) {
	var amendments, index;
	var tr, td, radioButton;
	var str;
	var link;
    
	// populate the care worker table
	amendments = response.value.Amendments;
	// remove existing rows
	ClearTable(tblAmendments);
	for(index=0; index<amendments.length; index++) {
	
		tr = AddRow(tblAmendments);
		td = AddCell(tr, "");			
		radioButton = AddRadio(td, "", "AmendmentSelect", amendments[index].VisitID, RadioButton_Click);
		
		AddCell(tr, amendments[index].ServiceUserName);
		td.style.textOverflow = "ellipsis";
		td.style.overflow = "hidden";

		AddCell(tr, amendments[index].ServiceUserRef);
		td.style.textOverflow = "ellipsis";
		td.style.overflow = "hidden";

		AddCell(tr, Date.strftime("%d/%m/%Y", amendments[index].VisitDate));
		td.style.textOverflow = "ellipsis";
		td.style.overflow = "hidden";

		AddCell(tr, Date.strftime("%H:%M", amendments[index].StartTimeClaimed));
		td.style.textOverflow = "ellipsis";
		td.style.overflow = "hidden";

		
		AddCell(tr, amendments[index].Status);
		td.style.textOverflow = "ellipsis";
		td.style.overflow = "hidden";
		
		// select the vists?
		if(selectedVisitID == amendments[index].VisitID || ( currentPage == 1 && amendments.length == 1)) {
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
	for (x = 0; x < tblAmendments.tBodies[0].rows.length; x++){
		Radio = tblAmendments.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblAmendments.tBodies[0].rows[x].className = "highlightedRow"
			selectedVisitID = Radio.value;
			btnView.disabled = false;
		} else {
			tblAmendments.tBodies[0].rows[x].className = ""
		}
	}
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

function btnView_Click() {
   
    document.location.href = "ViewInvoicedVisit.aspx?pscheduleid=" + pScheduleId + "&id=" + selectedVisitID + "&backUrl=" + GetBackUrl(); 
}

addEvent(window, "load", Init);
