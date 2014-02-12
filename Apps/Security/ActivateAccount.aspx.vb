
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Security.Collections

Namespace Apps.Security

    ''' <summary>
    ''' Screen to allow an end-user to confirm their details, choose a password and activate their account.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      14/09/2009  D11602 - menu improvements.
    '''     MikeVO      27/03/2009  D11538 - account activation/reset password overhaul.
    ''' </history>
    Partial Class ActivateAccount
        Inherits Target.Web.Apps.BasePage

        Private _webSecurityUserID As Integer
        Private _tokenID As Integer

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Const PAGE_OVERVIEW As String = _
                "You have previously supplied the information below to the site administrators and this was used to create your account. " & _
                "Please re-enter this information, to confirm your identity, and choose your password. " & _
                "Click on the 'Activate' button to activate your account."

            Const INVALID_TOKEN As String = _
                "The token contained in the link that brought you to this page is invalid.<br />" & _
                "This maybe because the token has expired.<br />" & _
                "Please <a href=""mailto:{0}"">contact the site administrators</a> to request a new link."

            Dim msg As ErrorMessage
            Dim isTokenValid As Boolean
            Dim token As String

            Me.RenderMenu = False

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Activate Account")

            litPageOverview.Text = PAGE_OVERVIEW
            token = Request.QueryString("token")

            ' validate the token
            msg = SecurityBL.IsSecurityTokenValid(Me.DbConnection, token, WebSecurityTokenValidUses.UserActivation, _webSecurityUserID, isTokenValid, _tokenID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If isTokenValid Then
                If Not IsPostBack Then
                    PopulateDropdowns()
                End If
            Else
                litError.Text = String.Format(INVALID_TOKEN, Me.Settings("SystemEmailAddress"))
                grpMyDetails.Visible = False
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

        Private Sub btnActivate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActivate.Click

            Const SUCCESS As String = _
                "Thank you. Your account has been activated.<br /><br />You can now <a href=""Login.aspx"">login</a> using your email address and password."

            Dim msg As ErrorMessage = New ErrorMessage()
            Dim params As UserAccountActivationParams
            Dim trans As SqlTransaction = Nothing

            PopulateDropdowns()
            cboSecurityQuestion.SelectPostBackValue()

            Me.Validate()

            If Me.IsValid Then

                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    params = New UserAccountActivationParams()
                    With params
                        .WebSecurityUserID = _webSecurityUserID
                        .FirstName = txtFirstName.Text
                        .Surname = txtSurname.Text
                        .Email = txtEmail.Text
                        .WebSecurityQuestionID = Utils.ToInt32(cboSecurityQuestion.GetPostBackValue())
                        .SecurityQuestionAnswer = txtAnswer.Text
                        .Password = txtPassword.Text
                    End With

                    msg = SecurityBL.UserAccountActivation(trans, params)
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
                    grpMyDetails.Visible = False

                Catch ex As Exception
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try

            End If
        End Sub
    End Class

End Namespace