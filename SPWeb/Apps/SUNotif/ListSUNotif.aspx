<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ListSUNotif.aspx.vb" Inherits="Target.SP.Web.Apps.SUNotif.ListSUNotif" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		The list below displays all of the service user notifications that you or a member of your company have submitted. 
		You can use the filters below to restrict the list to better locate the notifications you are interested in.
	</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <fieldset style="width:33em; float:left;">
		    <legend>Filters</legend>
		    <cc1:TextBoxEx id="dteFrom" runat="server" LabelText="From" Format="DateFormat" LabelWidth="7.5em"></cc1:TextBoxEx>
		    <cc1:TextBoxEx id="dteTo" runat="server" LabelText="To" Format="DateFormat" LabelWidth="7.5em"></cc1:TextBoxEx>
		    <label for="cboStatus" style="width:7.5em;">Status</label><select id="cboStatus" style="width:20em;" runat="server"></select>
		    <asp:Panel id="pnlRequestedByFilter" runat="server">
			    <label for="cboRequestedBy" style="width:7.5em;">Requested By</label><select id="cboRequestedBy" style="width:20em;"><option value="0"></option></select>
		    </asp:Panel>
	    </fieldset>
	    <input type="button" id="btnFilter" style="float:left; margin: 0.5em 1em; width:5em;" title="Click here to apply these filters to the list below." value="Filter" onclick="btnFilter_OnClick();" />
    	
	    <table class="listTable" id="Notifs_Table" style="float:left;table-layout:fixed;" cellpadding="2" cellspacing="0" width="100%" summary="Displays your service user notifications.">
	    <caption style="margin-top:2em;">Displays your service user notifications.</caption>
	    <thead>
		    <tr>
			    <th id="thRef">Reference</th>
			    <th id="thName" style="width:20%;">Service User</th>
			    <th style="width:15%;">Type</th>
			    <th>Created</th>
			    <th>Submitted</th>
			    <th>Completed</th>
			    <th>Status</th>
			    <th></th>
		    </tr>
	    </thead>
	    <tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
	    </table>
    	
	    <div id="Notifs_PagingLinks" style="float:left;"></div>
	    <div class="clearer"></div>
    </asp:Content>