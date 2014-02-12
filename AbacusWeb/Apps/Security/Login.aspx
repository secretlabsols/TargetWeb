<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Login.aspx.vb" Inherits="Target.Abacus.Web.Apps.Security.Login" EnableViewState="True" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">Please enter your Abacus username and password to login.</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server"><asp:Literal id=litLoginError runat="server"></asp:Literal></asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <asp:Label id=lblTimeout runat="server" CssClass="warningText" Visible="False">
		    You have been redirected to this page because your login session timed out. Please login again.<br /><br />
	    </asp:Label>
	    <fieldset style="width:350px;">
		    <legend>Login</legend>
		    <cc1:TextBoxEx id="txtUsername" runat="server" LabelText="Username" LabelWidth="80px" Width="225px" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your username." SetFocus="True" UpperCase="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtPassword" runat="server" Format="PasswordFormat" LabelText="Password" LabelWidth="80px" Width="225px" 
			    MaxLength="255" Required="False"></cc1:TextBoxEx>
		    <p>
			    <asp:Button id=btnLogin runat="server" Text="Login"></asp:Button>
		    </p>
	    </fieldset>
    </asp:Content>