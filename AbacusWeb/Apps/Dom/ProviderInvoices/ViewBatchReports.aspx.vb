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
    ''' Screen used to recreate interface files under a given domiciliary provider invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      04/08/2009  A4WA#5653 - added progress bar JS include.
    '''     JohnF       02/04/2009  Created (D11297A)
    ''' </history>
    Partial Class ViewBatchReports
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Const SP_NAME_FETCH_INVOICEBATCH As String = "spxDomProviderInvoiceBatch_FetchAll"

            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchid"))
            Dim jobID As Integer = 0
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reader As SqlDataReader = Nothing
            Dim js As String
            Dim style As New StringBuilder
            Dim msg As ErrorMessage = Nothing
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))

            If currentPage <= 0 Then currentPage = 1

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryProviderInvoiceBatchesViewReports"), "Domiciliary Provider Invoice Batches - View Job Results")

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add table sorting JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add progress bar JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/ProgressBar.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Dom/ProviderInvoices/ViewBatchReports.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.Jobs.WebSvc.JobService))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(JobStatus))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(JobStepStatus))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(JobStepXml))

            style.Append("label.label { float:left; width:14.5em; font-weight:bold; }")
            style.Append("span.label { float:left; width:14.5em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_INVOICEBATCH, False)
                Dim strTemp As String
                spParams(0).Value = batchID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICEBATCH, spParams)

                lblInvoiceCount.Text = "0"
                lblInvoiceValueNet.Text = Convert.ToDecimal("0").ToString("c")
                lblInvoiceValueVAT.Text = Convert.ToDecimal("0").ToString("c")
                lblInvoiceValueGross.Text = Convert.ToDecimal("0").ToString("c")
                Do While reader.Read
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
                    jobID = reader("LastJobID")
                Loop
                reader.Close()
                reader = Nothing
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEBATCH, "ViewBatchReports.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            js = String.Format("batchid={0};jobid={1};Init();", batchID, jobID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
        End Sub
    End Class
End Namespace