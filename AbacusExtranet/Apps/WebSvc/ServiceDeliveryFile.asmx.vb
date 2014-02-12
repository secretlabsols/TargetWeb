Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Namespace Apps.WebSvc

    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/ServiceDeliveryFile")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class ServiceDeliveryFile
        Inherits System.Web.Services.WebService

#Region " FetchServiceDeliveryFileList "

        ''' <summary>
        ''' Returns a filtered list of service delviery files.
        ''' </summary>
        ''' <param name="page">The page of results to display.</param>
        ''' <param name="datefrom">The start of the date range to filters the results on</param>
        ''' <param name="dateTo">The end of the date range to filter the results on.</param>
        ''' <param name="deleted">Whether to return deleted files.</param>
        ''' <param name="workInProgress">Whether to return WiP files.</param>
        ''' <param name="processed">Whether to return processed files.</param>
        ''' <param name="awaitingProcessing">Whether to return files that are awaiting processing.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterDescription">The custom list filter string to apply to the description column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' ColinD  05/04/2010  D11755 - implemented ability to filter by failed uploads
        ''' MikeVO  15/10/2009  D11546 - fixes/chnages to SubmittedBy filter option.
        '''                     Added Reference/Description column filters.
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchServiceDeliveryFileList(ByVal page As Integer, _
                                                     ByVal submittedByUserID As Integer, _
                                                     ByVal datefrom As Date, _
                                                     ByVal dateTo As Date, _
                                                     ByVal deleted As Boolean, _
                                                     ByVal workInProgress As Boolean, _
                                                     ByVal processed As Boolean, _
                                                     ByVal awaitingProcessing As Boolean, _
                                                     ByVal listFilterReference As String, _
                                                     ByVal listFilterDescription As String, _
                                                     ByVal failed As Boolean) As FetchServiceDeliveryFileListResult

            Dim msg As ErrorMessage
            Dim files As ArrayList = Nothing
            Dim result As FetchServiceDeliveryFileListResult = New FetchServiceDeliveryFileListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim currentUser As WebSecurityUser

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                currentUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the file list for the specified criteria
                msg = DomContractBL.FetchServiceDeliveryFiles( _
                    conn, _
                    currentUser.ExternalUserID, _
                    page, _
                    pageSize, _
                    submittedByUserID, _
                    datefrom, _
                    dateTo, _
                    deleted, _
                    processed, _
                    workInProgress, _
                    awaitingProcessing, _
                    failed, _
                    listFilterReference, _
                    listFilterDescription, _
                    totalRecords, _
                    files _
                )
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .ServiceDeliveryFile = files
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchServiceDeliveryFileList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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