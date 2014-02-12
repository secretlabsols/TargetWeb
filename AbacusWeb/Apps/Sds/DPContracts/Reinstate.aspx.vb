Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports System.Text
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web

Namespace Apps.Sds.DPContracts

    ''' <summary>
    ''' Main container screen used to terminate Direct Payment contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD  08/09/2011   D12203 - Remove Contract from any reference to Direct Payment Contract in UI.
    ''' ColinD  08/09/2011   Overhaul to remove business logic and inline sql strored proc calls...use bl instead.
    ''' JohnF   26/08/2011   Use correct menu item when initialising (#7000)
    ''' JohnF   24/08/2010   Added audit log info (D11801)
    ''' JohnF   27/07/2010   Created (D11801)
    ''' </history>
    Partial Class Reinstate
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        ' constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _PageTitle As String = "Re-instate Direct Payment"
        Private Const _QsDpContractID As String = "id"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.DirectPaymentContracts"

        ' locals
        Private _CurrentClientBudgetHolder As ViewableClientBudgetHolder = Nothing
        Private _CurrentDpContract As DPContract = Nothing
        Private _CurrentDpContractEndReason As ContractEndReason = Nothing

#End Region

#Region "Properties"

#Region "QueryString Properties"

        ''' <summary>
        ''' Gets the dp contract ID from the query string.
        ''' </summary>
        ''' <value>The dp contract ID.</value>
        Private ReadOnly Property QsDpContractID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString(_QsDpContractID))
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Gets the current client budget holder.
        ''' </summary>
        ''' <value>The current client budget holder.</value>
        Private ReadOnly Property CurrentClientBudgetHolder() As ViewableClientBudgetHolder
            Get
                If _CurrentClientBudgetHolder Is Nothing Then
                    ' if we havent fetched the item then do so
                    Dim msg As ErrorMessage = BudgetHolderBL.GetClientBudgetHolderDetails(DbConnection, CurrentDpContract.ClientBudgetHolderID, _CurrentClientBudgetHolder)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _CurrentClientBudgetHolder
            End Get
        End Property

        ''' <summary>
        ''' Gets the current dp contract.
        ''' </summary>
        ''' <value>The current dp contract.</value>
        Private ReadOnly Property CurrentDpContract() As DPContract
            Get
                If QsDpContractID > 0 AndAlso _CurrentDpContract Is Nothing Then
                    ' if we havent fetched the contract then do so if an id is stated in qs
                    Dim msg As ErrorMessage = DPContractBL.GetDPContract(DbConnection, QsDpContractID, _CurrentDpContract)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _CurrentDpContract
            End Get
        End Property

        ''' <summary>
        ''' Gets the current dp contract end reason.
        ''' </summary>
        ''' <value>The current dp contract end reason.</value>
        Private ReadOnly Property CurrentDpContractEndReason() As ContractEndReason
            Get
                If _CurrentDpContractEndReason Is Nothing Then
                    _CurrentDpContractEndReason = New ContractEndReason(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                    Dim msg As ErrorMessage = _CurrentDpContractEndReason.Fetch(CurrentDpContract.EndReasonID)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _CurrentDpContractEndReason
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

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), _PageTitle)

            ' setup the standard buttons control
            With StandardButtonsControl
                .AllowBack = True
                .AllowNew = False
                .AllowEdit = False
                .AllowDelete = False
                .AllowFind = False
                .InitialMode = StdButtonsMode.Fetched
            End With

            ' populate items
            PopulateHeaderDetails()

            ' setup css/js etc
            SetupCss()
            SetupJavaScript()

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim js As New StringBuilder()

            js.AppendFormat("qsDpContractID = {0};", QsDpContractID)

            ' output the scipt
            ClientScript.RegisterStartupScript(Me.GetType(), "ReInstateScript", js.ToString(), True)

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Populates the header details.
        ''' </summary>
        Private Sub PopulateHeaderDetails()

            ' setup labels
            lblNumber.Text = CurrentDpContract.Number
            lblStartDate.Text = CurrentDpContract.DateFrom.ToString(_DateFormat)
            If CurrentClientBudgetHolder.Type <> ClientBudgetHolderType.ServiceUser Then
                lblBudgetHolder.Text = String.Format("{0} : {1}", CurrentClientBudgetHolder.Reference, CurrentClientBudgetHolder.BudgetHolderName)
            End If
            lblServiceUser.Text = String.Format("{0} : {1}", CurrentClientBudgetHolder.ClientRef, CurrentClientBudgetHolder.ClientFullName)
            lblEndDate.Text = CurrentDpContract.DateTo.ToString(_DateFormat)
            lblEndReason.Text = CurrentDpContractEndReason.Description

        End Sub

        ''' <summary>
        ''' Setups the CSS.
        ''' </summary>
        Private Sub SetupCss()

            Dim style As New StringBuilder()

            style.Append("label.label { float:left; width:12em; font-weight:bold; }")
            style.Append("span.label { float:left; width:12em; padding-right:1em; font-weight:bold; }")
            style.Append(".Amendment {padding-left:2em; color:red; font-style:italic; )")

            Me.AddExtraCssStyle(style.ToString)

        End Sub

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            ' add in js link for handlers
            JsLinks.Add("Reinstate.js")

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DPContract))

        End Sub

#End Region

    End Class

End Namespace