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
    ''' Web service to retrieve contract information.
    ''' </summary>
    ''' <remarks></remarks>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/Contract")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class Contract
        Inherits System.Web.Services.WebService

#Region " FetchContractList "

        ''' <summary>
        ''' Retrieves a paginated list of contracts.
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
        Public Function FetchContractList(ByVal page As Integer, ByVal selectedDomContractID As Integer, _
                     ByVal establishmentID As Integer, ByVal contractType As String, ByVal contractGroupID As Integer, _
                     ByVal dateFrom As Date, ByVal dateTo As Date, ByVal listFilterNumber As String, _
                     ByVal listFilterTitle As String, ByVal listFilterSU As String, ByVal listFilterGroup As String, ByVal listFilterSvcGroup As String, _
                     ByVal contractEndReasonID As Integer, ByVal serviceGroupID As Integer, ByVal serviceGroupClassificationID As Integer) As FetchGenericContractListResult

            Dim msg As ErrorMessage
            Dim result As FetchGenericContractListResult = New FetchGenericContractListResult
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
                msg = GenericContractBL.FetchContractList( _
                 conn, page, pageSize, selectedDomContractID, establishmentID, contractType, contractGroupID, dateFrom, dateTo, _
                 listFilterNumber, listFilterTitle, listFilterSU, listFilterGroup, listFilterSvcGroup, contractEndReasonID, settings.CurrentApplicationID, _
                 serviceGroupID, serviceGroupClassificationID, totalRecords, result.Contracts)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchContractList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, "E0001")   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

    End Class

End Namespace