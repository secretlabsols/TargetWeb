var rdoCreateNow, rdoCreateLater, divCreateJobLater, hidDisplayMode;

function Init() {
    optCreate_Click();    
}

function optCreate_Click() {

    var mode = hidDisplayMode.value;
    var inputs = divCreateJobLater.getElementsByTagName('span');
    var enableFutureValidation;
    var disableMode = '1';

    if (rdoCreateNow.checked == true) {
        if (mode == disableMode) {
            divCreateJobLater.disabled = true;
        }
        else {
            divCreateJobLater.style.display = 'none';
        }
        enableFutureValidation = false;
    } else if (rdoCreateLater.checked == true) {
        if (mode == disableMode) {
            divCreateJobLater.disabled = false;
        }
        else {
            divCreateJobLater.style.display = 'block';
        }
        enableFutureValidation = true;
    }    
    
    for (var i = 0; i < inputs.length; i++) {

        ValidatorEnable(inputs[i], enableFutureValidation);
    }

}

addEvent(window, "load", Init);