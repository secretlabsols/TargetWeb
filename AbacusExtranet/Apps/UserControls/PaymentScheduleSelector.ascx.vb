Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    Partial Public Class PaymentScheduleSelector
        Inherits System.Web.UI.UserControl

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="thePage">the parent web page</param>
        ''' <param name="selectedPaymentScheduleID">selected payment schedule id</param>
        ''' <param name="providerID">providerId / establishment id</param>
        ''' <param name="contractID">contact id</param>
        ''' <param name="reference">payment schedule reference number</param>
        ''' <param name="periodFrom">period from </param>
        ''' <param name="periodTo">period to</param>
        ''' <param name="visitYes">visit based true</param>
        ''' <param name="visitNo">visit based false</param>
        ''' <param name="pfInvoiceNo">having no proforma invoice</param>
        ''' <param name="pfInvoiceAwait">having proforma invoice that are awaiting verification</param>
        ''' <param name="pfInvoiceVerified">having verified proforma invoice</param>
        ''' <param name="invUnpaid">having unpaid invoices</param>
        ''' <param name="invSuspend">having suspended invoices</param>
        ''' <param name="invAuthorised">having authorised invoices</param>
        ''' <param name="invPaid">having paid invoices</param>
        ''' <param name="varAwait">visit amendment request awaiting verification</param>
        ''' <param name="varVerified">visit amendment request verified</param>
        ''' <param name="varDeclined">visit amendment request declined</param>
        ''' <remarks></remarks>
        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal selectedPaymentScheduleID As Integer, _
                               ByVal providerID As Integer, _
                               ByVal contractID As Integer, _
                               ByVal reference As String, _
                               ByVal periodFrom As Date, _
                               ByVal periodTo As Date, _
                               ByVal visitYes As Boolean, _
                               ByVal visitNo As Boolean, _
                               ByVal pfInvoiceNo As Boolean, _
                               ByVal pfInvoiceAwait As Boolean, _
                               ByVal pfInvoiceVerified As Boolean, _
                               ByVal invUnpaid As Boolean, _
                               ByVal invSuspend As Boolean, _
                               ByVal invAuthorised As Boolean, _
                               ByVal invPaid As Boolean, _
                               ByVal varAwait As Boolean, _
                               ByVal varVerified As Boolean, _
                               ByVal varDeclined As Boolean _
                               )


            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            Dim btnNewVisible As Boolean = thePage.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.AddNew"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String

            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/PaymentScheduleSelector.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.PaymentSchedule))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProviderInvoiceStatus))

            js = String.Format( _
                 "currentPage={0};" & _
                 "providerID={1};" & _
                 "contractID={2};" & _
                 "reference=""{3}"";" & _
                 "periodFrom={4};" & _
                 "periodTo={5};" & _
                 "visitBasedYes={6};" & _
                 "visitBasedNo={7};" & _
                 "pfInvoiceNo={8}; " & _
                 "pfInvoiceAwait={9}; " & _
                 "pfInvoiceVerified={10};" & _
                 "invUnpaid={11};" & _
                 "invSuspend={12};" & _
                 "invAuthorised={13};" & _
                 "invPaid={14};" & _
                 "varAwait={15};" & _
                 "varVerified={16};" & _
                 "varDeclined={17};" & _
                 "seletedPaymentScheduleID={18};" & _
                 "serviceDeliveryFile=""{19}""; " & _
                 "btnNewVisible={20};" _
                 , currentPage, _
                 providerID, _
                 contractID, _
                 reference, _
                 IIf(Target.Library.Utils.IsDate(periodFrom), WebUtils.GetDateAsJavascriptString(periodFrom), "null"), _
                 IIf(Target.Library.Utils.IsDate(periodTo), WebUtils.GetDateAsJavascriptString(periodTo), "null"), _
                 visitYes.ToString().ToLower(), _
                 visitNo.ToString().ToLower(), _
                 pfInvoiceNo.ToString().ToLower(), _
                 pfInvoiceAwait.ToString().ToLower(), _
                 pfInvoiceVerified.ToString().ToLower(), _
                 invUnpaid.ToString().ToLower(), _
                 invSuspend.ToString().ToLower(), _
                 invAuthorised.ToString().ToLower(), _
                 invPaid.ToString().ToLower(), _
                 varAwait.ToString().ToLower(), _
                 varVerified.ToString().ToLower(), _
                 varDeclined.ToString().ToLower(), _
                 selectedPaymentScheduleID, _
                 "false", _
                 btnNewVisible.ToString().ToLower() _
            )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal selectedPaymentScheduleID As Integer, _
                               ByVal serviceDeliveryFileId As Integer)

            Dim js As String

            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/PaymentScheduleSelector.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.PaymentSchedule))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProviderInvoiceStatus))
            Dim btnNewVisible As Boolean = thePage.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.AddNew"))

            js = String.Format( _
                 "serviceDeliveryFile=""{0}"";" & _
                 "serviceDeliveryFileId={1};" & _
                 "disableNew=""{2}"";" & _
                 "btnNewVisible={3};" _
                 , "true", _
                 serviceDeliveryFileId, _
                 "true", _
                 btnNewVisible.ToString().ToLower() _
            )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
        End Sub
    End Class

End Namespace