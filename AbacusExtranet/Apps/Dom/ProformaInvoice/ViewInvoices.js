
var contractSvc, currentPage, paymentScheduleId, queried, mismatch, tolerance, batchStatus, rdbDcrFilterDefaultID, rdbDcrFilterNoID, rdbDcrFilterYesID;
var selectedBatchID, selectedInvoiceBatchID, domContractID, selectedInvoiceID, queryDesc, queryType, btnRecalculateID, btnRemoveID, selectedQueriedFilter, selectedMismatchFilter;
var tblInvoices, divPagingLinks, btnPrint, btnRemove, btnRecalculate, btnViewInvoiceLines, btnViewCostedVisits, cboQueried, cboMismatch, intNoRows, txtTolerance, btnApplyTolerance;
var populating = false;
var listFilter, listFilterSUReference = "", listFilterSUName = "";
var dcrFilter;
var statusAwait, statusVerified;
var estabId, contractId, batchStatusDesc;
var chkAwait, chkVer;
var filter = "false";
var btnUnVerify, btnVerify, btnDelete, canVerifyUnverify, canDeleteInvoice;
var hidNotesType;
var selectedNote, paymentMismatch, mismatchtolerance;
var VerificationTextID, VerificationText;

function Init() {
    contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();

	tblInvoices = GetElement("tblInvoices");
	divPagingLinks = GetElement("Invoice_PagingLinks");
	cboQueried = GetElement("cpe_cboQueried_cboDropDownList");
	cboMismatch = GetElement("cpe_cboMismatches_cboDropDownList");
	txtTolerance = GetElement("cpe_txtTolerance_txtTextBox");
	//btnApplyTolerance = GetElement("cpe_txtTolerance_btnApplyTolerance");
	hidNotesType = GetElement("hidNotesType");
	// need to apply check if btn verify and btn unverify exists or not
	// thats whay using the standard method
	if (canVerifyUnverify == "true") {
        btnUnVerify = GetElement("btnUnVerify");
        btnVerify = GetElement("btnVerify");
    }
    // needs to apply check if btndelete exists or not
    if (canDeleteInvoice == "true") {
        btnDelete = GetElement("btnDelete");
    }
	selectedInvoiceID = GetQSParam(document.location.search, "selectedInvoiceID");
	if(!selectedInvoiceID) selectedInvoiceID = 0;
	
	selectedQueriedFilter = GetQSParam(document.location.search, "q");
	if(!selectedQueriedFilter) selectedQueriedFilter = 0;
	cboQueried.value = selectedQueriedFilter;
	
	selectedMismatchFilter = GetQSParam(document.location.search, "m");
	if(!selectedMismatchFilter) selectedMismatchFilter = 0;
	cboMismatch.value = selectedMismatchFilter;
	
	ToggleTolerance();
		
	//btnPrint = GetElement("btnPrint");
	btnRemove = GetElement(btnRemoveID, true);
	btnRecalculate = GetElement(btnRecalculateID, true);
	btnViewInvoiceLines = GetElement("btnViewInvoiceLines");
	btnViewCostedVisits = GetElement("btnViewCostedVisits");
	btnVisits = GetElement("btnVisits");
	chkAwait = GetElement("chkAwait");
	chkVer = GetElement("chkVer");
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("S/U Reference", GetElement("thSURef"));
	listFilter.AddColumn("Service User", GetElement("thSUName"));

	statusAwait = GetQSParam(document.location.search, "await");
	statusVerified = GetQSParam(document.location.search, "ver");
	if (statusAwait == "true") {
	    chkAwait.checked = true;
	}
	else {
	    chkAwait.checked = false;
	}
	if (statusVerified == "true") {
	    chkVer.checked = true;
	}
	else {
	    chkVer.checked = false;
	}
	
	filter = GetQSParam(document.location.search, "filter");
	if (filter != "true") {
	    filter = "false";
	}

	var tolerance = GetQSParam(document.location.search, "t");
	if (null != tolerance) {
	    if (cboMismatch.value == "Mismatched" || cboMismatch.value == "Matched") {
	            txtTolerance.value = tolerance;
	    }
	}

	FetchInvoiceList(currentPage, selectedInvoiceID);

}



function EnableDisableVerifyUnVerify() {

    btnUnVerify.disabled = true;
    btnVerify.disabled = false;
    btnRecalculate.disabled = false;

    if (canDeleteInvoice == "true") {
        btnDelete.disabled = true;
    }


    var rowCount = $('#tblInvoices >tbody > tr').length;
    var TotalVerified = 0;
    var TotalUnVerified = 0;
    var value;
    $('#tblInvoices >tbody > tr').each(function() {
        var status = $(this).find("td").eq(3).html();
        if (status == "Awaiting Verification") {
            TotalUnVerified = TotalUnVerified + 1;
        }
        if (status == "Verified") {
            TotalVerified = TotalVerified + 1;
        }
    });

     //if no invoice is selected then enable verify button only if 1 or more unverified invoices are present
    //if no invoice is selected then enable unverify button only if 1 or more verified invoices are present
    
    //if (selectedItem.id == 0) {
    if (canVerifyUnverify == "true") {
        // if none of the invoices is selected then.
        if (selectedInvoiceID == 0) {
            if (TotalUnVerified > 0) {
                btnVerify.disabled = false;
                if (canDeleteInvoice == "true") {
                    btnDelete.disabled = false;
                }
            }
            else {
                btnVerify.disabled = true;
                if (canDeleteInvoice == "true") {
                    btnDelete.disabled = true;
                }
            }

            if (TotalVerified > 0) {
                btnUnVerify.disabled = false;
            }
            else {
                btnUnVerify.disabled = true;
            }
        }
        else {
            EnableDisableVerifyUnVerifyByItemID(selectedInvoiceID);
        }
    }


    // if all invoiced are either visit amendments or  Recalculation adustments then hide all change status buttons
    var everyBatchIsofType8OR16 = true; // 8 visit amendment  16 Recalculation Adjustment  
    var txtBatchtypeList = $("[id$='hidBatchType']");
    for (i = 0; i <= txtBatchtypeList.length - 1; i++) {
        if (!(txtBatchtypeList[i].value == 8 || txtBatchtypeList[i].value == 16)) {
            everyBatchIsofType8OR16 = false;
        }
    }

    if (everyBatchIsofType8OR16) {
        btnDelete.disabled = true;
        btnUnVerify.disabled = true;
        btnVerify.disabled = true;
    }
}

function EnableDisableVerifyUnVerifyByItemID(invId) {
    for (index = 0; index < tblInvoices.tBodies[0].rows.length; index++) {
        rdo = tblInvoices.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = tblInvoices.tBodies[0].rows[index];
            tblInvoices.tBodies[0].rows[index].className = "highlightedRow";
            selectedInvoiceID = rdo.value;
            if (CanChangestatus(tblInvoices.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[3].value)) {
                if (tblInvoices.tBodies[0].rows[index].cells[3].innerHTML == "Awaiting Verification") {
                    if (canVerifyUnverify == "true") {
                        btnUnVerify.disabled = true;
                        btnVerify.disabled = false;
                    }
                    if (canDeleteInvoice == "true") {
                        btnDelete.disabled = false;
                    }
                } else {
                    if (canVerifyUnverify == "true") {
                        btnVerify.disabled = true;
                        btnUnVerify.disabled = false;
                    }
                    if (canDeleteInvoice == "true") {
                        btnDelete.disabled = true;
                    }
                }
            }
        }
    }
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "S/U Reference":
			listFilterSUReference = column.Filter;
			break;
        case "Service User":
			listFilterSUName = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchInvoiceList(1);
}

function FetchInvoiceList(page, invoiceID) {
    currentPage = page;
	DisplayLoading(true);
	switch (selectedNote)
	{
	    case "No Note":
	        queried = 0;
	        break;
	    case "Private Note":
	        queried = 1
	        break;    
	    case "Invoice Note":
	        queried = 2;
	        break;
	    case "Any Note":
	        queried = 3;
	        break;
	    default:
	        queried = -1;
	        break;
	}
	
	switch (cboMismatch.value)
	{
	    case "Mismatched":
	        mismatch = 1;
	        tolerance = txtTolerance.value;
	        break;
	    case "Matched":
	        mismatch = 2;
	        //tolerance = mismatchtolerance;
	        tolerance = txtTolerance.value;
	        break;
	    default:
	        mismatch = 0;
	        tolerance = "0";
	        break;
	}
	// set dcr filter
	var rdo;
	switch (dcrFilter) 
	{   
	   	case -2:
	   	    rdo = GetElement(rdbDcrFilterDefaultID);
	   	    rdo.checked = true;
    	    break;
	    case  0:
	        rdo = GetElement(rdbDcrFilterNoID);
	        rdo.checked = true;
	        break;
	    case -1:
	        rdo = GetElement(rdbDcrFilterYesID);
	        rdo.checked = true;
	        break;
	}	

	if(btnRecalculate) btnRecalculate.disabled = true;
	if(btnRemove) btnRemove.disabled = true;
	btnViewInvoiceLines.disabled = true;
	btnViewCostedVisits.disabled = true;
	btnVisits.disabled = true;

	contractSvc.FetchProformaInvoiceResults(
	    currentPage,
	    paymentScheduleId, 
	    invoiceID, 
	    0, 
	    mismatch, 
	    tolerance, 
	    listFilterSUReference,
	    listFilterSUName,
	    dcrFilter,
	    (statusAwait === "true" ? true : false),
	    (statusVerified === "true" ? true : false),
	    queried,
	    FetchInvoiceList_Callback);
}
function FetchInvoiceList_Callback(response) {
    var index, invoices, str, img, imgSrc, hidQuery, hidQueryType, rounding, hidBatchType;
    
    if(CheckAjaxResponse(response, contractSvc.url)) {
        invoices = response.value.Invoices;
        ClearTable(tblInvoices);
        
        populating = true;
        intNoRows = invoices.length;
        if (invoices.length == 0) {
            selectedInvoiceID = 0;
            btnVerify.disabled = true;
            btnUnVerify.disabled = true;
            btnDelete.disabled = true;
        }
        for(index=0; index<invoices.length; index++) {
        
            tr = AddRow(tblInvoices);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "InvoiceSelect", invoices[index].InvoiceID, RadioButton_Click);
			hidQuery = AddInput(td, "hidQuery", "hidden", "", "", (invoices[index].QueryDescription) ? invoices[index].QueryDescription : "");
			hidQueryType = AddInput(td, "hidQueryType", "hidden", "", "", invoices[index].QueryType);
			hidBatchType = AddInput(td, "hidBatchType", "hidden", "", "", invoices[index].DomProformaInvoiceBatchTypeID);
			
			AddCell(tr, invoices[index].SvcUsrName);
			AddCell(tr, invoices[index].SvcUsrReference);
			AddCell(tr, invoices[index].StatusDesc);
			VerificationTextID = invoices[index].VerificationTextID

			str = invoices[index].DcrRounding;
			if (str == "True") {
			    str = "The Duration Claimed value of one or more visit was altered during application of rounding rules"
			    imgSrc = SITE_VIRTUAL_ROOT + "Images/rounding.png";
			    rounding = "<img align='middle' src='" + imgSrc + "' alt='" + str + "' style='margin-left:5px;'  />";
			}
			else {
			    str = "";
			    rounding = "";
			}
			
			td = AddCell(tr, "");
			td.innerHTML = "<div style='float:left;'>" + invoices[index].Payment.toString().formatCurrency() + "</div>";
			td.innerHTML += rounding;
			
			str = invoices[index].PaymentRef;
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
			
			td = AddCell(tr, "");
			td.innerHTML = invoices[index].Claimed.toString().formatCurrency();
			
			// query
			if (invoices[index].QueryType == 0) {
			    str = "Click here to create your notes for this invoice";
				imgSrc = SITE_VIRTUAL_ROOT + "Images/info18_grey.png";
            }else if (invoices[index].QueryType == 1) {
                str = "Click here to view/edit/delete your notes for this invoice";
			    imgSrc = SITE_VIRTUAL_ROOT + "Images/info18_blue.png";
			}
			else if (invoices[index].QueryType == 2) {
			    str = "Click here to view/edit/delete your notes for this invoice";
			    imgSrc = SITE_VIRTUAL_ROOT + "Images/info18_amber.png";
			}
			
			td = AddCell(tr, "");
			link = AddLink(td, "", "javascript:ShowQuery(" + index + ");", str);
			link.className = "transBg";
			img = new Image();
            img.style.marginLeft = "1.5em";
			img.src = imgSrc;
			link.appendChild(img);



			if (selectedInvoiceID != 0) {
			    if (invoices[index].InvoiceID == selectedInvoiceID || (currentPage == 1 && invoices.length == 1)) {
			        radioButton.click();
			    }
			}


        }
        GetVerificationText(VerificationTextID);
        populating = false;
        
        // load the paging link HTML
        divPagingLinks.innerHTML = response.value.PagingLinks;

    }
    DisplayLoading(false);

    EnableDisableVerifyUnVerify();

}

function GetVerificationText(id) {
    contractSvc.FetchVerificationTextNonResidential(id, GetVerificationText_Callnack);
}

function GetVerificationText_Callnack(response) {
    if (CheckAjaxResponse(response, contractSvc.url)) {
        VerificationText = response.value.SuccessMessage;
    }
}


function RadioButton_Click() {
    var index, rdo, selectedRow, batchTypeID, batchStatusID;
	for (index = 0; index < tblInvoices.tBodies[0].rows.length; index++){
		rdo = tblInvoices.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblInvoices.tBodies[0].rows[index];
			tblInvoices.tBodies[0].rows[index].className = "highlightedRow";
			selectedInvoiceID = rdo.value;

			//Update Url In Session State, this is used by the back command
			currentUrl = document.location.href;
			currentUrl = currentUrl.substr(currentUrl.indexOf(SITE_VIRTUAL_ROOT));

			newUrl = AddQSParam(RemoveQSParam(currentUrl, "selectedInvoiceID"), "selectedInvoiceID", selectedInvoiceID);
			var $response = contractSvc.UpdateSessionBackURL(currentUrl, newUrl)
			if (!CheckAjaxResponse($response, contractSvc.url)) {
			    return false;
			}
			
	        if (batchStatus == 1) {
			    if(btnRecalculate) btnRecalculate.disabled = false;
			    if (intNoRows > 1) {
			        if(btnRemove) btnRemove.disabled = false;
			    }
			}
	        btnViewInvoiceLines.disabled = false;
	        btnViewCostedVisits.disabled = false;
	        btnVisits.disabled = false;
	        EnableDisableVerifyUnVerifyByItemID(selectedInvoiceID);
		} else {
			tblInvoices.tBodies[0].rows[index].className = ""
		}
	}
}

function CanChangestatus(batchTypeId) {

    if (batchTypeId == 8 || batchTypeId == 16)
        return false;
    else
        return true;
}

function cboQueried_Click() {
    GetNoteValue();
}

function cboMismatch_Click() {
    ToggleTolerance();
}

function btnDelete_Click() {

    var isconfirmed = false;
    var confirmtextSpecific = "The selected Pro forma Invoice will be deleted." + 
                              "\n Please confirm that you understand this and that you wish to continue.";
    var confirmtextGeneral = "All “Awaiting Verification“ Pro forma Invoices that are listed in the table above " +
                              "(i.e. that meet filtering criteria) will be deleted. " +
                              "\n Please confirm that you understand this and that you wish to continue."
    if (selectedInvoiceID > 0) {
        if (confirm(confirmtextSpecific)) {
            isconfirmed = true;
        }
    }
    else {
        if (confirm(confirmtextGeneral)) {
            isconfirmed = true;
        }
    }
    //isconfirmed = false
    if (isconfirmed) {

        // run verify process.
        var serviceResponse = contractSvc.ChangeProformaInvoiceStatus(
	    currentPage,
	    paymentScheduleId,
	    selectedInvoiceID,
	    queried,
	    mismatch,
	    tolerance,
	    listFilterSUReference,
	    listFilterSUName,
	    dcrFilter,
	    statusAwait,
	    statusVerified,
	    "Delete"
	    );
        if (CheckAjaxResponse(serviceResponse, contractSvc.url) == false) {
            return false;
        }
        selectedInvoiceID = 0;
        FetchInvoiceList(currentPage, selectedInvoiceID);
       
    }

}

function btnVerify_Click() {

    Ext.MessageBox.confirm('Verify', VerificationText, function (btn) {
        if (btn === 'yes') {
                // run verify process.
                var serviceResponse = contractSvc.ChangeProformaInvoiceStatus(
	            currentPage,
	            paymentScheduleId,
	            selectedInvoiceID,
	            queried,
	            mismatch,
	            tolerance,
	            listFilterSUReference,
	            listFilterSUName,
	            dcrFilter,
	            statusAwait,
	            statusVerified,
	            "Verify"
	            );

                if (CheckAjaxResponse(serviceResponse, contractSvc.url) == false) {
                    return false;
                }

                /// populate invoices with same filter criteria.
                // set selectedInvoiceID = 0 . Beta 527
                selectedInvoiceID = 0;
                FetchInvoiceList(currentPage, selectedInvoiceID);
                btnVerify.disabled = true;
                btnUnVerify.disabled = false;
                btnRecalculate.disabled = true;

                if (canDeleteInvoice == "true") {
                    btnDelete.disabled = false;
                }

                EnableDisableVerifyUnVerify();
        }
    });
            
}

function btnUnVerify_Click() {

    Ext.MessageBox.confirm('UnVerify', VerificationText, function (btn) {
        if (btn === 'yes') {
            var serviceResponse = contractSvc.ChangeProformaInvoiceStatus(
	        currentPage,
	        paymentScheduleId,
	        selectedInvoiceID,
	        queried,
	        mismatch,
	        tolerance,
	        listFilterSUReference,
	        listFilterSUName,
	        dcrFilter,
	        statusAwait,
	        statusVerified,
	        "UnVerify");
            if (CheckAjaxResponse(serviceResponse, contractSvc.url) == false) {
                return false;
            }
            /// populate invoices with same filter criteria.
            // set selectedInvoiceID = 0 . Beta 527
            selectedInvoiceID = 0;
            FetchInvoiceList(currentPage, selectedInvoiceID);
            //document.location.href = window.location;

            EnableDisableVerifyUnVerify();
        }
    });
      
}

function btnBack_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}

function GetBackUrl() {
    var url = document.location.href;
    url = AddQSParam(RemoveQSParam(url, "selectedInvoiceID"), "selectedInvoiceID", selectedInvoiceID);
    return escape(url);
}

function btnVisits_Click() {
    document.location.href = "ManualEnterInvoice.aspx?=null" +
    "&estabid=" + estabId +
    "&contractid=" + contractId +
    "&pscheduleid=" + paymentScheduleId +
    "&invoiceid=" + selectedInvoiceID +
    "&id=" + selectedInvoiceID +
    "&mode=1" +
    "&pSWE=" + GetQSParam(document.location.href, "pSWE") +
    "&await=" + GetQSParam(document.location.href, "await") +
    "&ver=" + GetQSParam(document.location.href, "ver") +
    "&view=true" +  
    "&backUrl=" + GetBackUrl();
}

function btnViewVisits_Click() {
    document.location.href = "ViewInvoiceCostedVisits.aspx?id=" + selectedInvoiceID +
                             "&pScheduleId=" + paymentScheduleId +
                             "&await=" + statusAwait +
                             "&ver=" + statusVerified;  //+ 
//                             "&backUrl=" + GetBackUrl();
}

function btnViewInvoiceLines_Click() {
    document.location.href = "ViewInvoiceLines.aspx?id=" + selectedInvoiceID +
                             "&pScheduleId=" + paymentScheduleId +
                             "&await=" + statusAwait +
                             "&ver=" + statusVerified;   //+
//                              "&backUrl=" + GetBackUrl();
}

function btnRecalculate_Click() {
    if (window.confirm("Are you sure you wish to recalculate this invoice?")) {
        RecalculateInvoice(selectedInvoiceID);
    }
}

function RecalculateInvoice(invoiceID) {
    ShowProcessingModalDIV();
    contractSvc.RecalculateProformaInvoice(invoiceID, RecalculateInvoice_Callback);
}

function RecalculateInvoice_Callback(response) {
    HideModalDIV();
    if(CheckAjaxResponse(response, contractSvc.url)) {
        FetchInvoiceList(currentPage, selectedInvoiceID);
    }
}

function btnPrint_Click() {
    var url = "PrintInvoice.aspx?paymentScheduleId=" + paymentScheduleId + 
        "&queried=" + queried + 
        "&mismatch=" + mismatch + 
        "&tolerance=" + tolerance +
        "&filterSURef=" + listFilterSUReference +
        "&filterSUName=" + listFilterSUName;
    
    //OpenPopup(url, 75, 50, 1);
}

function ShowQuery(rowIndex) {
    selectedInvoiceID = tblInvoices.tBodies[0].rows[rowIndex].cells[0].getElementsByTagName("INPUT")[0].value;
    queryDesc = tblInvoices.tBodies[0].rows[rowIndex].cells[0].getElementsByTagName("INPUT")[1].value;
    queryType = tblInvoices.tBodies[0].rows[rowIndex].cells[0].getElementsByTagName("INPUT")[2].value;
    var d = new Apps.Dom.ProformaInvoice.Comment.Dialog("Notes", "Invoice Note:", queryDesc, queryType);
    if (queryType == 0) {
        queryType = 1;
    }
    Notes_Click(this, new Array(this, queryType));
    d.SetCallback(ShowQuery_Callback)
    d.SetType(2);
    d.Show();
}
function ShowQuery_Callback(evt, args) {
    var d = args[0];
    var result = args[1];
    switch(result) {
        case 2:
                contractSvc.ChangeProformaInvoiceQuery(selectedInvoiceID, d.FilterBox.value, hidNotesType.value, ChangeQuery_Callback);
                d.Hide();
            break;
        case 3:
            contractSvc.ChangeProformaInvoiceQuery(selectedInvoiceID, "", 0, ChangeQuery_Callback);
            hidNotesType.value = 1;
            d.Hide();
            break;
		default:
		    d.Hide();
		    break;
	}
}

function ChangeQuery_Callback() {
    FetchInvoiceList(currentPage);
    HideModalDIV();
}

function Notes_Click(evt, NotesType) {
    hidNotesType.value = NotesType[1];
}


function ToggleTolerance() {
    txtTolerance.disabled = (cboMismatch.value == "");
    if(!txtTolerance.disabled && txtTolerance.value == "") {
        txtTolerance.value = "0.50";
        tolerance = txtTolerance.value;
    }
    //btnApplyTolerance.disabled = txtTolerance.disabled;
}

function btnApplyTolerance_Click() {
    tolerance = txtTolerance.value;
}

function UpdateInvoiceStatus() {
    filter = "true";
    if (chkAwait.checked) {
        statusAwait = "true";
    }
    else { 
        statusAwait = "false"
    }
    if (chkVer.checked) {
        statusVerified = "true";
    }
    else {
        statusVerified = "false";
    }
}
// return the value of the radio button that is checked
// return an empty string if none are checked, or
// there are no radio buttons
function GetRDBValue() {
    var rdo;
    rdo = GetElement(rdbDcrFilterDefaultID);
    if (rdo.checked) {
        dcrFilter = -2;
    }
    rdo = GetElement(rdbDcrFilterNoID);
    if (rdo.checked) {
        dcrFilter = 0;
    }
    rdo = GetElement(rdbDcrFilterYesID);
    if (rdo.checked) {
        dcrFilter = -1;
    }
}

// return the value of the note drop down button that is checked
// return an empty string if none are selected
function GetNoteValue() {
    selectedNote = cboQueried.value;
}

function btnApplyFilters_Click() {
    tolerance = txtTolerance.value;
    FetchInvoiceList(currentPage, selectedInvoiceID);
}

function UnSelectAll() {

    $('#radio input').removeAttr('checked'); $("input:radio[name='InvoiceSelect']").each(function (i) {
        this.checked = false;
    });

    $('#radio input').removeAttr('checked'); $("input:radio[name='headerRadio']").each(function (i) {
        this.checked = false;
    });

    selectedInvoiceID = 0;

    EnableDisableVerifyUnVerify();

    btnViewInvoiceLines.disabled = true;
    btnViewCostedVisits.disabled = true;
    btnVisits.disabled = true;

}


addEvent(window, "load", Init);
