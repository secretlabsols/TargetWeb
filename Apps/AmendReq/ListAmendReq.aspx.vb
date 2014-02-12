
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.Security

Namespace Apps.AmendReq

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.AmendReq.ListAmendReq
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allows user to view their own amendment requests.
    '''     Admin users can view amendment request for all users and process each request.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	21/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ListAmendReq
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemAmendReqListRequests"), "Amendment Requests")

            Dim dropdownItem As ListItem
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim currentRequestedBy As Integer, currentStatus As Integer

            ' add table sorting JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))
            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS link
            Me.JsLinks.Add("ListAmendReq.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.AmendReq.WebSvc.AmendmentRequests))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Security.WebSvc.Security))

            If SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                                         user.ID, _
                                         ConstantsManager.GetConstant(Settings.CurrentApplication, "WebNavMenuItem.ProcessAmendmentRequests"), _
                                         Settings.CurrentApplicationID) Then
                ' for admin users, default status filter
                If currentStatus = 0 Then currentStatus = AmendReq.RequestStatus.Submitted
            Else
                pnlRequestedByFilter.Visible = False
                currentRequestedBy = user.ExternalUserID
            End If

            ' load status values
            With cboStatus.Items
                dropdownItem = New ListItem("", 0)
                .Add(dropdownItem)
                For Each status As Byte In [Enum].GetValues(GetType(RequestStatus))
                    dropdownItem = New ListItem(Target.Library.Utils.SplitOnCapitals([Enum].GetName(GetType(RequestStatus), status)), status)
                    .Add(dropdownItem)
                Next
            End With

            ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.AmendReq.ListAmendReq.Startup", WebUtils.WrapClientScript( _
                String.Format("currentUser={0};currentStatus={1};", currentRequestedBy, currentStatus)) _
            )

        End Sub

    End Class

End Namespace