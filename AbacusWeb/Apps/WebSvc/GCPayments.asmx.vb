Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Target.Library
Imports System.Data.SqlClient
Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.SDS


Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to retrieve Service Order information.
    ''' </summary>
    ''' <remarks></remarks>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/GCPayments")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class GCPayments
        Inherits System.Web.Services.WebService

#Region " FetchGCPaymentList "

        ''' <summary>
        ''' returns rows of service user contribution levels
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="selectedGCPaymentID"></param>
        ''' <param name="clientID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchGCPaymentList(ByVal page As Integer, ByVal selectedGCPaymentID As Integer, _
         ByVal clientID As Integer, ByVal listFilterContractNumber As String, ByVal listFilterCreditorName As String, _
         ByVal listFilterCreditorRef As String, ByVal listFilterServiceUser As String, ByVal listFilterPaymentNumber As String) As FetchGCPaymentListResult

            Dim msg As ErrorMessage
            Dim result As FetchGCPaymentListResult = New FetchGCPaymentListResult
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
                msg = GenericCreditorPaymentBL.FetchGCPaymentList( _
                 conn, page, pageSize, selectedGCPaymentID, clientID, listFilterContractNumber, _
                 listFilterCreditorName, listFilterCreditorRef, listFilterServiceUser, listFilterPaymentNumber, totalRecords, result.GCPaymentLists)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:GCPaymentSelector_FetchGCPaymentList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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