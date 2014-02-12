
function btnDecline_OnClick(evt) {
	var comment = GetElement("txtComment_txtTextBox");
	if(comment.value.trim().length == 0) {
		alert("Please enter a comment to explain your decision.");
		return false;
	}
	btnAccept_OnClick("False");
}
function btnAccept_OnClick(accept) {
	GetElement("hidDecision").value = accept;
	document.forms[0].submit();
}