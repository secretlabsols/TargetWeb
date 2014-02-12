
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Target.Library
Imports Target.Library.Web.Controls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls

Namespace Library.UserControls

    ''' <summary>
    ''' Implements a set of standard new/edit/save/cancel/delete buttons.
    ''' Raises events as each of the buttons are clicked.
    ''' Also includes "finder" controls.       
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     JohnF       20/11/2013  Pass audit table names directly into JS (#8326)
    '''     JohnF       19/09/2013  Added overridable OnEditClick functionality (#8171)
    '''     JohnF       21/05/2013  Added CustomControls property/method (D12479)
    '''     MikeVO      26/04/2011  SDS issue #562 - clear SelectedItemID after deletion.
    '''     MikeVO      26/04/2011  SDS issue #607 - corrected display of back button when backUrl QS param is present but empty.
    '''     ColinD      28/04/2010  A4WA#5603 - removed funtionality for implementation at a later date
    '''     ColinD      28/04/2010  A4WA#5603 - added functionality to disable save button when clicked
    '''     MikeVO      16/06/2009  A4WA#5518 - moved report control setup from Load to PreRender.
    '''     MikeVO      12/05/2009  D11549 - added reporting support.
    '''     MikeVO      02/02/2009  D11492 - added AddCustomControls event.
    '''                                    - support for ShowSave property.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Public Class StdButtons
        Inherits StdButtonsBase

#Region " Private variables & properties "

		Private _onNewClientClick As String
		Private _onSaveClientClick As String
		Private _onCancelClientClick As String
		Private _onDeleteClientClick As String
        Private _onFindClientClick As String
        Private _onBackClientClick As String
        Private _onEditClientClick As String

		Public Overrides Property SelectedItemID() As Integer
			Get
                Return Utils.ToInt32(hidSelectedItemID.Value)
			End Get
			Set(ByVal value As Integer)
				hidSelectedItemID.Value = value
			End Set
		End Property

		Public Overrides Property OnNewClientClick() As String
			Get
				Return _onNewClientClick
			End Get
			Set(ByVal value As String)
				_onNewClientClick = value
			End Set
		End Property

		Public Overrides Property OnSaveClientClick() As String
			Get
				Return _onSaveClientClick
			End Get
			Set(ByVal value As String)
				_onSaveClientClick = value
			End Set
		End Property

		Public Overrides Property OnCancelClientClick() As String
			Get
				Return _onCancelClientClick
			End Get
			Set(ByVal value As String)
				_onCancelClientClick = value
			End Set
		End Property

		Public Overrides Property OnDeleteClientClick() As String
			Get
				Return _onDeleteClientClick
			End Get
			Set(ByVal value As String)
				_onDeleteClientClick = value
			End Set
		End Property

		Public Overrides Property OnFindClientClick() As String
			Get
				Return _onFindClientClick
			End Get
			Set(ByVal value As String)
				_onFindClientClick = value
			End Set
        End Property

        Public Overrides Property OnBackClientClick() As String
            Get
                Return _onBackClientClick
            End Get
            Set(ByVal value As String)
                _onBackClientClick = value
            End Set
        End Property

        Public Overrides Property OnEditClientClick() As String
            Get
                Return _onEditClientClick
            End Get
            Set(ByVal value As String)
                _onEditClientClick = value
            End Set
        End Property

        Public Overrides ReadOnly Property CustomControls() As ControlCollection
            Get
                Return phCustomControls.Controls
            End Get
        End Property


#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

			Dim qsMode As StdButtonsMode = Utils.ToInt32(Request.QueryString("mode"))
			Dim qsID As Integer = Utils.ToInt32(Request.QueryString("id"))

			' if we haven't posted back, i.e. this is the first time the screen has been accessed, look at the querystring parameters
			If Not Me.IsPostBack AndAlso qsMode <> StdButtonsMode.Initial Then
				Me.Mode = qsMode
				Select Case Me.Mode
					Case StdButtonsMode.AddNew
						btnNew_Click(Nothing, Nothing)
					Case StdButtonsMode.Fetched
						hidSelectedItemID.Value = qsID
						btnFind_Click(Nothing, Nothing)
					Case StdButtonsMode.Edit
						hidSelectedItemID.Value = qsID
						btnEdit_Click(Nothing, Nothing)
				End Select
			Else
				Me.Mode = [Enum].Parse(GetType(StdButtonsMode), Utils.ToInt32(hidMode.Value))
				If Me.Mode = StdButtonsMode.Initial AndAlso Me.InitialMode <> StdButtonsMode.Unknown Then
					Me.Mode = Me.InitialMode
				End If
			End If

			' dropdown items are not persisted in view state so we need to add them every time
            With cboSearchBy.Items
                .Clear()
                For Each key As String In Me.SearchBy.Keys
                    .Add(New ListItem(key, Me.SearchBy(key)))
                Next
            End With
            ' persist the dropdown value to a hidden field as the dropdown can be disabled and hence will not be posted back
            If Not Request.Form(cboSearchBy.UniqueID) Is Nothing Then
                cboSearchBy.SelectedValue = Request.Form(cboSearchBy.UniqueID)
            Else
                cboSearchBy.SelectedValue = hidSearchBy.Value
            End If

        End Sub
       

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

			Const KEY_STDBUTTONS As String = "Target.Web.Library.UserControls.StdButtons"
			Const KEY_STDBUTTONS_STARTUP As String = "Target.Web.Library.UserControls.StdButtons.Startup"

            Dim tableNames As StringBuilder = New StringBuilder()
            Dim controlsToDisable As New List(Of Control)

			hidMode.Value = [Enum].ToObject(GetType(StdButtonsMode), Me.Mode)
			' persist the dropdown value to a hidden field as the dropdown can be disabled and hence will not be posted back
            hidSearchBy.Value = cboSearchBy.SelectedValue

            SetupUI()

			' add client-side JS to buttons
			If Not Me.OnNewClientClick Is Nothing Then btnNew.OnClientClick = Me.OnNewClientClick
			If Not Me.OnSaveClientClick Is Nothing Then btnSave.OnClientClick = Me.OnSaveClientClick
			If Not Me.OnCancelClientClick Is Nothing Then btnCancel.OnClientClick = Me.OnCancelClientClick
			If Not Me.OnDeleteClientClick Is Nothing Then btnDelete.OnClientClick = Me.OnDeleteClientClick
            If Not Me.OnFindClientClick Is Nothing Then btnFind.OnClientClick = Me.OnFindClientClick
            If Not Me.OnBackClientClick Is Nothing Then btnBack.Attributes("onclick") = Me.OnBackClientClick
            If Not Me.OnEditClientClick Is Nothing Then btnEdit.OnClientClick = Me.OnEditClientClick
            '
            ' now add standard button JS
			btnDelete.OnClientClick &= "return StdButtons_OnDeleteClicked()"

			btnFind.OnClientClick &= String.Format("return StdButtons_OnFindClicked('{0}', '{1}', '{2}', {3}, {4})", _
			 cboSearchBy.ClientID, txtSearchFor.ClientID, hidSelectedItemID.ClientID, CType(Me.GenericFinderTypeID, Integer), _
			 WebUtils.GetStringArrayAsJavascriptArray(Me.GenericFinderExtraParams.ToArray()))

            tableNames.Append("|")
            For Each table As String In Me.AuditLogTableNames
                tableNames.Append(table)
                tableNames.Append("|")
            Next

            btnAudit.Attributes.Add("onclick", String.Format("StdButtons_btnAudit_Click('{0}',{1},'{2}');", Me.ID, Me.UseApplicationIdFilter.ToString().ToLower(), tableNames.ToString()))

            If Not Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), KEY_STDBUTTONS) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), KEY_STDBUTTONS, WebUtils.GetVirtualPath("Library/Javascript/StdButtons.js"))
            End If

            With ctlReports
                .ButtonText = Me.ReportButtonText
                .ReportID = Me.ReportID
                .ButtonWidth = "5em"
                .ReportToExcel = Me.ReportToExcel
                .ReportToPdf = Me.ReportToPdf
                .Position = Me.ReportPosition
                .Parameters = Me.ReportButtonParameters

            End With

        End Sub

#End Region

#Region " Button Clicks "

		Private Sub btnNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNew.Click

			Dim beforeArgs As StdButtonBeforeModeEventArgs = New StdButtonBeforeModeEventArgs(False, Me.Mode)
			Me.OnBeforeModeChanged(beforeArgs)
			If beforeArgs.Cancel Then Return

			Me.SelectedItemID = 0
            Dim buttonArgs As StdButtonEventArgs = New StdButtonEventArgs(False, 0, Me)
			Me.OnNewClicked(buttonArgs)
			If buttonArgs.Cancel Then Return
			Me.Mode = StdButtonsMode.AddNew

			Dim afterArgs As StdButtonAfterModeEventArgs = New StdButtonAfterModeEventArgs(Me.Mode)
			Me.OnAfterModeChanged(afterArgs)

		End Sub

		Private Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEdit.Click

			Dim beforeArgs As StdButtonBeforeModeEventArgs = New StdButtonBeforeModeEventArgs(False, Me.Mode)
			Me.OnBeforeModeChanged(beforeArgs)
			If beforeArgs.Cancel Then Return

            Dim buttonArgs As StdButtonEventArgs = New StdButtonEventArgs(False, Utils.ToInt32(hidSelectedItemID.Value), Me)
			Me.OnEditClicked(buttonArgs)
			If buttonArgs.Cancel Then Return
			Me.Mode = StdButtonsMode.Edit

			Dim afterArgs As StdButtonAfterModeEventArgs = New StdButtonAfterModeEventArgs(Me.Mode)
			Me.OnAfterModeChanged(afterArgs)

		End Sub

		Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click

			Dim beforeArgs As StdButtonBeforeModeEventArgs = New StdButtonBeforeModeEventArgs(False, Me.Mode)
			Me.OnBeforeModeChanged(beforeArgs)
			If beforeArgs.Cancel Then Return

            Dim buttonArgs As StdButtonEventArgs = New StdButtonEventArgs(False, Utils.ToInt32(hidSelectedItemID.Value), Me)
			Me.OnCancelClicked(buttonArgs)
			If buttonArgs.Cancel Then Return
			Me.Mode = IIf(Me.Mode = StdButtonsMode.AddNew, StdButtonsMode.Initial, StdButtonsMode.Fetched)

			Dim afterArgs As StdButtonAfterModeEventArgs = New StdButtonAfterModeEventArgs(Me.Mode)
			Me.OnAfterModeChanged(afterArgs)

		End Sub

		Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

			Dim beforeArgs As StdButtonBeforeModeEventArgs = New StdButtonBeforeModeEventArgs(False, Me.Mode)
			Me.OnBeforeModeChanged(beforeArgs)
			If beforeArgs.Cancel Then Return

            Dim buttonArgs As StdButtonEventArgs = New StdButtonEventArgs(False, Utils.ToInt32(hidSelectedItemID.Value), Me)
			Me.OnSaveClicked(buttonArgs)
			If buttonArgs.Cancel Then Return
			hidSelectedItemID.Value = buttonArgs.ItemID
			Me.Mode = StdButtonsMode.Fetched

			Dim afterArgs As StdButtonAfterModeEventArgs = New StdButtonAfterModeEventArgs(Me.Mode)
			Me.OnAfterModeChanged(afterArgs)

		End Sub

		Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click

			Dim beforeArgs As StdButtonBeforeModeEventArgs = New StdButtonBeforeModeEventArgs(False, Me.Mode)
			Me.OnBeforeModeChanged(beforeArgs)
			If beforeArgs.Cancel Then Return

            Dim buttonArgs As StdButtonEventArgs = New StdButtonEventArgs(False, Utils.ToInt32(hidSelectedItemID.Value), Me)
			Me.OnDeleteClicked(buttonArgs)
			If buttonArgs.Cancel Then Return
            Me.Mode = StdButtonsMode.Initial
            Me.SelectedItemID = 0

			Dim afterArgs As StdButtonAfterModeEventArgs = New StdButtonAfterModeEventArgs(Me.Mode)
			Me.OnAfterModeChanged(afterArgs)

		End Sub

		Private Sub btnFind_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFind.Click

			Dim beforeArgs As StdButtonBeforeModeEventArgs = New StdButtonBeforeModeEventArgs(False, Me.Mode)
			Me.OnBeforeModeChanged(beforeArgs)
			If beforeArgs.Cancel Then Return

            Dim buttonArgs As StdButtonEventArgs = New StdButtonEventArgs(False, Utils.ToInt32(hidSelectedItemID.Value), Me)
			Me.OnFindClicked(buttonArgs)
			If buttonArgs.Cancel Then Return
			Me.Mode = StdButtonsMode.Fetched

			Dim afterArgs As StdButtonAfterModeEventArgs = New StdButtonAfterModeEventArgs(Me.Mode)
			Me.OnAfterModeChanged(afterArgs)

		End Sub

#End Region

#Region " SetupUI "

        Private Sub SetupUI()

            Dim thePage As Target.Web.Apps.BasePage = CType(Me.Page, Target.Web.Apps.BasePage)

            divBack.Visible = Me.AllowBack AndAlso Not (Request.QueryString("backUrl") Is Nothing) AndAlso Request.QueryString("backUrl").Length > 0
            btnNew.Visible = Me.ShowNew
            btnEdit.Visible = Me.AllowEdit
            btnDelete.Visible = Me.AllowDelete
			divFinder.Visible = Me.AllowFind
            divAudit.Visible = (Not Me.AuditLogTableNames Is Nothing AndAlso Me.AuditLogTableNames.Count > 0)

            ' store the menu item ID of the current page in the session for the audit loggin popup to use
            If divAudit.Visible Then
                Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = thePage.MenuItemID
            End If

            ' reports functionality
            If Me.ReportID > 0 Then
                divReports.Visible = True
            End If


            Select Case Me.Mode
                Case StdButtonsMode.Initial
                    btnNew.Enabled = Me.AllowNew
                    btnEdit.Enabled = False
                    btnSave.Visible = False
                    btnCancel.Visible = False
                    btnDelete.Enabled = False
                    cboSearchBy.Enabled = True
                    txtSearchFor.Enabled = True
                    btnFind.Enabled = True
                    btnAudit.Disabled = True
                    ToggleEditableFields(False)

                Case StdButtonsMode.Fetched
                    btnNew.Enabled = Me.AllowNew
                    btnEdit.Enabled = True
                    btnSave.Visible = False
                    btnCancel.Visible = False
                    btnDelete.Enabled = Me.AllowDelete
                    cboSearchBy.Enabled = True
                    txtSearchFor.Enabled = True
                    btnFind.Enabled = True
                    btnAudit.Disabled = False
                    ToggleEditableFields(False)

                Case StdButtonsMode.AddNew
                    btnNew.Enabled = False
                    btnEdit.Visible = False
                    btnSave.Visible = Me.ShowSave
                    btnCancel.Visible = True
                    btnDelete.Visible = False
                    ctlReports.Visible = Me.ShowReports
                    cboSearchBy.Enabled = False
                    txtSearchFor.Enabled = False
                    btnFind.Enabled = False
                    btnAudit.Disabled = True
                    ToggleEditableFields(True)

                Case StdButtonsMode.Edit
                    btnNew.Enabled = False
                    btnEdit.Visible = False
                    btnSave.Visible = Me.ShowSave
                    btnCancel.Visible = True
                    btnDelete.Visible = False
                    cboSearchBy.Enabled = False
                    txtSearchFor.Enabled = False
                    btnFind.Enabled = False
                    btnAudit.Disabled = False
                    ToggleEditableFields(True)

            End Select

            If Not btnNew.Visible AndAlso _
                    Not btnEdit.Visible AndAlso _
                    Not btnSave.Visible AndAlso _
                    Not btnCancel.Visible AndAlso _
                    Not btnDelete.Visible Then
                divButtons.Visible = False
            End If

        End Sub

#End Region

#Region " Render "

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

            Dim options As PostBackOptions = New PostBackOptions(Me.btnFind)
            If Not options Is Nothing Then
                options.PerformValidation = True
                Page.ClientScript.RegisterForEventValidation(options)
                litDoFindPostBackJS.Text = Page.ClientScript.GetPostBackEventReference(options)
            End If
            MyBase.Render(writer)

        End Sub

#End Region

#Region " CreateChildControls "

        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()
            MyBase.OnAddCustomControls(phCustomControls.Controls)
            If phCustomControls.Controls.Count = 0 Then
                fsCustomControls.Visible = False
            End If
        End Sub

#End Region

#Region " ClearCustomControls "
        Public Overrides Sub ClearCustomControls()
            phCustomControls.Controls.Clear()
            If phCustomControls.Controls.Count = 0 Then
                fsCustomControls.Visible = False
            End If
        End Sub
#End Region

	End Class

End Namespace