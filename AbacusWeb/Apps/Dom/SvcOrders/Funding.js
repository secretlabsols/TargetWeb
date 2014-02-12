
var treeFrame, contentFrame;

function Init() {
	treeFrame = GetElement("frmTree");
	contentFrame = GetElement("frmContent");
	ResizeFrames();
}
function ResizeFrames() {
	var treeOffset = FindOffset(treeFrame);
	var treeTop = treeOffset[1];
	var clientHeight = document.documentElement.clientHeight;
	var newHeight = clientHeight - treeTop - ConvertEmToPx(4);
	treeFrame.height = newHeight;
	// sort out the padding for the content frame and the differences between IE and FF
	if(ie)
		newHeight += 1;
	else
		newHeight -= 3;
	contentFrame.height = newHeight - ConvertEmToPx(1);
}
function NavigateContent(url) {
	contentFrame.src = url;
}
function RefreshTree(serviceOrderID, selectType, selectID) {
	var url = treeFrame.src;
	if(!selectType) selectType = "";
	if(!selectID) selectID = "";
	url = AddQSParam(RemoveQSParam(url, "id"), "id", serviceOrderID);
	url = AddQSParam(RemoveQSParam(url, "selectType"), "selectType", selectType);
	url = AddQSParam(RemoveQSParam(url, "selectID"), "selectID", selectID);
	treeFrame.src = url;
}
function btnBack_Click() {
	var url = GetQSParam(document.location.search, "backUrl");
	document.location.href = unescape(url);
}

addEvent(window, "load", Init);
addEvent(window, "resize", ResizeFrames);
