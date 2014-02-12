
Imports Target.Library
Imports Target.Web.Apps.Security.Collections

Namespace Apps.Security.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Security.Admin.RoleList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to view a list of security roles.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      07/03/2007  Added user level support.
    ''' 	[Mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class RoleList
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Roles"), "Admin: Security: List Roles")

            Dim roleID As Integer = Utils.ToInt32(Request.QueryString("roleID"))
            Dim showNewButton As Boolean = Me.UserHasMenuItemCommandInAnyMenuItem(Target.Library.Web.ConstantsManager.GetConstant(Me.Settings.CurrentApplication, "WebNavMenuItemCommand.EditRole.AddNew"))
            Dim showViewButton As Boolean = SecurityBL.UserHasMenuItem( _
                Me.DbConnection, _
                SecurityBL.GetCurrentUser.ID, _
                Target.Library.Web.ConstantsManager.GetConstant(Me.Settings.CurrentApplication, "WebNavMenuItem.EditRole"), _
                Me.Settings.CurrentApplicationID _
            )

            selector.InitControl(Me, roleID, showNewButton, showViewButton)

        End Sub

    End Class

End Namespace