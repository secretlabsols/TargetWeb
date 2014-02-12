Imports System.Text
Imports Target.Abacus.Library.InterfaceLogs
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.CreditorPayments.Batches

    ''' <summary>
    ''' Screen used to view reports for generic creditor payments batches.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   20/11/2012 D12310 - Updated to use new parameters for creditor payments reports.
    '''   ColinD   18/02/2010 D11874 - Created
    ''' </history>
    Partial Class Reports
        Inherits BasePage

#Region "Fields"

        ' constants
        Private Const _ErrorAccessDenied As String = "You do not have permissions to view Creditor Payment Batches Reports, please contact your system administrator."
        Private Const _ErrorBatchDoesNotExist As String = "The batch specified does not exist, please try again."
        Private Const _QsBatchID As String = "bid"
        Private Const _PageTitle As String = "Creditor Payment Batch - Reports"
        Private Const _WebCmdHasViewReportsPermission As String = "AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.CreditorPaymentBatches.ViewReports"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.CreditorPaymentBatches"
        Private Const _WebReportSimpleListOfCreditorPaymentsInBatch As String = "AbacusIntranet.WebReport.SimpleListOfCreditorPaymentsInBatch"
        Private Const _WebReportDomiciliaryProviderInvoiceBatchSummary As String = "AbacusIntranet.WebReport.DomiciliaryProviderInvoiceBatchSummary"
        Private Const _WebReportDomiciliaryProviderInvoices As String = "AbacusIntranet.WebReport.DomiciliaryProviderInvoices"
        Private Const _WebReportDomiciliaryProviderInvoiceLines As String = "AbacusIntranet.WebReport.DomiciliaryProviderInvoiceLines"
        Private Const _WebReportFirstPaymentDomiciliaryServiceOrders As String = "AbacusIntranet.WebReport.FirstPaymentDomiciliaryServiceOrders"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the creditor payment batch criteria control.
        ''' </summary>
        ''' <value>The creditor payment batch criteria control.</value>
        Private ReadOnly Property CreditorPaymentBatchCriteriaControl() As ucCreditorPaymentBatchCriteria
            Get
                Return CType(FilterCriteria1, ucCreditorPaymentBatchCriteria)
            End Get
        End Property

        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>The current user.</value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                Return SecurityBL.GetCurrentUser()
            End Get
        End Property

        ''' <summary>
        ''' Gets the batch id to filter on from the query string, 0 if not specified.
        ''' </summary>
        ''' <value>The filter batch ID.</value>
        Private ReadOnly Property FilterBatchID() As Integer
            Get
                Return Target.Library.Utils.ToInt32(Request.QueryString(_QsBatchID))
            End Get
        End Property

        ''' <summary>
        ''' Gets the standard buttons control.
        ''' </summary>
        ''' <value>The standard buttons control.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [user has view reports permission].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [user has view reports permission]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasViewReportsPermission() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebCmdHasViewReportsPermission))
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            ' init the page
            InitPage(ConstantsManager.GetConstant(_WebNavMenuItemKey), _PageTitle)

            ' disable the list of reports by default
            lstReports.Enabled = False

            If FilterBatchID > 0 Then
                ' if we have batch id

                ' setup pages controls
                SetupCreditorPaymentBatchCriteriaControl()
                SetupJavascriptLinks()
                SetupReports()
                SetupStandardButtonsControl()

            Else
                ' else no batch id so report error

                DisplayError(_ErrorBatchDoesNotExist)

            End If

            If UserHasViewReportsPermission = False Then
                ' if the user hasnt permission for this screen then advise,
                ' they will not be able to select report

                DisplayError(_ErrorAccessDenied)

            End If

            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Apps.UserControls.ServiceOrderReports", _
     Target.Library.Web.Utils.WrapClientScript(String.Format("Report_lstReportlistId='{0}';", _
                                                             lstReports.ClientID _
                                                )) _
                                        )
        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Displays the error message.
        ''' </summary>
        ''' <param name="msg">The message.</param>
        Private Sub DisplayError(ByVal msg As String)

            lblError.Text = String.Format("{0}<br /><br />", msg)

        End Sub

        ''' <summary>
        ''' Setups the creditor payment batch criteria control.
        ''' </summary>
        Private Sub SetupCreditorPaymentBatchCriteriaControl()

            With CreditorPaymentBatchCriteriaControl
                .FilterCreditorPaymentBatchID = FilterBatchID
            End With

        End Sub

        ''' <summary>
        ''' Sets up the javascript links for this control.
        ''' </summary>
        Private Sub SetupJavascriptLinks()

            ' add page JS
            JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

        End Sub

        ''' <summary>
        ''' Sets up the standard buttons control.
        ''' </summary>
        Private Sub SetupStandardButtonsControl()

            ' setup std buttons so only the back button is present
            With StandardButtonsControl
                .AllowBack = True
                .AllowDelete = False
                .AllowEdit = False
                .AllowFind = False
                .AllowNew = False
                .ShowNew = False
                .ShowSave = False
            End With

        End Sub

#Region "Reports"

        ''' <summary>
        ''' Setups the reports.
        ''' </summary>
        Private Sub SetupReports()

            SetupReportsList()
            SetupNonResidentialBatchSummaryReportsButton()
            SetupNonResidentialPaymentsInBatchReportsButton()
            SetupNonResidentialPaymentLinesInBatchReportsButton()
            SetupNonResidentialServiceOrdersFirstPaymentReportsButton()
            SetupGenericCreditorSimpleListOfPaymentsInBatch()

        End Sub

        ''' <summary>
        ''' Sets up the reports list.
        ''' </summary>
        Private Sub SetupReportsList()

            With lstReports
                .Rows = 10
                .Attributes.Add("onchange", "lstReports_Change();")
                With .Items
                    .Add(New ListItem("Simple list of payments in the batch", divGenCredSimplListOfPayments.ClientID))
                    .Add(New ListItem("Batch summary", divGenCredBatchSummary.ClientID))
                    .Add(New ListItem("Simple list of non-residential payments in the batch", divNonResListInBatch.ClientID))
                    .Add(New ListItem("Simple list of non-residential payments lines in the batch", divNonResLineListInBatch.ClientID))
                    .Add(New ListItem("Simple list of non-residential service orders first paid in the batch", divNonResFirstPaymentDsoList.ClientID))
                End With
                .Enabled = UserHasViewReportsPermission
            End With

        End Sub

        ''' <summary>
        ''' Setups the generic creditor simple list of payments in batch.
        ''' </summary>
        Private Sub SetupGenericCreditorSimpleListOfPaymentsInBatch()

            SetupReportButton(rbGenCredSimplListOfPayments, _WebReportSimpleListOfCreditorPaymentsInBatch, "ilid")

        End Sub

        ''' <summary>
        ''' Sets up the non residential batch summary reports button.
        ''' </summary>
        Private Sub SetupNonResidentialBatchSummaryReportsButton()

            SetupReportButton(rbGenCredBatchSummary, _WebReportDomiciliaryProviderInvoiceBatchSummary, "intBatchID")

        End Sub

        ''' <summary>
        ''' Sets up the non residential payments in batch reports button.
        ''' </summary>
        Private Sub SetupNonResidentialPaymentsInBatchReportsButton()

            SetupReportButton(rbNonResListInBatch, _WebReportDomiciliaryProviderInvoices, "ilid")

        End Sub

        ''' <summary>
        ''' Sets up the non residential payment lines in batch reports button.
        ''' </summary>
        Private Sub SetupNonResidentialPaymentLinesInBatchReportsButton()

            SetupReportButton(rbNonResLineListInBatch, _WebReportDomiciliaryProviderInvoiceLines, "ilid")

        End Sub

        ''' <summary>
        ''' Sets up the non residential service orders first payment reports button.
        ''' </summary>
        Private Sub SetupNonResidentialServiceOrdersFirstPaymentReportsButton()

            SetupReportButton(rbNonResFirstPaymentDsoList, _WebReportFirstPaymentDomiciliaryServiceOrders, "intBatchID")

        End Sub

        ''' <summary>
        ''' Sets up the reports button.
        ''' </summary>
        ''' <param name="reportsButton">The reports button.</param>
        ''' <param name="constant">The constant.</param>
        ''' <param name="interfaceLogParamName">Name of the interface log param.</param>
        Private Sub SetupReportButton(ByVal reportsButton As UserControl, ByVal constant As String, ByVal interfaceLogParamName As String)

            With CType(reportsButton, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant(constant)
                .Parameters.Add(interfaceLogParamName, FilterBatchID)
            End With

        End Sub

#End Region

#End Region

    End Class

End Namespace
