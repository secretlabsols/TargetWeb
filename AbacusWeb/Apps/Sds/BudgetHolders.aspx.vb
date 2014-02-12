Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Sds

    ''' <summary>
    ''' Admin page used to maintain Budget Holders.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO   20/11/2012  D12310 - Enable the back button.
    ''' MikeVO   26/04/2011  SDS issue #600 - corrected Documents tab behaviour.
    ''' MoTahir  19/04/2010  D11971 - SDS generic Creditor Notes
    ''' MoTahir  05/10/2010  Issue 156 - Sharepoint
    ''' MoTahir  24/09/2010  Issue 57 Sharepoint.
    ''' MikeVO   10/08/2010  Corrected various issues including GetDirectPaymentMethods() and the CanDelete flag for ThirdPartyBudgetHolders.
    ''' Mo Tahir 07/07/2010  D11798.
    ''' </history>
    Partial Public Class BudgetHolders
        Inherits BasePage

#Region "Fields"

        ' constants
        ' - service users dodgy table keys etc
        Private Const SERVICE_USERS_CTRL_ID_HIDDEN As String = "hidID"
        Private Const SERVICE_USERS_CTRL_ID_REMOVE As String = "suRemove"
        Private Const SERVICE_USERS_CTRL_ID_SERVICE_USER As String = "txtServiceUser"
        Private Const SERVICE_USERS_COUNTER_NEW As String = "ServiceUsersNewCounter"
        Private Const SERVICE_USERS_PREFIX_DELETE As String = "suD"
        Private Const SERVICE_USERS_PREFIX_NEW As String = "detailN"
        Private Const SERVICE_USERS_PREFIX_UPDATE As String = "detailU"
        Private Const SERVICE_USERS_VIEWSTATE_KEY As String = "ServiceUsersList"

        ' locals
        Private _canDelete As Integer = 0
        Private _CurrentBudgetHolder As ThirdPartyBudgetHolder = Nothing
        Private _currentUser As WebSecurityUser = Nothing
        Private _documentsTabAllowed As Boolean = False
        Private _newServiceUsersIdCounter As Integer = 0
        Private _stdBut As StdButtonsBase

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the audit log title.
        ''' </summary>
        ''' <value>
        ''' The audit log title.
        ''' </value>
        Private ReadOnly Property AuditLogTitle() As String
            Get
                Return BudgetHolderBL.AUDIT_LOG_TITLE_BUDGET_HOLDERS
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the audit log user.
        ''' </summary>
        ''' <value>The name of the audit log user.</value>
        Private ReadOnly Property AuditLogUserName() As String
            Get
                Return CurrentUser.ExternalUsername
            End Get
        End Property

        ''' <summary>
        ''' Gets the current budget holder.
        ''' </summary>
        ''' <value>
        ''' The current budget holder.
        ''' </value>
        Private ReadOnly Property CurrentBudgetHolder As ThirdPartyBudgetHolder
            Get
                Dim msg As New ErrorMessage()
                If (_CurrentBudgetHolder Is Nothing AndAlso _stdBut.SelectedItemID > 0) _
                    OrElse (Not _CurrentBudgetHolder Is Nothing AndAlso _CurrentBudgetHolder.ID <> _stdBut.SelectedItemID) Then
                    ' if nowt or different then fetch
                    msg = BudgetHolderBL.GetBudgetHolder(DbConnection, _CurrentBudgetHolder, _stdBut.SelectedItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _CurrentBudgetHolder
            End Get
        End Property

        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>
        ''' The current user.
        ''' </value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _currentUser Is Nothing Then
                    ' if current user not fetched then get current user
                    _currentUser = SecurityBL.GetCurrentUser()
                End If
                Return _currentUser
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.BudgetHolders"), "Budget Holders")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .EditableControls.Add(fsControls.Controls)
                .AllowBack = True
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.BudgetHolders.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.BudgetHolders.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.BudgetHolders.Delete"))
                With .SearchBy
                    .Add("Reference", "Reference")
                    .Add("Organisation", "Organisation")
                    .Add("Surname", "Surname")
                End With
                .GenericFinderTypeID = GenericFinderType.BudgetHolder
                .AuditLogTableNames.Add("ThirdPartyBudgetHolder")
                .AuditLogTableNames.Add("ThirdPartyBudgetHolder_ClientDetail")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.BudgetHolders")
            End With

            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            Me.JsLinks.Add("BudgetHolders.js")
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

            _documentsTabAllowed = SecurityBL.UserHasMenuItem(Me.DbConnection, CurrentUser.ID, _
                                   Target.Library.Web.ConstantsManager.GetConstant( _
                                   "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"), _
                                   Settings.CurrentApplicationID)

            tabDocuments.Visible = False
            Notes1.Visible = False
            tabNotes.Visible = False

            PopulateDirectPaymentMethod()

            ' create service users from view state...dodgy
            For Each id As String In GetServiceUserIdsFromViewState()

                OutputServiceUserControls(id, Nothing, Nothing)

            Next

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim js As New StringBuilder()
            Dim tpbh As ThirdPartyBudgetHolder = CurrentBudgetHolder
            Dim tpbhIsProtected As Boolean = False
            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)

            If _stdBut.ButtonsMode = StdButtonsMode.Edit AndAlso _canDelete <> 0 Then
                txtReference.Enabled = False
                txtOrganisationName.SetFocus = True
            Else
                txtReference.Enabled = True
                txtReference.SetFocus = True
            End If

            ' setup documents tab
            sm.RegisterScriptControl(tabDocuments)
            tabDocuments.Visible = (_documentsTabAllowed AndAlso _stdBut.SelectedItemID > 0)

            ' setup service users tab
            WebUtils.RecursiveDisable(tabServiceUsers.Controls, (_stdBut.ButtonsMode <> StdButtonsMode.Edit))
            tabServiceUsers.Enabled = (_stdBut.ButtonsMode = StdButtonsMode.Edit)

            ' setup notes selector
            InitialiseNotesSelector(_stdBut.SelectedItemID)

            If Not tpbh Is Nothing Then
                ' disable the is global checkbox when system protected

                tpbhIsProtected = (tpbh.SystemType > 0)
                chkIsGlobal.CheckBox.Enabled = Not tpbhIsProtected

                If tpbhIsProtected Then
                    ' always ensure checbox is checked

                    chkIsGlobal.CheckBox.Checked = True

                End If

            End If

            ' setup control events
            chkIsGlobal.CheckBox.Attributes.Add("onclick", String.Format("chkIsGlobal_OnChange('{0}');", chkIsGlobal.CheckBox.ClientID))

            ' create js for client side
            js.AppendFormat("Edit_bhID={0};", _stdBut.SelectedItemID)
            js.AppendFormat("tabContainerID='{0}';", tabStrip.ClientID)
            js.AppendFormat("chkIsGlobalID='{0}';", chkIsGlobal.ClientID)

            ' output js
            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

        End Sub

        ''' <summary>
        ''' Handles the new clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            chkIsGlobal.CheckBox.Checked = True
            tabDocuments.Visible = False
            txtReference.Text = BudgetHolderBL.AutoGeneratedTerm
            PopulateDirectPaymentMethod()
            _stdBut.SelectedItemID = 0
        End Sub


        ''' <summary>
        ''' Handles the find clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing
            Dim tpbh As ThirdPartyBudgetHolder = CurrentBudgetHolder
            Dim serviceUsers As List(Of ThirdPartyBudgetHolder_ClientDetail) = Nothing
            Dim serviceUserIds As List(Of String) = Nothing
            Dim serviceUserUniqueId As String = String.Empty

            _canDelete = tpbh.SystemType
            txtReference.Text = tpbh.Reference
            txtOrganisationName.Text = tpbh.OrganisationName
            txtTitleInitials.Text = tpbh.TitleAndInitials
            txtSurname.Text = tpbh.Surname
            txtAddress.Text = tpbh.Address
            txtPostcode.Text = tpbh.Postcode
            txtEmailAddress.Text = tpbh.EmailAddress
            txtTelephone.Text = tpbh.Telephone
            txtFax.Text = tpbh.Fax
            PopulateDirectPaymentMethod(tpbh.DirectPaymentMethod)
            txtCreditorReference.Text = tpbh.CreditorReference
            txtBacsSortCode.Text = tpbh.BacsSortCode
            txtBacsAccountNumber.Text = tpbh.BacsAccountNumber
            txtBacsReference.Text = tpbh.BacsReference
            chkRedundant.CheckBox.Checked = tpbh.Redundant
            chkIsGlobal.CheckBox.Checked = tpbh.IsGlobal

            ' clear anything from vs
            ClearViewState()

            If Not tpbh.IsGlobal AndAlso e.ItemID > 0 Then
                ' fetch any service users if the tph is not global

                ' get ids from vs
                serviceUserIds = GetServiceUserIdsFromViewState()

                ' get the service users to display
                msg = BudgetHolderBL.GetThirdPartyBudgetHolderClientDetailForBudgetHolder(DbConnection, Nothing, e.ItemID, serviceUsers)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                For Each su As ThirdPartyBudgetHolder_ClientDetail In serviceUsers
                    ' loop over each service user and output controls...this is exceptionally crap...as are viewstate tables $!%&!!!!!!

                    serviceUserUniqueId = GetUniqueServiceUserID(su)
                    OutputServiceUserControls(serviceUserUniqueId, su, Nothing)
                    serviceUserIds.Add(serviceUserUniqueId)

                Next

                ' persist the ids for later use
                PersistServiceUserIdsToViewState(serviceUserIds)

            End If

        End Sub

        ''' <summary>
        ''' Handles the save clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim tpbh As ThirdPartyBudgetHolder = CurrentBudgetHolder
            Dim serviceUsersToDelete As List(Of Integer) = Nothing
            Dim serviceUsersToSave As List(Of Integer) = Nothing

            If Me.IsPostBack Then
                PopulateDirectPaymentMethod()
                cboDirectPaymentMethod.SelectPostBackValue()
            End If

            Me.Validate("Save")

            If Me.IsValid Then
                ' if valid then persist changes

                If tpbh Is Nothing Then
                    ' if nowt then create new

                    tpbh = New ThirdPartyBudgetHolder(AuditLogUserName, AuditLogTitle)

                End If

                ' setup the item from the form
                With tpbh
                    .Reference = txtReference.Text.Trim
                    .OrganisationName = txtOrganisationName.Text.Trim
                    .TitleAndInitials = txtTitleInitials.Text.Trim
                    .Surname = txtSurname.Text.Trim
                    .Address = txtAddress.Text.Trim
                    .Postcode = txtPostcode.Text.Trim
                    .EmailAddress = txtEmailAddress.Text.Trim
                    .Telephone = txtTelephone.Text.Trim
                    .Fax = txtFax.Text.Trim
                    If Not cboDirectPaymentMethod.DropDownList.SelectedValue = String.Empty Then
                        .DirectPaymentMethod = cboDirectPaymentMethod.DropDownList.SelectedValue
                    End If
                    .CreditorReference = txtCreditorReference.Text.Trim
                    .BacsSortCode = txtBacsSortCode.Text.Trim
                    .BacsAccountNumber = txtBacsAccountNumber.Text.Trim
                    .BacsReference = txtBacsReference.Text.Trim
                    .Redundant = chkRedundant.CheckBox.Checked
                    .IsGlobal = chkIsGlobal.CheckBox.Checked
                End With

                ' get the service users
                GetServiceUsersToPersist(serviceUsersToSave, serviceUsersToDelete)

                ' save the service users
                msg = BudgetHolderBL.SaveBudgetHolderAndServiceUsers(DbConnection, tpbh, serviceUsersToSave, serviceUsersToDelete, AuditLogUserName, AuditLogTitle)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' reset form with new values after save
                e.ItemID = tpbh.ID
                _stdBut.SelectedItemID = e.ItemID
                FindClicked(e)

            Else
                ' else screen is not valid

                e.Cancel = True

            End If

        End Sub

        ''' <summary>
        ''' Handles the cancel clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

            ClearViewState()

            If e.ItemID = 0 Then
                ' if no id then set defaults for form

                txtReference.Text = String.Empty
                txtOrganisationName.Text = String.Empty
                txtTitleInitials.Text = String.Empty
                txtSurname.Text = String.Empty
                txtAddress.Text = String.Empty
                txtPostcode.Text = String.Empty
                txtEmailAddress.Text = String.Empty
                txtTelephone.Text = String.Empty
                txtFax.Text = String.Empty
                cboDirectPaymentMethod.Text = String.Empty
                txtCreditorReference.Text = String.Empty
                txtBacsSortCode.Text = String.Empty
                txtBacsAccountNumber.Text = String.Empty
                txtBacsReference.Text = String.Empty
                chkRedundant.CheckBox.Checked = False
                chkIsGlobal.CheckBox.Checked = True

            Else
                ' otherwise find the item

                FindClicked(e)

            End If

        End Sub

        ''' <summary>
        ''' Handles the delete clicked event.
        ''' </summary>
        ''' <param name="e">The <see cref="StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            ' check can delete flag
            msg = BudgetHolderBL.Delete(Me.DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)
            If Not msg.Success Then

                lblError.Text = String.Format("{0}<br /><br />", msg.Message)
                e.Cancel = True
                FindClicked(e)
                Return

            End If

            ' set id properties
            e.ItemID = 0
            _stdBut.SelectedItemID = e.ItemID

            ' find the id of nowt which will result in a blank screen
            FindClicked(e)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the btnAdd control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click

            Dim id As String = String.Empty
            Dim list As List(Of String) = Nothing
            Dim item As New ThirdPartyBudgetHolder_ClientDetail(AuditLogUserName, AuditLogTitle)

            ' get the list to add the new item
            list = GetServiceUserIdsFromViewState()

            ' generate an id
            id = GetUniqueServiceUserID(item)

            ' output a blank row
            OutputServiceUserControls(id, item, Nothing)

            ' add the item to view state and persist
            list.Add(id)
            PersistServiceUserIdsToViewState(list)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the btnRemove control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetServiceUserIdsFromViewState()
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(SERVICE_USERS_CTRL_ID_REMOVE, String.Empty)

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(SERVICE_USERS_PREFIX_NEW) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(SERVICE_USERS_PREFIX_UPDATE, SERVICE_USERS_PREFIX_DELETE)
                    End If
                    Exit For
                End If
            Next

            ' remove from the grid
            For index As Integer = 0 To phServiceUsers.Controls.Count - 1
                If phServiceUsers.Controls(index).ID = id Then
                    phServiceUsers.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistServiceUserIdsToViewState(list)

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Clears the state of the view.
        ''' </summary>
        Private Sub ClearViewState()

            ViewState.Remove(SERVICE_USERS_VIEWSTATE_KEY)
            ViewState.Remove(SERVICE_USERS_COUNTER_NEW)
            phServiceUsers.Controls.Clear()

        End Sub

        ''' <summary>
        ''' Gets the service user ids from view state.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetServiceUserIdsFromViewState() As List(Of String)

            Dim list As New List(Of String)()

            If Not ViewState.Item(SERVICE_USERS_VIEWSTATE_KEY) Is Nothing Then
                ' if in vs then use that instance

                list = CType(ViewState.Item(SERVICE_USERS_VIEWSTATE_KEY), List(Of String))

            End If

            If ViewState.Item(SERVICE_USERS_COUNTER_NEW) Is Nothing Then
                ' default to 0 if not found

                _newServiceUsersIdCounter = 0

            Else
                ' otherwise fetch count from vs

                _newServiceUsersIdCounter = CType(ViewState.Item(SERVICE_USERS_COUNTER_NEW), Integer)

            End If

            Return list

        End Function

        ''' <summary>
        ''' Persists the state of the service user ids to view.
        ''' </summary>
        ''' <param name="list">The list.</param>
        Private Sub PersistServiceUserIdsToViewState(ByVal list As List(Of String))

            ViewState.Add(SERVICE_USERS_VIEWSTATE_KEY, list)
            ViewState.Add(SERVICE_USERS_COUNTER_NEW, _newServiceUsersIdCounter)

        End Sub

        ''' <summary>
        ''' Populates the direct payment method.
        ''' </summary>
        ''' <param name="value">The value.</param>
        Private Sub PopulateDirectPaymentMethod(Optional ByVal value As Byte = 0)

            Dim msg As ErrorMessage
            Dim dpMethods As List(Of ViewablePair) = Nothing

            msg = BudgetHolderBL.GetDirectPaymentMethods(conn:=Me.DbConnection, dpMethods:=dpMethods)

            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboDirectPaymentMethod.DropDownList
                .Items.Clear()
                .DataSource = dpMethods
                .DataValueField = "Value"
                .DataTextField = "Text"
                .SelectedValue = Nothing
                .DataBind()
            End With

            If value <> 0 Then cboDirectPaymentMethod.DropDownList.SelectedValue = value

        End Sub

        ''' <summary>
        ''' Gets the service users to persist.
        ''' </summary>
        ''' <param name="toSave">To save.</param>
        ''' <param name="toDelete">To delete.</param>
        Private Sub GetServiceUsersToPersist(ByRef toSave As List(Of Integer), ByRef toDelete As List(Of Integer))

            Dim ids As List(Of String) = GetServiceUserIdsFromViewState()
            Dim clientSelector As InPlaceSelectors.InPlaceClientSelector = Nothing

            ' setup list instances
            toSave = New List(Of Integer)()
            toDelete = New List(Of Integer)()

            For Each id As String In ids
                ' loop over each row to save or delete and add items into relevant lists

                If id.StartsWith(SERVICE_USERS_PREFIX_DELETE) Then
                    ' if starts with the delete prefix then add to the list to delete

                    toDelete.Add(id.Replace(SERVICE_USERS_PREFIX_DELETE, String.Empty))

                Else
                    ' else is new or update so get the id from the ips

                    clientSelector = CType(phServiceUsers.FindControl(SERVICE_USERS_CTRL_ID_SERVICE_USER & id), InPlaceSelectors.InPlaceClientSelector)
                    toSave.Add(clientSelector.ClientDetailID)

                End If

            Next

        End Sub

        ''' <summary>
        ''' Gets the unique service user ID.
        ''' </summary>
        ''' <param name="item">The item.</param>
        ''' <returns></returns>
        Private Function GetUniqueServiceUserID(ByVal item As ThirdPartyBudgetHolder_ClientDetail) As String

            Dim id As String = String.Empty

            If item.ClientDetailID = 0 Then
                ' if no id then output new id

                id = SERVICE_USERS_PREFIX_NEW & _newServiceUsersIdCounter
                _newServiceUsersIdCounter += 1

            Else
                ' otherwise output update id

                id = SERVICE_USERS_PREFIX_UPDATE & item.ClientDetailID

            End If

            Return id

        End Function

        ''' <summary>
        ''' Outputs the service user controls.
        ''' </summary>
        ''' <param name="uniqueID">The unique ID.</param>
        ''' <param name="item">The item.</param>
        ''' <param name="rowIndex">Index of the row.</param>
        Private Sub OutputServiceUserControls(ByVal uniqueID As String, ByVal item As ThirdPartyBudgetHolder_ClientDetail, ByVal rowIndex As Nullable(Of Integer))

            Dim btnRemove As HtmlInputImage = Nothing
            Dim cell As TableCell = Nothing
            Dim serviceUserCtrl As InPlaceSelectors.InPlaceClientSelector = Nothing
            Dim row As TableRow = Nothing

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(SERVICE_USERS_PREFIX_DELETE) Then

                row = New TableRow()
                row.ID = uniqueID

                If Not rowIndex.HasValue Then
                    ' if we have no row index then add the row at the end 
                    phServiceUsers.Controls.Add(row)
                Else
                    ' else we have a row index so insert at that index
                    phServiceUsers.Controls.AddAt(rowIndex.Value, row)
                End If

                ' service user selector
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                serviceUserCtrl = CType(LoadControl("~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx"), InPlaceSelectors.InPlaceClientSelector)
                With serviceUserCtrl
                    .ID = String.Format("{0}{1}", SERVICE_USERS_CTRL_ID_SERVICE_USER, uniqueID)
                    .Required = True
                    .ValidationGroup = "Save"
                    If Not item Is Nothing AndAlso item.ClientDetailID > 0 Then
                        .ClientDetailID = item.ClientDetailID
                    End If
                End With
                cell.Controls.Add(serviceUserCtrl)

                '++ Remove button..
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "right"
                cell.HorizontalAlign = HorizontalAlign.Center
                btnRemove = New HtmlInputImage()
                With btnRemove
                    .ID = SERVICE_USERS_CTRL_ID_REMOVE & uniqueID
                    .Src = WebUtils.GetVirtualPath("Images/delete.png")
                    .Alt = "Remove this Service User?"
                    AddHandler .ServerClick, AddressOf btnRemove_Click
                End With
                cell.Controls.Add(btnRemove)
            End If

        End Sub

        ''' <summary>
        ''' Initialises the notes selector.
        ''' </summary>
        ''' <param name="selectedItemID">The selected item ID.</param>
        Private Sub InitialiseNotesSelector(ByVal selectedItemID As Integer)

            ScriptManager.GetCurrent(Page).RegisterScriptControl(tabNotes)

            'check if there is a client id
            Notes1.Visible = False
            tabNotes.Visible = False

            If selectedItemID > 0 Then
                'load the notes control
                tabNotes.Visible = True
                Notes1.Visible = True
                With CType(Notes1, Target.Abacus.Web.Apps.UserControls.NotesSelector)
                    .FilterNoteType = Abacus.Library.Notes.NoteTypes.ThirdPartyBudgetHolder
                    .FilterNoteTypeChildID = _stdBut.SelectedItemID
                    .ViewNoteInNewWindow = True
                    .InitControl(Me.Page)
                End With
            End If

        End Sub

#End Region

    End Class

End Namespace

