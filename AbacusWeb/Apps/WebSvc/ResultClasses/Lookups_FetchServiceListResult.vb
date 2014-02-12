Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library

Namespace Apps.WebSvc

    ''' <summary>
    ''' Simple class to represent the results of Lookups.FetchServiceListResult function
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D11964A Created 26/11/2010 Created
    ''' </history>
    Public Class Lookups_FetchServiceListResult

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _PagingLinks As String = String.Empty
        Private _Services As List(Of ViewableService) = Nothing

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrorMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the paging links.
        ''' </summary>
        ''' <value>The paging links.</value>
        Public Property PagingLinks() As String
            Get
                Return _PagingLinks
            End Get
            Set(ByVal value As String)
                _PagingLinks = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the services.
        ''' </summary>
        ''' <value>The services.</value>
        Public Property Services() As List(Of ViewableService)
            Get
                Return _Services
            End Get
            Set(ByVal value As List(Of ViewableService))
                _Services = value
            End Set
        End Property

#End Region

    End Class

End Namespace

