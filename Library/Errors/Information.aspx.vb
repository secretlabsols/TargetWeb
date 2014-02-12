Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.Security

Namespace Library.Errors

    Partial Class Information
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.RenderMenu = SecurityBL.IsUserLoggedOn()
            Dim msg As ErrorMessage = Session("LastError")
            Dim url As String = Session("LastErrorUrl")
            lnkHome.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Default.aspx")

            Try
                Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Information")

                lnkBack.NavigateUrl = url.ToString()
                Dim literalControl As New LiteralControl(msg.Message)
                infoPanel.Controls.Add(literalControl)
            Catch ex As Exception
                ' swallow errors
            End Try

        End Sub

    End Class

End Namespace