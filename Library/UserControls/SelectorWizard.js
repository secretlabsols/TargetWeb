
var SelectorWizard_currentStep, SelectorWizard_newUrl, SelectorWizard_lastStep, SelectorWizard_cboSavedSelections, SelectorWizard_defaultNewEnquiryParamsJS;

/* COMMON METHODS */
function SelectorWizard_NewEnquiry() {
    var baseUrl = document.location.pathname;
    if (SelectorWizard_defaultNewEnquiryParamsJS.length > 0)
        baseUrl += "?" + SelectorWizard_defaultNewEnquiryParamsJS;
	document.location.href = baseUrl;
}
function SelectorWizard_Back() {
	SelectorWizard_newUrl = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep - 1);
	SelectorWizard_Navigate();
}
function SelectorWizard_Navigate() {
	document.location.href = SelectorWizard_newUrl;
}
function SelectorWizard_Finish() {
	SelectorWizard_newUrl = SelectorWizard_GetNewUrl(SelectorWizard_newUrl, SelectorWizard_lastStep);
	SelectorWizard_Navigate();
}
function SelectorWizard_GetNewUrl(url, step) {
	return AddQSParam(RemoveQSParam(url, "currentStep"), "currentStep", step);
}
function SelectorWizard_ShowHiddenStep(step) {
    var url = SelectorWizard_newUrl;
    if(!url) url = document.location.href;
	SelectorWizard_newUrl = SelectorWizard_GetNewUrl(url, step);
	SelectorWizard_Navigate();
}
function SelectorWizard_JumpToSavedSelection() {
    var cbo = GetElement(SelectorWizard_cboSavedSelections);
    var selectedItem = cbo.options[cbo.selectedIndex];
    var url = selectedItem.value;
    var tag = selectedItem.getAttribute("tag");
    var appendUrl = false;
    if(url && url.trim().length > 0) {
        if(tag && tag == "appendUrl") appendUrl = true;
        if(appendUrl) {
            if(document.location.search.length > 0) url += "qs=" + escape(document.location.search.substr(1, document.location.search.length - 1)); 
            url += "&backUrl=" + escape(document.location.href);
        }
        document.location.href = url;
    }
}