
Imports System.Configuration.ConfigurationManager
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.IO
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.web
Imports Target.Web.Apps.FileStore
Imports Target.Web.Apps.FileStore.Collections

Namespace Apps.FileStore.HttpHandlers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.FileStore.HttpHandlers.GetFileHandler
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Handler to efficiently retrieve files stored in the file store.
    ''' </summary>
    ''' <remarks>
    '''     <b>For file recognised as images:</b><br />
    '''     Will create WebFileStoreImageVersion records as required when x and y querystring 
    '''     parameters are supplied.<br />
    '''     Will cache served images for the time period specified in the AppSettings key.
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      11/09/2006  SPWeb: extended to allow caller to just specifiy FileDataID if required.
    ''' 	[Mikevo]	23/05/2005	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class GetFileHandler
        Implements IHttpHandler, System.Web.SessionState.IRequiresSessionState

        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest

            Dim fileID As Integer = Target.Library.Utils.ToInt32(context.Request.QueryString("id"))
            Dim saveAs As Integer = Target.Library.Utils.ToInt32(context.Request.QueryString("saveas"))
            Dim width As Integer = Target.Library.Utils.ToInt32(context.Request.QueryString("x"))
            Dim height As Integer = Target.Library.Utils.ToInt32(context.Request.QueryString("y"))
            Dim fileDataID As Integer = Target.Library.Utils.ToInt32(context.Request.QueryString("fileDataID"))
            Dim conn As SqlConnection = Nothing
            Dim trans As SqlTransaction = Nothing
            Dim msg As ErrorMessage = Nothing
            Dim file As WebFileStoreFile
            Dim fileData As WebFileStoreData = Nothing

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                If fileID = 0 AndAlso fileDataID = 0 Then
                    context.Response.Redirect("~/Library/Errors/404.aspx")
                ElseIf fileID > 0 Then
                    ' get the file
                    file = New WebFileStoreFile(conn)
                    msg = file.Fetch(fileID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    If Not file.LoadedFromReader Then context.Response.Redirect("~/Library/Errors/404.aspx")

                    ' if its an image and width and height params have been specified
                    If file.IsImage AndAlso width > 0 AndAlso height > 0 Then
                        ' see if the original has those dimensions
                        If file.Width = width AndAlso file.Height = height Then
                            ' if it does then serve it
                            fileDataID = file.WebFileStoreDataID
                        Else
                            ' otherwise look for an existing image version
                            Dim imageVersions As WebFileStoreImageVersionCollection = Nothing
                            msg = WebFileStoreImageVersion.FetchList(conn, imageVersions, fileID, width, height)
                            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                            ' if we've found a version then serve it (it being the first one, but there shouldn't ever be more than one)
                            If imageVersions.Count > 0 Then
                                fileDataID = imageVersions(0).WebFileStoreDataID
                            Else
                                ' otherwise, create the resized image and serve that
                                ' get the original file data
                                Dim origFileData As WebFileStoreData = New WebFileStoreData(conn)
                                msg = origFileData.Fetch(file.WebFileStoreDataID)
                                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                                ' create the thunbnail
                                Dim newImage As Image = Target.Library.Utils.CreateThumbnailImage(origFileData.Data, width, height)
                                Dim newDataMemStream As MemoryStream = New MemoryStream
                                newImage.Save(newDataMemStream, Imaging.ImageFormat.Jpeg)
                                ' start a transaction
                                trans = SqlHelper.GetTransaction(conn)
                                ' create the new file data
                                Dim newFileData As WebFileStoreData = New WebFileStoreData(trans)
                                With newFileData
                                    .Data = newDataMemStream.ToArray()
                                    .Bytes = newDataMemStream.Length
                                    .MIMEType = "image/jpeg"
                                    .OriginalFilename = origFileData.OriginalFilename
                                    msg = .Save()
                                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                                    fileDataID = .ID
                                End With
                                ' create the image version record
                                Dim imageVersion As WebFileStoreImageVersion = New WebFileStoreImageVersion(trans)
                                With imageVersion
                                    .WebFileStoreFileID = fileID
                                    .WebFileStoreDataID = fileDataID
                                    .Width = width
                                    .Height = height
                                    msg = .Save()
                                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                                End With
                                ' commit the transaction
                                trans.Commit()
                            End If
                        End If
                    Else
                        ' otherwise just serve the original file
                        fileDataID = file.WebFileStoreDataID
                    End If

                    ' get the file data (only cache images)
                    If file.IsImage Then
                        msg = FileStoreBL.GetCachedFileData(conn, fileDataID, fileData)
                        If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    Else
                        fileData = New WebFileStoreData(conn)
                        msg = fileData.Fetch(fileDataID)
                        If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    End If

                Else
                    ' we just have raw data
                    fileData = New WebFileStoreData(conn)
                    msg = fileData.Fetch(fileDataID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                End If

                ' send the response
                With context.Response
                    .Clear()
                    .ContentType = fileData.MIMEType
                    .AddHeader("Content-Length", fileData.Bytes)
                    If saveAs = 1 Then
                        .AddHeader("Content-Disposition", "attachment; filename=" & fileData.OriginalFilename)
                    Else
                        .AddHeader("Content-Disposition", "inline; filename=" & fileData.OriginalFilename)
                    End If
                    .BinaryWrite(fileData.Data)
                End With

            Catch ex As Exception
                Target.Library.Web.Utils.DisplayError(Target.Library.Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))     ' unexpected error
            Finally
                SqlHelper.RollbackTransaction(trans)
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