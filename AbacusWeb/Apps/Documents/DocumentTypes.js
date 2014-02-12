function ShowHelp() {
    var d = new Target.Abacus.Web.Apps.Documents.DocumentTypes.HelpDialog("File Pattern Help", "<blockquote>" +
				"The <b>Filename Pattern</b> can be made up of a series of fixed characters and known placeholders. For example:" +
				 "<br /><br />[ServiceUserRef] [DocumentType] [Date]" +
				 "<br />e.g. <b>ABC123 Personal Budget Statement 2010-10-15</b>" +
				 "<br /><br />The appropriate file extension will be automatically added to the filename pattern. For example:" +
				 "<br/><b>ABC123 Personal Budget Statement 2010-10-15.pdf</b>" +
				 "<br /><br />The placeholders that can be used are:" +
				 "</blockquote>" +
				 "<ul style=\"margin-bottom:0em;\">" +
				 "    <li><b>[DocumentID]</b> is the system generated unique identifier of the Document e.g. <b>1234</b></li>" +
				 "    <li><b>[DocumentType]</b> is the description of the Document Type e.g. <b>Personal Budget Statement</b></li>" +
				 "    <li><b>[ServiceUserRef]</b> is the reference of the Service User e.g. <b>ABC1234</b></li>" +
				 "    <li><b>[ServiceUserAltRef]</b> is the alternative reference of the Service User e.g. <b>DEF5678</b></li>" +
				 "    <li><b>[ServiceUserName]</b> is the name of the Service User e.g. <b>BLOGGS JOE</b></li>" +
				 "    <li><b>[Date]</b> is the current date formatted as yyyy-MM-dd e.g. <b>2010-10-15</b></li>" +
				 "</ul>");

    d.SetCallback(ShowHelp_Callback)
    d.SetType(1);
    d.Show();
}

function ShowHelp_Callback(evt, args) {
    var d = args[0];
    d.Hide();
}

//*********************************************************************************************************
// Help DIALOG
//*********************************************************************************************************
addNamespace("Target.Abacus.Web.Apps.Documents.DocumentTypes.HelpDialog");

Target.Abacus.Web.Apps.Documents.DocumentTypes.HelpDialog = function(title, caption) {
    this.SetTitle(title);
    this.ClearContent;
    this.SetType(1);
    this.SetContentText(caption);
    this.SetWidth("45");
}

// inherit from base
Target.Abacus.Web.Apps.Documents.DocumentTypes.HelpDialog.prototype = new Target.Web.Dialog.Msg();

//*********************************************************************************************************
// End of Help DIALOG
//*********************************************************************************************************

var documentSvc = null;
var cboPaperSource = null, cboPrinter = null, cboPaperSize = null, cboPrintOn = null;
var documentPrinterID, paperSourceID, paperSizeID, printOnID;
var printerCanDuplex = false;
var validIcon = null, invalidIcon = null;
var editMode = false, setDropDownValue = true;

documentPrinterID = paperSourceID = paperSizeID = printOnID = -1;

function Init() {
    documentSvc = new Target.Abacus.Web.Apps.WebSvc.Documents_class();

    cboPrinter = GetElement("cboPrinter_cboDropDownList", true);
    cboPaperSource = GetElement("cboPaperTray_cboDropDownList", true);
    cboPaperSize = GetElement("cboPaperSize_cboDropDownList", true);
    cboPrintOn = GetElement("cboPrintOn_cboDropDownList", true);

    $("#" + cboPrinter.id).msDropDown();
    $("#" + cboPaperSource.id).msDropDown();
    $("#" + cboPaperSize.id).msDropDown();
    $("#" + cboPrintOn.id).msDropDown();

    cboPrinter_OnChange();
}

function cboPrinter_OnChange() {
    // if cboPaperSource is not found then do nothing!
    if (!cboPaperSource) return;

    // if no printer selected then empty and disable drop downs
    if (cboPrinter.value == "") {
        ClearDropDown(cboPaperSource);
        cboPaperSource.disabled = true;

        ClearDropDown(cboPaperSize);
        cboPaperSize.disabled = true;

        ClearDropDown(cboPrintOn);
        cboPrintOn.disabled = true;

        EnableValidators(false);

        documentPrinterID = -1;
        return;
    }

    documentPrinterID = cboPrinter.value;

    DisplayLoading(true);

    cboPaperSource.disabled = !editMode;
    cboPaperSize.disabled = !editMode;
    cboPrintOn.disabled = !editMode;

    documentSvc.FetchPrinterPaperSourceList(documentPrinterID, FetchPaperSource_Callback);
    documentSvc.FetchPrinterPaperSizeList(documentPrinterID, FetchPaperSize_Callback);
}

function EnableValidators(state) {
    EnableValidator(cboPaperSource.id, state);
    EnableValidator(cboPaperSize.id, state);
    EnableValidator(cboPrintOn.id, state);
}

// enables/disables validator
function EnableValidator(ControlID, state) {
    var objValidator = null, validatorID = null;

    validatorID = ControlID.replace('_cboDropDownList', '') + '_valRequired';

    objValidator = document.getElementById(validatorID);

    if (objValidator) ValidatorEnable(objValidator, state);
}

function FetchPaperSource_Callback(response) {
    var paperSources, opt, title;
    if (CheckAjaxResponse(response, documentSvc.url)) {
        paperSources = response.value.PrinterPaperSources;

        printerCanDuplex = response.value.PrinterCanDuplex;

        ClearDropDown(cboPaperSource);

	    // add blank
        opt = AddOption(cboPaperSource, "", "")

	    var optGroupValid = document.createElement("OPTGROUP");
	    optGroupValid.label = "Valid";
	    var optGroupInValid = document.createElement("OPTGROUP");
	    optGroupInValid.label = "Not Valid";

	    var validGrpAdded = false, inValidGrpAdded = false;
		
		for(index=0; index<paperSources.length; index++) {
            if (paperSources[index].IsValid) {
                if (!validGrpAdded) {
                    cboPaperSource.appendChild(optGroupValid);
                    validGrpAdded = true;
                }
                title = validIcon;
            }
		    else {
		        if (!inValidGrpAdded) {
		            cboPaperSource.appendChild(optGroupInValid);
		            inValidGrpAdded = true;
		        }
		        title = invalidIcon;
		    }

		    opt = AddOption(cboPaperSource, paperSources[index].ID, paperSources[index].SourceName)
		    opt.title = title;
		}

		if (setDropDownValue) cboPaperSource.value = paperSourceID;
		$("#" + cboPaperSource.id).msDropDown();
		EnableValidator(cboPaperSource.id, editMode);
		
		PopulateDuplexDropdownValues();
    }
}

function FetchPaperSize_Callback(response) {
    var paperSizes, opt;
    if (CheckAjaxResponse(response, documentSvc.url)) {
        paperSizes = response.value.PrinterPaperSizes;

        ClearDropDown(cboPaperSize);

        // add blank		
        opt = AddOption(cboPaperSize, "", "")

        var optGroupValid = document.createElement("OPTGROUP");
        optGroupValid.label = "Valid";
        var optGroupInValid = document.createElement("OPTGROUP");
        optGroupInValid.label = "Not Valid";

        var validGrpAdded = false, inValidGrpAdded = false;

        for (index = 0; index < paperSizes.length; index++) {
            if (paperSizes[index].IsValid) {
                if (!validGrpAdded) {
                    cboPaperSize.appendChild(optGroupValid);
                    validGrpAdded = true;
                }
                title = validIcon;
            }
            else {
                if (!inValidGrpAdded) {
                    cboPaperSize.appendChild(optGroupInValid);
                    inValidGrpAdded = true;
                }
                title = invalidIcon;
            }

            opt = AddOption(cboPaperSize, paperSizes[index].ID, paperSizes[index].PaperName)
            opt.title = title;
        }

        if (setDropDownValue) cboPaperSize.value = paperSizeID;
        $("#" + cboPaperSize.id).msDropDown();
        EnableValidator(cboPaperSize.id, editMode);
    }

    DisplayLoading(false);
}

function PopulateDuplexDropdownValues() {

    var title, opt;

    ClearDropDown(cboPrintOn);

    // add blank
    AddOption(cboPrintOn, "", "");

    var optGroupValid = document.createElement("OPTGROUP");
    optGroupValid.label = "Valid";
    var optGroupInValid = document.createElement("OPTGROUP");
    optGroupInValid.label = "Not Valid";

    // some options are always valid
    title = validIcon;
    cboPrintOn.appendChild(optGroupValid);
    opt = AddOption(cboPrintOn, "-1", "Printer default");
    opt.title = title;
    opt = AddOption(cboPrintOn, "1", "One side");
    opt.title = title;

    // remaining options are only valid if the printer can duplex
    if (!printerCanDuplex) {
        cboPrintOn.appendChild(optGroupInValid);
        title = invalidIcon;
    }
    opt = AddOption(cboPrintOn, "2", "Both sides, vertically");
    opt.title = title;
    opt = AddOption(cboPrintOn, "3", "Both sides, horizontally");
    opt.title = title;

    if (setDropDownValue) cboPrintOn.value = printOnID;
    $("#" + cboPrintOn.id).msDropDown();
    EnableValidator(cboPrintOn.id, editMode);
}

function AddOption(objDropDown, val, text) {
    var opt;
    opt = document.createElement("OPTION");
    objDropDown.options.add(opt);

    SetInnerText(opt, text);
    opt.value = val;

    return opt;
}

function ClearDropDown(objDropDown) {
    // clear options from select
    objDropDown.options.length = 0;

    // clear option groups from select
    var optGroups = objDropDown.getElementsByTagName("OPTGROUP");
    var numOfGrps = optGroups.length;

    for (var i = numOfGrps; i >= 0; i--) {
        if (optGroups[i]) objDropDown.removeChild(optGroups[i]);
    }

    // clear options from select
    objDropDown.options.length = 0;
    $("#" + objDropDown.id).msDropDown();
}

addEvent(window, "load", Init);