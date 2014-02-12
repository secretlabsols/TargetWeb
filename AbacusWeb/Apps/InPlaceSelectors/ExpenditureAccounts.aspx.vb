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
    Partial Class ExpenditureAccounts
        Inherits Target.Web.Apps.BasePage

        'Protected selector As Target.Abacus.Web.Apps.UserControls.ExpenditureAccountSelector

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select an organization")
            Me.JsLinks.Add("ExpenditureAccounts.js")

            Dim AccountID As Integer = Utils.ToInt32(Request.QueryString("aID"))
            Dim serviceType As Integer = Utils.ToInt32(Request.QueryString("serviceType"))
            Dim accountType As Integer = Utils.ToInt32(Request.QueryString("accountType"))
            Dim description As String = Request.QueryString("description")

            selector.InitControl(Me, AccountID, serviceType, accountType, description)

        End Sub

    End Class

End Namespace