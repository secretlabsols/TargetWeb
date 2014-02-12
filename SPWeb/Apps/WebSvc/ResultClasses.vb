Imports System.Collections.Specialized
Imports Target.Library
Imports Target.SP.Library
Imports Target.SP.Library.Collections

Namespace Apps.WebSvc

#Region " FetchProviderListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : Web.Apps.WebSvc.FetchProviderListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provide results for a Provider list.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[PaulW]	14/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchProviderListResult
        Public ErrMsg As ErrorMessage
        Public Providers As vwListProviderCollection
        Public PagingLinks As String
    End Class



#End Region

#Region " FetchServiceListResult "
    Public Class FetchServiceListResult
        Public ErrMsg As ErrorMessage
        Public Services As vwListSPServiceCollection
        Public PagingLinks As String
    End Class
#End Region

#Region " FetchPropertyListResult "
    Public Class FetchPropertyListResult
        Public ErrMsg As ErrorMessage
        Public Properties As vwListSPPropertyCollection
        Public PagingLinks As String
    End Class
#End Region

#Region " FetchRemittanceListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.WebSvc.FetchRemittanceListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchRemittanceList().
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	09/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchRemittanceListResult
        Public ErrMsg As ErrorMessage
        Public Remittances As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchClientsInServiceListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.WebSvc.FetchClientsInServiceListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchClientsInServiceList().
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	10/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchClientsInServiceListResult
        Public ErrMsg As ErrorMessage
        Public Clients As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchRemittanceDetailForUserListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.WebSvc.FetchRemittanceDetailForUserListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchRemittanceDetailForUserList().
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	11/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchRemittanceDetailForUserListResult
        Public ErrMsg As ErrorMessage
        Public DetailLines As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchProviderInterfaceFileListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.WebSvc.FetchProviderInterfaceFileListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchProviderInterfaceFileList().
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	11/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchProviderInterfaceFileListResult
        Public ErrMsg As ErrorMessage
        Public InterfaceFiles As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchOccupancyListResult "
    Public Class FetchOccupancyListResult
        Public ErrMsg As ErrorMessage
        Public OccupancyEnq As vwListSPOccupancyEnqCollection
        Public PagingLinks As String
    End Class
#End Region

#Region " FetchSUNotifListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.WebSvc.FetchSUNotifListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchSUNotifList().
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	23/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchSUNotifListResult
        Public ErrMsg As ErrorMessage
        Public Notifications As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchSubsidiesListResult "
    Public Class FetchSubsidiesListResult
        Public ErrMsg As ErrorMessage
        Public Subsidies As vwListSPSubsidiesCollection
        Public PagingLinks As String
    End Class
#End Region

#Region " FetchPISubmissionListResult "

''' -----------------------------------------------------------------------------
''' Project	 : Target.SP.Web
''' Class	 : SP.Web.Apps.WebSvc.FetchPISubmissionListResult
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' <remarks>
''' </remarks>
''' <history><![CDATA[
''' 	[paul]	02/03/2007	Created
''' ]]></history>
''' -----------------------------------------------------------------------------
''' 
    Public Class FetchPISubmissionListResult
        Public ErrMsg As ErrorMessage
        Public PIReturns As ArrayList
        Public PagingLinks As String
    End Class

#End Region

End Namespace