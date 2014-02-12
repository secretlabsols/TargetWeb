Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports System.Text
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports WebUtils = Target.Library.Web

Namespace Apps.Sds.DPContracts

    ''' <summary>
    ''' Main container screen used to terminate domiciliary contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD  13/11/2012   #7606 - Auto balance button does not listen to role permissions and is always available to the user.
    ''' ColinD  25/10/2012   D12394 - Alterations to cater for new first week direct payment contract frequency.
    ''' ColinD  08/09/2011   D12203 - Remove Contract from any reference to Direct Payment Contract in UI.
    ''' ColinD  08/09/2011   Overhaul (all code/markup replaced) to handle new projected terminations functionality.
    ''' JohnF   26/08/2011   Use correct menu item when initialising (#7000)
    ''' JohnF   29/09/2010   Exclude redundant end reasons (D11801) 
    ''' JohnF   24/08/2010   Added audit log info (D11801)
    ''' JohnF   27/07/2010   Created (D11801)
    ''' </history>
    Partial Class Terminate
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        ' constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _PageTitle As String = "Terminate Direct Payment"
        Private Const _QsDpContractID As String = "id"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.DirectPaymentContracts"

        ' locals
        Private _CurrentClientBudgetHolder As ViewableClientBudgetHolder = Nothing
        Private _CurrentDpContract As DPContract = Nothing
        Private _CurrentEndReasons As List(Of ContractEndReason) = Nothing

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
        ''' Gets the current end reasons.
        ''' </summary>
        ''' <value>The current end reasons.</value>
        Private ReadOnly Property CurrentEndReasons() As List(Of ContractEndReason)
            Get
                If _CurrentEndReasons Is Nothing Then
                    ' if not fetched then do so
                    Dim tmpEndReasons As ContractEndReasonCollection = Nothing
                    ' fetch the temp list
                    Dim msg As ErrorMessage = ContractEndReason.FetchList(conn:=DbConnection, list:=tmpEndReasons, redundant:=TriState.False, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                    ' add filtered items into list
                    _CurrentEndReasons = New List(Of ContractEndReason)()
                    For Each currentEndReason As ContractEndReason In tmpEndReasons
                        ' loop each end reason and add to collection when match
                        If (currentEndReason.Usage And ContractEndReasonUsage.DPContracts) <> 0 AndAlso _
                            (currentEndReason.Usage And ContractEndReasonUsage.DPContractDetails) <> 0 Then
                            _CurrentEndReasons.Add(currentEndReason)
                        End If
                    Next
                End If
                Return _CurrentEndReasons
            End Get
        End Property

        ''' <summary>
        ''' Gets the projected terminations control.
        ''' </summary>
        ''' <value>The projected terminations control.</value>
        Private ReadOnly Property ProjectedTerminationsControl() As UserControls.ucDPContractTerminationProjections
            Get
                Return CType(ucProjectedTerminations, UserControls.ucDPContractTerminationProjections)
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
            PopulateEndReasons()
            SetupProjectedTerminationsControl()

            ' setup js
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

            If CurrentDpContract.DateTerminated.Ticks > 0 Then
                ' if we have already terminated this then prevent any until reinstated

                js.Append("$(function(){ disableForm(true); });")

            End If

            ' output the scipt
            ClientScript.RegisterStartupScript(Me.GetType(), "TerminateScript", js.ToString(), True)

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Populates the end reasons.
        ''' </summary>
        Private Sub PopulateEndReasons()

            With cboEndReason
                With .DropDownList
                    .ToolTip = "Select an End Reason"
                    .Items.Clear()
                    .DataSource = CurrentEndReasons
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' add a blank entry first
                    .Items.Insert(0, New ListItem("Please Select...", String.Empty))
                End With
                .SelectPostBackValue()
            End With

        End Sub

        ''' <summary>
        ''' Populates the header details.
        ''' </summary>
        Private Sub PopulateHeaderDetails()

            Dim dateFromAllowableDay As Integer = 0
            Dim dateFromDefaultDate As DateTime = CurrentDpContract.DateFrom
            Dim dateFromMinimumDate As DateTime = CurrentDpContract.DateFrom
            Dim dateToAllowableDay As Integer = 0
            Dim dateToDefaultDate As DateTime = CurrentDpContract.DateTo
            Dim dateToMinimumDate As DateTime = CurrentDpContract.DateTo
            Dim msg As ErrorMessage = Nothing

            ' get the default dates etc
            msg = DPContractBL.GetDPContractDates(DbConnection, CurrentDpContract, CurrentClientBudgetHolder.ClientID, dateFromAllowableDay, dateFromMinimumDate, dateFromDefaultDate, dateToAllowableDay, dateToMinimumDate, dateToDefaultDate, Nothing)
            If Not msg.Success Then WebUtils.Utils.DisplayError(msg)

            ' setup labels
            lblNumber.Text = CurrentDpContract.Number
            lblStartDate.Text = CurrentDpContract.DateFrom.ToString(_DateFormat)
            If CurrentClientBudgetHolder.Type <> ClientBudgetHolderType.ServiceUser Then
                lblBudgetHolder.Text = String.Format("{0} : {1}", CurrentClientBudgetHolder.Reference, CurrentClientBudgetHolder.BudgetHolderName)
            End If
            lblServiceUser.Text = String.Format("{0} : {1}", CurrentClientBudgetHolder.ClientRef, CurrentClientBudgetHolder.ClientFullName)

            ' setup date selection
            With dteTerminateDate
                .MinimumValue = CurrentDpContract.DateFrom.ToString(_DateFormat)
                .Text = Date.Today
            End With

        End Sub

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            Dim hasAutoBalancePermission As Boolean = UserHasMenuItem(WebUtils.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.CreditorPayments.DirectPaymentsMonitor"))
            Dim js As New StringBuilder()

            ' add in js link for handlers
            JsLinks.Add("Terminate.js")

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery template library
            UseJqueryTemplates = True

            ' add any script to be output
            js.AppendFormat("hasAutoBalancePermission = {0}", WebUtils.Utils.GetBooleanAsJavascriptString(hasAutoBalancePermission))

            ' output the js to be output
            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", WebUtils.Utils.WrapClientScript(js.ToString))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DPContract))

        End Sub

        ''' <summary>
        ''' Setups the projected terminations control.
        ''' </summary>
        Private Sub SetupProjectedTerminationsControl()

            With ProjectedTerminationsControl
                .ParameterDpContractID = QsDpContractID
                .ParameterDpContractRequiredEndDate = Utils.ToDateTime(dteTerminateDate.Text)
            End With

        End Sub

#End Region

    End Class

End Namespace