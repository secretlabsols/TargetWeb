
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.DomContractReOpenedWeekSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' User control to encapsulate the listing and selecting of domiciliary
    ''' contract re-opened weeks.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	16/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class DomContractReOpenedWeekSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedWeekID As Integer, ByVal providerID As Integer, _
                               ByVal contractID As Integer, ByVal weDateFrom As Date, ByVal weDateTo As Date, _
                               ByVal closureDateFrom As Date, ByVal closureDateTo As Date, ByVal showNewButton As Boolean)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            btnNew.visible = showNewButton

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Date.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DomContractReOpenedWeekSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
                String.Format("currentPage={0};selectedWeekID={1};providerID={2};contractID={3};" & _
                              "weDateFrom={4};weDateTo={5};closureDateFrom={6};closureDateTo={7};" & _
                              "btnNewID='{8}';", _
                    currentPage, selectedWeekID, providerID, contractID, _
                    IIf(Utils.IsDate(weDateFrom), WebUtils.GetDateAsJavascriptString(weDateFrom), "null"), _
                    IIf(Utils.IsDate(weDateTo), WebUtils.GetDateAsJavascriptString(weDateTo), "null"), _
                    IIf(Utils.IsDate(closureDateFrom), WebUtils.GetDateAsJavascriptString(closureDateFrom), "null"), _
                    IIf(Utils.IsDate(closureDateTo), WebUtils.GetDateAsJavascriptString(closureDateTo), "null"), _
                    btnNew.ClientID), _
                True)

        End Sub

    End Class

End Namespace

