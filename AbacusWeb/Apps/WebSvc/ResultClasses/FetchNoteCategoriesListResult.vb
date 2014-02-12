Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.notes
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections

Namespace Apps.WebSvc

    ''' <summary>
    ''' Class used to represent the result of a call to Notes.GetPagedNotesResult
    ''' </summary>
    ''' <history>
    ''' MoTahir   D11971 Created 08/04/2011
    ''' </history>
    Public Class FetchNoteCategoriesListResult

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _Items As List(Of ViewableNoteCategory) = Nothing

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
        Public Property Items() As List(Of ViewableNoteCategory)
            Get
                Return _Items
            End Get
            Set(ByVal value As List(Of ViewableNoteCategory))
                _Items = value
            End Set
        End Property

#End Region

    End Class

End Namespace


