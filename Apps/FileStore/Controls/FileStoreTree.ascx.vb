
Imports System.Configuration.ConfigurationManager
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.FileStore.Collections
Imports Target.Web.Apps.Security

Namespace Apps.FileStore.Controls

#Region " WebFileStoreTreeMode "

    Public Enum WebFileStoreTreeMode As Integer
        FileStoreAdmin = 1
        FileStoreFolderSelect = 2
        FileStoreFileSelect = 3
    End Enum

#End Region

    Partial Class FileStoreTree
        Inherits System.Web.UI.UserControl

#Region " Private variables and properties"

        Private _treeMode As WebFileStoreTreeMode
        Private _allowEdit As Boolean = True
        Private _showRootFolder As Boolean = False
        Private _showFileID As Integer
        Private _showFolderID As Integer
        Private _moveFileID As Integer
        Private _moveFolderID As Integer
        Private _moveTargetID As Integer

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the mode that then tree is currently in.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property TreeMode() As WebFileStoreTreeMode
            Get
                Return _treeMode
            End Get
            Set(ByVal Value As WebFileStoreTreeMode)
                _treeMode = Value

                Select Case _treeMode
                    Case WebFileStoreTreeMode.FileStoreAdmin
                        litTitle.Text = "File/Folder Administration"
                        Me.AllowSelect = False
                        Me.AllowCancel = False
                        Me.ShowRootFolder = False

                    Case WebFileStoreTreeMode.FileStoreFolderSelect
                        litTitle.Text = "Please select a folder"
                        Me.AllowNewFolder = False
                        Me.AllowNewFile = False
                        Me.AllowMove = False
                        Me.AllowDelete = False
                        Me.AllowRefresh = False
                        Me.AllowEdit = False
                        Me.ShowRootFolder = True

                    Case WebFileStoreTreeMode.FileStoreFileSelect
                        litTitle.Text = "Please select a file"
                        Me.AllowNewFolder = False
                        Me.AllowNewFile = False
                        Me.AllowMove = False
                        Me.AllowDelete = False
                        Me.AllowRefresh = False
                        Me.AllowEdit = False
                        Me.ShowRootFolder = False

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
        '''     Gets or sets if the new file functionality is made available.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property AllowNewFile() As Boolean
            Get
                Return spanNewFile.Visible
            End Get
            Set(ByVal Value As Boolean)
                spanNewFile.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets if the file/folder moving functionality is made available.
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
        '''     Gets or sets if the file/folder delete functionality is made available.
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
        '''     Gets or sets the WebFileStoreFile ID of the node that should be displayed.
        '''     If set, the tree is expanded to the file node and the node is selected.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowFileID() As Integer
            Get
                Return _showFileID
            End Get
            Set(ByVal Value As Integer)
                _showFileID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the WebFileStoreFolder ID of the node that should be displayed.
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
        '''     Gets or sets the WebFileStoreFile ID of the node that should be moved.
        '''     If set, the file is moved to the folder specified by MoveTargetID.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	25/05/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property MoveFileID() As Integer
            Get
                Return _moveFileID
            End Get
            Set(ByVal Value As Integer)
                _moveFileID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the WebFileStoreFolder ID of the node that should be moved.
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
        '''     Gets or sets the WebFileStoreFolder ID that the file or folder specified by
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
                parentFolderID = FileStoreBL.ROOT_FOLDER_ID
            Else
                parentFolderID = DirectCast(args.Node.Parent, TreeNode).DataKey
            End If
            args.Node.ImageUrl = "closedfolder.gif"

            msg = FileStoreBL.NewFolder(hostingPage.DbConnection, args.Node.DataKey, args.Node.Text, parentFolderID)
            args.Node.Tag = FileStoreBL.FOLDER_NODE_TAG
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
                Dim folder As WebFileStoreFolder = New WebFileStoreFolder(hostingPage.DbConnection)
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
                Case FileStoreBL.FOLDER_NODE_TAG
                    msg = FileStoreBL.DeleteFolder(hostingPage.DbConnection, args.Node.DataKey)
                Case FileStoreBL.FILE_NODE_TAG
                    msg = FileStoreBL.DeleteFile(hostingPage.DbConnection, args.Node.DataKey)
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
            Dim folderList As WebFileStoreFolderCollection = Nothing
            Dim fileList As WebFileStoreFileCollection = Nothing
            Dim newNode As TreeNode
            Dim msg As ErrorMessage
            Dim folderID As Integer

            If parentNode Is Nothing Then
                folderID = FileStoreBL.ROOT_FOLDER_ID
            Else
                folderID = parentNode.DataKey
            End If

            msg = WebFileStoreFolder.FetchList(hostingPage.DbConnection, folderList, folderID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            If Me.TreeMode <> WebFileStoreTreeMode.FileStoreFolderSelect Then
                msg = WebFileStoreFile.FetchList(hostingPage.DbConnection, fileList, folderID, 0, 0)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            End If

            With TreeView1
                If Me.ShowRootFolder AndAlso folderID = FileStoreBL.ROOT_FOLDER_ID Then
                    newNode = New TreeNode(TreeView1, "[Top Level]")
                    newNode.DataKey = FileStoreBL.ROOT_FOLDER_ID
                    newNode.Tag = FileStoreBL.FOLDER_NODE_TAG
                    newNode.ImageUrl = "closedfolder.gif"
                    TreeView1.AddNode(newNode)
                End If
                For Each folder As WebFileStoreFolder In folderList
                    If folder.ID <> FileStoreBL.ROOT_FOLDER_ID Then
                        newNode = New TreeNode(TreeView1, folder.Name)
                        newNode.DataKey = folder.ID
                        newNode.Tag = FileStoreBL.FOLDER_NODE_TAG
                        newNode.ImageUrl = "closedfolder.gif"
                        If folder.SubFolderCount > 0 OrElse (Me.TreeMode <> WebFileStoreTreeMode.FileStoreFolderSelect AndAlso folder.FileCount > 0) Then newNode.HasChildNodes = True
                        If parentNode Is Nothing Then
                            TreeView1.AddNode(newNode)
                        Else
                            parentNode.AddNode(newNode)
                        End If
                        'folderNodes.Sort(False, False)
                    End If
                Next
                If Me.TreeMode <> WebFileStoreTreeMode.FileStoreFolderSelect Then
                    For Each file As WebFileStoreFile In fileList
                        newNode = New TreeNode(TreeView1, file.Description)
                        newNode.DataKey = file.ID
                        newNode.Tag = FileStoreBL.FILE_NODE_TAG
                        newNode.ImageUrl = "file.gif"
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

            If Me.ShowFileID <> 0 OrElse Me.ShowFolderID <> 0 Then

                Dim hostingPage As BasePage = Me.Page
                Dim file As WebFileStoreFile
                Dim folder As WebFileStoreFolder
                Dim folderID As Integer
                Dim msg As ErrorMessage
                Dim nodeStack As Stack
                Dim currentNode As TreeNode = Nothing

                ' get the starting folder
                ' if we are given a folder ID then use that, otherwise get the containing folder of the file
                If Me.ShowFolderID <> 0 Then
                    folderID = Me.ShowFolderID
                Else
                    file = New WebFileStoreFile(hostingPage.DbConnection)
                    msg = file.Fetch(Me.ShowFileID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    folderID = file.WebFileStoreFolderID
                End If

                nodeStack = New Stack
                ' loop, getting parent folder ID of the folder and push onto a Stack until we hit a root folder
                While folderID <> -1
                    ' push the folder onto the node stack
                    nodeStack.Push(folderID)
                    ' get its parent folder
                    folder = New WebFileStoreFolder(hostingPage.DbConnection)
                    msg = folder.Fetch(folderID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    folderID = folder.ParentFolderID
                End While

                ' Unwind the stack by finding the root level folder in treeFolders.Nodes.
                ' Make it the SelctedNode and call LoadFolder() on it, then look in its Nodes collection for the next on the stack - repeat
                While nodeStack.Count > 0
                    currentNode = FindNode(nodeStack.Pop(), currentNode, FileStoreBL.FOLDER_NODE_TAG)
                End While

                ' finally find the file node if required
                If Me.ShowFileID > 0 Then currentNode = FindNode(Me.ShowFileID, currentNode, FileStoreBL.FILE_NODE_TAG)

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
                    msg = FileStoreBL.MoveFolder(hostingPage.DbConnection, Me.MoveFolderID, Me.MoveTargetID)
                ElseIf Me.MoveFileID <> 0 Then
                    msg = FileStoreBL.MoveFile(hostingPage.DbConnection, Me.MoveFileID, Me.MoveTargetID)
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
            If Not [Enum].IsDefined(GetType(WebFileStoreTreeMode), Me.TreeMode) Then
                Response.Write("Invalid tree mode<br>")
                invalid = True
            End If

            If invalid Then Response.End()

        End Sub

#End Region

    End Class

End Namespace