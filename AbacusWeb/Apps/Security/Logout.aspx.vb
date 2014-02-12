
Imports System.Web.Security

Namespace Apps.Security

    Partial Class Logout
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Session.Abandon()
            FormsAuthentication.SignOut()

            If Not Request.QueryString("timeout") Is Nothing Then
                Response.Redirect("Login.aspx?timeout=1")
            Else
                Response.Redirect("Login.aspx")
            End If

        End Sub

    End Class

End Namespace