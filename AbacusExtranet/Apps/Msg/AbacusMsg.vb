
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Msg

Namespace Apps.Msg

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Apps.Msg.AbacusMsg
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class to implement the Abacus Extranet custom msg interface.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	04/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AbacusMsg
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
        ''' <history>
        ''' 	[Mikevo]	06/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function RestrictMsgRecipients(ByVal connectionString As String, ByVal externalUserID As Integer, _
                                                ByRef result As RestrictMsgRecipientsResult) As ErrorMessage Implements ICustomMsg.RestrictMsgRecipients
            Return AbacusClassesBL.RestrictMsgRecipients(connectionString, externalUserID, result.Restricted)
        End Function

        Public Function GetMessagePassingType() As MessagePassingType Implements ICustomMsg.GetMessagePassingType
            Return MessagePassingType.CompanyToCompany
        End Function

    End Class

End Namespace