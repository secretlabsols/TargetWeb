
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.ServiceOrderSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of service orders.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     PaulW       01/07/2010  D11795 - SDS Generic Contracts, original logic copied into new file.
    '''     ColinD      22/04/2010  D11807 - added copy button functionality
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	28/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ServiceOrderSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, _
                        ByVal selectedServiceOrderID As Integer, _
                        ByVal selectedGenericServiceOrderID As Integer, _
                        ByVal providerID As Integer, _
                        ByVal ContractID As Integer, _
                        ByVal clientID As Integer, _
                        ByVal dateFrom As Date, _
                        ByVal dateTo As Date, _
                        ByVal serviceGroupID As Integer, _
                        ByVal showNewButton As Boolean, _
                        ByVal showCopyButton As Boolean, _
                        ByVal showViewButton As Boolean, _
                        ByVal viewServiceOrderInNewWindow As Boolean)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String

            ServiceOrderSelector_btnNew.Visible = showNewButton
            ServiceOrderSelector_btnCopy.Visible = showCopyButton
            ServiceOrderSelector_btnView.Visible = showViewButton

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ServiceOrderSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.ServiceOrder))

            js = String.Format( _
                "ServiceOrderSelector_currentPage={0};ServiceOrderSelector_selectedServiceOrderID={1};ServiceOrderSelector_providerID={2};ServiceOrderSelector_contractID={3};ServiceOrderSelector_clientID={4};ServiceOrderSelector_dateFrom={5};ServiceOrderSelector_dateTo={6};ServiceOrderSelector_serviceGroupID={7};ServiceOrderSelector_viewServiceOrderInNewWindow={8};ServiceOrderSelector_btnNewID='{9}';ServiceOrderSelector_btnCopyID='{10}';ServiceOrderSelector_selectedGenericServiceOrderID={11}", _
                currentPage, _
                selectedServiceOrderID, _
                providerID, _
                ContractID, _
                clientID, _
                IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
                IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
                serviceGroupID, _
                IIf(viewServiceOrderInNewWindow, "true", "false"), _
                ServiceOrderSelector_btnNew.ClientID, _
                ServiceOrderSelector_btnCopy.ClientID, _
                selectedGenericServiceOrderID _
            )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.UserControls.ServiceOrderSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js) _
            )

            With CType(cTypeSelector, CareTypeSelector)
                .InitControl(thePage)
                .showRes = True
                .showNonRes = True
                .showDP = False
                .enableRes = False
                .enableNonRes = True
                .enableDP = False
                .defaultValue = CareTypeSelector.CareTypeType.NonResidential

            End With

        End Sub

    End Class

End Namespace

