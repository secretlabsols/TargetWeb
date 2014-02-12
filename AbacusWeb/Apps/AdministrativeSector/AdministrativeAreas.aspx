<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AdministrativeAreas.aspx.vb" Inherits="Target.Abacus.Web.Apps.AdministrativeSector.AdministrativeAreas" EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different administration areas.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="txtTitle"  runat="server" LabelText="Title" LabelWidth="12em" MaxLength="150" 
            Width="30em"  Required="true" RequiredValidatorErrMsg="Please enter a title" SetFocus="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="11.75em"></cc1:CheckBoxEx>
        <br /><br />
    </fieldset>
    <br />
</asp:Content>

