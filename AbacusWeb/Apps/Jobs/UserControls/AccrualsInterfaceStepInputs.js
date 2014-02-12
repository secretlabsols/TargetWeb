var lookupSvc;
var dteAccrualDate, dteAccrualDateID;
var cboFinancialYear, cboFinancialYearID;
var cboPeriodNum, cboPeriodNumID;
var txtFilePath, txtFilePathID;
var defAccrualDate, defFinancialYear, defPeriodNum;

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();

	dteAccrualDate = GetElement(dteAccrualDateID + "_txtTextBox");
	cboFinancialYear = GetElement(cboFinancialYearID + "_cboDropDownList");
	cboPeriodNum = GetElement(cboPeriodNumID + "_cboDropDownList");
	txtFilePath = GetElement(txtFilePathID + "_txtTextBox");

	SetDefaultFieldValues();
	FetchBudgetPeriodList(1);
}

function cboFinancialYear_OnChange(id) {
    FetchBudgetPeriodList(1);
}

function FetchBudgetPeriodList(page) {
	if (cboFinancialYear.value != "-1") {
	    DisplayLoading(true);

	    lookupSvc.FetchBudgetPeriodList(page, 9999, 0, cboFinancialYear.value,
	    "", 0, BudgetPeriodList_Callback);
	}
}

function BudgetPeriodList_Callback(response) {
    var budPeriods, index;
    var str;

	if (CheckAjaxResponse(response, lookupSvc.url)) {
	    budPeriods = response.value.BudgetPeriods;

	    // Clear the content of the Period Number combo..
	    cboPeriodNum.options.length = 0;

	    if (budPeriods.length == 0) 
		{
            // Do nothing..
		}
		else
		{
		    for (index = 0; index < budPeriods.length; index++) {
		        str = budPeriods[index].PeriodNumber + "  (" + budPeriods[index].DateFrom + " to " + budPeriods[index].DateTo + ")";
		        AddComboItem(cboPeriodNum, str, budPeriods[index].ID);

                // If a Period Number was previously used, select it if a match is found..
		        if (budPeriods[index].ID == defPeriodNum) {
		            cboPeriodNum.value = defPeriodNum;
		        }
		    }
		}
	}
	DisplayLoading(false);
}

function SetDefaultFieldValues() {
    dteAccrualDate.value = defAccrualDate;
    cboFinancialYear.value = defFinancialYear;
    cboPeriodNum.value = defPeriodNum;
}

function AddComboItem(cbo, itemText, itemVal) {
    var opt = document.createElement("OPTION");
    cbo.options.add(opt);
    SetInnerText(opt, itemText);
    opt.value = itemVal;
}

addEvent(window, "load", Init);
