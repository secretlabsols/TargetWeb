<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Header.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.Header" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsCopyingFrom" style="margin-bottom:1em;" runat="server" visible="false">
		<legend>Copying Contract</legend>
		
    </fieldset>
    <fieldset id="fsWarnings" style="margin-bottom:1em;" runat="server" visible="false">
		<legend>Warnings</legend>
		
    </fieldset>
    <fieldset id="fsControls" runat="server" EnableViewState="false">
        <cc1:TextBoxEx ID="txtNumber"  runat="server" LabelText="Number" LabelWidth="11em" MaxLength="10" 
            Width="20em" Required="true" RequiredValidatorErrMsg="Please enter a number"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtTitle"  runat="server" LabelText="Title" LabelWidth="11em" MaxLength="50" 
            Width="20em" Required="true" RequiredValidatorErrMsg="Please enter a title"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description" LabelWidth="11em" MaxLength="255" Width="20em"></cc1:TextBoxEx>
        <br />
        <asp:Label ID="Label1" AssociatedControlID="provider" runat="server" Text="Provider" Width="11em" style="float:left;" ></asp:Label>
		<uc2:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc2:InPlaceEstablishment>
        <br />
        <cc1:TextBoxEx ID="dteStartDate"  runat="server" LabelText="Start Date" Format="DateFormat" LabelWidth="11em"
            Width="20em" Required="true" RequiredValidatorErrMsg="Please enter a start date"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="dteEndDate" runat="server" LabelText="End Date" LabelWidth="11em" IsReadOnly="true"></cc1:TextBoxEx>
        <br /><br />
        <cc1:TextBoxEx ID="txtEndReason" runat="server" LabelText="End Reason" LabelWidth="11em" IsReadOnly="true"></cc1:TextBoxEx>
        <br /><br />
        <cc1:TextBoxEx ID="txtServiceGroup" runat="server" ReadOnlyContentCssClass="disabled" LabelText="Service Group" LabelWidth="11em" IsReadOnly="true"></cc1:TextBoxEx>
        <br /><br />        
        <cc1:TextBoxEx ID="txtAltRef"  runat="server" LabelText="Alt Reference" LabelWidth="11em" MaxLength="30" 
            Width="20em" Required="false" RequiredValidatorErrMsg="Please enter an alternate reference"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:DropDownListEx ID="cboContractType" runat="server" LabelText="Contract Type" LabelWidth="11em"
			Required="true" RequiredValidatorErrMsg="Please select a contract type" ValidationGroup="Save"></cc1:DropDownListEx>
		<br />
		<asp:Label AssociatedControlID="client" runat="server" Text="Service User" Width="11em"  style="float:left;" ></asp:Label>
        <uc3:InPlaceClient id="client" runat="server"></uc3:InPlaceClient>
        <br />
        <cc1:DropDownListEx ID="cboContractGroup" runat="server" LabelText="Contract Group" LabelWidth="11em"></cc1:DropDownListEx>
		<br />
        <cc1:DropDownListEx ID="cboAdministrativeArea" runat="server" LabelText="Administrative Area" LabelWidth="11em"></cc1:DropDownListEx>
		<br />
		<cc1:DropDownListEx ID="cboRateFramework" runat="server" LabelText="Rate Framework" LabelWidth="11em"
			Required="true" RequiredValidatorErrMsg="Please select a rate framework" ValidationGroup="Save"
			ReadOnlyContentCssClass="disabled"></cc1:DropDownListEx>
		<br />
		<cc1:CheckBoxEx ID="chkUseEnhancedRateDays" runat="server" Text="Use Enhanced Rate Days" LabelWidth="15.0em" IsReadOnly="true"></cc1:CheckBoxEx>
        <br /><br />
        <cc1:CheckBoxEx ID="chkBankHolidayCover" runat="server" Text="Bank Holiday Cover" LabelWidth="15.0em"></cc1:CheckBoxEx>
        <br /><br />
        <asp:Panel ID="pnlChargeSU" runat="server" >
            <cc1:CheckBoxEx ID="chkChargeSU" runat="server" Text="Charge Service Users For Additional Carers" LabelWidth="15.0em"></cc1:CheckBoxEx>
            <br /><br /><br />
        </asp:Panel>
        <cc1:CheckBoxEx ID="chkDsoMaintExternal" runat="server" Text="DSOs Maintained via Electronic Interface" LabelWidth="15.0em" IsReadOnly="true"></cc1:CheckBoxEx>
        <br /><br /><br />
        <cc1:CheckBoxEx ID="chkProviderUnitCostOverride" runat="server" Text="Allow Provider Unit Costs to be overridden" LabelWidth="15.0em" IsReadOnly="true"></cc1:CheckBoxEx>
        <br /><br />
         <cc1:DropDownListEx ID="cboVerificationText" runat="server" LabelText="Provider Invoice Verification Text" LabelWidth="11em"></cc1:DropDownListEx>
    </fieldset>
    <br />
</asp:Content>