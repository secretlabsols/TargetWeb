Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Popup screen that allows a user to select a client.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Paul W      10/10/2011  D11945A - View Service Orders in Extranet.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class GenericServiceOrders
        Inherits Target.Web.Apps.BasePage

        Protected selector As Target.Abacus.Web.Apps.UserControls.ServiceOrderSelector

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Service Order")
            Me.JsLinks.Add("GenericServiceOrders.js")

            Dim serviceOrderID As Integer = 0
            Dim genericServiceOrderID As Integer = Utils.ToInt32(Request.QueryString("genericServiceOrderID"))
            Dim clientID As Integer = Utils.ToInt32(Request.QueryString("clientID"))

            selector.InitControl(Me, serviceOrderID, genericServiceOrderID, Nothing, Nothing, clientID, Nothing, Nothing, Nothing, False, False, False, False)

        End Sub

    End Class

End Namespace

