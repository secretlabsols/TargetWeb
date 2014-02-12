
Imports System.Configuration.ConfigurationSettings
Imports Target.Library.Web
Imports Target.Web.Apps.Security

Partial Class DefaultPage
    Inherits Target.Web.Apps.BasePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Abacus Intranet Home")

        Dim siteName As String = Me.Settings("SiteName")

        If SecurityBL.IsUserLoggedOn() Then
            Response.Redirect("~/Apps/CMS/CMSGetPage.axd?id=2")
        Else
            Response.Redirect("~/Apps/Security/Login.aspx")
        End If

    End Sub

End Class
