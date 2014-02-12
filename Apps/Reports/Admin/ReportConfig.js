
var ssrsBaseUrl, ssrsBasePath, txtReportPathID, lnkTestID;
var txtReportPath, lnkTest;

function Init() {
    txtReportPath = GetElement(txtReportPathID + "_txtTextBox");
    lnkTest = GetElement(lnkTestID);
    txtReportPath_Changed(txtReportPathID);
}

function txtReportPath_Changed(id) {
    lnkTest.href = ssrsBaseUrl + "?" + ssrsBasePath + txtReportPath.value + "&rc:Command=render";
}

addEvent(window, "load", Init);
