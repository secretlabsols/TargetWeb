Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Library.Selectors
Imports Target.Library.JsonSerializerExtensions
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.CareHomePayments

    ''' <summary>
    ''' Web Form used to Care Home Payments
    ''' </summary>
    ''' <history>
    ''' ColinD  03/01/2013 D12396A - Updated to use correct WebNavMenuItem constant....was using service users. CR-40.
    ''' Waqas   03/01/2013 D12396A - Created
    ''' </history>
    ''' 
    Public Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.CareHomePayments"

#End Region

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Care Home Payments")

            Dim resultSettings As New CareHomePaymentSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "chResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")
           
        End Sub

    End Class

End Namespace