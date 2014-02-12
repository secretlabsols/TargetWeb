
Imports System.Collections.Generic
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Security.Admin

    ''' <summary>
    ''' Screen to allow the management of concurrent user logins.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      06/04/2009  D11539 - added warning message when "prevent multiple logins..." option is not enabled.
    '''                             Changed to use postbacks for single logout.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Public Class ManageLogins
        Inherits BasePage

        Dim _currentUser As WebSecurityUser
        Dim _canLogUsersOut As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.ManageLogins"), "Admin: Security: Manage Logins")

            Dim msg As ErrorMessage
            Dim users As List(Of WebSecurityUser) = Nothing

            pnlNotActive.Visible = (ConcurrentUsersLicence.ConcurrentUserLimit = 0)

            _currentUser = SecurityBL.GetCurrentUser()
            _canLogUsersOut = Me.UserHasMenuItemCommand(ConstantsManager.GetConstant(Me.Settings.CurrentApplication, "WebNavMenuItemCommand.ManageLogins.LogOutUsers"))

            msg = ConcurrentUsersLicence.FetchLoggedInUsers(users)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            btnLogoutAll.Visible = _canLogUsersOut
            If btnLogoutAll.Visible Then
                btnLogoutAll.Style.Add("float", "left")
                btnLogoutAll.Attributes.Add("onclick", "return window.confirm(""Are you sure you wish to logout ALL users (except yourself)?"");")
            End If

            rptUsers.DataSource = users
            rptUsers.DataBind()

        End Sub

        Private Sub btnLogoutAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogoutAll.Click

            Dim msg As ErrorMessage
            Dim users As List(Of WebSecurityUser) = Nothing

            ' check permissions
            If Not _canLogUsersOut Then WebUtils.DisplayAccessDenied()

            msg = ConcurrentUsersLicence.FetchLoggedInUsers(users)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each u As WebSecurityUser In users
                If u.ID <> _currentUser.ID Then
                    ConcurrentUsersLicence.UnregisterLogin(u.ID)
                End If
            Next

            Response.Redirect("ManageLogins.aspx")

        End Sub

        Protected Function GetLocation(ByVal ipAddress As String) As String
            Return String.Format("{0}<br />({1})", ipAddress, Target.Library.Utils.IPAddressToHostName(ipAddress))
        End Function

        Protected Function GetIdleTime(ByVal lastLoginDate As Date) As String
            Dim idleDate As TimeSpan = DateTime.Now.Subtract(lastLoginDate)
            Dim idleString As String = String.Format("{0:D2}:{1:D2}:{2:D2}", idleDate.Hours, idleDate.Minutes, idleDate.Seconds)
            Return idleString
        End Function

        Protected Function CurrentUserID() As Integer
            Return _currentUser.ID
        End Function

        Private Sub rptUsers_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptUsers.ItemCommand

            Select Case e.CommandName
                Case "logout"
                    ConcurrentUsersLicence.UnregisterLogin(Target.Library.Utils.ToInt32(e.CommandArgument))
            End Select

            Response.Redirect("ManageLogins.aspx")

        End Sub

    End Class

End Namespace