// Resizes the folder and settings container to make best use of the space available on the page
function ResizeFolderAndSettingsContainer() {

    var folderAndSettingsTable = GetElement("tblFolderAndSettings");
    var divFolderAndSettingsTree = GetElement("divFolderAndSettingsTree");
    var minHeight = 200;
    var heightAlreadyUsed = 300;    // this indicates space already used by existing controls, not precise
    var heightOfBody = document.documentElement.clientHeight;
    var heightOfFolderAndSettings = heightOfBody - heightAlreadyUsed;

    // don't allow the height of the controls to go below that of minHeight
    // otherwise they will disappear
    if (heightOfFolderAndSettings < minHeight) {
        heightOfFolderAndSettings = minHeight;
    }
    
    folderAndSettingsTable.style.height = heightOfFolderAndSettings + "px";
    divFolderAndSettingsTree.style.height = heightOfFolderAndSettings + "px";
}

function RefreshPage() {
    // Refresh the whole page if this function is invoked..
    var newUrl;

    newUrl = window.location.href;
    newUrl = newUrl.replace("?selectType=2", "");
    newUrl = newUrl.replace(".aspx", ".aspx?selectType=2");
    window.location = newUrl;
}

addEvent(window, "load", ResizeFolderAndSettingsContainer);
addEvent(window, "resize", ResizeFolderAndSettingsContainer);

