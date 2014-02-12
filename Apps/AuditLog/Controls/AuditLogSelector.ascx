<%@ Control Language="vb" AutoEventWireup="false" Codebehind="AuditLogSelector.ascx.vb" Inherits="Target.Web.Apps.AuditLog.Controls.AuditLogSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<div style="height:22em;padding:3px;" id="divLogEntries">
	<table class="listTable" id="tblLogEntries" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of audit log entries.">
	<caption>List of audit log entries.</caption>
	<thead>
		<tr>
			<th style="width:22%">Date</th>
			<th style="width:13%">User</th>
			<th style="width:55%">Area</th>
			<th style="width:10%">Activity</th>
		</tr>
	</thead>
	<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
	</table>
	<div id="divPagingLinks" style="float:left"></div>
	<input type="button" id="btnDisplayAll" style="float:right;width:6em;" value="Display All" title="Display all available details" onclick="btnDisplayAll_Click()" />
	<input type="button" id="btnDisplayPage" style="float:right;width:10em;" value="Display This Page" title="Display all details for the currently selected page" onclick="btnDisplayPage_Click()" />
	<div class="clearer"></div>
</div>
<hr style="height:2px;" />
<div style="padding:3px;" id="divDetails">
</div>
