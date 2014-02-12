
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Utils
Imports Target.Web.Apps.FileStore.Collections

Namespace Apps.FileStore.Admin

    Partial Class ImageGallery
        Inherits Target.Web.Apps.BasePage

#Region " Private variables and properties "

        Private _imageListHtml As StringBuilder = New StringBuilder

#End Region

#Region " Page Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim extraCss As StringBuilder = New StringBuilder

            Me.RenderMenu = False
            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: File Store: Image Gallery")

            extraCss.Append("div.imageThumbnail { border: silver 1px solid; margin: 0em; padding: 0em; width: 6.95em; height: 6.95em; }")
            extraCss.Append(vbCrLf)
            extraCss.Append("div.imageLabel { font-size: 0.75em; overflow: hidden; width: 7.75em; white-space: nowrap; text-overflow: ellipsis; }")
            extraCss.Append(vbCrLf)
            extraCss.Append("div.galleryItem { float: left; overflow: hidden; width: 7.75em; height: 8.53em; text-align: center; margin-left: 0.31em; margin-bottom: 0.75em; }")
            extraCss.Append(vbCrLf)
            extraCss.Append("fieldset.gallery { float:left; margin-left:0.25em; }")
            extraCss.Append(vbCrLf)
            extraCss.Append("div.gallery { overflow:auto; width:42em; height:40em; }")
            extraCss.Append(vbCrLf)
            extraCss.Append("div.preview { overflow:auto; height:11.63em; width:100%; }")
            extraCss.Append(vbCrLf)
            extraCss.Append("fieldset.gadget { float:right; margin-right:0.25em; margin-bottom:0.25em; padding:0em; width:16.28em; }")
            extraCss.Append(vbCrLf)
            extraCss.Append("input.numberGadget { width:3em; }")
            extraCss.Append(vbCrLf)
            extraCss.Append("input.textGadget { width:11em; }")
            extraCss.Append(vbCrLf)
            Me.AddExtraCssStyle(extraCss.ToString())

            Dim folderID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("folderID"))
            If folderID = 0 Then folderID = FileStoreBL.ROOT_FOLDER_ID

            CreateGalleryHtml(folderID)

            Dim startupScript As StringBuilder = New StringBuilder
            'startupScript.Append("if(!ie) {")
            'startupScript.Append(vbCrLf)
            'startupScript.Append("GetElement(""btnUpload"").disabled = true;")
            'startupScript.Append(vbCrLf)
            'startupScript.Append("}")
            'startupScript.Append(vbCrLf)
            startupScript.AppendFormat("var currentFolderID = {0};", folderID)
            startupScript.Append(vbCrLf)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.FileStore.Admin.ImageGallery", WrapClientScript(startupScript.ToString()))

        End Sub

#End Region

#Region " CreateGalleryHtml "

        Private Sub CreateGalleryHtml(ByVal folderID As Integer)

            ' 0=folderID, 1=image name, 2=image path
            Const FOLDER_TEMPLATE As String = "<div class=""galleryItem""><div class=""imageThumbnail"" ondblclick=""OpenFolder({0});""><img src=""{2}"" alt=""{1}"" style=""vertical-align:middle;padding:7px;"" /></div><div class=""imageLabel"">{1}</div></div>"
            ' 0=image FileID, 1=image name, 2=width, 3=height
            Const IMAGE_TEMPLATE As String = "<div class=""galleryItem""><div class=""imageThumbnail"" onclick=""PreviewImage({0},{2},{3},'{1}');"" ondblclick=""FTB_InsertImage();""><img src=""../FileStoreGetFile.axd?id={0}&x=80&y=80"" alt=""{1}"" style=""vertical-align:middle;padding:7px;width:80px;height:80px"" /></div><div class=""imageLabel"">{1}</div></div>"

            Dim currentFolder As WebFileStoreFolder
            Dim folders As WebFileStoreFolderCollection = Nothing
            Dim files As WebFileStoreFileCollection = Nothing
            Dim msg As ErrorMessage

            ' get current folder
            currentFolder = New WebFileStoreFolder(Me.DbConnection)
            msg = currentFolder.Fetch(folderID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' output parent folder link
            If currentFolder.ID <> FileStoreBL.ROOT_FOLDER_ID Then _imageListHtml.Append(String.Format(FOLDER_TEMPLATE, currentFolder.ParentFolderID, "Go Up", "../Images/folderUp.gif"))

            ' get folders
            msg = WebFileStoreFolder.FetchList(Me.DbConnection, folders, folderID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            For Each folder As WebFileStoreFolder In folders
                If folder.ID <> FileStoreBL.ROOT_FOLDER_ID Then _imageListHtml.Append(String.Format(FOLDER_TEMPLATE, folder.ID, folder.Name, "../Images/folderBig.gif"))
            Next

            ' get image files
            msg = WebFileStoreFile.FetchList(Me.DbConnection, files, folderID, 0, 0)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            For Each file As WebFileStoreFile In files
                If file.IsImage Then _imageListHtml.Append(String.Format(IMAGE_TEMPLATE, file.ID, file.Description, file.Width, file.Height))
            Next

            litImageList.Text = _imageListHtml.ToString()

        End Sub

#End Region

    End Class

End Namespace