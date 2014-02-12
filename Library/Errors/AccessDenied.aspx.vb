
Imports Target.Library.Web
Imports Target.Web.Apps.Security

Namespace Library.Errors

    Partial Class AccessDenied
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.RenderMenu = SecurityBL.IsUserLoggedOn()
            Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Access Denied")
            lnkHome.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Default.aspx")
            lnkLogin.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Apps/Security/Login.aspx")
        End Sub

    End Class

End Namespace