
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.BudgetHolders
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve establishment information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     ColinD  15/04/2011  Updated SDS585 - Updated FetchBudgetHolderList showRedundant parameter to read redundant to reflect its use in conjunction with stored procedure i.e. null = either redundant or non-redundant.
    ''' 	JohnF	15/07/2010	Created (D11801)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/BudgetHolders")> _
    Public Class BudgetHolders
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

#Region " FetchBudgetHolderList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of budget holders.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="budgetHolderID">The ID of the budget holder to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the Reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the Name column.</param>
        ''' <param name="listFilterCreditorRef">The custom list filter string to apply to the Creditor Reference column.</param>
        ''' <param name="listFilterOrg">The custom list filter string to apply to the Organisation column.</param>
        ''' <param name="redundant">Indicates whether to include redundant/non-redundant items. Null = both.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedBudgetHolderID overrides the specified page parameter such that
        '''     the page which contain the requested provider is displayed. 
        ''' </remarks>
        ''' <history>
        ''' 	JohnF	15/07/2010	Created (D11801)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchBudgetHolderList(ByVal page As Integer, ByVal budgetHolderID As Integer, _
          ByVal listFilterReference As String, ByVal listFilterCreditorRef As String, _
          ByVal listFilterName As String, ByVal listFilterOrg As String, _
          ByVal redundant As Nullable(Of Boolean), ByVal serviceUserID As Nullable(Of Integer)) As FetchBudgetHolderListResult

            Dim msg As ErrorMessage
            Dim budholders As Generic.List(Of Target.Abacus.Library.ViewableBudgetHolder)
            Dim result As FetchBudgetHolderListResult = New FetchBudgetHolderListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim webUser As WebSecurityUser
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                webUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                '++ Get the list of budget holders..
                budholders = New Generic.List(Of ViewableBudgetHolder)
                msg = BudgetHolderBL.FetchBudgetHolders( _
                   conn, page, pageSize, budgetHolderID, listFilterReference, listFilterCreditorRef, _
                   listFilterName, listFilterOrg, totalRecords, budholders, redundant, serviceUserID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .BudgetHolders = budholders
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchBudHolderList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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