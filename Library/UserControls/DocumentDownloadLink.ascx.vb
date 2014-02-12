Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Configuration.ConfigurationManager

Namespace Library.UserControls

    ''' <summary>
    ''' User Control providing link with file extension icon to download file
    ''' If file does not have a file type icon configured then a 'generic' icon is used
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Partial Public Class DocumentDownloadLink
        Inherits System.Web.UI.UserControl

#Region " Constants "

        Private Const IMAGE_DIR As String = "~/Images/FileTypes/"
        Private Const GENERIC_IMAGE As String = "~/Images/FileTypes/image.gif"
        Private Const DOWNLOAD_HANDLER As String = "Documents/DocumentDownloadHandler.axd"
        Private Const ALLOWED_HEIGHT As Integer = 20

#End Region

#Region " Private Properties "

        Private _conn As SqlConnection = Nothing
        Private _trans As SqlTransaction = Nothing

        Private ReadOnly Property conn() As SqlConnection
            Get
                If _conn Is Nothing Then
                    _conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                End If

                Return _conn
            End Get
        End Property

        Private ReadOnly Property trans() As SqlTransaction
            Get
                If _trans Is Nothing Then
                    _trans = conn.BeginTransaction
                End If

                Return _trans
            End Get
        End Property

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Method to Prime the control with File/docuemnt's 'Document.ID'
        ''' </summary>
        ''' <param name="documentID"></param>
        ''' <remarks></remarks>
        ''' <history>
        ''' IHS  11/03/2011  D11915 - added comments
        ''' </history>
        Public Sub InitControl(ByVal documentID As Integer)
            Dim fileName As String = Nothing
            Dim imageLocation As String = Nothing
            Dim objDocumentFileBase As DocumentFileBase = Nothing
            Dim objDocument As Document = Nothing

            Try
                objDocument = New Document(trans)
                objDocument.Fetch(documentID)

                objDocumentFileBase = DocumentFactory.GetFileObject(trans, objDocument.DocumentRepositoryID, documentID)

                imageLocation = GetImagePath(objDocumentFileBase.Document.MimeTypeID)

                SetImageAttributes(objDocumentFileBase.Document.Filename, imageLocation)

                SetLinkAttributes(objDocumentFileBase.Document.Filename, documentID)
            Catch ex As Exception
                Throw
            Finally
                If Not _trans Is Nothing Then
                    _trans.Commit()
                    _trans = Nothing
                End If
            End Try
        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Returns file icon based on file extension/Mime type
        ''' If no file icon is defined then generic file icon is returned
        ''' </summary>
        ''' <param name="mimeTypeID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' IHS  11/03/2011  D11915 - added comments
        ''' </history>
        Private Function GetImagePath(ByVal mimeTypeID As Integer) As String
            Dim mimeType As MIMEType = Nothing, imagePath As String = String.Empty, msg As New ErrorMessage

            mimeType = New MIMEType(trans)
            msg = mimeType.Fetch(mimeTypeID)

            If Not msg.Success Then Return String.Empty

            If String.IsNullOrEmpty(mimeType.IconFile) Then Return GENERIC_IMAGE

            imagePath = String.Format("{0}{1}", IMAGE_DIR, mimeType.IconFile)

            Return imagePath
        End Function

        Private Sub SetImageAttributes(ByVal fileName As String, ByVal imageLocation As String)
            imgFileType.Alt = fileName
            imgFileType.Src = imageLocation

            ResetImageSize()
        End Sub

        ''' <summary>
        ''' Sets image size based on ALLOWED_HEIGHT value and width is calculated based on ratio
        ''' </summary>
        ''' <remarks></remarks>
        ''' <history>
        ''' IHS  11/03/2011  D11915 - added comments
        ''' </history>
        Private Sub ResetImageSize()
            Dim img As System.Drawing.Image
            img = System.Drawing.Image.FromFile(Server.MapPath(imgFileType.Src))

            Dim aspectRatio As Decimal

            If img.Height <= ALLOWED_HEIGHT Then Return

            aspectRatio = img.Width / img.Height

            imgFileType.Height = ALLOWED_HEIGHT
            imgFileType.Width = aspectRatio * ALLOWED_HEIGHT
        End Sub

        Private Sub SetLinkAttributes(ByVal fileName As String, ByVal documentID As Integer)
            litFilename.Text = fileName
            lnkFileDownload.Title = String.Format("Click to download ""{0}""", fileName)
            lnkFileDownload.HRef = String.Format("{0}?id={1}&saveas=1", DOWNLOAD_HANDLER, documentID)
        End Sub

#End Region

    End Class

End Namespace