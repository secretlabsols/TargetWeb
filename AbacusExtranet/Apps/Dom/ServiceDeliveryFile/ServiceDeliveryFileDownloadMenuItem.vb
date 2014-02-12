
Imports Target.Library.Web.Controls.SearchableMenu

Namespace Apps.Dom.ServiceDeliveryFile

    ''' <summary>
    ''' Class used to represent an item within the copy invoice button menu.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ServiceDeliveryFileDownloadMenuItem
        Inherits SearchableMenuItem

#Region " Private variables "

        Private _fileID As Integer

#End Region

#Region " Properties "

        Public Property fileID() As Integer
            Get
                Return _fileID
            End Get
            Set(ByVal value As Integer)
                _fileID = value
            End Set
        End Property

#End Region

    End Class

End Namespace