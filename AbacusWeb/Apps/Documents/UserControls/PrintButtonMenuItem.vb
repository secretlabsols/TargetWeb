
Imports Target.Library.Web.Controls.SearchableMenu

Namespace Apps.Documents.UserControls

    ''' <summary>
    ''' Class used to represent an item within the print button menu.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PrintButtonMenuItem
        Inherits SearchableMenuItem

#Region " Private variables "

        Private _printAll As Boolean
        Private _printNow As Boolean

#End Region

#Region " Properties "

        Public Property printAll() As Boolean
            Get
                Return _printAll
            End Get
            Set(ByVal value As Boolean)
                _printAll = value
            End Set
        End Property

        Public Property printNow() As String
            Get
                Return _printNow
            End Get
            Set(ByVal value As String)
                _printNow = value
            End Set
        End Property

#End Region

    End Class

End Namespace