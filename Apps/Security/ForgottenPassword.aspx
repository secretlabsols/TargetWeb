
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ForgottenPassword.aspx.vb" Inherits="Target.Web.Apps.Security.ForgottenPassword" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		<asp:Literal id="litPageOverview" runat="server"></asp:Literal>
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litError" runat="server"></asp:Literal>
	</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <fieldset id="grpForgottenPassword" runat="server">
		    <legend>Forgotten Password</legend>
		    <cc1:TextBoxEx id="txtEmail" runat="server" LabelText="Email" LabelWidth="4em" Width="23.5em" MaxLength="255"
			    Required="True" RequiredValidatorErrMsg="Please enter your email address." SetFocus="True"></cc1:TextBoxEx>
			<br />
			<p runat="server" id="RecaptchControlSection">
			    Please enter the words you see in the image below into the box, in order and separated by a space.
			    <br />
			    Doing so helps prevent automated programs from abusing this service.
			    <div style="margin-top:1em;margin-left:3.65em;">
			        <%--
			            CAPTCHA service and ASP.NET control from http://recaptcha.net/
			            Use the account details in section 12 of the DevSpec for D11538 to access this site.
			            Public/private keys are from the "global-key.targetsys.co.uk" site.
			        --%>
			        <recaptcha:RecaptchaControl
                        ID="recaptcha"
                        runat="server"
                        Theme="white"
                        PublicKey="6LeEvgUAAAAAAKGU3AEPW3_I3QrYH-VqaOQeWBGk"            
                        PrivateKey="6LeEvgUAAAAAAH4nIHoJQCwD2DtYuuLDdbbKMNrf" 
                    />
                    <asp:Label id="lblCaptchaIncorrect" runat="server" visible="false" cssclass="errorText">The words you typed were incorrect. Please try again.</asp:Label>
                    <a id="captchaHelpLink" class="captchaHelpLink" href="javascript:ShowCaptchaHelp();">Need more help?</a>
                    <div id="captchaHelp" class="captchaHelp">
                        Having trouble with this challenge (image or audio)? Use the <img src="../../Images/Captcha/refresh.gif" alt="CAPTCHA refresh icon" /> icon to ask for another.
                        <br /><br />
                        Use the <img src="../../Images/Captcha/text.gif" alt="CAPTCHA text challenge icon" /> and <img src="../../Images/Captcha/audio.gif" alt="CAPTCHA audio challenge icon" /> icons to switch between a visual and audio challenge.
                        <br /><br />
                        Need more help? Use the <img src="../../Images/Captcha/help.gif" alt="CAPTCHA help icon" /> icon.
                    </div>
                </div>
            </p>
		    <p>
			    <asp:Button id="btnSubmit" runat="server" Text="Submit"></asp:Button>
		    </p>
		    
	    </fieldset>
    </asp:Content>