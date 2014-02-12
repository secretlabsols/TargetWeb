
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
    ''' Admin page used to maintain Service Outcome groups and Service Outcome codes.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Colin 24/01/2012  I805 - Altered remove button on groups table to be a standard red cross image rather than a button
    ''' Paul  24/08/2009  D11674 - Initial Implementation.
    ''' Paul  08/01/2010  A4WA#5989 - Extended the screen so outcomes can be for attendance and non attendance.
    ''' </history>
    Partial Public Class ServiceOutcomes
        Inherits BasePage

        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"

        Const CTRL_PREFIX_DESC As String = "desc"
        Const CTRL_PREFIX_CODE As String = "code"
        Const CTRL_PREFIX_PROVPAID As String = "provPaid"
        Const CTRL_PREFIX_CLIENTCHARGED As String = "clientChar"
        Const CTRL_PREFIX_DEFAULT As String = "default"
        Const CTRL_PREFIX_TYPE As String = "type"
        Const CTRL_PREFIX_REMOVED As String = "remove"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"

        Private _newCodeIDCounter As Integer
        Private _groupID As Integer
        Private _stdBut As StdButtonsBase
        Private _inUse As Boolean
        Private _startupJS As StringBuilder = New StringBuilder()

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOutcomes"), "Service Outcomes")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            _groupID = _stdBut.SelectedItemID
            If _groupID > 0 Then
                _inUse = ServiceOutcomeInUseByRegister(_groupID)
            Else
                _inUse = False
            End If

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOutcomes.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOutcomes.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOutcomes.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.ServiceOutcomes
                .AuditLogTableNames.Add("ServiceOutcomeGroup")
                .AuditLogTableNames.Add("ServiceOutcome")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ServiceOutcomes")
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
                OutputServiceOutcomeControls(id, Nothing)
            Next

        End Sub

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phServiceOutcomes.Controls.Clear()
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
            Dim group As ServiceOutcomeGroup
            Dim serviceOutcomes As ServiceOutcomeCollection = New ServiceOutcomeCollection
            Dim list As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            _groupID = e.ItemID

            group = New ServiceOutcomeGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With group
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
            End With

            ' get the visit codes
            msg = ServiceOutcome.FetchList(Me.DbConnection, serviceOutcomes, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), group.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' refresh the list of existing visit codes and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()
            For Each code As ServiceOutcome In serviceOutcomes
                Dim id As String = GetUniqueID(code)
                OutputServiceOutcomeControls(id, code)
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
            Dim serviceOutcomes As ServiceOutcomeCollection = Nothing

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                ' delete all of the visit codes first
                msg = ServiceOutcome.FetchList(trans, serviceOutcomes, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                For Each code As ServiceOutcome In serviceOutcomes
                    msg = ServiceOutcome.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), code.ID, e.ItemID)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        SqlHelper.RollbackTransaction(trans)
                        FindClicked(e)
                        Exit Sub
                    End If
                Next

                ' delete the group
                msg = ServiceOutcomeGroup.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
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
                msg = Utils.CatchError(ex, "E0503", "ServiceOutcomeGroup/ServiceOutcome")       ' could not delete
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim group As ServiceOutcomeGroup
            Dim ServiceOutcomes As ServiceOutcomeCollection = New ServiceOutcomeCollection
            Dim ServiceOutcomesToDelete As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim ServiceOutcomeList As List(Of String)
            Dim svcOutcome As ServiceOutcome
            'Dim inUse As Boolean

            Try
                If Me.IsValid Then

                    _inUse = ServiceOutcomeInUseByRegister(e.ItemID)

                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    ' first load up the group and the visit codes for validation
                    group = New ServiceOutcomeGroup(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
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
                    ServiceOutcomesToDelete = New List(Of String)
                    ServiceOutcomeList = GetUniqueIDsFromViewState()
                    For Each uniqueID As String In ServiceOutcomeList
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                            ' we are deleting
                            ServiceOutcomesToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                        Else
                            ' create the empty visit code
                            svcOutcome = New ServiceOutcome(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                            If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                                ' we are updating
                                msg = svcOutcome.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                            ' set the visit code properties
                            With svcOutcome
                                If _inUse Then
                                    CType(phServiceOutcomes.FindControl(CTRL_PREFIX_DESC & uniqueID), TextBoxEx).Text = .Description
                                    CType(phServiceOutcomes.FindControl(CTRL_PREFIX_CODE & uniqueID), TextBoxEx).Text = .Code
                                    CType(phServiceOutcomes.FindControl(CTRL_PREFIX_PROVPAID & uniqueID), CheckBoxEx).CheckBox.Checked = .ProviderPaid
                                    CType(phServiceOutcomes.FindControl(CTRL_PREFIX_CLIENTCHARGED & uniqueID), CheckBoxEx).CheckBox.Checked = .ClientCharged
                                    CType(phServiceOutcomes.FindControl(CTRL_PREFIX_DEFAULT & uniqueID), RadioButton).Checked = .DefaultCode
                                    If .ServiceOutcomeType Then
                                        CType(phServiceOutcomes.FindControl(CTRL_PREFIX_TYPE & uniqueID), DropDownListEx).DropDownList.SelectedValue = 1
                                    Else
                                        CType(phServiceOutcomes.FindControl(CTRL_PREFIX_TYPE & uniqueID), DropDownListEx).DropDownList.SelectedValue = 0
                                    End If
                                Else
                                    .Description = CType(phServiceOutcomes.FindControl(CTRL_PREFIX_DESC & uniqueID), TextBoxEx).Text
                                    .Code = CType(phServiceOutcomes.FindControl(CTRL_PREFIX_CODE & uniqueID), TextBoxEx).Text
                                    .ProviderPaid = CType(phServiceOutcomes.FindControl(CTRL_PREFIX_PROVPAID & uniqueID), CheckBoxEx).CheckBox.Checked
                                    .ClientCharged = CType(phServiceOutcomes.FindControl(CTRL_PREFIX_CLIENTCHARGED & uniqueID), CheckBoxEx).CheckBox.Checked
                                    .DefaultCode = CType(phServiceOutcomes.FindControl(CTRL_PREFIX_DEFAULT & uniqueID), RadioButton).Checked
                                    If CType(phServiceOutcomes.FindControl(CTRL_PREFIX_TYPE & uniqueID), DropDownListEx).DropDownList.SelectedValue = 1 Then
                                        .ServiceOutcomeType = TriState.True
                                    Else
                                        .ServiceOutcomeType = TriState.False
                                    End If
                                End If
                            End With
                            ' add to the collection
                            ServiceOutcomes.Add(svcOutcome)
                        End If
                    Next

                    ' validate the group and the Service Outcomes
                    msg = DomContractBL.ValidateServiceOutcomeGroup(group, ServiceOutcomes)
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
                        If Not _inUse Then
                            For Each svcOutcome In ServiceOutcomes
                                svcOutcome.ServiceOutcomeGroupID = group.ID
                                svcOutcome.AuditLogOverriddenParentID = group.ID
                                msg = svcOutcome.Save()
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            Next
                            For Each id As String In ServiceOutcomesToDelete
                                msg = ServiceOutcome.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), id, group.ID)
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
                msg = Utils.CatchError(ex, "E0502", "ServiceOutcomeGroup/ServiceOutcome")   ' could not save
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

#Region " btnAddServiceOutcome_Click "

        Private Sub btnAddServiceOutcome_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddServiceOutcome.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim newServiceOutcome As ServiceOutcome = New ServiceOutcome(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            ' add a new row to the visit codes list
            id = GetUniqueID(newServiceOutcome)
            ' create the controls
            newServiceOutcome.ServiceOutcomeType = TriState.UseDefault
            OutputServiceOutcomeControls(id, newServiceOutcome)
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
            For index As Integer = 0 To phServiceOutcomes.Controls.Count - 1
                If phServiceOutcomes.Controls(index).ID = id Then
                    phServiceOutcomes.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " OutputServiceOutcomeControls "

        Private Sub OutputServiceOutcomeControls(ByVal uniqueID As String, ByVal svcOutcome As ServiceOutcome)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim description As TextBoxEx
            Dim code As TextBoxEx
            Dim provPaid As CheckBoxEx
            Dim clientChar As CheckBoxEx
            Dim rdoDefault As RadioButton
            Dim removeButton As ImageButton
            Dim soType As DropDownListEx

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phServiceOutcomes.Controls.Add(row)

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
                    If Not svcOutcome Is Nothing Then .Text = svcOutcome.Description
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
                    '.Required = True
                    '.RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    .Width = New Unit(5, UnitType.Em)
                    If Not svcOutcome Is Nothing Then .Text = svcOutcome.Code
                End With
                cell.Controls.Add(code)

                ' provider paid
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                provPaid = New CheckBoxEx()
                With provPaid
                    .ID = CTRL_PREFIX_PROVPAID & uniqueID
                    If Not svcOutcome Is Nothing Then .CheckBox.Checked = svcOutcome.ProviderPaid
                End With
                cell.Controls.Add(provPaid)

                ' client charged
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                clientChar = New CheckBoxEx()
                With clientChar
                    .ID = CTRL_PREFIX_CLIENTCHARGED & uniqueID
                    If Not svcOutcome Is Nothing Then .CheckBox.Checked = svcOutcome.ClientCharged
                End With
                cell.Controls.Add(clientChar)

                ' default radio button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                rdoDefault = New RadioButton()
                With rdoDefault
                    .ID = CTRL_PREFIX_DEFAULT & uniqueID
                    .GroupName = "default"
                    If Not svcOutcome Is Nothing Then
                        .Checked = svcOutcome.DefaultCode
                    End If

                End With
                cell.Controls.Add(rdoDefault)


                'Service Outcome Type
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                soType = New DropDownListEx
                With soType
                    .ID = CTRL_PREFIX_TYPE & uniqueID
                    .DropDownList.Items.Add(New ListItem("Non Attendance", 0))
                    .DropDownList.Items.Add(New ListItem("Attendance", 1))
                    If Not svcOutcome Is Nothing Then
                        If svcOutcome.ServiceOutcomeType Then
                            .DropDownList.SelectedValue = 1
                        ElseIf svcOutcome.ServiceOutcomeType = TriState.False Then
                            .DropDownList.SelectedValue = 0
                        Else
                            .DropDownList.SelectedValue = 1
                        End If
                    End If

                End With
                cell.Controls.Add(soType)

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

        Private Function GetUniqueID(ByVal svcOutcome As ServiceOutcome) As String

            Dim id As String

            If svcOutcome.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW & _newCodeIDCounter
                _newCodeIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & svcOutcome.ID
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


            ' cannot edit/delete existing Service Outcome codes if one or more code is in use
            If _inUse Then
                For Each row As HtmlTableRow In phServiceOutcomes.Controls
                    If row.ID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                        WebUtils.RecursiveDisable(row.Controls, True)
                    End If
                Next
                btnAddServiceOutcome.Enabled = False
            End If


        End Sub

#End Region

#Region " VisitCodeGroupInUseByContractPeriod "

        ''' <summary>
        ''' Determines if the specified visit code group is in use by one or more domiciliary contract period(s).
        ''' </summary>
        ''' <param name="GroupID">The ID of the Service outcome group to check.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ServiceOutcomeInUseByRegister(ByVal GroupID As Integer) As Boolean

            Const SP_NAME As String = "spxServiceOutcome_InUse"

            Dim msg As ErrorMessage = Nothing
            Dim spParams As SqlParameter()

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME)
                spParams(0).Value = GroupID
                spParams(1).Direction = ParameterDirection.InputOutput

                SqlHelper.ExecuteNonQuery(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                ServiceOutcomeInUseByRegister = Convert.ToBoolean(spParams(1).Value)

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber, "ServiceOutcomeGroup/ServiceOutcomeInUseByRegister")
                WebUtils.DisplayError(msg)
            End Try

        End Function

#End Region


    End Class

End Namespace
