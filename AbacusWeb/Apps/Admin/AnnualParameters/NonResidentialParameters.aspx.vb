Imports System.Collections.Generic
Imports System.Text
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
    ''' Web Form for Maintaining Fairer Contribution Non Residential Parameters
    ''' </summary>
    ''' <history>
    ''' MikeVO   20/04/2011  SDS issue #588 - sensible limits on range validators.
    ''' ColinD   15/11/2010 D11807A - Created
    ''' </history>
    Partial Public Class NonResidentialParameters
        Inherits BasePage

#Region "Fields"

        ' locals
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _FetchedDomiciliaryParameters As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.DomiciliaryParameters) = Nothing

        ' constants
        Private Const _AuditLogTable As String = "DomiciliaryParameters"
        Private Const _CurrencyFormat As String = "N2"
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber
        Private Const _PageTitle As String = "Non-Residential Parameters"
        Private Const _WebCmdAddNewKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.NonResidentialParameters.AddNew"
        Private Const _WebCmdDeleteKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.NonResidentialParameters.Delete"
        Private Const _WebCmdEditKey As String = "AbacusIntranet.WebNavMenuItemCommand.ReferenceData.NonResidentialParameters.Edit"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Income.ReferenceData.NonResidentialParameters"

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

#Region "Parameter Control Properties"

        ''' <summary>
        ''' Gets or sets from date.
        ''' </summary>
        ''' <value>From date.</value>
        Private Property ParameterFromDate() As Nullable(Of DateTime)
            Get
                Return Target.Library.Utils.ToDateTime(txtDetailsFromDate.Text)
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If Not value Is Nothing AndAlso value.HasValue Then
                    ' if there is a date then display it
                    txtDetailsFromDate.Text = value.Value.ToString(_DateFormat)
                Else
                    ' else no date so display an empty string
                    txtDetailsFromDate.Text = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to date.
        ''' </summary>
        ''' <value>To date.</value>
        Private Property ParameterToDate() As Nullable(Of DateTime)
            Get
                Return Target.Library.Utils.ToDateTime(txtDetailsToDate.Text)
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If Not value Is Nothing AndAlso value.HasValue Then
                    ' if there is a date then display it
                    txtDetailsToDate.Text = value.Value.ToString(_DateFormat)
                Else
                    ' else no date so display an empty string
                    txtDetailsToDate.Text = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets minimum charge.
        ''' </summary>
        ''' <value>Minimum Charge.</value>
        Private Property ParameterMinimumCharge() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtDetailsMinCharge.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtDetailsMinCharge.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets maximum charge.
        ''' </summary>
        ''' <value>Maximum Charge.</value>
        Private Property ParameterMaximumCharge() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtDetailsMaxCharge.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtDetailsMaxCharge.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Auto Calculate Is Allowance.
        ''' </summary>
        ''' <value>Auto Calculate Is Allowance.</value>
        Private Property ParameterAutoCalculateIsAllowance() As Boolean
            Get
                Return cbAssessingClientAutoCalculateIsAllowance.Checked
            End Get
            Set(ByVal value As Boolean)
                cbAssessingClientAutoCalculateIsAllowance.Checked = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Do Not Apply Minimum ChargeWhen Calculating Weekly Charge.
        ''' </summary>
        ''' <value>Do Not Apply Minimum ChargeWhen Calculating Weekly Charge.</value>
        Private Property ParameterDoNotApplyMinimumChargeWhenCalculatingWeeklyCharge() As Boolean
            Get
                Return cbCalculatingWcDoNotApplyMinCharge.Checked
            End Get
            Set(ByVal value As Boolean)
                cbCalculatingWcDoNotApplyMinCharge.Checked = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Cap Unit Cost Of Service At Provider Charge.
        ''' </summary>
        ''' <value>Cap Unit Cost Of Service At Provider Charge.</value>
        Private Property ParameterCapUnitCostOfServiceAtProviderCharge() As Boolean
            Get
                Return cbCalculatingWcCapUnitCostOfServiceAtProvCharge.Checked
            End Get
            Set(ByVal value As Boolean)
                cbCalculatingWcCapUnitCostOfServiceAtProvCharge.Checked = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Mig 18 to 24.
        ''' </summary>
        ''' <value>Mig 18 to 24.</value>
        Private Property ParameterMig18to24() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAssessableMig18to24.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAssessableMig18to24.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Mig 25 to 59.
        ''' </summary>
        ''' <value>Mig 18 to 24.</value>
        Private Property ParameterMig25to29() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAssessableMig25to59.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAssessableMig25to59.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Mig 60 upwards.
        ''' </summary>
        ''' <value>Mig 60 upwards.</value>
        Private Property ParameterMig60Upwards() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAssessableMig60Up.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAssessableMig60Up.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Percentage Assessable Income.
        ''' </summary>
        ''' <value>Percentage Assessable Income.</value>
        Private Property ParameterPercentageAssessableIncome() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAssessablePercentAssessableIncome.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAssessablePercentAssessableIncome.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Pensioner Couple.
        ''' </summary>
        ''' <value>Pensioner Couple.</value>
        Private Property ParameterPensionerCouple() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAssessablePensionerCouple.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAssessablePensionerCouple.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Disabled Couple.
        ''' </summary>
        ''' <value>Disabled Couple.</value>
        Private Property ParameterDisabledCouple() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAssessableDisabledCouple.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAssessableDisabledCouple.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Dre Allowance.
        ''' </summary>
        ''' <value>Dre Allowance.</value>
        Private Property ParameterDreAllowance() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAssessableDreAllowance.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAssessableDreAllowance.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Dre Allowance Couple.
        ''' </summary>
        ''' <value>Dre Allowance Couple.</value>
        Private Property ParameterDreAllowanceCouple() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtAssessableDreAllowanceCouple.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtAssessableDreAllowanceCouple.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Dre Upper Limit.
        ''' </summary>
        ''' <value>Dre Upper Limit.</value>
        Private Property ParameterDreUpperLimit() As Decimal
            Get
                If String.IsNullOrEmpty(txtAssessableDreUpperLimit.Text) OrElse txtAssessableDreUpperLimit.Text.Trim().Length = 0 Then
                    Return 999999
                Else
                    Return Target.Library.Utils.ToDecimal(txtAssessableDreUpperLimit.Text).ToString(_CurrencyFormat)
                End If
            End Get
            Set(ByVal value As Decimal)
                txtAssessableDreUpperLimit.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Disregard Singles.
        ''' </summary>
        ''' <value>Disregard Singles</value>
        Private Property ParameterDisregardSingles() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtCapAllowancesDisgregardSingle.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtCapAllowancesDisgregardSingle.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Disregard Couples.
        ''' </summary>
        ''' <value>Disregard Couples</value>
        Private Property ParameterDisregardCouples() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtCapAllowancesDisgregardCouple.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtCapAllowancesDisgregardCouple.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Upper Limit Singles.
        ''' </summary>
        ''' <value>Upper Limit Singles</value>
        Private Property ParameterUpperLimitSingles() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtCapAllowancesUpperLimitSingle.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtCapAllowancesUpperLimitSingle.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Upper Limit Couples.
        ''' </summary>
        ''' <value>Upper Limit Couples</value>
        Private Property ParameterUpperLimitCouples() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtCapAllowancesUpperLimitCouple.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtCapAllowancesUpperLimitCouple.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Weekly Income.
        ''' </summary>
        ''' <value>Weekly Income</value>
        Private Property ParameterWeeklyIncome() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtCapAllowancesCalculationWeeklyIncome.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtCapAllowancesCalculationWeeklyIncome.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Per Part.
        ''' </summary>
        ''' <value>Per Part</value>
        Private Property ParameterPerPart() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtCapAllowancesCalculationPerPart.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtCapAllowancesCalculationPerPart.Text = value.ToString(_CurrencyFormat)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Interest Rate.
        ''' </summary>
        ''' <value>Interest Rate</value>
        Private Property ParameterInterestRate() As Decimal
            Get
                Return Target.Library.Utils.ToDecimal(txtCapAllowancesCalculationInterestRate.Text).ToString(_CurrencyFormat)
            End Get
            Set(ByVal value As Decimal)
                txtCapAllowancesCalculationInterestRate.Text = value.ToString(_CurrencyFormat)
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
        ''' Gets or sets the fetched domiciliary parameters.
        ''' </summary>
        ''' <value>The fetched fetched domiciliary parameters.</value>
        Private ReadOnly Property FetchedDomiciliaryParameters() As Dictionary(Of Integer, Target.Abacus.Library.DataClasses.DomiciliaryParameters)
            Get
                If _FetchedDomiciliaryParameters Is Nothing Then
                    ' always init the dictionary if null
                    _FetchedDomiciliaryParameters = New Dictionary(Of Integer, Target.Abacus.Library.DataClasses.DomiciliaryParameters)
                End If
                Return _FetchedDomiciliaryParameters
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
        ''' Gets the DomiciliaryParameter.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetDomiciliaryParameter(ByVal id As Integer) As Target.Abacus.Library.DataClasses.DomiciliaryParameters

            Dim item As Target.Abacus.Library.DataClasses.DomiciliaryParameters = Nothing
            Dim msg As New ErrorMessage()

            If FetchedDomiciliaryParameters.ContainsKey(id) Then
                ' if already fetched then return that object

                item = FetchedDomiciliaryParameters(id)

            Else
                ' else not already fetched so get the item from db

                msg = AnnualParametersBL.GetDomiciliaryParameter(DbConnection, id, AuditLogUserName, AuditLogTitle, item)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' add the item to the dictionary for further use
                FetchedDomiciliaryParameters.Add(id, item)

            End If

            Return item

        End Function

        ''' <summary>
        ''' Gets the DomiciliaryParameter from screen.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetDomiciliaryParameterFromScreen() As Target.Abacus.Library.DataClasses.DomiciliaryParameters

            Dim msg As New ErrorMessage()
            Dim param As Target.Abacus.Library.DataClasses.DomiciliaryParameters = Nothing
            Dim itemId As Integer = StandardButtonsControl.SelectedItemID

            If itemId > 0 Then
                ' if we are modifying an existing param then fetch db copy and make changes to that

                param = GetDomiciliaryParameter(itemId)

            Else
                ' else item is new so create new object

                param = New Target.Abacus.Library.DataClasses.DomiciliaryParameters(DbConnection, AuditLogUserName, AuditLogTitle)

            End If

            ' setup values on the param from the screen
            With param
                .DateFrom = ParameterFromDate.Value
                .DateTo = ParameterToDate.Value
                .MinCharge = ParameterMinimumCharge
                .MaxCharge = ParameterMaximumCharge
                .AutoCalcISAllowance = ParameterAutoCalculateIsAllowance
                .DomWeeklyChargeIgnoreMinMax = ParameterDoNotApplyMinimumChargeWhenCalculatingWeeklyCharge
                .DomCapUnitCostAtProviderCharge = ParameterCapUnitCostOfServiceAtProviderCharge
                .StandardAllowance = ParameterMig18to24
                .PercentageIncome = ParameterPercentageAssessableIncome
                .PensionerCoupleAllowance = ParameterPensionerCouple
                .DREAllowance = ParameterDreAllowance
                .MIG2559 = ParameterMig25to29
                .DisabledCoupleAllowance = ParameterDisabledCouple
                .DREAllowanceCouple = ParameterDreAllowanceCouple
                .MIG60plus = ParameterMig60Upwards
                .DREUpperLimit = ParameterDreUpperLimit
                .SavingsDisregard = ParameterDisregardSingles
                .SavingsUpperLimit = ParameterUpperLimitSingles
                .CoupleSavingsDisregard = ParameterDisregardCouples
                .CoupleSavingsUpperLimit = ParameterUpperLimitCouples
                .TariffIncome = ParameterWeeklyIncome
                .TariffPerUnit = ParameterPerPart
                .TariffInterestRate = ParameterInterestRate
                ' always 2 from this form = Use NAI as Assessed Charge regardless of package cost
                .DomAssessmentCalcOption = 2
                ' fairer contribution is always true from this form
                .FairerContribution = TriState.True
            End With

            Return param

        End Function

        ''' <summary>
        ''' Populates the screen with a PopulateDomiciliaryParameter using its id.
        ''' </summary>
        ''' <param name="e">The id.</param>
        Private Sub PopulateDomiciliaryParameter(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            If e.ItemID > 0 Then
                ' if we have an id to work with then do so

                ' populate the item on screen
                msg = PopulateDomiciliaryParameter(GetDomiciliaryParameter(e.ItemID))

            Else
                ' else no id so throw an error

                msg = PopulateDomiciliaryParameter(CType(Nothing, Target.Abacus.Library.DataClasses.DomiciliaryParameters))

            End If

            If Not msg.Success Then
                ' if errored populating screen display error

                e.Cancel = True
                WebUtils.DisplayError(msg)

            End If

        End Sub

        ''' <summary>
        ''' Populates the screen with a DomiciliaryParameter.
        ''' </summary>
        ''' <param name="param">The DomiciliaryParameter.</param>
        Private Function PopulateDomiciliaryParameter(ByVal param As Target.Abacus.Library.DataClasses.DomiciliaryParameters) As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                ' set properties on controls using properties of form
                ' they will handle all formatting etc
                If Not param Is Nothing Then
                    ' if we have a param then we are working with an existing one

                    With param
                        ParameterFromDate = .DateFrom
                        ParameterToDate = .DateTo
                        ParameterMinimumCharge = .MinCharge
                        ParameterMaximumCharge = .MaxCharge
                        ParameterAutoCalculateIsAllowance = .AutoCalcISAllowance
                        ParameterDoNotApplyMinimumChargeWhenCalculatingWeeklyCharge = .DomWeeklyChargeIgnoreMinMax
                        ParameterCapUnitCostOfServiceAtProviderCharge = .DomCapUnitCostAtProviderCharge
                        ParameterMig18to24 = .StandardAllowance
                        ParameterPercentageAssessableIncome = .PercentageIncome
                        ParameterPensionerCouple = .PensionerCoupleAllowance
                        ParameterDreAllowance = .DREAllowance
                        ParameterMig25to29 = .MIG2559
                        ParameterDisabledCouple = .DisabledCoupleAllowance
                        ParameterDreAllowanceCouple = .DREAllowanceCouple
                        ParameterMig60Upwards = .MIG60plus
                        ParameterDreUpperLimit = .DREUpperLimit
                        ParameterDisregardSingles = .SavingsDisregard
                        ParameterUpperLimitSingles = .SavingsUpperLimit
                        ParameterDisregardCouples = .CoupleSavingsDisregard
                        ParameterUpperLimitCouples = .CoupleSavingsUpperLimit
                        ParameterWeeklyIncome = .TariffIncome
                        ParameterPerPart = .TariffPerUnit
                        ParameterInterestRate = .TariffInterestRate
                    End With

                Else
                    ' else we have no param so we are working with a new/deleteed one, so setup defaults

                    ParameterFromDate = Nothing
                    ParameterToDate = Nothing
                    ParameterMinimumCharge = 0
                    ParameterMaximumCharge = 0
                    ParameterAutoCalculateIsAllowance = 0
                    ParameterDoNotApplyMinimumChargeWhenCalculatingWeeklyCharge = 0
                    ParameterCapUnitCostOfServiceAtProviderCharge = 0
                    ParameterMig18to24 = 0
                    ParameterPercentageAssessableIncome = 0
                    ParameterPensionerCouple = 0
                    ParameterDreAllowance = 0
                    ParameterMig25to29 = 0
                    ParameterDisabledCouple = 0
                    ParameterDreAllowanceCouple = 0
                    ParameterMig60Upwards = 0
                    ParameterDreUpperLimit = 999999
                    ParameterDisregardSingles = 0
                    ParameterUpperLimitSingles = 0
                    ParameterDisregardCouples = 0
                    ParameterUpperLimitCouples = 0
                    ParameterWeeklyIncome = 0
                    ParameterPerPart = 0
                    ParameterInterestRate = 0

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
        ''' Setups the java script handlers.
        ''' </summary>
        Private Sub SetupJavaScriptHandlers()

            ' add in js link for handlers
            JsLinks.Add("NonResidentialParameters.js")

            ' setup handlers i.e. on change events client side
            txtCapAllowancesCalculationWeeklyIncome.TextBox.Attributes.Add("onchange", "txtCapAllowancesCalculationWeeklyIncome_Changed()")
            txtCapAllowancesCalculationPerPart.TextBox.Attributes.Add("onchange", "txtCapAllowancesCalculationPerPart_Changed()")
            txtCapAllowancesCalculationInterestRate.TextBox.Attributes.Add("onchange", "txtCapAllowancesCalculationInterestRate_Changed()")

        End Sub

        ''' <summary>
        ''' Setups the validators for this screen.
        ''' </summary>
        Private Sub SetupValidators()

            ' setup txtDetailsMinCharge validators
            With txtDetailsMinCharge
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtDetailsMaxCharge validators
            With txtDetailsMaxCharge
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 9999999.0
                .SetupRangeValidator()
            End With

            ' setup txtAssessableMig18to24 validators
            With txtAssessableMig18to24
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 1
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtAssessablePercentAssessableIncome validators
            With txtAssessablePercentAssessableIncome
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 1
                .MaximumValue = 100
                .SetupRangeValidator()
            End With

            ' setup txtAssessablePensionerCouple validators
            With txtAssessablePensionerCouple
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtAssessableDreAllowance validators
            With txtAssessableDreAllowance
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtAssessableMig25to59 validators
            With txtAssessableMig25to59
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 1
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtAssessableDisabledCouple validators
            With txtAssessableDisabledCouple
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtAssessableDreAllowanceCouple validators
            With txtAssessableDreAllowanceCouple
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtAssessableMig60Up validators
            With txtAssessableMig60Up
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 1
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtAssessableDreUpperLimit validators
            With txtAssessableDreUpperLimit
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtCapAllowancesDisgregardSingle validators
            With txtCapAllowancesDisgregardSingle
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 1
                .MaximumValue = 99999.99
                .SetupRangeValidator()
            End With

            ' setup txtCapAllowancesDisgregardCouple validators
            With txtCapAllowancesDisgregardCouple
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 1
                .MaximumValue = 99999.99
                .SetupRangeValidator()
            End With

            ' setup txtCapAllowancesUpperLimitSingle validators
            With txtCapAllowancesUpperLimitSingle
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 1
                .MaximumValue = 99999.99
                .SetupRangeValidator()
            End With

            ' setup txtCapAllowancesUpperLimitCouple validators
            With txtCapAllowancesUpperLimitCouple
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 1
                .MaximumValue = 99999.99
                .SetupRangeValidator()
            End With

            ' setup txtCapAllowancesCalculationWeeklyIncome validators
            With txtCapAllowancesCalculationWeeklyIncome
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 99.99
                .SetupRangeValidator()
            End With

            ' setup txtCapAllowancesCalculationPerPart validators
            With txtCapAllowancesCalculationPerPart
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 999.99
                .SetupRangeValidator()
            End With

            ' setup txtCapAllowancesCalculationInterestRate validators
            With txtCapAllowancesCalculationInterestRate
                .Format = Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat.CurrencyFormat
                .MinimumValue = 0
                .MaximumValue = 99.99
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
                .EditableControls.Add(fsDomParamControls.Controls)
                .EditableControls.Add(fsWhenAssessingClientControls.Controls)
                .EditableControls.Add(fsWhenCalculatingWeeklyChargeControls.Controls)
                .EditableControls.Add(fsAssessableIncome.Controls)
                .EditableControls.Add(fsCapitalAllowanceDetailControls.Controls)
                .EditableControls.Add(fsCapitalAllowanceCalculationControls.Controls)
                .GenericFinderTypeID = GenericFinderType.NonResidentialParameters
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
                AddHandler .EditClicked, AddressOf EditClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
            End With

            ' setup validators i.e. ranges
            SetupValidators()

            ' setup javascript handlers for controls on this page
            SetupJavaScriptHandlers()

            AddJQuery()
        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim js As New StringBuilder()

            js.AppendFormat("weeklyIncomeTextBoxID = '{0}';", txtCapAllowancesCalculationWeeklyIncome.TextBox.ClientID)
            js.AppendFormat("perPartTextBoxID = '{0}';", txtCapAllowancesCalculationPerPart.TextBox.ClientID)
            js.AppendFormat("interestRateTextBoxID = '{0}';", txtCapAllowancesCalculationInterestRate.TextBox.ClientID)

            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

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

                ' try and delete the dom param, throw an error if cant
                msg = AnnualParametersBL.DeleteDomiciliaryParameter(DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)
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

            PopulateDomiciliaryParameter(e)

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

                Dim param As Target.Abacus.Library.DataClasses.DomiciliaryParameters = GetDomiciliaryParameterFromScreen()

                If IsValid Then
                    ' if this screen has valid controls continue

                    ' save the benefit param
                    msg = AnnualParametersBL.SaveDomiciliaryParameter(DbConnection, param)

                    If Not msg.Success Then
                        ' if didnt succeed then check why and display error

                        If msg.Number = AnnualParametersBL.ErrorCannotSaveNonResidentialParameter Then
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

                        e.ItemID = param.ID
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
