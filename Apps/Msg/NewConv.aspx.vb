
Imports System.Configuration.ConfigurationManager
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.FileStore
Imports Target.Web.Apps.Msg
Imports Target.Web.Apps.Security

Namespace Apps.Msg

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Msg.NewConv
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to create a new conversation and message (with optional attachments)
    '''     and "send" to to other users.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      26/11/2008  Removed SlickUpload references.
    '''     MikeVO      16/11/2006  Added support for ICustomMsg.
    '''     MikeVO      19/09/2006  Moved attachment code in FileStoreBL.
    ''' 	[Mikevo]	06/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class NewConv
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.NewConversation"), "New Conversation")
            Me.Form.Enctype = "multipart/form-data"

            Dim msg As ErrorMessage
            Dim company As WebSecurityCompany
            Dim user As WebSecurityUser
            Dim fromType As MessageFromType
            Dim fromID As Integer
            Dim restrictMsgRecipients As RestrictMsgRecipientsResult = Nothing
            Dim attachmentIDs As Integer()
            Dim msgPasssingType As MessagePassingType

            ' get user and company
            user = SecurityBL.GetCurrentUser()
            company = New WebSecurityCompany(Me.DbConnection)
            msg = company.Fetch(user.WebSecurityCompanyID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' set from name
            msg = MsgBL.GetMessagePassingType(Me.Settings, msgPasssingType)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            Select Case msgPasssingType
                Case MessagePassingType.CompanyToCompany
                    spnFrom.InnerText = company.Name
                Case MessagePassingType.UserToUser
                    spnFrom.InnerText = String.Format("{0}, {1}", user.Surname, user.FirstName)
            End Select

            ' check recipient restrictions
            msg = MsgBL.RestrictMsgRecipients(Me.DbConnection, ConnectionStrings("Abacus").ConnectionString, user.ExternalUserID, Me.Settings, restrictMsgRecipients)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            ' if restricted, default recipient to the specified company
            If restrictMsgRecipients.Restricted Then
                txtRecipients.Value = String.Format("{0}:{1};", Convert.ToByte(RecipientType.Company), restrictMsgRecipients.WebSecurityCompanyID)
                ' disable To button, populate company name
                btnTo.Disabled = True
                spnRecipients.InnerText = restrictMsgRecipients.WebSecurityCompanyName
            End If

            ' add page JS link
            Me.JsLinks.Add("NewConv.js")
            ' add msg common JS link
            Me.JsLinks.Add("MsgShared.js")

            ' if the page was submitted and passed validation
            If IsPostBack Then
                Page.Validate()
                If Page.IsValid Then
                    ' get the uploaded attachments
                    attachmentIDs = FileStoreBL.GetUploadedTempFileIDs()

                    ' determine the from type/ID
                    Select Case msgPasssingType
                        Case MessagePassingType.CompanyToCompany
                            fromType = MessageFromType.Company
                            fromID = company.ID
                        Case MessagePassingType.UserToUser
                            fromType = MessageFromType.User
                            fromID = user.ID
                    End Select

                    ' create the new conversation
                    msg = MsgBL.NewConversation(Me.DbConnection, fromType, fromID, txtRecipients.Value, txtSubject.Text, attachmentIDs, txtMessage.Text)
                    If Not msg.Success Then
                        Target.Library.Web.Utils.DisplayError(msg)
                    Else
                        Response.Redirect("ListConvs.aspx")
                    End If

                End If
            End If

            ' setup the UI
            With txtMessage.TextBox
                .TextMode = TextBoxMode.MultiLine
                .Rows = 10
                .Columns = 1
            End With

        End Sub

    End Class

End Namespace