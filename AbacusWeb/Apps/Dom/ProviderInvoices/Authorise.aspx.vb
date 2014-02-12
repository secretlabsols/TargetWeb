Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Jobs.Exports.FinancialExportInterface.DomCreditors
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Jobs.Core
Imports System.Collections.Generic
Imports Target.Abacus.Library.Core

Namespace Apps.Dom.ProviderInvoices

    ''' <summary>
    ''' Screen used to create a Authorise Dom Provider Invoice Job.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD      14/04/2010  A4WA#6215 - Client ID not found issue fixed
    '''     PaulW       18/06/2009  Created (D11551)
    ''' </history>
    Partial Class Authorise
        Inherits Target.Web.Apps.BasePage

        Private _providerID As Integer
        Private _providerName As String
        Private _contractID As Integer
        Private _contractNum As String
        Private _clientID As Integer
        Private _clientName As String
        Private _weFrom As String
        Private _weTo As String
        Private _invoiceNumber As String
        Private _invoiceRef As String
        Private _invNumFilter As String
        Private _invRefFilter As String
        Private _statusDateFrom As String
        Private _statusDateTo As String
        Private _statusIsUnpaid As String
        Private _statusIsAuthorised As String
        Private _statusIsPaid As String
        Private _statusIsSuspended As String

        Const PRINT_PAGE_SIZE As Integer = 1000000
        Const SP_FETCH_DPIS As String = "spxDomProviderInvoice_FetchListWithPaging"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim user As WebSecurityUser
            Dim msg As ErrorMessage
            Dim js As String
            Dim style As New StringBuilder
            Dim filters As StringBuilder
            Dim stringTemp As String = ""
            Dim filterString As String = ""

            'Display the list of invoices.
            _providerID = Target.Library.Utils.ToInt32(Request.QueryString("providerID"))
            _contractID = Target.Library.Utils.ToInt32(Request.QueryString("contractID"))
            _clientID = Target.Library.Utils.ToInt32(Request.QueryString("clientID"))
            If Utils.IsDate(Request.QueryString("weFrom")) Then _weFrom = Request.QueryString("weFrom")
            If Utils.IsDate(Request.QueryString("weTo")) Then _weTo = Request.QueryString("weTo")
            _invoiceNumber = Request.QueryString("invoiceNumber")
            _invoiceRef = Request.QueryString("invoiceRef")
            _invNumFilter = Request.QueryString("invoiceNumberFilter")
            _invRefFilter = Request.QueryString("invoiceRefFilter")
            If Utils.IsDate(Request.QueryString("statusDateFrom")) Then _statusDateFrom = Request.QueryString("statusDateFrom")
            If Utils.IsDate(Request.QueryString("statusDateTo")) Then _statusDateTo = Request.QueryString("statusDateTo")
            _statusIsUnpaid = Request.QueryString("statusIsUnpaid")
            _statusIsAuthorised = Request.QueryString("statusIsAuthorised")
            _statusIsPaid = Request.QueryString("statusIsPaid")
            _statusIsSuspended = Request.QueryString("statusIsSuspended")

            Me.EnableTimeout = False

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.AuthoriseDomiciliaryProviderInvoices"), "Authorise Domiciliary Provider Invoices")

            ' check user is logged in
            user = SecurityBL.GetCurrentUser()

            filters = New StringBuilder()

            'Get Provider Details
            If _providerID = 0 Then
                lblFilterProvider.Text = "(no filter applied)"
            Else
                Dim provider As Establishment = New Establishment(Me.DbConnection)
                msg = provider.Fetch(_providerID)
                If msg.Success Then
                    lblFilterProvider.Text = String.Format("{0}/{1}", provider.AltReference, provider.Name)
                Else
                    lblFilterProvider.Text = String.Format(">>WARNING: Provider ID {0} not found<<", _providerID)
                End If
            End If
            _providerName = lblFilterProvider.Text

            'Get Contract details
            If _contractID = 0 Then
                lblFilterContract.Text = "(no filter applied)"
            Else
                Dim contract As DomContract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                msg = contract.Fetch(_contractID)
                If msg.Success Then
                    lblFilterContract.Text = String.Format("{0}/{1}", contract.Number, contract.Title)
                Else
                    lblFilterContract.Text = String.Format(">>WARNING: Contract ID {0} not found<<", _contractID)
                End If
            End If
            _contractNum = lblFilterContract.Text
            'Get Service User Details
            If _clientID = 0 Then
                lblFilterClient.Text = "(no filter applied)"
            Else
                Dim client As ClientDetail = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
                msg = client.Fetch(_clientID)
                If msg.Success Then
                    lblFilterClient.Text = String.Format("{0}/{1}", client.Reference, client.Name)
                Else
                    lblFilterClient.Text = String.Format(">>WARNING: Client ID {0} not found<<", _clientID)
                End If
            End If
            _clientName = lblFilterClient.Text
            'Invoice number filter
            If _invoiceNumber = String.Empty Or _invoiceNumber = "null" Then
                lblFilterInvoiceNum.Text = "(no filter applied)"
            Else
                If _invoiceNumber.IndexOf("*") >= 0 Then
                    lblFilterInvoiceNum.Text = String.Format("Matching '{0}'", _invoiceNumber.ToUpper)
                Else
                    lblFilterInvoiceNum.Text = _invoiceNumber.ToUpper
                End If
            End If
            'Invoice Reference Filter
            If _invoiceRef = String.Empty Or _invoiceRef = "null" Then
                lblFilterInvoiceRef.Text = "(no filter applied)"
            Else
                If _invoiceRef.IndexOf("*") >= 0 Then
                    lblFilterInvoiceRef.Text = String.Format("Matching '{0}'", _invoiceRef.ToUpper)
                Else
                    lblFilterInvoiceRef.Text = _invoiceRef.ToUpper
                End If
            End If

            'W/E Filter Dates
            If Not Utils.IsDate(_weFrom) And Not Utils.IsDate(_weTo) Then
                lblFilterWEDates.Text = "(no filter applied)"
            ElseIf Utils.IsDate(_weFrom) And Not Utils.IsDate(_weTo) Then
                lblFilterWEDates.Text = String.Format("Invoices covering a period from {0}", Convert.ToDateTime(_weFrom).ToString("dd MMM yyyy"))
            ElseIf Not Utils.IsDate(_weFrom) And Utils.IsDate(_weTo) Then
                lblFilterWEDates.Text = String.Format("Invoices covering a period equal or prior to {0}", Convert.ToDateTime(_weTo).ToString("dd MMM yyyy"))
            Else
                lblFilterWEDates.Text = String.Format("Invoices covering a period between {0} and {1}", Convert.ToDateTime(_weFrom).ToString("dd MMM yyyy"), Convert.ToDateTime(_weTo).ToString("dd MMM yyyy"))
            End If

            stringTemp = ""
            'We only want to look at unpaid invoices
            If _statusIsUnpaid = "true" Then stringTemp &= IIf(stringTemp = "", "Unpaid", "; Unpaid")
            If stringTemp = "" Then stringTemp = "(none selected)"
            lblFilterStatus.Text = stringTemp

            'Status Date Filters
            If Not Utils.IsDate(_statusDateFrom) And Not Utils.IsDate(_statusDateTo) Then
                lblFilterStatusDates.Text = "(no filter applied)"
            ElseIf Utils.IsDate(_statusDateFrom) And Not Utils.IsDate(_statusDateTo) Then
                lblFilterStatusDates.Text = String.Format("Invoices authorised on or since {0}", Convert.ToDateTime(_statusDateFrom).ToString("dd MMM yyyy"))
            ElseIf Not Utils.IsDate(_statusDateFrom) And Utils.IsDate(_statusDateTo) Then
                lblFilterStatusDates.Text = String.Format("Invoices authorised on or before {0}", Convert.ToDateTime(_statusDateTo).ToString("dd MMM yyyy"))
            Else
                lblFilterStatusDates.Text = String.Format("Invoices authorised between {0} and {1}", Convert.ToDateTime(_statusDateFrom).ToString("dd MMM yyyy"), Convert.ToDateTime(_statusDateTo).ToString("dd MMM yyyy"))
            End If

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add page JS
            Me.JsLinks.Add("Authorise.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

            js = String.Format("CreateBatch_dteStartDateID='{0}';CreateBatch_tmeStartDateID='{1}';", dteStartDate.ClientID, tmeStartDate.ClientID)
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Authorise.Startup", Target.Library.Web.Utils.WrapClientScript(js))


            With optCreateNow
                .LabelAttributes.Add("style", "width:16.5em;")
            End With
            With optDefer
                .LabelAttributes.Add("style", "width:16.5em;")
            End With
            With dteStartDate
                .Label.Style.Add("float", "left")
                .Label.ForeColor = Color.LightGray
                .TextBox.Style.Add("float", "left")
            End With

        End Sub

        Private Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click
            Const PROVIDER_NAME As String = "ProviderName"
            Const PROVIDER_ID As String = "ProviderID"
            Const CONTRACT_NO_TITLE As String = "ContractNoTitle"
            Const CONTRACT_ID As String = "ContractID"
            Const SERVICE_USER_NAME As String = "ServiceUserName"
            Const CLIENT_ID As String = "ClientID"
            Const INVOICE_NUMBER As String = "InvoiceNumber"
            Const INVOICE_REFERENCE As String = "InvoiceReference"
            Const WEEK_ENDING_DATE_FROM As String = "WEDateFrom"
            Const WEEK_ENDING_DATE_TO As String = "WEDateTo"
            Const INVOICE_STATUS As String = "InvoiceStatus"
            Const INVOICE_STATUS_ID As String = "InvoiceStatusID"
            Const INVOICE_STATUS_DATE_FROM As String = "StatusDateFrom"
            Const INVOICE_STATUS_DATE_TO As String = "StatusDateTo"

            Dim msg As ErrorMessage = Nothing
            Dim currentUser As WebSecurityUser
            Dim input As vwJobStepInputs = Nothing
            Dim inputs As vwJobStepInputsCollection = Nothing
            Dim newInputs As List(Of Triplet) = Nothing
            Dim jobID As Integer = 0
            Dim invoiceID As Integer = 0, batchID As Integer = 0
            Dim providerID As Integer = 0, providerName As String = ""
            Dim contractID As Integer = 0, contractNum As String = ""
            Dim clientID As Integer = 0, clientName As String = ""
            Dim weFrom As String = "", weTo As String = ""
            Dim invoiceNumber As String = "", invoiceRef As String = ""
            Dim invNumFilter As String = "", invRefFilter As String = ""
            Dim statusDateFrom As String = "", statusDateTo As String = ""
            Dim startDateTime As String = ""
            Dim lPos As Integer = 0, lCount As Integer = 0

            currentUser = SecurityBL.GetCurrentUser()

            Try


                If optDefer.Checked Then
                    If dteStartDate.Text <> "" Then
                        startDateTime = String.Format("{0} {1}", dteStartDate.Text, tmeStartDate.Text)
                    End If
                End If
                If startDateTime = "" Then
                    '++ Default to today where not specified (and validation evaded)..
                    startDateTime = Format(DateTime.Now, "dd/MM/yyyy HH:mm:ss")
                End If

                '++ Get the list of inputs for the associated job types..
                msg = JobServiceBL.GetNewJobInputs(Me.DbConnection, JobTypes.AuthoriseDomProviderInvoice, inputs)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If Not Utils.IsDate(_weFrom) Or Convert.ToDateTime(_weFrom) = Convert.ToDateTime(Date.MinValue) Or _weFrom = "00:00:00" Then
                    _weFrom = ""
                End If
                If Not Utils.IsDate(_weTo) Or Convert.ToDateTime(_weTo) = Convert.ToDateTime(Date.MinValue) Or _weTo = "00:00:00" Then
                    _weTo = ""
                End If
                If Not Utils.IsDate(_statusDateFrom) Or Convert.ToDateTime(_statusDateFrom) = Convert.ToDateTime(Date.MinValue) Or _statusDateFrom = "00:00:00" Then
                    _statusDateFrom = ""
                End If
                If Not Utils.IsDate(_statusDateTo) Or Convert.ToDateTime(_statusDateTo) = Convert.ToDateTime(Date.MinValue) Or _statusDateTo = "00:00:00" Then
                    _statusDateTo = ""
                End If

                '++ Build the inputs chain..
                newInputs = New List(Of Triplet)
                For Each input In inputs
                    Select Case input.JobTypeID
                        Case JobTypes.AuthoriseDomProviderInvoice
                            Select Case input.JobStepTypeID
                                Case JobStepTypes.AuthoriseDomProviderInvoice
                                    Select Case input.InputName
                                        Case PROVIDER_NAME
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _providerName))
                                        Case PROVIDER_ID
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _providerID))
                                        Case CONTRACT_NO_TITLE
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _contractNum))
                                        Case CONTRACT_ID
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _contractID))
                                        Case SERVICE_USER_NAME
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _clientName))
                                        Case CLIENT_ID
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _clientID))
                                        Case INVOICE_NUMBER
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _invoiceNumber))
                                        Case INVOICE_REFERENCE
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _invoiceRef))
                                        Case WEEK_ENDING_DATE_FROM
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _weFrom))
                                        Case WEEK_ENDING_DATE_TO
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _weTo))
                                        Case INVOICE_STATUS
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, [Enum].GetName(GetType(DomProviderInvoiceStatus), DomProviderInvoiceStatus.Unpaid)))
                                        Case INVOICE_STATUS_ID
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, DomProviderInvoiceStatus.Unpaid))
                                        Case INVOICE_STATUS_DATE_FROM
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _statusDateFrom))
                                        Case INVOICE_STATUS_DATE_TO
                                            newInputs.Add(New Triplet(input.JobStepTypeID, input.InputName, _statusDateTo))
                                    End Select
                            End Select
                        Case Else
                    End Select
                Next

                '++ Create the job with the new inputs..
                msg = JobServiceBL.CreateNewJob(Me.DbConnection, JobTypes.AuthoriseDomProviderInvoice, Nothing, _
                        currentUser.ExternalUsername, Convert.ToDateTime(startDateTime), _
                        Nothing, currentUser.ExternalUserID, currentUser.Email, newInputs, jobID)
                If msg.Success Then
                    Response.Redirect("~/AbacusWeb/Apps/Jobs/Default.aspx")
                Else
                    WebUtils.DisplayError(msg)
                End If


            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
                WebUtils.DisplayError(msg)
            End Try
        End Sub
    End Class
End Namespace