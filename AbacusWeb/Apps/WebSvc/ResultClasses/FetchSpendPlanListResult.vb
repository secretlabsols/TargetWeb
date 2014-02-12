Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses.Collections
''' <summary>
''' Simple class to return the results of the FetchSpendPlanList() method.
''' </summary>
''' <remarks></remarks>
Public Class FetchSpendPlanListResult
    Public ErrMsg As ErrorMessage
    Public SpendPlans As List(Of ViewableSpendPlan)
    Public PagingLinks As String
End Class
