var GenericCreditorPaymentSelector_LookupService;
var GenericCreditorPaymentSelector_ResultsTable;
var GenericCreditorPaymentSelector_PagingLinks;
var GenericCreditorPaymentSelector_ListFilter;
var GenericCreditorPaymentSelector_ListFilter_CreditorRef = "";
var GenericCreditorPaymentSelector_ListFilter_CreditorName = "";
var GenericCreditorPaymentSelector_ListFilter_ContractNumber = "";
var GenericCreditorPaymentSelector_ListFilter_ServiceUser = "";
var GenericCreditorPaymentSelector_ListFilter_PaymentNumber = "";
var GenericCreditorPaymentSelector_GenericCreditorPayments;
var GenericCreditorPaymentSelector_ResidentialType = 1;
var GenericCreditorPaymentSelector_NonResidentialType = 2;
var GenericCreditorPaymentSelector_DirectPayment = 3;
var GenericCreditorPaymentSelector_WindowOpenMode_Redirect = 0;
var GenericCreditorPaymentSelector_WindowOpenMode_Popup = 1;
var GenericCreditorPaymentSelector_DomProviderInvoicesEditorUrl = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/ProviderInvoices/Edit.aspx";
var GenericCreditorPaymentSelector_CreateBatchUrl = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/CreditorPayments/Batches/Create.aspx";
var GenericCreditorPaymentSelector_AuthoriseUrl = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/CreditorPayments/Authorise.aspx";
var GenericCreditorPaymentSelector_SuspensionsUrl = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/CreditorPayments/Suspensions.aspx";
var GenericCreditorPaymentSelector_DomProviderInvoicesNotesUrl = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/ProviderInvoices/Notes.aspx";
var GenericCreditorPaymentSelector_SelectPaymentMsg = 'Please select a payment'
var GenericCreditorPaymentSelector_Charachter_Limit = 22;

function GenericCreditorPaymentSelector_Init() {
    
    GenericCreditorPaymentSelector_LookupService = new Target.Abacus.Web.Apps.WebSvc.CreditorPayments_class();
    GenericCreditorPaymentSelector_ResultsTable = GetElement("GCPaymentSelector_tblGCPaymentSelectors");
    GenericCreditorPaymentSelector_PagingLinks = GetElement("GCPaymentSelector_PagingLinks");

    // reset button availability i.e. disable etc
    GenericCreditorPaymentSelector_ResetButtonAvailability();
    GenericCreditorPaymentSelector_ResetBatchButtonAvailability();
    
    // setup list filters
    GenericCreditorPaymentSelector_ListFilter = new Target.Web.ListFilter(GenericCreditorPaymentSelector_ListFilter_Callback);
    GenericCreditorPaymentSelector_ListFilter.AddColumn("CreditorRef", GetElement("thCreditorRef"));
    GenericCreditorPaymentSelector_ListFilter.AddColumn("CreditorName", GetElement("thCreditorName"));
    GenericCreditorPaymentSelector_ListFilter.AddColumn("ContractNumber", GetElement("thContractNumber"));
    if (GenericCreditorPaymentSelector_ShowServiceUserColumn == true) {
        GenericCreditorPaymentSelector_ListFilter.AddColumn("ServiceUser", GetElement("thServiceUser"));
    }
    GenericCreditorPaymentSelector_ListFilter.AddColumn("PaymentNumber", GetElement("thPaymentNumber"));
    
    GenericCreditorPaymentSelector_FetchGenericCreditorPayments(GenericCreditorPaymentSelector_CurrentPage, GenericCreditorPaymentSelector_SelectedID);
}

function GenericCreditorPaymentSelector_ListFilter_Callback(column) {

    switch (column.Name.replace(/^\s*|\s*|\n*|\r*$/g, '')) {
        case "CreditorRef":
            GenericCreditorPaymentSelector_ListFilter_CreditorRef = column.Filter;
            break;
        case "CreditorName":
            GenericCreditorPaymentSelector_ListFilter_CreditorName = column.Filter;
            break;
        case "ContractNumber":
            GenericCreditorPaymentSelector_ListFilter_ContractNumber = column.Filter;
            break;
        case "ServiceUser":
            GenericCreditorPaymentSelector_ListFilter_ServiceUser = column.Filter;
            break;
        case "PaymentNumber":
            GenericCreditorPaymentSelector_ListFilter_PaymentNumber = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }

    GenericCreditorPaymentSelector_FetchGenericCreditorPayments(1, 0);
    
}

function GenericCreditorPaymentSelector_FetchGenericCreditorPayments(page, selectedID) {
    DisplayLoading(true);
    if (page == undefined) page = 0;
    if (selectedID == undefined) selectedID = 0;
    GenericCreditorPaymentSelector_CurrentPage = page;
    GenericCreditorPaymentSelector_LookupService.GetPagedGenericCreditorPayments(page, selectedID, GenericCreditorPaymentSelector_FilterClientID, GenericCreditorPaymentSelector_ListFilter_CreditorRef, GenericCreditorPaymentSelector_ListFilter_CreditorName, GenericCreditorPaymentSelector_ListFilter_ContractNumber, GenericCreditorPaymentSelector_ListFilter_ServiceUser, GenericCreditorPaymentSelector_ListFilter_PaymentNumber, GenericCreditorPaymentSelector_FilterClientID, GenericCreditorPaymentSelector_FilterDateFrom, GenericCreditorPaymentSelector_FilterDateTo, GenericCreditorPaymentSelector_FilterIncludeNonResidential, GenericCreditorPaymentSelector_FilterIncludeDirectPayment, GenericCreditorPaymentSelector_FilterPaymentStatusIncludeUnpaid, GenericCreditorPaymentSelector_FilterPaymentStatusIncludeAuthorised, GenericCreditorPaymentSelector_FilterPaymentStatusIncludePaid, GenericCreditorPaymentSelector_FilterPaymentStatusIncludeSuspended, GenericCreditorPaymentSelector_FilterPaymentStatusDateFrom, GenericCreditorPaymentSelector_FilterPaymentStatusDateTo, GenericCreditorPaymentSelector_FilterPaymentStatusIncludeExcludedFromCreditors, GenericCreditorPaymentSelector_FilterGenericContractID, GenericCreditorPaymentSelector_FilterGenericCreditorID, GenericCreditorPaymentSelector_FilterPaymentNumber, GenericCreditorPaymentSelector_FilterNonResidentialSuspensionReason, GenericCreditorPaymentSelector_FilterManuallySuspended, GenericCreditorPaymentSelector_FetchGenericCreditorPayments_Callback);
}

function GenericCreditorPaymentSelector_FetchGenericCreditorPayments_Callback(response) {

    var itemSelected = false;

    GenericCreditorPaymentSelector_GenericCreditorPayments = null;
    GenericCreditorPaymentSelector_ResetButtonAvailability();
    GenericCreditorPaymentSelector_ResetBatchButtonAvailability();
    
    if (CheckAjaxResponse(response, GenericCreditorPaymentSelector_LookupService.url)) {

        var index, tr, td, radioButton, str, link, sCount, currentCreditorPayment;

        GenericCreditorPaymentSelector_GenericCreditorPayments = response.value.Items;
        sCount = GenericCreditorPaymentSelector_GenericCreditorPayments.length;
        ClearTable(GenericCreditorPaymentSelector_ResultsTable);

        for (index = 0; index < sCount; index++) {

            currentCreditorPayment = GenericCreditorPaymentSelector_GenericCreditorPayments[index];

            tr = AddRow(GenericCreditorPaymentSelector_ResultsTable);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "GenericCreditorPaymentSelect", currentCreditorPayment.ID, GenericCreditorPaymentSelector_RadioButton_Click);

            // adding provider invoice id or direct payment id dependent on payment type
            hidID = document.createElement("input");
            hidID.type = "hidden";
            hidID.name = "hid_" + index + "_" + currentCreditorPayment.ID;
            hidID.value = currentCreditorPayment.ChildID;
            td.appendChild(hidID);

            // adding establishment id for use with dom provider invoices
            hidID = document.createElement("input");
            hidID.type = "hidden";
            hidID.name = "hid_" + index + "_" + currentCreditorPayment.EstablishmentID;
            hidID.value = currentCreditorPayment.EstablishmentID;
            td.appendChild(hidID);

            td = AddCell(tr, (currentCreditorPayment.CreditorRef.length == 0) ? ' ' : currentCreditorPayment.CreditorRef);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, (currentCreditorPayment.CreditorName.length > GenericCreditorPaymentSelector_Charachter_Limit) ? currentCreditorPayment.CreditorName.substring(0, GenericCreditorPaymentSelector_Charachter_Limit-1) : currentCreditorPayment.CreditorName);
            td = AddCell(tr, currentCreditorPayment.ContractNumber);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            if (GenericCreditorPaymentSelector_ShowServiceUserColumn) {
                td = AddCell(tr, (currentCreditorPayment.ServiceUserName.length > GenericCreditorPaymentSelector_Charachter_Limit) ? currentCreditorPayment.ServiceUserName.substring(0, GenericCreditorPaymentSelector_Charachter_Limit-1) : currentCreditorPayment.ServiceUserName);
            }

            td = AddCell(tr, "");
            link = AddLink(td,
                currentCreditorPayment.PaymentRef,
                "javascript:GenericCreditorPaymentSelector_ViewPayment(" + currentCreditorPayment.ID + ");", "View this Creditor Payment.");
            link.className = "transBg";

            td = AddCell(tr);
            td.innerHTML = String(currentCreditorPayment.Value).formatCurrency();

            td = AddCell(tr, currentCreditorPayment.TypeDescriptionAbbreviated);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";
            
            td = AddCell(tr, currentCreditorPayment.Status);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";
            if (currentCreditorPayment.NonResidentialSuspensionReason > 0) {
                if (currentCreditorPayment.NonResidentialSuspensionReasonColour != 'Transparent') {
                    td.bgColor = currentCreditorPayment.NonResidentialSuspensionReasonColour;
                }
                td.title = currentCreditorPayment.NonResidentialSuspensionReasonDescription;
            }            

            td = AddCell(tr, currentCreditorPayment.StatusDate.strftime("%d/%m/%Y"));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, "");
            SetInnerText(td, currentCreditorPayment.ExcludeFromCreditors ? "Yes" : "No");
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the item?
            if (GenericCreditorPaymentSelector_SelectedID == currentCreditorPayment.ID || ( GenericCreditorPaymentSelector_CurrentPage == 1 &&  GenericCreditorPaymentSelector_GenericCreditorPayments.length == 1)) {
                itemSelected = true;
                radioButton.click();
            }

        }

        if (sCount > 0) {

            if (response.value.NumberOfUnpaidPayments > 0) {
                if (GenericCreditorPaymentSelector_btnAuthorise) {
                    GenericCreditorPaymentSelector_btnAuthorise.disabled = false;
                    GenericCreditorPaymentSelector_btnAuthorise.title = 'Authorise all \'Unpaid\' Creditor Payments in the current results.';
                }
            }

            if (GenericCreditorPaymentSelector_btnSuspensions) {
                if (response.value.NumberOfPaidPayments > 0 && response.value.NumberOfAuthorisedPayments == 0 && response.value.NumberOfSuspendedPayments == 0 && response.value.NumberOfUnpaidPayments == 0) {
                    GenericCreditorPaymentSelector_btnSuspensions.disabled = true;
                    GenericCreditorPaymentSelector_btnSuspensions.title = 'Select filter criteria including non \'Paid\' Creditor Payments to Enable this button.';
                } else {
                    GenericCreditorPaymentSelector_btnSuspensions.disabled = false;
                    GenericCreditorPaymentSelector_btnSuspensions.title = 'Manage the suspension status of Creditor Payments in the current results.';
                }
            }

            if (response.value.NumberOfAuthorisedPayments > 0) {
                if (GenericCreditorPaymentSelector_btnCreateBatch) {
                    GenericCreditorPaymentSelector_btnCreateBatch.disabled = false;
                    GenericCreditorPaymentSelector_btnCreateBatch.title = 'Create a Batch including all \'Authorised\' Creditor Payments in the current results.';
                }
            }

        }

        if (itemSelected) {
            GenericCreditorPaymentSelector_RadioButton_Click();
        }

        // load the paging link HTML
        GenericCreditorPaymentSelector_PagingLinks.innerHTML = response.value.PagingLinks;
    }

    DisplayLoading(false);
}

function GenericCreditorPaymentSelector_RadioButton_Click() {

    var index, rdo, selectedRow, rowsLength, selectedObject;

    rowsLength = GenericCreditorPaymentSelector_ResultsTable.tBodies[0].rows.length;
    GenericCreditorPaymentSelector_ResetButtonAvailability();
    
    for (index = 0; index < rowsLength; index++) {
        rdo = GenericCreditorPaymentSelector_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {            
            selectedRow = GenericCreditorPaymentSelector_ResultsTable.tBodies[0].rows[index];
            GenericCreditorPaymentSelector_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            GenericCreditorPaymentSelector_SelectedID = rdo.value;
            selectedObject = GenericCreditorPaymentSelector_GetSelectedObject(GenericCreditorPaymentSelector_SelectedID);           
            if (GenericCreditorPaymentSelector_btnView) {
                GenericCreditorPaymentSelector_btnView.disabled = false;
                GenericCreditorPaymentSelector_btnView.title = 'View the selected Creditor Payment.';
            }
            if (GenericCreditorPaymentSelector_btnExcInc && selectedObject.Status != "Paid") {
                GenericCreditorPaymentSelector_btnExcInc.disabled = false;
                if (selectedObject.ExcludeFromCreditors == true) {
                    GenericCreditorPaymentSelector_btnExcInc.value = "Include";
                    GenericCreditorPaymentSelector_btnExcInc.title = 'Include the selected Creditor Payment.';
                } else {
                    GenericCreditorPaymentSelector_btnExcInc.value = "Exclude";
                    GenericCreditorPaymentSelector_btnExcInc.title = 'Exclude the selected Creditor Payment.';
                }
            }
            if (GenericCreditorPaymentSelector_btnNotes && selectedObject.Type == GenericCreditorPaymentSelector_NonResidentialType) {
                GenericCreditorPaymentSelector_btnNotes.disabled = false;
                GenericCreditorPaymentSelector_btnNotes.title = 'View Notes for the selected Creditor Payment.';
            }
            if (GenericCreditorPaymentSelector_btnSuspensions) {
                GenericCreditorPaymentSelector_btnSuspensions.disabled = false;
                GenericCreditorPaymentSelector_btnSuspensions.title = 'Manage the suspension status of the selected Creditor Payment.';
            }
        } else {
            GenericCreditorPaymentSelector_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof GenericCreditorPaymentSelector_SelectedItemChanged == "function") {
        GenericCreditorPaymentSelector_SelectedItemChanged(selectedObject);
    }
}

function GenericCreditorPaymentSelector_GetSelectedObject(id) {
    if (GenericCreditorPaymentSelector_GenericCreditorPayments != null) {
        var collectionLength = GenericCreditorPaymentSelector_GenericCreditorPayments.length;
        for (var j = 0; j < collectionLength; j++) {
            if (GenericCreditorPaymentSelector_GenericCreditorPayments[j].ID == id) {
                return GenericCreditorPaymentSelector_GenericCreditorPayments[j];
            }
        }
    }
}

function GenericCreditorPaymentSelector_BeforeNavigate() {

    var originalID = GetQSParam(document.location.search, GenericCreditorPaymentSelector_QsPaymentID);
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

    url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsPaymentID), GenericCreditorPaymentSelector_QsPaymentID, GenericCreditorPaymentSelector_SelectedID);
    SelectorWizard_newUrl = url;

    return true;

}

function GenericCreditorPaymentSelector_MruOnChange(mruListKey, selectedValue) {

    if (selectedValue.length > 0) {

        var url = document.location.href;

        url = RemoveQSParam(url, GenericCreditorPaymentSelector_QsPaymentID);
        url = AddQSParam(url, GenericCreditorPaymentSelector_QsPaymentID, selectedValue);
        document.location.href = url;

    }

}

function GenericCreditorPaymentSelector_GetBackUrl() {

    var selectedObject = GenericCreditorPaymentSelector_GetSelectedObject(GenericCreditorPaymentSelector_SelectedID);
    var url = document.location.href;

    if (selectedObject) {
        url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsPaymentID), GenericCreditorPaymentSelector_QsPaymentID, selectedObject.ID);
    }
        
    return escape(url);
}

function GenericCreditorPaymentSelector_btnNew_Click() {

    GenericCreditorPaymentSelector_NewPayment();   

}

function GenericCreditorPaymentSelector_btnView_Click() {
    
    GenericCreditorPaymentSelector_ViewPayment();

}

function GenericCreditorPaymentSelector_ResetButtonAvailability() {

    if (GenericCreditorPaymentSelector_btnView) {
        GenericCreditorPaymentSelector_btnView.disabled = true;
        GenericCreditorPaymentSelector_btnView.title = 'Select a Creditor Payment to Enable this button.';
    }

    if (GenericCreditorPaymentSelector_btnExcInc) {
        GenericCreditorPaymentSelector_btnExcInc.disabled = true;
        GenericCreditorPaymentSelector_btnExcInc.title = 'Select a Creditor Payment to Enable this button.';
    }

    if (GenericCreditorPaymentSelector_btnNotes) {
        GenericCreditorPaymentSelector_btnNotes.disabled = true;
        GenericCreditorPaymentSelector_btnNotes.title = 'Select a Non Residential Creditor Payment to Enable this button.';
    }

}

function GenericCreditorPaymentSelector_ResetBatchButtonAvailability() {

    if (GenericCreditorPaymentSelector_btnAuthorise) {
        GenericCreditorPaymentSelector_btnAuthorise.disabled = true;
        GenericCreditorPaymentSelector_btnAuthorise.title = 'Select filter criteria including \'Unpaid\' Creditor Payments to Enable this button.';
    }

    if (GenericCreditorPaymentSelector_btnSuspensions) {
        GenericCreditorPaymentSelector_btnSuspensions.disabled = true;
        GenericCreditorPaymentSelector_btnSuspensions.title = '';
    }

    if (GenericCreditorPaymentSelector_btnCreateBatch) {
        GenericCreditorPaymentSelector_btnCreateBatch.disabled = true;
        GenericCreditorPaymentSelector_btnCreateBatch.title = 'Select filter criteria including \'Authorised\' Creditor Payments to Enable this button.';
    }

}

function GenericCreditorPaymentSelector_NewPayment() {

    CareTypeSelector_Show(GenericCreditorPaymentSelector_NewPayment_CareTypeCallBackHandler);

}

function GenericCreditorPaymentSelector_NewPayment_CareTypeCallBackHandler(evt, args) {
    
    var answer = args[1];

    if (answer == 1) {

        var selectedCareType = CareTypeSelector_GetSelectedCareType();        
        var url;

        switch (selectedCareType) {

            case GenericCreditorPaymentSelector_NonResidentialType:

                url = GenericCreditorPaymentSelector_DomProviderInvoicesEditorUrl + "?id=null&estabID=" + GenericCreditorPaymentSelector_FilterNonResEstablishmentID + "&contractid=" + GenericCreditorPaymentSelector_FilterNonResContractID + "&clientid=" + GenericCreditorPaymentSelector_FilterNonResServiceUserID + "&mode=2";                
                break;
        }

        if (url) {
            GenericCreditorPaymentSelector_OpenUrl(url, 75, 50); 
        }

    }

    CareTypeSelector_Hide(args);

}

function GenericCreditorPaymentSelector_ViewPayment(id) {
    if (id) {
        GenericCreditorPaymentSelector_SelectedID = id;
    }
    
    var selectedObject = GenericCreditorPaymentSelector_GetSelectedObject(GenericCreditorPaymentSelector_SelectedID);

    if (selectedObject) {

        var url;
        var height;
        var width = 75;

        if (selectedObject.Type == GenericCreditorPaymentSelector_NonResidentialType) {
            
            url = GenericCreditorPaymentSelector_DomProviderInvoicesEditorUrl + "?id=" + selectedObject.ChildID + "&estabID=" + selectedObject.EstablishmentID + "&mode=1";
            height = 44;            
        }
        else if (selectedObject.Type == GenericCreditorPaymentSelector_DirectPayment) {
            url = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPPayment/View.aspx?id=" + selectedObject.ChildID;
            height = 35;
        }
        else {
            alert(selectedObject.TypeDescription + ' is an unconfigured creditor payment type.');
        }

        if (url) {
            GenericCreditorPaymentSelector_OpenUrl(url, width, height);           
        }

    } else {

        alert(GenericCreditorPaymentSelector_SelectPaymentMsg);

    }

}

function GenericCreditorPaymentSelector_btnExcInc_Click() {

    GenericCreditorPaymentSelector_ToggleGenericCreditorPaymentExcludeFromCreditorsFlag();

}

function GenericCreditorPaymentSelector_ToggleGenericCreditorPaymentExcludeFromCreditorsFlag() {

    var selectedObject = GenericCreditorPaymentSelector_GetSelectedObject(GenericCreditorPaymentSelector_SelectedID);

    if (selectedObject) {

        GenericCreditorPaymentSelector_LookupService.ToggleGenericCreditorPaymentExcludeFromCreditorsFlag(selectedObject.ID, GenericCreditorPaymentSelector_ToggleGenericCreditorPaymentExcludeFromCreditorsFlag_CallBack);
    
    } else {

        alert(GenericCreditorPaymentSelector_SelectPaymentMsg);
    
    }

}

function GenericCreditorPaymentSelector_ToggleGenericCreditorPaymentExcludeFromCreditorsFlag_CallBack(response) {

    if (CheckAjaxResponse(response, GenericCreditorPaymentSelector_LookupService.url)) {

        GenericCreditorPaymentSelector_FetchGenericCreditorPayments(GenericCreditorPaymentSelector_CurrentPage, GenericCreditorPaymentSelector_SelectedID);

    }

}

function GenericCreditorPaymentSelector_btnAuthorise_Click() {

    alert("Only \'Unpaid\' payments will be passed onto the authorisation process.");
    GenericCreditorPaymentSelector_OpenUrlWithCurrentSelections(GenericCreditorPaymentSelector_AuthoriseUrl, 75, 42, GenericCreditorPaymentSelector_QsPaymentStatusTypesUnpaid, 0);

}

function GenericCreditorPaymentSelector_btnCreateBatch_Click() {

    alert("Only \'Authorised\' payments that are not excluded will be passed onto the batch creation process.");
    GenericCreditorPaymentSelector_OpenUrlWithCurrentSelections(GenericCreditorPaymentSelector_CreateBatchUrl, 75, 55, GenericCreditorPaymentSelector_QsPaymentStatusTypesAuthorised, 0);

}

function GenericCreditorPaymentSelector_btnNotes_Click() {

    var selectedObject = GenericCreditorPaymentSelector_GetSelectedObject(GenericCreditorPaymentSelector_SelectedID);

    if (selectedObject) {

        var url = GenericCreditorPaymentSelector_DomProviderInvoicesNotesUrl + '?id=' + selectedObject.ChildID + '&estabID=' + selectedObject.EstablishmentID;
        GenericCreditorPaymentSelector_OpenUrl(url, 75, 30); 

    } else {

        alert(GenericCreditorPaymentSelector_SelectPaymentMsg);

    }

}

function GenericCreditorPaymentSelector_btnSuspensions_Click() {
    GenericCreditorPaymentSelector_OpenUrlWithCurrentSelections(GenericCreditorPaymentSelector_SuspensionsUrl, 75, 50, null);
}

function GenericCreditorPaymentSelector_OpenUrlWithCurrentSelections(baseUrlToOpen, optWidth, optHeight, optPaymentStatusType, optPaymentID) {

    // note baseUrlToOpen should not include query string
    if (baseUrlToOpen) {

        var qs = document.location.search;
        var url = baseUrlToOpen + qs;

        url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsFilterContractNumber), GenericCreditorPaymentSelector_QsFilterContractNumber, GenericCreditorPaymentSelector_ListFilter_ContractNumber);
        url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsFilterCreditorName), GenericCreditorPaymentSelector_QsFilterCreditorName, GenericCreditorPaymentSelector_ListFilter_CreditorName);
        url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsFilterCreditorReference), GenericCreditorPaymentSelector_QsFilterCreditorReference, GenericCreditorPaymentSelector_ListFilter_CreditorRef);
        url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsFilterPaymentReference), GenericCreditorPaymentSelector_QsFilterPaymentReference, GenericCreditorPaymentSelector_ListFilter_PaymentNumber);
        url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsFilterServiceUserName), GenericCreditorPaymentSelector_QsFilterServiceUserName, GenericCreditorPaymentSelector_ListFilter_ServiceUser);
        url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsServiceUserID), GenericCreditorPaymentSelector_QsServiceUserID, GenericCreditorPaymentSelector_FilterClientID);
        
        if (optPaymentStatusType) {
            url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsPaymentStatusTypes), GenericCreditorPaymentSelector_QsPaymentStatusTypes, optPaymentStatusType);
        }

        GenericCreditorPaymentSelector_OpenUrl(url, optWidth, optHeight, optPaymentID);

    }

}

function GenericCreditorPaymentSelector_OpenUrl(urlToOpen, optWidth, optHeight, optPaymentID) {

    if (urlToOpen) {

        var backUrl = GenericCreditorPaymentSelector_GetBackUrl();
        var url = urlToOpen;

        if (optPaymentID || optPaymentID == 0) {
            url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsPaymentID), GenericCreditorPaymentSelector_QsPaymentID, optPaymentID);
        } else {
            url = AddQSParam(RemoveQSParam(url, GenericCreditorPaymentSelector_QsPaymentID), GenericCreditorPaymentSelector_QsPaymentID, GenericCreditorPaymentSelector_SelectedID);
        }        

        if (GenericCreditorPaymentSelector_WindowOpenMode == GenericCreditorPaymentSelector_WindowOpenMode_Popup) {
            url = url + "&autopopup=1";
        }

        url = url + "&backUrl=" + backUrl;

        if (GenericCreditorPaymentSelector_WindowOpenMode == GenericCreditorPaymentSelector_WindowOpenMode_Popup) {
            OpenPopup(url, optWidth, optHeight, 1);
        } else {
            document.location.href = url;
        }

    }

}

function CreditorPaymentSuspensionsClosed() {

    document.location.href = document.location.href;

}

addEvent(window, "load", GenericCreditorPaymentSelector_Init);