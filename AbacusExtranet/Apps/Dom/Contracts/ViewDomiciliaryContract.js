function btnView_Click() {
	document.location.href = "ViewDomContractRates.aspx?dcid=" + GetQSParam(document.location.href, "ID") + "&backUrl=" + escape(document.location.href);
}


    function btnback_Click() {
        var url = GetQSParam(document.location.search, "backUrl");
        url = unescape(url);
        document.location.href = url;
    }
