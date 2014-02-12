Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Namespace Apps.WebSvc

<System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/Occupancy")> _
Public Class Occupancy
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

#Region " FetchResOccupancyList "


        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchResOccupancyList(ByVal page As Integer, ByVal selectedEstablishmentID As Integer, _
                                        ByVal datefrom As Date, ByVal dateTo As Date, ByVal movement As Int16) As FetchResOccupancyListResult

            Dim msg As ErrorMessage
            Dim homes As ArrayList = Nothing
            Dim result As FetchResOccupancyListResult = New FetchResOccupancyListResult
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

                ' get the occupancy list for the specified home
                msg = AbacusClassesBL.FetchResOccupancy( _
                                        conn, userID, page, pageSize, selectedEstablishmentID, datefrom, dateTo, movement, _
                                        totalRecords, homes)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Occupancy = homes
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchOccupancyList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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
