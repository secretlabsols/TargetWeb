<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ActivateAccount.aspx.vb" Inherits="Target.Web.Apps.Security.ActivateAccount" EnableViewState="True" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		<asp:Literal id="litPageOverview" runat="server"></asp:Literal>
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litError" runat="server"></asp:Literal>
	</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <fieldset style="width:31.00em;" id="grpMyDetails" runat="server">
		    <legend>My Details</legend>
    		
		    <cc1:TextBoxEx id="txtFirstName" runat="server" LabelText="First Name" LabelWidth="13em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your first name." SetFocus="True"></cc1:TextBoxEx>
		    <br />
    		
		    <cc1:TextBoxEx id="txtSurname" runat="server" LabelText="Surname" LabelWidth="13em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your surname."></cc1:TextBoxEx>
		    <br />
    		
		    <cc1:TextBoxEx id="txtEmail" runat="server" LabelText="Email" LabelWidth="13em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your email address."></cc1:TextBoxEx>
    		<br />
    		
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
			    <asp:Button id="btnActivate" runat="server" Text="Activate"></asp:Button>
		    </p>
	    </fieldset>
    </asp:Content>