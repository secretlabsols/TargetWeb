
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

Namespace Apps.Admin.Dom

    ''' <summary>
	''' Admin page used to maintain domiciliary manually amended indicator groups and manually amended indicators.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  12/05/2009  D11549 - added reporting support.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' MvO  03/03/2008  Added UseForManualVisits support.
    ''' </history>
	Partial Public Class ManAmendIndGroups
		Inherits BasePage

		Const VIEWSTATE_KEY_DATA As String = "DataList"
		Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"

		Const CTRL_PREFIX_DESC As String = "desc"
        Const CTRL_PREFIX_CODE As String = "code"
        Const CTRL_PREFIX_USE_FOR_MANUAL As String = "useForManual"
		Const CTRL_PREFIX_REMOVED As String = "remove"

		Const UNIQUEID_PREFIX_NEW As String = "N"
		Const UNIQUEID_PREFIX_UPDATE As String = "U"
		Const UNIQUEID_PREFIX_DELETE As String = "D"

        Private _newIndIDCounter As Integer
        Private _stdBut As StdButtonsBase

#Region " Page_Load "

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ManuallyAmendedindicators"), "Manually Amended Indicator Groups")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManuallyAmendedindicators.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManuallyAmendedindicators.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManuallyAmendedindicators.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.DomManuallyAmendedIndicatorGroup
                .AuditLogTableNames.Add("DomManuallyAmendedIndicatorGroup")
                .AuditLogTableNames.Add("DomManuallyAmendedIndicator")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ManuallyAmendedIndicators")
            End With
            AddHandler _stdBut.NewClicked, AddressOf ClearViewState
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

			' re-create the list of bands (from view state)
			Dim list As List(Of String) = GetUniqueIDsFromViewState()
			For Each id As String In list
				OutputIndicatorControls(id, Nothing)
			Next

		End Sub

#End Region

#Region " ClearViewState "

		Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
			ViewState.Remove(VIEWSTATE_KEY_DATA)
			phIndicators.Controls.Clear()
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
			Dim group As DomManuallyAmendedIndicatorGroup
			Dim inds As DomManuallyAmendedIndicatorCollection = New DomManuallyAmendedIndicatorCollection
			Dim list As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            group = New DomManuallyAmendedIndicatorGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With group
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
            End With

            ' get the indicators
            msg = DomManuallyAmendedIndicator.FetchList(Me.DbConnection, inds, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), group.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' refresh the list of existing bands and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()
            For Each ind As DomManuallyAmendedIndicator In inds
                Dim id As String = GetUniqueID(ind)
                OutputIndicatorControls(id, ind)
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
            Dim inds As DomManuallyAmendedIndicatorCollection = Nothing

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                ' delete all of the indicators first
                msg = DomManuallyAmendedIndicator.FetchList(trans, inds, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                For Each ind As DomManuallyAmendedIndicator In inds
                    msg = DomManuallyAmendedIndicator.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), ind.ID, e.ItemID)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        SqlHelper.RollbackTransaction(trans)
                        FindClicked(e)
                        Exit Sub
                    End If
                Next

                ' delete the group
                msg = DomManuallyAmendedIndicatorGroup.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    SqlHelper.RollbackTransaction(trans)
                    FindClicked(e)
                    Exit Sub
                End If

                trans.Commit()

                ClearViewState(e)

                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0503", "DomManuallyAmendedIndicatorGroup/DomManuallyAmendedIndicator")     ' could not delete
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim group As DomManuallyAmendedIndicatorGroup
            Dim inds As DomManuallyAmendedIndicatorCollection = New DomManuallyAmendedIndicatorCollection
            Dim indsToDelete As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim indList As List(Of String)
            Dim ind As DomManuallyAmendedIndicator
            Dim trans As SqlTransaction = Nothing
            Dim inUse As Boolean

            If Me.IsValid Then

                msg = DomContractBL.ManAmendIndGroupInUseByContractPeriod(Me.DbConnection, _stdBut.SelectedItemID, inUse)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' first load up the group and the indicators for validation
                group = New DomManuallyAmendedIndicatorGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With group
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                End If
                With group
                    .Description = txtDescription.Text
                    .Redundant = chkRedundant.CheckBox.Checked
                End With

                ' get list of indicators
                indsToDelete = New List(Of String)
                indList = GetUniqueIDsFromViewState()
                For Each uniqueID As String In indList
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                        ' we are deleting
                        indsToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                    Else
                        ' create the empty indicator
                        ind = New DomManuallyAmendedIndicator(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                            ' we are updating
                            msg = ind.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                        ' set the indicator properties
                        With ind
                            If inUse Then
                                CType(phIndicators.FindControl(CTRL_PREFIX_DESC & uniqueID), TextBoxEx).Text = .Description
                                CType(phIndicators.FindControl(CTRL_PREFIX_CODE & uniqueID), TextBoxEx).Text = .Code
                                CType(phIndicators.FindControl(CTRL_PREFIX_USE_FOR_MANUAL & uniqueID), RadioButton).Checked = .UseForManuallyEnteredVisits
                            Else
                                .Description = CType(phIndicators.FindControl(CTRL_PREFIX_DESC & uniqueID), TextBoxEx).Text
                                .Code = CType(phIndicators.FindControl(CTRL_PREFIX_CODE & uniqueID), TextBoxEx).Text
                                .UseForManuallyEnteredVisits = CType(phIndicators.FindControl(CTRL_PREFIX_USE_FOR_MANUAL & uniqueID), RadioButton).Checked
                            End If

                        End With
                        ' add to the collection
                        inds.Add(ind)
                    End If
                Next

                ' validate the group and the indicators
                msg = DomContractBL.ValidateManAmendIndGroup(group, inds)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                Else
                    Try
                        trans = SqlHelper.GetTransaction(Me.DbConnection)

                        ' save the group and save/delete the bands
                        group.DbTransaction = trans
                        msg = group.Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' delete indicators
                        For Each id As String In indsToDelete
                            msg = DomManuallyAmendedIndicator.Delete(trans, group.AuditUserName, group.AuditLogTitle, id, group.ID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        Next

                        ' save indicators
                        For Each ind In inds
                            With ind
                                .DbTransaction = trans
                                .DomManuallyAmendedIndicatorGroupID = group.ID
                                .AuditLogOverriddenParentID = group.ID
                                msg = .Save()
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End With
                        Next
                        e.ItemID = group.ID

                        trans.Commit()

                        msg.Success = True

                    Catch ex As Exception
                        msg = Utils.CatchError(ex, "E0502", "DomManuallyAmendedIndicatorGroup/DomManuallyAmendedIndicator")     ' could not save
                        WebUtils.DisplayError(msg)
                    Finally
                        If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                    End Try

                End If

            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " btnAddIndicator_Click "

        Private Sub btnAddIndicator_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddIndicator.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim newInd As DomManuallyAmendedIndicator = New DomManuallyAmendedIndicator(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            ' add a new row to the indicators list
            id = GetUniqueID(newInd)
            ' create the controls
            OutputIndicatorControls(id, newInd)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " Remove_Click "

		Private Sub Remove_Click(ByVal sender As Object, ByVal e As System.EventArgs)

			Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim id As String = CType(sender, ImageButton).ID.Replace(CTRL_PREFIX_REMOVED, String.Empty)

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
			For index As Integer = 0 To phIndicators.Controls.Count - 1
				If phIndicators.Controls(index).ID = id Then
					phIndicators.Controls.RemoveAt(index)
					Exit For
				End If
			Next

			' persist the data into view state
			PersistUniqueIDsToViewState(list)

		End Sub

#End Region

#Region " OutputIndicatorControls "

		Private Sub OutputIndicatorControls(ByVal uniqueID As String, ByVal ind As DomManuallyAmendedIndicator)

			Dim row As HtmlTableRow
			Dim cell As HtmlTableCell
			Dim description As TextBoxEx
            Dim code As TextBoxEx
            Dim useForManual As RadioButton
            Dim removeButton As ImageButton

			' don't output items marked as deleted
			If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

				row = New HtmlTableRow()
				row.ID = uniqueID
				phIndicators.Controls.Add(row)

				' description
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				description = New TextBoxEx()
				With description
					.ID = CTRL_PREFIX_DESC & uniqueID
					.MaxLength = 255
					.Required = True
					.RequiredValidatorErrMsg = "* Required"
					.ValidationGroup = "Save"
					.Width = New Unit(20, UnitType.Em)
					If Not ind Is Nothing Then .Text = ind.Description
				End With
				cell.Controls.Add(description)

				' code
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				code = New TextBoxEx()
				With code
					.ID = CTRL_PREFIX_CODE & uniqueID
					.MaxLength = 10
					.Required = True
					.RequiredValidatorErrMsg = "* Required"
					.ValidationGroup = "Save"
					If Not ind Is Nothing Then .Text = ind.Code
				End With
                cell.Controls.Add(code)

                ' use for manual
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                useForManual = New RadioButton()
                With useForManual
                    .ID = CTRL_PREFIX_USE_FOR_MANUAL & uniqueID
                    .GroupName = "useForManual"
                    If Not ind Is Nothing Then .Checked = ind.UseForManuallyEnteredVisits
                End With
                cell.Controls.Add(useForManual)

				' remove button
                cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "middle")
                cell.Style.Add("text-align", "right")
                removeButton = New ImageButton()
				With removeButton
					.ID = CTRL_PREFIX_REMOVED & uniqueID
                    .ImageUrl = WebUtils.GetVirtualPath("images/delete.png")
                    .Attributes.Add("class", "right")
                    AddHandler .Click, AddressOf Remove_Click
				End With
				cell.Controls.Add(removeButton)

			End If

		End Sub

#End Region

#Region " GetUniqueID "

		Private Function GetUniqueID(ByVal ind As DomManuallyAmendedIndicator) As String

			Dim id As String

			If ind.ID = 0 Then
				id = UNIQUEID_PREFIX_NEW & _newIndIDCounter
				_newIndIDCounter += 1
			Else
				id = UNIQUEID_PREFIX_UPDATE & ind.ID
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
				_newIndIDCounter = 0
			Else
				_newIndIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER), Integer)
			End If

			Return list

		End Function

#End Region

#Region " PersistUniqueIDsToViewState "

		Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
			ViewState.Add(VIEWSTATE_KEY_DATA, list)
			ViewState.Add(VIEWSTATE_KEY_COUNTER, _newIndIDCounter)
		End Sub

#End Region

#Region " Page_PreRenderComplete "

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim msg As ErrorMessage
            Dim inUse As Boolean

            msg = DomContractBL.ManAmendIndGroupInUseByContractPeriod(Me.DbConnection, _stdBut.SelectedItemID, inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' cannot edit/delete existing indicators if the group is in use
            If inUse Then
                For Each row As HtmlTableRow In phIndicators.Controls
                    If row.ID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                        WebUtils.RecursiveDisable(row.Controls, True)
                    End If
                Next
                btnAddIndicator.Enabled = False
            End If

        End Sub

#End Region

    End Class

End Namespace
