<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ResetPassword.aspx.vb" Inherits="Target.Web.Apps.Security.ResetPassword" EnableViewState="True" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		<asp:Literal id="litPageOverview" runat="server"></asp:Literal>
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litError" runat="server"></asp:Literal>
	</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <fieldset style="width:31.00em;" id="grpResetPassword" runat="server">
		    <legend>Reset Password</legend>
    		
		    <cc1:DropDownListEx ID="cboSecurityQuestion" runat="server" LabelText="Security Question" LabelWidth="13em"
    		    Required="true" RequiredValidatorErrMsg="Please select a security question"></cc1:DropDownListEx>
    		<br />
    		
    		<cc1:TextBoxEx id="txtAnswer" runat="server" LabelText="Answer" LabelWidth="13em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter the answer to your security question."></cc1:TextBoxEx>
			<br />
    		
    		<cc1:TextBoxEx id="txtPassword" runat="server" Format="PasswordFormat" LabelText="New Password" LabelWidth="13em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your new password."></cc1:TextBoxEx>	
		    <br />
		    
		    <cc1:TextBoxEx id="txtPasswordConfirm" runat="server" Format="PasswordFormat" LabelText="Confirm New Password" LabelWidth="13em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please confirm your new password."></cc1:TextBoxEx>
		    <br />
		    
		    <cc1:CompositeCompareValidator id="compNewPasswords" runat="server" ControlToValidate="txtPassword" ControlToCompare="txtPasswordConfirm"
			    ControlToValidateSuffix="_txtTextBox" ControlToCompareSuffix="_txtTextBox" Display="Dynamic" 
			    ErrorMessage="Please ensure you have confirmed your new password correctly."></cc1:CompositeCompareValidator>
		    		    	
		    <p>
			    <asp:Button id="btnSubmit" runat="server" Text="Submit"></asp:Button>
		    </p>
	    </fieldset>
    </asp:Content>