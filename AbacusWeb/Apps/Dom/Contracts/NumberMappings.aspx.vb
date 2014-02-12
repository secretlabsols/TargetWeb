Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Wizard screen to search for and view provider contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD   16/05/2013 D12486 - Created
    ''' </history>
    Partial Public Class NumberMappings
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Payments.ReferenceData.ContractNumberMapping"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Contract Number Mappings")

            Dim resultSettings As New ContractNumberMappingSettings()

            ' setup the results settings
            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "resultsSettings")
            End With

            ' add in page script
            JsLinks.Add("NumberMappings.js")

        End Sub

#End Region

    End Class

End Namespace