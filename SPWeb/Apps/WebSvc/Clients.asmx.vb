
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
    ''' Class	 : SP.Web.Apps.WebSvc.Clients
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to allow interaction with client (service user) records.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      24/11/2006  Removed FetchClientsAddress().
    ''' 	[Mikevo]	10/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/SPWeb/Apps/WebSvc/Clients")> _
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

#Region " FetchClientsInServiceList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of clients who are receiving the specified service.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="providerID">The ID of the provider.</param>
        ''' <param name="serviceID">The ID of the service.</param>
        ''' <param name="selectedClientID">The ID of the client to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedProviderID overrides the specified page parameter such that
        '''     the page which contain the requested client is displayed. 
        ''' </remarks>
        ''' <history><![CDATA[
        '''     MikeVO      14/12/2006  Added support for Reference and Name list filters.
        '''     MikeVO      13/10/2006  Added providerID param.
        ''' 	[MikeVO]	10/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchClientsInServiceList(ByVal page As Integer, ByVal providerID As Integer, _
                                                ByVal serviceID As Integer, ByVal selectedClientID As Integer, _
                                                ByVal listFilterReference As String, ByVal listFilterName As String) As FetchClientsInServiceListResult

            Dim msg As ErrorMessage
            Dim clients As ArrayList = Nothing
            Dim result As FetchClientsInServiceListResult = New FetchClientsInServiceListResult
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

                ' get the client list
                msg = SPClassesBL.FetchClientsInService(conn, userID, page, pageSize, providerID, serviceID, selectedClientID, listFilterReference, listFilterName, totalRecords, clients)
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
                result.ErrMsg = Utils.CatchError(ex, "E0001")   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

    End Class

End Namespace