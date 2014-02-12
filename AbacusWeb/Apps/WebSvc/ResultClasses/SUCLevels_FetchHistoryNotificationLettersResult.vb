Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library

Namespace Apps.WebSvc

    ''' <summary>
    ''' Class to represent Historic notification letter results
    ''' </summary>
    ''' <history>
    ''' Waqas  20/04/2011 D11852F-Contribution notification document generation and storage 
    ''' </history>
    ''' <remarks></remarks>
    Public Class SUCLevels_FetchHistoryNotificationLettersResult

#Region "Fields"

        'Private _ErrorMsg As ErrorMessage = Nothing
        'Private _PagingLinks As String = String.Empty
        'Private _Results As List(Of ViewableHistoryNotificationLetters) = Nothing

#End Region

        Private _errorMsg As ErrorMessage
        Public Property ErrorMsg() As ErrorMessage
            Get
                Return _errorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _errorMsg = value
            End Set
        End Property

        Private _pagingLinks As String
        Public Property PagingLinks() As String
            Get
                Return _pagingLinks
            End Get
            Set(ByVal value As String)
                _pagingLinks = value
            End Set
        End Property


        Private _results As List(Of ViewableHistoryNotificationLetters)
        Public Property Results() As List(Of ViewableHistoryNotificationLetters)
            Get
                Return _results
            End Get
            Set(ByVal value As List(Of ViewableHistoryNotificationLetters))
                _results = value
            End Set
        End Property


    End Class

End Namespace