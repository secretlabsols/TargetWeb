<%@ Control Language="vb" AutoEventWireup="false" Codebehind="SubmittedPIReturnsList.ascx.vb" Inherits="Target.SP.Web.Apps.UserControls.SubmittedPIReturnsList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListSubmittedPIReturns" style="table-layout:fixed; float:left"  cellpadding="2" cellspacing="0" width="100%" summary="List of submitted PI Returns.">
<caption>List of Submitted PI Returns.</caption>
<thead>
	<tr>
		<th style="width:1.5em"></th>
		<th style="width:10%">Provider Ref</th>
		<th style="width:25%">Provider</th>
		<th style="width:10%">Service Ref</th>
		<th style="width:25%">Service</th>
		<th>Financial Yr/Qtr</th>
		<th>Status</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="PIReturns_PagingLinks" style="float:left"></div>
<input type="button" id="btnViewDetails" disabled value="View Details" style="float:right; margin-left:0.5em" title="View additional details" onclick="btnViewDetails_Click()" >
<input type="button" id="btnDownloadFile" disabled value="Download File" style="float:right; " title="Download and view PI Return" onclick="btnDownloadFile_Click()" >
<div class="clearer"></div>
<br >
