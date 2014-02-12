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
    ''' Screen used to recreate interface files under a given domiciliary provider invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     09/02/2011  D11934 - Password Maintenance
    '''     MikeVO      11/08/2009  A4WA#5530 - creating InterfaceLog_Job record earlier.
    '''     MikeVO      18/06/2009  D11515 - added support for email notifications.
    '''     MikeVO      17/06/2009  A4WA#5529 - fix when re-read data is ticked.
    '''     MikeVO      07/04/2009  Fix to ensure default job step inputs are passed through.
    '''     JohnF       26/03/2009  Created (D11297A)
    ''' </history>
    Partial Class RecreateBatchFiles
        Inherits Target.Web.Apps.BasePage

        Private _OriginalXMLExists As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Const SP_NAME_FETCH_INVOICEBATCH As String = "spxDomProviderInvoiceBatch_FetchAll"

            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchid"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reader As SqlDataReader = Nothing
            Dim js As String
            Dim style As New StringBuilder
            Dim msg As ErrorMessage = Nothing
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))

            If currentPage <= 0 Then currentPage = 1

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryProviderInvoiceBatchesRecreate"), "Recreate Provider Invoice Batch Interface Files")

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Dom/ProviderInvoices/RecreateBatchFiles.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

            style.Append("label.label { float:left; width:14.5em; font-weight:bold; }")
            style.Append("span.label { float:left; width:14.5em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            chkRereadData.Label.Font.Bold = False
            optDoNotRollback.LabelAttributes.Add("style", "width:16.5em;")
            optFullRollback.LabelAttributes.Add("style", "width:16.5em;")
            optPartialRollback.LabelAttributes.Add("style", "width:16.5em;")
            optCreateNow.LabelAttributes.Add("style", "width:16.5em;")
            optDefer.LabelAttributes.Add("style", "width:16.5em;")
            With dteStartDate
                .Label.Style.Add("float", "left")
                .Label.ForeColor = Color.LightGray
                .TextBox.Style.Add("float", "left")
            End With

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_INVOICEBATCH, False)
                Dim strTemp As String
                spParams(0).Value = batchID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICEBATCH, spParams)

                lblInvoiceCount.Text = "0"
                lblInvoiceValueNet.Text = Convert.ToDecimal("0").ToString("c")
                lblInvoiceValueVAT.Text = Convert.ToDecimal("0").ToString("c")
                lblInvoiceValueGross.Text = Convert.ToDecimal("0").ToString("c")
                While reader.Read
                    lblCreatedDate.Text = Convert.ToDateTime(reader("CreatedDate")).ToString("dd MMM yyyy HH:mm:ss")
                    If reader("CreatedByFullName") <> "" Then
                        strTemp = String.Format("{0} ({1})", reader("CreatedBy"), reader("CreatedByFullName"))
                    Else
                        strTemp = reader("CreatedBy")
                    End If
                    lblCreatedBy.Text = strTemp
                    lblInvoiceCount.Text = Convert.ToString(reader("InvoiceCount"))
                    lblInvoiceValueNet.Text = Convert.ToDecimal(reader("InvoiceValueNet")).ToString("c")
                    lblInvoiceValueVAT.Text = Convert.ToDecimal(reader("InvoiceValueVAT")).ToString("c")
                    lblInvoiceValueGross.Text = Convert.ToDecimal(reader("InvoiceValueGross")).ToString("c")
                    If Utils.IsDate(reader("PostingDate")) Then
                        lblPostingDate.Text = Convert.ToDateTime(reader("PostingDate")).ToString("dd MMM yyyy")
                    Else
                        lblPostingDate.Text = "(not specified)"
                    End If
                    If reader("PostingYear") <> "0" Then
                        lblPostingYear.Text = reader("PostingYear")
                    Else
                        lblPostingYear.Text = "(not specified)"
                    End If
                    If reader("PeriodNumber") <> "0" Then
                        lblPeriodNum.Text = reader("PeriodNumber")
                    Else
                        lblPeriodNum.Text = "(not specified)"
                    End If

                    If Not IsDBNull(reader("OriginalXMLFileID")) Then
                        _OriginalXMLExists = True
                    Else
                        _OriginalXMLExists = False
                    End If

                End While
                reader.Close()
                reader = Nothing
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEBATCH, "RecreateBatchFiles.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            js = String.Format("batchid={0};", batchID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
        End Sub

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

            Const SCRIPT_STARTUP As String = "PreRenderStartup"

            Dim js As StringBuilder = New StringBuilder()

            js.AppendFormat("Recreate_dteStartDateID='{0}';", dteStartDate.ClientID)
            js.AppendFormat("Recreate_tmeStartDateID='{0}';", tmeStartDate.ClientID)
            js.AppendFormat("Recreate_dtePostingDateID='{0}';", dtePostingDate.ClientID)
            js.AppendFormat("Recreate_cboPostingYearID='{0}';", cboPostingYear.ClientID)
            js.AppendFormat("Recreate_cboPeriodNumID='{0}';", cboPeriodNum.ClientID)
            js.AppendFormat("Recreate_chkRereadDataID='{0}_chkCheckbox';", chkRereadData.ClientID)
            If Utils.IsDate(lblPostingDate.Text) Then
                js.AppendFormat("Recreate_postingDate='{0}';", Convert.ToDateTime(lblPostingDate.Text).ToShortDateString)
            Else
                js.Append("Recreate_postingDate='';")
            End If
            js.AppendFormat("Init();")

            If Not _OriginalXMLExists Then
                chkRereadData.CheckBox.Enabled = False
                chkRereadData.CheckBox.Checked = True
            End If

            Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, js.ToString(), True)
        End Sub

        Protected Sub CreationOptionClicked(ByVal sender As Object, ByVal e As System.EventArgs)
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

        Private Sub btnRecreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRecreate.Click
            
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser
            Dim jobID As Integer = 0
            Dim batchID As Integer = 0, savedBatchRef As String = ""
            Dim invoiceID As Integer = 0
            Dim providerID As Integer = 0, providerName As String = ""
            Dim contractID As Integer = 0, contractNum As String = ""
            Dim clientID As Integer = 0, clientName As String = ""
            Dim weFrom As String = "", weTo As String = ""
            Dim invoiceNumber As String = "", invoiceRef As String = ""
            Dim invNumFilter As String = "", invRefFilter As String = ""
            Dim statusDateFrom As String = "", statusDateTo As String = ""
            Dim startDateTime As String = ""
            Dim currQuery As String, asParams() As String, asValues() As String
            Dim lPos As Integer = 0, lCount As Integer = 0
            Dim bytRollback As Byte
            Dim a4wUser As Target.Abacus.Library.DataClasses.Users
            Dim recreateXML As Boolean

            Page_LoadComplete(sender, e)

            currentUser = SecurityBL.GetCurrentUser()
            a4wUser = New Target.Abacus.Library.DataClasses.Users(Me.DbConnection)
            msg = a4wUser.Fetch(currentUser.ExternalUserID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            Try
                currQuery = Me.ClientQueryString
                lPos = currQuery.IndexOf("&backUrl")
                If lPos >= 0 Then currQuery = Left(currQuery, lPos)
                asParams = currQuery.Split(New Char() {"&"})
                For lCount = LBound(asParams) To UBound(asParams)
                    asValues = asParams(lCount).Split(New Char() {"="}, 2)
                    Select Case asValues(0)
                        Case "batchid"
                            batchID = Utils.ToInt32(asValues(1))
                        Case Else
                    End Select
                Next lCount

                If optDefer.Checked Then
                    If dteStartDate.Text <> "" Then
                        startDateTime = String.Format("{0} {1}", dteStartDate.Text, tmeStartDate.Text)
                    End If
                End If
                If startDateTime = "" Then
                    '++ Default to today where not specified (and validation evaded)..
                    startDateTime = Format(DateTime.Now, "dd/MM/yyyy HH:mm:ss")
                End If

                If Not chkRereadData.CheckBox.Checked Then
                    '++ Get the batch ref for the known interface ID..
                    Dim interfaceLog As New InterfaceLog
                    interfaceLog.DbConnection = Me.DbConnection
                    msg = interfaceLog.Fetch(batchID)
                    If msg.Success Then
                        savedBatchRef = interfaceLog.BatchRef
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                End If

                If _OriginalXMLExists Then
                    recreateXML = chkRereadData.CheckBox.Checked
                Else
                    recreateXML = True
                End If


                '++ Create the job
                msg = DomCreditorsBL.CreateNewJob(Me.DbConnection, _
                                                    batchID, _
                                                    savedBatchRef, _
                                                    recreateXML, _
                                                    bytRollback, _
                                                    Convert.ToDateTime(startDateTime), _
                                                    currentUser.ExternalUsername, _
                                                    currentUser.ExternalUserID, _
                                                    a4wUser.EMail, _
                                                    jobID)
                If msg.Success Then
                    Response.Redirect(HttpUtility.UrlDecode(String.Format("ListBatch.aspx?currentPage=1&id={0}", batchID)), False)
                    Exit Sub
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