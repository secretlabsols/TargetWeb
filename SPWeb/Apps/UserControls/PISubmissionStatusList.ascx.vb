Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Partial Class PISubmissionStatusList
    Inherits System.Web.UI.UserControl

    Public Sub InitControl(ByVal thePage As BasePage, ByVal providerID As Integer, ByVal serviceID As Integer, _
                                ByVal financialYearFrom As String, ByVal quarterFrom As String, _
                                ByVal financialYearTo As String, ByVal quarterTo As String, ByVal status As String)

        Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
        If currentPage <= 0 Then currentPage = 1

        ' add table sorting JS
        thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))
        ' add date utility JS
        thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
        ' add utility JS link
        thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
        ' add dialog JS link
        thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/dialog.js"))
        ' add page JS
        thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/PISubmissionStatusList.js"))
        ' add AJAX-generated javascript to the page
        AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.SP.Web.Apps.WebSvc.PIReturns))

        thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.PISubmissionStatusList.Startup", _
            Target.Library.Web.Utils.WrapClientScript( _
                String.Format("currentPage={0};providerID={1};serviceID={2};financialYearFrom=""{3}"";quarterFrom=""{4}"";financialYearTo=""{5}"";quarterTo=""{6}"";status=""{7}"";Init();", _
                    currentPage, providerID, serviceID, financialYearFrom, quarterFrom, financialYearTo, quarterTo, status) _
            ) _
        )

    End Sub

End Class
