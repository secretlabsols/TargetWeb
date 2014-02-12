Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.StatementsSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of Personal Budget Statements.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Iftikhar]	02/03/2011	D11854 - renamed DocumentSelector to StatementsSelector
    ''' 	                        Made web service changes
    ''' 	[Motahir]	13/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class StatementsSelector
        Inherits System.Web.UI.UserControl

        ''' <summary>
        ''' Method to initialise this user-control with Prime data
        ''' </summary>
        ''' <param name="thePage"></param>
        ''' <param name="StatementsSelector_selectedStatementID"></param>
        ''' <param name="StatementsSelector_ClientID"></param>
        ''' <param name="viewStatementInNewWindow"></param>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[Iftikhar]	02/03/2011	D11854 - Created
        ''' </history>
        Public Sub InitControl(ByVal thePage As BasePage, ByVal StatementsSelector_selectedStatementID As Integer, _
                               ByVal StatementsSelector_ClientID As Integer, _
                               ByVal viewStatementInNewWindow As Boolean)

            Dim jsText As String = SetJSvars(thePage, StatementsSelector_selectedStatementID, _
                                             StatementsSelector_ClientID, _
                                             viewStatementInNewWindow)
            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/StatementsSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.PersonalBudgetStatements))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                       "Target.Abacus.Web.Apps.UserControls.StatementsSelector.Startup", _
                                                       jsText, True)

        End Sub

        ''' <summary>
        ''' Returns the JavaScript script which initialises the client-side variables
        ''' </summary>
        ''' <param name="thePage"></param>
        ''' <param name="selectedStatementID"></param>
        ''' <param name="clientID"></param>
        ''' <param name="viewStatementInNewWindow"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[Iftikhar]	02/03/2011	D11854 - Created
        ''' </history>
        Private Function SetJSvars(ByVal thePage As BasePage, ByVal selectedStatementID As Integer, _
                                   ByVal clientID As Integer, ByVal viewStatementInNewWindow As Boolean) As String

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(thePage.Request.QueryString("page"))
            Dim js As New System.Text.StringBuilder

            If currentPage <= 0 Then currentPage = 1

            js.AppendFormat("currentPage={0};", currentPage)
            js.AppendFormat("StatementsSelector_selectedStatementID={0};", selectedStatementID)
            js.AppendFormat("StatementsSelector_ClientID={0};", clientID)
            js.AppendFormat("StatementsSelector_viewStatementInNewWindow={0};", IIf(viewStatementInNewWindow, "true", "false"))

            Return js.ToString()
        End Function
    End Class
End Namespace
