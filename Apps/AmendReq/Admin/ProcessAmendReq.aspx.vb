
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.AmendReq.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.AmendReq.Admin.ProcessAmendReq
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allows an admin user to process a single amendment request.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	26/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ProcessAmendReq
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemAmendReqProcessRequest"), "Process Amendment Request")

            Dim msg As ErrorMessage
            Dim requestID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim req As WebAmendReq

            ' get the request
            req = New WebAmendReq(Me.DbConnection)
            msg = req.Fetch(requestID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' is the request in the right state to be processed?
            If req.Status <> RequestStatus.Submitted Then
                WebUtils.DisplayAccessDenied()
            End If

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add page JS link
            Me.JsLinks.Add("ProcessAmendReq.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.AmendReq.WebSvc.AmendmentRequests))

            ' extra CSS
            Me.AddExtraCssStyle("label.label { float:left;width:16em;font-weight:bold; } span.content { width:70%;float:left;margin-bottom:1em; }")

            With Me.txtComment.TextBox
                .TextMode = TextBoxMode.MultiLine
                .Rows = 5
            End With

        End Sub

    End Class

End Namespace