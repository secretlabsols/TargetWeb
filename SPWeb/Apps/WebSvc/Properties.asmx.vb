Imports System.Configuration.ConfigurationManager
Imports System.Web.Services
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.SP.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.SP.Library.Collections
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/SPWeb/Apps/WebSvc/Properties")> _
    Public Class Properties
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

#Region " FetchPropertyList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of Properties for the specified user/Service.
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="ServiceID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[paul]	03/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPropertyList(ByVal page As Integer, ByVal ServiceID As Integer) As FetchPropertyListResult

            Dim msg As ErrorMessage
            Dim Properties As vwListSPPropertyCollection = Nothing
            Dim result As FetchPropertyListResult = New FetchPropertyListResult
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

                ' get the Property list
                msg = SPClassesBL.FetchProperties(conn, userID, ServiceID, page, pageSize, totalRecords, Properties)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Properties = Properties
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchPropertyList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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
