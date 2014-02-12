<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DurationClaimedRoundingSelector.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.UserControls.DurationClaimedRoundingSelector" %>

<table class="listTable" id="tblDCR"  cellpadding="2" cellspacing="0" width="100%" summary="List of Duration Claimed Rounding.">
<caption>List of Duration Claimed Rounding.</caption>
<thead>
	<tr>
	    <th style="width:1.5em;"></th>
		<th id="thRef">Reference</th>
		<th id="thDescription">Description</th>
		<th id="thExternalAccount">External Account</th>
	</tr>
</thead>
<tbody>
    <tr>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
     </tr>
</tbody>
</table>
<div id="DCR_PagingLinks" style="float:left;"></div>
<div style="float:right;">
    <input type="button"  style="width:5em;" id="btnView" value="View" title="View" onclick="btnView_Click();" />
    <input type="button"  style="width:5em;" id="btnCopy" value="Copy" title="Copy" onclick="btnCopy_Click();" />
    <input type="button"  style="width:5em;" id="btnNew" value="New" title="New" onclick="btnNew_Click();" />
    <br />
</div>
<div class="clearer"></div>