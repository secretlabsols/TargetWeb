<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="StatementsSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.StatementsSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%> 
<table class="listTable" id="StatementsSelector_tblStatements" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of statements.">
<caption>List of Statements.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
        <th id="thRef">Reference</th>
        <th id="thDateFrom">Date From</th>
		<th id="thDateTo">Date To</th>
		<th id="thDateCreated">Date Created</th>
		<th id="thCreatedBy">Created By</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>

<div id="StatementsSelector_PagingLinks" style="float:left;"></div>

<input type="button" id="StatementsSelector_btnProperties" style="float:right;width:7em;" value="Properties" onclick="StatementsSelector_btnProperties_Click();" />
<input type="button" id="StatementsSelector_btnView" style="float:right;width:5em;" value="View" onclick="StatementsSelector_btnView_Click();" />
<input type="button" id="StatementsSelector_btnNew" style="float:right;width:5em;" value="New" onclick="StatementsSelector_btnNew_Click();" />

<div class="clearer"></div>
