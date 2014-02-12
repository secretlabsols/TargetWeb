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
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/DomProviderInvoice")> _
 <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
 <ToolboxItem(False)> _
 Public Class DomProviderInvoice
        Inherits System.Web.Services.WebService

#Region " FetchRetractProviderInvoiceVisibility "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchRetractProviderInvoiceVisibility( _
                ByVal selectedInvoiceID As Integer) As FetchRetractProviderInvoiceVisibilityResults
            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

            Dim result As FetchRetractProviderInvoiceVisibilityResults = New FetchRetractProviderInvoiceVisibilityResults
            Dim retractVisibility As Boolean = False
            msg = DomContractBL.FetchRetractProviderInvoiceVisibility(conn, selectedInvoiceID, retractVisibility)
            result.ErrMsg = msg
            If msg.Success Then
                result.Visibility = retractVisibility
            End If
            Return result
        End Function

#End Region

#Region " FetchDomProviderInvoiceEnquiryResults "

        ''' <summary>
        ''' Retrieves a list of domiciliary provider invoices for use with the Provider
        ''' invoice enquiry screen. The results being filtered by the specified filters below and by the 
        ''' provider that the specified user has rights to view.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="providerID">the ID of the provider.</param>
        ''' <param name="clientID">the service user ID to filter on.</param>
        ''' <param name="contractID">The ID of the contract to filter on.</param>
        ''' <param name="invoiceNumber">Invoice No to filter on.</param>
        ''' <param name="weekendingFrom">The weekending date from, to filter on.</param>
        ''' <param name="weekendingTo">The weekending date to, to filter on.</param>
        ''' <param name="paid">Include invoices that are paid</param>
        ''' <param name="unPaid">Include invoices that are unpaid</param>
        ''' <param name="authorised">Include invoices that are authorised</param>
        ''' <param name="suspended">Include invoices that are suspended</param>
        ''' <param name="dateFrom">The batch status date range start to filter the results on.</param>
        ''' <param name="dateTo">The batch status date range end to filter the results on.</param>
        ''' <param name="selectedInvoiceID">The ID of the invoice to select.</param>
        ''' <param name="listFilterSUReference">The custom list filter string to apply to the S/U reference column.</param>
        ''' <param name="listFilterSUName">The custom list filter string to apply to the S/U name column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceEnquiryResults( _
                ByVal page As Integer, _
                ByVal providerID As Integer, _
                ByVal clientID As Integer, _
                ByVal contractID As Integer, _
                ByVal invoiceNumber As String, _
                ByVal weekendingFrom As Date, _
                ByVal weekendingTo As Date, _
                ByVal unPaid As Boolean, _
                ByVal paid As Boolean, _
                ByVal authorised As Boolean, _
                ByVal suspended As Boolean, _
                ByVal dateFrom As Date, _
                ByVal dateTo As Date, _
                ByVal selectedInvoiceID As Integer, _
                ByVal listFilterSUReference As String, _
                ByVal listFilterSUName As String, _
                ByVal listInvNumber As String, _
                ByVal listFilterPaymentRef As String, _
                ByVal doReTractInvoice As Boolean, _
                ByVal pScheduleId As Integer, _
                ByVal hideRetraction As Boolean) As FetchDomProviderInvoiceEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderInvoiceEnquiryListResult = New FetchDomProviderInvoiceEnquiryListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)


            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' if Invoice is to be Retracted then first Retract and then Pull Results
                ' Retract Invoice if reTractInvoice = true
                If (doReTractInvoice) Then
                    msg = RetractInvoice(conn, selectedInvoiceID, pScheduleId, user.ID, user.ExternalUsername)
                    If msg.Success Then
                        '++ Following used by the JS to display a confirmatory message..
                        result.RetractSuccessful = True
                    Else
                        If Not conn Is Nothing Then conn.Close()
                        result.ErrMsg = msg
                        Return result
                    End If
                End If

                ' get the list of invoices
                msg = DomContractBL.FetchDomProviderInvoiceEnquiryResults( _
                        conn, _
                        user.ExternalUserID, _
                        page, _
                        pageSize, _
                        providerID, _
                        clientID, _
                        contractID, _
                        invoiceNumber, _
                        weekendingFrom, _
                        weekendingTo, _
                        unPaid, _
                        paid, _
                        authorised, _
                        suspended, _
                        dateFrom, _
                        dateTo, _
                        selectedInvoiceID, _
                        listFilterSUReference, _
                        listFilterSUName, _
                        listInvNumber, _
                        listFilterPaymentRef, _
                        pScheduleId, _
                        hideRetraction, _
                        totalRecords, _
                        result.Invoices)
                If Not msg.Success Then
                    If Not conn Is Nothing Then conn.Close()
                    result.ErrMsg = msg
                    Return result
                End If
                ' get the visibility of retract provider if selectedInvoiceId  > 0
                If (selectedInvoiceID > 0) Then
                    Dim retractVisibility As Boolean = False
                    msg = DomContractBL.FetchRetractProviderInvoiceVisibility(conn, selectedInvoiceID, retractVisibility)
                    result.RetractVisible = retractVisibility
                End If
                If Not msg.Success Then
                    If Not conn Is Nothing Then conn.Close()
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
        ''' <summary>
        ''' Retract the specified invoice.
        ''' </summary>
        ''' <param name="conn">The standard connection object.</param>
        ''' <param name="invoiceID">The provider invoice to be retracted.</param>
        ''' <param name="auditUserID">The ID of the current user.</param>
        ''' <param name="a4WName">The name of the current user when using A4W.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RetractInvoice(ByVal conn As SqlConnection, _
                                       ByVal invoiceID As Integer, _
                                       ByVal PaymentScheduleId As Integer, _
                                       ByVal auditUserID As Integer, _
                                       ByVal a4WName As String) As ErrorMessage
            Dim msg As ErrorMessage
            Dim contraInvID As Integer = 0

            '++ before retraction we need to delete the visit amendment requests against the specific invoice 
            msg = New ErrorMessage
            msg = Target.Abacus.Library.DomContractBL.DeleteUnProcessedAmendmentRequest(conn, invoiceID)
            If Not msg.Success Then Return msg

            '++ retract the invoice 
            msg = Target.Abacus.Library.DomProviderInvoice.DomProviderInvoiceBL.Retract(conn, _
                                                            auditUserID, a4WName, invoiceID, contraInvID)

            If Not msg.Success Then Return msg

            '++ after retraction We need to Recalculate PaymentSchedule Total counts
            msg = New ErrorMessage
            msg = Target.Abacus.Library.DomContractBL.PaymentscheduleRecalculateCountsAttributesAndNetValues_SpecificType(conn, PaymentScheduleId, True, True, True)

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
        ''' <param name="careWorkerID">The ID of the care Worker to filter on.</param>
        ''' <param name="dateFrom">The start of the date range to filter on.</param>
        ''' <param name="dateTo">The end of the date range to filter on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  19/10/2009  D11546 - renamed "pro forma" to "provider".
        '''                     Changed weekending filter to be a date range.
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceVisitsEnquiryResults( _
                ByVal page As Integer, _
                ByVal providerID As Integer, _
                ByVal clientID As Integer, _
                ByVal contractID As Integer, _
                ByVal careWorkerID As Integer, _
                ByVal dateFrom As Date, _
                ByVal dateTo As Date) As FetchDomProviderInvoiceVisitsEnquiryListResult

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
                msg = DomContractBL.FetchDomProviderInvoiceVisitsEnquiryResults( _
                    conn, _
                    user.ExternalUserID, _
                    page, _
                    pageSize, _
                    providerID, _
                    clientID, _
                    contractID, _
                    careWorkerID, _
                    dateFrom, _
                    dateTo, _
                    totalRecords, _
                    result.Visits _
                )
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

#Region " FetchVisitAmendmentRequestEnquiryResults "

        ''' <summary>
        ''' Retrieves a list of Visit amendment requests for use with the enquiry
        ''' screen. The results being filtered by the specified filters below. 
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="providerID">the ID of the provider.</param>
        ''' <param name="requestDateFrom">The Request date range start to filter the results on.</param>
        ''' <param name="requestDateTo">The Request date range end to filter the results on.</param>
        ''' <param name="status">The status value to filter results on.</param>
        ''' <param name="statusDateFrom">The date from, to filter status dates on.</param>
        ''' <param name="statusDateTo">the date to, to filter status dates on.</param>
        ''' <param name="contractID">The ID of the contract to filter on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchVisitAmendmentRequestEnquiryResults( _
            ByVal page As Integer, _
            ByVal providerID As Integer, _
            ByVal contractID As Integer, _
            ByVal requestDateFrom As Date, _
            ByVal requestDateTo As Date, _
            ByVal status As Integer, _
            ByVal statusDateFrom As Date, _
            ByVal statusDateTo As Date, _
            ByVal reqCompanyID As Integer, _
            ByVal reqUserID As Integer, _
            ByVal clientID As Integer, _
            ByVal pScheduleId As Integer, _
            ByVal listFilterServiceUser As String, _
            ByVal listFilterSUReference As String) As FetchVisitAmendmentRequestEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchVisitAmendmentRequestEnquiryListResult = New FetchVisitAmendmentRequestEnquiryListResult
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
                msg = DomContractBL.FetchVisitAmendmentRequestEnquiryResults( _
                    conn, user.ExternalUserID, page, pageSize, providerID, requestDateFrom, requestDateTo, _
                    status, statusDateFrom, statusDateTo, contractID, reqCompanyID, reqUserID, clientID, pScheduleId, _
                    totalRecords, listFilterServiceUser, listFilterSUReference, result.Amendments)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchAmendmentList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " Dom Provider Invoice Detail Non Delivery Unit Based Enquiry Results "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceDetailNonDeliveryUnitBasedEnquiryResults(ByVal invoiceDetailsId As Integer) As  _
        FetchDomProviderInvoiceDetailNonDeliveryUnitBasedListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderInvoiceDetailNonDeliveryUnitBasedListResult = _
            New FetchDomProviderInvoiceDetailNonDeliveryUnitBasedListResult()
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser

            Try

                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchProviderInvoiceDetailNonDeliveryUnitBased(conn, invoiceDetailsId, result.NonDeliveryUnits)
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

#Region " Dom Provider Invoice Detail Non Delivery Visit Based Enquiry Results "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
      Public Function FetchDomProviderInvoiceDetailNonDeliveryVisitBasedEnquiryResults(ByVal invoiceDetailsId As Integer) As  _
      FetchDomProviderInvoiceDetailNonDeliveryVisitBasedListResult

            Dim msg As ErrorMessage
            Dim result As FetchDomProviderInvoiceDetailNonDeliveryVisitBasedListResult = _
            New FetchDomProviderInvoiceDetailNonDeliveryVisitBasedListResult
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser

            Try

                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchProviderInvoiceDetailNonDeliveryVisitBased(conn, invoiceDetailsId, result.NonDeliveryVisits)
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

#Region " Get Current Provider Invoice "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
      Public Function FetchNonDeliveryCurrentProviderInvoice(ByVal invoiceId As Integer) As NonDeliveryProviderInvoiceResult
            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim result As NonDeliveryProviderInvoiceResult = New NonDeliveryProviderInvoiceResult()

            Try
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchNonDeliveryCurrentProviderInvoice(conn, invoiceId, result.CurrentProforma)
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

#Region " Get Current Provider Invoice Detail"
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
      Public Function FetchNonDeliveryCurrentProviderInvoiceDetail(ByVal invoiceDetailId As Integer) As NonDeliveryProviderInvoiceDetailResult
            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim result As NonDeliveryProviderInvoiceDetailResult = New NonDeliveryProviderInvoiceDetailResult()

            Try
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchNonDeliveryCurrentProviderInvoiceDetail(conn, invoiceDetailId, result.CurrentProformaDetail)
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

    End Class

End Namespace
