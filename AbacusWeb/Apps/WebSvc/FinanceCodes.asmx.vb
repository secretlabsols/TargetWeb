
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
    ''' Class	 : Abacus.Web.Apps.WebSvc.FinanceCodes
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve Finance Code information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul]	12/02/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/FinanceCodes")> _
    Public Class FinanceCodes
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

#Region " FetchOtherFundingOrganizationList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of organizations.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="selectedOrgID">The ID of the organization to select.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedOrganizationID overrides the specified page parameter such that
        '''     the page which contain the requested organization is displayed. 
        ''' </remarks>
        ''' <history>
        ''' 	[Paul]	12/02/2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchOtherFundingOrganizationList(ByVal page As Integer, ByVal selectedOrgID As Integer, _
         ByVal orgType As Integer, ByVal listFilterName As String) As FetchOtherFundingOrganizationListResult

            Dim msg As ErrorMessage
            Dim orgs As ArrayList = Nothing
            Dim result As FetchOtherFundingOrganizationListResult = New FetchOtherFundingOrganizationListResult
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
                userID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of homes
                msg = FinanceCodeBL.FetchOtherFundingOrganizations( _
                   conn, page, pageSize, selectedOrgID, orgType, _
                   listFilterName, totalRecords, orgs)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Organizations = orgs
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                     "<a href=""javascript:FetchOrgList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchExpenditureAccountList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of expenditure Accounts.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="selectedAccountID">The ID of the Expenditure Account to select.</param>
        ''' <param name="serviceTypeID">The ID of the Service Type to filter the results on</param>
        ''' <param name="expenditureAccType">The ID of the expenditure account type to filter the results on</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedOrganizationID overrides the specified page parameter such that
        '''     the page which contain the requested Expenditure Account is displayed. 
        ''' </remarks>
        ''' <history>
        ''' 	[Paul]	18/02/2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchExpenditureAccountList(ByVal page As Integer, ByVal selectedAccountID As Integer, _
         ByVal serviceTypeID As Integer, ByVal expenditureAccType As Integer, ByVal listFilterName As String) As FetchExpenditureAccountListResult

            Dim msg As ErrorMessage
            Dim accounts As ArrayList = Nothing
            Dim result As FetchExpenditureAccountListResult = New FetchExpenditureAccountListResult
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
                userID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of homes
                msg = FinanceCodeBL.FetchExpenditureAccounts( _
                   conn, page, pageSize, selectedAccountID, serviceTypeID, _
                   expenditureAccType, listFilterName, totalRecords, accounts)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Accounts = accounts
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                     "<a href=""javascript:FetchAccountList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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