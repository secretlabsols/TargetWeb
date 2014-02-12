Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.CreditorPayments

Namespace Apps.WebSvc

    ''' <summary>
    ''' Class used to represent the result of a call to CreditorPayments.GetPagedGenericRemittancesResult
    ''' </summary>
    ''' <history>
    ''' Paul Wheaver   D11968 Created 17/03/2011
    ''' </history>
    Public Class CreditorPayments_GetPagedGenericRemittancesResult

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _PagingLinks As String = String.Empty
        Private _Items As List(Of ViewableGenericRemittance) = Nothing

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
        Public Property Items() As List(Of ViewableGenericRemittance)
            Get
                Return _Items
            End Get
            Set(ByVal value As List(Of ViewableGenericRemittance))
                _Items = value
            End Set
        End Property

#End Region

    End Class

End Namespace


