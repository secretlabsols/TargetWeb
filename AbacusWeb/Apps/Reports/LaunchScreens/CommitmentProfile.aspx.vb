Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils
Imports System.Web.UI.DataVisualization.Charting
Imports Target.Library.Web.Charting
Imports Target.Web.Apps.Reports
Imports System.Collections.Specialized
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.Reports.LaunchScreens

    Partial Public Class CommitmentProfile
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim js As String
            Dim reportID As Integer

            reportID = Me.Request.QueryString("rID")
            cboOutput = cpDetail.FindControl("cboOutput")
            optCommType = cpDetail.FindControl("optCommType")
            optServClass = cpDetail.FindControl("optServClass")
            optServType = cpDetail.FindControl("optServType")

            If Not Me.IsPostBack Then
                With CType(ctlDateRange, DateRange)
                    .InitControl(Me, True)
                End With
            End If

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Reporting.Reports"), "Reports")
            Me.UseJQuery = True
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/date.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Utils.js"))
            Me.JsLinks.Add("CommitmentProfile.js")
            Me.Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), "ScriptLibrary", WebUtils.GetVirtualPath("Library/UserControls/ReportsButton.js"))

            js = String.Format("CommProfile_btnViewID='{0}'; cboOutputID='{1}'; divDownloadContainerID='{2}'; optCommTypeID='{3}'; optServClassID='{4}'; optServTypeID='{5}';dateFromValue='{6}'; dateToValue='{7}';cboSvcClassID='{8}';CommProfile_btnReportID='{9}';", _
                                        btnView.ClientID, cboOutput.ClientID, divDownloadContainer.ClientID, optCommType.ClientID, _
                                        optServClass.ClientID, optServType.ClientID, CType(ctlDateRange, DateRange).DateFrom, _
                                        CType(ctlDateRange, DateRange).DateTo, cboSvcClassification.ClientID, btnReport.ClientID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                 "Target.Abacus.Web.Apps.Reports.LaunchScreens.DateRange.Startup", _
                 Target.Library.Web.Utils.WrapClientScript(js))

            cboSvcClassification.DropDownList.Attributes.Add("onchange", "cboSvcClassification_Changed();")

            With CType(btnView, IReportsButton)
                .ReportID = reportID
                .Enabled = True
                .Position = Target.Library.Web.Controls.SearchableMenu.SearchableMenuPosition.BottomLeft
            End With

            PopulateDropdown()
            If Me.IsPostBack Then
                cboOutput.SelectPostBackValue()
            Else
                optCommType.Checked = True
                cpDetail.Expanded = True
            End If

            'Chart1.Visible = False

        End Sub

        Private Sub btnReport_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReport.ServerClick
            Dim builder As ChartBuilder = Nothing
            Dim queryString As NameValueCollection = New NameValueCollection
            Dim msg As ErrorMessage

            'Check if parameters actually return data.
            lblError.Text = String.Empty
            msg = WillReportReturnData()
            If Not msg.Success Then
                lblError.Text = msg.Message
            Else
                'Chart1.Visible = True
                Charting.ReportChartFactory.ApplyChartDefaults(Chart1)

                If optCommType.Checked Then
                    ' create the chart builder
                    builder = Charting.ReportChartFactory.GetChartBuilder(Chart1, _
                                                                          Charting.ReportChartType.SDSCommitmentCommTypeTransType, _
                                                                          Me.DbConnection)
                    With CType(btnView, IReportsButton)
                        .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.CommitmentByTypeAndTransactionType")
                    End With
                End If

                If optServClass.Checked Then
                    ' create the chart builder
                    builder = Charting.ReportChartFactory.GetChartBuilder(Chart1, _
                                                                          Charting.ReportChartType.SDSCommitmentSvcClassificationSvcType, _
                                                                          Me.DbConnection)
                    With CType(btnView, IReportsButton)
                        .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.CommitmentByServiceClassificationAndServiceType")
                    End With
                End If

                If optServType.Checked Then
                    ' create the chart builder
                    builder = Charting.ReportChartFactory.GetChartBuilder(Chart1, _
                                                                          Charting.ReportChartType.SDSCommitmentSvcTypeBudgetCategory, _
                                                                          Me.DbConnection)
                    With CType(btnView, IReportsButton)
                        .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.CommitmentByServiceTypeAndBudgetCategory")
                    End With
                End If

                With Chart1
                    .Height = New Unit(630)
                    .Width = New Unit(850)
                End With

                queryString.Remove("dateFrom")
                queryString.Remove("dateTo")
                queryString.Remove("svcClassificationID")
                queryString.Add("dateFrom", CType(ctlDateRange, DateRange).DateFrom.ToShortDateString)
                queryString.Add("dateTo", CType(ctlDateRange, DateRange).DateTo.ToShortDateString)
                If optServType.Checked Then
                    cboSvcClassification.SelectPostBackValue()
                    queryString.Add("svcClassificationID", cboSvcClassification.DropDownList.SelectedValue)
                End If

                builder.BuildChart(queryString)

                cpDetail.Expanded = False
            End If
        End Sub

#Region " PopulateDropdown "

        Private Sub PopulateDropdown()
            Dim msg As ErrorMessage
            Dim svcClassifications As ServiceGroupClassificationCollection = Nothing

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

            msg = ServiceGroupClassification.FetchList(conn:=Me.DbConnection, list:=svcClassifications, _
                                                       auditUserName:=String.Empty, auditLogTitle:=String.Empty)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboSvcClassification.DropDownList
                .Items.Clear()
                For Each svcGroup As ServiceGroupClassification In svcClassifications
                    .Items.Add(New ListItem(svcGroup.Description, svcGroup.ID))
                Next
                '.Items.Add(New ListItem("Cash", 0))
            End With
        End Sub

#End Region

#Region " WillReportReturnData "
        ''' <summary>
        ''' This procedure is used to determine if any data will be returned with the selected parameters.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function WillReportReturnData() As ErrorMessage
            Dim msg As ErrorMessage
            Dim SP_NAME As String = String.Empty
            Dim spParams As SqlParameter()

            'Decide which Stored Procedure to use
            If optCommType.Checked Then
                SP_NAME = "spxReport_CommitmentByCommTypeTransType"
            End If

            If optServClass.Checked Then
                SP_NAME = "spxReport_CommitmentBySvcClassSvcType"
            End If

            If optServType.Checked Then
                SP_NAME = "spxReport_CommitmentBySvcTypeBudgetCategory"
            End If

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)

                If optServType.Checked Then
                    cboSvcClassification.SelectPostBackValue()
                    spParams(0).Value = cboSvcClassification.DropDownList.SelectedValue
                    spParams(1).Value = CType(ctlDateRange, DateRange).DateFrom
                    spParams(2).Value = CType(ctlDateRange, DateRange).DateTo
                    spParams(3).Value = 1
                Else
                    spParams(0).Value = CType(ctlDateRange, DateRange).DateFrom
                    spParams(1).Value = CType(ctlDateRange, DateRange).DateTo
                    spParams(2).Value = 1
                End If
                Dim dataDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                If dataDataset.Tables(0).Rows.Count > 0 Then
                    msg = New ErrorMessage
                    msg.Success = True
                Else
                    msg = New ErrorMessage
                    msg.Success = False
                    msg.Message = "No data will be returned with the parameters supplied."
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            End Try

            Return msg


        End Function
#End Region


    End Class

End Namespace