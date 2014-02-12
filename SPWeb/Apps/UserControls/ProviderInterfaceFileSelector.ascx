<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ProviderInterfaceFileSelector.ascx.vb" Inherits="Target.SP.Web.Apps.UserControls.ProviderInterfaceFileSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListInterfaceFile" style="table-layout:fixed; float:left;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available interface files.">
<caption>List of available interface files.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th>File Number</th>
		<th>Date From</th>
		<th>Date To</th>
		<th>No of Records</th>
		<th>Total Value</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="InterfaceFile_PagingLinks" style="float:left;"></div>
<input type="button" id="btnCapFormat" style="float:right;width:8em;" value="Download" title="Click here to download the selected interface file in the standard data interchange format." onclick="btnDownload_OnClick('cap');" />
<!--
Removed in Release 1
<input type="button" id="btnTargetFormat" style="float:right;width:10em;" value="Download Custom" title="Click here to download the selected interface file in the custom Comma Separated Values (CSV) format." onclick="btnDownload_OnClick('target');" />
-->
<div class="clearer"></div>
<br />