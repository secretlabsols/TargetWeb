Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Library.Selectors
Imports Target.Library.JsonSerializerExtensions
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web



Namespace Apps.Payments.ReferenceData.VerificationText



Public Class lister
        Inherits BasePage


#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Payments.ReferenceDate.VerificationText"

#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Verification Text")

            Dim resultSettings As New VerificationTextSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "vtResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")
    End Sub

    End Class

End Namespace