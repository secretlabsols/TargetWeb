<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ServiceDeliveryFileSelector.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.ServicedeliveryFileSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListServiceDeliveryFiles" style="table-layout:fixed; width:100%"  cellpadding="2" cellspacing="0" width="100%" summary="Service Delivery File list.">
<caption>List of uploaded service delivery files.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thRef" style="width:12%;">Reference</th>
		<th id="thDesc">File Description</th>
		<th style="width:15%;">Submitted By</th>
		<th style="width:13%;">Uploaded On</th>
		<th style="width:17%;">Status</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Detail_PagingLinks" style="float:left;"></div>
<input type="button" id="btnViewServiceDeliveryFile" value="View" style="float:right;width:7em; margin-bottom:3px;" title="View details of the selected service delivery file" onclick="btnViewServiceDeliveryFile_Click()" />
<input type="button" id="btnNewServiceDeliveryFile" value="New" style="float:right;width:7em; margin-right:5px; margin-bottom:3px;" title="Upload a new Service Delivery File" onclick="btnNewServiceDeliveryFile_Click()" />

<div class="clearer"></div>
