<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CreateRegister.aspx.vb" Inherits="Target.Abacus.Web.Apps.Actuals.DayCare.CreateRegister" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>

<asp:Content ID="conOverview" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to create new service registers.
</asp:Content>

<asp:Content ID="conError" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" />
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
        <asp:Label AssociatedControlID="provider" runat="server" Text="Provider" Width="10.5em" />
	    <uc2:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders" />
	    <br />
	    <asp:Label AssociatedControlID="domContract" runat="server" Text="Contract" Width="10.5em" />
	    <uc3:InPlaceDomContract id="domContract" runat="server" />
	    <br />
	    <cc1:TextBoxEx ID="dteDatesEffectiveDate" runat="server" LabelText="Week Ending Date" LabelWidth="11em" Format="DateFormatJquery" Required="true" RequiredValidatorErrMsg="Please enter a valid week ending date" Width="6em" />        
        <br />
    </fieldset>
    <br />
</asp:Content>