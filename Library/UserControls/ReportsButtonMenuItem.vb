
Imports Target.Library.Web.Controls.SearchableMenu

Namespace UserControls

    ''' <summary>
    ''' Class used to represent an item within the reports button menu.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ReportsButtonMenuItem
        Inherits SearchableMenuItem

#Region " Private variables "

        Private _url As String

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets the url that the menu item will invoke.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property url() As String
            Get
                Return _url
            End Get
            Set(ByVal value As String)
                _url = value
            End Set
        End Property

#End Region

    End Class

End Namespace