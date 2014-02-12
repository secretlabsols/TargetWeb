Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.ProviderInvoices

    ''' <summary>
    ''' Screen used to list all contracts under a given domiciliary provider invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     JohnF       17/02/2009  Created (D11493)
    ''' </history>
    Partial Class ViewBatchContracts
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Const SP_NAME_FETCH_INVOICEBATCH As String = "spxDomProviderInvoiceBatch_FetchAll"

            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim providerID As String = Convert.ToString(Request.QueryString("providerid"))
            Dim contractID As String = Convert.ToString(Request.QueryString("contractid"))
            Dim providerName As String = Convert.ToString(Request.QueryString("providerName"))
            Dim contractNum As String = Convert.ToString(Request.QueryString("contractNum"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reader As SqlDataReader = Nothing
            Dim js As String
            Dim style As New StringBuilder
            Dim msg As ErrorMessage = Nothing
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryProviderInvoiceBatches"), "Domiciliary Provider Invoice Batches")

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Dom/ProviderInvoices/ViewBatchContracts.js"))
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
                lblInvoiceValueNet.Text = Convert.ToDecimal("0").ToString("c")
                lblInvoiceValueVAT.Text = Convert.ToDecimal("0").ToString("c")
                lblInvoiceValueGross.Text = Convert.ToDecimal("0").ToString("c")
                While reader.Read
                    lblCreatedDate.Text = Convert.ToDateTime(reader("CreatedDate")).ToString("dd MMM yyyy  HH:mm:ss")
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
                End While
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEBATCH, "ViewBatchContracts.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            js = String.Format("currentPage={0};batchID={1};providerID={2};providerName={3}contractID={4};contractNum={5};", _
                 currentPage, batchID, _
                  IIf(providerID Is Nothing Or providerID = "null", "0", String.Format("'{0}'", providerID)), _
                  IIf(providerName Is Nothing Or providerName = "null", "null", String.Format("'{0}'", providerName)), _
                  IIf(contractID Is Nothing Or contractID = "null", "0", String.Format("'{0}'", contractID)), _
                  IIf(contractNum Is Nothing Or contractNum = "null", "null", String.Format("'{0}'", contractNum)))

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

    End Class
End Namespace