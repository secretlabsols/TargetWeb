<%@ Page Language="vb" AutoEventWireup="false" Codebehind="SelectRecipients.aspx.vb" Inherits="Target.Web.Apps.Msg.SelectRecipients" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageError" runat="server"><asp:Literal id=litError runat="server"></asp:Literal></asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <div style="padding-left:2em;">
		    <div id="title">
			    Please select who you wish to send the message to.
		    </div>
		    <br />			
		    <label for="cboSource">Select from </label>
		    <select id="cboSource" onchange="cboSource_OnChange();"></select>
		    <br />
		    <br />
		    <cc1:DualList id="dlRecipients" runat="server"
			    SrcListCaption="Available" SrcListRows="10" SrcListWidth="15em"
			    DestListCaption="Selected" DestListRows="10" DestListWidth="15em">
		    </cc1:DualList>
		    <div class="clearer"></div>
		    <br />
		    <input type="button" id="btnOK" value="OK" onclick="btnOK_OnClick()" />
		    <input type="button" id="btnOK" value="Cancel" onclick="btnCancel_OnClick();" />
	    </div>
    </asp:Content>