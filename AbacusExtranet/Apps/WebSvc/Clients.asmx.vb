
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.Clients
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve client information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      12/11/2008  Added FetchDomProviderInvoiceClientList().
    ''' 	[Mikevo]	07/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/Clients")> _
    Public Class Clients
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

#Region " FetchResClientList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of residential clients that have been in homes that are
        '''     available to the current user.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="establishmentID">The ID of the home to filter the results on.</param>
        ''' <param name="selectedClientID">The ID of the client to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedClientID overrides the specified page parameter such that
        '''     the page which contain the requested client is displayed. 
        ''' </remarks>
        ''' <history>
        ''' 	[MikeVO]	07/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchResClientList(ByVal page As Integer, ByVal establishmentID As Integer, _
                                        ByVal selectedClientID As Integer, ByVal listFilterReference As String, _
                                        ByVal listFilterName As String, ByVal dateFrom As Date, _
                                        ByVal dateTo As Date) As FetchClientsListResult

            Dim msg As ErrorMessage
            Dim clients As ArrayList = Nothing
            Dim result As FetchClientsListResult = New FetchClientsListResult
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

                ' get the list of homes
                msg = AbacusClassesBL.FetchResClients( _
                                        conn, userID, page, pageSize, establishmentID, selectedClientID, _
                                        listFilterReference, listFilterName, dateFrom, dateTo, totalRecords, clients)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Clients = clients
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchClientList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchValidDomSvcOrderClientList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Retrieves a list of domiciliary clients that have domiciliary service orders
        ''' linked to the specified contract, where the contract is linked to a provider
        ''' available to the current user.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="providerID">The ID of the provider to filter the results on.</param>
        ''' <param name="contractID">The ID of the contract to filter the results on.</param>
        ''' <param name="selectedClientID">The ID of the client to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <param name="dateFrom">The DSO date range start to filter the results on.</param>
        ''' <param name="dateTo">The DSO date range end to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedClientID overrides the specified page parameter such that
        '''     the page which contain the requested client is displayed. 
        ''' </remarks>
        ''' <history>
        ''' MvO  28/02/2008 Created
        ''' Paul 15/04/2008 Added providerID param
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchValidDomSvcOrderClientList(ByVal page As Integer, ByVal providerID As Integer, ByVal contractID As Integer, _
                                        ByVal selectedClientID As Integer, ByVal listFilterReference As String, _
                                        ByVal listFilterName As String, ByVal dateFrom As Date, _
                                        ByVal dateTo As Date, _
                                        ByVal pScheduleId As Integer) As FetchClientsListResult

            Dim msg As ErrorMessage
            Dim clients As ArrayList = Nothing
            Dim result As FetchClientsListResult = New FetchClientsListResult
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

                ' get the list of homes
                msg = AbacusClassesBL.FetchValidDomSvcOrderClientList( _
                                            conn, userID, page, pageSize, providerID, contractID, selectedClientID, _
                                            listFilterReference, listFilterName, dateFrom, dateTo, totalRecords, pScheduleId, clients)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Clients = clients
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchClientList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomSvcOrderClientList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Retrieves a list of domiciliary clients that have domiciliary service orders
        ''' linked to the specified contract, where the contract is linked to a provider
        ''' available to the current user.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="providerID">The ID of the provider to filter the results on.</param>
        ''' <param name="contractID">The ID of the contract to filter the results on.</param>
        ''' <param name="selectedClientID">The ID of the client to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <param name="dateFrom">The DSO date range start to filter the results on.</param>
        ''' <param name="dateTo">The DSO date range end to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedClientID overrides the specified page parameter such that
        '''     the page which contain the requested client is displayed. 
        ''' </remarks>
        ''' <history>
        ''' MvO  28/02/2008 Created
        ''' Paul 15/04/2008 Added providerID param
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomSvcOrderClientList(ByVal page As Integer, ByVal providerID As Integer, ByVal contractID As Integer, _
                                        ByVal selectedClientID As Integer, ByVal listFilterReference As String, _
                                        ByVal listFilterName As String, ByVal dateFrom As Date, _
                                        ByVal dateTo As Date _
                                        ) As FetchClientsListResult

            Dim msg As ErrorMessage
            Dim clients As ArrayList = Nothing
            Dim result As FetchClientsListResult = New FetchClientsListResult
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

                msg = AbacusClassesBL.FetchDomSvcOrderClientList( _
                                        conn, userID, page, pageSize, providerID, contractID, selectedClientID, _
                                        listFilterReference, listFilterName, dateFrom, dateTo, totalRecords, clients)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Clients = clients
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchClientList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProviderInvoiceClientList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Retrieves a list of domiciliary clients that have domiciliary provider invoices
        ''' linked to the specified contract, where the contract is linked to a provider
        ''' available to the current user.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="providerID">The ID of the provider to filter the results on.</param>
        ''' <param name="contractID">The ID of the contract to filter the results on.</param>
        ''' <param name="selectedClientID">The ID of the client to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <param name="visitBased">
        ''' Whether to only look for visit-based provider invoices. 
        ''' 0 = Do not filter
        ''' 1 = True
        ''' 2 = False
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedClientID overrides the specified page parameter such that
        '''     the page which contain the requested client is displayed. 
        ''' </remarks>
        ''' <history>
        ''' MvO  12/11/2008 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceClientList( _
                                        ByVal page As Integer, ByVal providerID As Integer, ByVal contractID As Integer, _
                                        ByVal selectedClientID As Integer, ByVal listFilterReference As String, _
                                        ByVal listFilterName As String, ByVal visitBased As Integer) As FetchClientsListResult

            Dim msg As ErrorMessage
            Dim clients As ArrayList = Nothing
            Dim result As FetchClientsListResult = New FetchClientsListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim localVisitBased As TriState

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

                Select Case visitBased
                    Case 1
                        localVisitBased = TriState.True
                    Case 2
                        localVisitBased = TriState.False
                    Case Else
                        localVisitBased = TriState.UseDefault
                End Select

                ' get the list of homes
                msg = AbacusClassesBL.FetchDomProviderInvoiceClientList( _
                                        conn, userID, page, pageSize, providerID, contractID, selectedClientID, _
                                        listFilterReference, listFilterName, localVisitBased, totalRecords, clients)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Clients = clients
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchClientList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

    End Class

End Namespace