
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Screen used to maintain a domiciliary contract period.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir  21/10/2013  A8263 -  Modify Contract Type selector
    ''' MoTahir  17/09/2013  Block Contract Payment Plan (D12459)
    ''' JF   19/09/2013  Re-jig check for linked visits/amendments for Edit mode only (#8171)
    ''' JAF  02/09/2013  For Block Guarantee contracts, limit choice of provider invoice input methods (D12503)
    ''' CD   09/08/2011  D11965 - Changes to use Framework Type to determine bevahiour of form.
    ''' CD   29/04/2010  A4WA#6252 - prevented refresh of tree if delete failed (DeleteClicked), so user can see error message
    ''' MvO  16/12/2009  D11743 - remove restriction/add warning on specify DoW checkbox for contracts with enhanced rate days.
    ''' MvO  15/12/2009  A4WA#5967 - fix to enabling/priming of DateFrom when period is the first in the contract.
    ''' MvO  16/07/2009  A4WA#5518 - fix to report button setup when in different screen modes.
    ''' MvO  12/05/2009  D11549 - added reporting support.
    ''' MvO  23/04/2009  A4WA#5395 - fix to re-displaying DateFrom/DateTo when disabled.
    '''                  Fix when edit/re-save redundant visit code groups/man amend inds.
    ''' MvO  22/04/2009  A4WA#5395 - fix to the "in use" flags.
    ''' MvO  07/04/2009  D11537 - need to suppress Csrf check due to use of iframe.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class Period
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _contractID As Integer
        Private _providerID As Integer
        Private _periodID As Integer
        Private _refreshTree As Boolean
        Private _visitBasedReturns As Boolean
        Private _useEnhancedRateDays As Boolean
        Private _inUse As Boolean
        Private _inUseByNonVisitBasedDso As Boolean
        Private _inUseByNonSpecifyDoWDso As Boolean
        Private _contractEnded As Boolean
        Private _isFirstPeriodForContract As Boolean
        Private _svcGroupID As Integer = 0
        Private _svcGroup As ServiceGroup = Nothing
        Private _contractStartDate As Date
        Private _frameworkId As Integer = 0
        Private _FrameworkHasAtLeastOneTimeBasedUom As Nullable(Of Boolean)
        Private _framework As DomRateFramework = Nothing
        Private _FrameworkHasAtLeastNonTimeBasedUomWithServiceType As Nullable(Of Boolean) = Nothing

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Period")

            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _periodID = Utils.ToInt32(Request.QueryString("id"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            Dim contract As DomContract
            Dim periods As DomContractPeriodCollection = Nothing
            Dim msg As ErrorMessage
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

            With _stdBut
                .AllowNew = canEdit
                .ShowNew = False
                .AllowFind = False
                .AllowEdit = canEdit
                .AllowDelete = canEdit
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            Me.JsLinks.Add("Period.js")
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' check the contract to see if we need a visit code group
            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            With contract
                msg = .Fetch(_contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                cboVisitCodeGroup.Required = .VisitBasedReturns
                cboVisitCodeGroup.RequiredValidator.Enabled = .VisitBasedReturns
                _visitBasedReturns = .VisitBasedReturns
                _svcGroupID = .ServiceGroupID
                _useEnhancedRateDays = .UseEnhancedRateDays
                _contractEnded = (.EndDate <> DataUtils.MAX_DATE)
                _contractStartDate = .StartDate
                _frameworkId = .DomRateFrameworkID
                _providerID = .ProviderID
            End With

            Dim provider As New Establishment(Me.DbConnection)
            msg = provider.Fetch(_providerID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            hidProviderEmail.Value = provider.EmailAddress

            Me.UseJQuery = True
            Me.UseJqueryUI = True

            If _svcGroupID > 0 Then
                _svcGroup = New ServiceGroup(Me.DbConnection, String.Empty, String.Empty)
                msg = _svcGroup.Fetch(_svcGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            If _frameworkId > 0 Then

                _framework = New DomRateFramework(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                msg = _framework.Fetch(_frameworkId)
                If Not msg.Success Then WebUtils.DisplayError(msg)

            End If

            If Me.IsPostBack Then _periodID = _stdBut.SelectedItemID

            ' determine _inUseByNonVisitBasedDso
            msg = DomContractBL.ContractPeriodInUseByDso(Me.DbConnection, _periodID, TriState.False, TriState.UseDefault, _inUseByNonVisitBasedDso)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' determine _inUseByNonSpecifyDoWDso
            msg = DomContractBL.ContractPeriodInUseByDso(Me.DbConnection, _periodID, TriState.UseDefault, TriState.False, _inUseByNonSpecifyDoWDso)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' determine _isFirstPeriodForContract
            msg = DomContractBL.IsFirstPeriodForContract(Me.DbConnection, _periodID, _isFirstPeriodForContract)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not _isFirstPeriodForContract Then
                ' check for no periods in the contract
                msg = DomContractPeriod.FetchList(Me.DbConnection, periods, String.Empty, String.Empty, _contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _isFirstPeriodForContract = (periods.Count = 0)
            End If

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            PopulateDropdowns(0)
            PopulateFormFromLastPeriod(e)
            Dim contract As DomContract
            Dim msg As ErrorMessage

            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            With contract
                msg = .Fetch(_contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End With

            If Not contract.ContractType = DomContractType.BlockPeriodic.ToString Then
                ' Clear the date from, as we cannot determine this date automatically
                dteDateFrom.Text = String.Empty
            End If

            ' We need to reset these controls, as by populating from a prior period means we have a periodId
            ' By having this value set, the standard buttons are placed in the incorrect mode
            _stdBut.InitialMode = StdButtonsMode.AddNew
            _stdBut.ShowReports = False
        End Sub

        ''' <summary>
        ''' Populates the form based on the last entered period
        ''' </summary>
        ''' <remarks></remarks>
        Private Overloads Sub PopulateFormFromLastPeriod(ByRef e As StdButtonEventArgs)

            Dim periods As DomContractPeriodCollection = Nothing
            Dim lastPeriod As DomContractPeriod = Nothing
            Dim msg As ErrorMessage = Nothing

            msg = DomContractPeriod.FetchList(Me.DbConnection, periods, String.Empty, String.Empty, _contractID)
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
                Exit Sub
            End If

            If periods Is Nothing OrElse periods.Count = 0 Then
                Exit Sub
            End If

            lastPeriod = periods.ToArray().OrderByDescending(Function(p As DomContractPeriod) p.ID).FirstOrDefault()
            PopulateFormFromPeriod(e, lastPeriod.ID)

        End Sub

        ''' <summary>
        ''' Populates the form fields based on the chosen periodId (from DomInvoicePeriod.ID)
        ''' </summary>
        ''' <param name="periodId">The unique identifier of the period to populate details from</param>
        ''' <remarks></remarks>
        Private Overloads Sub PopulateFormFromPeriod(ByRef e As StdButtonEventArgs, ByVal periodId As Int32)

            Dim msg As ErrorMessage
            Dim period As DomContractPeriod = New DomContractPeriod(Me.DbConnection, String.Empty, String.Empty)
            msg = period.Fetch(periodId)
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
                Exit Sub
            End If

            PopulateFormFromPeriod(e, period)
        End Sub

        ''' <summary>
        ''' Populates the form fields based on the data held within the DomContractPeriod object
        ''' </summary>
        ''' <param name="period">DomContractPeriod object</param>
        ''' <remarks></remarks>
        Private Overloads Sub PopulateFormFromPeriod(ByRef e As StdButtonEventArgs, ByVal period As DomContractPeriod)

            Dim periods As DomContractPeriodCollection = Nothing
            Dim lastPeriod As DomContractPeriod = Nothing
            Dim msg As ErrorMessage = Nothing

            msg = DomContractPeriod.FetchList(Me.DbConnection, periods, String.Empty, String.Empty, _contractID)
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
                Exit Sub
            End If

            If periods Is Nothing OrElse periods.Count = 0 Then
                lastPeriod = Nothing
            Else
                lastPeriod = periods.ToArray().OrderByDescending(Function(p As DomContractPeriod) p.ID).FirstOrDefault()
            End If


            With period

                PopulateDropdowns(.ServiceOutcomeGroupID)
                txtReference.Text = .Reference
                txtDescription.Text = .Description
                dteDateFrom.Text = .DateFrom
                If Not e.ButtonsControl.ButtonsMode = Target.Library.Web.UserControls.StdButtonsMode.AddNew Then If .DateTo < DataUtils.MAX_DATE Then dteDateTo.Text = .DateTo
                'If .DateTo < DataUtils.MAX_DATE And (Not lastPeriod Is Nothing AndAlso Not lastPeriod.DateTo < DataUtils.MAX_DATE OrElse lastPeriod.ID = .ID) Then dteDateTo.Text = .DateTo
                If .VisitCodeGroupID > 0 Then
                    AddRedundantVisitCodeGroup(.VisitCodeGroupID)
                    cboVisitCodeGroup.DropDownList.SelectedValue = .VisitCodeGroupID
                End If
                If .DomManuallyAmendedIndicatorGroupID > 0 Then
                    AddRedundantManAmendInd(.DomManuallyAmendedIndicatorGroupID)
                    cboManAmendGroup.DropDownList.SelectedValue = .DomManuallyAmendedIndicatorGroupID
                End If
                If .ServiceOutcomeGroupID > 0 Then
                    AddRedundantServiceOutcomeGroup(.ServiceOutcomeGroupID)
                    cboServiceOutcomeGroup.DropDownList.SelectedValue = .ServiceOutcomeGroupID
                End If
                If .ContractedUnits > 0 Then txtContractedUnits.Text = .ContractedUnits
                chkVisitBasedPlans.CheckBox.Checked = .VisitBasedPlans
                chkSpecifyPlannedDoW.CheckBox.Checked = .SpecifyDayOfWeekOnDsoPlan
                SetupReport(period.ID)
                SetProviderInvoiceInputMethod(.ProviderInvoiceInputMethod)

                SetPaymentRequestType(.ProviderInvoiceInputMethod, .PaymentRequests)
                ddPrMinPayPeriod.DropDownList.SelectedValue = .PaymentRequestMinPeriod
                cbPrProviderEmail.CheckBox.Checked = .PaymentRequestsUseProviderEmail
                If Not .AutoSetSecondaryVisit = TriState.UseDefault Then
                    chkAutoSetSecondryVisit.Checked = .AutoSetSecondaryVisit
                End If

                If .ProviderInvoiceInputMethod = ProviderInvoiceInputMethod.InvoicesCreatedFromSummaryLevelProFormaInvoices Then
                    If .PaymentRequestsUseProviderEmail Then
                        If .PaymentRequests = PaymentRequestType.CareProvider Or .PaymentRequests = PaymentRequestType.CouncilStaff Then
                            tbPrProviderEmail.Text = hidProviderEmail.Value
                        End If
                    Else
                        tbPrProviderEmail.Text = .PaymentRequestsCustomEmail
                    End If
                End If

            End With
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            PopulateFormFromPeriod(e, e.ItemID)
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                PopulateDropdowns(0)
                txtReference.Text = String.Empty
                txtDescription.Text = String.Empty
                dteDateFrom.Text = String.Empty
                dteDateTo.Text = String.Empty
                txtContractedUnits.Text = String.Empty
                chkVisitBasedPlans.CheckBox.Checked = False
                chkSpecifyPlannedDoW.CheckBox.Checked = False
            Else
                rbInvCreatedFromServiceRegisters.Checked = False
                rbInvCreatedSummaryProForma.Checked = False
                rbInvCreatedVisitProForma.Checked = False
                rbInvManuallyEntered.Checked = False
                rbInvNotEntered.Checked = False

                FindClicked(e)
            End If

            lblError.Text = String.Empty
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Dim validated As Boolean = False
            msg = DomContractBL.DeleteContractPeriodValidate(Me.DbConnection, e.ItemID, validated)
            If Not msg.Success Then
                lblError.Text = msg.Message
                Return
            End If
            If Not validated Then
                lblError.Text = "Change not permitted: amending the Contract Period history in this way would invalidate visit codes recorded against one or more visit."
                FindClicked(e)
            Else
                msg = DomContractBL.DeleteContractPeriod(Me.DbConnection, e.ItemID, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), True)
                If Not msg.Success Then
                    If msg.Number = "E3006" Then    ' could not delete period
                        FindClicked(e)
                        lblError.Text = msg.Message
                        e.Cancel = True
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                Else
                    _periodID = 0
                    _refreshTree = True
                End If
            End If



        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim period As DomContractPeriod = Nothing
            Dim lastPeriod As DomContractPeriod = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = DomContractBL.FetchLastDomContractPeriod(connection:=Me.DbConnection, transaction:=Nothing, contractID:=_contractID, item:=lastPeriod)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' as we are not using viewstate because it fecks other things up, dropdowns need a bit of extra work to pass validation
            PopulateDropdowns(Utils.ToInt32(cboServiceOutcomeGroup.GetPostBackValue()))

            If _inUse Then
                If cboVisitCodeGroup.Visible Then cboVisitCodeGroup.RequiredValidator.Enabled = False
                If cboServiceOutcomeGroup.Visible Then cboServiceOutcomeGroup.RequiredValidator.Enabled = False
                If cboManAmendGroup.Visible Then cboManAmendGroup.RequiredValidator.Enabled = False
            Else
                If cboVisitCodeGroup.Visible Then cboVisitCodeGroup.SelectPostBackValue()
                If cboServiceOutcomeGroup.Visible Then cboServiceOutcomeGroup.SelectPostBackValue()
                If cboManAmendGroup.Visible Then cboManAmendGroup.SelectPostBackValue()
            End If

            ddPrMinPayPeriod.SelectPostBackValue()

            Select Case _framework.FrameworkTypeId
                Case FrameworkTypes.CommunityGeneral
                    cboVisitCodeGroup.RequiredValidator.Enabled = False
                    cboServiceOutcomeGroup.RequiredValidator.Enabled = False
            End Select

            Me.Validate("Save")

            If Me.IsValid Then

                period = New DomContractPeriod(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With period
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                Else
                    period.DomContractID = _contractID
                End If

                Dim contract As DomContract

                contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                With contract
                    msg = .Fetch(_contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With

                With period
                    .Reference = IIf(txtReference.Text.Trim().Length > 0, txtReference.Text, Nothing)
                    .Description = IIf(txtDescription.Text.Trim().Length > 0, txtDescription.Text, Nothing)

                    If Not _isFirstPeriodForContract Then
                        If dteDateFrom.Text = String.Empty Then
                            If Not lastPeriod Is Nothing Then
                                dteDateFrom.Text = lastPeriod.DateFrom
                            End If
                        End If
                        .DateFrom = dteDateFrom.Text
                        If contract.ContractType = DomContractType.BlockPeriodic.ToString Then
                            GetLastPeriod(msg, lastPeriod)
                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            If Not lastPeriod Is Nothing Then
                                If lastPeriod.DateTo = DataUtils.MAX_DATE Then
                                    '.DateFrom = lastPeriod.DateFrom.AddDays(1)
                                    With lastPeriod
                                        .DbConnection = Me.DbConnection
                                        lastPeriod.DateTo = lastPeriod.DateFrom
                                        msg = .Save
                                        If Not msg.Success Then WebUtils.DisplayError(msg)
                                    End With
                                Else
                                    .DateFrom = lastPeriod.DateTo.AddDays(1)
                                End If
                            End If

                        End If
                    Else
                        .DateFrom = _contractStartDate
                        dteDateFrom.Text = .DateFrom
                    End If

                        '.DateTo = IIf(Utils.IsDate(dteDateTo.Text), dteDateTo.Text, DataUtils.MAX_DATE)
                        If .IsNew Then
                            .DateTo = DataUtils.MAX_DATE
                        Else
                            dteDateTo.Text = .DateTo
                        End If

                        If _inUse And cboVisitCodeGroup.Visible Then
                            AddRedundantVisitCodeGroup(.VisitCodeGroupID)
                        Else
                            If cboVisitCodeGroup.Visible Then
                                If cboVisitCodeGroup.Enabled Then
                                    .VisitCodeGroupID = IIf(cboVisitCodeGroup.DropDownList.SelectedValue.Length > 0, cboVisitCodeGroup.DropDownList.SelectedValue, Nothing)
                                End If
                            End If
                        End If
                        If cboVisitCodeGroup.Visible Then cboVisitCodeGroup.DropDownList.SelectedValue = IIf(.VisitCodeGroupID <= 0, "", .VisitCodeGroupID)

                        If _inUse And cboManAmendGroup.Visible Then
                            AddRedundantManAmendInd(.DomManuallyAmendedIndicatorGroupID)
                            cboManAmendGroup.DropDownList.SelectedValue = .DomManuallyAmendedIndicatorGroupID
                        Else
                            If cboManAmendGroup.Visible Then .DomManuallyAmendedIndicatorGroupID = cboManAmendGroup.DropDownList.SelectedValue
                        End If

                        '' if visit code group is being used by some proforma invoices
                    If Not cboManAmendGroup.GetPostBackValue() Is Nothing _
                        AndAlso Not String.IsNullOrEmpty(cboManAmendGroup.GetPostBackValue()) _
                        AndAlso cboManAmendGroup.GetPostBackValue() <> .DomManuallyAmendedIndicatorGroupID And _inUse Then
                        lblError.Text = "The requested changes to the Manually Amended Indicator Group history is not permitted as this would invalidate one or more Pro forma / Provider Invoice."
                        e.Cancel = True
                        Return
                    End If

                        '' if visit code group is being used by some proforma invoices
                    If Not cboVisitCodeGroup.GetPostBackValue() Is Nothing _
                        AndAlso Not String.IsNullOrEmpty(cboVisitCodeGroup.GetPostBackValue()) _
                        AndAlso cboVisitCodeGroup.GetPostBackValue() <> .VisitCodeGroupID And _inUse Then
                        lblError.Text = "The requested changes to the Visit Code Group history is not permitted as this would invalidate non-Delivery of service data recorded against one or more Pro forma / Provider Invoice."
                        e.Cancel = True
                        Return
                    End If

                    If _inUse And cboServiceOutcomeGroup.Visible Then
                        AddRedundantServiceOutcomeGroup(.ServiceOutcomeGroupID)
                    Else
                        If cboServiceOutcomeGroup.Visible Then
                            If cboServiceOutcomeGroup.Enabled Then
                                .ServiceOutcomeGroupID = Utils.ToInt32(cboServiceOutcomeGroup.DropDownList.SelectedValue)
                            End If
                        End If
                    End If
                    If cboServiceOutcomeGroup.Visible Then cboServiceOutcomeGroup.DropDownList.SelectedValue = IIf(.ServiceOutcomeGroupID <= 0, "", .ServiceOutcomeGroupID)

                    Decimal.TryParse(txtContractedUnits.Text, .ContractedUnits)
                    If .ContractedUnits = 0 Then .ContractedUnits = Nothing

                    If Not _visitBasedReturns Then
                        .VisitBasedPlans = False
                        chkVisitBasedPlans.CheckBox.Checked = .VisitBasedPlans
                    Else
                        .VisitBasedPlans = chkVisitBasedPlans.CheckBox.Checked
                    End If

                    If _inUseByNonSpecifyDoWDso Then
                        .SpecifyDayOfWeekOnDsoPlan = False
                        chkSpecifyPlannedDoW.CheckBox.Checked = .SpecifyDayOfWeekOnDsoPlan
                    ElseIf chkVisitBasedPlans.CheckBox.Checked Then
                        .SpecifyDayOfWeekOnDsoPlan = True
                        chkSpecifyPlannedDoW.CheckBox.Checked = .SpecifyDayOfWeekOnDsoPlan
                    Else
                        .SpecifyDayOfWeekOnDsoPlan = chkSpecifyPlannedDoW.CheckBox.Checked
                    End If

                    ' get the provider invoice method from the  form
                    .ProviderInvoiceInputMethod = GetProviderInvoiceInputMethod()
                    .AutoSetSecondaryVisit = chkAutoSetSecondryVisit.Checked

                    .PaymentRequests = GetPaymentRequestType(.ProviderInvoiceInputMethod)

                    If .PaymentRequests = PaymentRequestType.CouncilStaff Then
                        .PaymentRequestMinPeriod = ddPrMinPayPeriod.DropDownList.SelectedValue
                    Else
                        .PaymentRequestMinPeriod = 0
                    End If

                    .PaymentRequestsUseProviderEmail = cbPrProviderEmail.CheckBox.Checked

                    If .PaymentRequestsUseProviderEmail = TriState.True Then
                        .PaymentRequestsCustomEmail = String.Empty
                    Else
                        .PaymentRequestsCustomEmail = tbPrProviderEmail.Text
                    End If

                    msg = DomContractBL.SaveContractPeriod(Me.DbConnection, period, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    If Not msg.Success Then
                        If msg.Number = "E3007" Then    ' could not save period
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                        Exit Sub
                    End If

                    If e.ItemID = 0 Then
                        msg = DomContractBL.CopyPreviousPeriod(Me.DbConnection, period)
                    End If

                    lblError.Text = String.Empty
                    e.ItemID = .ID
                    _periodID = .ID
                    _refreshTree = True
                    SetupReport(e.ItemID)
                End With
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub PopulateDropdowns(ByVal selectedServiceOutcomeGroupId As Integer)

            Dim msg As ErrorMessage
            Dim vcGroups As DomVisitCodeGroupCollection = Nothing
            Dim maGroups As DomManuallyAmendedIndicatorGroupCollection = Nothing
            Dim soGroups As ServiceOutcomeGroupCollection = Nothing
            Dim rateCategories As DomRateCategoryCollection = Nothing

            With cboVisitCodeGroup
                ' get a list of non-redundant groups
                msg = DomVisitCodeGroup.FetchList(Me.DbConnection, vcGroups, String.Empty, String.Empty, TriState.False)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = vcGroups
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

            With cboManAmendGroup
                ' get a list of non-redundant groups
                msg = DomManuallyAmendedIndicatorGroup.FetchList(Me.DbConnection, maGroups, String.Empty, String.Empty, TriState.False)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = maGroups
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

            With cboServiceOutcomeGroup
                msg = ServiceOutcomeGroup.FetchList(Me.DbConnection, soGroups, String.Empty, String.Empty, TriState.False)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = soGroups
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                    If selectedServiceOutcomeGroupId > 0 AndAlso .Items.FindByValue(selectedServiceOutcomeGroupId) Is Nothing Then
                        Dim selectedServiceOutcomeGroup As New ServiceOutcomeGroup(auditLogTitle:=String.Empty, auditUserName:=String.Empty, conn:=DbConnection)
                        msg = selectedServiceOutcomeGroup.Fetch(selectedServiceOutcomeGroupId)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        .Items.Add(New ListItem(selectedServiceOutcomeGroup.Description, selectedServiceOutcomeGroup.ID))
                    End If
                End With
            End With

            With ddPrMinPayPeriod
                Dim frequencies As LookupCollection = Nothing
                msg = Lookup.FetchList(Me.DbConnection, frequencies, "WEEKLYFREQUENCY")
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = frequencies
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty, 0))
                End With
            End With

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim script As StringBuilder = New StringBuilder()

            If _refreshTree Then
                script.AppendFormat("window.parent.RefreshTree({0}, 'p', {1});", _contractID, _periodID)
            End If

            script.AppendFormat("Period_useEnhancedRateDays={0};", _useEnhancedRateDays.ToString().ToLower())
            script.AppendFormat("Period_inUseByNonSpecifyDoWDso={0};", _inUseByNonSpecifyDoWDso.ToString().ToLower())
            script.AppendFormat("Period_FrameworkTypeId={0};", _framework.FrameworkTypeId)

            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", script.ToString(), True)

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim msg As New ErrorMessage()

            Dim contract As DomContract

            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            With contract
                msg = .Fetch(_contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End With

            Dim lastPeriod As DomContractPeriod = Nothing
            If contract.ContractType = DomContractType.BlockPeriodic.ToString Then
                GetLastPeriod(msg, lastPeriod)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            _inUse = False
            If _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                ' check if the period is in use by any kind of visit
                msg = DomContractBL.ContractPeriodInUseByVisit(Me.DbConnection, _periodID, _inUse)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If _inUse Then
                    cboManAmendGroup.DropDownList.Enabled = False
                    cboVisitCodeGroup.DropDownList.Enabled = False
                End If
            End If

            With chkVisitBasedPlans.CheckBox
                If Not _visitBasedReturns Then
                    .Checked = False
                Else
                    .InputAttributes.Add("onclick", "chkVisitBasedPlans_Click();")
                End If
            End With

            With chkSpecifyPlannedDoW.CheckBox
                If _inUseByNonSpecifyDoWDso Then
                    .Checked = False
                    .Enabled = False
                ElseIf chkVisitBasedPlans.CheckBox.Checked Then
                    .Checked = True
                    .Enabled = False
                ElseIf _useEnhancedRateDays AndAlso _periodID = 0 Then
                    .Checked = True
                End If
                .InputAttributes.Add("onclick", "chkSpecifyPlannedDoW_Click();")
            End With

            With dteDateTo
                .Enabled = False
            End With

            If _isFirstPeriodForContract Then
                With dteDateFrom
                    .Enabled = False
                    .Text = _contractStartDate
                End With
            End If

            If contract.ContractType = DomContractType.BlockPeriodic.ToString And _stdBut.ButtonsMode = Target.Library.Web.UserControls.StdButtonsMode.AddNew Then

                If Not lastPeriod Is Nothing AndAlso lastPeriod.DateTo < DataUtils.MAX_DATE Then
                    With dteDateFrom
                        .Enabled = False
                        .Text = lastPeriod.DateTo.AddDays(1)
                    End With
                Else
                    If Not lastPeriod Is Nothing Then
                        With dteDateFrom
                            .Text = lastPeriod.DateFrom.AddDays(1)
                        End With
                    End If
                End If

            End If

            If contract.ContractType = DomContractType.BlockPeriodic.ToString Then
                dteDateFrom.Enabled = False
            End If

            _stdBut.Visible = Not _contractEnded

            ' disable as default all rbs for payment requests
            rbInvCreatedFromServiceRegisters.Enabled = False
            rbInvCreatedSummaryProForma.Enabled = False
            rbInvCreatedVisitProForma.Enabled = False
            rbInvManuallyEntered.Enabled = False
            rbInvNotEntered.Enabled = False

            'Controls Matrix - dependent on Framework Type
            Select Case _framework.FrameworkTypeId
                Case FrameworkTypes.ElectricMonitoring

                    pnlManAmendGroup.Visible = True
                    pnlVisitBasedReturns.Visible = True
                    pnlVisitCodeGroup.Visible = True
                    pnlServiceOutcomeGroup.Visible = False

                    If _stdBut.ButtonsMode = StdButtonsMode.Edit _
                        OrElse _stdBut.ButtonsMode = StdButtonsMode.AddNew Then

                        rbInvManuallyEntered.Enabled = True
                        rbInvCreatedVisitProForma.Enabled = True
                        rbInvCreatedSummaryProForma.Enabled = True
                        rbInvNotEntered.Enabled = True

                        If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then

                            rbInvCreatedVisitProForma.Checked = True

                        End If

                    End If

                Case FrameworkTypes.ServiceRegister

                    pnlManAmendGroup.Visible = False
                    pnlVisitBasedReturns.Visible = False
                    pnlVisitCodeGroup.Visible = False
                    pnlServiceOutcomeGroup.Visible = True

                    If _stdBut.ButtonsMode = StdButtonsMode.Edit _
                        OrElse _stdBut.ButtonsMode = StdButtonsMode.AddNew Then

                        rbInvManuallyEntered.Enabled = True
                        rbInvCreatedFromServiceRegisters.Enabled = True
                        rbInvCreatedSummaryProForma.Enabled = True
                        rbInvNotEntered.Enabled = True

                        If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then

                            rbInvCreatedFromServiceRegisters.Checked = True

                        End If

                    End If

                Case FrameworkTypes.CommunityGeneral

                    Dim rcs As DomRateCategoryCollection = Nothing

                    pnlManAmendGroup.Visible = False
                    pnlVisitBasedReturns.Visible = False
                    pnlVisitCodeGroup.Visible = True
                    pnlServiceOutcomeGroup.Visible = True

                    If _stdBut.ButtonsMode = StdButtonsMode.Edit _
                        OrElse _stdBut.ButtonsMode = StdButtonsMode.AddNew Then

                        rbInvManuallyEntered.Enabled = True
                        rbInvCreatedSummaryProForma.Enabled = True
                        rbInvNotEntered.Enabled = True

                        With cboVisitCodeGroup
                            .Enabled = FrameworkHasAtLeastOneTimeBasedUom
                            .RequiredValidator.Enabled = False
                        End With

                        With cboServiceOutcomeGroup
                            .Enabled = FrameworkHasAtLeastNonTimeBasedUomWithServiceType
                            .RequiredValidator.Enabled = False
                        End With

                        If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then

                            rbInvManuallyEntered.Checked = True

                        End If

                    End If

            End Select

            '++ Contracts of type 'Block Guarantee' are limited to the following invoice input methods only:
            '++     - Provider invoices created by summary-level pro forma invoices
            '++     - No provider invoices on contract
            If _contractID > 0 Then
                Dim contractRec As DomContract = New DomContract(Me.DbConnection, "", "")

                msg = contractRec.Fetch(_contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If contractRec.ContractType = DomContractType.BlockGuarantee.ToString() Or contractRec.ContractType = DomContractType.BlockPeriodic.ToString() Then
                    rbInvManuallyEntered.Enabled = False
                    rbInvCreatedFromServiceRegisters.Enabled = False
                    rbInvCreatedVisitProForma.Enabled = False
                    chkAutoSetSecondryVisit.Enabled = False
                    rbInvCreatedSummaryProForma.Enabled = True
                    rbInvNotEntered.Enabled = True

                    If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                        rbInvCreatedSummaryProForma.Checked = True
                    End If

                    If contractRec.ContractType = DomContractType.BlockPeriodic.ToString() Then
                        rbInvNotEntered.Enabled = False
                    End If
                End If
            End If

            '++  Contracts of type 'Periodic Block' are limited to the following invoice input methods only:
            '++     - Provider invoices created by summary-level pro forma invoices
            '++     - No provider invoices on contract
            If _contractID > 0 Then
                Dim contractRec As DomContract = New DomContract(Me.DbConnection, "", "")

                msg = contractRec.Fetch(_contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If contractRec.ContractType = "BlockGuarantee" Then
                    rbInvManuallyEntered.Enabled = False
                    rbInvCreatedFromServiceRegisters.Enabled = False
                    rbInvCreatedVisitProForma.Enabled = False
                    chkAutoSetSecondryVisit.Enabled = False
                    rbInvCreatedSummaryProForma.Enabled = True
                    rbInvNotEntered.Enabled = True

                    If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                        rbInvCreatedSummaryProForma.Checked = True
                    End If
                End If
            End If

        End Sub

        Private Sub AddRedundantVisitCodeGroup(ByVal visitCodeGroupID As Integer)

            Dim msg As ErrorMessage
            Dim vcGroup As DomVisitCodeGroup

            If cboVisitCodeGroup.DropDownList.Items.FindByValue(visitCodeGroupID) Is Nothing Then
                ' the current visit code group is redundant so we need to add it in
                vcGroup = New DomVisitCodeGroup(Me.DbConnection, String.Empty, String.Empty)
                msg = vcGroup.Fetch(visitCodeGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                cboVisitCodeGroup.DropDownList.Items.Add(New ListItem(vcGroup.Description, vcGroup.ID))
            End If

        End Sub

        Private Sub AddRedundantManAmendInd(ByVal manAmendIndID As Integer)

            Dim msg As ErrorMessage
            Dim maGroup As DomManuallyAmendedIndicatorGroup

            If cboManAmendGroup.DropDownList.Items.FindByValue(manAmendIndID) Is Nothing Then
                ' the current man amend group is redundant so we need to add it in
                maGroup = New DomManuallyAmendedIndicatorGroup(Me.DbConnection, String.Empty, String.Empty)
                msg = maGroup.Fetch(manAmendIndID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                cboManAmendGroup.DropDownList.Items.Add(New ListItem(maGroup.Description, maGroup.ID))
            End If

        End Sub

        Private Sub AddRedundantServiceOutcomeGroup(ByVal serviceOutcomeGroupID As Integer)

            Dim msg As ErrorMessage
            Dim soGroup As ServiceOutcomeGroup

            If cboServiceOutcomeGroup.DropDownList.Items.FindByValue(serviceOutcomeGroupID) Is Nothing Then
                ' the current service outcome group is redundant so we need to add it in
                soGroup = New ServiceOutcomeGroup(Me.DbConnection, String.Empty, String.Empty)
                msg = soGroup.Fetch(serviceOutcomeGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                cboServiceOutcomeGroup.DropDownList.Items.Add(New ListItem(soGroup.Description, soGroup.ID))
            End If

        End Sub

        Private Sub SetupReport(ByVal periodID As Integer)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            With _stdBut
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ContractPeriod")
                .ReportButtonText = "Print"
                .ReportButtonParameters.Add("intUserID", currentUser.ID)
                .ReportButtonParameters.Add("intDomContractPeriodID", periodID)
            End With
        End Sub

        ''' <summary>
        ''' Gets the provider invoice input method.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetProviderInvoiceInputMethod() As ProviderInvoiceInputMethod

            If rbInvCreatedFromServiceRegisters.Checked = True Then

                Return ProviderInvoiceInputMethod.InvoicesCreatedFromServiceRegisters

            ElseIf rbInvCreatedSummaryProForma.Checked = True Then

                Return ProviderInvoiceInputMethod.InvoicesCreatedFromSummaryLevelProFormaInvoices

            ElseIf rbInvCreatedVisitProForma.Checked = True Then

                Return ProviderInvoiceInputMethod.InvoicesCreatedFromVisitBasedProFormaInvoices

            ElseIf rbInvManuallyEntered.Checked = True Then

                Return ProviderInvoiceInputMethod.InvoicesManuallyEnteredInIntranet

            Else

                Return ProviderInvoiceInputMethod.InvoicesNotEntered

            End If

        End Function

        ''' <summary>
        ''' Sets the provider invoice input method.
        ''' </summary>
        ''' <param name="method">The method.</param>
        Private Sub SetProviderInvoiceInputMethod(ByVal method As ProviderInvoiceInputMethod)

            Select Case method

                Case ProviderInvoiceInputMethod.InvoicesCreatedFromServiceRegisters

                    rbInvCreatedFromServiceRegisters.Checked = True

                Case ProviderInvoiceInputMethod.InvoicesCreatedFromSummaryLevelProFormaInvoices

                    rbInvCreatedSummaryProForma.Checked = True

                Case ProviderInvoiceInputMethod.InvoicesCreatedFromVisitBasedProFormaInvoices

                    rbInvCreatedVisitProForma.Checked = True

                Case ProviderInvoiceInputMethod.InvoicesManuallyEnteredInIntranet

                    rbInvManuallyEntered.Checked = True

                Case Else

                    rbInvNotEntered.Checked = True

            End Select

        End Sub

        ''' <summary>
        ''' Gets a value indicating whether [framework has at least one time based uom].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [framework has at least one time based uom]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property FrameworkHasAtLeastOneTimeBasedUom() As Boolean
            Get
                If _FrameworkHasAtLeastOneTimeBasedUom.HasValue = False AndAlso _frameworkId > 0 Then
                    Dim msg As New ErrorMessage()
                    Dim frameworksUoms As DomUnitsOfMeasureCollection = Nothing
                    msg = DomContractBL.GetDomUnitsOfMeasureForDomRateFrameworkID(DbConnection, _frameworkId, frameworksUoms, Nothing)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    _FrameworkHasAtLeastOneTimeBasedUom = False
                    If (From tmpUom In frameworksUoms.ToArray() _
                            Where tmpUom.MinutesPerUnit > 0 _
                        Select tmpUom).ToList().Count > 0 Then
                        _FrameworkHasAtLeastOneTimeBasedUom = True
                    End If
                End If
                Return _FrameworkHasAtLeastOneTimeBasedUom.Value
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [framework has at least non time based uom with service type].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [framework has at least non time based uom with service type]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property FrameworkHasAtLeastNonTimeBasedUomWithServiceType() As Boolean
            Get
                If _FrameworkHasAtLeastNonTimeBasedUomWithServiceType.HasValue = False AndAlso _frameworkId > 0 Then
                    Dim msg As New ErrorMessage()
                    Dim frameworksUoms As DomUnitsOfMeasureCollection = Nothing
                    msg = DomContractBL.GetDomUnitsOfMeasureForDomRateFrameworkID(DbConnection, _frameworkId, frameworksUoms, True)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    _FrameworkHasAtLeastNonTimeBasedUomWithServiceType = False
                    If (From tmpUom In frameworksUoms.ToArray() _
                            Where tmpUom.MinutesPerUnit = 0 _
                        Select tmpUom).ToList().Count > 0 Then
                        _FrameworkHasAtLeastNonTimeBasedUomWithServiceType = True
                    End If
                End If
                Return _FrameworkHasAtLeastNonTimeBasedUomWithServiceType.Value
            End Get
        End Property

        ''' <summary>
        ''' Used to determine the payment request type
        ''' </summary>
        ''' <param name="method"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetPaymentRequestType(ByVal method As ProviderInvoiceInputMethod) As PaymentRequestType
            If method = ProviderInvoiceInputMethod.InvoicesCreatedFromSummaryLevelProFormaInvoices Then
                If rbPrCareProvRequestPayments.Checked Then
                    Return PaymentRequestType.CareProvider
                ElseIf rbPrCouncilStaffRequestPayments.Checked Then
                    Return PaymentRequestType.CouncilStaff
                Else
                    Return PaymentRequestType.NotPermitted
                End If
            Else
                Return PaymentRequestType.NotPermitted
            End If
        End Function

        ''' <summary>
        ''' Procedure to set the payment request type on the screen
        ''' </summary>
        ''' <param name="method"></param>
        ''' <param name="PRType"></param>
        ''' <remarks></remarks>
        Private Sub SetPaymentRequestType(ByVal method As ProviderInvoiceInputMethod, ByVal PRType As PaymentRequestType)
            If method = ProviderInvoiceInputMethod.InvoicesCreatedFromSummaryLevelProFormaInvoices Then
                Select Case PRType
                    Case PaymentRequestType.CareProvider
                        rbPrCareProvRequestPayments.Checked = True
                    Case PaymentRequestType.CouncilStaff
                        rbPrCouncilStaffRequestPayments.Checked = True
                    Case PaymentRequestType.NotPermitted
                        rbPrCareProvRequestPayments.Checked = False
                        rbPrCouncilStaffRequestPayments.Checked = False
                End Select
            Else
                rbPrCareProvRequestPayments.Checked = False
                rbPrCouncilStaffRequestPayments.Checked = False
            End If

        End Sub

        Private Sub GetLastPeriod(ByRef msg As ErrorMessage, ByRef lastPeriod As DomContractPeriod)

            msg = DomContractBL.FetchLastDomContractPeriod(Me.DbConnection, Nothing, _contractID, lastPeriod)
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

    End Class

End Namespace