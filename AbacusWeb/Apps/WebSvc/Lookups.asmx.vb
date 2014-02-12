
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

	''' <summary>
	''' Web service to retrieve various Abacus lookup data.
	''' </summary>
	''' <remarks></remarks>
	<System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/Lookups")> _
	<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
	<ToolboxItem(False)> _
	Public Class Lookups
        Inherits System.Web.Services.WebService

#Region " FetchFinancialAssessmentList "

        ''' <summary>
        ''' Fetches the paged financial assessment list.
        ''' </summary>
        ''' <param name="page">The page.</param>
        ''' <param name="selectedServiceID">The selected service ID.</param>
        ''' <param name="clientId">The client id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchFinancialAssessmentList(ByVal page As Integer, _
                                                     ByVal selectedServiceID As Integer, _
                                                     ByVal clientId As Integer) _
                                                     As Lookups_FetchFinancialAssessmentList

            Dim conn As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim pageSize As Integer = 10
            Dim result As Lookups_FetchFinancialAssessmentList = New Lookups_FetchFinancialAssessmentList()
            Dim totalRecords As Integer = 0
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try

                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of service types and throw error if not succeeded
                msg = AssessmentBL.GetPagedFinancialAssessments(conn, _
                                                                 page, _
                                                                 pageSize, _
                                                                 totalRecords, _
                                                                 clientId, _
                                                                 result.Results)

                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FinancialAssessmentSelector_FetchFinancialAssessmentList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                ' catch and wrap the unexpected error

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            Finally
                ' always close the connection

                SqlHelper.CloseConnection(conn)

            End Try

            Return result

        End Function

#End Region

#Region " FetchBudgetCategoryList "

        ''' <summary>
        ''' Fecthes a paged list of budget categories
        ''' </summary>
        ''' <param name="page">The page to fetch.</param>
        ''' <param name="selectedServiceID">The selected service id.</param>
        ''' <param name="budgetCategoryDescription">The budget category description.</param>
        ''' <param name="budgetCategoryReference">The budget category reference.</param>
        ''' <param name="budgetCategoryGroupDescription">The budget category group description.</param>
        ''' <param name="redundant">Whether to fetch redundant records, null = both.</param>
        ''' <param name="excludeIds">The ids to exclude from the result set.</param>
        ''' <param name="includeIds">The ids to include in the result set.</param>
        ''' <param name="includeServiceTypeIds">The service type ids to include in the result set.</param>
        ''' <returns>List of budget categories as Lookups_FetchBudgetCategoryListResult.</returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchBudgetCategoryList(ByVal page As Integer, _
                                                ByVal selectedServiceID As Integer, _
                                                 ByVal budgetCategoryDescription As String, _
                                                 ByVal budgetCategoryReference As String, _
                                                 ByVal budgetCategoryGroupDescription As String, _
                                                 ByVal redundant As Nullable(Of Boolean), _
                                                 ByVal excludeIds() As Integer, _
                                                 ByVal includeIds() As Integer, _
                                                 ByVal includeServiceTypeIds() As Integer) As Lookups_FetchBudgetCategoryListResult

            Dim conn As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim pageSize As Integer = 10
            Dim result As Lookups_FetchBudgetCategoryListResult = New Lookups_FetchBudgetCategoryListResult()
            Dim totalRecords As Integer = 0
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try

                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of service types and throw error if not succeeded
                msg = BudgetCategoryBL.FetchListWithPagingForInPlaceSelector(conn, _
                                                                             page, _
                                                                             pageSize, _
                                                                             totalRecords, _
                                                                             budgetCategoryDescription, _
                                                                             budgetCategoryReference, _
                                                                             budgetCategoryGroupDescription, _
                                                                             redundant, _
                                                                             excludeIds, _
                                                                             includeIds, _
                                                                             includeServiceTypeIds, _
                                                                             result.BudgetCategories)

                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:BudgetCategorySelector_FetchBudgetCategoryList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                ' catch and wrap the unexpected error

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            Finally
                ' always close the connection

                SqlHelper.CloseConnection(conn)

            End Try

            Return result

        End Function

#End Region

#Region " FetchCareManagerList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of care managers.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedCareManagerID">The ID of the care manager to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchCareManagerList(ByVal page As Integer, ByVal selectedCareManagerID As Integer, _
         ByVal listFilterReference As String, ByVal listFilterName As String) As FetchCareManagerListResult

            Dim msg As ErrorMessage
            Dim result As FetchCareManagerListResult = New FetchCareManagerListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of care managers
                msg = AbacusClassesBL.FetchCareManagerList(conn, page, pageSize, selectedCareManagerID, listFilterReference, listFilterName, totalRecords, result.CareManagers)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchCareManagerList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchClientGroupList "

        ''' <summary>
        ''' Retrieves a paginated list of client groups.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedClientGroupID">The ID of the client group to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchClientGroupList(ByVal page As Integer, ByVal selectedClientGroupID As Integer, _
         ByVal listFilterReference As String, ByVal listFilterName As String) As FetchClientGroupListResult

            Dim msg As ErrorMessage
            Dim result As FetchClientGroupListResult = New FetchClientGroupListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of teams
                msg = AbacusClassesBL.FetchClientGroupList(conn, page, pageSize, selectedClientGroupID, listFilterReference, listFilterName, totalRecords, result.ClientGroups)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchClientGroupList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchClientSubGroupList "

        ''' <summary>
        ''' Retrieves a paginated list of client sub groups.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedClientSubGroupID">The ID of the client group to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchClientSubGroupList(ByVal page As Integer, ByVal selectedClientSubGroupID As Integer, _
         ByVal listFilterReference As String, ByVal listFilterName As String) As FetchClientSubGroupListResult

            Dim msg As ErrorMessage
            Dim result As FetchClientSubGroupListResult = New FetchClientSubGroupListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of teams
                msg = AbacusClassesBL.FetchClientSubGroupList(conn, page, pageSize, selectedClientSubGroupID, listFilterReference, listFilterName, totalRecords, result.ClientSubGroups)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchClientSubGroupList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomServiceList "

        ''' <summary>
        ''' Retrieves a list of domiciliary services.
        ''' </summary>
        ''' <param name="redundant">To include/exclude/do not filter on the Redundant flag.</param>
        ''' <param name="forDomContracts">To include/exclude/do not filter on the ForUseWithDomiciliaryContracts flag.</param>
        ''' <param name="visitBasedReturns">To include/exclude/do not filter on the VisitBasedReturns flag.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     NOTE - the redundant/forDomContracts/visitBasedReturns parameters take a TriState value as follows:
        '''         Include = TriState.True
        '''         Exclude = TriState.False
        '''         Do Not Filter = TriState.UseDefault
        ''' </remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomServiceList(ByVal redundant As String, ByVal forDomContracts As String, _
                                            ByVal visitBasedReturns As String, _
                                            ByVal ServiceTypeID As Integer) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing
            Dim services As DomServiceCollection = Nothing
            Dim redundantTriState As TriState = [Enum].Parse(GetType(TriState), redundant)
            Dim forDomContractsTriState As TriState = [Enum].Parse(GetType(TriState), forDomContracts)
            Dim visitBasedReturnsTriState As TriState = [Enum].Parse(GetType(TriState), visitBasedReturns)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of service
                msg = DomService.FetchList(conn, services, redundantTriState, forDomContractsTriState, visitBasedReturnsTriState, ServiceTypeID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .List = New List(Of ViewablePair)
                    For Each ds As DomService In services
                        .List.Add(New ViewablePair(ds.ID, ds.Title))
                    Next
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

#Region " FetchDomProviderInvoiceSuspensionCommentList "

        ''' <summary>
        ''' Retrieves a list of Suspension Comments.
        ''' </summary>
        ''' <param name="suspensionReasonType">Filter on The Suspension Reason Type.</param>
        ''' <param name="suspensionReasonAutoType">Filter on the Suspension reason Auto type.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceSuspensionCommentList(ByVal suspensionReasonType As DomProviderInvoiceSuspensionReasonType, _
                                            ByVal suspensionReasonAutoType As DomProviderInvoiceSuspensionReasonAutoType) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing
            Dim reasons As vwSuspensionCommentCollection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomProviderInvoiceBL.FetchSuspensionReasons(conn, _
                                                              suspensionReasonType, _
                                                              suspensionReasonAutoType, _
                                                              reasons)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .List = New List(Of ViewablePair)
                    For Each comment As vwSuspensionComment In reasons
                        .List.Add(New ViewablePair(comment.ID, comment.Description))
                    Next
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

#Region " FetchDomProviderInvoiceNotesList "

        ''' <summary>
        ''' Retrieves a list of Suspension Notes.
        ''' </summary>
        ''' <param name="domProviderInvoiceID">Filter on provider Invoice ID.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceNotesList(ByVal domProviderInvoiceID As Integer) As FetchDomProviderInvoiceNotesResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderInvoiceNotesResult = New FetchDomProviderInvoiceNotesResult
            Dim Notes As New List(Of DomProviderInvoiceNotesEx)
            result.Notes = Notes
            Dim conn As SqlConnection = Nothing
            Dim reasons As vwSuspensionCommentCollection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomProviderInvoiceBL.FetchInvoiceNotes(conn, domProviderInvoiceID, result.Notes)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchFinanceCodeList "

        ''' <summary>
        ''' Retrieves a paginated list of finance codes.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedFinanceCodeID">The ID of the finance code to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchFinanceCodeList(ByVal page As Integer, _
                                              ByVal listFilterFinanceCode As String, ByVal listFilterFinanceCodeDescription As String, _
                                              ByVal selectedFinanceCodeID As Integer, ByVal category As Integer, _
                                              ByVal selectedExpenditureAccountGroupID As Integer) As FetchFinanceCodeListResult

            Dim msg As ErrorMessage
            Dim result As FetchFinanceCodeListResult = New FetchFinanceCodeListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                If selectedExpenditureAccountGroupID = 0 Then
                    ' get the list of FCs
                    msg = AbacusClassesBL.FetchFinanceCodeList(conn, page, pageSize, listFilterFinanceCode, _
                                                               listFilterFinanceCodeDescription, selectedFinanceCodeID, _
                                                               TriState.False, totalRecords, result.FinanceCodes, category)
                Else
                    'Get a list of finance codes linked to an expenditure account.
                    msg = FinanceCodeBL.FetchFinanceCodeListForExpenditureAccount(conn, page, pageSize, listFilterFinanceCode, _
                                                               listFilterFinanceCodeDescription, selectedFinanceCodeID, _
                                                               selectedExpenditureAccountGroupID, TriState.False, totalRecords, _
                                                               result.FinanceCodes, category)
                End If
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If


                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchFinanceCodeList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchJobScheduleList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of Job schedules.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchJobScheduleList(ByVal page As Integer) As FetchJobScheduleListResult

            Dim msg As ErrorMessage
            Dim result As FetchJobScheduleListResult = New FetchJobScheduleListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = AbacusClassesBL.FetchJobScheduleList(conn, page, pageSize, totalRecords, result.JobSchedules)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchJobScheduleList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchPctList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of primary care trusts.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedPctID">The ID of the PCT to select.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPctList(ByVal page As Integer, ByVal selectedPctID As Integer, ByVal listFilterName As String) As FetchPctListResult

            Dim msg As ErrorMessage
            Dim result As FetchPctListResult = New FetchPctListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = AbacusClassesBL.FetchPctList(conn, page, pageSize, selectedPctID, listFilterName, totalRecords, result.Pcts)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchPctList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchServiceGroupList "

        ''' <summary>
        ''' Retrieves a paginated list of service groups.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedServiceGroupID">The ID of the service group to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceGroupList(ByVal page As Integer, ByVal selectedServiceGroupID As Integer, _
                                              ByVal onlyShowGroupsAvailableToUser As Boolean) As FetchServiceGroupListResult

            Dim msg As ErrorMessage
            Dim result As FetchServiceGroupListResult = New FetchServiceGroupListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of SGs
                msg = AbacusClassesBL.FetchServiceGroupList(conn, page, pageSize, selectedServiceGroupID, TriState.False, _
                                                            totalRecords, onlyShowGroupsAvailableToUser, user.ID, result.ServiceGroups)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchServiceGroupList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchServiceList "

        ''' <summary>
        ''' Fetches a paged list of dom service types.
        ''' </summary>
        ''' <param name="page">The page to fetch, if lower than 1 will return first page.</param>
        ''' <param name="selectedServiceID">The selected service ID.</param>
        ''' <param name="serviceTitle">The service title.</param>
        ''' <param name="serviceDescription">The service description.</param>
        ''' <param name="redundant">Whether to include Redundant records, null = both.</param>
        ''' <param name="serviceTypeVisitBased">Whether to include service type visit based records.</param>
        ''' <param name="excludeIds">The ids to include, includeIds will override anything in this list. Use 0 in list to indicate null.</param>
        ''' <param name="includeIds">The ids to include.</param>
        ''' <param name="excludeIfNotAssociatedWithServiceType">Whether to exlude records if not assoicated with a service type record.</param>
        ''' <param name="forUseWithDomiciliaryContracts">For use with domiciliary contracts.</param>
        ''' <param name="includeUomIds">The uom ids to include. Use 0 in list to indicate null.</param>
        ''' <param name="includeServiceTypeIds">The service type ids to include. Use 0 in list to indicate null.</param>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceList(ByVal page As Integer, _
                                         ByVal selectedServiceID As Integer, _
                                         ByVal serviceTitle As String, _
                                         ByVal serviceDescription As String, _
                                         ByVal redundant As Nullable(Of Boolean), _
                                         ByVal serviceTypeVisitBased As Nullable(Of Boolean), _
                                         ByVal excludeIds() As Integer, _
                                         ByVal includeIds() As Integer, _
                                         ByVal excludeIfNotAssociatedWithServiceType As Nullable(Of Boolean), _
                                         ByVal forUseWithDomiciliaryContracts As Nullable(Of Boolean), _
                                         ByVal includeUomIds() As Integer, _
                                         ByVal includeServiceTypeIds() As Integer, _
                                         ByVal savedId As Integer) As Lookups_FetchServiceListResult

            Dim conn As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As Lookups_FetchServiceListResult = New Lookups_FetchServiceListResult()
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer = 0
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try

                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                ' get db connection
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of service types and throw error if not succeeded
                msg = AbacusClassesBL.FetchServiceList(conn, _
                                                       page, _
                                                       pageSize, _
                                                       totalRecords, _
                                                       selectedServiceID, _
                                                       serviceTitle, _
                                                       serviceDescription, _
                                                       redundant, _
                                                       serviceTypeVisitBased, _
                                                       excludeIds, _
                                                       includeIds, _
                                                       excludeIfNotAssociatedWithServiceType, _
                                                       forUseWithDomiciliaryContracts, _
                                                       includeUomIds, _
                                                       includeServiceTypeIds, _
                                                       savedId, _
                                                       result.Services)

                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:ServiceSelector_FetchServiceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                ' catch and wrap the unexpected error

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            Finally
                ' always close the connection

                SqlHelper.CloseConnection(conn)

            End Try

            Return result

        End Function

#End Region

#Region " FetchServiceTypeList "

        ''' <summary>
        ''' Fetches a paged list of service type records.
        ''' </summary>
        ''' <param name="page">The page to fetch, if lower than 1 will return first page.</param>
        ''' <param name="selectedServiceTypeID">The selected service type ID.</param>
        ''' <param name="serviceCategoryDescription">The service category description.</param>
        ''' <param name="serviceGroupClassificationDescription">The service group classification description.</param>
        ''' <param name="serviceGroupDescription">The service group description.</param>
        ''' <param name="redundant">Whether to include Redundant records, null = both.</param>
        ''' <param name="excludeIds">The ids to include, includeIds will override anything in this list. Use 0 in list to indicate null.</param>
        ''' <param name="excludeServiceCategories">The service categories to exclude.</param>
        ''' <param name="includeIds">The ids to include.</param>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceTypeList(ByVal page As Integer, _
                                             ByVal selectedServiceTypeID As Integer, _
                                             ByVal serviceCategoryDescription As String, _
                                             ByVal serviceGroupClassificationDescription As String, _
                                             ByVal serviceGroupDescription As String, _
                                             ByVal redundant As Nullable(Of Boolean), _
                                             ByVal excludeIds() As Integer, _
                                             ByVal excludeServiceCategories() As Integer, _
                                             ByVal includeIds() As Integer) As Lookups_FetchServiceTypeListResult

            Dim conn As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim pageSize As Integer = 10
            Dim result As Lookups_FetchServiceTypeListResult = New Lookups_FetchServiceTypeListResult()
            Dim totalRecords As Integer = 0
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try

                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                ' get db connection
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of service types and throw error if not succeeded
                msg = AbacusClassesBL.FetchServiceTypeList(conn, _
                                                           page, _
                                                           pageSize, _
                                                           totalRecords, _
                                                           selectedServiceTypeID, _
                                                           serviceCategoryDescription, _
                                                           serviceGroupClassificationDescription, _
                                                           serviceGroupDescription, _
                                                           redundant, _
                                                           excludeIds, _
                                                           excludeServiceCategories, _
                                                           includeIds, _
                                                           result.ServiceTypes)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:ServiceTypeSelector_FetchServiceTypeList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                ' catch and wrap the unexpected error

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            Finally
                ' always close the connection

                SqlHelper.CloseConnection(conn)

            End Try

            Return result

        End Function

#End Region

#Region " FetchTeamList "

        ''' <summary>
        ''' Retrieves a paginated list of team.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedTeamID">The ID of the team to select.</param>
        ''' <param name="availableToRes">To include/exclude/do not filter on the AvailableToRes flag.</param>
        ''' <param name="availableToDom">To include/exclude/do not filter on the AvailableToDom flag.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     NOTE - the availableToRes and availableToDom parameters take a TriState value as follows:
        '''         Include = TriState.True
        '''         Exclude = TriState.False
        '''         Do Not Filter = TriState.UseDefault
        ''' </remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchTeamList(ByVal page As Integer, ByVal selectedTeamID As Integer, _
         ByVal availableToRes As String, ByVal availableToDom As String, _
         ByVal listFilterReference As String, ByVal listFilterName As String) As FetchTeamListResult

            Dim msg As ErrorMessage
            Dim result As FetchTeamListResult = New FetchTeamListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim availRes As TriState = [Enum].Parse(GetType(TriState), availableToRes)
            Dim availDom As TriState = [Enum].Parse(GetType(TriState), availableToDom)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of teams
                msg = AbacusClassesBL.FetchTeamList(conn, page, pageSize, selectedTeamID, availRes, availDom, listFilterReference, listFilterName, totalRecords, result.Teams)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchTeamList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchRegisterGroupList "

        ''' <summary>
        ''' Retrieves a paginated list of register groups.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="filterFrameworkID">The ID of the framework to filter on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRegisterGroupList(ByVal page As Integer, ByVal filterFrameworkID As Integer) As FetchRegisterGroupListResult

            Dim msg As ErrorMessage
            Dim result As FetchRegisterGroupListResult = New FetchRegisterGroupListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                'get the list of SGs
                msg = AbacusClassesBL.FetchRegisterGroupList(conn, page, pageSize, filterFrameworkID, _
                                                            totalRecords, result.RegisterGroups)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchRegisterGroupList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomUnitsOfMeasureList "

        ''' <summary>
        ''' Retrieves a list of dom units of measure.
        ''' </summary>
        ''' <param name="visitBasedReturns">To include/exclude/do not filter on the VisitBasedReturns flag.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomUnitsOfMeasureList(ByVal visitBasedReturns As String) As Lookups_FetchDomUnitsOfMeasureListResult

            Dim msg As ErrorMessage
            Dim result As Lookups_FetchDomUnitsOfMeasureListResult = New Lookups_FetchDomUnitsOfMeasureListResult()
            Dim conn As SqlConnection = Nothing
            Dim visitBasedReturnsTriState As TriState = [Enum].Parse(GetType(TriState), visitBasedReturns)
            Dim dums As DomUnitsOfMeasureCollection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of dom units of measure
                msg = DomUnitsOfMeasure.FetchList(conn:=conn, list:=dums, auditUserName:=String.Empty, auditLogTitle:=String.Empty)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .UnitsOfMeasure = New List(Of DomUnitsOfMeasure)
                    For Each ds As DomUnitsOfMeasure In dums
                        If visitBasedReturnsTriState = TriState.UseDefault _
                            OrElse (visitBasedReturnsTriState = TriState.False AndAlso ds.MinutesPerUnit <= 0) _
                            OrElse (visitBasedReturnsTriState = TriState.True AndAlso ds.MinutesPerUnit > 0) Then
                            .UnitsOfMeasure.Add(ds)
                        End If

                    Next
                    .ErrorMsg = New ErrorMessage
                    .ErrorMsg.Success = True
                End With

            Catch ex As Exception
                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchBudgetPeriodList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of budget periods.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="pageLength">The size of the current page required.</param>
        ''' <param name="selectedBudgetPeriodID">The ID of the budget period to select.</param>
        ''' <param name="listBudgetYearID">The custom list filter string to apply to the BudgetYearID column.</param>
        ''' <param name="listTargetDate">The custom list filter string to apply to the DateFrom/To columns.</param>
        ''' <param name="listPeriodNum">The custom list filter string to apply to the PeriodNumber column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchBudgetPeriodList(ByVal page As Integer, ByVal pageLength As Integer, ByVal selectedBudgetPeriodID As Integer, _
         ByVal listBudgetYearID As Integer, ByVal listTargetDate As String, ByVal listPeriodNum As Integer) As FetchBudgetPeriodListResult

            Dim msg As ErrorMessage
            Dim result As FetchBudgetPeriodListResult = New FetchBudgetPeriodListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10

            Try
                '++ Check user is logged in..
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                '++ Get the list of budget periods..
                If pageLength > pageSize Then pageSize = pageLength
                msg = AbacusClassesBL.FetchBudgetPeriodList(conn, page, pageSize, selectedBudgetPeriodID, listBudgetYearID, listTargetDate, listPeriodNum, totalRecords, result.BudgetPeriods)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchBudgetPeriodList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDebtorInvoiceList "

        ''' <summary>
        ''' Retrieves a paginated list of debtor invoices.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedDebtorInvoiceID">The ID of the debtor invoice to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDebtorInvoiceList(ByVal page As Integer, ByVal selectedDebtorInvoiceID As Integer) As FetchDebtorInvoiceListResult

            Return FetchDebtorInvoiceListInternal(page, selectedDebtorInvoiceID, Nothing, Nothing, Nothing)

        End Function


        ''' <summary>
        ''' Retrieves a paginated list of debtor invoices.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedDebtorInvoiceID">The ID of the debtor invoice to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDebtorInvoiceListInternal(ByVal page As Integer, ByVal selectedDebtorInvoiceID As Integer, ByVal listFilterDebtor As String, _
                                               ByVal listFilterReference As String, ByVal listFilterInvNo As String) As FetchDebtorInvoiceListResult

            Dim msg As ErrorMessage
            Dim result As FetchDebtorInvoiceListResult = New FetchDebtorInvoiceListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of Debtor Invoices
                msg = DomContractBL.FetchDebtorInvoiceList(conn, page, pageSize, selectedDebtorInvoiceID, Nothing, True, True, True, _
                                                           True, True, True, True, True, True, True, True, _
                                                           True, True, True, True, True, Nothing, Nothing, Nothing, _
                                                           Nothing, Nothing, listFilterDebtor, listFilterInvNo, listFilterReference, Nothing, Nothing, _
                                                           totalRecords, result.Invoices)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDebtorInvoiceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchSelectorDebtorInvoiceList "

        ''' <summary>
        ''' Retrieves a paginated list of debtor invoices.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedDebtorInvoiceID">The ID of the debtor invoice to select.</param>
        ''' <param name="listFilterDebtor">The debtor name search filter</param>
        ''' <param name="listFilterReference">The reference search filter</param>
        ''' <param name="listFilterInvNo">The invoice no search filter</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSelectorDebtorInvoiceList(ByVal page As Integer, ByVal selectedDebtorInvoiceID As Integer, ByVal listFilterDebtor As String, _
                                                       ByVal listFilterReference As String, ByVal listFilterInvNo As String) As FetchSelectorDebtorInvoiceListResult

            Dim msg As ErrorMessage
            Dim result As FetchSelectorDebtorInvoiceListResult = New FetchSelectorDebtorInvoiceListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of Debtor Invoices
                msg = DomContractBL.FetchSelectorDebtorInvoiceList(conn, page, pageSize, selectedDebtorInvoiceID, False, Nothing, True, True, True, _
                                                           True, True, True, True, True, True, True, True, _
                                                           True, True, True, True, True, Nothing, Nothing, Nothing, _
                                                           Nothing, Nothing, listFilterDebtor, listFilterInvNo, listFilterReference, Nothing, Nothing, totalRecords, result.Invoices)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchSelectorDebtorInvoiceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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