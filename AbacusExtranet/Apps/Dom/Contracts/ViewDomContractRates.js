var cboPeriodDates;
function cboPeriodDates_Click() {
    cboPeriodDates = GetElement("cboPeriodDates_cboDropDownList", true);
    //alert(cboPeriodDates.DropDownList.SelectedValue);
    var backUrl = GetQSParam(document.location.search, "backUrl");

    //alert(backUrl);
        
    var newurl = document.location.href;
    newurl = AddQSParam(RemoveQSParam(newurl, "pid"), "pid", cboPeriodDates.value);
    newurl = AddQSParam(RemoveQSParam(newurl, "backUrl"), "backUrl", backUrl);

    document.location.href = newurl;
    
}

function btnback_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}