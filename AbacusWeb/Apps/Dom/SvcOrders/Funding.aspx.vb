Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls

Namespace Apps.Dom.SvcOrders

    ''' <summary>
    ''' Main container screen used to maintain domiciliary contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Paul  16/02/2009  D11491 - Initial version.
    ''' </history>
    Partial Class Funding
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrderFunding"), "Service Order Funding")

            Dim serviceOrderID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim builder As Target.Library.Web.UriBuilder = New Target.Library.Web.UriBuilder(Request.Url)
            builder.QueryItems.Remove("backUrl")

            frmDSODetails.Attributes.Add("src", String.Format("svcOrderHeader.aspx{0}", builder.Query))
            frmTree.Attributes.Add("src", String.Format("FundingTree.aspx{0}&selectType=o", builder.Query))
            'frmContent.Attributes.Add("src", String.Format("Header.aspx{0}&mode={1}", builder.Query, Convert.ToInt32(IIf(serviceOrderID = 0, StdButtonsMode.AddNew, StdButtonsMode.Fetched))))

            Me.JsLinks.Add("Funding.js")

        End Sub

    End Class

End Namespace
