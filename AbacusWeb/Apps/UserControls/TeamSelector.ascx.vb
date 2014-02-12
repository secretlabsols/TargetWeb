
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
	''' Class	 : Abacus.Web.Apps.UserControls.TeamSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
	'''     User control to encapsulate the listing and selecting of team.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
	''' 	[Mikevo]	28/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
	Partial Class TeamSelector
		Inherits System.Web.UI.UserControl

		Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedTeamID As Integer, _
		 ByVal availableToRes As TriState, ByVal availableToDom As TriState)

			Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
			If currentPage <= 0 Then currentPage = 1

			' add utility JS link
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
			' add dialog JS
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
			' add list filter JS
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
			' add page JS
			thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/TeamSelector.js"))
			' add AJAX-generated javascript to the page
			AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))

			thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.TeamSelector.Startup", _
			 Target.Library.Web.Utils.WrapClientScript(String.Format( _
			  "currentPage={0};TeamSelector_selectedTeamID={1};TeamSelector_availableToRes='{2}';TeamSelector_availableToDom='{3}';", _
			  currentPage, selectedTeamID, availableToRes, availableToDom) _
			 ) _
			)

		End Sub

	End Class

End Namespace

