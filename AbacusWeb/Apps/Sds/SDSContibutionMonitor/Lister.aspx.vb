Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Library.Selectors
Imports Target.Library.JsonSerializerExtensions
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.Dom.SDSContibutionMonitor

    ''' <summary>
    ''' Web Form used to List SDS Contribution Monitor
    ''' </summary>
    ''' <history>
    ''' MoTahir   D12306 SDS Contribution Monitor Wizard Redevelopment Created 21/08/2012 - Created.
    ''' </history>
    Partial Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Income.Contributions.SdsContributionMonitor"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "SDS Contribution Monitor")

            Dim resultSettings As New SDSContributionMonitorSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "SDSContributionMonitorResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")

        End Sub

#End Region

    End Class

End Namespace


