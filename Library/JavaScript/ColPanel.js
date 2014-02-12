
function ColPanel_Toggle(colPanelID, show) {
    
    var imageBasePath = SITE_VIRTUAL_ROOT + "Images/ColPanel/";
    var expandImage = imageBasePath + "expand.jpg";
    var collapseImage = imageBasePath + "collapse.jpg";

    var colPanel = jQuery("#" + colPanelID);
    var hState = jQuery(colPanel).children("input")[0];
    var hLink = jQuery(colPanel).children("a")[0];
    var hImg = jQuery(hLink).children("img")[0];
    var fs = jQuery(colPanel).children("fieldset")[0];
    var fsVisibleBefore = jQuery(fs).is(":visible");
    var fsVisibleAfter = !fsVisibleBefore;

    if (show != undefined) {
        jQuery(fs).toggle();
    } else {
        jQuery(fs).toggle(show);
    }

    //This code should be elsewhere, but had to go here as IFrames in IE9 was causing problems especially 
    //on the Direct Payments Screen.
    //I have checked other areas that use the collapsable panel and this code does not seem to effect functionality.
    if (jQuery(fs).is(":visible")) {
        var frameWindow = document.parentWindow || document.defaultView;
        var outerDiv = $(frameWindow.frameElement);
        var newHeight = jQuery(fs).height() + 60;
        jQuery(outerDiv).height(newHeight);
    } else {
        var frameWindow = document.parentWindow || document.defaultView;
        var outerDiv = $(frameWindow.frameElement);
        jQuery(outerDiv).height("32px");
    }

    var newImage = (fsVisibleBefore) ? expandImage : collapseImage;
    jQuery(hImg).attr("src", newImage);

    if (hState) {
        jQuery(hState).val(fsVisibleAfter);
    }

    return fsVisibleAfter;
}
