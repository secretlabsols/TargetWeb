
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
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/Sds")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class Sds
        Inherits System.Web.Services.WebService

#Region " FetchPersonalBudgetEnquiryList "

        ''' <summary>
        ''' Retrieves a paginated list of SDS personal budgets.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="dateFrom">The start of the date range to filter the results on.</param>
        ''' <param name="dateTo">The end of the date range to filter the results on.</param>
        ''' <param name="selectedBudgetID">The ID of the personal budget to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPersonalBudgetEnquiryList(ByVal page As Integer, ByVal clientID As Integer, _
                                            ByVal dateFrom As Date, ByVal dateTo As Date, _
                                            ByVal selectedBudgetID As Integer) As FetchPersonalBudgetEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchPersonalBudgetEnquiryListResult = New FetchPersonalBudgetEnquiryListResult
            Dim currentUser As WebSecurityUser
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

                currentUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of personal budgets
                msg = SdsBL.FetchPersonalBudgetEnquiryResults(conn, page, pageSize, _
                                    clientID, dateFrom, dateTo, selectedBudgetID, _
                                    totalRecords, result.Budgets)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchBudgetList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " CalculatePersonalBudget "

        ''' <summary>
        ''' Calculates a RAS personal budget from the RAS type, date and point score.
        ''' </summary>
        ''' <param name="rasTypeID">The ID of the RAS type.</param>
        ''' <param name="rasDate">The date of the RAS score.</param>
        ''' <param name="pointScore">The RAS point score.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CalculatePersonalBudget(ByVal rasTypeID As Integer, ByVal rasDate As Date, _
                                                ByVal pointScore As Integer) As DecimalResult

            Dim msg As ErrorMessage
            Dim result As DecimalResult = New DecimalResult()
            Dim currentUser As WebSecurityUser
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                currentUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' calc the personal budgets
                msg = SdsBL.CalculatePersonalBudget(conn, rasTypeID, rasDate, pointScore, result.Value)
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

#Region " FetchIndicativeBudgetEnquiryList "

        ''' <summary>
        ''' Retrieves a paginated list of SDS indicated budget objects.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="dateFrom">The start of the date range to filter the results on.</param>
        ''' <param name="dateTo">The end of the date range to filter the results on.</param>
        ''' <param name="selectedIndicativeBudgetID">The ID of the indicative budget to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchIndicativeBudgetEnquiryList(ByVal page As Integer, ByVal clientID As Integer, _
                                            ByVal dateFrom As Date, ByVal dateTo As Date, _
                                            ByVal selectedIndicativeBudgetID As Integer) As FetchIndicativeEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchIndicativeEnquiryListResult = New FetchIndicativeEnquiryListResult
            Dim currentUser As WebSecurityUser
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

                currentUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of records
                msg = SdsBL.FetchIndicativeBudgetEnquiryResults(conn, page, pageSize, _
                                    clientID, dateFrom, dateTo, selectedIndicativeBudgetID, _
                                    totalRecords, result.IndicativeBudgets)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchIndicativeBudgetList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " DisplayOverlappingWarningMessageOnBudgetPeriod "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     determines if a message should be displayed in the UI when changing the 
        '''     date from value on the budget period screen.
        ''' </summary>
        ''' <param name="clientID">The ID of the client to filter the results on.</param>
        ''' <param name="newDateFrom">The start of the period to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DisplayOverlappingWarningMessageOnBudgetPeriod(ByVal clientID As Integer, _
                                                                       ByVal originalDateFrom As String, _
                                                                       ByVal newDateFrom As String, _
                                                                       ByVal dateToValue As String) As Boolean

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim dateFromChangedDifferentDay As Boolean = False
            Dim overlappingDirectPayments As Boolean = False
            Dim minDateFrom As Date
            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    Return False
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                'has the start date of the plan changed
                dateFromChangedDifferentDay = (Convert.ToDateTime(originalDateFrom).DayOfWeek <> Convert.ToDateTime(newDateFrom).DayOfWeek)

                If Convert.ToDateTime(originalDateFrom) < Convert.ToDateTime(newDateFrom) Then
                    minDateFrom = Convert.ToDateTime(originalDateFrom)
                Else
                    minDateFrom = Convert.ToDateTime(newDateFrom)
                End If

                'the plan has been changed such that it now starts on a different day of the week
                If dateFromChangedDifferentDay Then
                    'See if there are any overlapping DP Payments for the duration of this spend plan
                    msg = DPContractBL.OverlappingSDSDirectPayments(conn, clientID, minDateFrom, _
                                                                    Convert.ToDateTime(dateToValue), 0, overlappingDirectPayments)
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