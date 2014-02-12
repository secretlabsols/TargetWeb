
var cboDocumentTypeID = null;
var ServiceUserTypeID = 99;
var $documentSvc;

// in-place-selectors' IDs (set in the DocumentAdd.aspx file)
var clientSelector1ID = null;
var budgetholderSelector1ID = null;
var creditorSelector1ID = null;
var serviceOrderSelector1ID = null;
var clientSelector2ID = null;
var budgetholderSelector2ID = null;
var creditorSelector2ID = null;
var serviceOrderSelector2ID = null;
var clientSelector3ID = null;
var budgetholderSelector3ID = null;
var creditorSelector3ID = null;
var serviceOrderSelector3ID = null;
var clientSelector4ID = null;
var budgetholderSelector4ID = null;
var creditorSelector4ID = null;
var serviceOrderSelector4ID = null;


var menuItems = [{ description: 'None' },
	                { description: 'Budget Holder' },
                    { description: 'Creditor' },
                    { description: 'Service Order' },
                    { description: 'Service User'}];



function btnAdd_click() {

    var uploadFile = GetElement("flDocumentAddFileUpload").getElementsByTagName("DIV")[0].getElementsByTagName("INPUT")[0];
	
	if(uploadFile.value.trim().length == 0) {
		window.alert("You have not selected a document to upload.\n\n" +
					 "Please select a document.");
		return false;
    }

	if(!IsValidFileExt(uploadFile.value.trim())){
	    window.alert('You have selected an invalid file type.\n\n' +
	                 'Following file types can be uploaded:\n\n' +
	                 '* Microsoft Word (*.docx, *.doc)\n' +
	                 '* Open Document Text (*.odt)\n' +
	                 '* Rich Text Format (*.rtf)\n' +
	                 '* Plain Text (*.txt)\n' +
	                 '* HyperText Markup Language (*.html, *.htm)\n' +
	                 '* MIME HyperText Markup Language (*.mhtml, *.mht)\n' +
	                 '* Adobe Reader i.e Portable Document Format (*.pdf)\n' +
	                 '* Excel (*.xlsx, *.xls)\n' +
	                 '* Comma Delimited (*.csv)\n' +
	                 '* Images (*.tiff, *.tif, *.gif, *.jpeg, *.jpg, *.png, *.bmp)\n'
	                );
	    return false;
	}

	if ($('button[id*="btnAssociateWith1"]').text() == 'Associate With'  &&
	     $('button[id*="btnAssociateWith2"]').text() == 'Associate With' &&
	     $('button[id*="btnAssociateWith3"]').text() == 'Associate With' &&
	     $('button[id*="btnAssociateWith4"]').text() == 'Associate With') {
        window.alert('You need to associate the document with at least one association.');
        return false;
	};

	if (document.getElementById(cboDocumentTypeID).options.length <= 1) {
	    window.alert('Either:\n\n' +
	                 '* no document types have been setup\n\n' +
	                 '   or \n\n' +
	                 '* you do not have permissions to upload documents');
	    return false;
	}

	if (Page_ClientValidate()) {
	    OpenFileUploadProgress(FileUploader_UploadID);
	    window.setTimeout('document.forms[0].submit();', 1000);
	}
}

function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}

function RefreshParentAndCloseDialog() {
    // refresh document list in parent window
    GetParentWindow().FetchDocumentList(1, 0);

    GetParentWindow().HideModalDIV();
    btnCancel_Click();
    DialogUnload();
}

function IsValidFileExt(filename) {

    //Code to handle . in folder names
    var indexofLastDot;
    var fileExtension;

    indexofLastDot = filename.lastIndexOf('.');
    fileExtension = filename.substring(indexofLastDot)
    
    var re = /\..+$/;
    var ext = fileExtension.match(re)[0].toLowerCase();
    var allowedExts = [".docx", ".doc", ".odt", ".rtf", ".txt", ".html",
                       ".htm", ".mhtml", ".mht", ".pdf", ".xlsx", ".xls",
                       ".csv", ".tiff", ".tif", ".gif", ".jpeg", ".jpg", 
                       ".png", ".bmp"];

    return (jQuery.inArray(ext, allowedExts) != -1);
}

function ShowHideInPlaceSelector(associateGroupNum, selectorType) {


    // hide selectors via their parent SPAN element
    $('#spanclientSelector' + associateGroupNum).hide();
    $('#spanbudgetholderSelector' + associateGroupNum).hide();
    $('#spancreditorSelector' + associateGroupNum).hide();
    $('#spanserviceOrderSelector' + associateGroupNum).hide();

    var selectorID = "";
    var buttonID = '#btnAssociateWith' + associateGroupNum + ' span:eq(0)';
        
    switch (selectorType) {
        case 1:
            $(buttonID).text('Service User')
            selectorID = 'clientSelector' + associateGroupNum
            $('input[id$="hidAssoc' + associateGroupNum + 'TypeID"]').val(1);
            break
        case 2:
            $(buttonID).text('Creditor')
            selectorID = 'creditorSelector' + associateGroupNum
            $('input[id$="hidAssoc' + associateGroupNum + 'TypeID"]').val(2);
            break
        case 3:
            $(buttonID).text('Budget Holder')
            selectorID = 'budgetholderSelector' + associateGroupNum
            $('input[id$="hidAssoc' + associateGroupNum + 'TypeID"]').val(3);
            break
        case 4:
            $(buttonID).text('Service Order')
            selectorID = 'serviceOrderSelector' + associateGroupNum
            $('input[id$="hidAssoc' + associateGroupNum + 'TypeID"]').val(4);
            break
        default:
            $(buttonID).text('Associate With')
            $('input[id$="hidAssoc' + associateGroupNum + 'TypeID"]').val(99)
            break
    }

    EnableSpecificInPlaceSelector(selectorType, associateGroupNum, true);

    enableChkPublish();
    
    $('#span' + selectorID).show();
    return false;
}

// enables/disables in-place-selector
function EnableInPlaceSelector(SelectorID, state) {
    var objInPlaceSelector = null;

    objInPlaceSelector = document.getElementById(SelectorID + '_valRequired');

    if(objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
}

// enables in-place-selector based on its supplied ID
function EnableSpecificInPlaceSelector(selectorType, associateGroupNum, state) {
    var objInPlaceSelector = null;

    switch (associateGroupNum) {
        case "1":
            switch (selectorType) {
                case 1:
                    objInPlaceSelector = document.getElementById(clientSelector1ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 2:
                    objInPlaceSelector = document.getElementById(creditorSelector1ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 3:
                    objInPlaceSelector = document.getElementById(budgetholderSelector1ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 4:
                    objInPlaceSelector = document.getElementById(serviceOrderSelector1ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                default:
                    objInPlaceSelector = document.getElementById(clientSelector1ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(creditorSelector1ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(budgetholderSelector1ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(serviceOrderSelector1ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    break;
            }
            break;
        case "2":
            switch (selectorType) {
                case 1:
                    objInPlaceSelector = document.getElementById(clientSelector2ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 2:
                    objInPlaceSelector = document.getElementById(creditorSelector2ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 3:
                    objInPlaceSelector = document.getElementById(budgetholderSelector2ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 4:
                    objInPlaceSelector = document.getElementById(serviceOrderSelector2ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                default:
                    objInPlaceSelector = document.getElementById(clientSelector2ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(creditorSelector2ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(budgetholderSelector2ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(serviceOrderSelector2ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    break;
            }
            break;
        case "3":
            switch (selectorType) {
                case 1:
                    objInPlaceSelector = document.getElementById(clientSelector3ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 2:
                    objInPlaceSelector = document.getElementById(creditorSelector3ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 3:
                    objInPlaceSelector = document.getElementById(budgetholderSelector3ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 4:
                    objInPlaceSelector = document.getElementById(serviceOrderSelector3ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                default:
                    objInPlaceSelector = document.getElementById(clientSelector3ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(creditorSelector3ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(budgetholderSelector3ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(serviceOrderSelector3ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    break;
            }
            break;
        case "4":
            switch (selectorType) {
                case 1:
                    objInPlaceSelector = document.getElementById(clientSelector4ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 2:
                    objInPlaceSelector = document.getElementById(creditorSelector4ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 3:
                    objInPlaceSelector = document.getElementById(budgetholderSelector4ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                case 4:
                    objInPlaceSelector = document.getElementById(serviceOrderSelector4ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, state);
                    break;
                default:
                    objInPlaceSelector = document.getElementById(clientSelector4ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(creditorSelector4ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(budgetholderSelector4ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    objInPlaceSelector = document.getElementById(serviceOrderSelector4ID + '_valRequired');
                    if (objInPlaceSelector) ValidatorEnable(objInPlaceSelector, false);
                    break;
            }
            break;
    }

}


$(document).ready(function() {
    $('button[id*="btnAssociateWith"]').button({
        icons: { secondary: "ui-icon-triangle-1-s" }
    });
    $('button[id*="btnAssociateWith"]').searchableMenu(menuItems, { 'cssClass': 'Menu', position: 'TopLeft', showSearchBox: false, width: '10em', shouldStopPropagation: false });
    $('input[id*="chkPublish"]').attr("disabled", true);
    
    $documentSvc = new Target.Abacus.Web.Apps.WebSvc.Documents_class();
    
    EnableSpecificInPlaceSelector(99, "1", false);
    EnableSpecificInPlaceSelector(99, "2", false);
    EnableSpecificInPlaceSelector(99, "3", false);
    EnableSpecificInPlaceSelector(99, "4", false);
    ShowHideInPlaceSelector(1, ServiceUserTypeID);


    $('button[id*="btnAssociateWith"]').bind('MenuItemClicked', function(src, menuItem) {
        var AssociationNo = src.target.id.replace('btnAssociateWith', '');
        EnableSpecificInPlaceSelector(99, AssociationNo, false);
        switch (menuItem.description) {
            case 'Service User':
                return ShowHideInPlaceSelector(AssociationNo, 1);
                break
            case 'Creditor':
                return ShowHideInPlaceSelector(AssociationNo, 2);
                break
            case 'Budget Holder':
                return ShowHideInPlaceSelector(AssociationNo, 3);
                break
            case 'Service Order':
                return ShowHideInPlaceSelector(AssociationNo, 4);
                break
            case 'None':
                return ShowHideInPlaceSelector(AssociationNo, 99);
                break
        }



    });

    $('select[id*="cboDocumentType"]').change(function(e) {
        enableChkPublish();
        if (!$('input[id*="chkPublish"]').is(" :disabled ")) {
            //If the publish to extranet checkbox is enabled then default its checked value
            setChkPublishDefaultValue($('select[id*="cboDocumentType"]').val());
        };
    });

});

function enableChkPublish() {
    var svcOrderAssociationSelected = false;
    var wasDisabled = false;

    //Was the checkbox already disabled?
    wasDisabled = $('input[id*="chkPublish"]').is(" :disabled ");
    
    //Check to see if a service order association has been selected
    $('button[id*="btnAssociateWith"]').each(function() {
        if ($(this).text() == "Service Order") {
            svcOrderAssociationSelected = true;
        }
    });

    if ($('select[id*="cboDocumentType"]').val() != "" && svcOrderAssociationSelected) {
        $('input[id*="chkPublish"]').attr("disabled", false);
    } else {
        $('input[id*="chkPublish"]').attr("disabled", true);
        $('input[id*="chkPublish"]').attr("checked", false);
    }

    //If the Publish To extranet was disabled but is now enabled set its default value
    if (wasDisabled && $('input[id*="chkPublish"]').is(" :disabled ") == false) {
        setChkPublishDefaultValue($('select[id*="cboDocumentType"]').val());
    }

}

function setChkPublishDefaultValue(DocumentTypeID) {
    $response = $documentSvc.DefaultValueForPublishToExtranetChkbx(parseInt(DocumentTypeID))

    $('input[id*="chkPublish"]').attr("checked", $response.value);
    
}

addEvent(window, "unload", DialogUnload);
