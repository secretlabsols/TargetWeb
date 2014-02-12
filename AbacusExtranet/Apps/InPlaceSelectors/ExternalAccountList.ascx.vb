Imports System.Text
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.InPlaceSelectors

    Partial Public Class ExternalAccountList
        Inherits System.Web.UI.UserControl


        Public Sub InitControl(ByVal thePage As BasePage)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            Dim js As New StringBuilder()
            Dim selectedId As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))

            If currentPage <= 0 Then currentPage = 1

            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/InPlaceSelectors/ExternalAccountList.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DurationClaimedRounding))

            js.AppendFormat("currentPage={0};", currentPage)
            js.AppendFormat("selectedExternalAccountID={0};", selectedId)

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

    End Class

End Namespace