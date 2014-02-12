
function ReportsButton_Init(reportsButtonID, position, downloadContainerID) {

    var reportBtn = jQuery("#" + reportsButtonID);
    var menuItems = ReportsButton_GetMenuItems(reportsButtonID);

    reportBtn.bind('MenuItemClicked', function(src, menuItem) {
        OpenReport(menuItem.url, downloadContainerID);
    });

    reportBtn.searchableMenu(menuItems, { 'cssClass': 'Menu', position: position, showSearchBox: false, width: '13em' });
}

function ReportsButton_GetMenuItems(reportsButtonID) {
    return eval(reportsButtonID + "_menuItems");
}

function ReportsButton_GetMenuUrlByIndex(reportsButtonID, index) {
    var menuItems = ReportsButton_GetMenuItems(reportsButtonID);
    return menuItems[index].url;
}

function ReportsButton_SetMenuUrl(menuItem, url) {
    menuItem.url = url;
}

function ReportsButton_AddParam(reportsButtonID, name, value) {
    var menuItems = ReportsButton_GetMenuItems(reportsButtonID);
    var url;
    for (index = 0; index < menuItems.length; index++) {
        url = menuItems[index].url;
        url = AddQSParam(RemoveQSParam(url, name), name, value);
        ReportsButton_SetMenuUrl(menuItems[index], url);
    }
}

function ReportsButton_RemoveParam(reportsButtonID, name) {
    var menuItems = ReportsButton_GetMenuItems(reportsButtonID);
    var url;
    for (index = 0; index < menuItems.length; index++) {
        url = menuItems[index].url;
        url = RemoveQSParam(url, name);
        ReportsButton_SetMenuUrl(menuItems[index], url);
    }
}