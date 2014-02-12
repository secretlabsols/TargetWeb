
var nextStatementStartFromDate = null;
var statementFrequencies = null;
var cboStatementFrequencyID = null;
var spnStatementDue;
var cboStatementFrequency;

function Init() {

    spnStatementDue = jQuery("#spnNextStatementDue");
    cboStatementFrequency = jQuery("#" + cboStatementFrequencyID);

    cboStatementFrequency.bind("change", SetNextStatementDue);

    PanelClick("Details", true);
    SetNextStatementDue();
}

function PanelClick(panelName, expand) {

    var height, width;

    parent.resizeIframe(document.body.scrollHeight, 'ifrAdministration');

    // sync fieldset heights
    if (expand) {
        switch (panelName) {
            case "Details":
                height = Math.max(jQuery("#fsDetailsPersonal").height(), jQuery("#fsDetailsGeneral").height());
                jQuery("#fsDetailsPersonal").height(height);
                jQuery("#fsDetailsGeneral").height(height);
                break;

            case "Administrative Details":
                height = Math.max(jQuery("#fsAdminDetailPersonalBudgetStatements").height(), jQuery("#fsBankAccountDetails").height());
                jQuery("#fsAdminDetailPersonalBudgetStatements").height(height);
                jQuery("#fsBankAccountDetails").height(height);
                break;

            case "Legacy":
                height = Math.max(jQuery("#fsAdminResBilling").height(), jQuery("#fsAdminNonResBilling").height());
                jQuery("#fsAdminResBilling").height(height);
                jQuery("#fsAdminNonResBilling").height(height);

                // get width in px
                width = jQuery("#fsAdminResBilling").width() + jQuery("#fsAdminNonResBilling").outerWidth(true);
                // get width as a % so that it resizes when the window resizes
                width = (100 * width) / parseFloat(jQuery("#fsResNonResStatements").parent().width()) + "%";
                jQuery("#fsResNonResStatements").width(width);
                
                break;

            case "Other Information":
                break;
        }
    }
}

function SetNextStatementDue() {

    if (nextStatementStartFromDate != null) {

        var selectedFrequencyID = cboStatementFrequency.val();

        // locate the frequency from the lookup list
        var selectedFrequency = jQuery.grep(statementFrequencies, function(freq) {
            return freq.ID == selectedFrequencyID;
        });
        if (selectedFrequency.length == 1) {
            // parse out the InfoString which is in the form A-BBB
            // A is the frequency interval: D (day) or M (month)
            // BBB is the number of intervals
            // e.g. D-14 means 14 days
            var infoString = selectedFrequency[0].InfoString;
            var interval = infoString.substring(0, 1).toLowerCase();
            var number = parseInt(infoString.substring(2));
            
            // determine new date
            var nextDueDate = Date.add(interval, number, nextStatementStartFromDate);

            spnStatementDue.text("(Next due: " + nextDueDate.strftime("%d/%m/%Y") + ")");
        }
    }
}

addEvent(window, "load", Init);
