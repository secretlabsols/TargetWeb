
Namespace Apps.Reports.UserControls

    ''' <summary>
    ''' User control to encapsulate the listing of reports.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  26/10/2009  D11710 - created.
    ''' </history>
    Partial Public Class ReportSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/Reports/UserControls/ReportSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Reports.WebSvc.Reports))

            thePage.ClientScript.RegisterStartupScript( _
                Me.GetType(), _
                "Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format( _
                        "currentPage={0};", _
                        currentPage _
                    ) _
                ) _
            )

        End Sub

    End Class

End Namespace
