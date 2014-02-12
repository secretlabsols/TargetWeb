
Imports System.Net.Mail
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.CMS.Controls
Imports Target.Web.Apps.EmailSender

Namespace Apps.CMS.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.CMS.Admin.ViewPage
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to view and/or email a page created in the CMS editor.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      29/08/2006  D10921 - support for config settings in database.
    ''' 	[mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewPage
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: CMS: View/Email Page")

            With wizHeader
                .PageID = Target.Library.Utils.ToInt32(Request.QueryString("pageID"))
            End With

            lnkViewPage.NavigateUrl = String.Format("../CMSGetPage.axd?id={0}", wizHeader.PageID)
            lnkViewPage.Attributes.Add("onclick", "window.open(this.href,'_blank');return false;")

        End Sub

        Private Sub btnEmailPage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEmailPage.Click

            If IsValid Then

                Dim pageID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("pageID"))
                Dim addresses As String() = txtAddresses.Text.Split(vbCrLf)
                Dim msg As ErrorMessage
                Dim emailBodyWriter As System.IO.StringWriter = New System.IO.StringWriter
                Dim cmsPage As WebCMSPage

                ' get the page
                cmsPage = New WebCMSPage(Me.DbConnection)
                msg = cmsPage.Fetch(pageID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

                ' get the html
                Server.Execute(String.Format("../GetPage.aspx?id={0}&asEmail=1", pageID), emailBodyWriter)
                ' tidy it up so it can be emailed
                msg = EmailSenderBL.FixHtmlForEmailing(emailBodyWriter, Me.Settings("SiteBaseUrl"), Target.Library.Web.Utils.GetAbsolutePath("Apps/CMS/"))
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

                For i As Integer = addresses.GetLowerBound(0) To addresses.GetUpperBound(0)
                    If addresses(i).Trim().Length > 0 Then
                        msg = EmailSenderBL.QueueMessage(DbConnection, Nothing, _
                                        Me.Settings("SystemEmailAddress"), _
                                        Me.Settings("SiteName") & ": " & cmsPage.Title, _
                                        emailBodyWriter.ToString(), _
                                        True, _
                                        MailPriority.Normal, _
                                        addresses(i).Trim())

                        If msg.Success Then
                            lblError.Text = ""
                            lblEmailsSent.Text = "<br />This page has been successfully queued to be emailed to the addresses below.<br />"
                        Else
                            lblError.Text = msg.Message
                            lblEmailsSent.Text = ""
                            Exit For
                        End If
                    End If
                Next

            End If

        End Sub


    End Class

End Namespace