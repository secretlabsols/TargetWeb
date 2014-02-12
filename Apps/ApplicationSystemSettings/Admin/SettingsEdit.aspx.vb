Imports System.Collections.Generic
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security

Namespace Apps.ApplicationSystemSettings.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Apps.ApplicationSystemSettings.Admin.SettingEditor
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows users to edit one or more system settings.
    ''' 
    '''     Requires at least one of the following parameters in query string:
    ''' 
    '''         1. ID = The ID of a visible ApplicationSetting - display a single setting
    '''         2. FID = The ID of an ApplicationSettingFolder - displays all settings in a folder (takes precedence of two parameters)
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     ColinD      07/06/2010  A4WA#6313 - hide 'Edit' and 'Audit Log' buttons if there are no editable settings 
    ''' 	[ColinD]	01/04/2010	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class SettingsEdit
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        Private _AtLeastOneSettingEditable As Boolean = False   ' indicates if at least one setting is editable, this controls access to the 'Edit' and 'Audit Log' buttons. See AllowEdit property for accessing value.
        Private Const _AuditTable As String = "ApplicationSetting"
        Private Const _ERROR_NoSettingOrFolderSelected As String = "Please select a setting or folder."
        Private Const _ERROR_FolderContainsNoSettings As String = "Folder contains no settings."
        Private Const _ERROR_SettingNotVisible As String = "Setting '{0}' cannot be viewed."
        Private Const _QS_FolderID As String = "fid"
        Private Const _QS_SettingID As String = "id"

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
                            , "Edit Settings" _
                        )

            ' if not a cancellation request then we want to restore setting controls
            ' so we keep viewstate of setting controls etc
            If Not IsCancellationRequest Then

                SetupSettings()

            End If

        End Sub

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If ApplicationSettingFolderID.HasValue OrElse ApplicationSettingID.HasValue Then
                ' only setup page if we have either a folder or setting id to work with

                If Not IsPostBack Then
                    ' if first hit then always set all settings to appear read only

                    SetAllSettingsAsReadOnly(True)

                End If

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
                With stdButtons1
                    .AllowNew = False
                    .AllowEdit = Me.AllowEdit
                    .AllowDelete = False
                    .AllowFind = False
                    If ApplicationSettingID.HasValue AndAlso Me.AllowEdit Then
                        ' if we have specified a specific setting and it is editable then
                        ' enable audit log buttons. We only want this option for single settings not all in folder
                        .AuditLogTableNames.Add(_AuditTable)
                    End If
                    .Visible = True
                End With
            Else
                ' else cannot edit settings so hide standard buttons
                stdButtons1.Visible = False
            End If

        End Sub

        ''' <summary>
        ''' Handles SaveClicked Event for StdButtons Control
        ''' </summary>
        ''' <param name="e">The event arguments</param>
        ''' <remarks></remarks>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs) Handles stdButtons1.SaveClicked

            If Me.IsValid Then
                ' page is valid so save settings

                SaveSettings()

            Else
                ' page is invalid so cancel

                e.Cancel = True

            End If

        End Sub

        ''' <summary>
        ''' Handles EditClicked Event for StdButtons Control
        ''' </summary>
        ''' <param name="e">The event arguments</param>
        ''' <remarks></remarks>
        Private Sub EditClicked(ByRef e As StdButtonEventArgs) Handles stdButtons1.EditClicked

            ' set all settings to not be read only, this will only allow editing of
            ' settings which are editable
            SetAllSettingsAsReadOnly(False)

        End Sub

        ''' <summary>
        ''' Handles CancelClicked Event for StdButtons Control
        ''' </summary>
        ''' <param name="e">The event arguments</param>
        ''' <remarks></remarks>
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs) Handles stdButtons1.CancelClicked

            ' output settings again and set all to read only
            SetupSettings()
            SetAllSettingsAsReadOnly(True)

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
                    ' the user has authorisation to edit this record, return the value of _AtLeastOneSettingEditable
                    ' which is used to indicate if we have at least one editable setting on the page

                    Return _AtLeastOneSettingEditable

                Else
                    ' the user does not have authorisation to edit this record, return false always

                    Return False

                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the ID of setting to edit from the request (using parameter id)
        ''' </summary>
        ''' <value>The ID of setting to edit from the request</value>
        ''' <returns>The ID of setting to edit from the request</returns>
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
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the ID of setting folder to edit from the request (using parameter fid)
        ''' </summary>
        ''' <value>The ID of setting folder to edit from the request</value>
        ''' <returns>The ID of setting folder to edit from the request</returns>
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
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets if the user selected the cancel button from the standard buttons control
        ''' </summary>
        ''' <value>If the user selected the cancel button from the standard buttons control</value>
        ''' <returns>If the user selected the cancel button from the standard buttons control</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsCancellationRequest() As Boolean
            Get
                Dim isCancellationControlName As String = stdButtons1.FindControl("btnCancel").ClientID.Replace("_", "$")
                Dim isCancellation As String = Request.Form(isCancellationControlName)

                ' if isCancellation is null then this button wasn't pressed
                If IsNothing(isCancellation) Then
                    Return False
                Else
                    Return True
                End If

            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Add a setting to the page
        ''' </summary>
        ''' <param name="setting">The setting to add to the page</param>
        ''' <remarks></remarks>
        Private Sub AddSetting(ByVal setting As ApplicationSetting)

            If setting.Visible Then

                Dim settingEditor As UserControls.SettingEditor

                settingEditor = LoadControl("../UserControls/SettingEditor.ascx")   ' load the setting editor control
                settingEditor.InitControl(Me, setting)
                phSettingControls.Controls.Add(settingEditor)   ' add control to the place holders controls, add at end of list of controls

                If setting.Editable Then
                    ' if we have added an editable setting then flag 
                    ' this will control the 'Edit' and 'Audit Log' buttons for this page

                    _AtLeastOneSettingEditable = True

                End If

            End If

        End Sub

        ''' <summary>
        ''' Displays an error message
        ''' </summary>
        ''' <param name="errorMessage">The error message to display</param>
        ''' <remarks></remarks>
        Private Sub DisplayError(ByVal errorMessage As String)

            litPageError.Text = errorMessage
            pnlForm.Visible = False

        End Sub

        ''' <summary>
        ''' Sets all settings as read only, on screen only i.e. does not persist anything to db
        ''' </summary>
        ''' <param name="isReadOnly">If settings are to be read only</param>
        ''' <remarks></remarks>
        Private Sub SetAllSettingsAsReadOnly(ByVal isReadOnly As Boolean)

            ' loop each control in place holder
            For Each control As Control In phSettingControls.Controls

                ' if control is setting editor then set readonly flag as required
                If TypeOf control Is UserControls.SettingEditor Then

                    Dim settingEditor As UserControls.SettingEditor = CType(control, UserControls.SettingEditor)

                    settingEditor.SetSettingReadOnly(isReadOnly)

                End If

            Next

        End Sub

        ''' <summary>
        ''' Save Settings
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SaveSettings()

            ' loop each control in the place holder
            For Each control As Control In phSettingControls.Controls

                ' if the control is a setting editor then save to db
                If TypeOf control Is UserControls.SettingEditor Then

                    Dim settingEditor As UserControls.SettingEditor = CType(control, UserControls.SettingEditor)

                    ' only save editable settings, these will be ignored in the
                    ' setting editor control anyway
                    If settingEditor.SettingEditable Then

                        Dim msg As ErrorMessage

                        msg = settingEditor.SaveSetting(Me)

                        If msg.Success = False Then

                            WebUtils.DisplayError(msg)

                        End If

                    End If

                End If

            Next

            SetAllSettingsAsReadOnly(True)

        End Sub

        ''' <summary>
        ''' Sets up settings on page
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SetupSettings()

            If ApplicationSettingFolderID.HasValue Then
                ' folder id specified so display all settings in folder

                Dim settings As New ApplicationSettingCollection
                Dim msg As ErrorMessage

                ' fetch all settings by folder id
                msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, _
                                                   list:=settings, _
                                                   auditUserName:=String.Empty, _
                                                   auditLogTitle:=String.Empty, _
                                                   applicationID:=Me.Settings.CurrentApplicationID, _
                                                   settingKey:=Nothing, _
                                                   applicationSettingFolderID:=ApplicationSettingFolderID.Value)

                If msg.Success Then
                    ' we fetched settings by folder id

                    Dim settingsToDisplay As Boolean = False

                    If Not IsNothing(settings) Then

                        Dim allowEdit As Boolean = False

                        ' sort the settings in the collection by name
                        settings.Sort(New CollectionSorter("Name", SortDirection.Ascending))

                        For Each setting As ApplicationSetting In settings
                            ' loop each setting and add to page if visible

                            If setting.Visible Then
                                ' the setting is visble

                                AddSetting(setting)
                                settingsToDisplay = True

                            End If

                        Next

                    End If

                    If Not settingsToDisplay Then
                        ' we found no settings to display so display an error

                        DisplayError(_ERROR_FolderContainsNoSettings)

                    End If

                Else
                    ' an error occurred fetching settings by folder id

                    WebUtils.DisplayError(msg)

                End If

            ElseIf ApplicationSettingID.HasValue Then
                ' we have specified that we want to see a specific setting rather than all settings in a folder

                Dim msg As ErrorMessage
                Dim setting As New ApplicationSetting(conn:=Me.DbConnection, _
                                                      auditLogTitle:=String.Empty, _
                                                      auditUserName:=String.Empty)

                ' fetch the setting from the database
                msg = setting.Fetch(ApplicationSettingID.Value)

                If msg.Success Then
                    ' we have found the setting

                    If setting.Visible Then
                        ' we have a visible setting to display so add to page

                        AddSetting(setting)

                    Else
                        ' the setting isn't visible so advise end user

                        DisplayError(String.Format(_ERROR_SettingNotVisible, setting.ID))

                    End If

                Else
                    ' we haven't found setting for the specified id

                    WebUtils.DisplayError(msg)

                End If

            Else
                ' no ids specified so advise user

                DisplayError(_ERROR_NoSettingOrFolderSelected)

            End If

        End Sub

#End Region

    End Class

End Namespace

