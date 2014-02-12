<%@ Control Language="vb" AutoEventWireup="false" Codebehind="RoleSelector.ascx.vb" Inherits="Target.Web.Apps.Security.UserControls.RoleSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<table class="listTable" style="table-layout:fixed;" id="tblRoles" cellpadding="2" cellspacing="0" width="100%" summary="List of security roles.">
<caption>List of security roles.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thName" style="width:30%">Name</th>
		<th>Description</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Role_PagingLinks" style="float:left;"></div>
<div style="float:right;">
    <input type="button" id="btnView" runat="server" style="width:4em;" onclick="btnView_Click();" value="View" />
    <input type="button" id="btnNew" runat="server" style="width:4em;" onclick="btnNew_Click();" value="New" />
    <uc1:ReportsButton id="ctlList" runat="server"></uc1:ReportsButton>
</div>
<div class="clearer"></div>
<br />