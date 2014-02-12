<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="MyDetails.aspx.vb" Inherits="Target.Web.Apps.Security.MyDetails" EnableViewState="True" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	    <asp:Label id="lblForced" runat="server" Visible="False" CssClass="warningText">
			You have been redirected to this page because you are required to confirm your details.<br /><br />
		</asp:Label>
		<asp:Literal id="litPageOverview" runat="server"></asp:Literal>
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litError" runat="server"></asp:Literal>
	</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <fieldset style="width:31.00em;" id="grpMyDetails" runat="server">
		    <legend>My Details</legend>
    		
		    <cc1:TextBoxEx id="txtFirstName" runat="server" LabelText="First Name" LabelWidth="10em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your first name." SetFocus="True"></cc1:TextBoxEx>
		    <br />
    		
		    <cc1:TextBoxEx id="txtSurname" runat="server" LabelText="Surname" LabelWidth="10em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your surname."></cc1:TextBoxEx>
		    <br />
    		
		    <cc1:TextBoxEx id="txtEmail" runat="server" LabelText="Email" LabelWidth="10em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your email address."></cc1:TextBoxEx>
    		<br />
    		
    		<cc1:DropDownListEx ID="cboSecurityQuestion" runat="server" LabelText="Security Question" LabelWidth="10em"
    		    Required="true" RequiredValidatorErrMsg="Please select a security question"></cc1:DropDownListEx>
    		<br />
    		
    		<cc1:TextBoxEx id="txtAnswer" runat="server" LabelText="Answer" LabelWidth="10em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter the answer to your security question."></cc1:TextBoxEx>
			<br />
    		
    		<cc1:TextBoxEx id="txtPassword" runat="server" Format="PasswordFormat" LabelText="Current Password" LabelWidth="10em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your current password."></cc1:TextBoxEx>
		    		    	
		    <p>
			    <asp:Button id="btnSave" runat="server" Text="Save"></asp:Button>
		    </p>
	    </fieldset>
    </asp:Content>