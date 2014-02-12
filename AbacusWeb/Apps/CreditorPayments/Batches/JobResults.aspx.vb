Imports System.Text
Imports Target.Abacus.Jobs.Core
Imports Target.Abacus.Library.InterfaceLogs
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library.Core

Namespace Apps.CreditorPayments.Batches

    ''' <summary>
    ''' Screen used to view batch job results for generic creditor payments.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   18/02/2010 D11874 - Created
    ''' </history>
    Partial Class JobResults
        Inherits BasePage

#Region "Fields"

        'locals
        Private _FilterBatch As ViewableInterfaceLog = Nothing

        ' constants
        Private Const _ErrorBatchDoesNotExists As String = "The batch specified does not exist, please try again."
        Private Const _JsLibrary As String = "Library/JavaScript/"
        Private Const _PageTitle As String = "Creditor Payment Batch - Job Results"
        Private Const _QsBatchID As String = "bid"
        Private Const _WebCmdViewReportsPermission As String = "AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.CreditorPaymentBatches.ViewReports"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.CreditorPaymentBatches"

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
        ''' Gets the filter batch using the FilterBatchID.
        ''' </summary>
        ''' <value>The filter batch.</value>
        Private ReadOnly Property FilterBatch() As ViewableInterfaceLog
            Get
                If FilterBatchID > 0 AndAlso _FilterBatch Is Nothing Then
                    ' if we have a batch id and no batch object
                    Dim msg As ErrorMessage = Nothing
                    ' fetch the item, throw error if occurs
                    msg = InterfaceLogsBL.GetInterfaceLog(DbConnection, FilterBatchID, _FilterBatch)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _FilterBatch
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
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebCmdViewReportsPermission))
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

            If FilterBatchID > 0 AndAlso Not FilterBatch Is Nothing Then
                ' if we have a valid batch

                ' setup pages controls
                SetupJavascriptLinks()
                SetupStandardButtonsControl()
                SetupCreditorPaymentBatchCriteriaControl()

            Else
                ' else no batch id so report error

                DisplayError(_ErrorBatchDoesNotExists)

            End If

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

            ' add date utility JS
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}Date.js", _JsLibrary)))

            ' add table sorting JS
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}SortTable.js", _JsLibrary)))

            ' add utility JS link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}WebSvcUtils.js", _JsLibrary)))

            ' add dialog JS
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}Dialog.js", _JsLibrary)))

            ' add progress bar JS
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}ProgressBar.js", _JsLibrary)))

            ' add page JS
            JsLinks.Add("JobResults.js")

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.Jobs.WebSvc.JobService))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(JobStatus))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(JobStepStatus))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(JobStepXml))

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

#End Region

    End Class

End Namespace
