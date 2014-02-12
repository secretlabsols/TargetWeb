Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.Sds.DPPayments

    ''' <summary>
    ''' View Direct Payments using parameters passed to the page. 
    ''' 
    '''    Required Parameters are:
    '''       1. ID = The ID of the DPPayment record
    ''' 
    '''    Optional Parameters are:
    '''       1. BackURL = The URL to return to when using the back button. Back button will not be available if not specified.
    ''' 
    ''' </summary>
    ''' <history>
    ''' Colin Daly  30/04/2011  Updated (D11990E) - Updated to account for removal of NetValue column from the DPPaymentDetail table.
    ''' Colin Daly  03/03/2011  Updated (D11874) - Updated behaviour of page when in popup mode.
    ''' Colin Daly  11/08/2010  Created (D11802)
    ''' </history>
    Partial Public Class View
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        ' locals
        Private _DPPayment As DataClasses.DPPayment
        Private _DPPaymentDetails As DataClasses.Collections.DPPaymentDetailCollection = Nothing
        Private _DPPaymentDetailsFetched As Boolean = False

        ' Constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _DateTimeFormat As String = "dd/MM/yyyy HH:mm:ss"
        Private Const _DecimalNumbersFormat As String = "C"
        Private Const _ErrorNoPaymentID As String = "Direct Payment ID must be specified."
        Private Const _ErrorNoPermission As String = "You do not have permission to view direct payments."
        Private Const _NavigationItemKey As String = "AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.CreditorPayments"
        Private Const _NoPaymentDetailsMessage As String = "There are currently no payment details."
        Private Const _PageTitle As String = "View Direct Payments"
        Private Const _QsAutoPopup As String = "autopopup"
        Private Const _QSPaymentID As String = "id"
        Private Const _ReferenceAndNameFormat As String = "{0}, {1}"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Me.InitPage(ConstantsManager.GetConstant(_NavigationItemKey), _PageTitle)

            ' hook onto AddCustomControls event of standard buttons control so we can add addtional buttons later
            AddHandler StandardButtonsControl.AddCustomControls, AddressOf StandardButtons_AddCustomControls

        End Sub

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not IsPostBack And ValidateRequest() Then
                ' if first hit and request is valid

                ' only allow the back button, this a view only interface
                With StandardButtonsControl
                    .AllowBack = Not IsPopupWindow
                    .AllowDelete = False
                    .AllowEdit = False
                    .AllowFind = False
                    .AllowNew = False
                End With

                ' populate all tabs with data
                PopulatePaymentTabs()

            End If

        End Sub

        ''' <summary>
        ''' Allows you to add custom controls to the standard buttons control.
        ''' </summary>
        ''' <param name="controls">The controls.</param>
        Private Sub StandardButtons_AddCustomControls(ByRef controls As ControlCollection)

            Dim btnAuditDetails As HtmlInputButton = New HtmlInputButton("button")

            With btnAuditDetails
                .ID = "btnAuditDetails"
                .Value = "Audit Details"
            End With

            ' add audit button into list of controls in standard buttons
            controls.Add(btnAuditDetails)

            ' setup the audit details control, setting defaults
            With AuditDetailsControl
                .ToggleControlID = btnAuditDetails.ClientID
                .Collapsed = True
            End With

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the audit details control.
        ''' </summary>
        ''' <value>The audit details control.</value>
        Private ReadOnly Property AuditDetailsControl() As IBasicAuditDetails
            Get
                Return CType(auditDetails, IBasicAuditDetails)
            End Get
        End Property

        ''' <summary>
        ''' Gets the DP payment using the property PaymentID.
        ''' </summary>
        ''' <value>The DP payment.</value>
        Private ReadOnly Property DPPayment() As DataClasses.DPPayment
            Get

                If PaymentID.HasValue AndAlso _DPPayment Is Nothing Then
                    ' if we havent already fetched the payment

                    Dim msg As ErrorMessage = Nothing

                    ' fetch dp payment
                    msg = DirectPaymentPaymentsBL.GetDPPayment(DbConnection, PaymentID.Value, _DPPayment)

                    If msg.Success = False Then
                        ' if failed to get payment then return error

                        _DPPayment = Nothing
                        Target.Library.Web.Utils.DisplayError(msg)

                    End If

                End If

                Return _DPPayment

            End Get
        End Property

        ''' <summary>
        ''' Gets the DP payment details for the PaymentID.
        ''' </summary>
        ''' <value>The DP payment details.</value>
        Private ReadOnly Property DPPaymentDetails() As DataClasses.Collections.DPPaymentDetailCollection
            Get

                If PaymentID.HasValue AndAlso _DPPaymentDetailsFetched = False Then
                    ' if we havent already fetched the payment details already

                    Dim msg As ErrorMessage = Nothing

                    ' fetch dp payment
                    msg = DirectPaymentPaymentsBL.GetDPPaymentDetailsByDpPaymentID(DbConnection, PaymentID.Value, _DPPaymentDetails)

                    If msg.Success = False Then
                        ' failed to fetch payment details so throw an error

                        _DPPaymentDetails = Nothing
                        _DPPaymentDetailsFetched = False
                        Target.Library.Web.Utils.DisplayError(msg)

                    Else
                        ' negate ServiceUserContribution lines (for display purposes only)
                        For Each dp As DataClasses.DPPaymentDetail In _DPPaymentDetails
                            If dp.Type = DirectPaymentPaymentsBL.DPPaymentDetailType.ServiceUserContribution Then
                                dp.LineValue = Decimal.Negate(dp.LineValue)
                            End If
                        Next

                        ' flag this property as used so we dont fetch again (during this hit only)
                        _DPPaymentDetailsFetched = True

                    End If

                End If

                Return _DPPaymentDetails

            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance is popup window.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance is popup window; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property IsPopupWindow() As Boolean
            Get
                Return (Target.Library.Utils.ToInt32(Request.QueryString(_QsAutoPopup)) = 1)
            End Get
        End Property

        ''' <summary>
        ''' Gets the payment ID from the query string. Null if not specified or has no value.
        ''' </summary>
        ''' <value>The budget holder ID.</value>
        Private ReadOnly Property PaymentID() As Nullable(Of Integer)
            Get
                Return GetIntegerFromQueryString(_QSPaymentID)
            End Get
        End Property

        ''' <summary>
        ''' Gets the standard buttons control.
        ''' </summary>
        ''' <value>The standard buttons.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Displays an error message
        ''' </summary>
        ''' <param name="errorMessage">The error message to display</param>
        ''' <remarks></remarks>
        Private Sub DisplayError(ByVal errorMessage As String)

            lblError.Text = String.Format("{0}<br /><br />", errorMessage)
            pnlForm.Visible = False

        End Sub

        ''' <summary>
        ''' Gets an integer from query string.
        ''' </summary>
        ''' <param name="key">The key.</param>
        ''' <returns></returns>
        Private Function GetIntegerFromQueryString(ByVal key As String) As Nullable(Of Integer)

            Dim qsInt As String = Request.QueryString(key)
            Dim tmpInt As Integer = Target.Library.Utils.ToInt32(qsInt)

            If tmpInt > 0 Then
                ' if the value is larger than 0 return

                Return tmpInt

            Else
                ' else return nothing

                Return Nothing

            End If

        End Function

        ''' <summary>
        ''' Populates the payment tabs.
        ''' </summary>
        Private Sub PopulatePaymentTabs()

            If PaymentID.HasValue Then
                ' if we have payment id to work with

                PopulatePayment()
                PopulatePaymentDetails()

            Else
                ' no payment id to work with so advise

                DisplayError(_ErrorNoPaymentID)

            End If

        End Sub

        ''' <summary>
        ''' Populates the payment.
        ''' </summary>
        Private Sub PopulatePayment()

            If ValidateRequest() Then
                ' if we have a valid request

                Dim budgetHolder As ClientBudgetHolder = Nothing
                Dim budgetHolderReferenceAndName As String = String.Empty
                Dim clientDetail As DataClasses.ClientDetail = Nothing
                Dim contract As DataClasses.DPContract = Nothing
                Dim msg As ErrorMessage = Nothing
                Dim payment As DataClasses.DPPayment = Nothing

                ' fetch dp payment
                msg = DirectPaymentPaymentsBL.GetDPPayment(DbConnection, PaymentID.Value, payment)
                If msg.Success = False Then Target.Library.Web.Utils.DisplayError(msg)

                ' fetch dp contract for dp payment
                msg = DPContractBL.GetDPContract(DbConnection, payment.DPContractID, contract)
                If msg.Success = False Then Target.Library.Web.Utils.DisplayError(msg)

                ' fetch client budget holder
                msg = BudgetHolderBL.GetClientBudgetHolder(Me.DbConnection, contract.ClientBudgetHolderID, String.Empty, String.Empty, budgetHolder)
                If msg.Success = False Then Target.Library.Web.Utils.DisplayError(msg)

                clientDetail = New ClientDetail(conn:=Me.DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                msg = clientDetail.Fetch(budgetHolder.ClientID)
                If msg.Success = False Then Target.Library.Web.Utils.DisplayError(msg)

                If budgetHolder.Type = CType(ClientBudgetHolderType.ServiceUser, Byte) Then
                    ' budget holder is service user

                    budgetHolderReferenceAndName = String.Format(_ReferenceAndNameFormat, clientDetail.Reference, clientDetail.Name)

                Else
                    ' budget holder is third party

                    Dim partyBudgetHolder As ThirdPartyBudgetHolder = Nothing

                    msg = BudgetHolderBL.GetBudgetHolder(Me.DbConnection, partyBudgetHolder, budgetHolder.ThirdPartyBudgetHolderID)
                    If msg.Success = False Then Target.Library.Web.Utils.DisplayError(msg)
                    budgetHolderReferenceAndName = String.Format(_ReferenceAndNameFormat, partyBudgetHolder.Reference, partyBudgetHolder.OrganisationName)

                End If

                ' header tab
                tbeContractNumber.Text = contract.Number
                tbeSvcUserRefAndName.Text = String.Format(_ReferenceAndNameFormat, clientDetail.Reference, clientDetail.Name)
                tbeBudgetHolderRefAndName.Text = budgetHolderReferenceAndName
                tbeDateFrom.Text = payment.DateFrom.ToString(_DateFormat)
                tbeDateTo.Text = payment.DateTo.ToString(_DateFormat)
                tbePaymentNumber.Text = payment.PaymentNumber
                tbeStatusAndStatusDate.Text = String.Format("{0} at {1}", _
                                                            [Enum].GetName(GetType(DirectPaymentPaymentsBL.DPPaymentStatus), payment.Status), _
                                                                           payment.StatusDate.ToString(_DateTimeFormat))

                ' audit details
                With AuditDetailsControl
                    .EnteredBy = payment.CreatedBy
                    .EnteredByCaption = "Created By:"
                    .DateEntered = payment.CreatedDate.ToString(_DateFormat)
                    .DateEnteredCaption = "Created Date:"
                    If Target.Library.Utils.IsDate(payment.StatusDate) Then
                        .AmendedVisible = True
                        .LastAmendedBy = payment.StatusSetBy
                        .LastAmendedByCaption = "Status Set By:"
                        .DateLastAmended = payment.StatusDate.ToString(_DateFormat)
                        .DateLastAmendedCaption = "Status Set Date:"
                    Else
                        .AmendedVisible = False
                    End If
                End With

                'detail tab
                PopulatePaymentDetails()

                ' summary tab
                PopulatePaymentSummary()

            End If

        End Sub

        ''' <summary>
        ''' Populates the payment details.
        ''' </summary>
        Private Sub PopulatePaymentDetails()

            If ValidateRequest() Then
                ' if request params are valid

                Dim paymentDetails As DataClasses.Collections.DPPaymentDetailCollection = DPPaymentDetails

                If Not paymentDetails Is Nothing AndAlso paymentDetails.Count > 0 Then
                    ' if we have some payment details

                    gvDetails.Visible = True
                    lblNoDetails.Visible = False

                    ' set data source to payment details, sorted by DateFrom, SequenceNumber
                    gvDetails.DataSource = paymentDetails.ToArray().OrderBy(Function(pd) pd.DateFrom).ThenBy(Function(pd) pd.SequenceNumber)
                    gvDetails.DataBind()

                Else
                    ' else we have no payment details

                    gvDetails.Visible = False
                    lblNoDetails.Visible = True
                    lblNoDetails.Text = _NoPaymentDetailsMessage

                    ' set data source to nowt so nothing is returned
                    gvDetails.DataSource = Nothing
                    gvDetails.DataBind()

                End If

            End If

        End Sub

        ''' <summary>
        ''' Populates the payment summary.
        ''' </summary>
        Private Sub PopulatePaymentSummary()

            If ValidateRequest() Then
                ' if we have valid request details

                Dim paymentDetails As DataClasses.Collections.DPPaymentDetailCollection = DPPaymentDetails
                Dim totalGrossPayment As Decimal = 0
                Dim totalServiceUserContribution As Decimal = 0

                If Not paymentDetails Is Nothing AndAlso paymentDetails.Count > 0 Then
                    ' if we have some payment details to process

                    ' get total gross payments
                    totalGrossPayment = (From grossPayment In paymentDetails.ToArray() _
                                            Where grossPayment.Type = CType(DirectPaymentPaymentsBL.DPPaymentDetailType.GrossPayment, Byte) _
                                                Select grossPayment.LineValue).Sum()

                    ' get total service user contributions
                    totalServiceUserContribution = (From sucPayment In paymentDetails.ToArray() _
                                                         Where sucPayment.Type = CType(DirectPaymentPaymentsBL.DPPaymentDetailType.ServiceUserContribution, Byte) _
                                                            Select sucPayment.LineValue).Sum()

                End If

                ' set totals 
                tbeTotalGrossPayments.Text = totalGrossPayment.ToString(_DecimalNumbersFormat)
                tbeTotalServiceUserContributions.Text = totalServiceUserContribution.ToString(_DecimalNumbersFormat)
                tbeNetPayment.Text = (totalGrossPayment + totalServiceUserContribution).ToString(_DecimalNumbersFormat)

            End If

        End Sub

        ''' <summary>
        ''' Validates the request i.e. has the correct query string params.
        ''' </summary>
        ''' <returns></returns>
        Private Function ValidateRequest() As Boolean

            If PaymentID.HasValue Then
                ' if we have payment id to work with

                Return True

            Else
                ' no payment id to work with so advise

                DisplayError(_ErrorNoPaymentID)
                Return False

            End If

        End Function

#End Region

    End Class

End Namespace