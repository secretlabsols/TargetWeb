
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports DPI = Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Abacus.Library.SDS
Imports Target.Web.Apps.WebServices.Responses

Namespace Apps.WebSvc

	''' <summary>
	''' Web service to retrieve Direct Payment contract information.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Waqas   14/02/2011  D12009 : Updated to add SDS filter 
    ''' MikeVO  03/12/2010  SDS issues #398 - corrected paging link JS in FetchDPContractList().
    ''' </history>
	<System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/DPContract")> _
	<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
	<ToolboxItem(False)> _
	Public Class DPContract
        Inherits System.Web.Services.WebService

#Region "Fields"

        ' locals
        Private _ConnectionString As String

        ' constants
        Private Const _ConnectionStringKey As String = "Abacus"
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber

#End Region

#Region "Functions"

#Region " FetchDPContractList "

        ''' <summary>
        ''' Retrieves a paginated list of Direct Payment contracts.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedDPContractID">The ID of the contract to select.</param>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="budgetHolderID">The ID of the budget holder to filter the results on.</param>
        ''' <param name="dateFrom">The start of the period to filter the results on.</param>
        ''' <param name="dateTo">The end of the period to filter the result son.</param>
        ''' <param name="listFilterNumber">The custom list filter string to apply to the Number column.</param>
        ''' <param name="listFilterSURef">The custom list filter string to apply to the S/U Ref column.</param>
        ''' <param name="listFilterSUName">The custom list filter string to apply to the S/U Name column.</param>
        ''' <param name="listFilterBHRef">The custom list filter string to apply to the B/H Ref column.</param>
        ''' <param name="listFilterBHName">The custom list filter string to apply to the B/H Name column.</param>
        ''' <param name="listFilterIsSDS"> The SDS filter Tristate to apply</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDPContractList(ByVal page As Integer, ByVal selectedDPContractID As Integer, _
                     ByVal clientID As Integer, ByVal budgetHolderID As Integer, _
                     ByVal dateFrom As Date, ByVal dateTo As Date, ByVal listFilterNumber As String, _
                     ByVal listFilterSURef As String, ByVal listFilterSUName As String, _
                     ByVal listFilterBHRef As String, ByVal listFilterBHName As String, _
                     ByVal listFilterIsSDS As TriState) As FetchDPContractListResult

            Dim msg As ErrorMessage
            Dim result As FetchDPContractListResult = New FetchDPContractListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                '++ Check user is logged in..
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                '++ Get the list of Direct Payment contracts..
                msg = DPContractBL.FetchDPContractList( _
                 conn, page, pageSize, selectedDPContractID, clientID, budgetHolderID, dateFrom, dateTo, _
                 listFilterNumber, listFilterSURef, listFilterSUName, listFilterBHRef, listFilterBHName, _
                 listFilterIsSDS, totalRecords, result.Contracts)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:DPContractSelector_FetchDPContractList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " DisplaySpansMultipleFrequenciesMessage "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     determines if a message should be displayed in the UI when saving 
        '''     an sds DP Contract Payment.
        ''' </summary>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="dateFrom">The start of the period to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DisplaySpansMultipleFrequenciesMessage(ByVal clientID As Integer, _
                                                           ByVal dateFrom As String, _
                                                           ByVal dateTo As String, _
                                                           ByVal frequencyID As Integer, _
                                                           ByVal noPayments As Integer) As Boolean

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim freq As Lookup = Nothing
            Dim minDateTo As Date = Date.MaxValue
            Dim freqDates As List(Of Date)
            Dim spansMultiplePeriods As Boolean = False
            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return False
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                'get the frequency used

                freq = New Lookup(conn)
                msg = freq.Fetch(frequencyID)
                If Not msg.Success Then Return False


                msg = ServiceUserBudgetPeriodBL.GetMaxDateToForClient(conn, clientID, minDateTo)
                If Not msg.Success Then Return False

                If dateTo = "(open-ended)" Then dateTo = Date.MaxValue
                If minDateTo > dateTo Then
                    minDateTo = dateTo
                End If

                freqDates = DateUtility.GetDatesByDateRangeAndFrequencyAsDays(dateFrom, minDateTo, freq.InfoString, False)

                Dim index As Integer = 1
                For Each dtFrom As Date In freqDates
                    If index <> freqDates.Count Then
                        msg = DPContractBL.DPPaymentSpansMultipleBudgetPeriods(conn, clientID, dtFrom, freqDates(index).AddDays(-1), spansMultiplePeriods)
                    Else
                        msg = DPContractBL.DPPaymentSpansMultipleBudgetPeriods(conn, clientID, dtFrom, minDateTo.AddDays(-1), spansMultiplePeriods)
                    End If
                    If spansMultiplePeriods Then Return spansMultiplePeriods
                    index += 1
                Next


            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return False

        End Function

#End Region

#Region " GetPaymentsInBudgetPeriods "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     determines if a message should be displayed in the UI when saving 
        '''     an sds DP Contract Payment.
        ''' </summary>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="dateFrom">The start of the period to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetPaymentsInBudgetPeriods(ByVal clientID As Integer, _
                                                    ByVal dateFrom As String, _
                                                    ByVal dateTo As String, _
                                                    ByVal frequencyID As Integer, _
                                                    ByVal dpContractID As Integer, _
                                                    ByVal paidToDate As String) As GetPaymentsInBudgetPeriodsResult

            Dim msg As New ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim result As GetPaymentsInBudgetPeriodsResult = New GetPaymentsInBudgetPeriodsResult
            Dim freq As Lookup = Nothing
            Dim maxDateTo As Date = Date.MaxValue
            Dim freqDates As List(Of Date)
            Dim spansMultiplePeriods As Boolean = False
            Dim dpc As Target.Abacus.Library.DataClasses.DPContract = Nothing
            Dim noPaymentsPaidInCurrentPeriod As Integer = 0
            Dim noPaymentsPaidInNextPeriod As Integer = 0
            Dim noPaymentsDueInCurrentPeriod As Integer = 0
            Dim noPaymentsDueInNextPeriod As Integer = 0
            Dim paymentOverlapps As Boolean = False
            Dim paidUpToDate As Date
            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                If dateTo = "(open-ended)" Then dateTo = Date.MaxValue

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                If paidToDate = "" Then
                    paidUpToDate = Date.MinValue
                Else
                    paidUpToDate = Convert.ToDateTime(paidToDate)
                End If

                'Get the contract
                dpc = New Target.Abacus.Library.DataClasses.DPContract(conn, String.Empty, String.Empty)
                msg = dpc.Fetch(dpContractID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                If dateFrom <> "" Then
                    'Populate the Payments in budget periods section.
                    Dim currentPeriod As ClientBudgetPeriod = Nothing
                    Dim nextPeriod As ClientBudgetPeriod = Nothing
                    Dim nextPeriodFromDate As Date = Date.MaxValue
                    'Fetch the current budget period
                    msg = ServiceUserBudgetPeriodBL.GetClientBudgetPeriodForDate(conn, clientID, dateFrom, currentPeriod)
                    If Not msg.Success Then
                        result.ErrMsg = msg
                        Return result
                    End If

                    If Not currentPeriod Is Nothing Then
                        result.CurrentPeriod = String.Format("{0} to {1}", currentPeriod.DateFrom.ToShortDateString, _
                                                                 currentPeriod.DateTo.ToShortDateString)
                        maxDateTo = currentPeriod.DateTo
                        'get the start date of the next period
                        nextPeriodFromDate = currentPeriod.DateTo.AddDays(1)
                    Else
                        result.CurrentPeriod = "No period available"
                    End If
                    'fetch the next budget period
                    If nextPeriodFromDate <> Date.MaxValue Then
                        msg = ServiceUserBudgetPeriodBL.GetClientBudgetPeriodForDate(conn, clientID, nextPeriodFromDate, nextPeriod)
                        If Not msg.Success Then
                            result.ErrMsg = msg
                            Return result
                        End If
                        If Not nextPeriod Is Nothing Then
                            result.NextPeriod = String.Format("{0} to {1}", nextPeriod.DateFrom.ToShortDateString, _
                                                                  nextPeriod.DateTo.ToShortDateString)

                            maxDateTo = nextPeriod.DateTo
                        Else
                            result.NextPeriod = "No period available"
                        End If
                    Else
                        result.NextPeriod = "No period available"
                    End If

                    'The max date to can not be greater than the date to of the payment
                    If dateTo < maxDateTo Then maxDateTo = dateTo
                    'the max date to can not be greater than the date to of the contract
                    If dpc.DateTo < maxDateTo Then maxDateTo = dpc.DateTo

                    'get the frequency used
                    If frequencyID > 0 Then
                        freq = New Lookup(conn)
                        msg = freq.Fetch(frequencyID)
                        If Not msg.Success Then
                            result.ErrMsg = msg
                            Return result
                        End If

                        'Get the dates on which payments will be due.
                        freqDates = DateUtility.GetDatesByDateRangeAndFrequencyAsDays(dateFrom, maxDateTo, freq.InfoString, False)

                        'loop through the list of payment dates, working out the totals
                        Dim index As Integer = 1
                        For Each paymentDate As Date In freqDates
                            If Not currentPeriod Is Nothing Then
                                If paymentDate >= currentPeriod.DateFrom And paymentDate <= currentPeriod.DateTo Then
                                    If paymentDate >= paidUpToDate Then
                                        noPaymentsDueInCurrentPeriod += 1
                                    Else
                                        noPaymentsPaidInCurrentPeriod += 1
                                    End If
                                End If
                            End If
                            If Not nextPeriod Is Nothing Then
                                If paymentDate >= nextPeriod.DateFrom And paymentDate <= nextPeriod.DateTo Then
                                    If paymentDate >= paidUpToDate Then
                                        noPaymentsDueInNextPeriod += 1
                                    Else
                                        noPaymentsPaidInNextPeriod += 1
                                    End If
                                End If
                            End If
                            If Not currentPeriod Is Nothing AndAlso Not nextPeriod Is Nothing Then
                                If index <> freqDates.Count Then

                                    If (paymentDate >= currentPeriod.DateFrom And paymentDate <= currentPeriod.DateTo) _
                                        And (freqDates(index).AddDays(-1) > currentPeriod.DateTo) Then
                                        paymentOverlapps = True
                                    End If


                                Else
                                    If (paymentDate >= currentPeriod.DateFrom And paymentDate <= currentPeriod.DateTo) _
                                        And (maxDateTo.AddDays(-1) > currentPeriod.DateTo) Then
                                        paymentOverlapps = True
                                    End If
                                End If
                            End If
                            index += 1
                        Next
                    ElseIf frequencyID = 0 Then
                        'This assumes that the frequency ONCE has been selected
                        If paidUpToDate <> Date.MinValue Then
                            noPaymentsPaidInCurrentPeriod += 1
                        Else
                            noPaymentsDueInCurrentPeriod += 1
                        End If
                    End If
                End If

                msg = New ErrorMessage
                msg.Success = True

                With result
                    If result.CurrentPeriod <> "No period available" Then
                        .CurrentPaymentsMade = String.Format("{0} payments have been paid", noPaymentsPaidInCurrentPeriod)
                        .currentPaymentsToBePaid = String.Format("{0} payments will be paid", noPaymentsDueInCurrentPeriod)
                    Else
                        .CurrentPaymentsMade = String.Empty
                        .currentPaymentsToBePaid = String.Empty
                    End If
                    If result.NextPeriod <> "No period available" Then
                        .NextPaymentsMade = String.Format("{0} payments have been paid", noPaymentsPaidInNextPeriod)
                        .NextPaymentsToBePaid = String.Format("{0} payments will be paid", noPaymentsDueInNextPeriod)
                    Else
                        .NextPaymentsMade = String.Empty
                        .NextPaymentsToBePaid = String.Empty
                    End If
                    If paymentOverlapps Then
                        .OvelappingPayments = "A payment will overlap the current and next periods."
                    Else
                        .OvelappingPayments = String.Empty
                    End If
                    .ErrMsg = msg
                End With

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region "GetProjectedTerminationDetailsForContract"

        ''' <summary>
        ''' Gets the projected termination details for contract.
        ''' </summary>
        ''' <param name="dpContractId">The dp contract id.</param>
        ''' <param name="dpContractRequiredEndDate">The dp contract required end date.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="GetProjectedTerminationDetailsForContract."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetProjectedTerminationDetailsForContract(ByVal dpContractId As Integer, ByVal dpContractRequiredEndDate As DateTime) As WebServiceReponseWithItems(Of ViewableDPContractDetailTerminationProjection)

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New WebServiceReponseWithItems(Of ViewableDPContractDetailTerminationProjection)
            Dim currentUser As WebSecurityUser = Nothing

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                With result
                    .ErrMsg = SecurityBL.ValidateLogin()
                    If Not .ErrMsg.Success Then Return result
                End With

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the projected items
                With result
                    .ErrMsg = DPContractBL.GetProjectedTerminationDetailsForContract(connection, dpContractId, dpContractRequiredEndDate, result.Items)
                    If Not .ErrMsg.Success Then Return result
                End With

            Catch ex As Exception
                ' wrap the exception and add to the result

                With result
                    .ErrMsg = Utils.CatchError(ex, _GeneralErrorNumber)
                    .ErrMsg.Success = False
                End With

            Finally
                ' finally close the db connection 

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

#End Region

#Region "GetPagedTerminatedContracts"

        ''' <summary>
        ''' Gets the terminated contract.
        ''' </summary>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="GetPagedTerminatedContracts."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetPagedTerminatedContracts(ByVal page As Integer, _
                                                     ByVal selectedId As Integer, _
                                                     ByVal isSds As Nullable(Of Boolean), _
                                                     ByVal isUnderPaid As Nullable(Of Boolean), _
                                                     ByVal isBalanced As Nullable(Of Boolean), _
                                                     ByVal terminatedDateFrom As Nullable(Of DateTime), _
                                                     ByVal terminatedDateTo As Nullable(Of DateTime), _
                                                     ByVal serviceUserName As String, _
                                                     ByVal serviceUserReference As String, _
                                                     ByVal budgetHolderName As String, _
                                                     ByVal budgetHolderReference As String, _
                                                     ByVal contractNumber As String) _
                                                     As WebServiceReponseWithItemsAndPagingLinks(Of ViewableDpContractTermination)

            Dim connection As SqlConnection = Nothing
            Dim currentUser As WebSecurityUser = Nothing
            Dim msg As New ErrorMessage()
            Dim pageSize As Integer = 10
            Dim result As New WebServiceReponseWithItemsAndPagingLinks(Of ViewableDpContractTermination)()
            Dim totalRecords As Integer = 0

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                With result
                    .ErrMsg = SecurityBL.ValidateLogin()
                    If Not .ErrMsg.Success Then Return result
                End With

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the projected items
                With result
                    .ErrMsg = DPContractBL.GetPagedTerminatedContracts(connection, page, pageSize, totalRecords, selectedId, isSds, isUnderPaid, isBalanced, terminatedDateFrom, terminatedDateTo, serviceUserName, serviceUserReference, budgetHolderName, budgetHolderReference, contractNumber, .Items)
                    If Not .ErrMsg.Success Then Return result
                    .SetPagingLinks("FetchTerminatedContracts", page, pageSize, totalRecords)
                End With

            Catch ex As Exception
                ' wrap the exception and add to the result

                With result
                    .ErrMsg = Utils.CatchError(ex, _GeneralErrorNumber)
                    .ErrMsg.Success = False
                End With

            Finally
                ' finally close the db connection 

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

#End Region

#Region "TerminateDpContract"

        ''' <summary>
        ''' Terminates the dp contract.
        ''' </summary>
        ''' <param name="dpContractId">The dp contract id.</param>
        ''' <param name="endReasonId">The end reason id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="TerminateDpContract."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function TerminateDpContract(ByVal dpContractId As Integer, ByVal endDate As DateTime, ByVal endReasonId As Integer) As WebServiceResponseBase

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New WebServiceResponseBase()


            Try

                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim auditLogName As String = currentUser.ExternalFullName
                Dim auditLogTitle As String = "AbacusIntranet:Terminate Direct Payment Contract"

                ' check the requesting user is logged in
                With result
                    .ErrMsg = SecurityBL.ValidateLogin()
                    If Not .ErrMsg.Success Then Return result
                End With

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the projected items
                With result
                    .ErrMsg = DPContractBL.TerminateDpContract(connection, dpContractId, endReasonId, endDate, currentUser.ExternalUsername, auditLogTitle)
                    If Not .ErrMsg.Success Then Return result
                End With

            Catch ex As Exception
                ' wrap the exception and add to the result

                With result
                    .ErrMsg = Utils.CatchError(ex, _GeneralErrorNumber)
                    .ErrMsg.Success = False
                End With

            Finally
                ' finally close the db connection 

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

#End Region

#Region "CreateBalancingPaymentForDpContract"

        ''' <summary>
        ''' Creates the balancing payment for dp contract.
        ''' </summary>
        ''' <param name="dpContractId">The dp contract id.</param>
        ''' <param name="balancingAmount">The balancing amount.</param>
        ''' <param name="periodFrom">The period from.</param>
        ''' <param name="periodTo">The period to.</param>
        ''' <param name="excludeFromCreditors">if set to <c>true</c> [exclude from creditors].</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="CreateBalancingPaymentForDpContract."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CreateBalancingPaymentForDpContract(ByVal dpContractId As Integer, ByVal balancingAmount As Decimal, ByVal periodFrom As DateTime, ByVal periodTo As DateTime, ByVal excludeFromCreditors As Boolean) As WebServiceResponseBase

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New WebServiceResponseBase()

            Try

                Dim createdItem As DPContractDetail = Nothing
                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim triExcludeFromCreditors As TriState = TriState.False

                ' check the requesting user is logged in
                With result
                    .ErrMsg = SecurityBL.ValidateLogin()
                    If Not .ErrMsg.Success Then Return result
                End With

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                If excludeFromCreditors = True Then

                    triExcludeFromCreditors = TriState.True

                End If

                ' create the payment
                With result
                    .ErrMsg = DPContractBL.CreateBalancingPaymentForDpContract(connection, dpContractId, balancingAmount, periodFrom, periodTo, triExcludeFromCreditors, createdItem)
                    If Not .ErrMsg.Success Then Return result
                End With

            Catch ex As Exception
                ' wrap the exception and add to the result

                With result
                    .ErrMsg = Utils.CatchError(ex, _GeneralErrorNumber)
                    .ErrMsg.Success = False
                End With

            Finally
                ' finally close the db connection 

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

#End Region

#Region "MarkDpContractAsBalanced"


        ''' <summary>
        ''' Marks the dp contract as balanced.
        ''' </summary>
        ''' <param name="dpContractId">The dp contract id.</param>
        ''' <param name="isBalanced">if set to <c>true</c> [is balanced].</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="MarkDpContractAsBalanced."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function MarkDpContractAsBalanced(ByVal dpContractId As Integer, ByVal isBalanced As Boolean) As WebServiceResponseBase

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New WebServiceResponseBase()

            Try

                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim auditLogName As String = currentUser.ExternalFullName
                Dim auditLogTitle As String = "AbacusIntranet:Balance Direct Payment Contract"

                ' check the requesting user is logged in
                With result
                    .ErrMsg = SecurityBL.ValidateLogin()
                    If Not .ErrMsg.Success Then Return result
                End With

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' create the payment
                With result
                    .ErrMsg = DPContractBL.MarkDpContractAsBalanced(connection, dpContractId, isBalanced, auditLogName, auditLogTitle)
                    If Not .ErrMsg.Success Then Return result
                End With

            Catch ex As Exception
                ' wrap the exception and add to the result

                With result
                    .ErrMsg = Utils.CatchError(ex, _GeneralErrorNumber)
                    .ErrMsg.Success = False
                End With

            Finally
                ' finally close the db connection 

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

#End Region

#Region "ReinstateDpContract"

        ''' <summary>
        ''' Marks the dp contract as balanced.
        ''' </summary>
        ''' <param name="dpContractId">The dp contract id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="ReinstateDpContract."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ReInstateDpContract(ByVal dpContractId As Integer) As WebServiceResponseBase

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New WebServiceResponseBase()

            Try

                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                Dim auditLogName As String = currentUser.ExternalFullName
                Dim auditLogTitle As String = "AbacusIntranet:Re-instate Direct Payment Contract"

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' create the payment
                With result
                    .ErrMsg = DPContractBL.ReinstateDpContract(connection, dpContractId, auditLogName, auditLogTitle)
                    If Not .ErrMsg.Success Then Return result
                End With

            Catch ex As Exception
                ' wrap the exception and add to the result

                With result
                    .ErrMsg = Utils.CatchError(ex, _GeneralErrorNumber)
                    .ErrMsg.Success = False
                End With

            Finally
                ' finally close the db connection 

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

#End Region

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the connection string.
        ''' </summary>
        ''' <value>The connection string.</value>
        Public ReadOnly Property ConnectionString() As String
            Get
                If String.IsNullOrEmpty(_ConnectionString) OrElse _ConnectionString.Trim().Length = 0 Then
                    ' if we havent already fetched the connection string then do so
                    _ConnectionString = ConnectionStrings(_ConnectionStringKey).ConnectionString
                End If
                Return _ConnectionString
            End Get
        End Property

#End Region

    End Class

End Namespace