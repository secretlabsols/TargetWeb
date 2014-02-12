
function PrintButton_Init(printButtonID, position, downloadContainerID) {

    var printBtn = jQuery("#" + printButtonID);
    var menuItems = PrintButton_GetMenuItems(printButtonID);

    printBtn.bind('MenuItemClicked', function(src, menuItem) {
        PrintDocuments(menuItem.printAll, menuItem.printNow);
    });

    printBtn.searchableMenu(menuItems, { 'cssClass': 'Menu', position: position, showSearchBox: false, width: '13em' });
}

function PrintButton_GetMenuItems(printButtonID) {
    return eval(printButtonID + "_menuItems");
}
