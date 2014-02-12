
Imports System.Collections.Specialized
Imports Target.Library
Imports Target.Web.Apps.Msg.Collections

Namespace Apps.Msg.WebSvc

#Region " NameValueListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Security.WebSvc.NameValueListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class ot provider results for simple name/value pair lists.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[mikevo]	04/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class NameValueListResult
        Public ErrMsg As ErrorMessage
        Public List As NameValueCollection
    End Class

#End Region

#Region " SingleDistributionListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Msg.WebSvc.SingleDistributionListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provider results for a single distribution list.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[mikevo]	06/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class SingleDistributionListResult
        Public ErrMsg As ErrorMessage
        Public ID As Integer
        Public Name As String
        Public Members As vwWebMsgDistribListMemberCollection
    End Class

#End Region

#Region " FetchConversationListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Msg.WebSvc.FetchConversationListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provider results for a conversations list.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[mikevo]	07/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchConversationListResult
        Public ErrMsg As ErrorMessage
        Public Conversations As WebMsgConversationCollection
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchMessageListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Msg.WebSvc.FetchMessageListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provide results for a message list.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[mikevo]	11/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchMessageListResult
        Public ErrMsg As ErrorMessage
        Public Messages As WebMsgMessageCollection
        Public PagingLinks As String
    End Class

#End Region

#Region " GetMessagePassingTypeResult "

    Public Class GetMessagePassingTypeResult
        Public ErrMsg As ErrorMessage
        Public MessagePassingType As MessagePassingType
    End Class

#End Region

End Namespace

