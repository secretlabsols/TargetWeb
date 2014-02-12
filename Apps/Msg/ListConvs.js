
var msgSvc, securitySvc, cboStartedBy, cboInvolving, cboLabel, cboStatus, dteStartedFrom, dteStartedTo, dteLastSentFrom, dteLastSentTo, tblConvs, divPagingLinks;
var currentPage, currentLabel, currentStatus, currentStartedFrom, currentStartedTo;
var currentLastSentFrom, currentLastSentTo, currentStartedByID, currentStartedByType, currentInvolvingID, currentInvolvingType;
var webSecurityCompanyID;
var cboStartedByID = "cboStartedBy";
var cboInvolvingID = "cboInvolving";
var dteStartedFromID = "dteStartedFrom_txtTextBox";
var dteStartedToID = "dteStartedTo_txtTextBox";
var dteLastMessageFromID = "dteLastSentFrom_txtTextBox";
var dteLastMessageToID = "dteLastSentTo_txtTextBox"; 
var cboLabelID = "cboLabel";
var cboStatusID = "cboStatus";
var tblConvsID = "Conversations_Table";
var divPagingLinksID = "Conversations_PagingLinks";

function Init() {
	msgSvc = new Target.Web.Apps.Msg.WebSvc.Messaging_class();
	securitySvc = new Target.Web.Apps.Security.WebSvc.Security_class();
	cboStartedBy = GetElement(cboStartedByID);
	cboInvolving = GetElement(cboInvolvingID);
	cboLabel = GetElement(cboLabelID);
	cboStatus = GetElement(cboStatusID);
	dteStartedFrom =  GetElement(dteStartedFromID);
	dteStartedTo =  GetElement(dteStartedToID);	
	dteLastSentFrom =  GetElement(dteLastMessageFromID);
	dteLastSentTo =  GetElement(dteLastMessageToID);
	
	tblConvs = GetElement(tblConvsID);
	divPagingLinks = GetElement(divPagingLinksID);
	// populate started by filter
	FetchStartedByList();
	// populate Invloving filter
//	FetchInvlovingList();
	// populate label list
	FetchLabelList();
	// populate table
	
	currentLabel = parseInt(cboLabel.value, 10);
	currentStatus = cboStatus.value;
	currentStartedFrom = dteStartedFrom.value;
	currentStartedTo = dteStartedTo.value;
	currentLastSentFrom = dteLastSentFrom.value;
	currentLastSentTo = dteLastSentTo.value;

	FetchConvList(currentPage);
}

function btnFilter_OnClick() {
	currentLabel = parseInt(cboLabel.value, 10);
	currentStatus = cboStatus.value;
	currentStartedFrom = dteStartedFrom.value;
	currentStartedTo = dteStartedTo.value;
	currentLastSentFrom = dteLastSentFrom.value;
	currentLastSentTo = dteLastSentTo.value;
	
	currentStartedByID = parseInt(cboStartedBy.value, 10);
	currentStartedByType = parseInt(cboStartedBy.options[cboStartedBy.selectedIndex].getAttribute("tag"), 10);
	currentInvolvingID = parseInt(cboInvolving.value, 10);
	currentInvolvingType = parseInt(cboInvolving.options[cboInvolving.selectedIndex].getAttribute("tag"), 10);
	
	currentPage = 1;
	FetchConvList(currentPage);
}

function btnReset_OnClick() {

	cboLabel.selectedIndex = 0;
	cboStatus.selectedIndex = 0;
	cboStartedBy.selectedIndex = 0;
	cboInvolving.selectedIndex = 0;
	
	dteStartedFrom.value = "";
	dteStartedTo.value = "";
	dteLastSentFrom.value = "";
	dteLastSentTo.value = "";
	
	btnFilter_OnClick();
}

function btnNew_OnClick() {
	document.location.href = "NewConv.aspx?backUrl=" + GetCurrentUrl();
}
function GetCurrentUrl() {
	// build the current Url
	var strQueryString
	
	strQueryString = "startedByID=" + currentStartedByID + 
		"&startedByType=" + currentStartedByType +
		"&involvingID=" + currentInvolvingID +
		"&involvingType=" + currentInvolvingType +
		"&status=" + currentStatus ;
		if (IsDate(currentLastSentFrom)) {
			strQueryString = strQueryString + "&lastSentFrom=" + Date.strftime("%d/%m/%Y", currentLastSentFrom.toDate())
		}
		else{
			strQueryString = strQueryString + "&lastSentFrom=" 
		}
		if (IsDate(currentLastSentTo)) {
			strQueryString = strQueryString + "&lastSentTo=" + Date.strftime("%d/%m/%Y", currentLastSentTo.toDate())
		}
		else{
			strQueryString = strQueryString + "&lastSentTo="
		}
		if (IsDate(currentStartedFrom)) {
			strQueryString = strQueryString + "&startedFrom=" + Date.strftime("%d/%m/%Y", currentStartedFrom.toDate()) 
		}
		else{
			strQueryString = strQueryString + "&startedFrom="
		}
		if (IsDate(currentStartedTo)) {
			strQueryString = strQueryString + "&startedTo=" + Date.strftime("%d/%m/%Y", currentStartedTo.toDate())
		}
		else{
			strQueryString = strQueryString + "&startedTo=" 
		}
		//"&toID=" + currentTo + 
		strQueryString = strQueryString + "&labelID=" + currentLabel + "&page=" + currentPage;
		
		return escape("ListConvs.aspx?" + strQueryString);
}

/* Started By LIST METHODS */
function FetchStartedByList() {
	var messagePassingType;
	
	if(cboStartedBy) {
		DisplayLoading(true);
		msgSvc.GetMessagePassingType(FetchStartedByList_Callback);
	}
}

function FetchStartedByList_Callback(response) {
	if(CheckAjaxResponse(response, securitySvc.url)) {
		if (response.value.MessagePassingType == Target.Web.Apps.Msg.MessagePassingType.CompanyToCompany) {
			securitySvc.FetchCompaniesAvailableToUser(FetchStartedByListCompanies_Callback)
		} else {
			securitySvc.FetchUsersAvailableToUser(FetchStartedByListUsers_Callback)
		}
	}
}

function FetchStartedByListCompanies_Callback(response) {
	var companyCounter, companies, keys, keyCounter, items, optGroup, opt, optCounter;
	if(CheckAjaxResponse(response, securitySvc.url)) {
		// populate the Started By filter dropdown
		cboStartedBy.options.length = 0;
		opt = new Option("", 0);
		opt.setAttribute("tag", 0);	// no type
		cboStartedBy.options.add(opt);
		companies = response.value.Companies;
		for(companyCounter=0; companyCounter<companies.length; companyCounter++) {
			opt = new Option(companies[companyCounter].Name, companies[companyCounter].ID);
			opt.setAttribute("tag", 2);	// Company to Company
			cboStartedBy.options.add(opt);
		}
		// set default
		
		for(optCounter=0; optCounter<cboStartedBy.options.length; optCounter++) {
			if((parseInt(cboStartedBy.options[optCounter].getAttribute("tag"), 10) == currentStartedByType) &&
					(parseInt(cboStartedBy.options[optCounter].value, 10) == currentStartedByID)) {
				// IE6 doesn't seem to want to set the selected item unless focus is set
				if(ie6) cboStartedBy.focus();
				cboStartedBy.selectedIndex = optCounter;
				break;
			}
		}
		
	}
	DisplayLoading(false);
}

function FetchStartedByListUsers_Callback(response) {
	var companyCounter, companies, keys, keyCounter, items, optGroup, opt, optCounter;
	if(CheckAjaxResponse(response, securitySvc.url)) {
		// populate the From filter dropdown
		cboStartedBy.options.length = 0;
		opt = new Option("", 0);
		opt.setAttribute("tag", 0);	// no type
		cboStartedBy.options.add(opt);
		companies = response.value.Companies;
		for(companyCounter=0; companyCounter<companies.length; companyCounter++) {
			// add opt group for the company
			optGroup = document.createElement("OPTGROUP");
			optGroup.setAttribute("label", companies[companyCounter].Name);
			items = companies[companyCounter].Users;
			keys = items.getKeys();
			for(keyCounter=0; keyCounter<keys.length; keyCounter++) {
				opt = new Option(keys[keyCounter], items.getValue(keys[keyCounter]));
				opt.setAttribute("tag", 1);	// user type
				optGroup.appendChild(opt);
			}
			cboStartedBy.appendChild(optGroup);
		}
		// set default
		for(optCounter=0; optCounter<cboStartedBy.options.length; optCounter++) {
			if((parseInt(cboStartedBy.options[optCounter].getAttribute("tag"), 10) == currentStartedByType) &&
					(parseInt(cboStartedBy.options[optCounter].value, 10) == currentStartedByID)) {
				// IE6 doesn't seem to want to set the selected item unless focus is set
				if(ie6) cboStartedBy.focus();
				cboStartedBy.selectedIndex = optCounter;
				break;
			}
		}
		
	}
	DisplayLoading(false);
}

/* Invloving LIST METHODS */
function FetchInvlovingList() {
	if(cboInvolving) {
		DisplayLoading(true);
		msgSvc.GetMessagePassingType(FetchInvolvingList_Callback);
	}
}

function FetchInvolvingList_Callback(response) {
	if(CheckAjaxResponse(response, securitySvc.url)) {
		if (response.value.MessagePassingType == Target.Web.Apps.Msg.MessagePassingType.CompanyToCompany) {
			securitySvc.FetchCompaniesAvailableToUser(FetchInvolvingListCompanies_Callback)
		} else {
			securitySvc.FetchUsersAvailableToUser(FetchInvolvingListUsers_Callback)
		}
	}
}

function FetchInvolvingListCompanies_Callback(response) {
	var companyCounter, companies, keys, keyCounter, items, optGroup, opt, optCounter;
	if(CheckAjaxResponse(response, securitySvc.url)) {
		// populate the From filter dropdown
		cboInvolving.options.length = 0;
		opt = new Option("", 0);
		opt.setAttribute("tag", 0);	// no type
		cboInvolving.options.add(opt);
		companies = response.value.Companies;
		for(companyCounter=0; companyCounter<companies.length; companyCounter++) {
			opt = new Option(companies[companyCounter].Name, companies[companyCounter].ID);
			opt.setAttribute("tag", 2);	// Company to Company
			cboInvolving.options.add(opt);
		}
		// set default
		for(optCounter=0; optCounter<cboInvolving.options.length; optCounter++) {
			if((parseInt(cboInvolving.options[optCounter].getAttribute("tag"), 10) == currentInvolvingType) &&
					(parseInt(cboInvolving.options[optCounter].value, 10) == currentInvolvingID)) {
				// IE6 doesn't seem to want to set the selected item unless focus is set
				if(ie6) cboInvolving.focus();
				cboInvolving.selectedIndex = optCounter;
				break;
			}
		}
		
	}
	DisplayLoading(false);
}

function FetchInvolvingListUsers_Callback(response) {
	var companyCounter, companies, keys, keyCounter, items, optGroup, opt, optCounter;
	if(CheckAjaxResponse(response, securitySvc.url)) {
		// populate the From filter dropdown
		cboInvolving.options.length = 0;
		opt = new Option("", 0);
		opt.setAttribute("tag", 0);	// no type
		cboInvolving.options.add(opt);
		companies = response.value.Companies;
		for(companyCounter=0; companyCounter<companies.length; companyCounter++) {

			optGroup = document.createElement("OPTGROUP");
			optGroup.setAttribute("label", companies[companyCounter].Name);
			items = companies[companyCounter].Users;
			keys = items.getKeys();
			for(keyCounter=0; keyCounter<keys.length; keyCounter++) {
				opt = new Option(keys[keyCounter], items.getValue(keys[keyCounter]));
				opt.setAttribute("tag", 1);	// user type
				optGroup.appendChild(opt);
			}
			cboInvolving.appendChild(optGroup);
		}
		// set default
		for(optCounter=0; optCounter<cboInvolving.options.length; optCounter++) {
			if((parseInt(cboInvolving.options[optCounter].getAttribute("tag"), 10) == currentInvolvingType) &&
					(parseInt(cboInvolving.options[optCounter].value, 10) == currentInvolvingID)) {
				// IE6 doesn't seem to want to set the selected item unless focus is set
				if(ie6) cboInvolving.focus();
				cboInvolving.selectedIndex = optCounter;
				break;
			}
		}
		
	}
	DisplayLoading(false);
}

/* LABEL LIST METHODS */
function FetchLabelList() {
	DisplayLoading(true);
	msgSvc.FetchLabelListByCompany(FetchLabelList_Callback)
}
function FetchLabelList_Callback(response) {
	var keys, keyCounter, items;
	if(CheckAjaxResponse(response, msgSvc.url)) {
		// populate the To filter dropdown
		items = response.value.List;
		keys = items.getKeys();
		cboLabel.options.length = 0;
		cboLabel.options.add(new Option("", 0));
		for(keyCounter=0; keyCounter<keys.length; keyCounter++) {
			cboLabel.options.add(new Option(keys[keyCounter], items.getValue(keys[keyCounter])));
		}
		// set default
		cboLabel.value = currentLabel;
	}
	DisplayLoading(false);
}

/* CONV LIST METHODS */
function FetchConvList(page) {
	var startedFromDate, startedToDate, lastSentFromDate, lastSentToDate;
	
	currentPage = page;
	DisplayLoading(true);
	
	if(currentStartedFrom != null)
		startedFromDate = currentStartedFrom.toDate();
		
	if(currentStartedTo != null)
		startedToDate = currentStartedTo.toDate();
	
	if(currentLastSentFrom != null)
		lastSentFromDate = currentLastSentFrom.toDate();
		
	if(currentLastSentTo != null)
		lastSentToDate = currentLastSentTo.toDate();
		
	msgSvc.FetchConversationList(page, currentLabel, currentStatus, startedFromDate, startedToDate, lastSentFromDate, lastSentToDate, currentStartedByID, currentStartedByType,currentInvolvingID, currentInvolvingType, FetchConvList_Callback)
}
function FetchConvList_Callback(response) {
	var convs, convCounter;
	var tr, td;
	var labelItems, labelKeys, labelCounter;
	var viewUrl;
	
	if(CheckAjaxResponse(response, msgSvc.url)) {
		// build the View Conv Url to include the current Url for its Back button
		viewUrl = "ViewConv.aspx?backUrl=" + GetCurrentUrl() + "&id=";
		
		// populate the conversation table
		convs = response.value.Conversations;
		// remove existing rows
		ClearTable(tblConvs);
		for(convCounter=0; convCounter<convs.length; convCounter++) {
			tr = AddRow(tblConvs);
			td = AddCell(tr, " ");
			SetUIReadStatus(tr, td, convs[convCounter].ReadStatus);
			td = AddCell(tr, "");
			AddLink(td, convs[convCounter].Subject, viewUrl + convs[convCounter].ID, "Click here to view this conversation");
			AddCell(tr, Date.strftime("%a %d %b %Y %H:%M:%S", convs[convCounter].StartedOnDate));
			AddCell(tr, Date.strftime("%a %d %b %Y %H:%M:%S", convs[convCounter].LastMessageSent));
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}





