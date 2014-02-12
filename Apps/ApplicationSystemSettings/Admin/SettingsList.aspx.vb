Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security

Namespace Apps.ApplicationSystemSettings.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.ApplicationSystemSettings.Admin.SettingsList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows users to manage system settings
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[ColinD]	01/04/2010	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class SettingsList
        Inherits Target.Web.Apps.BasePage

#Region "Enums"
        Enum ApplicationSettingTypes
            SettingSimple = 0
            SettingDateBased = 1
            SettingServiceUserMinutesCalc = 2
        End Enum
#End Region

#Region "Fields"

        Private Const _FolderTreeViewNodeImageURL As String = "~/Images/FolderClosed.png"
        Private Const _SettingTreeViewNodeImageURL As String = "~/Images/Properties.png"
        Private Const _SettingsEditorURL As String = "SettingsEdit.aspx"
        Private Const _ServiceUserMinutesCalcURL As String = "~/AbacusWeb/Apps/Admin/ServiceUserMinutesCalc.aspx"
        Private _ApplicationSettings As ApplicationSettingCollection = Nothing
        Private _BranchToExpand As String

#End Region

#Region "Event Handlers"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage( _
                       Target.Library.Web.ConstantsManager.GetConstant( _
                               Me.CurrentApplicationFromConfig, _
                               "WebNavMenuItem.Administration.SystemSettings.SystemSettings" _
                               ) _
                           , "Administration: System Settings" _
                       )

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reportID As Integer = Utils.ToInt32(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, _
                                                                                                    "WebReport.SystemSettings"))
            _BranchToExpand = Utils.ToString(Request.QueryString("selectType"))

            With stdButtons1
                .AllowNew = False
                .AllowEdit = False
                .AllowDelete = False
                .AllowFind = False
                If reportID > 0 Then
                    .ReportID = reportID
                    .ReportButtonParameters.Add("intUserID", currentUser.ID)
                End If
            End With

            If Not IsPostBack Then

                PopulateFoldersAndSettings()

            End If

            Me.JsLinks.Add("SettingsList.js")


        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            '++ If this page is being refreshed (due to changes made),
            '++ select and expand this node..
            Dim folderNode As TreeNode = Nothing

            If _BranchToExpand = "2" Then
                folderNode = tvFoldersAndSettings.FindNode("Intranet/Income/Debtor Invoices/Service User Minutes Calculation")
                If folderNode IsNot Nothing Then
                    folderNode.Expanded = True
                    folderNode.Select()

                    Dim parentNode As TreeNode = folderNode.Parent
                    Do Until parentNode Is Nothing
                        parentNode.Expanded = True
                        parentNode = parentNode.Parent
                    Loop
                End If
            End If
        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Populates All Visible Folders and Settings
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub PopulateFoldersAndSettings()

            Dim folders As ApplicationSettingFolderCollection = GetApplicationSettingFolders()

            folders.Sort(New CollectionSorter("SortOrder", SortDirection.Ascending))
            tvFoldersAndSettings.Nodes.Clear()

            ' loop each folder
            For Each folder As ApplicationSettingFolder In folders

                ' if the parent id is 0 i.e. has no parent folder then
                ' populate children folders and settings...this part only populates top level nodes, prob only one
                If folder.ParentID = 0 Then

                    Dim rootNode As New TreeNode(folder.Description)

                    With rootNode
                        .ImageUrl = _FolderTreeViewNodeImageURL
                        .Target = ""
                        .SelectAction = TreeNodeSelectAction.None
                    End With

                    tvFoldersAndSettings.Nodes.Add(rootNode)
                    PopulateFoldersAndSettingsChildren(folder, rootNode, folders)   ' populate the children folders and settings
                    rootNode.CollapseAll()
                    rootNode.Expand()

                End If

            Next

        End Sub

        ''' <summary>
        ''' Recursively adds folders and settings to a parent folder
        ''' </summary>
        ''' <param name="parentFolder">The parent folder to add folders and settings to</param>
        ''' <param name="parentNode">The parent node to add folders and settings to</param>
        ''' <param name="folders">All folders</param>
        ''' <remarks></remarks>
        Private Sub PopulateFoldersAndSettingsChildren(ByVal parentFolder As ApplicationSettingFolder, _
                                                       ByVal parentNode As TreeNode, _
                                                       ByVal folders As ApplicationSettingFolderCollection)
            Const SP_FETCH_CALCULATION_METHODS As String = "spxServiceUserMinutesCalculationMethod_FetchAll"

            ' loop each folder 
            For Each subFolder As ApplicationSettingFolder In folders

                ' if the sub folders parent id is that of the parent folder id then add
                ' to node as a children folder
                If subFolder.ParentID = parentFolder.ID Then

                    Dim childNode As New TreeNode()
                    Dim childNodeTooltip As String = String.Format("View/Edit '{0}' Settings", subFolder.Description)
                    Dim appSettings As ApplicationSettingCollection
                    Dim useSubFolderURL As Boolean = False

                    ' determine if we should generate a URL to edit settings value or use an URL
                    ' defined in the folder.Url property
                    If Not String.IsNullOrEmpty(subFolder.Url) AndAlso subFolder.Url.Trim().Length > 0 Then
                        useSubFolderURL = True
                    End If

                    With childNode
                        .ImageToolTip = childNodeTooltip
                        .ImageUrl = _FolderTreeViewNodeImageURL
                        .SelectAction = TreeNodeSelectAction.SelectExpand
                        .ToolTip = childNodeTooltip
                        If useSubFolderURL Then
                            .NavigateUrl = String.Format("{0}{1}&mpmode=none", _
                                                         subFolder.Url, _
                                                         IIf(subFolder.Url.Contains("?"), "", "?"))
                        Else
                            .NavigateUrl = String.Format("{0}?fid={1}&Mode=1", _
                                                         _SettingsEditorURL, _
                                                         subFolder.ID)
                        End If
                        .Text = subFolder.Description
                    End With

                    parentNode.ChildNodes.Add(childNode)

                    ' if the sub folder has a parent then add folders and settings
                    If subFolder.ParentID > 0 Then

                        PopulateFoldersAndSettingsChildren(subFolder, childNode, folders)

                    End If

                    appSettings = GetApplicationSettings(subFolder.ID)

                    If Not IsNothing(appSettings) Then

                        appSettings.Sort(New CollectionSorter("Name", SortDirection.Ascending))

                        ' loop each setting and add  to child node
                        For Each appSetting As ApplicationSetting In appSettings

                            If appSetting.Visible Then

                                '++ Is the current app setting a special case..?
                                If appSetting.ApplicationSettingType = ApplicationSettingTypes.SettingServiceUserMinutesCalc Then
                                    '++ THE CURRENT APP SETTING IS A PLACEHOLDER FOR INFO STORED IN THE
                                    '++ ServiceUserMinutesCalculationMethod and ApplicationSettingPeriod
                                    '++ TABLES.
                                    '++ Retrieve all the Service User Minutes Calculation Method
                                    '++ records, adding them as child nodes under the current branch
                                    '++ grouped by the 'effective from' date..
                                    Dim folderNode As New TreeNode()
                                    Dim folderNodeTooltip As String = String.Format("View/Edit '{0}' Settings", appSetting.Name)
                                    With folderNode
                                        .ImageToolTip = folderNodeTooltip
                                        .ImageUrl = _FolderTreeViewNodeImageURL
                                        .SelectAction = TreeNodeSelectAction.SelectExpand
                                        .ToolTip = folderNodeTooltip
                                        .NavigateUrl = String.Format("{0}?fid={1}&Mode=1", _
                                                                         _ServiceUserMinutesCalcURL, _
                                                                         appSetting.ID)
                                        .Text = appSetting.Name
                                    End With

                                    Dim spSPParams() As SqlParameter = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_CALCULATION_METHODS, False)
                                    Dim spDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_CALCULATION_METHODS, spSPParams)
                                    Dim spTable As DataTable = spDataset.Tables(0)
                                    Dim lastDateFrom As Date = Date.MinValue

                                    spTable = spDataset.Tables(0)
                                    If spTable.Rows.Count > 0 Then
                                        For Each spRow As DataRow In spTable.Rows
                                            If lastDateFrom <> spRow("DateFrom") Then
                                                lastDateFrom = spRow("DateFrom")

                                                Dim applicationSettingNode As New TreeNode()
                                                Dim applicationSettingNodeToolTip As String = String.Format("View/Edit 'Effective from {0}' Setting", lastDateFrom.ToString("dd/MM/yyyy"))
                                                Dim appSettingPeriodID As Long = spRow("ApplicationSettingPeriodID")

                                                With applicationSettingNode
                                                    .ImageToolTip = applicationSettingNodeToolTip
                                                    .ImageUrl = _SettingTreeViewNodeImageURL
                                                    .ToolTip = applicationSettingNodeToolTip
                                                    .NavigateUrl = String.Format("{0}?fid={1}&id={2}&Mode=1", _
                                                                                     _ServiceUserMinutesCalcURL, _
                                                                                     appSetting.ID, _
                                                                                     appSettingPeriodID)
                                                    .Text = String.Format("Effective from {0}", lastDateFrom.ToString("dd/MM/yyyy"))
                                                End With

                                                folderNode.ChildNodes.Add(applicationSettingNode)
                                            End If
                                        Next
                                        childNode.ChildNodes.Add(folderNode)
                                    End If
                                Else
                                    '++ Add the current AppSetting as a new tree node..
                                    Dim applicationSettingNode As New TreeNode()
                                    Dim applicationSettingNodeToolTip As String = String.Format("View/Edit '{0}' Setting", _
                                                                                                appSetting.Name)

                                    With applicationSettingNode
                                        .ImageToolTip = applicationSettingNodeToolTip
                                        .ImageUrl = _SettingTreeViewNodeImageURL
                                        .ToolTip = applicationSettingNodeToolTip
                                        If useSubFolderURL Then
                                            .SelectAction = TreeNodeSelectAction.None
                                            .ToolTip = String.Format("To view/edit this setting select the '{0}' folder above", _
                                                                     subFolder.Description)
                                        Else
                                            .NavigateUrl = String.Format("{0}?id={1}&Mode=1", _
                                                                         _SettingsEditorURL, _
                                                                         appSetting.ID)
                                        End If
                                        .Text = appSetting.Name
                                    End With

                                    childNode.ChildNodes.Add(applicationSettingNode)
                                End If
                            End If
                        Next
                    End If

                    ' if we have no settings/folders in folder then hide
                    If childNode.ChildNodes.Count = 0 Then

                        parentNode.ChildNodes.Remove(childNode)

                    End If

                End If

            Next

        End Sub

        ''' <summary>
        ''' Gets Application Setting Folders for the current application
        ''' </summary>
        ''' <returns>Application Setting Folders for the current application</returns>
        ''' <remarks></remarks>
        Private Function GetApplicationSettingFolders() As ApplicationSettingFolderCollection

            Dim applicationSettingFolders As ApplicationSettingFolderCollection = New ApplicationSettingFolderCollection()
            Dim msg As ErrorMessage

            msg = ApplicationSettingFolder.FetchList(conn:=Me.DbConnection, _
                                                     list:=applicationSettingFolders, _
                                                     applicationID:=Me.Settings.CurrentApplicationID)

            If msg.Success Then

                Return applicationSettingFolders

            Else

                WebUtils.DisplayError(msg)
                Return Nothing

            End If

        End Function

        ''' <summary>
        ''' Gets Application Settings for the current application by folder id 
        ''' </summary>
        ''' <param name="folderID">The folder the application settings belong to</param>
        ''' <returns>Application Settings for the current application by folder id </returns>
        ''' <remarks></remarks>
        Private Function GetApplicationSettings(ByVal folderID As Integer) As ApplicationSettingCollection

            Dim applicationSettings As ApplicationSettingCollection = New ApplicationSettingCollection()

            ' loop each application setting for this application
            For Each appSetting As ApplicationSetting In GetApplicationSettings()

                ' if the folder matches the id passed in
                If appSetting.ApplicationSettingFolderID = folderID Then

                    applicationSettings.Add(appSetting)

                End If

            Next

            Return applicationSettings

        End Function

        ''' <summary>
        ''' Gets Application Settings for the current application
        ''' </summary>
        ''' <returns>Application Settings for the current application</returns>
        ''' <remarks></remarks>
        Private Function GetApplicationSettings() As ApplicationSettingCollection

            Dim applicationSettings As ApplicationSettingCollection = New ApplicationSettingCollection()
            Dim msg As ErrorMessage

            If Not IsNothing(_ApplicationSettings) Then
                ' if settings already fetched, return them

                applicationSettings = _ApplicationSettings

            Else
                ' else settings haven't been fetched in this postback
                ' so retrieve all of them for this application

                msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, _
                                                   list:=applicationSettings, _
                                                   auditUserName:=String.Empty, _
                                                   auditLogTitle:=String.Empty, _
                                                   applicationID:=Me.Settings.CurrentApplicationID)

                If Not msg.Success Then

                    WebUtils.DisplayError(msg)
                    applicationSettings = Nothing

                End If

            End If

            _ApplicationSettings = applicationSettings

            Return applicationSettings

        End Function


#End Region
    End Class

End Namespace

