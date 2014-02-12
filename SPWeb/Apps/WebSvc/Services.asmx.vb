Imports System.Configuration.ConfigurationManager
Imports System.Web.Services
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.SP.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.SP.Library.Collections
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/SPWeb/Apps/WebSvc/Services")> _
    Public Class Services
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

#Region " FetchServiceList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of Services for the specified user/provider.
        ''' </summary>
        ''' <param name="page">The current page in the results to display.</param>
        ''' <param name="ProviderID">The ID of the provider to view services for.</param>
        ''' <param name="selectedServiceID">The ID of the service to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedServiceID overrides the specified page parameter such that
        '''     the page which contain the requested service is displayed. 
        ''' </remarks>
        ''' <history><![CDATA[
        '''     MikeVO  14/12/2006  Added support for Reference and Name list filters.
        '''     MikeVO  09/10/2006  Added selectedServiceID param.
        ''' 	[PaulW]	26/09/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceList(ByVal page As Integer, ByVal ProviderID As Integer, ByVal selectedServiceID As Integer, _
                                        ByVal listFilterReference As String, ByVal listFilterName As String) As FetchServiceListResult

            Dim msg As ErrorMessage
            Dim Services As vwListSPServiceCollection = Nothing
            Dim result As FetchServiceListResult = New FetchServiceListResult
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

                ' get the Provider list
                msg = SPClassesBL.FetchServices(conn, userID, ProviderID, page, pageSize, selectedServiceID, listFilterReference, listFilterName, totalRecords, Services)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Services = Services
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchServiceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

