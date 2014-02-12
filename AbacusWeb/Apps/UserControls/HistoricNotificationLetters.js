var HistoricNotificationLetters_btnViewID;
var HistoricNotificationLetters_FilterClientID;
var HistoricNotificationLetters_CurrentPage;


function HistoricNotificationLetters_Init() {
    HistoricNotificationLetters_SVC = new Target.Abacus.Web.Apps.WebSvc.SUCLevels_class();
    //HistoricNotificationLetters_ResultsTable = GetElement("HistoricNotificationLetters_Results");
    //HistoricNotificationLetters_PagingLinks = GetElement("HistoricNotificationLetters_PagingLinks");
    //HistoricNotificationLetters_Information = GetElement("HistoricNotificationLetters_Information");
    //HistoricNotificationLetters_BtnNotify = GetElement(HistoricNotificationLetters_BtnNotifyID, true);
    var HistoricNotificationLetters_BtnView = GetElement(HistoricNotificationLetters_btnViewID, true);
    HistoricNotificationLetters_BtnView.disabled = true;
    HisstoryNotification_FetchHistoryNotificationLetters(HistoricNotificationLetters_CurrentPage);
}

function HisstoryNotification_FetchHistoryNotificationLetters(page) {
    DisplayLoading(true);
    if (page == undefined) page = 1;
    //if (selectedID == undefined) selectedID = 0;
    HistoricNotificationLetters_CurrentPage = page;

    HistoricNotificationLetters_SVC.FetchHistoryNotificationLetters(page, 0, HistoricNotificationLetters_FilterClientID, FetchHistoryNotificationLetters_Callback);
}

function FetchHistoryNotificationLetters_Callback(response) {
    var index, tr, td, radioButton, str, link, sCount;
    var info = '';
    var allowNotifyAction = true;
    var allowPreview = true;

    if (CheckAjaxResponse(response, HistoricNotificationLetters_SVC.url)) {

        var setContribEarlyDateInfo = false;

        HistoricNotificationLetters_PendingServiceUserContributionLevels = response.value.Results;
        sCount = HistoricNotificationLetters_PendingServiceUserContributionLevels.length;
        ClearTable(HistoricNotificationLetters_ResultsTable);

        if (sCount <= 0) {
            allowNotifyAction = false;
            allowPreview = false;
        }

        for (index = 0; index < sCount; index++) {

            tr = AddRow(HistoricNotificationLetters_ResultsTable);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "PsuclSelect", HistoricNotificationLetters_PendingServiceUserContributionLevels[index].DocumentID, HistoricNotificationLetters_RadioButton_Click);


            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";



            td = AddCell(tr, "");
            strLinkHoverMsg = "Click to download file";
            link = AddLink(td, Date.strftime("%d/%m/%Y", HistoricNotificationLetters_PendingServiceUserContributionLevels[index].DateFrom), HistoricNotificationLetters_GetViewURL(HistoricNotificationLetters_PendingServiceUserContributionLevels[index].DocumentID), strLinkHoverMsg);
            link.className = "transBg";
            

            td = AddCell(tr, Date.strftime("%d/%m/%Y", HistoricNotificationLetters_PendingServiceUserContributionLevels[index].DateTo));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, HistoricNotificationLetters_PendingServiceUserContributionLevels[index].AssessmentTypeDesc);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(HistoricNotificationLetters_PendingServiceUserContributionLevels[index].Assessedcharges).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(HistoricNotificationLetters_PendingServiceUserContributionLevels[index].ChargeableCost).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(HistoricNotificationLetters_PendingServiceUserContributionLevels[index].ContributionLevel).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(HistoricNotificationLetters_PendingServiceUserContributionLevels[index].PlannedAdditionalCost).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

//            // select the item?
//            if (HistoricNotificationLetters_SelectedID == HistoricNotificationLetters_PendingServiceUserContributionLevels[index].ID || HistoricNotificationLetters_PendingServiceUserContributionLevels.length == 1) {
//                // radioButton.click();
//            }

//            if (setContribEarlyDateInfo == false) {
//                info = "<br /><img src='" + HistoricNotificationLetters_WarningImg + "' />&nbsp;Contribution Levels from " + Date.strftime("%d/%m/%Y", HistoricNotificationLetters_PendingServiceUserContributionLevels[index].EarliestDateOfContribtionLevelChanged) + " have changed; the Service User has not yet been notified of these changes.";
//            }

            if (HistoricNotificationLetters_PendingServiceUserContributionLevels[index].SDSSuppressCollectionOfContributions == true) {
                allowNotifyAction = false;
            }

        }

        // load the paging link HTML
        HistoricNotificationLetters_PagingLinks.innerHTML = response.value.PagingLinks;

    } else {
        allowNotifyAction = false;
        allowPreview = false;
    }
   
   
    DisplayLoading(false);

   
}

function HistoricNotificationLetters_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = HistoricNotificationLetters_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = HistoricNotificationLetters_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = HistoricNotificationLetters_ResultsTable.tBodies[0].rows[index];
            HistoricNotificationLetters_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            HistoricNotificationLetters_SelectedID = rdo.value;
            var HistoricNotificationLetters_BtnView = GetElement(HistoricNotificationLetters_btnViewID, true);
            HistoricNotificationLetters_BtnView.disabled = false;
            
        } else {
            HistoricNotificationLetters_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof HistoricNotificationLetters_SelectedItemChanged == "function") {
        HistoricNotificationLetters_SelectedItemChanged(HistoricNotificationLetters_GetSelectedObject(HistoricNotificationLetters_SelectedID));
    }
}

function HistoricNotificationLetters_btnView_Click() {
    document.location.href = HistoricNotificationLetters_GetViewURL(HistoricNotificationLetters_SelectedID);
}

function HistoricNotificationLetters_GetSelectedObject(id) {
    if (HistoricNotificationLetters_PendingServiceUserContributionLevels != null) {
        var collectionLength = HistoricNotificationLetters_PendingServiceUserContributionLevels.length;
        for (var j = 0; j < collectionLength; j++) {
            if (HistoricNotificationLetters_PendingServiceUserContributionLevels[j].ID == id) {
                return HistoricNotificationLetters_PendingServiceUserContributionLevels[j];
            }
        }
    }
}

function HistoricNotificationLetters_GetViewURL(id) {
    return SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Documents/DocumentDownloadHandler.axd?id=" + id + "&saveas=1";
}

//FetchHistoryNotificationLetters
addEvent(window, "load", HistoricNotificationLetters_Init);