
Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses.Collections
''' <summary>
''' Simple class to return the results of the FetchServiceOrderList() method.
''' </summary>
''' <remarks></remarks>
Public Class FetchServiceOrderListResult
    Public ErrMsg As ErrorMessage
    Public Orders As List(Of ViewableServiceOrder)
    Public PagingLinks As String
End Class

