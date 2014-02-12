Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses.Collections

''' <summary>
''' Simple class to return the results of the FetchDomContractList() method.
''' </summary>
''' <remarks></remarks>
Public Class FetchGenericContractListResult

#Region "Fields"

    Private _Contracts As New List(Of CreditorPayments.ViewableGenericContract)()
    Private _ErrMsg As ErrorMessage = Nothing
    Private _PagingLinks As String = String.Empty

#End Region

#Region "Constructors"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="FetchGenericContractListResult" /> class.
    ''' </summary>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "Properties"

    ''' <summary>
    ''' Gets or sets the contracts.
    ''' </summary>
    ''' <value>The contracts.</value>
    Public Property Contracts() As List(Of CreditorPayments.ViewableGenericContract)
        Get
            Return _Contracts
        End Get
        Set(ByVal value As List(Of CreditorPayments.ViewableGenericContract))
            _Contracts = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the error message.
    ''' </summary>
    ''' <value>The error message.</value>
    Public Property ErrMsg() As ErrorMessage
        Get
            Return _ErrMsg
        End Get
        Set(ByVal value As ErrorMessage)
            _ErrMsg = value
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