
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.FileStore
Imports Target.Web.Apps.Msg.Collections
Imports Target.Web.Apps.Security

Namespace Apps.Msg

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Msg.ReplyTo
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen to allows users to reply to a message.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      26/11/2008  Removed SlickUpload references.''' 
    '''     MikeVO      19/09/2006  Moved attachment code in FileStoreBL.
    ''' 	[Mikevo]	12/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ReplyTo
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.ReplyToMessage"), "Reply To Message")
            Me.Form.Enctype = "multipart/form-data"

            Dim msg As ErrorMessage
            Dim msgID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim message As WebMsgMessage
            Dim conv As WebMsgConversation
            Dim fromUserID As Integer, fromCompanyID As Integer
            Dim fromCompany As WebSecurityCompany
            Dim fromName As String
            Dim fromType As MessageFromType
            Dim replyToUser As WebSecurityUser
            Dim replyToCompany As WebSecurityCompany
            Dim replyToUserID As Integer, replyToCompanyID As Integer
            Dim replyToName As String = Nothing
            Dim recipientList As String = Nothing
            Dim attachmentIDs As Integer()

            ' add page JS link
            Me.JsLinks.Add("ReplyTo.js")

            ' get the message
            message = New WebMsgMessage(Me.DbConnection)
            msg = message.Fetch(msgID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' get the conversation
            conv = New WebMsgConversation(Me.DbConnection)
            msg = conv.Fetch(message.WebMsgConversationID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' was the original message sent to the user or their company?
            If Target.Library.Utils.ToInt32(message.ToWebSecurityUserID) > 0 Then
                fromUserID = user.ID
                fromCompanyID = 0
                fromName = String.Format("{0}, {1}", user.Surname, user.FirstName)
                fromType = MessageFromType.User
            Else
                ' it must be the company
                fromUserID = 0
                fromCompanyID = user.WebSecurityCompanyID
                ' get the company name
                fromCompany = New WebSecurityCompany(Me.DbConnection)
                msg = fromCompany.Fetch(fromCompanyID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                fromName = fromCompany.Name
                fromType = MessageFromType.Company
            End If

            ' are we replying to a user or the company?
            replyToUserID = Target.Library.Utils.ToInt32(message.FromWebSecurityUserID)
            replyToCompanyID = Target.Library.Utils.ToInt32(message.FromWebSecurityCompanyID)
            If replyToUserID > 0 Then
                ' reply to a user
                replyToUser = New WebSecurityUser(Me.DbConnection)
                msg = replyToUser.Fetch(replyToUserID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                replyToName = String.Format("{0}, {1}", replyToUser.Surname, replyToUser.FirstName)
                recipientList = String.Format("{0}:{1};", Convert.ToInt32(MessageFromType.User), replyToUserID)
            End If
            If replyToCompanyID > 0 Then
                ' reply to a company
                replyToCompany = New WebSecurityCompany(Me.DbConnection)
                msg = replyToCompany.Fetch(replyToCompanyID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                replyToName = replyToCompany.Name
                recipientList = String.Format("{0}:{1};", Convert.ToInt32(MessageFromType.Company), replyToCompanyID)
            End If

            ' if the page was submitted and passed validation
            If IsPostBack Then
                Page.Validate()
                If Page.IsValid Then
                    ' get the uploaded attachments
                    attachmentIDs = FileStoreBL.GetUploadedTempFileIDs()

                    ' create the new message
                    msg = MsgBL.NewMessage(Me.DbConnection, conv.ID, fromType, _
                        IIf(fromType = MessageFromType.User, fromUserID, fromCompanyID), _
                        recipientList, attachmentIDs, txtMessage.Text)

                    If Not msg.Success Then
                        Target.Library.Web.Utils.DisplayError(msg)
                    Else
                        Response.Redirect("ListConvs.aspx")
                    End If

                End If
            End If

            ' setup the UI
            litFrom.Text = fromName
            litTo.Text = replyToName
            litSubject.Text = conv.Subject
            With txtMessage.TextBox
                .TextMode = TextBoxMode.MultiLine
                .Rows = 10
                .Text = String.Format("{2}{2}{2}[{0} wrote on {1}]{2}{2}{3}", replyToName, message.Sent, vbCrLf, message.Message)
            End With

        End Sub

    End Class

End Namespace

