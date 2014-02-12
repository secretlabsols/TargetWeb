var selectedServiceOrderID, tblSuspensions, ServiceOrder_btnViewID, ServiceOrder_btnSuspendID, ServiceOrder_mode;
var btnView, btnSuspend;

function Init() {

	tblSuspensions = GetElement("tblSuspensions", true);
	btnView = GetElement(ServiceOrder_btnViewID, true);
	btnSuspend = GetElement(ServiceOrder_btnSuspendID, true);
	
	if(btnView) btnView.disabled = true;
	if(btnSuspend) btnSuspend.style.display = "none";		
}

function RadioButton_Click() {
	var index, rdo, selectedRow, dataPartitioned;

	for (index = 0; index < tblSuspensions.tBodies[0].rows.length; index++){
		rdo = tblSuspensions.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblSuspensions.tBodies[0].rows[index];
			tblSuspensions.tBodies[0].rows[index].className = "highlightedRow"
			selectedServiceOrderID = parseInt(GetInnerText(selectedRow.cells[1]).trim());
			dataPartitioned = $('tr td', '#tblSuspensions').eq(4).find(':input').val();
			if (btnView && dataPartitioned == "No") {
			    btnView.disabled = false;
			}  
			if(ServiceOrder_mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
			    if(btnSuspend) btnSuspend.style.display = "block"
			    if (GetInnerText(selectedRow.cells[7]) == "Yes "){
                    if(btnSuspend) btnSuspend.value = "Re-instate";
                } else {
                    if(btnSuspend) btnSuspend.value = "Suspend";
                }
            }
		} else {
			tblSuspensions.tBodies[0].rows[index].className = "";
		}
	}
}

function btnView_Click() {
	ViewOrder(selectedServiceOrderID);
}

function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}

function ViewOrder(orderID) {
    //document.location.href = "Edit.aspx?id=" + orderID + "&mode=1&backUrl=" + GetBackUrl();
    document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/svcOrders/Edit.aspx?id=" + orderID + "&mode=1&backUrl=" + GetBackUrl();
}

addEvent(window, "load", Init);