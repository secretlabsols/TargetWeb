var cboExpAccountTypeID, expenditureAccountSelectorID, fundingDetail_serviceType, chkServiceUserFundedID, fundDetail_ExpCode, fundDetail_IncCode;
var fundingDetail_mode;

function setup_expenditureSelector(_accountType, _serviceType, _mode){
    var chkUserFunded;
    
    InPlaceExpenditureAccountSelector_accountType = _accountType;
    InPlaceExpenditureAccountSelector_serviceType = _serviceType;
    
    chkUserFunded = GetElement(chkServiceUserFundedID + "_chkCheckbox");
    if(_mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew || _mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
        switch(_accountType)
        {
        case 1:
          chkUserFunded.checked = true;
          chkUserFunded.disabled = true;
          break;    
        case 5:
          chkUserFunded.disabled = false;
          break;
        default:
          chkUserFunded.checked = false;
          chkUserFunded.disabled = true;
        }
    }
}

function cboExpaccountType_Click() 
{
    var cboExpAcc, chkUserFunded, expenditureSelector, divExpenditure;
    
    cboExpAcc = GetElement(cboExpAccountTypeID + "_cboDropDownList");
    chkUserFunded = GetElement(chkServiceUserFundedID + "_chkCheckbox");
    
    if (cboExpAcc.value == ""){
        InPlaceExpenditureAccountSelector_Enabled(expenditureAccountSelectorID, false);
    }else{
        InPlaceExpenditureAccountSelector_ClearStoredID(expenditureAccountSelectorID);
        InPlaceExpenditureAccountSelector_Enabled(expenditureAccountSelectorID, true);
    }
    InPlaceExpenditureAccountSelector_serviceType = fundingDetail_serviceType;
    InPlaceExpenditureAccountSelector_accountType = cboExpAcc.value;
    switch(cboExpAcc.value)
    {
    case "1":
      chkUserFunded.checked = true;
      chkUserFunded.disabled = true;
      break;    
    case "5":
      chkUserFunded.disabled = false;
      break;
    default:
      chkUserFunded.checked = false;
      chkUserFunded.disabled = true;
    }
}

function InPlaceExpenditureAccountSelector_Changed(expCode, incCode) {
    var txtExp, txtInc;
    
    txtExp = GetElement(fundDetail_ExpCode + "_lblReadOnlyContent");
    txtInc = GetElement(fundDetail_IncCode + "_lblReadOnlyContent");
    txtExp.innerText = expCode;
    txtInc.innerText = incCode;

}