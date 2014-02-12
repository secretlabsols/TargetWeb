
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.ServiceGroupSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of service groups.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     Mo Tahir    05/11/2012  D12343 - Remove Framework for Service Group.
    '''     ColinD      13/10/2010  D11918 - added ability to filter out blank framework types from result set
    ''' 	[Mo Tahir]	12/08/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class ServiceGroupSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedServiceGroupID As Integer, Optional ByVal onlyShowGroupsAvailableToUser As Boolean = False)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ServiceGroupSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.ServiceGroupSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format( _
              "currentPage={0};ServiceGroupSelector_selectedServiceGroupID={1};ServiceGroupSelector_onlyShowGroupsAvailableToUser={2};", currentPage, selectedServiceGroupID, onlyShowGroupsAvailableToUser.ToString.ToLower) _
             ) _
            )

        End Sub

    End Class
End Namespace
