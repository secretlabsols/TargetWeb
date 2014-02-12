<%@ Control Language="vb" AutoEventWireup="false" Codebehind="PasswordExceptionSelector.ascx.vb" Inherits="Target.Web.Apps.Security.UserControls.PasswordExceptionSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<table class="listTable" style="table-layout:fixed;" id="tblPEs" cellpadding="2" cellspacing="0" width="100%" summary="List of password exceptions.">
<caption>List of password exceptions.</caption>
<thead>
	<tr>
		<th id="thWord">Word</th>
		<th id="thList">List</th>
		<th>&nbsp;</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>
<div id="PasswordException_PagingLinks" style="float:left;"></div>
<input type="button" id="btnAdd" style="float:right;width:5.5em;" value="Add" title="Click here to add a new password exception" onclick="btnAdd_Click();" />
<div class="clearer"></div>
<br />