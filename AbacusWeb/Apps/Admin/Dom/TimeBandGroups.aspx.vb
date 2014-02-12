
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
	''' Admin page used to maintain domiciliary time band groups and time bands.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/03/2010  A4WA#6166 - fixed calls to vwDomRateCategory.FetchList().
    '''     MikeVO      12/05/2009  D11549 - added reporting support.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
	Partial Public Class TimeBandGroups
		Inherits BasePage

		Const VIEWSTATE_KEY_DATA As String = "DataList"
		Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"

		Const CTRL_PREFIX_DESC As String = "desc"
		Const CTRL_PREFIX_ABBREV As String = "abbrev"
		Const CTRL_PREFIX_FROM As String = "from"
		Const CTRL_PREFIX_TO As String = "to"
		Const CTRL_PREFIX_REMOVED As String = "remove"

		Const UNIQUEID_PREFIX_NEW As String = "N"
		Const UNIQUEID_PREFIX_UPDATE As String = "U"
		Const UNIQUEID_PREFIX_DELETE As String = "D"

        Private _newBandIDCounter As Integer
        Private _inUse As Boolean

#Region " Page_Load "

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.TimeBands"), "Time Band Groups")

			Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.TimeBands.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.TimeBands.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.TimeBands.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.DomTimeBandGroup
                .AuditLogTableNames.Add("DomTimeBandGroup")
                .AuditLogTableNames.Add("DomTimeBand")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.TimeBands")
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
				OutputBandControls(id, Nothing)
			Next

		End Sub

#End Region

#Region " ClearViewState "

		Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
			ViewState.Remove(VIEWSTATE_KEY_DATA)
			phBands.Controls.Clear()
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
			Dim group As DomTimeBandGroup
			Dim bands As DomTimeBandCollection = New DomTimeBandCollection
			Dim list As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim rateCategories As vwDomRateCategoryCollection = Nothing

            group = New DomTimeBandGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With group
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
            End With

            ' get the bands
            msg = DomTimeBand.FetchList(Me.DbConnection, bands, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), group.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'Check if one or more of the bands are in use
            For Each band As DomTimeBand In bands
                msg = vwDomRateCategory.FetchList(conn:=Me.DbConnection, _
                                                  list:=rateCategories, _
                                                  domTimeBandID:=band.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If rateCategories.Count > 0 Then
                    _inUse = True
                    Exit For
                End If
            Next

            ' refresh the list of existing bands and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()
            For Each band As DomTimeBand In bands
                Dim id As String = GetUniqueID(band)
                OutputBandControls(id, band)
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
            Dim bands As DomTimeBandCollection = Nothing

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                ' delete all of the bands first
                msg = DomTimeBand.FetchList(trans, bands, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                For Each band As DomTimeBand In bands
                    msg = DomTimeBand.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), band.ID, e.ItemID)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        SqlHelper.RollbackTransaction(trans)
                        FindClicked(e)
                        Exit Sub
                    End If
                Next

                ' delete the group
                msg = DomTimeBandGroup.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    SqlHelper.RollbackTransaction(trans)
                    FindClicked(e)
                End If

                trans.Commit()

                ClearViewState(e)

                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0503", "DomTimeBandGroup/DomTimeBand")     ' could not delete
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim group As DomTimeBandGroup
            Dim bands As DomTimeBandCollection = New DomTimeBandCollection
            Dim bandsToDelete As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim bandList As List(Of String)
            Dim band As DomTimeBand
            Dim time As TimePicker

            If Me.IsValid Then

                ' first load up the group and the bands for validation
                group = New DomTimeBandGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
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

                ' check if time bands are in use
                msg = DomContractBL.AreTimeBandsInUse(Me.DbConnection, Nothing, e.ItemID, _inUse)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' get list of bands
                bandsToDelete = New List(Of String)
                bandList = GetUniqueIDsFromViewState()
                For Each uniqueID As String In bandList
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                        ' we are deleting
                        bandsToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                    Else
                        ' create the empty band
                        band = New DomTimeBand(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                            ' we are updating
                            msg = band.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                        ' set the band properties
                        With band
                            If Not _inUse Then
                                .Description = CType(phBands.FindControl(CTRL_PREFIX_DESC & uniqueID), TextBoxEx).Text
                                .Abbreviation = CType(phBands.FindControl(CTRL_PREFIX_ABBREV & uniqueID), TextBoxEx).Text
                                time = CType(phBands.FindControl(CTRL_PREFIX_FROM & uniqueID), TimePicker)
                                .TimeFrom = DateTime.Parse(time.ToString(DomContractBL.TIME_ONLY_DATE))
                                time = CType(phBands.FindControl(CTRL_PREFIX_TO & uniqueID), TimePicker)
                                .TimeTo = DateTime.Parse(time.ToString(DomContractBL.TIME_ONLY_DATE))

                                ' if the time period crosses the midnight boundary, save the TimeTo as the next day
                                If .TimeTo <= .TimeFrom Then .TimeTo = .TimeTo.AddDays(1)
                            End If
                        End With
                        ' add to the collection
                        bands.Add(band)
                    End If
                Next

                ' validate/save the group and save/delete the bands
                msg = DomContractBL.SaveTimeBandGroup(Me.DbConnection, group, bands, bandsToDelete)
                If Not msg.Success Then
                    If msg.Number = "E3004" Or msg.Number = "E3001" Then
                        ' rate category abbrev invalid or could not save time band group
                        lblError.Text = msg.Message
                        e.Cancel = True
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                End If
                e.ItemID = group.ID

                FindClicked(e)

            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " btnAddBand_Click "

        Private Sub btnAddBand_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddBand.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim newBand As DomTimeBand = New DomTimeBand(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            ' add a new row to the time bands list
            id = GetUniqueID(newBand)
            ' create the controls
            OutputBandControls(id, newBand)
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
			For index As Integer = 0 To phBands.Controls.Count - 1
				If phBands.Controls(index).ID = id Then
					phBands.Controls.RemoveAt(index)
					Exit For
				End If
			Next

			' persist the data into view state
			PersistUniqueIDsToViewState(list)

		End Sub

#End Region

#Region " OutputBandControls "

		Private Sub OutputBandControls(ByVal uniqueID As String, ByVal band As DomTimeBand)

			Dim row As HtmlTableRow
			Dim cell As HtmlTableCell
			Dim description As TextBoxEx
			Dim abbreviation As TextBoxEx
			Dim fromTime As TimePicker
			Dim toTime As TimePicker
            Dim removeButton As ImageButton

			' don't output items marked as deleted
			If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

				row = New HtmlTableRow()
				row.ID = uniqueID
				phBands.Controls.Add(row)

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
					If Not band Is Nothing Then .Text = band.Description
				End With
				cell.Controls.Add(description)

				' abbreviation
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				abbreviation = New TextBoxEx()
				With abbreviation
					.ID = CTRL_PREFIX_ABBREV & uniqueID
					.MaxLength = 50
					If Not band Is Nothing Then .Text = band.Abbreviation
				End With
				cell.Controls.Add(abbreviation)

				' from
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				fromTime = New TimePicker()
				With fromTime
					.ID = CTRL_PREFIX_FROM & uniqueID
					.ShowSeconds = False
					If Not band Is Nothing Then
						.Hours = band.TimeFrom.Hour
						.Minutes = band.TimeFrom.Minute
					End If
				End With
				cell.Controls.Add(fromTime)

				' to
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				toTime = New TimePicker()
				With toTime
					.ID = CTRL_PREFIX_TO & uniqueID
					.ShowSeconds = False
					If Not band Is Nothing Then
						.Hours = band.TimeTo.Hour
						.Minutes = band.TimeTo.Minute
					End If
				End With
				cell.Controls.Add(toTime)

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

		Private Function GetUniqueID(ByVal band As DomTimeBand) As String

			Dim id As String

			If band.ID = 0 Then
				id = UNIQUEID_PREFIX_NEW & _newBandIDCounter
				_newBandIDCounter += 1
			Else
				id = UNIQUEID_PREFIX_UPDATE & band.ID
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
				_newBandIDCounter = 0
			Else
				_newBandIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER), Integer)
			End If

			Return list

		End Function

#End Region

#Region " PersistUniqueIDsToViewState "

		Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
			ViewState.Add(VIEWSTATE_KEY_DATA, list)
			ViewState.Add(VIEWSTATE_KEY_COUNTER, _newBandIDCounter)
		End Sub

#End Region

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            If _inUse Then
                WebUtils.RecursiveDisable(phBands.Controls, True)
                btnAddBand.Enabled = False
            End If

        End Sub
    End Class

End Namespace
