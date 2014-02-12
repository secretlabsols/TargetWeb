Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Library.Selectors
Imports Target.Library.JsonSerializerExtensions
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.CommissionedService

    ''' <summary>
    ''' Web Form used to list Service Order Suspensions
    ''' </summary>
    ''' <history>
    ''' JohnF   D12304 Created 30/07/2012 - Created.
    ''' </history>
    Partial Class ServiceOrderSuspensionLister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.ServiceOrderSuspensions"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Service Order Suspensions")

            Dim resultSettings As New ServiceOrderSuspensionSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "sosResultSettings")
            End With

            ' add in page script
            JsLinks.Add("ServiceOrderSuspensionLister.js")

        End Sub

#End Region

    End Class

End Namespace


