
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
Imports Target.Abacus.Library.SDS
Imports System.Text

Namespace Apps.Admin.ServiceUsers

    ''' <summary>
    ''' Admin page used to maintain budget budgetPeriods for individual service users.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Mo Tahir 14/07/2010  D11798 - Budget Holders.
    ''' </history>
    Partial Public Class ServiceUserBudgetPeriods
        Inherits BasePage

        Private Const SETTING_DEFAULT_BUDGET_PERIOD As String = "DefaultBudgetPeriod"

        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"

        Const CTRL_PREFIX_FROM As String = "from"
        Const CTRL_PREFIX_TO As String = "to"
        Const CTRL_PREFIX_REMOVED As String = "remove"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"

        Private _newPeriodIDCounter As Integer
        Dim _stdBut As StdButtonsBase
        Dim _reference As String = ""
        Dim _name As String = ""
        Dim _clientID As Integer = 0
        Private _startup2JS As StringBuilder = New StringBuilder()

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_STARTUP As String = "Startup"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUserBudgetPeriods"), "Service User Budget Periods")

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("clientid")) > 0 Then
                _clientID = Utils.ToInt32(Request.QueryString("clientid"))
            End If

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowBack = True
                .AllowNew = False
                .AllowDelete = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUserBudgetPeriods.Edit"))
                .AllowFind = False
                .EditableControls.Add(fsControls.Controls)
                .AuditLogTableNames.Add("ClientBudgetPeriod")
                .AuditLogTableNames.Add("ClientBudgetHolderPeriod")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ServiceUserBudgetPeriods")
            End With

            btnAddPeriod.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUserBudgetPeriods.AddNew"))

            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked

            ' output javascript
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Date.js"))
            Me.JsLinks.Add("ServiceUserBudgetPeriods.js")

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Sds))

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                 String.Format("SUBP_DefaultBudgetPeriod='{0}';SUBP_ClientID={1};", _
                               Me.Settings(ApplicationName.AbacusIntranet, SETTING_DEFAULT_BUDGET_PERIOD), _
                               _clientID), True)
            End If

            ' re-create from view state
            _reference = ViewState("reference")
            txtReference.Text = _reference
            _name = ViewState("name")
            txtName.Text = _name

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
            If _clientID = 0 Then
                txtName.Text = String.Empty
                txtReference.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim cd As ClientDetail = Nothing
            Dim budgetPeriods As ClientBudgetPeriodCollection = Nothing
            Dim list As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = ServiceUserBudgetPeriodBL.GetServiceUser(Me.DbConnection, cd, _clientID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'adding values to viewstate because label text looses value on postback
            txtReference.Text = cd.Reference
            _reference = txtReference.Text
            ViewState("reference") = _reference
            txtName.Text = cd.Name
            _name = txtName.Text
            ViewState("name") = _name

            ' get the budgetPeriods
            msg = ServiceUserBudgetPeriodBL.GetBudgetPeriods(Me.DbConnection, budgetPeriods, _clientID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' refresh the list of existing budgetPeriods and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()
            For Each period As ClientBudgetPeriod In budgetPeriods
                Dim id As String = GetUniqueID(period)
                OutputPeriodControls(id, period)
                list.Add(id)
            Next
            PersistUniqueIDsToViewState(list)

            With _stdBut
                .ReportButtonParameters.Add("intClientID", _clientID)
            End With

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim cd As ClientDetail = Nothing
            Dim budgetPeriods As ClientBudgetPeriodCollection = New ClientBudgetPeriodCollection
            Dim periodsToDelete As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim periodList As List(Of String)
            Dim period As ClientBudgetPeriod

            If Me.IsValid Then

                msg = ServiceUserBudgetPeriodBL.GetServiceUser(conn:=Me.DbConnection, cDetail:=cd, itemID:=_clientID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' get list of budgetPeriods
                periodsToDelete = New List(Of String)
                periodList = GetUniqueIDsFromViewState()
                For Each uniqueID As String In periodList
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                        ' we are deleting
                        periodsToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                    Else
                        ' create the empty period
                        period = New ClientBudgetPeriod(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
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
                        End With
                        ' add to the collection
                        budgetPeriods.Add(period)
                    End If
                Next

                msg = ServiceUserBudgetPeriodBL.SaveServiceUserBudgetPeriod(conn:=Me.DbConnection, cd:=cd, periods:=budgetPeriods, _
                                                                        periodsToDelete:=periodsToDelete, auditUserName:=currentUser.ExternalUsername, _
                                                                        auditLogTitle:=AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If Not msg.Success Then
                    ' could not save 
                    lblError.Text = msg.Message
                    e.Cancel = True
                End If
                _clientID = cd.ID
            Else
                e.Cancel = True
            End If

            If msg.Success Then FindClicked(e)
        End Sub

#End Region

#Region " btnAddPeriod_Click "

        Private Sub btnAddPeriod_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddPeriod.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim newPeriod As ClientBudgetPeriod = New ClientBudgetPeriod(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

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

        Private Sub OutputPeriodControls(ByVal uniqueID As String, ByVal period As ClientBudgetPeriod)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim dateFrom As TextBoxEx
            Dim dateTo As TextBoxEx
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
                    If Not period Is Nothing AndAlso Utils.IsDate(period.DateTo) AndAlso period.DateTo <> Utils.MAX_END_DATE Then .Text = period.DateTo
                End With
                cell.Controls.Add(dateTo)

                _startup2JS.AppendFormat("function {0}_Changed(){{SUBP_CanChange('{1}', '{2}', '{3}');}}", _
                                         dateFrom.ID, dateFrom.ClientID, dateFrom.Text, dateTo.ClientID)

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

                removeButton.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUserBudgetPeriods.Delete"))

            End If

        End Sub

#End Region

#Region " GetUniqueID "

        Private Function GetUniqueID(ByVal period As ClientBudgetPeriod) As String

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

#Region " ServiceUserBudgetPeriods_PreRenderComplete "

        Private Sub ServiceUserBudgetPeriods_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim js As StringBuilder = New StringBuilder()
            js.Append(_startup2JS.ToString())

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)

        End Sub

#End Region

    End Class

End Namespace