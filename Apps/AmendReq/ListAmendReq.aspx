<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ListAmendReq.aspx.vb" Inherits="Target.Web.Apps.AmendReq.ListAmendReq"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		The list below displays all of the amendment requests that you or a member of your company have submitted. 
		You can use the filters below to restrict the list to better locate the requests you are interested in.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server"></asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <!-- 
		    SPBG-322: Early IE6 versions don't like the float style on the fieldset on this screen, although it works fine on the
		    View SU Notif which is almost identical!! Anyway, place the float on a DIV instead and everyones happy.
	     -->
	    <div style="float:left;">
		    <fieldset style="width:33em;">
			    <legend>Filters</legend>
			    <cc1:TextBoxEx id="dteFrom" runat="server" LabelText="From" Format="DateFormat" LabelWidth="7.5em"></cc1:TextBoxEx>
			    <cc1:TextBoxEx id="dteTo" runat="server" LabelText="To" Format="DateFormat" LabelWidth="7.5em"></cc1:TextBoxEx>
			    <label for="cboStatus" style="width:7.5em;">Status</label><select id="cboStatus" style="width:20em;" runat="server"></select>
			    <asp:Panel id="pnlRequestedByFilter" runat="server">
				    <label for="cboRequestedBy" style="width:7.5em;">Requested By</label><select id="cboRequestedBy" style="width:20em;"><option value="0"></option></select>
			    </asp:Panel>
		    </fieldset>
	    </div>
	    <input type="button" id="btnFilter" style="float:left; margin: 0.5em 1em; width:5em;" title="Click here to apply these filters to the list below." value="Filter" onclick="btnFilter_OnClick();" />
    	
	    <table class="listTable sortable" id="Requests_Table" style="table-layout:fixed;" cellpadding="2" cellspacing="0" width="100%" summary="Displays your amendments requests.">
	    <caption style="margin-top:2em;">Displays your amendments requests.</caption>
	    <thead>
		    <tr>
			    <th style="width:10%;">Reference</th>
			    <th style="width:12%;">Requested</th>
			    <th>Request</th>
			    <th style="width:10%;">Status</th>
			    <th style="width:3.5em;"></th>
		    </tr>
	    </thead>
	    <tbody><tr><td></td><td></td><td></td><td></td><td></td></tr></tbody>
	    </table>
	    <div id="Requests_PagingLinks" style="float:left;"></div>
	    <div class="clearer"></div>	
    </asp:Content>