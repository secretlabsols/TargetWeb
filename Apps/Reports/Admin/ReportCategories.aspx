<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReportCategories.aspx.vb" Inherits="Target.Web.Apps.Reports.Admin.ReportCategories"
    EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different report categories.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="8em" MaxLength="50" 
            Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a description" SetFocus="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
    </fieldset>
    <br />
</asp:Content>