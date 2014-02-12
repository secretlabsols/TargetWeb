Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library.ProformaInvoices
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library

Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to manage dom proforma invoices
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/DomProformaInvoices")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class DomProfomaInvoice
        Inherits System.Web.Services.WebService


#Region "Fields"

        ' locals
        Private _ConnectionString As String

        ' constants
        Private Const _ConnectionStringKey As String = "Abacus"
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber
        Private Const _AuditLogTitle As String = "Service Registers"

#End Region

#Region "Methods"

#Region "ChangeDomProformaInvoiceStatus"

        ''' <summary>
        ''' Changes the dom proforma invoice status.
        ''' </summary>
        ''' <param name="pScheduleId">The p schedule id.</param>
        ''' <param name="selectedInvoiceID">The selected invoice ID.</param>
        ''' <param name="mismatch">The mismatch.</param>
        ''' <param name="tolerance">The tolerance.</param>
        ''' <param name="listFilterSUReference">The list filter SU reference.</param>
        ''' <param name="listFilterSUName">Name of the list filter SU.</param>
        ''' <param name="dcrFilter">The dcr filter.</param>
        ''' <param name="statusAwait">The status await.</param>
        ''' <param name="statusVerified">The status verified.</param>
        ''' <param name="ourReference">Our reference.</param>
        ''' <param name="weeks">The weeks.</param>
        ''' <param name="queryType">Type of the query.</param>
        ''' <param name="editStatus">The edit status.</param>
        ''' <param name="newStatus">The new status.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ChangeDomProformaInvoiceStatus(ByVal pScheduleId As Integer, _
                                                       ByVal selectedInvoiceID As Integer, _
                                                       ByVal mismatch As TriState, _
                                                       ByVal tolerance As Decimal, _
                                                       ByVal listFilterSUReference As String, _
                                                       ByVal listFilterSUName As String, _
                                                       ByVal dcrFilter As TriState, _
                                                       ByVal statusAwait As Nullable(Of Boolean), _
                                                       ByVal statusVerified As Nullable(Of Boolean), _
                                                       ByVal ourReference As String, _
                                                       ByVal weeks As Nullable(Of Integer), _
                                                       ByVal queryType As Nullable(Of Integer), _
                                                       ByVal editStatus As Nullable(Of Integer), _
                                                       ByVal newStatus As String) _
                                                       As DomProfomaInvoice_ChangeDomProformaInvoiceStatus

            Dim result As New DomProfomaInvoice_ChangeDomProformaInvoiceStatus()

            Try

                Using connection As SqlConnection = SqlHelper.GetConnection(ConnectionString)

                    Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
                    Dim settings As SystemSettings = SystemSettings.GetCachedSettings(connection.ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                    Dim tmpInvoiceList As New List(Of DomProformaInvoiceResult)()

                    Try

                        result.ErrMsg = _
                        ProformaInvoices.DomProformaInvoiceBL.ChangeDomProformaInvoiceStatus( _
                        connection, _
                        pScheduleId, _
                        selectedInvoiceID, _
                        Nothing, _
                        mismatch, _
                        tolerance, _
                        listFilterSUReference, _
                        listFilterSUName, _
                        dcrFilter, _
                        statusAwait, _
                        statusVerified, _
                        ourReference, _
                        weeks, _
                        queryType, _
                        editStatus, _
                        newStatus, _
                        user.ExternalUsername, _
                        AuditLogging.GetAuditLogTitle("ProformaInvoices.DomProformaInvoiceBL.ChangeDomProformaInvoiceStatus", settings), _
                        result.List)

                        If Not result.ErrMsg.Success Then Return result

                        ' clear the list as the values that are returned from the above
                        ' are complete garbage and cant be trusted!
                        result.List.Clear()

                        ' fetch dom proforma invoice results
                        result.ErrMsg = DomContractBL.FetchProformaInvoiceResults( _
                           connection, _
                           user.ExternalUserID, _
                           1, _
                           10000, _
                           pScheduleId, _
                           selectedInvoiceID, _
                           Nothing, _
                           mismatch, _
                           Utils.ToDecimal(tolerance), _
                           listFilterSUReference, _
                           listFilterSUName, _
                           dcrFilter, _
                           statusAwait, _
                           statusVerified, _
                           New Integer(), _
                           tmpInvoiceList, _
                           ourReference, _
                           weeks, _
                           queryType, _
                           editStatus _
                       )

                        If selectedInvoiceID > 0 Then
                            ' if we have a single invoice id just get that

                            result.List.Add((From tmp In tmpInvoiceList _
                                                Where tmp.InvoiceID = selectedInvoiceID _
                                            Select tmp).FirstOrDefault())

                        Else
                            ' else get all of em based on filter criteria

                            result.List.AddRange(tmpInvoiceList)

                        End If

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

                result.ErrMsg = Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return result

        End Function

#End Region

#Region " ChangeDomProformaInvoiceStatusFromIDs "

        ''' <summary>
        ''' Changes the dom proforma invoice status.
        ''' </summary>
        ''' <param name="pScheduleId">The p schedule id.</param>
        ''' <param name="ids">list of Proforma invoice ids</param>
        ''' <param name="newStatus">The new status.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ChangeDomProformaInvoiceStatusFromIDs(ByVal pScheduleId As Integer, _
                                                              ByVal ids As List(Of Integer), _
                                                              ByVal newStatus As String) As DomProfomaInvoice_ChangeDomProformaInvoiceStatus

            Dim result As New DomProfomaInvoice_ChangeDomProformaInvoiceStatus()

            Try

                Using connection As SqlConnection = SqlHelper.GetConnection(ConnectionString)

                    Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
                    Dim settings As SystemSettings = SystemSettings.GetCachedSettings(connection.ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                    Dim tmpInvoiceList As New List(Of DomProformaInvoiceResult)()

                    Try

                        'Get the ProformaInvoices
                        result.ErrMsg = ProformaInvoices.DomProformaInvoiceBL.FetchProformaInvoiceByIDS(connection, _
                                                                                                        Nothing, _
                                                                                                        ids, _
                                                                                                        tmpInvoiceList)
                        If Not result.ErrMsg.Success Then Return result


                        'Update the status
                        result.ErrMsg = ProformaInvoices.DomProformaInvoiceBL.ChangeDomProformaInvoiceStatus(connection, _
                                                                                                             pScheduleId, _
                                                                                                             tmpInvoiceList, _
                                                                                                             newStatus, _
                                                                                                             user.ExternalUsername, _
                                                                                                             AuditLogging.GetAuditLogTitle("ProformaInvoices.DomProformaInvoiceBL.ChangeDomProformaInvoiceStatus", settings), _
                                                                                                             user.ExternalUserID)
                        If Not result.ErrMsg.Success Then Return result

                        ' clear the list we will re-fetch the values back from the database
                        result.List.Clear()

                        ' Re-fetch dom proforma invoice results
                        result.ErrMsg = ProformaInvoices.DomProformaInvoiceBL.FetchProformaInvoiceByIDS(connection, _
                                                                                                        Nothing, _
                                                                                                        ids, _
                                                                                                        tmpInvoiceList)
                        If Not result.ErrMsg.Success Then Return result


                        result.List.AddRange(tmpInvoiceList)


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

                result.ErrMsg = Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return result

        End Function

#End Region

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
                                                           ByVal tag As String) _
                                                           As StringListResult

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

                msg = Target.Abacus.Library.DomProviderInvoice.DetailLine.FetchRateList(conn, domContractID, clientID, weekEnding, rateCategoryID, formattedRates)
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

        ''' <summary>
        ''' Gets the dom proforma invoice non delivery unit based.
        ''' </summary>
        ''' <param name="domProformaInvoiceId">The dom proforma invoice id.</param>
        ''' <param name="domProformaInvoiceDetailId">The dom proforma invoice detail id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="GetDomProformaInvoiceNonDeliveryUnitBased."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetDomProformaInvoiceNonDeliveryUnitBased(ByVal domProformaInvoiceId As Integer, ByVal domProformaInvoiceDetailId As Integer) As DomProfomaInvoice_GetDomProformaInvoiceNonDeliveryUnitBased

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New DomProfomaInvoice_GetDomProformaInvoiceNonDeliveryUnitBased()
            Dim currentUser As WebSecurityUser = Nothing

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the detail lines
                msg = DomProformaInvoiceBL.GetDomProformaInvoiceNonDeliveryUnitBased(connection, domProformaInvoiceId, domProformaInvoiceDetailId, result.List)
                With result
                    .ErrMsg = msg
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

        ''' <summary>
        ''' Gets the dom proforma invoice non delivery visit based.
        ''' </summary>
        ''' <param name="domProformaInvoiceId">The dom proforma invoice id.</param>
        ''' <param name="domProformaInvoiceDetailId">The dom proforma invoice detail id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="GetDomProformaInvoiceNonDeliveryVisitBased."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetDomProformaInvoiceNonDeliveryVisitBased(ByVal domProformaInvoiceId As Integer, ByVal domProformaInvoiceDetailId As Integer) As DomProfomaInvoice_GetDomProformaInvoiceNonDeliveryVisitBased

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New DomProfomaInvoice_GetDomProformaInvoiceNonDeliveryVisitBased()
            Dim currentUser As WebSecurityUser = Nothing

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the detail lines
                msg = DomProformaInvoiceBL.GetDomProformaInvoiceNonDeliveryVisitBased(connection, domProformaInvoiceId, domProformaInvoiceDetailId, result.List)
                With result
                    .ErrMsg = msg
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

        ''' <summary>
        ''' Gets the dom proforma invoice contract service outcomes.
        ''' </summary>
        ''' <param name="domContractId">The dom contract id.</param>
        ''' <param name="targetDate">The target date.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="GetDomProformaInvoiceContractServiceOutcomes."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetDomProformaInvoiceContractServiceOutcomes(ByVal domContractId As Integer, ByVal targetDate As DateTime) As DomProfomaInvoice_GetDomProformaInvoiceContractServiceOutcomes

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New DomProfomaInvoice_GetDomProformaInvoiceContractServiceOutcomes()
            Dim currentUser As WebSecurityUser = Nothing

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the detail lines
                msg = DomProformaInvoiceBL.GetDomProformaInvoiceContractServiceOutcomes(connection, domContractId, targetDate, result.List)
                With result
                    .ErrMsg = msg
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

        ''' <summary>
        ''' Gets the dom proforma invoice contract visit codes.
        ''' </summary>
        ''' <param name="domContractId">The DOM contract id.</param>
        ''' <param name="targetDate">The target date.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="GetDomProformaInvoiceContractVisitCodes."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetDomProformaInvoiceContractVisitCodes(ByVal domContractId As Integer, ByVal targetDate As DateTime) As DomProfomaInvoice_GetDomProformaInvoiceContractVisitCodes

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New DomProfomaInvoice_GetDomProformaInvoiceContractVisitCodes()
            Dim currentUser As WebSecurityUser = Nothing

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the detail lines
                msg = DomProformaInvoiceBL.GetDomProformaInvoiceContractVisitCodes(connection, domContractId, targetDate, result.List)
                With result
                    .ErrMsg = msg
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

        ''' <summary>
        ''' Gets the dom proforma invoice details by dom profoma invoice.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Gets Dom Proforma Invoices By Payment Schedule ID."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetDomProformaInvoicesByPaymentSchedule(ByVal id As Integer, ByVal domProformaInvoiceId As Integer) As DomProfomaInvoice_GetDomProformaInvoice

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New DomProfomaInvoice_GetDomProformaInvoice()
            Dim currentUser As WebSecurityUser = Nothing

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the detail lines
                msg = DomProformaInvoiceBL.GetDomProformaInvoicesByPaymentSchedule(connection, id, domProformaInvoiceId, result.List)
                With result
                    .ErrMsg = msg
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

        ''' <summary>
        ''' Gets the dom proforma invoice details by dom profoma invoice.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Gets Dom Proforma Invoices Details."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetDomProformaInvoiceDetailsByDomProfomaInvoice(ByVal id As Integer) As DomProfomaInvoice_GetDomProformaInvoiceDetailsByDomProfomaInvoice

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New DomProfomaInvoice_GetDomProformaInvoiceDetailsByDomProfomaInvoice()
            Dim currentUser As WebSecurityUser = Nothing
            Dim settings As SystemSettings

            Try

                Dim domProformaInv As DataClasses.DomProformaInvoice = Nothing

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get settings
                settings = SystemSettings.GetCachedSettings(ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

                ' get the detail lines
                With result
                    .ErrMsg = DomProformaInvoiceBL.GetDomProformaInvoiceDetailsByDomProformaInvoice(connection, id, False, result.List, result.VoidPayment)
                    If Not .ErrMsg.Success Then Return result
                End With

                ' get the proforma invoice
                domProformaInv = New DataClasses.DomProformaInvoice( _
                    conn:=connection, _
                    auditUserName:=currentUser.ExternalUsername, _
                    auditLogTitle:=AuditLogging.GetAuditLogTitle("", settings) _
                )
                msg = domProformaInv.Fetch(id)
                With result
                    .ErrMsg = domProformaInv.Fetch(id)
                    If Not .ErrMsg.Success Then Return result
                End With

                ' update the status if required
                If domProformaInv.EditStatus = DomProformaInvoiceBL.DomProformaInvoiceEditStatus.Unread Then
                    With result
                        .ErrMsg = DomProformaInvoiceBL.SetDomProformaInvoiceEditStatus( _
                        connection, _
                        domProformaInv, _
                        DomProformaInvoiceBL.DomProformaInvoiceEditStatus.Read, _
                        currentUser.ExternalUsername, _
                        AuditLogging.GetAuditLogTitle("DomProformaInvoiceBL.SetDomProformaInvoiceEditStatus", settings) _
                        )
                        If Not .ErrMsg.Success Then Return result
                    End With
                End If

                ' set the error message to successful
                result.ErrMsg = New ErrorMessage()
                result.ErrMsg.Success = True

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


        ''' <summary>
        ''' Gets the dom proforma invoice detail for new.
        ''' </summary>
        ''' <param name="domProformaInvoiceID">The dom proforma invoice ID.</param>
        ''' <param name="domRateCategoryID">The dom rate category ID.</param>
        ''' <param name="weekEndingDate">The week ending date.</param>
        ''' <param name="unitCost">The unit cost.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Gets Dom Proforma Invoices Details For New Records."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetDomProformaInvoiceDetailForNew(ByVal domProformaInvoiceID As Integer, ByVal domRateCategoryID As Integer, ByVal weekEndingDate As DateTime, ByVal unitCost As Decimal) As DomProfomaInvoice_GetDomProformaInvoiceDetailForNew

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New DomProfomaInvoice_GetDomProformaInvoiceDetailForNew()
            Dim currentUser As WebSecurityUser = Nothing

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the detail lines
                msg = DomProformaInvoiceBL.GetDomProformaInvoiceDetailForNew(connection, domProformaInvoiceID, domRateCategoryID, weekEndingDate, unitCost, result.Item)
                With result
                    .ErrMsg = msg
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

        ''' <summary>
        ''' Saves the dom proforma invoice details.
        ''' </summary>
        ''' <param name="items">The items.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Saves Dom Proforma Invoice Details for a single Dom Proforma Invoice."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function SaveDomProformaInvoiceDetails(ByVal items As List(Of UpdatableDomProformaInvoiceDetail)) As DomProfomaInvoice_SaveDomProformaInvoiceDetails

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New DomProfomaInvoice_SaveDomProformaInvoiceDetails()
            Dim currentUser As WebSecurityUser = Nothing
            Dim tmpProformaInvoice As DataClasses.DomProformaInvoice = Nothing
            Dim tmpProformaInvoices As List(Of ViewableDomProformaInvoice) = Nothing

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                With result
                    .ErrMsg = SecurityBL.ValidateLogin()
                    If Not .ErrMsg.Success Then
                        Return result
                    End If
                End With

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)
                Dim settings As SystemSettings = SystemSettings.GetCachedSettings(connection.ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                ' get the detail lines
                With result
                    .ErrMsg = DomProformaInvoiceBL.SaveDomProformaInvoiceDetails( _
                    connection, _
                    items, _
                    currentUser.ExternalUsername, _
                    AuditLogging.GetAuditLogTitle("DomProfomaInvoice.SaveDomProformaInvoiceDetails", settings), _
                    tmpProformaInvoice, _
                    currentUser.ExternalUserID)
                    If Not .ErrMsg.Success Then
                        Return result
                    End If
                End With

                ' get the saved dom proforma invoice 
                With result
                    .ErrMsg = DomProformaInvoiceBL.GetDomProformaInvoicesByPaymentSchedule(connection, tmpProformaInvoice.PaymentScheduleID, tmpProformaInvoice.ID, tmpProformaInvoices)
                    If Not .ErrMsg.Success Then
                        Return result
                    End If
                End With

                ' get the first invoice...should only be one
                result.Item = tmpProformaInvoices(0)

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

        <WebMethod(EnableSession:=True, Description:="Recalculate Dom proforma Invoice Detail."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function RecalculateDomProformaInvoice(ByVal proformaInvoiceId As Integer) As DomProfomaInvoice_RecalculateProformaInvoiceDetails

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New DomProfomaInvoice_RecalculateProformaInvoiceDetails
            Dim currentUser As WebSecurityUser = Nothing

            Try

                currentUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                With result
                    .ErrMsg = SecurityBL.ValidateLogin()
                    If Not .ErrMsg.Success Then
                        Return result
                    End If
                End With

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)
                Dim settings As SystemSettings = SystemSettings.GetCachedSettings(connection.ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                Dim dpIv As DataClasses.DomProformaInvoice = New DataClasses.DomProformaInvoice(currentUser.ExternalUsername, _
                                                                                 AuditLogging.GetAuditLogTitle("DomProfomaInvoice.RecalculateDomProformaInvoice", settings))
                Dim dpIvDetailCollection As DataClasses.Collections.DomProformaInvoiceDetailCollection = _
                New DataClasses.Collections.DomProformaInvoiceDetailCollection()
                ' Recalculate proforma invoice lines
                With result
                    .ErrMsg = DomProformaInvoiceBL.RecalculateDomProformaInvoice(connection, _
                                                                                 proformaInvoiceId, _
                                                                                 Nothing, _
                                                                                 Nothing, _
                                                                                 currentUser.ExternalUsername, _
                                                                                 AuditLogging.GetAuditLogTitle("DomProfomaInvoice.RecalculateDomProformaInvoice", settings), _
                                                                                 dpIv, _
                                                                                 dpIvDetailCollection, _
                                                                                 currentUser.ExternalUserID)

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

#Region "Classes"

    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.GetDomProformaInvoiceDetailsByDomProfomaInvoice
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_GetDomProformaInvoiceDetailsByDomProfomaInvoice

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _List As New List(Of ViewableDomProformaInvoiceDetail)()
        Private _VoidPayment As Boolean
#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the list.
        ''' </summary>
        ''' <value>The list.</value>
        Public Property List() As List(Of ViewableDomProformaInvoiceDetail)
            Get
                Return _List
            End Get
            Set(ByVal value As List(Of ViewableDomProformaInvoiceDetail))
                _List = value
            End Set
        End Property


        Public Property VoidPayment() As Boolean
            Get
                Return _VoidPayment
            End Get
            Set(ByVal value As Boolean)
                _VoidPayment = value
            End Set
        End Property

#End Region

    End Class

    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.GetDomProformaInvoiceDetailsByDomProfomaInvoice
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_GetDomProformaInvoice

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _List As New List(Of ViewableDomProformaInvoice)()

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the item.
        ''' </summary>
        ''' <value>The item.</value>
        Public Property List() As List(Of ViewableDomProformaInvoice)
            Get
                Return _List
            End Get
            Set(ByVal value As List(Of ViewableDomProformaInvoice))
                _List = value
            End Set
        End Property

#End Region

    End Class

    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.GetDomProformaInvoiceDetailForNew
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_GetDomProformaInvoiceDetailForNew

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _Item As ViewableDomProformaInvoiceDetail = Nothing

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the item.
        ''' </summary>
        ''' <value>The item.</value>
        Public Property Item() As ViewableDomProformaInvoiceDetail
            Get
                Return _Item
            End Get
            Set(ByVal value As ViewableDomProformaInvoiceDetail)
                _Item = value
            End Set
        End Property

#End Region

    End Class

    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.GetDomProformaInvoiceNonDeliveryUnitBased
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_GetDomProformaInvoiceNonDeliveryUnitBased

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _List As List(Of ViewableDomProformaInvoiceNonDeliveryUnitBased)

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the list.
        ''' </summary>
        ''' <value>The list.</value>
        Public Property List() As List(Of ViewableDomProformaInvoiceNonDeliveryUnitBased)
            Get
                Return _List
            End Get
            Set(ByVal value As List(Of ViewableDomProformaInvoiceNonDeliveryUnitBased))
                _List = value
            End Set
        End Property

#End Region

    End Class

    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.GetDomProformaInvoiceNonDeliveryVisitBased
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_GetDomProformaInvoiceNonDeliveryVisitBased

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _List As List(Of ViewableDomProformaInvoiceNonDeliveryVisitBased)

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the list.
        ''' </summary>
        ''' <value>The list.</value>
        Public Property List() As List(Of ViewableDomProformaInvoiceNonDeliveryVisitBased)
            Get
                Return _List
            End Get
            Set(ByVal value As List(Of ViewableDomProformaInvoiceNonDeliveryVisitBased))
                _List = value
            End Set
        End Property

#End Region

    End Class

    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.GetDomProformaInvoiceContractVisitCodes
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_GetDomProformaInvoiceContractVisitCodes

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _List As List(Of ViewableDomVisitCode)

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the list.
        ''' </summary>
        ''' <value>The list.</value>
        Public Property List() As List(Of ViewableDomVisitCode)
            Get
                Return _List
            End Get
            Set(ByVal value As List(Of ViewableDomVisitCode))
                _List = value
            End Set
        End Property

#End Region

    End Class

    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.GetDomProformaInvoiceContractServiceOutcomes
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_GetDomProformaInvoiceContractServiceOutcomes

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _List As List(Of ProformaInvoices.ViewableServiceOutcome)

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the list.
        ''' </summary>
        ''' <value>The list.</value>
        Public Property List() As List(Of ProformaInvoices.ViewableServiceOutcome)
            Get
                Return _List
            End Get
            Set(ByVal value As List(Of ProformaInvoices.ViewableServiceOutcome))
                _List = value
            End Set
        End Property

#End Region

    End Class

    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.SaveDomProformaInvoiceDetails
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_SaveDomProformaInvoiceDetails

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _Item As ViewableDomProformaInvoice = Nothing

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the item.
        ''' </summary>
        ''' <value>The item.</value>
        Public Property Item() As ViewableDomProformaInvoice
            Get
                Return _Item
            End Get
            Set(ByVal value As ViewableDomProformaInvoice)
                _Item = value
            End Set
        End Property

#End Region

    End Class

    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.ChangeDomProformaInvoiceStatus
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12102 Created 22/08/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_ChangeDomProformaInvoiceStatus

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _List As New List(Of DomProformaInvoiceResult)()

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the list.
        ''' </summary>
        ''' <value>The list.</value>
        Public Property List() As List(Of DomProformaInvoiceResult)
            Get
                Return _List
            End Get
            Set(ByVal value As List(Of DomProformaInvoiceResult))
                _List = value
            End Set
        End Property

#End Region

    End Class

    Public Class StringListResult
        Public ErrMsg As ErrorMessage
        Public Values As List(Of String)
        Public Tag As String
    End Class



    ''' <summary>
    ''' Class used to represent the result of a call to DomProfomaInvoice.RecalculateDomProformaInvoice
    ''' </summary>
    ''' <history>
    ''' Waqas  D11818 Created 12/10/2011 Created
    ''' </history>
    Public Class DomProfomaInvoice_RecalculateProformaInvoiceDetails

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _List As New List(Of DomProformaInvoiceResult)()
#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property


        ''' <summary>
        ''' Gets or sets the list.
        ''' </summary>
        ''' <value>The list.</value>
        Public Property List() As List(Of DomProformaInvoiceResult)
            Get
                Return _List
            End Get
            Set(ByVal value As List(Of DomProformaInvoiceResult))
                _List = value
            End Set
        End Property

#End Region

    End Class



#End Region

End Namespace