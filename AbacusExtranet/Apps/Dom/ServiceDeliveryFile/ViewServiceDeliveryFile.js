
var highlightExceptionsScript;
var serviceDeliveryFileId;
var btnDownloadID;
var btnPaymentSchedulesID;
var disablePaymentschedule;

function Init() {

    var btnPayment = jQuery('#' + btnPaymentSchedulesID);
    if (disablePaymentschedule) {
        btnPayment.attr('disabled', 'disabled'); 
    } else {
        btnPayment.removeAttr('disabled'); 
    }
    
    if (highlightExceptionsScript) {
        window.setInterval(highlightExceptionsScript, 750);
       
       // btnPayment.attr('disabled', 'disabled'); 
    }

    DownloadButton_Init(btnDownloadID);
    
}

function btnViewExceptions_Click(fileID) {
    window.open("FileExceptions.aspx?id=" + fileID);
}

function btnPaymentSchedules_Click() {
    var url = SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/Dom/ServiceDeliveryFile/ServiceDeliveryFilePaymentSchedule.aspx?fileID=" + serviceDeliveryFileId + "&backUrl=" + GetBackUrl();
    document.location.href = url;
    //"ServiceDeliveryFilePaymentSchedule.aspx?fileID=" + serviceDeliveryFileId + "&backUrl=" + GetBackUrl();
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

function DownloadButton_Init(buttonID) {

    var btn = jQuery("#" + buttonID);
    var menuItems = DownloadButton_GetMenuItems(buttonID);

    btn.bind('MenuItemClicked', function(src, menuItem) {
        document.location.href = "FileStoreGetFile.axd?saveas=1&fileDataId=" + menuItem.fileID;
    });

    btn.searchableMenu(menuItems, { 'cssClass': 'Menu', position: 'BottomLeft', showSearchBox: false, width: '13em' });
}

function DownloadButton_GetMenuItems(buttonID) {
    return eval(buttonID + "_menuItems");
}

addEvent(window, "load", Init);