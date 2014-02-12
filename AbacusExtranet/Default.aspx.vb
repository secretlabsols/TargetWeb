
Imports System.Configuration.ConfigurationSettings
Imports Target.Library.Web
Imports Target.Web.Apps.Security

''' -----------------------------------------------------------------------------
''' Project	 : Target.Abacus.Extranet
''' Class	 : _Default
''' 
''' -----------------------------------------------------------------------------
''' <summary>
'''     Home page for the Abacus Extranet site.
''' </summary>
''' <remarks>
''' </remarks>
''' <history><![CDATA[
'''     MikeVO      01/12/2008  D11444 - security overhaul.
''' 	[Mikevo]	23/08/2007	Created
''' ]]></history>
''' -----------------------------------------------------------------------------
Partial Class DefaultPage
    Inherits Target.Web.Apps.BasePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Abacus Extranet Home")

        Dim siteName As String = Me.Settings("SiteName")
        Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

        If SecurityBL.IsUserLoggedOn(user.ID) Then
            Response.Redirect("~/Apps/CMS/CMSGetPage.axd?id=1")
        Else
            Response.Redirect("~/Apps/Security/Login.aspx")
        End If

    End Sub

End Class
