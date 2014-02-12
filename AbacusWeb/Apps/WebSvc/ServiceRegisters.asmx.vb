Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library.ServiceRegisters
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to manage service registers
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12140 Created 07/07/2011 Created
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/ServiceRegisters")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class ServiceRegisters
        Inherits System.Web.Services.WebService

#Region "Fields"

        ' locals
        Private _ConnectionString As String

        ' constants
        Private Const _ConnectionStringKey As String = "Abacus"
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber
        Private Const _AuditLogTitle As String = "Service Registers"

#End Region

#Region "Functions"

        ''' <summary>
        ''' Clears the register attendance by day.
        ''' </summary>
        ''' <param name="registerId">The register id.</param>
        ''' <param name="clientId">The client id.</param>
        ''' <param name="dayId">The day id.</param>
        ''' <param name="serviceOutcomeId">The service outcome id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Clears Attended Service for a Service Register Day."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ClearRegisterAttendanceByDay(ByVal registerId As Integer, ByVal clientId As Integer, ByVal dayId As Integer, ByVal serviceOutcomeId As Integer) As ServiceRegisters_ClearRegisterAttendanceByDayResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New ServiceRegisters_ClearRegisterAttendanceByDayResult()
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

                ' update the register
                msg = ServiceRegisterBL.ClearRegisterAttendanceByDay(connection, registerId, clientId, dayId, serviceOutcomeId, currentUser.ExternalUsername, _AuditLogTitle)
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
        ''' Creates the new register.
        ''' </summary>
        ''' <param name="providerId">The provider id.</param>
        ''' <param name="contractId">The contract id.</param>
        ''' <param name="weekEndingDate">The week ending date.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Create a New Service Register."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CreateNewRegister(ByVal providerId As Integer, ByVal contractId As Integer, ByVal weekEndingDate As DateTime) As ServiceRegisters_CreateNewRegisterResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim reg As Target.Abacus.Library.DataClasses.Register = Nothing
            Dim result As New ServiceRegisters_CreateNewRegisterResult()

            Try

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' update the register
                msg = ServiceRegisterBL.CreateNewRegister(connection, providerId, contractId, weekEndingDate, _AuditLogTitle, reg)
                With result
                    .ErrMsg = msg
                    .RegisterID = reg.ID
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
        ''' Delete a Service Register
        ''' </summary>
        ''' <param name="id">The id of the Service Register to delete.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True, Description:="Delete a Service Register."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DeleteServiceRegister(ByVal id As Integer) As ServiceRegisters_DeleteServiceRegisterResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New ServiceRegisters_DeleteServiceRegisterResult()

            Try

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' create and open a db connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' delete the register
                msg = ServiceRegisterBL.DeleteRegister(connection, id)
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
        ''' Gets Planned or Attended Service for a Client on a Service Register.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <param name="clientId">The client id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Gets Planned or Attended Service for a Client on a Service Register."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function GetRegisterPlannedOrAttendedService(ByVal id As Integer, ByVal clientId As Integer, ByVal dayId As Integer, ByVal isPlanned As Nullable(Of Boolean), ByVal isAttended As Nullable(Of Boolean)) As ServiceRegisters_GetRegisterPlannedOrAttendedServiceResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New ServiceRegisters_GetRegisterPlannedOrAttendedServiceResult()
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

                ' delete the register
                msg = ServiceRegisterBL.GetRegisterPlannedOrAttendedService(connection, id, clientId, dayId, currentUser.ExternalUsername, _AuditLogTitle, result.List)
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
        ''' Primes the Service Register for a Client.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <param name="clientId">The client id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Primes the Service Register for a Client."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function PrimeServiceRegister(ByVal id As Integer, ByVal clientId As Integer) As ServiceRegisters_PrimeServiceRegisterResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New ServiceRegisters_PrimeServiceRegisterResult()
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

                ' update the register
                msg = ServiceRegisterBL.PrimeRegister(connection, id, clientId, New Boolean())
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
        ''' Submits the service register.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <param name="status">The status.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Submits the Service Register"), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function SubmitServiceRegister(ByVal id As Integer, ByVal status As ServiceRegisterBL.RegisterStatus) As ServiceRegisters_SubmitServiceRegisterResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New ServiceRegisters_SubmitServiceRegisterResult()
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

                ' update the register
                msg = ServiceRegisterBL.SubmitServiceRegister(connection, id, status, _AuditLogTitle, currentUser.ExternalUsername)
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
        ''' Updates Planned or Attended Service for a Client on a Service Register.
        ''' </summary>
        ''' <param name="id">The id.</param>
        ''' <param name="clientId">The client id.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Updates Planned or Attended Service for a Client on a Service Register."), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function UpdateRegisterPlannedOrAttendedService(ByVal id As Integer, ByVal clientId As Integer, ByVal changes As List(Of UpdatablePlannedOrAttendedService)) As ServiceRegisters_UpdateRegisterPlannedOrAttendedServiceResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New ServiceRegisters_UpdateRegisterPlannedOrAttendedServiceResult()
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

                ' update the register
                msg = ServiceRegisterBL.UpdateRegisterPlannedOrAttendedService(connection, id, clientId, _AuditLogTitle, currentUser.ExternalUsername, changes)
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