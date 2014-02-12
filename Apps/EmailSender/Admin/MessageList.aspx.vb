
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.EmailSender.Collections

Namespace Apps.EmailSender.Admin

    ''' <summary>
    ''' Screen to allow a user to view message with the specified status.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class MessageList
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.EmailSender"), "Admin: Email Queue: Messages")

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))

            Dim status As WebEmailSenderRecipientStatus = [Enum].Parse(GetType(WebEmailSenderRecipientStatus), Request.QueryString("statusid"))
            Dim currentPage As Integer = 1
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim msg As ErrorMessage
            Dim messages As vwWebEmailSenderMessage_WebEmailSenderRecipientCollection = Nothing

            If Not Request.QueryString("page") Is Nothing Then currentPage = Request.QueryString("page")

            msg = EmailSenderBL.GetMessageList(Me.DbConnection, status, currentPage, pageSize, totalRecords, messages)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            rptMessages.DataSource = messages
            rptMessages.DataBind()

            litStatus.Text = [Enum].GetName(GetType(WebEmailSenderRecipientStatus), status)
            litPagingLink.Text = Target.Library.Web.Utils.BuildPagingLinks( _
                "<a href=""MessageList.aspx?statusid=" & Request.QueryString("statusid") & "&amp;page={0}"" title=""{2}"">{1}</a>&nbsp;", _
                currentPage, Math.Ceiling(totalRecords / pageSize))

        End Sub

        Private Sub btnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBack.Click
            CustomNavGoBack()
        End Sub
    End Class

End Namespace