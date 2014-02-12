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
    ''' Screen used to list batches of generic creditor payments.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   PaulW    23/03/2011 D11698 Direct Payment Remittance Layout
    '''   ColinD   16/02/2010 D11874 - Created
    ''' </history>
    Partial Class List
        Inherits BasePage

#Region "Fields"

        ' constants
        Private Const _PageTitle As String = "Creditor Payment Batches"
        Private Const _QsInterfaceLogID As String = "ilid"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.CreditorPaymentBatches"

#End Region

#Region "Properties"

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
        ''' Gets the interface log selector control.
        ''' </summary>
        ''' <value>The interface log selector control.</value>
        Private ReadOnly Property InterfaceLogSelectorControl() As Apps.UserControls.InterfaceLogSelector
            Get
                Return CType(interfaceLogSelector1, Apps.UserControls.InterfaceLogSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the reports button control.
        ''' </summary>
        ''' <value>The reports button control.</value>
        Private ReadOnly Property ReportsButtonControl() As IReportsButton
            Get
                Return CType(btnReportsList, IReportsButton)
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
        ''' Gets a value indicating whether [user has recreate interface files permission].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [user has recreate interface files permission]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasRecreateInterfaceFilesPermission() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.CreditorPaymentBatches.ReCreateInterfaceFiles"))
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
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.CreditorPaymentBatches.ViewReports"))
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

            ' setup the pages controls
            SetupJavascriptLinks()
            SetupInterfaceLogSelectorControl()
            SetupReportsButtonsControl()
            SetupStandardButtonsControl()

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            SetupJavascript()
            btnRecreateFiles.Visible = UserHasRecreateInterfaceFilesPermission
            btnReports.Visible = UserHasViewReportsPermission

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Sets up the interface log selector control.
        ''' </summary>
        Private Sub SetupInterfaceLogSelectorControl()

            With InterfaceLogSelectorControl
                .FilterInterfaceLogTypes = InterfaceLogsBL.InterfaceLogType.CreditorsGeneric Or InterfaceLogsBL.InterfaceLogType.CreditorsNonResidential
                .FilterSelectedID = Target.Library.Utils.ToInt32(Request.QueryString(_QsInterfaceLogID))
                .TableCaption = "List of creditor payment batches."
                .TableSummary = .TableCaption
                .InitControl()
            End With

        End Sub

        ''' <summary>
        ''' Setups the javascript.
        ''' </summary>
        Private Sub SetupJavascript()

            Dim js As New StringBuilder()

            ' add filter properties to page in js format
            js.AppendFormat("var QsInterfaceLogID = '{0}';", _QsInterfaceLogID)
            js.AppendFormat("var btnRecreateFiles = GetElement('{0}', true);", btnRecreateFiles.ClientID)
            js.AppendFormat("var btnReports = GetElement('{0}', true);", btnReports.ClientID)
            js.AppendFormat("var btnRemittances = GetElement('{0}', true);", btnRemittances.ClientID)

            ClientScript.RegisterStartupScript(Me.GetType(), _
                                               "Target.Abacus.Web.Apps.CreditorPayments.Batches.List.Startup", _
                                               Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

        ''' <summary>
        ''' Sets up the javascript links for this control.
        ''' </summary>
        Private Sub SetupJavascriptLinks()

            ' add page JS
            JsLinks.Add("List.js")

        End Sub

        Private Sub SetupReportsButtonsControl()

            ' setup std buttons so only the back button is present
            With ReportsButtonControl
                .ButtonText = "List"
                .ReportID = ConstantsManager.GetConstant("AbacusIntranet.WebReport.SimpleListOfCreditorPaymentBatches")
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
