
Imports System.Configuration.ConfigurationManager
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.AuditLog.Controls

	''' -----------------------------------------------------------------------------
	''' Project	 : Target.Web
	''' Class	 : Apps.AuditLog.Controls.AuditLogSelector
	''' 
	''' -----------------------------------------------------------------------------
	''' <summary>
	'''     User control to encapsulate the listing and selecting of audit log entries.
	''' </summary>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[Mikevo]	11/01/2008	Created
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial Class AuditLogSelector
		Inherits System.Web.UI.UserControl

		Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

			Dim thePage As BasePage = CType(Me.Page, BasePage)
			Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
			Dim tableName As String = Request.QueryString("tbl")
			Dim parentID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
			Dim logType As AuditLogType = Utils.ToInt32(Request.QueryString("logType"))
			Dim userName As String = Request.QueryString("user")
            Dim dateFrom As Date, dateTo As Date
            Dim useApplicationIdFilter As Nullable(Of Boolean) = Utils.ToBoolean(Request.QueryString("UseApplicationFilter"))
            Dim useApplicationIdFilterValue As Integer = 0

            If currentPage <= 0 Then currentPage = 1
            If Utils.IsDate(Request.QueryString("dateFrom")) Then dateFrom = Convert.ToDateTime(Request.QueryString("dateFrom"))
            If Utils.IsDate(Request.QueryString("dateTo")) Then dateTo = Convert.ToDateTime(Request.QueryString("dateTo"))

            If useApplicationIdFilter.HasValue Then
                ' if the caller has specified this value in the query string then
                ' use the current application id otherwise it will be 0

                useApplicationIdFilterValue = Utils.ToInt32(AppSettings("CurrentApplication"))

            End If

            thePage.AddExtraCssStyle("@media screen { #divDetails { overflow-y:scroll; } }")

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/AuditLog/Controls/AuditLogSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.AuditLog.WebSvc.AuditLog))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.AuitLog.Controls.AduitLogSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
             String.Format("currentPage={0};applicationID={1};tableName='{2}';parentID={3};logType={4};userName='{5}';dateFrom={6};dateTo={7};", _
             currentPage, _
             useApplicationIdFilterValue, _
             tableName, _
             parentID, _
             IIf(logType <> AuditLogType.Unknown, Convert.ToInt32(logType), "0"), _
             userName, _
             IIf(Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
             IIf(Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null") _
            )))

		End Sub
	End Class

End Namespace

