
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

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

        Protected selector As Target.Abacus.Web.Apps.UserControls.ClientSelector

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' get query string params
            Dim hideDebtorRef As Nullable(Of Boolean) = Utils.ToBoolean(Request.QueryString("hideDebtorRef"))
            Dim hideCreditorRef As Nullable(Of Boolean) = Utils.ToBoolean(Request.QueryString("hideCreditorRef"))
            Dim clientID As Integer = Utils.ToInt32(Request.QueryString("clientID"))
            Dim reference As String = Utils.ToString(Request.QueryString("ref"))
            Dim name As String = Utils.ToString(Request.QueryString("name"))

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Service User")
            Me.JsLinks.Add("Clients.js")

            If hideDebtorRef.HasValue Then
                ' if we have specified whether to hide debtor ref then set property

                selector.HideDebtorRef = hideDebtorRef.Value

            Else
                ' else always show it

                selector.HideDebtorRef = False

            End If

            If hideCreditorRef.HasValue Then
                ' if we have specified whether to hide creditor ref then set property

                selector.HideCreditorRef = hideCreditorRef.Value

            Else
                ' else always show it

                selector.HideCreditorRef = False

            End If

            selector.InitControl(Me, clientID)

        End Sub

    End Class

End Namespace