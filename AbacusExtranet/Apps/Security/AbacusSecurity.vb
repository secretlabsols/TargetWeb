
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports Target.Library
Imports Target.Library.Collections
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports System.Configuration.ConfigurationManager

Namespace Apps.Security

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Apps.Security.AbacusSecurity
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class to implement the Abacus custom security interface.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MoTahir     04/02/2011  D11934 Password Maintenance
    '''     MikeVO      06/04/2009  D11539 - support for concurrent licensing in Abacus Extranet.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	03/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AbacusSecurity
        Implements ICustomSecurity

        Public ReadOnly Property ShowPreventSameLoginDifferentLocation() As Boolean Implements ICustomSecurity.ShowPreventSameLoginDifferentLocation
            Get
                Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                If settings.CurrentApplicationID = ApplicationName.AbacusExtranet Then Return True
            End Get
        End Property

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

            Return False

        End Function

        Public Function AfterLogin(ByVal connectionString As String, ByVal externalUserID As Integer) As Target.Library.ErrorMessage Implements ICustomSecurity.AfterLogin
            Return AbacusClassesBL.ValidateLogin(connectionString, externalUserID)
        End Function

        Public Sub Logout(ByVal connectionString As String) Implements ICustomSecurity.AfterLogout
            ' not implemented
        End Sub

        Public Function AfterActivateUser(ByVal webSecurityUserID As Integer) As ErrorMessage Implements ICustomSecurity.AfterActivateUser
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Removes the "Abacus Extranet Primary Basic User" role from the cloned user.
        ''' </summary>
        ''' <param name="trans">An already open database transaction.</param>
        ''' <param name="clonedUserID">The ID of the cloned user.</param>
        ''' <param name="applicationID">The current application.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     MikeVO      01/12/2008  D11444 - security overhaul.
        ''' 	[Mikevo]	04/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function AfterCloneUser(ByVal trans As SqlTransaction, ByVal clonedUserID As Integer, ByVal applicationID As ApplicationName) As ErrorMessage Implements ICustomSecurity.AfterCloneUser

            Const ROLE_NAME As String = "Primary Provider Extranet User"

            Dim msg As ErrorMessage
            Dim roles As WebSecurityRoleCollection = Nothing

            ' get the role we want to remove
            msg = WebSecurityRole.FetchList(trans, roles, String.Empty, String.Empty, ROLE_NAME, applicationID)
            If Not msg.Success Then Return msg

            If roles.Count > 1 Then
                msg = New ErrorMessage
                msg.Number = "E2001"
                msg.Message = String.Format(msg.Message, ROLE_NAME, roles.Count)
                Return msg
            Else
                ' delete the role from the cloned user
                msg = WebSecurityUser_WebSecurityRole.Delete(trans, clonedUserID, roles(0).ID)
                If Not msg.Success Then Return msg
            End If

            msg = New ErrorMessage
            msg.Success = True
            Return msg

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves version information for the Abacus Extranet application.
        ''' </summary>
        ''' <param name="appInfo">Upon success, contains file name and version information.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	03/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetVersionInformation(ByVal conn As SqlConnection, ByRef appInfo As ApplicationInfo) As ErrorMessage Implements ICustomSecurity.GetVersionInformation

            Dim msg As ErrorMessage = New ErrorMessage
            Dim section As ApplicationInfoSection
            Dim assem As AssemblyName
            Dim folder As String
            Dim files As ArrayList
            Dim sysInfo As SystemInfoCollection = Nothing

            appInfo = New ApplicationInfo

            Try
                ' ASSEMBLIES
                section = appInfo.AddSection("Assemblies")

                ' build a list of assemblies we are after
                files = New ArrayList
                files.Add("Target.Abacus.Extranet.dll")
                files.Add("Target.Abacus.Library.dll")
                files.Add("Target.Library.dll")
                files.Add("Target.Library.Web.dll")
                files.Add("Target.Web.Apps.dll")
                files.Add("Target.Web.dll")

                ' get the bin folder path from the current assembly
                assem = [Assembly].GetExecutingAssembly().GetName()
                folder = Path.GetDirectoryName(New Uri(assem.CodeBase & "").LocalPath)

                ' add detail for each of the files we are after
                For Each file As String In files
                    section.AddDetail(file, FileVersionInfo.GetVersionInfo(Path.Combine(folder, file)).ProductVersion)
                Next

                ' DATABASE
                msg = SystemInfo.FetchList(conn, sysInfo)
                If Not msg.Success Then Return msg
                section = appInfo.AddSection("Database")
                section.AddDetail("Version", sysInfo(0).CurrentBuild)

                ' FRAMEWORK
                section = appInfo.AddSection(".NET Framework")
                section.AddDetail("Version", Utils.GetFrameworkVersion())

                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected error
            End Try

            Return msg

        End Function

        Public Function GetConcurrentUserLoginLimit(ByVal connString As String, ByRef limit As Integer) As ErrorMessage Implements ICustomSecurity.GetConcurrentUserLoginLimit

            Dim msg As ErrorMessage
            Dim accountSettings As ApplicationSettingCollection = Nothing
            Dim conn As SqlConnection = Nothing

            Try
                conn = SqlHelper.GetConnection(connString)

                msg = ApplicationSetting.FetchList(conn:=conn, _
                                                    list:=accountSettings, _
                                                    applicationID:=ApplicationName.AbacusExtranet, _
                                                    settingKey:="Apps.Security.PreventMultipleLogins", _
                                                    auditLogTitle:=String.Empty, _
                                                    auditUserName:=String.Empty _
                                                    )
                If Not msg.Success Then Return msg

                If accountSettings(0).SettingValue = Boolean.TrueString Then
                    limit = Int32.MaxValue
                Else
                    limit = 0
                End If

            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg

        End Function

        Public Function IsCouncilUser(ByVal conn As System.Data.SqlClient.SqlConnection, _
                              ByVal externalUserID As Integer) As Boolean Implements ICustomSecurity.IsCouncilUser

            Dim msg As ErrorMessage
            Dim u As Target.Abacus.Library.DataClasses.Users
            Dim result As Boolean

            u = New Target.Abacus.Library.DataClasses.Users(conn)
            msg = u.Fetch(externalUserID)
            If Not msg.Success Then
                Throw New ApplicationException(msg.ToString())
            End If

            If u.AbacusExtranetUser = AbacusExtranetUser.AbacusExtranetAdminUser Then
                result = True
            End If

            Return result

        End Function

    End Class

End Namespace