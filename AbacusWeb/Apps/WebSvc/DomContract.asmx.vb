
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library
Imports Target.Abacus.Library.ServiceOrder
Imports Target.Abacus.Library.DataClasses
Imports DPI = Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections

Namespace Apps.WebSvc

	''' <summary>
	''' Web service to retrieve domiciliary contract information.
	''' </summary>
	''' <remarks></remarks>
	<System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/DomContract")> _
	<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
	<ToolboxItem(False)> _
	Public Class DomContract
		Inherits System.Web.Services.WebService

#Region " FetchRateCategoriesInFrameworkList "

		''' <summary>
		''' Retrieves a list of domiciliary rate categories in the specified rate framework.
		''' </summary>
		''' <param name="rateFrameworkID">The ID of the framework.</param>
		''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  25/03/2010  A4WA#6166 - fixed calls to vwDomRateCategory.FetchList().
        ''' </history>
		<WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
		Public Function FetchRateCategoriesInFrameworkList(ByVal rateFrameworkID As Integer) As FetchRateCategoryListResult

			Dim msg As ErrorMessage
            Dim categories As vwDomRateCategoryCollection = Nothing
			Dim result As FetchRateCategoryListResult = New FetchRateCategoryListResult
			Dim conn As SqlConnection = Nothing

			Try
				' check user is logged in
				msg = SecurityBL.ValidateLogin()
				If Not msg.Success Then
					result.ErrMsg = msg
					Return result
				End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of categories
                msg = vwDomRateCategory.FetchList(conn:=conn, _
                                                  list:=categories, _
                                                  domRateFrameworkID:=rateFrameworkID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Categories = categories
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " GetRateCategoryAbbreviation "

        ''' <summary>
        ''' Retrieves the constructed rate category abbreviation.
        ''' </summary>
        ''' <param name="rateFrameworkID">the ID of the rate framework.</param>
        ''' <param name="svcTypeID">The ID of the service type.</param>
        ''' <param name="dayCategoryID">The ID of the day category.</param>
        ''' <param name="timeBandID">The ID of the time band.</param>
        ''' <returns></returns>
        ''' <remarks>If an ID is not available, use zero.</remarks>
        ''' <history>
        ''' MvO  15/02/2008  Added rateFrameowkrID parameter.
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetRateCategoryAbbreviation(ByVal rateFrameworkID As Integer, ByVal svcTypeID As Integer, _
                                                    ByVal dayCategoryID As Integer, ByVal timeBandID As Integer) As StringResult

            Dim msg As ErrorMessage
            Dim result As StringResult = New StringResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' build the abbreviation
                msg = DomContractBL.ConstructRateCategoryAbbreviation(conn, Nothing, rateFrameworkID, svcTypeID, dayCategoryID, timeBandID, result.Value)
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
        ''' <param name="listFilterSU">The custom list filter string to apply to the Service User column.</param>
        ''' <param name="listFilterGroup">The custom list filter string to apply to the Group column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomContractList(ByVal page As Integer, ByVal selectedDomContractID As Integer, _
                     ByVal establishmentID As Integer, ByVal contractType As String, ByVal contractGroupID As Integer, _
                     ByVal dateFrom As Date, ByVal dateTo As Date, ByVal listFilterNumber As String, _
                     ByVal listFilterTitle As String, ByVal listFilterSU As String, ByVal listFilterGroup As String, _
                     ByVal contractEndReasonID As Integer, ByVal serviceGroupID As Integer, ByVal serviceGroupClassificationID As Integer, ByVal frameworkTypeID As Integer) As FetchDomContractListResult

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
                 listFilterNumber, listFilterTitle, listFilterSU, listFilterGroup, contractEndReasonID, settings.CurrentApplicationID, _
                 serviceGroupID, serviceGroupClassificationID, totalRecords, result.Contracts, frameworkTypeID)
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

#Region " FetchRateCategoriesAvailableToVrc "

        ''' <summary>
        ''' Retrieves a list of rate categories available to a visit rate category,
        ''' i.e. the list being filtered by rate framework/day category/service type and
        ''' optionally time band.
        ''' </summary>
        ''' <param name="frameworkID">The ID of the rate framework.</param>
        ''' <param name="dayCategoryID">The ID of the day category.</param>
        ''' <param name="serviceTypeID">The ID of the service type.</param>
        ''' <param name="timeBandID">The ID of the time band or zero not to filter by time band.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRateCategoriesAvailableToVrc(ByVal frameworkID As Integer, _
         ByVal dayCategoryID As Integer, ByVal serviceTypeID As Integer, ByVal timeBandID As Integer) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of rate categories
                msg = DomContractBL.FetchRateCategoriesAvailableToVrc(conn, frameworkID, dayCategoryID, serviceTypeID, timeBandID, result.List)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
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

#Region " FetchDomServiceOrderList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of domiciliary service orders.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedDomServiceOrderID">The ID of the DSO to select.</param>
        ''' <param name="providerID">The ID of the provider to filter the results on.</param>
        ''' <param name="domContractID">The ID of the com contract to filter the results on.</param>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="dateFrom">The start of the period to filter the results on.</param>
        ''' <param name="dateTo">The end of the period to filter the result son.</param>
        ''' <param name="listFilterOrderRef">List Filter order Ref</param>
        ''' <param name="listFilterProvider">list filter Provider</param>
        ''' <param name="listFilterSvcUserName">List Filter Service User Name</param>
        ''' <param name="listFilterSvcUserRef">List Filter Service User Reference</param>
        ''' <param name="listFilterContract">List Filter Contract</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomServiceOrderList(ByVal page As Integer, ByVal selectedDomServiceOrderID As Integer, _
         ByVal providerID As Integer, ByVal domContractID As Integer, ByVal clientID As Integer, _
         ByVal dateFrom As Date, ByVal dateTo As Date, ByVal listFilterSvcUserName As String, _
         ByVal listFilterSvcUserRef As String, ByVal listFilterProvider As String, _
         ByVal listFilterOrderRef As String, ByVal listFilterContract As String) As FetchDomServiceOrderListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomServiceOrderListResult = New FetchDomServiceOrderListResult
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
                msg = ServiceOrderBL.FetchDomServiceOrderList( _
                 conn, page, pageSize, selectedDomServiceOrderID, providerID, domContractID, clientID, dateFrom, dateTo, _
                 listFilterSvcUserName, listFilterSvcUserRef, listFilterProvider, listFilterOrderRef, listFilterContract, _
                 totalRecords, result.Orders)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDomServiceOrderList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProviderInvoiceBatchList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of domiciliary provider invoice batches.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="currentDPIBatchID">The ID of the batch to select.</param>
        ''' <param name="createdBy">The name pattern of the user who created the batch to filter on.</param>
        ''' <param name="created">The date/time (as a string) pattern of the batch created date to filter on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  30/07/2009  D11547 - support for created filter.
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceBatchList(ByVal page As Integer, _
                                                        ByVal currentDPIBatchID As Integer, _
                                                        ByVal createdBy As String, _
                                                        ByVal created As String) As FetchDomProviderInvoiceBatchListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderInvoiceBatchListResult = New FetchDomProviderInvoiceBatchListResult
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

                ' get the list of provider invoices
                msg = DomContractBL.FetchDomProviderInvoiceBatchList( _
                     conn, page, pageSize, currentDPIBatchID, createdBy, created, _
                     totalRecords, result.Batches)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDomProviderInvoiceBatchList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProviderContractsByBatchList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of domiciliary provider invoice batches.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="currentDPIBatchID">The ID of the batch to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderContractsByBatchList(ByVal page As Integer, _
                ByVal currentDPIBatchID As Integer, _
                ByVal providerID As Integer, ByVal providerName As String, _
                ByVal contractID As Integer, ByVal contractNum As String _
                ) As FetchDomProviderContractsByBatchResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderContractsByBatchResult = New FetchDomProviderContractsByBatchResult
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

                ' get the list of provider invoices
                msg = DomContractBL.FetchDomProviderContractsByBatchList( _
                     conn, page, pageSize, currentDPIBatchID, _
                     providerID, providerName, contractID, contractNum, _
                     totalRecords, result.Contracts)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDomProviderContractsByBatchList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProviderInvoiceList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of domiciliary provider invoices.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedDomProviderInvoiceID">The ID of the DSO to select.</param>
        ''' <param name="providerID">The ID of the provider to filter the results on.</param>
        ''' <param name="domContractID">The ID of the contract to filter the results on.</param>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="weFrom">The start of the period to filter the results on.</param>
        ''' <param name="weTo">The end of the period to filter the result son.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceList(ByVal page As Integer, ByVal selectedDomProviderInvoiceID As Integer, _
         ByVal batchID As Integer, ByVal providerID As Integer, ByVal providerName As String, _
         ByVal domContractID As Integer, ByVal domContractNum As String, _
         ByVal clientID As Integer, ByVal clientName As String, _
         ByVal weFrom As String, ByVal weTo As String, ByVal invoiceNumber As String, ByVal invoiceRef As String, _
         ByVal statusDateFrom As String, ByVal statusDateTo As String, _
         ByVal statusIsUnpaid As Boolean, ByVal statusIsAuthorised As Boolean, _
         ByVal statusIsPaid As Boolean, ByVal statusIsSuspended As Boolean, ByVal exclude As String, _
         ByVal invNumFilter As String, ByVal invRefFilter As String, ByVal additionalFilter As Integer) As FetchDomProviderInvoiceListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderInvoiceListResult = New FetchDomProviderInvoiceListResult
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

                ' get the list of provider invoices
                If page = 0 Then
                    '++ Deliberate means to retrieve results unrestricted by paging..
                    page = 1
                    pageSize = 1000000
                End If

                msg = DomContractBL.FetchDomProviderInvoiceList( _
                 conn, page, pageSize, selectedDomProviderInvoiceID, batchID, _
                 providerID, providerName, domContractID, domContractNum, _
                 clientID, clientName, _
                 weFrom, weTo, invoiceNumber, invoiceRef, statusDateFrom, statusDateTo, _
                 statusIsUnpaid, statusIsAuthorised, statusIsPaid, statusIsSuspended, _
                 exclude, invNumFilter, invRefFilter, additionalFilter, totalRecords, result.Invoices)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDomProviderInvoiceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProviderInvoiceListCount "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of domiciliary provider invoices.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedDomProviderInvoiceID">The ID of the DSO to select.</param>
        ''' <param name="providerID">The ID of the provider to filter the results on.</param>
        ''' <param name="domContractID">The ID of the contract to filter the results on.</param>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="weFrom">The start of the period to filter the results on.</param>
        ''' <param name="weTo">The end of the period to filter the result son.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceListCount(ByVal page As Integer, ByVal selectedDomProviderInvoiceID As Integer, _
         ByVal batchID As Integer, ByVal providerID As Integer, ByVal providerName As String, _
         ByVal domContractID As Integer, ByVal domContractNum As String, _
         ByVal clientID As Integer, ByVal clientName As String, _
         ByVal weFrom As String, ByVal weTo As String, ByVal invoiceNumber As String, ByVal invoiceRef As String, _
         ByVal statusDateFrom As String, ByVal statusDateTo As String, _
         ByVal statusIsUnpaid As Boolean, ByVal statusIsAuthorised As Boolean, _
         ByVal statusIsPaid As Boolean, ByVal statusIsSuspended As Boolean, ByVal exclude As String, _
         ByVal invNumFilter As String, ByVal invRefFilter As String) As FetchDomProviderInvoiceListCountResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderInvoiceListCountResult = New FetchDomProviderInvoiceListCountResult
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

                ' get the list of provider invoices
                If page = 0 Then
                    '++ Deliberate means to retrieve results unrestricted by paging..
                    page = 1
                    pageSize = 1000000
                End If

                msg = DomContractBL.FetchDomProviderInvoiceListCount( _
                 conn, page, pageSize, selectedDomProviderInvoiceID, batchID, _
                 providerID, providerName, domContractID, domContractNum, _
                 clientID, clientName, _
                 weFrom, weTo, invoiceNumber, invoiceRef, statusDateFrom, statusDateTo, _
                 statusIsUnpaid, statusIsAuthorised, statusIsPaid, statusIsSuspended, _
                 exclude, invNumFilter, invRefFilter, totalRecords, result.Invoices)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDomProviderInvoiceListCount({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of debtor invoices.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="invoiceID">The ID of the invoice to select.</param>
        ''' <param name="clientID">The ID of the service user to filter the results on.</param>
        ''' <param name="invRes">Include residential invoices filter</param>
        ''' <param name="invDom">Include domiciliary invoices filter</param>
        ''' <param name="invLD">Include learning disability invoices filter</param>
        ''' <param name="invClient">Include client invoices filter</param>
        ''' <param name="invTP">Include third party invoices filter</param>
        ''' <param name="invProp">Include property invoices filter</param>
        ''' <param name="invOLA">Include OLA invoices filter</param>
        ''' <param name="invPenColl">Include pension collection invoices filter</param>
        ''' <param name="invHomeColl">Include home collection invoices filter</param>
        ''' <param name="invStd">Include standard (i.e. non-manual) invoices filter</param>
        ''' <param name="invManual">Include manual invoices filter</param>
        ''' <param name="invSDS">Include SDS invoices filter</param>
        ''' <param name="invActual">Include non-provisional invoices filter</param>
        ''' <param name="invProvisional">Include provisional invoices filter</param>
        ''' <param name="invRetracted">Include retracted invoices filter</param>
        ''' <param name="invViaRetract">Include invoices produced via retraction filter</param>
        ''' <param name="invZeroValue">Include zero-value invoices filter</param>
        ''' <param name="invCreateDateFrom">Include invoices created on/after this date filter</param>
        ''' <param name="invCreateDateTo">Include invoices created on/before this date filter</param>
        ''' <param name="invBatchSelector">Include invoices on batch status filter</param>
        ''' <param name="listDebtorNameFilter">Filter on debtor name</param>
        ''' <param name="listInvoiceNumFilter">Filter on invoice number</param>
        ''' <param name="listClientRefFilter">Filter on client ref</param>
        ''' <param name="listCommentFilter">Filter on additional info</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        '''     MikeVO        24/05/2011  SDS issue #654 - improve debtor invoice performance.
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDebtorInvoiceList(ByVal page As Integer, ByVal invoiceID As String, _
                 ByVal clientID As String, ByVal invRes As String, ByVal invDom As String, _
                 ByVal invLD As String, ByVal invClient As String, ByVal invTP As String, _
                 ByVal invProp As String, ByVal invOLA As String, ByVal invPenColl As String, _
                 ByVal invHomeColl As String, ByVal invStd As String, ByVal invManual As String, _
                 ByVal invSDS As String, ByVal invActual As String, ByVal invProvisional As String, _
                 ByVal invRetracted As String, ByVal invViaRetract As String, ByVal invZeroValue As String, _
                 ByVal invCreateDateFrom As String, ByVal invCreateDateTo As String, _
                 ByVal invBatchSelector As String, ByVal invExclude As String, _
                 ByVal listDebtorNameFilter As String, ByVal listInvoiceNumFilter As String, _
                 ByVal listClientRefFilter As String, ByVal listCommentFilter As String, _
                 ByVal batchID As String) As FetchDebtorInvoiceListResult

            Dim msg As ErrorMessage
            Dim result As FetchDebtorInvoiceListResult = New FetchDebtorInvoiceListResult
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

                '++ Get the list of debtor invoices..
                If page = 0 Then
                    '++ Deliberate means to retrieve results unrestricted by paging..
                    page = 1
                    pageSize = 1000000
                End If

                msg = DomContractBL.FetchDebtorInvoiceList( _
                        conn, page, pageSize, invoiceID, clientID, invRes, invDom, invLD, invClient, invTP, invProp, _
                        invOLA, invPenColl, invHomeColl, invStd, invManual, invSDS, _
                        invActual, invProvisional, invRetracted, invViaRetract, invZeroValue, _
                        invCreateDateFrom, invCreateDateTo, invBatchSelector, _
                        invExclude, listDebtorNameFilter, listInvoiceNumFilter, _
                        listClientRefFilter, listCommentFilter, batchID, _
                        totalRecords, result.Invoices)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:DebtorInvoiceResults_FetchDebtorInvoiceList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDebtorInvoiceBatchList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of debtor invoice batches.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="batchID">The ID of the batch to select.</param>
        ''' <param name="createdBy">The name pattern of the user who created the batch to filter on.</param>
        ''' <param name="created">The date/time (as a string) pattern of the batch created date to filter on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  29/07/2009  D11547 - support for created filter.
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDebtorInvoiceBatchList(ByVal page As Integer, _
                                                            ByVal batchID As Integer, _
                                                            ByVal createdBy As String, _
                                                            ByVal created As String) As FetchDebtorInvoiceBatchListResult

            Dim msg As ErrorMessage
            Dim result As FetchDebtorInvoiceBatchListResult = New FetchDebtorInvoiceBatchListResult
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

                ' get the list of provider invoices
                msg = DomContractBL.FetchDebtorInvoiceBatchList( _
                     conn, page, pageSize, batchID, createdBy, created, _
                     totalRecords, result.Batches)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDebtorInvoiceBatchList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " DomProviderInvoiceToggleExclude "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a single invoice where the 'exclude from creditors' flag is being toggled.
        ''' </summary>
        ''' <param name="invoiceID">The ID of the invoice to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DomProviderInvoiceToggleExclude(ByVal invoiceID As String) As DomProviderInvoiceToggleExcludeResult

            Dim msg As ErrorMessage
            Dim result As DomProviderInvoiceToggleExcludeResult = New DomProviderInvoiceToggleExcludeResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.DomProviderInvoiceToggleExclude( _
                        conn, invoiceID, totalRecords, result.Invoices)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:DomProviderInvoiceToggleExclude({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      1, 1)
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchRegisterList "

        ''' <summary>
        ''' Retrieves a paginated list of registers.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedRegisterID">The ID of the register to select.</param>
        ''' <param name="establishmentID">The ID of the establishment to filter the results on.</param>
        ''' <param name="dateFrom">The start of the period to filter the results on.</param>
        ''' <param name="dateTo">The end of the period to filter the result on.</param>
        ''' <param name="contractID">The id of the contract .</param>
        ''' <param name="regStatusInProgress">Filter on Inprogress Registers</param>
        ''' <param name="regStatusSubmitted">Filter on submitted Registers</param>
        ''' <param name="regStatusAmended">Filter on amended Registers</param>
        ''' <param name="regStatusProcessed">Filter on processed Registers</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRegisterList(ByVal page As Integer, ByVal selectedRegisterID As Integer, _
                     ByVal establishmentID As Integer, _
                     ByVal dateFrom As Date, ByVal dateTo As Date, ByVal contractID As Integer, _
                     ByVal regStatusInProgress As String, ByVal regStatusSubmitted As String, _
                     ByVal regStatusAmended As String, ByVal regStatusProcessed As String, _
                     ByVal strProviderName As String, ByVal strContractNumber As String, _
                     ByVal strContractTitle As String, ByVal strStatus As String) As FetchRegisterListResult

            Dim msg As ErrorMessage
            Dim result As FetchRegisterListResult = New FetchRegisterListResult
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

                ' get the list of registers
                msg = DomContractBL.FetchRegisterList( _
                 conn, page, pageSize, selectedRegisterID, establishmentID, dateFrom, dateTo, contractID, _
                 Convert.ToBoolean(regStatusInProgress), Convert.ToBoolean(regStatusSubmitted), Convert.ToBoolean(regStatusAmended), Convert.ToBoolean(regStatusProcessed), _
                 settings.CurrentApplicationID, _
                 totalRecords, strProviderName, strContractNumber, strContractTitle, strStatus, result.Registers)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchRegisterList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchRowVwRegisterWeek "

        ''' <summary>
        ''' Retrieves a list of registers rows from the view vwRegisterWeek .
        ''' </summary>
        ''' <param name="vwRegisterWeekID">the id of the row in vwRegisterWeek.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRowVwRegisterWeek(ByVal vwRegisterWeekID As String) As FetchVwRegisterWeekListResult

            Dim msg As ErrorMessage
            Dim result As FetchVwRegisterWeekListResult = New FetchVwRegisterWeekListResult
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of registers
                msg = DomContractBL.FetchRegisterVwWeekRow(conn, vwRegisterWeekID, result.vwRegisterWeekRows)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
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

#Region " FetchServiceOutcomeList "

        ''' <summary>
        ''' D11681
        ''' </summary>
        ''' <param name="contractID"></param>
        ''' <param name="cellDate"></param>
        ''' <param name="checked"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceOutcomeList(ByVal contractID As Integer, ByVal cellDate As Date, ByVal checked As String) As FetchServiceOutcomeListResult

            Dim msg As ErrorMessage
            Dim result As FetchServiceOutcomeListResult = New FetchServiceOutcomeListResult
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of registers
                msg = AbacusClassesBL.FetchServiceOutcomesByRegisterDay(conn, contractID, result.ServiceOutcomes, cellDate, checked)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
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

#Region " FetchTimeBandsAvailableToDayCategory "

        ''' <summary>
        ''' Retrieves a list of time bands available to a day category.
        ''' </summary>
        ''' <param name="dayCategoryID">The ID of the day category.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchTimeBandsAvailableToDayCategory(ByVal dayCategoryID As Integer) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim dayCategory As DomDayCategory
            Dim timeBands As DomTimeBandCollection = Nothing
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the time band group from the day category
                dayCategory = New DomDayCategory(conn, String.Empty, String.Empty)
                msg = dayCategory.Fetch(dayCategoryID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                ' get the list of time bands
                msg = DomTimeBand.FetchList(conn, timeBands, String.Empty, String.Empty, dayCategory.DomTimeBandGroupID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                result.List = New List(Of ViewablePair)
                For Each timeBand As DomTimeBand In timeBands
                    result.List.Add(New ViewablePair(timeBand.ID, timeBand.Description))
                Next

                With result
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

#Region " FetchSystemAccountList "

        ''' <summary>
        ''' Retrieves a list of system accounts for the specified contract period.
        ''' </summary>
        ''' <param name="providerID">Provider ID</param>
        ''' <param name="domContractID">Contract ID</param>
        ''' <param name="dateFrom">Date Range From</param>
        ''' <param name="dateTo">Date Range To</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSystemAccountList(ByVal providerID As Integer, ByVal domContractID As Integer, _
                                               ByVal dateFrom As Date, ByVal dateTo As Date) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing

            Try

                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of accounts
                result.List = New List(Of ViewablePair)
                msg = DomContractBL.FetchDomContractPeriodSystemAccountList(conn, providerID, domContractID, dateFrom, dateTo, result.List)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
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

#Region " FetchSystemAccountListByPeriod "
        ''' <summary>
        ''' Retrieves a list of system accounts for the specified contract period.
        ''' </summary>
        ''' <param name="providerID">Provider ID</param>
        ''' <param name="domContractID">Contract ID</param>
        '''<param name="periodID">The ID of a contract period</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSystemAccountListByPeriod(ByVal providerID As Integer, ByVal domContractID As Integer, _
                                               ByVal periodID As Integer) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing

            Dim period As DomContractPeriod = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                'fetch the period
                period = New DomContractPeriod(conn, String.Empty, String.Empty)
                msg = period.Fetch(periodID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the list of accounts
                result.List = New List(Of ViewablePair)
                msg = DomContractBL.FetchDomContractPeriodSystemAccountList(conn, providerID, domContractID, period.DateFrom, period.DateTo, result.List)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
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

#Region " FetchContractPeriodList "

        ''' <summary>
        ''' Retrieves a list of system accounts for the specified contract period.
        ''' </summary>
        ''' <param name="domContractID">Contract ID</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchContractPeriodList(ByVal domContractID As Integer) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim periods As DomContractPeriodCollection = Nothing
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing
            Dim currentWeekEndingDate As Date = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                currentWeekEndingDate = DomContractBL.GetWeekEndingDate(conn, Nothing)

                ' get periods
                msg = DomContractPeriod.FetchList(conn, periods, String.Empty, String.Empty, domContractID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                periods.Sort(New CollectionSorter("DateFrom", SortDirection.Descending))
                result.List = New List(Of ViewablePair)
                For Each period As DomContractPeriod In periods
                    'We dont want periods that start in the future (i.e. after the current weekending date)
                    If period.DateFrom <= currentWeekEndingDate Then
                        result.List.Add(New ViewablePair(period.ID, String.Format("{0} - {1}", period.DateFrom.ToString("dd/MM/yyyy"), _
                                                                                  period.DateTo.ToString("dd/MM/yyyy"))))
                    End If
                Next

                With result
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

#Region " FetchManualPaymentDomProformaList "

        ''' <summary>
        ''' Retrieves a paginated list of domiciliary contracts.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="providerID">The ID of the provider to filter the results on.</param>
        ''' <param name="contractID">The ID of the contract to filter the results on.</param>
        ''' <param name="systemAccountID">The ID of the system account to filter the results on.</param>
        ''' <param name="selectedInvoiceID">The ID of the invoice to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchManualPaymentDomProformaList(ByVal page As Integer, ByVal providerID As Integer, _
                ByVal contractID As Integer, ByVal systemAccountID As Integer, _
                ByVal selectedInvoiceID As Integer) As FetchManualPaymentDomProformaListResult

            Dim msg As ErrorMessage
            Dim result As FetchManualPaymentDomProformaListResult = New FetchManualPaymentDomProformaListResult
            Dim currentUser As WebSecurityUser
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

                currentUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                msg = DomContractBL.FetchManualPaymentDomProformaInvoiceEnquiryResults( _
                 conn, currentUser.ExternalUserID, page, pageSize, selectedInvoiceID, _
                 providerID, contractID, systemAccountID, totalRecords, result.Invoices)
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

#Region " GetUnitCost "

        ''' <summary>
        ''' Retrieves the unit cost for a rate category for a contract period.
        ''' </summary>
        ''' <param name="contractPeriodID">The ID of the contract period.</param>
        ''' <param name="rateCategoryID">The ID of the rate category.</param>
        ''' <param name="rowID">A caller defined value to pass through back to the client-side callback function.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetUnitCost(ByVal contractPeriodID As Integer, ByVal rateCategoryID As Integer, _
                                    ByVal rowID As String) As GetUnitCostResult

            Dim msg As ErrorMessage
            Dim result As GetUnitCostResult = New GetUnitCostResult
            Dim currentUser As WebSecurityUser
            Dim conn As SqlConnection = Nothing
            Dim unitCosts As DomContractDomRateCategory_DomContractPeriodCollection = Nothing
            Dim unitCost As Decimal

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                currentUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.GetUnitCost(conn, contractPeriodID, rateCategoryID, unitCost)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .UnitCost = unitCost
                    .RowID = rowID
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchReOpenedWeekList "

        ''' <summary>
        ''' Retrieves a filtered list of domiciliary contract re-opened weeks.
        ''' </summary>
        ''' <param name="page">The page in the resultset to retrieve.</param>
        ''' <param name="providerID">The ID os the provider to filter the results on, or zero.</param>
        ''' <param name="contractID">The ID of the contract to filter the results on, or zero.</param>
        ''' <param name="weDateFrom">The start of the date range to filter the week ending date on, or Nothing.</param>
        ''' <param name="weDateTo">The end of the date range to filter the week ending date on, or Nothing.</param>
        ''' <param name="closureDateFrom">The start of the date range to filter the expected closure date on, or Nothing.</param>
        ''' <param name="closureDateTo">The end of the date range to filter the expected closure date on, or Nothing.</param>
        ''' <param name="selectedWeekID">The ID of the week record to select and return the page for in the resultset.</param>
        ''' <param name="filterReOpenedBy">The re-opened by user to filter the result on, or Nothing.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Shared Function FetchReOpenedWeekList(ByVal page As Integer, ByVal providerID As Integer, ByVal contractID As Integer, _
                ByVal weDateFrom As Date, ByVal weDateTo As Date, ByVal closureDateFrom As Date, ByVal closureDateTo As Date, _
                ByVal selectedWeekID As Integer, ByVal filterReOpenedBy As String) As FetchReOpenedWeekListResult

            Dim msg As ErrorMessage
            Dim result As FetchReOpenedWeekListResult = New FetchReOpenedWeekListResult()
            Dim currentUser As WebSecurityUser
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

                currentUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchReOpenedWeekList(conn, page, pageSize, providerID, contractID, _
                    weDateFrom, weDateTo, closureDateFrom, closureDateTo, selectedWeekID, filterReOpenedBy, _
                    totalRecords, result.Weeks)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchReOpenedWeekList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " RateFrameworkUsesEnhancedRateDays "

        ''' <summary>
        ''' Determines if the specified rate framework uses enhanced rate days or not.
        ''' </summary>
        ''' <param name="rateFrameworkID">The ID of the rate framework.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function RateFrameworkUsesEnhancedRateDays(ByVal rateFrameworkID As Integer) As BooleanResult

            Dim msg As ErrorMessage
            Dim result As BooleanResult = New BooleanResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                result.ErrMsg = DomContractBL.RateFrameworkUsesEnhancedRateDays(conn, rateFrameworkID, result.Value)

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchUnitsOfMeasureList "

        ''' <summary>
        ''' Retrieves a list of units of measure.
        ''' </summary>
        ''' <param name="visitBased">Whether to retreive non-visit-based or visit-based units.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  08/01/2009  D11469 - created.
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchUnitsOfMeasureList(ByVal visitBased As Boolean) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim uoms As DomUnitsOfMeasureCollection = Nothing
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of uoms
                result.List = New List(Of ViewablePair)
                msg = DomUnitsOfMeasure.FetchList(conn, uoms, String.Empty, String.Empty, visitBased)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                uoms.Sort(New CollectionSorter("Description", SortDirection.Ascending))
                For Each uom As DomUnitsOfMeasure In uoms
                    result.List.Add(New ViewablePair(uom.ID, uom.Description))
                Next

                With result
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

#Region " FetchServiceOrderSuspensionPeriodList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of service order Suspensions.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedServiceOrderSuspensionID">The ID of the Service Order Suspension to select.</param>
        ''' <param name="serviceUserID">The ID of the User to filter the results on.</param>
        ''' <param name="suspensionReasonID">The ID of the Suspension End Reason to filter the results on.</param>
        ''' <param name="dateFrom">The start of the period to filter the results on.</param>
        ''' <param name="dateTo">The end of the period to filter the result son.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceOrderSuspensionPeriodList(ByVal page As Integer, ByVal selectedServiceOrderSuspensionID As Integer, _
         ByVal serviceUserID As Integer, ByVal suspensionReasonID As Integer, _
         ByVal dateFrom As Date, ByVal dateTo As Date) As FetchServiceOrderSuspensionPeriodListResult

            Dim msg As ErrorMessage
            Dim result As FetchServiceOrderSuspensionPeriodListResult = New FetchServiceOrderSuspensionPeriodListResult
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

                ' get the list of suspensions
                msg = ServiceOrderSuspensionsBL.FetchServiceOrderSuspensionPeriodList( _
                 conn, page, pageSize, selectedServiceOrderSuspensionID, serviceUserID, suspensionReasonID, dateFrom, dateTo, _
                 totalRecords, result.Suspensions)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchServiceOrderSuspensionPeriodList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProviderInvoiceDetailRates "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of distinct rates for the contract/client/week ending/rate category.
        ''' </summary>
        ''' <param name="domContractID">the ID of the domiciliary contract.</param>
        ''' <param name="clientID">The ID of the client.</param>
        ''' <param name="weekEnding">The week ending date.</param>
        ''' <param name="rateCategoryID">The ID of the rate category.</param>
        ''' <param name="tag">Caller defined value that is passed back in the callback.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceDetailRates(ByVal domContractID As Integer, _
                                                           ByVal clientID As Integer, _
                                                           ByVal weekEnding As Date, _
                                                           ByVal rateCategoryID As Integer, _
                                                           ByVal tag As String) As StringListResult

            Dim msg As ErrorMessage
            Dim result As StringListResult = New StringListResult
            Dim conn As SqlConnection = Nothing
            Dim formattedRates As List(Of String) = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DPI.DetailLine.FetchRateList(conn, domContractID, clientID, weekEnding, rateCategoryID, formattedRates)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage()
                    .ErrMsg.Success = True
                    .Values = formattedRates
                    .Tag = tag
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return result

        End Function

#End Region

#Region " FetchDomProviderInvoiceVATRate "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves the VAT rate applicable to the provider invoice ID passed.
        ''' </summary>
        ''' <param name="dpiDate">the Date of the dom provider invoice.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MoTahir  22/12/2010  A4WA#6580 - VAT Rate not corking correctly.
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceVATRate(ByVal dpiDate As String) As StringListResult

            Dim msg As ErrorMessage
            Dim result As StringListResult = New StringListResult
            Dim conn As SqlConnection = Nothing
            Dim vatRate As List(Of String) = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                msg = DPI.DomProviderInvoiceBL.FetchDomProviderInvoiceVATRate(conn, dpiDate, vatRate)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage()
                    .ErrMsg.Success = True
                    .Values = vatRate
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return result

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
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

#Region " FetchDomProviderInvoiceVisitsEnquiryResults "

        ''' <summary>
        ''' Retrieves a list of domiciliary provider invoice Visits for use with the invoiced visits
        ''' screen. The results being filtered by the specified filters below and by the 
        ''' provider that the specified user has rights to view.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="providerID">the ID of the provider.</param>
        ''' <param name="clientID">the service user ID to filter on.</param>
        ''' <param name="contractID">The ID of the contract to filter on.</param>
        ''' <param name="dateFrom">The date from, to filter on.</param>
        ''' <param name="dateTo">The date to, to filter on.</param>
        ''' <param name="selectedVisitID">The selected visit id</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceVisitsEnquiryResults( _
            ByVal page As Integer, ByVal providerID As Integer, ByVal clientID As Integer, _
            ByVal contractID As Integer, ByVal dateFrom As Date, _
            ByVal dateTo As Date, ByVal selectedVisitID As Integer) As FetchDomProviderInvoiceVisitsEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderInvoiceVisitsEnquiryListResult = New FetchDomProviderInvoiceVisitsEnquiryListResult
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
                msg = DomContractBL.FetchDomProviderInvoiceVisitsEnquiryIntranetResults( _
                    conn, user.ExternalUserID, page, pageSize, providerID, clientID, contractID, _
                      dateFrom, dateTo, selectedVisitID, totalRecords, result.Visits)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchVisitsList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " DomProviderInvoiceNotes_CreateNew "

        ''' <summary>
        ''' Creates an new note.
        ''' </summary>
        ''' <param name="InvoiceID">The ID of the invoice the note is to be linked to.</param>
        ''' <param name="noteID">The ID of the note, 0 indicates a new note.</param>
        ''' <param name="note">The text of the new note.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DomProviderInvoiceNotes_CreateNew(ByVal InvoiceID As Integer, ByVal noteID As Integer, ByVal note As String) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim newNote As DomProviderInvoiceNotes
            Dim currentUser As WebSecurityUser

            If (InvoiceID <= 0) Or Not IsNumeric(InvoiceID) Then
                msg = New ErrorMessage
                msg.Success = False
                msg.Message = "An DOM Provider Invoice ID must be supplied."
                Return msg
            End If

            Try
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then Return msg

                currentUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                newNote = New DomProviderInvoiceNotes(conn)

                If noteID > 0 Then
                    msg = newNote.Fetch(noteID)
                    If Not msg.Success Then Return msg
                    newNote.DateAmended = Date.Now
                    newNote.AmendedBy = currentUser.ExternalUsername
                Else
                    newNote.Date = Date.Now
                    newNote.Time = Date.Now
                    newNote.EnteredBy = currentUser.ExternalUsername
                End If

                'set new properties of note record
                newNote.DomProviderInvoiceID = InvoiceID
                newNote.Notes = note
                msg = newNote.Save()
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

#Region " DomProviderInvoiceNotes_fetchNoteText "

        ''' <summary>
        ''' Creates an new note.
        ''' </summary>
        ''' <param name="noteID">The ID of the note.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DomProviderInvoiceNotes_fetchNoteText(ByVal noteID As Integer, ByRef noteText As String) As StringResult

            Dim msg As ErrorMessage
            Dim result As StringResult = New StringResult
            Dim conn As SqlConnection = Nothing
            Dim newNote As DomProviderInvoiceNotes

            Try
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                newNote = New DomProviderInvoiceNotes(conn)
                msg = newNote.Fetch(noteID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                Else
                    'return the note
                    result.Value = newNote.Notes
                End If

                msg = New ErrorMessage
                msg.Success = True
                result.ErrMsg = msg

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return result


        End Function

#End Region

#Region " DomProviderInvoiceNotes_deleteNote "

        ''' <summary>
        ''' Deletes a note.
        ''' </summary>
        ''' <param name="noteID">The ID of the note.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DomProviderInvoiceNotes_deleteNote(ByVal noteID As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing

            Try
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then Return msg

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomProviderInvoiceNotes.Delete(conn, noteID)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg


        End Function

#End Region

#Region " NoServiceGroupsAvailableToUser "

        ''' <summary>
        ''' Returns a count of Service Groups available to a specific user.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function NoServiceGroupsAvailableToUser(ByVal ignoreBlankFrameworks As Boolean) As Integer

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim groups As vwWebSecurityRole_ServiceGroupCollection = Nothing
            Dim noGroups As Integer = 0
            Dim includeWithNoFrameworkTypeID As TriState = TriState.UseDefault

            Try

                If ignoreBlankFrameworks Then
                    ' if caller wants to ignore results with no framework then apply 
                    ' else we should leave as default

                    includeWithNoFrameworkTypeID = TriState.True

                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' see if the user is allowed to perform the operation
                msg = DomContractBL.GetServiceGroupsAvailableToUser(conn, user.ID, includeWithNoFrameworkTypeID, groups)

                noGroups = groups.Count

            Catch ex As Exception

                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected

            Finally

                SqlHelper.CloseConnection(conn)

            End Try

            Return noGroups

        End Function

#End Region

#Region " FetchOnlyAvailableServiceGroupToUser "

        ''' <summary>
        ''' Returns the ID of a service group available to a specific user.
        ''' This should only be used is its know that a user only has
        ''' permission for one service Group.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchOnlyAvailableServiceGroupToUser() As Integer

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim groups As vwWebSecurityRole_ServiceGroupCollection = Nothing
            Dim serviceGroupID As Integer = 0
            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' see if the user is allowed to perform the operation
                msg = DomContractBL.GetServiceGroupsAvailableToUser(conn, user.ID, groups)

                serviceGroupID = groups.Item(0).ServiceGroupID

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return serviceGroupID

        End Function

#End Region

#Region " UpdateAttendanceEffectiveDate "

        ''' <summary>
        ''' Updates an effective date.
        ''' </summary>
        ''' <param name="dsoID">Dom Service Order ID</param>
        ''' <param name="currentEffectiveDate">The Current Effective Date of the attendance Schedule</param>
        ''' <param name="newEffectiveDate">The New effective Date.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function UpdateAttendanceEffectiveDate(ByVal dsoID As Integer, ByVal currentEffectiveDate As Date, ByVal newEffectiveDate As Date) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' see if the user is allowed to perform the operation
                msg = ServiceOrderBL.UpdateAttendanceEffectiveDate(conn, dsoID, currentEffectiveDate, newEffectiveDate)

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

#Region " GetManualInvoiceWeekendingDate "

        ''' <summary>
        ''' Validates a weekending date.
        ''' </summary>
        ''' <param name="domContractPeriodID">Dom Contract Period ID</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetManualInvoiceWeekendingDate(ByVal domContractPeriodID As Integer) As StringResult

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            'Dim period As DomContractPeriod = Nothing
            Dim actualDate As Date = Nothing
            Dim result As StringResult = New StringResult


            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                'period = New DomContractPeriod(conn, String.Empty, String.Empty)
                'msg = period.Fetch(domContractPeriodID)
                'If Not msg.Success Then
                '    result.ErrMsg = msg
                '    Return result
                'End If

                'If period.DateTo < Date.Today Then
                '    actualDate = period.DateTo
                'Else
                '    actualDate = Date.Today
                'End If

                ' Get the weekending date
                msg = DomContractBL.GetManualInvoiceWeekendingDate(conn, domContractPeriodID, actualDate)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                result.Value = actualDate.ToShortDateString

                msg = New ErrorMessage
                msg.Success = True
                result.ErrMsg = msg

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return result

        End Function

#End Region

#Region " PrimeRegister "

        ''' <summary>
        ''' D11681
        ''' </summary>
        ''' <param name="providerID"></param>
        ''' <param name="contractID"></param>
        ''' <param name="weekEndingDate"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function PrimeRegister(ByVal providerID As Integer, ByVal contractID As Integer, _
                                      ByVal weekEndingDate As Date, ByVal pageTitle As String) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, _
                                                      SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' see if the user is allowed to perform the operation
                msg = DomContractBL.PrimeNewRegister(conn, providerID, contractID, weekEndingDate, pageTitle, settings)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

#Region " PrimeRegisterByUser "
        ''' <summary>
        ''' Primes register records for a user
        ''' </summary>
        ''' <param name="clientID"></param>
        ''' <param name="registerID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function PrimeRegisterByUser(ByVal clientID As Integer, ByVal registerID As Integer, _
                                            ByVal pageTitle As String) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, _
                                                                  SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' see if the user is allowed to perform the operation
                msg = DomContractBL.PrimeRegisterByUser(conn, clientID, registerID, pageTitle, settings)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

#Region " GetServiceOutcomeCode "

        ''' <summary>
        ''' retreives the serviceoutcome code 
        ''' </summary>
        ''' <param name="serviceOutcomeID">ID of the service outcome</param>
        ''' <returns></returns>
        ''' <remarks>MOT Created 06/12/2009 D11681</remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetServiceOutcomeCode(ByVal serviceOutcomeID As Integer) As StringResult

            Dim msg As ErrorMessage
            Dim result As StringResult = New StringResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.GetServiceOutcomeCode(conn, serviceOutcomeID, result.Value)
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

#Region " GetDateFromWeekDayNumber "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dayID"></param>
        ''' <param name="weekEnding"></param>
        ''' <returns></returns>
        ''' <remarks>Mo D11681 Created 01/12/2009</remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetDateFromWeekDayNumber(ByVal dayID As Integer, ByVal weekEndDayId As Integer, ByVal weekEnding As String) As Date

            Dim msg As ErrorMessage
            Dim result As StringResult = New StringResult
            Dim conn As SqlConnection = Nothing
            Dim dayDate As Date

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return Nothing
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.GetDateFromWeekDayNumber(conn, dayID, weekEndDayId, weekEnding, dayDate)
                If Not msg.Success Then
                    Return Nothing
                End If

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return dayDate

        End Function

#End Region

#Region " AmendRegister "
        ''' <summary>
        ''' D11681
        ''' </summary>
        ''' <param name="registerID"></param>
        ''' <param name="registerStatus"></param>
        ''' <returns></returns>
        ''' <remarks>Mo Tahir Created 31/12/2009</remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function AmendRegister(ByVal registerID As Integer, ByVal registerStatus As String, _
                                      ByVal pageTitle As String) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, _
                                                      SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                ' see if the user is allowed to perform the operation
                msg = DomContractBL.AmendRegister(conn, registerID, registerStatus, pageTitle, settings)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

#Region " DeleteRegister "
        ''' <summary>
        ''' D11681
        ''' </summary>
        ''' <param name="registerID"></param>
        ''' <param name="registerStatus"></param>
        ''' <returns></returns>
        ''' <remarks>Mo Tahir Created 04/01/2010</remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DeleteRegister(ByVal registerID As Integer, ByVal registerStatus As String) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                ' see if the user is allowed to perform the operation
                msg = DomContractBL.DeleteRegister(conn, registerID, registerStatus)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

#Region " SubmitRegister "
        ''' <summary>
        ''' D11681
        ''' </summary>
        ''' <param name="registerID"></param>
        ''' <param name="registerStatus"></param>
        ''' <returns></returns>
        ''' <remarks>Mo Tahir Created 31/12/2009</remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function SubmitRegister(ByVal registerID As Integer, ByVal registerStatus As String, _
                                       ByVal pageTitle As String) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, _
                                                                  SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                ' see if the user is allowed to perform the operation
                msg = DomContractBL.SubmitRegister(conn, registerID, registerStatus, pageTitle, settings)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

#Region " CheckServiceDay "
        ''' <summary>
        ''' D11681
        ''' </summary>
        ''' <param name="contractID"></param>
        ''' <param name="dayOfWeekDate"></param>
        ''' <param name="dayOfWeek"></param>
        ''' <returns></returns>
        ''' <remarks>Created 19/01/2009 Mo</remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CheckServiceDay(ByVal contractID As Integer, ByVal dayOfWeekDate As Date, ByVal dayOfWeek As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                ' see if the user is allowed to perform the operation
                msg = DomContractBL.CheckServiceDay(conn, contractID, dayOfWeekDate, dayOfWeek)
                If Not msg.Success Then Return msg

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

#Region " FetchRatePreclusionsList "
        ''' <summary>
        ''' D11681
        ''' </summary>
        ''' <param name="contractID"></param>
        ''' <returns></returns>
        ''' <remarks>Created 20/01/2010 Mo</remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRatePreclusionsList(ByVal contractID As Integer) As FetchRatePreclusionListResult

            Dim msg As ErrorMessage
            Dim result As FetchRatePreclusionListResult = New FetchRatePreclusionListResult
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of preclsuions
                msg = DomContractBL.FetchRatePreclusionsList(conn, contractID, result.RatePreclusions)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
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

#Region " ValidateRegisterPriorToSave "
        ''' <summary>
        ''' D11681
        ''' </summary>
        ''' <param name="cells"></param>
        ''' <returns></returns>
        ''' <remarks>Created 09/03/2010 PaulW </remarks>
        ''' <history>
        ''' MoTahir 25/10/2010  A4WA#6508 - Unexpected Error and Very Slow Operation of Register
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ValidateRegisterPriorToSave(ByVal cells As List(Of ViewableRegisterCell)) As String

            Dim msg As ErrorMessage
            Dim result As FetchRatePreclusionListResult = New FetchRatePreclusionListResult
            Dim conn As SqlConnection = Nothing
            Dim tmpErrorMessages As String = String.Empty
            Dim rateCategory As DomRateCategory
            Dim inclusions As DomRateCategoryInclusionsCollection = Nothing
            Dim count As Integer = 0

            conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

            rateCategory = New DomRateCategory(conn, String.Empty, String.Empty)
            msg = rateCategory.Fetch(cells(0).DomRateCategoryID)
            If Not msg.Success Then Return msg.Message

            msg = DomRateCategoryInclusions.FetchList(conn, inclusions, rateCategory.DomRateFrameworkID)
            If Not msg.Success Then Return msg.Message

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return msg.Message
                End If

                For Each regCell As ViewableRegisterCell In cells

                    For Each inc As DomRateCategoryInclusions In inclusions
                        If inc.DomRateFrameworkID = rateCategory.DomRateFrameworkID And _
                        inc.DomRateCategoryID = rateCategory.ID Then
                            count += 1
                        End If
                    Next

                    If count > 0 Then
                        Dim includedInPlan As Boolean = False
                        'loop round each detail line
                        For Each regCell_compare As ViewableRegisterCell In cells
                            'loop round each inclusion record
                            For Each inclusion As DomRateCategoryInclusions In inclusions
                                If inclusion.IncludedDomRateCategoryID = regCell_compare.DomRateCategoryID AndAlso _
                                        regCell.DayOfWeek = regCell_compare.DayOfWeek AndAlso _
                                        regCell.ClientID = regCell_compare.ClientID Then

                                    includedInPlan = True

                                End If

                            Next
                        Next
                        If Not includedInPlan Then
                            'is this message already added to the collection
                            If Not tmpErrorMessages.Contains(String.Format("The rate category '{0}' can not be added to the plan on a {1}, unless one or more of its inclusions is also included on this day.", _
                                                                 rateCategory.Description, [Enum].GetName(GetType(DayOfWeek), regCell.DayOfWeek))) Then
                                tmpErrorMessages = String.Format("{0} {3} The rate category '{1}' can not be added to the plan on a {2}, unless one or more of its inclusions is also included on this day.", _
                                                                 tmpErrorMessages, rateCategory.Description, [Enum].GetName(GetType(DayOfWeek), regCell.DayOfWeek), vbCrLf)
                            End If
                        End If
                    End If

                Next


            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                'If Not conn Is Nothing Then conn.Close()
            End Try

            Return tmpErrorMessages

        End Function

#End Region

#Region " FetchRateFrameworkList "

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="listFilterRateFramework"></param>
        ''' <param name="listFilterFrameWorkType"></param>
        ''' <param name="listFilterRedundant"></param>
        ''' <returns></returns>
        ''' <remarks>Mo Created 16/03/2010</remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRateFrameworkList(ByVal page As Integer, _
          ByVal listFilterRateFramework As String, ByVal listFilterFrameWorkType As String, _
          ByVal listFilterRedundant As String, ByVal selectedId As Integer) As FetchRateFrameworkListResult

            Dim msg As ErrorMessage
            Dim rateframeworks As ArrayList = Nothing
            Dim result As FetchRateFrameworkListResult = New FetchRateFrameworkListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of rateframeworks
                msg = DomContractBL.FetchRateFrameworks( _
                   conn, page, pageSize, listFilterRateFramework, _
                   listFilterFrameWorkType, listFilterRedundant, totalRecords, rateframeworks, selectedId)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .RateFrameworks = rateframeworks
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchRateFrameWorkList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " DisplayDebtorInvoiceIncludeWarningMessage "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     determines if a message should be displayed in the UI when trying to Include an Invoice in a debtors invoice batch.
        ''' </summary>
        ''' <param name="invoiceID">The ID of the invoice.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DisplayDebtorInvoiceIncludeWarningMessage(ByVal invoiceID As Integer) As Boolean

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim displayMessage As Boolean
            Dim inv As Invoice
            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return False
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                inv = New Invoice(conn)
                msg = inv.Fetch(invoiceID)
                If Not msg.Success Then Return False

                If inv.SDSVersion = 2 Then
                    displayMessage = (inv.HasAdditionalCost = False And inv.HasRemainingDebt = False)
                Else
                    displayMessage = False
                End If


            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return displayMessage

        End Function

#End Region

#Region " CanEditRateFramework "

        ''' <summary>
        ''' determines if a rate framework Can be Edited 
        ''' </summary>
        ''' <param name="frameworkID">The framework ID.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CanEditRateFramework(ByVal frameworkID As Integer) As BooleanResult

            Dim conn As SqlConnection = Nothing
            Dim result As BooleanResult = New BooleanResult
            Try
                ' check user is logged in
                result.ErrMsg = SecurityBL.ValidateLogin()
                If Not result.ErrMsg.Success Then
                    result.Value = False
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                'See if there are any overlapping DP Payments for the duration of this spend plan
                result.ErrMsg = DomContractBL.CanEditRateFramework(conn, frameworkID, result.Value)
                If Not result.ErrMsg.Success Then
                    result.Value = False
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



    End Class

End Namespace