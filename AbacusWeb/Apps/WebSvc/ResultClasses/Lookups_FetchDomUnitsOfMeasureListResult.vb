Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses

Namespace Apps.WebSvc

    ''' <summary>
    ''' Simple class to represent the results of Lookups.FetchDomUnitsOfMeasureList function
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D11964A Created 26/11/2010 Created
    ''' </history>
    Public Class Lookups_FetchDomUnitsOfMeasureListResult

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _PagingLinks As String = String.Empty
        Private _UnitsOfMeasure As List(Of DomUnitsOfMeasure) = Nothing

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
        ''' Gets or sets the units of measure.
        ''' </summary>
        ''' <value>The services.</value>
        Public Property UnitsOfMeasure() As List(Of DomUnitsOfMeasure)
            Get
                Return _UnitsOfMeasure
            End Get
            Set(ByVal value As List(Of DomUnitsOfMeasure))
                _UnitsOfMeasure = value
            End Set
        End Property

#End Region

    End Class

End Namespace

