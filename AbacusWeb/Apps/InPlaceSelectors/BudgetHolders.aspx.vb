
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Popup screen that allows a user to select a budget holder.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     JohnF   28/07/2010   Created (D11801)
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class BudgetHolders
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Budget Holder")
            Me.JsLinks.Add("BudgetHolders.js")

            Dim bhID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim redundant As Nullable(Of Boolean) = Utils.ToBoolean(Request.QueryString("redundant"))
            Dim serviceUserID As Nullable(Of Integer) = Utils.ToInt32(Request.QueryString("suid"))

            selector.InitControl(Me, bhID, redundant, serviceUserID)

        End Sub

    End Class

End Namespace