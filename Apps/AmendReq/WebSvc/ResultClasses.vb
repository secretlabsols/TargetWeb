
Imports Target.Library

Namespace Apps.AmendReq.WebSvc

#Region " FetchAmendmentRequestListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.AmendReq.WebSvc.FetchAmendmentRequestListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provide results for the fetch amendment request method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	21/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchAmendmentRequestListResult
        Public ErrMsg As ErrorMessage
        Public Requests As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " AskBeforeAcceptQuestionResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.AmendReq.WebSvc.AskBeforeAcceptQuestionResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provide results for AskBeforeAcceptQuestion().
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	27/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class AskBeforeAcceptQuestionResult
        Public ErrMsg As ErrorMessage
        Public Question As BeforeAcceptQuestion
    End Class

#End Region

#Region " BooleanResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.AmendReq.WebSvc.BooleanResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to Boolean results.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	27/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class BooleanResult
        Public ErrMsg As ErrorMessage
        Public BooleanValue As Boolean
    End Class

#End Region

End Namespace

