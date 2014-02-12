
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
	''' Class	 : Abacus.Web.Apps.UserControls.ClientSubGroupSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
	'''     User control to encapsulate the listing and selecting of client groups.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     JohnF      14/05/2013    Initial version (D12479)
    ''' </history>
    ''' -----------------------------------------------------------------------------
	Partial Class ClientSubGroupSelector
		Inherits System.Web.UI.UserControl

		Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedClientSubGroupID As Integer)

			Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
			If currentPage <= 0 Then currentPage = 1

			' add utility JS link
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
			' add dialog JS
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
			' add list filter JS
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
			' add page JS
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ClientSubGroupSelector.js"))
			' add AJAX-generated javascript to the page
			AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))

			thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.ClientSubGroupSelector.Startup", _
			 Target.Library.Web.Utils.WrapClientScript(String.Format( _
			  "currentPage={0};ClientSubGroupSelector_selectedClientSubGroupID={1};", _
			  currentPage, selectedClientSubGroupID) _
			 ) _
			)

		End Sub

	End Class

End Namespace

