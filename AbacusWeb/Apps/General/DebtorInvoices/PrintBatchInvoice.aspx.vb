Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.General.DebtorInvoices

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.General.DebtorInvoices.PrintBatchInvoice
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Popup window to print the debtor invoices selected from a batch.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	JohnF   	13/05/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class PrintBatchInvoice
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Const PRINT_PAGE_SIZE As Integer = 1000000

            Dim user As WebSecurityUser
            Dim msg As ErrorMessage
            Dim spParams As SqlParameter()
            Dim style As New StringBuilder
            Dim dtTable As DataTable = Nothing
            Dim stringTemp As String = ""
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))
            Dim filterDebtor As String = Target.Library.Utils.ToString(Request.QueryString("filterDebtor"))
            Dim filterInvNum As String = Target.Library.Utils.ToString(Request.QueryString("filterInvNum"))
            Dim filterString As String = ""
            Dim invTotalNet As Decimal = 0
            Dim invCount As Integer = 0

            Const SP_FETCH_INVS As String = "spxDebtorInvoice_FetchListWithPaging"
            Const SP_FETCH_INVOICEBATCH As String = "spxDebtorInvoiceBatch_FetchAll"

            'Display the list of invoices.
            Try
                Me.EnableTimeout = False

                Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DebtorInvoiceBatches"), "Debtor Invoice Batches")

                style.Append("label.label { float:left; width:8em; font-weight:bold; }")
                style.Append("span.label { float:left; width:8em; padding-right:1em; font-weight:bold; }")
                Me.AddExtraCssStyle(style.ToString)

                ' check user is logged in
                user = SecurityBL.GetCurrentUser()

                Dim spBatchParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_INVOICEBATCH, False)
                spBatchParams(0).Direction = ParameterDirection.Input
                spBatchParams(0).Value = batchID
                Dim batchDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_INVOICEBATCH, spBatchParams)
                If batchDataset.Tables.Count > 0 Then
                    Dim SUDataTable As DataTable = batchDataset.Tables(0)
                    lblCreatedDate.Text = Convert.ToDateTime(SUDataTable.Rows(0)("CreatedDate")).ToString("dd MMM yyyy HH:mm:ss")
                    If SUDataTable.Rows(0)("CreatedByFullName") <> "" Then
                        stringTemp = String.Format("{0} ({1})", SUDataTable.Rows(0)("CreatedBy"), SUDataTable.Rows(0)("CreatedByFullName"))
                    Else
                        stringTemp = SUDataTable.Rows(0)("CreatedBy")
                    End If
                    lblCreatedBy.Text = stringTemp
                    lblInvoiceCount.Text = Convert.ToString(SUDataTable.Rows(0)("InvoiceCount"))
                    lblInvoiceValueNet.Text = Convert.ToDecimal(SUDataTable.Rows(0)("InvoiceValueNet")).ToString("c")
                Else
                    lblCreatedDate.Text = String.Format(">>WARNING: Invoice Batch ID {0} not found<<", batchID)
                End If
                spBatchParams = Nothing
                batchDataset = Nothing

                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_INVS, False)
                spParams(0).Direction = ParameterDirection.InputOutput  '++ Current page..
                spParams(0).Value = 1
                spParams(1).Direction = ParameterDirection.InputOutput  '++ Records returned..
                spParams(2).Value = PRINT_PAGE_SIZE
                spParams(23).Value = 1  '++ 'Batched' invoices only
                If filterDebtor <> String.Empty AndAlso filterDebtor <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Debtor = '{0}'", filterDebtor.ToUpper)
                    spParams(25).Value = filterDebtor.Replace("*", "%")
                End If
                If filterInvNum <> String.Empty AndAlso filterInvNum <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Invoice No. = '{0}'", filterInvNum.ToUpper)
                    spParams(26).Value = filterInvNum.Replace("*", "%")
                End If
                spParams(27).Value = batchID    '++ Invoices linked to this batch only..
                dtTable = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_INVS, spParams).Tables(0)

                rptInvoices.DataSource = dtTable
                If filterString <> "" Then
                    lblFilter.Text = "Filter(s):"
                    lblFilters.Text = filterString
                    lblFilter.Visible = True
                    lblFilters.Visible = True
                    invCount = (dtTable.Rows.Count)
                    For Each dtRow As DataRow In dtTable.Rows
                        invTotalNet += dtRow("InvoiceTotalValue")
                        Exit For
                    Next

                    lblInvoiceCount.Text = invCount.ToString
                    lblInvoiceValueNet.Text = invTotalNet.ToString("c")
                Else
                    lblFilter.Visible = False
                    lblFilters.Visible = False
                End If
                rptInvoices.DataBind()

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_FETCH_INVS, "PrintBatchInvoice.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If dtTable IsNot Nothing Then dtTable = Nothing
            End Try
        End Sub

        Protected Function GetQueryText(ByVal queryDesc As Object, ByVal queryDate As Object) As String

            If Not Convert.IsDBNull(queryDesc) AndAlso Not queryDesc Is Nothing Then
                Return String.Format("{0} - {1}", Convert.ToDateTime(queryDate).ToString("dd MMM yyyy"), Convert.ToString(queryDesc).Replace(vbCrLf, "<br />"))
            Else
                Return String.Empty
            End If

        End Function
    End Class

End Namespace