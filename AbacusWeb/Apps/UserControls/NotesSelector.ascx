<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="NotesSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.NotesSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<div id="divError" runat="server">
   <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</div>
<div id="Notes_Content" runat="server">
 <fieldset id="filterControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
 <legend>Filters</legend>
         <select id="DropDownListFilter">
        </select>
 </fieldset>

<table class="listTable" id="tblNotes" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of Notes.">
<caption>
    <div class="caption">List of Notes.</div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thCreatedDate" style="width:15%;">Created Date</th>
		<th id="thCreatedBy" style="width:15%;">Created By</th>
		<th id="thCategory" style="width:20%;">Category</th>
		<th id="thNotes" style="width:50%;">Notes</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
</div>
<div id="Notes_PagingLinks" style="float:left;"></div>
<div style="float:right;border: 2 solid red;" >

<uc2:ReportsButton id="Notes_btnList" disabled="False" runat="server" ButtonText="List" ButtonWidth="65px" ></uc2:ReportsButton>
</div>
<input type="button" id="Notes_btnEdit" style="float:right;width:5em;" value="Edit" runat="server" onclick="NotesSelector_btnEdit_Click();" />
<input type="button" id="Notes_btnView" style="float:right;width:5em;" value="View" runat="server" onclick="NotesSelector_btnView_Click();" />
<input type="button" id="Notes_btnNew" style="float:right;width:5em;" value="New"  runat="server" onclick="NotesSelector_btnNew_Click();" />
<div class="clearer"></div>

