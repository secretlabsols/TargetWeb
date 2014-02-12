Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

	''' <summary>
	''' Web service to retrieve domiciliary contract information.
	''' </summary>
    ''' <remarks></remarks>
    '''  <history>
    '''  Waqas  06/10/2010  Created D11941A Duration claimed rounding
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/DomContract")> _
 <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
 <ToolboxItem(False)> _
 Public Class DomContract
        Inherits System.Web.Services.WebService


#Region "Fields"

        ' constants
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber

#End Region



#Region " FetchDomContractList "

        ''' <summary>
        ''' Retrieves a paginated list of domiciliary contracts.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedDomContractID">The ID of the contract to select.</param>
        ''' <param name="establishmentID">The ID of the establishment to filter the results on.</param>
        ''' <param name="contractType">The contract type to filter the result on.</param>
        ''' <param name="contractGroupID">The ID of the contract group to filter the results on.</param>
        ''' <param name="dateFrom">The start of the period to filter the results on.</param>
        ''' <param name="dateTo">The end of the period to filter the result son.</param>
        ''' <param name="listFilterNumber">The custom list filter string to apply to the Number column.</param>
        ''' <param name="listFilterTitle">The custom list filter string to apply to the Title column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomContractList(ByVal page As Integer, ByVal selectedDomContractID As Integer, _
         ByVal establishmentID As Integer, ByVal contractType As String, ByVal contractGroupID As Integer, _
         ByVal dateFrom As Date, ByVal dateTo As Date, ByVal listFilterNumber As String, _
         ByVal listFilterTitle As String) As FetchDomContractListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomContractListResult = New FetchDomContractListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = DomContractBL.FetchDomContractList( _
                 conn, page, pageSize, selectedDomContractID, establishmentID, contractType, contractGroupID, dateFrom, dateTo, _
                 listFilterNumber, listFilterTitle, String.Empty, String.Empty, 0, settings.CurrentApplicationID, 0, 0, totalRecords, result.Contracts, 0)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDomContractList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDcrDomContractList "

        ''' <summary>
        ''' Retrieves a paginated list of domiciliary contracts for duration claimed rounding.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="listFilterNumber">The custom list filter string to apply to the Number column.</param>
        ''' <param name="listFilterTitle">The custom list filter string to apply to the Title column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDcrDomContractList(ByVal page As Integer, _
                                                 ByVal ExternalAccountId As Integer, _
                                                 ByVal listFilterNumber As String, _
                                                 ByVal listFilterTitle As String, _
                                                 ByVal listFilterGroup As String, _
                                                 ByVal selectedDomContractID As Integer, _
                                                 ByVal ignoreContractsInUseOutsideDcrID As Integer) _
                                                 As FetchDomContractListResult

            Dim isCouncilUser As Boolean
            Dim msg As ErrorMessage
            Dim result As FetchDomContractListResult = New FetchDomContractListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
                isCouncilUser = SecurityBL.IsCouncilUser(conn, settings, user.ExternalUserID)

                ' get the list of contracts
                msg = DomContractBL.FetchDcrDomContractList( _
                 conn, _
                 page, _
                 pageSize, _
                 ExternalAccountId, _
                 isCouncilUser, _
                 user.ID, _
                 listFilterNumber, _
                 listFilterTitle, _
                 listFilterGroup, _
                 totalRecords, _
                 result.Contracts, _
                 selectedDomContractID, _
                 ignoreContractsInUseOutsideDcrID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDcrDomContractList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchManualDomProformaInvoiceEnquiryResults "

        ''' <summary>
        ''' Retrieves a list of domiciliary proforma invoices for use with the manual proforma
        ''' invoice enquiry. The results being filtered by the specified filters below and by the 
        ''' provider that the specified user has rights to view.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="providerID">the ID of the provider.</param>
        ''' <param name="contractID">The ID of the contract to filter on.</param>
        ''' <param name="clientID">The ID of the service user.</param>
        ''' <param name="batchType">The sum of invoice batch types to filter the results on.</param>
        ''' <param name="batchStatus">The sum of invoice batch status values to filter the results on.</param>
        ''' <param name="selectedInvoiceID">the ID of the invoice to select.</param>
        ''' <param name="dateFrom">The batch status date range start to filter the results on.</param>
        ''' <param name="dateTo">The batch status date range end to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchManualDomProformaInvoiceEnquiryResults( _
            ByVal page As Integer, ByVal selectedInvoiceID As Integer, ByVal providerID As Integer, _
            ByVal contractID As Integer, ByVal clientID As Integer, ByVal batchType As Integer, _
            ByVal batchStatus As Integer, ByVal dateFrom As Date, ByVal dateTo As Date) As FetchManualDomProformaInvoiceEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchManualDomProformaInvoiceEnquiryListResult = New FetchManualDomProformaInvoiceEnquiryListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of invoices
                msg = DomContractBL.FetchManualDomProformaInvoiceEnquiryResults( _
                    conn, user.ExternalUserID, page, pageSize, selectedInvoiceID, providerID, contractID, clientID, _
                    batchType, batchStatus, dateFrom, dateTo, totalRecords, result.Invoices)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchInvoiceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProformaInvoiceEnquiryResults "

        ''' <summary>
        ''' Retrieves a list of domiciliary proforma invoices for use with the manual proforma
        ''' invoice enquiry. The results being filtered by the specified filters below and by the 
        ''' provider that the specified user has rights to view.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="providerID">the ID of the provider.</param>
        ''' <param name="contractID">The ID of the contract to filter on.</param>
        ''' <param name="batchType">The sum of invoice batch types to filter the results on.</param>
        ''' <param name="batchStatus">The sum of invoice batch status values to filter the results on.</param>
        ''' <param name="selectedBatchID">the ID of the invoice to select.</param>
        ''' <param name="dateFrom">The batch status date range start to filter the results on.</param>
        ''' <param name="dateTo">The batch status date range end to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProformaInvoiceEnquiryResults( _
            ByVal page As Integer, ByVal fileID As Integer, ByVal selectedBatchID As Integer, ByVal providerID As Integer, _
            ByVal contractID As Integer, ByVal batchType As Integer, _
            ByVal batchStatus As Integer, ByVal dateFrom As Date, ByVal dateTo As Date) As FetchDomProformaInvoiceEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProformaInvoiceEnquiryListResult = New FetchDomProformaInvoiceEnquiryListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of invoices
                msg = DomContractBL.FetchDomProformaInvoiceEnquiryResults( _
                    conn, user.ExternalUserID, fileID, page, pageSize, selectedBatchID, providerID, contractID, _
                    batchType, batchStatus, dateFrom, dateTo, totalRecords, result.Batches)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchBatchList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProformaInvoiceResults "

        ''' <summary>
        ''' Retrieves a list of domiciliary proforma invoices for use with the View invoice enquiry. 
        ''' The results being filtered by the specified filters below and by the 
        ''' provider that the specified user has rights to view.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="batchID">The ID of the Proforma invoice batch.</param>
        ''' <param name="selectedInvoiceID">The invoice to selected in the results list</param>
        ''' <param name="queried">How to filter the results on queried items</param>
        ''' <param name="mismatch">How to filter the result on mismatched items</param>
        ''' <param name="tolerance">What tolerance to apply when comparing payment mismatches</param>
        ''' <param name="listFilterSUReference">The custom list filter string to apply to the S/U reference column.</param>
        ''' <param name="listFilterSUName">The custom list filter string to apply to the S/U name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  16/10/2009  D11546 - added payment mismatch tolerance and S/U Ref/Name filters.
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProformaInvoiceResults( _
                ByVal page As Integer, _
                ByVal batchID As Integer, _
                ByVal selectedInvoiceID As Integer, _
                ByVal queried As Integer, _
                ByVal mismatch As Integer, _
                ByVal tolerance As String, _
                ByVal listFilterSUReference As String, _
                ByVal listFilterSUName As String, _
                ByVal dcrFilter As TriState) As FetchDomProformaInvoiceListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProformaInvoiceListResult = New FetchDomProformaInvoiceListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser
            Dim triQueried As TriState
            Dim triMismatch As TriState

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                Select Case queried
                    Case 0
                        triQueried = TriState.UseDefault
                    Case 1
                        triQueried = TriState.True
                    Case 2
                        triQueried = TriState.False
                End Select

                Select Case mismatch
                    Case 0
                        triMismatch = TriState.UseDefault
                    Case 1
                        triMismatch = TriState.True
                    Case 2
                        triMismatch = TriState.False
                End Select

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of invoices
                msg = DomContractBL.FetchDomProformaInvoiceResults( _
                    conn, _
                    user.ExternalUserID, _
                    page, _
                    pageSize, _
                    batchID, _
                    selectedInvoiceID, _
                    triQueried, _
                    triMismatch, _
                    Utils.ToDecimal(tolerance), _
                    listFilterSUReference, _
                    listFilterSUName, _
                    dcrFilter, _
                    totalRecords, _
                    result.Invoices _
                )
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchInvoiceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchProformaInvoiceResults "

        ''' <summary>
        ''' Retrieves a list of domiciliary proforma invoices for use with the View invoice enquiry. 
        ''' The results being filtered by the specified filters below and by the 
        ''' provider that the specified user has rights to view.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="pScheduleId">The ID of the Payment schedule.</param>
        ''' <param name="selectedInvoiceID">The invoice to selected in the results list</param>
        ''' <param name="queried">How to filter the results on queried items</param>
        ''' <param name="mismatch">How to filter the result on mismatched items</param>
        ''' <param name="tolerance">What tolerance to apply when comparing payment mismatches</param>
        ''' <param name="listFilterSUReference">The custom list filter string to apply to the S/U reference column.</param>
        ''' <param name="listFilterSUName">The custom list filter string to apply to the S/U name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  16/10/2009  D11546 - added payment mismatch tolerance and S/U Ref/Name filters.
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchProformaInvoiceResults( _
                ByVal page As Integer, _
                ByVal pScheduleId As Integer, _
                ByVal selectedInvoiceID As Integer, _
                ByVal queried As Integer, _
                ByVal mismatch As Integer, _
                ByVal tolerance As String, _
                ByVal listFilterSUReference As String, _
                ByVal listFilterSUName As String, _
                ByVal dcrFilter As TriState, _
                ByVal statusAwait As Nullable(Of Boolean), _
                ByVal statusVerified As Nullable(Of Boolean), _
                ByVal queryType As Nullable(Of Integer)) As FetchDomProformaInvoiceListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProformaInvoiceListResult = New FetchDomProformaInvoiceListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser
            Dim triMismatch As TriState
            Dim isQueried As Nullable(Of Boolean) = Nothing

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                Select Case queried
                    Case 0
                        isQueried = Nothing
                    Case 1
                        isQueried = True
                    Case 2
                        isQueried = False
                End Select

                Select Case mismatch
                    Case 0
                        triMismatch = TriState.UseDefault
                    Case 1
                        triMismatch = TriState.True
                    Case 2
                        triMismatch = TriState.False
                End Select

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of invoices
                msg = DomContractBL.FetchProformaInvoiceResults( _
                    conn, _
                    user.ExternalUserID, _
                    page, _
                    pageSize, _
                    pScheduleId, _
                    selectedInvoiceID, _
                    isQueried, _
                    triMismatch, _
                    Utils.ToDecimal(tolerance), _
                    listFilterSUReference, _
                    listFilterSUName, _
                    dcrFilter, _
                    statusAwait, _
                    statusVerified, _
                    totalRecords, _
                    result.Invoices, _
                    Nothing, _
                    Nothing, _
                    queryType, _
                    Nothing _
                )
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchInvoiceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " ChangeProformaInvoiceStatus(s) "

        ''' <summary>
        ''' Retrieves a list of domiciliary proforma invoices for use with the View invoice enquiry. 
        ''' The results being filtered by the specified filters below and by the 
        ''' provider that the specified user has rights to view.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="pScheduleId">The ID of the Payment schedule.</param>
        ''' <param name="selectedInvoiceID">The invoice to selected in the results list</param>
        ''' <param name="queried">How to filter the results on queried items</param>
        ''' <param name="mismatch">How to filter the result on mismatched items</param>
        ''' <param name="tolerance">What tolerance to apply when comparing payment mismatches</param>
        ''' <param name="listFilterSUReference">The custom list filter string to apply to the S/U reference column.</param>
        ''' <param name="listFilterSUName">The custom list filter string to apply to the S/U name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  16/10/2009  D11546 - added payment mismatch tolerance and S/U Ref/Name filters.
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ChangeProformaInvoiceStatus( _
                ByVal page As Integer, _
                ByVal pScheduleId As Integer, _
                ByVal selectedInvoiceID As Integer, _
                ByVal queried As Integer, _
                ByVal mismatch As Integer, _
                ByVal tolerance As String, _
                ByVal listFilterSUReference As String, _
                ByVal listFilterSUName As String, _
                ByVal dcrFilter As TriState, _
                ByVal statusAwait As String, _
                ByVal statusVerified As String, _
                ByVal NewStatus As String) As FetchDomProformaInvoiceListResult


            Dim result As New FetchDomProformaInvoiceListResult()

            Try

                Dim triMismatch As TriState
                Dim isQueried As Nullable(Of Boolean) = Nothing

                Select Case queried
                    Case 0
                        isQueried = Nothing
                    Case 1
                        isQueried = True
                    Case 2
                        isQueried = False
                End Select

                Select Case mismatch
                    Case 0
                        triMismatch = TriState.UseDefault
                    Case 1
                        triMismatch = TriState.True
                    Case 2
                        triMismatch = TriState.False
                End Select

                Using connection As SqlConnection = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                    Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
                    Dim settings As SystemSettings = SystemSettings.GetCachedSettings(connection.ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                    Try

                        result.ErrMsg = _
                        ProformaInvoices.DomProformaInvoiceBL.ChangeDomProformaInvoiceStatus( _
                        connection, _
                        pScheduleId, _
                        selectedInvoiceID, _
                        isQueried, _
                        triMismatch, _
                        tolerance, _
                        listFilterSUReference, _
                        listFilterSUName, _
                        dcrFilter, _
                        statusAwait, _
                        statusVerified, _
                        Nothing, _
                        Nothing, _
                        Nothing, _
                        Nothing, _
                        NewStatus, _
                        user.ExternalUsername, _
                        AuditLogging.GetAuditLogTitle("ProformaInvoices.DomProformaInvoiceBL.ChangeDomProformaInvoiceStatus", settings), _
                        result.Invoices)

                    Catch ex As Exception
                        ' rethrow up stack

                        Throw

                    Finally
                        ' always close connection

                        SqlHelper.CloseConnection(connection)

                    End Try


                End Using

            Catch ex As Exception
                ' catch and wrap exception

                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            End Try

            Return result

        End Function

#End Region

#Region " FetchDomProformaInvoiceLines "

        ''' <summary>
        ''' Retrieves a list of domiciliary proforma invoice lines. 
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="invoiceID">the ID of the Proforma invoice.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProformaInvoiceLines( _
            ByVal page As Integer, ByVal invoiceID As Integer) As FetchDomProformaInvoiceLinesResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProformaInvoiceLinesResult = New FetchDomProformaInvoiceLinesResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of invoices
                msg = DomContractBL.FetchDomProformaInvoiceLines( _
                    conn, user.ExternalUserID, page, pageSize, invoiceID, totalRecords, result.InvoiceLines)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchInvoiceLineList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProformaInvoiceCostedVisits "

        ''' <summary>
        ''' Retrieves a list of domiciliary proforma invoice lines. 
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="invoiceID">the ID of the Proforma invoice.</param>
        ''' <param name="selectedVisitID">The ID of the visit to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProformaInvoiceCostedVisits(ByVal page As Integer, ByVal invoiceID As Integer, _
                                                            ByVal selectedVisitID As Integer, _
                                                            ByVal dcrfilter As String) As FetchDomProformaInvoiceCostedVisitsResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProformaInvoiceCostedVisitsResult = New FetchDomProformaInvoiceCostedVisitsResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of invoices
                msg = DomContractBL.FetchDomProformaInvoiceCostedVisits( _
                    conn, user.ExternalUserID, page, pageSize, invoiceID, selectedVisitID, dcrfilter, totalRecords, result.InvoiceCostedVisits)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchInvoiceVisitList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProviderInvoiceCostedVisits "

        ''' <summary>
        ''' Retrieves a list of domiciliary provider invoice lines. 
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="invoiceID">the ID of the Provider invoice.</param>
        ''' <param name="selectedVisitID">The ID of the visit to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceCostedVisits(ByVal page As Integer, ByVal invoiceID As Integer, _
                                                            ByVal selectedVisitID As Integer, _
                                                            ByVal dcrfilter As String) As FetchDomProviderInvoiceCostedVisitsResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderInvoiceCostedVisitsResult = New FetchDomProviderInvoiceCostedVisitsResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of invoices
                msg = DomContractBL.FetchDomProviderInvoiceCostedVisits( _
                    conn, user.ExternalUserID, page, pageSize, invoiceID, selectedVisitID, dcrfilter, totalRecords, result.InvoiceCostedVisits)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchInvoiceVisitList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchVisitCodesAvailableForVisit "

        ''' <summary>
        ''' Retrieves a list of visit codes that can be chosen from for the specified contract and visit date. 
        ''' </summary>
        ''' <param name="contractID">The ID of the contract.</param>
        ''' <param name="visitDate">The date of the visit.</param>
        ''' <param name="uniqueID">A unique ID used by the caller to identify the response when it is returned (as web service calls are done asynchronously).</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchVisitCodesAvailableForVisit(ByVal contractID As Integer, ByVal visitDate As Date, ByVal uniqueID As String) As FetchVisitCodesAvailableForVisitResult

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim result As New FetchVisitCodesAvailableForVisitResult()
            Dim user As WebSecurityUser
            Dim visitCodes As DomVisitCodeCollection = Nothing

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchVisitCodesAvailableForVisit(conn, contractID, visitDate, visitCodes)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .List = New List(Of ViewablePair)
                    For Each code As DomVisitCode In visitCodes
                        .List.Add(New ViewablePair(code.ID, code.Description))
                        If code.DefaultCode Then .DefaultCodeID = code.ID
                    Next
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .UniqueID = uniqueID
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " ChangeProformaInvoiceBatchStatus "

        ''' <summary>
        ''' Changes the status of a proforma invoice batch.
        ''' </summary>
        ''' <param name="invoiceBatchID">The ID of the invoice batch.</param>
        ''' <param name="newStatus">The new status for the invoice batch.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ChangeProformaInvoiceBatchStatus(ByVal invoiceBatchID As Integer, ByVal newStatus As DomProformaInvoiceBatchStatus) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim batch As DomProformaInvoiceBatch
            Dim canView As Boolean

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then Return msg

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the batch
                batch = New DomProformaInvoiceBatch(conn, String.Empty, String.Empty)
                msg = batch.Fetch(invoiceBatchID)
                If Not msg.Success Then Return msg

                ' see if the user is allowed to perform the operation
                msg = AbacusClassesBL.UserCanViewEstablishment(conn, user.ExternalUserID, batch.ProviderID, True, canView)
                If Not msg.Success Then Return msg
                If Not canView Then
                    msg = New ErrorMessage()
                    msg.Number = "E3020"    ' insufficient rights
                    Return msg
                End If

                ' delete the batch
                msg = DomContractBL.ChangeProformaInvoiceBatchStatus(conn, batch, newStatus, user.ExternalUsername, "DomContractWebSvc.ChangeProformaInvoiceBatchStatus()")
                If Not msg.Success Then Return msg

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber) ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg

        End Function

#End Region

#Region " ChangeProformaInvoiceQuery "

        ''' <summary>
        ''' Routine to set or clear the Query on an invoice record.
        ''' </summary>
        ''' <param name="invoiceID">the ID of the invoice we want to update</param>
        ''' <param name="queryText">The Query, Blank indicates we want to remove the query.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ChangeProformaInvoiceQuery(ByVal invoiceID As Integer, _
                                                   ByVal queryText As String, _
                                                   ByVal QueryType As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim invoice As DomProformaInvoice
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)


            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then Return msg

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the batch
                invoice = New DomProformaInvoice(conn, _
                                                 auditUserName:=user.ExternalUsername, _
                                                 auditLogTitle:=AuditLogging.GetAuditLogTitle("", settings))
                msg = invoice.Fetch(invoiceID)
                If Not msg.Success Then Return msg

                'Update the invoice with the query text passed in, set all values related to query at the same time.
                With invoice
                    If queryText <> "" Then
                        .Query = TriState.True
                        .QueryDescription = queryText
                        .QueryBy = user.ExternalUsername
                        .QueryLoggedOnUser = user.Email
                        .QueryDate = Now
                        .QueryType = QueryType
                    Else
                        'As the queryString was blank we asume this means to blank the query.
                        .Query = TriState.False
                        .QueryDescription = Nothing
                        .QueryBy = Nothing
                        .QueryDate = Nothing
                        .QueryType = 0
                        .QueryLoggedOnUser = user.Email
                    End If
                End With

                msg = invoice.Save
                If Not msg.Success Then Return msg

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber) ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg

        End Function

#End Region

#Region " RecalculateProformaInvoice "

        ''' <summary>
        ''' Recalculates a single proforma invoice.
        ''' </summary>
        ''' <param name="invoiceID">The ID of the invoice.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function RecalculateProformaInvoice(ByVal invoiceID As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim invoice As DomProformaInvoice
            Dim batch As DomProformaInvoiceBatch
            Dim canView As Boolean
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then Return msg

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the invoice
                invoice = New DomProformaInvoice(conn, String.Empty, String.Empty)
                msg = invoice.Fetch(invoiceID)
                If Not msg.Success Then Return msg

                ' get the batch
                batch = New DomProformaInvoiceBatch(conn, String.Empty, String.Empty)
                msg = batch.Fetch(invoice.DomProformaInvoiceBatchID)
                If Not msg.Success Then Return msg

                ' see if the user is allowed to perform the operation
                msg = AbacusClassesBL.UserCanViewEstablishment(conn, user.ExternalUserID, batch.ProviderID, True, canView)
                If Not msg.Success Then Return msg
                If Not canView Then
                    msg = New ErrorMessage()
                    msg.Number = "E3020"    ' insufficient rights
                    Return msg
                End If

                ' recalc the invoice
                msg = DomContractBL.RecalculateProformaInvoice(conn, _
                                                               invoice, _
                                                               user.ExternalUsername, _
                                                               "DomContractWebSvc.RecalculateProformaInvoice()", settings.CurrentApplicationID)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage()
                msg = DomContractBL.PaymentscheduleRecalculateCountsAttributesAndNetValues(conn, invoice.PaymentScheduleID)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber) ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg

        End Function

#End Region

#Region " WeekendingDateValid "

        ''' <summary>
        ''' Validates a weekending date.
        ''' </summary>
        ''' <param name="weekendingDate">The Weekending date to be validated.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function WeekendingDateValid(ByVal weekendingDate As Date) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' see if the user is allowed to perform the operation
                msg = DomContractBL.ValidateWeekEndingDate(conn, weekendingDate)

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg

        End Function

#End Region

#Region " GetWeekendingDate "

        ''' <summary>
        ''' Returns a Week Ending Date
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetWeekendingDate() As GetWeekendingDateResult

            Dim msg As New ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim result As GetWeekendingDateResult = New GetWeekendingDateResult()
            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' see if the user is allowed to perform the operation
                Dim WeDate As DateTime = DomContractBL.GetWeekEndingDate(conn, Nothing)
                msg.Success = True
                result.ErrMsg = msg
                result.WeekEndingDate = WeDate
            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " Create CareWorker "
        ''' <summary>
        ''' Creates Care worler
        ''' </summary>
        ''' <param name="CareWorkerRef">Reference number of care worker</param>
        ''' <param name="CareWorkerName">Name of care worker</param>
        ''' <param name="ProviderId">Provider Id</param>
        ''' <returns>CreateCareWorkerResult</returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CreateCareWorker(ByVal CareWorkerRef As String, _
                                            ByVal CareWorkerName As String, _
                                            ByVal ProviderId As Integer) As CreateCareWorkerResult


            Dim msg As ErrorMessage
            Dim result As CreateCareWorkerResult = New CreateCareWorkerResult()
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim careWorkerId As Integer = 0

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    result.CareWorkerID = 0
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = DomContractBL.CreateCareWorker(Connection:=conn, _
                                                     CareWorkerRef:=CareWorkerRef, _
                                                     CareWorkerName:=CareWorkerName, _
                                                     ProviderId:=ProviderId, _
                                                     CareWorkerId:=careWorkerId)
                result.ErrMsg = msg
                result.CareWorkerID = careWorkerId

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchCareWorkerList "

        ''' <summary>
        ''' Retrieves a paginated list of Care Workers.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedCareWorkerID">The ID of the care worker to select.</param>
        ''' <param name="providerID">The ID of the Provider to filter the results on.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the Name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchCareWorkerList(ByVal page As Integer, ByVal selectedCareWorkerID As Integer, _
         ByVal providerID As Integer, ByVal listFilterName As String) As FetchCareWorkerListResult

            Dim msg As ErrorMessage
            Dim result As FetchCareWorkerListResult = New FetchCareWorkerListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim user As WebSecurityUser
            Dim pageSize As Integer = 10

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = DomContractBL.FetchCareWorkersForProviderResults( _
                 conn, user.ExternalUserID, page, pageSize, providerID, selectedCareWorkerID, listFilterName, totalRecords, result.CareWorkers)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchCareWorkerList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

        ''' <summary>
        ''' Retrieves a non paginated list of Care Workers.
        ''' </summary>
        ''' <param name="providerID">The ID of the Provider to filter the results on.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the Name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchCareWorkerListByProviderID(ByVal providerID As Integer, _
                                            ByVal listFilterName As String, _
                                            ByVal existingIds As String) As FetchCareWorkerListResult


            Dim msg As ErrorMessage
            Dim result As FetchCareWorkerListResult = New FetchCareWorkerListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim user As WebSecurityUser
            Dim pageSize As Integer = 1000
            Dim Page As Integer = 1
            Dim selectedCareWorkerID As Integer = 0

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = DomContractBL.FetchCareWorkersForProviderResults( _
                 conn, user.ExternalUserID, Page, pageSize, providerID, selectedCareWorkerID, listFilterName, totalRecords, result.CareWorkers, existingIds)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchCareWorkerListByProviderID({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      Page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchInvoiceList "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchInvoiceReferenceList(ByVal paymentScheduleId As Integer _
                                         ) As FetchInvoiceReferenceListResult

            Dim msg As ErrorMessage
            Dim result As FetchInvoiceReferenceListResult = New FetchInvoiceReferenceListResult
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = InvoiceReferences.InvoiceReferencesBL.GetInvoiceList( _
                 conn, paymentScheduleId, result.InvoiceReferences)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                Else
                    result.ErrMsg = msg
                End If

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result
        End Function
#End Region

#Region " Update Invoice Reference "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function UpdateInvoiceReference(ByVal invoiceId As Integer, _
                                               ByVal isProformaInvoice As Integer, _
                                               ByVal newReference As String _
                                         ) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return msg
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = InvoiceReferences.InvoiceReferencesBL.UpdateInvoiceReference( _
                                conn, _
                                invoiceId, _
                                isProformaInvoice, _
                                newReference)


                Return msg
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg
        End Function
#End Region

#Region " Update Invoice Date "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function UpdateInvoiceDate(ByVal invoiceId As Integer, _
                                               ByVal isProformaInvoice As Integer, _
                                               ByVal newDate As String _
                                         ) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return msg
                End If

                If String.IsNullOrEmpty(newDate) Then
                    msg.Success = False
                    msg.Message = "Invalid Date!"
                    Return msg
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = InvoiceReferences.InvoiceReferencesBL.UpdateInvoiceDate( _
                                conn, _
                                invoiceId, _
                                isProformaInvoice, _
                                newDate)


                Return msg
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg
        End Function
#End Region

#Region " Update Session Back Url "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)> _
        Public Function UpdateSessionBackURL(ByVal oldUrl As String, ByVal replacementUrl As String) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Const SESSION_CUSTOMNAV As String = "CustomNav"

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return msg
                End If

                If Context.Session(SESSION_CUSTOMNAV).Count > 0 Then
                    If String.IsNullOrEmpty(oldUrl) OrElse Context.Session(SESSION_CUSTOMNAV).Peek().Contains(oldUrl) Then
                        Context.Session(SESSION_CUSTOMNAV).Pop()
                    End If
                End If
                Context.Session(SESSION_CUSTOMNAV).Push(replacementUrl)

                Return msg
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg
        End Function
#End Region

        ''' <summary>
        ''' Fetches the verification text non residential.
        ''' </summary>
        ''' <param name="id">The identifier.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="FetchVerificationTextNonResidential."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchVerificationTextNonResidential(ByVal id As Integer) As ErrorMessage

            Dim connection As SqlConnection = Nothing
            Dim outVerificationText As String = String.Empty
            Dim msg As New ErrorMessage
            Try

                Dim currentUser As Web.Apps.Security.WebSecurityUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return msg
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnectionToAbacus()

                ' fetch specific onr  text 
                msg = Target.Abacus.Library.ReferenceDataCommon.ReferenceDataBL.FetchVerificationTextNonResidential(connection, id, outVerificationText)
                msg.SuccessMessage = outVerificationText

            Catch ex As Exception
                ' wrap the exception and add to the result
                msg = Utils.CatchError(ex, _GeneralErrorNumber)
                msg.Success = False

            Finally
                ' finally close the db connection 

                SqlHelper.CloseConnection(connection)

            End Try

            Return msg

        End Function


        ''' <summary>
        ''' Fetches the verification text residential.
        ''' </summary>
        ''' <param name="id">The identifier.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="FetchVerificationTextResidential."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchVerificationTextResidential(ByVal id As Integer) As ErrorMessage

            Dim connection As SqlConnection = Nothing
            Dim outVerificationText As String = String.Empty
            Dim msg As New ErrorMessage
            Try

                Dim currentUser As Web.Apps.Security.WebSecurityUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return msg
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnectionToAbacus()

                ' fetch specific onr  text 
                msg = Target.Abacus.Library.ReferenceDataCommon.ReferenceDataBL.FetchVerificationTextResidential(connection, id, outVerificationText)
                msg.SuccessMessage = outVerificationText

            Catch ex As Exception
                ' wrap the exception and add to the result
                msg = Utils.CatchError(ex, _GeneralErrorNumber)
                msg.Success = False

            Finally
                ' finally close the db connection 

                SqlHelper.CloseConnection(connection)

            End Try

            Return msg

        End Function

    End Class

End Namespace