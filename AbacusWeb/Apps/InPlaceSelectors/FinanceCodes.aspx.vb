
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

	''' <summary>
    ''' Popup screen that allows a user to select a finance code.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD      13/06/2011  D12063 - Added support for filtering by finance code category.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class FinanceCodes
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Finance Code")
            Me.JsLinks.Add("FinanceCodes.js")

            Dim fcID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim categoryID As Integer = Utils.ToInt32(Request.QueryString("catId"))
            Dim selectedExpAccGroupID As Integer = Utils.ToInt32(Request.QueryString("expAccID"))

            selector.InitControl(Me, fcID, categoryID, selectedExpAccGroupID)

        End Sub

    End Class

End Namespace