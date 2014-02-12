Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Popup screen that allows a user to select an organisation.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Paul      12/02/2009  Initial version.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class OtherFundingOrganizations
        Inherits Target.Web.Apps.BasePage

        Protected selector As Target.Abacus.Web.Apps.UserControls.OtherFundingOrgSelector

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select an organization")
            Me.JsLinks.Add("OtherFundingOrganisations.js")

            Dim orgID As Integer = Utils.ToInt32(Request.QueryString("orgID"))
            Dim orgType As Integer = Request.QueryString("orgType")
            Dim name As String = Request.QueryString("name")

            selector.InitControl(Me, orgID, orgType)

        End Sub

    End Class

End Namespace