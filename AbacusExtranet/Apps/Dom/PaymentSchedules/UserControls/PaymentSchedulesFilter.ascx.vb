
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DebtorInvoices
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports System.Text

Namespace Apps.Dom.PaymentSchedules.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.UserControls.PaymentSchedulesFilter
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the display of criteria for payment schedules' filtering.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     IHS       24/05/2011  D12084 - Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class PaymentSchedulesFilter
        Inherits System.Web.UI.UserControl

        Private _thePage As BasePage
        Public Property thePage() As BasePage
            Get
                Return _thePage
            End Get
            Set(ByVal value As BasePage)
                _thePage = value
            End Set
        End Property


        Private _qsParser As WizardScreenParameters
        Public Property qsParser() As WizardScreenParameters
            Get
                Return _qsParser
            End Get
            Set(ByVal value As WizardScreenParameters)
                _qsParser = value
            End Set
        End Property

        Private Sub InitControl()

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("currentStep"))

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/PaymentSchedules/UserControls/PaymentSchedulesFilter.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(WebSvc.DomContract))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                "Apps.Dom.PaymentSchedules.UserControls.PaymentSchedulesFilter.Startup", Me.InitialiseJS(), True)

            ' prime each field based on the setting in the query string (if any)
            Me.PrimeFields(qsParser)

        End Sub

        Private Function InitialiseJS() As String

            Dim js As New StringBuilder

            ' Header Details
            js.AppendFormat("SelectorWizard_ReferenceID=""{0}"";", txtReference.ClientID)
            js.AppendFormat("SelectorWizard_PeriodFromID=""{0}"";", dtePeriodFrom.ClientID)
            js.AppendFormat("SelectorWizard_PeriodToID=""{0}"";", dtePeriodTo.ClientID)
            js.AppendFormat("SelectorWizard_VisitBasedYesID=""{0}"";", chkVisitBasedYes.ClientID)
            js.AppendFormat("SelectorWizard_VisitBasedNoID=""{0}"";", chkVisitBasedNo.ClientID)

            ' Unprocessed Pro forma Invoices
            js.AppendFormat("SelectorWizard_ProformaInvoicesNoneID=""{0}"";", chkProformaInvoicesNone.ClientID)
            js.AppendFormat("SelectorWizard_ProformaInvoicesAwaitingID=""{0}"";", chkProformaInvoicesAwaiting.ClientID)
            js.AppendFormat("SelectorWizard_ProformaInvoicesVerifiedID=""{0}"";", chkProformaInvoicesVerified.ClientID)

            ' Provider Invoices
            js.AppendFormat("SelectorWizard_InvoicesUnpaidID=""{0}"";", chkInvoicesUnpaid.ClientID)
            js.AppendFormat("SelectorWizard_InvoicesSuspendedID=""{0}"";", chkInvoicesSuspended.ClientID)
            js.AppendFormat("SelectorWizard_InvoicesAuthorisedID=""{0}"";", chkInvoicesAuthorised.ClientID)
            js.AppendFormat("SelectorWizard_InvoicesPaidID=""{0}"";", chkInvoicesPaid.ClientID)

            ' Unprocessed Visit Amendment Requests
            js.AppendFormat("SelectorWizard_VARawaitingID=""{0}"";", chkVARawaiting.ClientID)
            js.AppendFormat("SelectorWizard_VARverifiedID=""{0}"";", chkVARverified.ClientID)
            js.AppendFormat("SelectorWizard_VARdeclinedID=""{0}"";", chkVARdeclined.ClientID)

            Return js.ToString()

        End Function

        Private Sub PrimeFields(ByVal qsParser As WizardScreenParameters)

            '++ Prime each field based on the setting in the query string (if any)..

            txtReference.Text = qsParser.Reference

            If qsParser.PeriodFrom <> "null" AndAlso Convert.ToDateTime(qsParser.PeriodFrom) <> _
                                        Convert.ToDateTime(WizardScreenParameters.NULL_DATE_FROM) Then
                dtePeriodFrom.Text = Convert.ToDateTime(qsParser.PeriodFrom).ToShortDateString
            Else
                dtePeriodFrom.Text = Date.Now.AddYears(-1).ToShortDateString()
            End If

            If qsParser.PeriodTo <> "null" AndAlso Convert.ToDateTime(qsParser.PeriodTo) <> _
                                        Convert.ToDateTime(WizardScreenParameters.NULL_DATE_TO) Then
                dtePeriodTo.Text = Convert.ToDateTime(qsParser.PeriodTo).ToShortDateString
            Else
                dtePeriodTo.Text = ""
            End If

            chkVisitBasedYes.CheckBox.Checked = (qsParser.VisitYes = "true")
            chkVisitBasedNo.CheckBox.Checked = (qsParser.VisitNo = "true")

            chkProformaInvoicesNone.CheckBox.Checked = (qsParser.ProformaInvoicesNone = "true")
            chkProformaInvoicesAwaiting.CheckBox.Checked = (qsParser.ProformaInvoicesAwaitingVerification = "true")
            chkProformaInvoicesVerified.CheckBox.Checked = (qsParser.ProformaInvoicesVerified = "true")

            chkInvoicesUnpaid.CheckBox.Checked = (qsParser.InvoicesUnpaid = "true")
            chkInvoicesSuspended.CheckBox.Checked = (qsParser.InvoicesSuspended = "true")
            chkInvoicesAuthorised.CheckBox.Checked = (qsParser.InvoicesAuthorised = "true")
            chkInvoicesPaid.CheckBox.Checked = (qsParser.InvoicesPaid = "true")

            chkVARawaiting.CheckBox.Checked = (qsParser.VAR_Awaiting = "true")
            chkVARverified.CheckBox.Checked = (qsParser.VAR_Verified = "true")
            chkVARdeclined.CheckBox.Checked = (qsParser.VAR_Declined = "true")

        End Sub

        Private Sub InitialiseControls()
            chkVisitBasedYes.CheckBox.TextAlign = TextAlign.Right
            chkVisitBasedNo.CheckBox.TextAlign = TextAlign.Right

            chkProformaInvoicesNone.CheckBox.TextAlign = TextAlign.Right
            chkProformaInvoicesAwaiting.CheckBox.TextAlign = TextAlign.Right
            chkProformaInvoicesVerified.CheckBox.TextAlign = TextAlign.Right

            chkInvoicesUnpaid.CheckBox.TextAlign = TextAlign.Right
            chkInvoicesSuspended.CheckBox.TextAlign = TextAlign.Right
            chkInvoicesAuthorised.CheckBox.TextAlign = TextAlign.Right
            chkInvoicesPaid.CheckBox.TextAlign = TextAlign.Right

            chkVARawaiting.CheckBox.TextAlign = TextAlign.Right
            chkVARverified.CheckBox.TextAlign = TextAlign.Right
            chkVARdeclined.CheckBox.TextAlign = TextAlign.Right
        End Sub

        Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
            InitControl()
        End Sub


        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Me.InitialiseControls()

        End Sub



    End Class

End Namespace

