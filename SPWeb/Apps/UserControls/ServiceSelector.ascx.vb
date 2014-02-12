Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.ServiceSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of Services
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO  14/12/2006  Removed table sorting.
    '''                         Added custom table filter.
    ''' 	[paul]	02/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ServiceSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal allowViewService As Boolean, ByVal ProviderID As Integer, ByVal selectedServiceID As Integer)

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
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
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/ServiceSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.SP.web.Apps.WebSvc.Services))

            btnViewService.Visible = allowViewService

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.ServiceSelector.Startup", Target.Library.Web.Utils.WrapClientScript( _
                String.Format("allowViewService={0};currentPage={1};ProviderID={2};selectedServiceID={3};Init();", _
                    allowViewService.ToString().ToLower(), currentPage, ProviderID, selectedServiceID)))
        End Sub

    End Class

End Namespace
