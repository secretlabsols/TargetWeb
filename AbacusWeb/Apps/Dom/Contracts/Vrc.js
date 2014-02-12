
var contractSvc;
var frameworkID, entireVisitDomRateCategoryID = "", partVisitDomRateCategoryID = "";
var cboDayCategory, cboServiceType, cboTimeBand;
var cboEntireVisitRateCategory, cboPartVisitRateCategory;
var currentTimeBandID;

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	cboDayCategory = GetElement("cboDayCategory_cboDropDownList");
	cboServiceType = GetElement("cboServiceType_cboDropDownList");
	cboTimeBand = GetElement("cboTimeBand_cboDropDownList");
	cboEntireVisitRateCategory = GetElement("cboEntireVisitRateCategory_cboDropDownList");
	cboPartVisitRateCategory = GetElement("cboPartVisitRateCategory_cboDropDownList");
	
	FetchTimeBands();
}
function RefreshRateCategories() {
	FetchRateCategoryList();
}
function FetchRateCategoryList() {
	var dayCategoryID = cboDayCategory.value.length > 0 ? parseInt(cboDayCategory.value, 10) : 0;
	var serviceTypeID = cboServiceType.value.length > 0 ? parseInt(cboServiceType.value, 10) : 0;
	var timeBandID = cboTimeBand.value.length > 0 ? parseInt(cboTimeBand.value, 10) : 0;
	DisplayLoading(true);
	contractSvc.FetchRateCategoriesAvailableToVrc(frameworkID, dayCategoryID, serviceTypeID, timeBandID, FetchRateCategoryList_Callback);
}
function FetchRateCategoryList_Callback(response) {
	var categories;		
	if(CheckAjaxResponse(response, contractSvc.url)) {
		categories = response.value.List;
		PopulateRateCategory(cboEntireVisitRateCategory, categories, entireVisitDomRateCategoryID);
		PopulateRateCategory(cboPartVisitRateCategory, categories, partVisitDomRateCategoryID);
	}
	DisplayLoading(false);
}

function PopulateRateCategory(dropdown, categories, selectedValue) {
	var opt, index;
	
	// clear
	dropdown.options.length = 0;
	// add blank		
	opt = document.createElement("OPTION");
	dropdown.options.add(opt);
	SetInnerText(opt, "");
	opt.value = "";		
	
	for(index=0; index<categories.length; index++) {
		opt = document.createElement("OPTION");
		dropdown.options.add(opt);
		SetInnerText(opt, categories[index].Text);
		opt.value = categories[index].Value;
	}

	// select existing value
	dropdown.value = selectedValue;
}

function FetchTimeBands() {
    var dayCategoryID = cboDayCategory.value;
	if(dayCategoryID.length == 0) {
	    cboTimeBand.options.length = 0;
	    dayCategoryID = 0
	} else {
	    DisplayLoading(true);
	    contractSvc.FetchTimeBandsAvailableToDayCategory(dayCategoryID, FetchTimeBands_Callback);
    }
}
function FetchTimeBands_Callback(response) {
    var timeBands, opt;
    if(CheckAjaxResponse(response, contractSvc.url)) {
        timeBands = response.value.List;
    
		// clear
	    cboTimeBand.options.length = 0;
	    // add blank		
	    opt = document.createElement("OPTION");
	    cboTimeBand.options.add(opt);
	    SetInnerText(opt, "");
	    opt.value = "";
		
		for(index=0; index<timeBands.length; index++) {
		    opt = document.createElement("OPTION");
		    cboTimeBand.options.add(opt);
		    SetInnerText(opt, timeBands[index].Text);
		    opt.value = timeBands[index].Value;
		}
		
		// select existing value
	    cboTimeBand.value = currentTimeBandID;
	    
	}
	DisplayLoading(false);
}

function SetMinutes(id, value) {
    var minutesField = GetElement(id + "_txtTextBox");
    minutesField.value = value;
}

addEvent(window, "load", Init);
