Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' <summary>
    ''' Control to provide access to the different domiciliary contract reports.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' PaulW  30/10/2009  D11642 - Dom Manual Payment Invoice amendments.
    ''' </history>
    Partial Class ManualPaymentDomProformaInvoiceReports
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, _
                                ByVal establishmentID As Integer, _
                                ByVal contractID As Integer, _
                                ByVal invoiceID As Integer, _
                                ByVal systemAccountID As Integer)

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            With lstReports
                .Rows = 10
                .Attributes.Add("onchange", "lstReports_Change();")
                With .Items
                    .Add(New ListItem("Simple list of manual payment invoice", divManualInvoiceList.ClientID))
                    .Add(New ListItem("Simple list of manual payment invoice lines", divManualInvoiceLines.ClientID))

                End With
            End With

            ' simple manual payment invoice list
            With CType(ctlContractList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ManualPaymentInvoice")
                If establishmentID > 0 Then .Parameters.Add("intProviderID", establishmentID)
                If contractID > 0 Then .Parameters.Add("intDomContractID", contractID)
                If invoiceID > 0 Then .Parameters.Add("intInvoiceID", invoiceID)
                If systemAccountID > 0 Then .Parameters.Add("intSysAccountID", systemAccountID)
            End With

            ' simple manual payment invoice lines list
            With CType(ctlUnitCosts, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ManualPaymentInvoiceLines")
                If establishmentID > 0 Then .Parameters.Add("intProviderID", establishmentID)
                If contractID > 0 Then .Parameters.Add("intDomContractID", contractID)
                If invoiceID > 0 Then .Parameters.Add("intInvoiceID", invoiceID)
                If systemAccountID > 0 Then .Parameters.Add("intSysAccountID", systemAccountID)
            End With

        End Sub

    End Class

End Namespace

