<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ReportSelector.ascx.vb" Inherits="Target.Web.Apps.Reports.UserControls.ReportSelector" %>

<table class="listTable" id="tblReports" style="table-layout:fixed;" cellpadding="2" cellspacing="0" width="100%" summary="List of reports.">
<caption>List of reports.</caption>
<thead>
	<tr>
	    <th style="width:1.5em;"></th>
		<th id="thDesc">Description</th>
		<th id="thPath">Path</th>
		<th id="thCategories">Categories</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>
<div id="divPagingLinks" style="float:left;"></div>
<div style="float:right;">
    <input type="button" id="btnView" value="View" onclick="btnView_Click();" />
</div>
<div class="clearer"></div>
<br />