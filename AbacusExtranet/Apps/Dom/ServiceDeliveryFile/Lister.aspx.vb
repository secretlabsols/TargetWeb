Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.Dom.ServiceDeliveryFile

    ''' <summary>
    ''' Web Form used to List and Maintain Service Delivery files
    ''' </summary>
    ''' <history>
    ''' Paul Wheaver   14/11/2013 D12526F - Created
    ''' </history>
    Partial Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusExtranet.WebNavMenuItem.ServiceDeliveryFileEnquiry"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Service Delivery Files")

            Dim resultSettings As New ServiceDeliveryFileSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "psResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")

        End Sub

#End Region

    End Class

End Namespace