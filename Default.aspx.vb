
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

''' -----------------------------------------------------------------------------
''' Project	 : Target.Web
''' Class	 : Web._Default
''' 
''' -----------------------------------------------------------------------------
''' <summary>
'''     Home page for the site.
''' </summary>
''' <remarks>
''' </remarks>
''' <history><![CDATA[
'''     MikeVO      01/12/2008  D11444 - security overhaul.
'''     MikeVO      14/11/2006  Added redirect to app-specific home page.
'''     MikeVO      29/08/2006  D10921 - support for config settings in database.
''' 	[mikevo]	??/??/????	Created
''' ]]></history>
''' -----------------------------------------------------------------------------
Partial Class DefaultPage
    Inherits Target.Web.Apps.BasePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Home")

        Dim siteName As String = Me.Settings("SiteName")
        Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
        Dim redirectUrl As String = Me.Settings("HomePageUrl")

        If SecurityBL.IsUserLoggedOn(user.ID) Then
            litWelcomeMsg.Text = String.Format("Hi {0}, welcome to {1}.", user.FirstName, siteName)
        Else
            litWelcomeMsg.Text = String.Format("Welcome to {0}.", siteName)
        End If

        ' redirect if not redirecting back to this page
        If redirectUrl <> Request.Url.AbsolutePath Then
            Response.Redirect(redirectUrl)
        End If


    End Sub

End Class
