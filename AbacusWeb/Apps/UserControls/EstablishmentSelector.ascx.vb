
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.EstablishmentSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of providers.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      13/04/2011  SDS issue #415 - corrected default DateFrom behaviour (reversing SDS issue #322).
    ''' 	[Mikevo]	08/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EstablishmentSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedEstablishmentID As Integer, ByVal mode As EstablishmentSelectorMode, Optional ByVal includeRedundant As Boolean = True)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            Dim sRedundant As String

            If currentPage <= 0 Then currentPage = 1

            If includeRedundant Then
                sRedundant = "true"
            Else
                sRedundant = "false"
            End If

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/EstablishmentSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Establishments))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.EstablishmentSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format("currentPage={0};selectedEstablishmentID={1};establishmentSelector_mode={2};estabSelector_Redundant={3}", currentPage, selectedEstablishmentID, Convert.ToInt32(mode), sRedundant)))

        End Sub

    End Class

    Public Enum EstablishmentSelectorMode
        Unknown = 0
        Establishments = 1
        ResidentialHomes = 2
        DomProviders = 3
        DayCareProviders = 4
    End Enum

End Namespace
