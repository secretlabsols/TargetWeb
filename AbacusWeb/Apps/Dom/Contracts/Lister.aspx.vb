Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports System.Collections.Generic
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Library.Selectors
Imports Target.Library.JsonSerializerExtensions
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Wizard screen to search for and view provider contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Waqas  02/08/2012  D12309 - created.
    ''' </history>
    Partial Public Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"

#End Region

#Region " events"

        ''' <summary>
        ''' Handles the load event of the page control
        ''' </summary>
        ''' <param name="sender">the source of event</param>
        ''' <param name="e">The instance containing the event data.</param>
        ''' <remarks></remarks>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Provider Contracts")

            Dim resultSettings As New ProviderContractSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "pcResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Date.js"))

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
        End Sub
#End Region

    End Class



End Namespace