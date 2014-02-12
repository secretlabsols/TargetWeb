
function ProviderSelector_GetBackUrl(selectedID) {
	// build the current Url
	return escape(AddQSParam(RemoveQSParam(document.location.href, "providerID"), "providerID", selectedID));
}
function ServiceSelector_GetBackUrl(selectedID) {
	// build the current Url
	return escape(AddQSParam(RemoveQSParam(document.location.href, "serviceID"), "serviceID", selectedID));
}
