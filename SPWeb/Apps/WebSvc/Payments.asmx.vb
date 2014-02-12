
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.WebSvc.Payments
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to allow interaction with the payments application.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	09/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/SPWeb/Apps/WebSvc/Payments")> _
    Public Class Payments
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
        '''     Retrieves a list of remittances for the specified provider/service/date range.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="providerID">The ID of the provider.</param>
        ''' <param name="serviceID">The ID of the service.</param>
        ''' <param name="dateFrom">The start of the date range.</param>
        ''' <param name="dateTo">The end of the date range.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history><![CDATA[
        ''' 	[MikeVO]	09/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRemittanceList(ByVal page As Integer, ByVal providerID As Integer, ByVal serviceID As Integer, _
                                            ByVal dateFrom As Date, ByVal dateTo As Date) As FetchRemittanceListResult

            Dim msg As ErrorMessage
            Dim remittances As ArrayList = Nothing
            Dim result As FetchRemittanceListResult = New FetchRemittanceListResult
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

                ' get the remittance list
                msg = SPClassesBL.FetchRemittances(conn, userID, page, pageSize, providerID, serviceID, dateFrom, dateTo, totalRecords, remittances)
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
                result.ErrMsg = Utils.CatchError(ex, "E0001")   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchRemittanceDetailForUserList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of remittance detail lines for the specified service/client.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="serviceID">The ID of the service.</param>
        ''' <param name="clientID">The ID of the client.</param>
        ''' <param name="dateFrom">The start of the date range.</param>
        ''' <param name="dateTo">The end of the date range.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history><![CDATA[
        ''' 	[MikeVO]	09/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRemittanceDetailForUserList(ByVal page As Integer, _
                                                        ByVal serviceID As Integer, ByVal clientID As Integer, _
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
                msg = SPClassesBL.FetchRemittanceDetailForUser(conn, page, pageSize, user.ExternalUserID, serviceID, clientID, dateFrom, dateTo, totalRecords, detailLines)
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
                result.ErrMsg = Utils.CatchError(ex, "E0001")   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchProviderInterfaceFileList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of interface files for the specified provider/date range.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="providerID">The ID of the provider.</param>
        ''' <param name="dateFrom">The start of the date range.</param>
        ''' <param name="dateTo">The end of the date range.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history><![CDATA[
        ''' 	[MikeVO]	09/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchProviderInterfaceFileList(ByVal page As Integer, ByVal providerID As Integer, _
                                                        ByVal dateFrom As Date, ByVal dateTo As Date) As FetchProviderInterfaceFileListResult

            Dim msg As ErrorMessage
            Dim interfaceFiles As ArrayList = Nothing
            Dim result As FetchProviderInterfaceFileListResult = New FetchProviderInterfaceFileListResult
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

                userID = SecurityBL.GetCurrentUser().ExternalUserID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the interface file list
                msg = SPClassesBL.FetchProviderInterfaceFiles(conn, page, pageSize, userID, providerID, dateFrom, dateTo, totalRecords, interfaceFiles)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .InterfaceFiles = interfaceFiles
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchInterfaceFileList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                        page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, "E0001")   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

    End Class

End Namespace
