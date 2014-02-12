
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Web.Services
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.Security

Namespace Apps.AmendReq.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.AmendReq.WebSvc.AmendmentRequests
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to allow interaction with the amendment requests application.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      10/10/2007  Ported to use AspCompatWebServiceHandler.    
    ''' 	[Mikevo]	26/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/Apps/AmendmentRequests")> _
    Public Class AmendmentRequests
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

#Region " FetchAmendmentRequestList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of amendment requests for the specified filters.
        ''' </summary>
        ''' <param name="page">The current page in the results.</param>
        ''' <param name="requestID">The ID of a specific amendment request, zero to return a list.</param>
        ''' <param name="fromDate">The date from filter.</param>
        ''' <param name="toDate">The date to filter.</param>
        ''' <param name="status">The status filter.</param>
        ''' <param name="externalUserID">The external user filter.</param>
        ''' <param name="includeLiveValues">Whether the live current values from the database will be included in the results.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	21/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchAmendmentRequestList(ByVal page As Integer, ByVal requestID As Integer, _
                                        ByVal fromDate As Date, ByVal toDate As Date, _
                                        ByVal status As Byte, ByVal externalUserID As Integer, _
                                        ByVal includeLiveValues As Boolean) As FetchAmendmentRequestListResult

            Dim msg As ErrorMessage
            Dim requests As ArrayList = Nothing
            Dim result As FetchAmendmentRequestListResult = New FetchAmendmentRequestListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                user = SecurityBL.GetCurrentUser()

                ' get the request list
                msg = AmendReqBL.FetchAmendmentRequests( _
                                    conn, _
                                    ConnectionStrings("Abacus").ConnectionString, _
                                    requestID, _
                                    fromDate, _
                                    toDate, _
                                    status, _
                                    externalUserID, _
                                    page, _
                                    pageSize, _
                                    includeLiveValues, _
                                    AppSettings("SiteID"), _
                                    user.ExternalUsername, _
                                    totalRecords, _
                                    requests)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Requests = requests
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchRequestList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " AskBeforeAcceptQuestion "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Returns the question to ask before accepting an amendment request.
        ''' </summary>
        ''' <param name="requestID">The ID of the request.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        '''     MikeVO      01/12/2008  D11444 - security overhaul.
        ''' 	[Mikevo]	26/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function AskBeforeAcceptQuestion(ByVal requestID As Integer) As AskBeforeAcceptQuestionResult

            Dim result As AskBeforeAcceptQuestionResult = New AskBeforeAcceptQuestionResult
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim request As WebAmendReq
            Dim msg As ErrorMessage
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                result.ErrMsg = SecurityBL.ValidateLogin
                If Not result.ErrMsg.Success Then Return result

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                user = SecurityBL.GetCurrentUser()

                ' check user has correct rights
                If Not SecurityBL.UserHasMenuItemCommand(conn, _
                                         user.ID, _
                                         Target.Library.Web.ConstantsManager.GetConstant(settings.CurrentApplication, "WebNavMenuItem.ProcessAmendmentRequests"), _
                                         settings.CurrentApplicationID) Then
                    Throw New ApplicationException("Access is denied.")
                End If

                ' fetch the request
                request = New WebAmendReq(conn)
                msg = request.Fetch(requestID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                Else
                    ' get the question
                    msg = AmendReqBL.AskBeforeAcceptQuestion( _
                                        request.WebAmendReqDataItemID, _
                                        conn, _
                                        ConnectionStrings("Abacus").ConnectionString, _
                                        request.RecordID, _
                                        request.ParentRecordID, _
                                        AppSettings("SiteID"), _
                                        user.ExternalUsername, _
                                        result.Question)

                    ' check for errors
                    If Not msg.Success Then
                        result.ErrMsg = msg
                    Else
                        result.ErrMsg = New ErrorMessage
                        result.ErrMsg.Success = True
                    End If

                End If

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " ProcessRequest "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Processes a single amendment request.
        ''' </summary>
        ''' <param name="requestID">The ID of the request.</param>
        ''' <param name="accept">If the requets is being accepted or declined.</param>
        ''' <param name="beforeAcceptQuestionAnswer">The answer to any before accept question.</param>
        ''' <param name="comment">The comment entered by the user.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        '''     MikeVO      01/12/2008  D11444 - security overhaul.
        ''' 	[Mikevo]	27/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ProcessRequest(ByVal requestID As Integer, ByVal accept As Boolean, _
                                        ByVal beforeAcceptQuestionAnswer As DialogAnswer, ByVal comment As String) As BooleanResult

            Dim result As BooleanResult = New BooleanResult
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim msg As ErrorMessage
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                result.ErrMsg = SecurityBL.ValidateLogin
                If Not result.ErrMsg.Success Then Return result

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                user = SecurityBL.GetCurrentUser()

                ' check user has correct rights
                If Not SecurityBL.UserHasMenuItemCommand(conn, _
                                         user.ID, _
                                         Target.Library.Web.ConstantsManager.GetConstant(settings.CurrentApplication, "WebNavMenuItem.ProcessAmendmentRequests"), _
                                         settings.CurrentApplicationID) Then
                    Throw New ApplicationException("Access is denied.")
                End If

                msg = AmendReqBL.ProcessRequest( _
                                    requestID, _
                                    conn, _
                                    ConnectionStrings("Abacus").ConnectionString, _
                                    accept, _
                                    beforeAcceptQuestionAnswer, _
                                    comment, _
                                    AppSettings("SiteID"), _
                                    user.ExternalUsername, _
                                    user.ID, _
                                    result.BooleanValue)

                ' check for errors
                If Not msg.Success Then
                    result.ErrMsg = msg
                Else
                    result.ErrMsg = New ErrorMessage
                    result.ErrMsg.Success = True
                End If

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result


        End Function

#End Region

    End Class

End Namespace