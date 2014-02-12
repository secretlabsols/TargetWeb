Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.OccupancyEnqList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Control to list the occupancy details of a service and property.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO  21/03/2007  Support for list sorting.
    '''     MikeVO  14/12/2006  Removed table sorting.
    '''                         Added custom table filter.
    ''' 	[Paul]	??/??/??	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
Partial Class OccupancyEnqList
    Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal PropertyID As Integer, ByVal ServiceID As Integer, ByVal DateFrom As Date, ByVal DateTo As Date, ByVal Status As Integer)
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
            ' add list sorter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListSorter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/OccupancyEnqList.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.SP.web.Apps.WebSvc.OccupancyEnquiry))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.OccupancyEnqList.Startup", Target.Library.Web.Utils.WrapClientScript( _
                String.Format("currentPage={0};ServiceID={1};PropertyID={2};DateFrom={3};DateTo={4};Status={5};Init();", _
                    currentPage, ServiceID, PropertyID, WebUtils.GetDateAsJavascriptString(DateFrom), WebUtils.GetDateAsJavascriptString(DateTo), Status)))
        End Sub

End Class

End Namespace