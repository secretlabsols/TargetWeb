
Partial Public Class PageAsEmail
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lnkHeaderLogo.ImageUrl = Target.Library.Web.Utils.GetVirtualPath("Library/Controls/MasterPages/Default/Images/HeaderLogo.gif")
        lnkHeaderLogo.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Default.aspx")
        lnkCannotView.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Apps/CMS/CMSGetPage.axd?id=") & Request.QueryString("id")
    End Sub

End Class