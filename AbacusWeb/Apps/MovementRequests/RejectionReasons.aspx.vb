Imports Target.Web.Apps
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Library
Imports Target.Abacus.Library.Movements
Imports Target.Abacus.Library.DataClasses
Imports WebUtilities = Target.Library.Web.Utils
Imports Target.Web.Apps.Security

Namespace MovementRequests

    ''' <summary>
    ''' Web Form used to maintain Movement Request Rejection Reasons
    ''' </summary>
    ''' <history>
    ''' ColinD   04/03/2013 D12396 - Created
    ''' </history>
    Partial Class RejectionReasons
        Inherits BasePage

#Region "Fields"

        ' locals
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _RejectionReason As MovementRequestRejectionReason = Nothing

        ' constants
        ' - menu items
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Commitments.Administration.MovementRequestRejectionReasons"
        ' - menu item commands
        Private Const _WebNavMenuItemCommandAddKey As String = "AbacusIntranet.WebNavMenuItemCommand.Commitments.Administration.MovementRequestRejectionReasons.AddNew"
        Private Const _WebNavMenuItemCommandDeleteKey As String = "AbacusIntranet.WebNavMenuItemCommand.Commitments.Administration.MovementRequestRejectionReasons.Delete"
        Private Const _WebNavMenuItemCommandEditKey As String = "AbacusIntranet.WebNavMenuItemCommand.Commitments.Administration.MovementRequestRejectionReasons.Edit"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(ConstantsManager.GetConstant(_WebNavMenuItemKey), "Movement Request Rejection Reasons")

            ' setup standard buttons
            With StandardButtonsControl
                ' config button access
                .AllowBack = True
                .AllowDelete = UserHasDeleteCommand
                .AllowEdit = UserHasEditCommand
                .AllowFind = True
                .AllowNew = UserHasAddCommand
                ' config auditing
                .AuditLogTableNames.Add("MovementRequestRejectionReason")
                ' config editable controls
                .EditableControls.Add(fsControls.Controls)
                ' config search items
                .GenericFinderTypeID = GenericFinderType.MovementRequestRejectionReason
                .SearchBy.Add("Description", "Description")
                ' set event handlers
                AddHandler .CancelClicked, AddressOf FindClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
                AddHandler .EditClicked, AddressOf FindClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .NewClicked, AddressOf FindClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
            End With

        End Sub

        ''' <summary>
        ''' Handles the delete clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            ' delete the rejection reason
            msg = MovementsBL.DeleteMovementRejectionReason(DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)
            If Not msg.Success Then

                e.Cancel = True

                If msg.Number = ErrorMessage.GeneralErrorNumber Then
                    ' if general error then display the whole error

                    WebUtilities.DisplayError(msg)

                Else
                    ' otherwise business error so just display the message

                    lblError.Text = String.Format("{0}<br /><br />", msg.Message)
                    FindClicked(e)

                End If

                Return

            End If

            ' set the id to nowt
            e.ItemID = 0

            ' find the id of nowt which will result in a blank screen
            FindClicked(e)

        End Sub

        ''' <summary>
        ''' Handles the find clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            PopulateRejectionReason(e.ItemID)

        End Sub

        ''' <summary>
        ''' Handles the save clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing
            Dim reason As MovementRequestRejectionReason = RejectionReason

            If IsValid Then
                ' if valid then try and save

                msg = MovementsBL.SaveMovementRequestRejectionReason(DbConnection, reason)
                If Not msg.Success Then
                    ' failed so advise

                    If msg.Number = ErrorMessage.GeneralErrorNumber Then
                        ' if general error then display the whole error

                        WebUtilities.DisplayError(msg)
                        e.Cancel = True

                    Else
                        ' otherwise business error so just display the message

                        lblError.Text = String.Format("{0}<br /><br />", msg.Message)
                        e.Cancel = True

                    End If

                Else
                    ' saved so continue

                    If Not e.Cancel Then
                        ' set the id of the reason and then find it to reset

                        _RejectionReason = reason
                        e.ItemID = reason.ID
                        FindClicked(e)

                    End If

                End If

            Else
                ' else screen is not valid

                e.Cancel = True

            End If

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the audit log title.
        ''' </summary>
        ''' <value>
        ''' The audit log title.
        ''' </value>
        Private ReadOnly Property AuditLogTitle() As String
            Get
                Return MovementsBL.AuditTitleMovementRejectionReasons
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the audit log user.
        ''' </summary>
        ''' <value>The name of the audit log user.</value>
        Private ReadOnly Property AuditLogUserName() As String
            Get
                Return CurrentUser.ExternalUsername
            End Get
        End Property

        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>
        ''' The current user.
        ''' </value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    ' if current user not fetched then get current user
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property

        ''' <summary>
        ''' Gets the rejection reason.
        ''' </summary>
        ''' <value>
        ''' The rejection reason.
        ''' </value>
        Private ReadOnly Property RejectionReason As MovementRequestRejectionReason
            Get
                Dim msg As New ErrorMessage()
                ' create an instance of the reason if needed
                If _RejectionReason Is Nothing Then
                    _RejectionReason = New MovementRequestRejectionReason(conn:=DbConnection, AuditLogTitle:=AuditLogTitle, auditUserName:=AuditLogUserName)
                    If StandardButtonsControl.SelectedItemID > 0 Then
                        msg = MovementsBL.GetMovementRejectionReason(DbConnection, Nothing, StandardButtonsControl.SelectedItemID, _RejectionReason)
                    End If
                End If
                ' setup the reason
                With _RejectionReason
                    .AuditLogTitle = AuditLogTitle
                    .AuditUserName = AuditLogUserName
                    .Description = RejectionReasonDescription
                    .IsRedundant = RejectionReasonIsRedundant
                End With
                Return _RejectionReason
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the rejection reason description.
        ''' </summary>
        ''' <value>
        ''' The rejection reason description.
        ''' </value>
        Private Property RejectionReasonDescription() As String
            Get
                Return txtDescription.Text
            End Get
            Set(ByVal value As String)
                txtDescription.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [rejection reason is redundant].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [rejection reason is redundant]; otherwise, <c>false</c>.
        ''' </value>
        Private Property RejectionReasonIsRedundant() As Boolean
            Get
                Return chkRedundant.CheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                chkRedundant.CheckBox.Checked = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the standard buttons control.
        ''' </summary>
        ''' <value>The standard buttons control.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [user has add command].
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if [user has add command]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasAddCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebNavMenuItemCommandAddKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [user has delete command].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [user has delete command]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasDeleteCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebNavMenuItemCommandDeleteKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [user has edit command].
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if [user has edit command]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasEditCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebNavMenuItemCommandEditKey))
            End Get
        End Property

#End Region

#Region "Functions"

        ''' <summary>
        ''' Populates the rejection reason.
        ''' </summary>
        ''' <param name="id">The id.</param>
        Private Sub PopulateRejectionReason(id As Integer)

            Dim msg As ErrorMessage = Nothing
            Dim rejectionReason As New MovementRequestRejectionReason(String.Empty, String.Empty)

            If id > 0 Then
                ' if we have an id

                ' then fetch the movement throwing an error if found
                msg = MovementsBL.GetMovementRejectionReason(DbConnection, Nothing, id, rejectionReason)
                If Not msg.Success Then WebUtilities.DisplayError(msg)

            End If

            ' setup the form
            With Me
                .RejectionReasonDescription = rejectionReason.Description
                .RejectionReasonIsRedundant = rejectionReason.IsRedundant
            End With

        End Sub

#End Region

    End Class

End Namespace