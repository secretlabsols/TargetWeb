
var chkAllowUseServiceRegisters, chkUnitsDisplayedAsHoursMins, txtMinutesPerUnit, valMinutesPerUnit, uom_Mode, uom_inUseByDSO;

function Init() {
    chkAllowUseServiceRegisters = GetElement("chkAllowUseServiceRegisters_chkCheckbox");
    chkUnitsDisplayedAsHoursMins = GetElement("chkUnitsDisplayedAsHoursMins_chkCheckbox");
    txtMinutesPerUnit = GetElement("txtMinutesPerUnit_txtTextBox");
    valMinutesPerUnit = GetElement("txtMinutesPerUnit_valRequired");
    
    if(uom_Mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew || uom_Mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
        chkAllowUseServiceRegisters_Click();
        chkUnitsDisplayedAsHoursMins_Click();
    }
}

function chkAllowUseServiceRegisters_Click() {
    if (!uom_inUseByDSO) {
        if (chkAllowUseServiceRegisters.checked == false) {
            chkUnitsDisplayedAsHoursMins.disabled = false;
        } else {
            chkUnitsDisplayedAsHoursMins.checked = false;
            chkUnitsDisplayedAsHoursMins.disabled = true;
        }
        chkUnitsDisplayedAsHoursMins_Click();
    }
}

function chkUnitsDisplayedAsHoursMins_Click() {
    if(chkAllowUseServiceRegisters.checked) {
        txtMinutesPerUnit.value = "";
        txtMinutesPerUnit.disabled = true;
    } else if (chkUnitsDisplayedAsHoursMins.checked) {
        txtMinutesPerUnit.value = "60";
        txtMinutesPerUnit.disabled = true;
    } else if (!uom_inUseByDSO) {
        txtMinutesPerUnit.disabled = false;
    }
    ValidatorEnable(valMinutesPerUnit, !txtMinutesPerUnit.disabled);
}

addEvent(window, "load", Init);