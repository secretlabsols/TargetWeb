
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Popup screen that allows a user to select a team.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class Teams
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Team")
            Me.JsLinks.Add("Teams.js")

            Dim teamID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim availableToRes As TriState = [Enum].Parse(GetType(TriState), Request.QueryString("availableToRes"))
            Dim availableToDom As TriState = [Enum].Parse(GetType(TriState), Request.QueryString("availableToDom"))

            selector.InitControl(Me, teamID, availableToRes, availableToDom)

        End Sub

    End Class

End Namespace