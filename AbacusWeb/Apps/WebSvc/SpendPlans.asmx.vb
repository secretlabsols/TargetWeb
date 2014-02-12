Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Target.Library
Imports System.Data.SqlClient
Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections

Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to retrieve Service Order information.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  25/07/2011  A4WA#6959 - pull back current Gross Cost rather than latest.
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/SpendPlans")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class SpendPlans
        Inherits System.Web.Services.WebService

#Region " FetchSpendPlanList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a paginated list of spend plans.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="selectedSpendPlanID">The ID of the spendplan to select.</param>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="dateFrom">The start of the period to filter the results on.</param>
        ''' <param name="dateTo">The end of the period to filter the result son.</param>
        ''' <param name="listFilterSpendPlanRef">List Filter order Ref</param>
        ''' <param name="listFilterSvcUserName">List Filter Service User Name</param>
        ''' <param name="listFilterSvcUserRef">List Filter Service User Reference</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSpendPlanList(ByVal page As Integer, ByVal selectedSpendPlanID As Integer, _
         ByVal clientID As Integer, ByVal dateFrom As Date, ByVal dateTo As Date, _
         ByVal listFilterSvcUserName As String, ByVal listFilterSvcUserRef As String, _
         ByVal listFilterSpendPlanRef As String) As FetchSpendPlanListResult

            Dim msg As ErrorMessage
            Dim result As FetchSpendPlanListResult = New FetchSpendPlanListResult
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
                msg = SpendPlanBL.FetchSpendPlanList( _
                 conn, page, pageSize, selectedSpendPlanID, clientID, dateFrom, dateTo, _
                 listFilterSvcUserName, listFilterSvcUserRef, listFilterSpendPlanRef, totalRecords, result.SpendPlans)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:SpendPlanSelector_FetchSpendPlanList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " DisplayOverlappingWarningMessage "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     determines if a message should be displayed in the UI when saving.
        ''' </summary>
        ''' <param name="spendPlanID">The ID of the spend plan record.</param>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="newDateFrom">The start of the period to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DisplayOverlappingWarningMessage(ByVal spendPlanID As Integer, ByVal clientID As Integer, ByVal newDateFrom As String) As Boolean

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim dateFromChangedDifferentDay As Boolean = False
            Dim plan As SpendPlan
            Dim overlappingDirectPayments As Boolean = False
            Dim minDateFrom As Date
            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return False
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                plan = New SpendPlan(conn, String.Empty, String.Empty)
                msg = plan.Fetch(spendPlanID)
                If Not msg.Success Then Return False

                'has the start date of the plan changed
                dateFromChangedDifferentDay = (plan.DateFrom.DayOfWeek <> Convert.ToDateTime(newDateFrom).DayOfWeek)

                If plan.DateFrom < Convert.ToDateTime(newDateFrom) Then
                    minDateFrom = plan.DateFrom
                Else
                    minDateFrom = Convert.ToDateTime(newDateFrom)
                End If

                'the plan has been changed such that it now starts on a different day of the week
                If dateFromChangedDifferentDay Then
                    'See if there are any overlapping DP Payments for the duration of this spend plan
                    msg = DPContractBL.OverlappingSDSDirectPayments(conn, plan.ClientID, minDateFrom, plan.DateTo, spendPlanID, overlappingDirectPayments)
                    If Not msg.Success Then Return False

                End If


            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return overlappingDirectPayments

        End Function

#End Region

    End Class

End Namespace