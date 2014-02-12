
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps.SUNotif

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.SUNotif.ListSUNotif
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to view all of the service user notifications created by them or their company.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      13/12/2006  Removed table sorting.
    '''                             Added custom table filter.
    ''' 	[Mikevo]	23/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ListSUNotif
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPSUNotifsList"), "Service User Notifications")

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim currentFromDate As Date, currentToDate As Date
            Dim currentStatus As Integer = Target.Library.Utils.ToInt32(Request.QueryString("status"))
            Dim currentRequestedBy As Integer = Target.Library.Utils.ToInt32(Request.QueryString("requestedBy"))
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            Dim isAdminUser As Boolean
            Dim dropdownItem As ListItem

            If currentPage <= 0 Then currentPage = 1

            ' check admin rights
            isAdminUser = SecurityBL.UserHasItem(Me.DbConnection, user.ID, ConstantsManager.GetConstant("webSecurityItemSPSUNotifsProcess"))
            If isAdminUser Then
                ' for admin users, default status filter
                If currentStatus = 0 Then currentStatus = SUNotifStatus.Submitted
            Else
                ' hide the requested by filter
                pnlRequestedByFilter.Visible = False
                currentRequestedBy = user.ExternalUserID
            End If

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add("ListSUNotif.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.SP.Web.Apps.WebSvc.SUNotif))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Security.WebSvc.Security))

            ' load status values
            With cboStatus.Items
                dropdownItem = New ListItem("", 0)
                .Add(dropdownItem)
                For Each status As Byte In [Enum].GetValues(GetType(SUNotifStatus))
                    dropdownItem = New ListItem(Target.Library.Utils.SplitOnCapitals([Enum].GetName(GetType(SUNotifStatus), status)), status)
                    .Add(dropdownItem)
                Next
            End With

            ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Web.Apps.SUNotif.ListSUNotif.Startup", WebUtils.WrapClientScript( _
                String.Format("currentFromDate={0};currentToDate={1};currentStatus={2};currentRequestedBy={3};currentPage={4};Init();", _
                    WebUtils.GetDateAsJavascriptString(currentFromDate), WebUtils.GetDateAsJavascriptString(currentToDate), _
                    currentStatus, currentRequestedBy, currentPage)) _
            )

        End Sub

    End Class

End Namespace