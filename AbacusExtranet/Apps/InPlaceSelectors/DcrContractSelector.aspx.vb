Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security

Namespace Apps.InPlaceSelectors

    Partial Public Class DcrContractSelector
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.EnableTimeout = False
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ReferenceData.DurationClaimRounding"), "Duration Claimed Rounding")
            Me.JsLinks.Add("DcrContractSelector.js")
            CType(DcrDomContractSelector1, UserControls.DcrDomContractSelector).InitControl(Me.Page, 1, 1, 1)


        End Sub

    End Class

End Namespace