
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Popup screen that allows a user to select an establishment.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class Establishments
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Provider")
            Me.JsLinks.Add("Establishments.js")

            Dim mode As EstablishmentSelectorMode = Convert.ToInt32(Request.QueryString("mode"))
            Dim estabID As Integer = Utils.ToInt32(Request.QueryString("estabID"))
            Dim reference As String = Request.QueryString("ref")
            Dim name As String = Request.QueryString("name")
            Dim redundant As Boolean = Request.QueryString("redundant")

            selector.InitControl(Me, estabID, mode, redundant)

        End Sub

    End Class

End Namespace