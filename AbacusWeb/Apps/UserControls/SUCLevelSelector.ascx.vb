Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.SUCLevelSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of SDS Contribution Levels.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [MoTahir] 31/08/2010 D11814
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class SUCLevelSelector
        Inherits System.Web.UI.UserControl
        Private _showNewButton As Boolean

        Public Sub InitControl(ByVal thePage As BasePage, ByVal SUCLevelSelector_selectedSUCLevelID As Integer, ByVal selectedClientID As Integer, ByVal SUCLevelSelector_isSDS As Boolean, ByVal showNewButton As Boolean)

            Dim SUCLevelSelector_currentPage As Integer = Target.Library.Utils.ToInt32(thePage.Request.QueryString("page"))
            Dim js As String
            If SUCLevelSelector_currentPage <= 0 Then SUCLevelSelector_currentPage = 1

            _showNewButton = showNewButton

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/SUCLevelSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.SUCLevels))

            js = String.Format( _
             "SUCLevelSelector_currentPage={0};SUCLevelSelector_clientID={1};SUCLevelSelector_selectedSUCLevelID={2};SUCLevelSelector_isSDS={3};", _
             SUCLevelSelector_currentPage, selectedClientID, SUCLevelSelector_selectedSUCLevelID, IIf(SUCLevelSelector_isSDS, "true", "false"))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.UserControls.SUCLevelSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js) _
            )

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ' show or hide new button
            Me.SUCLevelSelector_btnNew.Visible = _showNewButton
        End Sub
    End Class
End Namespace
