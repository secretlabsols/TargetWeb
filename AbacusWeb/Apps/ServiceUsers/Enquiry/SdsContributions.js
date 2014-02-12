var SdsContributions_cpPendingContributionLevelsID, SdsContributions_cpPendingContributionLevels;

function SdsContributions_Init() {
    parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs');
}

function PendingSuclChangesSelector_FetchPendingServiceUserContributionLevelsList_Completed(items) {
    if (items == null || items == undefined || items.length == 0) {
        var headerControl, detailControl;
        SdsContributions_cpPendingContributionLevels = GetElement(SdsContributions_cpPendingContributionLevelsID);
        headerControl = SdsContributions_cpPendingContributionLevels.childNodes[0].childNodes[0];
        headerControl.className = "colPanelHeaderLinkEmpty";
        detailControl = SdsContributions_cpPendingContributionLevels.childNodes[1].childNodes[0].childNodes[0];
        detailControl.removeChild(detailControl.childNodes[0]);
    }
}

addEvent(window, "load", SdsContributions_Init);