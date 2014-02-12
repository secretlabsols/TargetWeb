Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.UI.DataVisualization.Charting
Imports Target.Library.Web.Charting

Namespace Apps.Reports.Charting

    ''' <summary>
    ''' Example chart builder class.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  22/09/2010  Created to illustrate how to use the ChartBuilder class.
    ''' </history>
    Public Class ExampleChartBuilder
        Inherits ChartBuilder

        Public Sub New(ByVal conn As SqlConnection, _
                       ByVal chart As Chart)
            MyBase.New(Nothing, chart)
        End Sub

        Protected Overrides Sub BuildChartSeries()
            ' create a new data series
            Dim mySeries As Series = Me.AddSeries("Example Series")

        End Sub

        Protected Overrides Sub PopulateChartData(ByRef seriesList As System.Web.UI.DataVisualization.Charting.SeriesCollection)
            Dim data As Dictionary(Of String, Decimal)
            Dim mySeries As Series = seriesList("Example Series")

            ' generate example data
            data = New Dictionary(Of String, Decimal)
            With data
                .Add("Chai", 4887.0)
                .Add("Chang", 7039.0)
                .Add("Chartreuse verte", 4476.0)
                .Add("Côte de Blaye", 49198.0)
                .Add("Guaraná Fantástica", 1630.0)
                .Add("Ipoh Coffee", 11070.0)
                .Add("Lakkalikööri", 7379.0)
                .Add("Laughing Lumberjack Lager", 910.0)
                .Add("Outback Lager", 5468.0)
                .Add("Rhönbräu Klosterbier", 4486.0)
                .Add("Sasquatch Ale", 2107.0)
                .Add("Steeleye Stout", 5275.0)
            End With

            ' populate the series with data
            With mySeries
                ' bind the data
                For Each key As String In data.Keys
                    .Points.AddXY(key, data.Item(key))
                Next
            End With

        End Sub

        Protected Overrides Sub CustomiseChartTitle(ByVal chartTitle As Title)
            chartTitle.Text = "Example Chart - Beverage Sales"
        End Sub

    End Class

End Namespace

