
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Web.Apps.Security.PasswordPolicyChecker
Imports System.Collections.Generic

Namespace Apps.Security

    ''' <summary>
    ''' Implements custom security for Abacus Intranet.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     04/02/2011  D11934 Password Maintenance
    '''     MikeVO      13/07/2010  Added GetCompilerVersion().
    '''     MoTahir     28/06/2010  D11829 - Licensing
    '''     MikeVO      06/04/2009  D11539 - support for Abacus Extranet changes in ConcurrentUsersLicence.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Public Class IntranetSecurity
        Implements ICustomSecurity, IReplacementLogin

        Public Const APPS_SECURITY_PASSWORD_EXPIRYDAYS As String = "Apps.Security.Password.ExpiryDays"


#Region " ICustomSecurity implementation "

        Public ReadOnly Property ShowPreventSameLoginDifferentLocation() As Boolean Implements ICustomSecurity.ShowPreventSameLoginDifferentLocation
            Get
                Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                If settings.CurrentApplicationID = ApplicationName.AbacusIntranet Then Return False
            End Get
        End Property

        Public Function AfterActivateUser(ByVal webSecurityUserID As Integer) As ErrorMessage Implements ICustomSecurity.AfterActivateUser
            Dim msg As ErrorMessage = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function

        Public Function AfterCloneUser(ByVal trans As System.Data.SqlClient.SqlTransaction, ByVal clonedUserID As Integer, ByVal applicationID As ApplicationName) As ErrorMessage Implements ICustomSecurity.AfterCloneUser
            Dim msg As ErrorMessage = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function

        Public Function AfterLogin(ByVal connectionString As String, ByVal externalUserID As Integer) As ErrorMessage Implements ICustomSecurity.AfterLogin
            Dim msg As ErrorMessage = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function

        Public Sub AfterLogout(ByVal connectionString As String) Implements ICustomSecurity.AfterLogout
        End Sub

        Public Function GetVersionInformation(ByVal conn As System.Data.SqlClient.SqlConnection, ByRef appInfo As ApplicationInfo) As ErrorMessage Implements ICustomSecurity.GetVersionInformation

            Dim msg As ErrorMessage = New ErrorMessage
            Dim section As ApplicationInfoSection
            Dim assem As AssemblyName
            Dim folder As String
            Dim files As ArrayList
            Dim sysInfo As SystemInfoCollection = Nothing
            Dim myFile As String
            Dim myFolder As String

            appInfo = New ApplicationInfo

            Try
                ' DATABASE
                msg = SystemInfo.FetchList(conn, sysInfo)
                If Not msg.Success Then Return msg
                section = appInfo.AddSection("Database")
                section.AddDetail("Version", sysInfo(0).CurrentBuild)

                ' FRAMEWORK
                section = appInfo.AddSection(".NET Framework")
                section.AddDetail("Version", Utils.GetFrameworkVersion())
                section.AddDetail("Compiler", GetCompilerVersion(".vb"))

                ' ASSEMBLIES
                section = appInfo.AddSection("Assemblies")

                ' get the bin folder path from the current assembly
                assem = [Assembly].GetExecutingAssembly().GetName()
                folder = Path.GetDirectoryName(New Uri(assem.CodeBase & "").LocalPath)

                'get all bin dll files
                files = New ArrayList

                For Each myFile In Directory.GetFiles(folder, "*.dll")
                    files.Add(myFile)
                Next

                folder = Utils.Strip(folder, "\bin")
                'get remaining files from specified folders
                For Each myFolder In Directory.GetDirectories(folder, "Target.*")
                    For Each myFile In Directory.GetFiles(myFolder, "*.dll")
                        files.Add(myFile)
                    Next
                Next

                ' add detail for each of the files we are after
                For Each file As String In files
                    section.AddDetail(Path.GetFileName(file), FileVersionInfo.GetVersionInfo(file).ProductVersion)
                Next

                section.Details.Sort()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected error
            End Try

            Return msg

        End Function

        Private Function GetCompilerVersion(ByVal extension As String) As String

            GetCompilerVersion = "Not available"

            Dim result As String = String.Empty

            Try
                Dim rootWebConfig As System.Configuration.Configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/")
                Dim codeDomSection As DefaultSection = rootWebConfig.GetSection("system.codedom")
                Dim codeDomSectionXml As String = codeDomSection.SectionInformation.GetRawXml()
                Dim codeDomXmlDoc As XmlDocument = New XmlDocument()
                Dim xPath As String

                With codeDomXmlDoc
                    .LoadXml(codeDomSectionXml)
                    xPath = String.Format( _
                        "system.codedom/compilers/compiler[@extension = '{0}']/providerOption[@name = 'CompilerVersion']/@value", _
                        extension _
                    )
                    result = .SelectSingleNode(xPath).Value
                End With

            Catch ex As Exception
                ' swallow
            End Try

            Return result

        End Function

        Public Function IsOffline(ByVal connectionString As String) As Boolean Implements ICustomSecurity.IsOffline

            Const SP_NAME As String = "pr_GetIsOffline"

            Dim conn As SqlConnection = Nothing
            Dim params As SqlParameter()

            Try
                conn = SqlHelper.GetConnection(connectionString)
                ' make sure we include the return value in the parameter set
                params = SqlHelperParameterCache.GetSpParameterSet(conn, SP_NAME, True)
                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, SP_NAME, params)
                Return Convert.ToBoolean(params(0).Value)
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

        End Function

        Public Function GetConcurrentUserLoginLimit(ByVal connString As String, ByRef limit As Integer) As ErrorMessage Implements ICustomSecurity.GetConcurrentUserLoginLimit

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim sysInfos As SystemInfoCollection = Nothing
            Dim encryptedLicence As Integer
            Dim errorStr As String = String.Empty

            Try
                conn = SqlHelper.GetConnection(connString)

                msg = SystemInfo.FetchList(conn, sysInfos)
                If Not msg.Success Then Return msg

                ' get the encrypted licence count
                encryptedLicence = sysInfos(0).LicencedUsers

                ' decrypt it
                If Not Utils.AbacusDecryptLicenceCount(encryptedLicence, limit, errorStr) Then
                    Throw New ApplicationException(errorStr)
                Else
                    msg = New ErrorMessage()
                    msg.Success = True
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

        Public Function IsCouncilUser(ByVal conn As System.Data.SqlClient.SqlConnection, _
                                      ByVal externalUserID As Integer) As Boolean Implements ICustomSecurity.IsCouncilUser
            Return True
        End Function

#End Region

#Region " IReplacementLogin implementation "

        Public ReadOnly Property PageOverviewText() As String Implements IReplacementLogin.PageOverviewText
            Get
                Return "Please enter your Abacus username and password to log in."
            End Get
        End Property

        Public ReadOnly Property EmailAddressLabel() As String Implements IReplacementLogin.EmailAddressLabel
            Get
                Return "Username"
            End Get
        End Property

        Public ReadOnly Property EmailAddressUpperCase() As Boolean Implements IReplacementLogin.EmailAddressUpperCase
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property PasswordRequired() As Boolean Implements IReplacementLogin.PasswordRequired
            Get
                Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                If settings.CurrentApplicationID = ApplicationName.AbacusIntranet Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property

        Public Function Login(ByVal connString As String, _
                              ByVal username As String, _
                              ByVal password As String, _
                              ByVal application As ApplicationName, _
                              ByVal forceLogin As Boolean) As ErrorMessage Implements IReplacementLogin.Login

            Dim msg As ErrorMessage
            Dim userList As Target.Abacus.Library.DataClasses.Collections.UsersCollection = Nothing
            Dim user As Target.Abacus.Library.DataClasses.Users = Nothing
            Dim conn As SqlConnection = Nothing
            Dim webUser As WebSecurityUser = Nothing
            Dim webUsers As WebSecurityUserCollection = Nothing
            Dim loggedIn As Boolean
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
            Dim passwordExpiryDays As Integer = Convert.ToInt32(settings(PasswordPolicyManager.APPS_SECURITY_PASSWORD_EXPIRYDAYS))

            Try
                conn = SqlHelper.GetConnection(connString)

                ' is the user already logged in?
                If SecurityBL.IsUserLoggedOn() Then
                    msg = New ErrorMessage
                    msg.Success = True
                    Return msg
                End If

                ' check username was input
                If username Is Nothing OrElse username.Trim().Length = 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If
                ' password can be blank
                If password Is Nothing Then password = String.Empty

                ' get users with user name
                msg = Target.Abacus.Library.DataClasses.Users.FetchList(conn, userList, username)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

                ' if we have none returned then login failed
                If userList.Count = 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If

                ' find the first user with the specified username (there should only ever be one anyway)
                For Each u As Target.Abacus.Library.DataClasses.Users In userList
                    If u.Name.ToUpper() = username.ToUpper() Then
                        user = u
                        Exit For
                    End If
                Next

                ' check password
                'If Utils.AbacusEncrypt(True, password.ToUpper()) <> user.Password Then
                '    msg = New ErrorMessage
                '    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                '    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                '    Return msg
                'End If

                ' username and password are good

                ' is the account blocked?
                If user.BlockLogin Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_ACCOUNT_SUSPENDED)
                    Return msg
                End If

                ' the account is good

                ' find web login
                ' see if any users with the specified username for the specified application
                msg = WebSecurityUser.FetchList(conn, webUsers, , , username, , , , Convert.ToByte(application))
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

                ' if we have none returned then login failed
                If webUsers.Count = 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If

                ' find the first user with the specified username (there should only ever be one anyway)
                For Each u As WebSecurityUser In webUsers
                    If u.Email.ToUpper() = username.ToUpper() Then
                        webUser = u
                        Exit For
                    End If
                Next

                '' if forceLogin is true , user wants to login anyway, and want to reject existing session
                If Not forceLogin Then
                    ' is the user logged in?
                    msg = ConcurrentUsersLicence.IsUserLoggedIn(webUser.ID, loggedIn)
                    If Not msg.Success Then Return msg

                    ' if the user is already logged in and they are returning from the same IP address,
                    ' force a logout of the previous login and then proceeed as normal
                    ' (if the IP address is different, then the "RegisterLogin()" call below will sort them out)
                    If loggedIn And webUser.LastLoginIPAddress = HttpContext.Current.Request.UserHostAddress Then
                        ' unregister the concurrent user licence
                        ConcurrentUsersLicence.UnregisterLogin(webUser.ID)
                    End If

                    ' register the concurrent user licence
                    msg = ConcurrentUsersLicence.RegisterLogin(webUser, True)
                    If Not msg.Success Then Return msg
                End If

                ' save login details to user record
                With webUser
                    ' set user connection
                    .DbConnection = conn
                    ' the RegisterLogin() method updates the WebSecurityUser record so re-fetch the record
                    msg = .Fetch(webUser.ID)
                    If Not msg.Success Then Return msg
                    ' update
                    .LastLoginUserAgent = HttpContext.Current.Request.UserAgent
                    .LastLoginIPAddress = HttpContext.Current.Request.UserHostAddress
                    .LastLoginDate = DateTime.Now
                    'check the password expiry
                    If Not user.LastPasswordChange.ToString = String.Empty And passwordExpiryDays <> 0 Then
                        ' force user to change password
                        If user.LastPasswordChange.AddDays(passwordExpiryDays) < DateTime.Now Then .Status = WebSecurityUserStatus.ChangePassword
                    End If
                    msg = .Save()
                    If Not msg.Success Then Return msg
                End With

                ' put user info into session
                HttpContext.Current.Session(SecurityBL.SESSION_WEBSECURITYUSER) = webUser
                SecurityBL.StoreAnyAppLoginID(webUser.ID)

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

        Public Function ValidatePassword(ByVal WebSecurityId As Integer, ByVal password As String) As ErrorMessage Implements IReplacementLogin.ValidatePassword
            Dim msg As ErrorMessage
            Dim user As Target.Abacus.Library.DataClasses.Users = Nothing
            Dim conn As SqlConnection = Nothing
            Dim webUser As WebSecurityUser = Nothing
            Dim externaluserID As Integer
            Dim connString As String

            connString = ConnectionStrings("Abacus").ConnectionString

            Try
                conn = SqlHelper.GetConnection(connString)

                ' get externaluserid
                webUser = New WebSecurityUser(conn)
                With webUser
                    msg = .Fetch(WebSecurityId)
                    If Not msg.Success Then Return msg
                    externaluserID = webUser.ExternalUserID
                End With

                ' check username was input
                If externaluserID = 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If
                ' password can be blank
                If password Is Nothing Then password = String.Empty

                ' get users with externalUserID
                user = New Target.Abacus.Library.DataClasses.Users(conn:=conn)
                With user
                    msg = .Fetch(externaluserID)
                    If Not msg.Success Then Return msg
                End With

                ' check password
                If Utils.AbacusEncrypt(True, password.ToUpper()) <> user.Password Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If

                ' username and password are good

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg
        End Function

        Public Function AfterPasswordChange(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, ByVal externaluserid As Integer, ByVal WebSecurityId As Integer, ByVal password As String) As ErrorMessage Implements IReplacementLogin.AfterPasswordChanged
            Dim msg As ErrorMessage
            Dim user As Target.Abacus.Library.DataClasses.Users = Nothing
            Dim webUser As WebSecurityUser = Nothing
            Dim webUserId As Integer = 0

            'get webuserid
            webUser = SecurityBL.GetCurrentUser
            webUserId = webUser.ID

            Try
                ' check existance of external userid
                If externaluserid = 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If
                ' password can be blank
                If password Is Nothing Then password = String.Empty

                ' fetch user details
                If Not trans Is Nothing Then
                    user = New Target.Abacus.Library.DataClasses.Users(trans)
                Else
                    user = New Target.Abacus.Library.DataClasses.Users(conn)
                End If

                ' A4W user save password
                With user
                    msg = .Fetch(externaluserid)
                    If Not msg.Success Then Return msg
                    .Password = Utils.AbacusEncrypt(True, password.ToUpper())
                    .LastPasswordChange = DateTime.Now
                    msg = .Save()
                    If Not msg.Success Then Return msg
                End With

                ' fetch user details
                If Not trans Is Nothing Then
                    webUser = New WebSecurityUser(trans)
                Else
                    webUser = New WebSecurityUser(conn)
                End If

                ' Websecurity user update status
                If webUserId > 0 Then
                    With webUser
                        msg = .Fetch(webUserId)
                        If Not msg.Success Then Return msg
                        If webUser.Status = WebSecurityUserStatus.ChangePassword Then webUser.Status = WebSecurityUserStatus.Active
                        msg = .Save()
                        If Not msg.Success Then Return msg
                    End With
                End If

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            End Try

            Return msg
        End Function

        Public Function GetEmailAddress(ByVal webUser As WebSecurityUser, ByRef email As String) As ErrorMessage Implements IReplacementLogin.GetEmailAddress
            Dim msg As ErrorMessage
            Dim user As Target.Abacus.Library.DataClasses.Users = Nothing
            Dim conn As SqlConnection = Nothing
            Dim connString As String
            Dim application As ApplicationName
            Dim userList As Target.Abacus.Library.DataClasses.Collections.UsersCollection = Nothing

            connString = ConnectionStrings("Abacus").ConnectionString
            application = ApplicationName.AbacusIntranet

            Try
                conn = SqlHelper.GetConnection(connString)

                ' check existence of externaluserid
                If webUser.ExternalUserID = 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If

                ' get user
                user = New Target.Abacus.Library.DataClasses.Users(conn:=conn)
                With user
                    msg = .Fetch(webUser.ExternalUserID)
                    If Not msg.Success Then Return msg
                    ' get the email address
                    email = user.EMail
                End With

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

        Public Function SaveEmailAddress(ByVal webSecurityUserId As Integer, ByVal email As String) As ErrorMessage Implements IReplacementLogin.SaveEmailAddress
            Dim msg As ErrorMessage
            Dim user As Target.Abacus.Library.DataClasses.Users = Nothing
            Dim conn As SqlConnection = Nothing
            Dim webUser As WebSecurityUser = Nothing
            Dim connString As String
            Dim application As ApplicationName
            Dim externalUserID As Integer
            Dim userList As Target.Abacus.Library.DataClasses.Collections.UsersCollection = Nothing

            connString = ConnectionStrings("Abacus").ConnectionString
            application = ApplicationName.AbacusIntranet

            Try
                conn = SqlHelper.GetConnection(connString)

                ' get externaluserID
                webUser = New WebSecurityUser(conn)
                With webUser
                    msg = .Fetch(webSecurityUserId)
                    If Not msg.Success Then Return msg
                    externalUserID = webUser.ExternalUserID
                End With

                ' check for existence of externaluserID 
                If externalUserID = 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If

                ' get user
                user = New Target.Abacus.Library.DataClasses.Users(conn:=conn)
                With user
                    msg = .Fetch(externalUserID)
                    If Not msg.Success Then Return msg
                    ' save the email address
                    .EMail = email
                    msg = .Save
                    If Not msg.Success Then Return msg
                End With

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

        Public Function EmailEmailAddressInUse(ByVal webSecurityUserId As Integer, ByVal email As String) As ErrorMessage Implements IReplacementLogin.EmailAddressInUse
            Dim msg As ErrorMessage
            Dim user As Target.Abacus.Library.DataClasses.Users = Nothing
            Dim conn As SqlConnection = Nothing
            Dim webUser As WebSecurityUser = Nothing
            Dim connString As String
            Dim application As ApplicationName
            Dim externalUserID As Integer
            Dim userList As Target.Abacus.Library.DataClasses.Collections.UsersCollection = Nothing

            connString = ConnectionStrings("Abacus").ConnectionString
            application = ApplicationName.AbacusIntranet

            Try
                conn = SqlHelper.GetConnection(connString)

                ' get externaluserID
                webUser = New WebSecurityUser(conn)
                With webUser
                    msg = .Fetch(webSecurityUserId)
                    If Not msg.Success Then Return msg
                    externalUserID = webUser.ExternalUserID
                End With

                ' check for existence of externaluserID 
                If externalUserID = 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If

                'get user
                user = New Target.Abacus.Library.DataClasses.Users(conn:=conn)
                With user
                    msg = .Fetch(externalUserID)
                    If Not msg.Success Then Return msg
                End With

                ' get all users
                msg = Target.Abacus.Library.DataClasses.Users.FetchList(conn, userList)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

                ' if we have none returned then no matches on email
                If userList.Count = 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.LOGIN_DETAILS_INVALID)
                    Return msg
                End If

                ' find other users that might have the same email address
                Dim u As List(Of Target.Abacus.Library.DataClasses.Users) = (From us In userList.ToArray _
                        Where us.EMail.ToUpper = email.ToUpper _
                        And us.ID <> user.ID).ToList

                If u.count > 0 Then
                    msg = New ErrorMessage
                    msg.Number = SecurityBL.LOGIN_ERROR_NUMBER
                    msg.Message = String.Format(msg.Message, SecurityBL.EMAIL_DETAILS_INVALID)
                End If

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

    End Class

End Namespace

