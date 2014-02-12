Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Navigation
Imports Target.Web.Apps.Navigation.Collections
Imports Target.Web.Apps.Security.Collections
Imports Target.Web.Apps.Reports
Imports Target.Web.Apps.Reports.Collections
Imports Target.Web.Apps.Licensing
Imports Target.Web.Apps.Licensing.Collections
Imports Target.Web.Library


Namespace Apps.Security.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Security.Admin.RoleEdit
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows admin users to edit security role.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MoTahir     17/06/2011  A4WA6878 - Undo fix applied in A4WA6829.
    '''     MoTahir     03/06/2011  A4WA6829 - Create batch in Creditor Payment Wizard not visible in view jobs.
    '''     MikeVO      16/05/2011  SDS issue #655 - fix (to #477/#509/#594) when creating a new role.
    '''     MikeVO      18/04/2011  SDS issue #594 - ensure expired menu items are still available.
    '''     MikeVO      06/04/2011  SDS issues #477/509 - Various changed to support ModuleLicence changes and correct logic in PopulateTreeView().
    '''     MoTahir     08/10/2010  Issue 134 Sharepoint 
    '''     MikeVO      01/09/2010  Added validation summary.
    '''     MoTahir     28/06/2010  D11829 - Licensing
    '''     ColinD      04/05/2010  A4WA#6223 - added read only version of tree view roleTree using images instead of check boxes
    '''     MikeVO      27/01/2010  D11435 - ensure the available JobTypes are only those for the current site, not everything.
    '''     MikeVO      11/01/2010  D11435 - Added JobTypes tab for role/user-jobs security.
    '''                             Corrected logic flaws in PopulateServiceGroups(), PopulateProviderTypes() & PopulateReportCategories() methods.
    '''     MikeVO      23/10/2009  D11710 - added Report Categories tab.
    '''                             Corrected delete method where linked service groups and provider type were not being deleted.
    '''                             Corrected audit logging setup in Page_Load.
    '''                             Added audit logging to linked provider types and service groups.
    '''     MoTahir     21/09/2009  D11678 - Added new tab Provider Types and added new dual list 
    '''                             to manage the security for new service groups
    '''     MoTahir     18/09/2009 - fixed bug in service groups available list index out of range error
    '''     MoTahir     19/08/2009  D11671 - Added new tab and Service Groups and added new dual list
    '''                             to manage the security for new service groups
    '''     MikeVO      10/07/2009  D11630 - added reports.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      10/03/2008  A4WA#4522 - view state fix.
    '''     MikeVO      07/03/2007  Added user level support.
    ''' 	[Mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class RoleEdit
        Inherits Target.Web.Apps.BasePage

        Private Const TREEVIEW_NODE_MENU_ITEM As String = "MenuItem"
        Private Const TREEVIEW_NODE_MENU_ITEM_COMMAND As String = "MenuItemCommand"

        Private _stdBut As StdButtonsBase
        Private _btnReports As HtmlInputButton = New HtmlInputButton("button")
        Private _isTreeviewEditable As Boolean = False
        Private _menuItemHash As Dictionary(Of Integer, MenuItemLicensedState) = Nothing
        Private _expiredModulesHash As Hashtable = Nothing, _webModuleJobTypeHash As Hashtable = Nothing
        Private _isSystemRole As Boolean

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.EditRole"), "Admin: Security: Edit Role")
            Me.ShowValidationSummary = True

            Dim roleID As Integer = Utils.ToInt32(Request.QueryString("id"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            SetupReports(roleID)

        End Sub

#End Region
        
#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim msg As ErrorMessage

            With txtDescription.TextBox
                .TextMode = TextBoxMode.MultiLine
                .Rows = 3
                .Columns = 80
            End With

            With _stdBut
                .AllowBack = True
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant(Me.Settings.CurrentApplication, "WebNavMenuItemCommand.EditRole.AddNew"))
                .ShowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant(Me.Settings.CurrentApplication, "WebNavMenuItemCommand.EditRole.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant(Me.Settings.CurrentApplication, "WebNavMenuItemCommand.EditRole.Delete"))
                .AllowFind = False
                .EditableControls.Add(tabServiceGroups.Controls)
                .EditableControls.Add(tabProviderTypes.Controls)
                .EditableControls.Add(tabReportCategories.Controls)
                .EditableControls.Add(tabJobTypes.Controls)
                .EditableControls.Add(tabDocumentTypes.Controls)
                .AuditLogTableNames.Add("WebSecurityRole")
                .AuditLogTableNames.Add("WebSecurityRole_WebNavMenuItem")
                .AuditLogTableNames.Add("WebSecurityRole_WebNavMenuItemCommand")
                .AuditLogTableNames.Add("WebSecurityRole_ServiceGroup")
                .AuditLogTableNames.Add("WebSecurityRole_ProviderType")
                .AuditLogTableNames.Add("WebSecurityRole_WebReportCategory")
                .AuditLogTableNames.Add("WebSecurityRole_JobType")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            WebUtils.TreeViewClientCascadingCheckboxes(Me.roleTree, "ANY_NO_UNTICK")

            Me.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            ' get fast lookups of all menu items and their licensed state, expired modules and modules vs. jobs
            msg = ModuleLicence.GetWebNavMenuItemLookup(Me.DbConnection, _menuItemHash)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            msg = ModuleLicence.GetWebModule_WebModuleLicenceExpiredLookup(Me.DbConnection, _expiredModulesHash)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            msg = ModuleLicence.GetWebModule_WebModuleJobTypeLookup(Me.DbConnection, _webModuleJobTypeHash)
            If Not msg.Success Then WebUtils.DisplayError(msg)


            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Apps.UserControls.ServiceOrderReports", _
         Target.Library.Web.Utils.WrapClientScript(String.Format("Report_lstReportlistId='{0}';", _
                                                                 lstReports.ClientID _
                                                    )) _
                                            )
        End Sub

#End Region

#Region " EditClicked "

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            ' edit has been clicked so we want the tree view to be editable
            _isTreeviewEditable = True
            FindClicked(e)
            txtName.Enabled = Not _isSystemRole
            txtDescription.Enabled = True

        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim role As WebSecurityRole
            Dim roleMenuItemsHash As Hashtable = Nothing, roleMenuItemCommandsHash As Hashtable = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim roleId As Integer

            role = New WebSecurityRole(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With role
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If .ApplicationID <> Me.Settings.CurrentApplicationID Then WebUtils.DisplayAccessDenied()

                _isSystemRole = .SystemRole
                txtName.Text = .Name
                txtDescription.Text = .Description

                _stdBut.AllowDelete = Not _isSystemRole

                roleId = .ID
            End With

            txtName.Enabled = False
            txtDescription.Enabled = False

            ' POPULATE THE TREE VIEW

            ' get fast lookups of all menu items and commands currently linked to this role
            msg = SecurityBL.GetRoleMenuItemsLookup(Me.DbConnection, e.ItemID, roleMenuItemsHash)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            msg = SecurityBL.GetRoleMenuItemCommandsLookup(Me.DbConnection, e.ItemID, roleMenuItemCommandsHash)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            roleTree.Nodes.Clear()

            PopulateTreeView(roleMenuItemsHash, _
                             roleMenuItemCommandsHash, _
                             NavigationBL.ROOT_MENU_ID, _
                             roleTree.Nodes, _
                             Nothing, _
                             _menuItemHash)
            roleTree.CollapseAll()

            PopulateServiceGroups(roleId)
            PopulateProviderTypes(roleId)
            PopulateReportCategories(roleId)
            PopulateJobTypes(roleId, _webModuleJobTypeHash)
            PopulateDocumentTypes(roleId)

        End Sub

#End Region

#Region " PopulateServiceGroups "

        Private Sub PopulateServiceGroups(ByVal roleId As Integer)

            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim grantedServiceGroups As vwWebSecurityRole_ServiceGroupCollection = Nothing
            Dim availableServiceGroups As ServiceGroupCollection = Nothing, alreadySelected As ServiceGroupCollection = Nothing
            Dim canEditRoleServiceGroups As Boolean
            Dim allServiceGroupsAvailable As Boolean

            canEditRoleServiceGroups = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            currentUser.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant( _
                                                Me.Settings.CurrentApplication, _
                                                "WebNavMenuItem.EditRoleServiceGroups"), _
                                            currentApp)

            allServiceGroupsAvailable = _
                    Me.UserHasMenuItemCommandInAnyMenuItem( _
                        Target.Library.Web.ConstantsManager.GetConstant( _
                            Me.Settings.CurrentApplication, _
                            "WebNavMenuItemCommand.EditRoleServiceGroups.AllServiceGroupsAvailable") _
                    )

            ' roles
            If canEditRoleServiceGroups Then
                ' granted
                msg = SecurityBL.GetGrantedRoleServiceGroups(Me.DbConnection, Nothing, roleId, grantedServiceGroups)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                dlServiceGroups.DestList.Items.Clear()
                For Each servicegroup As vwWebSecurityRole_ServiceGroup In grantedServiceGroups
                    With dlServiceGroups.DestList.Items
                        .Add(New ListItem(servicegroup.ServiceGroupDescription, servicegroup.ServiceGroupID))
                    End With
                Next
                ' available
                If allServiceGroupsAvailable Then
                    msg = ServiceGroup.FetchList(Me.DbConnection, availableServiceGroups, String.Empty, String.Empty)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    availableServiceGroups.Sort(New CollectionSorter("Description", SortDirection.Ascending))

                    ' remove those already selected
                    alreadySelected = New ServiceGroupCollection()
                    For Each servicegroup As vwWebSecurityRole_ServiceGroup In grantedServiceGroups
                        For Each aservicegroup As ServiceGroup In availableServiceGroups
                            If aservicegroup.ID = servicegroup.ServiceGroupID Then
                                alreadySelected.Add(aservicegroup)
                            End If
                        Next
                    Next

                    Dim index As Integer = alreadySelected.Count - 1
                    While index >= 0
                        availableServiceGroups.Remove(alreadySelected(index))
                        index -= 1
                    End While

                    dlServiceGroups.SrcList.Items.Clear()
                    For Each servicegroup As ServiceGroup In availableServiceGroups
                        With dlServiceGroups.SrcList.Items
                            .Add(New ListItem(servicegroup.Description, servicegroup.ID))
                        End With
                    Next
                End If
            Else
                tabServiceGroups.Visible = False
                tabServiceGroups.HeaderText = String.Empty
            End If
        End Sub
#End Region

#Region " PopulateProviderTypes "

        Private Sub PopulateProviderTypes(ByVal roleId As Integer)

            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim grantedProviderTypes As vwWebSecurityRole_ProviderTypeCollection = Nothing
            Dim availableProviderTypes As ProviderTypeCollection = Nothing, alreadySelected As ProviderTypeCollection = Nothing
            Dim canEditRoleProviderTypes As Boolean
            Dim allProviderTypesAvailable As Boolean

            canEditRoleProviderTypes = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            currentUser.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant( _
                                                Me.Settings.CurrentApplication, _
                                                "WebNavMenuItem.EditRoleProviderTypes"), _
                                            currentApp)

            allProviderTypesAvailable = _
                    Me.UserHasMenuItemCommandInAnyMenuItem( _
                        Target.Library.Web.ConstantsManager.GetConstant( _
                            Me.Settings.CurrentApplication, _
                            "WebNavMenuItemCommand.EditRoleProviderTypes.AllProviderTypesAvailable") _
                    )

            ' roles
            If canEditRoleProviderTypes Then
                ' granted
                msg = SecurityBL.GetGrantedRoleProviderTypes(Me.DbConnection, Nothing, roleId, grantedProviderTypes)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                dlProviderTypes.DestList.Items.Clear()
                For Each providertype As vwWebSecurityRole_ProviderType In grantedProviderTypes
                    With dlProviderTypes.DestList.Items
                        .Add(New ListItem(providertype.ProviderType, providertype.ProviderTypeID))
                    End With
                Next
                ' available
                If allProviderTypesAvailable Then
                    msg = ProviderType.FetchList(Me.DbConnection, availableProviderTypes)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    availableProviderTypes.Sort(New CollectionSorter("Description", SortDirection.Ascending))

                    ' remove those already selected
                    alreadySelected = New ProviderTypeCollection()
                    For Each providertype As vwWebSecurityRole_ProviderType In grantedProviderTypes
                        For Each aprovidertype As ProviderType In availableProviderTypes
                            If aprovidertype.ID = providertype.ProviderTypeID Then
                                alreadySelected.Add(aprovidertype)
                            End If
                        Next
                    Next

                    Dim index As Integer = alreadySelected.Count - 1
                    While index >= 0
                        availableProviderTypes.Remove(alreadySelected(index))
                        index -= 1
                    End While

                    dlProviderTypes.SrcList.Items.Clear()
                    For Each providertype As ProviderType In availableProviderTypes
                        With dlProviderTypes.SrcList.Items
                            .Add(New ListItem(providertype.Description, providertype.ID))
                        End With
                    Next
                End If
            Else
                tabProviderTypes.Visible = False
                tabProviderTypes.HeaderText = String.Empty
            End If
        End Sub
#End Region

#Region " PopulateReportCategories "

        Private Sub PopulateReportCategories(ByVal roleId As Integer)

            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim granted As vwWebSecurityRole_WebReportCategoryCollection = Nothing
            Dim available As WebReportCategoryCollection = Nothing, alreadySelected As WebReportCategoryCollection = Nothing
            Dim canEditRoleReportCategories As Boolean

            canEditRoleReportCategories = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            currentUser.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant( _
                                                Me.Settings.CurrentApplication, _
                                                "WebNavMenuItem.EditRoleReportCategories"), _
                                            currentApp)

            ' roles
            If canEditRoleReportCategories Then
                ' granted
                msg = SecurityBL.GetGrantedRoleReportCategories(Me.DbConnection, Nothing, roleId, granted)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                dlReportCategories.DestList.Items.Clear()
                For Each cat As vwWebSecurityRole_WebReportCategory In granted
                    With dlReportCategories.DestList.Items
                        .Add(New ListItem(cat.WebReportCategoryDescription, cat.WebReportCategoryID))
                    End With
                Next
                ' available
                msg = WebReportCategory.FetchList(Me.DbConnection, available)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                available.Sort(New CollectionSorter("Description", SortDirection.Ascending))
                ' remove those already selected
                alreadySelected = New WebReportCategoryCollection()
                For Each cat As vwWebSecurityRole_WebReportCategory In granted
                    For Each aCat As WebReportCategory In available
                        If aCat.ID = cat.WebReportCategoryID Then
                            alreadySelected.Add(aCat)
                        End If
                    Next
                Next

                Dim index As Integer = alreadySelected.Count - 1
                While index >= 0
                    available.Remove(alreadySelected(index))
                    index -= 1
                End While

                dlReportCategories.SrcList.Items.Clear()
                For Each cat As WebReportCategory In available
                    With dlReportCategories.SrcList.Items
                        .Add(New ListItem(cat.Description, cat.ID))
                    End With
                Next
            Else
                tabReportCategories.Visible = False
                tabReportCategories.HeaderText = String.Empty
            End If
        End Sub
#End Region

#Region " PopulateJobTypes "

        Private Sub PopulateJobTypes(ByVal roleId As Integer, ByVal webModuleJobTypes As Hashtable)

            Const SP_NAME_AVAILABLE_JOBS As String = "spxJobType_FetchListForSite"

            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim granted As vwWebSecurityRole_JobTypeCollection = Nothing
            Dim available As JobTypeCollection = Nothing, alreadySelected As JobTypeCollection = Nothing
            Dim canEditRoleJobs As Boolean
            Dim spParams As SqlParameter()
            Dim reader As SqlDataReader = Nothing
            Dim siteID As String = ConfigurationManager.AppSettings("SiteID")
            Dim tmpAvailableJobTypes As New JobTypeCollection()
            Dim jobTypeAlreadySelected As Boolean = False

            canEditRoleJobs = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            currentUser.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant( _
                                                Me.Settings.CurrentApplication, _
                                                "WebNavMenuItem.EditRoleJobs"), _
                                            currentApp)

            ' roles
            If canEditRoleJobs Then
                ' granted
                msg = SecurityBL.GetGrantedRoleJobTypes(Me.DbConnection, Nothing, roleId, granted)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                dlJobTypes.DestList.Items.Clear()
                For Each jt As vwWebSecurityRole_JobType In granted
                    With dlJobTypes.DestList.Items
                        .Add(New ListItem(jt.JobTypeName, jt.JobTypeID))
                    End With
                Next

                ' available to site
                Try
                    spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_AVAILABLE_JOBS, False)
                    spParams(0).Value = siteID

                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_AVAILABLE_JOBS, spParams)

                    available = New JobTypeCollection()
                    While reader.Read()
                        Dim jt As JobType = New JobType(reader)
                        available.Add(jt)
                    End While
                    available.Sort(New CollectionSorter("Name", SortDirection.Ascending))

                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                    WebUtils.DisplayError(msg)
                Finally
                    SqlHelper.CloseReader(reader)
                End Try

                ' remove those already selected
                alreadySelected = New JobTypeCollection()
                For Each jt As vwWebSecurityRole_JobType In granted
                    For Each aJt As JobType In available
                        If aJt.ID = jt.JobTypeID Then
                            alreadySelected.Add(aJt)
                        End If
                    Next
                Next

                ' loop each available job type
                For Each availableJobType As JobType In available
                    jobTypeAlreadySelected = False
                    ' loop each already selected job type
                    For Each alreadySelectedJobType As JobType In alreadySelected
                        ' if the available job type is already selected then exit
                        If availableJobType.ID = alreadySelectedJobType.ID Then
                            jobTypeAlreadySelected = True
                            Exit For
                        End If
                    Next
                    ' if the job type isnt selected then add to available list
                    If jobTypeAlreadySelected = False Then
                        tmpAvailableJobTypes.Add(availableJobType)
                    End If
                Next

                available = tmpAvailableJobTypes

                dlJobTypes.SrcList.Items.Clear()
                For Each jt As JobType In available
                    With dlJobTypes.SrcList.Items
                        If Not webModuleJobTypes.ContainsKey(jt.ID) Then .Add(New ListItem(jt.Name, jt.ID))
                    End With
                Next
            Else
                tabJobTypes.Visible = False
                tabJobTypes.HeaderText = String.Empty
            End If
        End Sub
#End Region

#Region " PopulateDocumentTypes "

        Private Sub PopulateDocumentTypes(ByVal roleId As Integer)

            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim grantedDocumentTypes As vwWebSecurityRole_DocumentTypeCollection = Nothing
            Dim availableDocumentTypes As DocumentTypeCollection = Nothing
            Dim alreadySelected As DocumentTypeCollection = Nothing
            Dim canEditRoleDocumentTypes As Boolean

            canEditRoleDocumentTypes = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            currentUser.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant( _
                                                Me.Settings.CurrentApplication, _
                                                "WebNavMenuItem.EditRoleDocumentTypes"), _
                                            currentApp)

            ' roles
            If canEditRoleDocumentTypes Then
                ' granted
                msg = SecurityBL.GetGrantedRoleDocumentTypes(Me.DbConnection, Nothing, roleId, grantedDocumentTypes)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                dlDocumentTypes.DestList.Items.Clear()
                For Each docType As vwWebSecurityRole_DocumentType In grantedDocumentTypes
                    With dlDocumentTypes.DestList.Items
                        .Add(New ListItem(docType.DocumentType, docType.DocumentTypeID))
                    End With
                Next

                ' available
                msg = DocumentType.FetchList(Me.DbConnection, availableDocumentTypes, Nothing, Nothing)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                availableDocumentTypes.Sort(New CollectionSorter("Description", SortDirection.Ascending))

                ' remove those already selected
                alreadySelected = New DocumentTypeCollection()
                For Each docType As vwWebSecurityRole_DocumentType In grantedDocumentTypes
                    For Each aDocType As DocumentType In availableDocumentTypes
                        If aDocType.ID = docType.DocumentTypeID Then
                            alreadySelected.Add(aDocType)
                        End If
                    Next
                Next

                Dim index As Integer = alreadySelected.Count - 1
                While index >= 0
                    availableDocumentTypes.Remove(alreadySelected(index))
                    index -= 1
                End While

                dlDocumentTypes.SrcList.Items.Clear()
                For Each docType As DocumentType In availableDocumentTypes
                    With dlDocumentTypes.SrcList.Items
                        .Add(New ListItem(docType.Description, docType.ID))
                    End With
                Next
            Else
                tabDocumentTypes.Visible = False
                tabDocumentTypes.HeaderText = String.Empty
            End If
        End Sub
#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtName.Text = String.Empty
                txtDescription.Text = String.Empty
                ' populate an empty tree
                roleTree.Nodes.Clear()
                PopulateTreeView(New Hashtable(), _
                                 New Hashtable(), _
                                 NavigationBL.ROOT_MENU_ID, _
                                 roleTree.Nodes, _
                                 Nothing, _
                                 _menuItemHash)
                roleTree.CollapseAll()
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            _isTreeviewEditable = True
            ' populate an empty tree
            roleTree.Nodes.Clear()
            PopulateTreeView(New Hashtable(), _
                             New Hashtable(), _
                             NavigationBL.ROOT_MENU_ID, _
                             roleTree.Nodes, _
                             Nothing, _
                             _menuItemHash)
            roleTree.CollapseAll()
        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim auditLogTitle As String, auditLogUsername As String
            Dim menuItems As WebSecurityRole_WebNavMenuItemCollection = Nothing
            Dim menuItemCommands As WebSecurityRole_WebNavMenuItemCommandCollection = Nothing
            Dim userRoles As WebSecurityUser_WebSecurityRoleCollection = Nothing
            Dim serviceGroups As WebSecurityRole_ServiceGroupCollection = Nothing
            Dim providerTypes As WebSecurityRole_ProviderTypeCollection = Nothing
            Dim reportCategories As WebSecurityRole_WebReportCategoryCollection = Nothing
            Dim jobTypes As WebSecurityRole_JobTypeCollection = Nothing
            Dim docTypes As WebSecurityRole_DocumentTypeCollection = Nothing

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)
                auditLogTitle = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
                auditLogUsername = currentUser.ExternalUsername

                ' is role in use?
                msg = WebSecurityUser_WebSecurityRole.FetchList(trans, userRoles, , e.ItemID)
                If Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End If
                If userRoles.Count > 0 Then
                    litPageError.Text = "This role cannot be deleted because it is in use by one or more user(s)."
                    SqlHelper.RollbackTransaction(trans)
                    e.Cancel = True
                    Exit Sub
                End If

                ' delete menu items
                msg = WebSecurityRole_WebNavMenuItem.FetchList(trans, menuItems, auditLogUsername, auditLogTitle, e.ItemID)
                If Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End If
                For Each mi As WebSecurityRole_WebNavMenuItem In menuItems
                    mi.DbTransaction = trans
                    msg = mi.Delete(e.ItemID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If
                Next

                ' delete menu item commands
                msg = WebSecurityRole_WebNavMenuItemCommand.FetchList(trans, menuItemCommands, auditLogUsername, auditLogTitle, e.ItemID)
                If Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End If
                For Each mic As WebSecurityRole_WebNavMenuItemCommand In menuItemCommands
                    mic.DbTransaction = trans
                    msg = mic.Delete(e.ItemID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If
                Next

                ' delete service groups
                msg = WebSecurityRole_ServiceGroup.FetchList(trans, serviceGroups, auditLogUsername, auditLogTitle, e.ItemID)
                If Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End If
                For Each sg As WebSecurityRole_ServiceGroup In serviceGroups
                    sg.DbTransaction = trans
                    msg = sg.Delete(e.ItemID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If
                Next

                ' delete provider types
                msg = WebSecurityRole_ProviderType.FetchList(trans, providerTypes, auditLogUsername, auditLogTitle, e.ItemID)
                If Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End If
                For Each pt As WebSecurityRole_ProviderType In providerTypes
                    pt.DbTransaction = trans
                    msg = pt.Delete(e.ItemID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If
                Next

                ' delete report categories
                msg = WebSecurityRole_WebReportCategory.FetchList(trans, reportCategories, auditLogUsername, auditLogTitle, e.ItemID)
                If Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End If
                For Each rc As WebSecurityRole_WebReportCategory In reportCategories
                    rc.DbTransaction = trans
                    msg = rc.Delete(e.ItemID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If
                Next

                ' delete document types
                msg = WebSecurityRole_DocumentType.FetchList(trans, docTypes, auditLogUsername, auditLogTitle, e.ItemID)
                If Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End If
                For Each dt As WebSecurityRole_DocumentType In docTypes
                    dt.DbTransaction = trans
                    msg = dt.Delete(e.ItemID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If
                Next

                ' delete job types
                msg = WebSecurityRole_JobType.FetchList(trans, jobTypes, auditLogUsername, auditLogTitle, e.ItemID)
                If Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End If
                For Each jt As WebSecurityRole_JobType In jobTypes
                    jt.DbTransaction = trans
                    msg = jt.Delete(e.ItemID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If
                Next

                ' delete the role itself
                msg = WebSecurityRole.Delete(trans, auditLogUsername, auditLogTitle, e.ItemID)
                If Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End If

                trans.Commit()

                ' clear screen
                e.ItemID = 0
                CancelClicked(e)

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                SqlHelper.RollbackTransaction(trans)
                WebUtils.DisplayError(msg)
            End Try



        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim role As WebSecurityRole
            Dim roleID As Integer
            Dim grantedServiceGroups As vwWebSecurityRole_ServiceGroupCollection = Nothing
            Dim grantedProviderTypes As vwWebSecurityRole_ProviderTypeCollection = Nothing
            Dim grantedReportCategories As vwWebSecurityRole_WebReportCategoryCollection = Nothing
            Dim grantedJobTypes As vwWebSecurityRole_JobTypeCollection = Nothing
            Dim grantedDocumentTypes As vwWebSecurityRole_DocumentTypeCollection = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim roleMenuItemsHash As Hashtable = Nothing, roleMenuItemCommandsHash As Hashtable = Nothing
            Dim auditLogTitle As String, auditLogUsername As String
            Dim canEditRoleServiceGroups, canEditRoleProviderTypes As Boolean, canEditReportCategories As Boolean, canEditReportJobTypes As Boolean, canEditReportDocumentTypes As Boolean
            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID

            If Me.IsValid Then
                Try
                    ' get fast lookups of all menu items and commands currently linked to this role
                    msg = SecurityBL.GetRoleMenuItemsLookup(Me.DbConnection, e.ItemID, roleMenuItemsHash)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    msg = SecurityBL.GetRoleMenuItemCommandsLookup(Me.DbConnection, e.ItemID, roleMenuItemCommandsHash)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    trans = SqlHelper.GetTransaction(Me.DbConnection)
                    auditLogTitle = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
                    auditLogUsername = currentUser.ExternalUsername

                    role = New WebSecurityRole(trans, auditLogUsername, auditLogTitle)
                    If e.ItemID > 0 Then
                        ' update
                        msg = role.Fetch(e.ItemID)
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        ' new
                        role.ApplicationID = Me.Settings.CurrentApplicationID
                    End If

                    With role
                        If Not .SystemRole Then .Name = txtName.Text
                        .Description = txtDescription.Text
                        msg = .Save()
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            WebUtils.DisplayError(msg)
                        End If
                        e.ItemID = .ID

                        ' process the tree
                        ProcessTreeView(roleTree.Nodes, e.ItemID, trans, roleMenuItemsHash, roleMenuItemCommandsHash, auditLogUsername, auditLogTitle)

                    End With

                    roleID = e.ItemID

                    ' Save Role Service Group

                    canEditRoleServiceGroups = _
                                        SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                           currentUser.ID, _
                                           Target.Library.Web.ConstantsManager.GetConstant( _
                                               Me.Settings.CurrentApplication, _
                                               "WebNavMenuItem.EditRoleServiceGroups"), _
                                           currentApp)


                    If canEditRoleServiceGroups Then
                        ' get currently granted service groups
                        msg = SecurityBL.GetGrantedRoleServiceGroups(Nothing, trans, roleID, grantedServiceGroups)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' revoke any roles that are not now granted 
                        For Each servicegroup As vwWebSecurityRole_ServiceGroup In grantedServiceGroups
                            Dim revoke As Boolean = True
                            For Each item As ListItem In dlServiceGroups.DestList.Items
                                If Utils.ToInt32(item.Value) = servicegroup.ServiceGroupID Then
                                    revoke = False
                                    Exit For
                                End If
                            Next
                            If revoke Then
                                msg = SecurityBL.RevokeServiceGroup(trans, _
                                                                    servicegroup.ID, _
                                                                    auditLogUsername, _
                                                                    auditLogTitle, _
                                                                    roleID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next

                        ' grant any new roles that weren't there before
                        For Each item As ListItem In dlServiceGroups.DestList.Items
                            Dim grant As Boolean = True
                            For Each servicegroup As vwWebSecurityRole_ServiceGroup In grantedServiceGroups
                                If servicegroup.ServiceGroupID = Utils.ToInt32(item.Value) Then
                                    grant = False
                                    Exit For
                                End If
                            Next
                            If grant Then
                                msg = SecurityBL.GrantRoleServiceGroup(trans, _
                                                                       roleID, _
                                                                       Utils.ToInt32(item.Value), _
                                                                       auditLogUsername, _
                                                                       auditLogTitle)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next
                    End If

                    ' Save Role Service Group End

                    ' Save Role Provider Types
                    canEditRoleProviderTypes = _
                                    SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                        currentUser.ID, _
                                    Target.Library.Web.ConstantsManager.GetConstant( _
                                        Me.Settings.CurrentApplication, _
                                        "WebNavMenuItem.EditRoleProviderTypes"), _
                                    currentApp)


                    If canEditRoleProviderTypes Then
                        ' get currently granted service groups
                        msg = SecurityBL.GetGrantedRoleProviderTypes(Nothing, trans, roleID, grantedProviderTypes)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' revoke any roles that are not now granted 
                        For Each providertype As vwWebSecurityRole_ProviderType In grantedProviderTypes
                            Dim revoke As Boolean = True
                            For Each item As ListItem In dlProviderTypes.DestList.Items
                                If Utils.ToInt32(item.Value) = providertype.ProviderTypeID Then
                                    revoke = False
                                    Exit For
                                End If
                            Next
                            If revoke Then
                                msg = SecurityBL.RevokeProviderType(trans, _
                                                                    providertype.ID, _
                                                                    auditLogUsername, _
                                                                    auditLogTitle, _
                                                                    roleID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next

                        ' grant any new roles that weren't there before
                        For Each item As ListItem In dlProviderTypes.DestList.Items
                            Dim grant As Boolean = True
                            For Each providertype As vwWebSecurityRole_ProviderType In grantedProviderTypes
                                If providertype.ProviderTypeID = Utils.ToInt32(item.Value) Then
                                    grant = False
                                    Exit For
                                End If
                            Next
                            If grant Then
                                msg = SecurityBL.GrantRoleProviderType(trans, _
                                                                       roleID, _
                                                                       Utils.ToInt32(item.Value), _
                                                                       auditLogUsername, _
                                                                       auditLogTitle)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next
                    End If

                    ' Save Role Provider Type End

                    ' Save Role Report Category

                    canEditReportCategories = _
                                        SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                           currentUser.ID, _
                                           Target.Library.Web.ConstantsManager.GetConstant( _
                                               Me.Settings.CurrentApplication, _
                                               "WebNavMenuItem.EditRoleReportCategories"), _
                                           currentApp)


                    If canEditReportCategories Then
                        ' get currently granted report categories
                        msg = SecurityBL.GetGrantedRoleReportCategories(Nothing, trans, roleID, grantedReportCategories)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' revoke any categories that are not now granted 
                        For Each cat As vwWebSecurityRole_WebReportCategory In grantedReportCategories
                            Dim revoke As Boolean = True
                            For Each item As ListItem In dlReportCategories.DestList.Items
                                If Utils.ToInt32(item.Value) = cat.WebReportCategoryID Then
                                    revoke = False
                                    Exit For
                                End If
                            Next
                            If revoke Then
                                msg = SecurityBL.RevokeReportCategory(trans, _
                                                                    cat.ID, _
                                                                    auditLogUsername, _
                                                                    auditLogTitle, _
                                                                    roleID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next

                        ' grant any new categories that weren't there before
                        For Each item As ListItem In dlReportCategories.DestList.Items
                            Dim grant As Boolean = True
                            For Each cat As vwWebSecurityRole_WebReportCategory In grantedReportCategories
                                If cat.WebReportCategoryID = Utils.ToInt32(item.Value) Then
                                    grant = False
                                    Exit For
                                End If
                            Next
                            If grant Then
                                msg = SecurityBL.GrantRoleReportCategory(trans, _
                                                                       roleID, _
                                                                       Utils.ToInt32(item.Value), _
                                                                       auditLogUsername, _
                                                                       auditLogTitle)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next
                    End If

                    ' Save Role Report Category End


                    ' Save Role JobType

                    canEditReportJobTypes = _
                                        SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                           currentUser.ID, _
                                           Target.Library.Web.ConstantsManager.GetConstant( _
                                               Me.Settings.CurrentApplication, _
                                               "WebNavMenuItem.EditRoleJobs"), _
                                           currentApp)


                    If canEditReportJobTypes Then
                        ' get currently granted job types
                        msg = SecurityBL.GetGrantedRoleJobTypes(Nothing, trans, roleID, grantedJobTypes)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' revoke any job types that are not now granted 
                        For Each jt As vwWebSecurityRole_JobType In grantedJobTypes
                            Dim revoke As Boolean = True
                            For Each item As ListItem In dlJobTypes.DestList.Items
                                If Utils.ToInt32(item.Value) = jt.JobTypeID Then
                                    revoke = False
                                    Exit For
                                End If
                            Next
                            If revoke Then
                                msg = SecurityBL.RevokeJobType(trans, _
                                                               jt.ID, _
                                                               auditLogUsername, _
                                                               auditLogTitle, _
                                                               roleID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next

                        ' grant any new job types that weren't there before
                        For Each item As ListItem In dlJobTypes.DestList.Items
                            Dim grant As Boolean = True
                            For Each jt As vwWebSecurityRole_JobType In grantedJobTypes
                                If jt.JobTypeID = Utils.ToInt32(item.Value) Then
                                    grant = False
                                    Exit For
                                End If
                            Next
                            If grant Then
                                msg = SecurityBL.GrantRoleJobType(trans, _
                                                              roleID, _
                                                              Utils.ToInt32(item.Value), _
                                                              auditLogUsername, _
                                                              auditLogTitle)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next
                    End If

                    ' Save Role Job Type End


                    ' Save Role DocumentType

                    canEditReportDocumentTypes = _
                                        SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                           currentUser.ID, _
                                           Target.Library.Web.ConstantsManager.GetConstant( _
                                               Me.Settings.CurrentApplication, _
                                               "WebNavMenuItem.EditRoleDocumentTypes"), _
                                           currentApp)


                    If canEditReportDocumentTypes Then
                        ' get currently granted document types
                        msg = SecurityBL.GetGrantedRoleDocumentTypes(Nothing, trans, roleID, grantedDocumentTypes)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' revoke any document types that are not now granted 
                        For Each dt As vwWebSecurityRole_DocumentType In grantedDocumentTypes
                            Dim revoke As Boolean = True
                            For Each item As ListItem In dlDocumentTypes.DestList.Items
                                If Utils.ToInt32(item.Value) = dt.DocumentTypeID Then
                                    revoke = False
                                    Exit For
                                End If
                            Next
                            If revoke Then
                                msg = SecurityBL.RevokeDocumentType(trans, _
                                                               dt.ID, _
                                                               auditLogUsername, _
                                                               auditLogTitle, _
                                                               roleID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next

                        ' grant any new document types that weren't there before
                        For Each item As ListItem In dlDocumentTypes.DestList.Items
                            Dim grant As Boolean = True
                            For Each dt As vwWebSecurityRole_DocumentType In grantedDocumentTypes
                                If dt.DocumentTypeID = Utils.ToInt32(item.Value) Then
                                    grant = False
                                    Exit For
                                End If
                            Next
                            If grant Then
                                msg = SecurityBL.GrantRoleDocumentType(trans, _
                                                              roleID, _
                                                              Utils.ToInt32(item.Value), _
                                                              auditLogUsername, _
                                                              auditLogTitle)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next
                    End If

                    ' Save Role DocumentType End

                    trans.Commit()

                    SecurityBL.ClearCacheByRole(Me.DbConnection, e.ItemID)
                    NavigationBL.ClearCacheByRole(Me.DbConnection, e.ItemID)

                    FindClicked(e)

                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End Try
            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " ProcessTreeView "

        Private Sub ProcessTreeView(ByVal nodes As TreeNodeCollection, _
                                    ByVal roleID As Integer, _
                                    ByVal trans As SqlTransaction, _
                                    ByVal roleMenuItems As Hashtable, _
                                    ByVal roleMenuItemCommands As Hashtable, _
                                    ByVal auditLogUsername As String, _
                                    ByVal auditLogTitle As String)

            Dim msg As ErrorMessage
            Dim id As Integer

            For Each node As TreeNode In nodes

                If node.Value.Contains(TREEVIEW_NODE_MENU_ITEM_COMMAND) Then
                    ' MENU ITEM COMMAND
                    id = Utils.ToInt32(node.Value.Replace(TREEVIEW_NODE_MENU_ITEM_COMMAND, String.Empty))
                    If node.Checked AndAlso Not roleMenuItemCommands.ContainsKey(id) Then
                        ' create
                        msg = SecurityBL.AddMenuItemCommandToRole(trans, roleID, id, auditLogUsername, auditLogTitle)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                    ElseIf Not node.Checked AndAlso roleMenuItemCommands.ContainsKey(id) Then
                        ' delete
                        msg = SecurityBL.RemoveMenuItemCommandFromRole(trans, roleMenuItemCommands(id), auditLogUsername, auditLogTitle)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                    End If
                Else
                    ' MENU ITEM
                    id = Utils.ToInt32(node.Value.Replace(TREEVIEW_NODE_MENU_ITEM, String.Empty))
                    If node.Checked AndAlso Not roleMenuItems.ContainsKey(id) Then
                        ' create
                        msg = SecurityBL.AddMenuItemToRole(trans, roleID, id, auditLogUsername, auditLogTitle)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                    ElseIf Not node.Checked AndAlso roleMenuItems.ContainsKey(id) Then
                        ' delete
                        msg = SecurityBL.RemoveMenuItemFromRole(trans, roleMenuItems(id), auditLogUsername, auditLogTitle)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                    End If
                End If

                ' recursively process sub-nodes
                ProcessTreeView(node.ChildNodes, roleID, trans, roleMenuItems, roleMenuItemCommands, auditLogUsername, auditLogTitle)

            Next

        End Sub

#End Region

#Region " PopulateTreeView "

        Private Sub PopulateTreeView(ByVal roleMenuItems As Hashtable, _
                                     ByVal roleMenuItemCommands As Hashtable, _
                                     ByVal menuID As Integer, _
                                     ByRef nodes As TreeNodeCollection, _
                                     ByRef parentNode As TreeNode, _
                                     ByVal menuItemsHash As Dictionary(Of Integer, MenuItemLicensedState))

            Dim menuItemNode As TreeNode, menuItemCommandNode As TreeNode = Nothing
            Dim msg As ErrorMessage
            Dim menuItems As WebNavMenuItemCollection = Nothing
            Dim menuItemCommands As WebNavMenuItemCommandCollection = Nothing
            Dim imgTick As String = "~/Images/Tick.png"
            Dim imgCross As String = "~/Images/Cross.png"
            Dim imgTriState As String = "~/Images/TriState.png"
            Dim imgWarning As String = "~/Images/WarningHS.png"
            Dim allItemsSelectedToolTip As String = "All items are selected"
            Dim noItemsSelectedToolTip As String = "No items are selected"
            Dim notAllItemsSelectedToolTip As String = "Not all items are selected"
            Dim warningToolTip As String = "These options are unlicenced ({0})"

            ' get menu items that hang off the specified menu item
            msg = WebNavMenuItem.FetchList(Me.DbConnection, menuItems, menuID, Me.Settings.CurrentApplicationID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            menuItems.Sort(New CollectionSorter("SortOrder", SortDirection.Ascending))

            If Not _isTreeviewEditable Then
                ' if the tree view can't be edited then remove check boxes

                roleTree.ShowCheckBoxes = TreeNodeTypes.None

            Else
                ' else tree view can be edited so add check boxes

                roleTree.ShowCheckBoxes = TreeNodeTypes.All

            End If

            For Each mi As WebNavMenuItem In menuItems
                If (mi.Visibility = WebNavMenuItemVisibility.WhenLoggedIn Or mi.Visibility = WebNavMenuItemVisibility.Hidden) AndAlso mi.ParentID = menuID Then
                    ' create the menu item node
                    menuItemNode = New TreeNode(mi.Name, String.Format("{0}{1}", TREEVIEW_NODE_MENU_ITEM, mi.ID))
                    menuItemNode.SelectAction = TreeNodeSelectAction.None

                    If Not IsNothing(parentNode) Then
                        ' if the parent node exists then add as a child node

                        parentNode.ChildNodes.Add(menuItemNode)

                    Else
                        ' else this node is the parent

                        parentNode = menuItemNode

                    End If

                    If roleMenuItems.ContainsKey(mi.ID) Then menuItemNode.Checked = True

                    If Not _isTreeviewEditable Then
                        ' if item is editable

                        If menuItemNode.Checked Then
                            ' item is checked so show tick

                            menuItemNode.ImageUrl = imgTick
                            menuItemNode.ImageToolTip = allItemsSelectedToolTip
                        Else
                            ' item isn't checked so show cross
                            menuItemNode.ImageUrl = imgCross
                            menuItemNode.ImageToolTip = noItemsSelectedToolTip
                        End If

                    End If

                    ' get menu item commands for this menu item
                    msg = WebNavMenuItemCommand.FetchList(Me.DbConnection, menuItemCommands, mi.ID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    menuItemCommands.Sort(New CollectionSorter("Description", SortDirection.Ascending))

                    ' create commands for this menu item
                    For Each mic As WebNavMenuItemCommand In menuItemCommands
                        If Not mic.AutoGrant Then
                            menuItemCommandNode = New TreeNode(mic.Description, String.Format("{0}{1}", TREEVIEW_NODE_MENU_ITEM_COMMAND, mic.ID))
                            menuItemCommandNode.SelectAction = TreeNodeSelectAction.None
                            menuItemNode.ChildNodes.Add(menuItemCommandNode)
                            ' ticked?

                            If roleMenuItemCommands.ContainsKey(mic.ID) Then

                                menuItemCommandNode.Checked = True

                            End If

                            If Not _isTreeviewEditable Then
                                ' if this treeview isn't editable then setup images

                                If menuItemCommandNode.Checked Then
                                    menuItemCommandNode.ImageUrl = imgTick
                                    menuItemNode.ImageToolTip = allItemsSelectedToolTip
                                Else

                                    menuItemCommandNode.ImageUrl = imgCross
                                    menuItemNode.ImageToolTip = noItemsSelectedToolTip
                                End If

                            End If

                            ' set the licensed state of the menu item
                            SetMenuItemLicensedStatus(mi.ID, menuItemsHash, menuItemCommandNode)

                        End If
                    Next

                    ' recursively create menu items and commands undeneath this menu item
                    PopulateTreeView(roleMenuItems, _
                                     roleMenuItemCommands, _
                                     mi.ID, _
                                     Nothing, _
                                     menuItemNode, _
                                     menuItemsHash)

                    If Not _isTreeviewEditable Then
                        ' if item is editable

                        If menuItemNode.ChildNodes.Count > 0 Then
                            ' if item has children 

                            Dim childNodesAllUnChecked As Boolean = True
                            Dim childNodesAllChecked As Boolean = True
                            Dim childNodesAllUnlicensed As Boolean = True

                            ' loop each sub node and check to see if it is unchecked
                            For Each childNode As TreeNode In menuItemNode.ChildNodes

                                If String.Compare(imgCross, childNode.ImageUrl, True) <> 0 Then

                                    childNodesAllUnChecked = False
                                    Exit For

                                End If

                            Next

                            ' loop each sub node and check to see if it is checked
                            For Each childNode As TreeNode In menuItemNode.ChildNodes

                                If String.Compare(imgTick, childNode.ImageUrl, True) <> 0 Then

                                    childNodesAllChecked = False
                                    Exit For

                                End If

                            Next

                            ' loop each sub node and check to see if it is unlicensed
                            For Each childNode As TreeNode In menuItemNode.ChildNodes

                                If String.Compare(imgWarning, childNode.ImageUrl, True) <> 0 Then

                                    childNodesAllUnlicensed = False
                                    Exit For

                                End If

                            Next

                            If menuItemNode.Checked = True And childNodesAllChecked = True Then
                                ' if the item is checked and all children are checked
                                menuItemNode.ImageUrl = imgTick
                                menuItemNode.ImageToolTip = allItemsSelectedToolTip

                            ElseIf menuItemNode.Checked = False And childNodesAllUnChecked = True Then
                                ' if the item is unchecked and all children are unchecked
                                menuItemNode.ImageUrl = imgCross
                                menuItemNode.ImageToolTip = noItemsSelectedToolTip

                            Else
                                ' else tristate
                                menuItemNode.ImageUrl = imgTriState
                                menuItemNode.ImageToolTip = notAllItemsSelectedToolTip
                            End If

                        End If

                    End If

                    If menuItemNode.Parent Is Nothing Then
                        ' if this node has no parent then add into nodes collection
                        ' this is a top level node

                        nodes.Add(menuItemNode)
                        parentNode = Nothing

                    End If

                    ' set the licensed state of the menu item
                    SetMenuItemLicensedStatus(mi.ID, menuItemsHash, menuItemNode)

                End If

            Next

        End Sub

        Private Sub SetMenuItemLicensedStatus(ByVal menuItemID As Integer, _
                                              ByVal menuItemsHash As Dictionary(Of Integer, MenuItemLicensedState), _
                                              ByRef node As TreeNode)

            Dim imgWarning As String = "~/Images/WarningHS.png"
            Dim warningToolTip As String = "These options are unlicenced ({0})"

            Dim miLicensedState As MenuItemLicensedState

            miLicensedState = menuItemsHash.Item(menuItemID)
            If miLicensedState <> MenuItemLicensedState.Licensed Then
                With node
                    .ImageUrl = imgWarning
                    .ImageToolTip = _
                        String.Format(warningToolTip, Utils.SplitOnCapitals([Enum].GetName(GetType(MenuItemLicensedState), miLicensedState)))

                    ' expired items are still available
                    If miLicensedState <> MenuItemLicensedState.Expired Then
                        .Checked = False
                        .ShowCheckBox = False
                    End If
                End With
            End If

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

        Private Sub SetupReports(ByVal roleID As Integer)

            Dim permissionsReportID As Integer = _
                Utils.ToInt32( _
                    Target.Library.Web.ConstantsManager.GetConstant( _
                        String.Format("{0}.WebReport.SecurityRolePermissions", Me.Settings.CurrentApplication) _
                    ) _
                )
            Dim membershipReportID As Integer = _
                Utils.ToInt32( _
                    Target.Library.Web.ConstantsManager.GetConstant( _
                        String.Format("{0}.WebReport.SecurityRoleMembership", Me.Settings.CurrentApplication) _
                    ) _
                )

            ' if we don't have any reports configured for the current application, hide the relevant controls
            If permissionsReportID <= 0 And membershipReportID <= 0 Then
                cpe.Enabled = False
                pnlReports.Visible = False
            Else
                AddHandler _stdBut.AddCustomControls, AddressOf StdButtons_AddCustomControls

                With lstReports
                    .Rows = 5
                    .Attributes.Add("onchange", "lstReports_Change();")
                    With .Items
                        .Add(New ListItem("Role permissions", divPermissions.ClientID))
                        .Add(New ListItem("Role membership", divMembership.ClientID))
                    End With
                End With

                ' permissions
                With CType(ctlPermissions, IReportsButton)
                    .ReportID = permissionsReportID
                    .Parameters.Add("intRoleID", roleID)
                End With

                ' membership
                With CType(ctlMembership, IReportsButton)
                    .ReportID = membershipReportID
                    .Parameters.Add("intRoleID", roleID)
                End With
            End If

        End Sub

#End Region

    End Class

End Namespace