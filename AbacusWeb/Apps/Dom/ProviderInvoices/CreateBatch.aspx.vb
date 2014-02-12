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

Namespace Apps.Dom.ProviderInvoices

    ''' <summary>
    ''' Screen used to create a domiciliary provider invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     JohnF       15/02/2011  Pass rollback flag on batch creation (D11976)
    '''     MoTahir     02/02/2011  D11934 - Password Maintenance
    '''     MikeVO      11/08/2009  A4WA#5530 - creating InterfaceLog_Job record earlier.
    '''     MikeVO      30/07/2009  D11547 - replace rollback checkbox with radio buttons.
    '''     MikeVO      15/07/2009  A4WA#5594 - removed create interface file checkbox.
    '''     MikeVO      15/06/2009  D11515 - added support for email notifications.
    '''     MikeVO      07/04/2009  Fix to ensure default job step inputs are passed through.
    '''     JohnF       20/03/2009  Created (D11297A)
    ''' </history>
    Partial Class CreateBatch
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim user As WebSecurityUser
            Dim msg As ErrorMessage
            Dim js As String
            Dim style As New StringBuilder
            Dim filters As StringBuilder
            Dim stringTemp As String = ""
            Dim invoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("invID"))
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))
            Dim providerID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("providerID"))
            Dim providerName As String = Target.Library.Utils.ToString(Request.QueryString("providerName"))
            Dim contractID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("contractID"))
            Dim contractNum As String = Target.Library.Utils.ToString(Request.QueryString("contractNum"))
            Dim clientID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("clientID"))
            Dim clientName As String = Target.Library.Utils.ToString(Request.QueryString("clientName"))
            Dim weFrom As String = Target.Library.Utils.ToString(Request.QueryString("weFrom"))
            Dim weTo As String = Target.Library.Utils.ToString(Request.QueryString("weTo"))
            Dim invoiceNumber As String = Target.Library.Utils.ToString(Request.QueryString("invoiceNumber"))
            Dim invoiceRef As String = Target.Library.Utils.ToString(Request.QueryString("invoiceRef"))
            Dim invNumFilter As String = Target.Library.Utils.ToString(Request.QueryString("invoiceNumberFilter"))
            Dim invRefFilter As String = Target.Library.Utils.ToString(Request.QueryString("invoiceRefFilter"))
            Dim statusDateFrom As String = Target.Library.Utils.ToString(Request.QueryString("statusDateFrom"))
            Dim statusDateTo As String = Target.Library.Utils.ToString(Request.QueryString("statusDateTo"))
            Dim statusIsUnpaid As String = Target.Library.Utils.ToString(Request.QueryString("statusIsUnpaid"))
            Dim statusIsAuthorised As String = Target.Library.Utils.ToString(Request.QueryString("statusIsAuthorised"))
            Dim statusIsPaid As String = Target.Library.Utils.ToString(Request.QueryString("statusIsPaid"))
            Dim statusIsSuspended As String = Target.Library.Utils.ToString(Request.QueryString("statusIsSuspended"))
            Dim excluded As String = Target.Library.Utils.ToString(Request.QueryString("exclude"))
            Dim filterString As String = ""

            Const SP_FETCH_DPIS As String = "spxDomProviderInvoice_FetchListWithPaging"
            Const SP_FETCH_PROVIDER As String = "spxEstablishment_Fetch"
            Const SP_FETCH_DOMCONTRACT As String = "spxDomContract_Fetch"
            Const SP_FETCH_CLIENT As String = "spxClientDetail_Fetch"

            'Display the list of invoices.
            Try
                Me.EnableTimeout = False

                Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryProviderInvoiceBatchesCreate"), "Create Domiciliary Provider Invoice Batches")

                ' check user is logged in
                user = SecurityBL.GetCurrentUser()

                filters = New StringBuilder()

                If providerID = 0 Then
                    lblFilterProvider.Text = "(no filter applied)"
                Else
                    Dim spProvParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_PROVIDER, False)
                    spProvParams(0).Direction = ParameterDirection.Input
                    spProvParams(0).Value = providerID
                    Dim provDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_PROVIDER, spProvParams)
                    If provDataset.Tables.Count > 0 Then
                        Dim provDataTable As DataTable = provDataset.Tables(0)
                        lblFilterProvider.Text = String.Format("{0}/{1}", provDataTable.Rows(0)("AltReference"), provDataTable.Rows(0)("Name"))
                    Else
                        lblFilterProvider.Text = String.Format(">>WARNING: Provider ID {0} not found<<", providerID)
                    End If
                    spProvParams = Nothing
                    provDataset = Nothing
                End If

                If contractID = 0 Then
                    lblFilterContract.Text = "(no filter applied)"
                Else
                    Dim spContParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_DOMCONTRACT, False)
                    spContParams(0).Direction = ParameterDirection.Input
                    spContParams(0).Value = contractID
                    Dim contDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_DOMCONTRACT, spContParams)
                    If contDataset.Tables.Count > 0 Then
                        Dim contDataTable As DataTable = contDataset.Tables(0)
                        lblFilterContract.Text = String.Format("{0}/{1}", contDataTable.Rows(0)("Number"), contDataTable.Rows(0)("Title"))
                    Else
                        lblFilterContract.Text = String.Format(">>WARNING: Contract ID {0} not found<<", contractID)
                    End If
                    spContParams = Nothing
                    contDataset = Nothing
                End If

                If clientID = 0 Then
                    lblFilterClient.Text = "(no filter applied)"
                Else
                    Dim spSUParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_CLIENT, False)
                    spSUParams(0).Direction = ParameterDirection.Input
                    spSUParams(0).Value = clientID
                    Dim SUDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_CLIENT, spSUParams)
                    If SUDataset.Tables.Count > 0 Then
                        Dim SUDataTable As DataTable = SUDataset.Tables(0)
                        lblFilterClient.Text = String.Format("{0}/{1}", SUDataTable.Rows(0)("Reference"), SUDataTable.Rows(0)("Name"))
                    Else
                        lblFilterClient.Text = String.Format(">>WARNING: Client ID {0} not found<<", clientID)
                    End If
                    spSUParams = Nothing
                    SUDataset = Nothing
                End If

                If invoiceNumber = String.Empty Or invoiceNumber = "null" Then
                    lblFilterInvoiceNum.Text = "(no filter applied)"
                Else
                    If invoiceNumber.IndexOf("*") >= 0 Then
                        lblFilterInvoiceNum.Text = String.Format("Matching '{0}'", invoiceNumber.ToUpper)
                    Else
                        lblFilterInvoiceNum.Text = invoiceNumber.ToUpper
                    End If
                End If

                If invoiceRef = String.Empty Or invoiceRef = "null" Then
                    lblFilterInvoiceRef.Text = "(no filter applied)"
                Else
                    If invoiceRef.IndexOf("*") >= 0 Then
                        lblFilterInvoiceRef.Text = String.Format("Matching '{0}'", invoiceRef.ToUpper)
                    Else
                        lblFilterInvoiceRef.Text = invoiceRef.ToUpper
                    End If
                End If

                If Not IsValidDate(weFrom) And Not IsValidDate(weTo) Then
                    lblFilterWEDates.Text = "(no filter applied)"
                ElseIf IsValidDate(weFrom) And Not IsValidDate(weTo) Then
                    lblFilterWEDates.Text = String.Format("Invoices covering a period from {0}", Convert.ToDateTime(weFrom).ToString("dd MMM yyyy"))
                ElseIf Not IsValidDate(weFrom) And IsValidDate(weTo) Then
                    lblFilterWEDates.Text = String.Format("Invoices covering a period equal or prior to {0}", Convert.ToDateTime(weTo).ToString("dd MMM yyyy"))
                Else
                    lblFilterWEDates.Text = String.Format("Invoices covering a period between {0} and {1}", Convert.ToDateTime(weFrom).ToString("dd MMM yyyy"), Convert.ToDateTime(weTo).ToString("dd MMM yyyy"))
                End If

                stringTemp = ""
                'If statusIsUnpaid = "true" Then stringTemp &= IIf(stringTemp = "", "Unpaid", "; Unpaid")
                If statusIsAuthorised = "true" Then stringTemp &= IIf(stringTemp = "", "Authorised", "; Authorised")
                'If statusIsPaid = "true" Then stringTemp &= IIf(stringTemp = "", "Paid", "; Paid")
                'If statusIsSuspended = "true" Then stringTemp &= IIf(stringTemp = "", "Suspended", "; Suspended")
                If stringTemp = "" Then stringTemp = "(none selected)"
                lblFilterStatus.Text = stringTemp

                If Not IsValidDate(statusDateFrom) And Not IsValidDate(statusDateTo) Then
                    lblFilterStatusDates.Text = "(no filter applied)"
                ElseIf IsValidDate(statusDateFrom) And Not IsValidDate(statusDateTo) Then
                    lblFilterStatusDates.Text = String.Format("Invoices authorised on or since {0}", Convert.ToDateTime(statusDateFrom).ToString("dd MMM yyyy"))
                ElseIf Not IsValidDate(statusDateFrom) And IsValidDate(statusDateTo) Then
                    lblFilterStatusDates.Text = String.Format("Invoices authorised on or before {0}", Convert.ToDateTime(statusDateTo).ToString("dd MMM yyyy"))
                Else
                    lblFilterStatusDates.Text = String.Format("Invoices authorised between {0} and {1}", Convert.ToDateTime(statusDateFrom).ToString("dd MMM yyyy"), Convert.ToDateTime(statusDateTo).ToString("dd MMM yyyy"))
                End If

                If providerName <> String.Empty AndAlso providerName <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Provider = '{0}'", providerName.ToUpper)
                End If
                If contractNum <> String.Empty AndAlso contractNum <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Contract = '{0}'", contractNum.ToUpper)
                End If
                If clientName <> String.Empty AndAlso clientName <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Service User = '{0}'", clientName.ToUpper)
                End If
                If invNumFilter <> String.Empty AndAlso invNumFilter <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Invoice No. = '{0}'", invNumFilter.ToUpper)
                End If
                If invRefFilter <> String.Empty AndAlso invRefFilter <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Invoice Ref. = '{0}'", invRefFilter.ToUpper)
                End If

                If filterString <> "" Then
                    lblFilter.Text = "Filter(s):"
                    lblFilters.Text = filterString
                    lblFilter.Visible = True
                    lblFilters.Visible = True
                Else
                    lblFilter.Visible = False
                    lblFilters.Visible = False
                End If

                ' add date utility JS
                Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
                ' add utility JS link
                Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
                ' add dialog JS
                Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
                ' add page JS
                Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Dom/ProviderInvoices/CreateBatch.js"))
                ' add AJAX-generated javascript to the page
                AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

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
                With optFullRollback
                    .LabelAttributes.Add("style", "width:16.5em;")
                End With
                With optPartialRollback
                    .LabelAttributes.Add("style", "width:16.5em;")
                End With

                If weFrom = "null" OrElse Not Utils.IsDate(weFrom) Then
                    weFrom = "null"
                Else
                    weFrom = String.Format("{0}", Convert.ToDateTime(weFrom).ToShortDateString)
                End If

                If weTo = "null" OrElse Not Utils.IsDate(weTo) Then
                    weTo = "null"
                Else
                    weTo = String.Format("{0}", Convert.ToDateTime(weTo).ToShortDateString)
                End If

                If statusDateFrom = "null" OrElse Not Utils.IsDate(statusDateFrom) Then
                    statusDateFrom = "null"
                Else
                    statusDateFrom = String.Format("{0}", Convert.ToDateTime(statusDateFrom).ToShortDateString)
                End If

                If statusDateTo = "null" OrElse Not Utils.IsDate(statusDateTo) Then
                    statusDateTo = "null"
                Else
                    statusDateTo = String.Format("{0}", Convert.ToDateTime(statusDateTo).ToShortDateString)
                End If

                js = String.Format("currentPage={0};batchID={1};providerID={2};providerName={3};contractID={4};contractNum={5};" _
                        & "clientID={6};clientName={7};weFrom={8};weTo={9};invoiceNumber={10};invoiceRef={11};" _
                        & "invoiceNumberFilter={12};invoiceRefFilter={13};statusDateFrom={14};statusDateTo={15};" _
                        & "statusIsAuthorised={16};statusIsPaid={17};statusIsSuspended={18};statusIsUnpaid={19};exclude=false", _
                     1, _
                      IIf(batchID = 0, "0", String.Format("'{0}'", batchID)), _
                      IIf(providerID = 0, "0", String.Format("'{0}'", providerID)), _
                      IIf(providerName Is Nothing Or providerName = "null", "null", String.Format("'{0}'", providerName)), _
                      IIf(contractID = 0, "0", String.Format("'{0}'", contractID)), _
                      IIf(contractNum Is Nothing Or contractNum = "null", "null", String.Format("'{0}'", contractNum)), _
                      IIf(clientID = 0, "0", String.Format("'{0}'", clientID)), _
                      IIf(clientName Is Nothing Or clientName = "null", "null", String.Format("'{0}'", clientName)), _
                      weFrom, weTo, _
                      IIf(invoiceNumber Is Nothing Or invoiceNumber = "null", "null", String.Format("'{0}'", invoiceNumber)), _
                      IIf(invoiceRef Is Nothing Or invoiceRef = "null", "null", String.Format("'{0}'", invoiceRef)), _
                      IIf(invNumFilter Is Nothing Or invNumFilter = "null", "null", String.Format("'{0}'", invNumFilter)), _
                      IIf(invRefFilter Is Nothing Or invRefFilter = "null", "null", String.Format("'{0}'", invRefFilter)), _
                      statusDateFrom, statusDateTo, _
                      statusIsAuthorised, statusIsPaid, statusIsSuspended, statusIsUnpaid)

                Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_FETCH_DPIS, "CreateBatch.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
            End Try
        End Sub

        Protected Sub RadioButtonClicked(ByVal sender As Object, ByVal e As System.EventArgs)
            dteStartDate.Enabled = optDefer.Checked
            tmeStartDate.Enabled = optDefer.Checked
            If dteStartDate.Enabled Then
                dteStartDate.Text = Today.ToShortDateString
                dteStartDate.Label.ForeColor = Color.Black
                tmeStartDate.Hours = Now.Hour
                tmeStartDate.Minutes = Now.Minute
            Else
                dteStartDate.Text = ""
                dteStartDate.Label.ForeColor = Color.LightGray
                tmeStartDate.Hours = "0"
                tmeStartDate.Minutes = "0"
            End If
        End Sub

        Protected Function IsValidDate(ByRef psDate As String) As Boolean
            Dim lPos As Integer

            If psDate Is Nothing OrElse psDate.Trim = "" Then
                IsValidDate = False
            Else
                lPos = psDate.IndexOf("GMT")
                If lPos >= 0 Then psDate = psDate.Substring(0, lPos).Trim
                IsValidDate = Utils.IsDate(psDate)
            End If
        End Function

        Private Sub Page_LoadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete
            Dim lTemp As Integer

            If cboPostingYear.DropDownList.Items.Count = 0 Then
                cboPostingYear.DropDownList.Items.Add("")
                For lTemp = 2008 To 2030
                    cboPostingYear.DropDownList.Items.Add(lTemp.ToString())
                Next lTemp
            End If
            cboPostingYear.SelectPostBackValue()

            If cboPeriodNum.DropDownList.Items.Count = 0 Then
                cboPeriodNum.DropDownList.Items.Add("")
                For lTemp = 1 To 13
                    cboPeriodNum.DropDownList.Items.Add(lTemp.ToString)
                Next lTemp
            End If
            cboPeriodNum.SelectPostBackValue()
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Const SCRIPT_STARTUP As String = "Startup2"

            Dim js As StringBuilder = New StringBuilder()

            js.AppendFormat("CreateBatch_dteStartDateID='{0}';", dteStartDate.ClientID)
            js.AppendFormat("CreateBatch_tmeStartDateID='{0}';", tmeStartDate.ClientID)
            js.AppendFormat("CreateBatch_dtePostingDateID='{0}';", dtePostingDate.ClientID)
            js.AppendFormat("CreateBatch_cboPostingYearID='{0}';", cboPostingYear.ClientID)
            js.AppendFormat("CreateBatch_cboPeriodNumID='{0}';", cboPeriodNum.ClientID)
            js.AppendFormat("Init();")

            Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, js.ToString(), True)
        End Sub

        Private Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click

            Dim msg As ErrorMessage = Nothing
            Dim currentUser As WebSecurityUser
            Dim jobID As Integer = 0
            Dim interfaceID As Integer = 0, savedBatchRef As String = ""
            Dim startDateTime As String = ""

            Dim invoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("invID"))
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))
            Dim providerID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("providerID"))
            Dim providerName As String = Target.Library.Utils.ToString(Request.QueryString("providerName"))
            Dim contractID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("contractID"))
            Dim contractNum As String = Target.Library.Utils.ToString(Request.QueryString("contractNum"))
            Dim clientID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("clientID"))
            Dim clientName As String = Target.Library.Utils.ToString(Request.QueryString("clientName"))
            Dim weFrom As String = Target.Library.Utils.ToString(Request.QueryString("weFrom"))
            Dim weTo As String = Target.Library.Utils.ToString(Request.QueryString("weTo"))
            Dim invoiceNumber As String = Target.Library.Utils.ToString(Request.QueryString("invoiceNumber"))
            Dim invoiceRef As String = Target.Library.Utils.ToString(Request.QueryString("invoiceRef"))
            Dim invNumFilter As String = Target.Library.Utils.ToString(Request.QueryString("invoiceNumberFilter"))
            Dim invRefFilter As String = Target.Library.Utils.ToString(Request.QueryString("invoiceRefFilter"))
            Dim statusDateFrom As String = Target.Library.Utils.ToString(Request.QueryString("statusDateFrom"))
            Dim statusDateTo As String = Target.Library.Utils.ToString(Request.QueryString("statusDateTo"))

            Dim lPos As Integer = 0, lCount As Integer = 0
            Dim bytRollback As Byte
            Dim a4wUser As Target.Abacus.Library.DataClasses.Users

            Page_LoadComplete(sender, e)

            currentUser = SecurityBL.GetCurrentUser()
            a4wUser = New Target.Abacus.Library.DataClasses.Users(Me.DbConnection)
            msg = a4wUser.Fetch(currentUser.ExternalUserID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

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

                If Not Utils.IsDate(weFrom) Then weFrom = "null"
                If Not Utils.IsDate(weTo) Then weTo = "null"
                If Not Utils.IsDate(statusDateFrom) Then statusDateFrom = "null"
                If Not Utils.IsDate(statusDateTo) Then statusDateTo = "null"

                msg = DomContractBL.CreateDomCreditorsInterface( _
                     Me.DbConnection, providerID, providerName, contractID, contractNum, _
                     clientID, clientName, weFrom, weTo, invoiceNumber, invoiceRef, _
                     statusDateFrom, statusDateTo, invNumFilter, invRefFilter, _
                     SecurityBL.GetCurrentUser().ExternalUsername, "", startDateTime, _
                     Utils.ToString(dtePostingDate.Text), Utils.ToString(cboPeriodNum.Value), _
                     Utils.ToString(cboPostingYear.Value), _
                     interfaceID, savedBatchRef)

                If msg.Success Then
                    If optFullRollback.Checked Then
                        bytRollback = RollbackOption.FullRollback
                    ElseIf optPartialRollback.Checked Then
                        bytRollback = RollbackOption.PartialRollback
                    End If

                    '++ Create the job
                    msg = DomCreditorsBL.CreateNewJob(Me.DbConnection, _
                                                        interfaceID, _
                                                        savedBatchRef, _
                                                        True, _
                                                        bytRollback, _
                                                        Convert.ToDateTime(startDateTime), _
                                                        currentUser.ExternalUsername, _
                                                        currentUser.ExternalUserID, _
                                                        a4wUser.EMail, _
                                                        jobID)
                    If msg.Success Then
                        Response.Redirect(HttpUtility.UrlDecode(String.Format("ListBatch.aspx?currentPage=1&id={0}", interfaceID)), False)
                        Exit Sub
                    Else
                        WebUtils.DisplayError(msg)
                    End If
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