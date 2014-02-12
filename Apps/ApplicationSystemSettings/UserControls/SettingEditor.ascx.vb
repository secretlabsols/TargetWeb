Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Target.Library
Imports Target.Library.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports AjaxControlToolkit
Imports Target.Library.Web.Controls

Namespace Apps.ApplicationSystemSettings.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Apps.ApplicationSystemSettings.UserControls.SettingEditor
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows users to edit a system setting
    ''' </summary>
    ''' <remarks>
    ''' InitControl() method is used to populate control, in order to maintain viewstate 
    ''' in between postback this method must be called prior to the Page_Load event on each postback.
    ''' </remarks>
    ''' <history><![CDATA[
    '''     ColinD      09/08/2011  D11965 - Altered GetApplicationSettingValuesFromStoredProcedure to pass the currently selected value to the stored procedure.
    '''     ColinD      12/11/2010  D11807A - Altered interface to use CheckBoxTrueOrFalse or CheckBoxZeroOrOne Setting Types i.e. to output a checkbox which can then be saved in either true/false or 1/0 format...because different areas use different boolean indicators
    '''     ColinD      08/11/2010  SDS Issue 283 - Altered interface to display friendly values for non editable settings i.e. use the application setting value or stored procedure to resolve value of SettingValue
    '''     ColinD      07/07/2010  A4WA#3692 - Index out of range error fixed when selecting items from drop down lists which have no items, caused by A4WA#6306 as there was always at least a default item in the list.
    '''     ColinD      07/06/2010  A4WA#6306 - Removed 'No Value' from drop down list, developers must add entries to ApplicationSettingValue table or lookup stored procedure to allow no value.
    ''' 	[ColinD]	01/04/2010	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class SettingEditor
        Inherits System.Web.UI.UserControl

#Region "Enums"

        ''' <summary>
        ''' Represents setting types
        ''' </summary>
        ''' <remarks></remarks>
        Enum SettingTypes
            TextBox = 1
            DropDown = 2
            DatePicker = 3
            CheckBoxTrueOrFalse = 4
            CheckBoxZeroOrOne = 5
        End Enum

#End Region

#Region "Fields"

        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _DatePickerButtonID As String = "btnDatePicker"
        Private Const _GeneralErrorCode As String = ErrorMessage.GeneralErrorNumber
        Private Const _SaveDateFormat As String = "yyyy-MM-dd"
        Private Const _SettingControlSizePercentage As Double = 98

#End Region

#Region "Methods"

        ''' <summary>
        ''' Initialise the control i.e. populate setting value to be viewed/edited
        ''' </summary>
        ''' <param name="basePage">The base page to use, this page will contain an open database connection</param>
        ''' <param name="applicationSetting">The application setting to view/edit</param>
        ''' <remarks></remarks>
        Public Sub InitControl(ByVal basePage As BasePage, _
                               ByVal applicationSetting As ApplicationSetting)

            Dim msg As New ErrorMessage()

            hdnSettingID.Value = applicationSetting.ID
            hdnSettingEditable.Value = applicationSetting.Editable.ToString()
            SettingLastValue = applicationSetting.SettingValue
            SettingType = CType(applicationSetting.SettingType, SettingTypes)

            ' only display visible settings
            If applicationSetting.Visible Then

                litSettingName.Text = String.Format("<strong>{0}</strong>", _
                                                    applicationSetting.Name)

                ' if editable add editable control
                If applicationSetting.Editable Then

                    litSettingDetails.Text = String.Format("<strong>Description :</strong> {0}<br />" & _
                                       "<strong>Valid Values :</strong> {1}", _
                                       applicationSetting.Description, _
                                       applicationSetting.ValidValues)

                    ' determine the type of control required for this setting and display it
                    Select Case SettingType

                        Case SettingTypes.TextBox, SettingTypes.DatePicker

                            msg = OutputTextboxSettingControl(basePage, applicationSetting)

                        Case SettingTypes.DropDown

                            msg = OutputDropDownSettingControl(basePage, applicationSetting)

                        Case SettingTypes.CheckBoxTrueOrFalse, SettingTypes.CheckBoxZeroOrOne

                            msg = OutputCheckBoxSettingControl(basePage, applicationSetting)

                        Case Else

                            msg = New ErrorMessage()

                            With msg
                                .Success = False
                                .Message = String.Format("Control is not configured to handle settings of type {0}.", SettingType)
                            End With

                    End Select

                    ' if the we couldnt configure a setting control then show an error
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                Else
                    ' else the setting isn't editable so use a label control for display

                    litSettingDetails.Text = String.Format("<strong>Description :</strong> {0}<br />", _
                                                           applicationSetting.Description)

                    msg = OutputLabelSettingControl(basePage, applicationSetting)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                End If

            Else
                ' else setting isn't visible so hide it

                fsSettings.Visible = False

            End If

        End Sub

        ''' <summary>
        ''' Gets Dictionary of settings from stored procedure, uses 1st column returned as key and 2nd as value
        ''' </summary>
        ''' <param name="sqlConnection">The sql connection to use</param>
        ''' <param name="sqlProcedureName">The sql procedure to use</param>
        ''' <returns>Dictionary of settings from stored procedure</returns>
        ''' <remarks></remarks>
        Private Function GetApplicationSettingValuesFromStoredProcedure(ByVal sqlConnection As SqlConnection, _
                                                                        ByVal sqlProcedureName As String) _
                                                                        As Dictionary(Of Integer, String)

            Dim sqlParams(1) As SqlParameter

            sqlParams(0) = New SqlParameter("@selectedId", Utils.ToInt32(SettingLastValue))

            ' use a reader to get all values back from the provided stored proc
            Using reader As SqlDataReader = SqlHelper.ExecuteReader(sqlConnection, CommandType.StoredProcedure, sqlProcedureName, sqlParams)

                Try

                    Dim settings As New Dictionary(Of Integer, String)

                    While reader.Read

                        ' do not allow null keys
                        If Not IsDBNull(reader.Item(0)) Then

                            Dim settingKey As Integer = Convert.ToInt32(reader.Item(0))
                            Dim settingValue As String = IIf(IsDBNull(reader.Item(1)), "", reader.Item(1))

                            If settings.ContainsKey(settingKey) Then
                                settings(settingKey) = settingValue
                            Else
                                settings.Add(settingKey, settingValue)
                            End If

                        End If

                    End While

                    Return settings

                Catch ex As Exception

                    Throw

                Finally

                    SqlHelper.CloseReader(reader)

                End Try

            End Using

        End Function

        ''' <summary>
        ''' Gets the drop down list items for an application setting.
        ''' </summary>
        ''' <param name="basePage">The base page.</param>
        ''' <param name="applicationSetting">The application setting.</param>
        ''' <param name="items">The items.</param>
        ''' <returns></returns>
        Private Function GetDropDownListItems(ByVal basePage As BasePage, _
                                              ByVal applicationSetting As ApplicationSetting, _
                                              ByRef items As List(Of ListItem)) _
                                              As ErrorMessage

            Dim msg As New ErrorMessage()

            items = New List(Of ListItem)()

            If String.IsNullOrEmpty(applicationSetting.LookupStoredProcedure) = False _
                    AndAlso applicationSetting.LookupStoredProcedure.Trim().Length > 0 Then
                ' if LookupStoredProcedure is defined then retrieve drop down values from this stored procedure

                Dim applicationSettingsDictionary As Dictionary(Of Integer, String) _
                    = GetApplicationSettingValuesFromStoredProcedure(basePage.DbConnection, _
                                                                     applicationSetting.LookupStoredProcedure)

                If Not IsNothing(applicationSettingsDictionary) _
                    AndAlso applicationSettingsDictionary.Count > 0 Then

                    ' loop each item from the stored proc and add into the settings drop down list
                    For Each settingItem As KeyValuePair(Of Integer, String) In applicationSettingsDictionary

                        items.Add(New ListItem(settingItem.Value, settingItem.Key))

                    Next

                End If

            Else
                ' else try the application setting value table for data

                Dim applicationSettingValues As New ApplicationSettingValueCollection

                ' fetch a list of applications setting values
                msg = ApplicationSettingValue.FetchList(conn:=basePage.DbConnection, _
                                                        list:=applicationSettingValues, _
                                                        applicationSettingID:=applicationSetting.ID)
                If Not msg.Success Then Return msg

                If Not applicationSettingValues Is Nothing AndAlso applicationSettingValues.Count > 0 Then
                    ' if we have some setting values to display then setup data binding etc

                    ' loop each setting value and add into items list
                    For Each settingValue As ApplicationSettingValue In (From tmpApplicationSettingValue As ApplicationSettingValue In applicationSettingValues.ToArray() _
                                                                            Order By tmpApplicationSettingValue.SortOrder Ascending _
                                                                                Select tmpApplicationSettingValue)

                        items.Add(New ListItem(settingValue.DisplayText, settingValue.Value))

                    Next

                End If

            End If

            msg = New ErrorMessage()
            msg.Success = True

            Return msg

        End Function

        ''' <summary>
        ''' Outputs the setting control as a check box control.
        ''' </summary>
        ''' <param name="basePage">The base page.</param>
        ''' <param name="applicationSetting">The application setting.</param>
        ''' <returns></returns>
        Private Function OutputCheckBoxSettingControl(ByVal basePage As BasePage, _
                                                      ByVal applicationSetting As ApplicationSetting) _
                                                      As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                Dim applicationSettingCheckBox As New CheckBox()

                With applicationSettingCheckBox
                    .ID = SettingValueCheckBoxName
                    ' default the check box to unchecked
                    applicationSettingCheckBox.Checked = False
                    If Not IsNothing(applicationSetting.SettingValue) _
                        AndAlso applicationSetting.SettingValue.Trim().Length > 0 _
                        AndAlso (String.Compare(applicationSetting.SettingValue, "1", True) = 0 _
                                 OrElse String.Compare(applicationSetting.SettingValue, "True", True) = 0) Then
                        ' if the setting value is either 1 or True then check the check box
                        applicationSettingCheckBox.Checked = True
                    End If
                    .ToolTip = applicationSetting.Description
                End With

                phSettingControls.Controls.Add(applicationSettingCheckBox)
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Utils.CatchError(ex, _GeneralErrorCode)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Outputs the setting control as a drop down control.
        ''' </summary>
        ''' <param name="basePage">The base page.</param>
        ''' <param name="applicationSetting">The application setting.</param>
        ''' <returns></returns>
        Private Function OutputDropDownSettingControl(ByVal basePage As BasePage, _
                                                     ByVal applicationSetting As ApplicationSetting) _
                                                     As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                Dim applicationSettingDropDownList As New DropDownList()
                Dim applicationSettingDropDownListItemToSelect As ListItem
                Dim applicationSettingDropDownListItems As New List(Of ListItem)()

                ' setup the basic properties of the drop down list
                With applicationSettingDropDownList
                    .Enabled = Not applicationSetting.Editable
                    .ID = SettingValueDropDownName
                    .ToolTip = applicationSetting.Description
                    .Width = Unit.Percentage(_SettingControlSizePercentage)
                End With

                ' get the drop down list items to add
                msg = GetDropDownListItems(basePage, applicationSetting, applicationSettingDropDownListItems)
                If Not msg.Success Then Return msg

                If Not applicationSettingDropDownListItems Is Nothing AndAlso applicationSettingDropDownListItems.Count > 0 Then
                    ' if we have some items to add into the drop down list

                    ' add the items into the drop down list
                    applicationSettingDropDownList.Items.AddRange(applicationSettingDropDownListItems.ToArray())

                    If applicationSettingDropDownList.Items.Count > 0 Then
                        ' if we have some items try and select a value

                        applicationSettingDropDownListItemToSelect = applicationSettingDropDownList.Items.FindByValue(applicationSetting.SettingValue)

                        If Not IsNothing(applicationSettingDropDownListItemToSelect) Then
                            ' if we have an item in the list that matches the  value from the db then select it

                            applicationSettingDropDownListItemToSelect.Selected = True

                        Else
                            ' else select the first item in the list

                            applicationSettingDropDownList.Items(0).Selected = True

                        End If

                    End If

                End If

                phSettingControls.Controls.Add(applicationSettingDropDownList)
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Utils.CatchError(ex, _GeneralErrorCode)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Outputs setting control as a label i.e. read only.
        ''' </summary>
        ''' <param name="basePage">The base page.</param>
        ''' <param name="applicationSetting">The application setting.</param>
        ''' <returns></returns>
        Private Function OutputLabelSettingControl(ByVal basePage As BasePage, _
                                                     ByVal applicationSetting As ApplicationSetting) _
                                                     As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                Dim applicationSettingLabel As New Label()

                With applicationSettingLabel
                    .Font.Bold = True
                    .ID = SettingValueLabelName
                    If Not IsNothing(applicationSetting.SettingValue) _
                        AndAlso applicationSetting.SettingValue.Trim().Length > 0 Then
                        ' if we have a setting value to work with then set the text property of the label

                        .Text = applicationSetting.SettingValue

                        If SettingType = SettingTypes.DropDown Then
                            ' if this is a drop down list setting then get the friendly description

                            Dim items As List(Of ListItem) = Nothing

                            ' get the drop down list items
                            msg = GetDropDownListItems(basePage, applicationSetting, items)
                            If Not msg.Success Then Return msg

                            ' get the value from the list that matches applicationSetting.SettingValue field
                            .Text = (From item As ListItem In items _
                                        Where item.Value = applicationSetting.SettingValue _
                                            Select item.Text).FirstOrDefault()

                        ElseIf SettingType = SettingTypes.CheckBoxTrueOrFalse OrElse SettingType = SettingTypes.CheckBoxZeroOrOne Then
                            ' if the setting is a boolean type then determine what it should be on screen

                            If String.Compare(applicationSetting.SettingValue, "1", True) = 0 _
                                     OrElse String.Compare(applicationSetting.SettingValue, "True", True) = 0 Then

                                .Text = "True"

                            Else

                                .Text = "False"

                            End If

                        ElseIf SettingType = SettingTypes.DatePicker Then
                            ' if the setting is a date then format it

                            Dim tmpDate As DateTime = DateTime.MinValue

                            If DateTime.TryParse(applicationSetting.SettingValue, tmpDate) Then

                                .Text = tmpDate.ToString(_DateFormat)

                            End If

                        End If

                    Else

                        .ForeColor = Color.Red
                        .Text = "Value not set"

                    End If
                    .ToolTip = applicationSetting.Description
                    .Width = Unit.Percentage(_SettingControlSizePercentage)
                End With

                phSettingControls.Controls.Add(applicationSettingLabel)
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Utils.CatchError(ex, _GeneralErrorCode)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Outputs setting control as a text box.
        ''' </summary>
        ''' <param name="basePage">The base page.</param>
        ''' <param name="applicationSetting">The application setting.</param>
        ''' <returns></returns>
        Private Function OutputTextboxSettingControl(ByVal basePage As BasePage, _
                                                     ByVal applicationSetting As ApplicationSetting) _
                                                     As ErrorMessage

            Dim applicationSettingTextBox As New TextBoxEx()
            Dim msg As New ErrorMessage()
            Dim validationGroup As String = "Save"

            Try

                With applicationSettingTextBox
                    .ID = SettingValueTextBoxName
                    .TextBox.ReadOnly = Not applicationSetting.Editable
                    .TextBox.ToolTip = applicationSetting.Description
                    .Width = Unit.Percentage(_SettingControlSizePercentage)
                    ' setup the control dependant on setting type
                    Select Case SettingType
                        Case SettingTypes.DatePicker
                            ' setup the setting control for date entry only
                            .AllowableContent = TextBoxEx.TextBoxExAllowableContent.Date
                            .Format = TextBoxEx.TextBoxExFormat.DateFormat
                            If Not String.IsNullOrEmpty(applicationSetting.SettingValue) _
                                AndAlso applicationSetting.SettingValue.Trim().Length > 0 Then
                                .Text = CDate(applicationSetting.SettingValue).ToString(_DateFormat)
                            End If
                        Case Else
                            ' setup the control for free text entry
                            .Text = applicationSetting.SettingValue
                    End Select
                    .ValidationGroup = validationGroup
                End With

                ' if this is a free text box and FieldValueRegExID is larger than 0 then we need to validate this text box
                If applicationSetting.FieldValueRegExID > 0 _
                    AndAlso SettingType = SettingTypes.TextBox Then

                    Dim fieldRegex As New FieldValueRegEx(basePage.DbConnection)

                    msg = fieldRegex.Fetch(applicationSetting.FieldValueRegExID)

                    If msg.Success Then

                        Dim applicationSettingTextBoxValidator As New RegularExpressionValidator()

                        With applicationSettingTextBoxValidator
                            .ControlToValidate = String.Format("{0}$txtTextBox", applicationSettingTextBox.ClientID)
                            .Display = ValidatorDisplay.Dynamic
                            .EnableClientScript = True
                            .ErrorMessage = String.Empty
                            .Font.Bold = True
                            .ID = SettingValueValidatorName
                            .SetFocusOnError = True
                            .Text = String.Format("Field is not valid, valid values for this field are : {0}", _
                                                  applicationSetting.ValidValues)
                            .ValidationExpression = fieldRegex.RegEx
                            .ValidationGroup = validationGroup
                        End With

                        phSettingControls.Controls.Add(applicationSettingTextBoxValidator)

                    Else

                        WebUtils.DisplayError(msg)

                    End If

                End If

                phSettingControls.Controls.Add(applicationSettingTextBox)
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Utils.CatchError(ex, _GeneralErrorCode)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Saves the setting to the database
        ''' </summary>
        ''' <param name="basePage">The base page</param>
        ''' <returns>ErrorMessage object detailing success</returns>
        ''' <remarks>Please note uneditable settings will always return successful without saving</remarks>
        Public Function SaveSetting(ByVal basePage As BasePage) As ErrorMessage

            Dim msg As ErrorMessage

            Try

                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim auditLogTitle As String = AuditLogging.GetAuditLogTitle(basePage.PageTitle, basePage.Settings)
                Dim auditLogUser As String = currentUser.ExternalUsername
                Dim currentSettingValue As String = SettingValue
                Dim setting As New ApplicationSetting(basePage.DbConnection, auditLogUser, auditLogTitle)
                Dim settingValidation As RegularExpressionValidator = SettingValidator

                ' if we haven't got an id then InitControl() hasn't completed
                If SettingID.HasValue Then

                    ' if the setting has changed then save
                    If SettingValueHasChanged Then

                        ' if we have a validation control then validate the setting value
                        If Not IsNothing(settingValidation) Then

                            settingValidation.Validate()

                        End If

                        ' if the value of the setting is valid
                        If IsNothing(settingValidation) OrElse settingValidation.IsValid Then

                            ' if the setting is editable then save
                            If SettingEditable Then

                                msg = setting.Fetch(SettingID.Value)

                                If msg.Success AndAlso setting.Editable Then

                                    setting.SettingValue = currentSettingValue
                                    setting.SettingValueDescription = SettingValueDescription

                                    msg = setting.Save()

                                    If msg.Success Then

                                        SettingLastValue = currentSettingValue

                                        If String.IsNullOrEmpty(setting.SystemInfoColumn) = False _
                                            AndAlso setting.SystemInfoColumn.Trim().Length > 0 Then

                                            msg = UpdateSystemInfoColumn(basePage.DbConnection, _
                                                                         setting.SystemInfoColumn)

                                        End If

                                    End If

                                End If

                            Else

                                msg = New ErrorMessage()

                                With msg
                                    .Success = True
                                    .Message = String.Format("Setting ID {0} cannot be saved as it is not editable.", _
                                                             SettingID.Value)
                                End With

                            End If

                        Else

                            msg = New ErrorMessage()

                            With msg
                                .Success = False
                                .Message = String.Format("Setting ID {0} cannot be saved as the value provided is not valid.", _
                                                         SettingID.Value)
                            End With

                        End If

                    Else

                        msg = New ErrorMessage()

                        With msg
                            .Success = True
                            .Message = String.Format("Setting ID {0} value has not changed.", _
                                                     SettingID.Value)
                        End With

                    End If

                Else

                    msg = New ErrorMessage()

                    With msg
                        .Success = False
                        .Message = "Method InitControl of class SettingEditor must be executed prior to saving a setting."
                    End With

                End If

            Catch ex As Exception

                msg = Utils.CatchError(ex, _GeneralErrorCode)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Sets this setting as Read Only or Not
        ''' </summary>
        ''' <param name="isReadOnly">If setting should be read only</param>
        ''' <remarks>Please note this will have no effect on uneditable settings</remarks>
        Public Sub SetSettingReadOnly(ByVal isReadOnly As Boolean)

            If SettingEditable Then

                Dim settingControl As Control = SettingValueControl
                Dim htmlDisabledAttribute As String = "disabled"
                Dim htmlDisabledAttributeTrueFlag As String = "true"

                If Not IsNothing(settingControl) Then
                    ' if we have found a control

                    If TypeOf settingControl Is TextBox Then
                        ' if the control is a textbox

                        Dim settingTextBoxControl As TextBox = CType(settingControl, TextBox)

                        If isReadOnly Then
                            settingTextBoxControl.Attributes.Add(htmlDisabledAttribute, htmlDisabledAttributeTrueFlag)
                        Else
                            settingTextBoxControl.Attributes.Remove(htmlDisabledAttribute)
                        End If

                    ElseIf TypeOf settingControl Is TextBoxEx Then
                        ' if the control is a textboxex

                        Dim settingTextBoxControl As TextBoxEx = CType(settingControl, TextBoxEx)
                        Dim settingBtnDatePicker As HtmlInputButton = Nothing

                        If isReadOnly Then
                            settingTextBoxControl.TextBox.Attributes.Add(htmlDisabledAttribute, htmlDisabledAttributeTrueFlag)
                        Else
                            settingTextBoxControl.TextBox.Attributes.Remove(htmlDisabledAttribute)
                        End If

                        settingBtnDatePicker = settingTextBoxControl.FindControl(_DatePickerButtonID)

                        If Not settingBtnDatePicker Is Nothing Then
                            ' if we have a date picker control contained in the text box control then disable it

                            If isReadOnly Then
                                settingBtnDatePicker.Visible = False
                            Else
                                settingBtnDatePicker.Visible = True
                            End If

                        End If

                    ElseIf TypeOf settingControl Is DropDownList Then
                        ' if the control is a drop down list

                        Dim settingDropDownControl As DropDownList = CType(settingControl, DropDownList)

                        If isReadOnly Then
                            settingDropDownControl.Attributes.Add(htmlDisabledAttribute, htmlDisabledAttributeTrueFlag)
                        Else
                            settingDropDownControl.Attributes.Remove(htmlDisabledAttribute)
                        End If

                        settingDropDownControl.Enabled = Not isReadOnly

                    ElseIf TypeOf settingControl Is CheckBox Then

                        Dim settingCheckBoxControl As CheckBox = CType(settingControl, CheckBox)

                        If isReadOnly Then
                            settingCheckBoxControl.Attributes.Add(htmlDisabledAttribute, htmlDisabledAttributeTrueFlag)
                        Else
                            settingCheckBoxControl.Attributes.Remove(htmlDisabledAttribute)
                        End If

                    End If

                End If

            End If

        End Sub

        ''' <summary>
        ''' Updates SystemInfo table with the setting value
        ''' </summary>
        ''' <param name="sqlConnection">The connection to use when updating the SystemInfo table</param>
        ''' <param name="columnName">The column in the SystemInfo table to update</param>
        ''' <returns>ErrorMessage</returns>
        ''' <remarks></remarks>
        Private Function UpdateSystemInfoColumn(ByVal sqlConnection As SqlConnection, _
                                                ByVal columnName As String) _
                                                As ErrorMessage

            Dim msg As New ErrorMessage

            Try

                Dim sqlParameters As New List(Of SqlParameter)

                sqlParameters.Add(New SqlParameter("@SystemInfoValue", SettingValue))

                Using command As SqlCommand = SqlHelper.CreateCommand(sqlConnection, _
                                                                      String.Format("UPDATE SystemInfo " & _
                                                                                    "SET {0} = @SystemInfoValue", _
                                                                                    columnName), _
                                                                      CommandType.Text, _
                                                                      sqlParameters.ToArray())

                    SqlHelper.ExecuteNonQuery(command)

                End Using

                msg.Success = True

            Catch ex As Exception

                msg = Utils.CatchError(ex, _GeneralErrorCode)

            End Try

            Return msg

        End Function

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the Setting ID associated with this control
        ''' </summary>
        ''' <value>The Setting ID associated with this control</value>
        ''' <returns>The Setting ID associated with this control</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SettingID() As Nullable(Of Integer)
            Get
                Dim testInteger As Integer

                ' might not always have an id as InitControl hasn't been called etc
                If Integer.TryParse(hdnSettingID.Value, testInteger) Then
                    Return testInteger
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets\Sets the settings last value. This is used to evaluate if we need to save a setting or not.
        ''' </summary>
        ''' <value>The settings last value</value>
        ''' <returns>The settings last value</returns>
        ''' <remarks></remarks>
        Private Property SettingLastValue() As String
            Get
                Return hdnSettingLastValue.Value
            End Get
            Set(ByVal value As String)
                hdnSettingLastValue.Value = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type of the setting.
        ''' </summary>
        ''' <value>The type of the setting.</value>
        Private Property SettingType() As SettingTypes
            Get
                Return CType(Integer.Parse(hdnSettingType.Value), SettingTypes)
            End Get
            Set(ByVal value As SettingTypes)
                hdnSettingType.Value = CType(value, Integer)
            End Set
        End Property

        ''' <summary>
        ''' Gets the value for this setting
        ''' </summary>
        ''' <value>Value for this setting</value>
        ''' <returns>Value for this setting</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SettingValue() As String
            Get

                Dim settingControl As Control = SettingValueControl

                ' we might not always have a control i.e. when the InitControl hasn't been called
                If Not IsNothing(settingControl) Then

                    If TypeOf settingControl Is TextBox Then

                        Return CType(settingControl, TextBox).Text

                    ElseIf TypeOf settingControl Is TextBoxEx Then

                        Dim settingControlText As String = CType(settingControl, TextBoxEx).Text

                        If SettingType = SettingTypes.DatePicker Then
                            If Not String.IsNullOrEmpty(settingControlText) _
                                AndAlso settingControlText.Trim().Length > 0 Then
                                settingControlText = CDate(settingControlText).ToString(_SaveDateFormat)
                            Else
                                settingControlText = Nothing
                            End If
                        End If

                        Return settingControlText

                    ElseIf TypeOf settingControl Is DropDownList Then

                        Return CType(settingControl, DropDownList).SelectedValue

                    ElseIf TypeOf settingControl Is Label Then

                        Return CType(settingControl, Label).Text

                    ElseIf TypeOf settingControl Is CheckBox Then

                        Dim settingControlChecked As Boolean = CType(settingControl, CheckBox).Checked

                        If SettingType = SettingTypes.CheckBoxTrueOrFalse Then
                            ' if check box true or false then just return as is
                            Return settingControlChecked.ToString()
                        Else
                            ' else check box should return 0 or 1 instead of true or false
                            Return IIf(settingControlChecked, "1", "0")
                        End If

                    End If

                End If

                Return Nothing

            End Get
        End Property

        ''' <summary>
        ''' Gets the description for this setting value
        ''' </summary>
        ''' <value>Description for this setting</value>
        ''' <returns>Description for this setting</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SettingValueDescription() As String
            Get

                Dim settingControl As Control = SettingValueControl

                ' we might not always have a control i.e. when the InitControl hasn't been called
                If Not IsNothing(settingControl) Then

                    If TypeOf settingControl Is TextBox Then

                        Return CType(settingControl, TextBox).Text

                    ElseIf TypeOf settingControl Is TextBoxEx Then

                        Return CType(settingControl, TextBoxEx).Text

                    ElseIf TypeOf settingControl Is DropDownList Then

                        Return CType(settingControl, DropDownList).SelectedItem.Text

                    ElseIf TypeOf settingControl Is Label Then

                        Return CType(settingControl, Label).Text

                    ElseIf TypeOf settingControl Is CheckBox Then

                        Return CType(settingControl, CheckBox).Checked.ToString()

                    End If

                End If

                Return Nothing

            End Get
        End Property

        ''' <summary>
        ''' Gets if the setting value has changed since the last Save()
        ''' </summary>
        ''' <value>If the setting value has changed since the last Save()</value>
        ''' <returns>If the setting value has changed since the last Save()</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SettingValueHasChanged() As Boolean
            Get
                If String.Compare(SettingValue, SettingLastValue, True) = 0 Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets if this setting is editable
        ''' </summary>
        ''' <value>If this setting is editable</value>
        ''' <returns>If this setting is editable</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SettingEditable() As Boolean
            Get

                Dim testBoolean As Boolean

                If Boolean.TryParse(hdnSettingEditable.Value, testBoolean) Then
                    Return True
                Else
                    Return False
                End If

            End Get
        End Property

        ''' <summary>
        ''' Gets the control used to store the setting value
        ''' </summary>
        ''' <value>The control used to store the setting value</value>
        ''' <returns>The control used to store the setting value</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property SettingValueControl() As Control
            Get

                Dim controlToReturn As Control = Nothing

                If Not IsNothing(phSettingControls.FindControl(SettingValueTextBoxName)) Then

                    controlToReturn = phSettingControls.FindControl(SettingValueTextBoxName)

                ElseIf Not IsNothing(phSettingControls.FindControl(SettingValueDropDownName)) Then

                    controlToReturn = phSettingControls.FindControl(SettingValueDropDownName)

                ElseIf Not IsNothing(phSettingControls.FindControl(SettingValueLabelName)) Then

                    controlToReturn = phSettingControls.FindControl(SettingValueLabelName)

                ElseIf Not IsNothing(phSettingControls.FindControl(SettingValueCheckBoxName)) Then

                    controlToReturn = phSettingControls.FindControl(SettingValueCheckBoxName)

                End If

                Return controlToReturn

            End Get
        End Property

        ''' <summary>
        ''' Gets the validator for this setting
        ''' </summary>
        ''' <value>The validator for this setting</value>
        ''' <returns>The validator for this setting</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property SettingValidator() As RegularExpressionValidator
            Get
                Return TryCast(phSettingControls.FindControl(SettingValueValidatorName),  _
                                RegularExpressionValidator)
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the setting value textbox
        ''' </summary>
        ''' <value>The name to be used for the setting value textbox</value>
        ''' <returns>The name to be used for the setting value textbox</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property SettingValueTextBoxName() As String
            Get
                Return String.Format("tbApplicationSetting_{0}", _
                                     IIf(SettingID.HasValue, _
                                         SettingID.Value, _
                                         "0"))
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the setting value drop down
        ''' </summary>
        ''' <value>The name to be used for the setting value drop down</value>
        ''' <returns>The name to be used for the setting value drop down</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property SettingValueDropDownName() As String
            Get
                Return String.Format("ddApplicationSetting_{0}", _
                                     IIf(SettingID.HasValue, _
                                         SettingID.Value, _
                                         "0"))
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the setting value label
        ''' </summary>
        ''' <value>The name to be used for the setting value label</value>
        ''' <returns>The name to be used for the setting value label</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property SettingValueLabelName() As String
            Get
                Return String.Format("lblApplicationSetting_{0}", _
                                     IIf(SettingID.HasValue, _
                                         SettingID.Value, _
                                         "0"))
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the setting value check box.
        ''' </summary>
        ''' <value>The name of the setting value check box.</value>
        Private ReadOnly Property SettingValueCheckBoxName() As String
            Get
                Return String.Format("cbApplicationSetting_{0}", _
                                     IIf(SettingID.HasValue, _
                                         SettingID.Value, _
                                         "0"))
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the setting value validator
        ''' </summary>
        ''' <value>The name to be used for the setting value validator</value>
        ''' <returns>The name to be used for the setting value validator</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property SettingValueValidatorName() As String
            Get
                Return String.Format("revApplicationSetting_{0}", _
                                     IIf(SettingID.HasValue, _
                                         SettingID.Value, _
                                         "0"))
            End Get
        End Property

#End Region

    End Class

End Namespace




