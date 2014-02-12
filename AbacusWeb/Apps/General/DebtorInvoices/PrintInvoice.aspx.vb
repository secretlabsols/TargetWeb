
Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DebtorInvoices
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.General.DebtorInvoices

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.General.DebtorInvoices.PrintInvoice
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Popup window to print the selected debtor invoices.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      23/07/2009  D11651 - parameter consolidation
    ''' 	JohnF   	13/05/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class PrintInvoice
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const PRINT_PAGE_SIZE As Integer = 1000000
            
            Dim msg As ErrorMessage
            Dim spParams As SqlParameter()
            Dim style As New StringBuilder
            Dim dtTable As DataTable = Nothing
            Dim filters As StringBuilder
            Dim filterString As String = ""
            Dim qsParser As WizardScreenParameters = New WizardScreenParameters(Request.QueryString)

            Const SP_FETCH_INVS As String = "spxDebtorInvoice_FetchListWithPaging"
            Const SP_FETCH_CLIENT As String = "spxClientDetail_Fetch"

            'Display the list of invoices.
            Try
                Me.EnableTimeout = False

                Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DebtorInvoices"), "View Debtor Invoices")

                style.Append("label.label { float:left; width:8em; font-weight:bold; }")
                style.Append("span.label { float:left; width:8em; padding-right:1em; font-weight:bold; }")
                Me.AddExtraCssStyle(style.ToString)

                filters = New StringBuilder()

                If qsParser.ClientID = 0 Then
                    lblFilterDebtor.Text = "(all debtors)"
                Else
                    Dim spSUParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_CLIENT, False)
                    spSUParams(0).Direction = ParameterDirection.Input
                    spSUParams(0).Value = qsParser.ClientID
                    Dim SUDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_CLIENT, spSUParams)
                    If SUDataset.Tables.Count > 0 Then
                        Dim SUDataTable As DataTable = SUDataset.Tables(0)
                        lblFilterDebtor.Text = String.Format("{0}/{1}", SUDataTable.Rows(0)("Reference"), SUDataTable.Rows(0)("Name"))
                    Else
                        lblFilterDebtor.Text = String.Format(">>WARNING: Client ID {0} not found<<", qsParser.ClientID)
                    End If
                    spSUParams = Nothing
                    SUDataset = Nothing
                End If

                lblFilterInvTypes.Text = qsParser.SelectedInvoiceTypesDesc
                lblFilterCreationDates.Text = qsParser.SelectedCreationDatesDesc
                lblFilterOther.Text = qsParser.SelectedOtherSettingsDesc

                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_INVS, False)
                spParams(0).Direction = ParameterDirection.InputOutput  '++ Current page..
                spParams(0).Value = 1
                spParams(1).Direction = ParameterDirection.InputOutput  '++ Records returned..
                spParams(2).Value = PRINT_PAGE_SIZE
                spParams(3).Value = Convert.DBNull
                spParams(4).Value = Convert.ToInt32(qsParser.ClientID)
                spParams(5).Value = Convert.ToBoolean(qsParser.InvoiceRes)
                spParams(6).Value = Convert.ToBoolean(qsParser.InvoiceDom)
                spParams(7).Value = Convert.ToBoolean(qsParser.InvoiceLD)
                spParams(8).Value = Convert.ToBoolean(qsParser.InvoiceClient)
                spParams(9).Value = Convert.ToBoolean(qsParser.InvoiceTP)
                spParams(10).Value = Convert.ToBoolean(qsParser.InvoiceProp)
                spParams(11).Value = Convert.ToBoolean(qsParser.InvoiceOLA)
                spParams(12).Value = Convert.ToBoolean(qsParser.InvoicePenColl)
                spParams(13).Value = Convert.ToBoolean(qsParser.InvoiceHomeColl)
                spParams(14).Value = Convert.ToBoolean(qsParser.InvoiceStd)
                spParams(15).Value = Convert.ToBoolean(qsParser.InvoiceMan)
                spParams(16).Value = Convert.ToBoolean(qsParser.InvoiceSDS)
                spParams(17).Value = Convert.ToBoolean(qsParser.InvoiceActual)
                spParams(18).Value = Convert.ToBoolean(qsParser.InvoiceProvisional)
                spParams(19).Value = Convert.ToBoolean(qsParser.InvoiceRetracted)
                spParams(20).Value = Convert.ToBoolean(qsParser.InvoiceViaRetract)
                If IsDate(qsParser.InvoiceDateFrom) Then spParams(21).Value = Convert.ToDateTime(qsParser.InvoiceDateFrom)
                If IsDate(qsParser.InvoiceDateTo) Then spParams(22).Value = Convert.ToDateTime(qsParser.InvoiceDateTo)
                spParams(23).Value = Convert.ToInt32(qsParser.InvoiceBatchSel)
                If qsParser.InvoiceExclude.ToLower = "true" Or qsParser.InvoiceExclude.ToLower = "false" Then
                    spParams(24).Value = Convert.ToBoolean(qsParser.InvoiceExclude)
                Else
                    spParams(24).Value = Convert.DBNull
                End If
                If qsParser.FilterDebtor <> String.Empty AndAlso qsParser.FilterDebtor <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Debtor = '{0}'", qsParser.FilterDebtor.ToUpper)
                    spParams(25).Value = qsParser.FilterDebtor.Replace("*", "%")
                End If
                If qsParser.FilterInvoiceNumber <> String.Empty AndAlso qsParser.FilterInvoiceNumber <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Invoice No. = '{0}'", qsParser.FilterInvoiceNumber.ToUpper)
                    spParams(26).Value = qsParser.FilterInvoiceNumber.Replace("*", "%")
                End If
                spParams(27).Value = 0
                spParams(28).Value = Convert.ToBoolean(qsParser.InvoiceZeroValue)
                If qsParser.FilterClientRef <> String.Empty AndAlso qsParser.FilterClientRef <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Client Ref. = '{0}'", qsParser.FilterClientRef.ToUpper)
                    spParams(29).Value = qsParser.FilterClientRef.Replace("*", "%")
                End If
                If qsParser.FilterComment <> String.Empty AndAlso qsParser.FilterComment <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Comment = '{0}'", qsParser.FilterComment.ToUpper)
                    spParams(30).Value = qsParser.FilterComment.Replace("*", "%")
                End If

                dtTable = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_INVS, spParams).Tables(0)
                rptInvoices.DataSource = dtTable
                If filterString <> "" Then
                    lblFilter.Text = "Filter(s):"
                    lblFilters.Text = filterString
                    lblFilter.Visible = True
                    lblFilters.Visible = True
                Else
                    lblFilter.Visible = False
                    lblFilters.Visible = False
                End If
                rptInvoices.DataBind()

                dtTable = Nothing

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_FETCH_INVS, "PrintInvoice.Page_Load")   ' could not read data
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