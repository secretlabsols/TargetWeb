Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library.SdsTransactions
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to manage sds transactions and sds transaction reconsiderations
    ''' </summary>
    ''' <history>
    ''' ColinD      11/05/2011 SDS671 - Added additional data to the response from ReconsiderTransactionsByClientBudgetPeriod i.e. reconsideration exceptions\warnings are included. This is mainly for debugging purposes as it will not be displayed on the ui.
    ''' MikeVO      11/04/2011 SDS508 - Support for new SdsTransactions_ReconsiderTransactionsResult.ReconsiderExceptionsWarningsCount property.
    ''' ColinDaly   SDS370 Created 23/11/2010 Created with Function ReconsiderTransactionsByClientBudgetPeriod
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/SdsTransactions")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
    Public Class SdsTransactions
        Inherits System.Web.Services.WebService

#Region "Fields"

        ' locals
        Private _ConnectionString As String

        ' constants
        Private Const _ConnectionStringKey As String = "Abacus"
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber

#End Region

#Region "Functions"

        ''' <summary>
        ''' Reconsider Transactions by Client Budget Period and Transaction Type (Bitwise).
        ''' </summary>
        ''' <param name="clientBudgetPeriodID">The client budget period to reconsider.</param>
        ''' <param name="type">The transaction type(s) to reconsider (bitwise).</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(EnableSession:=True, _
                    Description:="Reconsider Transactions by Client Budget Period and Transaction Type (Bitwise)."), _
        AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function ReconsiderTransactionsByClientBudgetPeriod(ByVal clientBudgetPeriodID As Integer, _
                                                                   ByVal type As SdsTransactionBL.SdsTransactionType) _
                                                                   As SdsTransactions_ReconsiderTransactionsResult

            Dim conn As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New SdsTransactions_ReconsiderTransactionsResult()

            Try

                ' create and open a db connection
                conn = SqlHelper.GetConnection(ConnectionString)

                Using transactionManager As New SdsTransactionManager(conn)
                    ' create a new transaction manager object in order to reconsider 
                    ' the transaction types and client budget period id, ignores client id

                    msg = transactionManager.Reconsider(Nothing, clientBudgetPeriodID, type)

                    ' setup the result with exceptions and warnings etc
                    With result
                        .ReconsiderationExceptions = transactionManager.ReconsiderationExceptions
                        .ReconsiderExceptionsWarningsCount = transactionManager.ReconsiderationExceptions.Count + transactionManager.ReconsiderationWarnings.Count
                        .ReconsiderationWarnings = transactionManager.ReconsiderationWarnings
                    End With

                End Using

                If Not msg.Success Then
                    'reconsideration wasnt successful so throw an error to caller

                    result.ErrMsg = msg
                    Return result

                End If

                ' setup a successful error message
                With result
                    .ErrMsg = New ErrorMessage()
                    .ErrMsg.Success = True
                End With

            Catch ex As Exception
                ' wrap the exception and add to the result

                With result
                    .ErrMsg = Utils.CatchError(ex, _GeneralErrorNumber)
                    .ErrMsg.Success = False
                End With

            Finally
                ' finally close the db connection 

                SqlHelper.CloseConnection(conn)

            End Try

            Return result

        End Function

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the connection string.
        ''' </summary>
        ''' <value>The connection string.</value>
        Public ReadOnly Property ConnectionString() As String
            Get
                If String.IsNullOrEmpty(_ConnectionString) OrElse _ConnectionString.Trim().Length = 0 Then
                    ' if we havent already fetched the connection string then do so
                    _ConnectionString = ConnectionStrings(_ConnectionStringKey).ConnectionString
                End If
                Return _ConnectionString
            End Get
        End Property

#End Region

    End Class

End Namespace
