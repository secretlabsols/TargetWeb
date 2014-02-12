<%@ Page Language="vb" AutoEventWireup="false" Codebehind="UploadServiceDeliveryFileCompletion.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ServiceDeliveryFile.UploadServiceDeliveryFileCompletion" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		<asp:Label ID="lblconfirmation" runat="server" ></asp:Label>
        <asp:HyperLink id="lnkServiceDelEnq" runat="server"></asp:HyperLink>
	</asp:Content>
	<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        
    </asp:Content>
