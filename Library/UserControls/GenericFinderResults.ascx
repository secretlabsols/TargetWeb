<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GenericFinderResults.ascx.vb"
    Inherits="Target.Web.Library.UserControls.GenericFinderResults" %>
<div style="text-align: left;">
    <table class="listTable" id="tblGenericFinder" style="table-layout: fixed;" cellpadding="4"
        cellspacing="0" width="100%">
        <tr>
            <th style="width: 1.5em;">
            </th>
            <asp:PlaceHolder ID="phHeaderCells" runat="server"></asp:PlaceHolder>
        </tr>
        <asp:PlaceHolder ID="phDataRows" runat="server"></asp:PlaceHolder>
    </table>
</div>
<div style="padding: 0.5em;">
    <div id="divPagingLinks" runat="server" style="float: left;">
    </div>
    <input type="button" id="btnCancel" value="Cancel" style="float: right;" title="Cancel the selection"
        onclick="btnCancel_Click()" />
    <input type="button" id="btnSelect" value="Select" style="float: right; margin-right: 0.5em;"
        disabled="disabled" title="Select the item" onclick="btnSelect_Click()" />
    <div class="clearer">
    </div>
</div>
