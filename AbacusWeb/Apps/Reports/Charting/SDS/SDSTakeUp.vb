Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.UI.DataVisualization.Charting
Imports Target.Library.Web.Charting
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.Reports.Charting

    ''' <summary>
    ''' Example chart builder class.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' PaulW  10/10/2010  Initial Version D11905.
    ''' </history>
    Public Class SDSTakeUp
        Inherits ChartBuilder

        Public Sub New(ByVal conn As SqlConnection, _
                       ByVal chart As Chart)
            MyBase.New(conn, chart)

        End Sub


        Protected Overrides Sub CustomiseChartLegend(ByVal chartLegend As System.Web.UI.DataVisualization.Charting.Legend)
            With chartLegend
                'Set legend style
                .LegendStyle = LegendStyle.Table
                ' Set table style if legend style is Table
                .TableStyle = LegendTableStyle.Auto
                ' Set legend docking
                .Docking = Docking.Top
                ' Set legend alignment
                .Alignment = StringAlignment.Center
                ' Set Legend back colour
                .BackColor = Color.Transparent
                'Set the legend font
                .Font = New Font("Arial", 14, FontStyle.Bold)
            End With
        End Sub

        Protected Overrides Sub BuildChartSeries()
            
            'We dont want a 3D chart
            Chart.ChartAreas.Single.Area3DStyle.Enable3D = False
            Chart.ChartAreas.Single.AxisX.Interval = 1

            Dim SDSOnly As Series = Me.AddSeries("SDS Care Only", "Default", SeriesChartType.Column)
            SDSOnly.Color = Color.MediumSeaGreen
            SDSOnly.BackSecondaryColor = Color.Green
            SDSOnly.BackGradientStyle = GradientStyle.DiagonalLeft
            SDSOnly("PointWidth") = "1"
            SDSOnly("BarLabelStyle") = "Center"
            SDSOnly("DrawingStyle") = "Cylinder"

            Dim ConvSDS As Series = Me.AddSeries("Converted to SDS", "Default", SeriesChartType.Column)
            ConvSDS("PointWidth") = "1"
            ConvSDS("BarLabelStyle") = "Center"
            ConvSDS("DrawingStyle") = "Cylinder"

            Dim NonSDSOnly As Series = Me.AddSeries("Non-SDS Care Only", "Default", SeriesChartType.Column)
            NonSDSOnly("PointWidth") = "1"
            NonSDSOnly("BarLabelStyle") = "Center"
            NonSDSOnly("DrawingStyle") = "Cylinder"

            Dim ConvNonSDS As Series = Me.AddSeries("Converted to Non-SDS", "Default", SeriesChartType.Column)
            ConvNonSDS("PointWidth") = "1"
            ConvNonSDS("BarLabelStyle") = "Center"
            ConvNonSDS("DrawingStyle") = "Cylinder"

        End Sub

        Protected Overrides Sub PopulateChartData(ByRef seriesList As System.Web.UI.DataVisualization.Charting.SeriesCollection)
            Dim dateFrom As Date = Nothing
            Dim dateTo As Date = Nothing
            Dim showPermRes As Nullable(Of Boolean) = Nothing
            Dim dtSDSCareOnly As DataTable = Nothing
            Dim dtNonSDSCareOnly As DataTable = Nothing
            Dim dtConvToSDS As DataTable = Nothing
            Dim dtConvNonSDS As DataTable = Nothing
            Dim msg As ErrorMessage = Nothing
            Dim SDSOnly As Series = seriesList("SDS Care Only")
            Dim ConvSDS As Series = seriesList("Converted to SDS")
            Dim NonSDSOnly As Series = seriesList("Non-SDS Care Only")
            Dim ConvNonSDS As Series = seriesList("Converted to Non-SDS")

            If Not Me.queryString("dateFrom") Is Nothing And Not Me.queryString("dateFrom") = "" Then
                dateFrom = Me.queryString("dateFrom")
            End If
            If Not Me.queryString("dateTo") Is Nothing And Not Me.queryString("dateTo") = "" Then
                dateTo = Me.queryString("dateTo")
            End If

            If Not Me.queryString("ShowPermRes") Is Nothing And Not Me.queryString("ShowPermRes") = "" Then
                showPermRes = Me.queryString("ShowPermRes")
            End If

            msg = FetchReportData(Me.DbConnection, dateFrom, dateTo, showPermRes, _
                                  dtSDSCareOnly, dtConvToSDS, dtNonSDSCareOnly, dtConvNonSDS)
            If msg.Success Then
                Dim yValue As Integer = 0
                Dim tmpDate As Date = dateFrom

                Do While tmpDate <= dateTo
                    yValue = 0
                    For Each row As DataRow In dtSDSCareOnly.Rows
                        If row("Month") = tmpDate.Month And row("Year") = tmpDate.Year Then
                            yValue = row("Number")
                            Exit For
                        End If
                    Next
                    SDSOnly.Points.AddXY(String.Format("{0}-{1}", MonthName(tmpDate.Month, True), tmpDate.Year), yValue)

                    yValue = 0
                    For Each row As DataRow In dtConvToSDS.Rows
                        If row("Month") = tmpDate.Month And row("Year") = tmpDate.Year Then
                            yValue = row("Number")
                            Exit For
                        End If
                    Next
                    ConvSDS.Points.AddXY(String.Format("{0}-{1}", MonthName(tmpDate.Month, True), tmpDate.Year), yValue)

                    yValue = 0
                    For Each row As DataRow In dtNonSDSCareOnly.Rows
                        If row("Month") = tmpDate.Month And row("Year") = tmpDate.Year Then
                            yValue = row("Number")
                            Exit For
                        End If
                    Next
                    NonSDSOnly.Points.AddXY(String.Format("{0}-{1}", MonthName(tmpDate.Month, True), tmpDate.Year), yValue)

                    yValue = 0
                    For Each row As DataRow In dtConvNonSDS.Rows
                        If row("Month") = tmpDate.Month And row("Year") = tmpDate.Year Then
                            yValue = row("Number")
                            Exit For
                        End If
                    Next
                    ConvNonSDS.Points.AddXY(String.Format("{0}-{1}", MonthName(tmpDate.Month, True), tmpDate.Year), yValue)

                    'Move the tmp date on by a month
                    tmpDate = tmpDate.AddMonths(1)
                Loop

                'Turn on Data labels
                Me.ApplyDataPointLabels()

            End If

        End Sub

        Protected Overrides Sub CustomiseChartTitle(ByVal chartTitle As Title)
            chartTitle.Text = "SDS Take-up"
        End Sub

        Private Shared Function FetchReportData(ByVal conn As SqlConnection, _
                                                ByVal dateFrom As Date, _
                                                ByVal dateTo As Date, _
                                                ByVal showPermRes As Nullable(Of Boolean), _
                                                ByRef dtSDSCareOnly As DataTable, _
                                                ByRef dtConvToSDS As DataTable, _
                                                ByRef dtNonSDSCareOnly As DataTable, _
                                                ByRef dtConvNonSDS As DataTable) As ErrorMessage

            Const SP_NAME As String = "spxReport_SDSTakeUpForChart"

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spParams As SqlParameter()

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(conn, SP_NAME, False)
                If Utils.IsDate(dateFrom) Then spParams(0).Value = dateFrom
                If Utils.IsDate(dateTo) Then spParams(1).Value = dateTo
                If showPermRes.HasValue Then spParams(2).Value = showPermRes

                Dim dataDataset As DataSet = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, SP_NAME, spParams)

                dtSDSCareOnly = dataDataset.Tables(0)
                dtConvToSDS = dataDataset.Tables(1)
                dtNonSDSCareOnly = dataDataset.Tables(2)
                dtConvNonSDS = dataDataset.Tables(3)

                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            End Try

            Return msg

        End Function


    End Class

End Namespace
