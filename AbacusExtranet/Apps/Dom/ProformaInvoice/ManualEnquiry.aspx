<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ManualEnquiry.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.ManualEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
<%@ Register TagPrefix="uc2" TagName="psHeader" 
Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx"%>
<%@ Reference Control="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">

	
   <%-- <input runat="server" type="button" style="width:4.5em;"  value="Back" id="btnBack" name="btnBack"
           onclick="history.back(-1);"  />
	<br /><br />--%>
	<uc2:psHeader id="psHeader1" runat="server"></uc2:psHeader>
	<div style="margin-top:10px;">
      <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
    </div>
    </asp:Content>
