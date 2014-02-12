
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library

Namespace Apps.UserControls

    ''' <summary>
    ''' User control to encapsulate the display of provider invoice header details on sub-screens of the DPI wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF   17/12/2009  Added Invoice Total to fields (#5966)
    ''' MikeVO  19/10/2009  D11546 - created.
    ''' </history>
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
            Dim dateFrom As Date
            Dim dateTo As Date

            CType(pScheduleHeader, Apps.UserControls.PaymentScheduleHeader).SingleLiner = True
            CType(pScheduleHeader, Apps.UserControls.PaymentScheduleHeader).LabelWidth = "11em"

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

                    '' get datefrom and date according to the contract type
                    msg = New ErrorMessage
                    msg = PeriodicContractDates(InvoiceID, thePage, dateFrom, dateTo)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

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

        Private Function PeriodicContractDates(invoiceId As Integer, thePage As BasePage, ByRef dateFrom As Date, ByRef dateTo As Date) As ErrorMessage
            Dim msg As New ErrorMessage
            Dim invoice As DataClasses.DomProviderInvoice = New DataClasses.DomProviderInvoice(thePage.DbConnection)
            msg = invoice.Fetch(invoiceId)
            If Not msg.Success Then Return msg

            Dim pSchedule As DataClasses.PaymentSchedule = New DataClasses.PaymentSchedule(thePage.DbConnection, String.Empty, String.Empty)
            msg = pSchedule.Fetch(invoice.PaymentScheduleID)
            If Not msg.Success Then Return msg

            Dim contract As DataClasses.DomContract = New DataClasses.DomContract(thePage.DbConnection, String.Empty, String.Empty)
            msg = contract.Fetch(pSchedule.DomContractID)
            If Not msg.Success Then Return msg

            If contract.ContractType = [Enum].GetName(GetType(DomContractType), DomContractType.BlockPeriodic) Then
                lblInvoicePeriod.Text = String.Format( _
                       "{0}&nbsp;To {1}", _
                       invoice.ServiceFromDate.ToString("dd/MM/yyyy"), _
                       invoice.ServiceToDate.ToString("dd/MM/yyyy") _
                   )

                lblForServiceUser.Text = "System Account"
            Else
                lblInvoicePeriod.Text = String.Format( _
                         "{0}&nbsp;To {1}", _
                         DomContractBL.GetWeekCommencingDate(thePage.DbConnection, Nothing, Convert.ToDateTime(invoice.WEFrom)).ToString("dd/MM/yyyy"), _
                         Convert.ToDateTime(invoice.WETo).ToString("dd/MM/yyyy") _
                     )
            End If

            Return msg
        End Function
    End Class

End Namespace