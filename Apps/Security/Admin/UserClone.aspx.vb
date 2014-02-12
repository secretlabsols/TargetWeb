
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections

Namespace Apps.Security.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Security.Admin.UserClone
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to clone their user account but with a different name and email address.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[mikevo]	02/11/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class UserClone
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.UserClone"), "Admin: Security: Clone My Account")

            If Not SecurityBL.CanCloneAccount(Me.DbConnection, Me.Settings, SecurityBL.GetCurrentUser()) Then WebUtils.DisplayAccessDenied()

        End Sub

        Private Sub btnClone_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClone.Click

            If IsValid Then

                Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim msg As ErrorMessage
                Dim newUserID As Integer

                ' clone the user
                msg = SecurityBL.CloneUser(Me.DbConnection, Me.Settings, user.ID, txtFirstName.Text, _
                                            txtSurname.Text, txtEmail.Text, newUserID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                Response.Redirect(String.Format("UserActivate.aspx?id={0}", newUserID))

            End If

        End Sub

    End Class

End Namespace