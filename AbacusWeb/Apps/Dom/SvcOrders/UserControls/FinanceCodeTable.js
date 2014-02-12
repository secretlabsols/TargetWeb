var webSvcProxy, webSvcResponse;

$(document).ready(function() {
    webSvcProxy = Target.Abacus.Library.ServiceOrder.ServiceOrderService;
});
   
//Handle the change event of the radio buttons within the grid.
$('[id*=gvFinanceCodes] input[type=radio]').live('change', function () {
    //Get the id of the Grid
    var gvFinanceCodes_clientID = this.parentNode.parentNode.parentNode.parentNode.id
    //Deselect all radio buttons within the grid
    $("[id*='" + gvFinanceCodes_clientID + "'] input[type=radio]").attr('checked', false);
    $("[id^='hidoptBalancing']").val('false');
    //Re-Select the radio button that was originally selected
    $("[id^='" + this.id + "']").attr('checked', true);
    $("[id^='" + this.id.replace('optBalancing', 'hidoptBalancing') + "']").val('true');

    //Call a function on the ServiceOrderFunding.js file
    if (typeof (FlagControlsToBeRecreated) == "function") {
        //Controls will need to be recreated if we save
        FlagControlsToBeRecreated();
    }
});

$("[id*='ddlCallOff']").live("change", function (e) {
    //Call a function on the ServiceOrderFunding.js file
    if (typeof (FlagControlsToBeRecreated) == "function") {
        //Controls will need to be recreated if we save
        FlagControlsToBeRecreated();
    }
});

$("[id*='ddlFundedBy']").live("change", function (e) {
    //Call a function on the ServiceOrderFunding.js file
    if (typeof (FlagControlsToBeRecreated) == "function") {
        //Controls will need to be recreated if we save
        FlagControlsToBeRecreated();
    }
});
    

function FinanceCodeTable_SetFundedBy(expAccID, expAccType, txtExpAccount) {

    $("[id*=hidExpAccType]").val(expAccType);
        
    webSvcResponse = webSvcProxy.FetchServiceOrderFundingFundedBy(expAccID)
    if (!CheckAjaxResponse(webSvcResponse, webSvcProxy.url)) {
        return false;
    }

    var tmpID = txtExpAccount.id.replace('expenditureAccount_txtName', 'ddlFundedBy');
    var ddlList = $(tmpID);
    //remove items
    $("[id*='" + tmpID + "']").empty();
    //Add Items to Drop Down
    $.each(webSvcResponse.value.Items, function(index, item) {
        $("[id*='" + tmpID + "']").append($("<option value=\"" + index + "\">" + item + "</option>"));
    });

}

