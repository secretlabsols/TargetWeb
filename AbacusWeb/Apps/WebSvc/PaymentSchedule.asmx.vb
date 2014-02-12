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
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/PaymentSchedule")> _
 <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
 <ToolboxItem(False)> _
 Public Class PaymentSchedule
        Inherits System.Web.Services.WebService

#Region " Fetch Current PaymentSchedule "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
       Public Function FetchPaymenstSchedule(ByVal psId As Integer _
               ) As NonDelivery.FetchPaymenstScheduleEnquiryResult


            Dim msg As ErrorMessage
            Dim result As NonDelivery.FetchPaymenstScheduleEnquiryResult = New NonDelivery.FetchPaymenstScheduleEnquiryResult
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