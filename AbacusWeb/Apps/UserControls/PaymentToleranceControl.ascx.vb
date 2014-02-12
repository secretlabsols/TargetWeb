Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Abacus.Library.Notes
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library
Imports Target.Abacus.Library.PaymentTolerance

Namespace Apps.UserControls

    ''' <summary>
    ''' Class representing a set of payment tolerance settings
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   MoTahir   20/11/2011  BTI-367 Payment Tolerances - Error saving contract changes  
    '''   MoTahir   20/10/2011  BTI241 Orange text not displayed on Payment Tolerance Group node on contract. 
    '''   Motahir   12/10/2011  B162 Beta 2 Testing Issue 194 - Payments Tolerance TAB on service order displays no information. 
    '''   MoTahir   04/10/2011 Beta Testing Issue 136 -  Drop down blank when setting up 
    '''                        visit-based Service Payment Tolerance Group on Contract 
    '''   MoTahir   07/04/2011 D11766 - Created - eInvoicing Provider Invoice Tolerances 
    ''' </history>
    Partial Public Class PaymentToleranceControl
        Inherits System.Web.UI.UserControl

#Region " Constants "

        ' constants
        Private Const CONTROL_NAME As String = "PaymentToleranceControl"
        Private Const JAVASCRIPT_PATH As String = "AbacusWeb/Apps/UserControls/PaymentToleranceControl.js"
        Private Const AND_TOL_COMBI_MESSAGE As String = "Units and Cost Payment Tolerance conditions must be met.  If either tolerance" _
                                              & " is exceeded the Provider Invoice will be suspended."
        Private Const OR_TOL_COMBI_MESSAGE As String = "At least one of the Payment Tolerance conditions must be met; the Provider" _
                                             & " Invoice will only be suspended where both tolerances are exceeded."
        Private Const DROP_DOWN_LIST As String = "_cboDropDownList"
        Private Const CHECKBOX As String = "_chkCheckbox"

        Private Const _CurrencyFormat As String = "0.00"

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the base web page.
        ''' </summary>
        ''' <value>The base web page.</value>
        Private ReadOnly Property MyBasePage() As BasePage
            Get
                Return CType(Me.Page, BasePage)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets rate category descriptions.
        ''' </summary>
        ''' <value>The selected id.</value>
        Public Property RateCategoryDescriptions() As String
            Get
                Return lblRateCategoryDescriptions.Text
            End Get
            Set(ByVal value As String)
                lblRateCategoryDescriptions.Text = value
            End Set
        End Property


        ''' <summary>
        ''' Gets or sets selected payment tolerance id.
        ''' </summary>
        ''' <value>The selected id.</value>
        Public Property PaymentToleranceID() As Integer
            Get
                Return Integer.Parse(hidPaymentToleranceID.Value)
            End Get
            Set(ByVal value As Integer)
                hidPaymentToleranceID.Value = CType(value, Integer).ToString()
                IsPaymentToleranceSet = True
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets selected payment tolerance type.
        ''' </summary>
        ''' <value>The selected payment tolerance type.</value>
        Public Property PaymentToleranceDisplayMode() As PaymentToleranceDisplayMode
            Get
                Return CType(Integer.Parse(hidPaymentToleranceDisplayMode.Value), PaymentToleranceDisplayMode)
            End Get
            Set(ByVal value As PaymentToleranceDisplayMode)
                hidPaymentToleranceDisplayMode.Value = CType(value, Integer).ToString()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets selected payment tolerance group system type.
        ''' </summary>
        ''' <value>The selected payment tolerance group system type</value>
        Public Property PaymentToleranceGroupSystemType() As PaymentToleranceGroupSystemTypes
            Get
                Return CType(Integer.Parse(hidPaymentToleraceGroupSystemType.Value), PaymentToleranceGroupSystemTypes)
            End Get
            Set(ByVal value As PaymentToleranceGroupSystemTypes)
                hidPaymentToleraceGroupSystemType.Value = CType(value, Integer).ToString()
                SetUpControlView()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets acceptable additional units.
        ''' </summary>
        ''' <value>The selected acceptable additional unit</value>
        Public Property AcceptableAdditionalUnits() As Single
            Get
                Return Single.Parse(IIf(Trim(txtAcceptableAdditionalUnits.TextBox.Text) = String.Empty, _
                                        Target.Library.Utils.ToDecimal(txtAcceptableAdditionalUnits.Text).ToString(_CurrencyFormat), _
                                        Trim(txtAcceptableAdditionalUnits.TextBox.Text)))
            End Get
            Set(ByVal value As Single)
                txtAcceptableAdditionalUnits.TextBox.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets acceptable additional units as percentage.
        ''' </summary>
        ''' <value>The acceptable additional units as percentage</value>
        Public Property AcceptableAdditionalUnitsAsPercentage() As Integer
            Get
                Return Integer.Parse(IIf(Trim(txtAcceptableAdditionalUnitsAsPercentage.TextBox.Text) = String.Empty, _
                                    "0", _
                                    Trim(txtAcceptableAdditionalUnitsAsPercentage.TextBox.Text)))
            End Get
            Set(ByVal value As Integer)
                txtAcceptableAdditionalUnitsAsPercentage.TextBox.Text = value.ToString
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets units of service capped at.
        ''' </summary>
        ''' <value>The units of service capped at</value>
        Public Property UnitsOfServiceCappedAt() As Single
            Get
                Return Single.Parse(IIf(Trim(txtUnitsOfServiceCappedAt.TextBox.Text) = String.Empty, _
                        Target.Library.Utils.ToDecimal(txtUnitsOfServiceCappedAt.Text).ToString(_CurrencyFormat), _
                        Trim(txtUnitsOfServiceCappedAt.TextBox.Text)))
            End Get
            Set(ByVal value As Single)
                txtUnitsOfServiceCappedAt.TextBox.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets acceptable additional payment.
        ''' </summary>
        ''' <value>The acceptable additional payment</value>
        Public Property AcceptableAdditionalPayment() As Decimal
            Get
                Return Decimal.Parse(IIf(Trim(txtAcceptableAdditionalPayment.TextBox.Text) = String.Empty, _
                                    Target.Library.Utils.ToDecimal(txtAcceptableAdditionalPayment.Text).ToString(_CurrencyFormat), _
                                    Trim(txtAcceptableAdditionalPayment.TextBox.Text)))
            End Get
            Set(ByVal value As Decimal)
                txtAcceptableAdditionalPayment.TextBox.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets acceptable additional payment as percentage.
        ''' </summary>
        ''' <value>The acceptable additional payment as percentage</value>
        Public Property AcceptableAdditionalPaymentAsPercentage() As Integer
            Get
                Return Integer.Parse(IIf(Trim(txtAcceptableAdditionalPaymentAsPercentage.TextBox.Text) = String.Empty, _
                                  "0", _
                                  Trim(txtAcceptableAdditionalPaymentAsPercentage.TextBox.Text)))
            End Get
            Set(ByVal value As Integer)
                txtAcceptableAdditionalPaymentAsPercentage.TextBox.Text = value.ToString
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets cost of service capped at.
        ''' </summary>
        ''' <value>The cost of service capped at</value>
        Public Property CostOfServiceCappedAt() As Decimal
            Get
                Return Decimal.Parse(IIf(Trim(txtCostOfServiceCappedAt.TextBox.Text) = String.Empty, _
                                  Target.Library.Utils.ToDecimal(txtCostOfServiceCappedAt.Text).ToString(_CurrencyFormat), _
                                  Trim(txtCostOfServiceCappedAt.TextBox.Text)))
            End Get
            Set(ByVal value As Decimal)
                txtCostOfServiceCappedAt.TextBox.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets suspend invoice when planned units exceeded.
        ''' </summary>
        ''' <value>The susupend invoice when planned units exceeded value</value>
        Public Property SuspendInvoiceWhenPlannedUnitsExceeded() As Boolean
            Get
                Return Boolean.Parse(chkSuspendInvoiceWhenPlannedUnitsExceeded.CheckBox.Checked)
            End Get
            Set(ByVal value As Boolean)
                chkSuspendInvoiceWhenPlannedUnitsExceeded.CheckBox.Checked = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the tolerance combination method.
        ''' </summary>
        ''' <value>The tolerance combination method </value>
        Public Property ToleranceCombiMethod() As ToleranceCombinationMethod
            Get

                'get the post back value if postback
                If IsPostBack Then
                    cboPaymentToleranceCombinationMethod.SelectPostBackValue()
                Else
                    If cboPaymentToleranceCombinationMethod.DropDownList.SelectedValue = String.Empty Then
                        'set the payment tolerance combination method from system settings
                        Dim settings As SystemSettings = SystemSettings.GetCachedSettings(MyBasePage.DbConnection.ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                        cboPaymentToleranceCombinationMethod.DropDownList.SelectedValue = DirectCast([Enum].Parse(GetType(ToleranceCombinationMethod), settings("DefaultToleranceCombinationMethod"), True), ToleranceCombinationMethod)
                    End If
                End If

                Return CType(Integer.Parse(cboPaymentToleranceCombinationMethod.DropDownList.SelectedValue), ToleranceCombinationMethod)

            End Get
            Set(ByVal value As ToleranceCombinationMethod)
                PopulateToleranceCombinationMethod()
                cboPaymentToleranceCombinationMethod.DropDownList.SelectedValue = CType(value, Integer).ToString()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets this property to determine whether status tolerances are set or not.
        ''' </summary>
        ''' <value>The Is payment tolerances set value.</value>
        Public Property IsPaymentToleranceSet() As Boolean
            Get
                Return Boolean.Parse(hidIsPaymentTolerancesSet.Value.ToString)
            End Get
            Set(ByVal value As Boolean)
                hidIsPaymentTolerancesSet.Value = CType(value, Boolean).ToString()
            End Set
        End Property

#End Region

#Region " Page_Events "

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            'SetupJavascript()

            If Not IsPostBack Then

                ' if first time hit this user control then set defaults
                SetupDefaults()

            End If

        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' if first time hit this user control then set the view
            If Not IsPostBack Then

                MyBasePage.UseJQuery = True

                'set up the view of the control
                SetUpControlView()

                'populate tolerance combination method
                If Not IsPaymentToleranceSet Then PopulateToleranceCombinationMethod()

            End If

            'SetTextBoxFormats()
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            'set the format of textboxes i.e. currency format etc
            'SetTextBoxFormats()
            If ToleranceCombiMethod = ToleranceCombinationMethod.And Then
                lblWarning.Text = AND_TOL_COMBI_MESSAGE
            Else
                lblWarning.Text = OR_TOL_COMBI_MESSAGE
            End If
        End Sub

#End Region

#Region " Methods "

#Region " SetupDefaults "
        ''' <summary>
        ''' Setups the defaults for job options.
        ''' </summary>
        Private Sub SetupDefaults()

            IsPaymentToleranceSet = False

        End Sub
#End Region

#Region " SetUpControlView "
        ''' <summary>
        ''' Setups what panels are displayed for the control based on payment tolerance group system type.
        ''' </summary>
        Private Sub SetUpControlView()

            ' hide and display appropriate panels based on tolerance group system type
            If PaymentToleranceGroupSystemType = PaymentToleranceGroupSystemTypes.VisitBasedServiceToleranceGroup Then
                pnlUserEnteredUnitsofService.Visible = False
                pnlVisitBasedUnitsofService.Visible = True
            Else
                pnlUserEnteredUnitsofService.Visible = True
                pnlVisitBasedUnitsofService.Visible = False
            End If

            SetupJavascript()

        End Sub
#End Region

#Region " PopulatePaymentToleranceSettings "
        ''' <summary>
        ''' Populate the payments tolerance settings.
        ''' </summary>
        Private Sub PopulatePaymentToleranceSettings()

            'populate based on payment tolerance group system type
            If PaymentToleranceGroupSystemType = PaymentToleranceGroupSystemTypes.VisitBasedServiceToleranceGroup Then
                txtAcceptableAdditionalUnits.TextBox.Text = AcceptableAdditionalUnits
                txtAcceptableAdditionalUnitsAsPercentage.TextBox.Text = AcceptableAdditionalUnitsAsPercentage
                txtUnitsOfServiceCappedAt.TextBox.Text = UnitsOfServiceCappedAt
            Else
                chkSuspendInvoiceWhenPlannedUnitsExceeded.CheckBox.Checked = SuspendInvoiceWhenPlannedUnitsExceeded
            End If

            cboPaymentToleranceCombinationMethod.DropDownList.SelectedValue = ToleranceCombiMethod

            txtAcceptableAdditionalPayment.TextBox.Text = AcceptableAdditionalPayment
            txtAcceptableAdditionalPaymentAsPercentage.TextBox.Text = AcceptableAdditionalPaymentAsPercentage
            txtCostOfServiceCappedAt.TextBox.Text = CostOfServiceCappedAt

        End Sub
#End Region

#Region " PopulateToleranceCombinationMethod "
        ''' <summary>
        ''' Populate the payments tolerance combination method
        ''' </summary>
        Private Sub PopulateToleranceCombinationMethod()

            'clear the combo box
            cboPaymentToleranceCombinationMethod.DropDownList.Items.Clear()

            'get the values from ToleranceCombinationMethod enum
            Dim names As String() = [Enum].GetNames(GetType(ToleranceCombinationMethod))
            Dim values As Array = [Enum].GetValues(GetType(ToleranceCombinationMethod))

            'populate the dropdownlist
            For i As Integer = 0 To names.Length - 1
                cboPaymentToleranceCombinationMethod.DropDownList.Items.Add(New ListItem(names(i), Convert.ToInt32(values.GetValue(i)).ToString()))
            Next

            'set the selected value
            If PaymentToleranceGroupSystemType = PaymentToleranceGroupSystemTypes.VisitBasedServiceToleranceGroup Then
                cboPaymentToleranceCombinationMethod.DropDownList.SelectedValue = ToleranceCombiMethod
            Else
                cboPaymentToleranceCombinationMethod.DropDownList.SelectedValue = ToleranceCombiMethod
            End If

        End Sub
#End Region

#Region " SetTextBoxFormats "

        Private Sub SetTextBoxFormats()

            'set text box formats, this does not seem to work in a control so have commented out, but left the code in place
            ' in case there is an oppurtunity to fix the TextBoxEx control
            If PaymentToleranceGroupSystemType = PaymentToleranceGroupSystemTypes.VisitBasedServiceToleranceGroup Then

                'With txtAcceptableAdditionalUnits
                '    .EnsureChildControls()
                '    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                '    .MinimumValue = 0
                '    .MaximumValue = 999.9
                '    .SetupRangeValidator()
                'End With

                'With txtAcceptableAdditionalUnitsAsPercentage
                '    .EnsureChildControls()
                '    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                '    .MinimumValue = 0
                '    .MaximumValue = 100
                '    .SetupRangeValidator()
                'End With

                'With txtUnitsOfServiceCappedAt
                '    .EnsureChildControls()
                '    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                '    .MinimumValue = 0
                '    .MinimumValue = 999.9
                '    .SetupRangeValidator()
                'End With

            End If

            'set cost of service texbox formats
            'With txtAcceptableAdditionalPayment
            '    .EnsureChildControls()
            '    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
            '    .MinimumValue = 0
            '    .MaximumValue = 999.99
            '    .SetupRangeValidator()
            'End With

            'With txtAcceptableAdditionalPaymentAsPercentage
            '    .EnsureChildControls()
            '    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
            '    .MinimumValue = 0
            '    .MaximumValue = 100
            '    .SetupRangeValidator()
            'End With

            'With txtCostOfServiceCappedAt
            '    .EnsureChildControls()
            '    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
            '    .MinimumValue = 0
            '    .MaximumValue = 999.99
            '    .SetupRangeValidator()
            'End With

        End Sub

#End Region

#Region " SetUpJavaScript "

        ''' <summary>
        ''' Setups the javascript.
        ''' </summary>
        Private Sub SetupJavascript()

            Dim jsStartupScript As New StringBuilder()

            chkSuspendInvoiceWhenPlannedUnitsExceeded.CheckBox.Attributes.Add("onclick", "chkSuspendInvoiceWhenPlannedUnitsExceeded_OnClick();")
            cboPaymentToleranceCombinationMethod.DropDownList.Attributes.Add("onchange", "cboPaymentToleranceCombinationMethod_OnChange();")

            ' add the payment tolerance group enum to use in javascript
            AjaxPro.Utility.RegisterEnumForAjax(GetType(PaymentToleranceGroupSystemTypes))

            ' add the payment tolerance group enum to use in javascript
            AjaxPro.Utility.RegisterEnumForAjax(GetType(ToleranceCombinationMethod))

            ' create a script thn sets ids of controls for this user control
            jsStartupScript.AppendFormat("var {0}_AND_TOL_COMBI_MESSAGE = '{1}';", CONTROL_NAME, AND_TOL_COMBI_MESSAGE)
            jsStartupScript.AppendFormat("var {0}_OR_TOL_COMBI_MESSAGE = '{1}';", CONTROL_NAME, OR_TOL_COMBI_MESSAGE)
            jsStartupScript.AppendFormat("var {0}_cboPaymentToleranceCombinationMethod = GetElement('{1}', true);", _
                                         CONTROL_NAME, cboPaymentToleranceCombinationMethod.ClientID & DROP_DOWN_LIST)
            jsStartupScript.AppendFormat("var {0}_ptgSystemType = {1};", CONTROL_NAME, CInt(PaymentToleranceGroupSystemType))
            jsStartupScript.AppendFormat("var {0}_lblWarning = GetElement('{1}', true);", CONTROL_NAME, lblWarning.ClientID)
            jsStartupScript.AppendFormat("var {0}_chkSuspendInvoiceWhenPlannedUnitsExceeded = GetElement('{1}', true);", _
                                         CONTROL_NAME, chkSuspendInvoiceWhenPlannedUnitsExceeded.ClientID & CHECKBOX)

            ' register javascript file for this user control
            MyBasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(JAVASCRIPT_PATH.ToString))

            ' register startup script for this user control
            MyBasePage.ClientScript.RegisterStartupScript(MyBasePage.GetType(), "PaymentToleranceControl.Init", _
                                                          jsStartupScript.ToString(), _
                                                          True)

        End Sub

#End Region

#End Region

    End Class
End Namespace
