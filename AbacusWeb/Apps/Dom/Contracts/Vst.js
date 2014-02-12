
var spnExpectedVisitTimes;
var expMinStartTime_HoursID, expMinStartTime_MinsID, expMaxEndTime_HoursID, expMaxEndTime_MinsID;
var expMinStartTime_Hours, expMinStartTime_Mins, expMaxEndTime_Hours, expMaxEndTime_Mins;

function Init() {
    spnExpectedVisitTimes = GetElement("spnExpectedVisitTimes");
    expMinStartTime_Hours = GetElement(expMinStartTime_HoursID);
    expMinStartTime_Mins = GetElement(expMinStartTime_MinsID);
    expMaxEndTime_Hours = GetElement(expMaxEndTime_HoursID);
    expMaxEndTime_Mins = GetElement(expMaxEndTime_MinsID);
    BuildExpectedVisitTimes();
}

function BuildExpectedVisitTimes() {

    var MIDNIGHT = "00:00";

    var msg = "";
    var minStartHours = parseInt(expMinStartTime_Hours.value, 10);
    var minStartMinutes = parseInt(expMinStartTime_Mins.value, 10);
    var maxEndHours = parseInt(expMaxEndTime_Hours.value, 10);
    var maxEndMinutes = parseInt(expMaxEndTime_Mins.value, 10);
    var minStartTime = expMinStartTime_Hours.value + ":" + expMinStartTime_Mins.value;
    var maxEndTime = expMaxEndTime_Hours.value + ":" + expMaxEndTime_Mins.value;
    
    var crossMidnight = false;
    if((maxEndTime != MIDNIGHT) && ((maxEndHours < minStartHours) || ((maxEndHours == minStartHours) && (maxEndMinutes < minStartMinutes)))) {
        crossMidnight = true;
    }

    if(minStartTime == maxEndTime) {
        msg = "There is no expectation on when visits should occur";
    } else if(crossMidnight) {
        msg = "Visits are expected to occur between " + minStartTime + " and " + maxEndTime + " the next day";
    } else {
        msg = "Visits are expected to occur between " + minStartTime + " and ";
        if(maxEndTime == MIDNIGHT)
            msg += "midnight";
        else
            msg += maxEndTime;
    }

    SetInnerText(spnExpectedVisitTimes, msg);
}

addEvent(window, "load", Init);