Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library

Namespace Apps.WebSvc

    ''' <summary>
    ''' Simple class to represent the results of SUCLevels.FetchContributionMonitorSummaryResult function
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D11852C Created 14/12/2010 Created
    ''' </history>
    Public Class SUCLevels_FetchContributionMonitorSummaryResult

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _PagingLinks As String = String.Empty
        Private _Results As List(Of ViewableContributionMonitorSummary) = Nothing

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
        ''' Gets or sets the results.
        ''' </summary>
        ''' <value>The services.</value>
        Public Property Results() As List(Of ViewableContributionMonitorSummary)
            Get
                Return _Results
            End Get
            Set(ByVal value As List(Of ViewableContributionMonitorSummary))
                _Results = value
            End Set
        End Property

#End Region

    End Class

End Namespace

