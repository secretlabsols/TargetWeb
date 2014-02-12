Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports System.Text
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.UserLetters
Imports WebUtils = Target.Library.Web

Namespace Apps.Sds.DPContracts

    ''' <summary>
    ''' Main container screen used to terminate domiciliary contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD   30/10/2012   Updated (D12394) - Alterations to cater for new first week direct payment contract frequency - removed date retrictions.
    ''' ColinD   25/10/2012   Updated (D12394) - Alterations to cater for new first week direct payment contract frequency.
    ''' ColinD   12/09/2011   Created (D12161)
    ''' </history>
    Partial Class Balance
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        ' constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _FileNameDirectPaymentContractBalancingPayment As String = "directPaymentContractBalancingPayment.doc"
        Private Const _PageTitle As String = "Balance Direct Payment Contract"
        Private Const _QsDpContractID As String = "id"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.CreditorPayments.DirectPaymentsMonitor"

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

            ' output the scipt
            ClientScript.RegisterStartupScript(Me.GetType(), "BalanceScript", js.ToString(), True)

        End Sub

        ''' <summary>
        ''' Handles the ServerClick event of the btnCreateLetter control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnCreateLetter_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateLetter.ServerClick

            Dim msg As New ErrorMessage()
            Dim letter As New DirectPaymentContractBalancingPaymentUserLetter()

            ' setup letter and merge
            With letter
                .BalancingAmount = Utils.ToDecimal(txtBalancingAmount.GetPostBackValue())
                .BalancingPeriodFrom = Utils.ToDateTime(dtePeriodFrom.GetPostBackValue())
                .BalancingPeriodTo = Utils.ToDateTime(dtePeriodTo.GetPostBackValue())
                .DbConnection = DbConnection
                .DpContractID = QsDpContractID
                ' merge the letter
                msg = .Merge()
                If Not msg.Success Then
                    If msg.Number = UserLetterBase.ERROR_USER_LETTER_NOT_CONFIGURED Then
                        lblError.Text = msg.Message + "<BR /><BR />"
                        txtBalancingAmount.Text = .BalancingAmount
                    Else
                        WebUtils.Utils.DisplayError(msg)
                    End If
                Else
                    ' output letter to client
                    .MergedUserLetter.Save(Response, _FileNameDirectPaymentContractBalancingPayment, Aspose.Words.ContentDisposition.Attachment, Aspose.Words.Saving.DocSaveOptions.CreateSaveOptions(Aspose.Words.SaveFormat.Doc))
                End If
            End With

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Populates the header details.
        ''' </summary>
        Private Sub PopulateHeaderDetails()

            Dim msg As ErrorMessage = Nothing

            ' setup labels
            lblNumber.Text = CurrentDpContract.Number
            lblStartDate.Text = CurrentDpContract.DateFrom.ToString(_DateFormat)
            If CurrentClientBudgetHolder.Type <> ClientBudgetHolderType.ServiceUser Then
                lblBudgetHolder.Text = String.Format("{0} : {1}", CurrentClientBudgetHolder.Reference, CurrentClientBudgetHolder.BudgetHolderName)
            End If
            lblServiceUser.Text = String.Format("{0} : {1}", CurrentClientBudgetHolder.ClientRef, CurrentClientBudgetHolder.ClientFullName)
            lblEndDate.Text = CurrentDpContract.DateTo.ToString(_DateFormat)
            lblEndReason.Text = CurrentDpContractEndReason.Description
            lblRequiredEndDate.Text = CurrentDpContract.RequiredTerminationDate.ToString(_DateFormat)
            lblBalanced.Text = IIf(CurrentDpContract.Balanced = TriState.True, "Yes", "No")

            ' setup date from selection
            With dtePeriodFrom
                .MinimumValue = CurrentDpContract.DateFrom.ToString(_DateFormat)
                .MaximumValue = CurrentDpContract.DateTo.ToString(_DateFormat)
            End With

            ' setup date to selection
            With dtePeriodTo
                .MaximumValue = dtePeriodFrom.MaximumValue
                .MinimumValue = dtePeriodFrom.MinimumValue
                .Text = CurrentDpContract.DateTo.ToString(_DateFormat)
            End With

            ' the balancing amount should be negated as we want to balance the overall cost down to zero as default
            With txtBalancingAmount
                .TextBox.ToolTip = "Enter Balancing Amount - A number not equal to 0"
                .Text = Decimal.Negate(CurrentDpContract.UnderOverPaymentAmount)
            End With

            ' alernate the mark as balanced button depending on state of record
            If CurrentDpContract.Balanced = TriState.True Then

                With btnMarkAsBalanced
                    .Value = "Un-Mark As Balanced"
                    .Attributes.Add("title", "Un-Mark As Balanced?")
                    .Attributes.Add("onclick", "btnMarkAsBalanced_Click(false);")
                End With

            Else

                With btnMarkAsBalanced
                    .Value = "Mark As Balanced"
                    .Attributes.Add("title", "Mark As Balanced?")
                    .Attributes.Add("onclick", "btnMarkAsBalanced_Click(true);")
                End With

            End If

        End Sub

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            ' add in js link for handlers
            JsLinks.Add("Balance.js")

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add date js link 
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Date.js"))

            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery template library
            UseJqueryTemplates = True

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DPContract))

        End Sub

        ''' <summary>
        ''' Setups the projected terminations control.
        ''' </summary>
        Private Sub SetupProjectedTerminationsControl()

            With ProjectedTerminationsControl
                .ParameterDpContractID = QsDpContractID
                .ParameterDpContractRequiredEndDate = CurrentDpContract.RequiredTerminationDate
            End With

        End Sub

#End Region

    End Class

End Namespace