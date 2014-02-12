
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Library.HttpHandlers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Library.HttpHandlers.GetSystemLogoHandler
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Retrieves the logo stored in the SystemLogo table.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	11/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class GetSystemLogoHandler
        Implements IHttpHandler, System.Web.SessionState.IRequiresSessionState

        Private Shared LOGO_CACHEKEY As String = "Target.Abacus.Extranet.SystemLogo"
        Private Shared LOGO_CACHETIME As Integer = 10

        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest

            Dim conn As SqlConnection = Nothing
            Dim ds As DataSet
            Dim logo As Byte()

            Try
                If HttpRuntime.Cache(LOGO_CACHEKEY) Is Nothing Then
                    ' get logo from database
                    conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                    ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, "SELECT Logo FROM SystemLogo")
                    logo = ds.Tables(0).Rows(0).Item(0)

                    ' insert the list into the cache
                    Target.Library.Web.Utils.PopulateCache(LOGO_CACHEKEY, logo, LOGO_CACHETIME)
                Else
                    logo = DirectCast(HttpRuntime.Cache(LOGO_CACHEKEY), Byte())
                End If

                ' send the response
                With context.Response
                    .Clear()
                    .ContentType = "image/jpg"
                    .AddHeader("Content-Length", logo.Length)
                    .AddHeader("Content-Disposition", "inline; filename=logo.jpg")
                    .BinaryWrite(logo)
                End With

            Catch ex As Exception
                Target.Library.Web.Utils.DisplayError(Target.Library.Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))     ' unexpected error
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

        End Sub

        Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

    End Class

End Namespace
