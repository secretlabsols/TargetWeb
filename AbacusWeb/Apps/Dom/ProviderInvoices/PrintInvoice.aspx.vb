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
    ''' Class	 : Abacus.Web.Apps.UserControls.Dom.ProviderInvoices.PrintInvoice
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Popup window to print the selected domiciliary provider invoices.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	JohnF   	13/02/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class PrintInvoice
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Const PRINT_PAGE_SIZE As Integer = 1000000

            Dim user As WebSecurityUser
            Dim msg As ErrorMessage
            Dim spParams As SqlParameter()
            Dim style As New StringBuilder
            Dim dtTable As DataTable = Nothing
            Dim filters As StringBuilder
            Dim stringTemp As String = ""
            Dim invoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("invID"))
            Dim providerID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("providerID"))
            Dim providerName As String = Request.QueryString("providerName")
            Dim contractID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("contractID"))
            Dim contractNum As String = Request.QueryString("contractNum")
            Dim clientID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("clientID"))
            Dim clientName As String = Request.QueryString("clientName")
            Dim weFrom As String = Request.QueryString("weFrom")
            Dim weTo As String = Request.QueryString("weTo")
            Dim invoiceNumber As String = Request.QueryString("invoiceNumber")
            Dim invoiceRef As String = Request.QueryString("invoiceRef")
            Dim invNumFilter As String = Request.QueryString("invoiceNumberFilter")
            Dim invRefFilter As String = Request.QueryString("invoiceRefFilter")
            Dim statusDateFrom As String = Request.QueryString("statusDateFrom")
            Dim statusDateTo As String = Request.QueryString("statusDateTo")
            Dim statusIsUnpaid As String = Request.QueryString("statusIsUnpaid")
            Dim statusIsAuthorised As String = Request.QueryString("statusIsAuthorised")
            Dim statusIsPaid As String = Request.QueryString("statusIsPaid")
            Dim statusIsSuspended As String = Request.QueryString("statusIsSuspended")
            Dim filterString As String = ""

            Const SP_FETCH_DPIS As String = "spxDomProviderInvoice_FetchListWithPaging"
            Const SP_FETCH_PROVIDER As String = "spxEstablishment_Fetch"
            Const SP_FETCH_DOMCONTRACT As String = "spxDomContract_Fetch"
            Const SP_FETCH_CLIENT As String = "spxClientDetail_Fetch"

            'Display the list of invoices.
            Try
                Me.EnableTimeout = False

                Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryProviderInvoices"), "View Domiciliary Provider Invoices")

                style.Append("label.label { float:left; width:8em; font-weight:bold; }")
                style.Append("span.label { float:left; width:8em; padding-right:1em; font-weight:bold; }")
                Me.AddExtraCssStyle(style.ToString)

                ' check user is logged in
                user = SecurityBL.GetCurrentUser()

                filters = New StringBuilder()

                If providerID = 0 Then
                    lblFilterProvider.Text = "(all providers)"
                Else
                    Dim spProvParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_PROVIDER, False)
                    spProvParams(0).Direction = ParameterDirection.Input
                    spProvParams(0).Value = providerID
                    Dim provDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_PROVIDER, spProvParams)
                    If provDataset.Tables.Count > 0 Then
                        Dim provDataTable As DataTable = provDataset.Tables(0)
                        lblFilterProvider.Text = String.Format("{0}/{1}", provDataTable.Rows(0)("AltReference"), provDataTable.Rows(0)("Name"))
                    Else
                        lblFilterProvider.Text = String.Format(">>WARNING: Provider ID {0} not found<<", providerID)
                    End If
                    spProvParams = Nothing
                    provDataset = Nothing
                End If

                If contractID = 0 Then
                    lblFilterContract.Text = "(all contracts)"
                Else
                    Dim spContParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_DOMCONTRACT, False)
                    spContParams(0).Direction = ParameterDirection.Input
                    spContParams(0).Value = contractID
                    Dim contDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_DOMCONTRACT, spContParams)
                    If contDataset.Tables.Count > 0 Then
                        Dim contDataTable As DataTable = contDataset.Tables(0)
                        lblFilterContract.Text = String.Format("{0}/{1}", contDataTable.Rows(0)("Number"), contDataTable.Rows(0)("Title"))
                    Else
                        lblFilterContract.Text = String.Format(">>WARNING: Contract ID {0} not found<<", contractID)
                    End If
                    spContParams = Nothing
                    contDataset = Nothing
                End If

                If clientID = 0 Then
                    lblFilterClient.Text = "(all service users)"
                Else
                    Dim spSUParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_CLIENT, False)
                    spSUParams(0).Direction = ParameterDirection.Input
                    spSUParams(0).Value = clientID
                    Dim SUDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_CLIENT, spSUParams)
                    If SUDataset.Tables.Count > 0 Then
                        Dim SUDataTable As DataTable = SUDataset.Tables(0)
                        lblFilterClient.Text = String.Format("{0}/{1}", SUDataTable.Rows(0)("Reference"), SUDataTable.Rows(0)("Name"))
                    Else
                        lblFilterClient.Text = String.Format(">>WARNING: Client ID {0} not found<<", clientID)
                    End If
                    spSUParams = Nothing
                    SUDataset = Nothing
                End If

                If invoiceNumber = String.Empty Or invoiceNumber = "null" Then
                    lblFilterInvoiceNum.Text = "(all invoice numbers)"
                Else
                    If invoiceNumber.IndexOf("*") >= 0 Then
                        lblFilterInvoiceNum.Text = String.Format("Matching '{0}'", invoiceNumber.ToUpper)
                    Else
                        lblFilterInvoiceNum.Text = invoiceNumber.ToUpper
                    End If
                End If

                If invoiceRef = String.Empty Or invoiceRef = "null" Then
                    lblFilterInvoiceRef.Text = "(all invoice references)"
                Else
                    If invoiceRef.IndexOf("*") >= 0 Then
                        lblFilterInvoiceRef.Text = String.Format("Matching '{0}'", invoiceRef.ToUpper)
                    Else
                        lblFilterInvoiceRef.Text = invoiceRef.ToUpper
                    End If
                End If

                If Not IsValidDate(weFrom) And Not IsValidDate(weTo) Then
                    lblFilterWEDates.Text = "(all w/e dates)"
                ElseIf IsValidDate(weFrom) And Not IsValidDate(weTo) Then
                    lblFilterWEDates.Text = String.Format("From {0} onwards", Convert.ToDateTime(weFrom).ToString("dd MMM yyyy"))
                ElseIf Not IsValidDate(weFrom) And IsValidDate(weTo) Then
                    lblFilterWEDates.Text = String.Format("Up to {0}", Convert.ToDateTime(weTo).ToString("dd MMM yyyy"))
                Else
                    lblFilterWEDates.Text = String.Format("Between {0} and {1}", Convert.ToDateTime(weFrom).ToString("dd MMM yyyy"), Convert.ToDateTime(weTo).ToString("dd MMM yyyy"))
                End If

                stringTemp = ""
                If statusIsUnpaid = "true" Then stringTemp &= IIf(stringTemp = "", "Unpaid", "; Unpaid")
                If statusIsAuthorised = "true" Then stringTemp &= IIf(stringTemp = "", "Authorised", "; Authorised")
                If statusIsPaid = "true" Then stringTemp &= IIf(stringTemp = "", "Paid", "; Paid")
                If statusIsSuspended = "true" Then stringTemp &= IIf(stringTemp = "", "Suspended", "; Suspended")
                If stringTemp = "" Then stringTemp = "(none selected)"
                lblFilterStatus.Text = stringTemp

                If Not IsValidDate(statusDateFrom) And Not IsValidDate(statusDateTo) Then
                    lblFilterStatusDates.Text = "(all status dates)"
                ElseIf IsValidDate(statusDateFrom) And Not IsValidDate(statusDateTo) Then
                    lblFilterStatusDates.Text = String.Format("From {0} onwards", Convert.ToDateTime(statusDateFrom).ToString("dd MMM yyyy"))
                ElseIf Not IsValidDate(statusDateFrom) And IsValidDate(statusDateTo) Then
                    lblFilterStatusDates.Text = String.Format("Up to {0}", Convert.ToDateTime(statusDateTo).ToString("dd MMM yyyy"))
                Else
                    lblFilterStatusDates.Text = String.Format("Between {0} and {1}", Convert.ToDateTime(statusDateFrom).ToString("dd MMM yyyy"), Convert.ToDateTime(statusDateTo).ToString("dd MMM yyyy"))
                End If

                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_DPIS, False)
                spParams(0).Direction = ParameterDirection.InputOutput
                spParams(0).Value = 1
                spParams(1).Value = PRINT_PAGE_SIZE
                If invoiceID > 0 Then spParams(2).Value = invoiceID
                If providerID > 0 Then spParams(3).Value = providerID
                If providerName <> String.Empty AndAlso providerName <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Provider = '{0}'", providerName.ToUpper)
                    spParams(4).Value = providerName.Replace("*", "%")
                End If
                If contractID > 0 Then spParams(5).Value = contractID
                If contractNum <> String.Empty AndAlso contractNum <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Contract = '{0}'", contractNum.ToUpper)
                    spParams(6).Value = contractNum.Replace("*", "%")
                End If
                If clientID > 0 Then spParams(7).Value = clientID
                If clientName <> String.Empty AndAlso clientName <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Svc User Name = '{0}'", clientName.ToUpper)
                    spParams(8).Value = clientName.Replace("*", "%")
                End If
                If Utils.IsDate(weFrom) Then spParams(9).Value = weFrom
                If Utils.IsDate(weTo) Then spParams(10).Value = weTo
                If invoiceNumber <> String.Empty AndAlso invoiceNumber <> "null" Then spParams(11).Value = invoiceNumber.Replace("*", "%")
                If invoiceRef <> String.Empty AndAlso invoiceRef <> "null" Then spParams(12).Value = invoiceRef.Replace("*", "%")
                If Utils.IsDate(statusDateFrom) Then spParams(13).Value = statusDateFrom
                If Utils.IsDate(statusDateTo) Then spParams(14).Value = statusDateTo
                spParams(15).Value = statusIsUnpaid
                spParams(16).Value = statusIsAuthorised
                spParams(17).Value = statusIsPaid
                spParams(18).Value = statusIsSuspended
                spParams(19).Direction = ParameterDirection.InputOutput
                If invNumFilter <> String.Empty AndAlso invNumFilter <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Invoice No. = '{0}'", invNumFilter.ToUpper)
                    spParams(21).Value = invNumFilter.Replace("*", "%")
                End If
                If invRefFilter <> String.Empty AndAlso invRefFilter <> "null" Then
                    filterString &= IIf(filterString <> "", "; ", "") & String.Format("Invoice Ref. = '{0}'", invRefFilter.ToUpper)
                    spParams(22).Value = invRefFilter.Replace("*", "%")
                End If
                dtTable = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_DPIS, spParams).Tables(0)
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
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_FETCH_DPIS, "PrintInvoice.Page_Load")   ' could not read data
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