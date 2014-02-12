Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Dom.Contracts

	''' <summary>
    ''' Screen used to maintain invoice line cost overrides.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD  17/08/2011  D12102 - Created.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class LineCostOverride
        Inherits BasePage

#Region "Fields"

        ' constants
        Private Const _GeneralErrorCode As String = ErrorMessage.GeneralErrorNumber
        Private Const _PageTitle As String = "Domiciliary Contract Period Invoice Line Cost Ovverrides"
        Private Const _QsContractID As String = "contractID"
        Private Const _QsPeriodID As String = "periodID"
        Private Const _WebCmdEditKey As String = "AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"

        ' locals
        Private _AuditLogUserTitle As String = Nothing
        Private _Contract As DomContract = Nothing
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _InvoiceLineCostOverrides As List(Of ViewableContractPeriodInvoiceLineCostOverride) = Nothing
        Private _RefreshParentTree As Boolean = False

#End Region

#Region "Properties"

#Region "Authorisation Properties"

        ''' <summary>
        ''' Gets a value indicating whether can edit records.
        ''' </summary>
        ''' <value><c>true</c> if user can edit records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasEditCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant(_WebCmdEditKey))
            End Get
        End Property

#End Region

#Region "QueryString Properties"

        ''' <summary>
        ''' Gets the contract ID from the query string.
        ''' </summary>
        ''' <value>The contract ID.</value>
        Private ReadOnly Property QsContractID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString(_QsContractID))
            End Get
        End Property

        ''' <summary>
        ''' Gets the period ID from the query string.
        ''' </summary>
        ''' <value>The period ID.</value>
        Private ReadOnly Property QsPeriodID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString(_QsPeriodID))
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Gets the audit log user title.
        ''' </summary>
        ''' <value>The audit log user title.</value>
        Private ReadOnly Property AuditLogUserTitle() As String
            Get
                If String.IsNullOrEmpty(_AuditLogUserTitle) Then
                    _AuditLogUserTitle = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
                End If
                Return _AuditLogUserTitle
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the audit log user.
        ''' </summary>
        ''' <value>The name of the audit log user.</value>
        Private ReadOnly Property AuditLogUserName() As String
            Get
                Return CurrentUser.ExternalUsername
            End Get
        End Property

        ''' <summary>
        ''' Gets the contract.
        ''' </summary>
        ''' <value>The contract.</value>
        Private ReadOnly Property Contract() As DomContract
            Get
                If _Contract Is Nothing Then
                    ' get the contract if not already got 
                    Dim msg As New ErrorMessage()
                    _Contract = New DomContract(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                    msg = _Contract.Fetch(QsContractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _Contract
            End Get
        End Property

        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>The current user.</value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property

        ''' <summary>
        ''' Gets the invoice line cost overrides.
        ''' </summary>
        ''' <value>The invoice line cost overrides.</value>
        Private ReadOnly Property InvoiceLineCostOverrides() As List(Of ViewableContractPeriodInvoiceLineCostOverride)
            Get
                If _InvoiceLineCostOverrides Is Nothing Then
                    ' get if not already done so
                    Dim msg As New ErrorMessage()
                    msg = DomContractBL.GetContractPeriodInvoiceLineCostOverride(DbConnection, QsContractID, QsPeriodID, _InvoiceLineCostOverrides)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _InvoiceLineCostOverrides
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether contract is ended.
        ''' </summary>
        ''' <value>
        ''' Gets a value indicating whether contract is ended.
        ''' </value>
        Private ReadOnly Property IsContractEnded() As Boolean
            Get
                Return (Contract.EndDate < DataUtils.MAX_DATE)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [refresh parent tree].
        ''' </summary>
        ''' <value><c>true</c> if [refresh parent tree]; otherwise, <c>false</c>.</value>
        Public Property RefreshParentTree() As Boolean
            Get
                Return _RefreshParentTree
            End Get
            Set(ByVal value As Boolean)
                _RefreshParentTree = value
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

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(Target.Library.Web.ConstantsManager.GetConstant(_WebNavMenuItemKey), _PageTitle)

            ' setup the standard buttons control
            With StandardButtonsControl
                .AllowBack = True
                .AllowDelete = False
                .AllowEdit = UserHasEditCommand()
                .AllowFind = False
                .AllowNew = False
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                .EditableControls.Add(fsControls.Controls)
            End With

            ' setup js
            SetupJavaScript()

            If Not IsPostBack Then

                ' populate the invoice line cost overrides
                PopulateInvoiceLineCostOverrides()

            End If

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim js As New StringBuilder()

            If RefreshParentTree Then

                js.AppendFormat("window.parent.RefreshTree({0}, 'lco', {1});", QsContractID, QsPeriodID)

            End If

            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            With StandardButtonsControl
                .Visible = Not IsContractEnded
            End With

        End Sub

#Region "Button Events"

        ''' <summary>
        ''' Handles the cancel clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

            ' populate the invoice line cost overrides
            PopulateInvoiceLineCostOverrides()

        End Sub

        ''' <summary>
        ''' Handles the save clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim currentOverride As ViewableContractPeriodInvoiceLineCostOverride = Nothing
            Dim itemToSave As DomContractDomRateCategory_DomContractPeriod = Nothing
            Dim lineCostOverrides As List(Of ViewableContractPeriodInvoiceLineCostOverride) = InvoiceLineCostOverrides
            Dim msg As New ErrorMessage()
            Dim rowId As Integer = 0
            Dim rowOverrideCheckbox As CheckBox = Nothing

            Using transaction As SqlTransaction = SqlHelper.GetTransaction(DbConnection)
                ' start a new transaction

                Try

                    For Each row As GridViewRow In gvOverrides.Rows
                        ' loop each row to detect changes

                        rowId = gvOverrides.DataKeys(row.RowIndex)("ID")
                        currentOverride = (From tmpOverride In lineCostOverrides Where tmpOverride.ID = rowId Select tmpOverride).FirstOrDefault()
                        rowOverrideCheckbox = CType(row.Cells(2).Controls(0), CheckBox)

                        If currentOverride.AllowOverride <> rowOverrideCheckbox.Checked Then
                            ' if the values have changed then save them tut db

                            ' fetch the item to save
                            itemToSave = New DomContractDomRateCategory_DomContractPeriod(trans:=transaction, auditLogTitle:=AuditLogUserTitle, auditUserName:=AuditLogUserName)
                            msg = itemToSave.Fetch(rowId)
                            If Not msg.Success Then
                                ' errored so rollback and display error
                                SqlHelper.RollbackTransaction(transaction)
                                WebUtils.DisplayError(msg)
                                Exit Sub
                            End If

                            ' save the item to the db
                            With itemToSave
                                .AllowOverride = rowOverrideCheckbox.Checked
                                .AuditLogOverriddenParentID = QsContractID
                                msg = .Save()
                                If Not msg.Success Then
                                    ' errored so rollback and display error
                                    SqlHelper.RollbackTransaction(transaction)
                                    WebUtils.DisplayError(msg)
                                    Exit Sub
                                End If
                            End With

                            ' set the in memory items value so its re output...saves fetching back from the db again
                            currentOverride.AllowOverride = rowOverrideCheckbox.Checked

                        End If

                    Next

                    ' commit work to db
                    transaction.Commit()

                    ' populate the invoice line cost overrides
                    PopulateInvoiceLineCostOverrides()

                Catch ex As Exception
                    ' catch exception and rollback

                    SqlHelper.RollbackTransaction(transaction)
                    WebUtils.DisplayError(Utils.CatchError(ex, _GeneralErrorCode))

                End Try

            End Using

        End Sub

#End Region

#End Region

#Region "Functions\Methods"

        ''' <summary>
        ''' Populates the invoice line cost overrides.
        ''' </summary>
        Private Sub PopulateInvoiceLineCostOverrides()

            ' bind the data to the datagrid
            With gvOverrides
                .DataSource = InvoiceLineCostOverrides
                .DataBind()
            End With

        End Sub

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            ' add in js link for handlers
            JsLinks.Add("LineCostOverride.js")

            ' add in the jquery library
            UseJQuery = True

        End Sub

#End Region

    End Class

End Namespace