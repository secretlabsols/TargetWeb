Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.RequestPayments
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports System.Web.Script.Serialization


Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to retrieve domiciliary contract information.
    ''' </summary>
    ''' <remarks></remarks>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/RequestPayments")> _
 <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
 <ToolboxItem(False)> _
 Public Class RequestPayments
        Inherits System.Web.Services.WebService

#Region " GetContractList "

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="payUpToDate">Payment Up To Date</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetContractList(ByVal payUpToDate As Date) As FetchContractListResult

            Dim msg As ErrorMessage
            Dim result As FetchContractListResult = New FetchContractListResult
            Dim conn As SqlConnection = Nothing
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

                ' get the list of contracts
                msg = RequestPaymentBL.GetContractList(conn, payUpToDate, user.ExternalUserID, user.ID, result.ContractList)
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

#Region " CreatePaymentRequest "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="payUpToDate">Payment Up To Date</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CreatePaymentRequest(ByVal payUpToDate As Date, ByVal paymentScheduleId As Integer) As CreatePaymentRequestResult

            Dim msg As ErrorMessage
            Dim result As CreatePaymentRequestResult = New CreatePaymentRequestResult
            Dim conn As SqlConnection = Nothing
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

                ' get the list of contracts
                msg = RequestPaymentBL.CreatePaymentRequest(conn, payUpToDate, user.ID, paymentScheduleId, result.PaymentRequestID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                ' get the visibility of retract provider if selectedInvoiceId  > 0
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

#Region " CreatePaymentRequest_DomContract "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="paymentRequestID">ID of the payment request</param>
        ''' <param name="domContractID">ID of the dom Contract</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CreatePaymentRequest_DomContract(ByVal paymentRequestID As Integer, _
                                                         ByVal domContractID As Integer) As CreatePaymentRequest_DomContractResult

            Dim result As CreatePaymentRequest_DomContractResult = New CreatePaymentRequest_DomContractResult
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)


            Try

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                result.ErrMsg = RequestPaymentBL.CreatePaymentRequest_DomContract(conn, paymentRequestID, domContractID)


            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function



#End Region

#Region " CreateJob_ProcessPaymentRequest "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="paymentRequestID">ID of the payment request</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CreateJob_ProcessPaymentRequest(ByVal paymentRequestID As Integer) As CreateJob_ProcessPaymentRequestResult

            Dim result As CreateJob_ProcessPaymentRequestResult = New CreateJob_ProcessPaymentRequestResult
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
            Dim user As WebSecurityUser
            Dim msg As ErrorMessage
            Dim jobID As Integer

            Try

                ' get user credentials
                user = SecurityBL.GetCurrentUser()
                'check if logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of contracts
                result.ErrMsg = RequestPaymentBL.CreateJob_ProcessPaymentRequest(conn, _
                                                                                 paymentRequestID, _
                                                                                 user.ExternalUsername, _
                                                                                 Date.Now, _
                                                                                 "Payment Request", _
                                                                                 user.ExternalUserID, _
                                                                                 user.Email, _
                                                                                 jobID)
                If Not result.ErrMsg.Success Then Return result

                ' get the email address to use
                msg = RequestPaymentBL.GetPaymentRequestEmailAddress(conn, paymentRequestID, result.EmailAddress)
                If Not result.ErrMsg.Success Then Return result


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