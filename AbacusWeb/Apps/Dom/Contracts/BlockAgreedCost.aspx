<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BlockAgreedCost.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.BlockAgreedCost" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceFinCode" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" runat="server" EnableViewState="false">
        <asp:Label AssociatedControlID="txtAgreedPayment" runat="server" Text="Agreed Weekly Payment" Width="15em" style="float:left;" ></asp:Label>
        <cc1:TextBoxEx ID="txtAgreedPayment"  runat="server" MaxLength="10" Format="CurrencyFormat"
            Width="6em" Required="true" RequiredValidatorErrMsg="Please specify an amount"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
		<asp:Label AssociatedControlID="client" runat="server" Text="System Account" Width="15em"  style="float:left;" ></asp:Label>
        <uc3:InPlaceClient id="client" runat="server" Required="true" RequiredValidatorErrMsg="Please select a system account" ValidationGroup="Save"></uc3:InPlaceClient>
        <br />
        <asp:Label AssociatedControlID="cboRateCategory" runat="server" Text="Rate Category" Width="15em" style="float:left;" ></asp:Label>
		<cc1:DropDownListEx ID="cboRateCategory" runat="server" Required="true" RequiredValidatorErrMsg="Please select a rate category" ValidationGroup="Save"></cc1:DropDownListEx>
		<br />
        <asp:Label AssociatedControlID="financeCode" runat="server" Text="Finance Code" Width="15em" style="float:left;" ></asp:Label>
        <uc2:InPlaceFinCode id="financeCode" runat="server"></uc2:InPlaceFinCode>
        <br />
        <br /><br />
    </fieldset>
    <br />
</asp:Content>