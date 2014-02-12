Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace MovementRequests

    ''' <summary>
    ''' Web Form used to List Movement Requests
    ''' </summary>
    ''' <history>
    ''' ColinD   27/02/2013 D12396 - Created
    ''' </history>
    Partial Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Commitments.Administration.MovementRequestMonitor"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Movement Request Monitor")

            Dim resultSettings As New MovementRequestSettings()

            ' setup the results settings
            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "mrResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")

        End Sub

#End Region

    End Class

End Namespace