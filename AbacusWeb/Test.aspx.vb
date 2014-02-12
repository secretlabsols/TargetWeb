
Imports System.Configuration.ConfigurationManager
Imports Target.Library.Web.UserControls

Partial Public Class Test
    Inherits Target.Web.Apps.BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitPage(-1, "Test Page", AppSettings("ConnectionString"))

        Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

        With stdBut
            With .SearchBy
                .Add("Description", "Description")
                .Add("Abbreviation", "Abbreviation")
            End With
            .EditableControls = fsControls.Controls
            .GenericFinderTypeID = GenericFinderType.DomServiceType
            AddHandler .FindClicked, AddressOf FindClicked
        End With

    End Sub

    Private Sub FindClicked(ByRef e As FindButtonEventArgs)
        Dim turd As Integer = e.SelectedItemID
    End Sub

End Class