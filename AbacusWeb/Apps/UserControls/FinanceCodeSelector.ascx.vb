
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.FinanceCodeSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of finance codes.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [ColinD]    13/06/2011  Updated to add support for finance code category.
    ''' 	[Mikevo]	09/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class FinanceCodeSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedFinanceCodeID As Integer, ByVal category As Integer, ByVal expAccGroupID As Integer)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/FinanceCodeSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.FinanceCodeSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format( _
              "currentPage={0};FinanceCodeSelector_selectedFinanceCodeID={1};FinanceCodeSelector_Category={2};FinanceCode_SelectedexpenditureAccountGroupID={3};", currentPage, selectedFinanceCodeID, category, expAccGroupID) _
             ) _
            )

        End Sub

    End Class

End Namespace

