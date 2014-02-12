Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.UI.DataVisualization.Charting
Imports Target.Library.Web.Charting

Namespace Apps.Reports.Charting

    ''' <summary>
    ''' Class used to return ChartBuilder derived instances to allow report charts to be programmatically created.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  22/09/2010  Created.
    ''' </history>
    Public Class ReportChartFactory

        Public Const CHART_DEFAULT_HEIGHT As Integer = 250
        Public Const CHART_DEFAULT_WIDTH As Integer = 250

#Region " Private variables "

        Private Shared _chartTypeMap As Dictionary(Of ReportChartType, Type)

#End Region

#Region " Ctor "

        ''' <summary>
        ''' Shared constructor.
        ''' </summary>
        ''' <remarks>As new report charts are added, load the enum and type into this list to make them available.</remarks>
        Shared Sub New()
            ' build up the list of available chart types vs ChartBuilder classes
            _chartTypeMap = New Dictionary(Of ReportChartType, Type)
            With _chartTypeMap
                .Add(ReportChartType.Example, GetType(ExampleChartBuilder))
                .Add(ReportChartType.SDSTakeUp, GetType(SDSTakeUp))
                .Add(ReportChartType.SDSProfile, GetType(SDSProfile))
                .Add(ReportChartType.SDSCommitmentCommTypeTransType, GetType(ByCommTypeTransType))
                .Add(ReportChartType.SDSCommitmentSvcClassificationSvcType, GetType(BySvcClassificationSvcType))
                .Add(ReportChartType.SDSCommitmentSvcTypeBudgetCategory, GetType(BySvcTypeBudgetCategory))
            End With
        End Sub

#End Region

#Region " GetChart "

        ''' <summary>
        ''' Returns a new Chart instance, primed with the default properties.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetChart() As Chart

            Dim theChart As Chart

            theChart = New Chart()
            ApplyChartDefaults(theChart)

            Return theChart

        End Function

#End Region

#Region " ApplyChartDefaults "
        ''' <summary>
        ''' Apply defaults to the chart
        ''' </summary>
        ''' <param name="theChart"></param>
        ''' <remarks></remarks>
        Public Shared Sub ApplyChartDefaults(ByRef theChart As Chart)
            With theChart
                .Width = CHART_DEFAULT_WIDTH
                .Height = CHART_DEFAULT_HEIGHT
                .BackColor = Color.FromArgb(255, 211, 223, 240)
                .BackGradientStyle = GradientStyle.TopBottom
                .BorderSkin.SkinStyle = BorderSkinStyle.Emboss
            End With
        End Sub

#End Region


#Region " GetChartBuilder "

        ''' <summary>
        ''' Factory method to return a configured ChartBuilder derived class that can be used to programmatically create charts.
        ''' </summary>
        ''' <param name="theChart">A Chart instance.</param>
        ''' <param name="chartType">The ReportChartType that is to be created.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetChartBuilder(ByVal theChart As Chart, _
                                               ByVal chartType As ReportChartType, _
                                               ByVal conn As SqlConnection) As ChartBuilder

            Dim builder As ChartBuilder = TryCast( _
                Activator.CreateInstance(_chartTypeMap(chartType), _
                                        conn, _
                                        theChart _
                ),  _
                ChartBuilder _
            )

            Return builder

        End Function

#End Region

    End Class

End Namespace
