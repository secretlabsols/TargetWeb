
Imports Target.Library.Web
Imports Target.Web.Apps.Security

Namespace Library.Errors

    Partial Class Error404
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.RenderMenu = SecurityBL.IsUserLoggedOn()
            Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Page Not Found")
            lnkHome.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Default.aspx")
        End Sub

    End Class

End Namespace