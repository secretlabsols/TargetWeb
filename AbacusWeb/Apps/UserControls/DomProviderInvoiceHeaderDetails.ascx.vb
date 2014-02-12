Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library
Imports Target.Abacus.Web

Partial Public Class DomProviderInvoiceHeaderDetails
    Inherits System.Web.UI.UserControl
    Private _invoiceID As Integer

    Public Property InvoiceID() As Integer
        Get
            Return _invoiceID
        End Get
        Set(ByVal value As Integer)
            _invoiceID = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'CType(pScheduleHeader, Apps.UserControls.PaymentScheduleHeader).SingleLiner = True

        Const SP_NAME_FETCH_INVOICE_FOR_VIEW As String = "spxDomProviderInvoice_FetchForView"

        Dim ds As DataSet
        Dim msg As ErrorMessage
        Dim thePage As BasePage = CType(Me.Page, BasePage)

        Try
            Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(thePage.DbConnection, SP_NAME_FETCH_INVOICE_FOR_VIEW, False)
            spParams(0).Value = InvoiceID
            ds = SqlHelper.ExecuteDataset(thePage.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICE_FOR_VIEW, spParams)

            With ds.Tables(0).Rows(0)
                lblProvider.Text = String.Format("{0}:{1}", .Item("ProviderReference"), .Item("ProviderName"))
                lblContract.Text = String.Format("{0}:{1}", .Item("ContractNumber"), .Item("ContractTitle"))
                If Not IsDBNull(.Item("ClientName")) Then
                    lblServiceUser.Text = String.Format("{0}:{1}", .Item("ClientReference"), .Item("ClientName"))
                End If
                lblInvoiceNumber.Text = .Item("InvoiceNumber")
                lblInvoicePeriod.Text = String.Format( _
                       "{0}&nbsp;<strong>To</strong> {1}", _
                       DomContractBL.GetWeekCommencingDate(thePage.DbConnection, Nothing, Convert.ToDateTime(.Item("WEFrom"))).ToString("dd/MM/yyyy"), _
                       Convert.ToDateTime(.Item("WETo")).ToString("dd/MM/yyyy") _
                   )
                lblInvoiceStatus.Text = String.Format( _
                    "{0} on {1} at {2}", _
                    [Enum].GetName(GetType(DomProviderInvoiceStatus), .Item("Status")), _
                    Convert.ToDateTime(.Item("StatusDate")).ToString("dd/MM/yyyy"), _
                    Convert.ToDateTime(.Item("StatusDate")).ToString("HH:mm:ss") _
                )
                lblInvoiceTotal.Text = Utils.ToDecimal(.Item("InvoiceTotal")).ToString("c")
                If Not IsDBNull(.Item("PaymentRef")) Then lblReference.Text = .Item("PaymentRef")
                If Not IsDBNull(.Item("DirectIncome")) Then lblDirectIncome.Text = _
                Utils.ToDecimal(.Item("DirectIncome")).ToString("c")


            End With

        Catch ex As Exception
            msg = Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICE_FOR_VIEW, "ViewInvoiceLines.Page_Load")   ' could not read data
            WebUtils.DisplayError(msg)
        End Try


    End Sub

End Class