Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library
Imports System.Reflection

Namespace Apps.Documents

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.Documents.DocumentDownloadHandler
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     HTTP handler to download files from a specific repository.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	Iftikhar	10/02/2011	D11915 - Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DocumentDownloadHandler
        Implements IHttpHandler, System.Web.SessionState.IReadOnlySessionState

        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest

            Dim documentID As Integer = Utils.ToInt32(context.Request.QueryString("id"))
            Dim saveAs As Integer = Utils.ToInt32(context.Request.QueryString("saveas"))

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing

            Dim objDocument As Document
            Dim objDocumentFileBase As DocumentFileBase
            Dim objDocumentDownload As DocumentForDownload

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then context.Response.End()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                objDocument = New Document(conn)
                msg = objDocument.Fetch(documentID)
                If Not msg.Success Then Throw New ApplicationException(msg.ToString())

                objDocumentFileBase = DocumentFactory.GetFileObject(conn, objDocument.DocumentRepositoryID, documentID)

                objDocumentDownload = objDocumentFileBase.GetForDownload()

                ' send the response
                With context.Response
                    .Clear()
                    .ContentType = objDocumentDownload.MimeType
                    .AddHeader("Content-Length", objDocumentDownload.Size)
                    If saveAs = 1 Then
                        .AddHeader("Content-Disposition", "attachment; filename=" & objDocumentDownload.Filename)
                    Else
                        .AddHeader("Content-Disposition", "inline; filename=" & objDocumentDownload.Filename)
                    End If
                    .BinaryWrite(objDocumentDownload.Data)
                End With

            Catch ex As Exception
                Target.Library.Web.Utils.DisplayError(Utils.CatchError(ex, "E0001"))     ' unexpected error
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