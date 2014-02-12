<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="IndicativeBudget.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.IndicativeBudget" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to view and edit the details of a indicative budget.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    
    <fieldset id="fsControls" runat="server" EnableViewState="false">
        <br />
        <asp:Label AssociatedControlID="ipClient" runat="server" Text="Service User" Width="10em" style="float:left;" />
        <uc2:InPlaceClient id="ipClient" runat="server" />
        <br />
        <cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Date From" LabelWidth="10em"
	        Required="true" RequiredValidatorErrMsg="Please enter a start date" Format="DateFormat"
	        ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Date To" LabelWidth="10em"
	        Format="DateFormat" ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtValue" runat="server" LabelText="Value" LabelWidth="10em"
            Required="true" RequiredValidatorErrMsg="Please enter an indicative budget value" Format="CurrencyFormat"
            ValidationGroup="Save" Width="5em"></cc1:TextBoxEx>
        <br />
        
    </fieldset>
            
</asp:Content>