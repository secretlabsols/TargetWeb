<%@ Control Language="vb" AutoEventWireup="false" Codebehind="NewSUNotifEnterDetails.ascx.vb" Inherits="Target.SP.Web.Apps.UserControls.NewSUNotifEnterDetails" 
	TargetSchema="http://schemas.microsoft.com/intellisense/ie5" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<br />
	<fieldset>
		<legend>Service Details</legend>
		<cc1:TextBoxEx id="txtExpectedStartDate" runat="server" LabelText="Expected Start Date" LabelWidth="11em" Format="DateFormat"
			Required="True" RequiredValidatorErrMsg="Please enter the expected start date." SetFocus="true"></cc1:TextBoxEx>
		<br />
		<cc1:CheckBoxEx id="chkTenancySupport" runat="server" 
			Text="Does the service user already have a signed tenancy/support agreement?&nbsp;"></cc1:CheckBoxEx>
		<br />
		<br />
		<cc1:TextBoxEx id="txtYourReference" runat="server" LabelText="Your Reference" LabelWidth="11em" MaxLength="25"></cc1:TextBoxEx>
		<br />
		<cc1:TextBoxEx id="txtServiceLevel" runat="server" LabelText="Service Level" LabelWidth="11em" MaxLength="25"></cc1:TextBoxEx>
		<br />
		<cc1:TextBoxEx id="txtUnitCost" runat="server" LabelText="Unit Cost" LabelWidth="11em" MaxLength="7" Width="4em" Format="CurrencyFormat"></cc1:TextBoxEx>
	</fieldset>
	<br />
	<fieldset>
		<legend>Service User</legend>
		<asp:Label style="width:11em;" id="lblPrimaryTitle" runat="server">Title</asp:Label> 
		<cc1:DropDownListEx id="cboPrimaryTitle" runat="server"></cc1:DropDownListEx>
		<br />
		<cc1:TextBoxEx id="txtPrimaryFirstNames" runat="server" LabelText="First Name(s)" LabelWidth="11em" MaxLength="20"
			Required="True" RequiredValidatorErrMsg="Please enter the first name(s) of the service user."></cc1:TextBoxEx>
		<br />
		<cc1:TextBoxEx id="txtPrimarySurname" runat="server" LabelText="Surname" LabelWidth="11em"  MaxLength="20"
			Required="True" RequiredValidatorErrMsg="Please enter the surname of the service user."></cc1:TextBoxEx>
		<br />
		<cc1:TextBoxEx id="txtPrimaryNINo" runat="server" LabelText="NI No." LabelWidth="11em"  MaxLength="9"></cc1:TextBoxEx>
		<br />
		<cc1:TextBoxEx id="txtPrimaryDoB" runat="server" LabelText="Date of Birth" LabelWidth="11em" Format="DateFormat"></cc1:TextBoxEx>
	</fieldset>
	<br />
	<fieldset>
		<legend>Address</legend>
		<cc1:TextBoxEx id="txtAddress" runat="server" LabelText="Address" LabelWidth="11em" Width="20em"
			Required="True" RequiredValidatorErrMsg="Please enter the address of the service user."></cc1:TextBoxEx>
		<br />
		<cc1:TextBoxEx id="txtPostcode" runat="server" LabelText="Postcode" LabelWidth="11em" MaxLength="10"
			Required="True" RequiredValidatorErrMsg="Please enter the postcode of the service user."></cc1:TextBoxEx>
		<br />
	</fieldset>
	<br />
	<fieldset>
		<legend>Secondary Service User</legend>
		<asp:Label style="width:11em;" id="lblSecondaryTitle" runat="server">Title</asp:Label> 
		<cc1:DropDownListEx id="cboSecondaryTitle" runat="server"></cc1:DropDownListEx>
		<br />
		<cc1:TextBoxEx id="txtSecondaryFirstNames" runat="server" LabelText="First Name(s)" LabelWidth="11em" MaxLength="20"></cc1:TextBoxEx>
		<br />
		<cc1:TextBoxEx id="txtSecondarySurname" runat="server" LabelText="Surname" LabelWidth="11em" MaxLength="20"></cc1:TextBoxEx>
		<br />
		<cc1:TextBoxEx id="txtSecondaryNINo" runat="server" LabelText="NI No." LabelWidth="11em" MaxLength="9"></cc1:TextBoxEx>
		<br />
		<cc1:TextBoxEx id="txtSecondaryDoB" runat="server" LabelText="Date of Birth" LabelWidth="11em" Format="DateFormat"></cc1:TextBoxEx>
	</fieldset>
	<br />
	
	
	