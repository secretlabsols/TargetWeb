Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Namespace Apps.Dom.ProformaInvoice

    ''' <summary>
    ''' Screen to allow a user to view the lines on a domiciliary pro forma invoice.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      16/10/2009  D11546 - added Unit Cost column.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewInvoiceLines
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim invoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))
            Dim pScheduleId As Integer = Target.Library.Utils.ToInt32(Request.QueryString("pScheduleId"))
            Dim invFilterAwait As String = Target.Library.Utils.ToString(Request.QueryString("await"))
            Dim invFilterVer As String = Target.Library.Utils.ToString(Request.QueryString("ver"))
            Dim backUrl As String = Target.Library.Utils.ToString(Request.QueryString("backUrl"))

            Dim js As String

            Dim style As New StringBuilder
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))

            If currentPage <= 0 Then currentPage = 1



            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Pro forma Invoice - Invoice Lines")

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProformaInvoice/ViewInvoiceLines.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProformaInvoice/CommentDialog.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            style.Append("label.label { float:left; width:9em; font-weight:bold; }")
            style.Append("span.label { float:left; width:9em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            ''Set breadcrumb
            'With CType(breadCrumb, InvoiceBatchBreadcrumb)
            '    .BatchID = batchID
            '    .InvoiceID = invoiceID
            '    .PaymentScheduleId = pScheduleId
            '    .InvFilterAwait = invFilterAwait
            '    .InvFilterVer = invFilterVer
            '    .backUrl = backUrl
            'End With

            If Not Me.IsPostBack Then
                Me.CustomNavAdd(False)
            End If

            js = String.Format("currentPage={0};invoiceID={1};", currentPage, invoiceID)
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

        Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.ServerClick
            Me.CustomNavRemoveLast()
            Me.CustomNavGoBack()
        End Sub
     


    End Class
End Namespace