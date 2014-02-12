
Imports System.Configuration.ConfigurationManager
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.CMS
Imports Target.Web.Apps.CMS.Collections
Imports Target.Web.Apps.Navigation
Imports Target.Web.Apps.Security

Namespace Apps.CMS.Controls

#Region " WebCMSTreeMode "

    Public Enum WebCMSTreeMode As Integer
        CMSPageSelect = 1
        CMSFolderSelect = 2
        CMSFolderAdmin = 3
    End Enum

#End Region

    Partial Class CMSTree
        Inherits System.Web.UI.UserControl

#Region " Private variables and properties"

        Private _treeMode As WebCMSTreeMode
        Private _showPageID As Integer
        Private _showFolderID As Integer
        Private _movePageID As Integer
        Private _moveFolderID As Integer
        Private _moveTargetID As Integer
        Private _allowEdit As Boolean
        Private _showRootFolder As Boolean
        Private _currentFolderID As Integer

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the mode that the tree is currently in.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property TreeMode() As WebCMSTreeMode
            Get
                Return _treeMode
            End Get
            Set(ByVal Value As WebCMSTreeMode)
                _treeMode = Value

                Select Case _treeMode
                    Case WebCMSTreeMode.CMSFolderSelect
                        litTitle.Text = "Please select a folder"
                        Me.AllowNewFolder = False
                        Me.AllowMove = False
                        Me.AllowDelete = False
                        Me.AllowRefresh = False
                        Me.AllowEdit = False
                        Me.ShowRootFolder = True

                    Case WebCMSTreeMode.CMSPageSelect
                        litTitle.Text = "Please select a page"
                        Me.AllowNewFolder = False
                        Me.AllowMove = False
                        Me.AllowDelete = False
                        Me.AllowRefresh = False
                        Me.AllowEdit = False
                        Me.ShowRootFolder = False

                    Case WebCMSTreeMode.CMSFolderAdmin
                        litTitle.Visible = False
                        Me.AllowNewFolder = True
                        Me.AllowMove = True
                        Me.AllowDelete = True
                        Me.AllowRefresh = False
                        Me.AllowEdit = True
                        Me.ShowRootFolder = False
                        Me.AllowSelect = False
                        Me.AllowCancel = False

                End Select

            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets whether the top level root folder should be displayed.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowRootFolder() As Boolean
            Get
                Return _showRootFolder
            End Get
            Set(ByVal Value As Boolean)
                _showRootFolder = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets if the new folder functionality is made available.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property AllowNewFolder() As Boolean
            Get
                Return spanNewFolder.Visible
            End Get
            Set(ByVal Value As Boolean)
                spanNewFolder.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets if the page/folder moving functionality is made available.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property AllowMove() As Boolean
            Get
                Return spanMove.Visible
            End Get
            Set(ByVal Value As Boolean)
                spanMove.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets if the page/folder delete functionality is made available.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property AllowDelete() As Boolean
            Get
                Return spanDelete.Visible
            End Get
            Set(ByVal Value As Boolean)
                spanDelete.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets if the tree refresh functionality is made available.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property AllowRefresh() As Boolean
            Get
                Return spanRefresh.Visible
            End Get
            Set(ByVal Value As Boolean)
                spanRefresh.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets if the node select and return functionality is made available.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property AllowSelect() As Boolean
            Get
                Return spanSelect.Visible
            End Get
            Set(ByVal Value As Boolean)
                spanSelect.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets if the close button is made available.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property AllowCancel() As Boolean
            Get
                Return spanCancel.Visible
            End Get
            Set(ByVal Value As Boolean)
                spanCancel.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets if folder renaming or file editing is made available.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property AllowEdit() As Boolean
            Get
                Return _allowEdit
            End Get
            Set(ByVal Value As Boolean)
                _allowEdit = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the WebCMSPage ID of the node that should be displayed.
        '''     If set, the tree is expanded to the page node and the node is selected.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowPageID() As Integer
            Get
                Return _showPageID
            End Get
            Set(ByVal Value As Integer)
                _showPageID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the WebCMSFolder ID of the node that should be displayed.
        '''     If set, the tree is expanded to the folder node and the node is selected.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowFolderID() As Integer
            Get
                Return _showFolderID
            End Get
            Set(ByVal Value As Integer)
                _showFolderID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the WebCMSPage ID of the node that should be moved.
        '''     If set, the page is moved to the folder specified by MoveTargetID.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property MovePageID() As Integer
            Get
                Return _movePageID
            End Get
            Set(ByVal Value As Integer)
                _movePageID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the WebCMSFolder ID of the node that should be moved.
        '''     If set, the folder is moved to the folder specified by MoveTargetID.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property MoveFolderID() As Integer
            Get
                Return _moveFolderID
            End Get
            Set(ByVal Value As Integer)
                _moveFolderID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the WebCMSFolder ID that the file or folder specified by
        '''     MoveFileID or MoveFolderID should be moved to.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property MoveTargetID() As Integer
            Get
                Return _moveTargetID
            End Get
            Set(ByVal Value As Integer)
                _moveTargetID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the ID of the folder that is currently being operated on.
        '''     e.g. if moving folder 123, set this property to 123.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	31/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property CurrentFolderID() As Integer
            Get
                Return _currentFolderID
            End Get
            Set(ByVal Value As Integer)
                _currentFolderID = Value
            End Set
        End Property

#End Region

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim hostingPage As BasePage = Me.Page

            TreeView1.ImageFolder = Target.Library.Web.Utils.GetVirtualPath("Images/TreeView/")

            ' check the tree mode
            ValidateTreeSetup()

            ' move any files/folders
            MoveNode()

            If Not IsPostBack Then
                LoadFolder(Nothing)
            End If

            ' if showFileNode was passed in we need to recurse back up from the file to a root folder to display it
            SelectNode()

        End Sub

#End Region

#Region " TreeView1_NodeExpanded "

        Private Sub TreeView1_NodeExpanded(ByVal sender As Object, ByVal args As TreeViewNodeEventArgs) Handles TreeView1.NodeExpanded
            LoadFolder(args.Node)
        End Sub

#End Region

#Region " TreeView1_NodeAdded "

        Private Sub TreeView1_NodeAdded(ByVal sender As Object, ByVal args As TreeViewNodeEventArgs) Handles TreeView1.NodeAdded

            Dim msg As ErrorMessage
            Dim parentFolderID As Integer
            Dim hostingPage As BasePage = Me.Page

            If TypeOf args.Node.Parent Is TreeView OrElse DirectCast(args.Node.Parent, TreeNode).DataKey = 0 Then
                parentFolderID = CmsBL.ROOT_FOLDER_ID
            Else
                parentFolderID = DirectCast(args.Node.Parent, TreeNode).DataKey
            End If
            args.Node.ImageUrl = "closedfolder.gif"

            msg = CmsBL.NewFolder(hostingPage.DbConnection, args.Node.DataKey, args.Node.Text, parentFolderID)
            args.Node.Tag = CmsBL.FOLDER_NODE_TAG
            If msg.Success Then
                lblErrorMsg.Text = ""
            Else
                lblErrorMsg.Text = msg.Message
            End If

        End Sub

#End Region

#Region " TreeView1_NodeTextChanged "

        Private Sub TreeView1_NodeTextChanged(ByVal sender As Object, ByVal args As TreeViewNodeEventArgs) Handles TreeView1.NodeTextChanged

            Dim hostingPage As BasePage = Me.Page

            If args.Node.DataKey <> 0 Then

                Dim msg As ErrorMessage
                Dim folder As WebCMSFolder = New WebCMSFolder(hostingPage.DbConnection)
                With folder
                    msg = .Fetch(args.Node.DataKey)
                    If msg.Success Then
                        lblErrorMsg.Text = ""
                    Else
                        lblErrorMsg.Text = msg.Message
                    End If
                    .Name = args.Node.Text
                    msg = .Save()
                    If msg.Success Then
                        lblErrorMsg.Text = ""
                    Else
                        lblErrorMsg.Text = msg.Message
                    End If
                End With

            End If

        End Sub

#End Region

#Region " TreeView1_NodeRemoved "

        Private Sub TreeView1_NodeRemoved(ByVal sender As Object, ByVal args As TreeViewNodeEventArgs) Handles TreeView1.NodeRemoved

            Dim msg As ErrorMessage = Nothing
            Dim hostingPage As BasePage = Me.Page

            Select Case args.Node.Tag
                Case CmsBL.FOLDER_NODE_TAG
                    msg = CmsBL.DeleteFolder(hostingPage.DbConnection, args.Node.DataKey)
                Case CmsBL.PAGE_NODE_TAG
                    msg = CmsBL.DeletePage(hostingPage.DbConnection, args.Node.DataKey)
            End Select

            If msg.Success Then
                Response.Redirect(Request.Url.ToString())
            Else
                lblErrorMsg.Text = msg.Message
            End If

        End Sub

#End Region

#Region " LoadFolder "

        Private Sub LoadFolder(ByVal parentNode As TreeNode)

            ' ensure we are loading the folder twice
            If Not parentNode Is Nothing AndAlso parentNode.Controls.Count > 0 Then Return

            Dim hostingPage As BasePage = Me.Page
            Dim folderList As WebCMSFolderCollection = Nothing
            Dim pageList As WebCMSPageCollection = Nothing
            Dim newNode As TreeNode
            Dim msg As ErrorMessage
            Dim folderID As Integer

            If parentNode Is Nothing Then
                folderID = CmsBL.ROOT_FOLDER_ID
            Else
                folderID = parentNode.DataKey
            End If

            msg = WebCMSFolder.FetchList(hostingPage.DbConnection, folderList, folderID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            If Me.TreeMode <> WebCMSTreeMode.CMSFolderSelect Then
                msg = WebCMSPage.FetchList(hostingPage.DbConnection, pageList, folderID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            End If

            With TreeView1
                If Me.ShowRootFolder AndAlso folderID = CmsBL.ROOT_FOLDER_ID Then
                    newNode = New TreeNode(TreeView1, "[Top Level]")
                    newNode.DataKey = CmsBL.ROOT_FOLDER_ID
                    newNode.Tag = CmsBL.FOLDER_NODE_TAG
                    newNode.ImageUrl = "closedfolder.gif"
                    TreeView1.AddNode(newNode)
                End If
                For Each folder As WebCMSFolder In folderList
                    If folder.ID <> CmsBL.ROOT_FOLDER_ID Then
                        newNode = New TreeNode(TreeView1, folder.Name)
                        newNode.DataKey = folder.ID
                        newNode.Tag = CmsBL.FOLDER_NODE_TAG
                        newNode.ImageUrl = "closedfolder.gif"
                        If folder.SubFolderCount > 0 OrElse (Me.TreeMode <> WebCMSTreeMode.CMSFolderSelect AndAlso folder.PageCount > 0) Then newNode.HasChildNodes = True
                        If parentNode Is Nothing Then
                            TreeView1.AddNode(newNode)
                        Else
                            parentNode.AddNode(newNode)
                        End If
                        'folderNodes.Sort(False, False)
                    End If
                Next
                If Me.TreeMode <> WebCMSTreeMode.CMSFolderSelect Then
                    For Each cmsPage As WebCMSPage In pageList
                        newNode = New TreeNode(TreeView1, cmsPage.Title)
                        newNode.DataKey = cmsPage.ID
                        newNode.Tag = CmsBL.PAGE_NODE_TAG
                        newNode.ImageUrl = "ie.gif"
                        If parentNode Is Nothing Then
                            TreeView1.AddNode(newNode)
                        Else
                            parentNode.AddNode(newNode)
                        End If
                        'folderNodes.Sort(False, False)
                    Next
                End If
            End With

        End Sub

#End Region

#Region " SelectNode and FindNode "

        Private Sub SelectNode()

            If Me.ShowPageID <> 0 OrElse Me.ShowFolderID <> 0 Then

                Dim hostingPage As BasePage = Me.Page
                Dim cmsPage As WebCMSPage
                Dim folder As WebCMSFolder
                Dim folderID As Integer
                Dim msg As ErrorMessage
                Dim nodeStack As Stack
                Dim currentNode As TreeNode = Nothing

                ' get the starting folder
                ' if we are given a folder ID then use that, otherwise get the containing folder of the file
                If Me.ShowFolderID <> 0 Then
                    folderID = Me.ShowFolderID
                Else
                    cmsPage = New WebCMSPage(hostingPage.DbConnection)
                    msg = cmsPage.Fetch(Me.ShowPageID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    folderID = cmsPage.WebCMSFolderID
                End If

                nodeStack = New Stack
                ' loop, getting parent folder ID of the folder and push onto a Stack until we hit a root folder
                While folderID <> -1
                    ' push the folder onto the node stack
                    nodeStack.Push(folderID)
                    ' get its parent folder
                    folder = New WebCMSFolder(hostingPage.DbConnection)
                    msg = folder.Fetch(folderID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    folderID = folder.ParentFolderID
                End While

                ' Unwind the stack by finding the root level folder in treeFolders.Nodes.
                ' Make it the SelctedNode and call LoadFolder() on it, then look in its Nodes collection for the next on the stack - repeat
                While nodeStack.Count > 0
                    currentNode = FindNode(nodeStack.Pop(), currentNode, CmsBL.FOLDER_NODE_TAG)
                End While

                ' finally select the page node if required
                If Me.ShowPageID > 0 Then currentNode = FindNode(Me.ShowPageID, currentNode, CmsBL.PAGE_NODE_TAG)

                TreeView1.SelectNode(currentNode)

            End If

        End Sub

        Private Function FindNode(ByVal dataKey As Integer, ByVal currentNode As TreeNode, ByVal nodeTag As String) As TreeNode

            Dim nodeList As ControlCollection

            FindNode = Nothing

            If currentNode Is Nothing Then
                ' search in the root nodes
                nodeList = TreeView1.Controls
            Else
                ' otherwise search in the child nodes of the currentNode
                nodeList = currentNode.Controls
            End If

            For Each n As TreeNode In nodeList
                If n.DataKey = dataKey And n.Tag = nodeTag Then
                    ' load the child nodes if necessary
                    If n.Controls.Count = 0 Then LoadFolder(n)
                    n.IsExpanded = True
                    Return n
                End If
            Next

        End Function

#End Region

#Region " MoveNode "

        Private Sub MoveNode()

            If Me.MoveTargetID <> 0 Then

                Dim hostingPage As BasePage = Me.Page
                Dim msg As ErrorMessage = Nothing

                If Me.MoveFolderID <> 0 Then
                    msg = CmsBL.MoveFolder(hostingPage.DbConnection, Me.MoveFolderID, Me.MoveTargetID)
                ElseIf Me.MovePageID <> 0 Then
                    msg = CmsBL.MovePage(hostingPage.DbConnection, Me.MovePageID, Me.MoveTargetID)
                End If

                If msg.Success Then
                    lblErrorMsg.Text = ""
                ElseIf msg.Number = "E0511" Then     ' invalid folder move
                    lblErrorMsg.Text = msg.Message
                Else
                    Target.Library.Web.Utils.DisplayError(msg)
                End If
            End If

        End Sub

#End Region

#Region " ValidateTreeSetup "

        Private Sub ValidateTreeSetup()

            Dim invalid As Boolean = False

            ' mode
            If Not [Enum].IsDefined(GetType(WebCMSTreeMode), Me.TreeMode) Then
                Response.Write("Invalid tree mode<br>")
                invalid = True
            End If

            If invalid Then Response.End()

        End Sub

#End Region

    End Class

End Namespace