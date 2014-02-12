
var NODE_LEVEL_COUNTER = ":_";
var treeView_selectedNodes = new Collection();

function TreeView_Init(treeID) {
	addEvent(GetElement(treeID), "selectstart", TreeView_CancelSelection);
}
function TreeView_CancelSelection() {
	return false;
}
function TreeView_GetSelectedNode(treeID) {
	if(treeView_selectedNodes.exists(treeID))
		return treeView_selectedNodes.item(treeID);
	else
		return null;
}
function TreeView_SelectNode(treeID, nodeID) {
	var node = GetElement(nodeID);
	TreeView_UnSelectNode(treeID);
	node.className = "treeViewNodeSelected";
	treeView_selectedNodes.add(treeID, node);
}
function TreeView_UnSelectNode(treeID) {
	var node = TreeView_GetSelectedNode(treeID);
	if(node != null) {
		node.className = "treeViewNode";
		treeView_selectedNodes.remove(treeID);
	}
}
function TreeView_EditNodeText(treeID, nodeID, prompt) {
	var result = window.prompt(prompt, GetInnerText(nodeID));
	if(result != null) __doPostBack(treeID, nodeID + ":EditText~" + result);
}
function TreeView_AddRootNode(treeID, prompt, defaultText) {
	var result = window.prompt(prompt, defaultText);
	if(result != null) __doPostBack(treeID, "ROOT:AddNode~" + result);
}
function TreeView_AddChildNode(treeID, parentNodeID, prompt, defaultText) {
	var result = window.prompt(prompt, defaultText);
	if(result != null) __doPostBack(treeID, parentNodeID + ":AddNode~" + result);
}
function TreeView_RemoveNode(treeID, nodeID) {
	__doPostBack(treeID, nodeID + ":RemoveNode");
}
function TreeView_GetTag(treeID, nodeID) {
	return eval(treeID.replace(":","_") + "_Tags.item('" + nodeID + "')");
}
function TreeView_GetDataKey(treeID, nodeID) {
	return eval(treeID.replace(":","_") + "_DataKeys.item('" + nodeID + "')");
}
function TreeView_GetPrevSibling(treeID, nodeID) {
	var node = GetElement(nodeID);
	var nodeLevel = node.id.countString(NODE_LEVEL_COUNTER);
	var parentNode = node.parentNode;	
	var prevSibling = null;
	var tempNode = parentNode.previousSibling;
	if(!tempNode) return null;
	while(tempNode && tempNode.tagName != "SPAN" && tempNode.tagName != "DIV") {
		if(tempNode.tagName == "DIV") return null;
		tempNode = tempNode.previousSibling;
	}
	if(tempNode) prevSibling = tempNode.getElementsByTagName("SPAN")(0);
	if(!prevSibling) return null;
	if(prevSibling.id.countString(NODE_LEVEL_COUNTER) == nodeLevel)
		return prevSibling.id;
	else
		return null;
}
function TreeView_GetNextSibling(treeID, nodeID) {
	var node = GetElement(nodeID);
	var nodeLevel = node.id.countString(NODE_LEVEL_COUNTER);
	var parentNode = node.parentNode;	
	var nextSibling = null;
	var tempNode = parentNode.nextSibling;
	if(!tempNode) return null;
	while(tempNode && tempNode.tagName != "SPAN") {
		tempNode = tempNode.nextSibling;
	}
	if(tempNode) nextSibling = tempNode.getElementsByTagName("SPAN")(0);
	if(!nextSibling) return null;
	if(nextSibling.id.countString(NODE_LEVEL_COUNTER) == nodeLevel)
		return nextSibling.id;
	else
		return null;
}
