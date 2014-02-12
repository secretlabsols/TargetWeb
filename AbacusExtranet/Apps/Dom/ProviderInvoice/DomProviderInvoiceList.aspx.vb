Imports Constants = Target.Library.Web
Imports Target.Library
Imports Target.Web.Apps.Security


Namespace Apps.Dom.ProviderInvoice

    Partial Public Class DomProviderInvoiceList
        Inherits Target.Web.Apps.BasePage

#Region " fields "
        Private Const _WebCmdRetractKey As String = "AbacusExtranet.WebNavMenuItemCommand.DomiciliaryCare.ProviderInvoices.RetractProviderInvoice"
        Private _pScheduleId As Integer
        Private _providerId As Integer
        Private _clientId As Integer
        Private _contractId As Integer
        Private _unPaid As Boolean
        Private _suspended As Boolean
        Private _authorised As Boolean
        Private _paid As Boolean
        Private _UserHasRetractcommand As Boolean
        Private _dateFrom As New Date
        Private _dateTo As New Date
        Private _hideRetraction As Boolean = True
        Private _showPaymentschedule As Boolean = True
#End Region

#Region " Page Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.UseJQuery = True
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ProviderInvoiceEnquiry"), "Provider Invoices")
            Me.JsLinks.Add("DomProviderInvoiceList.js")

            LoadDetails()
            LoadQueryStrings()
            SetFilters()


            _UserHasRetractcommand = UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdRetractKey))
            PopulateProviderInvoice()
            pSchedules.SingleLiner = False

            IntialiseJsVariables()

            SetReports()

            If Not Me.IsPostBack Then
                Me.CustomNavAdd(True)
            End If

        End Sub


#End Region

#Region " Initialize javascript "
        Private Sub IntialiseJsVariables()

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.DomProviderInvoiceList.Startup", _
              Target.Library.Web.Utils.WrapClientScript(String.Format("chkUnpaidId=""{0}"";" & _
                                                                      "chkSuspendedId=""{1}"";" & _
                                                                      "chkAuthorisedId=""{2}"";" & _
                                                                      "chkPaidId=""{3}"";" & _
                                                                      "dteFromId=""{4}"";" & _
                                                                      "dteToId=""{5}"";", _
                                                                      chkUnpaid.ClientID, _
                                                                      chkSuspended.ClientID, _
                                                                      chkAuthorised.ClientID, _
                                                                      chkPaid.ClientID, _
                                                                      dteFrom.ClientID, _
                                                                      dteTo.ClientID _
                                                                      )) _
                                                    )

            'InPlaceProviderSelector.ClientID
        End Sub

#End Region

#Region " Populate Data"
        Private Sub PopulateProviderInvoice()
            pInvoice.HideColumnContractNumber = True
            pInvoice.HideColumnProviderReference = True
            pInvoice.HideColumnSUReference = False
            pInvoice.thePage = Me.Page
            pInvoice.selectedInvoiceID = 0
            pInvoice.providerID = _providerId
            pInvoice.serviceUserID = _clientId
            pInvoice.contractID = _contractId
            pInvoice.invoiceNumber = Nothing
            pInvoice.weekendingFrom = New DateTime
            pInvoice.weekendingTo = New DateTime
            pInvoice.unPaid = chkUnpaid.Checked
            pInvoice.paid = chkPaid.Checked
            pInvoice.authorised = chkAuthorised.Checked
            pInvoice.suspended = chkSuspended.Checked
            pInvoice.dateFrom = _dateFrom
            pInvoice.dateTo = _dateTo
            pInvoice.userHasRetractCommand = _UserHasRetractcommand
            pInvoice.hideRetraction = _hideRetraction
            pInvoice.pscheduleId = _pScheduleId
            pInvoice.showPaymentSchedule = _showPaymentschedule
            pInvoice.Initialize()
        End Sub
#End Region

#Region "Reload private variabled from query string "

        Private Sub LoadQueryStrings()
            _pScheduleId = Utils.ToInt32(Request.QueryString("pScheduleId"))
            _unPaid = Utils.ToBoolean(Request.QueryString("unpaid"))
            _suspended = Utils.ToBoolean(Request.QueryString("sus"))
            _authorised = Utils.ToBoolean(Request.QueryString("auth"))
            _paid = Utils.ToBoolean(Request.QueryString("paid"))
            _providerId = Utils.ToInt32(Request.QueryString("estabid"))
            _contractId = Utils.ToInt32(Request.QueryString("contractid"))
            If Date.TryParse(Request.QueryString("dtfrom"), _dateFrom) Then
                dteFrom.Text = _dateFrom.ToShortDateString()
            Else
                _dateFrom = New DateTime
            End If
            If Date.TryParse(Request.QueryString("dtto"), _dateTo) Then
                dteTo.Text = _dateTo.ToShortDateString()
            Else
                _dateTo = New DateTime
            End If
        End Sub

#End Region

#Region " Set Filters"
        Private Sub SetFilters()
            chkPaid.Checked = _paid
            chkAuthorised.Checked = _authorised
            chkSuspended.Checked = _suspended
            chkUnpaid.Checked = _unPaid

        End Sub
#End Region

#Region " Reload controls from collapsable panel"

        Private Sub LoadDetails()
            chkPaid = cpe.FindControl("chkPaid")
            chkUnpaid = cpe.FindControl("chkUnpaid")
            chkAuthorised = cpe.FindControl("chkAuthorised")
            chkPaid = cpe.FindControl("chkPaid")
            dteFrom = cpe.FindControl("dteFrom")
            dteTo = cpe.FindControl("dteTo")
        End Sub

#End Region

        Private Sub SetReports()
            Dim user As WebSecurityUser
            user = SecurityBL.GetCurrentUser()

            With CType(rptPrint, Target.Library.Web.UserControls.IReportsButton)
                .Position = Target.Library.Web.Controls.SearchableMenu.SearchableMenuPosition.TopRight
                .Enabled = True
                .ButtonText = "Print"
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebReport.DomiciliaryProviderInvoices")
                If _pScheduleId > 0 Then .Parameters.Add("intPScheduleID", _pScheduleId)
                .Parameters.Add("blnPaid", _paid)
                .Parameters.Add("blnUnPaid", _unPaid)
                .Parameters.Add("blnSuspended", _suspended)
                .Parameters.Add("blnAuthorised", _authorised)
                .Parameters.Add("intUserId", user.ExternalUserID)
            End With
        End Sub


#Region " Payment Schedule Visit based "
        Public Function IsPaymentScheduleVisitBased(ByVal pScheduleId As Integer) As Boolean
            Dim pSchedule As Target.Abacus.Library.DataClasses.PaymentSchedule = _
            New Target.Abacus.Library.DataClasses.PaymentSchedule(Me.DbConnection, String.Empty, String.Empty)
            Dim msg As ErrorMessage = New ErrorMessage()

            msg = pSchedule.Fetch(pScheduleId)

            Return pSchedule.VisitBased

        End Function


#End Region

    End Class

End Namespace