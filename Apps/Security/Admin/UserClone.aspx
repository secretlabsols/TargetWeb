<%@ Page Language="vb" AutoEventWireup="false" Codebehind="UserClone.aspx.vb" Inherits="Target.Web.Apps.Security.Admin.UserClone" 
	EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to "clone" your own user account to create a new account for another person within your organisation.
		This new account will have access to this site with the same access rights as you do.  
		To create a new account, fill in the new user details below and click on "Clone".<br /><br />
		NOTE: once created, you will then have to <strong>activate the new account</strong> before it can be used.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litPageError" runat="server"></asp:Literal>
	</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <fieldset style="width:60%;">
		    <legend>User Details</legend>
		    <cc1:TextBoxEx id="txtFirstName" runat="server" LabelText="First Name" LabelWidth="14em" LabelBold="True" MaxLength="50" Width="55%"
			    Required="True" RequiredValidatorErrMsg="Please enter the users first name."></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtSurname" runat="server" LabelText="Surname" LabelWidth="14em" LabelBold="True" MaxLength="50" Width="55%"
			    Required="True" RequiredValidatorErrMsg="Please enter the users surname."></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtEmail" runat="server" LabelText="Email Address" LabelWidth="14em" LabelBold="True" MaxLength="255" Width="55%"
			    Required="True" RequiredValidatorErrMsg="Please enter the users email address."></cc1:TextBoxEx>
	    </fieldset>
	    <br /><br />
	    <asp:Button id="btnClone" runat="server" Text="Clone" title="Click here to clone your account."></asp:Button>
	    <input type="button" id="btnCancel" value="Cancel" title="Click here to return to the previous screen." onclick="history.go(-1);" />	
    </asp:Content>