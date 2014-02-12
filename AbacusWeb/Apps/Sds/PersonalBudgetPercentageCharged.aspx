<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PersonalBudgetPercentageCharged.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.PersonalBudgetPercentageCharged"
    EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain percentages charged for personal budgets.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Date From" LabelWidth="10em"
            Required="true" RequiredValidatorErrMsg="Please enter a Date From" Format="DateFormat"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Date To" LabelWidth="10em"
            Format="DateFormat"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="valPercentCharged" runat="server" LabelText="Percentage Charged" LabelWidth="10em"
            Required="true" RequiredValidatorErrMsg="Please enter a valid percentage figure" Format="IntegerFormat"
            ValidationGroup="Save" MinimumValue="0" MaximumValue="100"></cc1:TextBoxEx>
        <br />
    </fieldset>
    <br />
</asp:Content>