Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports Target.Library.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports AjaxControlToolkit
Imports Target.Library.Web.Controls
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' <summary>
    ''' User control which allows the modification of Service User Minutes Calculation Method
    ''' settings, which are an extended form of ApplicationSetting values.
    ''' </summary>
    ''' <history>
    ''' John Finch        16/10/2013   Corrupted record-saving and screen-handling now rectified (#8228)
    ''' John Finch        D12051   Created 27/07/2011
    ''' </history>
    Partial Public Class ucServiceUserMinutesCalc
        Inherits System.Web.UI.UserControl

#Region "Enums"

        Private Enum CalcMethodSetting As Byte
            NotSet = 0
            LowestOfClaimedPaid = 1
            LowestOfClaimedPaidActual = 2
            Actual = 3
            Paid = 4
        End Enum

#End Region

#Region "Module Variables"

        Private Const _JavascriptPath As String = "~/AbacusWeb/Apps/UserControls/ucServiceUserMinutesCalc.js"
        Private Const _SizePercentTextBox As Double = 50
        Private Const _SizePercentDropdown As Double = 100
        Private _LeaveIDsIntact As Boolean

#End Region

#Region "Page Events"

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            '++ Register the associated Javascript file for this user control..
            BaseWebPage.JsLinks.Add(Me.JavascriptPath)
            'SetupJavascript()
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            SetupJavascript()
        End Sub

        ''' <summary>
        ''' Server-side custom validator.
        ''' </summary>
        Protected Sub validatorServiceUserMinutesCalc_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
            args.IsValid = True

            '++ No validation needed if the first MinutesTo returns an empty postback,
            '++ since the screen must be disabled..
            If txtMinsTo1.GetPostBackValue Is Nothing Then Return

            '++ Any one of the following conditions within the control is invalid..
            If txtMinsTo1.GetPostBackValue = "" And (cboCalcMethod1.GetPostBackValue = CStr(CalcMethodSetting.NotSet)) Then
                args.IsValid = False
                validatorServiceUserMinutesCalc.ErrorMessage = "A Calculation Method is required for the first range."
                Exit Sub
            End If

            If (txtMinsFrom2.GetPostBackValue <> "" AndAlso (cboCalcMethod2.GetPostBackValue = CStr(CalcMethodSetting.NotSet))) Then
                args.IsValid = False
                validatorServiceUserMinutesCalc.ErrorMessage = "A Calculation Method is required for the second range."
                Exit Sub
            End If

            If (txtMinsFrom3.GetPostBackValue <> "" AndAlso (cboCalcMethod3.GetPostBackValue = CStr(CalcMethodSetting.NotSet))) Then
                args.IsValid = False
                validatorServiceUserMinutesCalc.ErrorMessage = "A Calculation Method is required for the third range."
                Exit Sub
            End If

            If txtMinsTo1.GetPostBackValue <> "" AndAlso Utils.ToNumeric(txtMinsTo1.GetPostBackValue) > Byte.MaxValue Then
                args.IsValid = False
                validatorServiceUserMinutesCalc.ErrorMessage = "Minutes values exceeding 255 are not permitted."
                Exit Sub
            End If

            If txtMinsTo2.GetPostBackValue <> "" AndAlso Utils.ToNumeric(txtMinsTo2.GetPostBackValue) > Byte.MaxValue Then
                args.IsValid = False
                validatorServiceUserMinutesCalc.ErrorMessage = "Minutes values exceeding 255 are not permitted."
                Exit Sub
            End If

            If (txtMinsFrom1.GetPostBackValue <> "" AndAlso txtMinsTo1.GetPostBackValue <> "" _
                    AndAlso (Convert.ToInt32(txtMinsFrom1.GetPostBackValue) > Convert.ToInt32(txtMinsTo1.GetPostBackValue))) Then
                args.IsValid = False
                validatorServiceUserMinutesCalc.ErrorMessage = _
                    "The Minutes To is less than the Minutes From for the first range."
                Exit Sub
            End If

            If (txtMinsFrom2.GetPostBackValue <> "" AndAlso txtMinsTo2.GetPostBackValue <> "" _
                    AndAlso (Convert.ToInt32(txtMinsFrom2.GetPostBackValue) > Convert.ToInt32(txtMinsTo2.GetPostBackValue))) Then
                args.IsValid = False
                validatorServiceUserMinutesCalc.ErrorMessage = _
                    "The Minutes To is less than the Minutes From for the second range."
                Exit Sub
            End If

            If txtMinsFrom1.GetPostBackValue = "" AndAlso _
                    ((cboCalcMethod2.GetPostBackValue <> CStr(CalcMethodSetting.NotSet)) _
                        Or (cboCalcMethod3.GetPostBackValue <> CStr(CalcMethodSetting.NotSet))) Then
                args.IsValid = False
                validatorServiceUserMinutesCalc.ErrorMessage = "A Minutes To value is required for the first range."
                Exit Sub
            End If

            If txtMinsFrom2.GetPostBackValue = "" AndAlso _
                    (cboCalcMethod3.GetPostBackValue <> CStr(CalcMethodSetting.NotSet)) Then
                args.IsValid = False
                validatorServiceUserMinutesCalc.ErrorMessage = "A Minutes To value is required for the second range."
                Exit Sub
            End If
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the base web page.
        ''' </summary>
        ''' <value>The base web page.</value>
        Private ReadOnly Property BaseWebPage() As BasePage
            Get
                Return CType(Me.Page, BasePage)
            End Get
        End Property

        Private ReadOnly Property SettingEditableName() As String
            Get
                If _LeaveIDsIntact Then
                    Return "hdnSettingEditable"
                Else
                    Return String.Format("hdnSettingEditable_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        Private ReadOnly Property SettingIDName() As String
            Get
                If _LeaveIDsIntact Then
                    Return "hdnSettingID"
                Else
                    Return String.Format("hdnSettingID_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        Private ReadOnly Property SettingDisplayModeName() As String
            Get
                If _LeaveIDsIntact Then
                    Return "hidDisplayMode"
                Else
                    Return String.Format("hidDisplayMode_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        Private ReadOnly Property ValidatorName() As String
            Get
                If _LeaveIDsIntact Then
                    Return "validatorServiceUserMinutesCalc"
                Else
                    Return String.Format("validatorServiceUserMinutesCalc_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        Private ReadOnly Property FrameSettingsName() As String
            Get
                If _LeaveIDsIntact Then
                    Return "fsSettings"
                Else
                    Return String.Format("fsSettings_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the 1st Minutes From textbox
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly Property FieldMinsFrom1Name() As String
            Get
                If _LeaveIDsIntact Then
                    Return "txtMinsFrom1"
                Else
                    Return String.Format("txtMinsFrom1_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the 2nd Minutes From textbox
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly Property FieldMinsFrom2Name() As String
            Get
                If _LeaveIDsIntact Then
                    Return "txtMinsFrom2"
                Else
                    Return String.Format("txtMinsFrom2_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the 3rd Minutes From textbox
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly Property FieldMinsFrom3Name() As String
            Get
                If _LeaveIDsIntact Then
                    Return "txtMinsFrom3"
                Else
                    Return String.Format("txtMinsFrom3_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the 1st Minutes To textbox
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly Property FieldMinsTo1Name() As String
            Get
                If _LeaveIDsIntact Then
                    Return "txtMinsTo1"
                Else
                    Return String.Format("txtMinsTo1_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the 2nd Minutes To textbox
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly Property FieldMinsTo2Name() As String
            Get
                If _LeaveIDsIntact Then
                    Return "txtMinsTo2"
                Else
                    Return String.Format("txtMinsTo2_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the 3rd Minutes To textbox
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly Property FieldMinsTo3Name() As String
            Get
                If _LeaveIDsIntact Then
                    Return "txtMinsTo3"
                Else
                    Return String.Format("txtMinsTo3_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the 1st Calculation Method dropdown
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly Property FieldCalcMethod1Name() As String
            Get
                If _LeaveIDsIntact Then
                    Return "cboCalcMethod1"
                Else
                    Return String.Format("cboCalcMethod1_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the 2nd Calculation Method dropdown
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly Property FieldCalcMethod2Name() As String
            Get
                If _LeaveIDsIntact Then
                    Return "cboCalcMethod2"
                Else
                    Return String.Format("cboCalcMethod2_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the name to be used for the 3rd Calculation Method dropdown
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly Property FieldCalcMethod3Name() As String
            Get
                If _LeaveIDsIntact Then
                    Return "cboCalcMethod3"
                Else
                    Return String.Format("cboCalcMethod3_{0}", IIf(SettingID.HasValue, SettingID.Value, "0"))
                End If
            End Get
        End Property

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
        ''' Gets or sets the MinutesFrom1 field content
        ''' </summary>
        ''' <value>The 'Minutes From' to be set.</value>
        Public Property MinutesFrom1() As Object
            Get
                Dim objValue As Object = txtMinsFrom1.GetPostBackValue
                If objValue = "" Then
                    Return Nothing
                Else
                    Return objValue
                End If
            End Get
            Set(ByVal value As Object)
                If Convert.IsDBNull(value) OrElse (Not IsNumeric(value)) Then
                    txtMinsFrom1.Text = ""
                Else
                    txtMinsFrom1.Text = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the MinutesTo1 field content
        ''' </summary>
        ''' <value>The 'Minutes To' to be set.</value>
        Public Property MinutesTo1() As Object
            Get
                Dim objValue As Object = txtMinsTo1.GetPostBackValue
                If objValue = "" Then
                    Return Nothing
                Else
                    Return objValue
                End If
            End Get
            Set(ByVal value As Object)
                If Convert.IsDBNull(value) OrElse (Not IsNumeric(value)) OrElse Utils.ToNumeric(value) = 0 Then
                    txtMinsTo1.Text = ""
                Else
                    txtMinsTo1.Text = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the current selection in the CalcMethod1 dropdown
        ''' </summary>
        ''' <value>The 'Calculation Method' to be set.</value>
        Public Property CalcMethod1() As Object
            Get
                Dim objValue As Object = cboCalcMethod1.GetPostBackValue
                If objValue = "" Then
                    Return Nothing
                Else
                    Return objValue
                End If
            End Get
            Set(ByVal value As Object)
                If Convert.IsDBNull(value) Then
                    cboCalcMethod1.DropDownList.SelectedValue = CalcMethodSetting.NotSet.ToString
                ElseIf IsNumeric(value) AndAlso [Enum].IsDefined(GetType(CalcMethodSetting), value) Then
                    cboCalcMethod1.DropDownList.SelectedValue = value
                Else
                    cboCalcMethod1.DropDownList.SelectedValue = CalcMethodSetting.NotSet.ToString
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the MinutesFrom2 field content
        ''' </summary>
        ''' <value>The 'Minutes From' to be set.</value>
        Public Property MinutesFrom2() As String
            Get
                Dim objValue As Object = txtMinsFrom2.GetPostBackValue
                If objValue = "" Then
                    Return Nothing
                Else
                    Return objValue
                End If
            End Get
            Set(ByVal value As String)
                If Convert.IsDBNull(value) OrElse (Not IsNumeric(value)) OrElse Utils.ToNumeric(value) = 0 Then
                    txtMinsFrom2.Text = ""
                Else
                    txtMinsFrom2.Text = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the MinutesTo2 field content
        ''' </summary>
        ''' <value>The 'Minutes To' to be set.</value>
        Public Property MinutesTo2() As String
            Get
                Dim objValue As Object = txtMinsTo2.GetPostBackValue
                If objValue = "" Then
                    Return Nothing
                Else
                    Return objValue
                End If
            End Get
            Set(ByVal value As String)
                If Convert.IsDBNull(value) OrElse (Not IsNumeric(value)) OrElse Utils.ToNumeric(value) = 0 Then
                    txtMinsTo2.Text = ""
                Else
                    txtMinsTo2.Text = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the current selection in the CalcMethod2 dropdown
        ''' </summary>
        ''' <value>The 'Calculation Method' to be set.</value>
        Public Property CalcMethod2() As Object
            Get
                Dim objValue As Object = cboCalcMethod2.GetPostBackValue
                If objValue = "" Then
                    Return Nothing
                Else
                    Return objValue
                End If
            End Get
            Set(ByVal value As Object)
                If Convert.IsDBNull(value) Then
                    cboCalcMethod2.DropDownList.SelectedValue = CalcMethodSetting.NotSet.ToString
                ElseIf IsNumeric(value) AndAlso [Enum].IsDefined(GetType(CalcMethodSetting), value) Then
                    cboCalcMethod2.DropDownList.SelectedValue = value
                Else
                    cboCalcMethod2.DropDownList.SelectedValue = CalcMethodSetting.NotSet.ToString
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the MinutesFrom3 field content
        ''' </summary>
        ''' <value>The 'Minutes From' to be set.</value>
        Public Property MinutesFrom3() As String
            Get
                Dim objValue As Object = txtMinsFrom3.GetPostBackValue
                If objValue = "" Then
                    Return Nothing
                Else
                    Return objValue
                End If
            End Get
            Set(ByVal value As String)
                If Convert.IsDBNull(value) OrElse (Not IsNumeric(value)) OrElse Utils.ToNumeric(value) = 0 Then
                    txtMinsFrom3.Text = ""
                Else
                    txtMinsFrom3.Text = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the MinutesTo3 field content
        ''' </summary>
        ''' <value>The 'Minutes To' to be set.</value>
        Public Property MinutesTo3() As String
            Get
                Return Nothing
            End Get
            Set(ByVal value As String)
                '++ The final MinutesTo is always shown blank..
                txtMinsTo3.Text = ""
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the current selection in the CalcMethod1 dropdown
        ''' </summary>
        ''' <value>The 'Calculation Method' to be set.</value>
        Public Property CalcMethod3() As Object
            Get
                Dim objValue As Object = cboCalcMethod3.GetPostBackValue
                If objValue = "" Then
                    Return Nothing
                Else
                    Return objValue
                End If
            End Get
            Set(ByVal value As Object)
                If Convert.IsDBNull(value) Then
                    cboCalcMethod3.DropDownList.SelectedValue = CalcMethodSetting.NotSet.ToString
                ElseIf IsNumeric(value) AndAlso [Enum].IsDefined(GetType(CalcMethodSetting), value) Then
                    cboCalcMethod3.DropDownList.SelectedValue = value
                Else
                    cboCalcMethod3.DropDownList.SelectedValue = CalcMethodSetting.NotSet.ToString
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets the javascript path for this user control.
        ''' </summary>
        ''' <value>The javascript path.</value>
        Private ReadOnly Property JavascriptPath() As String
            Get
                Return Target.Library.Web.Utils.GetVirtualPath(_JavascriptPath)
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Initialise the control i.e. populate setting value to be viewed/edited
        ''' </summary>
        ''' <param name="basePage">The base page to use, this page will contain an open database connection</param>
        ''' <param name="appSetting">The application setting to view/edit</param>
        ''' <param name="calcMethod">The Service User Minutes Calc record to populate dropdowns with</param>
        ''' <param name="currentRecID">The current record to use for setting defaults</param>
        ''' <remarks></remarks>
        Public Sub InitControl(ByVal basePage As BasePage, _
                               ByVal appSetting As ApplicationSetting, _
                               ByVal calcMethod As ServiceUserMinutesCalculationMethod, _
                               ByVal currentRecID As Integer)

            Const TOOLTIP_MINSFROM As String = "The lower end of the minute range"
            Const TOOLTIP_MINSTO As String = "The upper end of the minute range"
            Const TOOLTIP_CALCMETHOD As String = "The selected Calculation Method option"
            Dim msg As New ErrorMessage()

            If appSetting Is Nothing Then
                '++ Find an ApplicationSetting record with an ApplicationSettingType of 2 (Service User Min Calc)..
                Dim appSettingRecs As ApplicationSettingCollection = Nothing
                msg = ApplicationSetting.FetchList(conn:=basePage.DbConnection, list:=appSettingRecs, _
                            auditUserName:="", auditLogTitle:="", applicationSettingType:=2)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If appSettingRecs.Count > 0 Then
                    appSetting = appSettingRecs.Item(0)
                End If
            End If

            If currentRecID = Nothing Then
                currentRecID = calcMethod.ID
                _LeaveIDsIntact = True
            Else
                _LeaveIDsIntact = False
            End If

            hdnSettingID.Value = currentRecID
            hdnSettingEditable.Value = appSetting.Editable.ToString()

            '++ Only display visible settings..
            If appSetting.Visible Then
                '++ Make the user control editable or not based on the ApplicationSetting flag..
                SetSettingReadOnly(Not appSetting.Editable)

                If appSetting.Editable Then
                    hdnSettingEditable.ID = SettingEditableName
                    hdnSettingID.ID = SettingIDName
                    hidDisplayMode.ID = SettingDisplayModeName
                    validatorServiceUserMinutesCalc.ID = ValidatorName
                    fsSettings.ID = FrameSettingsName

                    With txtMinsFrom1
                        .ID = FieldMinsFrom1Name
                        .TextBox.ToolTip = TOOLTIP_MINSFROM
                        .TextBox.Width = Unit.Percentage(_SizePercentTextBox)

                        MinutesFrom1 = calcMethod.MinutesFrom1
                    End With

                    With txtMinsTo1
                        .ID = FieldMinsTo1Name
                        .TextBox.ToolTip = TOOLTIP_MINSTO
                        .TextBox.Width = Unit.Percentage(_SizePercentTextBox)
                        MinutesTo1 = calcMethod.MinutesTo1
                    End With

                    With cboCalcMethod1
                        .ID = FieldCalcMethod1Name
                        .DropDownList.ToolTip = TOOLTIP_CALCMETHOD
                        .DropDownList.Width = Unit.Percentage(_SizePercentDropdown)
                    End With
                    msg = PopulateDropdownList(basePage, cboCalcMethod1, appSetting, Utils.ToNumeric(calcMethod.CalculationMethod1))
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With txtMinsFrom2
                        .ID = FieldMinsFrom2Name
                        .TextBox.ToolTip = TOOLTIP_MINSFROM
                        .TextBox.Width = Unit.Percentage(_SizePercentTextBox)

                        MinutesFrom2 = calcMethod.MinutesFrom2
                    End With

                    With txtMinsTo2
                        .ID = FieldMinsTo2Name
                        .TextBox.ToolTip = TOOLTIP_MINSTO
                        .TextBox.Width = Unit.Percentage(_SizePercentTextBox)

                        MinutesTo2 = calcMethod.MinutesTo2
                    End With

                    With cboCalcMethod2
                        .ID = FieldCalcMethod2Name
                        .DropDownList.ToolTip = TOOLTIP_CALCMETHOD
                        .DropDownList.Width = Unit.Percentage(_SizePercentDropdown)
                    End With
                    msg = PopulateDropdownList(basePage, cboCalcMethod2, appSetting, Utils.ToNumeric(calcMethod.CalculationMethod2))
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    With txtMinsFrom3
                        .ID = FieldMinsFrom3Name
                        .TextBox.ToolTip = TOOLTIP_MINSFROM
                        .TextBox.Width = Unit.Percentage(_SizePercentTextBox)

                        MinutesFrom3 = calcMethod.MinutesFrom3
                    End With

                    With txtMinsTo3
                        .ID = FieldMinsTo3Name
                        .TextBox.ToolTip = TOOLTIP_MINSTO
                        .TextBox.Width = Unit.Percentage(_SizePercentTextBox)

                        MinutesTo3 = calcMethod.MinutesTo3
                    End With

                    With cboCalcMethod3
                        .ID = FieldCalcMethod3Name
                        .DropDownList.ToolTip = TOOLTIP_CALCMETHOD
                        .DropDownList.Width = Unit.Percentage(_SizePercentDropdown)
                    End With
                    msg = PopulateDropdownList(basePage, cboCalcMethod3, appSetting, Utils.ToNumeric(calcMethod.CalculationMethod3))
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
            End If

        End Sub

        ''' <summary>
        ''' Sets this setting as Read Only or Not
        ''' </summary>
        ''' <param name="isReadOnly">If setting should be read only</param>
        ''' <remarks>Please note this will have no effect on uneditable settings</remarks>
        Public Sub SetSettingReadOnly(ByVal isReadOnly As Boolean)

            If SettingEditable Then
                '++ All 'Minutes From' fields are always read-only..
                txtMinsFrom1.TextBox.ReadOnly = True
                txtMinsFrom1.TextBox.Enabled = Not isReadOnly
                txtMinsFrom2.TextBox.ReadOnly = True
                txtMinsFrom2.TextBox.Enabled = Not isReadOnly
                txtMinsFrom3.TextBox.ReadOnly = True
                txtMinsFrom3.TextBox.Enabled = Not isReadOnly
                '++ as well as the final 'Minutes To' field..
                txtMinsTo3.TextBox.ReadOnly = True
                txtMinsTo3.TextBox.Enabled = Not isReadOnly

                txtMinsTo1.TextBox.Enabled = Not isReadOnly
                txtMinsTo2.TextBox.Enabled = Not isReadOnly

                If isReadOnly Then
                    txtMinsTo1.TextBox.ReadOnly = True
                    txtMinsTo2.TextBox.ReadOnly = True

                    cboCalcMethod1.DropDownList.Enabled = False
                    cboCalcMethod2.DropDownList.Enabled = False
                    cboCalcMethod3.DropDownList.Enabled = False
                Else
                    txtMinsTo1.TextBox.ReadOnly = False
                    cboCalcMethod1.DropDownList.Enabled = True

                    '++ Enable certain controls depending on their current selection/content..
                    If txtMinsTo1.TextBox.Text <> "" Then
                        cboCalcMethod2.DropDownList.Enabled = True
                        txtMinsTo2.TextBox.ReadOnly = False
                    End If

                    If txtMinsTo2.TextBox.Text <> "" Then
                        cboCalcMethod3.DropDownList.Enabled = True
                    End If
                End If
            End If

        End Sub

        ''' <summary>
        ''' Clears the content of the entire control into a default entry state.
        ''' </summary>
        Public Sub Reset()
            txtMinsFrom1.TextBox.Text = "0"
            txtMinsFrom2.TextBox.Text = ""
            txtMinsFrom3.TextBox.Text = ""
            txtMinsTo1.TextBox.Text = ""
            txtMinsTo2.TextBox.Text = ""
            txtMinsTo3.TextBox.Text = ""
            If cboCalcMethod1.DropDownList.Items.Count > 0 Then
                cboCalcMethod1.DropDownList.SelectedValue = CStr(CalcMethodSetting.NotSet)
            End If
            cboCalcMethod1.DropDownList.Enabled = True
            If cboCalcMethod2.DropDownList.Items.Count > 0 Then
                cboCalcMethod2.DropDownList.SelectedValue = CStr(CalcMethodSetting.NotSet)
            End If
            cboCalcMethod2.DropDownList.Enabled = True
            If cboCalcMethod3.DropDownList.Items.Count > 0 Then
                cboCalcMethod3.DropDownList.SelectedValue = CStr(CalcMethodSetting.NotSet)
            End If
            cboCalcMethod3.DropDownList.Enabled = True
        End Sub
#End Region

#Region "Local Routines"

        ''' <summary>
        ''' Sets up the javascript for this user control.
        ''' </summary>
        Private Sub SetupJavascript()

            Const SCRIPT_STARTUP As String = "ucServiceUserMinutesCalc.Init"
            Dim js As StringBuilder = New StringBuilder()

            '++ Set up the required Javascript change events..
            cboCalcMethod2.DropDownList.Attributes.Add("onchange", String.Format("cboCalcMethod2_OnChange('{0}')", cboCalcMethod2.ClientID))
            cboCalcMethod3.DropDownList.Attributes.Add("onchange", String.Format("cboCalcMethod2_OnChange('{0}')", cboCalcMethod3.ClientID))

            '++ Create a script that sets IDs of constituent controls for this user control..
            js.AppendFormat("txtMinsFrom1ID='{0}';", txtMinsFrom1.ClientID)
            js.AppendFormat("txtMinsTo1ID='{0}';", txtMinsTo1.ClientID)
            js.AppendFormat("cboCalcMethod1ID='{0}';", cboCalcMethod1.ClientID)
            js.AppendFormat("txtMinsFrom2ID='{0}';", txtMinsFrom2.ClientID)
            js.AppendFormat("txtMinsTo2ID='{0}';", txtMinsTo2.ClientID)
            js.AppendFormat("cboCalcMethod2ID='{0}';", cboCalcMethod2.ClientID)
            js.AppendFormat("txtMinsFrom3ID='{0}';", txtMinsFrom3.ClientID)
            js.AppendFormat("txtMinsTo3ID='{0}';", txtMinsTo3.ClientID)
            js.AppendFormat("cboCalcMethod3ID='{0}';", cboCalcMethod3.ClientID)
            js.AppendFormat("function {0}_Changed(id) {{ txtMinsTo1_OnChange(id); }};", txtMinsTo1.ID)
            js.AppendFormat("function {0}_Changed(id) {{ txtMinsTo2_OnChange(id); }};", txtMinsTo2.ID)

            '++ Register the additional startup script for this user control..
            BaseWebPage.ClientScript.RegisterStartupScript(BaseWebPage.GetType(), _
                                                               SCRIPT_STARTUP, _
                                                               WebUtils.WrapClientScript(js.ToString))

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

            ' use a reader to get all values back from the provided stored proc
            Using reader As SqlDataReader = SqlHelper.ExecuteReader(sqlConnection, _
                                                                    CommandType.StoredProcedure, _
                                                                    sqlProcedureName)

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
                '++ If LookupStoredProcedure is defined, retrieve drop down values from this stored procedure..
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
                '++ Else try the application setting value table for data..
                Dim applicationSettingValues As New ApplicationSettingValueCollection

                '++ Fetch a list of application setting values..
                msg = ApplicationSettingValue.FetchList(conn:=basePage.DbConnection, _
                                                        list:=applicationSettingValues, _
                                                        applicationSettingID:=applicationSetting.ID)
                If Not msg.Success Then Return msg

                If Not applicationSettingValues Is Nothing AndAlso applicationSettingValues.Count > 0 Then
                    applicationSettingValues.Sort(New CollectionSorter("SortOrder", SortDirection.Ascending))

                    '++ Loop each setting value and add into items list..
                    For Each settingValue As ApplicationSettingValue In applicationSettingValues
                        items.Add(New ListItem(settingValue.DisplayText, settingValue.Value))
                    Next

                End If
                items.Insert(0, New ListItem(String.Empty, "0"))

            End If

            msg = New ErrorMessage()
            msg.Success = True

            Return msg

        End Function

        Private Function PopulateDropdownList(ByVal basePage As BasePage, ByRef cboControl As DropDownListEx, _
                                              ByVal appSetting As ApplicationSetting, _
                                              ByVal selectedItem As Long) As ErrorMessage
            Dim msg As New ErrorMessage
            Dim applicationSettingDropDownListItemToSelect As ListItem
            Dim applicationSettingDropDownListItems As New List(Of ListItem)()

            msg = GetDropDownListItems(basePage, appSetting, applicationSettingDropDownListItems)
            If msg.Success Then
                If Not applicationSettingDropDownListItems Is Nothing AndAlso applicationSettingDropDownListItems.Count > 0 Then
                    '++ Add the items into the drop down list..
                    cboControl.DropDownList.Items.Clear()
                    cboControl.DropDownList.Items.AddRange(applicationSettingDropDownListItems.ToArray())

                    If cboControl.DropDownList.Items.Count > 0 Then
                        '++ If there are items, try and select a value..
                        applicationSettingDropDownListItemToSelect = cboControl.DropDownList.Items.FindByValue(selectedItem.ToString)

                        If Not IsNothing(applicationSettingDropDownListItemToSelect) Then
                            '++ If we have an item in the list that matches the value from the db then select it..
                            applicationSettingDropDownListItemToSelect.Selected = True
                        Else
                            '++ ..otherwise select the first item in the list..
                            cboControl.DropDownList.Items(0).Selected = True
                        End If
                    End If
                End If
            End If
            Return msg
        End Function
#End Region
    End Class

End Namespace

