
Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library.SdsTransactions

''' <summary>
''' Simple class to represent the results of SdsTransactions.ReconsiderTransactions function
''' </summary>
''' <history>
''' ColinDaly   11/05/2011 SDS671 - Added properties ReconsiderationExceptions and ReconsiderationWarnings.
''' MikeVO      11/04/2011 SDS508 - Added ReconsiderExceptionsWarningsCount property.
''' ColinDaly   SDS370 Created 23/11/2010 Created
''' </history>
Public Class SdsTransactions_ReconsiderTransactionsResult

#Region "Fields"

    Private _ErrMsg As ErrorMessage
    Private _ReconsiderationExceptions As New List(Of SdsTransactionReconsiderationException)()
    Private _ReconsiderExceptionsWarningsCount As Integer = 0
    Private _ReconsiderationWarnings As New List(Of SdsTransactionReconsiderationWarning)()

#End Region

#Region "Properties"

    ''' <summary>
    ''' Gets or sets the error message for the call.
    ''' </summary>
    ''' <value>The err MSG.</value>
    Public Property ErrMsg() As ErrorMessage
        Get
            Return _ErrMsg
        End Get
        Set(ByVal value As ErrorMessage)
            _ErrMsg = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the reconsideration exceptions.
    ''' </summary>
    ''' <value>The reconsideration exceptions.</value>
    Public Property ReconsiderationExceptions() As List(Of SdsTransactionReconsiderationException)
        Get
            Return _ReconsiderationExceptions
        End Get
        Set(ByVal value As List(Of SdsTransactionReconsiderationException))
            _ReconsiderationExceptions = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the number of exceptions/warnings encountered.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReconsiderExceptionsWarningsCount() As Integer
        Get
            Return _ReconsiderExceptionsWarningsCount
        End Get
        Set(ByVal value As Integer)
            _ReconsiderExceptionsWarningsCount = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the reconsideration warnings.
    ''' </summary>
    ''' <value>The reconsideration warnings.</value>
    Public Property ReconsiderationWarnings() As List(Of SdsTransactionReconsiderationWarning)
        Get
            Return _ReconsiderationWarnings
        End Get
        Set(ByVal value As List(Of SdsTransactionReconsiderationWarning))
            _ReconsiderationWarnings = value
        End Set
    End Property

#End Region

End Class
