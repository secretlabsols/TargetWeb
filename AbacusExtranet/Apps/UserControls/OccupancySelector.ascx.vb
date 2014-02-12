Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils
Namespace Apps.UserControls

    Partial Class OccupancySelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal establishmentID As Integer, _
                                ByVal dateFrom As Date, ByVal dateTo As Date, ByVal movement As Int16)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add table sorting JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))
            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/OccupancySelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.Occupancy))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.UserControls.OccupancySelector.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("currentPage={0};establishmentID={1};dateFrom={2};dateTo={3};movement={4};Init();", _
                        currentPage, establishmentID, _
                        IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
                        IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
                        movement _
                    ) _
                ) _
            )

        End Sub

    End Class

End Namespace
