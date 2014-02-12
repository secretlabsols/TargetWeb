Imports System.Web.Services
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.SP.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.SP.Library.Collections
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/SPWeb/Apps/WebSvc/Subsidies")> _
    Public Class Subsidies
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


#Region " FetchSubsidiesList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of subsidies.
        ''' </summary>
        ''' <param name="page">The page to display.</param>
        ''' <param name="ServiceID">The ID of the service.</param>
        ''' <param name="ClientID">The ID of the client.</param>
        ''' <param name="ProviderID">The ID of the provider.</param>
        ''' <param name="DateFrom">The start of the date range.</param>
        ''' <param name="DateTo">The end of the date range.</param>
        ''' <param name="Status">The statuc values to include.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <param name="sortColumn">The index of the available sortable columns to sort on.</param>
        ''' <param name="sortDirection">The direction of the sort.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        '''     MikeVO  19/03/2007  Added sorting support.
        '''     MikeVO  14/12/2006  Added support for Reference and Name list filters.
        ''' 	[paul]	03/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSubsidiesList(ByVal page As Integer, ByVal ServiceID As Integer, ByVal ClientID As Integer, _
                        ByVal ProviderID As Integer, ByVal DateFrom As Date, ByVal DateTo As Date, ByVal Status As Integer, _
                        ByVal listFilterReference As String, ByVal listFilterName As String, _
                        ByVal sortColumn As Byte, ByVal sortDirection As Byte) As FetchSubsidiesListResult

            Dim msg As ErrorMessage
            Dim Subsidy As vwListSPSubsidiesCollection = Nothing
            Dim result As FetchSubsidiesListResult = New FetchSubsidiesListResult
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

                ' get the Property list
                msg = SPClassesBL.FetchSubsidies(conn, ServiceID, ClientID, ProviderID, DateFrom, DateTo, Status, page, pageSize, listFilterReference, listFilterName, sortColumn, sortDirection, totalRecords, Subsidy)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Subsidies = Subsidy
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchSubsidyList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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
