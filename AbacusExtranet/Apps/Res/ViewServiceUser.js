
function btnback_Click() {
    var url = GetQSParam(document.location.search, "backUrl");

    if (url == undefined || url == '') {
        history.go(-1);
    } else {
        url = unescape(url);
        document.location.href = url;
    }
}