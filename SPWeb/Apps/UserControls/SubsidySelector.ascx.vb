
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SubsidySelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO  19/03/2007  Support for list sorting.
    '''     MikeVO  14/12/2006  Removed table sorting.
    '''                         Added custom table filter.
    ''' 	[?]	    ??/??/??	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class SubsidySelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal providerID As Integer, ByVal serviceID As Integer, _
                                ByVal clientID As Integer, ByVal dateFrom As Date, ByVal dateTo As Date, ByVal status As Integer)

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
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/SubsidySelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.SP.Web.Apps.WebSvc.Subsidies))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SubsidySelector.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("currentPage={0};providerID={1};serviceID={2};dateFrom={3};dateTo={4};clientID={5};status={6};Init();", _
                        currentPage, providerID, serviceID, _
                        IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
                        IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), clientID, status _
                    ) _
                ) _
            )

        End Sub


    End Class
End Namespace