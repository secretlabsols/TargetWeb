Imports System.Data.SqlClient
Imports Target.Abacus.Library.SavedSelections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Navigation
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.SavedWizardSelections

    ''' <summary>
    ''' Allows a user to view, create, edit and delete saved wizard selections.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD  04/12/2012  D12390 - Overhaul to use new bl methods plus tidy up at same time.
    ''' MikeVO  22/07/2009  D11651 - created.
    ''' </history>
    Partial Class Edit
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        ' instance
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _SavedSelection As WebSavedWizardSelection = Nothing

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets a value indicating whether this instance can delete.
        ''' </summary>
        ''' <value>
        ''' 
        ''' <c>true</c> if this instance can delete; otherwise, <c>false</c>.
        ''' 
        ''' </value>
        Private ReadOnly Property CanDelete() As Boolean
            Get
                Return SecurityBL.UserHasMenuItemCommand( _
                            Me.DbConnection, _
                            CurrentUser.ID, _
                            Target.Library.Web.ConstantsManager.GetConstant( _
                                Me.Settings.CurrentApplication, _
                                "WebNavMenuItemCommand.SavedWizardSelectionsEdit.Delete"), _
                            Settings.CurrentApplicationID _
                    )
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance can edit.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if this instance can edit; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property CanEdit() As Boolean
            Get
                Return SecurityBL.UserHasMenuItemCommand( _
                            Me.DbConnection, _
                            CurrentUser.ID, _
                            Target.Library.Web.ConstantsManager.GetConstant( _
                                Me.Settings.CurrentApplication, _
                                "WebNavMenuItemCommand.SavedWizardSelectionsEdit.Edit"), _
                            Settings.CurrentApplicationID _
                    )
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance can manage globals.
        ''' </summary>
        ''' <value>
        ''' 
        ''' <c>true</c> if this instance can manage globals; otherwise, <c>false</c>.
        ''' 
        ''' </value>
        Private ReadOnly Property CanManageGlobals() As Boolean
            Get
                Return SecurityBL.UserHasMenuItemCommand( _
                            Me.DbConnection, _
                            CurrentUser.ID, _
                            Target.Library.Web.ConstantsManager.GetConstant( _
                                String.Format("{0}.WebNavMenuItemCommand.SavedWizardSelectionsEdit.ManageGlobal", Settings.CurrentApplicationID) _
                            ), _
                            Settings.CurrentApplicationID _
                        )
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance can manage other users.
        ''' </summary>
        ''' <value>
        ''' 
        ''' <c>true</c> if this instance can manage other users; otherwise, <c>false</c>.
        ''' 
        ''' </value>
        Private ReadOnly Property CanManageOtherUsers() As Boolean
            Get
                Return SecurityBL.UserHasMenuItemCommand( _
                            Me.DbConnection, _
                            CurrentUser.ID, _
                            Target.Library.Web.ConstantsManager.GetConstant( _
                                String.Format("{0}.WebNavMenuItemCommand.SavedWizardSelectionsEdit.ManageOtherUsers", Settings.CurrentApplicationID) _
                            ), _
                            Settings.CurrentApplicationID _
                        )
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the current user.
        ''' </summary>
        ''' <value>
        ''' The current user.
        ''' </value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    ' if nowt then get current
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property


        ''' <summary>
        ''' Gets the saved selection.
        ''' </summary>
        Private ReadOnly Property SavedSelection() As WebSavedWizardSelection
            Get
                If _SavedSelection Is Nothing AndAlso SavedSelectionID > 0 Then
                    ' if nowt then fetch

                    Dim msg As ErrorMessage = Nothing

                    ' fetch and throw error if found
                    _SavedSelection = New WebSavedWizardSelection(DbConnection, CurrentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(PageTitle, Settings))
                    msg = _SavedSelection.Fetch(SavedSelectionID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                Else
                    ' else create a new item

                    _SavedSelection = New WebSavedWizardSelection(DbConnection, CurrentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(PageTitle, Settings))

                End If
                ' setup saved selection with data from screen
                With _SavedSelection
                    If _SavedSelection.ID <= 0 Then
                        ' if new
                        .ApplicationID = Settings.CurrentApplicationID
                        .WebSecurityUserID = CurrentUser.ID
                        .WebNavMenuItemID = SavedSelectionWebNavMenuItemID
                        .QueryString = Utils.ToString(Request.QueryString("qs"))
                    End If
                    ' setup general info
                    .Name = SavedSelectionName
                    If CanManageGlobals Then
                        ' if allowed to set globals then do so
                        .GlobalSelection = SavedSelectionIsGlobal
                    Else
                        ' otherwise setup not global
                        .GlobalSelection = TriState.False
                    End If
                End With
                Return _SavedSelection
            End Get
        End Property

        ''' <summary>
        ''' Gets the saved selection ID.
        ''' </summary>
        Private ReadOnly Property SavedSelectionID() As Integer
            Get
                Return StandardButtonsControl.SelectedItemID
            End Get
        End Property

        ''' <summary>
        ''' Gets the saved selection is global.
        ''' </summary>
        Private Property SavedSelectionIsGlobal() As TriState
            Get
                If CanManageGlobals Then
                    ' if can manage globals then return user value
                    Return IIf(chkGlobal.CheckBox.Checked, TriState.True, TriState.False)
                ElseIf SavedSelectionID > 0 Then
                    ' else if we have a saved selection use its value
                    Return SavedSelection.GlobalSelection
                Else
                    ' otherwise false as no permissions and no current saved selection
                    Return TriState.False
                End If
            End Get
            Set(ByVal value As TriState)
                chkGlobal.CheckBox.Checked = (value = TriState.True)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the saved selection.
        ''' </summary>
        ''' <value>
        ''' The name of the saved selection.
        ''' </value>
        Private Property SavedSelectionName() As String
            Get
                Return txtName.Text
            End Get
            Set(ByVal value As String)
                txtName.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the saved selection owner.
        ''' </summary>
        ''' <value>
        ''' The saved selection owner.
        ''' </value>
        Public Property SavedSelectionOwner() As String
            Get
                Return txtOwner.Text
            End Get
            Set(ByVal value As String)
                txtOwner.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the saved selection screen.
        ''' </summary>
        ''' <value>
        ''' The saved selection screen.
        ''' </value>
        Public Property SavedSelectionScreen() As String
            Get
                Return txtScreen.Text
            End Get
            Set(ByVal value As String)
                txtScreen.Text = value
            End Set
        End Property


        ''' <summary>
        ''' Gets the saved selection web nav menu item ID.
        ''' </summary>
        Private ReadOnly Property SavedSelectionWebNavMenuItemID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString("mi"))
            End Get
        End Property

        ''' <summary>
        ''' Gets the standard buttons control.
        ''' </summary>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return stdButtons1
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' init the page
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(CurrentApplicationFromConfig, "WebNavMenuItem.SavedWizardSelectionsEdit"), "Edit Saved Wizard Selections")

            ' setup the standard buttons control
            With StandardButtonsControl
                .AllowBack = True
                .AllowNew = False
                .AllowEdit = CanEdit
                .AllowDelete = CanDelete
                .AllowFind = False
                .ShowNew = False
                .EditableControls.Add(fsControls.Controls)
                ' add button handlers
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
                AddHandler .EditClicked, AddressOf FindClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
            End With

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            If StandardButtonsControl.ButtonsMode = StdButtonsMode.Edit _
                OrElse StandardButtonsControl.ButtonsMode = StdButtonsMode.AddNew Then
                ' if editing or adding a new record ensure that we can only edit the global
                ' flag if we have permissions to do so

                chkGlobal.CheckBox.Enabled = CanManageGlobals

            End If

        End Sub

        ''' <summary>
        ''' Handles the cancel button being clicked.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub

        ''' <summary>
        ''' Handles the delete button being clicked.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            ' delete and display error if found
            msg = SavedSelectionsBL.DeleteSavedSelection(DbConnection, e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' redirect to the list screen
            Response.Redirect("List.aspx")

        End Sub

        ''' <summary>
        ''' Handles the find button being clicked.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim sel As ViewableSavedWebSelection = Nothing

            ' update existing
            msg = SavedWizardSelectionsBL.FetchSavedSelectionForEdit(DbConnection, e.ItemID, sel)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' setup form with saved selections info
            With sel
                SavedSelectionIsGlobal = .GlobalSelection
                SavedSelectionName = .Name
                SavedSelectionOwner = .Owner
                SavedSelectionScreen = .Screen
                If Not CanManageGlobals AndAlso .GlobalSelection Then
                    ' if cant manage globals and is global then display error
                    WebUtils.DisplayNotFound()
                End If
                If Not CanManageOtherUsers AndAlso sel.OwnerID <> CurrentUser.ID Then
                    ' if cant manage other users items then display error
                    WebUtils.DisplayNotFound()
                End If
            End With

        End Sub

        ''' <summary>
        ''' Handles the save button being clicked.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim sel As WebSavedWizardSelection = SavedSelection
            Dim selOut As WebSavedWizardSelection = Nothing

            If Me.IsValid Then
                ' if is valid then try and save

                ' save the selection and throw error if found
                msg = SavedSelectionsBL.SaveSavedSelection(DbConnection, sel.ID, sel.Name, sel.WebNavMenuItemID, sel.GlobalSelection, sel.QueryString, selOut)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' set the item id 
                e.ItemID = selOut.ID

                ' display the item
                FindClicked(e)

            Else
                ' not valid so cancel

                e.Cancel = True

            End If

        End Sub

#End Region

    End Class

End Namespace