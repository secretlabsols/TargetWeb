
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.ClientSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of clients.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      14/12/2006  Removed table sorting.
    '''                             Added custom table filter.
    ''' 	[Mikevo]	10/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ClientSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal serviceID As Integer, ByVal selectedClientID As Integer)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/ClientSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.SP.Web.Apps.WebSvc.Clients))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.ClientSelector.Startup", _
                Target.Library.Web.Utils.WrapClientScript(String.Format("currentPage={0};selectedClientID={1};Init();", currentPage, selectedClientID)))

        End Sub

    End Class

End Namespace

