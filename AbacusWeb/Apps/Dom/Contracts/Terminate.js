function btnBack_Click() {
	var url = GetQSParam(document.location.search, "backUrl");
	document.location.href = unescape(url);
}