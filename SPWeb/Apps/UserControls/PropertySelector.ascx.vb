Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls


Partial Class PropertySelector
    Inherits System.Web.UI.UserControl

    Public Sub init_Control(ByVal ServiceID As Int32)
        Dim thePage As BasePage = DirectCast(Me.Parent.Page, BasePage)
        Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
        Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
        If currentPage <= 0 Then currentPage = 1

        ' add table sorting JS
        thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))
        ' add date utility JS
        thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
        ' add utility JS link
        thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
        ' add page JS
        thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/PropertySelector.js"))
        ' add AJAX-generated javascript to the page
        AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.SP.web.Apps.WebSvc.Properties))

        thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.PropertySelector.Startup", Target.Library.Web.Utils.WrapClientScript( _
            String.Format("webSecurityCompanyID={0};currentPage={1};ServiceID={2};Init();", _
                user.WebSecurityCompanyID, currentPage, ServiceID)))
    End Sub

End Class

End Namespace