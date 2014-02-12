
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.Security.UserControls

    ''' <summary>
    ''' User control to encapsulate the listing and selectiing of password exceptions.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      20/03/2008  D11535 - created.
    ''' </history>
    Partial Class PasswordExceptionSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal selectedWordID As Integer)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/Security/UserControls/PasswordExceptionSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Security.WebSvc.Security))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format( _
              "currentPage={0};PasswordExceptionSelector_selectedUserID={1};", _
              currentPage, selectedWordID) _
             ) _
            )

        End Sub

    End Class

End Namespace

