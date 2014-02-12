Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security


Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Popup screen that allows a user to select an External Account.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Waqas      23/02/2011  Initial version.
    ''' </history>
    Partial Public Class ExternalAccounts
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.EnableTimeout = False
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ReferenceData.DurationClaimRounding"), "Duration Claimed Rounding")
            Me.JsLinks.Add("ExternalAccounts.js")
            CType(ExternalAccount1, ExternalAccountList).InitControl(Me.Page)
        End Sub

    End Class

End Namespace