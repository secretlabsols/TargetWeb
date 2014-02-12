
function DualList_Init() {
	if (typeof(GetElement) == "undefined" ) return;
	if (typeof(DualListBoxes) == "undefined") return;
	var i, dualListBox;
	for (i = 0; i < DualListBoxes.length; i++) {
		dualListBox = DualListBoxes[i];
		if (typeof(dualListBox) == "string") {
			DualList_Load(dualListBox);
		}
	}
}
function DualList_Load(dualListID) {
	var btns = GetElement(dualListID + "_Btns");
	var src = GetElement(dualListID + "_lstSrc");
	btns.style.marginTop = (src.size / 2) + "em";
}
function DualList_Move(srcID, destID, isAdd) {			
	var src = GetElement(srcID);
	var dest = GetElement(destID);
	
	if(src.options.length == 0) return;
	
	var originalIndex = src.options.selectedIndex;
	
	while(src.options.selectedIndex >= 0) {
		DualList_MoveItem(src.options.selectedIndex, src, dest, isAdd);
	}
	if(originalIndex < src.options.length) {
		src.options.selectedIndex = originalIndex;
	} else {
		src.options.selectedIndex = src.options.length - 1;
	}
	dest.options.selectedIndex = dest.options.length - 1;
}
function DualList_MoveItem(itemIndex, src, dest, isAdd) {
	if(src.options.length == 0) return;
	var itemValue = src.options[itemIndex].value;
	var itemText = src.options[itemIndex].text;
	var itemTag = src.options[itemIndex].getAttribute("tag");
	var newItem = new Option(itemText, itemValue);
	newItem.setAttribute("tag", itemTag);
	src.remove(itemIndex);
	dest.options.add(newItem);
	// fire event
	if(isAdd) {
		if(typeof(DualList_ItemAdded) == "function") DualList_ItemAdded(newItem);
	} else {
		if(typeof(DualList_ItemRemoved) == "function") DualList_ItemRemoved(newItem);
	}
}

