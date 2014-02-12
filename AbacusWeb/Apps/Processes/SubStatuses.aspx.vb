Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.Processes
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Utils = Target.Library.Utils
Imports WebUtilities = Target.Library.Web.Utils

Namespace Processes

    ''' <summary>
    ''' Web Form used to maintain Sub Statuses for Processes
    ''' </summary>
    ''' <history>
    ''' Waqas    17/04/2013 D12480 - Added Service Order Exception
    ''' ColinD   06/03/2013 D12396 - Created
    ''' </history>
    Partial Class SubStatuses
        Inherits BasePage

#Region "Fields"

        ' locals
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _Process As Process = Nothing
        Private _ProcessToWebNavMenuItemMap As Dictionary(Of ProcessesBL.Process, String) = Nothing
        Private _SubStatus As ProcessSubStatus = Nothing

        ' constants
        ' - errors
        Private Const ErrorGeneral As String = ErrorMessage.GeneralErrorNumber
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

            If QsProcessID = ProcessesBL.Process.NotSet Then
                ' if we have no process specified then deny access

                WebUtilities.DisplayAccessDenied()

            End If

            ' setup the page conditionally based on the passed in process id
            InitPage(ConstantsManager.GetConstant(GetWebNavMenuItemConstant(QsProcessID)), String.Format("{0} - Process Sub Statuses", Process.Description))

            ' setup standard buttons
            With StandardButtonsControl
                ' config button access
                .AllowBack = True
                .AllowDelete = UserHasDeleteCommand
                .AllowEdit = UserHasEditCommand
                .AllowFind = True
                .AllowNew = UserHasAddCommand
                ' config auditing
                .AuditLogTableNames.Add("ProcessSubStatus")
                ' config editable controls
                .EditableControls.Add(fsControls.Controls)
                ' config search items
                .GenericFinderTypeID = GenericFinderType.ProcessSubStatuses
                .GenericFinderExtraParams.Add(QsProcessID)
                .SearchBy.Add("Description", "Description")
                ' set event handlers
                AddHandler .CancelClicked, AddressOf FindClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
                AddHandler .EditClicked, AddressOf FindClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .NewClicked, AddressOf FindClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
            End With

            ' setup other controls
            SubStatusProcess = Process.Description

        End Sub

        ''' <summary>
        ''' Handles the delete clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            ' delete the item
            msg = ProcessesBL.DeleteProcessSubStatus(DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)
            If Not msg.Success Then

                e.Cancel = True

                If msg.Number = ErrorGeneral Then
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

            PopulateSubStatus(e.ItemID)

        End Sub

        ''' <summary>
        ''' Handles the save clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing
            Dim status As ProcessSubStatus = SubStatus

            If IsValid Then
                ' if valid then try and save

                msg = ProcessesBL.SaveProcessSubStatus(DbConnection, status)
                If Not msg.Success Then
                    ' failed so advise

                    If msg.Number = ErrorGeneral Then
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

                    ' set the id of the reason and then find it to reset
                    _SubStatus = status
                    e.ItemID = status.ID
                    FindClicked(e)

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
                Return ProcessesBL.AuditTitleForProcessSubStatus
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
        ''' Gets the process to web nav menu item map.
        ''' </summary>
        ''' <value>
        ''' The process to web nav menu item map.
        ''' </value>
        Private ReadOnly Property ProcessToWebNavMenuItemMap As Dictionary(Of ProcessesBL.Process, String)
            Get
                If _ProcessToWebNavMenuItemMap Is Nothing Then
                    ' if nowt then add items
                    _ProcessToWebNavMenuItemMap = New Dictionary(Of ProcessesBL.Process, String)()
                    _ProcessToWebNavMenuItemMap.Add(ProcessesBL.Process.MovementRequests, "AbacusIntranet.WebNavMenuItem.Commitments.ReferenceData.MovementRequestSubStatuses")
                    _ProcessToWebNavMenuItemMap.Add(ProcessesBL.Process.PrivateHomePayments, "AbacusIntranet.WebNavMenuItem.Payments.ReferenceData.PrivateHomePaymentSubStatuses")
                    _ProcessToWebNavMenuItemMap.Add(ProcessesBL.Process.ServiceOrderImport, "AbacusIntranet.WebNavMenuItem.Commitments.ReferenceDate.OrderExceptionSubstatuses")
                    _ProcessToWebNavMenuItemMap.Add(ProcessesBL.Process.DeceasedWorkTray, "AbacusIntranet.WebNavMenuItem.ServiceUsers.ReferenceData.DeceasedSubStatuses")
                    _ProcessToWebNavMenuItemMap.Add(ProcessesBL.Process.ServiceUserImport, "AbacusIntranet.WebNavMenuItem.ServiceUsers.ReferenceData.ServiceUserImportSubStatuses")
                End If
                Return _ProcessToWebNavMenuItemMap
            End Get
        End Property

        ''' <summary>
        ''' Gets the process.
        ''' </summary>
        ''' <value>
        ''' The process.
        ''' </value>
        Private ReadOnly Property Process As Process
            Get
                Dim msg As New ErrorMessage()
                If _Process Is Nothing Then
                    ' if nowt then fetch
                    Using connection As SqlConnection = SqlHelper.GetConnectionToAbacus()
                        ' create a connection as when we need this the base page hasnt initd the DbConnection property!
                        Try
                            msg = ProcessesBL.GetProcess(connection, Nothing, QsProcessID, _Process)
                            If Not msg.Success Then WebUtilities.DisplayError(msg)
                        Catch ex As Exception
                            ' rethrow 
                            Throw
                        Finally
                            ' always close the connection
                            SqlHelper.CloseConnection(connection)
                        End Try
                    End Using
                End If
                Return _Process
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs process ID.
        ''' </summary>
        ''' <value>
        ''' The qs process ID.
        ''' </value>
        Private ReadOnly Property QsProcessID As ProcessesBL.Process
            Get
                Return Utils.ToInt32(Request.QueryString("pid"))
            End Get
        End Property

        ''' <summary>
        ''' Gets the sub status.
        ''' </summary>
        ''' <value>
        ''' The sub status.
        ''' </value>
        Private ReadOnly Property SubStatus As ProcessSubStatus
            Get
                Dim msg As New ErrorMessage()
                ' create an instance of the reason if needed
                If _SubStatus Is Nothing Then
                    _SubStatus = New ProcessSubStatus(conn:=DbConnection, AuditLogTitle:=AuditLogTitle, auditUserName:=AuditLogUserName)
                    If StandardButtonsControl.SelectedItemID > 0 Then
                        ' if we have id then fetch it
                        msg = ProcessesBL.GetProcessSubStatus(DbConnection, Nothing, StandardButtonsControl.SelectedItemID, _SubStatus)
                    End If
                End If
                ' setup the staus
                With _SubStatus
                    ' set auditing
                    .AuditLogTitle = AuditLogTitle
                    .AuditUserName = AuditLogUserName
                    ' set new data
                    .Description = SubStatusDescription
                    .IsRedundant = SubStatusIsRedundant
                    .ProcessID = QsProcessID
                End With
                Return _SubStatus
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the sub status description.
        ''' </summary>
        ''' <value>
        ''' The sub status description.
        ''' </value>
        Private Property SubStatusDescription() As String
            Get
                Return txtDescription.Text
            End Get
            Set(ByVal value As String)
                txtDescription.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [sub status is redundant].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [sub status is redundant]; otherwise, <c>false</c>.
        ''' </value>
        Private Property SubStatusIsRedundant() As Boolean
            Get
                Return chkRedundant.CheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                chkRedundant.CheckBox.Checked = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the sub status process.
        ''' </summary>
        ''' <value>
        ''' The sub status process.
        ''' </value>
        Private Property SubStatusProcess() As String
            Get
                Return txtProcess.Text
            End Get
            Set(ByVal value As String)
                txtProcess.Text = value
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

                Return (UserHasMenuItemCommand("AddNew"))
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
                Return UserHasMenuItemCommand("Delete")
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
                Return UserHasMenuItemCommand("Edit")
            End Get
        End Property

#End Region

#Region "Functions"

        ''' <summary>
        ''' Gets the web nav menu item constant.
        ''' </summary>
        ''' <param name="process">The process.</param>
        ''' <returns></returns>
        Private Function GetWebNavMenuItemConstant(process As ProcessesBL.Process) As String

            Dim result As String = Nothing

            If ProcessToWebNavMenuItemMap.ContainsKey(process) Then
                ' if the process is mapped get the key

                result = ProcessToWebNavMenuItemMap(process)

            End If

            Return result

        End Function

        ''' <summary>
        ''' Populates the sub status.
        ''' </summary>
        ''' <param name="id">The id.</param>
        Private Sub PopulateSubStatus(id As Integer)

            Dim msg As ErrorMessage = Nothing
            Dim subStatus As New ProcessSubStatus(String.Empty, String.Empty)

            If id > 0 Then
                ' if we have an id

                ' then fetch the sub status throwing an error if found
                msg = ProcessesBL.GetProcessSubStatus(DbConnection, Nothing, id, _SubStatus)
                If Not msg.Success Then WebUtilities.DisplayError(msg)

                ' set the global status so we dont have to fetch it again
                subStatus = _SubStatus

            End If

            ' setup the form
            With Me
                .SubStatusDescription = subStatus.Description
                .SubStatusIsRedundant = subStatus.IsRedundant
            End With

        End Sub

#End Region

    End Class

End Namespace