
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Msg
Imports Target.SP.Library

Namespace Apps.Msg

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : Apps.Msg.SPMsg
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class to implement the SP custom msg interface.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      21/11/2006  Added GetMessagePassingType().
    ''' 	[Mikevo]	16/11/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class SPMsg
        Implements ICustomMsg

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Determines if the specified external user is allowed to send message to
        '''     any recipient.
        ''' </summary>
        ''' <param name="connectionString">A database connection string.</param>
        ''' <param name="externalUserID">The ID of an external user.</param>
        ''' <param name="result">Upon success, contains the result.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Checks if the specified user is a web-admin user.  If not, they are restricted.
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	16/11/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Function RestrictMsgRecipients(ByVal connectionString As String, ByVal externalUserID As Integer, _
                                                ByRef result As RestrictMsgRecipientsResult) As ErrorMessage Implements ICustomMsg.RestrictMsgRecipients

            Const SP_NAME As String = "pr_FetchSPUserDetail"

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim spParams As SqlParameter()
            Dim reader As SqlDataReader = Nothing
            Dim webUser As Byte

            Try
                ' get the user record
                conn = SqlHelper.GetConnection(connectionString)
                spParams = SqlHelperParameterCache.GetSpParameterSet(conn, SP_NAME, False)
                spParams(0).Value = externalUserID
                reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, SP_NAME, spParams)
                reader.Read()
                webUser = reader("SpWebUser")
                reader.Close()
                conn.Close()

                If webUser <> SPClassesBL.SpWebUser.SpWebAdminUser Then result.Restricted = True

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0001")     ' unexpected
            Finally
                If Not reader Is Nothing Then reader.Close()
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg

        End Function

        Public Function GetMessagePassingType() As MessagePassingType Implements ICustomMsg.GetMessagePassingType
            Return MessagePassingType.CompanyToCompany
        End Function

    End Class

End Namespace