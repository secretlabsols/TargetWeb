
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Navigation
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports System.Collections.Generic

Namespace Apps.Security.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Security.Admin.UserEdit
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to edit a user in the system.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MoTahir     28/11/2013  D12535 - Partioning of Data by Administrative Area
    '''     Waqas       23/08/2012  D12219 - Password Change Notification Email
    '''     MikeVO      11/08/2011  A4WA#6980 - more robust removal of granted roles from available roles list.
    '''     Waqas       16/02/2011  D11867 Show access denied if change user fro URL
    '''     MoTahir     08/02/2010  D11934 Intranet Password Maintenance
    '''     MikeVO      01/09/2010  Added validation summary.
    '''     MikeVO      27/07/2009  A4WA#5605 - prevent XSS attack via browser UserAgent.
    '''     MikeVO      10/07/2009  D11630 - added reports.
    '''     MikeVO      17/04/2009  A4WA#5388 - fixes after internal testing.
    '''     MikeVO      27/03/2009  D11538 - account activation/reset password overhaul.
    '''     MikeVO      23/03/2009  A4WA#5332 - fixing to email address validation.
    '''     MikeVO      26/01/2009  D11444 - fix to editing roles when have no access.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      11/06/2008  Ensure that current user is permitted to view details of the requested user.
    '''     MikeVO      16/11/2006  Fix to populate of company dropdown.
    '''     MikeVO      31/10/2006  Moved activate functionality to new UserActivate screen.
    '''     MikeVO      29/08/2006  D10921 - support for config settings in database.
    ''' 	[mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class UserEdit
        Inherits Target.Web.Apps.BasePage

        Dim _user As WebSecurityUser
        Dim _stdBut As StdButtonsBase
        Private _btnReports As HtmlInputButton = New HtmlInputButton("button")

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.EditUser"), "Admin: Security: Edit User")
            Me.ShowValidationSummary = True

            Dim userID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            SetupReports(userID)

        End Sub

#End Region

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            With _stdBut
                .AllowBack = True
                .AllowNew = False
                .AllowEdit = _
                    SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            currentUser.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant( _
                                                Me.Settings.CurrentApplication, _
                                                "WebNavMenuItem.EditUser"), _
                                            currentApp)
                .AllowDelete = False
                .AllowFind = False
                .EditableControls.Add(tabDetails.Controls)
                .EditableControls.Add(tabRoles.Controls)
                .EditableControls.Add(tabInfo.Controls)
                .EditableControls.Add(tabAdministrativeArea.Controls)
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf FindClicked

            Me.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Apps.UserControls.ServiceOrderReports", _
                Target.Library.Web.Utils.WrapClientScript(String.Format("Report_lstReportlistId='{0}';", _
                                                             lstReports.ClientID _
                                                )) _
                                        )
        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            FetchUser(e.ItemID)
            PopulateScreen()
            ValidateSecurity(_user, currentUser)

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim userID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim msg As ErrorMessage
            Dim users As WebSecurityUserCollection = Nothing
            Dim trans As SqlTransaction = Nothing
            Dim grantedRoles As WebSecurityRoleCollection = Nothing
            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim canEditRoles As Boolean
            Dim customInterface As ICustomSecurity = Nothing
            Dim replacementLogin As IReplacementLogin = Nothing
            Dim canEditAdministrativeAreas As Boolean
            Dim grantedAdministrativeAreas As AdministrativeSectorCollection = Nothing

            If Me.IsValid Then

                FetchUser(e.ItemID)

                canEditRoles = _
                    SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                                currentUser.ID, _
                                                Target.Library.Web.ConstantsManager.GetConstant( _
                                                    Me.Settings.CurrentApplication, _
                                                    "WebNavMenuItem.EditUserRoles"), _
                                                currentApp)

                canEditAdministrativeAreas = _
                    SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                                currentUser.ID, _
                                                Target.Library.Web.ConstantsManager.GetConstant( _
                                                    Me.Settings.CurrentApplication, _
                                                    "WebNavMenuItem.EditUserAdministrativeAreas"), _
                                                currentApp)

                ' get custom login interface
                customInterface = SecurityBL.GetCustomInterface(Me.Settings)
                If Not customInterface Is Nothing Then
                    replacementLogin = TryCast(customInterface, IReplacementLogin)
                End If

                ' check the email address of existing users if it has been changed

                If _user.Email.ToLower() <> txtEmail.Text.ToLower() Then
                    ' check if custom interface is to be used or not
                    If replacementLogin Is Nothing Then
                        msg = WebSecurityUser.FetchList(Me.DbConnection, users, , , txtEmail.Text, , , , Convert.ToInt32(currentApp))
                        If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                        If users.Count > 0 Then
                            litPageError.Text = "The user could not be saved because the email address you have specified is already in use."
                            e.Cancel = True
                            Exit Sub
                        End If
                    Else
                        msg = replacementLogin.EmailAddressInUse(currentUser.ID, txtEmail.Text)
                        If Not msg.Success Then
                            litPageError.Text = msg.Message
                            e.Cancel = True
                            Exit Sub
                        End If
                    End If
                End If


                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    ' save user
                    With _user
                        .DbTransaction = trans
                        .FirstName = txtFirstName.Text
                        .Surname = txtSurname.Text
                        If (replacementLogin Is Nothing) Then
                            .Email = txtEmail.Text
                        Else
                            msg = replacementLogin.SaveEmailAddress(.ID, txtEmail.Text)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    ' SAVE USER ROLES
                    If canEditRoles Then
                        ' get currently granted roles
                        msg = SecurityBL.GetGrantedRoles(Nothing, trans, _user.ID, grantedRoles)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' revoke any roles that are not now granted 
                        For Each role As WebSecurityRole In grantedRoles
                            Dim revoke As Boolean = True
                            For Each item As ListItem In dlRoles.DestList.Items
                                If Utils.ToInt32(item.Value) = role.ID Then
                                    revoke = False
                                    Exit For
                                End If
                            Next
                            If revoke Then
                                msg = SecurityBL.RevokeRole(trans, userID, role.ID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next

                        ' grant any new roles that weren't there before
                        For Each item As ListItem In dlRoles.DestList.Items
                            Dim grant As Boolean = True
                            For Each role As WebSecurityRole In grantedRoles
                                If role.ID = Utils.ToInt32(item.Value) Then
                                    grant = False
                                    Exit For
                                End If
                            Next
                            If grant Then
                                msg = SecurityBL.GrantRole(trans, userID, Utils.ToInt32(item.Value))
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next
                    End If

                    If canEditAdministrativeAreas Then
                        ' get currently granted administrative areas
                        msg = SecurityBL.GetGrantedAdministrativeAreas(Nothing, trans, _user.ID, grantedAdministrativeAreas)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' revoke any roles that are not now granted 
                        For Each adminArea As AdministrativeSector In grantedAdministrativeAreas
                            Dim revoke As Boolean = True
                            For Each item As ListItem In dlAdministrativeAreas.DestList.Items
                                If Utils.ToInt32(item.Value) = adminArea.ID Then
                                    revoke = False
                                    Exit For
                                End If
                            Next
                            If revoke Then
                                msg = SecurityBL.RevokeAdministrativeArea(trans, userID, adminArea.ID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next

                        ' grant any new roles that weren't there before
                        For Each item As ListItem In dlAdministrativeAreas.DestList.Items
                            Dim grant As Boolean = True
                            For Each adminArea As AdministrativeSector In grantedAdministrativeAreas
                                If adminArea.ID = Utils.ToInt32(item.Value) Then
                                    grant = False
                                    Exit For
                                End If
                            Next
                            If grant Then
                                msg = SecurityBL.GrantAdministrativeArea(trans, userID, Utils.ToInt32(item.Value))
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next
                    End If

                    trans.Commit()

                    SecurityBL.ClearCacheByUser(e.ItemID)
                    NavigationBL.ClearCacheByUser(e.ItemID)

                    Response.Redirect(Request.Url.ToString())

                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End Try

            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " ValidateSecurity "

        Private Sub ValidateSecurity(ByVal user As WebSecurityUser, ByVal currentUser As WebSecurityUser)

            Dim canViewOtherExternalUsers As Boolean

            canViewOtherExternalUsers = _
                    Me.UserHasMenuItemCommandInAnyMenuItem( _
                        Target.Library.Web.ConstantsManager.GetConstant( _
                            Me.Settings.CurrentApplication, _
                            "WebNavMenuItemCommand.Users.ViewOtherExternalUsers") _
                    )

            ' ensure requested user application ID matches current application ID
            If user.ApplicationID <> Me.Settings.CurrentApplicationID Then WebUtils.DisplayAccessDenied()
            ' if the user cannot view other external users, ensure requested user has the same external user ID as the current user
            If Not canViewOtherExternalUsers AndAlso user.ExternalUserID <> currentUser.ExternalUserID Then WebUtils.DisplayAccessDenied()
            ' un-activated users should not be edited with this screen
            If user.Status = WebSecurityUserStatus.Created Then WebUtils.DisplayAccessDenied()

        End Sub

#End Region

#Region " ChangeStatus_Command "

        Protected Sub ChangeStatus_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)

            Dim newStatus As WebSecurityUserStatus = [Enum].Parse(GetType(WebSecurityUserStatus), CType(e.CommandArgument, String))
            Dim userID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim msg As ErrorMessage

            FetchUser(userID)

            ' change the status
            _user.Status = newStatus
            msg = _user.Save()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' Password change notification email for extranet 
            msg = New ErrorMessage
            If (Me.Settings.CurrentApplicationID = ApplicationName.AbacusExtranet) Then
                msg = SecurityBL.NotifyAccountStatusChange(Me.DbConnection, _user)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If


            Response.Redirect(Request.Url.ToString())
        End Sub

#End Region

#Region " FetchUser "

        Private Sub FetchUser(ByVal userID As Integer)
            Dim msg As ErrorMessage
            _user = New WebSecurityUser(Me.DbConnection)
            msg = _user.Fetch(userID)
            If msg.Success = False Then
                If msg.Number = "E0513" Then
                    WebUtils.DisplayAccessDenied()
                Else
                    WebUtils.DisplayError(msg)
                End If
            End If
        End Sub

#End Region

#Region " PopulateScreen "

        Private Sub PopulateScreen()

            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim grantedRoles As WebSecurityRoleCollection = Nothing
            Dim availableRoles As WebSecurityRoleCollection = Nothing
            Dim canEditRoles As Boolean
            Dim allRolesAvailable As Boolean
            Dim customInterface As ICustomSecurity = Nothing
            Dim replacementLogin As IReplacementLogin = Nothing
            Dim grantedAdministrativeAreas As AdministrativeSectorCollection = Nothing
            Dim availableAdministrativeAreas As AdministrativeSectorCollection = Nothing
            Dim canEditAdministrativeAreas As Boolean

            canEditRoles = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            currentUser.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant( _
                                                Me.Settings.CurrentApplication, _
                                                "WebNavMenuItem.EditUserRoles"), _
                                            currentApp)

            canEditAdministrativeAreas = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            currentUser.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant( _
                                                Me.Settings.CurrentApplication, _
                                                "WebNavMenuItem.EditUserAdministrativeAreas"), _
                                            currentApp)

            allRolesAvailable = _
                    Me.UserHasMenuItemCommandInAnyMenuItem( _
                        Target.Library.Web.ConstantsManager.GetConstant( _
                            Me.Settings.CurrentApplication, _
                            "WebNavMenuItemCommand.EditUserRoles.AllRolesAvailable") _
                    )

            'get custom interface
            customInterface = SecurityBL.GetCustomInterface(Me.Settings)
            If Not customInterface Is Nothing Then
                replacementLogin = TryCast(customInterface, IReplacementLogin)
            End If

            With _user

                ' details
                txtFirstName.Text = .FirstName
                txtSurname.Text = .Surname
                If (replacementLogin Is Nothing) Then
                    txtEmail.Text = .Email
                Else
                    msg = replacementLogin.GetEmailAddress(_user, txtEmail.Text)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                txtStatus.Text = Utils.SplitOnCapitals([Enum].GetName(GetType(WebSecurityUserStatus), .Status))

                ' roles
                If canEditRoles Then
                    ' granted
                    msg = SecurityBL.GetGrantedRoles(Me.DbConnection, Nothing, _user.ID, grantedRoles)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    dlRoles.DestList.Items.Clear()
                    For Each role As WebSecurityRole In grantedRoles
                        With dlRoles.DestList.Items
                            .Add(New ListItem(role.Name, role.ID))
                        End With
                    Next
                    ' available
                    If allRolesAvailable Then
                        msg = WebSecurityRole.FetchList(Me.DbConnection, availableRoles, String.Empty, String.Empty, , currentApp)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' remove those already granted
                        Dim grantedRoleIDs() As Integer = _
                            (From r As WebSecurityRole In grantedRoles _
                            Select r.ID).ToArray()

                        Dim tempAvailableRoles() As WebSecurityRole = _
                            (From r As WebSecurityRole In availableRoles _
                            Where Not grantedRoleIDs.Contains(r.ID) _
                            Select r).ToArray()

                        availableRoles.Clear()
                        availableRoles.AddRange(tempAvailableRoles)

                        ' populate the list
                        dlRoles.SrcList.Items.Clear()
                        For Each role As WebSecurityRole In availableRoles
                            With dlRoles.SrcList.Items
                                .Add(New ListItem(role.Name, role.ID))
                            End With
                        Next
                    End If
                Else
                    tabRoles.Visible = False
                    tabRoles.HeaderText = String.Empty
                End If

                ' administrative area
                If canEditAdministrativeAreas Then
                    ' granted
                    msg = SecurityBL.GetGrantedAdministrativeAreas(Me.DbConnection, Nothing, _user.ID, grantedAdministrativeAreas)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    dlAdministrativeAreas.DestList.Items.Clear()
                    For Each adminArea As AdministrativeSector In grantedAdministrativeAreas
                        With dlAdministrativeAreas.DestList.Items
                            .Add(New ListItem(adminArea.Title, adminArea.ID))
                        End With
                    Next
                    ' available
                    msg = AdministrativeSector.FetchList(conn:=Me.DbConnection, list:=availableAdministrativeAreas, _
                                                         auditLogTitle:=String.Empty, auditUserName:=String.Empty, _
                                                         redundant:=TriState.False)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' remove those already granted
                    Dim grantedAdminAreasIDs() As Integer = _
                                (From r As AdministrativeSector In grantedAdministrativeAreas _
                                Select r.ID).ToArray()

                    Dim tempAdminAreasIDs() As AdministrativeSector = _
                                (From r As AdministrativeSector In availableAdministrativeAreas _
                                Where Not grantedAdminAreasIDs.Contains(r.ID) _
                                Select r).ToArray()

                    availableAdministrativeAreas.Clear()
                    availableAdministrativeAreas.AddRange(tempAdminAreasIDs)

                        ' populate the list
                    dlAdministrativeAreas.SrcList.Items.Clear()

                    For Each adminArea As AdministrativeSector In availableAdministrativeAreas
                        With dlAdministrativeAreas.SrcList.Items
                            .Add(New ListItem(adminArea.Title, adminArea.ID))
                        End With
                    Next
                Else
                    tabAdministrativeArea.Visible = False
                    tabAdministrativeArea.HeaderText = String.Empty
                End If

                ' info
                If Utils.IsDate(.CreateDate) Then txtCreateDate.Text = .CreateDate
                If Utils.IsDate(.LastLoginDate) Then txtLastLoginDate.Text = .LastLoginDate
                txtLastLoginUserAgent.Text = WebUtils.EncodeOutput(.LastLoginUserAgent)
                txtLastLoginIPAddress.Text = WebUtils.EncodeOutput(.LastLoginIPAddress)
                If Utils.IsDate(.PasswordDate) Then txtPasswordDate.Text = .PasswordDate
                If Utils.IsDate(.LastFailedLoginDate) Then txtLastFailedLoginDate.Text = .LastFailedLoginDate
                txtRejectedLogins.Text = .RejectedLogins

            End With

        End Sub

#End Region

#Region " btnReactivate_Click "

        Private Sub btnReactivate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReactivate.Click
            Response.Redirect(String.Format("UserActivate.aspx?id={0}", Request.QueryString("id")))
        End Sub

#End Region

#Region " StdButtons_AddCustomControls "

        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            With _btnReports
                .ID = "_btnReports"
                .Value = "Reports"
            End With
            controls.Add(_btnReports)

            With CType(cpe, AjaxControlToolkit.CollapsiblePanelExtender)
                .ExpandControlID = _btnReports.ClientID
                .CollapseControlID = .ExpandControlID
                .Collapsed = True
            End With

        End Sub

#End Region

#Region " SetupReports "

        Private Sub SetupReports(ByVal userID As Integer)

            Dim permissionsReportID As Integer = _
                Utils.ToInt32( _
                    Target.Library.Web.ConstantsManager.GetConstant( _
                        String.Format("{0}.WebReport.SecurityUserPermissions", Me.Settings.CurrentApplication) _
                    ) _
                )

            ' if we don't have any reports configured for the current application, hide the relevant controls
            If permissionsReportID <= 0 Then
                cpe.Enabled = False
                pnlReports.Visible = False
            Else
                AddHandler _stdBut.AddCustomControls, AddressOf StdButtons_AddCustomControls

                With lstReports
                    .Rows = 3
                    .Attributes.Add("onchange", "lstReports_Change();")
                    With .Items
                        .Add(New ListItem("User permissions", divPermissions.ClientID))
                    End With
                End With

                ' permissions
                With CType(ctlPermissions, IReportsButton)
                    .ReportID = permissionsReportID
                    .Parameters.Add("intUserID", userID)
                End With
            End If

        End Sub

#End Region

#Region " Page_PreRenderComplete "

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID

            If currentApp = ApplicationName.AbacusIntranet Then
                pnlChangeStatus.Disabled = False
                btnChangePassword.Enabled = False
                btnActive.Enabled = False
                btnSuspended.Enabled = False
                btnLocked.Enabled = False
                btnReactivate.Enabled = True
            Else
                pnlChangeStatus.Disabled = False
                btnChangePassword.Enabled = True
                btnActive.Enabled = True
                btnSuspended.Enabled = True
                btnLocked.Enabled = True
                btnReactivate.Enabled = True
            End If

        End Sub

#End Region

    End Class

End Namespace