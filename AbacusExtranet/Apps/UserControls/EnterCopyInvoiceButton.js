
function EnterCopyInvoiceButton_Init(buttonID, position, downloadContainerID) {

    var btn = jQuery("#" + buttonID);
    var menuItems = EnterCopyInvoiceButton_GetMenuItems(buttonID);

    btn.bind('MenuItemClicked', function(src, menuItem) {
        ProformaInvoiceEntry(menuItem.createNew);
    });

    btn.searchableMenu(menuItems, { 'cssClass': 'Menu', position: position, showSearchBox: false, width: '13em' });
}

function EnterCopyInvoiceButton_GetMenuItems(buttonID) {
    return eval(buttonID + "_menuItems");
}
