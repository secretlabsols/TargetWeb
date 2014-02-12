Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' <summary>
    ''' User control to encapsulate the listing and selecting of domiciliary provider invoices.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Class DomProviderInvoiceSelector
        Inherits System.Web.UI.UserControl


        Private _hideColumnContractNumber As Boolean = True
        Public Property HideColumnContractNumber() As Boolean
            Get
                Return _hideColumnContractNumber
            End Get
            Set(ByVal value As Boolean)
                _hideColumnContractNumber = value
            End Set
        End Property


        Private _hideColumnProviderReference As Boolean = True
        Public Property HideColumnProviderReference() As Boolean
            Get
                Return _hideColumnProviderReference
            End Get
            Set(ByVal value As Boolean)
                _hideColumnProviderReference = value
            End Set
        End Property


        Private _hideColumnSUReference As Boolean = True
        Public Property HideColumnSUReference() As Boolean
            Get
                Return _hideColumnSUReference
            End Get
            Set(ByVal value As Boolean)
                _hideColumnSUReference = value
            End Set
        End Property


        Private _enableCostedVisits As Boolean = True
        Public Property enableCostedVisits() As Boolean
            Get
                Return _enableCostedVisits
            End Get
            Set(ByVal value As Boolean)
                _enableCostedVisits = value
            End Set
        End Property


        Private _thePage As BasePage
        Public Property thePage() As BasePage
            Get
                Return _thePage
            End Get
            Set(ByVal value As BasePage)
                _thePage = value
            End Set
        End Property

        Private _selectedInvoiceID As Integer
        Public Property selectedInvoiceID() As Integer
            Get
                Return _selectedInvoiceID
            End Get
            Set(ByVal value As Integer)
                _selectedInvoiceID = value
            End Set
        End Property

        Private _providerID As Integer
        Public Property providerID() As Integer
            Get
                Return _providerID
            End Get
            Set(ByVal value As Integer)
                _providerID = value
            End Set
        End Property

        Private _serviceUserID As Integer
        Public Property serviceUserID() As Integer
            Get
                Return _serviceUserID
            End Get
            Set(ByVal value As Integer)
                _serviceUserID = value
            End Set
        End Property

        Private _contractID As Integer
        Public Property contractID() As Integer
            Get
                Return _contractID
            End Get
            Set(ByVal value As Integer)
                _contractID = value
            End Set
        End Property

        Private _invoiceNumber As String
        Public Property invoiceNumber() As String
            Get
                Return _invoiceNumber
            End Get
            Set(ByVal value As String)
                _invoiceNumber = value
            End Set
        End Property

        Private _weekendingFrom As Date
        Public Property weekendingFrom() As Date
            Get
                Return _weekendingFrom
            End Get
            Set(ByVal value As Date)
                _weekendingFrom = value
            End Set
        End Property

        Private _weekendingTo As Date
        Public Property weekendingTo() As Date
            Get
                Return _weekendingTo
            End Get
            Set(ByVal value As Date)
                _weekendingTo = value
            End Set
        End Property

        Private _unPaid As Boolean
        Public Property unPaid() As Boolean
            Get
                Return _unPaid
            End Get
            Set(ByVal value As Boolean)
                _unPaid = value
            End Set
        End Property

        Private _paid As Boolean
        Public Property paid() As Boolean
            Get
                Return _paid
            End Get
            Set(ByVal value As Boolean)
                _paid = value
            End Set
        End Property

        Private _authorised As Boolean
        Public Property authorised() As Boolean
            Get
                Return _authorised
            End Get
            Set(ByVal value As Boolean)
                _authorised = value
            End Set
        End Property

        Private _suspended As Boolean
        Public Property suspended() As Boolean
            Get
                Return _suspended
            End Get
            Set(ByVal value As Boolean)
                _suspended = value
            End Set
        End Property

        Private _dateFrom As Date
        Public Property dateFrom() As Date
            Get
                Return _dateFrom
            End Get
            Set(ByVal value As Date)
                _dateFrom = value
            End Set
        End Property

        Private _dateTo As Date
        Public Property dateTo() As Date
            Get
                Return _dateTo
            End Get
            Set(ByVal value As Date)
                _dateTo = value
            End Set
        End Property

        Private _userHasRetractCommand As Boolean
        Public Property userHasRetractCommand() As Boolean
            Get
                Return _userHasRetractCommand
            End Get
            Set(ByVal value As Boolean)
                _userHasRetractCommand = value
            End Set
        End Property

        Private _hideRetraction As Boolean
        Public Property hideRetraction() As Boolean
            Get
                Return _hideRetraction
            End Get
            Set(ByVal value As Boolean)
                _hideRetraction = value
            End Set
        End Property

        Private _pscheduleId As Integer
        Public Property pscheduleId() As Integer
            Get
                Return _pscheduleId
            End Get
            Set(ByVal value As Integer)
                _pscheduleId = value
            End Set
        End Property

        Private _showPaymentSchedule As Boolean
        Public Property showPaymentSchedule() As Boolean
            Get
                Return _showPaymentSchedule
            End Get
            Set(ByVal value As Boolean)
                _showPaymentSchedule = value
            End Set
        End Property


        Private _IsPeriodicBlockContract As Boolean = False
        Public Property IsPeriodicBlockContract() As Boolean
            Get
                Return _IsPeriodicBlockContract
            End Get
            Set(ByVal value As Boolean)
                _IsPeriodicBlockContract = value
            End Set
        End Property

      

        Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
            Initialize()
        End Sub

        Public Sub Initialize()
            Dim msg As New ErrorMessage
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String

            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/DomProviderInvoiceSelector.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomProviderInvoice))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProviderInvoiceStatus))

            msg = New ErrorMessage
            msg = PopulateContractType()
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
            End If

            js = String.Format( _
                 "currentPage={0};" & _
                 "providerID={1};" & _
                 "clientID={2};" & _
                 "contractID={3};" & _
                 "paid={4};" & _
                 "unPaid={5};" & _
                 "authorised={6};" & _
                 "suspended={7};" & _
                 "invoiceNumber=""{8}"";" & _
                 "weekendingDateFrom={9};" & _
                 "weekendingDateTo={10};" & _
                 "dateFrom={11};" & _
                 "dateTo={12};" & _
                 "selectedInvoiceID={13};" & _
                 "userHasRetractCommand={14};" & _
                 "pScheduleId={15};" & _
                 "hideContractNumber={16};" & _
                 "hideProviderReference={17};" & _
                 "hideSUReference={18};" & _
                 "hideRetraction={19};" & _
                 "showPaymentSchedule={20};" & _
                 "currentContractIsPeriodicBlock={21}", _
                 currentPage, providerID, serviceUserID, contractID, paid.ToString.ToLower, unPaid.ToString.ToLower, authorised.ToString.ToLower, suspended.ToString.ToLower, _
                 invoiceNumber, IIf(Target.Library.Utils.IsDate(weekendingFrom), WebUtils.GetDateAsJavascriptString(weekendingFrom), "null"), _
                 IIf(Target.Library.Utils.IsDate(weekendingTo), WebUtils.GetDateAsJavascriptString(weekendingTo), "null"), _
                 IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
                 IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
                 selectedInvoiceID, _
                 userHasRetractCommand.ToString.ToLower(), _
                 pscheduleId, _
                 Me.HideColumnContractNumber.ToString().ToLower(), _
                 Me.HideColumnProviderReference.ToString().ToLower(), _
                 Me.HideColumnSUReference.ToString.ToLower(), _
                 hideRetraction.ToString().ToLower(), _
                 showPaymentSchedule.ToString().ToLower(), _
                 IsPeriodicBlockContract.ToString().ToLower() _
            )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub


        ''' <summary>
        ''' Populates the type of the contract.
        ''' </summary>
        Private Function PopulateContractType() As ErrorMessage
            Dim msg As New ErrorMessage
            Dim tempContractId As Integer = 0

            If (Target.Library.Utils.ToInt32(contractID) = 0) Then
                If pscheduleId <> 0 Then
                    Dim pschedule As DataClasses.PaymentSchedule = New DataClasses.PaymentSchedule(thePage.DbConnection, String.Empty, String.Empty)
                    msg = pschedule.Fetch(pscheduleId)
                    If Not msg.Success Then Return msg
                    tempContractId = pschedule.DomContractID
                End If
            Else
                tempContractId = contractID
            End If

            If tempContractId <> 0 Then
                Dim contract As DataClasses.DomContract = New DataClasses.DomContract(thePage.DbConnection, String.Empty, String.Empty)
                msg = contract.Fetch(tempContractId)
                If Not msg.Success Then Return msg

                If contract.ContractType = [Enum].GetName(GetType(DomContractType), DomContractType.BlockPeriodic) Then
                    IsPeriodicBlockContract = True
                Else
                    IsPeriodicBlockContract = False
                End If

            End If

            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

    End Class



End Namespace

