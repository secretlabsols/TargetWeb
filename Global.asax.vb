
Imports System.Configuration.ConfigurationManager
Imports System.Diagnostics
Imports System.Reflection
Imports System.Threading.Thread
Imports System.Web
Imports System.Web.Caching
Imports System.Web.SessionState
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Licensing

''' -----------------------------------------------------------------------------
''' Project	 : Target.Web
''' Class	 : Web.Global
''' 
''' -----------------------------------------------------------------------------
''' <summary>
'''     Global, customised http application for the site.
''' </summary>
''' <remarks>
''' </remarks>
''' <history><![CDATA[
'''     MoTahir     28/06/2010  D11829 - Licensing
'''     MikeVO      25/03/2009  D11536 - force all cookies to be in virtual dir path.
'''     MikeVO      26/01/2007  Added text size to Session_OnStart().
'''     MikeVO      07/12/2006  Added Global_BeginRequest to force en-GB culture where needed.
'''     MikeVO      29/08/2006  D10921 - support for config settings in database.
''' 	[mikevo]	??/??/????	Created
''' ]]></history>
''' -----------------------------------------------------------------------------
Public Class [Global]
    Inherits System.Web.HttpApplication

    Private Shared emailSenderTimer As System.Timers.Timer

#Region " Application Events "

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)

        Dim settings As SystemSettings
        Dim msg As ErrorMessage

        ' load system settings into the cache
        Try
            settings = New SystemSettings(ConnectionStrings("Abacus").ConnectionString)
            settings.LoadSettings()
            SystemSettings.CacheSettings(settings)
        Catch ex As Exception
            Throw New ApplicationException("The application settings could not be loaded.", ex)
        End Try

        ' load the audit log lookup info
        msg = AuditLogging.LoadLookupInfo(ConnectionStrings("Abacus").ConnectionString, Convert.ToInt32(AppSettings("CurrentApplication")))
        If Not msg.Success Then Throw New ApplicationException(msg.ToString())

        ' convert interval from minutes to milliseconds
        Dim interval As Double = settings("Apps.EmailSender.PollInterval") * 60 * 1000
        ' startup the email sender timer
        emailSenderTimer = New System.Timers.Timer
        AddHandler emailSenderTimer.Elapsed, AddressOf OnEmailSenderTimerElasped
        emailSenderTimer.Interval = interval
        emailSenderTimer.Start()

        ' initialise concurrent user licensing
        Dim concurrentUserLimit As Integer = 0

        ' IMPORTANT: The two lines of code below implement concurrent licensing.
        msg = SecurityBL.GetConcurrentUserLoginLimit(ConnectionStrings("Abacus").ConnectionString, concurrentUserLimit)
        If Not msg.Success Then Throw New ApplicationException(msg.ToString())

        ConcurrentUsersLicence.Startup(ConnectionStrings("Abacus").ConnectionString, _
                                       concurrentUserLimit, _
                                       settings.CurrentApplicationID)

        ModuleLicence.RefreshCacheLicences(ConnectionStrings("Abacus").ConnectionString)

        ' apply Aspose licenses
        Dim asposeWordLic As Aspose.Words.License = New Aspose.Words.License()
        asposeWordLic.SetLicense("Aspose.Total.lic")

    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        'Dim msg As ErrorMessage = Target.Library.Utils.CatchError(Server.GetLastError.GetBaseException(), ErrorMessage.GeneralErrorNumber)
        'If (System.Configuration.ConfigurationSettings.AppSettings("customErrorAutomaticLogging").ToLower) = "on" Then
        '    Target.Library.Web.Utils.WriteErrorToLog(msg, HttpContext.Current.Request.RawUrl)
        'End If
        'If System.Configuration.ConfigurationSettings.AppSettings("customErrorAutomaticEmail").ToLower = "on" Then
        '    Target.Library.Web.Utils.EmailError(msg, HttpContext.Current.Request.RawUrl, "(Error caught in Application_Error() event.)")
        'End If
        'Response.Write("<h2>An unexpected application error occurred.</h2>")
        'Response.Write(String.Format("URL: {0}", HttpContext.Current.Request.RawUrl))
        'Response.Write(msg.ToString().Replace(vbCrLf, "<br>"))
        'Response.End()
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)

        ' stop email sender timer
        emailSenderTimer.Stop()

        ' shutdown concurrent user licence
        ConcurrentUsersLicence.Shutdown()

        Try
            ' log why the application was shutdown
            Dim runtime As HttpRuntime = DirectCast( _
                                            GetType(System.Web.HttpRuntime).InvokeMember("_theRuntime", _
                                                                                BindingFlags.NonPublic Or _
                                                                                BindingFlags.[Static] Or _
                                                                                BindingFlags.GetField, _
                                                                                Nothing, _
                                                                                Nothing, _
                                                                                Nothing),  _
                                            HttpRuntime)

            If runtime Is Nothing Then
                Return
            End If

            Dim shutDownMessage As String = DirectCast( _
                                                runtime.[GetType]().InvokeMember("_shutDownMessage", _
                                                                        BindingFlags.NonPublic Or _
                                                                        BindingFlags.Instance Or _
                                                                        BindingFlags.GetField, _
                                                                        Nothing, _
                                                                        runtime, _
                                                                        Nothing), _
                                                String)

            Dim shutDownStack As String = DirectCast( _
                                            runtime.[GetType]().InvokeMember("_shutDownStack", _
                                                                    BindingFlags.NonPublic Or _
                                                                    BindingFlags.Instance Or _
                                                                    BindingFlags.GetField, _
                                                                    Nothing, _
                                                                    runtime, _
                                                                    Nothing), _
                                            String)

            Dim log As New EventLog()
            log.Source = ".NET Runtime"

            Dim virtualDir As String = AppDomain.CurrentDomain.FriendlyName
            virtualDir = virtualDir.Substring(virtualDir.LastIndexOf("/"))
            virtualDir = virtualDir.Substring(0, virtualDir.IndexOf("-"))

            log.WriteEntry( _
                String.Format("{0}{0}virtualDir={1}{0}shutDownMessage={2}{0}{0}shutDownStack={3}", _
                    vbCrLf, virtualDir, shutDownMessage, shutDownStack), _
                EventLogEntryType.Information _
            )

        Catch
            ' swallow errors
        End Try

    End Sub

#End Region

#Region " Session Events "

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' create an empty custom navigation stack
        Session("CustomNav") = New Stack
        ' default text size
        Session("TextSize") = "0.9"

        ' would prefer to do this in Application_Start, just once for the application
        ' however, session state is not availabe there
        ConcurrentUsersLicence.Timeout = Session.Timeout

        ' development only - auto-logs in
        'Dim msg As Target.Library.ErrorMessage
        'Dim conn As SqlClient.SqlConnection = New SqlClient.SqlConnection(ConnectionStrings("Abacus").ConnectionString)
        'conn.Open()
        'Dim user As Target.Web.Apps.Security.WebSecurityUser = New Target.Web.Apps.Security.WebSecurityUser(conn)
        'msg = user.Fetch(1)
        'If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
        'Session("WebSecurityUser") = user
        'conn.Close()
        ' end development only

        ' load testing only - auto-logs in users from min to max IDs
        'Const MIN_USER_ID As Integer = 2
        'Const MAX_USER_ID As Integer = 51

        'Dim userID As Integer
        'Dim msg As Target.Library.ErrorMessage
        'Dim conn As SqlClient.SqlConnection = New SqlClient.SqlConnection(ConnectionStrings("Abacus").ConnectionString)
        'conn.Open()
        'Dim user As Target.Web.Apps.Security.WebSecurityUser = New Target.Web.Apps.Security.WebSecurityUser(conn)
        'SyncLock Application
        '    If Application("CurrentUserID") Is Nothing Then Application("CurrentUserID") = MIN_USER_ID
        '    If Application("CurrentUserID") > MAX_USER_ID Then Application("CurrentUserID") = MIN_USER_ID
        '    userID = Application("CurrentUserID")
        '    Application("CurrentUserID") += 1
        'End SyncLock
        'msg = user.Fetch(userID)
        'If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
        'Session("WebSecurityUser") = user
        'conn.Close()
        ' end load testing only

    End Sub

#End Region

#Region " OnEmailSenderTimerElasped "

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Called when the email sender timer interval elapses.
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      17/06/2009  D11515 - support currentApp param in call to EmailSenderBL.Send()
    ''' 	[Mikevo]	25/04/2005	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Private Shared Sub OnEmailSenderTimerElasped(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

        Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

        If Convert.ToBoolean(settings("Apps.EmailSender.Enabled")) Then
            Target.Web.Apps.EmailSender.EmailSenderBL.Send(Convert.ToInt32(AppSettings("CurrentApplication")))
            If Convert.ToBoolean(settings("Apps.EmailSender.AutoDeleteSentEnabled")) Then
                Target.Web.Apps.EmailSender.EmailSenderBL.DeleteAgedSentMessages()
            End If
        End If
    End Sub

#End Region

#Region " Global Events "

    Private Sub Global_BeginRequest(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.BeginRequest

        ' MvO 30/07/2008 - this shouldn't now be needed as using the "globalization" web.config element to force the culture

        ' force the current culture to en-GB for when the hosting server regional settings aren't set to UK
        'If CurrentThread.CurrentCulture.Name <> "en-GB" Then CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en-GB")
    End Sub

    Private Sub Global_PreSendRequestHeaders(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreSendRequestHeaders

        ' force all cookies to be in the virtual dir
        For Each cookie As String In Response.Cookies
            Response.Cookies(cookie).Path = Request.ApplicationPath
        Next

    End Sub

    Private Sub Global_PreRequestHandlerExecute(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRequestHandlerExecute
        ' validate the (concurrent) login licence
        Dim msg As ErrorMessage
        Dim currentUser As WebSecurityUser
        ' if we are limiting the number of users...
        If Not HttpContext.Current.Request.Url.IsLoopback() AndAlso ConcurrentUsersLicence.ConcurrentUserLimit > 0 Then
            currentUser = SecurityBL.GetCurrentUser()
            ' ...and the user is logged in...
            If Not currentUser Is Nothing AndAlso currentUser.ID <> 0 Then
                msg = ConcurrentUsersLicence.ValidateLogin(currentUser.ID, HttpContext.Current.Request.Url)
                If Not msg.Success Then
                    If msg.Number = ConcurrentUsersLicence.ERR_LOGIN_TERMINATED Then
                        ConcurrentUsersLicence.RedirectToLicenceError(msg)
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                End If
            End If
        End If
    End Sub

#End Region

End Class
