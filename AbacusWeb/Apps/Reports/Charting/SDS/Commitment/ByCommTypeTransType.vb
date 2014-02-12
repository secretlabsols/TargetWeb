Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.UI.DataVisualization.Charting
Imports Target.Library.Web.Charting
Imports Target.Library
Imports Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.Reports.Charting

    ''' <summary>
    ''' SDS Profile Chart.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' PaulW  10/10/2010  Initial Version D11905.
    ''' </history>
    Public Class ByCommTypeTransType
        Inherits ChartBuilder

        Private Const FORECASTED_COMMISSIONED_SERVICE As Int16 = 8
        Private Const FORECASTED_DIRECT_PAYMENT As Int16 = 4
        Private Const ACTUAL_COMMISSIONED_SERVICE As Int16 = 512
        Private Const ACTUAL_DIRECT_PAYMENT As Int16 = 256

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
                .TableStyle = LegendTableStyle.Tall
                ' Set legend docking
                .Docking = Docking.Bottom
                ' Set legend alignment
                '.Alignment = StringAlignment.Center
                ' Set Legend back colour
                .BackColor = Color.Transparent
                'Set the legend font
                .Font = New Font("Arial", 10, FontStyle.Bold)

                .BorderColor = Color.Black
                .BorderWidth = 1
                .BorderDashStyle = ChartDashStyle.Solid
                .ShadowOffset = 2


                Select Case .Name
                    Case "Default"
                        .Title = "Transaction Types"
                        'position the legend here
                        .Position = New ElementPosition(60, 67, 35, 17)
                        .IsTextAutoFit = True
                        .TextWrapThreshold = 100
                    Case "outerLegend"
                        .Title = "Commitment Type"
                        .TableStyle = LegendTableStyle.Tall
                        'position the legend here
                        .Position = New ElementPosition(5, 67, 35, 10)
                        .IsTextAutoFit = True
                        .TextWrapThreshold = 100
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
            Dim DateFrom As Date
            Dim dateTo As Date

            If Not Me.queryString("dateFrom") Is Nothing And Not Me.queryString("dateFrom") = "" Then
                DateFrom = Me.queryString("dateFrom")
            End If

            If Not Me.queryString("dateTo") Is Nothing And Not Me.queryString("dateTo") = "" Then
                dateTo = Me.queryString("dateTo")
            End If

            msg = FetchReportData(Me.DbConnection, DateFrom, dateTo, dtData)
            If msg.Success Then

                For Each row As DataRow In dtData.Rows

                    'populate the outer pie chart
                    outer.Points.AddXY("Commissioned Service", row("CommissionedService"))
                    outer.Points(0).Color = Color.FromArgb(255, 65, 140, 240) 'Blue
                    outer.Points(0)("PieLabelStyle") = "Disabled"
                    outer.Points(0).Url = "javascript:CommitmentProfile_CommSvc();"
                    outer.Points(0).ToolTip = String.Format("{0} represents {1} of the commitment for the period {2} - {3}, totalling {4}", _
                                                            "#VALX", "#PERCENT{P2}", _
                                                            DateFrom.ToShortDateString, dateTo.ToShortDateString, "#VALY{C}")
                    outer.Points(0).LegendToolTip = outer.Points(0).ToolTip
                    outer.Points(0).LegendUrl = outer.Points(0).Url

                    outer.Points.AddXY("Direct Payments", row("DirectPayment"))
                    outer.Points(1).Color = Color.FromArgb(255, 224, 64, 10) 'Red
                    outer.Points(1).Url = "javascript:CommitmentProfile_DirectPayment();"
                    outer.Points(1)("PieLabelStyle") = "Disabled"
                    outer.Points(1).ToolTip = _
                        String.Format("{0} represents {1} of the commitment for the period {2} - {3}, totalling {4}", _
                                                            "#VALX", "#PERCENT{P2}", _
                                                            DateFrom.ToShortDateString, dateTo.ToShortDateString, "#VALY{C}")
                    outer.Points(1).LegendToolTip = outer.Points(1).ToolTip
                    outer.Points(1).LegendUrl = outer.Points(1).Url

                    'populate the middle pie chart
                    inner.Points.AddXY("Actual Commissioned Service", row("ActualCommissionedService"))
                    inner.Points(0).Color = Color.FromArgb(255, 134, 185, 252)
                    inner.Points(0)("PieLabelStyle") = "Disabled"
                    inner.Points(0).Url = "javascript:CommitmentProfile_ActualCommSvc();"
                    inner.Points(0).ToolTip = _
                        String.Format("{0} represents {1} of the commitment for the period {2} - {3}, totalling {4}", _
                                                            "#VALX", "#PERCENT{P2}", _
                                                            DateFrom.ToShortDateString, dateTo.ToShortDateString, "#VALY{C}")
                    inner.Points(0).LegendToolTip = inner.Points(0).ToolTip
                    inner.Points(0).LegendUrl = inner.Points(0).Url

                    inner.Points.AddXY("Forecasted Commissioned Service", row("ForecastedCommissionedService"))
                    inner.Points(1).Color = Color.FromArgb(255, 175, 210, 255)
                    inner.Points(1)("PieLabelStyle") = "Disabled"
                    inner.Points(1).Url = "javascript:CommitmentProfile_ForecastedCommSvc();"
                    inner.Points(1).ToolTip = _
                    String.Format("{0} represents {1} of the commitment for the period {2} - {3}, totalling {4}", _
                                                            "#VALX", "#PERCENT{P2}", _
                                                            DateFrom.ToShortDateString, dateTo.ToShortDateString, "#VALY{C}")
                    inner.Points(1).LegendToolTip = inner.Points(1).ToolTip
                    inner.Points(1).LegendUrl = inner.Points(1).Url

                    inner.Points.AddXY("Actual Direct Payments", row("ActualDirectPayment"))
                    inner.Points(2).Color = Color.FromArgb(255, 238, 121, 81) 'Middle Red
                    inner.Points(2)("PieLabelStyle") = "Disabled"
                    inner.Points(2).Url = "javascript:CommitmentProfile_ActualDirectPayment();"
                    inner.Points(2).ToolTip = _
                        String.Format("{0} represents {1} of the commitment for the period {2} - {3}, totalling {4}", _
                                                            "#VALX", "#PERCENT{P2}", _
                                                            DateFrom.ToShortDateString, dateTo.ToShortDateString, "#VALY{C}")
                    inner.Points(2).LegendToolTip = inner.Points(2).ToolTip
                    inner.Points(2).LegendUrl = inner.Points(2).Url

                    inner.Points.AddXY("Forecasted Direct Payments", row("ForecastedDirectPayment"))
                    inner.Points(3).Color = Color.FromArgb(255, 246, 169, 143)
                    inner.Points(3)("PieLabelStyle") = "Disabled"
                    inner.Points(3).Url = "javascript:CommitmentProfile_ForecastedDirectPayment();"
                    inner.Points(3).ToolTip = _
                        String.Format("{0} represents {1} of the commitment for the period {2} - {3}, totalling {4}", _
                                                            "#VALX", "#PERCENT{P2}", _
                                                            DateFrom.ToShortDateString, dateTo.ToShortDateString, "#VALY{C}")
                    inner.Points(3).LegendToolTip = inner.Points(3).ToolTip
                    inner.Points(3).LegendUrl = inner.Points(3).Url

                    'Populate the inner pie chart
                    center.Points.AddXY("All Service Users", row("AllTypes"))
                    center.Points(0).Color = Color.Wheat
                    center.Points(0)("PieLabelStyle") = "Disabled"
                    center.Points(0).Url = "javascript:CommitmentProfile_All();"
                    center.Points(0).ToolTip = _
                        String.Format("The total commitment for the period {0} - {1} totals {2}", _
                                      DateFrom.ToShortDateString, dateTo.ToShortDateString, "#VALY{C}")
                    center.Points(0).LegendToolTip = center.Points(0).ToolTip
                    center.Points(0).LegendUrl = center.Points(0).Url
                Next
            End If
        End Sub

        Protected Overrides Sub CustomiseChartTitle(ByVal chartTitle As Title)
            chartTitle.Text = ""
        End Sub

        Private Shared Function FetchReportData(ByVal conn As SqlConnection, _
                                                ByVal dateFrom As Date, _
                                                ByVal dateTo As Date, _
                                                ByRef dtData As DataTable) As ErrorMessage

            Const SP_NAME As String = "spxChart_CommitmentByCommTypeTransType"

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spParams As SqlParameter()

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(conn, SP_NAME, False)
                If IsDate(dateFrom) Then spParams(0).Value = dateFrom
                If IsDate(dateTo) Then spParams(1).Value = dateTo

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


