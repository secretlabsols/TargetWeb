
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Popup screen that allows a user to select a service group.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir       05/11/2012 D12343 - remove Framework Type from Service Group
    '''     ColinD        13/10/2010 D11918 - added ability to filter by non blank framework types
    '''     Mo Tahir      13/08/2009.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class ServiceGroups
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Service Group")
            Me.JsLinks.Add("ServiceGroups.js")

            Dim fcID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim filterByUser As Boolean = Request.QueryString("fUser")

            selector.InitControl(Me, fcID, filterByUser)

        End Sub

    End Class

End Namespace
