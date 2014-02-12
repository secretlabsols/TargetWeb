
Imports System.Configuration.ConfigurationManager
Imports Target.Library.Web
Imports Target.Web.Apps.Security

''' -----------------------------------------------------------------------------
''' Project	 : Target.SP.Web
''' Class	 : SP.Web._Default
''' 
''' -----------------------------------------------------------------------------
''' <summary>
'''     Home page for the SP Web site.
''' </summary>
''' <remarks>
''' </remarks>
''' <history><![CDATA[
'''     MikeVO      14/11/2006  Added redirect to default CMS page when logged in
'''                             and login page when not.
''' 	[Mikevo]	??/??/????	Created
''' ]]></history>
''' -----------------------------------------------------------------------------
Partial Class DefaultPage
    Inherits Target.Web.Apps.BasePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.InitPage(ConstantsManager.GetConstant("webSecurityItemFreeView"), "Supporting People Home")

        Dim siteName As String = Me.Settings("SiteName")
        Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

        If SecurityBL.IsUserLoggedOn(user.ID) Then
            Response.Redirect("~/Apps/CMS/CMSGetPage.axd?id=1")
        Else
            Response.Redirect("~/Apps/Security/Login.aspx")
        End If

    End Sub

End Class
