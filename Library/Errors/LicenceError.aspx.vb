
Imports System.Web.Security
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security

Namespace Library.Errors

    ''' <summary>
    ''' Displays the reason for a licence violation.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      06/04/2009  D11539 - changed to accept encrypted FormsAuthenticationTicket ticket rather than text.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class LicenceError
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.RenderMenu = SecurityBL.IsUserLoggedOn()

            Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Licence Error")

            Dim encryptedText As String = Request.QueryString("e")
            Dim decryptedText As String
            Dim ticket As FormsAuthenticationTicket

            Try
                ticket = FormsAuthentication.Decrypt(encryptedText)
                decryptedText = ticket.UserData
            Catch ex As Exception
                decryptedText = "Unknown error"
            End Try

            litErrorMsg.Text = decryptedText
            lnkHome.NavigateUrl = WebUtils.GetVirtualPath("Default.aspx")
            lnkLogin.NavigateUrl = WebUtils.GetVirtualPath("Apps/Security/Login.aspx")
            lnkForceLogin.NavigateUrl = WebUtils.GetVirtualPath("Apps/Security/Login.aspx?forcelogin=1")

        End Sub
    End Class

End Namespace