Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.PaymentSchedules
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.WebServices.Responses


Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to retrieve domiciliary contract information.
    ''' </summary>
    ''' <remarks></remarks>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/PaymentSchedule")> _
 <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
 <ToolboxItem(False)> _
 Public Class PaymentSchedule
        Inherits System.Web.Services.WebService

#Region "Fields"

        ' locals
        Private _ConnectionString As String

        ' constants
        Private Const _ConnectionStringKey As String = "Abacus"
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber

#End Region

#Region "Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="page">requested page</param>
        ''' <param name="seletedPaymentScheduleID">selected payment schedule id</param>
        ''' <param name="providerID">provider id / establishment id</param>
        ''' <param name="contractID">contract id</param>
        ''' <param name="reference">payment schedule reference number</param>
        ''' <param name="periodFrom">period from </param>
        ''' <param name="periodTo">period to </param>
        ''' <param name="visitBasedYes">visit base</param>
        ''' <param name="visitBasedNo">visit base</param>
        ''' <param name="pfInvoiceNo">having no proforma invoices</param>
        ''' <param name="pfInvoiceAwait">Having Pro forma Invoices that are 'Awaiting Verification'</param>
        ''' <param name="pfInvoiceVerified">Having 'verified' Pro forma Invoices</param>
        ''' <param name="invUnpaid">Having Unpaid Invoices</param>
        ''' <param name="invSuspend">Having Suspended Invoices </param>
        ''' <param name="invAuthorised">Having Authorised Invoices</param>
        ''' <param name="invPaid">Having Paid Invoices</param>
        ''' <param name="varAwait">Having Visit Amendment Requests that are 'Awaiting Verification' </param>
        ''' <param name="varVerified">Having 'verified' Visit Amendment Requests </param>
        ''' <param name="varDeclined">Having 'declined' Visit Amendment Requests</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPaymenstScheduleEnquiryResults( _
                ByVal page As Integer, _
                ByVal seletedPaymentScheduleID As Integer, _
                ByVal providerID As Integer, _
                ByVal contractID As Integer, _
                ByVal reference As String, _
                ByVal periodFrom As Date, _
                ByVal periodTo As Date, _
                ByVal visitBasedYes As Boolean, _
                ByVal visitBasedNo As Boolean, _
                ByVal pfInvoiceNo As Boolean, _
                ByVal pfInvoiceAwait As Boolean, _
                ByVal pfInvoiceVerified As Boolean, _
                ByVal invUnpaid As Boolean, _
                ByVal invSuspend As Boolean, _
                ByVal invAuthorised As Boolean, _
                ByVal invPaid As Boolean, _
                ByVal varAwait As Boolean, _
                ByVal varVerified As Boolean, _
                ByVal varDeclined As Boolean _
                ) As FetchPaymenstScheduleEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchPaymenstScheduleEnquiryListResult = New FetchPaymenstScheduleEnquiryListResult
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

                ' get the list of invoices
                msg = DomContractBL.FetchPaymenstScheduleEnquiryResults( _
                        conn, _
                        user.ExternalUserID, _
                        page, _
                        pageSize, _
                        seletedPaymentScheduleID, _
                        providerID, _
                        contractID, _
                        reference, _
                        periodFrom, _
                        periodTo, _
                        visitBasedYes, _
                        visitBasedNo, _
                        pfInvoiceNo, _
                        pfInvoiceAwait, _
                        pfInvoiceVerified, _
                        invUnpaid, _
                        invSuspend, _
                        invAuthorised, _
                        invPaid, _
                        varAwait, _
                        varVerified, _
                        varDeclined, _
                        totalRecords, _
                        result.PaymentSchedules)
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
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchPaymentScheduleList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
                Public Function FetchServiceFilePaymentScheduleList(ByVal page As Integer, _
                        ByVal serviceDeliveryfileId As Integer _
                        ) As FetchPaymenstScheduleEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchPaymenstScheduleEnquiryListResult = New FetchPaymenstScheduleEnquiryListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 1000
            Dim totalRecords As Integer
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

                ' get the list of invoices
                msg = DomContractBL.FetchServiceFilePaymentScheduleList( _
                        conn, _
                        serviceDeliveryfileId, _
                        result.PaymentSchedules)
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
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchServiceFilePaymentScheduleList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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
        ''' Gets the outstanding service users.
        ''' </summary>
        ''' <param name="id">The payment schedule id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="GetOutstandingServiceUsers."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetOutstandingServiceUsers(ByVal id As Integer) As WebServiceReponseWithItems(Of ViewableOutstandingServiceUser)

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New WebServiceReponseWithItems(Of ViewableOutstandingServiceUser)
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
                    .ErrMsg = PaymentScheduleBL.GetOutstandingServiceUsers(connection, id, result.Items)
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

        <WebMethod(EnableSession:=True, Description:="IsVisitBased."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function IsVisitBased(ByVal providerId As Integer, _
                                     ByVal contractId As Integer, _
                                     ByVal dateFrom As Date, _
                                     ByVal dateTo As Date) As PaymentScheduleReturnObjects.IsVisitBased

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim currentUser As WebSecurityUser = Nothing
            Dim result As PaymentScheduleReturnObjects.IsVisitBased = New PaymentScheduleReturnObjects.IsVisitBased()
            Try

                currentUser = SecurityBL.GetCurrentUser()

                connection = SqlHelper.GetConnection(ConnectionString)

                With result
                    .msg = PaymentScheduleBL.IsVisitBased(connection, providerId, contractId, dateFrom, result)
                End With

                If Not result.msg.Success Then Return result

            Catch ex As Exception

            Finally
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

#Region " Fetch Current PaymentSchedule "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
       Public Function FetchPaymenstSchedule(ByVal psId As Integer _
               ) As FetchPaymenstScheduleEnquiryResult


            Dim msg As ErrorMessage
            Dim result As FetchPaymenstScheduleEnquiryResult = New FetchPaymenstScheduleEnquiryResult
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

                msg = DomContractBL.FetchPaymenstSchedule(conn, psId:=psId, pschedule:=result.PaymentSchedule)
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