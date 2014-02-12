Imports System.Configuration.ConfigurationManager
Imports System.Web.Security
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Licensing
Imports System.Collections.Generic
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Security

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Security.Login
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to login to the site.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     Mo Tahir    24/01/2013  D12454 - ABACUS Test System – Safety Net
    '''     MoTahir     08/02/2010  D11934 Intranet Password Maintenance
    '''     MoTahir     28/06/2010  D11829 - Licensing
    '''     MikeVO      14/09/2009  D11602 - menu improvements.
    '''     MikeVO      18/06/2009  D11515 - changed GenerateNewSession() redirect to preserve the full url.
    '''     MikeVO      05/06/2009  A4WA#5496 - don't call Me.Settings() before Me.InitPage().
    '''     MikeVO      03/04/2009  Attempt to generate new session cookie after authentication.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      09/10/2008  Fix to "The ConnectionString property has not been initialized" error.
    '''     MikeVO      15/03/2007  Changed error reporting to display the full error if a non-login related error has occurred.
    ''' 	[Mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class Login
        Inherits Target.Web.Apps.BasePage

        Private Const LOGIN_COOKIE_ID As String = "__LOGINCOOKIE__"
        Private Const SESSION_COOKIE_ID As String = "ASP.NET_SessionId"

        Public daysUntilExpiry As Integer

        Private forceLogin As Boolean = False


        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Login")
            SetupCustomLoginInterface()
        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Not Request.QueryString("timeout") Is Nothing Then lblTimeout.Visible = True
            If Not Request.QueryString("forcelogin") Is Nothing Then
                If Target.Library.Utils.ToInt32(Request.QueryString("forcelogin")) = 1 Then
                    forceLogin = True
                End If
            End If

            Me.RenderMenu = False
            GenerateNewSession()
            checkLicences()
            setLiveParametersInFTPConfig()
        End Sub

        ''' <summary>
        ''' Forces a new session just before authentication.
        ''' </summary>
        ''' <remarks><![CDATA[
        ''' ASP.NET does not support requesting a new session ID AFTER authentication, although this is against security best practice.
        ''' See https://connect.microsoft.com/feedback/viewfeedback.aspx?FeedbackID=143361&wa=wsignin1.0&siteid=210
        ''' Also see http://keepitlocked.net/archive/2007/12/26/preventing-session-fixation-through-session-id-regeneration-in-java-and-asp-net.aspx
        ''' 
        ''' Here we attempt to mitigate against a session fixation attack by forcing a new session just before authentication, 
        ''' thus reducing the time available to hijack the session.
        ''' See http://support.microsoft.com/kb/899918 for original solution.
        ''' ]]></remarks>
        Private Sub GenerateNewSession()

            Dim loginCookie As HttpCookie
            Dim ticket As FormsAuthenticationTicket

            loginCookie = Request.Cookies(LOGIN_COOKIE_ID)

            If Not IsPostBack AndAlso (loginCookie Is Nothing OrElse loginCookie.Value = "") Then
                ' At this point, we do not know if the session ID that we have is a new 
                ' session ID or if the session ID was passed by the client. 
                ' Update the session ID. 

                Session.Abandon()
                Response.Cookies.Add(New HttpCookie(SESSION_COOKIE_ID, ""))

                ' To make sure that the client clears the session ID cookie, respond to the client to tell 
                ' it that we have responded. To do this, set another cookie. 
                AddRedirCookie()
                Response.Redirect(Request.RawUrl)
            End If

            ' Make sure that someone is not trying to spoof. 
            Try
                If Not loginCookie Is Nothing AndAlso loginCookie.Value <> String.Empty Then
                    ticket = FormsAuthentication.Decrypt(loginCookie.Value)
                    If ticket Is Nothing OrElse ticket.Expired = True Then
                        Throw New Exception()
                    End If
                    RemoveRedirCookie()
                End If
            Catch
                ' If someone is trying to spoof, do it again. 
                AddRedirCookie()
                Response.Redirect(Request.Path)
            End Try

        End Sub

        Private Sub RemoveRedirCookie()
            Response.Cookies.Add(New HttpCookie(LOGIN_COOKIE_ID, ""))
        End Sub

        Private Sub AddRedirCookie()
            Dim ticket As New FormsAuthenticationTicket(1, "Test", DateTime.Now, DateTime.Now.AddSeconds(30), False, "", Request.ApplicationPath)
            Dim encryptedText As String = FormsAuthentication.Encrypt(ticket)
            Response.Cookies.Add(New HttpCookie(LOGIN_COOKIE_ID, encryptedText))
        End Sub

        Private Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
            If IsValid Then
                Dim msg As ErrorMessage = SecurityBL.Login(Me.DbConnection, txtEmail.Text, txtPassword.Text, Me.Settings.CurrentApplicationID, forceLogin)

                If msg.Success Then
                    FormsAuthentication.RedirectFromLoginPage(SecurityBL.GetCurrentUser().ID, False)
                ElseIf msg.Number = "E0505" Then
                    ' a normal login error has occurred
                    litLoginError.Text = msg.Message
                Else
                    ' otherwise something more serious has gone wrong
                    Target.Library.Web.Utils.DisplayError(msg)
                End If

            End If
        End Sub

        Private Sub SetupCustomLoginInterface()

            Dim customInterface As ICustomSecurity
            Dim replacementLogin As IReplacementLogin

            ' get custom login interface
            customInterface = SecurityBL.GetCustomInterface(Me.Settings)
            If Not customInterface Is Nothing Then
                replacementLogin = TryCast(customInterface, IReplacementLogin)
                If Not replacementLogin Is Nothing Then
                    With replacementLogin
                        litPageOverview.Text = .PageOverviewText
                        txtEmail.LabelText = .EmailAddressLabel
                        txtEmail.UpperCase = .EmailAddressUpperCase
                        txtEmail.RequiredValidatorErrMsg = String.Format("Please enter your {0}", .EmailAddressLabel.ToLower())
                        txtPassword.Required = False
                        lnkForgottenPassword.Visible = True
                    End With
                End If
            End If

        End Sub

        Private Sub checkLicences()
            Dim licences As ModuleLicence = ModuleLicence.GetCachedLicences(ConnectionStrings("Abacus").ConnectionString)
            If Date.Today > licences.minExpiryDate Then dvLicenceError.Visible = True Else dvLicenceError.Visible = False
            If Date.Today.AddDays(ModuleLicence.EXPIRY_WARNING_DEFAULT) <= licences.minExpiryDate And _
            licences.daysUntilExpiry.Days >= ModuleLicence.EXPIRY_WARNING_DEFAULT And _
            licences.daysUntilExpiry.Days < 0 Then _
            dvLicenceWarning.Visible = True Else dvLicenceWarning.Visible = False
            daysUntilExpiry = Math.Abs(licences.DaysUntilExpiry.Days)
        End Sub

        Private Sub setLiveParametersInFTPConfig()
            Dim liveParametersSet As Boolean = False
            Dim msg As ErrorMessage = Nothing

            msg = SecurityBL.SetLiveParametersInFTPConfig(Me.DbConnection, liveParametersSet)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not liveParametersSet Then
                msg = New ErrorMessage
                msg.Success = False
                msg.Number = SecurityBL.LIVE_FTP_SETTING_ERROR
                WebUtils.DisplayError(msg)
            End If

        End Sub

    End Class

End Namespace