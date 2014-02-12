<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DomProviderInvoiceSelector.ascx.vb"
    Inherits="Target.Abacus.Extranet.Apps.UserControls.DomProviderInvoiceSelector"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls"
    TagPrefix="cc1" %>
<table class="listTable" id="tblInvoices" cellpadding="2" cellspacing="0" width="100%"
    summary="List of available proforma invoices.">
    <caption>
        List of available provider invoices.</caption>
    <% If Not Me.HideColumnContractNumber And Not Me.HideColumnProviderReference Then%>
    <thead>
        <tr>
            <th style="width: 1.5em;">
            </th>
            <th id="thInvNumber">Invoice <br />Number</th>
            <th> Contract No  </th>
            <th> Provider <br />Ref </th>
            <th id="thPaymentRef">Payment <br /> Ref</th>
            <th id="thSUName"><span id="periodicServiceUser">Service User</span></th>
            <% If Not Me.HideColumnSUReference Then%>
                <th id="thSURef"><span id="periodicSUReference">S/U Ref</span></th>
            <% End If%>
            <th> Period <br />From </th>
            <th> Period To </th>
            <th> Net <br />Payment </th>
            <th> Status </th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>            </td>
            <td>            </td>
            <td>            </td>
            <td>            </td>  
            <td>            </td>
            <td>        </td>
             <% If Not Me.HideColumnSUReference Then%>
                <td>            </td>
             <% End If%> 
            <td>            </td>
            <td>            </td>
            <td>            </td>
            <td>            </td>
        </tr>
    </tbody>
    <% Else%>
    <thead>
        <tr>
            <th style="width: 1.5em;">
            </th>
            <th id="thInvNumber">Invoice Number</th>
            <th id="thPaymentRef">Payment Ref</th>
            <th id="thSUName"><span id="periodicSU"> Service User</span></th>
            <th id="thSURef"><span id="periodicSURef"> S/U Reference</span> </th>
            <th> Period From </th>
            <th> Period To </th>
            <th> Net Payment </th>
            <th> Status </th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>            </td>
            <td>            </td>  
            <td>            </td>
            <td>            </td>
            <td>            </td>
            <td>            </td>
            <td>            </td>
            <td>            </td>
            <td>            </td>
        </tr>
    </tbody>
    <% End If%>
</table>
<div id="Invoice_PagingLinks" style="float: left;">
</div>
<div style="float: right;">

    <input type="button" style="width: 7em;" id="btnViewInvoiceLines" value="Invoice Lines"
        title="View the Invoice Lines of the selected Invoice" onclick="btnViewInvoiceLines_Click();" />
    <input type="button" style="width: 7em;" id="btnViewCostedVisits" value="Costed Visits"
        title="View the Costed Visits of the selected Invoice" onclick="btnViewVisits_Click();" />
    <input type="button" style="width: 7em;" id="btnRetract" value="Retract" title="Retract selected invoice"
        onclick="btnRetract_Click();" />

</div>
<%--<div class="clearer">
</div>
--%>