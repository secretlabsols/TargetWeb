<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GenericCreditorPaymentSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.GenericCreditorPaymentSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%> 
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="CareTypeSelector" Src="~/AbacusWeb/Apps/UserControls/CareTypeSelector.ascx" %>

<table class="listTable" id="GCPaymentSelector_tblGCPaymentSelectors" cellpadding="2" cellspacing="0" width="100%" summary="List of service user contribution level.">
<caption>
    <div class="caption">List of creditor payments.</div>
    <div class="mruList">
        <cc1:MruList ID="mru" runat="server" MruListKey="GENERIC_CREDITOR_PAYMENT" BlankItemText="[Recent Creditor Payments]" ClientOnChange="GenericCreditorPaymentSelector_MruOnChange" />
    </div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thCreditorRef" style="vertical-align:bottom;">Creditor<br />Ref</th>
		<th id="thCreditorName" style="vertical-align:bottom;">Creditor<br />Name</th>
		<th id="thContractNumber" style="vertical-align:bottom;">Contract<br />Number</th>
		<%If ShowServiceUserColumn Then%>
		    <th id="thServiceUser" style="vertical-align:bottom;">Service<br />User</th>
		<%End If%>
		<th id="thPaymentNumber" style="vertical-align:bottom;">Payment<br />Number</th>
		<th id="thTotal" style="vertical-align:bottom;">Total</th>
		<th id="thType" style="vertical-align:bottom;">Type</th>
		<th id="thStatus" style="vertical-align:bottom;">Status</th>
		<th id="thStatusDate" style="vertical-align:bottom;">Status<br />Date</th>
		<th id="thExclude" style="vertical-align:bottom;">Exclude</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>
<div id="GCPaymentSelector_PagingLinks" style="float:left;"></div>
<br />
<div id="divButtons" runat="server" style="float:right">

    <input type="button" id="GenericCreditorPaymentSelector_btnExcInc" runat="server" title="Exclude/include the invoice from batching" style="float:left;width:7em;margin-right:0.5em;" value="Exclude" onclick="GenericCreditorPaymentSelector_btnExcInc_Click();" />
    <input type="button" id="GenericCreditorPaymentSelector_btnAuthorise" runat="server" style="float:left;width:7em;margin-right:0.5em;" value="Authorise" onclick="GenericCreditorPaymentSelector_btnAuthorise_Click();" />
    <input type="button" id="GenericCreditorPaymentSelector_btnCreateBatch" runat="server" style="float:left;width:7em;margin-right:4em;" value="Create Batch" onclick="GenericCreditorPaymentSelector_btnCreateBatch_Click();" />

    <input type="button" id="GenericCreditorPaymentSelector_btnNotes" runat="server" style="float:left;width:7em;margin-right:0.5em;" value="Notes" onclick="GenericCreditorPaymentSelector_btnNotes_Click();" />
    <input type="button" id="GenericCreditorPaymentSelector_btnSuspensions" runat="server" style="float:left;width:7em;margin-right:9em;" value="Suspensions" onclick="GenericCreditorPaymentSelector_btnSuspensions_Click();" />


    <input type="button" id="GenericCreditorPaymentSelector_btnView" runat="server" style="float:left;width:6em;margin-right:0.5em;" value="View" onclick="GenericCreditorPaymentSelector_btnView_Click();" />
    <input type="button" id="GenericCreditorPaymentSelector_btnNew" runat="server" style="float:left;width:6em;" value="New" onclick="GenericCreditorPaymentSelector_btnNew_Click();" title="Create a New Creditor Payment." />

</div>
<div class="clearer"></div>
<uc1:CareTypeSelector id="cTypeSelector" runat="server" />
