<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ContractEndReasons.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.ContractEndReasons"
    EnableViewState="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different contract end reasons.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="10em" MaxLength="255" 
            Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a description" SetFocus="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <asp:Label ID="lblUsage" runat="server" AssociatedControlID="divUsage" Text="Usage" Width="10em" />
        <div id="divUsage" runat="server" style="float:left;margin-left:0em;width:17em; border:solid 1px silver;padding:5px;" >
            <cc1:CheckBoxEx ID="chkDomContracts" runat="server" Text="Non-Residential Contracts" LabelWidth="15em"></cc1:CheckBoxEx>
            <cc1:CheckBoxEx ID="chkDPContracts" runat="server" Text="DP Contracts" LabelWidth="15em"></cc1:CheckBoxEx>
            <cc1:CheckBoxEx ID="chkDPContractDetails" runat="server" Text="DP Contract Payments" LabelWidth="15em"></cc1:CheckBoxEx>
        </div>
        <div class="clearer"></div>
        <br />
        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="9.7em"></cc1:CheckBoxEx>
    </fieldset>
    <br />
</asp:Content>