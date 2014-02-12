
Imports System.Configuration.ConfigurationManager
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security

Namespace Library.Errors

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Library.Errors.DisplayError
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page to display error information and allow the submission of an error report.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      27/07/2009  A4WA#5604 - override Page_Error and encode output.
    '''     MikeVO      30/05/2007  Support for passing user info in error report.
    '''     MikeVO      29/08/2006  D10921 - support for config settings in database.
    ''' 	[mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class DisplayError
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.PageTitle = "Unexpected Error"
            Me.RenderMenu = False
            Me.EnableTimeout = False
            Me.MenuItemID = ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView")

            lnkHome.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Default.aspx")

            Try
                Dim msg As ErrorMessage = Session("LastError")
                Dim url As String = Session("LastErrorUrl")

                AddExtraCssStyle(String.Format(CSS_TEXT_SIZE_TEMPLATE, Session(SESSION_TEXT_SIZE)))

                litError.Text = msg.Number & " - " & msg.Message
                litDateTime.Text = Now.ToString()
                litUrl.Text = WebUtils.EncodeOutput(url)

                ' decide whether to allow the error report
                If Me.Settings("customErrorAllowReport").ToLower() = "on" Then
                    pnlSubmitReport.Visible = True
                End If

                ' decide whether to display the error details
                Dim ipClient As New IPAddress(HttpContext.Current.Request.UserHostAddress)
                Dim ipRangeMin As String = Me.Settings("customErrorIpRangeMin")
                Dim ipRangeMax As String = Me.Settings("customErrorIpRangeMax")
                If ipClient.IsLoopback() OrElse ipClient.IsInRange(ipRangeMin, ipRangeMax) Then
                    pnlDetails.Visible = True
                    litsystemexception.Text = WebUtils.EncodeOutput(msg.ExName)
                    litsystemmessage.Text = WebUtils.EncodeOutput(msg.ExMessage)
                    litsource.Text = WebUtils.EncodeOutput(msg.ExSource)
                    littarget.Text = WebUtils.EncodeOutput(msg.ExTargetMethod)
                    If Not msg.ExInnerExceptions Is Nothing Then litinnerexcpetions.Text = msg.ExInnerExceptions.Replace(vbCrLf, "<br>")
                    If Not msg.ExStackTrace Is Nothing Then litstacktrace.Text = msg.ExStackTrace.Replace(vbCrLf, "<br>")
                    If Not msg.ExExtraInfo Is Nothing Then litextrainfo.Text = msg.ExExtraInfo.Replace(vbCrLf, "<br>")
                Else
                    pnlDetails.Visible = False
                End If

            Catch ex As Exception
                ' swallow errors
            End Try

        End Sub

        Private Sub btnSubmitReport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmitReport.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            lblReportSent.Visible = True
            If Target.Library.Web.Utils.EmailError(Session("LastError"), Session("LastErrorUrl"), txtUserComments.Text, _
                    String.Format("{0} {1}", currentUser.FirstName, currentUser.Surname), currentUser.Email) Then
                pnlSubmitReport.Visible = False
            Else
                lblReportSent.CssClass = "errorText"
                lblReportSent.Text = "The error report could not be sent. Please contact the system administrator."
            End If

        End Sub

        Protected Overrides Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs)
            ' do nothing and let the error bubble up
        End Sub

    End Class

End Namespace