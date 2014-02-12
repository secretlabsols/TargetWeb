
Imports System.Collections.Specialized
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Msg.Collections

Namespace Apps.Msg.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Jobs.Security
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to allow interaction with the messaging application.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[mikevo]	04/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/Apps/Messaging")> _
    Public Class Messaging
        Inherits System.Web.Services.WebService

#Region " Web Services Designer Generated Code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Web Services Designer.
            InitializeComponent()

            'Add your own initialization code after the InitializeComponent() call

        End Sub

        'Required by the Web Services Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Web Services Designer
        'It can be modified using the Web Services Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            components = New System.ComponentModel.Container
        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            'CODEGEN: This procedure is required by the Web Services Designer
            'Do not modify it using the code editor.
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region

#Region " FetchDistributionLists "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves the list of distribution lists, sorted by name.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	06/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDistributionLists() As NameValueListResult

            Dim msg As ErrorMessage
            Dim simpleList As NameValueCollection
            Dim distLists As WebMsgDistribListCollection = Nothing
            Dim result As NameValueListResult = New NameValueListResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the list of distribution lists
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                msg = WebMsgDistribList.FetchList(conn, distLists)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' sort by name
                If distLists.Count > 1 Then distLists.Sort(New CollectionSorter("Name", SortDirection.Ascending))

                ' load the name/value collection
                simpleList = New NameValueCollection
                For Each list As WebMsgDistribList In distLists
                    simpleList.Add(list.Name, list.ID)
                Next

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .List = simpleList
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchSingleDistributionList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves the details of a single distribution list.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	06/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSingleDistributionList(ByVal distListID As Integer) As SingleDistributionListResult

            Dim msg As ErrorMessage
            Dim distList As WebMsgDistribList
            Dim distListMembers As vwWebMsgDistribListMemberCollection = Nothing
            Dim result As SingleDistributionListResult = New SingleDistributionListResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the distribution list
                distList = New WebMsgDistribList(conn)
                msg = distList.Fetch(distListID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the list of members
                msg = vwWebMsgDistribListMember.FetchList(conn, distListMembers, distListID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .ID = distList.ID
                    .Name = distList.Name
                    .Members = distListMembers
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchConversationList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of conversations for the specified user/sender/recipient/label.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	07/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchConversationList(ByVal page As Integer, _
                                            ByVal labelID As Integer, ByVal status As String, _
                                            ByVal startedOnFrom As Date, ByVal startedOnTo As Date, _
                                            ByVal lastSentFrom As Date, ByVal lastSentTo As Date, _
                                            ByVal startedByID As Integer, ByVal startedByType As MessageFromType, _
                                            ByVal involvingID As Integer, ByVal involvingType As MessageFromType) As FetchConversationListResult

            Dim msg As ErrorMessage
            Dim convs As WebMsgConversationCollection = Nothing
            Dim result As FetchConversationListResult = New FetchConversationListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the conversation list
                msg = MsgBL.FetchConversations(conn, userID, labelID, status, startedOnFrom, startedOnTo, lastSentFrom, lastSentTo, startedByID, startedByType, involvingID, involvingType, page, pageSize, totalRecords, convs)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Conversations = convs
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchConvList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                        page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchLabelListByCompany "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves the list of conversation labels for the current user's company.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	15/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchLabelListByCompany() As NameValueListResult

            Dim msg As ErrorMessage
            Dim labelList As WebMsgLabelCollection = Nothing
            Dim simpleList As NameValueCollection
            Dim result As NameValueListResult = New NameValueListResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the list labels
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                msg = MsgBL.FetchLabelsByCompany(conn, SecurityBL.GetCurrentUser().WebSecurityCompanyID, labelList)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' sort by name
                If labelList.Count > 1 Then labelList.Sort(New CollectionSorter("Label", SortDirection.Ascending))

                ' load the name/value collection
                simpleList = New NameValueCollection
                For Each lbl As WebMsgLabel In labelList
                    simpleList.Add(lbl.Label, lbl.ID)
                Next

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .List = simpleList
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchMessageList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of message for the specified conversation.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="convID">The ID of the conversation.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	11/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchMessageList(ByVal page As Integer, ByVal convID As Integer) As FetchMessageListResult

            Dim msg As ErrorMessage
            Dim messages As WebMsgMessageCollection = Nothing
            Dim result As FetchMessageListResult = New FetchMessageListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim userID As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                userID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the message list
                msg = MsgBL.FetchMessages(conn, convID, userID, page, pageSize, totalRecords, messages)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Messages = messages
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:ViewMsg({0})"" title=""{2}"">{1}</a>&nbsp;", _
                        page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " SetMessageReadStatus "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Set the read status of the specified message for the current user (or their company).
        ''' </summary>
        ''' <param name="msgID">The ID of the message.</param>
        ''' <param name="readStatus">The new read status.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	14/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function SetMessageReadStatus(ByVal msgID As Integer, ByVal readStatus As Boolean) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then Return msg

                ' get the current user
                user = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = MsgBL.SetMessageReadStatus(conn, user.ID, readStatus, msgID)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            End Try

            Return msg

        End Function

#End Region

#Region " GetMessagePassingType "
    <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
    Public Function GetMessagePassingType() As GetMessagePassingTypeResult
        Dim result As GetMessagePassingTypeResult = New GetMessagePassingTypeResult
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

        Try
            result.ErrMsg = MsgBL.GetMessagePassingType(settings, result.MessagePassingType)
            If Not result.ErrMsg.Success Then Return result

        Catch ex As Exception
            result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
        End Try

        Return result

    End Function
#End Region

    End Class

End Namespace