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
    ''' Screen used to manage rate preclusions for chosen rate categories.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Mo Tahir 19/05/2010  D11806 Rate Category Ordering
    ''' Paul W   04/05/2010  A4WA#6258 disable edit if framework in use
    ''' Mo Tahir 05/10/2009  A5821
    ''' Mo Tahir 22/09/2009  A5797
    ''' Mo Tahir 27/08/2009  D11673.
    ''' </history>
    Partial Public Class RatePreclusions
        Inherits BasePage

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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim js As StringBuilder = New StringBuilder()
            Dim msg As ErrorMessage

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.RateCategories"), "Rate Category Preclusions")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            _frameworkID = Utils.ToInt32(Request.QueryString("frameworkID"))

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

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' re-create the list of Services (from view state)
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            For Each id As String In list
                OutputCategoryControls(id, Nothing, js, True)
            Next

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)

            If Me.IsPostBack Then
                '_frameworkID = Utils.ToInt32(Request.QueryString("frameworkID"))
                _rateCategoryID = _stdBut.SelectedItemID
                PopulateRateFramework(_frameworkID, _rateCategoryID)
            End If

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim category As DomRateCategory
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim rcList As vwDomRateCategoryPreclusionsCollection = New vwDomRateCategoryPreclusionsCollection
            Dim rpList As vwDomRateCategoryPreclusionsCollection = New vwDomRateCategoryPreclusionsCollection
            Dim list As List(Of String)
            Dim js As StringBuilder

            category = New DomRateCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            With category
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _rateCategoryID = .ID
                _frameworkID = .DomRateFrameworkID
            End With

            PopulateRateFramework(_frameworkID, _rateCategoryID)

            'get list of categories explicitly precluded
            msg = vwDomRateCategoryPreclusions.FetchList(Me.DbConnection, rcList, , _rateCategoryID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'get list of categories implicitly precluded
            msg = vwDomRateCategoryPreclusions.FetchList(Me.DbConnection, rpList, , , _rateCategoryID)
            If Not msg.Success Then WebUtils.DisplayError(msg)


            'refresh the list of existing bands and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()

            js = New StringBuilder()
            With js
                .Append("RateCategory_preclusions=new Collection();")
            End With

            For Each ratecategory As vwDomRateCategoryPreclusions In rcList
                Dim id As String = GetUniqueID(ratecategory)
                OutputCategoryControls(id, ratecategory, js, True)
                list.Add(id)
            Next

            For Each ratecategory As vwDomRateCategoryPreclusions In rpList
                Dim id As String = GetUniqueID(ratecategory)
                OutputCategoryControls(id, ratecategory, js, False)
                list.Add(id)
            Next

            PersistUniqueIDsToViewState(list)
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                ClearViewState(e)
                txtDescription.Text = String.Empty
                txtRateFramework.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub


        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim RateCategoriesToDelete As List(Of Integer)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim RateCategoriesList As List(Of String)
            Dim RateCategoriesToSave As ArrayList = New ArrayList
            Dim RateCategoriesToUpdate As ArrayList = New ArrayList
            Dim prevRCID As Integer
            Dim errorOccurred, transRolledBack As Boolean
            Dim uRatePreclusion As DomRateCategoryPreclusions

            If Me.IsValid Then
                Try

                    trans = SqlHelper.GetTransaction(Me.DbConnection)
                    ' get list of rate categories
                    RateCategoriesToDelete = New List(Of Integer)
                    RateCategoriesList = GetUniqueIDsFromViewState()
                    For Each uniqueID As String In RateCategoriesList
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                            ' we are deleting
                            RateCategoriesToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                        ElseIf uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                            'updating
                            RateCategoriesToUpdate.Add(Utils.ToInt32(CType(phRateCategories.FindControl(CTRL_PREFIX_SERVICE & uniqueID), DropDownListEx).DropDownList.SelectedValue))
                            uRatePreclusion = New DomRateCategoryPreclusions(trans)
                            msg = uRatePreclusion.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                            If Not msg.Success Then
                                trans.Rollback()
                                transRolledBack = True
                                WebUtils.DisplayError(msg)
                            Else
                                With uRatePreclusion
                                    If .DomRateCategoryID <> Utils.ToInt32(CType(phRateCategories.FindControl(CTRL_PREFIX_SERVICE & uniqueID), DropDownListEx).DropDownList.SelectedValue) Then
                                        .PrecludedDomRateCategoryID = Utils.ToInt32(CType(phRateCategories.FindControl(CTRL_PREFIX_SERVICE & uniqueID), DropDownListEx).DropDownList.SelectedValue)
                                        msg = .Save()
                                        If Not msg.Success Then
                                            trans.Rollback()
                                            transRolledBack = True
                                            WebUtils.DisplayError(msg)
                                        End If
                                    End If
                                End With
                            End If
                        ElseIf uniqueID.StartsWith(UNIQUEID_PREFIX_NEW) Then
                            ' add to the collection
                            Dim rateCategoryID As Integer = Utils.ToInt32(CType(phRateCategories.FindControl(CTRL_PREFIX_SERVICE & uniqueID), DropDownListEx).DropDownList.SelectedValue)
                            RateCategoriesToSave.Add(rateCategoryID)
                        End If
                    Next

                    RateCategoriesToSave.Sort()

                    For Each rateCategoryID As Integer In RateCategoriesToSave
                        If prevRCID = rateCategoryID Then
                            lblError.Text = "Cannot have duplicate rate category preclusions entered."
                            errorOccurred = True
                            trans.Rollback()
                            transRolledBack = True
                            Exit For
                        End If
                        prevRCID = rateCategoryID
                    Next

                    For Each rateCategoryID As Integer In RateCategoriesToSave
                        prevRCID = rateCategoryID
                        For Each rateCategoryIDU As Integer In RateCategoriesToUpdate
                            If prevRCID = rateCategoryIDU Then
                                lblError.Text = "Cannot have duplicate rate category preclusions entered."
                                errorOccurred = True
                                trans.Rollback()
                                transRolledBack = True
                                Exit For
                            End If
                        Next
                    Next

                    If Not errorOccurred Then
                        For Each rcID As Integer In RateCategoriesToSave
                            Dim rCategoryPreclusion As DomRateCategoryPreclusions = New DomRateCategoryPreclusions(trans)
                            rCategoryPreclusion.DomRateFrameworkID = Utils.ToInt32(Request.QueryString("frameworkID"))
                            rCategoryPreclusion.DomRateCategoryID = e.ItemID
                            rCategoryPreclusion.PrecludedDomRateCategoryID = rcID
                            msg = rCategoryPreclusion.Save
                            If Not msg.Success Then
                                trans.Rollback()
                                transRolledBack = True
                                WebUtils.DisplayError(msg)
                            End If
                        Next
                        For Each id As String In RateCategoriesToDelete
                            msg = DomRateCategoryPreclusions.Delete(trans, id)
                            If Not msg.Success Then
                                trans.Rollback()
                                transRolledBack = True
                                WebUtils.DisplayError(msg)
                            End If
                        Next
                    End If

                    If Not transRolledBack Then
                        trans.Commit()
                    Else
                        e.Cancel = True
                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0001 RatePreclusions Save")     ' unexpected
                    e.Cancel = True
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try
            Else
                e.Cancel = True
            End If

        End Sub


        Private Sub PopulateRateFramework(ByVal frameworkID As Integer, ByVal rateCategoryId As Integer)

            Dim msg As ErrorMessage

            _frameworkID = frameworkID

            _framework = New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
            msg = _framework.Fetch(frameworkID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            txtRateFramework.Text = _framework.Description

            _category = New DomRateCategory(Me.DbConnection, String.Empty, String.Empty)
            With _category
                msg = .Fetch(rateCategoryId)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
            End With
        End Sub

#Region " Rate Category Preclusion Table Code "

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phRateCategories.Controls.Clear()
            _categories = Nothing
        End Sub

#End Region

#Region " btnAddRateCategory_Click "

        Private Sub btnAddRateCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddRateCategory.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim rateCategory As vwDomRateCategoryPreclusions = New vwDomRateCategoryPreclusions(Me.DbConnection)
            Dim js As StringBuilder
            Dim serviceList As List(Of String) = New List(Of String)

            js = New StringBuilder()
            With js
                .Append("RateCategory_preclusions=new Collection();")
            End With

            serviceList = GetUniqueIDsFromViewState()
            For Each uniqueID As String In serviceList
                If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                    ' add to the collection
                    Dim rateCategoryID As Integer = Utils.ToInt32(CType(phRateCategories.FindControl(CTRL_PREFIX_SERVICE & uniqueID), DropDownListEx).DropDownList.SelectedValue)

                    With js
                        .AppendFormat("RateCategory_preclusions.add({0},{1});", _
                                              String.Format("'{0}_cboDropDownList'", CType(phRateCategories.FindControl(CTRL_PREFIX_SERVICE & uniqueID), DropDownListEx).ClientID), _
                                              rateCategoryID)
                    End With

                End If
            Next

            ' add a new row to the rate category list
            id = GetUniqueID(rateCategory)
            ' create the controls
            OutputCategoryControls(id, rateCategory, js, True)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState(list)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)

        End Sub

#End Region

#Region "OutputCategoryControls "

        Private Sub OutputCategoryControls(ByVal uniqueID As String, ByVal rateCategory As vwDomRateCategoryPreclusions, ByRef js As StringBuilder, ByVal explicit As Boolean)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim removeButton As ImageButton
            Dim cboRateCategory As DropDownListEx

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New HtmlTableRow()
                row.ID = ROW_PREFIX & uniqueID
                phRateCategories.Controls.Add(row)

                ' Preclusions
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboRateCategory = New DropDownListEx()
                With cboRateCategory
                    .ID = CTRL_PREFIX_SERVICE & uniqueID
                    .ValidationGroup = "Save"
                    .Required = True
                    .RequiredValidatorErrMsg = "Please Enter a Rate Category."
                    LoadCategoriesDropdown(cboRateCategory, _stdBut.SelectedItemID)
                    If explicit Then
                        If Not rateCategory Is Nothing AndAlso rateCategory.PrecludedDomRateCategoryId > 0 Then .DropDownList.SelectedValue = rateCategory.PrecludedDomRateCategoryId
                    Else
                        If Not rateCategory Is Nothing AndAlso rateCategory.DomRateCategoryID > 0 Then .DropDownList.SelectedValue = rateCategory.DomRateCategoryID
                    End If
                End With
                cell.Controls.Add(cboRateCategory)

                If Not rateCategory Is Nothing AndAlso rateCategory.ID <> 0 Then
                    With js
                        .AppendFormat("RateCategory_preclusions.add({0},{1});", _
                                              String.Format("'{0}_cboDropDownList'", cboRateCategory.ClientID), _
                                              rateCategory.ID)
                    End With
                End If


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

#Region " GetUniqueID "

        Private Function GetUniqueID(ByVal rateCategory As vwDomRateCategoryPreclusions) As String

            Dim id As String

            If rateCategory.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW & _newCodeIDCounter
                _newCodeIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & rateCategory.ID
            End If

            Return id

        End Function

#End Region

#Region " PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER, _newCodeIDCounter)
        End Sub

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

#Region "LoadCategoriesDropdown "

        Private Sub LoadCategoriesDropdown(ByVal dropdown As DropDownListEx, ByVal rateCategory As Integer)

            Dim msg As ErrorMessage
            Dim rCategory As DomRateCategory = Nothing

            _frameworkID = Utils.ToInt32(Request.QueryString("frameworkID"))

            msg = DomRateCategory.FetchList(Me.DbConnection, _categories, String.Empty, String.Empty, , , _frameworkID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each aRateCategory As DomRateCategory In _categories
                If aRateCategory.ID = rateCategory Then
                    rCategory = aRateCategory
                End If
            Next

            If Not rCategory Is Nothing Then
                _categories.Remove(rCategory)
            End If

            _categories.Sort(New CollectionSorter("Description", SortDirection.Ascending))
            _categories.Sort(New CollectionSorter("SortOrder", SortDirection.Ascending))

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
