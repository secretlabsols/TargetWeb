Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.CreditorPayments

Namespace Apps.WebSvc

    ''' <summary>
    ''' Class used to represent the result of a call to CreditorPayments.GetPagedGenericCreditorPaymentsResult
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D11874 Created 18/02/2010
    ''' </history>
    Public Class CreditorPayments_GetPagedGenericCreditorPaymentsResult

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _Items As List(Of ViewableGenericCreditorPayment) = Nothing
        Private _NumberOfAuthorisedPayments As Integer = 0
        Private _NumberOfPaidPayments As Integer = 0
        Private _NumberOfSuspendedPayments As Integer = 0
        Private _NumberOfUnpaidPayments As Integer = 0
        Private _PagingLinks As String = String.Empty

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
        ''' Gets or sets the items.
        ''' </summary>
        ''' <value>The items.</value>
        Public Property Items() As List(Of ViewableGenericCreditorPayment)
            Get
                Return _Items
            End Get
            Set(ByVal value As List(Of ViewableGenericCreditorPayment))
                _Items = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the number of authorised payments.
        ''' </summary>
        ''' <value>The number of authorised payments.</value>
        Public Property NumberOfAuthorisedPayments() As Integer
            Get
                Return _NumberOfAuthorisedPayments
            End Get
            Set(ByVal value As Integer)
                _NumberOfAuthorisedPayments = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the number of paid payments.
        ''' </summary>
        ''' <value>The number of paid payments.</value>
        Public Property NumberOfPaidPayments() As Integer
            Get
                Return _NumberOfPaidPayments
            End Get
            Set(ByVal value As Integer)
                _NumberOfPaidPayments = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the number of suspended payments.
        ''' </summary>
        ''' <value>The number of suspended payments.</value>
        Public Property NumberOfSuspendedPayments() As Integer
            Get
                Return _NumberOfSuspendedPayments
            End Get
            Set(ByVal value As Integer)
                _NumberOfSuspendedPayments = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the number of unpaid payments.
        ''' </summary>
        ''' <value>The number of unpaid payments.</value>
        Public Property NumberOfUnpaidPayments() As Integer
            Get
                Return _NumberOfUnpaidPayments
            End Get
            Set(ByVal value As Integer)
                _NumberOfUnpaidPayments = value
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

#End Region

    End Class

End Namespace


