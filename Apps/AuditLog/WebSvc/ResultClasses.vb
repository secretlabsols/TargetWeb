
Imports System.Collections.Generic
Imports Target.Library

Namespace Apps.AuditLog.WebSvc

#Region " FetchAuditLogListResults "

	''' <summary>
	''' Simple class to provide results for the FetchAuditLogList() method.
	''' </summary>
	''' <remarks></remarks>
	Public Class FetchAuditLogListResults
		Public ErrMsg As ErrorMessage
		Public LogEntries As List(Of ViewableAuditLogEntry)
		Public PagingLinks As String
	End Class

#End Region

#Region " StringResult "

	''' <summary>
	''' Simple class to provide results for the methods that just return a string.
	''' </summary>
	''' <remarks></remarks>
	Public Class StringResult
		Public ErrMsg As ErrorMessage
		Public Value As String
	End Class

#End Region

End Namespace
