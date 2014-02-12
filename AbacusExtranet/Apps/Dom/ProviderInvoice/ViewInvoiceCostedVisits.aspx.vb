Imports Target.Web.Apps.Security
Imports Target.Web
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils


Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Screen to allow a user to view the the cost of each individual visit.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     JohnF       17/12/2009  Allow for flexible back navigation (#5966)
    '''     MikeVO      19/10/2009  D11546 - corrected screen name.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewInvoiceCostedVisits
        Inherits Target.Web.Apps.BasePage

        Private _InvoiceHasNotes As Boolean = False
        Private _invoiceID As Integer
        Private _psView As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ProviderInvoiceEnquiry"), "Provider Invoice - Costed Visits")

            _invoiceID = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            If _invoiceID > 0 Then
                CType(ctrlInvoiceNotes, Apps.UserControls.ViewProviderInvoiceNotes).InvoiceID = _invoiceID
                _InvoiceHasNotes = CType(ctrlInvoiceNotes, Apps.UserControls.ViewProviderInvoiceNotes).InvoiceHasNotes
            End If

            Dim visitID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("visitID"))
            Dim js As String
            Dim style As New StringBuilder
            Me.UseJQuery = True
            UseJqueryUI = True

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))

            _psView = Target.Library.Utils.ToBoolean(Request.QueryString("psview"))

            If currentPage <= 0 Then currentPage = 1



            CType(headerDetails, UserControls.DomProviderInvoiceHeaderDetails).InvoiceID = _invoiceID

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProviderInvoice/ViewInvoiceCostedVisits.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            style.Append("label.label { float:left; width:10em; font-weight:bold; }")
            style.Append("span.label { float:left; width:10em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            js = String.Format("currentPage={0};invoiceID={1};selectedVisitID={2};InvoiceHasNotes=""{3}"";showPaymentSchedule={4};", currentPage, _invoiceID, visitID, _InvoiceHasNotes.ToString().ToLower(), _psView.ToString().ToLower())

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

            Me.CustomNavRemoveLast("AbacusExtranet/Apps/Dom/ProviderInvoice/ViewInvoiceCostedVisits.aspx")
            Me.CustomNavAdd(False)

            If Not Me.IsPostBack Then
                Me.CustomNavAdd(False)
            End If

        End Sub

        Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.ServerClick
            Me.CustomNavRemoveLast()
            Me.CustomNavGoBack()
        End Sub

      

      
    End Class


End Namespace