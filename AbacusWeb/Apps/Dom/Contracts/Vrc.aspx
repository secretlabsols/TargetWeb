<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Vrc.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.Vrc" 
	EnableEventValidation="false" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:DropDownListEx ID="cboDayCategory" runat="server" LabelText="Day Category" LabelWidth="10em"
			Required="True" RequiredValidatorErrMsg="Please select a day category" ValidationGroup="Save"></cc1:DropDownListEx>
		<br />
		<cc1:DropDownListEx ID="cboTimeBand" runat="server" LabelText="Time Band" LabelWidth="10em"
			Required="True" RequiredValidatorErrMsg="Please select a time band" ValidationGroup="Save"></cc1:DropDownListEx>
		<br />
		<cc1:DropDownListEx ID="cboServiceType" runat="server" LabelText="Service Type" LabelWidth="10em"
			Required="True" RequiredValidatorErrMsg="Please select a service type" ValidationGroup="Save"></cc1:DropDownListEx>
		<br />
		<cc1:DropDownListEx ID="cboCareWorkers" runat="server" LabelText="Care Workers" LabelWidth="10em"></cc1:DropDownListEx>
		<br />
        <cc1:TextBoxEx ID="txtMinutesFrom" runat="server" LabelText="Minutes From" Format="IntegerFormat" LabelWidth="10em" Width="4em"
            Required="true" RequiredValidatorErrMsg="Please enter a minutes from value" ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtMinutesTo" runat="server" LabelText="Minutes To" Format="IntegerFormat" LabelWidth="10em" Width="4em"
            Required="true" RequiredValidatorErrMsg="Please enter a minutes to value" ValidationGroup="Save"></cc1:TextBoxEx>
        <cc1:CompositeCompareValidator id="compMinutes" runat="server" ControlToValidate="txtMinutesFrom" ControlToCompare="txtMinutesTo"
			    ControlToValidateSuffix="_txtTextBox" ControlToCompareSuffix="_txtTextBox" Display="Dynamic" 
			    ErrorMessage="The minute values entered are invalid." Type="Integer" Operator="LessThan" ValidationGroup="Save"></cc1:CompositeCompareValidator>
        <br />
		<fieldset>
			<legend>Where Total Minutes Represents The Entire Visit</legend>
			<cc1:DropDownListEx ID="cboEntireVisitUnitRounding" runat="server" LabelText="Unit Rounding" LabelWidth="10em"
				Required="true" RequiredValidatorErrMsg="Please select a rounding value" ValidationGroup="Save"></cc1:DropDownListEx>
			<br />
			<cc1:DropDownListEx ID="cboEntireVisitUnitConversion" runat="server" LabelText="Unit Conversion" LabelWidth="10em"
				Required="true" RequiredValidatorErrMsg="Please select a conversion value" ValidationGroup="Save"></cc1:DropDownListEx>
			<br />
			<cc1:DropDownListEx ID="cboEntireVisitRateCategory" runat="server" LabelText="Rate Category" LabelWidth="10em"
				Required="true" RequiredValidatorErrMsg="Please select a rate category" ValidationGroup="Save"></cc1:DropDownListEx>
			<br />
		</fieldset>
		<br />
		<fieldset>
			<legend>Where Total Minutes Represents Part Of A Visit</legend>
			<cc1:DropDownListEx ID="cboPartVisitUnitRounding" runat="server" LabelText="Unit Rounding" LabelWidth="10em"
				Required="true" RequiredValidatorErrMsg="Please select a rounding value" ValidationGroup="Save"></cc1:DropDownListEx>
			<br />
			<cc1:DropDownListEx ID="cboPartVisitUnitConversion" runat="server" LabelText="Unit Conversion" LabelWidth="10em"
				Required="true" RequiredValidatorErrMsg="Please select a conversion value" ValidationGroup="Save"></cc1:DropDownListEx>
			<br />
			<cc1:DropDownListEx ID="cboPartVisitRateCategory" runat="server" LabelText="Rate Category" LabelWidth="10em"
				Required="true" RequiredValidatorErrMsg="Please select a rate category" ValidationGroup="Save"></cc1:DropDownListEx>
			<br />
		</fieldset>
    </fieldset>
    <br />
</asp:Content>