Imports Target.Library

''' <summary>
''' Simple class to return the results of the GetPaymentsInBudgetPeriods() method.
''' </summary>
''' <remarks></remarks>
Public Class GetPaymentsInBudgetPeriodsResult
    Public ErrMsg As ErrorMessage
    Public CurrentPeriod As String
    Public CurrentPaymentsMade As String
    Public currentPaymentsToBePaid As String
    Public NextPeriod As String
    Public NextPaymentsMade As String
    Public NextPaymentsToBePaid As String
    Public OvelappingPayments As String
End Class