
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
	''' Class	 : Abacus.Web.Apps.UserControls.ClientGroupSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
	'''     User control to encapsulate the listing and selecting of client groups.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
	''' 	[Mikevo]	29/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
	Partial Class ClientGroupSelector
		Inherits System.Web.UI.UserControl

		Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedClientGroupID As Integer)

			Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
			If currentPage <= 0 Then currentPage = 1

			' add utility JS link
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
			' add dialog JS
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
			' add list filter JS
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
			' add page JS
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ClientGroupSelector.js"))
			' add AJAX-generated javascript to the page
			AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))

			thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.ClientGroupSelector.Startup", _
			 Target.Library.Web.Utils.WrapClientScript(String.Format( _
			  "currentPage={0};ClientGroupSelector_selectedClientGroupID={1};", _
			  currentPage, selectedClientGroupID) _
			 ) _
			)

		End Sub

	End Class

End Namespace

