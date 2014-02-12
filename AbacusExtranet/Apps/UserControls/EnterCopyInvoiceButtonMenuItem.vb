
Imports Target.Library.Web.Controls.SearchableMenu

Namespace Apps.UserControls

    ''' <summary>
    ''' Class used to represent an item within the copy invoice button menu.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EnterCopyInvoiceButtonMenuItem
        Inherits SearchableMenuItem

#Region " Private variables "

        Private _createNew As Boolean

#End Region

#Region " Properties "

        Public Property createNew() As Boolean
            Get
                Return _createNew
            End Get
            Set(ByVal value As Boolean)
                _createNew = value
            End Set
        End Property

#End Region

    End Class

End Namespace