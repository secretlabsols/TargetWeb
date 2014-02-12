<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Login.aspx.vb" Inherits="Target.Web.Apps.Security.Login" 
	EnableViewState="True" AspCompat="True" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server"><asp:Literal id="litPageOverview" runat="server">Please enter your email address and password to login.</asp:Literal></asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server"><asp:Literal id="litLoginError" runat="server"></asp:Literal></asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <asp:Label id=lblTimeout runat="server" CssClass="warningText" Visible="False">
		    You have been redirected to this page because your login session timed out. Please login again.<br /><br />
	    </asp:Label>
	    <fieldset style="width:27em;float:left;">
		    <legend>Login</legend>
		    <cc1:TextBoxEx id="txtEmail" runat="server" LabelText="Email" LabelWidth="6em" Width="17em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your email address." SetFocus="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtPassword" runat="server" Format="PasswordFormat" LabelText="Password" LabelWidth="6em" Width="17em" 
			    MaxLength="255" Required="True" RequiredValidatorErrMsg="Please enter your password."></cc1:TextBoxEx>
			<a href="ForgottenPassword.aspx" id="lnkForgottenPassword" runat="server" style="margin-left:6em;">Forgotten your password?</a>
		    <p>
			    <asp:Button id="btnLogin" runat="server" Text="Login" CssClass="stdButton" ></asp:Button>
		    </p>
	    </fieldset>
	    <div style="float:left;margin-left:5em;" id="dvLicenceWarning" runat="server">
	            <img src=<% Response.Write(Target.Library.Web.Utils.GetVirtualPath("Images/warning.png")) %> style="float:left;" />
	            <h2 style="float:left;margin:0.6em 0em 0em 0.6em;">Licence Warning</h2>
	            <div class="clearer"></div>
	            <br />
	            <div class="warningText">
	                One or more licensed module(s) will expire in <% Response.Write(daysUntilExpiry)%> days.
	                <br /><br />
	                Please contact your System Administrator.
                  </div>
         </div>
         <div style="float:left;margin-left:5em;" id="dvLicenceError" runat="server">
	            <img src=<% Response.Write(Target.Library.Web.Utils.GetVirtualPath("Images/error.png")) %> style="float:left;" />
	            <h2 style="float:left;margin:0.6em 0em 0em 0.6em;">Licence Warning</h2>
	            <div class="clearer"></div>
	            <br />
	            <div class="errorText">
	                One or more licensed module(s) have expired and are now unlicensed.
	                <br /><br />
	                Please contact your System Administrator.
                  </div>
               </div>
        <div class="clearer"></div>
    </asp:Content>