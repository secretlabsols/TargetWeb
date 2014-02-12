Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.ProviderInvoices

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.Dom.ProviderInvoices.PrintBatchInvoice
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Popup window to print the domiciliary provider invoices selected from a batch.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	JohnF   	19/02/2009	Created
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
            Dim dpiID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("invID"))
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))
            Dim providerID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("providerID"))
            Dim contractID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("contractID"))
            Dim clientID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("clientID"))
            Dim providerName As String = Request.QueryString("providerName")
            Dim contractNum As String = Request.QueryString("contractNum")
            Dim clientName As String = Request.QueryString("clientName")
            Dim filterString As String = ""
            Dim invTotalNet As Decimal = 0, invTotalVAT As Decimal = 0, invTotalGross As Decimal = 0
            Dim invCount As Integer = 0

            Const SP_FETCH_DPIS As String = "spxDomProviderInvoice_FetchListWithPaging"
            Const SP_FETCH_INVOICEBATCH As String = "spxDomProviderInvoiceBatch_FetchAll"

            'Display the list of invoices.
            Try
                Me.EnableTimeout = False

                Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryProviderInvoiceBatches"), "Domiciliary Provider Invoice Batches")

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
                    lblInvoiceValueVAT.Text = Convert.ToDecimal(SUDataTable.Rows(0)("InvoiceValueVAT")).ToString("c")
                    lblInvoiceValueGross.Text = Convert.ToDecimal(SUDataTable.Rows(0)("InvoiceValueGross")).ToString("c")
                Else
                    lblCreatedDate.Text = String.Format(">>WARNING: Invoice Batch ID {0} not found<<", batchID)
                End If
                spBatchParams = Nothing
                batchDataset = Nothing

                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_DPIS, False)
                spParams(0).Direction = ParameterDirection.InputOutput
                spParams(0).Value = 1
                spParams(1).Value = PRINT_PAGE_SIZE
                If dpiID > 0 Then spParams(2).Value = dpiID
                If providerID > 0 Then spParams(3).Value = providerID
                If providerName <> String.Empty AndAlso providerName <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Provider Name = '{0}'", providerName.ToUpper)
                    spParams(4).Value = providerName.Replace("*", "%")
                End If
                If contractID > 0 Then spParams(5).Value = contractID
                If contractNum <> String.Empty AndAlso contractNum <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Contract No. = '{0}'", contractNum.ToUpper)
                    spParams(6).Value = contractNum.Replace("*", "%")
                End If
                If clientID > 0 Then spParams(7).Value = clientID
                If clientName <> String.Empty AndAlso clientName <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Svc User Name = '{0}'", clientName.ToUpper)
                    spParams(8).Value = clientName.Replace("*", "%")
                End If
                spParams(15).Value = True
                spParams(16).Value = True
                spParams(17).Value = True
                spParams(18).Value = True
                spParams(19).Direction = ParameterDirection.InputOutput
                If batchID > 0 Then spParams(20).Value = batchID
                dtTable = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_DPIS, spParams).Tables(0)
                rptInvoices.DataSource = dtTable
                If filterString <> "" Then
                    lblFilter.Text = "Filter(s):"
                    lblFilters.Text = filterString
                    lblFilter.Visible = True
                    lblFilters.Visible = True
                    invCount = (dtTable.Rows.Count)
                    For Each dtRow As DataRow In dtTable.Rows
                        invTotalNet += dtRow("InvoiceTotalNet")
                        invTotalVAT += dtRow("InvoiceTotalVAT")
                        invTotalGross += dtRow("InvoiceTotalGross")
                        Exit For
                    Next

                    lblInvoiceCount.Text = invCount.ToString
                    lblInvoiceValueNet.Text = invTotalNet.ToString("c")
                    lblInvoiceValueVAT.Text = invTotalVAT.ToString("c")
                    lblInvoiceValueGross.Text = invTotalGross.ToString("c")
                Else
                    lblFilter.Visible = False
                    lblFilters.Visible = False
                End If
                rptInvoices.DataBind()

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_FETCH_DPIS, "PrintBatchInvoice.Page_Load")   ' could not read data
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
    End Class

End Namespace