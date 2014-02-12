var PropertySvc, securitySvc, tblProperties, divPagingLinks, currentPage
var webSecurityCompanyID;
var tblPropertyID = "ListProperty";
var divPagingLinksID = "Property_PagingLinks";

function Init() {
	PropertySvc = new Target.SP.Web.Apps.WebSvc.Properties_class();
	tblProperties = GetElement(tblPropertyID);
	divPagingLinks = GetElement(divPagingLinksID);

	// populate table
	FetchPropertyList(currentPage);
}


/* CONV LIST METHODS */
function FetchPropertyList(page) {
	currentPage = page;
	DisplayLoading(true);
	PropertySvc.FetchPropertyList(page, ServiceID, FetchPropertyList_Callback)
}
function FetchPropertyList_Callback(response) {
	var Properties, PropertyCounter;
	var tr, td, radioButton;
	var viewUrl;
	var str;
	var link;	
	
	if(CheckAjaxResponse(response, PropertySvc.url)) {
		// populate the Property table
		Properties = response.value.Properties;
		
		// build the View Conv Url to include the current Url for its Back button
		viewUrl = "ViewProperty.aspx?pid=";
		
		// remove existing rows
		ClearTable(tblProperties);
		for(PropertyCounter=0; PropertyCounter<Properties.length; PropertyCounter++) {
			tr = AddRow(tblProperties);
			tr.className = "PropertyList";
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "PropertySelect", Properties[PropertyCounter].PropertyID, RadioButton_Click);
			
			td = AddCell(tr, "");
			link = AddLink(td, Properties[PropertyCounter].Reference, viewUrl + Properties[PropertyCounter].PropertyID + "&sid=" + ServiceID, "Click here to view this Properties details");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			str = Properties[PropertyCounter].AltRef;
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, "");
			link = AddLink(td, Properties[PropertyCounter].Name, viewUrl + Properties[PropertyCounter].PropertyID + "&sid=" + ServiceID, "Click here to view this Properties details");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = Properties[PropertyCounter].Address;
			if (str.length == 0) str = " ";
			td = AddCell(tr, str.replace(/\r\n/g, ", "));
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = Properties[PropertyCounter].PostCode;
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// select the property?
			if(Properties.length == 1) {
				radioButton.click();
			}
			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function RadioButton_Click() {
	var x
	var Radio
	for (x = 0; x < tblProperties.tBodies[0].rows.length; x++){
		Radio = tblProperties.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblProperties.tBodies[0].rows[x].className = "highlightedRow"
		} else {
			tblProperties.tBodies[0].rows[x].className = ""
		}
	}
}

function viewPropertyButton_Click() {
	var x
	var Radio
	var itemFound
	var Url
	var viewUrl
	
	itemFound = false

	viewUrl = "ViewProperty.aspx?" + "pid=";
	//loop round items in the table finding a selected item, if an item is found
	//we used the PropertyID stored in the value element of the radio button
	//to build the URL.
	for (x = 0; x < tblProperties.tBodies[0].rows.length; x++){
		Radio = tblProperties.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			Url = viewUrl + Radio.value +  "&sid=" + ServiceID
			itemFound = true
		}
	}
	
	if (itemFound) {
		window.location.href = Url
	} else {
		alert("Please select a property.")
	}
}