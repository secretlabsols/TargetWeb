Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Library.Selectors
Imports Target.Library.JsonSerializerExtensions
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.Sds

    ''' <summary>
    ''' Web Form used to list Indicative Budgets
    ''' </summary>
    ''' <history>
    ''' JohnF   25/07/2012  D12301 - Created
    ''' </history>
    Partial Class IndicativeBudgetLister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.IndicativeBudgetEnquiry"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Indicative Budgets")

            Dim resultSettings As New IndicativeBudgetSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "ibResultSettings")
            End With

            ' add in page script
            JsLinks.Add("IndicativeBudgetLister.js")

        End Sub

#End Region

    End Class

End Namespace


