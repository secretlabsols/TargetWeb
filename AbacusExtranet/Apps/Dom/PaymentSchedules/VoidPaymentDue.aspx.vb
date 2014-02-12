Imports Target.Abacus.Library.PaymentSchedules
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library.DataClasses

Namespace Apps.Dom.PaymentSchedules

    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class VoidPaymentDue
        Inherits Target.Web.Apps.BasePage

        Private _pScheduleId As Integer = 0
        Public Property pScheduleId() As Integer
            Get
                Return _pScheduleId
            End Get
            Set(ByVal value As Integer)
                _pScheduleId = value
            End Set
        End Property


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.EnableTimeout = False

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Payment Schedules")

            Me.JsLinks.Add("VoidPaymentDue.js")

            If Not Request.QueryString("psid") Is Nothing Then
                pScheduleId = Utils.ToInt32(Request.QueryString("psid"))
            End If

            PopulateExplaination(pScheduleId)
        End Sub

        Private Sub PopulateExplaination(pScheduleId As Integer)
            Dim msg As New ErrorMessage()
            Dim voiddue As Decimal = 0
            Dim sumProformaandProvider As Decimal = 0
            Dim voidDetail As New VoidPaymentdetail()
            msg = VoidPaymentsBL.GetVoidPaymentDetails(Me.DbConnection, pScheduleId, voidDetail)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            lblPendingPayments.Text = getCurrency(voidDetail.ProformaNonVoidSum)
            lblPaymentsMade.Text = getCurrency(voidDetail.providerNonVoidSum)
            lblVoidalreadyMade.Text = getCurrency(voidDetail.providerVoidSum)
            sumProformaandProvider = voidDetail.ProformaNonVoidSum + _
                                                      voidDetail.providerNonVoidSum + _
                                                      voidDetail.providerVoidSum

            lblProformaProviderSum.Text = getCurrency(sumProformaandProvider)

            lblVoidPaymentDue.Text = getCurrency(voidDetail.VoidPaymentDue)

            lblResult.Text = getCurrency(sumProformaandProvider + voidDetail.VoidPaymentDue)

        End Sub

        Private Function getCurrency(value As Decimal) As String
            Return value.ToString("C")
        End Function


    End Class

End Namespace