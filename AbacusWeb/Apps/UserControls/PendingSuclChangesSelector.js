var PendingSuclChangesSelector_FilterClientID;
var PendingSuclChangesSelector_LookupService;
var PendingSuclChangesSelector_ResultsTable;
var PendingSuclChangesSelector_PagingLinks;
var PendingSuclChangesSelector_CurrentPage;
var PendingSuclChangesSelector_SelectedID;
var PendingSuclChangesSelector_PendingServiceUserContributionLevels;
var PendingSuclChangesSelector_TickImg;
var PendingSuclChangesSelector_CrossImg;
var PendingSuclChangesSelector_WarningImg;
var PendingSuclChangesSelector_Information;
var PendingSuclChangesSelector_BtnNotifyID, PendingSuclChangesSelector_BtnNotify;
var PendingSuclChangesSelector_BtnPreviewID, PendingSuclChangesSelector_BtnPreview;
var PendingSuclChangesSelector_HasCanGenerateSdsContributionNotificationsPermission;

function PendingSuclChangesSelector_Init() {

    PendingSuclChangesSelector_LookupService = new Target.Abacus.Web.Apps.WebSvc.SUCLevels_class();
    PendingSuclChangesSelector_ResultsTable = GetElement("PendingSuclChangesSelector_Results");
    PendingSuclChangesSelector_PagingLinks = GetElement("PendingSuclChangesSelector_PagingLinks");
    PendingSuclChangesSelector_Information = GetElement("PendingSuclChangesSelector_Information");
    PendingSuclChangesSelector_BtnNotify = GetElement(PendingSuclChangesSelector_BtnNotifyID, true);
    PendingSuclChangesSelector_BtnPreview = GetElement(PendingSuclChangesSelector_BtnPreviewID, true);
    PendingSuclChangesSelector_FetchPendingServiceUserContributionLevelsList(PendingSuclChangesSelector_CurrentPage, PendingSuclChangesSelector_SelectedID);
}

function PendingSuclChangesSelector_FetchPendingServiceUserContributionLevelsList(page, selectedID) {    
    DisplayLoading(true);
    if (page == undefined) page = 1;
    if (selectedID == undefined) selectedID = 0;
    PendingSuclChangesSelector_CurrentPage = page;
    PendingSuclChangesSelector_LookupService.FetchPendingServiceUserContributionLevelPendingChanges(page, selectedID, PendingSuclChangesSelector_FilterClientID, PendingSuclChangesSelector_FetchPendingServiceUserContributionLevelsList_Callback);
}

function PendingSuclChangesSelector_FetchPendingServiceUserContributionLevelsList_Callback(response) {
    
    var index, tr, td, radioButton, str, link, sCount;
    var info = '';
    var allowNotifyAction = true;
    var allowPreview = true;

    PendingSuclChangesSelector_PendingServiceUserContributionLevels = null;

    if (CheckAjaxResponse(response, PendingSuclChangesSelector_LookupService.url)) {

        var setContribEarlyDateInfo = false;

        PendingSuclChangesSelector_PendingServiceUserContributionLevels = response.value.Results;
        sCount = PendingSuclChangesSelector_PendingServiceUserContributionLevels.length;
        ClearTable(PendingSuclChangesSelector_ResultsTable);

        if (sCount <= 0) {
            allowNotifyAction = false;
            allowPreview = false;
        }

        for (index = 0; index < sCount; index++) {

            tr = AddRow(PendingSuclChangesSelector_ResultsTable);
            //td = AddCell(tr, "");
            //radioButton = AddRadio(td, "", "PsuclSelect", PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].ID, PendingSuclChangesSelector_RadioButton_Click);

            td = AddCell(tr);
            if (PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].ChargeableSpendPlanCost == 0 || PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].AssessmentType == 0) {
                td.innerHTML = "<img src='" + PendingSuclChangesSelector_CrossImg + "' />";
                allowNotifyAction = false;
            } else {
                td.innerHTML = "<img src='" + PendingSuclChangesSelector_TickImg + "' />";
            }
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, Date.strftime("%d/%m/%Y", PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].DateFrom));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, Date.strftime("%d/%m/%Y", PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].DateTo));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].AssessmentTypeDesc);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].AssessedCharge).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].ChargeableSpendPlanCost).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].ContributionLevel).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].PlannedAdditionalCost).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the item?
            if (PendingSuclChangesSelector_SelectedID == PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].ID || PendingSuclChangesSelector_PendingServiceUserContributionLevels.length == 1) {
               // radioButton.click();
            }

            if (setContribEarlyDateInfo == false) {
                info = "<br /><img src='" + PendingSuclChangesSelector_WarningImg + "' />&nbsp;Contribution Levels from " + Date.strftime("%d/%m/%Y", PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].EarliestDateOfContribtionLevelChanged) + " have changed; the Service User has not yet been notified of these changes.";
            }

            if (PendingSuclChangesSelector_PendingServiceUserContributionLevels[index].SDSSuppressCollectionOfContributions == true) {
                allowNotifyAction = false;
            }

        }

        // load the paging link HTML
        PendingSuclChangesSelector_PagingLinks.innerHTML = response.value.PagingLinks;

    } else {
        allowNotifyAction = false;
        allowPreview = false;
    }

    if (info != '') {
        PendingSuclChangesSelector_Information.innerHTML = info;
    }

    if (PendingSuclChangesSelector_BtnNotify) {
        if (PendingSuclChangesSelector_HasCanGenerateSdsContributionNotificationsPermission == false) {
            allowNotifyAction = false;
        }
        PendingSuclChangesSelector_BtnNotify.disabled = !allowNotifyAction;
    }

    PendingSuclChangesSelector_BtnPreview.disabled = !allowPreview;

    DisplayLoading(false);
    
    if (typeof (PendingSuclChangesSelector_FetchPendingServiceUserContributionLevelsList_Completed) == "function") {
        PendingSuclChangesSelector_FetchPendingServiceUserContributionLevelsList_Completed(PendingSuclChangesSelector_PendingServiceUserContributionLevels);
    }
    
}

function PendingSuclChangesSelector_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = PendingSuclChangesSelector_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = PendingSuclChangesSelector_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = PendingSuclChangesSelector_ResultsTable.tBodies[0].rows[index];
            PendingSuclChangesSelector_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            PendingSuclChangesSelector_SelectedID = rdo.value;
        } else {
            PendingSuclChangesSelector_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof PendingSuclChangesSelector_SelectedItemChanged == "function") {
        PendingSuclChangesSelector_SelectedItemChanged(PendingSuclChangesSelector_GetSelectedObject(PendingSuclChangesSelector_SelectedID));
    }
}

function PendingSuclChangesSelector_GetSelectedObject(id) {
    if (PendingSuclChangesSelector_PendingServiceUserContributionLevels != null) {
        var collectionLength = PendingSuclChangesSelector_PendingServiceUserContributionLevels.length;
        for (var j = 0; j < collectionLength; j++) {
            if (PendingSuclChangesSelector_PendingServiceUserContributionLevels[j].ID == id) {
                return PendingSuclChangesSelector_PendingServiceUserContributionLevels[j];
            }
        }
    }
}

addEvent(window, "load", PendingSuclChangesSelector_Init);