
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.SDS
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Reports
Imports Target.Web.Apps.Reports.Collections
Imports Target.Library.Collections
Imports Target.Library.Web.Extensions.AjaxControlToolkit
Imports Target.Library.LinqExtensions

Namespace Apps.Sds

    ''' <summary>
    ''' Screen used to view/maintain an SDS budget category.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD 06/10/2011  BetaI     #64 - Changes to fix fault where unit of measure is not selected correctly.
    ''' ColinD 06/05/2011  SDS issue #651 - Changes to remove OverridenWeeklyCharge
    ''' ColinD 06/05/2011  SDS issue #650 - rate cap persistence issue.
    ''' MikeVO 20/04/2011  SDS issue #588 - sensible limits on range validators.
    ''' ColinD 20/04/2001  D11990E - Added Max Charge and Additional Cost Charge Cap columns to rates tab.
    ''' MikeVO 08/04/2001  SDS issue 484 - removed rate validation for zero amounts.
    ''' ColinD 24/03/2011  D11884A - Added Finance Codes 1 and 2.
    ''' ColinD 07/02/2011  D11962 - Removal of the Additional Costs tab and any associated code.
    ''' ColinD 04/02/2011  D12008 - Changes to indicate when personal budget percent charged are applied to budget category rates.
    ''' ColinD 21/01/2010  D12007 - Changes to the way in which Rates\Additional costs are saved
    ''' ColinD 08/12/2010  D11964A - Replaced selection of Dom Service Types from drop down to in place selector, other small ui tweaks.
    ''' ColinD 14/10/2010  D11918 - alterations to include new field ServiceCategory on BudgetCategory table
    ''' MikeVO 01/09/2010  Added validation summary.
    ''' JohnF  28/06/2010  D11794 - created
    ''' PaulW  20/07/2010  D11796 - SDS Spend Plans, fixed the screen as you are unable to create new records.
    ''' MikeVO 11/08/2010  Corrected error when deleting.
    ''' MikeVO 13/08/2010  Corrected the display of redundant items in dropdown lists.
    ''' MikeVO 13/08/2010  Corrected persistence of Additional Costs flag between post-backs.
    ''' MikeVO 13/08/2010  Corrected Additional Costs checkbox not setting Budget Category Type.
    ''' MikeVO 13/08/2010  Corrected the availability of Service Type and UoM.
    ''' JohnF  02/09/2010  Disable Date To fields (D11801 - SDS Sharepoint #29)
    ''' JohnF  02/09/2010  Select correct tab after PostBack (D11801 - SDS Sharepoint #2)
    ''' Mo     24/09/2010  Audit Log fixes Issue 34 Sharepoint (D11801)
    ''' JohnF  27/09/2010  SDS #203 (D11794)
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class BudgetCategory
        Inherits Target.Web.Apps.BasePage

#Region " Constants "

        Private Const SESSION_NEW_BUDGET_CATEGORY As String = "NewBudgetCategoryData"

        Const VIEWSTATE_KEY_RATES As String = "RatesList"
        Const VIEWSTATE_KEY_RATES_COUNTER As String = "RatesCounter"
        Const VIEWSTATE_KEY_RATES_REMOVED As String = "RemovedRatesList"

        Const CTRL_PREFIX_RATE_DATE_FROM As String = "rateDateFrom"
        Const CTRL_PREFIX_RATE_DATE_TO As String = "rateDateTo"
        Const CTRL_PREFIX_RATE_EXP_UNIT_RATE As String = "rateExpUnitRate"
        Const CTRL_PREFIX_RATE_INC_UNIT_RATE As String = "rateIncUnitRate"
        Const CTRL_PREFIX_RATE_INC_UNIT_RATE_MAX_CHARGE As String = "rateIncUnitRateMaxCharge"
        Const CTRL_PREFIX_RATE_ADDITIONAL_COST As String = "rateAdditionalCost"
        Const CTRL_PREFIX_RATE_ADDITIONAL_COST_CHARGE_CAP As String = "rateAdditionalCostChargeCap"
        Const CTRL_PREFIX_RATE_USE_ACTUAL_COST As String = "rateUseActualCost"

        Const CTRL_PREFIX_RATE_REMOVE As String = "rateRemove"

        Const UNIQUEID_PREFIX_NEW_RATE As String = "rateN"
        Const UNIQUEID_PREFIX_UPDATE_RATE As String = "rateU"
        Const UNIQUEID_PREFIX_DELETE_RATE As String = "rateD"

        Const CTRL_PREFIX_COST_DATE_FROM As String = "costDateFrom"
        Const CTRL_PREFIX_COST_DATE_TO As String = "costDateTo"
        Const CTRL_PREFIX_COST_WEEKLY_COST As String = "costWeeklyCost"

        Const CTRL_PREFIX_COST_REMOVE As String = "costRemove"

        Const UNIQUEID_PREFIX_NEW_COST As String = "costN"
        Const UNIQUEID_PREFIX_UPDATE_COST As String = "costU"
        Const UNIQUEID_PREFIX_DELETE_COST As String = "costD"

#End Region

#Region " Private variables "

        Private _stdBut As StdButtonsBase
        Private _btnReports As HtmlInputButton = New HtmlInputButton("button")
        Private _budcat As Target.Abacus.Library.DataClasses.BudgetCategory = Nothing
        Private _budcatRates As BudgetCategoryRateCollection = Nothing
        Private _budcatID As Integer
        Private _detailItemStartupJS As StringBuilder = New StringBuilder()
        Private _newRateIDCounter As Integer
        Private _inEditMode As Boolean = False
        Private _budgetCategoryGroupsJS As StringBuilder = Nothing
        Private _PersonalBudgetChargedPeriods As PersonalBudgetPercentageChargedCollection = Nothing
        Private _invoicingMethod As Byte
#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the personal budget charged periods.
        ''' </summary>
        ''' <value>The personal budget charged periods.</value>
        Public ReadOnly Property PersonalBudgetChargedPeriods() As PersonalBudgetPercentageChargedCollection
            Get
                If _PersonalBudgetChargedPeriods Is Nothing Then
                    ' if we havent fetched personal budget charged period then do so

                    Dim msg As New ErrorMessage()

                    ' get list of records
                    msg = PersonalBudgetPercentageChargedBL.FetchList(DbConnection, _PersonalBudgetChargedPeriods, String.Empty, String.Empty, 0)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                End If
                Return _PersonalBudgetChargedPeriods
            End Get
        End Property

#End Region

#Region " StdButtons_AddCustomControls "

        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            With _btnReports
                .ID = "_btnReports"
                .Value = "Reports"
            End With
            controls.Add(_btnReports)

            With CType(cpe, AjaxControlToolkit.CollapsiblePanelExtender)
                .ExpandControlID = _btnReports.ClientID
                .CollapseControlID = .ExpandControlID
                .Collapsed = True
            End With

        End Sub

#End Region

#Region " SetupReports "

        Private Sub SetupReports()

            Dim categoriesReportID As Integer = _
                Utils.ToInt32( _
                    Target.Library.Web.ConstantsManager.GetConstant( _
                        String.Format("{0}.WebReport.BudgetCategories", Me.Settings.CurrentApplication) _
                    ) _
                )

            Dim ratesReportID As Integer = _
                Utils.ToInt32( _
                    Target.Library.Web.ConstantsManager.GetConstant( _
                        String.Format("{0}.WebReport.BudgetCategoryRates", Me.Settings.CurrentApplication) _
                    ) _
                )

            Dim categoriesWithoutServiceTypeReportID As Integer = _
                Utils.ToInt32( _
                    Target.Library.Web.ConstantsManager.GetConstant( _
                        String.Format("{0}.WebReport.BudgetCategoriesWithoutServiceType", Me.Settings.CurrentApplication) _
                    ) _
                )

            '++ If we don't have any reports configured for the current application, hide the relevant controls..
            If categoriesReportID <= 0 AndAlso ratesReportID <= 0 AndAlso categoriesWithoutServiceTypeReportID <= 0 Then
                cpe.Enabled = False
                pnlReports.Visible = False
            Else
                AddHandler _stdBut.AddCustomControls, AddressOf StdButtons_AddCustomControls

                With lstReports
                    .Rows = 4
                    .Attributes.Add("onchange", "lstReports_Change();")
                    With .Items
                        .Add(New ListItem("Simple list of Budget Categories", divCategories.ClientID))
                        .Add(New ListItem("Simple list of Budget Categories without a Service Type", divCategoriesWithoutServiceType.ClientID))
                        .Add(New ListItem("Simple list of Budget Category Rates", divRates.ClientID))
                    End With
                End With

                '++ Budget Categories..
                With CType(ctlCategories, IReportsButton)
                    .ReportID = categoriesReportID
                End With

                '++ Budget Categories without Service Type..
                With CType(ctlCategoriesWithoutServiceType, IReportsButton)
                    .ReportID = categoriesWithoutServiceTypeReportID
                End With

                '++ Budget Category Rates..
                With CType(ctlRates, IReportsButton)
                    .ReportID = ratesReportID
                End With
            End If

        End Sub

#End Region

#Region " Page_Init "

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.BudgetCategories"), "Budget Categories")
            Me.ShowValidationSummary = True

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            SetupReports()

        End Sub

#End Region

#Region " Page_Load "

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim currentUser As WebSecurityUser
            Dim css As StringBuilder = New StringBuilder()
            Dim itemIDs As List(Of String)
            Dim settings As SystemSettings
            Dim settingKey As String = "SDSInvoicingMethod"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.BudgetCategories"), "Budget Categories")

            With Me.JsLinks
                .Add("BudgetCategory.js")
                .Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
                .Add(WebUtils.GetVirtualPath("Library/Javascript/Dialog.js"))
                .Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            End With

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

            With css
                .Append("td.expenditure {background-color:#ffddac;} ")
                .Append("td.income {background-color:#fffea6;} ")
                .Append("td.centre {align:center;} ")
                .Append("td.left {align:left;} ")
                .Append("td.indent {padding-left:20px;} ")
                Me.AddExtraCssStyle(.ToString())
            End With

            currentUser = SecurityBL.GetCurrentUser()

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.BudgetCategories.AddNew"))
                .AllowFind = True
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.BudgetCategories.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.BudgetCategories.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(tabDetails.Controls)
                .EditableControls.Add(tabRates.Controls)
                .GenericFinderTypeID = GenericFinderType.SDS_BudgetCategory
                .AuditLogTableNames.Add("BudgetCategory")
                .AuditLogTableNames.Add("BudgetCategoryAdditionalCost")
                .AuditLogTableNames.Add("BudgetCategoryRate")
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf EditClicked
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
            End With

            Me.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            settings = SystemSettings.GetCachedSettings(Me.DbConnection.ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
            _invoicingMethod = settings(ApplicationName.AbacusIntranet, settingKey)


            If Not IsPostBack Then
                ' if first hit then only show the details tab

                ShowTabRates(False)

            End If

            '++ Re-create the list of Rates (from view state)..
            itemIDs = GetUniqueRateIDsFromViewState()
            For Each id As String In itemIDs
                OutputRatesControls(id, New BudgetCategoryRate(Me.DbConnection, "", ""))
            Next


            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Apps.UserControls.ServiceOrderReports", _
         Target.Library.Web.Utils.WrapClientScript(String.Format("Report_lstReportlistId='{0}';", _
                                                                 lstReports.ClientID _
                                                    )) _
                                            )
        End Sub

#End Region

#Region " ClearViewState "

        ''' <summary>
        ''' Clears any view state items.
        ''' </summary>
        Private Sub ClearViewState()


            ViewState.Remove(VIEWSTATE_KEY_RATES)
            ViewState.Remove(VIEWSTATE_KEY_RATES_COUNTER)

            phRates.Controls.Clear()

        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            '++ Clear any new budget category data from the current session..
            _budcatID = 0

            msg = ClearBudgetCategory()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ShowTabRates(False)
            ClearNewBudgetCategoryData()

            If msg.Success Then msg = PopulateScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

#Region "EditClicked"

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            _inEditMode = True
            FindClicked(e)

        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            _budcatID = e.ItemID
            _budcat = New Target.Abacus.Library.DataClasses.BudgetCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            msg = BudgetCategoryBL.Fetch(Me.DbConnection, e.ItemID, _budcat, _budcatRates, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), currentUser.ExternalUsername)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ClearViewState()

            If msg.Success Then
                msg = PopulateScreen()
            End If
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

            ClearViewState()
            ViewState.Remove(VIEWSTATE_KEY_RATES_REMOVED)

            If e.ItemID = 0 Then
                '++ Item hasn't been created yet, so redirect to the previous page..
                Dim backUrl As String = Request.QueryString("backUrl")
                If String.IsNullOrEmpty(backUrl) = False _
                    AndAlso backUrl.Trim().Length > 0 Then
                    '++ If we have a 'back' url specified, then redirect to it
                    '++ as we don't need to see this page..
                    Response.Redirect(backUrl)
                Else
                    NewClicked(e)
                End If
            Else
                '++ ..is an existing item..
                FindClicked(e)
            End If

        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                msg = BudgetCategoryBL.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If msg.Success Then
                    trans.Commit()
                    msg = New ErrorMessage()
                    msg.Success = True

                    '++ Clear any new budget category data from the current session..
                    _stdBut.SelectedItemID = 0
                    _budcatID = 0
                    _budcatRates = Nothing

                    msg = ClearBudgetCategory()
                    If msg.Success Then
                        ShowTabRates(False)
                        ClearNewBudgetCategoryData()
                    End If

                    If msg.Success Then msg = PopulateScreen()
                End If

            Catch ex As Exception
                msg.Success = False
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
            Finally
                If msg.Success Then
                    lblError.Text = ""
                Else
                    SqlHelper.RollbackTransaction(trans)
                    trans = Nothing
                    lblError.Text = msg.Message
                    e.Cancel = True
                    FindClicked(e)
                End If
            End Try

        End Sub
#End Region

#Region " SaveClicked "

        ''' <summary>
        ''' Saves the budget category and any associated rates\costs
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim costsToDelete As New List(Of Integer)()
            Dim itemIDs As List(Of String) = Nothing
            Dim ratesToDelete As New List(Of Integer)()

            Try

                If e.ItemID > 0 AndAlso _budcat Is Nothing Then
                    ' if we have an item id but no budget category then load it

                    ' fetch the budget category and any associated costs\rates
                    msg = BudgetCategoryBL.Fetch(DbConnection, e.ItemID, _budcat, _budcatRates, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), currentUser.ExternalUsername)
                    If Not msg.Success Then
                        WebUtils.DisplayError(msg)
                        e.Cancel = True
                    End If

                End If

                ' repopulate the screen
                msg = RepopulateScreen()
                If Not msg.Success Then
                    WebUtils.DisplayError(msg)
                    e.Cancel = True
                End If

                Me.Validate("Save")

                If IsValid() Then

                    Dim numberOfReconsiderationRecordsAffected As Integer = 0

                    ' find all the rates marked for deletion and add them into the ratesToDelete collection
                    itemIDs = GetUniqueRateIDsFromViewState()
                    For Each itemID As String In itemIDs
                        If itemID.StartsWith(UNIQUEID_PREFIX_DELETE_RATE) Then
                            ratesToDelete.Add(Utils.ToInt32(itemID.Replace(UNIQUEID_PREFIX_DELETE_RATE, "")))
                        End If
                    Next

                    ' save the budget category and any costs\rates
                    msg = BudgetCategoryBL.Save(DbConnection, _
                                                currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(PageTitle, Settings), _
                                                _budcat, _
                                                costsToDelete, _
                                                ratesToDelete, _
                                                _budcatRates, _
                                                numberOfReconsiderationRecordsAffected)

                    If Not msg.Success Then
                        ' if saving has failed

                        If msg.Number = BudgetCategoryBL.ERROR_NUMBER_BUDGET_CATEGORY_INVALID _
                            OrElse msg.Number = BudgetCategoryBL.ERROR_NUMBER_BUDGET_CATEGORY_RATE_INVALID Then
                            ' if the error reported is a validation error then just display on the screen

                            With lblError
                                .Text = String.Format("{0}<br /><br />", msg.Message)
                                .CssClass = "errorText"
                            End With
                            e.Cancel = True

                        Else
                            ' else it is another error so throw it to generic handler

                            WebUtils.DisplayError(msg)
                            e.Cancel = True

                        End If

                    Else
                        ' else saving was successful

                        If numberOfReconsiderationRecordsAffected > 0 Then
                            ' if we have at least one record affected then display message

                            With lblError
                                .Text = "Changes made to the Budget Category may result in adjustments to historic charges for one or more Service User.<br /><br />"
                                .CssClass = "warningText"
                            End With

                        End If

                        e.ItemID = _budcat.ID
                        _stdBut.SelectedItemID = _budcat.ID
                        FindClicked(e)

                    End If

                End If

            Catch ex As Exception
                ' catch and display any exceptions

                WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))
                e.Cancel = True

            End Try

        End Sub

#End Region

#Region " PopulateScreen "

        Private Function PopulateScreen() As ErrorMessage
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            '++ Header tab..
            _budcatID = _stdBut.SelectedItemID

            If _budcatID > 0 Then
                If _budcat Is Nothing Then
                    _budcat = New Target.Abacus.Library.DataClasses.BudgetCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                End If
                msg = BudgetCategoryBL.Fetch(Me.DbConnection, _budcatID, _budcat, _budcatRates, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), currentUser.ExternalUsername)
                If Not msg.Success Then Return msg
                With _budcat
                    '++ Existing budget category..
                    FillDropdownBudgetCategoryGroup(.BudgetCategoryGroupID)
                    FillDropdownBudgetCategoryType(.Type)
                    CType(txtServiceType, InPlaceServiceTypeSelector).ItemID = .DomServiceTypeID
                    FillDropdownUnitsOfMeasure(.DomUnitsOfMeasureID)
                    txtReference.Text = .Reference
                    txtDescription.Text = .Description
                    chkRedundant.CheckBox.Checked = .Redundant
                    If _budcat.SystemType > BudgetCategoryBL.BudgetCategorySystemType.None Then
                        _stdBut.AllowDelete = False
                        If _budcat.SystemType = BudgetCategoryBL.BudgetCategorySystemType.DirectPaymentBalancing Then
                            _stdBut.AllowEdit = False
                        End If
                    End If
                    txtFinanceCode1.Text = .FinanceCode1
                    txtFinanceCode2.Text = .FinanceCode2
                End With
            Else
                Dim isNewBudgetCategory As Boolean = False
                Dim newBudgetCategory As NewBudgetCategoryData

                If HaveNewBudgetCategoryData() Then
                    '++ Part-way through creating a new budget category..
                    newBudgetCategory = FetchNewBudgetCategoryData()
                    If newBudgetCategory Is Nothing Then newBudgetCategory = New NewBudgetCategoryData()
                    With newBudgetCategory
                        FillDropdownBudgetCategoryGroup(.BudgetCategoryGroupID)
                        FillDropdownBudgetCategoryType(.Type)
                        CType(txtServiceType, InPlaceServiceTypeSelector).ItemID = .DomServiceTypeID
                        FillDropdownUnitsOfMeasure(.DomUnitsOfMeasureID)
                        txtReference.Text = .Reference
                        txtDescription.Text = .Description
                        chkRedundant.CheckBox.Checked = .Redundant
                        txtFinanceCode1.Text = .FinanceCode1
                        txtFinanceCode2.Text = .FinanceCode2
                    End With
                Else
                    '++ Clear the screen ready for a new budget category..
                    newBudgetCategory = New NewBudgetCategoryData()
                    FillDropdownBudgetCategoryGroup(0)
                    FillDropdownBudgetCategoryType()
                    CType(txtServiceType, InPlaceServiceTypeSelector).ItemID = 0
                    FillDropdownUnitsOfMeasure(0)
                    txtReference.Text = ""
                    txtDescription.Text = ""
                    chkRedundant.CheckBox.Checked = False
                    txtFinanceCode1.Text = ""
                    txtFinanceCode2.Text = ""
                    isNewBudgetCategory = True
                End If
            End If

            If _budcat.ID > 0 Then
                tabRates.Enabled = True
                ShowTabRates(True)
                msg = PopulateRates(_budcatRates)
                If Not msg.Success Then Return msg
            Else
                ShowTabRates(False)
            End If

            msg = New ErrorMessage()
            msg.Success = True

            tabStrip.SetActiveTabByHeaderText(hidSelectedTab.Value)
            Return msg

        End Function

#End Region

#Region " RepopulateScreen "

        ''' <summary>
        ''' Repopulates the screen.
        ''' </summary>
        ''' <returns></returns>
        Private Function RepopulateScreen() As ErrorMessage

            Dim msg As ErrorMessage
            Dim newBudCatData As NewBudgetCategoryData
            Dim rates As BudgetCategoryRateCollection = Nothing
            Dim budgetCategoryGroupID As Integer
            Dim reference As String
            Dim description As String
            Dim fixedPrice As Boolean
            Dim categoryType As BudgetCategoryBL.BudgetCategoryType
            Dim domServiceTypeID As Integer
            Dim domUnitsOfMeasureID As Integer
            Dim redundant As Boolean
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim dst As DomServiceType = Nothing
            Dim financeCode1 As String
            Dim financeCode2 As String

            _budcatID = _stdBut.SelectedItemID

            If _budcatID > 0 Then
                '++ ..editing an existing budget category..
                If _budcat Is Nothing Then
                    _budcat = New Target.Abacus.Library.DataClasses.BudgetCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                End If
                msg = BudgetCategoryBL.Fetch(Me.DbConnection, _budcatID, _budcat, _budcatRates, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), currentUser.ExternalUsername)
                If Not msg.Success Then Return msg
                With _budcat
                    budgetCategoryGroupID = .BudgetCategoryGroupID
                    reference = .Reference
                    description = .Description
                    If fixedPrice Then .Type = BudgetCategoryBL.BudgetCategoryType.UnitsOfService
                    categoryType = .Type
                    domServiceTypeID = .DomServiceTypeID
                    domUnitsOfMeasureID = .DomUnitsOfMeasureID
                    redundant = .Redundant
                    financeCode1 = .FinanceCode1
                    financeCode2 = .FinanceCode2
                End With
            Else
                '++ ..creating a new budget category..
                '++ Overwrite any header changes made on the screen..
                newBudCatData = FetchNewBudgetCategoryData()
                If newBudCatData Is Nothing Then newBudCatData = New NewBudgetCategoryData()
                With newBudCatData
                    budgetCategoryGroupID = .BudgetCategoryGroupID
                    reference = .Reference
                    description = .Description
                    If fixedPrice Then .Type = BudgetCategoryBL.BudgetCategoryType.UnitsOfService
                    categoryType = .Type
                    domServiceTypeID = .DomServiceTypeID
                    domUnitsOfMeasureID = .DomUnitsOfMeasureID
                    redundant = .Redundant
                    financeCode1 = .FinanceCode1
                    financeCode2 = .FinanceCode2
                End With
                newBudCatData = StoreNewBudgetCategoryData(newBudCatData, budgetCategoryGroupID, _
                                                           reference, description, fixedPrice, _
                                                           categoryType, domServiceTypeID, _
                                                           domUnitsOfMeasureID, redundant, _
                                                           financeCode1, financeCode2)

                msg = PrimeBudgetCategoryClass()
                If Not msg.Success Then Return msg
            End If

            '++ Overwrite header changes made on the screen..
            With _budcat

                If _budcat.SystemType = BudgetCategoryBL.BudgetCategorySystemType.None Then
                    ' if we have a system type = 0 then this is amendable by the user
                    ' so reset the values from the interface

                    Dim partValue As String, lPos As Integer

                    .BudgetCategoryGroupID = Utils.ToInt32(cboBudgetCategoryGroup.GetPostBackValue())
                    .Reference = txtReference.Text
                    .Description = txtDescription.Text
                    .FinanceCode1 = txtFinanceCode1.Text
                    .FinanceCode2 = txtFinanceCode2.Text

                    If fixedPrice Then
                        .Type = BudgetCategoryBL.BudgetCategoryType.UnitsOfService
                    Else
                        .Type = Utils.ToInt32(cboBudgetCategoryType.GetPostBackValue())
                    End If


                    If _budcat.ID > 0 AndAlso .ServiceCategory = 1 AndAlso .DomServiceTypeID > 0 Then

                        .DomServiceTypeID = .DomServiceTypeID

                    Else

                        .DomServiceTypeID = CType(txtServiceType, InPlaceServiceTypeSelector).ItemID

                    End If

                    If .DomServiceTypeID > 0 Then

                        ' get the service type and set the budget cats service category based on this value
                        dst = New DomServiceType(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                        msg = dst.Fetch(.DomServiceTypeID)
                        If Not msg.Success Then Return msg
                        .ServiceCategory = dst.ServiceCategory

                    Else

                        .ServiceCategory = 0

                    End If

                    '++ Strip off the non-ID stuff from the select item's Value..
                    partValue = hidUnitOfMeasure.Value
                    lPos = partValue.IndexOf("?")
                    If lPos <> -1 Then partValue = partValue.Substring(0, lPos)
                    .DomUnitsOfMeasureID = Utils.ToInt32(partValue)

                    .Redundant = chkRedundant.CheckBox.Checked

                Else

                    ' get the service type and set the budget cats service category based on this value
                    dst = New DomServiceType(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                    msg = dst.Fetch(.DomServiceTypeID)
                    If Not msg.Success Then Return msg

                End If

                If Not dst Is Nothing Then

                    .Permanent = dst.Permanent

                Else

                    .Permanent = TriState.False

                End If

                txtReference.Text = .Reference
                txtDescription.Text = .Description
                cboBudgetCategoryType.DropDownList.SelectedValue = .Type
                chkRedundant.CheckBox.Checked = .Redundant
                txtFinanceCode1.Text = .FinanceCode1
                txtFinanceCode2.Text = .FinanceCode2
                FillDropdownBudgetCategoryGroup(.BudgetCategoryGroupID)
                FillDropdownBudgetCategoryType(.Type)
                CType(txtServiceType, InPlaceServiceTypeSelector).ItemID = .DomServiceTypeID
                FillDropdownUnitsOfMeasure(.DomUnitsOfMeasureID)

                '++ Grab existing related rates with those from the screen..
                ShowTabRates(.Type = BudgetCategoryBL.BudgetCategoryType.UnitsOfService)
                If tabRates.Enabled Then
                    msg = GetRatesFromScreen(_budcatRates)
                    If Not msg.Success Then Return msg
                Else
                    _budcatRates = Nothing
                End If

                '++ Re-populate the screen with the additional costs / rates..
                Dim idRateList As List(Of String) = GetUniqueRateIDsFromViewState()
                Dim newRateList As New List(Of String)
                ClearViewState()
                For Each item As String In idRateList
                    If item.StartsWith(UNIQUEID_PREFIX_DELETE_RATE) Then
                        newRateList.Add(item)
                    End If
                Next
                PersistUniqueRateIDsToViewState(newRateList)

                msg = PopulateRates(_budcatRates)
                If Not msg.Success Then Return msg
            End With

            msg = New ErrorMessage()
            msg.Success = True

            tabStrip.SetActiveTabByHeaderText(hidSelectedTab.Value)
            Return msg

        End Function

#End Region

#Region " GetRatesFromScreen "

        ''' <summary>
        ''' Gets the rates from screen.
        ''' </summary>
        ''' <param name="rates">The rates.</param>
        ''' <returns>Collection of BudgetCategoryRate's</returns>
        Private Function GetRatesFromScreen(ByRef rates As BudgetCategoryRateCollection) As ErrorMessage

            Dim msg As New ErrorMessage
            Dim id As String
            Dim idList As List(Of String)
            Dim idMap As New Dictionary(Of Integer, String)()
            Dim dateFrom As Date, dateFromString As String
            Dim dateTo As Date, dateToString As String
            Dim expUnitRate As Decimal, incUnitRate As Decimal
            Dim line As BudgetCategoryRate = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim recID As Integer = 0
            Dim newIdIndex As Integer = -1
            Dim hasAdditionalCost As Boolean = False
            Dim maxCharge As Decimal = 0
            Dim additionalCostChargeCap As Integer = 0
            Dim useActualCost As Boolean = False

            idList = GetUniqueRateIDsFromViewState()

            If rates Is Nothing Then
                ' if we have null collection then init it...init!

                rates = New BudgetCategoryRateCollection()

            End If

            For Each id In idList
                ' loop each id in the list to persist

                line = Nothing

                If Not id.StartsWith(UNIQUEID_PREFIX_DELETE_RATE) Then
                    ' if the fetched id is not for deletion

                    ' validate that a date from field has been specified correctly
                    dateFromString = CType(phRates.FindControl(CTRL_PREFIX_RATE_DATE_FROM & "H" & id), HiddenField).Value
                    If Utils.IsDate(dateFromString) Then
                        dateFrom = CDate(dateFromString)
                    Else
                        msg.Success = False
                        msg.Message = "A valid Date From must be specified for each Rate item."
                        Return msg
                    End If

                    ' validate that a date to field has been specified
                    dateToString = CType(phRates.FindControl(CTRL_PREFIX_RATE_DATE_TO & "H" & id), HiddenField).Value
                    If dateToString = "" Then
                        dateTo = DataUtils.MAX_DATE
                    ElseIf Utils.IsDate(dateToString) Then
                        dateTo = Convert.ToDateTime(dateToString)
                    Else
                        msg.Success = False
                        msg.Message = "A valid Date To must be specified for each Rate item."
                        Return msg
                    End If

                    ' set rates on record
                    expUnitRate = Utils.ToDecimal(CType(phRates.FindControl(CTRL_PREFIX_RATE_EXP_UNIT_RATE & id), TextBoxEx).Value)
                    useActualCost = CType(phRates.FindControl(CTRL_PREFIX_RATE_USE_ACTUAL_COST & id), CheckBox).Checked
                    incUnitRate = Utils.ToDecimal(CType(phRates.FindControl(CTRL_PREFIX_RATE_INC_UNIT_RATE & id), TextBoxEx).Value)
                    hasAdditionalCost = CType(phRates.FindControl(CTRL_PREFIX_RATE_ADDITIONAL_COST & id), CheckBox).Checked
                    maxCharge = Utils.ToDecimal(CType(phRates.FindControl(CTRL_PREFIX_RATE_INC_UNIT_RATE_MAX_CHARGE & id), TextBoxEx).Value)
                    additionalCostChargeCap = Utils.ToInt32(CType(phRates.FindControl(CTRL_PREFIX_RATE_ADDITIONAL_COST_CHARGE_CAP & id), DropDownListEx).GetPostBackValue())

                    If id.StartsWith(UNIQUEID_PREFIX_NEW_RATE) Then
                        ' if new rate then set the record id to 0

                        recID = 0

                    Else
                        ' else updating rate so set from id

                        recID = Utils.ToInt32(id.Replace(UNIQUEID_PREFIX_UPDATE_RATE, ""))

                    End If

                    If recID > 0 Then
                        ' if we are operating on an existing record then use from current collection

                        ' fetch rate with matching id from collection
                        line = (From tmpRate In rates.ToArray() _
                                    Where tmpRate.ID = recID _
                                        Select tmpRate).FirstOrDefault()

                        If line Is Nothing Then
                            ' if we havent found the line in the collection then fetch and add to collection from db

                            line = New BudgetCategoryRate(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                            msg = line.Fetch(recID)
                            If Not msg.Success Then Return msg
                            rates.Add(line)

                        End If

                        idMap.Add(line.ID, id)

                    Else
                        ' else create a new record

                        line = New BudgetCategoryRate(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        rates.Add(line)
                        line.ID = newIdIndex
                        idMap.Add(line.ID, id)
                        newIdIndex -= 1

                    End If

                    With line
                        .DateFrom = dateFrom
                        .DateTo = dateTo
                        .ExpenditureUnitRate = expUnitRate
                        .UseActualCostForIncome = useActualCost
                        .IncomeUnitRate = incUnitRate
                        If hasAdditionalCost Then
                            .AdditionalCost = TriState.True
                        Else
                            .AdditionalCost = TriState.False
                        End If
                        .MaximumWeeklyCharge = maxCharge
                        .AdditionalCostChargeCap = additionalCostChargeCap
                    End With

                End If

            Next

            If Not rates Is Nothing AndAlso rates.Count > 0 Then
                ' if we have some rates then set date to on object and on screen

                Dim itemIDs As List(Of String) = idList
                Dim ratesToDelete As New List(Of Integer)()
                Dim previousRate As BudgetCategoryRate = Nothing
                Dim ratesToOutput As New List(Of BudgetCategoryRate)()
                Dim ratesIndex As Integer = 0

                ' get deleted items from the list, we do not want to output controls for deleted items
                ratesToDelete = (From item In itemIDs _
                                     Where item.StartsWith(UNIQUEID_PREFIX_DELETE_RATE) _
                                        Select Utils.ToInt32(item.Replace(UNIQUEID_PREFIX_DELETE_RATE, ""))).ToList()

                ' get the rates to output ordered by descending date from
                ratesToOutput = (From tmpRate In rates.ToArray() _
                                    Where (From tmpRateToDelete In ratesToDelete Select tmpRateToDelete).Contains(tmpRate.ID) = False _
                                    Order By tmpRate.DateFrom Descending _
                                 Select tmpRate).ToList()

                For Each bcRate As BudgetCategoryRate In ratesToOutput
                    ' loop each rate to output to screen

                    ' get the id for this cost
                    id = idMap(bcRate.ID)

                    If ratesIndex > 0 Then
                        ' if not the first record then set date to to that of previosu record date from - 1 day

                        previousRate = ratesToOutput(ratesIndex - 1)
                        bcRate.DateTo = previousRate.DateFrom.AddDays(-1)

                    Else
                        ' else first record so set to max date

                        bcRate.DateTo = DataUtils.MAX_DATE

                    End If

                    ' set the date to on the ui
                    CType(phRates.FindControl(CTRL_PREFIX_RATE_DATE_TO & id), TextBoxEx).Text = bcRate.DateTo.ToString("dd/MM/yyyy")
                    CType(phRates.FindControl(CTRL_PREFIX_RATE_DATE_TO & "H" & id), HiddenField).Value = bcRate.DateTo.ToString("dd/MM/yyyy")

                    If bcRate.ID < 0 Then
                        ' reset the id back to 0

                        bcRate.ID = 0

                    End If

                    ratesIndex += 1

                Next

            End If

            msg = New ErrorMessage()
            msg.Success = True

            Return msg

        End Function

#End Region

#Region " PopulateRates "

        ''' <summary>
        ''' Populates the budget category rates on screen.
        ''' </summary>
        ''' <param name="bcRates">The bc rates.</param>
        ''' <returns></returns>
        Private Function PopulateRates(ByVal bcRates As BudgetCategoryRateCollection) As ErrorMessage

            Dim msg As New ErrorMessage()

            If bcRates.Count = 0 Then
                'No rates have been entered yet
                lblWarning.Text = "Rates have not yet been set up for this rate category."
            End If


            ' determine the visibility of the rates tab
            If _budcat.ID > 0 Then

                Dim id As String
                Dim itemIDs As List(Of String) = Nothing
                Dim ratesToDelete As New List(Of Integer)()

                ' alter the visibility of the tabs
                pnlRates.Visible = True

                ' get any items from viewstate to populate
                itemIDs = GetUniqueRateIDsFromViewState()

                ' get deleted items from the list, we do not want to output controls for deleted items
                ratesToDelete = (From item In itemIDs _
                                     Where item.StartsWith(UNIQUEID_PREFIX_DELETE_RATE) _
                                        Select Utils.ToInt32(item.Replace(UNIQUEID_PREFIX_DELETE_RATE, ""))).ToList()

                ' create a new item line for each rate held..
                If bcRates IsNot Nothing AndAlso bcRates.Count > 0 Then

                    Dim ratesToOutput As New List(Of BudgetCategoryRate)()

                    ' get the rates to output ordered by descending date from
                    ratesToOutput = (From tmpRate In bcRates.ToArray() _
                                        Where (From tmpRateToDelete In ratesToDelete Select tmpRateToDelete).Contains(tmpRate.ID) = False _
                                        Order By tmpRate.DateFrom Descending _
                                     Select tmpRate).ToList()

                    For Each bcRate As BudgetCategoryRate In ratesToOutput
                        ' loop each rate to output to screen

                        id = GetUniqueRateID(bcRate)
                        OutputRatesControls(id, bcRate)

                        If itemIDs.Contains(id) = False Then
                            ' if the collection doesnt contain this id then add

                            itemIDs.Add(id)

                        End If

                    Next

                End If

                ' persist ids to view state 
                PersistUniqueRateIDsToViewState(itemIDs)

            Else

                pnlRates.Visible = False

            End If

            msg = New ErrorMessage()
            msg.Success = True

            Return msg

        End Function

#End Region

#Region " OutputRatesControls "

        Private Sub OutputRatesControls(ByVal uniqueID As String, ByVal bcRate As BudgetCategoryRate)
            OutputRatesControls(uniqueID, bcRate, Nothing)
        End Sub
        Private _AtLeastOneRateWithoutFullPercentagePeriod As Boolean = False

        Private Sub OutputRatesControls(ByVal uniqueID As String, ByVal bcRate As BudgetCategoryRate, ByVal rowIndex As Nullable(Of Integer))

            Dim row As TableRow
            Dim cell As TableCell
            Dim txt As TextBoxEx
            Dim txtUnitRate As TextBoxEx
            Dim dd As DropDownListEx
            Dim btnRemove As HtmlInputImage
            Dim chkAdditionalCost As CheckBox
            Dim chkUseActualCost As CheckBox

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_RATE) Then
                row = New TableRow()
                row.ID = uniqueID

                If Not rowIndex.HasValue Then
                    ' if we have no row index then add the row at the end 
                    phRates.Controls.Add(row)
                Else
                    ' else we have a row index so insert at that index
                    phRates.Controls.AddAt(rowIndex.Value, row)
                End If

                '++ Date From..
                cell = New TableCell()
                row.Cells.Add(cell)
                txt = CreateDateField(CTRL_PREFIX_RATE_DATE_FROM & uniqueID, bcRate.DateFrom)
                With txt
                    .Required = True
                    .RequiredValidatorErrMsg = "Date From is required."
                    .ValidationGroup = "Save"
                End With
                If bcRate.DateFrom = DataUtils.MAX_DATE Then txt.Text = ""
                cell.Controls.Add(txt)
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_RATE_DATE_FROM & "H" & uniqueID, txt.Value))
                _detailItemStartupJS.AppendFormat("function {0}_Changed(id) {{ txtRateDateFrom_Change(id); }};", txt.ID)

                '++ Date To..
                cell = New TableCell()
                row.Cells.Add(cell)
                txt = CreateDateField(CTRL_PREFIX_RATE_DATE_TO & uniqueID, bcRate.DateTo)
                If bcRate.DateTo = DataUtils.MAX_DATE Then txt.Text = ""
                cell.Controls.Add(txt)
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_RATE_DATE_TO & "H" & uniqueID, txt.Value))
                _detailItemStartupJS.AppendFormat("function {0}_Changed(id) {{ txtRateDateTo_Change(id); }};", txt.ID)

                '++ Expenditure Unit Rate..
                cell = New TableCell()
                row.Cells.Add(cell)
                'cell.CssClass = "expenditure indent"
                cell.CssClass = "con indent"
                cell.HorizontalAlign = HorizontalAlign.Left
                txt = CreateMoneyField(CTRL_PREFIX_RATE_EXP_UNIT_RATE & uniqueID, bcRate.ExpenditureUnitRate.ToString("F2"))
                With txt
                    .Required = True
                    .RequiredValidatorErrMsg = "Unit Rate is required."
                    .MinimumValue = "0.00"
                    .MaximumValue = 9999.99
                    .ValidationGroup = "Save"
                End With
                cell.Controls.Add(txt)

                'Use Actual Cost
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "income"
                cell.HorizontalAlign = HorizontalAlign.Center
                chkUseActualCost = New CheckBox()
                With chkUseActualCost
                    .Attributes.Add("onClick", String.Format("javascript:chkUseActualCostClicked(this, '{0}_txtTextBox');", CTRL_PREFIX_RATE_INC_UNIT_RATE & uniqueID))

                    If _invoicingMethod = SdsInvoicingMethodV2.CapActualServiceCostAtAssessedCharge Then
                        .Checked = bcRate.UseActualCostForIncome
                    Else
                        .Checked = False
                    End If
                    .ID = CTRL_PREFIX_RATE_USE_ACTUAL_COST & uniqueID
                End With
                cell.Controls.Add(chkUseActualCost)

                '++ Income Unit Rate..
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "income indent"
                cell.HorizontalAlign = HorizontalAlign.Left
                txtUnitRate = CreateMoneyField(CTRL_PREFIX_RATE_INC_UNIT_RATE & uniqueID, bcRate.IncomeUnitRate.ToString("F2"))
                With txtUnitRate
                    .Required = True
                    .RequiredValidatorErrMsg = "Unit Rate is required."
                    .MinimumValue = "0.00"
                    .MaximumValue = 9999.99
                    .ValidationGroup = "Save"
                    .OutputBrAfter = False
                End With
                cell.Controls.Add(txtUnitRate)

                If bcRate.ExpenditureUnitRate = bcRate.IncomeUnitRate Then
                    ' if the income and expenditure are the same then determine personal budget charged periods

                    Dim flagRateAsCharged As Boolean = False
                    Dim ratesPeriods As List(Of DataClasses.PersonalBudgetPercentageCharged) = (From tmpPeriod In PersonalBudgetChargedPeriods.ToArray() _
                                                                                                    Where tmpPeriod.DateFrom <= IIf(bcRate.DateTo.Ticks > 0, bcRate.DateTo, CDate("31-DEC-9999")) _
                                                                                                        AndAlso tmpPeriod.DateTo >= bcRate.DateFrom _
                                                                                                    Select tmpPeriod).ToList()

                    If ratesPeriods Is Nothing OrElse ratesPeriods.Count = 0 Then
                        ' if not rates then indicate personal budget charged

                        flagRateAsCharged = True

                    Else

                        Dim minDate As DateTime = ratesPeriods.Min(Function(tmpCharge As DataClasses.PersonalBudgetPercentageCharged) tmpCharge.DateFrom)
                        Dim maxDate As DateTime = ratesPeriods.Max(Function(tmpCharge As DataClasses.PersonalBudgetPercentageCharged) tmpCharge.DateTo)
                        Dim numberOfNonFullCharges As Integer = (From tmpCharge In ratesPeriods.ToArray() Where tmpCharge.PercentageCharged <> 100 Select tmpCharge).Count()

                        If minDate > bcRate.DateFrom _
                            OrElse maxDate < bcRate.DateTo _
                            OrElse numberOfNonFullCharges > 0 Then
                            ' if we do not have chanrges covering the whole of the rate or we have some 
                            ' charges which are not 100% then indicate budget charged

                            flagRateAsCharged = True

                        End If

                    End If

                    If flagRateAsCharged = True Then
                        ' if we have some periods then out put more info

                        Dim ratePeriodLabel As New Label()

                        _AtLeastOneRateWithoutFullPercentagePeriod = True

                        With ratePeriodLabel
                            .CssClass = "income centre"
                            .Style.Add("color", "orange")
                            .Text = "*"
                        End With

                        cell.Controls.Add(ratePeriodLabel)

                    End If

                End If

                '++ Income Unit Rate Max Charge..
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "income"
                cell.HorizontalAlign = HorizontalAlign.Left
                txt = CreateMoneyField(CTRL_PREFIX_RATE_INC_UNIT_RATE_MAX_CHARGE & uniqueID, bcRate.MaximumWeeklyCharge.ToString("F2"))
                With txt
                    .Required = True
                    .RequiredValidatorErrMsg = "Max. Charge(£) is required."
                    .MinimumValue = "0.00"
                    .MaximumValue = 9999.99
                    .ValidationGroup = "Save"
                    .OutputBrAfter = False
                End With
                cell.Controls.Add(txt)

                '++ Additional Cost Checkbox
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "income"
                cell.HorizontalAlign = HorizontalAlign.Left
                chkAdditionalCost = New CheckBox()
                With chkAdditionalCost

                    .Checked = bcRate.AdditionalCost
                    .ID = CTRL_PREFIX_RATE_ADDITIONAL_COST & uniqueID
                End With
                cell.Controls.Add(chkAdditionalCost)

                '++ Additional Cost Cap Drop Down..
                dd = New DropDownListEx()
                dd.Width = Unit.Percentage(85)
                With dd
                    .ID = CTRL_PREFIX_RATE_ADDITIONAL_COST_CHARGE_CAP & uniqueID
                    .OutputBrAfter = False
                    FillBudgetCategoryRateAdditionalCostChargeCapDropDownList(dd, bcRate.AdditionalCostChargeCap)
                End With
                cell.Controls.Add(dd)


                chkUseActualCost.Attributes.Add("onClick", String.Format("javascript:chkUseActualCostClicked(this, '{0}_txtTextBox');", txtUnitRate.ClientID))

                chkAdditionalCost.Attributes.Add("onClick", String.Format("javascript:chkAdditionalCostClicked(this, '{0}_cboDropDownList');", dd.ClientID))

                '++ Remove button..
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "right"
                cell.HorizontalAlign = HorizontalAlign.Center
                btnRemove = New HtmlInputImage()
                With btnRemove
                    .ID = CTRL_PREFIX_RATE_REMOVE & uniqueID
                    .Src = WebUtils.GetVirtualPath("Images/delete.png")
                    .Alt = "Remove this Rate entry"
                    AddHandler .ServerClick, AddressOf btnRemoveRate_Click
                    .Attributes.Add("onclick", "return btnRemoveRate_Click();")
                End With
                cell.Controls.Add(btnRemove)
            End If

        End Sub

#End Region

#Region " HaveNewBudgetCategoryData "

        Private Function HaveNewBudgetCategoryData() As Boolean
            Return Not (Session(SESSION_NEW_BUDGET_CATEGORY) Is Nothing)
        End Function

#End Region

#Region " FetchNewBudgetCategoryData "

        Private Function FetchNewBudgetCategoryData() As NewBudgetCategoryData
            Return Session(SESSION_NEW_BUDGET_CATEGORY)
        End Function

#End Region

#Region " StoreNewBudgetCategoryData "

        Private Function StoreNewBudgetCategoryData(ByVal newBudgetCategory As NewBudgetCategoryData, _
                                        ByVal budgetCategoryGroupID As Integer, _
                                        ByVal reference As String, _
                                        ByVal description As String, _
                                        ByVal fixedPrice As Boolean, _
                                        ByVal categoryType As BudgetCategoryBL.BudgetCategoryType, _
                                        ByVal domServiceTypeID As Integer, _
                                        ByVal domUnitsOfMeasureID As Integer, _
                                        ByVal redundant As Boolean, _
                                        ByVal financeCode1 As String, _
                                        ByVal financeCode2 As String) As NewBudgetCategoryData

            With newBudgetCategory
                .BudgetCategoryGroupID = budgetCategoryGroupID
                .Reference = reference
                .Description = description
                .AdditionalCost = fixedPrice
                .Type = categoryType
                .DomServiceTypeID = domServiceTypeID
                .DomUnitsOfMeasureID = domUnitsOfMeasureID
                .Redundant = redundant
                .FinanceCode1 = financeCode1
                .FinanceCode2 = financeCode2
            End With
            Session(SESSION_NEW_BUDGET_CATEGORY) = newBudgetCategory

            Return newBudgetCategory

        End Function

#End Region

#Region " ClearNewBudgetCategoryData "

        Private Sub ClearNewBudgetCategoryData()
            Session(SESSION_NEW_BUDGET_CATEGORY) = Nothing
        End Sub

#End Region

#Region " FillDropdownBudgetCategoryGroup "

        ''' <summary>
        ''' Fills the budget category group with items.
        ''' </summary>
        ''' <param name="selectedID">The selected ID.</param>
        Private Sub FillDropdownBudgetCategoryGroup(Optional ByVal selectedID As Integer = 0)

            Dim msg As New ErrorMessage()
            Dim budgetCategoryGroups As BudgetCategoryGroupCollection = Nothing

            ' init the budget category js and define class/fucntion for budget category groups
            _budgetCategoryGroupsJS = New StringBuilder("function BudgetCategoryGroup(id, description, redundant, groupUnitsOfServiceOnServiceUserStatement, unitOfMeasureID) { this.ID = id; this.Description = description; this.Redundant = redundant; this.GroupUnitsOfServiceOnServiceUserStatement = groupUnitsOfServiceOnServiceUserStatement; this.UnitOfMeasureID = unitOfMeasureID; }")

            ' get the budget category groups
            msg = BudgetCategoryGroupBL.FetchList(DbConnection, budgetCategoryGroups)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not budgetCategoryGroups Is Nothing AndAlso budgetCategoryGroups.Count > 0 Then
                ' if we have some groups then create some js for client side processing

                Dim bcgIndex As Integer = 0

                ' loop each budget category group and add item to js array/collection
                For Each bcg As Target.Abacus.Library.DataClasses.BudgetCategoryGroup In (From bcgs In budgetCategoryGroups.ToArray() _
                                                                                            Order By bcgs.Description _
                                                                                                Select bcgs).ToList()

                    _budgetCategoryGroupsJS.AppendFormat("budgetCategoryGroupCollection[{0}] = new BudgetCategoryGroup({1}, '{2}', {3}, {4}, {5});", _
                                                         bcgIndex, _
                                                         bcg.ID, _
                                                         bcg.Description, _
                                                         bcg.Redundant.ToString().ToLower(), _
                                                         bcg.GroupUnitsOfServiceOnServiceUserStatement.ToString().ToLower(), _
                                                         bcg.UnitOfMeasureID)
                    bcgIndex += 1

                Next

            End If

            ' clear all items from the list
            cboBudgetCategoryGroup.DropDownList.Items.Clear()

            With cboBudgetCategoryGroup.DropDownList
                ' add all non redundant budget categories into the drop down list
                .DataSource = (From bcgs In budgetCategoryGroups.ToArray() _
                                    Where bcgs.Redundant = TriState.False _
                                        Order By bcgs.Description _
                                    Select bcgs).ToList()

                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
            End With

            ' add in default and select value in list
            cboBudgetCategoryGroup.DropDownList.Items.Insert(0, New ListItem(" ", ""))
            WebUtils.SetDropdownListValue(cboBudgetCategoryGroup.DropDownList, selectedID)

        End Sub

#End Region

#Region " FillDropdownBudgetCategoryType "
        Private Sub FillDropdownBudgetCategoryType(Optional ByVal selectedID As Integer = 0)
            Dim cboItem As ListItem = Nothing

            If cboBudgetCategoryType.DropDownList.Items.Count = 0 Then
                cboItem = New ListItem(" ", "0")
                cboBudgetCategoryType.DropDownList.Items.Add(cboItem)
                cboItem = New ListItem( _
                    Utils.SplitOnCapitals(BudgetCategoryBL.BudgetCategoryType.MonetaryAmount.ToString()), _
                    BudgetCategoryBL.BudgetCategoryType.MonetaryAmount _
                )
                cboBudgetCategoryType.DropDownList.Items.Add(cboItem)
                cboItem = New ListItem( _
                    Utils.SplitOnCapitals(BudgetCategoryBL.BudgetCategoryType.UnitsOfService.ToString()), _
                    BudgetCategoryBL.BudgetCategoryType.UnitsOfService _
                )
                cboBudgetCategoryType.DropDownList.Items.Add(cboItem)
            End If

            WebUtils.SetDropdownListValue(cboBudgetCategoryType.DropDownList, selectedID)

        End Sub
#End Region

#Region " FillDropdownUnitsOfMeasure "
        Private Sub FillDropdownUnitsOfMeasure(Optional ByVal selectedID As Integer = 0)
            Const SP_GET_UNITS_OF_MEASURE As String = "spxDomUnitsOfMeasure_FetchListWithPaging"
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage = Nothing
            Dim cboItem As ListItem = Nothing
            Dim spParams As SqlParameter() = Nothing
            Dim selValue As String, selItem As String = ""
            Dim visitBased As Integer = 0

            If cboUnitOfMeasure.DropDownList.Items.Count = 0 Then
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_GET_UNITS_OF_MEASURE, False)
                spParams(0).Value = ""
                spParams(1).Value = ""
                spParams(2).Value = 1
                spParams(3).Value = 9999    '++ Retrieve all records without regard to paging..
                spParams(4).Direction = ParameterDirection.InputOutput
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_GET_UNITS_OF_MEASURE, spParams)

                While reader.Read
                    visitBased = IIf((Utils.ToInt32(reader("MinutesPerUnit")) > 0), 1, 0)
                    selValue = String.Format("{0}?{1}:{2}", reader("ID"), visitBased, reader("SystemType"))
                    cboItem = New ListItem(reader("Description"), selValue)
                    If reader("ID") = selectedID Then selItem = selValue
                    cboUnitOfMeasure.DropDownList.Items.Add(cboItem)
                End While

                cboItem = New ListItem(" ", "")
                cboUnitOfMeasure.DropDownList.Items.Insert(0, cboItem)

                reader.Close()
                reader = Nothing

                WebUtils.SetDropdownListValue(cboUnitOfMeasure.DropDownList, selItem)
                hidUnitOfMeasure.Value = selItem

            Else
                For Each item As ListItem In cboUnitOfMeasure.DropDownList.Items
                    If item.Value.Contains("?") Then
                        Dim lPos As Integer = item.Value.IndexOf("?")
                        selValue = item.Value.Substring(0, lPos)
                    Else
                        selValue = item.Value
                    End If
                    If selValue = selectedID.ToString Then
                        WebUtils.SetDropdownListValue(cboUnitOfMeasure.DropDownList, item.Value)
                        hidUnitOfMeasure.Value = item.Value
                        Exit For
                    End If
                Next
            End If

        End Sub
#End Region

#Region " CreateDateField "

        Private Function CreateDateField(ByVal uniqueID As String, _
                                          ByVal units As Date) As TextBoxEx
            Dim txt As TextBoxEx

            txt = New TextBoxEx()
            With txt
                .ID = uniqueID
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .Width = New Unit(9, UnitType.Em)
                .Text = units.ToString("dd/MM/yyyy")
            End With
            Return txt

        End Function

#End Region

#Region " CreateMoneyField "

        Private Function CreateMoneyField(ByVal uniqueID As String, _
                                          ByVal units As Decimal) As Control
            Dim txt As TextBoxEx

            txt = New TextBoxEx()
            With txt
                .ID = uniqueID
                .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                .Width = New Unit(5, UnitType.Em)
                .Text = units.ToString("F2")
            End With
            Return txt

        End Function

#End Region

#Region " GetUniqueRateIDsFromViewState "

        Private Function GetUniqueRateIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_RATES) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_RATES), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_RATES_COUNTER) Is Nothing Then
                _newRateIDCounter = 0
            Else
                _newRateIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_RATES_COUNTER), Integer)
            End If

            Return list

        End Function

#End Region

#Region " btnRemoveRate_Click "

        Private Sub btnRemoveRate_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim msg As ErrorMessage

            Dim list As List(Of String) = GetUniqueRateIDsFromViewState()
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_RATE_REMOVE, String.Empty)

            ' change the id in viewstate
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_RATE) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_RATE, UNIQUEID_PREFIX_DELETE_RATE)
                    End If
                    Exit For
                End If
            Next

            ' remove from the grid
            For index As Integer = 0 To phRates.Controls.Count - 1
                If phRates.Controls(index).ID = id Then
                    phRates.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueRateIDsToViewState(list)

            msg = RepopulateScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

#Region " GetUniqueRateID "

        Private Function GetUniqueRateID(ByVal bcRate As BudgetCategoryRate) As String
            Dim id As String

            If Utils.ToInt32(bcRate.ID) = 0 Then
                id = UNIQUEID_PREFIX_NEW_RATE & _newRateIDCounter
                _newRateIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_RATE & bcRate.ID
            End If

            Return id

        End Function

#End Region

#Region " PersistDetailUniqueIDsToViewState "

        Private Sub PersistUniqueRateIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_RATES, list)
            ViewState.Add(VIEWSTATE_KEY_RATES_COUNTER, _newRateIDCounter)
        End Sub

#End Region

#Region " GetRemovedDetailsToViewState "

        Private Function GetRemovedRatesToViewState() As List(Of Triplet)

            Dim list As List(Of Triplet)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_RATES_REMOVED) Is Nothing Then
                list = New List(Of Triplet)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_RATES_REMOVED), List(Of Triplet))
            End If

            Return list

        End Function

#End Region

#Region " PersistRemovedRatesToViewState "

        Private Sub PersistRemovedRatesToViewState(ByVal list As List(Of Triplet))
            ViewState.Add(VIEWSTATE_KEY_RATES_REMOVED, list)
        End Sub

#End Region

#Region " Tab Manipulation "

        Private Sub ShowTabRates(ByVal isVisible As Boolean)
            tabRates.Visible = isVisible
            If isVisible Then
                tabRates.HeaderText = "Rates"
            Else
                tabRates.HeaderText = ""
            End If
        End Sub

#End Region

#Region " PrimeBudgetCategoryClass "

        Private Function PrimeBudgetCategoryClass() As ErrorMessage
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If _budcatID > 0 Then
                '++ ..editing an existing budget category, so re-fetch it..
                If _budcat Is Nothing Then
                    _budcat = New Target.Abacus.Library.DataClasses.BudgetCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                End If
                msg = BudgetCategoryBL.Fetch(Me.DbConnection, _budcatID, _budcat, _budcatRates, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), currentUser.ExternalUsername)
                If Not msg.Success Then Return msg
            Else
                '++ Re-call AddNew() to re-create the suggested budget category.
                msg = ClearBudgetCategory()
                ShowTabRates(False)
            End If

            Return msg

        End Function

#End Region

#Region " btnAddRate_Click "

        ''' <summary>
        ''' Handles the Click event of the btnAddRate control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnAddRate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddRate.Click

            Dim msg As ErrorMessage
            Dim id As String
            Dim itemIDs As List(Of String)
            Dim list As List(Of String)
            Dim newLine As BudgetCategoryRate = Nothing
            Dim nextDate As Date
            Dim ratesToDelete As New List(Of Integer)()

            msg = RepopulateScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get any items from viewstate to populate
            itemIDs = GetUniqueRateIDsFromViewState()

            ' get deleted items from the list, we do not want to output controls for deleted items
            ratesToDelete = (From item In itemIDs _
                                 Where item.StartsWith(UNIQUEID_PREFIX_DELETE_RATE) _
                                    Select Utils.ToInt32(item.Replace(UNIQUEID_PREFIX_DELETE_RATE, ""))).ToList()

            If _budcatRates IsNot Nothing AndAlso _budcatRates.Count > 0 Then
                ' if we have some rates then get the max date 

                Dim maxDateFrom As DateTime = DateTime.MinValue
                Dim maxDateTo As DateTime = DateTime.MaxValue

                ' get the max date from of any rate record
                maxDateFrom = (From tmpRate In _budcatRates.ToArray() _
                                Where _
                                    tmpRate.DateFrom < DataUtils.MAX_DATE _
                                    AndAlso (From tmpRateToDelete In ratesToDelete _
                                                Select tmpRateToDelete).Contains(tmpRate.ID) = False _
                                Select tmpRate.DateFrom).MaxOrDefault()

                ' get the max date to of any rate record
                maxDateTo = (From tmpRate In _budcatRates.ToArray() _
                                Where _
                                    tmpRate.DateTo < DataUtils.MAX_DATE _
                                    AndAlso (From tmpRateToDelete In ratesToDelete _
                                                Select tmpRateToDelete).Contains(tmpRate.ID) = False _
                                Select tmpRate.DateTo).MaxOrDefault()

                If maxDateFrom > maxDateTo Then

                    nextDate = maxDateFrom

                Else

                    nextDate = maxDateTo

                End If

                nextDate = nextDate.AddDays(1)

            Else
                ' else set the next date to max date as no existing rates

                nextDate = DataUtils.MAX_DATE

            End If

            newLine = New BudgetCategoryRate(DbConnection, "", "")
            With newLine
                .DateFrom = nextDate
                .DateTo = DataUtils.MAX_DATE
            End With

            list = GetUniqueRateIDsFromViewState()
            id = GetUniqueRateID(newLine)

            '++ Create the controls. Add the new item at the start, since items
            '++ will be listed in reverse chronological order..
            OutputRatesControls(id, newLine, 0)

            '++ Persist the data into view state using the calculated index..
            list.Insert(0, id)
            PersistUniqueRateIDsToViewState(list)

            Me.Validate("Save")

        End Sub

#End Region

#Region "GetIndexOfNewRateLine"

        ''' <summary>
        ''' Gets the index of a new Rate line by looping each detail line 
        ''' and evaluating the Date From values
        ''' </summary>
        ''' <param name="newLine">The new line to insert</param>
        ''' <returns></returns>
        Public Function GetIndexOfNewRateLine(ByVal newLine As BudgetCategoryRate) As Integer
            Dim currentControlIndex As Integer = 0
            Dim insertDetailLineAtIndex As Integer = 0
            Dim dateFromCell As TableCell
            Dim dateFromCellSpan As TextBoxEx
            Dim dateFromCellSpanValue As String
            Dim dateFromDate As DateTime

            For Each tmpControl As Control In phRates.Controls
                '++ Get the 1st cell of the row, which should contain the Date From field..
                dateFromCell = CType(tmpControl.Controls(0), TableCell)
                '++ Get the 1st control within the cell, which should actually be the Date From..
                dateFromCellSpan = CType(dateFromCell.Controls(0), TextBoxEx)
                dateFromCellSpanValue = dateFromCellSpan.Text

                If String.IsNullOrEmpty(dateFromCellSpanValue) = False _
                        AndAlso dateFromCellSpanValue.Trim().Length > 0 _
                        AndAlso DateTime.TryParse(dateFromCellSpanValue, dateFromDate) Then
                    '++ If the value in the control is a valid date then compare to the passed row..
                    If newLine.DateFrom >= dateFromDate Then
                        insertDetailLineAtIndex = currentControlIndex + 1
                    ElseIf newLine.DateFrom < dateFromDate Then
                        Exit For
                    End If
                End If

                currentControlIndex += 1
            Next

            Return insertDetailLineAtIndex
        End Function

#End Region

#Region " ClearBudgetCategory "
        Private Function ClearBudgetCategory() As ErrorMessage
            Dim msg As New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Try
                _budcat = New Target.Abacus.Library.DataClasses.BudgetCategory(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                _budcatRates = Nothing

                PersistUniqueRateIDsToViewState(New List(Of String))
                msg.Success = True

            Catch ex As Exception
                msg.Success = False
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
            Finally
            End Try

            Return msg
        End Function
#End Region

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim js As StringBuilder = New StringBuilder()
            Dim msg As ErrorMessage = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim svcTypes As DomServiceTypeCollection = Nothing

            _budcatID = _stdBut.SelectedItemID
            If _budcat Is Nothing Then
                _budcat = New Target.Abacus.Library.DataClasses.BudgetCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            End If
            If _budcatID > 0 Then
                msg = BudgetCategoryBL.Fetch(Me.DbConnection, _budcatID, _budcat, _budcatRates, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            End If
            If _budcat.ID > 0 OrElse _budcat.ID = -1 Then
                If _budcat.ID = -1 Then
                    tabRates.Enabled = (_budcat.Type = BudgetCategoryBL.BudgetCategoryType.UnitsOfService)
                End If
            Else
                ' hide fields when creating
                tabRates.Enabled = False
            End If

            If _budcat.SystemType > BudgetCategoryBL.BudgetCategorySystemType.None Then
                ' if the system type is larger than 0 this signifies a protected budget category
                ' in which only rates can be altered, no details can be altered also cannot delete this record

                WebUtils.RecursiveDisable(tabDetails.Controls, True)

            End If

            If _budcat.ID > 0 AndAlso _budcat.DomServiceTypeID > 0 Then

                Dim serviceType As New DomServiceType(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)

                msg = serviceType.Fetch(_budcat.DomServiceTypeID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If serviceType.ServiceCategory = 1 Then

                    CType(txtServiceType, InPlaceServiceTypeSelector).Enabled = False

                Else

                    If _inEditMode Then

                        CType(txtServiceType, InPlaceServiceTypeSelector).Enabled = True

                    End If

                End If

            End If

            cboBudgetCategoryType.DropDownList.Attributes.Add("onchange", "cboBudgetCategoryType_Change();")
            cboUnitOfMeasure.DropDownList.Attributes.Add("onchange", "cboUnitOfMeasure_Change();")
            cboBudgetCategoryGroup.DropDownList.Attributes.Add("onchange", "cboBudgetCategoryGroup_Change();")

            js.AppendFormat("buttonsMode={0};", Convert.ToInt32(_stdBut.ButtonsMode))
            js.AppendFormat("tabRatesVisible={0};", IIf(tabRates.Visible And tabRates.Enabled, "'Y'", "'N'"))
            js.AppendFormat("hidUnitOfMeasureID='{0}';", hidUnitOfMeasure.ClientID)
            js.AppendFormat("lblServiceCategoryID='{0}';", lblServiceCategory.ClientID)
            js.AppendFormat("inEditMode = {0};", _inEditMode.ToString().ToLower())
            js.AppendFormat("currentBudgetCategoryGroupValue = {0};", _budcat.BudgetCategoryGroupID)
            js.AppendFormat("txtServiceTypeID = '{0}';", txtServiceType.ClientID)
            js.AppendFormat("bcSystemType = {0};", _budcat.SystemType)

            Dim disableUseActualCost As Boolean = Not ((_stdBut.ButtonsMode = StdButtonsMode.Edit Or _stdBut.ButtonsMode = StdButtonsMode.AddNew) _
                                    AndAlso _invoicingMethod = SdsInvoicingMethodV2.CapActualServiceCostAtAssessedCharge)
            js.AppendFormat("disableActualCostIndicator = {0};", disableUseActualCost.ToString.ToLower)

            '++ Pass the full contents of the Unit of Measure dropdown to the Javascript.
            '++ (The dropdown will be auto-filled depending on the setting of the
            '++ adjacent Service Type dropdown)..
            Dim iIndex As Integer
            For iIndex = 0 To cboUnitOfMeasure.DropDownList.Items.Count - 1
                js.AppendFormat(String.Format("uomDescs[{0}]='{1}';", iIndex, cboUnitOfMeasure.DropDownList.Items(iIndex).Text))
                js.AppendFormat(String.Format("uomValues[{0}]='{1}';", iIndex, cboUnitOfMeasure.DropDownList.Items(iIndex).Value))
            Next

            js.Append(_detailItemStartupJS.ToString())
            js.Append(_budgetCategoryGroupsJS)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

            If _AtLeastOneRateWithoutFullPercentagePeriod Then

                lblRateNotFullWarning.Text = "* The Income Unit Cost may be reduced by the Personal Budget %age Charged."
                lblRateNotFullWarning.Style.Add("color", "orange")

            Else

                lblRateNotFullWarning.Text = ""

            End If

        End Sub

#Region " CreateHiddenField "
        Private Function CreateHiddenField(ByVal id As String, ByVal value As String) As HiddenField
            Dim hid As HiddenField

            hid = New HiddenField()
            With hid
                .ID = id
                .Value = value
            End With

            Return hid
        End Function
#End Region

#Region "FillBudgetCategoryRateAdditionalCostChargeCapDropDownList"

        ''' <summary>
        ''' Fills the budget category rate additional cost charge cap drop down list.
        ''' </summary>
        Private Sub FillBudgetCategoryRateAdditionalCostChargeCapDropDownList(ByRef dropDown As DropDownListEx, ByVal selectedID As Integer)

            If Not dropDown Is Nothing Then
                ' if we have a drop down list then populate it with items

                Dim newDropDownItem As ListItem = Nothing
                Dim newDropDownItemText As String = String.Empty

                ' clear existing items
                dropDown.DropDownList.Items.Clear()

                For Each newDropDownItemValue As Integer In [Enum].GetValues(GetType(BudgetCategoryBL.BudgetCategoryRateAdditionalCostChargeCap))
                    ' loop each item in the BudgetCategoryRateAdditionalCostChargeCap enumeration and add into drop down list

                    newDropDownItemText = Utils.SplitOnCapitals([Enum].GetName(GetType(BudgetCategoryBL.BudgetCategoryRateAdditionalCostChargeCap), newDropDownItemValue))
                    newDropDownItem = New ListItem(newDropDownItemText, newDropDownItemValue)
                    dropDown.DropDownList.Items.Add(newDropDownItem)

                Next

                ' select the post back value in the drop down
                dropDown.SelectPostBackValue()
                dropDown.DropDownList.SelectedValue = selectedID

            End If

        End Sub

#End Region

    End Class

End Namespace