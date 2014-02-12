Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data.SqlClient
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.ServiceOrder
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security


Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to retrieve generic Service Order information.
    ''' </summary>
    ''' <remarks></remarks>
    '''  <history>
    '''  [PaulW]  13/10/2011  Created D11945A
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/GenericServiceOrder")> _
    Public Class GenericServiceOrder
        Inherits System.Web.Services.WebService

#Region " FetchServiceOrderList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of Generic Service Orders.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedEstablishmentID overrides the specified page parameter such that
        '''     the page which contain the requested provider is displayed. 
        ''' </remarks>
        ''' <history>
        ''' 	[PaulW]	13/10/2011	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceOrderList(ByVal establishmentID As Integer, _
                                              ByVal domContractID As Integer, _
                                              ByVal dateFrom As Date, _
                                              ByVal dateTo As Date, _
                                              ByVal movement As Integer) As FetchServiceOrderListResult

            Dim msg As ErrorMessage
            Dim result As FetchServiceOrderListResult = New FetchServiceOrderListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ExternalUserID


                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of service Orders
                msg = ServiceOrderBL.FetchGenericServiceOrders_Extranet(conn, _
                                                                        userID, _
                                                                        establishmentID, _
                                                                        domContractID, _
                                                                        dateFrom, _
                                                                        dateTo, _
                                                                        movement, _
                                                                        result.dsoList)
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

#Region " FetchServiceOrder "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a Service Order record
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[PaulW]	13/10/2011	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceOrder(ByVal GenericServiceOrderID As Integer) As FetchServiceOrderResult

            Dim msg As ErrorMessage
            Dim result As FetchServiceOrderResult = New FetchServiceOrderResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ExternalUserID


                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the service Order
                msg = ServiceOrderBL.FetchGenericServiceOrder_Extranet(conn, _
                                                                       userID, _
                                                                        GenericServiceOrderID, _
                                                                        result.dso)
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

#Region " FetchServiceOrderDetailList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of Generic Service Order details.
        ''' </summary>
        ''' <returns></returns>
        ''' <history>
        ''' 	[PaulW]	18/10/2011	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceOrderDetailList(ByVal genericServiceOrderID As Integer, weekEndingDate As DateTime) As FetchServiceOrderDetailListResult

            Dim msg As ErrorMessage
            Dim result As FetchServiceOrderDetailListResult = New FetchServiceOrderDetailListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ExternalUserID


                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of detail records
                msg = ServiceOrderBL.FetchGenericServiceOrderDetail_Extranet(conn, _
                                                                        genericServiceOrderID, _
                                                                        weekEndingDate, _
                                                                        result.dsodList)
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

#Region " FetchServiceOrderSuspensionList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of Generic Service Order details.
        ''' </summary>
        ''' <returns></returns>
        ''' <history>
        ''' 	[PaulW]	18/10/2011	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceOrderSuspensionList(ByVal serviceOrderID As Integer) As FetchServiceOrderSuspensionListResult

            Dim msg As ErrorMessage
            Dim result As FetchServiceOrderSuspensionListResult = New FetchServiceOrderSuspensionListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ExternalUserID


                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of suspensions
                msg = ServiceOrderBL.FetchServiceOrderSuspensions(conn, _
                                                                    serviceOrderID, _
                                                                    result.suspensionList)
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

#Region " FetchServiceOrderCostList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of Service Order Costs.
        ''' </summary>
        ''' <returns></returns>
        ''' <history>
        ''' 	[PaulW]	18/10/2011	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceOrderCostList(ByVal serviceOrderID As Integer, weekEndingDate As DateTime) As FetchServiceOrderCostListResult

            Dim msg As ErrorMessage
            Dim result As FetchServiceOrderCostListResult = New FetchServiceOrderCostListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ExternalUserID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of costs
                Dim service As ServiceOrderBL = New ServiceOrderBL()
                msg = service.FetchServiceOrderCosts(conn, _
                                                            serviceOrderID, _
                                                            weekEndingDate, _
                                                            result.costList)
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

#Region " MarkServiceOrderAsRead "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Marks a Service Order record as being read
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[PaulW]	24/10/2011	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function MarkServiceOrderAsRead(ByVal GenericServiceOrderID As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            'Dim result As FetchServiceOrderResult = New FetchServiceOrderResult
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    Return msg
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the current user
                user = SecurityBL.GetCurrentUser()
                If Not SecurityBL.IsCouncilUser(conn, settings, user.ExternalUserID) Then
                    ' get the service Order
                    msg = ServiceOrderBL.MarkServiceOrderAsRead(conn, _
                                                                GenericServiceOrderID)
                    If Not msg.Success Then
                        Return msg
                    End If
                Else
                    msg = New ErrorMessage
                    msg.Success = False
                    Return msg
                End If


                msg = New ErrorMessage
                msg.Success = True


            Catch ex As Exception
                msg = New ErrorMessage
                msg.Success = False
                msg.Message = ex.Message
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg

        End Function

#End Region

#Region " GetWeekEnding Date"

        ''' <summary>
        ''' Returns a Week Ending Date
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetWeekendingDate(referenceDate As DateTime) As GetWeekendingDateResult

            Dim msg As New ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim result As GetWeekendingDateResult = New GetWeekendingDateResult()
            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' see if the user is allowed to perform the operation
                Dim WeDate As DateTime = DomContractBL.GetWeekEndingDate(conn, Nothing, referenceDate)
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