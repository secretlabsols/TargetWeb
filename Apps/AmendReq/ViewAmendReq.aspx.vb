
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.AmendReq

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.AmendReq.ViewAmendReq
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allows user to view the details of a single amendment request.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	21/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewAmendReq
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemAmendReqListRequests"), "View Amendment Request")

            Dim msg As ErrorMessage
            Dim requestID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim requestedByUser As WebSecurityUser
            Dim req As WebAmendReq
            Dim isAdminUser As Boolean

            ' get the request
            req = New WebAmendReq(Me.DbConnection)
            msg = req.Fetch(requestID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' check the current user is allowed to see this request
            If Not SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                                         currentUser.ID, _
                                         ConstantsManager.GetConstant(Settings.CurrentApplication, "WebNavMenuItem.ProcessAmendmentRequests"), _
                                         Settings.CurrentApplicationID) Then
                ' get the user who made the request
                requestedByUser = New WebSecurityUser(Me.DbConnection)
                msg = requestedByUser.Fetch(req.RequestedByWebSecurityUserID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                ' check the external user accounts match
                If currentUser.ExternalUserID <> requestedByUser.ExternalUserID Then
                    WebUtils.DisplayAccessDenied()
                End If
            Else
                isAdminUser = True
            End If

            ' setup process button
            With btnProcess
                .Disabled = (req.Status <> RequestStatus.Submitted)
                .Visible = isAdminUser
            End With

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS link
            Me.JsLinks.Add("ViewAmendReq.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.AmendReq.WebSvc.AmendmentRequests))

            ' extra CSS
            Me.AddExtraCssStyle("label.label { float:left;width:9em;font-weight:bold; } span.content { width:80%;float:left;margin-bottom:1em; }")

        End Sub

    End Class

End Namespace