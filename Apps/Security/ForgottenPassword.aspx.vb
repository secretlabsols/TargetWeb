
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Security.PasswordPolicyChecker
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Security

    ''' <summary>
    ''' Screen to allow a user to reset their password if they have forgotten it.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     07/08/2012  A4WA7489 - Newport are having issues with the forgotten password facility 
    '''                             in Extranet TEST. After entering the Email and recaptcha text then clicking 
    '''                             submit, a timeout occurs. 
    '''     MoTahir     08/02/2010  D11934 Intranet Password Maintenance
    '''     MikeVO      17/02/2010  A4WA#6101 - upgraded reCAPTCHA to v1.3 to allow use of OverrideSecureMode property.
    '''                             Forced reCAPTCHA to request over HTTPS to prevent mixed content browser issues when 
    '''                             HTTPS is off-loaded, e.g. to upstream load balancer.
    '''     MikeVO      14/09/2009  D11602 - menu improvements.
    '''     MikeVO      31/03/2009  D11538 - account activation and reset password overhaul.
    '''     MikeVO      30/01/2009  A4WA#5200 - guard against username enumeration.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ForgottenPassword
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Forgotten Password")

            Me.RenderMenu = False

            Dim css As StringBuilder = New StringBuilder()
            With css
                .Append("div.captchaHelp { display:none;margin-top:0.5em;font-style:italic; }")
                .Append("div.captchaHelp img { vertical-align:middle; }")
                .Append("a.captchaHelpLink { display:inline-block;margin-top:0.5em; }")
            End With

            Me.JsLinks.Add("ForgottenPassword.js")
            Me.AddExtraCssStyle(css.ToString())

            litPageOverview.Text = "If you have forgotten your password, enter the email address you use to login to this site below."

            Me.Form.DefaultButton = btnSubmit.UniqueID

            recaptcha.OverrideSecureMode = True

        End Sub

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            Try
                If IsValid Then
                    Dim msg As ErrorMessage = SecurityBL.ForgottenPassword(Me.DbConnection, txtEmail.Text, Me.Settings.CurrentApplicationID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    litPageOverview.Text = String.Format("Thank you. If the email address you have entered ({0}) matches a valid account then you will shortly receive an email sent to that address. Please follow the instructions in this email to reset your password.", txtEmail.Text)
                    grpForgottenPassword.Visible = False
                ElseIf Not recaptcha.IsValid Then
                    lblCaptchaIncorrect.Visible = True
                End If
            Catch ex As Exception
                Dim msg As ErrorMessage = Target.Library.Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                WebUtils.DisplayError(Msg)
            End Try
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim msg As ErrorMessage
            Dim policyOptions As PasswordPolicyOptions = Nothing
            Dim policyManager As PasswordPolicyManager = Nothing
            ' get current policy
            policyManager = New PasswordPolicyManager()
            msg = policyManager.FetchPasswordPolicy(Me.DbConnection, Nothing, Me.Settings, policyOptions)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            RecaptchControlSection.Visible = policyOptions.CheckUseRecaptcha
        End Sub
    End Class

End Namespace