var cboDocumentTypeID = null;
var $documentSvc;


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

	if($('input:radio[name*=Associate1]:checked').val() == 'None' &&
	   $('input:radio[name*=Associate2]:checked').val() == 'None') {
        window.alert('You need to associate the document with at least one service user.');
        return false;
    }

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
    GetParentWindow().FetchDocumentListFromAdd();

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


$(document).ready(function() {

    $documentSvc = new Target.Abacus.Extranet.Apps.WebSvc.Documents_class();
    
});



addEvent(window, "unload", DialogUnload);
