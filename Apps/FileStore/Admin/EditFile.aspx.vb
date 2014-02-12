
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Utils
Imports Target.Web.Apps.Security

Namespace Apps.FileStore.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.FileStore.Admin.EditFile
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows users to upload files directly to the file store.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	19/09/2006	Changed to support SlickUpload.
    '''     MikeVO      ??/??/????  Created.
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditFile
        Inherits Target.Web.Apps.BasePage

        Protected fileID As Integer

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: File Store: Edit File")
            Me.Form.Enctype = "multipart/form-data"

            fileID = Target.Library.Utils.ToInt32(Request.QueryString("fileID"))
            Dim file As WebFileStoreFile
            Dim msg As ErrorMessage

            btnView.Attributes.Add("onclick", String.Format("window.open('../FileStoreGetFile.axd?id={0}')", fileID))
            btnDownload.Attributes.Add("onclick", String.Format("window.open('../FileStoreGetFile.axd?id={0}&saveas=1')", fileID))
            imgBusy.Src = Target.Library.Web.Utils.GetVirtualPath("Images/busy.gif")

            If Not IsPostBack Then

                If fileID = 0 Then
                    grpLegend.InnerText = "Upload New File"
                    litPageOverview.Text = "Click ""Browse..."" to search for and select the file to upload.  Then enter a description and click ""Submit"" to upload the file."
                Else
                    grpLegend.InnerText = "Edit Existing File"
                    litPageOverview.Text = "To replace the existing file, click ""Browse..."" to search for and select the new file to upload.  To just change the description and leave the file as-is, simply enter the new description and click ""Submit""."
                    Me.reqNewFile.Enabled = False
                    '  get the existing file description
                    file = New WebFileStoreFile(Me.DbConnection)
                    msg = file.Fetch(fileID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    txtDescription.Text = file.Description
                    ' enable view/download buttons
                    Me.ClientScript.RegisterStartupScript(Me.GetType(), "", WrapClientScript("GetElement('btnView').disabled=false;GetElement('btnDownload').disabled=false;"))
                End If
            Else
                ' on submission
                Dim newFileID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("fileID"))
                Dim tempFileIDs As Integer() = FileStoreBL.GetUploadedTempFileIDs()

                If newFileID = 0 Then
                    msg = FileStoreBL.NewFile(Me.DbConnection, newFileID, tempFileIDs, txtDescription.Text, Request.QueryString("folderID"), SecurityBL.GetCurrentUser().ID)
                Else
                    msg = FileStoreBL.UpdateFile(Me.DbConnection, newFileID, tempFileIDs, txtDescription.Text)
                End If

                If msg.Success Then
                    lblErrorMsg.Text = ""
                    Me.ClientScript.RegisterStartupScript(Me.GetType(), "", WrapClientScript("parentWindow.HideModalDIV();parentWindow.EditFileComplete(" & newFileID & ");window.parent.close();"))
                Else
                    lblErrorMsg.Text = msg.Message
                End If


            End If

            Page.ClientScript.GetPostBackClientHyperlink(Me, "")

            Dim fileHelp As System.Text.StringBuilder = New System.Text.StringBuilder
            With fileHelp
                .Append("Any type of file can be uploaded. The larger the file, the longer it will take to upload and download.\n\n")
                .Append("Image files:\n")
                .Append("          are automatically recognised and made available in the image gallery\n")
                .Append("          have resized versions of the original file automatically created and served as JPEGs when required\n\n")
                .Append("The supported image types are:\n")
            End With
            Dim supportedFileTypeTemplate As String = "          {0} - filename extensions: {1}\n"

            Dim codecs As System.Drawing.Imaging.ImageCodecInfo() = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders()
            For Each codec As System.Drawing.Imaging.ImageCodecInfo In codecs
                fileHelp.Append(String.Format(supportedFileTypeTemplate, codec.FormatDescription, codec.FilenameExtension, codec.MimeType))
            Next

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "fileHelp", WrapClientScript("var fileHelp = """ & fileHelp.ToString() & """;"))

        End Sub

    End Class

End Namespace