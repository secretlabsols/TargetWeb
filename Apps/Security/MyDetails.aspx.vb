
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Security.Collections

Namespace Apps.Security

    ''' <summary>
    ''' Screen to allow a user to change their basic details.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      27/03/2009  D11538 - account activation/reset password overhaul.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class MyDetails
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim customInterface As ICustomSecurity = Nothing
            Dim replacementLogin As IReplacementLogin = Nothing
            Dim msg As ErrorMessage

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.MyDetails"), "My Details")

            If Not Request.QueryString("forced") Is Nothing Then lblForced.Visible = True

            If Not IsPostBack Then

                ' set the page title
                litPageOverview.Text = "To change your information simply use the fields below and click on the 'Save' button."

                'get custom interface
                customInterface = SecurityBL.GetCustomInterface(Me.Settings)
                If Not customInterface Is Nothing Then
                    replacementLogin = TryCast(customInterface, IReplacementLogin)
                End If

                ' get the user info
                Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
                With user
                    txtFirstName.Text = .FirstName
                    txtSurname.Text = .Surname
                    If (replacementLogin Is Nothing) Then
                        txtEmail.Text = .Email
                    Else
                        msg = replacementLogin.GetEmailAddress(user, txtEmail.Text)
                        If Not msg.Success Then litError.Text = msg.Message
                    End If
                    PopulateDropdowns()
                    cboSecurityQuestion.DropDownList.SelectedValue = .WebSecurityQuestionID
                End With
            End If

        End Sub

        Private Sub PopulateDropdowns()

            Dim msg As ErrorMessage
            Dim questions As WebSecurityQuestionCollection = Nothing

            msg = WebSecurityQuestion.FetchList(Me.DbConnection, questions)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            With cboSecurityQuestion.DropDownList
                .Items.Clear()
                .DataSource = questions
                .DataValueField = "ID"
                .DataTextField = "Question"
                .DataBind()
                .Items.Insert(0, New ListItem(String.Empty, String.Empty))
            End With

        End Sub

        Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim customInterface As ICustomSecurity = Nothing
            Dim replacementLogin As IReplacementLogin = Nothing

            PopulateDropdowns()
            cboSecurityQuestion.SelectPostBackValue()

            Me.Validate()

            If Me.IsValid Then
                ' get custom login interface
                customInterface = SecurityBL.GetCustomInterface(Me.Settings)
                If Not customInterface Is Nothing Then
                    replacementLogin = TryCast(customInterface, IReplacementLogin)
                End If

                ' check password is correct 
                If replacementLogin Is Nothing Then
                    If Not SecurityBL.GetPasswordHash(user.PasswordSalt, txtPassword.Text) = user.Password Then
                        litError.Text = "Your details could not be saved because the password you have entered is invalid."
                        Return
                    End If
                Else
                    msg = replacementLogin.ValidatePassword(user.ID, txtPassword.Text)
                    If Not msg.Success Then
                        litError.Text = msg.Message
                        Return
                    End If
                End If

                ' if the email address has been changed, check it doesn't belong to another existing user 
                If user.Email.ToLower() <> txtEmail.Text.ToLower() Then
                    ' check if custom interface is to be used or not
                    If replacementLogin Is Nothing Then
                        Dim users As WebSecurityUserCollection = Nothing
                        msg = WebSecurityUser.FetchList(Me.DbConnection, users, , , txtEmail.Text, , , , Convert.ToInt32(Me.Settings.CurrentApplicationID))
                        If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                        ' if we found any users
                        If users.Count > 0 Then
                            For Each u As WebSecurityUser In users
                                ' and the user wasn't the user we are currently editing
                                If u.ID <> user.ID Then
                                    litError.Text = "Your details could not be saved because the email address you have specified is already in use."
                                    Return
                                End If
                            Next
                        End If
                    Else
                        msg = replacementLogin.EmailAddressInUse(user.ID, txtEmail.Text)
                        If Not msg.Success Then
                            litError.Text = msg.Message
                            Return
                        End If
                    End If
                End If

                ' set the new values and save
                With user
                    .DbConnection = Me.DbConnection
                    .FirstName = txtFirstName.Text
                    .Surname = txtSurname.Text
                    If (replacementLogin Is Nothing) Then
                        .Email = txtEmail.Text
                    Else
                        replacementLogin.SaveEmailAddress(user.ID, txtEmail.Text)
                    End If
                    .WebSecurityQuestionID = Target.Library.Utils.ToInt32(cboSecurityQuestion.GetPostBackValue())
                    ' hash the answer to the security question
                    .SecurityQuestionAnswer = SecurityBL.GetPasswordHash(.PasswordSalt, txtAnswer.Text.ToLower())
                    If .Status = WebSecurityUserStatus.RequiresSecurityInformation Then
                        .Status = WebSecurityUserStatus.Active
                    End If
                    ' save
                    msg = .Save()
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    ' success
                    litPageOverview.Text = "Thank you. Your details have been successfully changed."
                    lblForced.Visible = False
                    litError.Visible = False
                    grpMyDetails.Visible = False
                End With
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

            If Not replacementLogin Is Nothing Then txtPassword.Required = replacementLogin.PasswordRequired
        End Sub
    End Class

End Namespace