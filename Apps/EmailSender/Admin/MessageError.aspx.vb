
Imports Target.Library
Imports Target.Library.Web

Namespace Apps.EmailSender.Admin

    ''' <summary>
    ''' Screen to allow a user to view the error for an individual email.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class MessageError
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.EmailSender"), "Admin: Email Queue: Message Error")

            Dim msg As ErrorMessage
            Dim recipient As WebEmailSenderRecipient = New WebEmailSenderRecipient(Me.DbConnection)
            msg = recipient.Fetch(Request.QueryString("id"))
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            If Not recipient.ErrorMessage Is Nothing AndAlso recipient.ErrorMessage.Length > 0 Then
                litError.Text = recipient.ErrorMessage.Replace(vbCrLf, "<br/><br/>")
            Else
                litError.Text = "No error is held against this message."
            End If

        End Sub

    End Class

End Namespace