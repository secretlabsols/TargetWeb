
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

	''' <summary>
    ''' Popup screen that allows a user to select a client sub group.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     JohnF      14/05/2013    Initial version (D12479)
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class ClientSubGroups
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Client Sub Group")
            Me.JsLinks.Add("ClientSubGroups.js")

            Dim clientSubGroupID As Integer = Utils.ToInt32(Request.QueryString("id"))

            selector.InitControl(Me, clientSubGroupID)

        End Sub

    End Class

End Namespace