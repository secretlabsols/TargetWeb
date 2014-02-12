<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OtherFundingOrganization.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.OtherFundingOrganizationMaintenance" EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="BasicAuditDetails" Src="~/Library/UserControls/BasicAuditDetails.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain details of Other Local Authorities / Other Organisation.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <uc2:BasicAuditDetails id="auditDetails" runat="server"></uc2:BasicAuditDetails>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
    
        <input id="optOLA" runat="server" type="radio" name="type" checked="True" style="float:left; margin-left:11em;" />
        <label class="label" style="float:left" for="optOLA" >Other Local Authority (OLA)</label>
        <input id="optOther" runat="server" type="radio" name="type" style="float:left; margin-left:2em;" />
        <label class="label" for="optOther" style="float:left" >Other Organisation</label>
        <br />
        <br />
        <cc1:TextBoxEx ID="txtName"  runat="server"  LabelText="Name" LabelWidth="11em" MaxLength="50" 
            Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a name." SetFocus="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtDebtorRef"  runat="server"  LabelText="Debtor Ref" LabelWidth="11em" MaxLength="10" 
            Width="5em" ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtAddress"  runat="server"  LabelText="Address" LabelWidth="11em"  
            Width="20em" Required="true" RequiredValidatorErrMsg="Please enter an Address." ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtPostCode"  runat="server"  LabelText="Postcode" LabelWidth="11em" MaxLength="10" 
            Width="5em" Required="true" RequiredValidatorErrMsg="Please enter a Postcode." ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtPhone"  runat="server"  LabelText="Phone" LabelWidth="11em" MaxLength="25" 
            Width="10em" Required="true" RequiredValidatorErrMsg="Please enter a Phone Number." ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtFax"  runat="server"  LabelText="Fax" LabelWidth="11em" MaxLength="25" 
            Width="10em" Required="true" RequiredValidatorErrMsg="Please enter a Fax Number." ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtEmail"  runat="server"  LabelText="Email" LabelWidth="11em" MaxLength="100" 
            Width="20em" Required="true" RequiredValidatorErrMsg="Please enter an Email Address." ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtContactTitleAndInitials"  runat="server"  LabelText="Title & Initials" LabelWidth="11em" MaxLength="20" 
            Width="10em" Required="true" RequiredValidatorErrMsg="Please enter Title and Initials." ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtContactSurname"  runat="server"  LabelText="Surname" LabelWidth="11em" MaxLength="20" 
            Width="10em" Required="true" RequiredValidatorErrMsg="Please enter a Surname." ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="10.5em"></cc1:CheckBoxEx>
    </fieldset>
    <br />
</asp:Content>
