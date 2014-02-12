Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.UI.DataVisualization.Charting
Imports Target.Library.Web.Charting
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.Reports.Charting

    ''' <summary>
    ''' SDS Profile Chart.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' PaulW  10/10/2010  Initial Version D11905.
    ''' </history>
    Public Class SDSProfile
        Inherits ChartBuilder

        Public Sub New(ByVal conn As SqlConnection, _
                       ByVal chart As Chart)
            MyBase.New(conn, chart)
        End Sub

        Protected Overrides Sub BuildChartAreas()
            Me.AddChartArea("outer")
            Me.AddChartArea("inner")
            Me.AddChartArea("center")
        End Sub

        Protected Overrides Sub CustomiseChartLegend(ByVal chartLegend As System.Web.UI.DataVisualization.Charting.Legend)
            With chartLegend
                '.Enabled = False
                'Set legend style
                .LegendStyle = LegendStyle.Column
                ' Set table style if legend style is Table
                .TableStyle = LegendTableStyle.Auto
                ' Set legend docking
                .Docking = Docking.Bottom
                ' Set legend alignment
                .Alignment = StringAlignment.Center
                ' Set Legend back colour
                .BackColor = Color.Transparent
                'Set the legend font
                .Font = New Font("Arial", 12, FontStyle.Bold)

                .BorderColor = Color.Black
                .BorderWidth = 1
                .BorderDashStyle = ChartDashStyle.Solid
                .ShadowOffset = 2


                Select Case .Name
                    Case "Default"
                        .Title = "Inner Section"
                        .TableStyle = LegendTableStyle.Tall
                        'position the legend here
                        .Position = New ElementPosition(60, 67, 35, 20)

                    Case "outerLegend"
                        .Title = "Outer Section"
                        .TableStyle = LegendTableStyle.Tall
                        'position the legend here
                        .Position = New ElementPosition(5, 67, 30, 12)
                    Case "centerLegend"
                        .Title = "Center "
                End Select


            End With
        End Sub

        Protected Overrides Sub CustomiseChartArea(ByVal area As System.Web.UI.DataVisualization.Charting.ChartArea)
            'We dont want a 3D chart
            area.Area3DStyle.Enable3D = False

            If area.Name = "outer" Then
                area.BackColor = Color.Transparent
                area.InnerPlotPosition.Auto = False
                area.InnerPlotPosition.Height = 80
                area.InnerPlotPosition.Width = 80
                area.InnerPlotPosition.X = 0
                area.InnerPlotPosition.Y = 0
                area.Position.X = 20
                area.Position.Y = 2
                area.Position.Width = 80
                area.Position.Height = 80
            End If

            If area.Name = "inner" Then
                area.BackColor = Color.Transparent
                area.InnerPlotPosition.Auto = False
                area.InnerPlotPosition.Height = 80
                area.InnerPlotPosition.Width = 80
                area.InnerPlotPosition.X = 0
                area.InnerPlotPosition.Y = 0
                area.Position.X = 32
                area.Position.Y = 14.5
                area.Position.Width = 50
                area.Position.Height = 49
            End If

            If area.Name = "center" Then
                area.BackColor = Color.Transparent
                area.InnerPlotPosition.Auto = False
                area.InnerPlotPosition.Height = 80
                area.InnerPlotPosition.Width = 80
                area.InnerPlotPosition.X = 0
                area.InnerPlotPosition.Y = 0
                area.Position.X = 46
                area.Position.Y = 28
                area.Position.Width = 15
                area.Position.Height = 15
            End If

        End Sub

        Protected Overrides Sub BuildChartSeries()
            Me.AddLegend("outerLegend")
            Dim outerSeries As Series = Me.AddSeries("Outer", "outer", SeriesChartType.Doughnut)
            outerSeries("DoughnutRadius") = 40
            outerSeries("PieDrawingStyle") = "Concave"
            outerSeries.Label = "#VALX (#PERCENT{P2})"
            'outerSeries("PieLabelStyle") = "outside"
            outerSeries("PieLineColor") = "Black"
            outerSeries.Legend = "outerLegend"
            'Me.Chart.Legends("outerLegend").Enabled = False


            Dim innerSeries As Series = Me.AddSeries("Inner", "inner", SeriesChartType.Doughnut)
            'innerSeries("PieLabelStyle") = "outside"
            innerSeries("PieDrawingStyle") = "SoftEdge"
            innerSeries.Label = "#VALX (#PERCENT{P2})"
            innerSeries("DoughnutRadius") = 70
            innerSeries.Legend = "Default"


            Me.AddLegend("centerLegend")
            Dim centerSeries As Series = Me.AddSeries("Center", "center", SeriesChartType.Pie)
            centerSeries.Legend = "centerLegend"
            Me.Chart.Legends("centerLegend").Enabled = False

        End Sub

        Protected Overrides Sub PopulateChartData(ByRef seriesList As System.Web.UI.DataVisualization.Charting.SeriesCollection)
            Dim showPermRes As Nullable(Of Boolean) = Nothing
            Dim dtData As DataTable = Nothing
            Dim msg As ErrorMessage = Nothing
            Dim inner As Series = seriesList("Inner")
            Dim outer As Series = seriesList("Outer")
            Dim center As Series = seriesList("Center")

            If Not Me.queryString("ShowPermRes") Is Nothing And Not Me.queryString("ShowPermRes") = "" Then
                showPermRes = Me.queryString("ShowPermRes")
            End If

            msg = FetchReportData(Me.DbConnection, showPermRes, dtData)
            If msg.Success Then

                'populate the outer pie chart
                outer.Points.AddXY("Non-SDS", dtData.Rows(0)("NonSDSUsers"))
                outer.Points(0).Color = Color.FromArgb(255, 65, 140, 240) 'Blue
                outer.Points(0).Url = "javascript:SDSProfile_NonSDS();"
                outer.Points(0)("PieLabelStyle") = "Disabled"
                outer.Points(0).ToolTip = _
                    String.Format("There are #VALY active #VALX service users{0}Representing {1} of the active service users.{0}{0}Click here to view the underlying data.", _
                                  ControlChars.Lf, "#PERCENT{P2}")
                outer.Points(0).LegendToolTip = outer.Points(0).ToolTip
                outer.Points(0).LegendUrl = outer.Points(0).Url

                outer.Points.AddXY("SDS", dtData.Rows(0)("SDSUsers"))
                outer.Points(1).Color = Color.FromArgb(255, 224, 64, 10) 'Red
                outer.Points(1).Url = "javascript:SDSProfile_SDS();"
                outer.Points(1)("PieLabelStyle") = "Disabled"
                outer.Points(1).ToolTip = _
                    String.Format("There are #VALY active #VALX service users{0}Representing {1} of the active service users.{0}{0}Click here to view the underlying data.", _
                                  ControlChars.Lf, "#PERCENT{P2}")
                outer.Points(1).LegendToolTip = outer.Points(1).ToolTip
                outer.Points(1).LegendUrl = outer.Points(1).Url

                'populate the middle pie chart
                inner.Points.AddXY("Non-SDS Only", dtData.Rows(0)("NonSDS"))
                inner.Points(0).Color = Color.FromArgb(255, 134, 185, 252)
                inner.Points(0)("PieLabelStyle") = "Disabled"
                inner.Points(0).Url = "javascript:SDSProfile_NonSDSOnly();"
                inner.Points(0).ToolTip = _
                    String.Format("There are #VALY active #VALX service users{0}Representing {1} of the active service users.{0}{0}Click here to view the underlying data.", _
                                  ControlChars.Lf, "#PERCENT{P2}")
                inner.Points(0).LegendToolTip = inner.Points(0).ToolTip
                inner.Points(0).LegendUrl = inner.Points(0).Url

                inner.Points.AddXY("Converted Non-SDS", dtData.Rows(0)("ConvertedNonSDS"))
                inner.Points(1).Color = Color.FromArgb(255, 175, 210, 255)
                inner.Points(1)("PieLabelStyle") = "Disabled"
                inner.Points(1).Url = "javascript:SDSProfile_ConvertedNonSDS();"
                inner.Points(1).ToolTip = _
                    String.Format("There are #VALY active #VALX service users{0}Representing {1} of the active service users.{0}{0}Click here to view the underlying data.", _
                                  ControlChars.Lf, "#PERCENT{P2}")
                inner.Points(1).LegendToolTip = inner.Points(1).ToolTip
                inner.Points(1).LegendUrl = inner.Points(1).Url

                inner.Points.AddXY("SDS Only", dtData.Rows(0)("SDSOnly"))
                inner.Points(2).Color = Color.FromArgb(255, 238, 121, 81) 'Middle Red
                inner.Points(2)("PieLabelStyle") = "Disabled"
                inner.Points(2).Url = "javascript:SDSProfile_SDSOnly();"
                inner.Points(2).ToolTip = _
                    String.Format("There are #VALY active #VALX service users{0}Representing {1} of the active service users.{0}{0}Click here to view the underlying data.", _
                                  ControlChars.Lf, "#PERCENT{P2}")
                inner.Points(2).LegendToolTip = inner.Points(2).ToolTip
                inner.Points(2).LegendUrl = inner.Points(2).Url

                inner.Points.AddXY("Converted SDS", dtData.Rows(0)("ConvertedSDS"))
                inner.Points(3).Color = Color.FromArgb(255, 246, 169, 143)
                inner.Points(3)("PieLabelStyle") = "Disabled"
                inner.Points(3).Url = "javascript:SDSProfile_ConvertedSDS();"
                inner.Points(3).ToolTip = _
                    String.Format("There are #VALY active #VALX service users{0}Representing {1} of the active service users.{0}{0}Click here to view the underlying data.", _
                                  ControlChars.Lf, "#PERCENT{P2}")
                inner.Points(3).LegendToolTip = inner.Points(3).ToolTip
                inner.Points(3).LegendUrl = inner.Points(3).Url

                'Populate the inner pie chart
                center.Points.AddXY("All Service Users", dtData.Rows(0)("AllActiveUsers"))
                center.Points(0).Color = Color.Wheat
                center.Points(0)("PieLabelStyle") = "Disabled"
                center.Points(0).Url = "javascript:SDSProfile_AllSvcUsers();"
                center.Points(0).ToolTip = _
                    String.Format("There are currently #VALY active service users{0}{0}Click here to view the underlying data.", _
                                  ControlChars.Lf)
                center.Points(0).LegendToolTip = center.Points(0).ToolTip
                center.Points(0).LegendUrl = center.Points(0).Url

            End If
        End Sub

        Protected Overrides Sub CustomiseChartTitle(ByVal chartTitle As Title)
            chartTitle.Text = ""
        End Sub

        Private Shared Function FetchReportData(ByVal conn As SqlConnection, _
                                                ByVal showPermRes As Nullable(Of Boolean), _
                                                ByRef dtData As DataTable) As ErrorMessage

            Const SP_NAME As String = "spxChart_SDSProfile"

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spParams As SqlParameter()

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(conn, SP_NAME, False)
                If showPermRes.HasValue Then spParams(0).Value = showPermRes

                Dim dataDataset As DataSet = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, SP_NAME, spParams)

                dtData = dataDataset.Tables(0)

                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            End Try

            Return msg

        End Function


    End Class

End Namespace

