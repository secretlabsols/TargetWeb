
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports System.Collections.Generic

Namespace Apps.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.PersonalBudgetStatements
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve Personal Budget Statement information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Iftikhar]	02/03/2011	D11854 - Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/PersonalBudgetStatements")> _
    Public Class PersonalBudgetStatements
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

#Region " FetchPersonalBudgetStatementList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of personalBudgetStatements..
        ''' </summary>
        ''' <param name="currentPage"></param>
        ''' <param name="clientID"></param>
        ''' <param name="selectedPersonalBudgetStatementID"></param>
        ''' <param name="listFilterReference"></param>
        ''' <param name="listFilterDateFrom"></param>
        ''' <param name="listFilterDateTo"></param>
        ''' <param name="listFilterDateCreated"></param>
        ''' <param name="listFilterCreatedBy"></param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedPersonalBudgetStatementID overrides the specified page parameter such that
        '''     the page which contain the requested personal budget statement is displayed. 
        ''' </remarks>
        ''' <history>
        ''' 	[Iftikhar]	03/03/2011	D11854 - Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPersonalBudgetStatementList(ByVal currentPage As Integer, _
                                                         ByVal clientID As Integer, _
                                                         ByVal selectedPersonalBudgetStatementID As Integer, _
                                                         ByVal listFilterReference As String, _
                                                         ByVal listFilterDateFrom As String, _
                                                         ByVal listFilterDateTo As String, _
                                                         ByVal listFilterDateCreated As String, _
                                                         ByVal listFilterCreatedBy As String) _
                                                         As FetchPersonalBudgetStatementsListResult

            Dim msg As ErrorMessage
            Dim personalBudgetStatements As List(Of ViewablePersonalBudgetStatement) = Nothing
            Dim result As FetchPersonalBudgetStatementsListResult = New FetchPersonalBudgetStatementsListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of personal budget statements
                msg = AbacusClassesBL.FetchPersonalBudgetStatements(conn, currentPage, pageSize, _
                                                                    clientID, selectedPersonalBudgetStatementID, _
                                                                    listFilterReference, listFilterDateFrom, _
                                                                    listFilterDateTo, listFilterDateCreated, _
                                                                    listFilterCreatedBy, totalRecords, _
                                                                    personalBudgetStatements)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PersonalBudgetStatements = personalBudgetStatements
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                     "<a href=""javascript:FetchObjectList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                     currentPage, Math.Ceiling(totalRecords / pageSize))
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