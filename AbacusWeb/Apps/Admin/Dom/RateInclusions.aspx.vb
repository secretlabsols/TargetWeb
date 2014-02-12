Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports System.Reflection
Imports System.Text
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls


Namespace Apps.Admin.Dom
    ''' <summary>
    ''' Screen used to manage rate inclusions for chosen rate categories.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Colin Daly   24/01/2012  I806 - Removed word Domiciliary
    ''' Mo Tahir     12/05/2010  D11806
    ''' Paul Wheaver 04/05/2010  A4WA#6258 disable edit if framework in use
    ''' Paul Wheaver 04/03/2010  D11788.
    ''' </history>
    Partial Public Class RateInclusions
        Inherits BasePage

#Region " Variables & Constants "

        Private _frameworkID As Integer
        Private _framework As DomRateFramework
        Private _category As DomRateCategory = Nothing
        Private _rateCategoryID As Integer
        Private _categories As DomRateCategoryCollection = Nothing
        Private _canEdit As Boolean

        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"
        Const CTRL_PREFIX_REMOVED As String = "remove"
        Const ROW_PREFIX As String = "row"
        Const CTRL_PREFIX_SERVICE As String = "service"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"

        Private _newCodeIDCounter As Integer
        Private _stdBut As StdButtonsBase

#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'Dim js As StringBuilder = New StringBuilder()
            Dim msg As ErrorMessage
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.RateCategories"), "Rate Category Inclusions")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            _frameworkID = Utils.ToInt32(Request.QueryString("frameworkID"))
            _rateCategoryID = Utils.ToInt32(Request.QueryString("ID"))
            populateHeaderDetails()

            msg = DomContractBL.CanEditRateFramework(Me.DbConnection, _frameworkID, _canEdit)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With _stdBut
                .AllowBack = True
                .AllowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RateCategories.Edit"))
                If Not _canEdit Then
                    .AllowEdit = False
                End If
                .AllowDelete = False
                .AllowFind = False
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked

            ' re-create the list of Services (from view state)
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            For Each id As String In list
                OutputCategoryControls(id, Nothing)
            Next

        End Sub

#End Region

#Region " populateHeaderDetails "

        Private Sub populateHeaderDetails()
            Dim msg As ErrorMessage = New ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            _category = New DomRateCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            With _category
                Msg = .Fetch(_rateCategoryID)
                If Not Msg.Success Then WebUtils.DisplayError(Msg)
                txtDescription.Text = .Description
            End With

            _framework = New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
            Msg = _framework.Fetch(_frameworkID)
            If Not Msg.Success Then WebUtils.DisplayError(Msg)
            txtRateFramework.Text = _framework.Description

        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim rCats As vwDomRateCategoryInclusionsCollection = Nothing
            Dim list As List(Of String)

            msg = vwDomRateCategoryInclusions.FetchList(conn:=Me.DbConnection, list:=rCats, domRateFrameworkID:=_frameworkID, domRateCategoryID:=_rateCategoryID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'refresh the list of existing bands and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()

            For Each inclusion As vwDomRateCategoryInclusions In rCats
                Dim id As String = GetUniqueID(inclusion)
                OutputCategoryControls(id, inclusion)
                list.Add(id)
            Next

            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                ClearViewState(e)
                txtDescription.Text = String.Empty
                txtRateFramework.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim msg As ErrorMessage = New ErrorMessage
            Dim RateCategoriesToSave As ArrayList = New ArrayList
            Dim trans As SqlTransaction = Nothing

            'Build up a collection of rate cats that have been selected in the list.
            For Each id As String In list
                If Not id.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                    RateCategoriesToSave.Add(Utils.ToInt32(CType(phRateCategories.FindControl(CTRL_PREFIX_SERVICE & id),  _
                                                                        DropDownListEx).DropDownList.SelectedValue))
                End If
            Next

            'chech for duplicate entries
            For Each compareID1 As Integer In RateCategoriesToSave
                Dim countIDFound As Integer = 0
                For Each compareID2 As Integer In RateCategoriesToSave
                    If compareID1 = compareID2 Then
                        countIDFound += 1
                    End If
                Next
                'The Item was listed more than once
                If countIDFound > 1 Then
                    lblError.Text = "Cannot have duplicate rate category inclusions entered."
                    e.Cancel = True
                    Exit Sub
                End If
            Next

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                'save the inclusions
                For Each id As String In list
                    Dim inclusion As DomRateCategoryInclusions = New DomRateCategoryInclusions(trans)

                    If id.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                        msg = DomRateCategoryInclusions.Delete(trans, id.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                        If Not msg.Success Then
                            trans.Rollback()
                            WebUtils.DisplayError(msg)
                        End If

                    ElseIf id.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                        msg = inclusion.Fetch(id.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty))
                        inclusion.IncludedDomRateCategoryID = Utils.ToInt32(CType(phRateCategories.FindControl(CTRL_PREFIX_SERVICE & id),  _
                                                                            DropDownListEx).DropDownList.SelectedValue)

                        msg = inclusion.Save()
                        If Not msg.Success Then
                            trans.Rollback()
                            WebUtils.DisplayError(msg)
                        End If

                    ElseIf id.StartsWith(UNIQUEID_PREFIX_NEW) Then
                        inclusion.DomRateCategoryID = _rateCategoryID
                        inclusion.DomRateFrameworkID = _frameworkID
                        inclusion.IncludedDomRateCategoryID = Utils.ToInt32(CType(phRateCategories.FindControl(CTRL_PREFIX_SERVICE & id),  _
                                                                            DropDownListEx).DropDownList.SelectedValue)

                        msg = inclusion.Save()
                        If Not msg.Success Then
                            trans.Rollback()
                            WebUtils.DisplayError(msg)
                        End If
                    End If

                Next

                trans.Commit()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0001 RateInclusions Save")     ' unexpected
                e.Cancel = True
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

#Region " Rate Category Preclusion Table Code "

#Region "       ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phRateCategories.Controls.Clear()
            _categories = Nothing
        End Sub

#End Region

#Region "       btnAddRateCategory_Click "

        Private Sub btnAddRateCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddRateCategory.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim inclusion As vwDomRateCategoryInclusions = New vwDomRateCategoryInclusions(Me.DbConnection)

            ' add a new row to the rate category list
            id = GetUniqueID(inclusion)
            ' create the controls
            OutputCategoryControls(id, inclusion)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region "       OutputCategoryControls "

        Private Sub OutputCategoryControls(ByVal uniqueID As String, ByVal inclusion As vwDomRateCategoryInclusions)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim removeButton As ImageButton
            Dim cboRateCategory As DropDownListEx

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New HtmlTableRow()
                row.ID = ROW_PREFIX & uniqueID
                phRateCategories.Controls.Add(row)

                ' Inclusions
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboRateCategory = New DropDownListEx()
                With cboRateCategory
                    .ID = CTRL_PREFIX_SERVICE & uniqueID
                    .ValidationGroup = "Save"
                    .Required = True
                    .RequiredValidatorErrMsg = "Please select a rate category."
                    LoadCategoriesDropdown(cboRateCategory, _rateCategoryID)
                    If Not inclusion Is Nothing Then .DropDownList.SelectedValue = inclusion.IncludedDomRateCategoryID
                End With
                cell.Controls.Add(cboRateCategory)

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

#Region "       Remove_Click "

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
            For index As Integer = 0 To phRateCategories.Controls.Count - 1
                If phRateCategories.Controls(index).ID.Replace(ROW_PREFIX, String.Empty) = id Then
                    phRateCategories.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region "       GetUniqueID "

        Private Function GetUniqueID(ByVal inclusion As vwDomRateCategoryInclusions) As String

            Dim id As String

            If inclusion.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW & _newCodeIDCounter
                _newCodeIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & inclusion.ID
            End If

            Return id

        End Function

#End Region

#Region "       PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER, _newCodeIDCounter)
        End Sub

#End Region

#Region "       GetUniqueIDsFromViewState "

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

#Region "       LoadCategoriesDropdown "

        Private Sub LoadCategoriesDropdown(ByVal dropdown As DropDownListEx, ByVal rateCategoryID As Integer)

            Dim msg As ErrorMessage
            Dim rCategory As DomRateCategory = Nothing

            msg = DomRateCategory.FetchList(Me.DbConnection, _categories, String.Empty, String.Empty, , , _frameworkID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each aRateCategory As DomRateCategory In _categories
                If aRateCategory.ID = rateCategoryID Then
                    rCategory = aRateCategory
                End If
            Next

            If Not rCategory Is Nothing Then
                _categories.Remove(rCategory)
            End If

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = _categories
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#End Region

    End Class
End Namespace
