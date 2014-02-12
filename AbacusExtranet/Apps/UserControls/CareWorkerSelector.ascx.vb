Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.UserControls.CareWorkerSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of care workers.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul]	14/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class CareWorkerSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal providerID As Integer, ByVal selectedCareWorkerID As Integer)

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
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/CareWorkerSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.UserControls.CareWorkerSelector.Startup", _
                Target.Library.Web.Utils.WrapClientScript(String.Format("currentPage={0};careWorkerID={1};providerID={2};", _
                    currentPage, selectedCareWorkerID, providerID _
                )) _
            )

        End Sub

    End Class

End Namespace