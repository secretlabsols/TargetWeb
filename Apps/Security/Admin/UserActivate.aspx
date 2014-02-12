<%@ Page Language="vb" AutoEventWireup="false" Codebehind="UserActivate.aspx.vb" Inherits="Target.Web.Apps.Security.Admin.UserActivate" 
	EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details of the newly created user. To activate this user to allow them to access this site 
		please fill in the fields below and click on "Activate".
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litPageError" runat="server"></asp:Literal>
	</asp:Content>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <fieldset style="width:60%;">
		    <legend>User Details</legend>
		    <cc1:TextBoxEx id="txtExternalUserName" runat="server" LabelText="External Username" LabelWidth="14em" LabelBold="True" IsReadOnly="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtExternalFullName" runat="server" LabelText="External Full Name" LabelWidth="14em" LabelBold="True" IsReadOnly="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtCreated" runat="server" LabelText="Created" LabelWidth="14em" LabelBold="True" IsReadOnly="True"></cc1:TextBoxEx>
		    <br /><br />
		    <cc1:TextBoxEx id="txtFirstName" runat="server" LabelText="First Name" LabelWidth="14em" LabelBold="True" MaxLength="50" Width="55%"
			    Required="True" RequiredValidatorErrMsg="Please enter the users first name."></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtSurname" runat="server" LabelText="Surname" LabelWidth="14em" LabelBold="True" MaxLength="50" Width="55%"
			    Required="True" RequiredValidatorErrMsg="Please enter the users surname."></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtEmail" runat="server" LabelText="Email Address" LabelWidth="14em" LabelBold="True" MaxLength="255" Width="55%"
			    Required="True" RequiredValidatorErrMsg="Please enter the users email address."></cc1:TextBoxEx>
			<br />
			<cc1:DropDownListEx ID="cboSecurityQuestion" runat="server" LabelText="Security Question" LabelWidth="14em" LabelBold="true"
    		    Required="true" RequiredValidatorErrMsg="Please select a security question"></cc1:DropDownListEx>
    		<br />    		
    		<cc1:TextBoxEx id="txtAnswer" runat="server" LabelText="Answer" LabelWidth="14em" LabelBold="true" Width="55%" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter an answer to the security question."></cc1:TextBoxEx>
			<br />
	    </fieldset>
	    <br /><br />
	    <asp:Button id="btnActivate" runat="server" Text="Activate" title="Click here to activate this user."></asp:Button>
	    <input type="button" id="btnCancel" value="Cancel" title="Click here to return to the previous screen." onclick="history.go(-1);" />
    </asp:Content>