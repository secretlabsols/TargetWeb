
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Popup screen that allows a user to select a care manager.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class CareManagers
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Care Manager")
            Me.JsLinks.Add("CareManagers.js")

            Dim cmID As Integer = Utils.ToInt32(Request.QueryString("id"))

            selector.InitControl(Me, cmID)

        End Sub

    End Class

End Namespace