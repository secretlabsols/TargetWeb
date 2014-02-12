<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ContractSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.ContractSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<style type="text/css">
    .style1
    {
        width: 290px;
    }
    .style2
    {
        width: 104px;
    }
</style>

<table class="listTable" id="tblContracts" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available domiciliary.">
<caption>
    <div class="caption">List of available contracts.</div>
    <div class="mruList">
        <cc1:MruList ID="mru" runat="server" MruListKey="CONTRACTS" BlankItemText="[Recent Contracts]" ClientOnChange="ContractSelector_MruOnChange"></cc1:MruList>
    </div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thNumber" style="width:12%" >Number</th>
		<th id="thTitle" class="style1" style="width:20%">Title</th>		
		<%If Me.ShowProviderColumn Then%>
		    <th style="width:20%">Provider</th>
		<% End If %>	
		<%If Me.ShowCreditorColumn Then%>
		    <th id="thCreditor" style="width:20%">Creditor</th>
		<% End If %>	
		<th style="width:10%">Date From</th>
		<th style="width:10%">Date To</th>
		<th id="thGroup">Contract Grp</th>
		<th id="thSvcGroup">Service Grp</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><%If Me.ShowProviderColumn Then%><td></td><% End If %><%If Me.ShowCreditorColumn Then%><td></td><% End If %><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Contract_PagingLinks" style="float:left;"></div>
<input type="button" id="btnTerminate" runat="server" style="float:right;width:7em;" value="Terminate" onclick="btnTerminate_Click();" />
<input type="button" id="btnReinstate" runat="server" style="float:right;width:7em;" value="Re-instate" onclick="btnReinstate_Click();" />
<input type="button" id="btnCopy" runat="server" style="float:right;width:5em;" value="Copy" onclick="btnCopy_Click();" />
<input type="button" id="btnView" runat="server" style="float:right;width:5em;" value="View" onclick="btnView_Click();" />
<input type="button" id="btnNew" runat="server" style="float:right;width:5em;" value="New" onclick="btnNew_Click();" />
<div class="clearer"></div>
