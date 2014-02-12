
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Reflection
Imports System.Text
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
	''' Admin page used to maintain domiciliary visit code groups and visit codes.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  12/05/2009  D11549 - added reporting support.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' MvO  03/03/2008  Added DefaultCode radio buttons.
    ''' </history>
	Partial Public Class VisitCodeGroups
		Inherits BasePage

		Const VIEWSTATE_KEY_DATA As String = "DataList"
		Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"

		Const CTRL_PREFIX_DESC As String = "desc"
		Const CTRL_PREFIX_CODE As String = "code"
		Const CTRL_PREFIX_PROVPAID As String = "provPaid"
		Const CTRL_PREFIX_CLIENTCHARGED As String = "clientChar"
		Const CTRL_PREFIX_STDTIMEPAID As String = "stdTimePaid"
        Const CTRL_PREFIX_MAXTIMEPAID As String = "maxTimePaid"
        Const CTRL_PREFIX_DEFAULT As String = "default"
		Const CTRL_PREFIX_REMOVED As String = "remove"

		Const UNIQUEID_PREFIX_NEW As String = "N"
		Const UNIQUEID_PREFIX_UPDATE As String = "U"
		Const UNIQUEID_PREFIX_DELETE As String = "D"

        Private _newCodeIDCounter As Integer
        Private _groupID As Integer
        Private _stdBut As StdButtonsBase

#Region " Page_Load "

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.VisitCodes"), "Visit Code Groups")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            _groupID = _stdBut.SelectedItemID

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.VisitCodes.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.VisitCodes.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.VisitCodes.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.DomVisitCodeGroup
                .AuditLogTableNames.Add("DomVisitCodeGroup")
                .AuditLogTableNames.Add("DomVisitCode")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.VisitCodes")
            End With
            AddHandler _stdBut.NewClicked, AddressOf ClearViewState
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

			' re-create the list of visit codes (from view state)
			Dim list As List(Of String) = GetUniqueIDsFromViewState()
			For Each id As String In list
				OutputVisitCodeControls(id, Nothing)
			Next

		End Sub

#End Region

#Region " ClearViewState "

		Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
			ViewState.Remove(VIEWSTATE_KEY_DATA)
			phVisitCodes.Controls.Clear()
		End Sub

#End Region

#Region " CancelClicked "

		Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
			ClearViewState(e)
			If e.ItemID = 0 Then
				txtDescription.Text = String.Empty
                chkRedundant.CheckBox.Checked = False
                _groupID = 0
			Else
				FindClicked(e)
			End If
		End Sub

#End Region

#Region " FindClicked "

		Private Sub FindClicked(ByRef e As StdButtonEventArgs)

			Dim msg As ErrorMessage
			Dim group As DomVisitCodeGroup
			Dim visitCodes As DomVisitCodeCollection = New DomVisitCodeCollection
			Dim list As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            _groupID = e.ItemID

            group = New DomVisitCodeGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With group
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
            End With

            ' get the visit codes
            msg = DomVisitCode.FetchList(Me.DbConnection, visitCodes, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), group.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' refresh the list of existing visit codes and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()
            For Each code As DomVisitCode In visitCodes
                Dim id As String = GetUniqueID(code)
                OutputVisitCodeControls(id, code)
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
            Dim visitCodes As DomVisitCodeCollection = Nothing

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                ' delete all of the visit codes first
                msg = DomVisitCode.FetchList(trans, visitCodes, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                For Each code As DomVisitCode In visitCodes
                    msg = DomVisitCode.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), code.ID, e.ItemID)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        SqlHelper.RollbackTransaction(trans)
                        FindClicked(e)
                        Exit Sub
                    End If
                Next

                ' delete the group
                msg = DomVisitCodeGroup.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
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

                _groupID = 0

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0503", "DomVisitCodeGroup/DomVisitCode")       ' could not delete
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim group As DomVisitCodeGroup
            Dim visitCodes As DomVisitCodeCollection = New DomVisitCodeCollection
            Dim visitCodesToDelete As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim visitCodeList As List(Of String)
            Dim visitCode As DomVisitCode
            Dim inUse As Boolean

            _groupID = e.ItemID

            Try
                If Me.IsValid Then

                    msg = DomContractBL.VisitCodeGroupInUseByContractPeriod(Me.DbConnection, _groupID, inUse)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    ' first load up the group and the visit codes for validation
                    group = New DomVisitCodeGroup(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
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

                    ' get list of visit codes
                    visitCodesToDelete = New List(Of String)
                    visitCodeList = GetUniqueIDsFromViewState()
                    For Each uniqueID As String In visitCodeList
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                            ' we are deleting
                            visitCodesToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                        Else
                            ' create the empty visit code
                            visitCode = New DomVisitCode(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                            If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                                ' we are updating
                                msg = visitCode.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                            ' set the visit code properties
                            With visitCode
                                If inUse Then
                                    CType(phVisitCodes.FindControl(CTRL_PREFIX_DESC & uniqueID), TextBoxEx).Text = .Description
                                    CType(phVisitCodes.FindControl(CTRL_PREFIX_CODE & uniqueID), TextBoxEx).Text = .Code
                                    CType(phVisitCodes.FindControl(CTRL_PREFIX_PROVPAID & uniqueID), CheckBoxEx).CheckBox.Checked = .ProviderPaid
                                    CType(phVisitCodes.FindControl(CTRL_PREFIX_CLIENTCHARGED & uniqueID), CheckBoxEx).CheckBox.Checked = .ClientCharged
                                    CType(phVisitCodes.FindControl(CTRL_PREFIX_STDTIMEPAID & uniqueID), TextBoxEx).Text = .StandardTimePaid
                                    CType(phVisitCodes.FindControl(CTRL_PREFIX_MAXTIMEPAID & uniqueID), TextBoxEx).Text = .MaximumTimePaid
                                    CType(phVisitCodes.FindControl(CTRL_PREFIX_DEFAULT & uniqueID), RadioButton).Checked = .DefaultCode
                                Else
                                    .Description = CType(phVisitCodes.FindControl(CTRL_PREFIX_DESC & uniqueID), TextBoxEx).Text
                                    .Code = CType(phVisitCodes.FindControl(CTRL_PREFIX_CODE & uniqueID), TextBoxEx).Text
                                    .ProviderPaid = CType(phVisitCodes.FindControl(CTRL_PREFIX_PROVPAID & uniqueID), CheckBoxEx).CheckBox.Checked
                                    .ClientCharged = CType(phVisitCodes.FindControl(CTRL_PREFIX_CLIENTCHARGED & uniqueID), CheckBoxEx).CheckBox.Checked
                                    .StandardTimePaid = CType(phVisitCodes.FindControl(CTRL_PREFIX_STDTIMEPAID & uniqueID), TextBoxEx).Text
                                    .MaximumTimePaid = CType(phVisitCodes.FindControl(CTRL_PREFIX_MAXTIMEPAID & uniqueID), TextBoxEx).Text
                                    .DefaultCode = CType(phVisitCodes.FindControl(CTRL_PREFIX_DEFAULT & uniqueID), RadioButton).Checked
                                End If
                            End With
                            ' add to the collection
                            visitCodes.Add(visitCode)
                        End If
                    Next

                    ' validate the group and the visit codes
                    msg = DomContractBL.ValidateVisitCodeGroup(group, visitCodes)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                    Else
                        ' save the group first
                        With group
                            msg = .Save()
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            e.ItemID = .ID
                        End With

                        ' save/delete each of the visit codes
                        If Not inUse Then
                            For Each visitCode In visitCodes
                                visitCode.DomVisitCodeGroupID = group.ID
                                visitCode.AuditLogOverriddenParentID = group.ID
                                msg = visitCode.Save()
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            Next
                            For Each id As String In visitCodesToDelete
                                msg = DomVisitCode.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), id, group.ID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            Next
                        End If

                        trans.Commit()

                        msg.Success = True

                    End If
                Else
                    e.Cancel = True
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0502", "DomVisitCodeGroup/DomVisitCode")   ' could not save
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

#Region " btnAddVisitCode_Click "

        Private Sub btnAddVisitCode_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddVisitCode.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim newVisitCode As DomVisitCode = New DomVisitCode(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            ' add a new row to the visit codes list
            id = GetUniqueID(newVisitCode)
            ' create the controls
            OutputVisitCodeControls(id, newVisitCode)
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
			For index As Integer = 0 To phVisitCodes.Controls.Count - 1
				If phVisitCodes.Controls(index).ID = id Then
					phVisitCodes.Controls.RemoveAt(index)
					Exit For
				End If
			Next

			' persist the data into view state
			PersistUniqueIDsToViewState(list)

		End Sub

#End Region

#Region " OutputVisitCodeControls "

		Private Sub OutputVisitCodeControls(ByVal uniqueID As String, ByVal visitCode As DomVisitCode)

			Dim row As HtmlTableRow
			Dim cell As HtmlTableCell
			Dim description As TextBoxEx
			Dim code As TextBoxEx
			Dim provPaid As CheckBoxEx
			Dim clientChar As CheckBoxEx
			Dim stdTimePaid As TextBoxEx
            Dim maxTimePaid As TextBoxEx
            Dim rdoDefault As RadioButton
            Dim removeButton As ImageButton

			' don't output items marked as deleted
			If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

				row = New HtmlTableRow()
				row.ID = uniqueID
				phVisitCodes.Controls.Add(row)

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
					.Width = New Unit(15, UnitType.Em)
					If Not visitCode Is Nothing Then .Text = visitCode.Description
				End With
				cell.Controls.Add(description)

				' code
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				code = New TextBoxEx()
				With code
					.ID = CTRL_PREFIX_CODE & uniqueID
					.MaxLength = 50
					.Required = True
					.RequiredValidatorErrMsg = "* Required"
					.ValidationGroup = "Save"
					.Width = New Unit(5, UnitType.Em)
					If Not visitCode Is Nothing Then .Text = visitCode.Code
				End With
				cell.Controls.Add(code)

				' provider paid
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				provPaid = New CheckBoxEx()
				With provPaid
					.ID = CTRL_PREFIX_PROVPAID & uniqueID
					If Not visitCode Is Nothing Then .CheckBox.Checked = visitCode.ProviderPaid
				End With
				cell.Controls.Add(provPaid)

				' client charged
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				clientChar = New CheckBoxEx()
				With clientChar
					.ID = CTRL_PREFIX_CLIENTCHARGED & uniqueID
					If Not visitCode Is Nothing Then .CheckBox.Checked = visitCode.ClientCharged
				End With
				cell.Controls.Add(clientChar)

				' std time paid
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				stdTimePaid = New TextBoxEx()
				With stdTimePaid
					.ID = CTRL_PREFIX_STDTIMEPAID & uniqueID
					.Format = TextBoxEx.TextBoxExFormat.IntegerFormat
					.Required = True
					.RequiredValidatorErrMsg = "* Required"
					.ValidationGroup = "Save"
					.Width = New Unit(5, UnitType.Em)
					If Not visitCode Is Nothing Then .Text = visitCode.StandardTimePaid
				End With
				cell.Controls.Add(stdTimePaid)

				' max time paid
				cell = New HtmlTableCell()
				row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
				maxTimePaid = New TextBoxEx()
				With maxTimePaid
					.ID = CTRL_PREFIX_MAXTIMEPAID & uniqueID
					.Format = TextBoxEx.TextBoxExFormat.IntegerFormat
					.Required = True
					.RequiredValidatorErrMsg = "* Required"
					.ValidationGroup = "Save"
					.Width = New Unit(5, UnitType.Em)
					If Not visitCode Is Nothing Then .Text = visitCode.MaximumTimePaid
				End With
                cell.Controls.Add(maxTimePaid)

                ' default radio button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                rdoDefault = New RadioButton()
                With rdoDefault
                    .ID = CTRL_PREFIX_DEFAULT & uniqueID
                    .GroupName = "default"
                    If Not visitCode Is Nothing Then .Checked = visitCode.DefaultCode
                End With
                cell.Controls.Add(rdoDefault)

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

		Private Function GetUniqueID(ByVal visitCode As DomVisitCode) As String

			Dim id As String

			If visitCode.ID = 0 Then
				id = UNIQUEID_PREFIX_NEW & _newCodeIDCounter
				_newCodeIDCounter += 1
			Else
				id = UNIQUEID_PREFIX_UPDATE & visitCode.ID
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
				_newCodeIDCounter = 0
			Else
				_newCodeIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER), Integer)
			End If

			Return list

		End Function

#End Region

#Region " PersistUniqueIDsToViewState "

		Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
			ViewState.Add(VIEWSTATE_KEY_DATA, list)
			ViewState.Add(VIEWSTATE_KEY_COUNTER, _newCodeIDCounter)
		End Sub

#End Region

#Region " Page_PreRenderComplete "

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim msg As ErrorMessage
            Dim inUse As Boolean

            msg = DomContractBL.VisitCodeGroupInUseByContractPeriod(Me.DbConnection, _groupID, inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' cannot edit/delete existing visit codes if the group is in use
            If inUse Then
                For Each row As HtmlTableRow In phVisitCodes.Controls
                    If row.ID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                        WebUtils.RecursiveDisable(row.Controls, True)
                    End If
                Next
                btnAddVisitCode.Enabled = False
            End If

        End Sub

#End Region

    End Class

End Namespace
