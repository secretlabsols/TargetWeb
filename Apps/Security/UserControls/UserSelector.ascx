<%@ Control Language="vb" AutoEventWireup="false" Codebehind="UserSelector.ascx.vb" Inherits="Target.Web.Apps.Security.UserControls.UserSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<table class="listTable" id="tblUsers" cellpadding="2" cellspacing="0" width="100%" summary="List of users.">
<caption>List of users.</caption>
<thead>
	<tr>
		<th id="thFirstName">First Name</th>
		<th id="thSurname">Surname</th>
		<th id="thEmail">Email/Username</th>
		<th id="thExternalAccount">External Account</th>
		<th id="thStatus">Status</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="User_PagingLinks" style="float:left;"></div>
<div style="float:right;">
    <uc1:ReportsButton id="ctlUsers" runat="server"></uc1:ReportsButton>
    <uc1:ReportsButton id="ctlUserRoles" runat="server"></uc1:ReportsButton>
</div>
<div class="clearer"></div>
<br />