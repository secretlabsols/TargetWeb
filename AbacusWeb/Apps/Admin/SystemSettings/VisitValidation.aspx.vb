Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Library.Collections
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient

Namespace Apps.Admin.SystemSettings

    ''' <summary>
    ''' Screen used to maintain the Visit Validation.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD     15/04/2010  D11797 - Altered all calls to ApplicationSetting.FetchList
    '''     MikeVO     31/03/2010  A4WA#6194 - Ensure Maximum No. of Carers per Visit setting is saved correctly.
    '''     MikeVO     15/03/2010  A4WA#6140 - correction to min visit duration and added page overview text.
    '''     Paul W     14/10/2009  D11687 - Initial Dev.
    ''' </history>
    Partial Public Class VisitValidation
        Inherits BasePage

        Const SETTING_MIN_NO_CARERS_PER_VISIT As String = "MinNoCarersPerVisit"
        Const SETTING_MAX_NO_CARERS_PER_VISIT As String = "MaxNoCarersPerVisit"
        Const SETTING_MIN_VISIT_DURATION As String = "MinVisitDuration"
        Const SETTING_MAX_VISIT_DURATION As String = "MaxVisitDuration"
        Const SETTING_MAX_NO_VISITS_PER_WEEK As String = "MaxNoVisitsPerWeek"
        Const SETTING_MAX_NO_HOURS_PER_WEEK As String = "MaxNoHoursPerWeek"
        Const APPLICATION_EXTRANET As Integer = 3
        Const APPLICATION_INTRANET As Integer = 2

        Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit

            Dim masterPageMode As String = Request.QueryString("mpmode")

            If Not IsNothing(masterPageMode) AndAlso String.Compare(masterPageMode, "none", True) = 0 Then

                MasterPageFile = "~/Popup.Master"

            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.VisitValidation"), "Visit Validation")
            Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.VisitValidation.Edit"))
                .EditableControls.Add(fsControls.Controls)
                .AllowBack = False
                .AllowDelete = False
                .AllowFind = False
                .AllowNew = False
            End With
            AddHandler stdBut.FindClicked, AddressOf FindClicked
            AddHandler stdBut.EditClicked, AddressOf FindClicked
            AddHandler stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler stdBut.CancelClicked, AddressOf FindClicked

            txtMaxHoursPerWeek.TextBox.Width = 60
            txtMaxVisitsPerWeek.TextBox.Width = 60
            txtMinimumVisitDuration.TextBox.Width = 60
            cboMaxCarersPerVisit.DropDownList.Items.Clear()
            For value As Integer = 1 To 3
                cboMaxCarersPerVisit.DropDownList.Items.Add(value)
            Next
            If Me.IsPostBack Then cboMaxCarersPerVisit.SelectPostBackValue()

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim appSetting As ApplicationSettingCollection = Nothing
            Dim msg As ErrorMessage = Nothing

            msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, list:=appSetting, applicationID:=Me.Settings.CurrentApplicationID, auditLogTitle:=String.Empty, auditUserName:=String.Empty, settingKey:=SETTING_MIN_NO_CARERS_PER_VISIT)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            txtMinCarersPerVisit.Text = appSetting(0).SettingValue

            msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, list:=appSetting, applicationID:=Me.Settings.CurrentApplicationID, auditLogTitle:=String.Empty, auditUserName:=String.Empty, settingKey:=SETTING_MAX_NO_CARERS_PER_VISIT)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            cboMaxCarersPerVisit.DropDownList.Text = appSetting(0).SettingValue


            msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, list:=appSetting, applicationID:=Me.Settings.CurrentApplicationID, auditLogTitle:=String.Empty, auditUserName:=String.Empty, settingKey:=SETTING_MIN_VISIT_DURATION)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            txtMinimumVisitDuration.Text = appSetting(0).SettingValue


            msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, list:=appSetting, applicationID:=Me.Settings.CurrentApplicationID, auditLogTitle:=String.Empty, auditUserName:=String.Empty, settingKey:=SETTING_MAX_VISIT_DURATION)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            Dim time As Date = Convert.ToDateTime(appSetting(0).SettingValue)
            tmeMaximumVisitDuration.Hours = time.Hour
            tmeMaximumVisitDuration.Minutes = time.Minute


            msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, list:=appSetting, applicationID:=Me.Settings.CurrentApplicationID, auditLogTitle:=String.Empty, auditUserName:=String.Empty, settingKey:=SETTING_MAX_NO_VISITS_PER_WEEK)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            txtMaxVisitsPerWeek.Text = appSetting(0).SettingValue

            msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, list:=appSetting, applicationID:=Me.Settings.CurrentApplicationID, auditLogTitle:=String.Empty, auditUserName:=String.Empty, settingKey:=SETTING_MAX_NO_HOURS_PER_WEEK)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            txtMaxHoursPerWeek.Text = appSetting(0).SettingValue


        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim errorText As String = ""
            Dim valid As Boolean = True
            Dim appSetting As ApplicationSettingCollection = Nothing
            Dim msg As ErrorMessage = New ErrorMessage
            Dim trans As SqlTransaction = Nothing

            If txtMinimumVisitDuration.Text.Trim = "" Then
                errorText = String.Format("{0}No value has been entered for Minimum Visit Duration.<br />", errorText)
                valid = False
            ElseIf Convert.ToInt32(txtMinimumVisitDuration.Text) > 60 Or Convert.ToInt32(txtMinimumVisitDuration.Text) < 0 Then
                errorText = String.Format("{0}Minimum Visit Duration must be a value between 0 and 60.<br />", errorText)
                valid = False
            Else
                Dim intMaxDurationsMins As Integer
                intMaxDurationsMins = (tmeMaximumVisitDuration.Hours * 60) + tmeMaximumVisitDuration.Minutes
                If Convert.ToInt32(txtMinimumVisitDuration.Text) > intMaxDurationsMins Then
                    errorText = String.Format("{0}The Minimum Visit Duration exceeds the Maximum Visit Duration.<br />", errorText)
                    valid = False
                End If
            End If

            If txtMaxVisitsPerWeek.Text.Trim = "" Then
                errorText = String.Format("{0}No value has been entered for Maximum No. of Visits per Week.<br />", errorText)
                valid = False
            ElseIf Convert.ToInt32(txtMaxVisitsPerWeek.Text) < 0 Or Convert.ToInt32(txtMaxVisitsPerWeek.Text) > 168 Then
                errorText = String.Format("{0}Maximum No. of Visits per Week must be between 0 and 168.<br />", errorText)
                valid = False
            End If

            If txtMaxHoursPerWeek.Text.Trim = "" Then
                errorText = String.Format("{0}No value has been entered for Maximum No. of Hours per Week.<br />", errorText)
                valid = False
            ElseIf Convert.ToInt32(txtMaxHoursPerWeek.Text) < 0 Or Convert.ToInt32(txtMaxHoursPerWeek.Text) > 504 Then
                errorText = String.Format("{0}Maximum No. of Hours per Week must be between 0 and 504.<br />", errorText)
                valid = False
            End If


            Try

                If valid Then

                    Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                    Dim auditLogTitle As String, auditLogUsername As String

                    auditLogTitle = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
                    auditLogUsername = currentUser.ExternalUsername
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    '*** Max no of carers per visit

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_INTRANET, settingKey:=SETTING_MAX_NO_CARERS_PER_VISIT)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = cboMaxCarersPerVisit.DropDownList.Text
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_EXTRANET, settingKey:=SETTING_MAX_NO_CARERS_PER_VISIT)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = cboMaxCarersPerVisit.DropDownList.Text
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    '*** Min Visit Duration

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_INTRANET, settingKey:=SETTING_MIN_VISIT_DURATION)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = txtMinimumVisitDuration.Text
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_EXTRANET, settingKey:=SETTING_MIN_VISIT_DURATION)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = txtMinimumVisitDuration.Text
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    '*** Max Visit Duration

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_INTRANET, settingKey:=SETTING_MAX_VISIT_DURATION)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = DateTime.Parse(tmeMaximumVisitDuration.ToString(DomContractBL.TIME_ONLY_DATE))
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_EXTRANET, settingKey:=SETTING_MAX_VISIT_DURATION)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = DateTime.Parse(tmeMaximumVisitDuration.ToString(DomContractBL.TIME_ONLY_DATE))
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    '*** Max Visits per Week

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_INTRANET, settingKey:=SETTING_MAX_NO_VISITS_PER_WEEK)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = txtMaxVisitsPerWeek.Text
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_EXTRANET, settingKey:=SETTING_MAX_NO_VISITS_PER_WEEK)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = txtMaxVisitsPerWeek.Text
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    '*** Max No Of Hours

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_INTRANET, settingKey:=SETTING_MAX_NO_HOURS_PER_WEEK)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = txtMaxHoursPerWeek.Text
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    msg = ApplicationSetting.FetchList(trans:=trans, list:=appSetting, auditLogTitle:=auditLogTitle, auditUserName:=auditLogUsername, applicationID:=APPLICATION_EXTRANET, settingKey:=SETTING_MAX_NO_HOURS_PER_WEEK)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With appSetting(0)
                        .DbTransaction = trans
                        .SettingValue = txtMaxHoursPerWeek.Text
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    trans.Commit()

                Else
                    lblError.Text = errorText
                    e.Cancel = True
                End If
            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0502", "Visit Validation")   ' could not save
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try
        End Sub

    End Class

End Namespace