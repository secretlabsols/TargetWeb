Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Popup screen that allows a user to select a client.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Paul      06/02/2009  D11491 - Initial Version.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class ThirdParty
        Inherits Target.Web.Apps.BasePage

        Protected selectorTP As Target.Abacus.Web.Apps.UserControls.ThirdPartySelector

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Service User")
            Me.JsLinks.Add("ThirdPartys.js")

            Dim clientID As Integer = Utils.ToInt32(Request.QueryString("clientID"))
            Dim thirdPartyID As Integer = Utils.ToInt32(Request.QueryString("thirdPartyID"))
            Dim surname As String = Request.QueryString("name")

            selectorTP.InitControl(Me, thirdPartyID, clientID)

        End Sub

    End Class

End Namespace