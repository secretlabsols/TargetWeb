
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
function RefreshTree(contractID, selectType, selectID) {
	var url = treeFrame.src;
	if(!selectType) selectType = "";
	if (!selectID) selectID = "";
	
	if (selectID != "")
	{
	    url = AddQSParam(RemoveQSParam(url, "id"), "id", contractID);
	    url = AddQSParam(RemoveQSParam(url, "selectType"), "selectType", selectType);
	    url = AddQSParam(RemoveQSParam(url, "selectID"), "selectID", selectID);
	    treeFrame.src = url;
	}
	else {
	    if (contractID != 0) {
	        url = AddQSParam(RemoveQSParam(url, "id"), "id", contractID);
	        url = AddQSParam(RemoveQSParam(url, "selectType"), "selectType", "c");
	        url = AddQSParam(RemoveQSParam(url, "selectID"), "selectID", selectID);
	        treeFrame.src = url;
	        NavigateContent("Header.aspx?id=" + contractID + "&selectid=&selecttype=c&mode=1");
	    }
	    else {
	        btnBack_Click(contractID);
	    }	    	    
	}	
}
function btnBack_Click(contractID) {
	var url = GetQSParam(document.location.search, "backUrl");

	if (contractID == 0) {
	    url = RemoveQSParam(unescape(url), "contractid")
        url = AddQSParam(RemoveQSParam(url, "contractid"), "contractid", contractID);
    }
	    
	document.location.href = unescape(url);
}

addEvent(window, "load", Init);
addEvent(window, "resize", ResizeFrames);
