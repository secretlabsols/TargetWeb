<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SystemAccount.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.SystemAccount" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
		<asp:Label ID="Label1" AssociatedControlID="client" runat="server" Text="Account" Width="8em" style="float:left;"></asp:Label>
        <uc3:InPlaceClient id="client" runat="server"></uc3:InPlaceClient>
        <br />
        <cc1:TextBoxEx ID="txtFinanceCode"  runat="server" LabelText="Finance Code" LabelWidth="8em" MaxLength="25" Required="true" RequiredValidatorErrMsg="Please select a system account" ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
    </fieldset>
    <br />
</asp:Content>