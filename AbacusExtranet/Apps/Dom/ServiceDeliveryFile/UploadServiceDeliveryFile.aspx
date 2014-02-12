<%@ Page Language="vb" AutoEventWireup="false" Codebehind="UploadServiceDeliveryFile.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ServiceDeliveryFile.UploadServiceDeliveryFile" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		To upload the service delivery file, click the "Browse..." button and select the file, then click "Next".
	</asp:Content>
    <asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <cc1:FileUpload ID="flServiceDelFileUpload" runat="server"  Caption="Upload Service Delivery File" MaxFiles="1" Width="50%"></cc1:FileUpload>
        <br /><br />
        
        <input type="button" id="btnBack" value="Back" style="width:4em;" onclick="btnBack_click();" />
        <input type="button" id="btnNext" value="Next" style="width:4em;"  onclick="btnNext_click();" />
        
    </asp:Content>
