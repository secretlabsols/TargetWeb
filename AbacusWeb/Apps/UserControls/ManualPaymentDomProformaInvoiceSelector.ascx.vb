
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.ManualPaymentDomProformaInvoiceSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' User control to encapsulate the listing and selecting of manual payment
    ''' domiciliary pro forma invoices.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	07/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ManualPaymentDomProformaInvoiceSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedInvoiceID As Integer, _
                              ByVal providerID As Integer, ByVal contractID As Integer, _
                              ByVal systemAccountID As Integer, ByVal showNewButton As Boolean)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String

            btnNew.Visible = showNewButton

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ManualPaymentDomProformaInvoiceSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

            js = String.Format( _
             "currentPage={0};providerID={1};contractID={2};systemAccountID={3};selectedInvoiceID={4};" & _
             "btnViewID='{5}';btnNewID='{6}';", _
             currentPage, providerID, contractID, systemAccountID, selectedInvoiceID, _
             btnView.ClientID, btnNew.ClientID _
             )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

    End Class

End Namespace

