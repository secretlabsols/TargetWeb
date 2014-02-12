Imports Target.Library

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Popup screen that allows a user to select a client.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class Client
        Inherits Target.Web.Apps.BasePage

        Protected selector As Target.Abacus.Extranet.Apps.UserControls.ClientSelector

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Service User")
            Me.JsLinks.Add("Clients.js")

            Dim clientID As Integer = Utils.ToInt32(Request.QueryString("clientID"))
            Dim reference As String = Request.QueryString("ref")
            Dim name As String = Request.QueryString("name")
            Dim clientMode As Integer = Request.QueryString("clientMode")

            selector.InitControl(Me, clientID, UserControls.ClientStepMode.ClientsWithVisitBasedDomProviderInvoices, Nothing, True)

        End Sub

    End Class

End Namespace