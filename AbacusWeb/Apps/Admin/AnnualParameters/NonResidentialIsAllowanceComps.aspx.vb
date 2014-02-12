Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security

Namespace Apps.Admin.AnnualParameters

    ''' <summary>
    ''' Web Form for Maintaining Fairer Contribution Non Residential I\S Allowance Components
    ''' </summary>
    ''' <history>
    ''' MikeVO   20/04/2011 SDS issue #588 - sensible limits on range validators.
    ''' ColinD   07/12/2010 D11807A - Updated - SDS Issue 411 - Upped max age to 999
    ''' ColinD   15/11/2010 D11807A - Created
    ''' </history>
    Partial Public Class NonResidentialIsAllowanceComps
        Inherits BasePage

#Region "Fields"

        ' locals
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _FetchedDomIsAllowanceComponents As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.DomISAllowanceComponents) = Nothing

        ' constants
        Private Const _AuditLogTable As String = "DomISAllowanceComponents"
        Private Const _CurrencyFormat As String = "N2"
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _DefaultAssessedAsValue As String = "INDIVIDUAL"
        Private Const _DropDownListDefaultText As String = "Please select..."
        Private Const _DropDownListDefaultValue As String = "0"
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber
        Private Const _MaxAge As Integer = 999
        Private Const _PageTitle As String = "Non-Residential I/S Allowance Components"
        Private Const _ReportsSimpleListID As String = "AbacusIntranet.WebReport.NonResIsAllowanceComponents"
        Private Const _WebCmdAddNewKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.NonResidentialIsAllowanceComponents.AddNew"
        Private Const _WebCmdDeleteKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.NonResidentialIsAllowanceComponents.Delete"
        Private Const _WebCmdEditKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.NonResidentialIsAllowanceComponents.Edit"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Income.ReferenceData.NonResidentialIsAllowanceComponents"

#End Region

#Region "Properties"

#Region "Authorisation Properties"

        ''' <summary>
        ''' Gets a value indicating whether can add new records.
        ''' </summary>
        ''' <value><c>true</c> if user can add new records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasAddNewCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebCmdAddNewKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can delete new records.
        ''' </summary>
        ''' <value><c>true</c> if user can delete new records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasDeleteCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebCmdDeleteKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can edit records.
        ''' </summary>
        ''' <value><c>true</c> if user can edit records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasEditCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebCmdEditKey))
            End Get
        End Property

#End Region

#Region "Component Control Properties"

        ''' <summary>
        ''' Gets or sets allowance.
        ''' </summary>
        ''' <value>Allowance.</value>
        Private Property ComponentAllowance() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAllowance.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAllowance.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets assessed as.
        ''' </summary>
        ''' <value>Assessed as.</value>
        Private Property ComponentAssessedAs() As String
            Get
                Return rblAssessedAs.SelectedValue
            End Get
            Set(ByVal value As String)
                rblAssessedAs.SelectedValue = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to from age.
        ''' </summary>
        ''' <value>From Age.</value>
        Private Property ComponentFromAge() As Integer
            Get
                Return Target.Library.Utils.ToInt32(txtFromAge.Text)
            End Get
            Set(ByVal value As Integer)
                txtFromAge.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets from date.
        ''' </summary>
        ''' <value>From date.</value>
        Private Property ComponentFromDate() As Nullable(Of DateTime)
            Get
                Return Target.Library.Utils.ToDateTime(txtFromDate.Text)
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If Not value Is Nothing AndAlso value.HasValue Then
                    ' if there is a date then display it
                    txtFromDate.Text = value.Value.ToString(_DateFormat)
                Else
                    ' else no date so display an empty string
                    txtFromDate.Text = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to age.
        ''' </summary>
        ''' <value>To Age.</value>
        Private Property ComponentToAge() As Integer
            Get
                Return Target.Library.Utils.ToInt32(txtToAge.Text)
            End Get
            Set(ByVal value As Integer)
                If value > 0 Then
                    ' if value is larger than 0 display that value
                    txtToAge.Text = value
                Else
                    ' else value is less to equal to 0 so display an empty string i.e. open ended
                    txtToAge.Text = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to date.
        ''' </summary>
        ''' <value>To date.</value>
        Private Property ComponentToDate() As Nullable(Of DateTime)
            Get
                Return Target.Library.Utils.ToDateTime(txtToDate.Text)
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If Not value Is Nothing AndAlso value.HasValue AndAlso value.Value > DateTime.MinValue Then
                    ' if there is a date then display it
                    txtToDate.Text = value.Value.ToString(_DateFormat)
                Else
                    ' else no date so display an empty string
                    txtToDate.Text = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the premium of the component.
        ''' </summary>
        ''' <value>The premium.</value>
        Private Property ComponentPremium() As Integer
            Get
                Return Target.Library.Utils.ToInt32(cboPremium.GetPostBackValue())
            End Get
            Set(ByVal value As Integer)

                If cboPremium.DropDownList.SelectedItem IsNot Nothing Then
                    ' if we have a selected item then unselect it

                    cboPremium.DropDownList.SelectedItem.Selected = False

                End If

                If cboPremium.DropDownList.Items.FindByValue(value) Is Nothing AndAlso value > 0 Then
                    ' if we cant find the item in the list then it is most probably redundant so fetch from db

                    Dim premium As New Target.Abacus.Library.DataClasses.Lookup(DbConnection)
                    Dim msg As New ErrorMessage()

                    ' get the item from db and display error if not successful
                    msg = premium.Fetch(value)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' add the item into the list
                    cboPremium.DropDownList.Items.Add(New ListItem(premium.Description, premium.ID))

                End If

                If cboPremium.DropDownList.Items.FindByValue(value) IsNot Nothing Then
                    ' if the item exists

                    cboPremium.DropDownList.Items.FindByValue(value).Selected = True

                Else
                    ' else the item doesn't exist so select first item

                    cboPremium.DropDownList.Items(0).Selected = True

                End If

            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the recipient of the for the component.
        ''' </summary>
        ''' <value>The recipient.</value>
        Private Property ComponentPremiumRecipient() As Integer
            Get
                Return Target.Library.Utils.ToInt32(cboPremiumRecipient.GetPostBackValue())
            End Get
            Set(ByVal value As Integer)

                If cboPremiumRecipient.DropDownList.SelectedItem IsNot Nothing Then
                    ' if we have a selected item then unselect it

                    cboPremiumRecipient.DropDownList.SelectedItem.Selected = False

                End If

                If cboPremiumRecipient.DropDownList.Items.FindByValue(value) Is Nothing AndAlso value > 0 Then
                    ' if we cant find the item in the list then it is most probably redundant so fetch from db

                    Dim recipient As New Target.Abacus.Library.DataClasses.Lookup(DbConnection)
                    Dim msg As New ErrorMessage()

                    ' get the item from db and display error if not successful
                    msg = recipient.Fetch(value)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' add the item into the list
                    cboPremiumRecipient.DropDownList.Items.Add(New ListItem(recipient.Description, recipient.ID))

                End If

                If cboPremiumRecipient.DropDownList.Items.FindByValue(value) IsNot Nothing Then
                    ' if the item exists

                    cboPremiumRecipient.DropDownList.Items.FindByValue(value).Selected = True

                Else
                    ' else the item doesn't exist so select first item

                    cboPremiumRecipient.DropDownList.Items(0).Selected = True

                End If

            End Set
        End Property

#End Region

        ''' <summary>
        ''' Gets the audit log title.
        ''' </summary>
        ''' <value>The audit log title.</value>
        Private ReadOnly Property AuditLogTitle() As String
            Get
                Return AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
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
        ''' Gets the current user.
        ''' </summary>
        ''' <value>The current user.</value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    ' if current user not fetched then get current user
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the fetched dom is allowance components.
        ''' </summary>
        ''' <value>The fetched dom is allowance components.</value>
        Private ReadOnly Property FetchedDomIsAllowanceComponents() As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.DomISAllowanceComponents)
            Get
                If _FetchedDomIsAllowanceComponents Is Nothing Then
                    ' always init the dictionary if null
                    _FetchedDomIsAllowanceComponents = New Dictionary(Of Integer, Target.Abacus.Library.DataClasses.DomISAllowanceComponents)
                End If
                Return _FetchedDomIsAllowanceComponents
            End Get
        End Property

        ''' <summary>
        ''' Gets the standard buttons control.
        ''' </summary>
        ''' <value>The standard buttons control.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Gets the DomIsAllowanceComponent.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetDomIsAllowanceComponent(ByVal id As Integer) As Target.Abacus.Library.DataClasses.DomISAllowanceComponents

            Dim item As Target.Abacus.Library.DataClasses.DomISAllowanceComponents = Nothing
            Dim msg As New ErrorMessage()

            If FetchedDomIsAllowanceComponents.ContainsKey(id) Then
                ' if already fetched then return that object

                item = FetchedDomIsAllowanceComponents(id)

            Else
                ' else not already fetched so get the item from db

                msg = AnnualParametersBL.GetDomIsAllowanceComponent(DbConnection, id, AuditLogUserName, AuditLogTitle, item)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' add the item to the dictionary for further use
                FetchedDomIsAllowanceComponents.Add(id, item)

            End If

            Return item

        End Function

        ''' <summary>
        ''' Gets the DomIsAllowanceComponent from screen.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetDomIsAllowanceComponentFromScreen() As Target.Abacus.Library.DataClasses.DomISAllowanceComponents

            Dim msg As New ErrorMessage()
            Dim comp As Target.Abacus.Library.DataClasses.DomISAllowanceComponents = Nothing
            Dim itemId As Integer = StandardButtonsControl.SelectedItemID

            ' populate the premium drop down
            msg = PopulatePremiumDropDown()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' populate the premium recipient drop down
            msg = PopulatePremiumRecipientDropDown()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If itemId > 0 Then
                ' if we are modifying an existing comp then fetch db copy and make changes to that

                comp = GetDomIsAllowanceComponent(itemId)

            Else
                ' else item is new so create new object

                comp = New Target.Abacus.Library.DataClasses.DomISAllowanceComponents(DbConnection, AuditLogUserName, AuditLogTitle)

            End If

            ' setup values on the comp from the screen
            With comp
                .DateFrom = ComponentFromDate.Value
                If ComponentToDate.HasValue Then
                    .DateTo = ComponentToDate.Value
                Else
                    .DateTo = Nothing
                End If
                .BenefitTypeID = ComponentPremium
                .RecipientID = ComponentPremiumRecipient
                .AssessedAs = ComponentAssessedAs
                .AgeFrom = ComponentFromAge
                .AgeTo = ComponentToAge
                .Allowance = ComponentAllowance
                ' fairer contribution is always true from this form
                .FairerContribution = TriState.True
            End With

            ' reset the drop down postback values
            ComponentPremium = comp.BenefitTypeID
            ComponentPremiumRecipient = comp.RecipientID

            Return comp

        End Function

        ''' <summary>
        ''' Populates the premium drop down list.
        ''' </summary>
        Private Function PopulatePremiumDropDown() As ErrorMessage

            Dim items As LookupCollection = Nothing
            Dim msg As New ErrorMessage()

            Try

                ' always clear all items from the list first
                cboPremium.DropDownList.Items.Clear()

                ' get tems from db
                msg = AnnualParametersBL.GetDomIsAllowanceComponentsBenefitTypes(DbConnection, items)
                If Not msg.Success Then Return msg

                If Not items Is Nothing AndAlso items.Count > 0 Then
                    'if we have some items to add to drop down then do so

                    With cboPremium.DropDownList
                        .DataTextField = "Description"
                        .DataValueField = "ID"
                        ' order items by description using LINQ
                        .DataSource = items.ToArray().OrderBy(Function(item As Lookup) item.Description)
                        .DataBind()
                    End With

                End If

                ' add default item in to dd
                cboPremium.DropDownList.Items.Insert(0, New ListItem(_DropDownListDefaultText, _DropDownListDefaultValue))

                ' setup success error message
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Populates the premium recipient drop down list.
        ''' </summary>
        Private Function PopulatePremiumRecipientDropDown() As ErrorMessage

            Dim items As LookupCollection = Nothing
            Dim msg As New ErrorMessage()

            Try

                ' always clear all items from the list first
                cboPremiumRecipient.DropDownList.Items.Clear()

                ' get tems from db
                msg = AnnualParametersBL.GetDomIsAllowanceComponentsRecipients(DbConnection, items)
                If Not msg.Success Then Return msg

                If Not items Is Nothing AndAlso items.Count > 0 Then
                    'if we have some items to add to drop down then do so

                    With cboPremiumRecipient.DropDownList
                        .DataTextField = "Description"
                        .DataValueField = "ID"
                        ' order items by description using LINQ
                        .DataSource = items.ToArray().OrderBy(Function(item As Lookup) item.Description)
                        .DataBind()
                    End With

                End If

                ' add default item in to dd
                cboPremiumRecipient.DropDownList.Items.Insert(0, New ListItem(_DropDownListDefaultText, _DropDownListDefaultValue))

                ' setup success error message
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Populates the screen with a DomIsAllowanceComponent using its id.
        ''' </summary>
        ''' <param name="e">The id.</param>
        Private Sub PopulateDomIsAllowanceComponent(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            If e.ItemID > 0 Then
                ' if we have an id to work with then do so

                ' populate the item on screen
                msg = PopulateDomIsAllowanceComponent(GetDomIsAllowanceComponent(e.ItemID))

            Else
                ' else no id so throw an error

                msg = PopulateDomIsAllowanceComponent(CType(Nothing, Target.Abacus.Library.DataClasses.DomISAllowanceComponents))

            End If

            If Not msg.Success Then
                ' if errored populating screen display error

                e.Cancel = True
                WebUtils.DisplayError(msg)

            End If

        End Sub

        ''' <summary>
        ''' Populates the screen with a DomIsAllowanceComponent.
        ''' </summary>
        ''' <param name="comp">The DomIsAllowanceComponent.</param>
        Private Function PopulateDomIsAllowanceComponent(ByVal comp As Target.Abacus.Library.DataClasses.DomISAllowanceComponents) As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                ' populate drop downs first
                msg = PopulatePremiumDropDown()
                If Not msg.Success Then Return msg

                msg = PopulatePremiumRecipientDropDown()
                If Not msg.Success Then Return msg

                ' set properties on controls using properties of form
                ' they will handle all formatting etc
                If Not comp Is Nothing Then
                    ' if we have a comp then we are working with an existing one

                    With comp
                        ComponentFromDate = .DateFrom
                        ComponentToDate = .DateTo
                        ComponentPremium = .BenefitTypeID
                        ComponentPremiumRecipient = .RecipientID
                        ComponentAssessedAs = .AssessedAs
                        ComponentFromAge = .AgeFrom
                        ComponentToAge = .AgeTo
                        ComponentAllowance = .Allowance
                    End With

                Else
                    ' else we have no comp so we are working with a new/deleteed one, so setup defaults

                    ComponentFromDate = Nothing
                    ComponentToDate = Nothing
                    ComponentPremium = 0
                    ComponentPremiumRecipient = 0
                    ComponentAssessedAs = _DefaultAssessedAsValue
                    ComponentFromAge = 0
                    ComponentToAge = 0
                    ComponentAllowance = 0

                End If

                ' setup success error message
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Setups the validators for this screen.
        ''' </summary>
        Private Sub SetupValidators()

            ' setup txtFromAge validators
            With txtFromAge
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.IntegerFormat
                .MinimumValue = 0
                .MaximumValue = _MaxAge.ToString()
                .SetupRangeValidator()
            End With

            ' setup txtToAge validators
            With txtToAge
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.IntegerFormat
                .MinimumValue = 0
                .MaximumValue = _MaxAge.ToString()
                .SetupRangeValidator()
            End With

            ' setup txtAllowance validators
            With txtAllowance
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 1
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

        End Sub

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(ConstantsManager.GetConstant(_WebNavMenuItemKey), _PageTitle)

            ' setup the standard buttons control
            With StandardButtonsControl
                .AllowBack = True
                .AllowDelete = UserHasDeleteCommand
                .AllowEdit = UserHasEditCommand
                .AllowFind = True
                .AllowNew = UserHasAddNewCommand
                .AuditLogTableNames.Add(_AuditLogTable)
                .EditableControls.Add(fsAllowanceControls.Controls)
                .EditableControls.Add(fsAssessedAsControls.Controls)
                .EditableControls.Add(fsDetailControls.Controls)
                .GenericFinderTypeID = GenericFinderType.NonResidentialIsAllowanceComponents
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant(_ReportsSimpleListID)
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
                AddHandler .EditClicked, AddressOf EditClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
            End With

            ' setup validators i.e. ranges
            SetupValidators()

            AddJQuery()
        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            cboPremium.RequiredValidator.Enabled = True
            cboPremiumRecipient.RequiredValidator.Enabled = True

        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons CancelClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons DeleteClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            Try

                ' try and delete the comp, throw an error if cant
                msg = AnnualParametersBL.DeleteDomIsAllowanceComponent(DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)
                If Not msg.Success Then

                    e.Cancel = True
                    WebUtils.DisplayError(msg)

                End If

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)
                If Not msg.Success Then

                    e.Cancel = True
                    WebUtils.DisplayError(msg)

                End If

            End Try

        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons EditClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons FindClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            PopulateDomIsAllowanceComponent(e)

        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons NewClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons SaveClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            Try

                Dim comp As Target.Abacus.Library.DataClasses.DomISAllowanceComponents = GetDomIsAllowanceComponentFromScreen()

                If IsPostBack Then
                    ' disable the type validator

                    cboPremium.RequiredValidator.Enabled = False
                    cboPremiumRecipient.RequiredValidator.Enabled = False

                End If

                If IsValid Then
                    ' if this screen has valid controls continue

                    ' save the benefit comp
                    msg = AnnualParametersBL.SaveDomIsAllowanceComponent(DbConnection, comp)

                    If Not msg.Success Then
                        ' if didnt succeed then check why and display error

                        If msg.Number = AnnualParametersBL.ErrorCannotSaveNonResidentialIsAllowanceComponent Then
                            ' a validation error of some sort occurred so display it

                            lblError.Text = msg.Message

                        Else
                            ' another type of error occurred so display it hard

                            WebUtils.DisplayError(msg)

                        End If

                        ' cancel remaining processing
                        e.Cancel = True

                    Else
                        ' save succeeded so set the id and find again from db

                        e.ItemID = comp.ID
                        FindClicked(e)

                    End If

                Else
                    ' else screen is not valid

                    e.Cancel = True

                End If

            Catch ex As Exception
                ' catch the exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)
                If Not msg.Success Then

                    e.Cancel = True
                    WebUtils.DisplayError(msg)

                End If

            End Try

        End Sub

#End Region


#Region " Use JQuery "
        Private Sub AddJQuery()

            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in the table filter library 
            UseJqueryTableFilter = True

            ' add the table scroller library as we might have large amounts of data
            UseJqueryTableScroller = True

            ' add the searchable menu
            UseJquerySearchableMenu = True

            ' add the jquery tooltip
            UseJqueryTooltip = True

            UseJqueryTemplates = True
        End Sub
#End Region
    End Class

End Namespace
