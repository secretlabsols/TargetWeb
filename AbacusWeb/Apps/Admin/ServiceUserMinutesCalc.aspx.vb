Imports System.Collections.Generic
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Apps.ApplicationSystemSettings.Admin.ServiceUserMinutesCalc
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows users to edit one or more Service User Minutes Calculation ranges.
    ''' 
    '''     Requires a parameter of [id] in the query string:
    ''' 
    '''     [id] = The ID of a ServiceUserMinutesCalculationMethod record.
    '''     If non-zero, a single ServiceUserMinutesCalculationMethod record.
    '''     If zero, all records are shown simultaneously.
    '''     Each record is INNER JOINed to the ApplicationSettingPeriod table
    '''     via the SettingValue field.
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     John Finch       16/10/2013  Corrupted record-saving and screen-handling now rectified (#8228)
    ''' 	John Finch	     29/07/2011  Created (D12051)
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class ServiceUserMinutesCalc
        Inherits BasePage

#Region "Fields"

        Private Const _APPSETTINGPERIODTABLE As String = "ApplicationSettingPeriod"
        Private Const _CALCMETHODTABLE As String = "ServiceUserMinutesCalculationMethod"
        Private Const _ERROR_FolderContainsNoSettings As String = "Folder contains no settings."
        Private Const _QS_SettingID As String = "id"
        Private Const _QS_FolderID As String = "fid"
        Private _stdBut As StdButtonsBase

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Me.InitPage( _
                        Target.Library.Web.ConstantsManager.GetConstant( _
                                Me.CurrentApplicationFromConfig, _
                                "WebNavMenuItem.Administration.SystemSettings.SystemSettings" _
                                ) _
                            , "Service User Minute Calculations" _
                        )

        End Sub

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            Dim refreshParent As Integer = Utils.ToInt32(Request.QueryString("refr"))
            If refreshParent > 0 Then
                ClientScript.RegisterStartupScript(Me.GetType(), "Startup", "<script language=javascript>top.RefreshPage();</script>")
            End If
        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If AllowEdit Then
                ' if we can edit this/these settings then setup standard buttons
                With _stdBut
                    .AllowNew = (ApplicationSettingID <> 0)
                    .AllowEdit = .AllowNew
                    .AllowDelete = .AllowNew
                    .AllowFind = False
                    If .AllowEdit Then
                        .AuditLogTableNames.Add(_APPSETTINGPERIODTABLE)
                        .AuditLogTableNames.Add(_CALCMETHODTABLE)
                    End If
                    .Visible = True

                    Select Case .ButtonsMode
                        Case StdButtonsMode.AddNew
                            SetupNewSetting()
                            .SelectedItemID = 0
                        Case StdButtonsMode.Edit
                            SetupSettings()
                            .SelectedItemID = ApplicationSettingID
                            SetAllSettingsAsReadOnly(False)
                        Case StdButtonsMode.Fetched, StdButtonsMode.Initial
                            SetupSettings()
                            SetAllSettingsAsReadOnly(True)
                        Case Else
                    End Select
                End With
            Else
                ' else cannot edit settings so hide standard buttons
                _stdBut.Visible = False
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim trans As SqlTransaction = Nothing
            Dim msg As New ErrorMessage

            Try
                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim auditLogTable As String = String.Format("{0}:{1}", Me.Settings.CurrentApplication, "System Settings")
                Dim appSettingPeriodRec As New Target.Library.ApplicationSettingPeriod(conn:=Me.DbConnection, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=auditLogTable)
                Dim appSettingPeriodRecs As Target.Library.Collections.ApplicationSettingPeriodCollection = Nothing
                Dim contractPeriods As DomContractPeriodCollection = Nothing
                Dim calcMethodID As Long = 0

                If _stdBut.SelectedItemID > 0 Then
                    '++ First, need to determine if the ServiceUserMinutesCalculation record
                    '++ linked to this AppSettingPeriod record is being used elsewhere 
                    '++ (i.e. on DomContractPeriod records)..
                    msg = appSettingPeriodRec.Fetch(_stdBut.SelectedItemID)
                    If Not msg.Success Then
                        DisplayError(msg.Message)
                        e.Cancel = True
                        Exit Sub
                    End If
                    calcMethodID = Utils.ToInt32(appSettingPeriodRec.SettingValue)

                    msg = DomContractPeriod.FetchList(conn:=Me.DbConnection, list:=contractPeriods, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=auditLogTable, serviceUserMinutesCalculationMethodID:=calcMethodID)
                    If Not msg.Success Then
                        DisplayError(msg.Message)
                        e.Cancel = True
                        Exit Sub
                    End If
                    If contractPeriods.Count > 0 Then
                        DisplayError("One or more contract periods are using this setting.")
                        e.Cancel = True
                        Exit Sub
                    End If

                    '++ This record cannot be deleted if it is the only one left..
                    msg = Target.Library.ApplicationSettingPeriod.FetchList( _
                            conn:=Me.DbConnection, _
                            list:=appSettingPeriodRecs, _
                            auditUserName:=currentUser.ExternalUsername, _
                            auditLogTitle:=auditLogTable)
                    If Not msg.Success Then
                        DisplayError(msg.Message)
                        e.Cancel = True
                        Exit Sub
                    End If

                    If appSettingPeriodRecs.Count = 1 Then
                        DisplayError("This is the only setting of this type available and cannot be deleted.")
                        e.Cancel = True
                        Exit Sub
                    End If

                    '++ OK to remove the setting at this point..
                    trans = SqlHelper.GetTransaction(Me.DbConnection)
                    msg = Target.Library.ServiceUserMinutesCalculationMethod.Delete(trans:=trans, _
                                        auditUserName:=currentUser.ExternalUsername, _
                                        auditLogTitle:=auditLogTable, id:=calcMethodID)
                    If Not msg.Success Then
                        If trans IsNot Nothing Then SqlHelper.RollbackTransaction(trans)
                        DisplayError(msg.Message)
                        e.Cancel = True
                        Exit Sub
                    End If

                    msg = Target.Library.ApplicationSettingPeriod.Delete(trans:=trans, auditUserName:=currentUser.ExternalUsername, _
                                        auditLogTitle:=auditLogTable, id:=_stdBut.SelectedItemID)
                    If Not msg.Success Then
                        If trans IsNot Nothing Then SqlHelper.RollbackTransaction(trans)
                        DisplayError(msg.Message)
                        e.Cancel = True
                        Exit Sub
                    End If

                    '++ All is good, so commit all changes and refresh the screen..
                    If trans IsNot Nothing Then
                        trans.Commit()
                        trans = Nothing
                    End If

                    'Dim newUrl As String = Me.Request.Url.AbsoluteUri
                    'If Not newUrl.Contains("&refr=1") Then
                    '    newUrl &= "&refr=1"
                    'End If
                    'Response.Redirect(newUrl)
                Else
                    '++ Should never get here, as can only delete an existing record..
                    e.Cancel = True
                End If

            Catch ex As Exception
                e.Cancel = True
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                WebUtils.DisplayError(msg)
            Finally
                If trans IsNot Nothing Then SqlHelper.RollbackTransaction(trans)
            End Try
        End Sub

        ''' <summary>
        ''' Handles SaveClicked Event for StdButtons Control and saves the relevant records.
        ''' </summary>
        ''' <param name="e">The event arguments</param>
        ''' <remarks></remarks>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            If Me.IsValid Then
                Dim appSettingPeriods As Target.Library.Collections.ApplicationSettingPeriodCollection = Nothing
                Dim msg As New ErrorMessage
                Dim trans As SqlTransaction = Nothing
                Dim calcMethodID As Long = 0
                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim auditLogTable As String = String.Format("{0}:{1}", Me.Settings.CurrentApplication, "System Settings")

                Try
                    SetupSettings()
                    SetAllSettingsAsReadOnly(False)

                    '++ The page is valid so save the current settings..
                    trans = SqlHelper.GetTransaction(Me.DbConnection)
                    Dim appSettingPeriodRec As New Target.Library.ApplicationSettingPeriod( _
                            trans:=trans, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=auditLogTable)
                    Dim calcMethodRec As New Target.Library.ServiceUserMinutesCalculationMethod( _
                            trans:=trans, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=auditLogTable)

                    If phSettingControls.Controls.Count > 0 Then
                        Dim thisTextBox As TextBoxEx = Nothing
                        Dim settingEditor As UserControls.ucServiceUserMinutesCalc = Nothing

                        '++ Get the two editable controls in the placeholder..
                        For Each control As Control In phSettingControls.Controls
                            If TypeOf control Is TextBoxEx Then
                                thisTextBox = CType(control, TextBoxEx)
                                '++ Get the date held in the Effective From field..
                            ElseIf TypeOf control Is UserControls.ucServiceUserMinutesCalc Then
                                '++ Get the Calc Method user control so we can grab its properties..
                                settingEditor = CType(control, UserControls.ucServiceUserMinutesCalc)
                            End If
                        Next

                        If thisTextBox Is Nothing OrElse settingEditor Is Nothing Then
                            SqlHelper.RollbackTransaction(trans)
                            trans = Nothing
                        Else
                            '++ Fill the two CodeSmith structures which need to be saved..
                            With appSettingPeriodRec
                                If _stdBut.SelectedItemID > 0 Then
                                    '++ Find the existing setting period record..
                                    msg = .Fetch(_stdBut.SelectedItemID)
                                    If Not msg.Success Then
                                        SqlHelper.RollbackTransaction(trans)
                                        trans = Nothing
                                        e.Cancel = True
                                        DisplayError(msg.Message)
                                        Exit Sub
                                    End If
                                Else
                                    '++ A new SettingPeriod record is required..
                                    .Unhook()
                                    .ID = 0
                                End If

                                .DateFrom = Utils.ToDateTime(thisTextBox.GetPostBackValue)
                                If _stdBut.SelectedItemID = 0 Then
                                    '++ The new ApplicationSettingPeriod record will be saved once 
                                    '++ the (corresponding) CalcMethod record is created..
                                Else
                                    msg = .Save()
                                    If Not msg.Success Then
                                        SqlHelper.RollbackTransaction(trans)
                                        trans = Nothing
                                        e.Cancel = True
                                        DisplayError(msg.Message)
                                        Exit Sub
                                    End If

                                    '++ Existing record, so the SettingValue field must already point
                                    '++ to an existing CalcMethod record - get the ID it holds..
                                    calcMethodID = Utils.ToInt32(.SettingValue)
                                End If
                            End With

                            With calcMethodRec
                                If _stdBut.SelectedItemID > 0 Then
                                    '++ Find the existing calc method record..
                                    msg = .Fetch(calcMethodID)
                                    If Not msg.Success Then
                                        SqlHelper.RollbackTransaction(trans)
                                        trans = Nothing
                                        e.Cancel = True
                                        DisplayError(msg.Message)
                                        Exit Sub
                                    End If
                                Else
                                    '++ The SettingPeriod record was new therefore we have to also create
                                    '++ a new CalcMethod record..
                                    .Unhook()
                                    .ID = 0
                                End If

                                If settingEditor.MinutesFrom1 <> "" Then
                                    .MinutesFrom1 = Convert.ToByte(settingEditor.MinutesFrom1)
                                Else
                                    .MinutesFrom1 = 0
                                End If
                                .CalculationMethod1 = Convert.ToByte(settingEditor.CalcMethod1)

                                If settingEditor.MinutesTo1 <> "" Then
                                    .MinutesTo1 = Convert.ToByte(settingEditor.MinutesTo1)
                                Else
                                    .MinutesTo1 = Nothing
                                End If

                                If settingEditor.MinutesFrom2 <> "" Then
                                    .MinutesFrom2 = Convert.ToByte(settingEditor.MinutesFrom2)
                                    .CalculationMethod2 = Convert.ToByte(settingEditor.CalcMethod2)
                                Else
                                    .MinutesFrom2 = Nothing
                                    .CalculationMethod2 = Nothing
                                End If

                                If settingEditor.MinutesTo2 <> "" Then
                                    .MinutesTo2 = Convert.ToByte(settingEditor.MinutesTo2)
                                Else
                                    .MinutesTo2 = Nothing
                                End If

                                If settingEditor.MinutesFrom3 <> "" Then
                                    .MinutesFrom3 = Convert.ToByte(settingEditor.MinutesFrom3)
                                    .CalculationMethod3 = Convert.ToByte(settingEditor.CalcMethod3)
                                Else
                                    .MinutesFrom3 = Nothing
                                    .CalculationMethod3 = Nothing
                                End If

                                If settingEditor.MinutesTo3 <> "" Then
                                    .MinutesTo3 = Convert.ToByte(settingEditor.MinutesTo3)
                                Else
                                    .MinutesTo3 = Nothing
                                End If

                                msg = .Save()
                                If Not msg.Success Then
                                    SqlHelper.RollbackTransaction(trans)
                                    trans = Nothing
                                    e.Cancel = True
                                    DisplayError(msg.Message)
                                    Exit Sub
                                End If

                                If _stdBut.SelectedItemID = 0 Then
                                    '++ Can now save the parent AppSettingPeriod record with the ID
                                    '++ of the new child CalcMethod record..
                                    appSettingPeriodRec.SettingValue = .ID.ToString
                                    msg = appSettingPeriodRec.Save()
                                    If Not msg.Success Then
                                        SqlHelper.RollbackTransaction(trans)
                                        trans = Nothing
                                        e.Cancel = True
                                        DisplayError(msg.Message)
                                        Exit Sub
                                    End If
                                End If
                            End With

                            '++ All is good, so commit all changes and refresh the screen..
                            If trans IsNot Nothing Then
                                trans.Commit()
                                trans = Nothing
                            End If
                        End If
                    End If

                    'Dim newUrl As String = Me.Request.Url.AbsoluteUri
                    'If Not newUrl.Contains("&refr=1") Then
                    '    newUrl &= "&refr=1"
                    'End If
                    'Response.Redirect(newUrl)

                Catch ex As Exception
                    e.Cancel = True
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                    WebUtils.DisplayError(msg)
                Finally
                    If trans IsNot Nothing Then SqlHelper.RollbackTransaction(trans)
                End Try
            Else
                '++ Something about the page is invalid so cancel changes..
                e.Cancel = True
            End If

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets if the user can edit the current settings
        ''' </summary>
        ''' <value>If the user can edit the current settings</value>
        ''' <returns>If the user can edit the current settings</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AllowEdit() As Boolean
            Get
                If Me.UserHasMenuItemCommand( _
                                        Target.Library.Web.ConstantsManager.GetConstant( _
                                            Me.Settings.CurrentApplication, _
                                            "WebNavMenuItem.Administration.SystemSettings.SystemSettings.Edit" _
                                            ) _
                                        ) Then
                    Return True
                Else
                    '++ The user does not have authorisation to edit this record, return false always..
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the ID of setting (i.e. the ApplicationSettingPeriod record) to edit from the request (using parameter id)
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property ApplicationSettingID() As Nullable(Of Integer)
            Get
                Dim testInteger As Integer
                Dim settingID As String = Request.QueryString(_QS_SettingID)

                If Not String.IsNullOrEmpty(settingID) _
                        AndAlso settingID.Trim().Length > 0 _
                        AndAlso Integer.TryParse(settingID, testInteger) Then
                    ' a valid setting id (integer) has been specified in the url
                    Return testInteger
                Else
                    ' no valid setting id has been specified in the url
                    Return 0
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the ID of the setting folder to edit from the request (using parameter fid)
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property ApplicationSettingFolderID() As Nullable(Of Integer)
            Get
                Dim testInteger As Integer
                Dim folderID As String = Request.QueryString(_QS_FolderID)

                If Not String.IsNullOrEmpty(folderID) _
                        AndAlso folderID.Trim().Length > 0 _
                        AndAlso Integer.TryParse(folderID, testInteger) Then
                    ' a valid folder id (integer) has been specified in the url
                    Return testInteger
                Else
                    ' no valid folder id has been specified in the url
                    Return 0
                End If
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Add the constituent controls (a DatePicker and a user control) to the page
        ''' </summary>
        ''' <param name="setting">The setting to add to the page</param>
        ''' <remarks></remarks>
        Private Sub AddSetting(ByVal setting As ApplicationSetting, _
                               ByVal calcMethod As Target.Library.ServiceUserMinutesCalculationMethod, _
                               ByVal settingPeriod As Target.Library.ApplicationSettingPeriod, _
                               ByVal currentRecID As Long)

            If setting.Visible Then

                Dim newTextBox As TextBoxEx = New TextBoxEx()
                Dim settingEditor As UserControls.ucServiceUserMinutesCalc = LoadControl("../UserControls/ucServiceUserMinutesCalc.ascx")   ' load the setting editor control
                settingEditor.InitControl(Me, setting, calcMethod, currentRecID)

                With newTextBox
                    .ID = String.Format("dtEffectiveFrom_{0}", currentRecID)
                    .LabelText = "Effective From"
                    .LabelWidth = "10em"
                    .TextBox.ReadOnly = Not setting.Editable
                    .TextBox.ToolTip = "The date from which the adjacent settings apply"
                    .Width = Unit.Percentage(12)
                    ' setup the control dependent on setting type
                    .AllowableContent = TextBoxEx.TextBoxExAllowableContent.Date
                    .Format = TextBoxEx.TextBoxExFormat.DateFormat
                    If Me.IsPostBack AndAlso .GetPostBackValue() <> "" Then
                        .TextBox.Text = .GetPostBackValue()
                    Else
                        .TextBox.Text = settingPeriod.DateFrom.ToString("dd/MM/yyyy")
                    End If
                    .Required = True
                    .RequiredValidatorErrMsg = "A valid date is required."
                    .ValidationGroup = "Save"
                End With

                phSettingControls.Controls.Add(newTextBox)   '++ Add 'Effective From' date picker..
                phSettingControls.Controls.Add(settingEditor)   '++ Add Calc Method control..

            End If

        End Sub

        ''' <summary>
        ''' Displays an error message relating to the page content.
        ''' </summary>
        ''' <param name="errorMessage">The error message to display</param>
        ''' <remarks></remarks>
        Private Sub DisplayError(ByVal errorMessage As String)

            lblPageError.Text = errorMessage
            'pnlForm.Visible = False

        End Sub

        ''' <summary>
        ''' Sets all settings as read only, on screen only i.e. does not persist anything to db
        ''' </summary>
        ''' <param name="isReadOnly">If settings are to be read only</param>
        ''' <remarks></remarks>
        Private Sub SetAllSettingsAsReadOnly(ByVal isReadOnly As Boolean)
            Dim htmlDisabledAttribute As String = "disabled"
            Dim htmlDisabledAttributeTrueFlag As String = "true"

            '++ Loop thru each control within placeholder..
            For Each control As Control In phSettingControls.Controls
                '++ If control is a user control, then set readonly flag as required..
                If TypeOf control Is UserControls.ucServiceUserMinutesCalc Then
                    Dim settingEditor As UserControls.ucServiceUserMinutesCalc = CType(control, UserControls.ucServiceUserMinutesCalc)
                    settingEditor.SetSettingReadOnly(isReadOnly)
                ElseIf TypeOf control Is TextBoxEx Then
                    Dim thisTextBox As TextBoxEx = CType(control, TextBoxEx)
                    If isReadOnly Then
                        thisTextBox.TextBox.Attributes.Add(htmlDisabledAttribute, htmlDisabledAttributeTrueFlag)
                    Else
                        thisTextBox.TextBox.Attributes.Remove(htmlDisabledAttribute)
                    End If
                    thisTextBox.DatePickerButton.Visible = Not isReadOnly
                End If
            Next
        End Sub

        ''' <summary>
        ''' Sets up Calculation Methods on page.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SetupSettings()
            Const SP_FETCH_CALCULATION_METHODS As String = "spxServiceUserMinutesCalculationMethod_FetchAll"
            Dim setting As New ApplicationSetting(conn:=Me.DbConnection, _
                                                  auditLogTitle:=String.Empty, _
                                                  auditUserName:=String.Empty)
            Dim calcMethodID As Long, appSettingPeriodID As Long
            Dim msg As ErrorMessage = Nothing

            '++ Should always be a positive value available here, but just in case..
            If ApplicationSettingFolderID > 0 Then
                msg = setting.Fetch(ApplicationSettingFolderID)
                If msg.Success = False Then WebUtils.DisplayError(msg)
            End If

            phSettingControls.Controls.Clear()

            Dim spSPParams() As SqlParameter = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_CALCULATION_METHODS, False)
            If ApplicationSettingID = 0 Then
                '++ Fetch all values available for this application setting..
                spSPParams(0).Value = Convert.DBNull
            Else
                '++ Pass the exact ApplicationSettingPeriod ID to match on..
                spSPParams(0).Value = ApplicationSettingID
            End If
            Dim spDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_CALCULATION_METHODS, spSPParams)
            Dim spTable As DataTable = spDataset.Tables(0)

            If spTable.Rows.Count > 0 Then
                '++ There are Calc Method records available to be displayed..
                Dim settingsToDisplay As Boolean = False
                Dim allowEdit As Boolean = False

                For Each spRow As DataRow In spTable.Rows
                    If setting.Visible Then
                        Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                        Dim auditLogTable As String = String.Format("{0}:{1}", Me.Settings.CurrentApplication, "System Settings")
                        Dim calcMethodRec As New Target.Library.ServiceUserMinutesCalculationMethod( _
                                conn:=Me.DbConnection, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=auditLogTable)
                        Dim appSettingPeriodRec As New Target.Library.ApplicationSettingPeriod( _
                                conn:=Me.DbConnection, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=auditLogTable)

                        '++ Get the two constituent records making up this application setting..
                        appSettingPeriodID = spRow("ApplicationSettingPeriodID")
                        msg = appSettingPeriodRec.Fetch(appSettingPeriodID)
                        If msg.Success = False Then WebUtils.DisplayError(msg)
                        calcMethodID = spRow("CalculationMethodID")
                        msg = calcMethodRec.Fetch(calcMethodID)
                        If msg.Success = False Then WebUtils.DisplayError(msg)

                        '++ Show the controls required for this current setting..
                        AddSetting(setting, calcMethodRec, appSettingPeriodRec, appSettingPeriodID)
                        settingsToDisplay = True
                    End If
                Next

                If Not settingsToDisplay Then
                    '++ No settings to display, so display an error..
                    DisplayError(_ERROR_FolderContainsNoSettings)
                End If
            End If

        End Sub

        ''' <summary>
        ''' Sets up a new Calculation Method on page.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SetupNewSetting()
            Dim setting As New ApplicationSetting(conn:=Me.DbConnection, _
                                                  auditLogTitle:=String.Empty, _
                                                  auditUserName:=String.Empty)
            Dim msg As ErrorMessage = Nothing

            '++ Should always be a positive value available here, but just in case..
            If ApplicationSettingFolderID > 0 Then
                msg = setting.Fetch(ApplicationSettingFolderID)
                If msg.Success = False Then WebUtils.DisplayError(msg)
            End If

            phSettingControls.Controls.Clear()

            If setting.Visible Then
                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim auditLogTable As String = String.Format("{0}:{1}", Me.Settings.CurrentApplication, "System Settings")
                Dim calcMethodRec As New Target.Library.ServiceUserMinutesCalculationMethod( _
                        conn:=Me.DbConnection, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=auditLogTable)
                Dim appSettingPeriodRec As New Target.Library.ApplicationSettingPeriod( _
                        conn:=Me.DbConnection, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=auditLogTable)

                '++ Show the controls required for this current setting..
                AddSetting(setting, calcMethodRec, appSettingPeriodRec, ApplicationSettingID)

                '++ Loop thru each (pair of) controls in the placeholder..
                For Each control As Control In phSettingControls.Controls
                    If TypeOf control Is TextBoxEx Then
                        Dim thisTextBox As TextBoxEx = CType(control, TextBoxEx)
                        thisTextBox.TextBox.Text = ""
                    ElseIf TypeOf control Is UserControls.ucServiceUserMinutesCalc Then
                        Dim settingEditor As UserControls.ucServiceUserMinutesCalc = CType(control, UserControls.ucServiceUserMinutesCalc)

                        settingEditor.Reset()
                    End If

                Next
            End If
        End Sub
#End Region

    End Class

End Namespace

