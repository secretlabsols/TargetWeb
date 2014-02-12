
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Navigation
Imports Target.Web.Apps.Navigation.Collections
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.CMS

Namespace Apps.Navigation.Controls

    Partial Class NavTree
        Inherits System.Web.UI.UserControl

#Region " Private variables and properties"

        Private _showMenuID As Integer
        Private _nodeFound As Boolean = False
        Private _newMenuUrl As String
        Private _cmsPageID As Integer
        Private _moveDownMenuID As Integer
        Private _moveUpMenuID As Integer

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or set the menu item to be displayed and selected in the tree upon load.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	02/06/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowMenuID() As Integer
            Get
                Return _showMenuID
            End Get
            Set(ByVal Value As Integer)
                _showMenuID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the URL that will be used if a new menu item is created.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	02/06/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property NewMenuUrl() As String
            Get
                Return _newMenuUrl
            End Get
            Set(ByVal Value As String)
                _newMenuUrl = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the ID of the CMS page.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	06/06/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property CMSPageID() As Integer
            Get
                Return _cmsPageID
            End Get
            Set(ByVal Value As Integer)
                _cmsPageID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the ID of the menu to move down in the sort order when used
        '''     with MoveUpMenuID.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	06/06/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property MoveDownMenuID() As Integer
            Get
                Return _moveDownMenuID
            End Get
            Set(ByVal Value As Integer)
                _moveDownMenuID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the ID of the menu to move up in the sort order when used
        '''     with MoveDownMenuID.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	06/06/2005	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property MoveUpMenuID() As Integer
            Get
                Return _moveUpMenuID
            End Get
            Set(ByVal Value As Integer)
                _moveUpMenuID = Value
            End Set
        End Property

#End Region

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim hostingPage As BasePage = Me.Page

            TreeView1.ImageFolder = Target.Library.Web.Utils.GetVirtualPath("Images/TreeView/")

            If Not IsPostBack Then
                LoadMenu(Nothing)
            End If

            ' move menus first
            MoveMenu()

            ' if showMenuNode was passed in we need to recurse back up from the menu to a menu folder to display it
            SelectNode()

            ' show/hide buttons
            spanNewMenu.Visible = Not _nodeFound
            spanDelete.Visible = _nodeFound
            spanMoveUp.Visible = _nodeFound
            spanMoveDown.Visible = _nodeFound

            TreeView1.CanClickNode = Not _nodeFound
            TreeView1.CanDoubleClickNode = _nodeFound

        End Sub

#End Region

#Region " TreeView1_NodeExpanded "

        Private Sub TreeView1_NodeExpanded(ByVal sender As Object, ByVal args As TreeViewNodeEventArgs) Handles TreeView1.NodeExpanded
            LoadMenu(args.Node)
        End Sub

#End Region

#Region " TreeView1_NodeAdded "

        Private Sub TreeView1_NodeAdded(ByVal sender As Object, ByVal args As TreeViewNodeEventArgs) Handles TreeView1.NodeAdded

            Dim msg As ErrorMessage
            Dim parentMenuID As Integer
            Dim hostingPage As BasePage = Me.Page

            If TypeOf args.Node.Parent Is TreeView OrElse DirectCast(args.Node.Parent, TreeNode).DataKey = 0 Then
                parentMenuID = NavigationBL.ROOT_MENU_ID
            Else
                parentMenuID = DirectCast(args.Node.Parent, TreeNode).DataKey
            End If
            args.Node.ImageUrl = "closedfolder.gif"
            args.Node.Tag = 0

            Dim newMenuItem As WebNavMenuItem = Nothing
            msg = CmsBL.NewMenu(hostingPage.DbConnection, Me.CMSPageID, newMenuItem, args.Node.Text, parentMenuID, Me.NewMenuUrl)
            If msg.Success Then
                Response.Redirect(hostingPage.Request.RawUrl)
            Else
                lblErrorMsg.Text = msg.Message
            End If

        End Sub

#End Region

#Region " TreeView1_NodeTextChanged "

        Private Sub TreeView1_NodeTextChanged(ByVal sender As Object, ByVal args As TreeViewNodeEventArgs) Handles TreeView1.NodeTextChanged

            Dim hostingPage As BasePage = Me.Page
            Dim msg As ErrorMessage

            msg = CmsBL.UpdateMenuName(hostingPage.DbConnection, Me.CMSPageID, args.Node.Text)
            If msg.Success Then
                NavigationBL.ClearCache()
                lblErrorMsg.Text = ""
            Else
                lblErrorMsg.Text = msg.Message
            End If

        End Sub

#End Region

#Region " TreeView1_NodeRemoved "

        Private Sub TreeView1_NodeRemoved(ByVal sender As Object, ByVal args As TreeViewNodeEventArgs) Handles TreeView1.NodeRemoved

            Dim msg As ErrorMessage
            Dim hostingPage As BasePage = Me.Page

            msg = CmsBL.DeleteMenu(hostingPage.DbConnection, Me.CMSPageID)
            If msg.Success Then
                Response.Redirect(hostingPage.Request.RawUrl)
            Else
                lblErrorMsg.Text = msg.Message
            End If

        End Sub

#End Region

#Region " LoadMenu "

        Private Sub LoadMenu(ByVal parentNode As TreeNode)

            Dim hostingPage As BasePage = Me.Page
            Dim menuList As WebNavMenuItemCollection = Nothing
            Dim newNode As TreeNode
            Dim msg As ErrorMessage
            Dim menuID As Integer

            If parentNode Is Nothing Then
                menuID = NavigationBL.ROOT_MENU_ID
            Else
                menuID = parentNode.DataKey
            End If

            msg = WebNavMenuItem.FetchList(hostingPage.DbConnection, menuList, menuID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            ' sort the list using the sort order
            If menuList.Count > 1 Then menuList.Sort(New CollectionSorter("SortOrder", SortDirection.Ascending))

            With TreeView1
                For Each menu As WebNavMenuItem In menuList
                    If menu.ID <> NavigationBL.ROOT_MENU_ID Then
                        newNode = New TreeNode(TreeView1, menu.Name)
                        newNode.DataKey = menu.ID
                        ' if its not a system menu and it is the selected menu item 
                        If Not menu.SystemMenuItem AndAlso menu.ID = Me.ShowMenuID Then
                            newNode.Tag = 1
                            newNode.ImageUrl = "menu.gif"
                        Else
                            newNode.Tag = 0
                            newNode.ImageUrl = "disabledMenu.gif"
                        End If
                        If menu.SubMenuCount > 0 Then newNode.HasChildNodes = True
                        If parentNode Is Nothing Then
                            TreeView1.AddNode(newNode)
                        Else
                            parentNode.AddNode(newNode)
                        End If
                    End If
                Next
            End With

        End Sub

#End Region

#Region " SelectNode and FindNode "

        Private Sub SelectNode()

            If Me.ShowMenuID <> 0 Then

                Dim hostingPage As BasePage = Me.Page
                Dim menu As WebNavMenuItem
                Dim menuID As Integer
                Dim msg As ErrorMessage
                Dim nodeStack As Stack
                Dim currentNode As TreeNode = Nothing

                nodeStack = New Stack
                menuID = Me.ShowMenuID
                ' loop, getting parent menu ID of the menu item and push onto a Stack until we hit a root menu
                While menuID <> -1
                    ' push the menu onto the node stack
                    nodeStack.Push(menuID)
                    ' get its parent menu
                    menu = New WebNavMenuItem(hostingPage.DbConnection)
                    msg = menu.Fetch(menuID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    menuID = menu.ParentID
                End While

                ' Unwind the stack by finding the root level menu in treeFolders.Nodes.
                ' Make it the SelctedNode and call LoadMenu() on it, then look in its Nodes collection for the next on the stack - repeat
                While nodeStack.Count > 0
                    currentNode = FindNode(nodeStack.Pop(), currentNode)
                End While

                TreeView1.SelectNode(currentNode)

            End If

        End Sub

        Private Function FindNode(ByVal dataKey As Integer, ByVal currentNode As TreeNode) As TreeNode

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
                If n.DataKey = dataKey Then
                    ' load the child nodes if necessary
                    If n.Controls.Count = 0 Then LoadMenu(n)
                    n.IsExpanded = True
                    ' set flag that the node was found
                    _nodeFound = True
                    Return n
                End If
            Next

        End Function

#End Region

#Region " MoveMenu "

        Private Sub MoveMenu()

            Dim msg As ErrorMessage
            Dim hostingPage As BasePage = Me.Page

            If Me.MoveDownMenuID <> 0 AndAlso Me.MoveUpMenuID <> 0 Then

                msg = NavigationBL.MoveMenu(hostingPage.DbConnection, Me.MoveDownMenuID, Me.MoveUpMenuID)

                If msg.Success Then
                    lblErrorMsg.Text = ""
                Else
                    lblErrorMsg.Text = msg.Message
                End If

            End If

        End Sub

#End Region

    End Class

End Namespace