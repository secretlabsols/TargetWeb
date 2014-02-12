
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.SP.Library
Imports Target.SP.Library.Collections
Imports Microsoft.Win32
Imports Microsoft.Win32.Registry

Namespace Apps.Security

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.SPSecurity
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class to implement the SP custom security interface.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      24/10/2007  Added framework version to version info.
    '''     MikeVO      02/11/2006  Added CloneUser().
    ''' 	[Mikevo]	29/06/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class SPSecurity
        Implements ICustomSecurity

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

        Public Function AfterLogin(ByVal connectionString As String, ByVal externalUserID As Integer) As Target.Library.ErrorMessage Implements ICustomSecurity.AfterLogin
            Return SPClassesBL.ValidateLogin(connectionString, externalUserID)
        End Function

        Public Sub AfterLogout(ByVal connectionString As String) Implements ICustomSecurity.AfterLogout
            ' not implemented
        End Sub

        Public Function AfterActivateUser(ByVal webSecurityUserID As Integer) As ErrorMessage Implements ICustomSecurity.AfterActivateUser

            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Removes the "SP Primary Basic User" role from the cloned user.
        ''' </summary>
        ''' <param name="trans">An already open database transaction.</param>
        ''' <param name="clonedUserID">The ID of the cloned user.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	02/11/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Function AfterCloneUser(ByVal trans As SqlTransaction, ByVal clonedUserID As Integer) As ErrorMessage Implements ICustomSecurity.AfterCloneUser

            Const ROLE_NAME As String = "SP Primary Basic User"

            Dim msg As ErrorMessage
            Dim roles As WebSecurityRoleCollection = Nothing

            ' get the role we want to remove
            msg = WebSecurityRole.FetchList(trans, roles, ROLE_NAME)
            If Not msg.Success Then Return msg

            If roles.Count <> 1 Then
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
        '''     Retrieves version information for the SP Web application.
        ''' </summary>
        ''' <param name="appInfo">Upon success, contains file name and version information.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	22/03/2007	Created
        ''' ]]></history>
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
                files.Add("ErrorClass.dll")
                files.Add("SPClasses.dll")
                files.Add("SPFunctions.dll")
                files.Add("Target.Library.dll")
                files.Add("Target.Library.Web.dll")
                files.Add("Target.SP.Library.dll")
                files.Add("Target.SP.Web.dll")
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
                section.AddDetail("Version", sysInfo(0).SPCurrentBuild)

                ' FRAMEWORK
                section = appInfo.AddSection(".NET Framework")
                section.AddDetail("Version", Utils.GetFrameworkVersion())

                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0001")     ' unexpected error
            End Try

            Return msg

        End Function

        Public Function GetConcurrentUserLoginLimit(ByVal connString As String, ByRef limit As Integer) As ErrorMessage Implements ICustomSecurity.GetConcurrentUserLoginLimit
            limit = 0
            Dim msg As ErrorMessage = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function

    End Class

End Namespace