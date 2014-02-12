
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.BudgetHolderSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of budget holders.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     ColinD  15/04/2011  Updated SDS585 - Updated showRedundant to redundant parameter to reflect its use in conjunction with stored procedure i.e. null = either redundant or non-redundant.
    ''' 	JohnF	15/07/2010	Created (D11801)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class BudgetHolderSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal budgetHolderID As Integer, _
                               ByVal redundant As Nullable(Of Boolean), _
                               ByVal serviceUserID As Nullable(Of Integer))

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
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/BudgetHolderSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.BudgetHolders))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.BudgetHolderSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format("currentPage={0};budgetHolderSelector_selectedbhID={1};budholderSelector_Redundant={2};budholderSelector_ServiceUserID={3};", currentPage, budgetHolderID, WebUtils.GetBooleanAsJavascriptString(redundant), WebUtils.GetIntegerAsJavascriptString(serviceUserID))))

        End Sub

    End Class

End Namespace

