<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EnhancedRateDays.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.EnhancedRateDays"
    EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different enhanced rate days.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="dteDate"  runat="server"  LabelText="Date" LabelWidth="6em" MaxLength="255" 
            Required="true" RequiredValidatorErrMsg="Please enter a date" SetFocus="true" Format="DateFormatJquery" AllowClear="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
    </fieldset>
    <br />
</asp:Content>