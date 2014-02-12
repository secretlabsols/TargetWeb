var webSvcProxy, webSvcResponse, dsoDetail;

function dsoDetail_PopulateControl(domServiceOrderID, extranet) {
    var tipPosition;
    if (domServiceOrderID === 0) {
        return false;
    }

    if (extranet === true) {
        tipPosition = -25;
    } else {
        tipPosition = 0;
    }
    webSvcProxy = Target.Abacus.Library.ServiceOrder.ServiceOrderService;

    webSvcResponse = webSvcProxy.GetDSOAdditionalDetails(domServiceOrderID)
    if (!CheckAjaxResponse(webSvcResponse, webSvcProxy.url)) {
        return false;
    }

    dsoDetail = webSvcResponse.value.Item;

    $("[id$='dsoOrderRef_lblReadOnlyContent']").text(dsoDetail.OrderReference)
    $("[id$='dsoReference_lblReadOnlyContent']").text(dsoDetail.ClientReference)
    $("[id$='dsoName_lblReadOnlyContent']").text(dsoDetail.ClientName)
    $("[id$='dsoDateOfBirth_lblReadOnlyContent']").text(Date.strftime("%d/%m/%Y", dsoDetail.ClientDateOfBirth))
    $("[id$='dsoDateOfDeath_lblReadOnlyContent']").text(Date.strftime("%d/%m/%Y", dsoDetail.ClientDateOfDeath))

    var tooltipItems = [];
    var tooltipInvoker = $("[id$='fdlDSODetails']");
    tooltipItems.push({ name: 'Provider', value: dsoDetail.ProviderReference + ': ' + dsoDetail.ProviderName });
    tooltipItems.push({ name: 'Contract', value: dsoDetail.ContractNumber + ': ' + dsoDetail.ContractTitle });
    tooltipItems.push({ name: 'Date From', value: Date.strftime("%d/%m/%Y", dsoDetail.DateFrom) });
    tooltipItems.push({ name: 'Date To', value:  dsoDetail.DateTo });
    tooltipItems.push({ name: 'End Reason', value: dsoDetail.EndReason });
    tooltipInvoker.each(function() {
        $(this).qtip({
            content: {
                title: 'Service Order Information',
                text: function(api) {
                    return getToolTipNameValueContent({ items: tooltipItems });
                }
            },
            position: {
                my: 'top right',
                at: 'bottom right',
                adjust: {
			            y: tipPosition
		        }
            }
        });
    });
}