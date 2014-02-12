<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CloseWeek.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.CloseWeek" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This date controls the weeks that are considered to be "closed".<br /><br />
    Providers are not permitted to enter new visits (either manually or by 
    uploading a service delivery file) or amend existing visits for closed weeks 
    unless the week has been explicitly <a href="ReOpenWeek.aspx">re-opened</a> for that contract.<br /><br />
    All week-ending dates upto and including this date are closed.<br />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="dteDate"  runat="server" LabelText="Date" Format="DateFormat" LabelWidth="5em"
            ValidationGroup="Save"></cc1:TextBoxEx>
    </fieldset>
    <br />
</asp:Content>