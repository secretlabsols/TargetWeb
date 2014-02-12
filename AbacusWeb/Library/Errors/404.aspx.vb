
Imports Target.Library.Web

Namespace Library.Errors

    Partial Class Error404
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            InitPage(-1, "Resource Not Found")
        End Sub

    End Class

End Namespace