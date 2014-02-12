
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Jobs.Core
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library
Imports Target.Abacus.Library.Selectors
Imports Target.Abacus.Library.Selectors.Messages
Imports Target.Abacus.Library.Selectors.Utilities
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Abacus.Library.Core
Imports Target.Abacus.Web.Apps.InPlaceSelectors

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control that provides custom inputs for the Publish Debtor Invoices and Statements job step.
    ''' </summary>
    ''' <history>
    ''' JohnF      Created (D12092B)
    ''' </history>
    ''' <remarks></remarks>
    Partial Public Class PublishDebtorInvoicesStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

#Region "Fields"

        Private Const _JavascriptPath As String = "AbacusWeb/Apps/Jobs/UserControls/PublishDebtorInvoicesStepInputs.js"
        Private Const _SP_FETCH_BILLING_TYPES As String = "spxBillingTypes_FetchAll"
        Private Const _SP_FETCH_DOCUMENT_SORT_TYPES As String = "spxPublishDebtorsInvoicesSort_FetchAll"
        Private Const _BILLING_TYPE_DEFAULTONLY As Integer = 1
        Private Const _BILLING_TYPE_DEFAULTPROPERTY As Integer = 3
        Private _SystemInfo As SystemInfo

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

        ''' <summary>
        ''' Gets the javascript path for this user control.
        ''' </summary>
        ''' <value>The javascript path.</value>
        Private ReadOnly Property JavascriptPath() As String
            Get
                Return WebUtils.GetVirtualPath(_JavascriptPath)
            End Get
        End Property

#End Region

#Region " Page Events "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            '++ Register the Javascript file for this user control..
            BaseWebPage.JsLinks.Add(JavascriptPath)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim value As String = ""

            _SystemInfo = DataCache.SystemInfo(BaseWebPage.DbConnection, Nothing, True)

            BaseWebPage.AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")
            SetupDateValidation()
            SetupJavascript()

            'If Not Me.IsPostBack Then
            PopulateScreen()
            'End If

            If Request.Form(String.Format("{0}$txtTextBox", txtStatementFooter.UniqueID)) IsNot Nothing Then
                RefreshASPControls()

                value = Request.Form(String.Format("{0}$cboDropDownList", cboBillingType.UniqueID))
                If value IsNot Nothing Then
                    FillBillingTypeDropdown()
                    cboBillingType.DropDownList.SelectedValue = Utils.ToInt32(value)
                End If

                dteTransFrom.TextBox.Text = Request.Form(String.Format("{0}$txtTextBox", dteTransFrom.UniqueID))
                dteTransTo.TextBox.Text = Request.Form(String.Format("{0}$txtTextBox", dteTransTo.UniqueID))
                txtStatementFooter.Text = Request.Form(String.Format("{0}$txtTextBox", txtStatementFooter.UniqueID))

                value = Request.Form(String.Format("{0}$cboDropDownList", cboSort.UniqueID))
                If value IsNot Nothing Then
                    FillDocumentSortDropdown()
                    cboSort.DropDownList.SelectedValue = Utils.ToInt32(value)
                End If
            End If
        End Sub

        Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

            RefreshASPControls()

            If IsDate(_SystemInfo.NextDDPaymentDateDom) Then
                txtDDNonRes.Text = _SystemInfo.NextDDPaymentDateDom.ToString("dd/MM/yyyy")
            Else
                txtDDNonRes.Text = "(unknown)"
            End If
            If IsDate(_SystemInfo.NextDDPaymentDateRes) Then
                txtDDRes.Text = _SystemInfo.NextDDPaymentDateRes.ToString("dd/MM/yyyy")
            Else
                txtDDRes.Text = "(unknown)"
            End If

            If _SystemInfo.DDNotificationDom Or _SystemInfo.DDNotificationRes Then
                fsDDPaymentDates.Visible = True
            Else
                fsDDPaymentDates.Visible = False
            End If

            If (IsDate(_SystemInfo.NextDDPaymentDateDom) AndAlso DateDiff(DateInterval.Day, Date.Today, _SystemInfo.NextDDPaymentDateDom) < 14) _
                    OrElse (IsDate(_SystemInfo.NextDDPaymentDateRes) AndAlso DateDiff(DateInterval.Day, Date.Today, _SystemInfo.NextDDPaymentDateRes) < 14) Then
                lblWarning.Visible = True
            Else
                lblWarning.Visible = False
            End If
        End Sub

#End Region

#Region " Methods "

        Private Sub PopulateScreen()
            Dim appSettings As ApplicationSettingCollection = Nothing
            Dim appSettingValues As ApplicationSettingValueCollection = Nothing
            Dim msg As New ErrorMessage

            optInvoiceUnprinted.Checked = True   '++ Default to 'Unprinted invoices'..
            chkReplaceText.Checked = False

            FillBillingTypeDropdown()
            cboBillingType.SelectPostBackValue()

            If IsDate(_SystemInfo.NextDDPaymentDateDom) Then
                txtDDNonRes.Text = _SystemInfo.NextDDPaymentDateDom.ToString("dd/MM/yyyy")
            Else
                txtDDNonRes.Text = "(unknown)"
            End If
            If IsDate(_SystemInfo.NextDDPaymentDateRes) Then
                txtDDRes.Text = _SystemInfo.NextDDPaymentDateRes.ToString("dd/MM/yyyy")
            Else
                txtDDRes.Text = "(unknown)"
            End If

            msg = ApplicationSetting.FetchList(BaseWebPage.DbConnection, appSettings, "", "", _
                                               2, "PublishTransactionStatementsDefault")
            If msg.Success AndAlso appSettings IsNot Nothing AndAlso appSettings.Count > 0 Then
                chkProduceStatements.Checked = (Utils.ToInt32(appSettings(0).SettingValue) <> 0)
            Else
                chkProduceStatements.Checked = False
            End If

            FillDocumentSortDropdown()
            cboSort.SelectPostBackValue()
            msg = ApplicationSetting.FetchList(BaseWebPage.DbConnection, appSettings, "", "", _
                                               2, "PublishDebtorInvoiceSortOrderDefault")
            If msg.Success AndAlso appSettings IsNot Nothing AndAlso appSettings.Count > 0 Then
                msg = ApplicationSettingValue.FetchList(BaseWebPage.DbConnection, appSettingValues, _
                                                        appSettings(0).ID, appSettings(0).SettingValue)
                If msg.Success AndAlso appSettingValues IsNot Nothing AndAlso appSettingValues.Count > 0 Then
                    cboSort.DropDownList.SelectedValue = Utils.ToInt32(appSettingValues(0).ID)
                End If
            End If

            txtStatementFooter.Text = Utils.ToString(_SystemInfo.TransactionStatementMessageFooter)
        End Sub

        Private Sub FillBillingTypeDropdown()
            Dim spParams As SqlParameter()
            Dim sqlDataset As DataSet = Nothing, sqlTable As DataTable = Nothing
            Dim iSelectedValue As Integer = 0

            '++ Fetch the current billing types..
            If cboBillingType.DropDownList.Items.Count = 0 Then
                With cboBillingType.DropDownList.Items
                    iSelectedValue = 0
                    .Add(New ListItem("(all billing types)", 0))

                    spParams = SqlHelperParameterCache.GetSpParameterSet(BaseWebPage.DbConnection, _SP_FETCH_BILLING_TYPES, False)
                    spParams(0).Value = Convert.DBNull
                    sqlDataset = SqlHelper.ExecuteDataset(BaseWebPage.DbConnection, CommandType.StoredProcedure, _SP_FETCH_BILLING_TYPES, spParams)
                    If sqlDataset.Tables.Count > 0 Then
                        sqlTable = sqlDataset.Tables(0)
                        For Each sqlRow As DataRow In sqlTable.Rows
                            .Add(New ListItem(Utils.ToString(sqlRow("Description")), Utils.ToInt32(sqlRow("ID"))))
                            If Utils.ToInt32(sqlRow("Flags")) = _BILLING_TYPE_DEFAULTONLY OrElse Utils.ToInt32(sqlRow("Flags")) = _BILLING_TYPE_DEFAULTPROPERTY Then
                                iSelectedValue = Utils.ToInt32(sqlRow("ID"))
                            End If
                        Next
                    End If
                    cboBillingType.DropDownList.SelectedValue = iSelectedValue
                End With
            End If
        End Sub

        Private Sub FillDocumentSortDropdown()
            Dim spParams As SqlParameter()
            Dim sqlDataset As DataSet = Nothing, sqlTable As DataTable = Nothing
            Dim iSelectedValue As Integer = 0

            If cboSort.DropDownList.Items.Count = 0 Then
                With cboSort.DropDownList.Items
                    iSelectedValue = 0

                    spParams = SqlHelperParameterCache.GetSpParameterSet(BaseWebPage.DbConnection, _SP_FETCH_DOCUMENT_SORT_TYPES, False)
                    spParams(0).Value = Convert.DBNull
                    sqlDataset = SqlHelper.ExecuteDataset(BaseWebPage.DbConnection, CommandType.StoredProcedure, _SP_FETCH_DOCUMENT_SORT_TYPES, spParams)
                    If sqlDataset.Tables.Count > 0 Then
                        sqlTable = sqlDataset.Tables(0)
                        For Each sqlRow As DataRow In sqlTable.Rows
                            .Add(New ListItem(Utils.ToString(sqlRow("DisplayText")), Utils.ToInt32(sqlRow("ID"))))
                            If Utils.ToInt32(sqlRow("Value")) = Utils.ToInt32(sqlRow("SettingValue")) Then
                                iSelectedValue = Utils.ToInt32(sqlRow("ID"))
                            End If
                        Next
                    End If
                    cboSort.DropDownList.SelectedValue = iSelectedValue
                End With
            End If
        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim value As String
            Dim billingTypeID As Integer = 0, billingTypeDesc As String = ""
            Dim sortOrderID As Integer = 0, sortOrderDesc As String = ""
            Dim invoiceID As Integer = 0

            '++ 'Unprinted Invoices' or 'Single Invoice' selection..
            value = Request.Form(hidInvoiceChoice.UniqueID)
            Select Case value
                Case "1"
                    optInvoiceUnprinted.Checked = True
                    optInvoiceSingle.Checked = False
                Case "2"
                    optInvoiceSingle.Checked = True
                    optInvoiceUnprinted.Checked = False
                Case Else
                    optInvoiceUnprinted.Checked = True
                    optInvoiceSingle.Checked = False
            End Select
            If optInvoiceUnprinted.Checked Then
                result.Add(New Triplet(jobStepTypeID, "SelectUnprintedInvoices", Boolean.TrueString))
            Else
                result.Add(New Triplet(jobStepTypeID, "SelectUnprintedInvoices", Boolean.FalseString))
            End If

            '++ Billing type info..
            If cboBillingType.DropDownList.SelectedItem IsNot Nothing Then
                billingTypeID = Utils.ToInt32(cboBillingType.DropDownList.SelectedItem.Value)
                billingTypeDesc = Utils.ToString(cboBillingType.DropDownList.SelectedItem.Text)
            Else
                billingTypeID = 0
                billingTypeDesc = "(all billing types)"
            End If
            result.Add(New Triplet(jobStepTypeID, "BillingTypeID", billingTypeID))
            result.Add(New Triplet(jobStepTypeID, "BillingTypeDesc", billingTypeDesc))

            invoiceID = Utils.ToInt32(CType(invoicePicker, InPlaceDebtorInvoiceSelector).DebtorInvoiceID)
            result.Add(New Triplet(jobStepTypeID, "InvoiceID", invoiceID))
            If invoiceID > 0 Then
                Dim invRec As Invoice
                Dim msg As New ErrorMessage

                If trans IsNot Nothing Then
                    invRec = New Invoice(trans)
                Else
                    invRec = New Invoice(conn)
                End If
                msg = invRec.Fetch(invoiceID)
                If msg.Success Then
                    result.Add(New Triplet(jobStepTypeID, "InvoiceNumber", invRec.InvoiceNumber))
                Else
                    result.Add(New Triplet(jobStepTypeID, "InvoiceNumber", ""))
                End If
            End If

            '++ D/D Payment date (Non-Res)....
            If IsDate(_SystemInfo.NextDDPaymentDateDom) Then
                result.Add(New Triplet(jobStepTypeID, "DDPaymentDateNonRes", _SystemInfo.NextDDPaymentDateDom.ToString("dd/MM/yyyy")))
            Else
                result.Add(New Triplet(jobStepTypeID, "DDPaymentDateNonRes", "(unknown)"))
            End If
            '++ D/D Payment date (Res)....
            If IsDate(_SystemInfo.NextDDPaymentDateRes) Then
                result.Add(New Triplet(jobStepTypeID, "DDPaymentDateRes", _SystemInfo.NextDDPaymentDateRes.ToString("dd/MM/yyyy")))
            Else
                result.Add(New Triplet(jobStepTypeID, "DDPaymentDateRes", "(unknown)"))
            End If

            '++ 'Produce Transaction Statements' checkbox..
            value = Request.Form(hidProduceStatements.UniqueID)
            Select Case value
                Case "0"
                    chkProduceStatements.Checked = False
                Case "1"
                    chkProduceStatements.Checked = True
                Case Else
                    chkProduceStatements.Checked = False
            End Select
            If chkProduceStatements.Checked Then
                result.Add(New Triplet(jobStepTypeID, "ProduceStatements", Boolean.TrueString))
            Else
                result.Add(New Triplet(jobStepTypeID, "ProduceStatements", Boolean.FalseString))
            End If

            '++ 'Transactions From' date..
            value = Request.Form(String.Format("{0}$txtTextBox", dteTransFrom.UniqueID))
            If value IsNot Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "TransactionsDateFrom", value))
            Else
                result.Add(New Triplet(jobStepTypeID, "TransactionsDateFrom", Date.Today.ToString("dd/MM/yyyy")))
            End If

            '++ 'Transactions To' date..
            value = Request.Form(String.Format("{0}$txtTextBox", dteTransTo.UniqueID))
            If value IsNot Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "TransactionsDateTo", value))
            Else
                result.Add(New Triplet(jobStepTypeID, "TransactionsDateTo", Date.Today.ToString("dd/MM/yyyy")))
            End If

            '++ Statement footer text..
            value = Request.Form(String.Format("{0}$txtTextBox", txtStatementFooter.UniqueID))
            result.Add(New Triplet(jobStepTypeID, "StatementFooterText", value))

            '++ 'Replace standard text' checkbox..
            value = Request.Form(hidReplaceText.UniqueID)
            Select Case value
                Case "0"
                    chkReplaceText.Checked = False
                Case "1"
                    chkReplaceText.Checked = True
                Case Else
                    chkReplaceText.Checked = False
            End Select
            If chkReplaceText.Checked Then
                result.Add(New Triplet(jobStepTypeID, "ReplaceStandardText", Boolean.TrueString))
            Else
                result.Add(New Triplet(jobStepTypeID, "ReplaceStandardText", Boolean.FalseString))
            End If
            result.Add(New Triplet(jobStepTypeID, "ReplaceStandardText", value))

            '++ Document sort order info..
            If cboSort.DropDownList.SelectedItem IsNot Nothing Then
                sortOrderID = Utils.ToInt32(cboSort.DropDownList.SelectedItem.Value)
                sortOrderDesc = Utils.ToString(cboSort.DropDownList.SelectedItem.Text)
            Else
                sortOrderID = 0
                sortOrderDesc = "Reference"
            End If
            result.Add(New Triplet(jobStepTypeID, "DocumentSortOrderID", sortOrderID))
            result.Add(New Triplet(jobStepTypeID, "DocumentSortOrderDesc", sortOrderDesc))

            Return result

        End Function

        ''' <summary>
        ''' Setups the javascript.
        ''' </summary>
        Private Sub SetupJavascript()

            Const SCRIPT_STARTUP As String = "Startup"
            Dim startupJavascript As String = ""

            '++ Create a script that sets all the IDs of controls within this user control..
            startupJavascript = String.Format("optInvoiceUnprinted=GetElement('{0}'); optInvoiceSingle=GetElement('{1}');" _
                                                & "cboBillingTypeID='{2}'; dteFromID='{3}';" _
                                                & "dteToID='{4}'; invoicePickerID='{5}';" _
                                                & "chkProduceStatements=GetElement('{6}'); chkReplaceText=GetElement('{7}');" _
                                                & "lblBillingType=GetElement('{8}'); hidCurrInvoice=GetElement('{9}');" _
                                                & "hidInvoiceChoice=GetElement('{10}'); hidProduceStatements=GetElement('{11}');" _
                                                & "hidReplaceText=GetElement('{12}')" _
                                                , _
                                                optInvoiceUnprinted.ClientID, optInvoiceSingle.ClientID, _
                                                cboBillingType.ClientID, dteTransFrom.ClientID, _
                                                dteTransTo.ClientID, invoicePicker.ClientID, _
                                                chkProduceStatements.ClientID, chkReplaceText.ClientID, _
                                                lblBillingType.ClientID, hidCurrInvoice.ClientID, _
                                                hidInvoiceChoice.ClientID, hidProduceStatements.ClientID, _
                                                hidReplaceText.ClientID)

            '++ Register the startup script for this user control..
            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, startupJavascript, True)
            End If

        End Sub

        Private Sub RefreshASPControls()
            Dim value As String

            '++ Invoice picker (mandatory when 'Single Invoice' is selected)..
            value = Request.Form(hidInvoiceChoice.UniqueID)
            Select Case value
                Case "1"
                    optInvoiceUnprinted.Checked = True
                    optInvoiceSingle.Checked = False
                Case "2"
                    optInvoiceSingle.Checked = True
                    optInvoiceUnprinted.Checked = False
                Case Else
                    optInvoiceUnprinted.Checked = True
                    optInvoiceSingle.Checked = False
            End Select

            value = Request.Form(hidProduceStatements.UniqueID)
            If value IsNot Nothing Then
                Select Case value
                    Case "0"
                        chkProduceStatements.Checked = False

                        With dteTransFrom
                            .Required = False
                        End With

                        With dteTransTo
                            .Required = False
                        End With
                    Case "1"
                        chkProduceStatements.Checked = True

                        With dteTransFrom
                            .Required = True
                        End With

                        With dteTransTo
                            .Required = True
                        End With
                    Case Else
                        chkProduceStatements.Checked = False
                End Select
            End If

            value = Request.Form(hidReplaceText.UniqueID)
            If value IsNot Nothing Then
                Select Case value
                    Case "0"
                        chkReplaceText.Checked = False
                    Case "1"
                        chkReplaceText.Checked = True
                    Case Else
                        chkReplaceText.Checked = False
                End Select
            End If

            With CType(invoicePicker, InPlaceDebtorInvoiceSelector)
                .Required = optInvoiceSingle.Checked
            End With

            value = Request.Form(CType(invoicePicker, InPlaceDebtorInvoiceSelector).HiddenFieldUniqueID)
            If value IsNot Nothing Then
                CType(invoicePicker, InPlaceDebtorInvoiceSelector).DebtorInvoiceID = Utils.ToInt32(value)
            End If
        End Sub

        ''' <summary>
        ''' Setup the date validation for the transaction date fields.
        ''' </summary>
        Private Sub SetupDateValidation()

            Dim maxDate As DateTime = Utils.ToDateTime("31-Dec-9999")
            Dim minDate As DateTime = Utils.ToDateTime("1-Jan-1980")
            Dim dateTooltip As String = "The date must be between {0} and {1}."
            Dim dateValidationError As String = "The date must be between {0} and {1}."
            Dim value As String = ""

            dteTransFrom.TextBox.ToolTip = String.Format(dateTooltip, _
                                                       minDate.ToString("dd/MM/yyyy"), _
                                                       maxDate.ToString("dd/MM/yyyy"))
            dteTransTo.TextBox.ToolTip = String.Format(dateTooltip, _
                                                       minDate.ToString("dd/MM/yyyy"), _
                                                       maxDate.ToString("dd/MM/yyyy"))

            '++ Set up validation of the start date so it is no later than today (if required)..
            value = Request.Form(hidProduceStatements.UniqueID)
            If value IsNot Nothing Then
                If value = "1" Then
                    With valTransFrom
                        .MinimumValue = minDate.ToString("dd/MM/yyyy")
                        .MaximumValue = maxDate.ToString("dd/MM/yyyy")
                        .ErrorMessage = String.Format(dateValidationError, _
                                                      .MinimumValue, _
                                                      .MaximumValue)
                        .SetFocusOnError = True
                        .Type = ValidationDataType.Date
                    End With

                    With valTransTo
                        .MinimumValue = minDate.ToString("dd/MM/yyyy")
                        .MaximumValue = maxDate.ToString("dd/MM/yyyy")
                        .ErrorMessage = String.Format(dateValidationError, _
                                                      .MinimumValue, _
                                                      .MaximumValue)
                        .SetFocusOnError = True
                        .Type = ValidationDataType.Date
                    End With
                Else
                    dteTransFrom.TextBox.Text = Date.Today.ToString("dd/MM/yyyy")
                    dteTransTo.TextBox.Text = Date.Today.ToString("dd/MM/yyyy")
                End If
            End If

        End Sub
#End Region
    End Class

End Namespace
