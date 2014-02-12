<%@ Control Language="vb" AutoEventWireup="false" Codebehind="DomContractSelector.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.DomContractSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" class="listTable" id="tblContracts" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available domiciliary.">
<caption>List of available contracts.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thNumber" style="width:8.8em;">Contract No.</th>
		<th id="thTitle" style="width:25%;">Contract Title</th>
		<th style="width:17%;">Provider Name</th>
		<th >Type</th>
		<th >Rounding</th>
		<th style="width:7em;" >Date From</th>
		<th style="width:7em;" >Date To</th>
	</tr>
</thead>
<tbody>
    <tr>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
     </tr>
 </tbody>
</table>
<div id="DomContract_PagingLinks" style="float:left;"></div>
<input  type="button" id="btnView" runat="server" style="float:right;width:3.9em; margin-right:3px; margin-bottom:4px;" value="View" onclick="btnView_Click();" />
<input  type="button" id="btnNew" runat="server" style="float:right;width:3.9em; margin-right:3px; margin-bottom:4px;" value="New" onclick="btnNew_Click();" />
<div class="clearer"></div>
