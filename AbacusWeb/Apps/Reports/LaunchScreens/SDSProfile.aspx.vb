Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils
Imports System.Web.UI.DataVisualization.Charting
Imports Target.Library.Web.Charting
Imports Target.Web.Apps.Reports
Imports System.Collections.Specialized

Namespace Apps.Reports.LaunchScreens

    Partial Public Class SDSProfile
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim js As String
            Dim reportID As Integer

            reportID = Me.Request.QueryString("rID")
            cboOutput = cpDetail.FindControl("cboOutput")
            optShowAll = cpDetail.FindControl("optShowAll")
            optPermResOnly = cpDetail.FindControl("optPermResOnly")
            optExcludePermRes = cpDetail.FindControl("optExcludePermRes")

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Reporting.Reports"), "Reports")
            Me.UseJQuery = True
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/date.js"))
            Me.JsLinks.Add("SDSProfile.js")
            Me.Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), "ScriptLibrary", WebUtils.GetVirtualPath("Library/UserControls/ReportsButton.js"))

            js = String.Format("DateRange_btnViewID='{0}'; cboOutputID='{1}'; divDownloadContainerID='{2}'; optShowAllID='{3}'; optPermResOnlyID='{4}'; optExcludePermResID='{5}'", _
                                        btnView.ClientID, cboOutput.ClientID, divDownloadContainer.ClientID, optShowAll.ClientID, optPermResOnly.ClientID, optExcludePermRes.ClientID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                 "Target.Abacus.Web.Apps.Reports.LaunchScreens.DateRange.Startup", _
                 Target.Library.Web.Utils.WrapClientScript(js))


            With CType(btnView, IReportsButton)
                .ReportID = reportID
                .Enabled = True
                .Position = Target.Library.Web.Controls.SearchableMenu.SearchableMenuPosition.BottomLeft
            End With

            PopulateDropdown()
            If Me.IsPostBack Then
                cboOutput.SelectPostBackValue()
            Else
                optShowAll.Checked = True
                cpDetail.Expanded = True
            End If

            'Chart1.Visible = False

        End Sub

        Private Sub btnReport_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReport.ServerClick
            Dim builder As ChartBuilder
            Dim queryString As NameValueCollection = New NameValueCollection

            'Chart1.Visible = True

            Charting.ReportChartFactory.ApplyChartDefaults(Chart1)

            ' create the chart builder
            builder = Charting.ReportChartFactory.GetChartBuilder(Chart1, Charting.ReportChartType.SDSProfile, Me.DbConnection)

            With Chart1
                .Height = New Unit(635)
                .Width = New Unit(850)

            End With

            queryString.Remove("ShowPermRes")
            If optExcludePermRes.Checked Then queryString.Add("ShowPermRes", False)
            If optPermResOnly.Checked Then queryString.Add("ShowPermRes", True)

            builder.BuildChart(queryString)

            cpDetail.Expanded = False

        End Sub

#Region " PopulateDropdown "

        Private Sub PopulateDropdown()

            With cboOutput
                With .DropDownList
                    .Items.Clear()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem("Standard Viewer", 0))
                    .Items.Insert(1, New ListItem("Export to Excel", 1))
                    .Items.Insert(2, New ListItem("Export to PDF", 2))
                    '.SelectedValue = "Standard Viewer"
                End With

            End With

        End Sub

#End Region



    End Class

End Namespace