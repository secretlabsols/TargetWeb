
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin.SDS

    ''' <summary>
    ''' Admin page used to maintain SDS RAS types and periods.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Public Class RASTypes
        Inherits BasePage

        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"

        Const CTRL_PREFIX_FROM As String = "from"
        Const CTRL_PREFIX_TO As String = "to"
        Const CTRL_PREFIX_PPP As String = "ppp"
        Const CTRL_PREFIX_REMOVED As String = "remove"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"

        Private _newPeriodIDCounter As Integer

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.RASTypes"), "RAS Types")

            Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RASTypes.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RASTypes.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RASTypes.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.RASType
                .AuditLogTableNames.Add("RASType")
                .AuditLogTableNames.Add("RASTypePeriod")
            End With
            AddHandler stdBut.NewClicked, AddressOf ClearViewState
            AddHandler stdBut.FindClicked, AddressOf FindClicked
            AddHandler stdBut.EditClicked, AddressOf FindClicked
            AddHandler stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler stdBut.DeleteClicked, AddressOf DeleteClicked

            ' re-create the list of bands (from view state)
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            For Each id As String In list
                OutputPeriodControls(id, Nothing)
            Next

        End Sub

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phPeriods.Controls.Clear()
        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            ClearViewState(e)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                chkRedundant.CheckBox.Checked = False
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim t As RASType
            Dim periods As RASTypePeriodCollection = New RASTypePeriodCollection
            Dim list As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            t = New RASType(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With t
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
            End With

            ' get the periods
            msg = RASTypePeriod.FetchList(Me.DbConnection, periods, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), t.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' refresh the list of existing periods and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()
            For Each period As RASTypePeriod In periods
                Dim id As String = GetUniqueID(period)
                OutputPeriodControls(id, period)
                list.Add(id)
            Next
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim periods As RASTypePeriodCollection = Nothing

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                ' delete all of the periods first
                msg = RASTypePeriod.FetchList(trans, periods, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                For Each period As RASTypePeriod In periods
                    msg = RASTypePeriod.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), period.ID, e.ItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                Next

                ' delete the type itself
                msg = RASType.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                trans.Commit()

                ClearViewState(e)

                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0503", "RASType/RASTypePeriod")     ' could not delete
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim t As RASType
            Dim periods As RASTypePeriodCollection = New RASTypePeriodCollection
            Dim periodsToDelete As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim periodList As List(Of String)
            Dim period As RASTypePeriod

            If Me.IsValid Then

                ' first load up the type and the periods for validation
                t = New RASType(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With t
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                End If
                With t
                    .Description = txtDescription.Text
                    .Redundant = chkRedundant.CheckBox.Checked
                End With

                ' get list of periods
                periodsToDelete = New List(Of String)
                periodList = GetUniqueIDsFromViewState()
                For Each uniqueID As String In periodList
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                        ' we are deleting
                        periodsToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                    Else
                        ' create the empty period
                        period = New RASTypePeriod(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                            ' we are updating
                            msg = period.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                        ' set the period properties
                        With period
                            .DateFrom = CType(phPeriods.FindControl(CTRL_PREFIX_FROM & uniqueID), TextBoxEx).Text
                            If Utils.IsDate(CType(phPeriods.FindControl(CTRL_PREFIX_TO & uniqueID), TextBoxEx).Text) Then
                                .DateTo = CType(phPeriods.FindControl(CTRL_PREFIX_TO & uniqueID), TextBoxEx).Text
                            Else
                                .DateTo = DataUtils.MAX_DATE
                            End If
                            .PoundsPerPoint = CType(phPeriods.FindControl(CTRL_PREFIX_PPP & uniqueID), TextBoxEx).Text
                        End With
                        ' add to the collection
                        periods.Add(period)
                    End If
                Next

                ' validate/save the type and save/delete the periods
                msg = SdsBL.SaveRASType(Me.DbConnection, t, periods, periodsToDelete)
                If Not msg.Success Then
                    If msg.Number = SdsBL.ERR_COULD_NOT_SAVE_RASTYPE Then
                        ' could not save ras type
                        lblError.Text = msg.Message
                        e.Cancel = True
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                End If
                e.ItemID = t.ID
            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " btnAddPeriod_Click "

        Private Sub btnAddPeriod_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddPeriod.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim newPeriod As RASTypePeriod = New RASTypePeriod(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            ' add a new row to the period list
            id = GetUniqueID(newPeriod)
            ' create the controls
            OutputPeriodControls(id, newPeriod)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " Remove_Click "

        Private Sub Remove_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim id As String = CType(sender, Button).ID.Replace(CTRL_PREFIX_REMOVED, String.Empty)

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
            For index As Integer = 0 To phPeriods.Controls.Count - 1
                If phPeriods.Controls(index).ID = id Then
                    phPeriods.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " OutputPeriodControls "

        Private Sub OutputPeriodControls(ByVal uniqueID As String, ByVal period As RASTypePeriod)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim dateFrom As TextBoxEx
            Dim dateTo As TextBoxEx
            Dim ppp As TextBoxEx
            Dim removeButton As Button

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phPeriods.Controls.Add(row)

                ' date from
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                dateFrom = New TextBoxEx()
                With dateFrom
                    .ID = CTRL_PREFIX_FROM & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.DateFormat
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    If Not period Is Nothing AndAlso Utils.IsDate(period.DateFrom) Then .Text = period.DateFrom
                End With
                cell.Controls.Add(dateFrom)

                ' date to
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                dateTo = New TextBoxEx()
                With dateTo
                    .ID = CTRL_PREFIX_TO & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.DateFormat
                    .ValidationGroup = "Save"
                    If Not period Is Nothing AndAlso Utils.IsDate(period.DateTo) Then .Text = period.DateTo
                End With
                cell.Controls.Add(dateTo)

                ' ppp
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                ppp = New TextBoxEx()
                With ppp
                    .ID = CTRL_PREFIX_PPP & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    .Width = New Unit(5, UnitType.Em)
                    If Not period Is Nothing Then .Text = period.PoundsPerPoint.ToString("F2")
                End With
                cell.Controls.Add(ppp)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cell.Style.Add("text-align", "right")
                removeButton = New Button()
                With removeButton
                    .ID = CTRL_PREFIX_REMOVED & uniqueID
                    .Text = "Remove"
                    AddHandler .Click, AddressOf Remove_Click
                End With
                cell.Controls.Add(removeButton)

            End If

        End Sub

#End Region

#Region " GetUniqueID "

        Private Function GetUniqueID(ByVal period As RASTypePeriod) As String

            Dim id As String

            If period.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW & _newPeriodIDCounter
                _newPeriodIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & period.ID
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
                _newPeriodIDCounter = 0
            Else
                _newPeriodIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER), Integer)
            End If

            Return list

        End Function

#End Region

#Region " PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER, _newPeriodIDCounter)
        End Sub

#End Region

    End Class

End Namespace