Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library.CreditorPayments
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to manage creditor payments
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D11874 Created 10/02/2010
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/CreditorPayments")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class CreditorPayments
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
        ''' Gets the generic creditor payment suspension comments.
        ''' </summary>
        ''' <param name="type">The type.</param>
        ''' <param name="systemType">Type of the system.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Get GenericCreditorPaymentSuspensionComment records"), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Shared Function GetGenericCreditorPaymentSuspensionComments(ByVal type As CreditorPaymentsBL.GenericCreditorPaymentSuspensionCommentType, _
                                                                            ByVal systemType As CreditorPaymentsBL.GenericCreditorPaymentSuspensionCommentSystemType) _
                                                                            As CreditorPayments_GetvwSuspensionCommentCollectionCommentsResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New CreditorPayments_GetvwSuspensionCommentCollectionCommentsResult()

            Try

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

                ' get the list of service types and throw error if not succeeded
                msg = CreditorPaymentsBL.GetGenericCreditorPaymentSuspensionComments(connection, type, systemType, result.Items)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage()
                    .ErrorMsg.Success = True
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

        ''' <summary>
        ''' Gets the paged generic contracts.
        ''' </summary>
        ''' <param name="page">The page.</param>
        ''' <param name="selectedDomContractID">The selected DOM contract ID.</param>
        ''' <param name="establishmentID">The establishment ID.</param>
        ''' <param name="contractType">Type of the contract.</param>
        ''' <param name="contractGroupID">The contract group ID.</param>
        ''' <param name="dateFrom">The date from.</param>
        ''' <param name="dateTo">The date to.</param>
        ''' <param name="listFilterNumber">The list filter number.</param>
        ''' <param name="listFilterTitle">The list filter title.</param>
        ''' <param name="listFilterSU">The list filter SU.</param>
        ''' <param name="listFilterGroup">The list filter group.</param>
        ''' <param name="listFilterSvcGroup">The list filter SVC group.</param>
        ''' <param name="contractEndReasonID">The contract end reason ID.</param>
        ''' <param name="serviceGroupID">The service group ID.</param>
        ''' <param name="serviceGroupClassificationID">The service group classification ID.</param>
        ''' <param name="listFilterCreditor">The list filter creditor.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetPagedGenericContracts(ByVal page As Integer, _
                                                 ByVal selectedDomContractID As Integer, _
                                                 ByVal establishmentID As Integer, _
                                                 ByVal contractType As String, _
                                                 ByVal contractGroupID As Integer, _
                                                 ByVal dateFrom As Date, _
                                                 ByVal dateTo As Date, _
                                                 ByVal listFilterNumber As String, _
                                                 ByVal listFilterTitle As String, _
                                                 ByVal listFilterSU As String, _
                                                 ByVal listFilterGroup As String, _
                                                 ByVal listFilterSvcGroup As String, _
                                                 ByVal contractEndReasonID As Integer, _
                                                 ByVal serviceGroupID As Integer, _
                                                 ByVal serviceGroupClassificationID As Integer, _
                                                 ByVal listFilterCreditor As String, _
                                                 ByVal genericCreditorId As Integer, _
                                                 ByVal types As List(Of Integer)) _
                                                 As FetchGenericContractListResult


            Dim msg As ErrorMessage
            Dim connection As SqlConnection = Nothing
            Dim result As New FetchGenericContractListResult()

            Try

                Dim pageSize As Integer = 10
                Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                Dim totalRecords As Integer = 0

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the list of contracts
                msg = CreditorPaymentsBL.GetPagedGenericContracts(connection, page, pageSize, selectedDomContractID, establishmentID, contractType, contractGroupID, dateFrom, dateTo, _
                 listFilterNumber, listFilterTitle, listFilterSU, listFilterGroup, listFilterSvcGroup, contractEndReasonID, settings.CurrentApplicationID, _
                 serviceGroupID, serviceGroupClassificationID, totalRecords, result.Contracts, listFilterCreditor, genericCreditorId, types)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchContractList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                ' catch and wrap the unexpected error

                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            Finally
                ' always close the connection

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

        ''' <summary>
        ''' Gets paged generic creditors.
        ''' </summary>
        ''' <param name="page">The page to fetch.</param>
        ''' <param name="pageSize">The size of the page to fetch.</param>
        ''' <param name="creditorName">Name of the generic creditor.</param>
        ''' <param name="creditorReference">The generic creditor reference.</param>
        ''' <param name="selectedId">The selected id, 0 if not specified</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Get paged generic creditors"), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Shared Function GetPagedGenericCreditors(ByVal page As Integer, _
                                                        ByVal pageSize As Integer, _
                                                        ByVal creditorName As String, _
                                                        ByVal creditorReference As String, _
                                                        ByVal selectedId As Integer) _
                                                        As CreditorPayments_GetPagedGenericCreditorsResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As CreditorPayments_GetPagedGenericCreditorsResult = New CreditorPayments_GetPagedGenericCreditorsResult()

            Try

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

                ' get the list of service types and throw error if not succeeded
                msg = CreditorPaymentsBL.GetPagedGenericCreditors(connection, page, pageSize, totalRecords, creditorName, creditorReference, selectedId, 0, result.Items)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage()
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:GenericCreditorSelector_FetchGenericCreditors({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

        ''' <summary>
        ''' Gets the paged generic creditor payments.
        ''' </summary>
        ''' <param name="page">The page.</param>
        ''' <param name="selectedID">The selected ID.</param>
        ''' <param name="clientID">The client ID.</param>
        ''' <param name="creditorReference">The creditor reference.</param>
        ''' <param name="creditorName">Name of the creditor.</param>
        ''' <param name="contractNumber">The contract number.</param>
        ''' <param name="serviceUser">The service user.</param>
        ''' <param name="paymentNumber">The payment number.</param>
        ''' <param name="serviceUserId">The service user id.</param>
        ''' <param name="dateFrom">The date from.</param>
        ''' <param name="dateTo">The date to.</param>
        ''' <param name="includeNonResidential">if set to <c>true</c> [include non residential].</param>
        ''' <param name="includeDirectPayment">if set to <c>true</c> [include direct payment].</param>
        ''' <param name="includeUnpaid">if set to <c>true</c> [include unpaid].</param>
        ''' <param name="includeAuhtorised">if set to <c>true</c> [include auhtorised].</param>
        ''' <param name="includePaid">if set to <c>true</c> [include paid].</param>
        ''' <param name="includeSuspended">if set to <c>true</c> [include suspended].</param>
        ''' <param name="statusDateFrom">The status date from.</param>
        ''' <param name="statusDateTo">The status date to.</param>
        ''' <param name="includeExcludedFromCreditors">The include excluded from creditors.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetPagedGenericCreditorPayments(ByVal page As Integer, _
                                                       ByVal selectedID As Integer, _
                                                       ByVal clientID As Integer, _
                                                       ByVal creditorReference As String, _
                                                        ByVal creditorName As String, _
                                                        ByVal contractNumber As String, _
                                                        ByVal serviceUser As String, _
                                                        ByVal filterPaymentNumber As String, _
                                                        ByVal serviceUserId As Integer, _
                                                        ByVal dateFrom As Nullable(Of DateTime), _
                                                        ByVal dateTo As Nullable(Of DateTime), _
                                                        ByVal includeNonResidential As Boolean, _
                                                        ByVal includeDirectPayment As Boolean, _
                                                        ByVal includeUnpaid As Boolean, _
                                                        ByVal includeAuhtorised As Boolean, _
                                                        ByVal includePaid As Boolean, _
                                                        ByVal includeSuspended As Boolean, _
                                                        ByVal statusDateFrom As Nullable(Of DateTime), _
                                                        ByVal statusDateTo As Nullable(Of DateTime), _
                                                        ByVal includeExcludedFromCreditors As Nullable(Of Boolean), _
                                                        ByVal genericContractID As Integer, _
                                                        ByVal genericCreditorID As Integer, _
                                                        ByVal paymentNumber As String, _
                                                        ByVal nonResAdditionalFilter As CreditorPaymentsBL.GenericCreditorNonResidentialAdditionalFilter, _
                                                        ByVal manuallySuspended As Nullable(Of Boolean)) _
                                                       As CreditorPayments_GetPagedGenericCreditorPaymentsResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim pageSize As Integer = 10
            Dim result As New CreditorPayments_GetPagedGenericCreditorPaymentsResult()

            Try

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

                ' get the list of service types and throw error if not succeeded
                msg = CreditorPaymentsBL.GetPagedGenericCreditorPayments(connection, page, 10, totalRecords, creditorReference, creditorName, contractNumber, serviceUser, paymentNumber, serviceUserId, dateFrom, dateTo, includeNonResidential, includeDirectPayment, includeUnpaid, includeAuhtorised, includePaid, includeSuspended, statusDateFrom, statusDateTo, includeExcludedFromCreditors, selectedID, genericContractID, genericCreditorID, filterPaymentNumber, 0, result.NumberOfUnpaidPayments, result.NumberOfAuthorisedPayments, result.NumberOfPaidPayments, result.NumberOfSuspendedPayments, nonResAdditionalFilter, manuallySuspended, result.Items)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage()
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:GenericCreditorPaymentSelector_FetchGenericCreditorPayments({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

        ''' <summary>
        ''' Gets the paged generic creditor payments summary.
        ''' </summary>
        ''' <param name="selectedID">The selected ID.</param>
        ''' <param name="clientID">The client ID.</param>
        ''' <param name="creditorReference">The creditor reference.</param>
        ''' <param name="creditorName">Name of the creditor.</param>
        ''' <param name="contractNumber">The contract number.</param>
        ''' <param name="serviceUser">The service user.</param>
        ''' <param name="paymentNumber">The payment number.</param>
        ''' <param name="serviceUserId">The service user id.</param>
        ''' <param name="dateFrom">The date from.</param>
        ''' <param name="dateTo">The date to.</param>
        ''' <param name="includeNonResidential">if set to <c>true</c> [include non residential].</param>
        ''' <param name="includeDirectPayment">if set to <c>true</c> [include direct payment].</param>
        ''' <param name="includeUnpaid">if set to <c>true</c> [include unpaid].</param>
        ''' <param name="includeAuhtorised">if set to <c>true</c> [include auhtorised].</param>
        ''' <param name="includePaid">if set to <c>true</c> [include paid].</param>
        ''' <param name="includeSuspended">if set to <c>true</c> [include suspended].</param>
        ''' <param name="statusDateFrom">The status date from.</param>
        ''' <param name="statusDateTo">The status date to.</param>
        ''' <param name="includeExcludedFromCreditors">The include excluded from creditors.</param>
        ''' <param name="genericContractID">The generic contract ID.</param>
        ''' <param name="genericCreditorID">The generic creditor ID.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetPagedGenericCreditorPaymentsSummary(ByVal selectedID As Integer, _
                                                               ByVal clientID As Integer, _
                                                               ByVal creditorReference As String, _
                                                                ByVal creditorName As String, _
                                                                ByVal contractNumber As String, _
                                                                ByVal serviceUser As String, _
                                                                ByVal filterPaymentNumber As String, _
                                                                ByVal serviceUserId As Integer, _
                                                                ByVal dateFrom As Nullable(Of DateTime), _
                                                                ByVal dateTo As Nullable(Of DateTime), _
                                                                ByVal includeNonResidential As Boolean, _
                                                                ByVal includeDirectPayment As Boolean, _
                                                                ByVal includeUnpaid As Boolean, _
                                                                ByVal includeAuhtorised As Boolean, _
                                                                ByVal includePaid As Boolean, _
                                                                ByVal includeSuspended As Boolean, _
                                                                ByVal statusDateFrom As Nullable(Of DateTime), _
                                                                ByVal statusDateTo As Nullable(Of DateTime), _
                                                                ByVal includeExcludedFromCreditors As Nullable(Of Boolean), _
                                                                ByVal genericContractID As Integer, _
                                                                ByVal genericCreditorID As Integer, _
                                                                ByVal paymentNumber As String, _
                                                                ByVal manuallySuspended As Nullable(Of Boolean), _
                                                                ByVal nonResAdditionalFilter As CreditorPaymentsBL.GenericCreditorNonResidentialAdditionalFilter) _
                                                               As CreditorPayments_GetPagedGenericCreditorPaymentsSummary

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As CreditorPayments_GetPagedGenericCreditorPaymentsSummary = New CreditorPayments_GetPagedGenericCreditorPaymentsSummary()

            Try

                Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                ' get the connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the summary and throw error if not succeeded
                msg = CreditorPaymentsBL.GetPagedGenericCreditorPaymentsSummary(connection, creditorReference, creditorName, contractNumber, serviceUser, paymentNumber, serviceUserId, dateFrom, dateTo, includeNonResidential, includeDirectPayment, includeUnpaid, includeAuhtorised, includePaid, includeSuspended, statusDateFrom, statusDateTo, includeExcludedFromCreditors, selectedID, genericContractID, genericCreditorID, filterPaymentNumber, manuallySuspended, nonResAdditionalFilter, result.Item)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

            Catch ex As Exception
                ' catch and wrap the unexpected error

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            Finally
                ' always close the connection

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

        ''' <summary>
        ''' Toggles the generic creditor payment exclude from creditors flag.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ToggleGenericCreditorPaymentExcludeFromCreditorsFlag(ByVal id As Integer) As CreditorPayments_ToggleGenericCreditorPaymentExcludeFromCreditorsFlag

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As CreditorPayments_ToggleGenericCreditorPaymentExcludeFromCreditorsFlag = New CreditorPayments_ToggleGenericCreditorPaymentExcludeFromCreditorsFlag()

            Try

                Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                ' get the connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' toggle the flag as requested
                msg = CreditorPaymentsBL.ToggleGenericCreditorPaymentExcludeFromCreditorsFlag(connection, id)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

            Catch ex As Exception
                ' catch and wrap the unexpected error

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            Finally
                ' always close the connection

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function

#Region " FetchPagedRemittanceList "
        ''' <summary>
        ''' Gets paged list of remittances.
        ''' </summary>
        ''' <param name="page">The page to fetch.</param>
        ''' <param name="pageSize">The size of the page to fetch.</param>
        ''' <param name="creditorName">Name of the generic creditor.</param>
        ''' <param name="creditorReference">The generic creditor reference.</param>
        ''' <param name="selectedBatchID">The selected id, 0 if not specified</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Fetch Paged Remittance List"), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Shared Function FetchPagedRemittanceList(ByVal page As Integer, _
                                                        ByVal pageSize As Integer, _
                                                        ByVal creditorName As String, _
                                                        ByVal creditorReference As String, _
                                                        ByVal selectedBatchID As Integer) _
                                                        As CreditorPayments_GetPagedGenericRemittancesResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As CreditorPayments_GetPagedGenericRemittancesResult = New CreditorPayments_GetPagedGenericRemittancesResult()

            Try

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

                ' get the list of service types and throw error if not succeeded
                msg = CreditorPaymentsBL.GetPagedGenericRemittances(connection, page, pageSize, totalRecords, creditorName, creditorReference, selectedBatchID, result.Items)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage()
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:RemittanceSelector_FetchRemittanceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#End Region

    End Class

End Namespace

