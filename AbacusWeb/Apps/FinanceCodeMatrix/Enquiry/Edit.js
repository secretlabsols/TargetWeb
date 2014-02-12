var Edit_dteEffectiveDateFromID, Edit_dteEffectiveDateToID, Edit_txtClientSubGroupID, Edit_txtClientGroupID;
var Edit_txtAgeRangeFromID, Edit_txtAgeRangeToID, Edit_txtTeamID, Edit_txtProviderID, Edit_txtServiceTypeID;
var Edit_txtContractID, Edit_txtServiceOrderRefID, InPlaceContractSelectorVisible;
var fcmSvc;
var fcLength;

function Init() {

    fcmSvc = Target.Abacus.Library.FinanceCodes.Services.FinanceCodeMatrixService;
    fcSvc = Target.Abacus.Library.FinanceCodes.Services.FinanceCodeService;

    fcSvc.GetMaxFinanceCodeLength( function (response) {
        if (!CheckAjaxResponse(response, fcSvc.url, true)) {
            return false;
        }
        fcLength = response.value.Item;
    });


}

function dteEffectiveDateFrom_Changed() {
    var dteEffectiveDateFrom = GetElement(Edit_dteEffectiveDateFromID + "_txtTextBox");

    if (dteEffectiveDateFrom.value.length > 0 && IsDate(dteEffectiveDateFrom.value)) {

        setTxtWeekEndingToMinDate();
    }
}
function setTxtWeekEndingToMinDate() {
    var dteEffectiveDateFrom = GetElement(Edit_dteEffectiveDateFromID + "_txtTextBox");
    var dteEffectiveDateTo = GetElement(Edit_dteEffectiveDateToID + "_txtTextBox");
    if (dteEffectiveDateFrom.value.length > 0 && IsDate(dteEffectiveDateFrom.value)) {
        $(dteEffectiveDateTo).datepicker("option", "minDate", dteEffectiveDateFrom.value.toDate());
    }
}

function btnSave_Click() {
    var matrixID=0;
    var webSvcResponse;
    if (GetQSParam(document.location.search, "id")) {
        matrixID = GetQSParam(document.location.search, "id");
    }

        DisplayLoading(true);
        webSvcResponse = fcmSvc.GetNumberOfServiceOrdersAssociatedToFinanceCodeMatrix(matrixID);
        DisplayLoading(false);
        if (!CheckAjaxResponse(webSvcResponse, fcmSvc.url)) {
            return false;
        }

        if (webSvcResponse.value.Item > 0) {
            if (Page_ClientValidate('Save')) {
                //return true;
                return window.confirm('Confirm you wish to recode ' + webSvcResponse.value.Item.toString() + ' service orders previously allocated finance code from this entry.'); 
            }
        }
    }

    function InPlaceServiceTypeSelector_GetQueryObject(parentControlID) {
        var query = new InPlaceServiceTypeSelector_Query();
        query.Redundant = false;
        query.IncludeIds = [InPlaceServiceTypeSelector_GetSelectedID(parentControlID)];
        return query;
    }

    function InPlaceEstablishment_Changed(newID) {
        if (InPlaceContractSelectorVisible) {
            InPlaceDomContractSelector_ClearStoredID(Edit_txtContractID);
            InPlaceDomContractSelector_providerID = newID;
        }
    }


    function btnNewFinanceCode_Click() {

       
        overrideUnitCostDialog = Ext.create('TargetExt.FinanceCodes.CreateNewFinanceCode', {
            financeCodeMaxLength: fcLength,
            listeners: {
                onChanged: {
                    fn: function (caller, newFC) {
                        $("[id*='txtFinanceCode_hidID']").val(newFC.ID)
                        $("[id*='txtFinanceCode_txtName']").val(newFC.Code)

                    },
                    //scope: me
                }
            }
        });

        overrideUnitCostDialog.show();

    }