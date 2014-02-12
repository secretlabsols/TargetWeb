
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Reports
Imports Target.Web.Apps.Security

Namespace Apps.Reports.WebSvc

    ''' <summary>
    ''' Web service functionality related to Abacus Reporting.
    ''' </summary>
    ''' <remarks></remarks>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/Reports")> _
     <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
     <ToolboxItem(False)> _
     Public Class Reports
        Inherits System.Web.Services.WebService

#Region " ClearSession "

        ''' <summary>
        ''' Clears cached RDL objects from session state.
        ''' </summary>
        ''' <remarks>This improves overall performance as session state is not cluttered with stale data.</remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)> _
        Public Sub ClearSession()

            Const CACHED_REPORT_TYPE As String = "Microsoft.Reporting.WebForms.ReportHierarchy"

            Dim msg As ErrorMessage

            ' check user is logged in
            msg = SecurityBL.ValidateLogin()
            If Not msg.Success Then Exit Sub

            ' purge all session objects of type
            Try
                For index As Integer = (Session.Keys.Count - 1) To 0 Step -1
                    If Session(index).GetType().FullName = CACHED_REPORT_TYPE Then
                        Session.RemoveAt(index)
                    End If
                Next
            Catch ex As Reflection.TargetInvocationException
                ' We may get this exception if a remote report has already expired
            End Try


        End Sub

#End Region

#Region " FetchReportsList "

        ''' <summary>
        ''' Retrieves a pagingated list of reports.
        ''' </summary>
        ''' <param name="page">The current page required.</param>
        ''' <param name="listFilterDesc">The custom list filter string to apply to the Description column.</param>
        ''' <param name="listFilterPath">The custom list filter string to apply to the Path column.</param>
        ''' <param name="listFilterCategories">The custom list filter string to apply to the Categories column.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  26/10/2009  D11710 - created.
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)> _
        Public Function FetchReportsList(ByVal page As Integer, _
                                        ByVal listFilterDesc As String, _
                                        ByVal listFilterPath As String, _
                                        ByVal listFilterCategories As String) As FetchReportListResult

            Dim msg As ErrorMessage
            Dim result As FetchReportListResult = New FetchReportListResult()
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings
            Dim currentUser As WebSecurityUser
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

                ' get security parameters
                currentUser = SecurityBL.GetCurrentUser()
                settings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

                ' get the list of reports
                msg = ReportsBL.FetchReportsList(conn, _
                                                 page, _
                                                 pageSize, _
                                                 settings.CurrentApplicationID, _
                                                 currentUser.ID, _
                                                 listFilterDesc, _
                                                 listFilterPath, _
                                                 listFilterCategories, _
                                                 totalRecords, _
                                                 result.Reports _
                )
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchReportsList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return result

        End Function

#End Region

    End Class

#Region " FetchReportListResult "

    ''' <summary>
    ''' Simple class to return data for the FetchReportList() web service.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchReportListResult
        Public ErrMsg As ErrorMessage
        Public Reports As List(Of ViewableWebReport)
        Public PagingLinks As String
    End Class

#End Region

End Namespace