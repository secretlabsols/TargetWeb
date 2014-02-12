Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports DPI = Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections

Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to retrieve Service Order information.
    ''' </summary>
    ''' <remarks></remarks>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/ServiceOrder")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class ServiceOrder
        Inherits System.Web.Services.WebService

#Region " FetchServiceOrderList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of service orders.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedServiceOrderID">The ID of the DSO to select.</param>
        ''' <param name="providerID">The ID of the provider to filter the results on.</param>
        ''' <param name="contractID">The ID of the com contract to filter the results on.</param>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="dateFrom">The start of the period to filter the results on.</param>
        ''' <param name="dateTo">The end of the period to filter the result son.</param>
        ''' <param name="serviceGroupID">Service Group filter</param>
        ''' <param name="listFilterOrderRef">List Filter order Ref</param>
        ''' <param name="listFilterProvider">list filter Provider</param>
        ''' <param name="listFilterSvcUserName">List Filter Service User Name</param>
        ''' <param name="listFilterSvcUserRef">List Filter Service User Reference</param>
        ''' <param name="listFilterContract">List Filter Contract</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceOrderList(ByVal page As Integer, ByVal selectedServiceOrderID As Integer, _
         ByVal providerID As Integer, ByVal contractID As Integer, ByVal clientID As Integer, _
         ByVal dateFrom As Date, ByVal dateTo As Date, ByVal serviceGroupID As Integer, ByVal listFilterSvcUserName As String, _
         ByVal listFilterSvcUserRef As String, ByVal listFilterProvider As String, _
         ByVal listFilterOrderRef As String, ByVal listFilterContract As String, ByVal listFilterServiceGrp As String) As FetchServiceOrderListResult

            Dim msg As ErrorMessage
            Dim result As FetchServiceOrderListResult = New FetchServiceOrderListResult
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
                msg = CommissionedServiceBL.FetchServiceOrderList( _
                 conn, page, pageSize, selectedServiceOrderID, providerID, contractID, clientID, dateFrom, dateTo, _
                 listFilterSvcUserName, listFilterSvcUserRef, listFilterProvider, listFilterOrderRef, listFilterContract, _
                 listFilterServiceGrp, serviceGroupID, totalRecords, result.Orders)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:ServiceOrderSelector_FetchServiceOrderList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

    End Class

End Namespace