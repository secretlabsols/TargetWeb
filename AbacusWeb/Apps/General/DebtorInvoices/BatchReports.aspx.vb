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

Namespace Apps.General.DebtorInvoices

    ''' <summary>
    ''' Screen used to view the available reports for debtor invoice batches.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     JohnF   26/08/2009  #5727 - fix batchID param on two reports
    '''     MikeVO  09/07/2009  D11630 - created.
    ''' </history>
    Partial Class BatchReports
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SP_NAME_FETCH_INVOICEBATCH As String = "spxDebtorInvoiceBatch_FetchAll"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DebtorInvoiceBatches"), "Debtor Invoice Batches - Reports")

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
                    .Add(New ListItem("Simple list of debtor invoices in the batch", divDpiList.ClientID))
                    .Add(New ListItem("Simple list of debtor invoice lines in the batch", divDpiLineList.ClientID))
                    .Add(New ListItem("Simple list of debtor payments in the batch", divBatchPayments.ClientID))
                End With
            End With

            ' batch summary
            With CType(ctlBatchSummary, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DebtorsInvoiceBatchSummary")
                .Parameters.Add("intBatchID", batchID)
            End With

            ' DPI list
            With CType(ctlDpiList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DebtorsInvoices")
                .Parameters.Add("pbResidential", True)
                .Parameters.Add("pbDomiciliary", True)
                .Parameters.Add("pbLearnDisab", True)
                .Parameters.Add("pbClient", True)
                .Parameters.Add("pbThirdParty", True)
                .Parameters.Add("pbProperty", True)
                .Parameters.Add("pbOLA", True)
                .Parameters.Add("pbPenCollect", True)
                .Parameters.Add("pbHomeCollect", True)
                .Parameters.Add("pbStandard", True)
                .Parameters.Add("pbManual", True)
                .Parameters.Add("pbSDS", True)
                .Parameters.Add("pbActual", True)
                .Parameters.Add("pbProvisional", True)
                .Parameters.Add("pbRetracted", True)
                .Parameters.Add("pbViaRetract", True)
                .Parameters.Add("pintBatchSelect", 0)
                .Parameters.Add("pintBatchID", batchID)
            End With

            ' DPI line list
            With CType(ctlDpiLineList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DebtorsInvoiceLines")
                .Parameters.Add("pbResidential", True)
                .Parameters.Add("pbDomiciliary", True)
                .Parameters.Add("pbLearnDisab", True)
                .Parameters.Add("pbClient", True)
                .Parameters.Add("pbThirdParty", True)
                .Parameters.Add("pbProperty", True)
                .Parameters.Add("pbOLA", True)
                .Parameters.Add("pbPenCollect", True)
                .Parameters.Add("pbHomeCollect", True)
                .Parameters.Add("pbStandard", True)
                .Parameters.Add("pbManual", True)
                .Parameters.Add("pbSDS", True)
                .Parameters.Add("pbActual", True)
                .Parameters.Add("pbProvisional", True)
                .Parameters.Add("pbRetracted", True)
                .Parameters.Add("pbViaRetract", True)
                .Parameters.Add("pintBatchSelect", 0)
                .Parameters.Add("pintBatchID", batchID)
            End With

            ' batch payments
            With CType(ctlBatchPayments, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DebtorsInvoiceBatchPayments")
                .Parameters.Add("intBatchID", batchID)
            End With


            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.General.DebtorInvoices.BatchReports", _
           Target.Library.Web.Utils.WrapClientScript(String.Format("Report_lstReportlistId='{0}';", _
                                                                   lstReports.ClientID _
                                                      )) _
                                              )

        End Sub

    End Class

End Namespace