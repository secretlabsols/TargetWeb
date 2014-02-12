Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library.InterfaceLogs
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to manage interface logs
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D11874 Created 10/02/2010
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/interfaceLogs")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class InterfaceLogs
        Inherits System.Web.Services.WebService

#Region "Fields"

        ' fields
        Private Shared _ConnectionString As String = Nothing

        ' constants
        Private Const _ConnectionStringKey As String = "Abacus"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the connection string.
        ''' </summary>
        ''' <value>The connection string.</value>
        Private Shared ReadOnly Property ConnectionString() As String
            Get
                If String.IsNullOrEmpty(_ConnectionString) OrElse _ConnectionString.Trim().Length = 0 Then
                    ' if we havent already fetched the connection string then do so
                    _ConnectionString = ConnectionStrings(_ConnectionStringKey).ConnectionString
                End If
                Return _ConnectionString
            End Get
        End Property

#End Region

#Region "Functions"

        ''' <summary>
        ''' Gets the paged interface logs.
        ''' </summary>
        ''' <param name="page">The page.</param>
        ''' <param name="pageSize">Size of the page.</param>
        ''' <param name="selectedId">The selected id.</param>
        ''' <param name="filterCreatedDate">The filter created date.</param>
        ''' <param name="filterCreatedBy">The filter created by.</param>
        ''' <param name="filterInterfaceLogTypes">The filter interface log types (Bitwise), null will fetch all.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Get paged interface logs"), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Shared Function GetPagedInterfaceLogs(ByVal page As Integer, _
                                                    ByVal pageSize As Integer, _
                                                    ByVal selectedId As Integer, _
                                                    ByVal filterCreatedDate As String, _
                                                    ByVal filterCreatedBy As String, _
                                                    ByVal filterInterfaceLogTypes As Integer) _
                                                    As InterfaceLogs_GetPagedInterfaceLogsResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New InterfaceLogs_GetPagedInterfaceLogsResult()

            Try

                Dim filterInterfaceLogTypesAsEnum As InterfaceLogsBL.InterfaceLogType = CType(filterInterfaceLogTypes, InterfaceLogsBL.InterfaceLogType)
                Dim totalRecords As Integer = 0
                Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                ' get the connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the list of interface logs and throw error if not succeeded
                msg = InterfaceLogsBL.GetPagedInterfaceLogs(connection, page, pageSize, selectedId, totalRecords, filterCreatedDate, filterCreatedBy, filterInterfaceLogTypes, 0, result.Items)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage()
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:InterfaceLogSelector_FetchInterfaceLogs({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                ' catch and wrap the unexpected error

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            Finally
                ' always close the connection

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

#End Region

    End Class

End Namespace
