
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls

Namespace Apps.Security

    ''' <summary>
    ''' Screen to allow a user to change their password.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Waqas       23/08/2012  D12219 - Password change notification email
    '''     MoTahir     10/02/2011  D11934 Password Maintenance
    '''     MikeVO      27/03/2009  D11538 - account activation/reset password overhaul.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ChangePassword
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.ChangePassword"), "Change Password")

            If Not Request.QueryString("forced") Is Nothing Then lblForced.Visible = True

        End Sub

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If IsValid Then
                Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim msg As New ErrorMessage
                msg = SecurityBL.ChangePassword(Me.DbConnection, txtCurrent.Text, txtNew.Text, True, user)
                If msg.Success Then
                    litError.Text = ""
                    lblForced.Visible = False
                    grpChangePassword.Visible = False
                    litSuccessMsg.Text = "Thank you. Your password has been successfully changed."

                    ' Password change notification email for extranet 
                    msg = New ErrorMessage
                    If (Me.Settings.CurrentApplicationID = ApplicationName.AbacusExtranet) Then
                        msg = SecurityBL.NotifyPasswordChange(Me.DbConnection, user)
                        If Not msg.Success Then
                            litError.Text = msg.Message
                        End If
                    End If
                Else
                    litError.Text = msg.Message
                End If
            End If
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim customInterface As ICustomSecurity = Nothing
            Dim replacementLogin As IReplacementLogin = Nothing

            ' get custom login interface
            customInterface = SecurityBL.GetCustomInterface(Me.Settings)
            If Not customInterface Is Nothing Then
                replacementLogin = TryCast(customInterface, IReplacementLogin)
            End If

            If Not replacementLogin Is Nothing Then txtCurrent.Required = replacementLogin.PasswordRequired
        End Sub

       

    End Class

End Namespace