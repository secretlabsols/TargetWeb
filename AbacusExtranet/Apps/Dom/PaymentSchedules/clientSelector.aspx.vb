Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Abacus.Library

Namespace Apps.Dom.PaymentSchedules

    Partial Public Class clientSelector
        Inherits Target.Web.Apps.BasePage

        Private _pScheduleId As Integer
        Private _estabId As Integer
        Private _contractId As Integer
        Private _clientId As Integer
        Private _viewClientBaseUrl As String
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _pSWE As String

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Manually Entered Visits")

            Me.JsLinks.Add("clientSelector.js")

            _estabId = Utils.ToInt32(Request.QueryString("estabid"))
            _contractId = Utils.ToInt32(Request.QueryString("contractid"))
            _clientId = Utils.ToInt32(Request.QueryString("clientid"))
            _pScheduleId = Utils.ToInt32(Request.QueryString("pScheduleId"))
            _pSWE = Utils.ToString(Request.QueryString("pSWE"))
            clientSelectorWithPaging.showServiceOrderWithValidPeriod = True


            clientSelectorWithPaging.InitControl(Me.Page, _
                                                 _estabId, _
                                                 _contractId, _
                                                 _clientId, _
                                                 _dateFrom, _
                                                 _dateTo, _
                                                 _viewClientBaseUrl, _
                                                 Apps.UserControls.ClientStepMode.ClientsWithDomSvcOrders, _
                                                 True)
            IntialiseJsVariables()
        End Sub

        Private Sub IntialiseJsVariables()

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.clientSelector.Startup", _
              Target.Library.Web.Utils.WrapClientScript(String.Format("estabid={0};" & _
                                                                      "contractId={1};" & _
                                                                      "clientId={2};" & _
                                                                      "invoiceid={3};" & _
                                                                      "mode={4};" & _
                                                                      "pScheduleId={5};" & _
                                                                      "pSWE=""{6}"";", _
                                                                      _estabId, _
                                                                      _contractId, _
                                                                      _clientId, _
                                                                      0, _
                                                                      2, _
                                                                      _pScheduleId, _
                                                                      _pSWE _
                                                                      )) _
                                                                  )

        End Sub


    End Class


End Namespace