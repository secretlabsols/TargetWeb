
Imports Target.Library
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Web.Apps.Navigation

Namespace Apps.CMS.Controls

    Partial Class PageAsEmailTemplate
        Inherits System.Web.UI.UserControl

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            lnkHeaderLogo.ImageUrl = Target.Library.Web.Utils.GetVirtualPath("Library/Controls/MasterPages/Default/Images/HeaderLogo.gif")
            lnkHeaderLogo.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Default.aspx")

            lnkCannotView.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Apps/CMS/CMSGetPage.axd?id=") & Request.QueryString("id")

        End Sub

    End Class

End Namespace