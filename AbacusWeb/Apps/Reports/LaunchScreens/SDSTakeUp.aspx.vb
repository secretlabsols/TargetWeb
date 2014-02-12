Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Reports.LaunchScreens

    Partial Public Class SDSTakeUp
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim reportID As Integer
            Me.EnableTimeout = False
            Dim js As String

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Reporting.Reports"), "Reports")

            If Not Me.IsPostBack Then
                With CType(ctlDateRange, DateRange)
                    .InitControl(Me, True)
                End With
            End If

            Me.JsLinks.Add("SDSTakeUp.js")

            reportID = Me.Request.QueryString("rID")

            js = String.Format("DateRange_btnViewID='{0}';dateFromValue='{1}'; dateToValue='{2}';", _
                                        btnView.ClientID, CType(ctlDateRange, DateRange).DateFrom, _
                                        CType(ctlDateRange, DateRange).DateTo)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                 "Target.Abacus.Web.Apps.Reports.LaunchScreens.DateRange.Startup", _
                 Target.Library.Web.Utils.WrapClientScript(js))

            With CType(ctlRptHeader, ReportHeader)
                .InitControl(Me, reportID)
            End With

            With CType(btnView, IReportsButton)
                .ReportID = reportID
                .Enabled = True
                .Position = Target.Library.Web.Controls.SearchableMenu.SearchableMenuPosition.TopRight
            End With

        End Sub

    End Class

End Namespace