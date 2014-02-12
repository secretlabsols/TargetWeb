Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.ProformaInvoice

    ''' <summary>
    ''' Screen to allow a user to print a list of invoices within a domiciliary pro forma invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      16/10/2009  D11546 - added payment mismatch tolerance and S/U Ref/Name filters.
    ''' </history>
    Partial Class PrintInvoice
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchid"))
            Dim queried As Integer = Target.Library.Utils.ToInt32(Request.QueryString("queried"))
            Dim mismatch As Integer = Target.Library.Utils.ToInt32(Request.QueryString("mismatch"))
            Dim tolerance As String = Request.QueryString("tolerance")
            Dim filterSURef As String = Request.QueryString("filterSURef")
            Dim filterSUName As String = Request.QueryString("filterSUName")

            Dim user As WebSecurityUser
            Dim msg As ErrorMessage
            Dim triQueried As TriState
            Dim triMismatch As TriState
            Dim spParams As SqlParameter()
            Dim style As New StringBuilder
            Dim reader As SqlDataReader = Nothing
            Dim filters As StringBuilder
            Dim decTolerance As Decimal

            Const SP_NAME As String = "spxDomProformaInvoice_FetchListWithPaging"
            Const SP_NAME_FETCH_INVOICEBATCH As String = "spxDomProformaInvoiceBatch_FetchForView"

            Me.EnableTimeout = False

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "View Domiciliary Proforma Invoice Batch")

            style.Append("label.label { float:left; width:8em; font-weight:bold; }")
            style.Append("span.label { float:left; width:8em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            ' check user is logged in
            user = SecurityBL.GetCurrentUser()

            filters = New StringBuilder()

            Select Case queried
                Case 0
                    triQueried = TriState.UseDefault
                    filters.Append("Queried & Non-Queried Invoices; ")
                Case 1
                    triQueried = TriState.True
                    filters.Append("Queried Invoices; ")
                Case 2
                    triQueried = TriState.False
                    filters.Append("Non-Queried Invoices; ")
            End Select

            If Not String.IsNullOrEmpty(tolerance) Then decTolerance = Convert.ToDecimal(tolerance)
            Select Case mismatch
                Case 0
                    triMismatch = TriState.UseDefault
                    filters.Append("Invoices with Mismatched & NonMismatched Payments; ")
                Case 1
                    triMismatch = TriState.True
                    filters.Append("Invoices with Mismatched Payments")
                    filters.AppendFormat(" (tolerance {0}); ", decTolerance.ToString("C"))
                Case 2
                    triMismatch = TriState.False
                    filters.Append("Invoices with NonMismatched Payments")
                    filters.AppendFormat(" (tolerance {0}); ", decTolerance.ToString("C"))
            End Select
            If Not String.IsNullOrEmpty(filterSURef) Then filters.AppendFormat("S/U Reference: {0}", filterSURef)
            If Not String.IsNullOrEmpty(filterSUName) Then filters.AppendFormat("S/U Name: {0}", filterSUName)

            lblFilters.Text = String.Format("Filters: {0}", filters.ToString())
            lblDatePrinted.Text = String.Format("Printed at {0} on {1}", Now.ToString("HH:mm:ss"), Now.ToString("dd MMM yyyy"))

            'Display the header information.
            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_INVOICEBATCH, False)
                spParams(0).Value = batchID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICEBATCH, spParams)

                While reader.Read
                    lblProvider.Text = String.Format("{0}/{1}", reader("ProviderReference"), reader("ProviderName"))
                    lblContract.Text = String.Format("{0}/{1}", reader("ContractNumber"), reader("ContractTitle"))
                    lblStatus.Text = Utils.SplitOnCapitals([Enum].GetName(GetType(DomProformaInvoiceBatchStatus), reader("DomProformaInvoiceBatchStatusID")))
                End While
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEBATCH, "ViewInvoices.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            'Display the list of invoices.
            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Direction = ParameterDirection.InputOutput
                spParams(0).Value = 1
                spParams(1).Value = Integer.MaxValue
                spParams(2).Value = user.ExternalUserID
                If batchID > 0 Then spParams(3).Value = batchID
                If triQueried <> TriState.UseDefault Then spParams(4).Value = triQueried
                If triMismatch <> TriState.UseDefault Then spParams(5).Value = triMismatch
                ' NOTE spParams(6) selectedInvoiceID not set
                If triMismatch <> TriState.UseDefault Then spParams(7).Value = decTolerance
                If Not String.IsNullOrEmpty(filterSURef) Then spParams(8).Value = filterSURef.Replace("*", "%")
                If Not String.IsNullOrEmpty(filterSUName) Then spParams(9).Value = filterSUName.Replace("*", "%")

                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                rptInvoices.DataSource = reader
                rptInvoices.DataBind()

                reader.Close()


            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME, "PrintInvoice.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
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