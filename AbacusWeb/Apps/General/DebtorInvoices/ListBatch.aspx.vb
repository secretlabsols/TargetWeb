Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls

Namespace Apps.General.DebtorInvoices

    ''' <summary>
    ''' Screen used to list all debtor invoice batches.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      08/07/2009  D11630 - added List and Reports buttons.
    '''                             Removed Invoices button.
    '''     JohnF       11/05/2009  Created (D11605)
    ''' </history>
    Partial Class ListBatch
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim createdBy As String = Convert.ToString(Request.QueryString("createdBy"))
            Dim jobID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("jobid"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reader As SqlDataReader = Nothing
            Dim js As String
            Dim style As New StringBuilder
            Dim msg As ErrorMessage = Nothing
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DebtorInvoiceBatches"), "Debtor Invoice Batches")

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/General/DebtorInvoices/ListBatch.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
            style.Append("label.label { float:left; width:14.5em; font-weight:bold; }")
            style.Append("span.label { float:left; width:14.5em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            With CType(ctlList, IReportsButton)
                .ButtonText = "List"
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DebtorsInvoiceBatches")
                .Position = SearchableMenu.SearchableMenuPosition.TopRight
            End With

            js = String.Format("currentPage={0};id={1};createdBy={2};jobid={3};", _
                 currentPage, batchID, _
                  IIf(createdBy Is Nothing, "null", String.Format("'{0}'", createdBy)), jobID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

    End Class
End Namespace