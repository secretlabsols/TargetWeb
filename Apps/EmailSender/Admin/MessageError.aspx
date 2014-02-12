<%@ Page Language="vb" AutoEventWireup="false" Codebehind="MessageError.aspx.vb" Inherits="Target.Web.Apps.EmailSender.Admin.MessageError" MasterPageFile="~/Popup.master" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		<p style="padding:10px 10px 0px 10px;float:left;">
			The error is displayed below.
		</p>
		<p style="float:right;margin:10px 5px 0px 0px;">
			<input onclick="window.close()" type="button" value="Close" />
		</p>
		<hr style="clear:both;width:98%" />
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <p style="padding:0px 10px 0px 10px;">
		    <asp:Literal id="litError" runat="server"></asp:Literal>
	    </p>
    </asp:Content>