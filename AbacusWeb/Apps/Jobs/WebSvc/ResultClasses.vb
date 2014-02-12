
Imports Target.Library
Imports Target.Abacus.Library.DataClasses.Collections

Namespace Apps.Jobs.WebSvc

#Region " FetchJobListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.Jobs.WebSvc.FetchJobListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provide results for the FetchJobList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	24/01/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchJobListResult
        Public ErrMsg As ErrorMessage
        Public Jobs As vwJobListCollection
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchJobStepListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.Jobs.WebSvc.FetchJobStepListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provide results for the FetchJobStepListResult() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	24/01/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchJobStepListResult
        Public ErrMsg As ErrorMessage
        Public Steps As vwJobStepListCollection
    End Class

#End Region

#Region " StringResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.Jobs.WebSvc.StringResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provide results for the methods that return a string.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	30/01/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class StringResult
        Public ErrMsg As ErrorMessage
        Public Data As String
    End Class

#End Region

#Region " AboutResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.Jobs.WebSvc.AboutResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class used to pass back application information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	25/05/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AboutResult
        Public ErrMsg As ErrorMessage
        Public AppInfo As ApplicationInfo
    End Class

#End Region

End Namespace