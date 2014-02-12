Imports System.Text
Imports Target.Abacus.Library.CreditorPayments
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
    ''' Screen used to recreate batches of generic creditor payments.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   12/04/2011 D11803 - Changes to actually create a job when clicking the Create button.
    '''   ColinD   16/02/2011 D11874 - Created
    ''' </history>
    Partial Class ReCreate
        Inherits BasePage

#Region "Fields"

        'locals
        Private _FilterBatch As ViewableInterfaceLog = Nothing

        ' constants
        Private Const _ErrorAccessDenied As String = "You do not have permissions to view Recreate Creditor Payment Batches, please contact your system administrator"
        Private Const _ErrorBatchDoesNotExists As String = "The batch specified does not exist, please try again."
        Private Const _PageTitle As String = "Creditor Payment Batch - Recreate Files"
        Private Const _QsBatchID As String = "bid"
        Private Const _WebCmdReCreateBatchPermission As String = "AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.CreditorPaymentBatches.ReCreateInterfaceFiles"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.CreditorPaymentBatches"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the create job point in time control.
        ''' </summary>
        ''' <value>The create job point in time control.</value>
        Private ReadOnly Property CreateJobPointInTimeControl() As Apps.Jobs.UserControls.ucCreateJobPointInTime
            Get
                Return CType(CreateJobPointInTime1, Apps.Jobs.UserControls.ucCreateJobPointInTime)
            End Get
        End Property

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
        ''' Gets or sets a value indicating whether [selected recreate XML].
        ''' </summary>
        ''' <value><c>true</c> if [selected recreate XML]; otherwise, <c>false</c>.</value>
        Public Property SelectedRecreateXML() As Boolean
            Get
                Return chkRereadData.CheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                chkRereadData.CheckBox.Checked = value
            End Set
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
        ''' Gets a value indicating whether [user has recreate batch permission].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [user has recreate batch permission]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasReCreateBatchPermission() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebCmdReCreateBatchPermission))
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
                ' if we have batch

                ' setup pages controls
                SetupStandardButtonsControl()
                SetupCreditorPaymentBatchCriteriaControl()

                If FilterBatch.OriginalXMLFileID <= 0 Then
                    ' if we have never had an original xml file then tick chk by default
                    ' and do not allow to change

                    With chkRereadData
                        .CheckBox.Enabled = False
                        .CheckBox.Checked = True
                    End With

                End If

            Else
                ' else no batch id so report error

                DisplayError(_ErrorBatchDoesNotExists)
                btnCreate.Enabled = False

            End If

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            ' setup the create button so only allowed users can use it
            btnCreate.Enabled = UserHasReCreateBatchPermission
            If UserHasReCreateBatchPermission = False Then
                DisplayError(_ErrorAccessDenied)
            End If

            WebUtils.RecursiveDisable(grpCreateInterface.Controls, Not UserHasReCreateBatchPermission)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the btnCreate control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click

            Dim msg As New ErrorMessage()
            Dim jobId As Integer = 0
            Dim recreateXml As Boolean = SelectedRecreateXML

            If FilterBatch.OriginalXMLFileID <= 0 Then
                ' if we have never had an original xml file then make sure we recreate

                recreateXml = True

            End If

            ' recreate the job and throw error if occurs
            msg = CreditorPaymentsBL.ReCreateGenericCreditorInterfaceJob(DbConnection, FilterBatch.ID, FilterBatch.BatchReference, recreateXml, CreateJobPointInTimeControl.CreateJobDateTime, CurrentUser.ExternalUsername, jobId)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' redirect the user to the created batch
            Response.Redirect(String.Format("List.aspx?ilid={0}", FilterBatchID))

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
