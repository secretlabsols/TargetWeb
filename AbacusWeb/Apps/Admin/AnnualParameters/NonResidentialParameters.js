var weeklyIncomeTextBoxID, weeklyIncomeText, perPartTextBoxID, perPartTextBox, interestRateTextBoxID, interestRateTextBox;
var minValueDecimalString = "0.00"

function Init() {
    weeklyIncomeText = GetElement(weeklyIncomeTextBoxID);
    perPartTextBox = GetElement(perPartTextBoxID);
    interestRateTextBox = GetElement(interestRateTextBoxID);
}

function txtCapAllowancesCalculationWeeklyIncome_Changed() {
    interestRateTextBox.value = minValueDecimalString;
}

function txtCapAllowancesCalculationPerPart_Changed() {
    interestRateTextBox.value = minValueDecimalString;
}

function txtCapAllowancesCalculationInterestRate_Changed() {
    weeklyIncomeText.value = minValueDecimalString;
    perPartTextBox.value = minValueDecimalString;
}
addEvent(window, "load", Init);