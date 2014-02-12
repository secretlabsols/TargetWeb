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
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/SUCLevels")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class SUCLevels
        Inherits System.Web.Services.WebService

#Region "FetchContributionMonitorSummary"

        ''' <summary>
        ''' Fetches the contribution monitor summary records, paged..
        ''' </summary>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchContributionMonitorSummary(ByVal page As Integer, _
                                                           ByVal selectedID As Integer, _
                                                           ByVal serviceUsersAreBeingCollected As Nullable(Of Boolean), _
                                                           ByVal serviceUsersRequiringContributionNotificationLetter As Nullable(Of Boolean), _
                                                           ByVal serviceUsersHavingOneOrMoreIncompleteContributionLevel As Nullable(Of Boolean), _
                                                           ByVal serviceUsersHavingOneOrMissingOrProvAssessment As Nullable(Of Boolean), _
                                                           ByVal serviceUsersHavingNilValueCostProvAssessment As Nullable(Of Boolean), _
                                                           ByVal serviceUserName As String, _
                                                           ByVal serviceUserReference As String) _
                                                           As SUCLevels_FetchContributionMonitorSummaryResult

            Dim msg As ErrorMessage
            Dim result As New SUCLevels_FetchContributionMonitorSummaryResult()
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10

            Try

                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the results
                msg = ServiceUserContributionLevelBL.FetchContributionMonitorSummary(conn, _
                                                                                    page, _
                                                                                    pageSize, _
                                                                                    serviceUsersAreBeingCollected, _
                                                                                    serviceUsersRequiringContributionNotificationLetter, _
                                                                                    serviceUsersHavingOneOrMoreIncompleteContributionLevel, _
                                                                                    serviceUsersHavingOneOrMissingOrProvAssessment, _
                                                                                    serviceUsersHavingNilValueCostProvAssessment, _
                                                                                    serviceUserName, _
                                                                                    serviceUserReference, _
                                                                                    totalRecords, _
                                                                                    result.Results)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage()
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:SdsContributionMonitorResults_FetchContributionMonitorSummary({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected

            Finally

                SqlHelper.CloseConnection(conn)

            End Try

            Return result

        End Function

#End Region

#Region "FetchPendingServiceUserContributionLevelPendingChanges"

        ''' <summary>
        ''' Fetches the pending service user contribution level with pending changes.
        ''' </summary>
        ''' <param name="page">The page to fetch.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPendingServiceUserContributionLevelPendingChanges(ByVal page As Integer, _
                                                                               ByVal selectedID As Integer, _
                                                                               ByVal clientID As Integer) _
                                                                               As SucLevels_FetchPendingServiceUserContributionLevelPendingChangesResult

            Dim msg As ErrorMessage
            Dim result As New SucLevels_FetchPendingServiceUserContributionLevelPendingChangesResult()
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            '' for notify 
            If page = -1 Then
                pageSize = 1000
                page = 1
            End If
            Try

                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the results
                msg = ServiceUserContributionLevelBL.FetchPendingServiceUserContributionLevelPendingChanges(conn, _
                                                                                                            page, _
                                                                                                            pageSize, _
                                                                                                            clientID, _
                                                                                                            totalRecords, _
                                                                                                            result.Results)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage()
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:PendingSuclChangesSelector_FetchPendingServiceUserContributionLevelsList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected

            Finally

                SqlHelper.CloseConnection(conn)

            End Try

            Return result

        End Function

#End Region

#Region " FetchSUCLevelList "

        ''' <summary>
        ''' returns rows of service user contribution levels
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="selectedSucLevelID"></param>
        ''' <param name="clientID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSUCLevelList(ByVal page As Integer, ByVal selectedSucLevelID As Integer, _
         ByVal clientID As Integer, ByVal isSDS As Boolean) As FetchSUCLevelListResult

            Dim msg As ErrorMessage
            Dim result As FetchSUCLevelListResult = New FetchSUCLevelListResult
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
                msg = ServiceUserContributionLevelBL.FetchSUCLevelList( _
                 conn, page, pageSize, selectedSucLevelID, clientID, isSDS, totalRecords, result.SUCLists)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:SUCLevelSelector_FetchSUCLevelList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region "FetchHistoryNotificationsLetters"

        ''' <summary>
        ''' Fetches history notification letters.
        ''' </summary>
        ''' <param name="page">The page to fetch.</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchHistoryNotificationLetters(ByVal page As Integer, _
                                                        ByVal selectedID As Integer, _
                                                        ByVal clientID As Integer) _
                                                        As SUCLevels_FetchHistoryNotificationLettersResult

            Dim msg As ErrorMessage
            Dim result As New SUCLevels_FetchHistoryNotificationLettersResult()
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10

            Try

                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the results
                msg = ServiceUserContributionLevelBL.FetchHistoryNotificationLetters(conn, _
                                                                                     page, _
                                                                                     pageSize, _
                                                                                     clientID, _
                                                                                     totalRecords, _
                                                                                     result.Results)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage()
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:HisstoryNotification_FetchHistoryNotificationLetters({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected

            Finally

                SqlHelper.CloseConnection(conn)

            End Try

            Return result

        End Function

#End Region

    End Class

End Namespace