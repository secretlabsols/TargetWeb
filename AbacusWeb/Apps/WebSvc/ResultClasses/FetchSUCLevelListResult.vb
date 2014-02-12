Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.SDS
Imports Target.Abacus.Library.DataClasses.Collections
''' <summary>
''' Simple class to return the results of the FetchSpendPlanList() method.
''' </summary>
''' <remarks></remarks>
Public Class FetchSUCLevelListResult
    Public ErrMsg As ErrorMessage
    Public SUCLists As List(Of ViewableSUCLevel)
    Public PagingLinks As String
End Class
