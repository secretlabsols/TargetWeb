
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.FileStore.Controls

Namespace Apps.FileStore.Admin

    Partial Class SelectItem
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: File Store: Select Item")

            With tree
                .TreeMode = Target.Library.Utils.ToInt32(Request.QueryString("treeMode"))
            End With

        End Sub

    End Class

End Namespace