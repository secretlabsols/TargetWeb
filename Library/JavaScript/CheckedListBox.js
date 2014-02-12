
function CheckedListBox_Init() {
    if (typeof( GetElement ) == "undefined" ) return;
    if (typeof(Page_CheckedListBoxes) == "undefined") return;
    var i, checkedListBox;
    for (i = 0; i < Page_CheckedListBoxes.length; i++) {
        checkedListBox = Page_CheckedListBoxes[i];
        if (typeof(checkedListBox) == "string") {
            checkedListBox = GetElement( checkedListBox );
            if ( checkedListBox != null ) {
                CheckedListBox_Load( checkedListBox );
            }
        }
    }
}
function CheckedListBox_Load( checkedListBox ) {
	checkedListBox.resizeWidth = CheckedListBox_resizeWidth;
	if ( checkedListBox.style.width == "" ) {
		checkedListBox.resizeWidth();
	}
}
function CheckedListBox_resizeWidth() {
    var container = GetElement( this.id );
    var ScrollbarPadding = 20;
    var ContainerWidth;
    if( typeof( document.defaultView ) != "undefined" ) { // The w3c standard
        ContainerWidth = document.defaultView.getComputedStyle( container, "" ).getPropertyValue("width");
    } else if ( typeof( document.all ) != "undefined" ) { // ie
        ContainerWidth = container.offsetWidth;
    }
    this.style.width = parseInt( ContainerWidth ) + ScrollbarPadding + "px";
}
function CheckedListBox_SelectAll(id) {
	CheckedListBox_SelectInternal(id, 1);
}
function CheckedListBox_SelectNone(id) {
	CheckedListBox_SelectInternal(id, 2);
}
function CheckedListBox_SelectInvert(id) {
	CheckedListBox_SelectInternal(id, 3);
}
function CheckedListBox_SelectInternal(id, mode) {
	// mode: 1=All, 2=None, 3=Invert
	var list = GetElement(id);
	var items = list.getElementsByTagName("INPUT");
	for(i=0; i<items.length; i++) {
		if(!items[i].disabled) {
			switch(mode) {
				case 1:
					items[i].checked = true;
					break;
				case 2:
					items[i].checked = false;
					break;
				case 3:
					items[i].checked = !items[i].checked;
					break;
			}
		}
	}
}