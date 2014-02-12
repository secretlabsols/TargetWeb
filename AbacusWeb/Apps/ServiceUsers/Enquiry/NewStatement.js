var cboBudgetPeriods, cboStatementLayout;
var NewStatement_btnViewID, NewStatement_btnView;
var NewStatement_btnGenerateID, NewStatement_btnGenerate;
var NewStatement_ClientBudgetPeriods = new Array();
var NewStatement_SdsTransactionsSvc;
var NewStatement_ClientBudgetNotFound = 'A client budget period could not be found at this time, please try again.';
var displayReportParameters = false;

function Init() {
    NewStatement_SdsTransactionsSvc = new Target.Abacus.Web.Apps.WebSvc.SdsTransactions_class();
    NewStatement_btnView = GetElement(NewStatement_btnViewID + '_btnReports', true);
    NewStatement_btnGenerate = GetElement(NewStatement_btnGenerateID, true);
    NewStatement_btnView.disabled = true;
    NewStatement_btnGenerate.disabled = true;
    
    cboBudgetPeriods = GetElement("cboBudgetPeriods_cboDropDownList");
    cboStatementLayout = GetElement("cboStatementLayout_cboDropDownList");

    setTimeout(function() { cboBudgetPeriods_OnChange(); }, 100);

}

function ClientBudgetPeriod(id, hasSdsTransactionReconsiderations) {
    this.ID = id;
    this.HasSdsTransactionReconsiderations = hasSdsTransactionReconsiderations;
}

function cboBudgetPeriods_OnChange() {
    if (NewStatement_btnView && NewStatement_btnGenerate) {
        var budgetPeriodID = cboBudgetPeriods.value;
        var cbp = GetClientBudgetPeriodObject(budgetPeriodID);        
        if (cbp) {
            displayReportParameters = true;
            if (cbp.HasSdsTransactionReconsiderations == true) {
                displayReportParameters = false;
                if (confirm('The selected budget period has outstanding transaction reconsiderations, you cannot view/create any statements for this budget period until these reconsiderations are processed.\n\nSelect OK to process these outstanding transaction reconsiderations or Cancel to leave this until another time.')) {
                    ShowProcessingModalDIV();
                    NewStatement_SdsTransactionsSvc.ReconsiderTransactionsByClientBudgetPeriod(budgetPeriodID, null, ReconsiderTransactions_Callback);
                }                
            }
            if (displayReportParameters) {
                SetReportparamters();
            }           
        } else {
            alert(NewStatement_ClientBudgetNotFound);
        }      
    }
}

function ReconsiderTransactions_Callback(response) {
    var budgetPeriodID = cboBudgetPeriods.value;
    displayReportParameters = false;
    var errorMessage = response.value.ErrMsg;
    if (errorMessage.Success == true) {
        if (response.value.ReconsiderExceptionsWarningsCount > 0) {
            alert('One or more warnings/exceptions were encountered whilst processing the outstanding ' +
                   'transaction reconsiderations for this budget period. Consequently this budget period ' +
                   'is not up to date.\n\nPlease create a "SDS Transaction Management" job, selecting this service ' +
                   'user and ticking the "Force reconsideration of the selected transactions?" option, ' +
                   'to fully process the budget period and view the reasons for the warnings/exceptions.');
        } else {
            var cbp = GetClientBudgetPeriodObject(budgetPeriodID);
            if (cbp) {
                displayReportParameters = true;
                cbp.HasSdsTransactionReconsiderations = false;
            } else {
                alert(NewStatement_ClientBudgetNotFound);
            }
        }
    } else {
        alert('An error occurred whilst processing outstanding transaction reconsiderations:\n\n' + errorMessage.Message);
    }    
    if (displayReportParameters) {
        SetReportparamters();
    }
    HideModalDIV();
}

function cboStatementLayout_OnChange() {
    if (displayReportParameters) {
        SetReportparamters();
    }
}

function SetReportparamters() {
    ReportsButton_AddParam(NewStatement_btnView.id, "ClientBudgetPeriodID", cboBudgetPeriods.value);
    ReportsButton_AddParam(NewStatement_btnView.id, "ShowBreakdownSection", true);
    ReportsButton_AddParam(NewStatement_btnView.id, "Format", cboStatementLayout.value);
    NewStatement_btnView.disabled = false;
    NewStatement_btnGenerate.disabled = false;
}

function GetClientBudgetPeriodObject(id) {
    if (NewStatement_ClientBudgetPeriods != null) {
        for (var j = 0; j < NewStatement_ClientBudgetPeriods.length; j++) {
            if (NewStatement_ClientBudgetPeriods[j].ID == id) {
                return NewStatement_ClientBudgetPeriods[j];
            }
        }
    }
}

function RefreshParentAndClose(personalBudgetStatementID) {
    if (window.opener && !window.opener.closed) {
        var url = window.opener.location.href;

        url = RemoveQSParam(url, "personalbudgetstatementid")
        url = AddQSParam(url, "personalbudgetstatementid", personalBudgetStatementID)

        GetParentWindow().location.href = url;

        window.opener.location.href = url;
    }
    
    window.parent.close();
}

function btnGenerate_Click() {
    ShowProcessingModalDIV();
}

addEvent(window, "load", Init);