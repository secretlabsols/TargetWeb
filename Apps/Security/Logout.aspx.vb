
Imports System.Configuration.ConfigurationManager
Imports System.Web.Security
Imports Target.Library
Imports Target.Web.Apps.Navigation
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.Security

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Security.Logout
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page used to log a user out.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      29/08/2006  Moved logout code to SecurityBL.Logout().
    ''' 	[mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class Logout
        Inherits System.Web.UI.Page

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim forceLogOut As Boolean = False
            Dim _connStr As String = ConnectionStrings("Abacus").ConnectionString
            Dim _dbConnection As SqlClient.SqlConnection
            _dbConnection = SqlHelper.GetConnection(_connStr)

            If Not Request.QueryString("forcelogout") Is Nothing Then
                If Target.Library.Utils.ToInt32(Request.QueryString("forcelogout")) = 1 Then
                    forceLogOut = True
                End If
            End If

            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(_connStr, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Dim trans As SqlTransaction = Nothing

            Try
                trans = _dbConnection.BeginTransaction()

                msg = SecurityBL.UpdateWebsecurityUserTableWithLoggedIn(trans)
                If msg.Success Then
                    trans.Commit()
                Else
                    Target.Library.Web.Utils.DisplayError(msg)
                End If

            Catch ex As Exception
                If Not trans Is Nothing Then
                    trans.Rollback()
                    msg = Utils.CatchError(ex, "E5001")
                End If
            Finally

                trans.Dispose()
            End Try



            SecurityBL.Logout(settings, forceLogOut)

            If Not Request.QueryString("timeout") Is Nothing Then
                Response.Redirect("Login.aspx?timeout=1")
            Else
                Response.Redirect("~/Default.aspx")
            End If
        End Sub

    End Class

End Namespace