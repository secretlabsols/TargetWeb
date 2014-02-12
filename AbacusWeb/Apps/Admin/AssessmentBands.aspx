<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AssessmentBands.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.AssessmentBands" EnableViewState="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different Assessment Bands.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
        <cc1:TextBoxEx ID="txtBand"  runat="server"  LabelText="Assessment Band" LabelWidth="10em" MaxLength="1" 
            Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a band"  
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="10em" MaxLength="50" 
            Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a description" 
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="9.7em"></cc1:CheckBoxEx>
    </fieldset>
    <br />
</asp:Content>