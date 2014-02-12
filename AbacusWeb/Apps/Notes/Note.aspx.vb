Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.Notes
Imports System.Collections.Generic
Imports System.Text


Namespace Apps.Notes

    ''' <summary>
    ''' Admin page used to maintain notes.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     28/10/2011  A4WA6963 - In View mode the user is unable to view any further 
    '''                 than the 5th line on the View Notes screen unless clicking Edit (below).
    '''     MikeVO      04/05/2011  SDS issue #627 - corrected validation issue(s).
    '''     MoTahir     11/04/2011  D11971 SDS Generic Notes
    ''' </history>
    Partial Public Class Note
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _errorOccured As Boolean
        Private _inUseByDSO As Boolean
        Private _note As Target.Abacus.Library.DataClasses.Note = Nothing
        Private _itemID As Integer
        Private _pageMode As StdButtonsMode = Nothing
        Private _noteType As NoteTypes = Nothing
        Private _noteTypeChildID As Integer = 0

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'disable the timeout
            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Note"), "Note")

            ' add the payment tolerance group enum to use in javascript
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))
            Me.JsLinks.Add("Note.js")
            Me.UseJQuery = True

            'set the item id
            _itemID = Target.Library.Utils.ToInt32(Request.QueryString("ID"))

            'get the notetype
            _noteType = Target.Library.Utils.ToInt32(Request.QueryString("notetype"))

            'get the note type child id
            _noteTypeChildID = Target.Library.Utils.ToInt32(Request.QueryString("notetypechildid"))

            'get the mode the page is openened in
            _pageMode = Target.Library.Utils.ToInt32(Request.QueryString("mode"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            'if existing note set the item id
            If _itemID > 0 Then
                _stdBut.ID = _itemID
            End If

            With _stdBut
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Note.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Note.Delete"))
                .EditableControls.Add(fsControls.Controls)
                .AuditLogTableNames.Add("Note")
                .AllowFind = False
                .AllowNew = IIf(_pageMode = StdButtonsMode.AddNew And _
                                Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Note.AddNew")), _
                                True, False)
                .ShowNew = False
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

            'populate the note categories drop down
            PopulateNoteCategoriesDropDown()

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage = Nothing
            If e.ItemID > 0 Then
                _note = New Target.Abacus.Library.DataClasses.Note(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                'get id of note from querystring

                msg = NoteBL.FetchNote(Me.DbConnection, _note, e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With _note
                    ddNoteCategories.DropDownList.SelectedValue = _note.NoteCategoryID
                    txtNote.Text = _note.Notes
                End With
            End If
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtNote.Text = String.Empty
                ClientScript.RegisterStartupScript(Me.GetType(), "myScript", "<script language=JavaScript>self.close();</script>")
            Else
                FindClicked(e)
                ClientScript.RegisterStartupScript(Me.GetType(), "myScript", "<script language=JavaScript>self.close();</script>")
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            msg = NoteBL.DeleteNote(Me.DbConnection, e.ItemID)

            If Not msg.Success Then
                e.Cancel = True
                lblError.Text = msg.Message
                FindClicked(e)
            End If
            'close window from javascript
            ClientScript.RegisterStartupScript(Me.GetType(), "myScript", "<script language=JavaScript>self.close();</script>")
        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            _note = New Target.Abacus.Library.DataClasses.Note(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            ddNoteCategories.SelectPostBackValue()

            If e.ItemID > 0 Then
                _note = New Target.Abacus.Library.DataClasses.Note(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                msg = NoteBL.FetchNote(Me.DbConnection, _note, e.ItemID)
            End If

            If Me.IsValid Then
                _note.Notes = txtNote.Text.Trim
                _note.NoteCategoryID = Utils.ToInt32(ddNoteCategories.DropDownList.SelectedValue)
                _note.NoteTypeID = _noteType
                _note.NoteTypeChildID = _noteTypeChildID
                msg = NoteBL.SaveNote(Me.DbConnection, _note)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    _errorOccured = True
                    Exit Sub
                End If
                e.ItemID = _note.ID
                FindClicked(e)
            Else
                e.Cancel = True
            End If
            'close window from javascript
            ClientScript.RegisterStartupScript(Me.GetType(), "myScript", "<script language=JavaScript>self.close();</script>")
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            txtNote.TextBox.TextMode = TextBoxMode.MultiLine
            txtNote.TextBox.Rows = 10

            Dim js As New StringBuilder()

            js.AppendFormat("selectedID={0};", _stdBut.SelectedItemID)
            js.AppendFormat("mode={0};", Convert.ToInt32(CType(stdButtons1, StdButtonsBase).ButtonsMode))

            Page.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.Notes.Note.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js.ToString))

            Select Case _pageMode
                Case StdButtonsMode.AddNew
                    noteHeader.InnerText = "Create New Note"
                Case StdButtonsMode.Fetched
                    noteHeader.InnerText = "View Note"
                Case StdButtonsMode.Edit
                    noteHeader.InnerText = "Edit Note"
            End Select

        End Sub

        Private Sub PopulateNoteCategoriesDropDown()

            Dim notCategories As NoteCategoryCollection = Nothing
            Dim msg As ErrorMessage = Nothing
            Dim defaultNoteCategoryId As List(Of Integer) = Nothing
            Dim firstItem As ListItem = Nothing

            'fetch all the note categories
            msg = NoteBL.FetchNoteCategories(Me.DbConnection, notCategories)
            If Not msg.Success Then WebUtils.DisplayError(msg)


            ' get the default id
            defaultNoteCategoryId = (From c In notCategories.ToArray _
                                    Where c.IsDefault = TriState.True _
                                    Select c.ID).ToList

            'create the first item in the list
            firstItem = New ListItem("Select a Category", String.Empty)
            'clear down the drop down list
            ddNoteCategories.DropDownList.Items.Clear()
            'add the first item
            ddNoteCategories.DropDownList.Items.Add(firstItem)
            'we want to databind and persist the firstitem already added.
            ddNoteCategories.DropDownList.AppendDataBoundItems = True
            ddNoteCategories.DropDownList.DataSource = notCategories
            ddNoteCategories.DropDownList.DataTextField = "Description"
            ddNoteCategories.DropDownList.DataValueField = "ID"
            'check if there is a default if not default to firstItem
            ddNoteCategories.DropDownList.SelectedValue = IIf(defaultNoteCategoryId.Count = 1, defaultNoteCategoryId(0), 0)
            ddNoteCategories.DropDownList.DataBind()

        End Sub
    End Class

End Namespace