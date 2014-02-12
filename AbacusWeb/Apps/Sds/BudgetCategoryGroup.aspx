<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BudgetCategoryGroup.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.BudgetCategoryGroup"
    EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain budget category groups.
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
        <fieldset id="fsSummarisationControls" style="padding:0.5em; width : 35em; margin-left : 8em;" runat="server">
            <legend>Unit Summarisation</legend>
            <div style="margin-left : 2.5em">
                <asp:CheckBox ID="chkGroupUnitsOfServiceOnInvoice" runat="server" Text="Group units of service on the service user invoice" EnableViewState="true" />            
                <br />
                <br />
                <asp:Label ID="lblUnitOfMeasure" runat="server" Text="Unit of Measure : Not Set" />        
            </div>
        </fieldset>
        <br />
        <br />
        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="7.60em"></cc1:CheckBoxEx>
    </fieldset>
    <br />
</asp:Content>