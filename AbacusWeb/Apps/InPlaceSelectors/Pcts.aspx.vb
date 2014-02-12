
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Popup screen that allows a user to select a PCT.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class Pcts
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Clinical Commissioning Group")
            Me.JsLinks.Add("Pcts.js")

            Dim pctID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim name As String = Request.QueryString("name")

            selector.InitControl(Me, pctID)

        End Sub

    End Class

End Namespace