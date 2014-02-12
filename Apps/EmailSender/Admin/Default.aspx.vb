
Imports System.Configuration.ConfigurationManager
Imports System.Data
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web

Namespace Apps.EmailSender.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.EmailSender.Admin._Default
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to view the status of the email sender service and the individual messages.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      27/03/2009  D11537 - changed "view"/"resubmit" links to do POSTs instead of GETs.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[mikevo]	29/08/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class DefaultPage
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.EmailSender"), "Admin: Email Queue")
            CustomNavAdd(True)

            lblActionError.Text = ""

            ' add IE CSS to position the table correctly
            Me.AddExtraCssStyle("if lt IE 7", "table#listTableIEMarginFix { margin-top: -0.78em; }")

            ' show/hide warnings
            lblDisabledWarning.Visible = Not Convert.ToBoolean(Me.Settings("Apps.EmailSender.Enabled"))

            ' populate configuration
            litPollInterval.Text = Me.Settings("Apps.EmailSender.PollInterval")
            litBatchLimit.Text = Me.Settings("Apps.EmailSender.BatchLimit")
            litRetryLimit.Text = Me.Settings("Apps.EmailSender.RetryLimit")
            If Convert.ToBoolean(Me.Settings("Apps.EmailSender.AutoDeleteSentEnabled")) Then
                litSentMessages.Text = "Sent messages <strong>are</strong> automatically deleted after <strong>" & Me.Settings("Apps.EmailSender.AutoDeleteSentAge") & "</strong> hour(s)."
            Else
                litSentMessages.Text = "Sent messages <strong>are not</strong> automatically deleted."
            End If

            ' get the counts from the database
            Dim counts As SqlDataReader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, "spxWebEmailSenderRecipient_GetStatusCounts")
            rptStatusCounts.DataSource = counts
            rptStatusCounts.DataBind()
            counts.Close()

        End Sub

        Private Sub rptStatusCounts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptStatusCounts.ItemCommand

            Dim msg As ErrorMessage

            Select Case e.CommandName
                Case "delete"
                    ' delete messages from queue
                    msg = EmailSenderBL.Delete(Me.DbConnection, [Enum].Parse(GetType(WebEmailSenderRecipientStatus), e.CommandArgument))
                    If Not msg.Success Then
                        If msg.Number = "E0508" Then
                            ' service currently processing
                            lblActionError.Text = msg.Message
                            Exit Sub
                        Else
                            Target.Library.Web.Utils.DisplayError(msg)
                        End If
                    End If

                Case "resubmit"
                    ' resubmit messages to queue
                    msg = EmailSenderBL.Resubmit(Me.DbConnection, [Enum].Parse(GetType(WebEmailSenderRecipientStatus), e.CommandArgument))
                    If Not msg.Success Then
                        If msg.Number = "E0508" Then
                            ' service currently processing
                            lblActionError.Text = msg.Message
                            Exit Sub
                        Else
                            Target.Library.Web.Utils.DisplayError(msg)
                        End If
                    End If

            End Select

            Response.Redirect("Default.aspx")

        End Sub

    End Class


End Namespace