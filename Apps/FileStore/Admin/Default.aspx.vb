
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.FileStore.Controls
Imports Target.Web.Apps.Security

Namespace Apps.FileStore.Admin

    Partial Class DefaultPage
        Inherits Target.Web.Apps.BasePage

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: File Store")

            With tree
                .TreeMode = WebFileStoreTreeMode.FileStoreAdmin
                .ShowFileID = Target.Library.Utils.ToInt32(Request.QueryString("showFileID"))
                .ShowFolderID = Target.Library.Utils.ToInt32(Request.QueryString("showFolderID"))
                .MoveFileID = Target.Library.Utils.ToInt32(Request.QueryString("moveFileID"))
                .MoveFolderID = Target.Library.Utils.ToInt32(Request.QueryString("moveFolderID"))
                .MoveTargetID = Target.Library.Utils.ToInt32(Request.QueryString("moveTargetID"))
            End With

        End Sub

#End Region

    End Class

End Namespace