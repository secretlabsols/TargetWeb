
Imports System.ComponentModel
Imports System.Collections.Specialized
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security

Namespace Apps.AuditLog.WebSvc

	<System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/Apps/AuditLog")> _
	<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
	<ToolboxItem(False)> _
	Public Class AuditLog
		Inherits System.Web.Services.WebService

		Private Const PAGE_SIZE As Integer = 10

#Region " FetchAuditLogList "

		''' <summary>
		''' Retrieves a paged list of audit log entries.
		''' </summary>
		''' <param name="page">The current page in the results to fetch.</param>
		''' <param name="applicationID">The application ID to filter the result on, or zero.</param>
		''' <param name="logType">The log record type to filters the results on, or AuditLogType.Unknown.</param>
		''' <param name="userName">The user name to filter the results on, or Nothing.</param>
		''' <param name="tableName">The table name to filter the results on, or Nothing.</param>
		''' <param name="dateFrom">The start of the date range to filter the results on, or Nothing.</param>
		''' <param name="dateTo">The end of the date range to filter the results on, or Nothing.</param>
		''' <param name="parentID">The ID of the record to filter the results on, or zero. Should be used in conjunction with tableName parameter.</param>
		''' <returns></returns>
		''' <remarks></remarks>
		<WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
		Public Function FetchAuditLogList(ByVal page As Integer, ByVal applicationID As Integer, ByVal logType As AuditLogType, _
		 ByVal userName As String, ByVal tableName As String, ByVal dateFrom As Date, _
		 ByVal dateTo As Date, ByVal parentID As Integer) As FetchAuditLogListResults

			Dim msg As ErrorMessage
			Dim result As FetchAuditLogListResults = New FetchAuditLogListResults
			Dim conn As SqlConnection = Nothing
			Dim totalRecords As Integer

			Try
				' check that any app user is logged in
				msg = SecurityBL.ValidateAnyAppLogin()
				If Not msg.Success Then
					result.ErrMsg = msg
					Return result
				End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

				' get the audit log list
				msg = AuditLogging.FetchAuditLogList(conn, page, PAGE_SIZE, applicationID, logType, userName, tableName, _
				 dateFrom, dateTo, parentID, totalRecords, result.LogEntries)
				If Not msg.Success Then
					result.ErrMsg = msg
					Return result
				End If

				With result
					.ErrMsg = New ErrorMessage
					.ErrMsg.Success = True
					.PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
					 "<a href=""javascript:FetchAuditLogList({0})"" title=""{2}"">{1}</a>&nbsp;", _
					 page, Math.Ceiling(totalRecords / PAGE_SIZE))
				End With

			Catch ex As Exception
				result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)	' unexpected
			Finally
				If Not conn Is Nothing Then conn.Close()
			End Try

			Return result

		End Function

#End Region

#Region " FetchAuditLogDetailForDisplay "

		''' <summary>
		''' Retrieves log entry details for display.
		''' </summary>
		''' <param name="id">The ID of the log record to display, or zero to retrieves one or more pages.</param>
		''' <param name="page">The current page in the results to fetch, or zero to retrieve a single record using the ID parameter.</param>
		''' <param name="applicationID">The application ID to filter the result on, or zero.</param>
		''' <param name="logType">The log record type to filters the results on, or AuditLogType.Unknown.</param>
		''' <param name="userName">The user name to filter the results on, or Nothing.</param>
		''' <param name="tableName">The table name to filter the results on, or Nothing.</param>
		''' <param name="dateFrom">The start of the date range to filter the results on, or Nothing.</param>
		''' <param name="dateTo">The end of the date range to filter the results on, or Nothing.</param>
		''' <param name="parentID">The ID of the record to filter the results on, or zero. Should be used in conjunction with tableName parameter.</param>
		''' <returns></returns>
		''' <remarks>
		''' Specifying the "id" parameter retrieves the details of that single record.
		''' Specifying the "page" paremters retrieves the details of all records in that page using the filter parameters.
		''' Specifying neither "id" or "page" retrieves details for all records returned using the filter parameters.
		''' </remarks>
		<WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
		Public Function FetchAuditLogDetailForDisplay(ByVal id As Integer, ByVal page As Integer, _
		 ByVal applicationID As Integer, ByVal logType As AuditLogType, _
		 ByVal userName As String, ByVal tableName As String, ByVal dateFrom As Date, _
		 ByVal dateTo As Date, ByVal parentID As Integer) As StringResult

			Dim msg As ErrorMessage
			Dim result As StringResult = New StringResult
			Dim conn As SqlConnection = Nothing
			Dim xsltPath As String

			Try
				' check that any app user is logged in
				msg = SecurityBL.ValidateAnyAppLogin()
				If Not msg.Success Then
					result.ErrMsg = msg
					Return result
				End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

				xsltPath = Server.MapPath(WebUtils.GetVirtualPath("Apps/AuditLog/FormatDetails.xslt"))

				' get the audit log list
				msg = AuditLogging.FetchAuditLogEntryDetail(conn, id, page, PAGE_SIZE, applicationID, logType, userName, tableName, _
				 dateFrom, dateTo, parentID, xsltPath, result.Value)
				If Not msg.Success Then
					result.ErrMsg = msg
					Return result
				End If

				With result
					.ErrMsg = New ErrorMessage
					.ErrMsg.Success = True
				End With

			Catch ex As Exception
				result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)	' unexpected
			Finally
				If Not conn Is Nothing Then conn.Close()
			End Try

			Return result

		End Function

#End Region

	End Class

End Namespace