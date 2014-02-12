Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Abacus.Library.Notes
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library

Namespace Apps.UserControls

    ''' <summary>
    ''' Class representing a selector tool for notes
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   MoTahir   23/05/2011  SDS Issue 624 - Clicking “List” on Service User “Notes” tab gives an error 
    '''   MoTahir   06/05/2011  SDS Issue 632 - Error on spendplans when users do not have permission to view notes.
    '''   MoTahir   21/04/2011  SDS Issue 562 - Error occurs when deleting spend plans
    '''   MoTahir   07/04/2011 D11971 - Created
    ''' </history>
    Partial Public Class NotesSelector
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' locals
        Private _FilterSelectedID As Integer = 0
        Private _noteType As NoteTypes = NoteTypes.ServiceUser
        Private _filterNoteTypeChildID As Integer
        Private _viewNoteInNewWindow As Boolean
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _ShowNewButton As Nullable(Of Boolean) = Nothing
        Private _ShowViewButton As Nullable(Of Boolean) = Nothing
        Private _ShowEditButton As Nullable(Of Boolean) = Nothing
        Private _ShowListButton As Nullable(Of Boolean) = Nothing
        Private _ShowNotes As Nullable(Of Boolean) = Nothing

        ' constants
        Private Const _JsLibrary As String = "Library/Javascript/"
        Private Const _SelectorName As String = "NotesSelector"
        Private Const _Report_Button_Suffix As String = "_btnReports"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the current page.
        ''' </summary>
        ''' <value>The current page.</value>
        Private ReadOnly Property CurrentPage() As Integer
            Get
                Dim page As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
                ' default always to page 1 if not larger
                If page <= 0 Then page = 1
                Return page
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets selected id.
        ''' </summary>
        ''' <value>The selected id.</value>
        Public Property FilterSelectedID() As Integer
            Get
                Return _FilterSelectedID
            End Get
            Set(ByVal value As Integer)
                _FilterSelectedID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets selected note type.
        ''' </summary>
        ''' <value>The selected note type.</value>
        Public Property FilterNoteType() As NoteTypes
            Get
                Return _noteType
            End Get
            Set(ByVal value As NoteTypes)
                _noteType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets selected filter note type child id.
        ''' </summary>
        ''' <value>The selected filter note type child id.</value>
        Public Property FilterNoteTypeChildID() As Integer
            Get
                Return _filterNoteTypeChildID
            End Get
            Set(ByVal value As Integer)
                _filterNoteTypeChildID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [view note in new window].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [view note in new window]; otherwise, <c>false</c>.
        ''' </value>
        Public Property ViewNoteInNewWindow() As Boolean
            Get
                Return _viewNoteInNewWindow
            End Get
            Set(ByVal value As Boolean)
                _viewNoteInNewWindow = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>The current user.</value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the new button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the new button]; otherwise, <c>false</c>.</value>
        Public Property ShowNewButton() As Boolean
            Get
                If _ShowNewButton Is Nothing OrElse _ShowNewButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowNewButton = SecurityBL.UserHasMenuItemCommand(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Note.AddNew"), 2)
                End If
                Return _ShowNewButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowNewButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the view button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the view button]; otherwise, <c>false</c>.</value>
        Public Property ShowViewButton() As Boolean
            Get
                If _ShowViewButton Is Nothing OrElse _ShowViewButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowViewButton = SecurityBL.UserHasMenuItemCommand(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Note.View"), 2)
                End If
                Return _ShowViewButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowViewButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the edit button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the edit button]; otherwise, <c>false</c>.</value>
        Public Property ShowEditButton() As Boolean
            Get
                If _ShowEditButton Is Nothing OrElse _ShowEditButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowEditButton = SecurityBL.UserHasMenuItemCommand(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Note.Edit"), 2)
                End If
                Return _ShowEditButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowEditButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the report button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the report button]; otherwise, <c>false</c>.</value>
        Public Property ShowListButton() As Boolean
            Get
                If _ShowListButton Is Nothing OrElse _ShowListButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowListButton = SecurityBL.UserHasMenuItemCommand(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Note.View"), 2)
                End If
                Return _ShowListButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowListButton = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the notes].
        ''' </summary>
        ''' <value><c>true</c> if [to show notes]; otherwise, <c>false</c>.</value>
        Public Property ShowNotes() As Boolean
            Get
                If _ShowNotes Is Nothing OrElse _ShowNotes.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowNotes = SecurityBL.UserHasMenuItemCommand(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Note.View"), 2)
                    If Not _ShowNotes Then lblError.Text = "You do not have permissions to view notes."
                End If
                Return _ShowNotes.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowNotes = value
            End Set
        End Property

        ''' <summary>
        ''' Gets base page.
        ''' </summary>
        ''' <value>Base page.</value>
        Private ReadOnly Property MyBasePage() As BasePage
            Get
                Return CType(Page, BasePage)
            End Get
        End Property
#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        ''' <param name="thePage">The page.</param>
        Public Sub InitControl(ByVal thePage As BasePage)
            If ShowNotes Then

                Dim js As New StringBuilder()

                ' add date utility JS
                thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}date.js", _JsLibrary)))

                ' add utility JS link
                thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}WebSvcUtils.js", _JsLibrary)))

                ' add dialog JS
                thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}Dialog.js", _JsLibrary)))

                ' add list filter JS
                thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}ListFilter.js", _JsLibrary)))

                ' add page JS
                thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/UserControls/{0}.js", _SelectorName)))

                ' add AJAX-generated javascript to the page
                AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Notes))

                ' add filter properties to page in js format
                js.AppendFormat("{1}_SelectedID = {0};", FilterSelectedID, _SelectorName)
                js.AppendFormat("{1}_CurrentPage = {0};", CurrentPage, _SelectorName)
                js.AppendFormat("{1}_NoteTypeID = {0};", Convert.ToInt16(FilterNoteType), _SelectorName)
                js.AppendFormat("{1}_NoteTypeChildID = {0};", FilterNoteTypeChildID, _SelectorName)
                js.AppendFormat("{1}_ViewNoteInNewWindow = {0};", ViewNoteInNewWindow.ToString().ToLower(), _SelectorName)
                js.AppendFormat("var {0}_btnList = GetElement('{1}', true);", _SelectorName, Notes_btnList.ClientID + _Report_Button_Suffix)
                js.AppendFormat("var {0}_btnEdit = GetElement('{1}', true);", _SelectorName, Notes_btnEdit.ClientID)
                js.AppendFormat("var {0}_btnView = GetElement('{1}', true);", _SelectorName, Notes_btnView.ClientID)
                js.AppendFormat("var {0}_btnNew = GetElement('{1}', true);", _SelectorName, Notes_btnNew.ClientID)
                js.AppendFormat("var {0}_btnListID = '{1}';", _SelectorName, Notes_btnList.ClientID)

                With CType(Notes_btnList, IReportsButton)
                    .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.Note")
                    .ShowButton = True
                    .ReportToExcel = True
                    .ReportToView = True
                    .Position = SearchableMenu.SearchableMenuPosition.TopRight
                End With

                thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                           String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup", _SelectorName), _
                                                           Target.Library.Web.Utils.WrapClientScript(js.ToString()))
            End If

        End Sub

#End Region
        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Notes_btnList.Visible = ShowListButton
            Notes_btnEdit.Visible = ShowEditButton
            Notes_btnView.Visible = ShowViewButton
            Notes_btnNew.Visible = ShowNewButton
            Notes_Content.Visible = ShowNotes
        End Sub
    End Class
End Namespace