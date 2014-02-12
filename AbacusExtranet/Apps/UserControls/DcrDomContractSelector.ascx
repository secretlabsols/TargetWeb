<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DcrDomContractSelector.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.UserControls.DcrDomContractSelector" %>
<table class="listTable" id="tblContracts" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available domiciliary.">
<caption>
    <div class="caption">List of available contracts.</div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thNumber" style="width:12%" >Number</th>
		<th id="thTitle" class="style1" style="width:25%">Title</th>
		<th style="width:25%">Provider</th>
		<th style="width:10%">Date From</th>
		<th style="width:10%">Date To</th>
		<th id="thGroup" class="style2">Group</th>
	</tr>
</thead>
<tbody>
    <tr>
        <td></td>
        <td></td>
        <td></td>
        <td class="style1"></td>
        <td class="style2"></td>
        <td></td>
        <td></td>
     </tr>
</tbody>
</table>
<div id="DcrDomContract_PagingLinks" style="float:left;"></div>