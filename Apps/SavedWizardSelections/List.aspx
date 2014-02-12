<%@ Page Language="vb" AutoEventWireup="false" Codebehind="List.aspx.vb" Inherits="Target.Web.Apps.SavedWizardSelections.List" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to view, edit and delete saved wizard selections.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litPageError" runat="server"></asp:Literal>
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <div id="divBtnBack" runat="server">
	        <input type="button" id="btnBack" value="Back" onclick="btnBack_Click();" />
    	    <br />
	    </div>
        <table class="listTable" style="table-layout:fixed;" id="tblSelections" cellpadding="2" cellspacing="0" width="100%" summary="List of saved wizard selections">
        <caption>List of saved wizard selections</caption>
        <thead>
	        <tr>
	            <th style="width:1.5em;"></th>
		        <th id="thName">Name</th>
		        <th id="thScreen">Screen</th>
		        <th id="thOwner">Owner</th>
		        <th>Global?</th>
	        </tr>
        </thead>
        <tbody><tr><td></td><td></td><td></td><td></td><td></td></tr></tbody>
        </table>
        <div id="Selections_PagingLinks" style="float:left;"></div>
        <div style="float:right;">
            <input type="button" id="btnView" value="View" disabled="true" onclick="btnView_Click();" />
        </div>
        <div class="clearer"></div>
    </asp:Content>