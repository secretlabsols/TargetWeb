<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReOpenedWeeks.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.ReOpenedWeeks" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="dteWeekEnding"  runat="server" LabelText="Week Ending Date" Format="DateFormat" LabelWidth="10em"
            Required="true" RequiredValidatorErrMsg="Please enter a week ending date"
            ValidationGroup="Save"></cc1:TextBoxEx>
    </fieldset>
    <br />
</asp:Content>