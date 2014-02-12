
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Navigation
Imports Target.Web.Apps.Navigation.Collections
Imports Target.Web.Apps.Security.Collections
Imports Target.Web.Apps.Security.PasswordPolicyChecker

Namespace Apps.Security.Admin

    ''' <summary>
    ''' Allows admin users to change the password policy for the application.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir 08/02/2010  D11934 Intranet Password Maintenance
    ''' MikeVO  17/04/2009  A4WA#5388 - fixes after internal testing.
    ''' MikeVO  06/04/2009  D11539 - changed to become "Account Policy".
    '''                     Added "Prevent multiple logins..." option.
    ''' MikeVO  27/03/2009  D11538 - account activation/reset password overhaul.
    ''' MikeVO  20/03/2009  D11535 - created.
    ''' </history>
    Partial Class AccountPolicy
        Inherits Target.Web.Apps.BasePage

        Private _stdBut As StdButtonsBase
        Private _lblMinLengthValidValues As Label = New Label()
        Private _lblBlockHistoricDaysValidValues As Label = New Label()
        Private _lblExpiryDaysValidValues As Label = New Label()
        Private _lblRejectedLoginLimitValidValues As Label = New Label()

        Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit

            Dim masterPageMode As String = Request.QueryString("mpmode")

            If Not IsNothing(masterPageMode) AndAlso String.Compare(masterPageMode, "none", True) = 0 Then

                MasterPageFile = "~/Popup.Master"

            End If

        End Sub

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            AddHandler _stdBut.AddCustomControls, AddressOf AddForceChangePassword

            AddHandler txtMinLength.AfterTextBoxControlAdded, AddressOf AddValidValuesLabel
            AddHandler txtBlockHistoricDays.AfterTextBoxControlAdded, AddressOf AddValidValuesLabel
            AddHandler txtExpiryDays.AfterTextBoxControlAdded, AddressOf AddValidValuesLabel
            AddHandler txtRejectedLoginLimit.AfterTextBoxControlAdded, AddressOf AddValidValuesLabel

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.AccountPolicy"), "Admin: Security: Account Policy")

            Me.AddExtraCssStyle("span.validValues { margin-left:1em; color:#aaaaaa; font-style:italic; }")

            With _stdBut
                .AllowNew = False
                .AllowEdit = True
                .AllowDelete = False
                .AllowFind = False
                .EditableControls.Add(fsAccountPolicy.Controls)
                .EditableControls.Add(fsPasswordPolicy.Controls)
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf FindClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .CancelClicked, AddressOf FindClicked
            End With

            With chkPreventMultipleLogins.CheckBox.Attributes
                .Add("onclick", _
                    "return window.confirm('WARNING: Changing this option requires that this web application is restarted. Doing so will log out all active users. Are you sure you wish to proceed?');" _
                )
            End With

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim policyManager As PasswordPolicyManager
            Dim policyOptions As PasswordPolicyOptions = Nothing
            Dim currentOption As DataRow
            Dim specialCharCheck As CheckSpecialChar = New CheckSpecialChar()
            Dim peLists As WebSecurityPasswordExceptionListCollection = Nothing
            Dim newItem As ListItem
            Dim preventMultipleLogins As ApplicationSetting = Nothing

            ' get current account policy
            msg = FetchPreventMultipleLoginsSetting(preventMultipleLogins)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            chkPreventMultipleLogins.CheckBox.Checked = (preventMultipleLogins.SettingValue = Boolean.TrueString)

            ' get current password policy
            policyManager = New PasswordPolicyManager()
            msg = policyManager.FetchPasswordPolicy(Me.DbConnection, Nothing, Me.Settings, policyOptions)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With txtMinLength
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_MINLENGTH)
                .LabelText = currentOption("Description")
                .Text = policyOptions.MinLength
                _lblMinLengthValidValues.Text = currentOption("ValidValues")
            End With
            With chkLowerCase
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_CHECKLOWERCASE)
                .Label.Text = currentOption("Description")
                .CheckBox.Checked = policyOptions.CheckLowerCase
            End With
            With chkUpperCase
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_CHECKUPPERCASE)
                .Label.Text = currentOption("Description")
                .CheckBox.Checked = policyOptions.CheckUpperCase
            End With
            With chkNumber
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_CHECKNUMBER)
                .Label.Text = currentOption("Description")
                .CheckBox.Checked = policyOptions.CheckNumber
            End With
            With chkSpecialChar
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_CHECKSPECIALCHARACTER)
                .Label.Text = currentOption("Description")
                .CheckBox.Checked = policyOptions.CheckSpecialChar
                lblSpecialChars.Text = specialCharCheck.GetReadableSpecialChars()
            End With
            With chkUsernameBased
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_CHECKUSERNAMEBASED)
                .Label.Text = currentOption("Description")
                .CheckBox.Checked = policyOptions.CheckUsernameBased
            End With
            With chkSameChar
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_CHECKSAMECONSECUTIVECHARACTER)
                .Label.Text = currentOption("Description")
                .CheckBox.Checked = policyOptions.CheckSameChar
            End With
            With chkUseRecaptcha
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_RECAPTCHA)
                .Label.Text = currentOption("Description")
                .CheckBox.Checked = policyOptions.CheckUseRecaptcha
            End With
            With chkDictionaryWord
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_CHECKDICTIONARYWORD)
                .Label.Text = currentOption("Description")
                .CheckBox.Checked = policyOptions.CheckDictionaryWord
            End With

            ' get password exceptions lists
            msg = WebSecurityPasswordExceptionList.FetchList(Me.DbConnection, peLists)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            dlLists.SrcList.Items.Clear()
            dlLists.DestList.Items.Clear()
            For Each lst As WebSecurityPasswordExceptionList In peLists
                Dim theList As ListBox
                If lst.Enabled Then
                    theList = dlLists.DestList
                Else
                    theList = dlLists.SrcList
                End If
                newItem = New ListItem(lst.Name, lst.ID)
                newItem.Attributes.Add("title", lst.Description)
                theList.Items.Add(newItem)
            Next

            With txtBlockHistoricDays
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_BLOCKHISTORICDAYS)
                .LabelText = currentOption("Description")
                .Text = policyOptions.BlockHistoricDays
                _lblBlockHistoricDaysValidValues.Text = currentOption("ValidValues")
            End With
            With txtExpiryDays
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_EXPIRYDAYS)
                .LabelText = currentOption("Description")
                .Text = policyOptions.ExpiryDays
                _lblExpiryDaysValidValues.Text = currentOption("ValidValues")
            End With
            With txtRejectedLoginLimit
                currentOption = policyOptions.FindOption(PasswordPolicyManager.APPS_SECURITY_PASSWORD_REJECTEDLOGINLIMIT)
                .LabelText = currentOption("Description")
                .Text = policyOptions.RejectedLoginLimit
                _lblRejectedLoginLimitValidValues.Text = currentOption("ValidValues")
            End With

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim policyManager As PasswordPolicyManager
            Dim policyOptions As PasswordPolicyOptions
            Dim enabledPeLists As WebSecurityPasswordExceptionListCollection = Nothing
            Dim lst As WebSecurityPasswordExceptionList = Nothing
            Dim trans As SqlTransaction = Nothing
            Dim preventMultipleLogins As ApplicationSetting = Nothing
            Dim recycleApp As Boolean

            If Me.IsValid Then

                ' get current account policy
                msg = FetchPreventMultipleLoginsSetting(preventMultipleLogins)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If chkPreventMultipleLogins.CheckBox.Checked.ToString() <> preventMultipleLogins.SettingValue Then
                    ' recyle the web app after saving if this setting has changed
                    recycleApp = True
                End If

                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    policyOptions = New PasswordPolicyOptions()
                    With policyOptions
                        .MinLength = Utils.ToInt32(txtMinLength.Text)
                        .CheckLowerCase = chkLowerCase.CheckBox.Checked
                        .CheckUpperCase = chkUpperCase.CheckBox.Checked
                        .CheckNumber = chkNumber.CheckBox.Checked
                        .CheckSpecialChar = chkSpecialChar.CheckBox.Checked
                        .CheckUsernameBased = chkUsernameBased.CheckBox.Checked
                        .CheckSameChar = chkSameChar.CheckBox.Checked
                        .CheckDictionaryWord = chkDictionaryWord.CheckBox.Checked
                        .BlockHistoricDays = Utils.ToInt32(txtBlockHistoricDays.Text)
                        .ExpiryDays = Utils.ToInt32(txtExpiryDays.Text)
                        .RejectedLoginLimit = Utils.ToInt32(txtRejectedLoginLimit.Text)
                        .CheckUseRecaptcha = chkUseRecaptcha.CheckBox.Checked
                    End With

                    ' get currently enabled lists
                    msg = WebSecurityPasswordExceptionList.FetchList(trans, enabledPeLists, TriState.True)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' disable any lists that are not now enabled
                    For Each lst In enabledPeLists
                        Dim disable As Boolean = True
                        For Each item As ListItem In dlLists.DestList.Items
                            If Utils.ToInt32(item.Value) = lst.ID Then
                                disable = False
                                Exit For
                            End If
                        Next
                        If disable Then
                            With lst
                                .DbTransaction = trans
                                .Enabled = TriState.False
                                msg = .Save()
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End With
                        End If
                    Next

                    ' enable any new lists that weren't enabled before
                    For Each item As ListItem In dlLists.DestList.Items
                        Dim enable As Boolean = True
                        lst = Nothing
                        For Each lst In enabledPeLists
                            If lst.ID = Utils.ToInt32(item.Value) Then
                                enable = False
                                Exit For
                            End If
                        Next
                        If enable Then
                            lst = New WebSecurityPasswordExceptionList(trans)
                            With lst
                                msg = .Fetch(Utils.ToInt32(item.Value))
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                                .Enabled = TriState.True
                                msg = .Save()
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End With
                        End If
                    Next

                    ' save policy options
                    With preventMultipleLogins
                        .DbTransaction = trans
                        .SettingValue = chkPreventMultipleLogins.CheckBox.Checked.ToString()
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    policyManager = New PasswordPolicyManager()
                    msg = policyManager.SavePasswordPolicy(trans, _
                                                            Me.Settings, _
                                                            policyOptions)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    trans.Commit()

                    If recycleApp Then
                        WebUtils.RecycleApplication()
                    End If

                    FindClicked(e)

                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                    WebUtils.DisplayError(msg)
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub AddValidValuesLabel(ByVal sender As TextBoxEx)

            Dim lbl As Label = Nothing

            Select Case sender.ID
                Case txtMinLength.ID
                    lbl = _lblMinLengthValidValues

                Case txtBlockHistoricDays.ID
                    lbl = _lblBlockHistoricDaysValidValues

                Case txtExpiryDays.ID
                    lbl = _lblExpiryDaysValidValues

                Case txtRejectedLoginLimit.ID
                    lbl = _lblRejectedLoginLimitValidValues

            End Select

            If Not lbl Is Nothing Then
                lbl.CssClass = "validValues"
                sender.Controls.Add(lbl)
            End If

        End Sub

        Private Sub AddForceChangePassword(ByRef controls As ControlCollection)

            Dim btn As Button

            btn = New Button()
            With btn
                .ID = "btnForceChangePassword"
                .Text = "Force Change Password"
                .Width = New Unit(13, UnitType.Em)
                .Attributes.Add("onclick", "return window.confirm('This will force all active users to change their password when they next log in. Are you sure you wish to proceed?');")
                AddHandler btn.Click, AddressOf btnForceChangePassword_Click
            End With
            controls.Add(btn)

        End Sub

        Private Sub btnForceChangePassword_Click(ByVal sender As Object, ByVal e As EventArgs)

            Dim msg As ErrorMessage

            msg = SecurityBL.ForceChangePassword(Me.DbConnection, Me.Settings.CurrentApplicationID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            litPageError.Text = "All active users will be forced to change their password when they next log in."

            FindClicked(Nothing)

        End Sub

        Private Function FetchPreventMultipleLoginsSetting(ByRef setting As ApplicationSetting) As ErrorMessage

            Dim msg As ErrorMessage
            Dim accountSettings As ApplicationSettingCollection = Nothing

            msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, _
                                               list:=accountSettings, _
                                                applicationID:=Me.Settings.CurrentApplicationID, _
                                                settingKey:="Apps.Security.PreventMultipleLogins", _
                                                 auditUserName:=String.Empty, _
                                                auditLogTitle:=String.Empty _
                                                )
            If msg.Success Then
                setting = accountSettings(0)
            End If

            Return msg

        End Function

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            If SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                          SecurityBL.GetCurrentUser().ID, _
                                          Target.Library.Web.ConstantsManager.GetConstant(Me.Settings.CurrentApplication, "WebNavMenuItem.PasswordExceptions"), _
                                           Me.Settings.CurrentApplicationID) Then
                btnManagePasswordException.Disabled = False
            Else
                btnManagePasswordException.Visible = False
            End If

            fsAccountPolicy.Visible = SecurityBL.ShowPreventSameLoginDifferentLocation

        End Sub

    End Class

End Namespace