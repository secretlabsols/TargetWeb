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

Namespace Apps.Res.Payments

    ''' <summary>
    ''' Wizard screen to search for and view residential payments.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF    05/11/2013  Initial version (D12526L)
    ''' </history>
    Partial Public Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusExtranet.WebNavMenuItem.PaymentEnquiry"

#End Region

#Region " Events "

        ''' <summary>
        ''' Handles the load event of the page control
        ''' </summary>
        ''' <param name="sender">the source of event</param>
        ''' <param name="e">The instance containing the event data.</param>
        ''' <remarks></remarks>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            '++ Setup the page..
            Me.InitPage(ConstantsManager.GetConstant(_WebNavMenuItemKey), "Residential Payment Enquiry")

            Dim resultSettings As New ResidentialPaymentSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "rpResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Date.js"))

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.ResPayments))
        End Sub

#End Region

    End Class

End Namespace