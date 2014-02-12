<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DocumentSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.DocumentSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%> 
<table class="listTable" id="DocumentSelector_tblDocuments" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of documents.">
<caption>List of Documents.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
        <th>Reference</th>
        <th>Description</th>
		<th>Filename</th>
		<th>Created Date</th>
		<th>Created By</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>
<div id="DocumentSelector_PagingLinks" style="float:left;"></div>
<input type="button" id="DocumentSelector_btnNew" runat="server" style="float:right;width:5em;" value="New" onclick="DocumentSelector_btnNew_Click();" />
<div class="clearer"></div>
