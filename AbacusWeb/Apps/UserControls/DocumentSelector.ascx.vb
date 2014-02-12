Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.DocumentSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of Spend Plans.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Motahir]	13/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class DocumentSelector
        Inherits System.Web.UI.UserControl
        Private _showNewButton As Boolean

        Public Sub InitControl(ByVal thePage As BasePage, ByVal DocumentSelector_selectedDocumentID As Integer, _
                               ByVal DocumentSelector_selectedClientID As Integer, ByVal showNewButton As Boolean, _
                               ByVal viewDocumentInNewWindow As Boolean)

            Dim DocumentSelector_currentPage As Integer = Target.Library.Utils.ToInt32(thePage.Request.QueryString("page"))
            Dim js As String
            If DocumentSelector_currentPage <= 0 Then DocumentSelector_currentPage = 1

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
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DocumentSelector.js"))
            'add AJAX-generated javascript to the page
            'AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Documents))

            js = String.Format( _
                    "DocumentSelector_currentPage={0};DocumentSelector_selectedDocumentID={1};DocumentSelector_selectedClientID={2};DocumentSelector_viewDocumentInNewWindow={3};", _
                    DocumentSelector_currentPage, DocumentSelector_selectedDocumentID, DocumentSelector_selectedClientID, IIf(viewDocumentInNewWindow, "true", "false"))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.UserControls.DocumentSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js) _
            )

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            DocumentSelector_btnNew.Visible = _showNewButton
        End Sub
    End Class
End Namespace
