<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PaymentScheduleSelector.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.UserControls.PaymentScheduleSelector" %>


<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls"
    TagPrefix="cc1" %>
<table class="listTable" id="tblPSchedules" cellpadding="2" cellspacing="0" width="100%"
    summary="List of available proforma invoices.">
    <caption>
        List of available payment schedules.</caption>
    <thead>
        <tr>
            <th style="width: 1.5em;">
            </th>
            <th>
                Reference
            </th>
            <th>
                Provider
            </th>
            <th>
                Contract
            </th>
            <th id="thSURef">Date From</th>
            <th id="thSUName">Date To</th>
            <th>
                Visit-based
            </th>
           
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
            
        </tr>
    </tbody>
</table>
<div id="Invoice_PagingLinks" style="float: left;">
</div>
<div style="float: right; margin-bottom:4px;">
    <input type="button" style="width: 7.3em;" id="btnNew" value="New" title="New payment schedule"
    onclick="btnNew_Click();" />
    <input  type="button" style="width: 3.9em; " id="btnView" value="View"
        title="View a payment schedule" onclick="btnView_Click();" />
    <br />
</div>
<div class="clearer">
</div>