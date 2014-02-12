Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.General.DebtorInvoices

    ''' <summary>
    ''' Screen used to list all invoices under a given debtor invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     JohnF       11/05/2009  Created (D11605)
    ''' </history>
    Partial Class ViewBatchInvoices
        Inherits Target.Web.Apps.BasePage

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

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DebtorInvoiceBatches"), "Debtor Invoice Batches - Invoices")

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/General/DebtorInvoices/ViewBatchInvoices.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
            style.Append("label.label { float:left; width:14.5em; font-weight:bold; }")
            style.Append("span.label { float:left; width:14.5em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_INVOICEBATCH, False)
                Dim strTemp As String
                spParams(0).Value = batchID
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
                End While
                reader.Close()
                reader = Nothing
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEBATCH, "ViewBatchInvoices.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            js = String.Format("currentPage={0};batchid={1};", currentPage, batchID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

    End Class
End Namespace