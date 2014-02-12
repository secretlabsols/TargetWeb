Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Library.Selectors
Imports Target.Library.JsonSerializerExtensions
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.DebtorInvoices

    ''' <summary>
    ''' Web Form used to List and Maintain Debtor Invoices
    ''' </summary>
    ''' <history>
    ''' Colin Daly     12/12/2012 D12307 - Updated - as part of code review removed unused code.
    ''' Paul Wheaver   06/08/2012 D12307 - Created
    ''' </history>
    Partial Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.DebtorInvoices"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Debtor Invoices")

            Dim resultSettings As New DebtorInvoiceSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "gsoResultSettings")
            End With

            ' add web service utils
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DebtorInvoice))

            ' add in page script
            JsLinks.Add("Lister.js")

        End Sub

#End Region

    End Class

End Namespace


