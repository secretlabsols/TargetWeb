
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Codesmith = Target.Abacus.Library.DataClasses
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.ResPayments
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve residential payment information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	07/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/ResPayments")> _
    Public Class ResPayments
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

#Region " FetchRemittanceList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of residential remittances for homes available to the current user.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="establishmentID">The home to filter the results on.</param>
        ''' <param name="dateFrom">The start of the date range to filter the results on.</param>
        ''' <param name="dateTo">The end of the date range to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[MikeVO]	07/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRemittanceList(ByVal page As Integer, ByVal establishmentID As Integer, _
                                        ByVal dateFrom As Date, ByVal dateTo As Date) As FetchRemittancesListResult

            Dim msg As ErrorMessage
            Dim remittances As ArrayList = Nothing
            Dim result As FetchRemittancesListResult = New FetchRemittancesListResult
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
                userID = SecurityBL.GetCurrentUser().ExternalUserID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of remittances
                msg = AbacusClassesBL.FetchRemittances(conn, userID, page, pageSize, establishmentID, dateFrom, dateTo, totalRecords, remittances)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Remittances = remittances
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchRemittanceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchRemittanceDetailForUserList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of remittance detail lines for the specified home/client.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="establishmentID">The ID of the home.</param>
        ''' <param name="clientID">The ID of the client.</param>
        ''' <param name="dateFrom">The start of the date range.</param>
        ''' <param name="dateTo">The end of the date range.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[MikeVO]	13/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRemittanceDetailForUserList(ByVal page As Integer, _
                                                        ByVal establishmentID As Integer, ByVal clientID As Integer, _
                                                        ByVal dateFrom As Date, ByVal dateTo As Date) As FetchRemittanceDetailForUserListResult

            Dim msg As ErrorMessage
            Dim detailLines As ArrayList = Nothing
            Dim result As FetchRemittanceDetailForUserListResult = New FetchRemittanceDetailForUserListResult
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

                user = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the remittance list
                msg = AbacusClassesBL.FetchRemittanceDetailForUser(conn, page, pageSize, user.ExternalUserID, establishmentID, clientID, dateFrom, dateTo, totalRecords, detailLines)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .DetailLines = detailLines
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchDetailLinesList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " AuthoriseRemittance "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Authorise a single remittance.
        ''' </summary>
        ''' <param name="remittanceID">The ID of the selected remittance.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function AuthoriseRemittance(ByVal remittanceID As String) As BooleanResult

            Dim msg As New ErrorMessage
            Dim result As BooleanResult = New BooleanResult
            Dim conn As SqlConnection = Nothing
            Dim statusBy As String = String.Empty
            Dim remittanceRec As Codesmith.Remittance = Nothing

            Try
                '++ Check user is still logged in..
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                remittanceRec = New Codesmith.Remittance(conn)
                msg = remittanceRec.Fetch(Utils.ToInt32(remittanceID))
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                msg = Remittances.RemittancesBL.ProvisionallyAuthoriseRemittance(conn, remittanceRec, statusBy)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

        ''' <summary>
        ''' Marks a single remittance as 'Provisional Rejected' along with a reason for doing so.
        ''' </summary>
        ''' <param name="remittanceID">The ID of the selected remittance.</param>
        ''' <param name="rejectReason">The reason for the rejection.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
#Region " RejectRemittance "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function RejectRemittance(ByVal remittanceID As String, ByVal rejectReason As String) As BooleanResult

            Const PROCESS_STATUS_PROV_REJECTED As Integer = 11
            Dim msg As New ErrorMessage
            Dim result As BooleanResult = New BooleanResult
            Dim conn As SqlConnection = Nothing
            Dim statusBy As String = String.Empty
            Dim remittanceRec As Codesmith.Remittance = Nothing
            Dim user As WebSecurityUser = Nothing

            Try
                '++ Check user is still logged in..
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                user = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                remittanceRec = New Codesmith.Remittance(conn)
                msg = remittanceRec.Fetch(Utils.ToInt32(remittanceID))
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With remittanceRec
                    .RejectionReason = Utils.Left(rejectReason, 100)
                    .ProcessStatusID = PROCESS_STATUS_PROV_REJECTED
                    .ProcessStatusDate = Date.Now
                    .ProcessStatusBy = user.ExternalUsername
                End With

                msg = Remittances.RemittancesBL.SaveRemittance(conn, remittanceRec)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                End With

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