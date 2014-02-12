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

Namespace Apps.CreditorPayments

    ''' <summary>
    ''' Wizard screen to search for and view invoiced visits.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF  29/08/2012  D12312 - created.
    ''' </history>
    Partial Public Class InvoicedVisitLister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.InvoicedVisits"

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
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Invoiced Visits")

            Dim resultSettings As New InvoicedVisitSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "ivResultSettings")
            End With

            ' add in page script
            JsLinks.Add("InvoicedVisitLister.js")

        End Sub
#End Region

    End Class



End Namespace