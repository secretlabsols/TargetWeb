
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.Msg
Imports Target.Web.Apps.Security

Namespace Apps.Msg

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Msg.ListConvs
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to view all of the conversation sent by or to them or their company.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      26/03/2007  Removed dteStartedFrom client-side onclick attribute.
    '''     MikeVO      13/12/2006  Changed page title to "Conversations".
    '''     MikeVO      21/11/2006  Removed To filter.
    ''' 	[Mikevo]	07/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ListConvs
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Messaging"), "Conversations")

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim currentLabel As Integer = Target.Library.Utils.ToInt32(Request.QueryString("labelID"))
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            Dim currentStartedByID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("startedByID"))
            Dim currentStartedByType As Integer = Target.Library.Utils.ToInt32(Request.QueryString("startedByType"))
            Dim currentInvolvingID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("involvingID"))
            Dim currentInvolvingType As Integer = Target.Library.Utils.ToInt32(Request.QueryString("involvingType"))
            Dim currentStatus As String = Request.QueryString("status")
            Dim currentlastSentFrom As String = Request.QueryString("LastSentFrom")
            Dim currentLastSentTo As String = Request.QueryString("LastSentTo")
            Dim currentStartedFrom As String = Request.QueryString("StartedFrom")
            Dim currentStartedTo As String = Request.QueryString("StartedTo")

            If currentPage <= 0 Then currentPage = 1

            btnNewConv.Visible = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                        user.ID, _
                                        ConstantsManager.GetConstant(Me.Settings.CurrentApplication, "WebNavMenuItem.NewConversation"), _
                                        Me.Settings.CurrentApplicationID)

            dteLastSentTo.Label.Style.Add("margin", "0.5em 0.5em")
            dteLastSentFrom.Label.Style.Add("margin", "0.5em 0em")
            dteStartedTo.Label.Style.Add("margin", "0.5em 0.5em")
            dteStartedFrom.Label.Style.Add("margin", "0.5em 0em")

            If Target.Library.Utils.IsDate(currentlastSentFrom) Then dteLastSentFrom.Text = currentlastSentFrom
            If Target.Library.Utils.IsDate(currentLastSentTo) Then dteLastSentTo.Text = currentLastSentTo
            If Target.Library.Utils.IsDate(currentStartedFrom) Then dteStartedFrom.Text = currentStartedFrom
            If Target.Library.Utils.IsDate(currentStartedTo) Then dteStartedTo.Text = currentStartedTo
            If currentStatus <> "" Then cboStatus.Value = currentStatus
            If currentLabel <> 0 Then cboLabel.Value = currentLabel

            ' add table sorting JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))
            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS
            Me.JsLinks.Add("ListConvs.js")
            ' add msg common JS link
            Me.JsLinks.Add("MsgShared.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Msg.WebSvc.Messaging))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Security.WebSvc.Security))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Web.Apps.Msg.MessagePassingType))

            ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.Msg.ListConvs.Startup", Target.Library.Web.Utils.WrapClientScript( _
                String.Format("webSecurityCompanyID={0};currentLabel={1};currentPage={2};currentStartedByID={3};currentStartedByType={4};currentInvolvingID={5};currentInvolvingType={6};currentStatus=""{7}"";currentlastSentFrom=""{8}"";currentLastSentTo=""{9}"";currentStartedFrom=""{10}"";currentStartedTo=""{11}"";Init();", _
                    user.WebSecurityCompanyID, currentLabel, currentPage, currentStartedByID, currentStartedByType, currentInvolvingID, currentInvolvingType, currentStatus, currentlastSentFrom, currentLastSentTo, currentStartedFrom, currentStartedTo)) _
            )

            IntialiseJsVariables()

        End Sub


        Private Sub IntialiseJsVariables()


            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in the table filter library 
            UseJqueryTableFilter = True

            ' add the table scroller library as we might have large amounts of data
            UseJqueryTableScroller = True

            ' add the searchable menu
            UseJquerySearchableMenu = True

            ' add the jquery tooltip
            UseJqueryTooltip = True

            UseJqueryTemplates = True

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

           
        End Sub
    End Class

End Namespace