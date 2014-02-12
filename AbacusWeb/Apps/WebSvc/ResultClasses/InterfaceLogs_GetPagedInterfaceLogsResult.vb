Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.InterfaceLogs

Namespace Apps.WebSvc

    ''' <summary>
    ''' Class used to represent the result of a call to InterfaceLogs.GetPagedInterfaceLogsResult
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D11874 Created 18/02/2010
    ''' </history>
    Public Class InterfaceLogs_GetPagedInterfaceLogsResult

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _PagingLinks As String = String.Empty
        Private _Items As List(Of ViewableInterfaceLog) = Nothing

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
        ''' Gets or sets the items.
        ''' </summary>
        ''' <value>The items.</value>
        Public Property Items() As List(Of ViewableInterfaceLog)
            Get
                Return _Items
            End Get
            Set(ByVal value As List(Of ViewableInterfaceLog))
                _Items = value
            End Set
        End Property

#End Region

    End Class

End Namespace


