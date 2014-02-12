
Imports System.Configuration.ConfigurationManager
Imports System.Web.Security
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Abacus.Web.Apps.Security

Namespace Apps.Security

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.Security.Login
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Login page.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      29/08/2006  D10921 - support for config settings in database.
    ''' 	[mikevo]	??/??/????	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Login
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            InitPage(-1, "Abacus Intranet Login", AppSettings("ConnectionString"))

            If SecurityBL.IsUserLoggedOn() Then Response.Redirect("../../Default.aspx")

            If Not Request.QueryString("timeout") Is Nothing Then lblTimeout.Visible = True

        End Sub

        Private Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
            If IsValid Then
                SecurityBL.Login(Me.DbConnection, txtUsername.Text, txtPassword.Text)
                If SecurityBL.IsUserLoggedOn() Then Response.Redirect("../../Default.aspx")
            End If
        End Sub

    End Class

End Namespace