
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' <summary>
    ''' Control to provide access to the different domiciliary service order reports.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  23/09/2009  D11701 - added support for order-level reporting.
    ''' MikeVO  14/07/2009  D11630 - added unpaid DSO report.
    ''' </history>
    Partial Class DomServiceOrderReports
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, _
                                ByVal establishmentID As Integer, _
                                ByVal contractID As Integer, _
                                ByVal clientID As Integer, _
                                ByVal dateFrom As Date, _
                                ByVal dateTo As Date, _
                                ByVal dsoID As Integer)

            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            With lstReports
                .Rows = 10
                .Attributes.Add("onchange", "lstReports_Change();")
                With .Items
                    .Add(New ListItem("Simple list of service orders", divDsoList.ClientID))
                    .Add(New ListItem("Simple list of service order details", divDsoDetailList.ClientID))
                    .Add(New ListItem("Simple list of service order visits", divDsoVisitList.ClientID))
                    .Add(New ListItem("Simple list of unpaid service orders", divUnpaidDsoList.ClientID))
                End With
            End With

            ' dso list
            With CType(ctlDsoList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DomiciliaryServiceOrders")
                If establishmentID > 0 Then .Parameters.Add("intProviderID", establishmentID)
                If contractID > 0 Then .Parameters.Add("intDomContractID", contractID)
                If clientID > 0 Then .Parameters.Add("intClientID", clientID)
                If Utils.IsDate(dateFrom) Then .Parameters.Add("dteDateFrom", dateFrom)
                If Utils.IsDate(dateTo) Then .Parameters.Add("dteDateTo", dateTo)
                If dsoID > 0 Then .Parameters.Add("intDsoID", dsoID)
            End With

            ' DSO detail list
            With CType(ctlDsoDetailList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DomiciliaryServiceOrderDetails")
                If establishmentID > 0 Then .Parameters.Add("intProviderID", establishmentID)
                If contractID > 0 Then .Parameters.Add("intDomContractID", contractID)
                If clientID > 0 Then .Parameters.Add("intClientID", clientID)
                If Utils.IsDate(dateFrom) Then .Parameters.Add("dteDateFrom", dateFrom)
                If Utils.IsDate(dateTo) Then .Parameters.Add("dteDateTo", dateTo)
                If dsoID > 0 Then .Parameters.Add("intDsoID", dsoID)
            End With

            ' DSO visit list
            With CType(ctlDsoVisitList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DomiciliaryServiceOrderVisits")
                If establishmentID > 0 Then .Parameters.Add("intProviderID", establishmentID)
                If contractID > 0 Then .Parameters.Add("intDomContractID", contractID)
                If clientID > 0 Then .Parameters.Add("intClientID", clientID)
                If Utils.IsDate(dateFrom) Then .Parameters.Add("dteDateFrom", dateFrom)
                If Utils.IsDate(dateTo) Then .Parameters.Add("dteDateTo", dateTo)
                If dsoID > 0 Then .Parameters.Add("intDsoID", dsoID)
            End With

            ' unpaid dso list
            With CType(ctlUnpaidDsoList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.UnpaidDomiciliaryServiceOrders")
                If establishmentID > 0 Then .Parameters.Add("intProviderID", establishmentID)
                If contractID > 0 Then .Parameters.Add("intDomContractID", contractID)
                If clientID > 0 Then .Parameters.Add("intClientID", clientID)
                If Utils.IsDate(dateFrom) Then .Parameters.Add("dteDsoDateFrom", dateFrom)
                If Utils.IsDate(dateTo) Then .Parameters.Add("dteDsoDateTo", dateTo)
                .Parameters.Add("blnUseDomProviderInvoices", False)
                If dsoID > 0 Then .Parameters.Add("intDsoID", dsoID)
            End With

        End Sub

    End Class

End Namespace

