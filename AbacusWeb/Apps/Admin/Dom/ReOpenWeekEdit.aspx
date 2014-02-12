<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReOpenWeekEdit.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.ReOpenWeekEdit" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    Displayed below are the details of this domiciliary contract re-opened week.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
    
        <cc1:TextBoxEx ID="txtProvider" runat="server" LabelText="Provider" LabelWidth="13em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
        <br /><br />
        <cc1:TextBoxEx ID="txtContract" runat="server" LabelText="Contract" LabelWidth="13em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
        <br /><br />
        <cc1:TextBoxEx ID="dteWeekEnding"  runat="server" LabelText="Week Ending Date" Format="DateFormat" LabelWidth="13em"
            Required="true" RequiredValidatorErrMsg="Please enter a week ending date"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtReason"  runat="server" LabelText="Reason for Re-open" LabelWidth="13em" Width="30em"
            Required="true" RequiredValidatorErrMsg="Please enter a reason for re-opening this week"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="dteClosure"  runat="server" LabelText="Expected Closure Date" Format="DateFormat" LabelWidth="13em"
            Required="true" RequiredValidatorErrMsg="Please enter an expected closure date"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <asp:Panel id="pnlViewOnly" runat="server" Visible="False">
            <cc1:TextBoxEx ID="txtReOpenedBy" runat="server" LabelText="Re-Opened By" LabelWidth="13em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
            <br /><br />
            <cc1:TextBoxEx ID="dteReOpenedDate" runat="server" LabelText="Re-Opened Date" LabelWidth="13em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
            <br /><br />
        </asp:Panel>
            
    </fieldset>
    <br />
</asp:Content>