var RemittanceSelector_creditorPaymentsSvc, RemittanceSelector_tblRemittances, RemittanceSelector_divPagingLinks, RemittanceSelector_btnView;
var RemittanceSelector_currentPage, RemittanceSelector_selectedBatchID, RemittanceSelector_btnViewID;
var RemittanceSelector_listFilter, RemittanceSelector_listFilterCreditorRef = "", RemittanceSelector_listFilterCreditorName = "";

function Init() {

    RemittanceSelector_creditorPaymentsSvc = new Target.Abacus.Web.Apps.WebSvc.CreditorPayments_class();
    RemittanceSelector_tblRemittances = GetElement("RemittanceSelector_tblRemittances");
    RemittanceSelector_divPagingLinks = GetElement("Remittance_PagingLinks");
    RemittanceSelector_btnView = GetElement(RemittanceSelector_btnViewID + '_btnReports', true);

    if (RemittanceSelector_btnView) RemittanceSelector_btnView.disabled = true;

    // setup list filters
    RemittanceSelector_listFilter = new Target.Web.ListFilter(RemittanceSelector_listFilter_Callback);
    RemittanceSelector_listFilter.AddColumn("Creditor Ref", GetElement("RemittanceSelector_thCreditorRef"));
    RemittanceSelector_listFilter.AddColumn("Creditor Name", GetElement("RemittanceSelector_thCreditorName"));

    // populate table

    RemittanceSelector_FetchRemittanceList(RemittanceSelector_currentPage, RemittanceSelector_selectedBatchID);
}

function RemittanceSelector_FetchRemittanceList(page, selectedBatchID) {
    RemittanceSelector_currentPage = page;
    DisplayLoading(true);
    if (!selectedBatchID || selectedBatchID <= 0) {
        selectedBatchID = RemittanceSelector_selectedBatchID;
    }
    RemittanceSelector_creditorPaymentsSvc.FetchPagedRemittanceList(page, 10, RemittanceSelector_listFilterCreditorName, RemittanceSelector_listFilterCreditorRef, selectedBatchID, RemittanceSelector_FetchRemittanceList_Callback)
}

function RemittanceSelector_listFilter_Callback(column) {
    switch (column.Name) {
        case "Creditor Ref":
            RemittanceSelector_listFilterCreditorRef = column.Filter;
            break;
        case "Creditor Name":
            RemittanceSelector_listFilterCreditorName = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    RemittanceSelector_FetchRemittanceList(1);
}


function RemittanceSelector_FetchRemittanceList_Callback(response) {
    var index, currentRemittance;
    var tr, td, radioButton;
    var str;
    var link;

    RemittanceSelector_Remittances = null;

//    if (RemittanceSelector_selectedRemittanceID == 0) {
//        if (RemittanceSelector_btnView) RemittanceSelector_btnView.disabled = true;
//    }

    if (CheckAjaxResponse(response, RemittanceSelector_creditorPaymentsSvc.url)) {
        // populate the table

        RemittanceSelector_Remittances = response.value.Items;

        // remove existing rows
        ClearTable(RemittanceSelector_tblRemittances);

        for (index = 0; index < RemittanceSelector_Remittances.length; index++) {

            currentRemittance = RemittanceSelector_Remittances[index];

            tr = AddRow(RemittanceSelector_tblRemittances);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "RemittanceSelect", currentRemittance.ID, Remittance_RadioButton_Click);

            AddInput(td, "hidFlag", "hidden", "", "", currentRemittance.GenericCreditorID);
            AddInput(td, "hidFlag", "hidden", "", "", currentRemittance.Type);
            
            if (currentRemittance.CreditorRef.length == 0) {
                td = AddCell(tr, " ");
            } else {
                td = AddCell(tr, currentRemittance.CreditorRef);
            }
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, currentRemittance.CreditorName);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            AddCell(tr, Date.strftime("%d/%m/%Y", currentRemittance.DateFrom));

            AddCell(tr, Date.strftime("%d/%m/%Y", currentRemittance.DateTo));

            td = AddCell(tr);
            td.innerHTML = String(currentRemittance.Total).formatCurrency();

            td = AddCell(tr, currentRemittance.TypeDescription);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

        }

        // load the paging link HTML
        RemittanceSelector_divPagingLinks.innerHTML = response.value.PagingLinks;
    }

    DisplayLoading(false);
}
function Remittance_RadioButton_Click() {
    var index, rdo, hGenericCreditorID, remittanceType;

    for (index = 0; index < RemittanceSelector_tblRemittances.tBodies[0].rows.length; index++) {
        rdo = RemittanceSelector_tblRemittances.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            RemittanceSelector_tblRemittances.tBodies[0].rows[index].className = "highlightedRow"
            //RemittanceSelector_tblRemittances = rdo.value;
            
            hGenericCreditorID = RemittanceSelector_tblRemittances.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[1].value;
            remittanceType = RemittanceSelector_tblRemittances.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[2].value;

            if (RemittanceSelector_btnView) {
                if (remittanceType == 1) {
                    RemittanceSelector_btnView.disabled = false;
                } else {
                    RemittanceSelector_btnView.disabled = true;
                }
            }  

            hGenericCreditorID = RemittanceSelector_tblRemittances.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[1].value;
            ReportsButton_AddParam(RemittanceSelector_btnView.id, "InterfaceLogID", RemittanceSelector_selectedBatchID);
            ReportsButton_AddParam(RemittanceSelector_btnView.id, "GenericCreditorID", hGenericCreditorID);
            
        } else {
        RemittanceSelector_tblRemittances.tBodies[0].rows[index].className = ""
        }
    }
}


addEvent(window, "load", Init);