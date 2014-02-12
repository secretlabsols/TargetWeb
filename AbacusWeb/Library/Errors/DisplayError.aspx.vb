
Imports System.Configuration.ConfigurationSettings
Imports Target.Library
Imports Target.Library.Web

Namespace Library.Errors

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Library.Errors.DisplayError
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page used to display error information an dallow the submission of an error report.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      29/08/2006  D10921 - support for config settings in database.
    ''' 	[mikevo]	??/??/????	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class DisplayError
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.PageTitle = "Unexpected Error"
            Me.RenderMenu = False
            Me.EnableTimeout = False

            Try
                Dim msg As ErrorMessage = Session("LastError")
                Dim url As String = Session("LastErrorUrl")

                litError.Text = msg.Number & " - " & msg.Message
                litDateTime.Text = Now.ToString()
                litUrl.Text = url

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
                    litSystemException.Text = msg.ExName
                    litSystemMessage.Text = msg.ExMessage
                    litSource.Text = msg.ExSource
                    litTarget.Text = msg.ExTargetMethod
                    litInnerExcpetions.Text = msg.ExInnerExceptions.Replace(vbCrLf, "<br>")
                    litStackTrace.Text = msg.ExStackTrace.Replace(vbCrLf, "<br>")
                    litExtraInfo.Text = msg.ExExtraInfo.Replace(vbCrLf, "<br>")
                Else
                    pnlDetails.Visible = False
                End If

            Catch ex As Exception
                ' swallow errors
            End Try

        End Sub

        Private Sub btnSubmitReport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmitReport.Click

            lblReportSent.Visible = True
            If Target.Library.Web.Utils.EmailError(Session("LastError"), Session("LastErrorUrl"), txtUserComments.Text) Then
                pnlSubmitReport.Visible = False
            Else
                lblReportSent.CssClass = "errorText"
                lblReportSent.Text = "The error report could not be sent. Please contact the system administrator."
            End If

        End Sub
    End Class

End Namespace