
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.Msg
Imports Target.Web.Apps.Msg.Collections
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Msg

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Msg.ViewConv
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to view a single conversation and its messages sent by or to 
    '''     them or their company.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      22/03/2007  Changed Actions dropdown to use tag attribute, not label attribute
    '''                             as IE7 displays the label attirubte over the top of the text.
    ''' 	[Mikevo]	08/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewConv
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Messaging"), "View Conversation")

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim convID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim conv As WebMsgConversation
            Dim convMsgs As WebMsgMessageCollection = Nothing
            Dim convLabels As ConversationLabelLists = Nothing
            Dim convLabelsString As StringBuilder
            Dim newItem As ListItem
            Dim addLabelID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("alID"))
            Dim removeLabelID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("rlID"))
            Dim newLabel As String = Request.QueryString("nl")
            Dim canView As Boolean

            ' is the user allowed to view this conversation?
            msg = MsgBL.UserCanView(Me.DbConnection, user.ID, convID, 0, canView)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            If Not canView Then WebUtils.DisplayAccessDenied()

            ' add IE CSS to position the table correctly
            Me.AddExtraCssStyle("if lt IE 7", "table#Messages_Table { margin-top: -0.5em; }")

            ' add table sorting JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))
            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS link
            Me.JsLinks.Add("ViewConv.js")
            ' add msg common JS link
            Me.JsLinks.Add("MsgShared.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Msg.WebSvc.Messaging))

            ' add/remove/create labels?
            If removeLabelID > 0 Then
                ' remove a label
                msg = MsgBL.RemoveLabel(Me.DbConnection, convID, removeLabelID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            End If
            If addLabelID > 0 Then
                ' add a label
                msg = MsgBL.ApplyLabel(Me.DbConnection, convID, addLabelID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            End If
            If Not newLabel Is Nothing AndAlso newLabel.Length > 0 Then
                ' create and then apply the label
                msg = MsgBL.CreateAndApplyLabel(Me.DbConnection, convID, newLabel, user.ID, user.WebSecurityCompanyID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            End If

            ' get conversation
            conv = New WebMsgConversation(Me.DbConnection)
            msg = conv.Fetch(convID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' get conversation labels
            msg = MsgBL.FetchConversationLabelList(Me.DbConnection, convID, user.ID, convLabels)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' get messages
            msg = WebMsgMessage.FetchList(Me.DbConnection, convMsgs, convID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' setup UI
            lblSubject.Text = conv.Subject
            ' label list
            If convLabels.Applied.Count > 0 Then
                convLabelsString = New StringBuilder(convLabels.Applied.Count)
                For Each lbl As String In convLabels.Applied.Keys
                    If convLabelsString.Length > 0 Then convLabelsString.Append(", ")
                    convLabelsString.Append(lbl)
                Next
                lblLabels.Text = convLabelsString.ToString()
            End If
            ' actions dropdown
            cboActions.Items.Add(New ListItem("", 0))
            cboActions.Items.Add(New ListItem("Mark as Read", "Read"))
            cboActions.Items.Add(New ListItem("Mark as Unread", "UnRead"))
            cboActions.StartOptGroup("Apply Label:")
            For Each lbl As String In convLabels.NotApplied.Keys
                newItem = New ListItem(lbl, convLabels.NotApplied(lbl))
                newItem.Attributes.Add("tag", "alID")
                cboActions.Items.Add(newItem)
            Next
            cboActions.Items.Add(New ListItem("New Label...", -1))
            If convLabels.Applied.Count > 0 Then
                cboActions.StartOptGroup("Remove Label:")
                For Each lbl As String In convLabels.Applied.Keys
                    newItem = New ListItem(lbl, convLabels.Applied(lbl))
                    newItem.Attributes.Add("tag", "rlID")
                    cboActions.Items.Add(newItem)
                Next
            End If
            
        End Sub

    End Class

End Namespace