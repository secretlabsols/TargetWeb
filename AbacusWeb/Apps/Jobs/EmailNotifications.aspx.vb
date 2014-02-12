
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Jobs.Core
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.Core

Namespace Apps.Jobs

    ''' <summary>
    ''' Screen to manage job type email notification settings.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      05/03/2010  A4WA#6128 - corrected display of JobType details during postbacks.
    '''     MikeVO      11/01/2010  D11435 - removed ambiguous references to JobType class.
    '''     MikeVO      09/06/2009  D11515 - created.
    ''' </history>
    Partial Class EmailNotifications
        Inherits Target.Web.Apps.BasePage

#Region " Consts "

        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"

        Const CTRL_PREFIX_EMAIL As String = "email"
        Const CTRL_PREFIX_STATUS As String = "status"
        Const CTRL_PREFIX_REMOVED As String = "remove"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"

#End Region

#Region " Private variables "

        Private _newIDCounter As Integer
        Private _stdBut As StdButtonsBase

#End Region

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.JobTypeEmailNotifications"), "Job Service - Email Notifications")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            Me.AddExtraCssStyle("td.headerGroup {text-align:center;border-width:0px;}")
            Me.AddExtraCssStyle("td.headerStatus {background-color:#ffddac;} ")

            With _stdBut
                .AllowBack = False
                .AllowNew = False
                .AllowFind = True
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.JobTypeEmailNotifications.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.JobTypeEmailNotifications.Delete"))
                With .SearchBy
                    .Add("Name", "Name")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.AvailableJobType
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.JobServiceEmailNotifications")
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf FindClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .CancelClicked, AddressOf FindClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
            End With

            ' re-create the list of notifications (from view state)
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            For Each id As String In list
                OutputNotificationControls(id, Nothing)
            Next

        End Sub

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phNotifs.Controls.Clear()
        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim notifs As JobTypeEmailNotificationCollection = Nothing

            PopulateJobTypeDetails(e.ItemID)

            msg = JobTypeEmailNotification.FetchList(Me.DbConnection, notifs, e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            RefreshNotificationList(notifs, e)

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim notifs As JobTypeEmailNotificationCollection = New JobTypeEmailNotificationCollection()
            Dim notifsToDelete As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim notifList As List(Of String)
            Dim notif As JobTypeEmailNotification
            Dim statusValues As List(Of JobStatusBitwise) = GetJobStatusList()
            Dim selectedStatusValues As JobStatusBitwise
            Dim chkStatus As CheckBoxEx

            If Me.IsValid Then

                ' get list of notifications
                notifsToDelete = New List(Of String)
                notifList = GetUniqueIDsFromViewState()
                For Each uniqueID As String In notifList
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                        ' we are deleting
                        notifsToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                    Else
                        ' create the empty notification
                        notif = New JobTypeEmailNotification(Me.DbConnection)
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                            ' we are updating
                            msg = notif.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        Else
                            notif.JobTypeID = e.ItemID
                        End If
                        ' set the properties
                        With notif
                            .EmailAddress = CType(phNotifs.FindControl(CTRL_PREFIX_EMAIL & uniqueID), TextBoxEx).Text.Trim()
                            ' accumulate the selected status values
                            selectedStatusValues = JobStatusBitwise.Unknown
                            For Each status As JobStatusBitwise In statusValues
                                chkStatus = CType(phNotifs.FindControl(GetStatusCheckboxID(status, uniqueID)), CheckBoxEx)
                                If chkStatus.CheckBox.Checked Then
                                    selectedStatusValues += status
                                End If
                            Next
                            .JobStatusValues = selectedStatusValues
                        End With
                        ' add to the collection
                        notifs.Add(notif)
                    End If
                Next

                ' save the notifications
                msg = JobServiceBL.SaveEmailNotifications(Me.DbConnection, notifs, notifsToDelete)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                PopulateJobTypeDetails(e.ItemID)

            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            msg = JobServiceBL.DeleteEmailNotifications(Me.DbConnection, e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ClearViewState(e)

            ' cancel the delete event as we wish to stay on the selected job type
            e.Cancel = True

        End Sub

#End Region

#Region " btnAdd_Click "

        Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click

            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim newNotif As JobTypeEmailNotification = New JobTypeEmailNotification()

            PopulateJobTypeDetails(_stdBut.SelectedItemID)

            ' add a new row to the notifications list
            id = GetUniqueID(newNotif)
            ' create the controls
            OutputNotificationControls(id, newNotif)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " btnRemove_Click "

        Private Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim id As String = CType(sender, Button).ID.Replace(CTRL_PREFIX_REMOVED, String.Empty)

            PopulateJobTypeDetails(_stdBut.SelectedItemID)

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE, UNIQUEID_PREFIX_DELETE)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phNotifs.Controls.Count - 1
                If phNotifs.Controls(index).ID = id Then
                    phNotifs.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " OutputNotificationControls "

        Private Sub OutputNotificationControls(ByVal uniqueID As String, _
                                                ByVal notif As JobTypeEmailNotification)

            Dim statusValues As List(Of JobStatusBitwise)
            Dim row As HtmlTableRow = Nothing
            Dim cell As HtmlTableCell
            Dim txtEmail As TextBoxEx
            Dim chkStatus As CheckBoxEx
            Dim btnRemove As Button

            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phNotifs.Controls.Add(row)

                ' email
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                txtEmail = New TextBoxEx()
                With txtEmail
                    .ID = CTRL_PREFIX_EMAIL & uniqueID
                    .MaxLength = 255
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    .Width = New Unit(20, UnitType.Em)
                    If Not notif Is Nothing Then .Text = notif.EmailAddress
                End With
                cell.Controls.Add(txtEmail)

                ' list of status values we are interested in
                statusValues = GetJobStatusList()
                For Each status As JobStatusBitwise In statusValues
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("text-align", "center")
                    chkStatus = New CheckBoxEx()
                    With chkStatus
                        .ID = GetStatusCheckboxID(status, uniqueID)
                        .CheckBox.ValidationGroup = "Save"
                        .Label.Text = status.ToString()
                        If Not notif Is Nothing Then .CheckBox.Checked = (notif.JobStatusValues And status)
                    End With
                    cell.Controls.Add(chkStatus)
                Next

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                btnRemove = New Button()
                With btnRemove
                    .ID = CTRL_PREFIX_REMOVED & uniqueID
                    .Text = "Remove"
                    AddHandler .Click, AddressOf btnRemove_Click
                End With
                cell.Controls.Add(btnRemove)

            End If

        End Sub

#End Region

#Region " GetUniqueID "

        Private Function GetUniqueID(ByVal notif As JobTypeEmailNotification) As String

            Dim id As String

            If notif.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW & _newIDCounter
                _newIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & notif.ID
            End If

            Return id

        End Function

#End Region

#Region " GetUniqueIDsFromViewState "

        Private Function GetUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER) Is Nothing Then
                _newIDCounter = 0
            Else
                _newIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER), Integer)
            End If

            Return list

        End Function

#End Region

#Region " PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER, _newIDCounter)
        End Sub

#End Region

#Region " GetJobStatusList "

        Private Function GetJobStatusList() As List(Of JobStatusBitwise)

            Dim statusValues As List(Of JobStatusBitwise)

            statusValues = New List(Of JobStatusBitwise)
            With statusValues
                .Add(JobStatusBitwise.Complete)
                .Add(JobStatusBitwise.Warnings)
                .Add(JobStatusBitwise.Exceptions)
                .Add(JobStatusBitwise.Failed)
            End With

            Return statusValues

        End Function

#End Region

#Region " GetStatusCheckboxID "

        Private Function GetStatusCheckboxID(ByVal status As JobStatusBitwise, _
                                             ByVal uniqueID As String) As String
            Return String.Format("{0}-{1}{2}", CTRL_PREFIX_STATUS, status.ToString(), uniqueID)
        End Function

#End Region

#Region " RefreshNotificationList "

        Private Sub RefreshNotificationList(ByVal notifs As JobTypeEmailNotificationCollection, _
                                            ByRef e As StdButtonEventArgs)

            Dim list As List(Of String)

            ' refresh the list of existing notifications and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()
            For Each notif As JobTypeEmailNotification In notifs
                Dim id As String = GetUniqueID(notif)
                OutputNotificationControls(id, notif)
                list.Add(id)
            Next
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " PopulateJobTypeDetails "

        Private Sub PopulateJobTypeDetails(ByVal id As Integer)

            Dim msg As ErrorMessage
            Dim jt As Target.Abacus.Library.DataClasses.JobType

            jt = New Target.Abacus.Library.DataClasses.JobType(Me.DbConnection)
            With jt
                msg = .Fetch(id)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtName.Text = .Name
                txtDesc.Text = .Description
            End With

        End Sub

#End Region

    End Class

End Namespace