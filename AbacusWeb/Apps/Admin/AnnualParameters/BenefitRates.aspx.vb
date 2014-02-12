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
    ''' Web Form for Maintaining Fairer Contribution Benefit Rate Parameters
    ''' </summary>
    ''' <history>
    ''' MikeVO   20/04/2011 SDS issue #588 - sensible limits on range validators.
    ''' PaulW    14/01/2011 D11807A - Updated - SDS Issue 459 - Issue Saving Stat Disregard
    ''' ColinD   07/12/2010 D11807A - Updated - SDS Issue 407 - Issue adding new records
    ''' ColinD   15/11/2010 D11807A - Created
    ''' </history>
    Partial Public Class BenefitRates
        Inherits BasePage

#Region "Fields"

        ' locals
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _FetchedBenefitRates As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.BenefitRates)

        ' constants
        Private Const _AuditLogTable As String = "BenefitRates"
        Private Const _CurrencyFormat As String = "N2"
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _DropDownListDefaultText As String = "Please select..."
        Private Const _DropDownListDefaultValue As String = ""
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber
        Private Const _PageTitle As String = "Benefit Rates"
        Private Const _ReportsSimpleListID As String = "AbacusIntranet.WebReport.BenefitRates"
        Private Const _WebCmdAddNewKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.BenefitRateParameters.AddNew"
        Private Const _WebCmdDeleteKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.BenefitRateParameters.Delete"
        Private Const _WebCmdEditKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.BenefitRateParameters.Edit"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Income.ReferenceData.BenefitRateParameters"

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

#Region "Benefit Control Properties"

        ''' <summary>
        ''' Gets or sets the benefit type from the aspx page.
        ''' </summary>
        ''' <value>The benefit.</value>
        Private Property BenefitBenefit() As Integer
            Get
                Return Target.Library.Utils.ToInt32(cboBenefit.GetPostBackValue())
            End Get
            Set(ByVal value As Integer)

                If cboBenefit.DropDownList.SelectedItem IsNot Nothing Then
                    ' if we have a selected item then unselect it

                    cboBenefit.DropDownList.SelectedItem.Selected = False

                End If

                If cboBenefit.DropDownList.Items.FindByValue(value) Is Nothing AndAlso value > 0 Then
                    ' if we cant find the item in the list then it is most probably redundant so fetch from db

                    Dim benefit As Target.Abacus.Library.DataClasses.BenefitDescription = Nothing
                    Dim msg As New ErrorMessage()

                    ' get the item from db and display error if not successful
                    msg = AnnualParametersBL.GetBenefitDescription(DbConnection, value, benefit)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' add the item into the list
                    cboBenefit.DropDownList.Items.Add(New ListItem(benefit.Description, benefit.ID))

                End If

                If cboBenefit.DropDownList.Items.FindByValue(value) IsNot Nothing Then
                    ' if the item exists

                    cboBenefit.DropDownList.Items.FindByValue(value).Selected = True

                Else
                    ' else the item doesn't exist so select default value...should always exist

                    cboBenefit.DropDownList.Items.FindByValue(_DropDownListDefaultValue).Selected = True

                End If

            End Set
        End Property

        ''' <summary>
        ''' Gets or description.
        ''' </summary>
        ''' <value>Description.</value>
        Private Property BenefitDescription() As String
            Get
                Return txtDescription.Text
            End Get
            Set(ByVal value As String)
                txtDescription.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets from date.
        ''' </summary>
        ''' <value>From date.</value>
        Private Property BenefitFromDate() As Nullable(Of DateTime)
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
        ''' Gets or sets Maximum Reasonable Amount.
        ''' </summary>
        ''' <value>Maximum Reasonable Amount.</value>
        Private Property BenefitMaximumReasonableAmount() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtMaxReasonableAmount.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtMaxReasonableAmount.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to Maximum Taken Into Account for Non-Residential.
        ''' </summary>
        ''' <value>Maximum Taken Into Account for Non-Residential.</value>
        Private Property BenefitMaximumTakenIntoAccountNonResidential() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtMaxTakenIntoAccountNonRes.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtMaxTakenIntoAccountNonRes.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Percentage Disregard for Non-Residential.
        ''' </summary>
        ''' <value>Percentage Disregard for Non-Residential.</value>
        Private Property BenefitPercentageDisregardNonResidential() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtPercentageDisregardNonRes.Text)
            End Get
            Set(ByVal value As Decimal)
                txtPercentageDisregardNonRes.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Percentage Disregard for Residential.
        ''' </summary>
        ''' <value>Percentage Disregard for Residential.</value>
        Private Property BenefitPercentageDisregardResidential() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtPercentageDisregardRes.Text)
            End Get
            Set(ByVal value As Decimal)
                txtPercentageDisregardRes.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to date.
        ''' </summary>
        ''' <value>To date.</value>
        Private Property BenefitToDate() As Nullable(Of DateTime)
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
        ''' Gets or sets to rate.
        ''' </summary>
        ''' <value>Rate.</value>
        Private Property BenefitRate() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtRate.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtRate.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to statutory disregard.
        ''' </summary>
        ''' <value>Statutory Disregard.</value>
        Private Property BenefitStatutoryDisregard() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtStatutoryDisregard.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtStatutoryDisregard.Text = value.ToString(_CurrencyFormat)
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
        ''' Gets or sets the fetched benefit rates.
        ''' </summary>
        ''' <value>The fetched benefit rates.</value>
        Private ReadOnly Property FetchedBenefitRates() As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.BenefitRates)
            Get
                If _FetchedBenefitRates Is Nothing Then
                    ' always init the dictionary if null
                    _FetchedBenefitRates = New Dictionary(Of Integer, Target.Abacus.Library.DataClasses.BenefitRates)
                End If
                Return _FetchedBenefitRates
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
        ''' Gets the benefit rate.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetBenefitRate(ByVal id As Integer) As Target.Abacus.Library.DataClasses.BenefitRates

            Dim item As Target.Abacus.Library.DataClasses.BenefitRates = Nothing
            Dim msg As New ErrorMessage()

            If FetchedBenefitRates.ContainsKey(id) Then
                ' if already fetched then return that object

                item = FetchedBenefitRates(id)

            Else
                ' else not already fetched so get the item from db

                msg = AnnualParametersBL.GetBenefitRate(DbConnection, id, AuditLogUserName, AuditLogTitle, item)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' add the item to the dictionary for further use
                FetchedBenefitRates.Add(id, item)

            End If

            Return item

        End Function

        ''' <summary>
        ''' Gets the benefit rate from screen.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetBenefitRateFromScreen() As Target.Abacus.Library.DataClasses.BenefitRates

            Dim msg As New ErrorMessage()
            Dim rate As Target.Abacus.Library.DataClasses.BenefitRates = Nothing
            Dim itemId As Integer = StandardButtonsControl.SelectedItemID

            ' populate the benefit descriptions
            msg = PopulateBenefitDescriptions()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If itemId > 0 Then
                ' if we are modifying an existing rate then fetch db copy and make changes to that

                rate = GetBenefitRate(itemId)

            Else
                ' else item is new so create new object

                rate = New Target.Abacus.Library.DataClasses.BenefitRates(DbConnection, AuditLogUserName, AuditLogTitle)

            End If

            ' setup values on the rate from the screen
            With rate
                .BenefitDescID = BenefitBenefit
                .DateFrom = BenefitFromDate.Value
                .DateTo = BenefitToDate.Value
                .Description = BenefitDescription
                .Rate = BenefitRate
                .StatDisregard = BenefitStatutoryDisregard
                .MaxTakenIntoAccount = BenefitMaximumTakenIntoAccountNonResidential
                .MaxReasonableAmount = BenefitMaximumReasonableAmount
                .PercentageDisregardedDom = BenefitPercentageDisregardNonResidential
                .PercentageDisregardedRes = BenefitPercentageDisregardResidential
                ' fairer contribution is always true from this form
                .FairerContribution = TriState.True
                .AgeFrom = 0.0
                .AgeTo = 0.0
                .LondonWeighting = 0.0
                .ResidentialAllowance = 0.0
            End With

            ' reset the benefit to whatever the postback value was
            BenefitBenefit = rate.BenefitDescID

            Return rate

        End Function

        ''' <summary>
        ''' Populates the benefit descriptions.
        ''' </summary>
        Private Function PopulateBenefitDescriptions() As ErrorMessage

            Dim items As BenefitDescriptionCollection = Nothing
            Dim msg As New ErrorMessage()

            Try

                ' always clear all items from the list first
                cboBenefit.DropDownList.Items.Clear()

                ' get non redundant items from db
                msg = AnnualParametersBL.GetBenefitDescriptions(DbConnection, TriState.False, items)
                If Not msg.Success Then Return msg

                If Not items Is Nothing AndAlso items.Count > 0 Then
                    'if we have some items to add to drop down then do so

                    With cboBenefit.DropDownList
                        .DataTextField = "Description"
                        .DataValueField = "ID"
                        ' order items by description using LINQ
                        .DataSource = items.ToArray().OrderBy(Function(benDesc As BenefitDescription) benDesc.Description)
                        .DataBind()
                    End With

                End If

                ' add default item in to dd
                cboBenefit.DropDownList.Items.Insert(0, New ListItem(_DropDownListDefaultText, _DropDownListDefaultValue))

                ' setup success error message
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Populates the screen with a benefit rate using its id.
        ''' </summary>
        ''' <param name="e">The id.</param>
        Private Sub PopulateBenefitRate(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            If e.ItemID > 0 Then
                ' if we have an id to work with then do so

                ' populate the item on screen
                msg = PopulateBenefitRate(GetBenefitRate(e.ItemID))

            Else
                ' else no id so throw an error

                msg = PopulateBenefitRate(CType(Nothing, Target.Abacus.Library.DataClasses.BenefitRates))

            End If

            If Not msg.Success Then
                ' if errored populating screen display error

                e.Cancel = True
                WebUtils.DisplayError(msg)

            End If

        End Sub

        ''' <summary>
        ''' Populates the screen with a benefit rate.
        ''' </summary>
        ''' <param name="rate">The rate.</param>
        Private Function PopulateBenefitRate(ByVal rate As Target.Abacus.Library.DataClasses.BenefitRates) As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                ' populate ben descriptions first so we can select as required in properties
                msg = PopulateBenefitDescriptions()
                If Not msg.Success Then Return msg

                ' set properties on controls using properties of form
                ' they will handle all formatting etc
                If Not rate Is Nothing Then
                    ' if we have a rate then we are working with an existing one

                    BenefitBenefit = rate.BenefitDescID
                    BenefitFromDate = rate.DateFrom
                    BenefitToDate = rate.DateTo
                    BenefitDescription = rate.Description
                    BenefitRate = rate.Rate
                    BenefitStatutoryDisregard = rate.StatDisregard
                    BenefitMaximumTakenIntoAccountNonResidential = rate.MaxTakenIntoAccount
                    BenefitMaximumReasonableAmount = rate.MaxReasonableAmount
                    BenefitPercentageDisregardNonResidential = rate.PercentageDisregardedDom
                    BenefitPercentageDisregardResidential = rate.PercentageDisregardedRes

                Else
                    ' else we have no rate so we are working with a new/deleteed one, so setup defaults

                    BenefitBenefit = 0
                    BenefitFromDate = Nothing
                    BenefitToDate = Nothing
                    BenefitDescription = ""
                    BenefitRate = 0
                    BenefitStatutoryDisregard = 0
                    BenefitMaximumTakenIntoAccountNonResidential = 0
                    BenefitMaximumReasonableAmount = 0
                    BenefitPercentageDisregardNonResidential = 0
                    BenefitPercentageDisregardResidential = 0

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

            ' setup txtRate validators
            With txtRate
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtStatutoryDisregard validators
            With txtStatutoryDisregard
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtMaxTakenIntoAccountNonRes validators
            With txtMaxTakenIntoAccountNonRes
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtMaxReasonableAmount validators
            With txtMaxReasonableAmount
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtPercentageDisregardNonRes validators
            With txtPercentageDisregardNonRes
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 100
                .SetupRangeValidator()
            End With

            ' setup txtPercentageDisregardRes validators
            With txtPercentageDisregardRes
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 100
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
                .EditableControls.Add(fsBenefitControls.Controls)
                .SearchBy.Add("Benefit", "Benefit")
                .GenericFinderTypeID = GenericFinderType.BenefitRates
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

            cboBenefit.RequiredValidator.Enabled = True

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
                msg = AnnualParametersBL.DeleteBenefitRate(DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)
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

            PopulateBenefitRate(e)

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

                Dim rate As Target.Abacus.Library.DataClasses.BenefitRates = GetBenefitRateFromScreen()

                If IsPostBack Then
                    ' disable the benefit validator

                    cboBenefit.RequiredValidator.Enabled = False

                End If

                If IsValid Then
                    ' if this screen has valid controls continue

                    ' save the benefit rate
                    msg = AnnualParametersBL.SaveBenefitRate(DbConnection, rate)

                    If Not msg.Success Then
                        ' if didnt succeed then check why and display error

                        If msg.Number = AnnualParametersBL.ErrorCannotSaveBenefitRate Then
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

                        e.ItemID = rate.ID
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
