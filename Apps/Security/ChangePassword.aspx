<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ChangePassword.aspx.vb" Inherits="Target.Web.Apps.Security.ChangePassword" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		<asp:Label id="lblForced" runat="server" Visible="False" CssClass="warningText">
			You have been redirected to this page because your password has expired.<br /><br />
		</asp:Label>
		<asp:Literal id="litSuccessMsg" runat="server">To change your password please enter your current password and then your new password below.</asp:Literal>
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litError" runat="server"></asp:Literal>
	</asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <fieldset style="width:32.95em;" id="grpChangePassword" runat="server">
		    <legend>Change Password</legend>
    		
		    <cc1:TextBoxEx id="txtCurrent" runat="server" Format="PasswordFormat" LabelText="Current Password" LabelWidth="12.40em" Width="17.44em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your current password." SetFocus="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtNew" runat="server" Format="PasswordFormat" LabelText="New Password" LabelWidth="12.40em" Width="17.44em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your new password."></cc1:TextBoxEx>	
		    <br />
		    <cc1:TextBoxEx id="txtConfirm" runat="server" Format="PasswordFormat" LabelText="Confirm New Password" LabelWidth="12.40em" Width="17.44em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please confirm your new password."></cc1:TextBoxEx>
		    <br />
		    <cc1:CompositeCompareValidator id="compNewPasswords" runat="server" ControlToValidate="txtNew" ControlToCompare="txtConfirm"
			    ControlToValidateSuffix="_txtTextBox" ControlToCompareSuffix="_txtTextBox" Display="Dynamic" 
			    ErrorMessage="Please ensure you have confirmed your new password correctly."></cc1:CompositeCompareValidator>
		    <p>
			    <asp:Button id="btnSubmit" runat="server" Text="Submit"></asp:Button>
		    </p>
	    </fieldset>
    </asp:Content>
