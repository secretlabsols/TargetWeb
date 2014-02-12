Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.DebtorInvoices
Imports Target.Abacus.Jobs.Exports.FinancialExportInterface.Debtors
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Jobs.Core
Imports System.Collections.Generic

Namespace Apps.General.DebtorInvoices

    ''' <summary>
    ''' Screen used to create a domiciliary provider invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     02/09/2011  D11934 - Password Maintenance
    '''     MikeVO      29/07/2009  D11547 - replace rollback checkbox with radio buttons.
    '''     MikeVO      29/07/2009  A4WA#5531 - creating InterfaceLog_Job record earlier.
    '''     MikeVO      22/07/2009  D11651 - consolidation of wizard screen filter parsing.
    '''     MikeVO      15/07/2009  A4WA#5594 - removed create interface file checkbox.
    '''     MikeVO      15/06/2009  D11515 - added support for email notifications.
    '''     MikeVO      07/04/2009  Fix to ensure default job step inputs are passed through.
    '''     JohnF       20/03/2009  Created (D11297A)
    ''' </history>
    Partial Class CreateBatch
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SP_FETCH_INVOICES As String = "spxDebtorInvoice_FetchListWithPaging"
            Const SP_FETCH_CLIENT As String = "spxClientDetail_Fetch"
            Const SP_GET_FIRST_INTERFACE_ID As String = "spxDebtorsInterface_FetchFirstID"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DebtorInvoiceBatchesCreate"), "Create Debtor Invoice Batches")

            Dim user As WebSecurityUser
            Dim msg As ErrorMessage
            Dim js As String
            Dim style As New StringBuilder
            Dim filters As StringBuilder
            Dim filterString As String = ""
            Dim qsParser As WizardScreenParameters = New WizardScreenParameters(Request.QueryString)

            'Display the list of invoices.
            Try
                user = SecurityBL.GetCurrentUser()

                filters = New StringBuilder()

                If qsParser.ClientID = 0 Then
                    lblFilterClient.Text = "(all clients)"
                Else
                    Dim spSUParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_CLIENT, False)
                    spSUParams(0).Direction = ParameterDirection.Input
                    spSUParams(0).Value = qsParser.ClientID
                    Dim SUDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_CLIENT, spSUParams)
                    If SUDataset.Tables.Count > 0 Then
                        Dim SUDataTable As DataTable = SUDataset.Tables(0)
                        lblFilterClient.Text = String.Format("{0}/{1}", SUDataTable.Rows(0)("Reference"), SUDataTable.Rows(0)("Name"))
                    Else
                        lblFilterClient.Text = String.Format(">>WARNING: Client ID {0} not found<<", qsParser.ClientID)
                    End If
                    spSUParams = Nothing
                    SUDataset = Nothing
                End If

                lblFilterInvTypes.Text = qsParser.SelectedInvoiceTypesDesc
                lblFilterCreationDates.Text = qsParser.SelectedCreationDatesDesc

                Dim sysInfo As SystemInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
                If sysInfo.XMLDebtorsCreateOpeningBalance = True AndAlso Not qsParser.OpeningBalanceRun Then
                    Dim spOBParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_GET_FIRST_INTERFACE_ID, False)
                    Dim OBDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_GET_FIRST_INTERFACE_ID, spOBParams)
                    If OBDataset.Tables.Count > 0 Then
                        Dim OBDataTable As DataTable = OBDataset.Tables(0)
                        Dim OBDataRow As DataRow = OBDataTable.Rows(0)
                        If OBDataRow("ID") = 0 Then
                            '++ The first run, so knobble the relevant parameters of the URL so that
                            '++ all unbatched, non-provisional, non-excluded invoices are batched..
                            qsParser = New WizardScreenParameters()
                            qsParser.InitialiseForOpeningBalanaceRun()
                            Response.Redirect(qsParser.BuildUrl(Request.Url), False)
                            Exit Sub
                        End If
                    End If
                End If

                lblFilterOther.Text = qsParser.SelectedOtherSettingsDesc

                filterString = String.Empty
                If qsParser.FilterDebtor <> String.Empty And qsParser.FilterDebtor <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Debtor = '{0}'", qsParser.FilterDebtor.ToUpper)
                End If
                If qsParser.FilterInvoiceNumber <> String.Empty And qsParser.FilterInvoiceNumber <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Invoice Num. = '{0}'", qsParser.FilterInvoiceNumber.ToUpper)
                End If
                If filterString <> String.Empty Then
                    lblFilter.Text = "Filter(s):"
                    lblFilters.Text = filterString
                    lblFilter.Visible = True
                    lblFilters.Visible = True
                Else
                    lblFilter.Visible = False
                    lblFilters.Visible = False
                End If

                If qsParser.OpeningBalanceRun Then
                    lblOpeningBalance.Text = "** OPENING BALANCE RUN **"
                Else
                    lblOpeningBalance.Text = ""
                End If

                ' add date utility JS
                Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
                ' add utility JS link
                Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
                ' add dialog JS
                Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
                ' add page JS
                Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/General/DebtorInvoices/CreateBatch.js"))
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

                js = String.Format( _
                    "clientID={0};invRes={1};invDom={2};invLD={3};" _
                        & "invClient={4};invTP={5};invProp={6};invOLA={7};invPenColl={8};invHomeColl={9};" _
                        & "invStd={10};invManual={11};invSDS={12};invActual={13};invProvisional={14};" _
                        & "invRetracted={15};invViaRetract={16};invDateFrom='{17}';invDateTo='{18}';invBatchSel={19};" _
                        & "invExclude={20};filterDebtor='{21}';filterInvNum='{22}';OBR='{23}';backURL='{24}'", _
                    qsParser.ClientID, _
                    qsParser.InvoiceRes.ToLower, qsParser.InvoiceDom.ToLower, qsParser.InvoiceLD.ToLower, qsParser.InvoiceClient.ToLower, qsParser.InvoiceTP.ToLower, qsParser.InvoiceProp.ToLower, _
                    qsParser.InvoiceOLA.ToLower, qsParser.InvoicePenColl.ToLower, qsParser.InvoiceHomeColl.ToLower, qsParser.InvoiceStd.ToLower, qsParser.InvoiceMan.ToLower, qsParser.InvoiceSDS.ToLower, _
                    qsParser.InvoiceActual.ToLower, qsParser.InvoiceProvisional.ToLower, qsParser.InvoiceRetracted.ToLower, qsParser.InvoiceViaRetract.ToLower, _
                    qsParser.InvoiceDateFrom, qsParser.InvoiceDateTo, qsParser.InvoiceBatchSel, _
                    qsParser.InvoiceExclude.ToLower, _
                    IIf(qsParser.FilterDebtor <> String.Empty, qsParser.FilterDebtor, "null"), _
                    IIf(qsParser.FilterInvoiceNumber <> String.Empty, qsParser.FilterInvoiceNumber, "null"), _
                    qsParser.OpeningBalanceRun, Request.QueryString("backurl"))

                Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_FETCH_INVOICES, "CreateBatch.Page_Load")   ' could not read data
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
            Dim invoiceID As String = "0"
            Dim qsParser As WizardScreenParameters = New WizardScreenParameters(Request.QueryString)

            ' override certain filters
            qsParser.OverrideForBatchCreation()

            Dim startDateTime As String = ""
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

                'msg = DebtorInvoiceBL.CreateDebtorsInterface( _
                '    Me.DbConnection, invoiceID, qsParser, _
                '    SecurityBL.GetCurrentUser().ExternalUsername, "", startDateTime, _
                '    Utils.ToString(dtePostingDate.Text), Utils.ToString(cboPeriodNum.Value), _
                '    Utils.ToString(cboPostingYear.Value), _
                '    interfaceID, savedBatchRef)

                If msg.Success Then
                    '++ No error, but has there been an interface/batch header created?
                    '++ (Possibly not, since there may not be any new invoice/payment/client data available)..
                    If msg.Message = "" Then
                        If optFullRollback.Checked Then
                            bytRollback = RollbackOption.FullRollback
                        ElseIf optPartialRollback.Checked Then
                            bytRollback = RollbackOption.PartialRollback
                        End If

                        '++ Create the job
                        msg = DebtorInvoicesBL.CreateNewJob(Me.DbConnection, _
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
                        '++ No batch or job created, so show the current batch list..
                        Response.Redirect(HttpUtility.UrlDecode("ListBatch.aspx?currentPage=1&id=-1"), False)
                        Exit Sub
                    End If
                Else
                    '++ Proper error found, so display as such..
                    WebUtils.DisplayError(msg)
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
                WebUtils.DisplayError(msg)
            End Try
        End Sub
    End Class
End Namespace