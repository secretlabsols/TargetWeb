Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI.DataVisualization.Charting
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Charting
Imports Target.Web.Apps.Security

Namespace Apps.Reports.Charting

    ''' <summary>
    ''' Http handler used to generate Intranet report charts as a PNG image.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  22/09/2010  Created.
    ''' </history>
    Public Class ReportChartHandler
        Implements IHttpHandler

        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest

            Dim chartType As ReportChartType
            Dim chartHeight As Integer
            Dim chartWidth As Integer
            Dim theChart As Chart
            Dim builder As ChartBuilder
            Dim imageData As MemoryStream
            Dim conn As SqlConnection = Nothing

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get querystring parameters
                chartType = Target.Library.Utils.ToInt32(context.Request.QueryString("id"))
                chartHeight = Target.Library.Utils.ToInt32(context.Request.QueryString("h"))
                chartWidth = Target.Library.Utils.ToInt32(context.Request.QueryString("w"))

                ' create the chart with default size
                theChart = ReportChartFactory.GetChart()

                ' create the chart builder
                builder = ReportChartFactory.GetChartBuilder(theChart, chartType, conn)

                ' change chart size?
                With theChart
                    If chartHeight > 0 Then .Height = chartHeight
                    If chartWidth > 0 Then .Width = chartWidth
                End With

                ' generate the chart
                builder.BuildChart(context.Request.QueryString)

                ' get the chart data as a PNG image
                imageData = New MemoryStream()
                theChart.SaveImage(imageData, ChartImageFormat.Png)
                imageData.Position = 0

                ' send the response
                With context.Response
                    .Clear()
                    .ContentType = "image/png"
                    .AddHeader("Content-Length", imageData.Length)
                    .AddHeader("Content-Disposition", "inline; filename=ChartImage.png")
                    .BinaryWrite(imageData.ToArray())
                End With

            Catch ex As Exception
                Throw
            Finally
                SqlHelper.CloseConnection(conn)
            End Try
        End Sub

        Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

    End Class

End Namespace

