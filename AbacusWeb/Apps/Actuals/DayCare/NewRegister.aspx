<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="NewRegister.aspx.vb" Inherits="Target.Abacus.Web.Apps.Actuals.DayCare.NewRegister"
    EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to create new registers.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label> 
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
    <div id ="selectors" runat="server">
		<asp:Label AssociatedControlID="provider" runat="server" Text="Provider" Width="10.5em"></asp:Label>
		<uc2:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc2:InPlaceEstablishment>
		<br />
		<asp:Label AssociatedControlID="domContract" runat="server" Text="Contract" Width="10.5em"></asp:Label>
		<uc3:InPlaceDomContract id="domContract" runat="server"></uc3:InPlaceDomContract>
		<br />
		<cc1:TextBoxEx ID="dteDatesEffectiveDate" runat="server" LabelText="Week Ending Date" LabelWidth="11em" Format="DateFormat"
            Required="true" RequiredValidatorErrMsg="Please enter a valid week ending date"></cc1:TextBoxEx>
        <asp:RangeValidator id="rvDatesEffectiveDate" runat="server" ValidationGroup="Save" Display="Dynamic" EnableClientScript="True" ErrorMessage="Actual service for future periods may not be entered" SetFocusOnError="True" />
        <br />
     </div>
    </fieldset>
<div class="clearer"></div>
    <br />
</asp:Content>