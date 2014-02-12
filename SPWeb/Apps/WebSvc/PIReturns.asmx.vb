Imports System.Web.Services
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps.WebSvc
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/SPWeb/Apps/WebSvc/PIReturns")> _
    Public Class PIReturns
        Inherits System.Web.Services.WebService

#Region " Web Services Designer Generated Code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Web Services Designer.
            InitializeComponent()

            'Add your own initialization code after the InitializeComponent() call

        End Sub

        'Required by the Web Services Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Web Services Designer
        'It can be modified using the Web Services Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            components = New System.ComponentModel.Container
        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            'CODEGEN: This procedure is required by the Web Services Designer
            'Do not modify it using the code editor.
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region


#Region " FetchSubmittedReturnsList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of remittances for the specified provider/service/date range.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="providerID">The ID of the provider.</param>
        ''' <param name="serviceID">The ID of the service.</param>
        ''' <param name="financialYear">The financial year to filter the result on.</param>
        ''' <param name="quarter">The quarter to filter the results on.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history><![CDATA[
        ''' 	[MikeVO]	09/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSubmittedReturnsList(ByVal page As Integer, ByVal providerID As Integer, ByVal serviceID As Integer, _
                                            ByVal financialYear As String, ByVal quarter As String, _
                                            ByVal status As Int32) As FetchPISubmissionListResult

            Dim msg As ErrorMessage
            Dim piReturns As ArrayList = Nothing
            Dim result As FetchPISubmissionListResult = New FetchPISubmissionListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ExternalUserID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the remittance list
                msg = SPClassesBL.FetchPIReturns(conn, userID, page, pageSize, providerID, serviceID, financialYear, quarter, _
                                                                                                status, totalRecords, piReturns)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PIReturns = piReturns
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchSubmittedReturnsList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchPISubmissionStatusList "

        ''' <summary>
        ''' Retrieves a list of PI submissions.
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="providerID"></param>
        ''' <param name="serviceID"></param>
        ''' <param name="financialYearFrom"></param>
        ''' <param name="quarterFrom"></param>
        ''' <param name="financialYearTo"></param>
        ''' <param name="quarterTo"></param>
        ''' <param name="status"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPISubmissionStatusList(ByVal page As Integer, ByVal providerID As Integer, ByVal serviceID As Integer, _
                                            ByVal financialYearFrom As String, ByVal quarterFrom As String, _
                                            ByVal financialYearTo As String, ByVal quarterTo As String, _
                                            ByVal status As String) As FetchPISubmissionListResult

            Dim msg As ErrorMessage
            Dim piReturns As ArrayList = Nothing
            Dim result As FetchPISubmissionListResult = New FetchPISubmissionListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ExternalUserID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the remittance list
                msg = SPClassesBL.FetchPISubmissionStatusList(conn, userID, page, pageSize, providerID, serviceID, financialYearFrom, quarterFrom, _
                                                                            financialYearTo, quarterTo, status, totalRecords, piReturns)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PIReturns = piReturns
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchPISubmissionStatusList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

        ' WEB SERVICE EXAMPLE
        ' The HelloWorld() example service returns the string Hello World.
        ' To build, uncomment the following lines then save and build the project.
        ' To test this web service, ensure that the .asmx file is the start page
        ' and press F5.
        '
        '<WebMethod()> _
        'Public Function HelloWorld() As String
        '   Return "Hello World"
        'End Function

    End Class
End Namespace
