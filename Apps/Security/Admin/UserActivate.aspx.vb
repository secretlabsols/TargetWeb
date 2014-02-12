
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections

Namespace Apps.Security.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Security.Admin.UserActivate
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to activate a user in the system.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MoTahir     08/02/2011  D11934 - password maintenance
    '''     MikeVO      15/10/2009  D11546 - corrected misaligned controls (A4WA#5842).
    '''     MikeVO      27/03/2009  D11538 - account activation/reset password overhaul.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[mikevo]	31/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class UserActivate
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.UserActivate"), "Admin: Security: Activate User")

            Dim userID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim user As WebSecurityUser
            Dim msg As ErrorMessage
            Dim customInterface As ICustomSecurity = Nothing
            Dim replacementLogin As IReplacementLogin = Nothing

            ' fetch the user
            user = New WebSecurityUser(Me.DbConnection)
            msg = user.Fetch(userID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get custom login interface
            customInterface = SecurityBL.GetCustomInterface(Me.Settings)
            If Not customInterface Is Nothing Then
                replacementLogin = TryCast(customInterface, IReplacementLogin)
            End If

            ValidateSecurity(user, SecurityBL.GetCurrentUser())

            With user
                txtExternalUserName.Text = WebUtils.EncodeOutput(.ExternalUsername)
                txtExternalFullName.Text = WebUtils.EncodeOutput(.ExternalFullName)
                txtCreated.Text = WebUtils.EncodeOutput(.CreateDate)
            End With

            If Not IsPostBack Then
                With user
                    txtFirstName.Text = .FirstName
                    txtSurname.Text = .Surname
                    If (replacementLogin Is Nothing) Then
                        txtEmail.Text = .Email
                    Else
                        msg = replacementLogin.GetEmailAddress(user, txtEmail.Text)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If
                    PopulateDropdowns()
                    cboSecurityQuestion.DropDownList.SelectedValue = .WebSecurityQuestionID
                End With
            End If

        End Sub

        Private Sub btnActivate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActivate.Click

            Dim userID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = New WebSecurityUser(Me.DbConnection)
            Dim params As ActivationUserParams
            Dim customInterface As ICustomSecurity = Nothing
            Dim replacementLogin As IReplacementLogin = Nothing

            PopulateDropdowns()
            cboSecurityQuestion.SelectPostBackValue()

            Me.Validate()

            If IsValid Then

                ' get custom login interface
                customInterface = SecurityBL.GetCustomInterface(Me.Settings)
                If Not customInterface Is Nothing Then
                    replacementLogin = TryCast(customInterface, IReplacementLogin)
                End If

                ' get the existing data first
                msg = user.Fetch(userID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

                ' check the email address for new users or if the email address of an existing user has been changed

                If user.Email.ToLower() <> txtEmail.Text.ToLower() Then
                    If replacementLogin Is Nothing Then
                        Dim users As WebSecurityUserCollection = Nothing
                        msg = WebSecurityUser.FetchList(Me.DbConnection, users, , , txtEmail.Text, , , , Convert.ToInt32(Me.Settings.CurrentApplicationID))
                        If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                        If users.Count > 0 Then
                            litPageError.Text = "The user could not be activated because the email address you have specified is already in use."
                            Return
                        End If
                    Else
                        msg = replacementLogin.EmailAddressInUse(user.ID, txtEmail.Text)
                        If Not msg.Success Then
                            litPageError.Text = msg.Message
                            Return
                        End If
                    End If
                End If

                    ' set the new values and activate
                    params = New ActivationUserParams()
                    With params
                        .WebSecurityUserID = userID
                        .FirstName = txtFirstName.Text
                    .Surname = txtSurname.Text
                    If replacementLogin Is Nothing Then
                        .Email = txtEmail.Text
                    End If
                    .WebSecurityQuestionID = Target.Library.Utils.ToInt32(cboSecurityQuestion.GetPostBackValue())
                    .SecurityQuestionAnswer = txtAnswer.Text
                    ' activate the user
                    msg = SecurityBL.ActivateUser(Me.DbConnection, params, Me.Settings)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                End With

                    Response.Redirect("UserList.aspx?status=Created")
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

        Private Sub ValidateSecurity(ByVal user As WebSecurityUser, ByVal currentUser As WebSecurityUser)

            Dim canViewOtherExternalUsers As Boolean

            canViewOtherExternalUsers = _
                    Me.UserHasMenuItemCommandInAnyMenuItem( _
                        Target.Library.Web.ConstantsManager.GetConstant( _
                            Me.Settings.CurrentApplication, _
                            "WebNavMenuItemCommand.Users.ViewOtherExternalUsers") _
                    )

            ' ensure requested user application ID matches current application ID
            If user.ApplicationID <> Me.Settings.CurrentApplicationID Then WebUtils.DisplayAccessDenied()
            ' if the user cannot view other external users, ensure requested user has the same external user ID as the current user
            If Not canViewOtherExternalUsers AndAlso user.ExternalUserID <> currentUser.ExternalUserID Then WebUtils.DisplayAccessDenied()

        End Sub

    End Class

End Namespace