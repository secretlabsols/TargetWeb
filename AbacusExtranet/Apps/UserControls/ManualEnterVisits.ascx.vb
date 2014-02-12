Imports Target.Abacus.Library.eInvoice

Partial Public Class ManualEnterVisits
    Inherits System.Web.UI.UserControl


#Region " Properties "
    Private _careProviderObjectIndex As Integer
    Public Property CareProviderObjectIndexd() As Integer
        Get
            Return _careProviderObjectIndex
        End Get
        Set(ByVal value As Integer)
            _careProviderObjectIndex = value
        End Set
    End Property

    Private _cProvider As CareProvider
    Public Property cProvider() As CareProvider
        Get
            If Not Session("CareProvider" & CareProviderObjectIndexd) Is Nothing Then
                _cProvider = Session("CareProvider" & CareProviderObjectIndexd)
            End If
            Return _cProvider
        End Get
        Set(ByVal value As CareProvider)
            Session.Add("CareProvider" & CareProviderObjectIndexd, value)
            _cProvider = value
        End Set
    End Property

#End Region



    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


   

End Class