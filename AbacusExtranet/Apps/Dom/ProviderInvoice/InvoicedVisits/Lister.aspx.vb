Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Web Form used to List and Invoiced visits
    ''' </summary>
    ''' <history>
    ''' Paul Wheaver   22/11/2013 D12526J - Created
    ''' </history>
    Partial Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusExtranet.WebNavMenuItem.InvoicedVisitEnquiry"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Invoiced Visits")

            Dim resultSettings As New InvoicedVisitSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "psResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")

            'Me.CustomNavAdd(True)

        End Sub

#End Region

    End Class

End Namespace