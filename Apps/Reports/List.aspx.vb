
Namespace Apps.Reports

    ''' <summary>
    ''' Screen that lists and allows users to launch reports. 
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  26/10/2009  D11710 - created.
    ''' </history>
    Partial Public Class List
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Reporting.Reports.LaunchReports"), "Reports")

            selector.InitControl(Me)

        End Sub

    End Class

End Namespace
