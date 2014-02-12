Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
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
    ''' Screen used to recreate interface files under a given debtor invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     09/02/2011  D11934 - Password Maintenance
    '''     JohnF       18/02/2010  Customer File checkbox and message fields added (D11759)
    '''     MikeVO      29/07/2009  D11547 - remove rollback options.
    '''     MikeVO      29/07/2009  A4WA#5531 - creating InterfaceLog_Job record earlier.
    '''     MikeVO      18/06/2009  D11515 - added support for email notifications.
    '''     MikeVO      17/06/2009  A4WA#5534 - fix when re-read data is ticked.
    '''     JohnF       12/05/2009  Created (D11605)
    ''' </history>
    Partial Class RecreateBatchFiles
        Inherits Target.Web.Apps.BasePage

#Region " Private variables "
        Private _interfaceID As Integer
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Const SP_NAME_FETCH_INVOICEBATCH As String = "spxDebtorInvoiceBatch_FetchAll"

            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchid"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reader As SqlDataReader = Nothing
            Dim js As String
            Dim style As New StringBuilder
            Dim msg As ErrorMessage = Nothing
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))

            If currentPage <= 0 Then currentPage = 1

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DebtorInvoiceBatchesRecreate"), "Debtor Invoice Batches - Recreate Files")

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/General/DebtorInvoices/RecreateBatchFiles.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

            style.Append("label.label { float:left; width:14.5em; font-weight:bold; }")
            style.Append("span.label { float:left; width:14.5em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            chkRereadData.LabelAttributes.Add("style", "width:14.75em;")
            With lblRereadData
                .ForeColor = Color.Orange
                .Font.Bold = True
            End With
            chkCustomerFile.LabelAttributes.Add("style", "width:14.75em;")
            With lblCustomerFile
                .ForeColor = Color.Orange
                .Font.Bold = True
            End With
            optCreateNow.LabelAttributes.Add("style", "width:14.63em;")
            optDefer.LabelAttributes.Add("style", "width:14.63em;")
            With dteStartDate
                .Label.Style.Add("float", "left")
                .Label.ForeColor = Color.LightGray
                .TextBox.Style.Add("float", "left")
            End With

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_INVOICEBATCH, False)
                Dim strTemp As String
                spParams(0).Value = batchID
                _interfaceID = batchID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICEBATCH, spParams)

                lblInvoiceCount.Text = "0"
                lblInvoiceValue.Text = Convert.ToDecimal("0").ToString("c")
                While reader.Read
                    lblCreatedDate.Text = Convert.ToDateTime(reader("CreatedDate")).ToString("dd MMM yyyy HH:mm:ss")
                    If reader("CreatedByFullName") <> "" Then
                        strTemp = String.Format("{0} ({1})", reader("CreatedBy"), reader("CreatedByFullName"))
                    Else
                        strTemp = reader("CreatedBy")
                    End If
                    lblCreatedBy.Text = strTemp
                    lblInvoiceCount.Text = Convert.ToString(reader("InvoiceCount"))
                    lblInvoiceValue.Text = Convert.ToDecimal(reader("InvoiceValueNet")).ToString("c")
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

            Me.RereadDataClicked(sender, e)
            js.AppendFormat("Recreate_dteStartDateID='{0}';", dteStartDate.ClientID)
            js.AppendFormat("Recreate_tmeStartDateID='{0}';", tmeStartDate.ClientID)
            js.AppendFormat("Recreate_dtePostingDateID='{0}';", dtePostingDate.ClientID)
            js.AppendFormat("Recreate_cboPostingYearID='{0}';", cboPostingYear.ClientID)
            js.AppendFormat("Recreate_cboPeriodNumID='{0}';", cboPeriodNum.ClientID)
            If Utils.IsDate(lblPostingDate.Text) Then
                js.AppendFormat("Recreate_postingDate='{0}';", Convert.ToDateTime(lblPostingDate.Text).ToShortDateString)
            Else
                js.Append("Recreate_postingDate='';")
            End If
            js.AppendFormat("Init();")

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

        Protected Sub RereadDataClicked(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim interfaceLog As New InterfaceLog
            Dim xmlTransforms As XMLTransformationCollection = Nothing
            Dim msg As New ErrorMessage

            '++ Determine whether this site uses Customer Files.
            '++ The XmlTransformation.SkipOnRereadData field where the Reference
            '++ field = 'DEBTWEBSTDNEW' is the governing flag..
            msg = XMLTransformation.FetchList(Me.DbConnection, xmlTransforms, "DEBTWEBSTDNEW")
            If msg.Success AndAlso xmlTransforms.Count > 0 Then
                If xmlTransforms(0).SkipOnRereadData Then   '++ Customer File in use..
                    interfaceLog.DbConnection = Me.DbConnection
                    msg = interfaceLog.Fetch(_interfaceID)

                    If msg.Success AndAlso interfaceLog.RerunAndReread Then
                        chkCustomerFile.LabelAttributes.Add("style", "color:lightgrey;width:14.75em;")
                        chkCustomerFile.Checked = False
                        chkCustomerFile.Enabled = False
                        lblCustomerFile.Text = "NOTE: Cannot recreate Customer File because underlying data has been re-read."
                    Else
                        If chkRereadData.Checked Then
                            If chkCustomerFile.Enabled Then
                                chkCustomerFile.LabelAttributes.Add("style", "color:lightgrey;width:14.75em;")
                                chkCustomerFile.Checked = False
                                chkCustomerFile.Enabled = False
                                lblRereadData.Text = "NOTE: Selecting this option will prevent future Customer File recreation for this batch."
                                lblCustomerFile.Text = "NOTE: Cannot recreate Customer File if underlying data is re-read."
                            End If
                        Else
                            If Not chkCustomerFile.Enabled Then
                                chkCustomerFile.Enabled = True
                                chkCustomerFile.LabelAttributes.Add("style", "color:black;width:14.75em;")
                                lblRereadData.Text = ""
                                lblCustomerFile.Text = ""
                            End If
                        End If
                    End If
                Else    '++ Customer File not in use..
                    chkCustomerFile.LabelAttributes.Add("style", "color:lightgrey;width:14.75em;")
                    chkCustomerFile.Checked = False
                    chkCustomerFile.Enabled = False
                    lblCustomerFile.Text = "NOTE: Customer File data not available."
                End If
            End If

            '++ Ensure the other PostBack fields are refreshed correctly..
            Me.CreationOptionClicked(sender, e)

            xmlTransforms = Nothing
            interfaceLog = Nothing
            msg = Nothing
        End Sub

        Private Sub btnRecreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRecreate.Click

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser
            Dim jobID As Integer = 0
            Dim batchID As Integer = 0, savedBatchRef As String = ""
            Dim startDateTime As String = ""
            Dim currQuery As String, asParams() As String, asValues() As String
            Dim lPos As Integer = 0, lCount As Integer = 0
            Dim a4wUser As Target.Abacus.Library.DataClasses.Users

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

                Dim interfaceLog As New InterfaceLog
                interfaceLog.DbConnection = Me.DbConnection
                msg = interfaceLog.Fetch(batchID)
                If chkRereadData.Checked Then
                    If msg.Success Then
                        '++ Store the fact that the batch is to be both re-run and
                        '++ the underlying data re-gathered. Customer Files are thus
                        '++ no longer possible for this batch..
                        savedBatchRef = interfaceLog.BatchRef
                        interfaceLog.RerunAndReread = TriState.True
                    End If
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                Else
                    '++ Get the batch ref for the known interface ID..
                    If msg.Success Then
                        savedBatchRef = interfaceLog.BatchRef
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                End If
                If msg.Success Then
                    interfaceLog.ProduceCustomerFile = chkCustomerFile.Checked
                    msg = interfaceLog.Save()
                End If

                '++ Create the job
                msg = DebtorInvoicesBL.CreateNewJob(Me.DbConnection, _
                                                    batchID, _
                                                    savedBatchRef, _
                                                    chkRereadData.Checked, _
                                                    RollbackOption.DoNotRollback, _
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