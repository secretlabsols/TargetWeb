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
Imports Target.Library.Web.UserControls

Namespace Apps.Dom.ProviderInvoices

    ''' <summary>
    ''' Screen used to view the available reports for domiciliary provider invoice batches.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO  09/07/2009  D11630 - created.
    ''' </history>
    Partial Class BatchReports
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SP_NAME_FETCH_INVOICEBATCH As String = "spxDomProviderInvoiceBatch_FetchAll"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryProviderInvoiceBatches"), "Domiciliary Provider Invoice Batches - Reports")

            Dim batchID As Integer = Utils.ToInt32(Request.QueryString("batchid"))
            Dim jobID As Integer = 0
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Dim style As StringBuilder = New StringBuilder()

            Me.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

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
                SqlHelper.CloseReader(reader)
            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEBATCH, "BatchReports.Page_Load")   ' could not read data
                WebUtils.DisplayError(msg)
            Finally
                SqlHelper.CloseReader(reader)
            End Try

            With lstReports
                .Rows = 10
                .Attributes.Add("onchange", "lstReports_Change();")
                With .Items
                    .Add(New ListItem("Batch summary", divBatchSummary.ClientID))
                    .Add(New ListItem("Simple list of provider invoices in the batch", divDpiList.ClientID))
                    .Add(New ListItem("Simple list of provider invoice lines in the batch", divDpiLineList.ClientID))
                    .Add(New ListItem("Simple list of service orders first paid in the batch", divFirstPaymentDsoList.ClientID))
                End With
            End With

            ' batch summary
            With CType(ctlBatchSummary, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DomiciliaryProviderInvoiceBatchSummary")
                .Parameters.Add("intBatchID", batchID)
            End With

            ' DPI list
            With CType(ctlDpiList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DomiciliaryProviderInvoices")
                .Parameters.Add("bStatusUnpaid", True)
                .Parameters.Add("bStatusAuthorised", True)
                .Parameters.Add("bStatusPaid", True)
                .Parameters.Add("bStatusSuspended", True)
                .Parameters.Add("intBatchID", batchID)
            End With

            ' DPI line list
            With CType(ctlDpiLineList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DomiciliaryProviderInvoiceLines")
                .Parameters.Add("bStatusUnpaid", True)
                .Parameters.Add("bStatusAuthorised", True)
                .Parameters.Add("bStatusPaid", True)
                .Parameters.Add("bStatusSuspended", True)
                .Parameters.Add("intBatchID", batchID)
            End With

            ' first payment DSO list
            With CType(ctlFirstPaymentDsoList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.FirstPaymentDomiciliaryServiceOrders")
                .Parameters.Add("intBatchID", batchID)
            End With

        End Sub

    End Class

End Namespace