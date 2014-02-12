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
    ''' Web Form for Maintaining Fairer Contribution Non Residential I\S Allowance Rates
    ''' </summary>
    ''' <history>
    ''' MikeVO   20/04/2011 SDS issue #588 - sensible limits on range validators.
    ''' PaulW    15/04/2011 D11990B - SDS Cap Charges for excluded services
    ''' ColinD   07/12/2010 D11807A - Updated - SDS Issue 411 - Upped max age to 999
    ''' ColinD   15/11/2010 D11807A - Created
    ''' </history>
    Partial Public Class NonResidentialIsAllowanceRates
        Inherits BasePage

#Region "Fields"

        ' locals
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _FetchedDomIsAllowanceRates As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.DomISAllowanceRates) = Nothing

        ' constants
        Private Const _AuditLogTable As String = "DomISAllowanceRates"
        Private Const _CurrencyFormat As String = "N2"
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _DropDownListDefaultText As String = "Please select..."
        Private Const _DropDownListDefaultValue As String = ""
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber
        Private Const _LineBreakString As String = "<br />"
        Private Const _MaxAge As Integer = 999
        Private Const _PageTitle As String = "Non-Residential I/S Allowance Rates"
        Private Const _ReportsSimpleListID As String = "AbacusIntranet.WebReport.NonResIsAllowanceRates"
        Private Const _WebCmdAddNewKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.NonResidentialIsAllowanceRates.AddNew"
        Private Const _WebCmdDeleteKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.NonResidentialIsAllowanceRates.Delete"
        Private Const _WebCmdEditKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.NonResidentialIsAllowanceRates.Edit"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Income.ReferenceData.NonResidentialIsAllowanceRates"

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

#Region "Rate Control Properties"

        ''' <summary>
        ''' Gets or sets couple allowance.
        ''' </summary>
        ''' <value>Couple Allowance.</value>
        Private Property RateCoupleAllowance() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAllowanceCouple.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAllowanceCouple.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to from age.
        ''' </summary>
        ''' <value>From Age.</value>
        Private Property RateFromAge() As Integer
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
        Private Property RateFromDate() As Nullable(Of DateTime)
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
        ''' Gets or sets half couple allowance.
        ''' </summary>
        ''' <value>Half Allowance.</value>
        Private Property RateHalfCoupleAllowance() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAllowanceHalfCouple.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAllowanceHalfCouple.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets single allowance.
        ''' </summary>
        ''' <value>Single Allowance.</value>
        Private Property RateSingleAllowance() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAllowanceSingle.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAllowanceSingle.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to age.
        ''' </summary>
        ''' <value>To Age.</value>
        Private Property RateToAge() As Integer
            Get
                Return Target.Library.Utils.ToInt32(txtToAge.Text)
            End Get
            Set(ByVal value As Integer)
                txtToAge.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to date.
        ''' </summary>
        ''' <value>To date.</value>
        Private Property RateToDate() As Nullable(Of DateTime)
            Get
                Return Target.Library.Utils.ToDateTime(txtToDate.Text)
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If Not value Is Nothing AndAlso value.HasValue Then
                    ' if there is a date then display it
                    txtToDate.Text = value.Value.ToString(_DateFormat)
                Else
                    ' else no date so display an empty string
                    txtToDate.Text = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type of the rate.
        ''' </summary>
        ''' <value>The type of the rate.</value>
        Private Property RateType() As Integer
            Get
                Return Target.Library.Utils.ToInt32(cboType.GetPostBackValue())
            End Get
            Set(ByVal value As Integer)

                If cboType.DropDownList.SelectedItem IsNot Nothing Then
                    ' if we have a selected item then unselect it

                    cboType.DropDownList.SelectedItem.Selected = False

                End If

                If cboType.DropDownList.Items.FindByValue(value) Is Nothing AndAlso value > 0 Then
                    ' if we cant find the item in the list then it is most probably redundant so fetch from db

                    Dim type As Target.Abacus.Library.DataClasses.DomISAllowanceTypes = Nothing
                    Dim msg As New ErrorMessage()

                    ' get the item from db and display error if not successful
                    msg = AnnualParametersBL.GetDomIsAllowanceType(DbConnection, value, type)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' add the item into the list
                    cboType.DropDownList.Items.Add(New ListItem(type.Description, type.ID))

                End If

                If cboType.DropDownList.Items.FindByValue(value) IsNot Nothing Then
                    ' if the item exists

                    cboType.DropDownList.Items.FindByValue(value).Selected = True

                Else
                    ' else the item doesn't exist so select default value...should always exist

                    cboType.DropDownList.Items.FindByValue(_DropDownListDefaultValue).Selected = True

                End If

            End Set
        End Property


        ''' <summary>
        ''' Gets or sets DWPMIG half couple allowance.
        ''' </summary>
        ''' <value>Half Allowance.</value>
        Private Property RateDWPMIGHalfCoupleAllowance() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtDWPMIGAllowanceHalfCouple.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtDWPMIGAllowanceHalfCouple.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets DWPMIG couple allowance.
        ''' </summary>
        ''' <value>Couple Allowance.</value>
        Private Property RateDWPMIGCoupleAllowance() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtDWPMIGAllowanceCouple.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtDWPMIGAllowanceCouple.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property


        ''' <summary>
        ''' Gets or sets DWPMIG single allowance.
        ''' </summary>
        ''' <value>Single Allowance.</value>
        Private Property RateDWPMIGSingleAllowance() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtDWPMIGAllowanceSingle.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtDWPMIGAllowanceSingle.Text = value.ToString(_CurrencyFormat)
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
        ''' Gets or sets the fetched dom is allowance rates.
        ''' </summary>
        ''' <value>The fetched dom is allowance rates.</value>
        Private ReadOnly Property FetchedDomIsAllowanceRates() As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.DomISAllowanceRates)
            Get
                If _FetchedDomIsAllowanceRates Is Nothing Then
                    ' always init the dictionary if null
                    _FetchedDomIsAllowanceRates = New Dictionary(Of Integer, Target.Abacus.Library.DataClasses.DomISAllowanceRates)
                End If
                Return _FetchedDomIsAllowanceRates
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
        ''' Gets the DomIsAllowanceRate.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetDomIsAllowanceRate(ByVal id As Integer) As Target.Abacus.Library.DataClasses.DomISAllowanceRates

            Dim item As Target.Abacus.Library.DataClasses.DomISAllowanceRates = Nothing
            Dim msg As New ErrorMessage()

            If FetchedDomIsAllowanceRates.ContainsKey(id) Then
                ' if already fetched then return that object

                item = FetchedDomIsAllowanceRates(id)

            Else
                ' else not already fetched so get the item from db

                msg = AnnualParametersBL.GetDomIsAllowanceRate(DbConnection, id, AuditLogUserName, AuditLogTitle, item)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' add the item to the dictionary for further use
                FetchedDomIsAllowanceRates.Add(id, item)

            End If

            Return item

        End Function

        ''' <summary>
        ''' Gets the DomIsAllowanceRate from screen.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetDomIsAllowanceRateFromScreen() As Target.Abacus.Library.DataClasses.DomISAllowanceRates

            Dim msg As New ErrorMessage()
            Dim rate As Target.Abacus.Library.DataClasses.DomISAllowanceRates = Nothing
            Dim itemId As Integer = StandardButtonsControl.SelectedItemID

            ' populate the benefit descriptions
            msg = PopulateDomIsAllowanceTypes()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If itemId > 0 Then
                ' if we are modifying an existing rate then fetch db copy and make changes to that

                rate = GetDomIsAllowanceRate(itemId)

            Else
                ' else item is new so create new object

                rate = New Target.Abacus.Library.DataClasses.DomISAllowanceRates(DbConnection, AuditLogUserName, AuditLogTitle)

            End If

            ' setup values on the rate from the screen
            With rate
                .DomISAllowanceTypeID = RateType
                .DateFrom = RateFromDate.Value
                .DateTo = RateToDate.Value
                .AgeFrom = RateFromAge
                .AgeTo = RateToAge
                .SingleAllowance = RateSingleAllowance
                .CoupleAllowance = RateCoupleAllowance
                .HalfCoupleAllowance = RateHalfCoupleAllowance
                ' fairer contribution is always true from this form
                .FairerContribution = TriState.True
                .DWPMIGAmount = RateDWPMIGSingleAllowance
                .DWPMIGAmountCouple = RateDWPMIGCoupleAllowance
                .DWPMIGAmountHalfCouple = RateDWPMIGHalfCoupleAllowance
            End With

            ' reset the type to whatever the postback value was
            RateType = rate.DomISAllowanceTypeID

            Return rate

        End Function

        ''' <summary>
        ''' Populates the types drop down list.
        ''' </summary>
        Private Function PopulateDomIsAllowanceTypes() As ErrorMessage

            Dim items As DomISAllowanceTypesCollection = Nothing
            Dim msg As New ErrorMessage()

            Try

                ' always clear all items from the list first
                cboType.DropDownList.Items.Clear()

                ' get non redundant items from db
                msg = AnnualParametersBL.GetDomIsAllowanceTypes(DbConnection, TriState.False, items)
                If Not msg.Success Then Return msg

                If Not items Is Nothing AndAlso items.Count > 0 Then
                    'if we have some items to add to drop down then do so

                    With cboType.DropDownList
                        .DataTextField = "Description"
                        .DataValueField = "ID"
                        ' order items by description using LINQ
                        .DataSource = items.ToArray().OrderBy(Function(item As DomISAllowanceTypes) item.Description)
                        .DataBind()
                    End With

                End If

                ' add default item in to dd
                cboType.DropDownList.Items.Insert(0, New ListItem(_DropDownListDefaultText, _DropDownListDefaultValue))

                ' setup success error message
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Populates the screen with a DomIsAllowanceRate using its id.
        ''' </summary>
        ''' <param name="e">The id.</param>
        Private Sub PopulateDomIsAllowanceRate(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            If e.ItemID > 0 Then
                ' if we have an id to work with then do so

                ' populate the item on screen
                msg = PopulateDomIsAllowanceRate(GetDomIsAllowanceRate(e.ItemID))

            Else
                ' else no id so throw an error

                msg = PopulateDomIsAllowanceRate(CType(Nothing, Target.Abacus.Library.DataClasses.DomISAllowanceRates))

            End If

            If Not msg.Success Then
                ' if errored populating screen display error

                e.Cancel = True
                WebUtils.DisplayError(msg)

            End If

        End Sub

        ''' <summary>
        ''' Populates the screen with a DomIsAllowanceRate.
        ''' </summary>
        ''' <param name="rate">The DomIsAllowanceRate.</param>
        Private Function PopulateDomIsAllowanceRate(ByVal rate As Target.Abacus.Library.DataClasses.DomISAllowanceRates) As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                ' populate types first so we can select as required in properties
                msg = PopulateDomIsAllowanceTypes()
                If Not msg.Success Then Return msg

                ' set properties on controls using properties of form
                ' they will handle all formatting etc
                If Not rate Is Nothing Then
                    ' if we have a rate then we are working with an existing one

                    With rate
                        RateType = .DomISAllowanceTypeID
                        RateFromDate = .DateFrom
                        RateToDate = .DateTo
                        RateFromAge = .AgeFrom
                        RateToAge = .AgeTo
                        RateSingleAllowance = .SingleAllowance
                        RateCoupleAllowance = .CoupleAllowance
                        RateHalfCoupleAllowance = .HalfCoupleAllowance
                        RateDWPMIGSingleAllowance = .DWPMIGAmount
                        RateDWPMIGCoupleAllowance = .DWPMIGAmountCouple
                        RateDWPMIGHalfCoupleAllowance = .DWPMIGAmountHalfCouple
                    End With

                Else
                    ' else we have no rate so we are working with a new/deleteed one, so setup defaults

                    RateType = 0
                    RateFromDate = Nothing
                    RateToDate = Nothing
                    RateFromAge = 0
                    RateToAge = 0
                    RateSingleAllowance = 0
                    RateCoupleAllowance = 0
                    RateHalfCoupleAllowance = 0
                    RateDWPMIGSingleAllowance = 0
                    RateDWPMIGCoupleAllowance = 0
                    RateDWPMIGHalfCoupleAllowance = 0
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
                .MaximumValue = _MaxAge
                .SetupRangeValidator()
            End With

            ' setup txtToAge validators
            With txtToAge
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.IntegerFormat
                .MinimumValue = 0
                .MaximumValue = _MaxAge
                .SetupRangeValidator()
            End With

            ' setup txtAllowanceSingle validators
            With txtAllowanceSingle
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtAllowanceCouple validators
            With txtAllowanceCouple
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtAllowanceHalfCouple validators
            With txtAllowanceHalfCouple
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtDWPMIGAllowanceSingle validators
            With txtDWPMIGAllowanceSingle
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtDWPMIGAllowanceCouple validators
            With txtDWPMIGAllowanceCouple
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtDWPMIGAllowanceHalfCouple validators
            With txtDWPMIGAllowanceHalfCouple
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
                .EditableControls.Add(fsAllowanceControls.Controls)
                .EditableControls.Add(fsDetailControls.Controls)
                .EditableControls.Add(fsDWPMIGRates.Controls)
                .SearchBy.Add("Type", "Type")
                .GenericFinderTypeID = GenericFinderType.NonResidentialIsAllowanceRates
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

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            cboType.RequiredValidator.Enabled = True

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

                ' try and delete the rate, throw an error if cant
                msg = AnnualParametersBL.DeleteDomIsAllowanceRate(DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)
                If Not msg.Success Then

                    e.Cancel = True
                    WebUtils.DisplayError(msg)

                End If

            Catch ex As Exception
                ' shouldnt ever hit here if bl methods are setup correctly but just in case 

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

            PopulateDomIsAllowanceRate(e)

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

                Dim rate As Target.Abacus.Library.DataClasses.DomISAllowanceRates = GetDomIsAllowanceRateFromScreen()

                If IsPostBack Then
                    ' disable the type validator

                    cboType.RequiredValidator.Enabled = False

                End If

                If IsValid Then
                    ' if this screen has valid controls continue

                    ' save the benefit rate
                    msg = AnnualParametersBL.SaveDomISAllowanceRate(DbConnection, rate)

                    If Not msg.Success Then
                        ' if didnt succeed then check why and display error

                        If msg.Number = AnnualParametersBL.ErrorCannotSaveNonResidentialIsAllowanceRate Then
                            ' a validation error of some sort occurred so display it

                            lblError.Text = String.Format("{0}{1}", msg.Message.Replace(vbCrLf, _LineBreakString), _LineBreakString)

                        Else
                            ' another type of error occurred so display it hard

                            WebUtils.DisplayError(msg)

                        End If

                        ' cancel remaining processing
                        e.Cancel = True

                    Else
                        ' save succeeded so set the id and find again from db

                        e.ItemID = rate.ID
                        FindClicked(e)

                    End If

                Else
                    ' else screen is not valid

                    e.Cancel = True

                End If

            Catch ex As Exception
                ' shouldnt ever hit here if bl methods are setup correctly but just in case 

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)
                If Not msg.Success Then

                    e.Cancel = True
                    WebUtils.DisplayError(msg)

                End If

            End Try

        End Sub

#End Region

    End Class

End Namespace
