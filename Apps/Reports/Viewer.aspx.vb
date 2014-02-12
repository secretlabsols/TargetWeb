
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Reports
Imports Target.Web.Apps.Reports.Collections
Imports Target.Web.Apps.Security
Imports Microsoft.Reporting.WebForms
Imports AjaxControlToolkit
Imports System.Net

Namespace Apps.Reports

    ''' <summary>
    ''' Screen with a report viewer control to display SSRS reports.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Waqas    05/04/2013  A7823 - chart images are not being displayed
    ''' Iftikhar 23/02/2011  D11966 - using object SsrsReportRenderer for rendering reports.
    ''' MoTahir 05/10/2010  SDS Issue 36 - Sharepoint
    ''' MikeVO  22/09/2010  Enabled external images for local, non-simple list reports. 
    ''' MikeVO  26/10/2009  D11710 - disabled page partial rendering
    ''' MikeVO  08/07/2009  D11630 - allow extended timeout of stored procedure call.
    ''' MikeVO  12/05/2009  D11549 - created.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class Viewer
        Inherits Target.Web.Apps.BasePage

        Private _startupJS As StringBuilder = New StringBuilder()
        Private _reportRenderer As SsrsReportRenderer

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Report Viewer")

            Dim reportID As Integer = Utils.ToInt32(Request.QueryString("rID"))
            Dim currentApplication As ApplicationName = Me.Settings.CurrentApplicationID
            Dim msg As ErrorMessage

            Me.AddExtraCssStyle("a, a:hover { background-color:transparent; }")
            Me.JsLinks.Add("Viewer.js")
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Reports.WebSvc.Reports))

            ' fetch report record(s)
            _reportRenderer = New SsrsReportRenderer(Me.DbConnection)
            msg = _reportRenderer.FetchWebReportRecord(reportID)
            If Not msg.Success Then
                If msg.Number = "E0513" Then
                    ' record not found
                    WebUtils.DisplayNotFound()
                Else
                    WebUtils.DisplayError(msg)
                End If
            End If

            If Not IsPostBack AndAlso Not IsAsync Then

                CheckSecurity(_reportRenderer.WebReportRecord)
                SetupViewer(_reportRenderer.WebReportRecord)
                SetupReport(_reportRenderer.WebReportRecord)
                RenderReport(_reportRenderer.WebReportRecord)

            End If

        End Sub

#End Region

#Region " CheckSecurity "

        Private Sub CheckSecurity(ByVal report As WebReport)

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID

            With report
                ' check application
                If .ApplicationID <> currentApp Then
                    WebUtils.DisplayNotFound()
                End If

                ' check security
                If Not SecurityBL.UserHasMenuItem(Me.DbConnection, currentUser.ID, report.WebNavMenuItemID, currentApp) Then
                    WebUtils.DisplayAccessDenied()
                End If
                If Utils.ToInt32(report.WebNavMenuItemCommandID) > 0 AndAlso _
                        Not SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                                                              currentUser.ID, _
                                                              report.WebNavMenuItemCommandID, _
                                                              currentApp) Then
                    WebUtils.DisplayAccessDenied()
                End If
            End With

        End Sub

#End Region

#Region " SetupViewer "

        Private Sub SetupViewer(ByVal report As WebReport)

            Me.PageTitle = String.Format("Report: {0}", report.Description)

            With rvReportViewer
                .ProcessingMode = report.ProcessingMode
                If report.ProcessingMode = ProcessingMode.Local Then
                    .ShowExportControls = False
                Else
                    .AsyncRendering = True
                End If
            End With

        End Sub

#End Region

#Region " SetupReport "

        Private Sub SetupReport(ByVal report As WebReport)

            If report.ProcessingMode = ProcessingMode.Local Then
                SetupReportLocal(report)
            Else
                SetupReportRemote(report)
            End If

        End Sub

#End Region

#Region " SetupReportLocal "

        Private Sub SetupReportLocal(ByVal report As WebReport)
            ' do nothing
        End Sub

#End Region

#Region " SetupReportRemote "

        Private Sub SetupReportRemote(ByVal report As WebReport)

            Dim reportCookie As Cookie = Nothing
            Dim cookie As HttpCookie = Request.Cookies("sqlAuthCookie")

            If cookie Is Nothing Then
                cookie = ReportServerProxy.AuthenticateReportServerAccess()
            End If

            reportCookie = New Cookie(cookie.Name, cookie.Value)
            reportCookie.Domain = Request.Url.Host

            With rvReportViewer
                .ServerReport.ReportServerUrl = New Uri(ReportServerConnection.ReportServerUrl, ReportServerConnection.ReportServerBasePath)
                .ServerReport.ReportPath = report.Path
                .ServerReport.ReportServerCredentials = New ReportCredentials(reportCookie)

                .ShowParameterPrompts = False
                .ShowToolBar = True
                .ShowPageNavigationControls = True
                .ShowExportControls = False
                .ShowPrintButton = False
                .ShowDocumentMapButton = False
            End With

        End Sub

#End Region

#Region " RenderReport "

        Private Sub RenderReport(ByVal report As WebReport)

            Dim msg As ErrorMessage
            Dim renderedReport As SsrsRenderedReport = Nothing

            If report.ProcessingMode = ProcessingMode.Local Then
                ' local report
                With _reportRenderer
                    AddHandler .RowLimitExceeded, AddressOf NotifyRowLimit
                    msg = .RenderReport(rvReportViewer.LocalReport, _
                                        Request.QueryString, _
                                        renderedReport _
                    )
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With
            Else
                ' remote report
                With _reportRenderer
                    msg = .RenderReport(rvReportViewer.ServerReport, _
                                        Request.QueryString, _
                                        renderedReport _
                    )
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With
            End If

            ' output result to browser
            If Not renderedReport.PageStreams Is Nothing AndAlso renderedReport.PageStreams.Count > 0 Then
                With Response
                    .Buffer = True
                    .Clear()
                    .ContentType = renderedReport.MimeType
                    .AddHeader("content-disposition", _
                        String.Format("attachment; filename={0}.{1}", _
                            report.Description.Replace(" ", "_"), _
                            renderedReport.FileNameExtension _
                        ) _
                    )
                    renderedReport.WriteReportData(Response.OutputStream)
                    .Flush()
                    .End()
                End With
            End If

        End Sub

#End Region

#Region " NotifyRowLimit "

        Private Sub NotifyRowLimit(ByVal sender As Object, ByVal rowLimit As Integer)
            _startupJS.AppendFormat("ReportViewer_NotifyRowLimit({0});", rowLimit)
        End Sub

#End Region

#Region " Viewer_PreRenderComplete "

        Private Sub Viewer_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            _startupJS.AppendFormat("ReportViewer_ClientID='{0}';", rvReportViewer.ClientID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                  "Startup", _
                                                  _startupJS.ToString(), _
                                                  True)

        End Sub

#End Region

    End Class

End Namespace