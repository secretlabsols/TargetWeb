var selectedID = 0, earliestRevisionID = 0, selectedRevisionID = 0;
var fundingSvc;
var fcTable_ServiceType='', fcTable_DefaultServiceType='';

$(document).ready(function () {
    initialiseControl();
});

function BindEvents() {
    $(document).ready(function() {
        initialiseControl();
    });
}

function initialiseControl() {
    //Initialise web service
    fundingSvc = new Target.Abacus.Web.Apps.WebSvc.DomServiceOrderFunding_class();
    
    //Set up the Mouse events on the svc type menu
    $("[id*='div_']").hide();
    $('.svcTypeItem').each(function looping(index) {
        $(this).mouseover(function onItemOver() {
            $(this).css("cursor", "pointer");
        });

        $(this).click(function onItemClick() {
            checkSelection(index);
        });
    });

    //Set up the bindings for the apportion radio button
    $('[id*=optApportion]').live('change', function () {
        enableBalancingCalloffColumns(true);
        //Reselect item in the svc Type menu
        checkSelection(selectedID);
    });
    //Set up the bindings for the calloff radio button
    $('[id*=optCallOff]').live('change', function () {
        enableBalancingCalloffColumns(false);
        //Reselect item in the svc Type menu
        checkSelection(selectedID);
    });
    //Auto select the correct menu in the svc type menu
    checkSelection(selectedID);
    
    //show and hide the correct Image on the list, to signify which svc type is the default
    $('[id*=imgDefault]').removeClass('scvTypeDefault');
    if (fcTable_DefaultServiceType != "") {
        $('[id*=imgDefault_' + fcTable_DefaultServiceType.replace(/ /g, '').replace('(', '').replace(')', '') + ']').addClass('scvTypeDefault');
    }

    //on startup/postback enable/disable grid controls based on the apportion/call off option buttons
    if ($('[id*=optApportion]').is(':checked')) {
        enableBalancingCalloffColumns(true);
    }
    if ($('[id*=optCallOff]').is(':checked')) {
        enableBalancingCalloffColumns(false);
    }

}


function AddFundingRow() {
    FlagControlsToBeRecreated();
}

function FlagControlsToBeRecreated() {
    var response;
    response = fundingSvc.UpdateSessionSoControlsGetRecreated();
    if (!CheckAjaxResponse(response, fundingSvc.url)) {
        return false;
    }
}

function SetSvcTypeAsDefault() {
    var response;
    fcTable_DefaultServiceType = fcTable_ServiceType;

    //loop through the Funding records setting the correct records to default.
    response = fundingSvc.SetSvcTypeAsDefault(fcTable_DefaultServiceType);
    if (!CheckAjaxResponse(response, fundingSvc.url)) {
        return false;
    }

    //show and hide the correct Image on the list
    $('[id*=imgDefault]').removeClass('scvTypeDefault');
    $('[id*=imgDefault_' + fcTable_DefaultServiceType.replace(/ /g, '') + ']').replace('(', '').replace(')', '').addClass('scvTypeDefault');

    //Need to instruct the screen to recreate the controls when posting back.
    //FlagControlsToBeRecreated();
    //Reselect the service Type as this gets lost on postback
    reselectItem();
}

function reselectItem() {
    //because of post back when adding a new row, re-select the correct service type.
    checkSelection(selectedID);
    FlagControlsToBeRecreated();
};


function checkSelection(id) {
    selectedID = id;
    $("[id*='div_']").hide();
    $(".svcTypeItem").each(function looping(index) {

        if (index == id) {
            fcTable_ServiceType = this.innerText;
            $(this).animate({ backgroundColor: "#FFFF99", color: "#6487DB" }, 500);
            $("[id*='div_" + this.innerText.replace(/ /g, '').replace('(', '').replace(')', '') + "']").show();
        }
        else {
            $(this).animate({ backgroundColor: "#6487DB", color: "#ffffff" }, 500);
            $("[id*='div_" + this.innerText.replace(/ /g, '').replace('(', '').replace(')', '') + "']").hide();
        }
    });
}

function enableBalancingCalloffColumns(apportion) {

    if (apportion == false) {
        $('[id*=gvFinanceCodes] input[type=radio]').attr('checked', false)
    } else {
    $('[id*=ddlCallOff]').val(" ");
    }

    $('[id*=gvFinanceCodes] input[type=radio]').attr('disabled', !apportion);
    $('[id*=ddlCallOff]').attr('disabled', apportion);
    $("[id*=hidFinCodeApportionEqually]").val(apportion);
}

function confirmDelete() {
    if (earliestRevisionID == $('[id*=dsoFunding_cbofundingEffectiveDate_cboDropDownList]').val()) {
        if (confirm('Are you sure you want to delete all Service order funding records for this order?') == true) {
            return true;
        } else {
            return false;
        }
    } else {
        return true;
    }
}




//*****************************************************************************************************************************************************


function ShowProportions() {
    var $response;
    var $initSettings = {
        svcType: fcTable_ServiceType, 
        dialogueDivID: 'dsoProportions_dialog',
        fundingSvc: fundingSvc
    }

    response = fundingSvc.UpdateSessionSoControlsGetRecreated();
    if (!CheckAjaxResponse(response, fundingSvc.url)) {
        return false;
    }

    //Show dialogue
    $(document).proportionsDialog($initSettings);
    $(document).proportionsDialog('show', $initSettings);
    FlagControlsToBeRecreated();
}

(function ($) {

    var $dlgDiv = null;
    var $svcType = null;
    var $dlgDivLoading = null;
    var $settings = {
        svcType: '',
        dialogueDivID: null,
        fundingSvc: null
    };
    var $classUiDisabled = 'ui-state-disabled';

    $.fn.proportionsDialog = function (method, options) {
        return this.each(function () {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.proportionsDialog');
            }
        });
    };

    function displayLoadingDiv(display, message) {
        $dlgDiv.dialog('displayLoading', { Text: message, Display: display });
    }

    function okClicked(src, svcType) {
        
        $("[id*=para]").each(function() {
            
            var $rowIdentifier = this.id.replace("slider", "").replace("para", "");
            var $percentFromDialog = $(this).text()
            var $response;

            $("[id*=" + svcType.replace(/ /g, '').replace('(', '').replace(')', '') + "] [id*=fundingRowIdentifier]").each(function() {
                //alert(this.id);

                if ($(this).text() == $rowIdentifier) {
                   $("#" + this.nextSibling.nextSibling.id).text($percentFromDialog)
                   $response = $settings.fundingSvc.UpdateRowWithProportion(svcType, $rowIdentifier, $percentFromDialog.replace("%", ""))
                   if (!CheckAjaxResponse($response, $settings.fundingSvc.url)) {
                        return false;
                   };
                }
            });

           

        });
        
        
        $dlgDiv.dialog('close');
        $dlgDiv = null
    }

    function cancelClicked(src) {
        $dlgDiv.dialog('close');
        $dlgDiv = null
    }


    var methods = {
        init: function (options) {

            if (!$dlgDiv) {
                if (options) {
                    $.extend($settings, options);
                }
                $dlgDiv = $('#' + $settings.dialogueDivID) 
                $dlgDiv.dialog({
                    autoOpen: false,
                    draggable: true,
                    minWidth: 650,
                    modal: true,
                    resizable: false,
                    closeOnEscape: true,
                    zIndex: 9999999,
                    title: 'Amend Proportions',
                    buttons: [
                        {
                            id: "button_ok",
                            text: "Ok",
                            click: function () {
                                okClicked($(this), options.svcType);
                            }
                        },
                        {
                            id: "button_cancel",
                            text: "Cancel",
                            click: function () {
                                cancelClicked($(this));
                            }
                        }
                    ]
                });

            }

            $('#chkSplitEqually').attr('checked', false);

            $('#chkSplitEqually').mousedown(function() {
                if (!$(this).is(':checked')) {
                    var $splitValue = 0;
                    var $noRows = $("[id*=slider_]").length;
                    if ($noRows > 0) {
                        $splitValue = 100/$noRows;
                        $splitValue = Math.round($splitValue*100)/100
                    } 
                    $("[id*=slider_]").slider('value', $splitValue);
                    $("[id*=slider_]").slider('disable', true);
                    $("[id*=para]").text($splitValue + '%');
                    $("[id*=para]").hide();
                    $("[id*=TotalPercent]").text('100%');
                    $("[id*=TotalPercent]").css("color", "black");
                    $("[id*=button_ok]").button("enable");
                } else {
                    //reset sliders to zero
                    $("[id*=slider_]").slider('value', 0);
                    $("[id*=slider_]").slider('enable', true);
                    $("[id*=para]").text('0%');
                    $("[id*=para]").show();
                    $("[id*=TotalPercent]").text('0%');
                    $("[id*=TotalPercent]").css("color", "red");
                    $("[id*=button_ok]").button("disable");
                }
            });
            
        },
        show: function (options) {
            var $orderHeader = null;
            var $orderDetail = null;
            var $orderSuspensions = null;
            var $orderCosts = null;
            var $response = null;
            var $tabs = null;

            if ($dlgDiv) {
                displayLoadingDiv(false, '');


                $response = $settings.fundingSvc.GetItemsForSvcType(options.svcType)
                if (!CheckAjaxResponse($response, $settings.fundingSvc.url)) {
                    return false;
                };
                
                var $items = $response.value.Items;

                $('#tblProportionsBody').empty();

                // Compile the markup as a named template
                $.template( "proportionTemplate", $("[id*=financeCodePercentagesTemplate]") );

                // Render the template with the data and insert
                // the rendered HTML into the table
                $.tmpl( "proportionTemplate", $items)
                    .appendTo("#tblProportionsBody");

                $("[id*=slider_]").slider({
                    range: 'min',
                    min: 1,
                    max: 100,  
                    slide: function( event, ui ) {
                        $('[id*=' + this.id.replace('_', '') + 'para]').text(ui.value + '%');
                        var sum = 0;
                        $("[id*=slider_]").each(function() {
                            sum += Number($('[id*=' + this.id.replace('_', '') + 'para]').text().replace('%', ''));
                            $("[id*=TotalPercent]").text(sum  + '%');
                            if (sum != 100) {
                                $("#button_ok").button("disable");
                                 $("[id*=TotalPercent]").css("color", "red");
                            } else {
                                $("#button_ok").button("enable");
                                $("[id*=TotalPercent]").css("color", "black");
                            }
                        });
                    },
                    create: function( event, ui ) {
                        $(this).slider('value', Number($('[id*=' + this.id.replace('_', '') + 'para]').text().replace('%', '')));
                    }
                });

                $dlgDiv.dialog('open');
            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }
        }
    };

})(jQuery);