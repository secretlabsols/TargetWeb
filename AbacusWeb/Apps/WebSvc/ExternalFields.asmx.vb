Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    'To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    <System.Web.Script.Services.ScriptService()> _
   <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/ExternalFields")> _
       <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
       <ToolboxItem(False)> _
    Public Class ExternalFields
        Inherits System.Web.Services.WebService

#Region " ToggleExternalIsMaster "

        ' ''' -----------------------------------------------------------------------------
        ' ''' <summary>
        ' '''     Toggles the External Is Master Flag on the External_Fields Table.
        ' ''' </summary>
        ' ''' <param name="ExternalFieldID">The ID of the External_Field Record.</param>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        ' ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ToggleExternalIsMaster(ByVal ExternalFieldID As String) As BooleanResult

            Dim msg As ErrorMessage
            Dim result As BooleanResult = New BooleanResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = ExternalFieldsBL.ToggleExternalIsMaster(conn, ExternalFieldID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    result.Value = False
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Value = True
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " ToggleExceptionIfBlank "

        ' ''' -----------------------------------------------------------------------------
        ' ''' <summary>
        ' '''     Toggles the ExceptionIfBlank Flag on the External_Fields Table.
        ' ''' </summary>
        ' ''' <param name="ExternalFieldID">The ID of the External_Field Record.</param>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        ' ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ToggleExceptionIfBlank(ByVal ExternalFieldID As String, ByVal toggleWithholdUpdate As Boolean) As BooleanResult

            Dim msg As ErrorMessage
            Dim result As BooleanResult = New BooleanResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = ExternalFieldsBL.ToggleExceptionIfBlank(conn, ExternalFieldID, toggleWithholdUpdate)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    result.Value = False
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Value = True
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " ToggleAllowEditInAbacus "

        ' ''' -----------------------------------------------------------------------------
        ' ''' <summary>
        ' '''     Toggles the AllowEditInAbacus on the External_Fields Table.
        ' ''' </summary>
        ' ''' <param name="ExternalFieldID">The ID of the External_Field Record.</param>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        ' ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ToggleAllowEditInAbacus(ByVal ExternalFieldID As String) As BooleanResult

            Dim msg As ErrorMessage
            Dim result As BooleanResult = New BooleanResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = ExternalFieldsBL.ToggleAllowEditInAbacus(conn, ExternalFieldID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    result.Value = False
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Value = True
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