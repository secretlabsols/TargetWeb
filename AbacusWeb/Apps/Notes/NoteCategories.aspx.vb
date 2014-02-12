Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.Notes


Namespace Apps.Notes

    ''' <summary>
    ''' Admin page used to maintain note categories.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     01/04/2011  D11971 SDS Generic Notes
    ''' </history>
    Partial Public Class NoteCategories
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _errorOccured As Boolean
        Private _inUseByDSO As Boolean
        Private _nCat As NoteCategory = Nothing
        Private _existingCategory As NoteCategory = Nothing

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(IIf(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUsers.ReferenceData.NoteCategories") > 0, Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUsers.ReferenceData.NoteCategories"), _
                Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.ReferenceData.NoteCategories")), "Note Categories")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = IIf(Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payments.ReferenceData.NoteCategories.AddNew")), True, _
                Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUsers.ReferenceData.NoteCategories.AddNew")))
                .AllowEdit = IIf(Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payments.ReferenceData.NoteCategories.Edit")), True, _
                Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUsers.ReferenceData.NoteCategories.Edit")))
                .AllowDelete = IIf(Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payments.ReferenceData.NoteCategories.Delete")), True, _
                Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUsers.ReferenceData.NoteCategories.Delete")))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.NoteCategory
                .AuditLogTableNames.Add("NoteCategory")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.NoteCategories")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing

            msg = NoteBL.FetchNoteCategory(Me.DbConnection, _nCat, e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With _nCat
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
                chkDefault.CheckBox.Checked = .IsDefault
            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                chkRedundant.CheckBox.Checked = False
                chkDefault.CheckBox.Checked = False
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            msg = NoteBL.DeleteNoteCategory(Me.DbConnection, e.ItemID)

            If Not msg.Success Then
                e.Cancel = True
                lblError.Text = msg.Message
                FindClicked(e)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            _nCat = New NoteCategory(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If e.ItemID > 0 Then
                _existingCategory = New NoteCategory(currentUser.ExternalUsername, _
                                                     AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                msg = NoteBL.FetchNoteCategory(Me.DbConnection, _existingCategory, e.ItemID)
            End If

            If Me.IsValid Then
                _nCat.Description = txtDescription.Text
                _nCat.Redundant = chkRedundant.CheckBox.Checked
                If e.ItemID > 0 Then
                    'existing note category
                    _nCat.IsDefault = IIf(Not _existingCategory Is Nothing And _existingCategory.IsDefault, _
                      _existingCategory.IsDefault, chkDefault.CheckBox.Checked)
                Else
                    'new note category
                    _nCat.IsDefault = chkDefault.CheckBox.Checked
                End If
                _nCat.ID = e.ItemID
                msg = NoteBL.SaveNoteCategory(Me.DbConnection, _nCat)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    _errorOccured = True
                    Exit Sub
                End If
                e.ItemID = _nCat.ID
                FindClicked(e)
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim msg As ErrorMessage = Nothing

            'set the default checkbox enabled or not
            If Not _nCat Is Nothing AndAlso _nCat.IsDefault Then
                chkDefault.CheckBox.Checked = _nCat.IsDefault
                chkDefault.CheckBox.Enabled = Not _nCat.IsDefault
            End If

            'ensures the default checkbox is ticked for new entries
            If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                chkDefault.CheckBox.Checked = True
            End If

            If Not _nCat Is Nothing AndAlso _nCat.SystemType <> NoteCategorySystemTypes.UserDefined Then
                'set the warning for system type note categories
                msg = Utils.CatchError(Nothing, "E3088", "NoteBL.DeleteNoteCategory()")
                lblError.Text = msg.Message
                lblError.ForeColor = Color.Orange
            End If
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim isInUse As Boolean
            Dim msg As ErrorMessage = Nothing

            If Not _nCat Is Nothing Then
                'check if notecategory is system type to determine display of Edit button
                _stdBut.AllowEdit = IIf(_nCat.SystemType = NoteCategorySystemTypes.UserDefined, True, False)
                msg = NoteBL.IsNoteCategoryInUse(Me.DbConnection, _nCat.ID, isInUse)

                'check if notecategory is system type to determine display of Delete button
                _stdBut.AllowDelete = IIf(_nCat.SystemType = NoteCategorySystemTypes.UserDefined And Not isInUse, True, False)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If
        End Sub
    End Class

End Namespace