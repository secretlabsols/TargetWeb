Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' <summary>
    ''' User control to encapsulate the listing and selecting of domiciliary proforma invoices.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Class DomProformaInvoiceSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal fileID As Integer, ByVal selectedInvoiceID As Integer, _
            ByVal providerID As Integer, ByVal contractID As Integer, _
            ByVal batchType As DomProformaInvoiceBatchType, ByVal batchStatus As DomProformaInvoiceBatchStatus, _
            ByVal dateFrom As Date, ByVal dateTo As Date, _
            ByVal displayOptions As DomProformaInvoiceSelectorDisplayOptions)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String

            With displayOptions
                btnViewContract.Visible = .ShowViewContractButton
                btnViewBatch.Visible = .ShowViewBatchButton
                btnViewInvoices.Visible = .ShowViewInvoicesButton
            End With

            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/DomProformaInvoiceSelector.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProformaInvoiceBatchType))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProformaInvoiceBatchStatus))

            js = String.Format( _
                 "currentPage={0};providerID={1};contractID={2};batchType={3};batchStatus={4};dateFrom={5};dateTo={6};selectedBatchID={7};fileID={8};" & _
                 "btnViewContractID=""{9}"";btnViewBatchID=""{10}"";btnViewInvoicesID=""{11}"";", _
                 currentPage, providerID, contractID, Convert.ToInt32(batchType), Convert.ToInt32(batchStatus), _
                 IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
                 IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
                 selectedInvoiceID, fileID, _
                 btnViewContract.ClientID, btnViewBatch.ClientID, btnViewInvoices.ClientID _
            )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

    End Class

    Public Class DomProformaInvoiceSelectorDisplayOptions
        Public ShowViewContractButton As Boolean
        Public ShowViewBatchButton As Boolean
        Public ShowViewInvoicesButton As Boolean
    End Class

End Namespace

