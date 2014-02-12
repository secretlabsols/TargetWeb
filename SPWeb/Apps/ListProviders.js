
function ProviderSelector_GetBackUrl(selectedID) {
	// build the current Url
	return escape("ListProviders.aspx?page=" + currentPage + "&selectedProviderID=" + selectedID);
}
