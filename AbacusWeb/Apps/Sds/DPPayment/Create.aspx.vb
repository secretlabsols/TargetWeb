Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Abacus.Web.Apps.Jobs.UserControls
Imports Target.Web.Apps.Security
Imports System.Collections.Specialized

Namespace Apps.Sds.DPPayments

    ''' <summary>
    ''' Creates a Direct Payments Job based on parameters passed to page.
    '''    
    '''    Optional Parameters:
    '''       1. clientid = Identifies a ClientDetail record to filter by.
    '''       2. bhid = Identifies a ThirdPartyBudgetHolder record to filter by.
    '''       3. issds = Indentifies whether to include sds or not or both, uses TriState values (0 = False, -1 = True, Both = -2)...would have been a better idea to use 1, 0, null.
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD      09/05/2013 D12336E : Added ability to control if Report or Generate options are available i.e. set the options as read only if required.
    ''' Mo Tahir    30/06/2011 D12104 : SDS Direct Payment Preview. 
    ''' ColinD      14/03/2011 Updated : Ensure sds filter added in development D12009 functions as specified.
    ''' ColinD      08/03/2011 Updated SDS491 : Ensure sds filter added in development D12009 functions as specified.
    ''' Waqas       14/02/2011 D12009 Add sds filter fieldset
    ''' MikeVO      02/12/2010  SDS issue #403. Support for "autopopup".
    ''' Colin Daly  01/11/2010  Updated (D11802) SDS Issue 249, removed validation of pay up to date
    ''' Colin Daly  04/08/2010  Created (D11802)
    ''' </history>
    Partial Public Class Create
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        ' Constants
        Private Const _CommandDoNotCollectReconsideredPaymentsPermission As String = "AbacusIntranet.WebNavMenuItemCommand.Payments.Administration.CreateDirectPayments.AlterDpcMarkedForReconsideration"
        Private Const _DatePayUpToTooltip As String = "The date to pay upto. Must be between {0} and {1}."
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _FilterOnAllMessage As String = "All"
        Private Const _IsSdsBothMessage As String = "All"
        Private Const _IsSdsNegativeMessage As String = "No"
        Private Const _IsSdsPositiveMessage As String = "Yes"
        Private Const _JavascriptPath As String = "Create.js"
        Private Const _JobServiceURL As String = "../../Jobs/Default.aspx?jobID={0}&autopopup={1}"
        Private Const _NavigationItemKey As String = "AbacusIntranet.WebNavMenuItem.Payments.Administration.CreateDirectPayments"
        Private Const _NoLastPaidUpToDateMessage As String = "N/A"
        Private Const _NoPermissionError As String = "You do not have permission to create direct payments."
        Private Const _PageTitle As String = "Create Direct Payments"
        Private Const _PayUpToDateValidationError As String = "The pay upto date must be between {0} and {1}"
        Private Const _QS_AutoPopup As String = "autopopup"
        Private Const _QS_BackUrl As String = "backUrl"
        Private Const _QS_BudgetHolderID As String = "bhid"
        Private Const _QS_IsSds As String = "issds"
        Private Const _QS_ServiceUserID As String = "clientid"
        Private Const _ReferenceAndNameFormat As String = "{0}: {1}"
        Private Const _WebNavMenuItemCommandGenerateKey As String = "AbacusIntranet.WebNavMenuItemCommand.CreateDirectPayments.Generate"
        Private Const _WebNavMenuItemCommandReportKey As String = "AbacusIntranet.WebNavMenuItemCommand.CreateDirectPayments.Report"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Click event of the btnCreate control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click

            If IsValid Then
                ' if the page has valid data

                CreateJob()

            End If

        End Sub

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            InitPage(ConstantsManager.GetConstant(_NavigationItemKey), _PageTitle)
            JobCreationPointInTimeControl.CreateJobDisplayMode = ucCreateJobPointInTime.DisplayMode.DisableCreateLaterOnClick

        End Sub

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not IsPostBack Then
                ' if first hit populate filter criteria

                PopulateDefaults()

            End If

            ' register javascript file for this page
            JsLinks.Add(_JavascriptPath)

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            chkDoNotCollectReconsideredPayments.Enabled = UserHasDoNotCollectReconsideredPaymentsPermission

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the budget holder ID from the query string. Null if not specified or has no value.
        ''' </summary>
        ''' <value>The budget holder ID.</value>
        Private ReadOnly Property BudgetHolderID() As Nullable(Of Integer)
            Get
                Return GetIntegerFromQueryString(_QS_BudgetHolderID)
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
        ''' Gets or sets a value indicating whether [do not collect reconsidered payments].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [do not collect reconsidered payments]; otherwise, <c>false</c>.
        ''' </value>
        Private Property DoNotCollectReconsideredPayments() As Boolean
            Get
                If UserHasDoNotCollectReconsideredPaymentsPermission = False Then
                    ' if the user doesnt have permission then return true always
                    Return True
                Else
                    ' else return the actual value
                    Return chkDoNotCollectReconsideredPayments.Checked
                End If
            End Get
            Set(ByVal value As Boolean)
                chkDoNotCollectReconsideredPayments.Checked = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the job point in time control.
        ''' </summary>
        ''' <value>The job point in time control.</value>
        Private ReadOnly Property JobCreationPointInTimeControl() As Target.Abacus.Web.Apps.Jobs.UserControls.ucCreateJobPointInTime
            Get
                Return CType(ucCreateJobPointInTime1, Target.Abacus.Web.Apps.Jobs.UserControls.ucCreateJobPointInTime)
            End Get
        End Property

        ''' <summary>
        ''' Gets when to create the job
        ''' </summary>
        ''' <value>The job creation time.</value>
        Private ReadOnly Property JobCreationTime() As DateTime
            Get
                With JobCreationPointInTimeControl
                    Return .CreateJobDateTime
                End With
            End Get
        End Property

        ''' <summary>
        ''' Gets the pay up to date.
        ''' </summary>
        ''' <value>The pay up to date.</value>
        Private ReadOnly Property PayUpToDate() As Nullable(Of DateTime)
            Get
                Return Target.Library.Utils.ToDateTime(dtePayUpTo.Text)
            End Get
        End Property

        ''' <summary>
        ''' Get the SDS, Non-SDS and Both filter on the 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private ReadOnly Property SdsFilter() As TriState
            Get
                Dim sdsFilterStr As String = Target.Library.Utils.ToString(Request.QueryString(_QS_IsSds))
                If sdsFilterStr <> "" Then
                    Return CType(Target.Library.Utils.ToInt32(sdsFilterStr), TriState)
                Else
                    Return TriState.UseDefault
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the service user ID from the query string. Null if not specified or has no value.
        ''' </summary>
        ''' <value>The service user ID.</value>
        Private ReadOnly Property ServiceUserID() As Nullable(Of Integer)
            Get
                Return GetIntegerFromQueryString(_QS_ServiceUserID)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [user has do not collect reconsidered payments permission].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [user has do not collect reconsidered payments permission]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasDoNotCollectReconsideredPaymentsPermission() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_CommandDoNotCollectReconsideredPaymentsPermission))
            End Get
        End Property

        ''' <summary>
        ''' Gets the job payment preview options control.
        ''' </summary>
        ''' <value>The payment preview options control.</value>
        Private ReadOnly Property PaymentPreviewOptionsControl() As Target.Abacus.Web.Apps.Jobs.UserControls.ucPaymentPreviewOptions
            Get
                Return CType(ucPaymentPreviewOptions1, Target.Abacus.Web.Apps.Jobs.UserControls.ucPaymentPreviewOptions)
            End Get
        End Property

        ''' <summary>
        ''' Gets Generate payments option
        ''' </summary>
        ''' <value>Generate Payments.</value>
        Private ReadOnly Property GeneratePaymentsOptionSelected() As Boolean
            Get
                With PaymentPreviewOptionsControl
                    Return .GeneratePayments
                End With
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [user has generate command].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [user has generate command]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasGenerateCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebNavMenuItemCommandGenerateKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [user has report command].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [user has report command]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasReportCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebNavMenuItemCommandReportKey))
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Creates the job.
        ''' </summary>
        Private Sub CreateJob()

            Dim msg As ErrorMessage = Nothing
            Dim jobID As Integer = 0
            Dim payUpDate As DateTime = DateTime.Now

            If PayUpToDate.HasValue Then
                ' if we have a pay up to date specified, should
                ' always be set if validation occurred correctly

                payUpDate = PayUpToDate.Value

            End If

            ' create the dp job at whatever time is specified in the point in time control
            msg = DirectPaymentPaymentsBL.CreateDirectPaymentsJob(DbConnection, _
                                                                  ServiceUserID, _
                                                                  BudgetHolderID, _
                                                                  payUpDate, _
                                                                  JobCreationTime, _
                                                                  CurrentUser.ExternalUsername, _
                                                                  String.Empty, _
                                                                  CurrentUser.ExternalUserID, _
                                                                  String.Empty, _
                                                                  jobID, _
                                                                  SdsFilter, _
                                                                  DoNotCollectReconsideredPayments, _
                                                                  GeneratePaymentsOptionSelected)

            If msg.Success Then
                ' created job, redirect to job service view
                ' to see the created job

                Response.Redirect( _
                    String.Format(_JobServiceURL, _
                                  jobID, _
                                  Target.Library.Utils.ToInt32(Request.QueryString(_QS_AutoPopup)) _
                    ) _
                )

            Else
                ' failed creating job

                Target.Library.Web.Utils.DisplayError(msg)

            End If

        End Sub

        ''' <summary>
        ''' Displays an error message
        ''' </summary>
        ''' <param name="errorMessage">The error message to display</param>
        ''' <remarks></remarks>
        Private Sub DisplayError(ByVal errorMessage As String)

            lblError.Text = errorMessage
            pnlForm.Visible = False

        End Sub

        ''' <summary>
        ''' Gets client detail.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Private Function GetClientDetail(ByVal id As Integer, ByRef client As ClientDetail) As ErrorMessage

            client = New ClientDetail(conn:=Me.DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
            Return client.Fetch(id)

        End Function

        ''' <summary>
        ''' Gets an integer from query string.
        ''' </summary>
        ''' <param name="key">The key.</param>
        ''' <returns></returns>
        Private Function GetIntegerFromQueryString(ByVal key As String) As Nullable(Of Integer)

            Dim qsInt As String = Request.QueryString(key)
            Dim tmpInt As Integer = Target.Library.Utils.ToInt32(qsInt)

            If tmpInt > 0 Then
                ' if the value exists and is an 
                ' integer larger than 0 then return 

                Return tmpInt

            Else
                ' else return nothing

                Return Nothing

            End If

        End Function

        ''' <summary>
        ''' Populates the budget holder.
        ''' </summary>
        Private Sub PopulateBudgetHolder()

            If BudgetHolderID.HasValue Then
                ' if we have a budget holder id

                Dim msg As New ErrorMessage()
                Dim partyBudgetHolder As ThirdPartyBudgetHolder = Nothing

                'fetch the third party budget holder and display error if unsuccessful
                msg = BudgetHolderBL.GetBudgetHolder(DbConnection, partyBudgetHolder, BudgetHolderID.Value)
                If msg.Success = False Then Target.Library.Web.Utils.DisplayError(msg)

                lblFilterBudgetHolder.Text = String.Format(_ReferenceAndNameFormat, _
                                                           partyBudgetHolder.Reference, _
                                                           String.Format("{0} {1} ({2})", partyBudgetHolder.TitleAndInitials.Trim(), _
                                                                                            partyBudgetHolder.Surname.Trim(), _
                                                                                            partyBudgetHolder.OrganisationName.Trim()))

            Else
                ' else no id so set to all

                lblFilterBudgetHolder.Text = _FilterOnAllMessage

            End If

        End Sub

        ''' <summary>
        ''' Populates the defaults.
        ''' </summary>
        Private Sub PopulateDefaults()

            PopulateBudgetHolder()
            PopulateServiceUser()
            PopulatePaidUpToDates()
            PopulateSDSFilter()
            PopulatePaymentOptions()

        End Sub

        ''' <summary>
        ''' Populate SDS filter 
        ''' </summary>
        ''' <remarks>By Waqas to pass through the sds fiter to Job</remarks>
        Private Sub PopulateSDSFilter()

            Select Case SdsFilter
                Case TriState.False
                    lblFilterSds.Text = _IsSdsNegativeMessage
                Case TriState.True
                    lblFilterSds.Text = _IsSdsPositiveMessage
                Case Else
                    lblFilterSds.Text = _IsSdsBothMessage
            End Select

        End Sub

        ''' <summary>
        ''' Populates the paid up to dates.
        ''' </summary>
        Private Sub PopulatePaidUpToDates()

            Dim lastPaidUpToDate As Nullable(Of DateTime) = Nothing
            Dim maxPayUpToDate As DateTime = DateTime.MaxValue
            Dim minPayUpToDate As DateTime = DateTime.MinValue
            Dim payUpToDate As DateTime = DateTime.MinValue
            Dim msg As ErrorMessage = Nothing

            ' get the default pay up to dates
            msg = DirectPaymentPaymentsBL.GetDefaultPayUpToDate(DbConnection, lastPaidUpToDate, payUpToDate)
            If msg.Success = False Then Target.Library.Web.Utils.DisplayError(msg)

            dtePayUpTo.Text = payUpToDate.ToString(_DateFormat)

            If lastPaidUpToDate.HasValue Then
                ' if we have a last paid up to date set the value and set min

                lblLastPaidUpTo.Text = lastPaidUpToDate.Value.ToString(_DateFormat)

            Else
                ' else no last paid up to date so default to today

                lblLastPaidUpTo.Text = _NoLastPaidUpToDateMessage

            End If

            dtePayUpTo.TextBox.ToolTip = String.Format(_DatePayUpToTooltip, _
                                                       minPayUpToDate.ToString(_DateFormat), _
                                                       maxPayUpToDate.ToString(_DateFormat))

            ' set up validation of the pay to column
            With valDates
                .MinimumValue = minPayUpToDate.ToString(_DateFormat)
                .MaximumValue = maxPayUpToDate.ToString(_DateFormat)
                .ErrorMessage = String.Format(_PayUpToDateValidationError, _
                                              .MinimumValue, _
                                              .MaximumValue)
                .SetFocusOnError = True
                .Type = ValidationDataType.Date
            End With

        End Sub

        ''' <summary>
        ''' Populates the payment options.
        ''' </summary>
        Private Sub PopulatePaymentOptions()

            Dim userCanCreateDirectPaymentsAtAll As Boolean = (UserHasGenerateCommand OrElse UserHasReportCommand)

            ' setup access to individual items
            With PaymentPreviewOptionsControl
                .AllowGenerate = UserHasGenerateCommand
                .AllowReport = UserHasReportCommand
            End With

            ' setup access to globals
            lblError.Text = IIf(userCanCreateDirectPaymentsAtAll, String.Empty, "You do not have either the 'Generate' or 'Report' permissions and as such cannot create Direct Payments. Please contact the system administrator to configure relevant permissions.<br /><br />")
            btnCreate.Enabled = userCanCreateDirectPaymentsAtAll
            grpCreateInterface.Disabled = Not userCanCreateDirectPaymentsAtAll

        End Sub

        ''' <summary>
        ''' Populates the service user.
        ''' </summary>
        Private Sub PopulateServiceUser()

            If ServiceUserID.HasValue Then
                ' if service user has value

                Dim client As ClientDetail = Nothing
                Dim msg As ErrorMessage = Nothing

                msg = GetClientDetail(ServiceUserID.Value, client)
                If msg.Success = False Then Target.Library.Web.Utils.DisplayError(msg)
                lblFilterServiceUser.Text = String.Format(_ReferenceAndNameFormat, client.Reference, client.Name)

            Else
                ' else all service users

                lblFilterServiceUser.Text = _FilterOnAllMessage

            End If

        End Sub

#End Region

    End Class

End Namespace