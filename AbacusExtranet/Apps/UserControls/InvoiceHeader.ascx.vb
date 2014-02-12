Imports Target.Web.Apps.Security
Imports Target.Web.Apps
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.UserControls

    Partial Public Class InvoiceHeader
        Inherits System.Web.UI.UserControl

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Dim invoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))

            Const SP_NAME_FETCH_INVOICEBATCH As String = "spxDomProformaInvoice_FetchForView"

            Dim msg As ErrorMessage
            Dim pnlInvoiceLines As New Panel

            Dim reader As SqlDataReader = Nothing
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(thePage.DbConnection, SP_NAME_FETCH_INVOICEBATCH, False)
                spParams(0).Value = invoiceID
                reader = SqlHelper.ExecuteReader(thePage.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICEBATCH, spParams)
                While reader.Read
                    lblProvider.Text = String.Format("{0}:{1}", reader("ProviderReference"), reader("ProviderName"))
                    lblContract.Text = String.Format("{0}:{1}", reader("ContractNumber"), reader("ContractTitle"))
                    If Not IsDBNull(reader("ClientName")) Then
                        lblServiceUser.Text = String.Format("{0}:{1}", reader("ClientReference"), reader("ClientName"))
                    End If
                    If Not IsDBNull(reader("PeriodFrom")) Then lblPeriodFrom.Text = Convert.ToDateTime(reader("PeriodFrom")).ToString("dd MMM yyyy")
                    If Not IsDBNull(reader("PeriodTo")) Then lblPeriodTo.Text = Convert.ToDateTime(reader("PeriodTo")).ToString("dd MMM yyyy")
                    'If Not IsDBNull(reader("CalculatedPayment")) Then lblPayment.Text = Convert.ToDecimal(reader("CalculatedPayment")).ToString("c")
                    If Not IsDBNull(reader("DirectIncome")) Then lblDirectIncome.Text = Convert.ToDecimal(reader("DirectIncome")).ToString("c")
                    If Not IsDBNull(reader("PaymentRef")) Then lblPaymentRef.Text = reader("PaymentRef")
                    If Not IsDBNull(reader("PaymentClaimed")) Then lblClaimed.Text = Convert.ToDecimal(reader("PaymentClaimed")).ToString("c")
                    If Not IsDBNull(reader("QueryDescription")) Then
                        lblQuery.Text = reader("QueryDescription")
                    Else
                        lblQuery.Text = "&nbsp;"
                    End If
                    If Not IsDBNull(reader("QueryDate")) Then lblQueriedOn.Text = reader("QueryDate")
                    If Not IsDBNull(reader("QueryBy")) AndAlso reader("QueryBy").ToString().Length > 1 Then
                        lblQueriedBy.Text = reader("QueryBy")
                        lblOn.Visible = True
                    End If


                End While

            Catch ex As Exception
                Msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEBATCH, "ViewInvoiceLines.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(Msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try






            CType(pSchedule, Apps.UserControls.PaymentScheduleHeader).BoldLabels = False
            CType(pSchedule, Apps.UserControls.PaymentScheduleHeader).LabelWidth = "11.4em"
            CType(pSchedule, Apps.UserControls.PaymentScheduleHeader).SingleLiner = True
            CType(pSchedule, Apps.UserControls.PaymentScheduleHeader).SingleLineLabelText = "Payment Schedule"

        End Sub

        Private Function AddInvoiceLines(ByRef reader As System.Data.SqlClient.SqlDataReader) As Panel

            'If Not IsDBNull(reader("CalculatedPayment")) Then lblPayment.Text = Convert.ToDecimal(reader("CalculatedPayment")).ToString("c")
            Dim labelWidth As String = "11.8em"

            Dim pnlExtraControlPScheduleHeader As New Panel
            Dim litHeader As LiteralControl = New LiteralControl
            Dim litFooter As LiteralControl = New LiteralControl

            'header
            litHeader = New LiteralControl("<div style='margin-bottom: 5px;'>")
            pnlExtraControlPScheduleHeader.Controls.Add(litHeader)

            Dim lblPaymentRefLabel As New Label
            lblPaymentRefLabel.Text = "Reference"
            lblPaymentRefLabel.CssClass = "content"
            lblPaymentRefLabel.Font.Bold = True
            lblPaymentRefLabel.Width = System.Web.UI.WebControls.Unit.Parse(labelWidth)
            pnlExtraControlPScheduleHeader.Controls.Add(lblPaymentRefLabel)
            Dim lblPaymentRef As New Label
            If Not IsDBNull(reader("PaymentRef")) Then lblPaymentRef.Text = reader("PaymentRef")
            pnlExtraControlPScheduleHeader.Controls.Add(lblPaymentRef)
            ' footer
            litFooter = New LiteralControl("</div>")
            pnlExtraControlPScheduleHeader.Controls.Add(litFooter)

            litHeader = New LiteralControl("<div style='margin-bottom: 5px;'>")
            pnlExtraControlPScheduleHeader.Controls.Add(litHeader)

            Dim lblclaimedlabel As New Label
            lblclaimedlabel.Text = "Payment Claimed"
            lblclaimedlabel.CssClass = "content"
            lblclaimedlabel.Font.Bold = True
            lblclaimedlabel.Width = System.Web.UI.WebControls.Unit.Parse(labelWidth)
            pnlExtraControlPScheduleHeader.Controls.Add(lblclaimedlabel)
            Dim lblclaimed As New Label
            If Not IsDBNull(reader("paymentclaimed")) Then lblclaimed.Text = Convert.ToDecimal(reader("paymentclaimed")).ToString("c")
            pnlExtraControlPScheduleHeader.Controls.Add(lblclaimed)

            Dim lblDirectIncomeLabel As New Label
            lblDirectIncomeLabel.Text = "Direct Income"
            lblDirectIncomeLabel.CssClass = "content"
            lblDirectIncomeLabel.Font.Bold = True
            lblDirectIncomeLabel.Width = System.Web.UI.WebControls.Unit.Parse(labelWidth)
            pnlExtraControlPScheduleHeader.Controls.Add(lblDirectIncomeLabel)
            Dim lblDirectIncome As New Label
            If Not IsDBNull(reader("DirectIncome")) Then lblDirectIncome.Text = Convert.ToDecimal(reader("DirectIncome")).ToString("c")
            pnlExtraControlPScheduleHeader.Controls.Add(lblDirectIncome)
            'footer
            litFooter = New LiteralControl("</div>")
            pnlExtraControlPScheduleHeader.Controls.Add(litFooter)

            ' header
            litHeader = New LiteralControl("<div style='margin-bottom: 5px;'>")
            pnlExtraControlPScheduleHeader.Controls.Add(litHeader)
            Dim lblQueryLabel As New Label
            lblQueryLabel.Text = "Query"
            lblQueryLabel.Width = System.Web.UI.WebControls.Unit.Parse(labelWidth)
            pnlExtraControlPScheduleHeader.Controls.Add(lblQueryLabel)
            Dim lblQuery As New Label
            lblQuery.Text = Utils.ToString(reader("QueryDescription")).Trim()
            pnlExtraControlPScheduleHeader.Controls.Add(lblQuery)
            ' footer
            litFooter = New LiteralControl("</div>")
            pnlExtraControlPScheduleHeader.Controls.Add(litFooter)

            ' header
            litHeader = New LiteralControl("<div style='margin-bottom: 5px;'>")
            pnlExtraControlPScheduleHeader.Controls.Add(litHeader)
            ' Queried by / on
            Dim lblQueriedByOnLabel As New Label
            lblQueriedByOnLabel.Text = "Queried By/On"
            lblQueriedByOnLabel.CssClass = "content"
            lblQueriedByOnLabel.Font.Bold = True
            lblQueriedByOnLabel.Width = System.Web.UI.WebControls.Unit.Parse(labelWidth)
            pnlExtraControlPScheduleHeader.Controls.Add(lblQueriedByOnLabel)
            ' query by 
            Dim lblQueriedBy As New Label
            If Not IsDBNull(reader("QueryBy")) Then lblQueriedBy.Text = reader("QueryBy")
            pnlExtraControlPScheduleHeader.Controls.Add(lblQueriedBy)
            ' on
            Dim lblOn As New Label
            lblOn.Text = "On"
            pnlExtraControlPScheduleHeader.Controls.Add(lblOn)
            ' query date
            Dim lblQueriedOn As New Label
            If Not IsDBNull(reader("QueryDate")) Then lblQueriedOn.Text = reader("QueryDate")

            pnlExtraControlPScheduleHeader.Controls.Add(lblQueriedOn)
            ' footer
            litFooter = New LiteralControl("</div>")
            pnlExtraControlPScheduleHeader.Controls.Add(litFooter)


            Return pnlExtraControlPScheduleHeader
        End Function

    End Class

End Namespace