Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' <summary>
    ''' User control to encapsulate the listing and selecting of manual domiciliary proforma invoices.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Class ManualDomProformaInvoiceSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedInvoiceID As Integer, _
            ByVal providerID As Integer, ByVal contractID As Integer, ByVal clientID As Integer, _
            ByVal batchType As DomProformaInvoiceBatchType, ByVal batchStatus As DomProformaInvoiceBatchStatus, _
            ByVal dateFrom As Date, ByVal dateTo As Date, ByVal pScheduleId As Integer, _
            ByVal displayOptions As ManualDomProformaInvoiceSelectorDisplayOptions)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String

            With displayOptions
                btnCopy.Visible = .ShowCopyButton
                btnNew.Visible = .ShowNewButton
                'btnVerify.Visible = .ShowVerifyButton
                'btnUnVerify.Visible = .ShowVerifyButton
                'btnDelete.Visible = .ShowDeleteButton
                'btnView.Visible = .ShowViewButton
            End With

            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/ManualDomProformaInvoiceSelector.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProformaInvoiceBatchType))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProformaInvoiceBatchStatus))

            js = String.Format( _
                 "currentPage={0};providerID={1};contractID={2};clientID={3};" & _
                 "batchType={4};batchStatus={5};dateFrom={6};dateTo={7};selectedInvoiceID={8};" & _
                 "btnCopyID=""{9}"";btnNewID=""{10}"";" & _
                 "btnEditID=""{11}"";btnViewID=""{12}"";pScheduleId={13};", _
                 currentPage, providerID, contractID, clientID, Convert.ToInt32(batchType), Convert.ToInt32(batchStatus), _
                 IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
                 IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
                 selectedInvoiceID, _
                 btnCopy.ClientID, btnNew.ClientID, _
                 btnEdit.ClientID, btnView.ClientID, _
                 pScheduleId _
            )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", _
                String.Format("dteCopyWeekEndingID=""{0}"";", dteCopyWeekEnding.ClientID), True)
        End Sub

    End Class

    Public Class ManualDomProformaInvoiceSelectorDisplayOptions
        Public ShowNewButton As Boolean
        Public ShowCopyButton As Boolean
        Public ShowViewButton As Boolean
        Public ShowVerifyButton As Boolean
        Public ShowUnVerifyButton As Boolean
        Public ShowDeleteButton As Boolean
    End Class

End Namespace

