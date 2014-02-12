
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Security.Collections

Namespace Apps.Security

    ''' <summary>
    ''' Screen to allow an end-user to confirm their details, and choose a new password.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      14/09/2009  D11602 - menu improvements.
    '''     MikeVO      31/03/2009  D11538 - account activation/reset password overhaul.
    ''' </history>
    Partial Class ResetPassword
        Inherits Target.Web.Apps.BasePage

        Private _webSecurityUserID As Integer
        Private _tokenID As Integer

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Const PAGE_OVERVIEW As String = _
                "Please re-enter your security question and answer, to confirm your identity, and choose your new password. " & _
                "Click on the 'Submit' button to reset your password."

            Const INVALID_TOKEN As String = _
                "The token contained in the link that brought you to this page is invalid.<br />" & _
                "This maybe because the token has expired.<br />" & _
                "Please use the <a href=""ForgottenPassword.aspx"">forgotten password</a> facility to request a new link."

            Dim msg As ErrorMessage
            Dim isTokenValid As Boolean
            Dim token As String

            Me.RenderMenu = False

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Reset Password")

            litPageOverview.Text = PAGE_OVERVIEW
            token = Request.QueryString("token")

            ' validate the token
            msg = SecurityBL.IsSecurityTokenValid(Me.DbConnection, token, WebSecurityTokenValidUses.UserPasswordReset, _webSecurityUserID, isTokenValid, _tokenID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If isTokenValid Then
                If Not IsPostBack Then
                    PopulateDropdowns()
                End If
            Else
                litError.Text = String.Format(INVALID_TOKEN, Me.Settings("SystemEmailAddress"))
                grpResetPassword.Visible = False
            End If

        End Sub

        Private Sub PopulateDropdowns()

            Dim msg As ErrorMessage
            Dim questions As WebSecurityQuestionCollection = Nothing

            msg = WebSecurityQuestion.FetchList(Me.DbConnection, questions)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With cboSecurityQuestion.DropDownList
                .Items.Clear()
                .DataSource = questions
                .DataValueField = "ID"
                .DataTextField = "Question"
                .DataBind()
                .Items.Insert(0, New ListItem(String.Empty, String.Empty))
            End With

        End Sub

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click

            Const SUCCESS As String = _
                "Thank you. Your password has been reset.<br /><br />You can now <a href=""Login.aspx"">login</a> using your email address and new password."

            Dim msg As ErrorMessage = New ErrorMessage()
            Dim params As ResetPasswordParams
            Dim trans As SqlTransaction = Nothing

            PopulateDropdowns()
            cboSecurityQuestion.SelectPostBackValue()

            Me.Validate()

            If Me.IsValid Then

                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    params = New ResetPasswordParams()
                    With params
                        .WebSecurityUserID = _webSecurityUserID
                        .WebSecurityQuestionID = Utils.ToInt32(cboSecurityQuestion.GetPostBackValue())
                        .SecurityQuestionAnswer = txtAnswer.Text
                        .Password = txtPassword.Text
                    End With

                    msg = SecurityBL.ResetPassword(trans, params)
                    If Not msg.Success Then
                        If msg.Number = SecurityBL.INVALID_SECURITY_INFO OrElse msg.Number = SecurityBL.CHANGE_PASSWORD_FAILED Then
                            litError.Text = msg.Message
                            litError.Visible = True
                            Exit Sub
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    End If

                    ' delete token
                    msg = SecurityBL.DestroySecurityToken(trans, _tokenID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    trans.Commit()

                    msg = New ErrorMessage()
                    msg.Success = True

                    ' success
                    litPageOverview.Text = SUCCESS
                    litError.Visible = False
                    grpResetPassword.Visible = False

                Catch ex As Exception
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try

            End If
        End Sub
    End Class

End Namespace