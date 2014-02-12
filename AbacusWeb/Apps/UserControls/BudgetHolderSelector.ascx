<%@ Control Language="vb" AutoEventWireup="false" Codebehind="BudgetHolderSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.BudgetHolderSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<table class="listTable sortable" id="tblBudgetHolders" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available budget holders.">
<caption>
    <div class="caption">List of available budget holders.</div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thRef" style="width:16%;">Reference</th>
		<th id="thCreditorRef" style="width:16%;">Creditor Ref.</th>
		<th id="thName" style="width:33%;">Name</th>
		<th id="thOrganisation" style="width:33%;">Organisation</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="BudgetHolder_PagingLinks" style="float:left;"></div>
<div class="clearer"></div>
