<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ClientSelector.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.UserControls.ClientSelector" 
TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<table class="listTable sortable" id="tblClients" style="table-layout:fixed;" cellpadding="2" cellspacing="0" width="100%" summary="List of available service users.">
<caption>List of available service users.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thRef" style="width:12%">Reference</th>
		<th id="thName" style="width:35%">Name</th>
		<%		    
		    If Me.Mode = Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithDomSvcOrders Or _
		        Me.Mode = Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithDomProviderInvoices Or _
		        Me.Mode = Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithVisitBasedDomProviderInvoices Then
        %>
		    <th>Address</th>
		<%Else%>
		    <th>NI No.</th>
		    <th>Date of Birth</th>
		    <th>Deceased Date</th>
		<%End If%>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><%If Me.Mode <> Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithDomSvcOrders Then%><td></td><td></td><%End If%></tr></tbody>
</table>
<div id="Client_PagingLinks" style="float:left;"></div>
<input type="button" id="btnViewServiceUser" value="View Service User" style="display:none;float:right;" title="View details of the selected service user" onclick="btnViewServiceUser_Click()" />
<div class="clearer"></div>
