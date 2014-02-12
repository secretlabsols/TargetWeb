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
    ''' Web Form for Maintaining Fairer Contribution Outgoing Maximum Amounts
    ''' </summary>
    ''' <history>
    ''' MikeVO   20/04/2011 SDS issue #588 - sensible limits on range validators.
    ''' ColinD   15/11/2010 D11807A - Created
    ''' </history>
    Partial Public Class OutgoingMaxAmounts
        Inherits BasePage

#Region "Fields"

        ' locals
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _FetchedMaximumOutgoingAmounts As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.OutgoingMaxAmounts) = Nothing

        ' constants
        Private Const _AuditLogTable As String = "OutgoingMaxAmounts"
        Private Const _CurrencyFormat As String = "N2"
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _DropDownListDefaultText As String = "Please select..."
        Private Const _DropDownListDefaultValue As String = ""
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber
        Private Const _PageTitle As String = "Outgoing Maximum Amounts"
        Private Const _ReportsSimpleListID As String = "AbacusIntranet.WebReport.OutgoingMaximumAmounts"
        Private Const _WebCmdAddNewKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.OutgoingMaximumAmounts.AddNew"
        Private Const _WebCmdDeleteKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.OutgoingMaximumAmounts.Delete"
        Private Const _WebCmdEditKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.OutgoingMaximumAmounts.Edit"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Income.ReferenceData.OutgoingMaximumAmounts"

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

#Region "Outgoing Control Properties"

        ''' <summary>
        ''' Gets or sets from date.
        ''' </summary>
        ''' <value>From date.</value>
        Private Property OutgoingFromDate() As Nullable(Of DateTime)
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
        ''' Gets or sets the outgoing description of the outgoing maximum amount.
        ''' </summary>
        ''' <value>The outgoing description.</value>
        Private Property OutgoingDescription() As Integer
            Get
                Return Target.Library.Utils.ToInt32(cboOutgoing.GetPostBackValue())
            End Get
            Set(ByVal value As Integer)

                If cboOutgoing.DropDownList.SelectedItem IsNot Nothing Then
                    ' if we have a selected item then unselect it

                    cboOutgoing.DropDownList.SelectedItem.Selected = False

                End If

                If cboOutgoing.DropDownList.Items.FindByValue(value) Is Nothing AndAlso value > 0 Then
                    ' if we cant find the item in the list then it is most probably redundant so fetch from db

                    Dim outgoing As Target.Abacus.Library.DataClasses.OutgoingDescription = Nothing
                    Dim msg As New ErrorMessage()

                    ' get the item from db and display error if not successful
                    msg = AnnualParametersBL.GetOutgoingDescription(DbConnection, value, outgoing)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' add the item into the list
                    cboOutgoing.DropDownList.Items.Add(New ListItem(outgoing.Description, outgoing.ID))

                End If

                If cboOutgoing.DropDownList.Items.FindByValue(value) IsNot Nothing Then
                    ' if the item exists

                    cboOutgoing.DropDownList.Items.FindByValue(value).Selected = True

                Else
                    ' else the item doesn't exist so select first item

                    cboOutgoing.DropDownList.Items(0).Selected = True

                End If

            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Maximum Reasonable Amount for Non-Residential.
        ''' </summary>
        ''' <value>Maximum Reasonable Amount for Non-Residential.</value>
        Private Property OutgoingMaximumReasonableAmountNonResidential() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtMaximumReasonableAmountNonRes.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtMaximumReasonableAmountNonRes.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Maximum Reasonable Amount for Residential.
        ''' </summary>
        ''' <value>Maximum Reasonable Amount for Residential.</value>
        Private Property OutgoingMaximumReasonableAmountResidential() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtMaximumReasonableAmountRes.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtMaximumReasonableAmountRes.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Maximum Taken Into Account for Non-Residential.
        ''' </summary>
        ''' <value>Maximum Taken Into Account for Non-Residential.</value>
        Private Property OutgoingMaximumTakenIntoAccountNonResidential() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtMaximumTakenIntoAccountNonRes.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtMaximumTakenIntoAccountNonRes.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Maximum Taken Into Account for Residential.
        ''' </summary>
        ''' <value>Maximum Taken Into Account for Residential.</value>
        Private Property OutgoingMaximumTakenIntoAccountResidential() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtMaximumTakenIntoAccountRes.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtMaximumTakenIntoAccountRes.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to date.
        ''' </summary>
        ''' <value>To date.</value>
        Private Property OutgoingToDate() As Nullable(Of DateTime)
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
        ''' Gets or sets the fetched outgoing maximum amounts.
        ''' </summary>
        ''' <value>The fetched dom is allowance components.</value>
        Private ReadOnly Property FetchedMaximumOutgoingAmounts() As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.OutgoingMaxAmounts)
            Get
                If _FetchedMaximumOutgoingAmounts Is Nothing Then
                    ' always init the dictionary if null
                    _FetchedMaximumOutgoingAmounts = New Dictionary(Of Integer, Target.Abacus.Library.DataClasses.OutgoingMaxAmounts)
                End If
                Return _FetchedMaximumOutgoingAmounts
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
        ''' Gets the OutgoingMaximumAmount.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetOutgoingMaximumAmount(ByVal id As Integer) As Target.Abacus.Library.DataClasses.OutgoingMaxAmounts

            Dim item As Target.Abacus.Library.DataClasses.OutgoingMaxAmounts = Nothing
            Dim msg As New ErrorMessage()

            If FetchedMaximumOutgoingAmounts.ContainsKey(id) Then
                ' if already fetched then return that object

                item = FetchedMaximumOutgoingAmounts(id)

            Else
                ' else not already fetched so get the item from db

                msg = AnnualParametersBL.GetOutgoingMaximumAmount(DbConnection, id, AuditLogUserName, AuditLogTitle, item)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' add the item to the dictionary for further use
                FetchedMaximumOutgoingAmounts.Add(id, item)

            End If

            Return item

        End Function

        ''' <summary>
        ''' Gets the OutgoingMaximumAmount from screen.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetOutgoingMaximumAmountFromScreen() As Target.Abacus.Library.DataClasses.OutgoingMaxAmounts

            Dim msg As New ErrorMessage()
            Dim outgoing As Target.Abacus.Library.DataClasses.OutgoingMaxAmounts = Nothing
            Dim itemId As Integer = StandardButtonsControl.SelectedItemID

            ' populate the outgoing drop down
            msg = PopulateOutgoingDropDown()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If itemId > 0 Then
                ' if we are modifying an existing max amt then fetch db copy and make changes to that

                outgoing = GetOutgoingMaximumAmount(itemId)

            Else
                ' else item is new so create new object

                outgoing = New Target.Abacus.Library.DataClasses.OutgoingMaxAmounts(DbConnection, AuditLogUserName, AuditLogTitle)

            End If

            ' setup values on the outgoing from the screen
            With outgoing
                .OutgoingDescID = OutgoingDescription
                .DateFrom = OutgoingFromDate.Value
                .DateTo = OutgoingToDate.Value
                .MaxTakenIntoAccountDom = OutgoingMaximumTakenIntoAccountNonResidential
                .MaxTakenIntoAccountRes = OutgoingMaximumTakenIntoAccountResidential
                .MaxReasonableAmountDom = OutgoingMaximumReasonableAmountNonResidential
                .MaxReasonableAmountRes = OutgoingMaximumReasonableAmountResidential
                ' fairer contribution is always true from this form
                .FairerContribution = TriState.True
            End With

            ' reset the drop down postback values
            OutgoingDescription = outgoing.OutgoingDescID

            Return outgoing

        End Function

        ''' <summary>
        ''' Populates the outgoing drop down list.
        ''' </summary>
        Private Function PopulateOutgoingDropDown() As ErrorMessage

            Dim items As OutgoingDescriptionCollection = Nothing
            Dim msg As New ErrorMessage()

            Try

                ' always clear all items from the list first
                cboOutgoing.DropDownList.Items.Clear()

                ' get tems from db
                msg = AnnualParametersBL.GetOutgoingDescriptions(DbConnection, TriState.False, items)
                If Not msg.Success Then Return msg

                If Not items Is Nothing AndAlso items.Count > 0 Then
                    'if we have some items to add to drop down then do so

                    With cboOutgoing.DropDownList
                        .DataTextField = "Description"
                        .DataValueField = "ID"
                        ' order items by description using LINQ
                        .DataSource = items.ToArray().OrderBy(Function(item As OutgoingDescription) item.Description)
                        .DataBind()
                    End With

                End If

                ' add default item in to dd
                cboOutgoing.DropDownList.Items.Insert(0, New ListItem(_DropDownListDefaultText, _DropDownListDefaultValue))

                ' setup success error message
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Populates the screen with a OutgoingMaxAmounts using its id.
        ''' </summary>
        ''' <param name="e">The id.</param>
        Private Sub PopulateOutgoingMaximumAmount(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            If e.ItemID > 0 Then
                ' if we have an id to work with then do so

                ' populate the item on screen
                msg = PopulateOutgoingMaximumAmount(GetOutgoingMaximumAmount(e.ItemID))

            Else
                ' else no id so throw an error

                msg = PopulateOutgoingMaximumAmount(CType(Nothing, Target.Abacus.Library.DataClasses.OutgoingMaxAmounts))

            End If

            If Not msg.Success Then
                ' if errored populating screen display error

                e.Cancel = True
                WebUtils.DisplayError(msg)

            End If

        End Sub

        ''' <summary>
        ''' Populates the screen with a OutgoingMaxAmounts.
        ''' </summary>
        ''' <param name="outgoing">The OutgoingMaxAmounts.</param>
        Private Function PopulateOutgoingMaximumAmount(ByVal outgoing As Target.Abacus.Library.DataClasses.OutgoingMaxAmounts) As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                ' populate drop downs first
                msg = PopulateOutgoingDropDown()
                If Not msg.Success Then Return msg

                ' set properties on controls using properties of form
                ' they will handle all formatting etc
                If Not outgoing Is Nothing Then
                    ' if we have a outgoing then we are working with an existing one

                    With outgoing
                        OutgoingDescription = .OutgoingDescID
                        OutgoingFromDate = .DateFrom
                        OutgoingToDate = .DateTo
                        OutgoingMaximumTakenIntoAccountNonResidential = .MaxTakenIntoAccountDom
                        OutgoingMaximumTakenIntoAccountResidential = .MaxTakenIntoAccountRes
                        OutgoingMaximumReasonableAmountNonResidential = .MaxReasonableAmountDom
                        OutgoingMaximumReasonableAmountResidential = .MaxReasonableAmountRes
                    End With

                Else
                    ' else we have no comp so we are working with a new/deleteed one, so setup defaults

                    OutgoingDescription = 0
                    OutgoingFromDate = Nothing
                    OutgoingToDate = Nothing
                    OutgoingMaximumTakenIntoAccountNonResidential = 0
                    OutgoingMaximumTakenIntoAccountResidential = 0
                    OutgoingMaximumReasonableAmountNonResidential = 0
                    OutgoingMaximumReasonableAmountResidential = 0

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

            ' setup txtMaximumReasonableAmountNonRes validators
            With txtMaximumReasonableAmountNonRes
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtMaximumReasonableAmountRes validators
            With txtMaximumReasonableAmountRes
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtMaximumTakenIntoAccountNonRes validators
            With txtMaximumTakenIntoAccountNonRes
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtMaximumTakenIntoAccountRes validators
            With txtMaximumTakenIntoAccountRes
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
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
                .EditableControls.Add(fsMaximumAmountControls.Controls)
                .EditableControls.Add(fsOutgoingControls.Controls)
                .SearchBy.Add("Outgoing", "Outgoing")
                .GenericFinderTypeID = GenericFinderType.OutgoingMaximumAmounts
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

            cboOutgoing.RequiredValidator.Enabled = True

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
                msg = AnnualParametersBL.DeleteOutgoingMaximumAmount(DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)
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

            PopulateOutgoingMaximumAmount(e)

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

                Dim comp As Target.Abacus.Library.DataClasses.OutgoingMaxAmounts = GetOutgoingMaximumAmountFromScreen()

                If IsPostBack Then
                    ' disable the type validator

                    cboOutgoing.RequiredValidator.Enabled = False

                End If

                If IsValid Then
                    ' if this screen has valid controls continue

                    ' save the benefit comp
                    msg = AnnualParametersBL.SaveOutgoingMaximumAmount(DbConnection, comp)

                    If Not msg.Success Then
                        ' if didnt succeed then check why and display error

                        If msg.Number = AnnualParametersBL.ErrorCannotSaveOutgoingMaximumAmount Then
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
