
Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.AdministrativeSector

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Screen used to maintain a domiciliary contract header.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JAF      29/11/2013  Add handling for read-only Start Date and/or Contract Type, i.e. when linked invs found (#8355)
    ''' MoTahir  25/10/2013  A8280 - A8620 - was completed to disable functionality of D12459A, this was due to outstanding issues around this development.  
    '''                      Now that those outstanding issues have been resolved, 
    '''                      the changes in A8260 are now being backed out so that D12459A functionality is now enabled.
    ''' MoTahir  21/10/2013  A8263 -  Modify Contract Type selector
    ''' JAF      18/10/2013  Remove contract type 'Block Guarantee' if non-Block Guarantee contract has invoices (#8259)
    ''' MoTahir  17/10/2013  Disable D12459A (A8260)
    ''' MoTahir  17/09/2013  Block Contract Payment Plan (D12459)
    ''' JAF      30/08/2013  For Block Guarantee contracts, prevent change of contract type where related invs found (D12503)
    ''' MoTahir  15/11/2012  D12343 - Remove Framework Type From Service Group
    ''' ColinD   14/10/2011  I183 - Removed the need to set VisitBasedReturns manually, this value is now set based on the current FrameworkType
    ''' PaulW    29/06/2010  D11795 - SDS, Generic Contracts and Service Orders
    ''' MvO      26/03/2010  A4WA#6163 - use Me.Settings instead of ApplicationSetting.FetchList().
    ''' MvO      14/09/2009  D11602 - menu improvements.
    ''' MvO      24/04/2009  A4WA#5395 - present friendly error when deleting in use contract.
    ''' MvO      07/04/2009  D11537 - need to suppress Csrf check due to use of iframe.
    ''' MvO      13/01/2009  D11490 - support for provider unit cost override.
    ''' MvO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class Header
        Inherits BasePage

        Private _contractID As Integer
        Private _copyFromID As Integer
        Private _estabID As Integer
        Private _contractType As DomContractType
        Private _contractGroupID As DomContractType
        Private _verificationTextID As Integer
        Private _refreshTree As Boolean
        Private _sysInfoDefaultChargeSU As Boolean
        Private _stdBut As StdButtonsBase
        Private _contractEnded As Boolean
        Private _svcGroup As ServiceGroup = Nothing
        Private _frType As FrameworkType = Nothing
        Private _contract As DomContract = Nothing
        Private _svcGroupID As Integer = 0
        Private _administrativeSectorID As Integer
        Private _contractTypeValid As Boolean = True

        Const SETTING_AUTO_GENERATE_CONTRACT_NOS As String = "AutoGenerateContractNos"
        Const ERROR_NO_MONEY_TYPE_DUM As String = "Rate Framework for {0} contracts must contain at least one rate category measured in {1}"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .hideCreditorRef = True
                .hideDebtorRef = True
            End With
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Header")

            _contractID = Utils.ToInt32(Request.QueryString("id"))
            _copyFromID = Utils.ToInt32(Request.QueryString("copyFromID"))
            _estabID = Utils.ToInt32(Request.QueryString("estabID"))
            _contractType = Utils.ToInt32(Request.QueryString("ctID"))
            _contractGroupID = Utils.ToInt32(Request.QueryString("cgID"))
            _verificationTextID = Utils.ToInt32(Request.QueryString("vtID"))
            _svcGroupID = Utils.ToInt32(Request.QueryString("svcGroupID"))
            _administrativeSectorID = Utils.ToInt32(Request.QueryString("asID"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.AddNew"))
                .ShowNew = False
                .AllowFind = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Delete"))
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

            'Contract type enum to javascript
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomContractType))

            Me.JsLinks.Add("Header.js")

            _sysInfoDefaultChargeSU = DataCache.SystemInfo(Me.DbConnection, Nothing, False).ChargeServiceUsersForAdditionalCarers

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            If _svcGroupID > 0 Then
                _svcGroup = New ServiceGroup(Me.DbConnection, String.Empty, String.Empty)
                Msg = _svcGroup.Fetch(_svcGroupID)
                If Not Msg.Success Then WebUtils.DisplayError(Msg)
                txtServiceGroup.Text = _svcGroup.Description
            End If

            If _contractID > 0 Then
                msg = DomContractBL.FetchDomContract(Me.DbConnection, _contractID, _contract)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                msg = DomContractBL.FetchFrameWorkTypeByContract(Me.DBConnection, _contract.DomRateFrameworkID, _frType)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            If _copyFromID = 0 Then
                ' creating a brand new contract
                PopulateDropdowns(True, _svcGroupID)
                cboContractType.DropDownList.SelectedValue = Convert.ToInt32(_contractType)
                cboVerificationText.DropDownList.SelectedValue = _verificationTextID
                cboContractGroup.DropDownList.SelectedValue = _contractGroupID
                cboAdministrativeArea.DropDownList.SelectedValue = _administrativeSectorID
                SetupProviderSelector(_estabID)
                SetupClientSelector(0)
                chkChargeSU.CheckBox.Checked = _sysInfoDefaultChargeSU
                txtAltRef.Text = String.Empty
            Else
                ' copying from an existing contract
                Dim contract As DomContract
                Dim framework As DomRateFramework

                contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                With contract
                    msg = .Fetch(_copyFromID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    _svcGroupID = .ServiceGroupID

                    _svcGroup = New ServiceGroup(Me.DbConnection, String.Empty, String.Empty)
                    msg = _svcGroup.Fetch(_svcGroupID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtServiceGroup.Text = _svcGroup.Description

                    msg = DomContractBL.FetchFrameWorkTypeByContract(Me.DBConnection, .DomRateFrameworkID, _frType)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    PopulateDropdowns(False, _svcGroupID)
                    PopulateCopyingFrom(.Number, .Title, .Description)

                    ' don't copy number/title/desc/end date/end reason/svc user/start date
                    SetupProviderSelector(.ProviderID)
                    cboContractType.DropDownList.SelectedValue = Convert.ToInt32([Enum].Parse(GetType(DomContractType), .ContractType))
                    SetupClientSelector(0)
                    cboContractGroup.DropDownList.SelectedValue = .GenericContractGroupID
                    cboAdministrativeArea.DropDownList.SelectedValue = .AdministrativeSectorID
                    cboVerificationText.DropDownList.SelectedValue = .VerificationTextID
                    chkBankHolidayCover.CheckBox.Checked = .BankHolidayCover
                    ' rate framework is readonly
                    framework = New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
                    msg = framework.Fetch(.DomRateFrameworkID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    cboRateFramework.IsReadOnly = True
                    cboRateFramework.Text = framework.Description
                    chkUseEnhancedRateDays.CheckBox.Checked = .UseEnhancedRateDays
                    chkBankHolidayCover.CheckBox.Checked = .BankHolidayCover
                    chkChargeSU.CheckBox.Checked = .ChargeServiceUsersForAdditionalCarers

                    txtAltRef.Text = String.Empty
                End With

            End If

        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim contract As DomContract

            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            With contract
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If .EndDate = DataUtils.MAX_DATE Then
                    FindClicked(e)
                Else
                    lblError.Text = "Unable to edit this contract, the contract has been terminated."
                    e.Cancel = True
                End If

            End With
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim contract As DomContract
            Dim endReason As ContractEndReason
            Dim framework As DomRateFramework

            If e.ItemID > 0 Then
                contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                With contract
                    msg = .Fetch(e.ItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    If contract.ContractType = String.Empty Then .ContractType = DomContractType.Unknown.ToString

                    _svcGroupID = .ServiceGroupID
                    PopulateDropdowns(False, .ServiceGroupID)

                    txtNumber.Text = .Number
                    txtTitle.Text = .Title
                    txtDescription.Text = .Description
                    SetupProviderSelector(.ProviderID)
                    dteStartDate.Text = .StartDate
                    If .EndDate <> DataUtils.MAX_DATE Then
                        dteEndDate.Text = .EndDate
                        endReason = New ContractEndReason(Me.DbConnection, String.Empty, String.Empty)
                        msg = endReason.Fetch(.ContractEndReasonID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        txtEndReason.Text = endReason.Description
                        _contractEnded = True
                    End If

                    '++ Does this contract have associated pro forma and/or provider invoices..?
                    '++ If so, the Contract Type cannot be changed from/to 'Block Guarantee'..
                    Dim hasProFormaInvs As Boolean
                    Dim hasProviderInvs As Boolean

                    msg = DomContractBL.ContractHasAssociatedProformaOrProviderInvoice(Nothing, Me.DbConnection, hasProviderInvs, hasProFormaInvs, .ID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    cboContractType.DropDownList.SelectedValue = Convert.ToInt32([Enum].Parse(GetType(DomContractType), .ContractType))
                    dteStartDate.IsReadOnly = (hasProFormaInvs OrElse hasProviderInvs)

                    SetupClientSelector(.ClientID)
                    cboContractGroup.DropDownList.SelectedValue = .GenericContractGroupID
                    cboVerificationText.DropDownList.SelectedValue = .VerificationTextID
                    chkBankHolidayCover.CheckBox.Checked = .BankHolidayCover
                    cboAdministrativeArea.DropDownList.SelectedValue = .AdministrativeSectorID

                    If _svcGroupID > 0 Then
                        _svcGroup = New ServiceGroup(Me.DbConnection, String.Empty, String.Empty)
                        msg = _svcGroup.Fetch(_svcGroupID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        txtServiceGroup.Text = _svcGroup.Description
                    End If

                    msg = DomContractBL.FetchFrameWorkTypeByContract(Me.DbConnection, .DomRateFrameworkID, _frType)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' rate framework
                    framework = New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
                    msg = framework.Fetch(.DomRateFrameworkID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    cboRateFramework.IsReadOnly = True
                    cboRateFramework.Text = framework.Description

                    chkUseEnhancedRateDays.CheckBox.Checked = .UseEnhancedRateDays
                    chkChargeSU.CheckBox.Checked = .ChargeServiceUsersForAdditionalCarers
                    chkDsoMaintExternal.CheckBox.Checked = .DSOsMaintainedViaElectronicInterface
                    chkProviderUnitCostOverride.CheckBox.Checked = .AllowProviderUnitCostOverride

                    txtAltRef.Text = Utils.ToString(.AltReference)
                End With

                PopulateWarnings(contract)
            End If

            If Not _contractTypeValid Then cboContractType.SelectPostBackValue()

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            If _svcGroupID > 0 Then
                _svcGroup = New ServiceGroup(Me.DbConnection, String.Empty, String.Empty)
                Msg = _svcGroup.Fetch(_svcGroupID)
                If Not Msg.Success Then WebUtils.DisplayError(Msg)
                txtServiceGroup.Text = _svcGroup.Description
            End If

            If _contractID > 0 Then
                msg = DomContractBL.FetchDomContract(Me.DbConnection, _contractID, _contract)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                msg = DomContractBL.FetchFrameWorkTypeByContract(Me.DBConnection, _contract.DomRateFrameworkID, _frType)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            If e.ItemID = 0 Then
                PopulateDropdowns(True, _svcGroupID)
                txtNumber.Text = String.Empty
                txtTitle.Text = String.Empty
                txtDescription.Text = String.Empty
                SetupProviderSelector(0)
                dteStartDate.Text = String.Empty
                cboContractType.DropDownList.SelectedValue = 0
                SetupClientSelector(0)
                cboContractGroup.DropDownList.SelectedValue = 0
                cboAdministrativeArea.DropDownList.SelectedValue = 0
                cboVerificationText.DropDownList.SelectedValue = 0
                chkBankHolidayCover.CheckBox.Checked = False
                chkChargeSU.CheckBox.Checked = _sysInfoDefaultChargeSU
                txtAltRef.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim contract As DomContract

            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            With contract
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If .EndDate = DataUtils.MAX_DATE Then
                    msg = DomContractBL.DeleteContract(Me.DbConnection, e.ItemID, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    If Not msg.Success Then
                        If msg.Number = DomContractBL.ERR_COULD_NOT_DELETE_CONTRACT Then    ' could not delete contract
                            lblError.Text = msg.Message
                            e.Cancel = True
                            FindClicked(e)
                        ElseIf msg.Number = "E0503" Then ' foriegn Key Constraint
                            lblError.Text = "Unable to delete this contract, the contract is in use."
                            e.Cancel = True
                            FindClicked(e)
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        _contractID = 0
                        _refreshTree = True
                    End If
                Else
                    lblError.Text = "Unable to delete this contract, the contract has been terminated."
                    e.Cancel = True
                End If

            End With

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing
            Dim contract As DomContract
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim newContract As Boolean = (e.ItemID = 0 And _copyFromID = 0)
            Dim inUse As Boolean, hasVrcs As Boolean, inUseByDsoWithProvUnitCostOverrides As Boolean, hasVsts As Boolean
            Dim vrcs As DomContractVisitRateCategoryCollection = Nothing
            Dim vsts As DomContractVisitServiceTypeCollection = Nothing
            Dim framework As DomRateFramework
            Dim dumMoneyBased As Boolean = False
            Dim hasProFormaInvs As Boolean = False
            Dim hasProviderInvs As Boolean = False
            Dim currentContract As DomContract = Nothing

            ' as we are not using viewstate because it fecks other things up, dropdowns need a bit of extra work to pass validation
            PopulateDropdowns(newContract)
            cboRateFramework.RequiredValidator.Enabled = newContract
            cboContractType.SelectPostBackValue()
            cboContractGroup.SelectPostBackValue()
            cboAdministrativeArea.SelectPostBackValue()
            cboVerificationText.SelectPostBackValue()
            If newContract Then
                cboRateFramework.SelectPostBackValue()
            End If

            'contract type validation
            If _contractID > 0 Then
                currentContract = New DomContract
                With currentContract
                    .DbConnection = Me.DbConnection
                    msg = .Fetch(_contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With

                msg = DomContractBL.ValidateContractType(Me.DbConnection, Nothing, cboContractType.DropDownList.SelectedValue, e.ItemID)
                If Not msg.Success Then
                    If msg.Number = DomContractBL.ERR_AGREED_COST Or msg.Number = DomContractBL.ERR_PROVIDER_PAYMENT _
                        Or msg.Number = DomContractBL.ERR_PAYMENT_PLAN Or msg.Number = DomContractBL.ERR_PROFORMA Then
                        _contractTypeValid = False
                        lblError.Text = String.Format(msg.Message, _
                                                      Utils.SplitOnCapitals(currentContract.ContractType), _
                                                      Utils.SplitOnCapitals([Enum].GetName(GetType(DomContractType), Convert.ToInt32(cboContractType.DropDownList.SelectedValue))))
                        e.Cancel = True
                        cboContractType.Required = False
                        cboContractType.ValidationGroup = Nothing
                        FindClicked(e)
                        Exit Sub
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                End If
            End If

            Me.Validate("Save")

            If Me.IsValid Then

                ' is contract in use?
                msg = DomContractBL.ContractInUse(Me.DbConnection, e.ItemID, inUse)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' does it have vrcs or vsts?
                msg = DomContractVisitRateCategory.FetchList(Me.DbConnection, vrcs, String.Empty, String.Empty, e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                hasVrcs = (vrcs.Count > 0)
                msg = DomContractVisitServiceType.FetchList(Me.DbConnection, vsts, String.Empty, String.Empty, e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                hasVsts = (vsts.Count > 0)

                ' is contract in use by a DSO which has overridden provider unit costs?
                msg = DomContractBL.ContractInUseByDomServiceOrder(Me.DbConnection, e.ItemID, True, inUseByDsoWithProvUnitCostOverrides)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                contract = New DomContract(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If _copyFromID > 0 Then
                    ' fetch the contract to copy from
                    With contract
                        msg = .Fetch(_copyFromID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                ElseIf e.ItemID > 0 Then
                    ' update
                    With contract
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                Else
                    ' new contracts are open-ended
                    contract.EndDate = DataUtils.MAX_DATE
                End If
                With contract

                    msg = DomContractBL.ContractHasAssociatedProformaOrProviderInvoice(Nothing, Me.DbConnection, hasProviderInvs, hasProFormaInvs, .ID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    If Not Convert.ToBoolean(Settings(ApplicationName.AbacusIntranet, SETTING_AUTO_GENERATE_CONTRACT_NOS)) Then
                        If inUse Then
                            txtNumber.Text = .Number
                        Else
                            .Number = txtNumber.Text
                        End If
                    End If

                    .Title = txtTitle.Text
                    .Description = IIf(txtDescription.Text.Trim().Length = 0, Nothing, txtDescription.Text)

                    If inUse Then
                        SetupProviderSelector(.ProviderID)
                    Else
                        .ProviderID = Utils.ToInt32(Request.Form(CType(provider, InPlaceEstablishmentSelector).HiddenFieldUniqueID))
                    End If

                    If Utils.IsDate(dteStartDate.Text) Then .StartDate = dteStartDate.Text

                    If Not String.IsNullOrEmpty(cboContractType.DropDownList.SelectedValue) Then
                        .ContractType = [Enum].GetName(GetType(DomContractType), Convert.ToInt32(cboContractType.DropDownList.SelectedValue))
                    End If

                    If inUse Then
                        SetupClientSelector(.ClientID)
                    Else
                        .ClientID = Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID))
                    End If
                    If .ClientID = 0 Then .ClientID = Nothing

                    .GenericContractGroupID = Utils.ToInt32(cboContractGroup.DropDownList.SelectedValue)
                    .VerificationTextID = Utils.ToInt32(cboVerificationText.DropDownList.SelectedValue)

                    .AdministrativeSectorID = Utils.ToInt32(cboAdministrativeArea.DropDownList.SelectedValue)

                    If newContract Then
                        ' rate framework
                        .DomRateFrameworkID = cboRateFramework.DropDownList.SelectedValue
                        'get rateframework
                        framework = New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
                        msg = framework.Fetch(.DomRateFrameworkID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        ' use enhanced rate days
                        msg = DomContractBL.RateFrameworkUsesEnhancedRateDays(Me.DbConnection, .DomRateFrameworkID, chkUseEnhancedRateDays.CheckBox.Checked)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        ' bank holiday cover
                        If chkUseEnhancedRateDays.CheckBox.Checked Then
                            .BankHolidayCover = chkBankHolidayCover.CheckBox.Checked
                        End If
                        .ServiceGroupID = _svcGroupID
                    Else
                        ' rate framework
                        framework = New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
                        msg = framework.Fetch(.DomRateFrameworkID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        cboRateFramework.IsReadOnly = True
                        cboRateFramework.Text = framework.Description

                        ' use enhanced rate days
                        msg = DomContractBL.RateFrameworkUsesEnhancedRateDays(Me.DbConnection, .DomRateFrameworkID, chkUseEnhancedRateDays.CheckBox.Checked)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        ' bank holiday cover
                        chkBankHolidayCover.CheckBox.Checked = .BankHolidayCover
                    End If

                    If framework.FrameworkTypeId = FrameworkTypes.ElectricMonitoring Then
                        .VisitBasedReturns = TriState.True
                    Else
                        .VisitBasedReturns = TriState.False
                    End If

                    .DSOsMaintainedViaElectronicInterface = chkDsoMaintExternal.CheckBox.Checked

                    If inUse Then
                        chkChargeSU.CheckBox.Checked = .ChargeServiceUsersForAdditionalCarers
                    Else
                        .ChargeServiceUsersForAdditionalCarers = chkChargeSU.CheckBox.Checked
                    End If

                    If inUseByDsoWithProvUnitCostOverrides And .AllowProviderUnitCostOverride Then
                        chkProviderUnitCostOverride.CheckBox.Checked = .AllowProviderUnitCostOverride
                    Else
                        .AllowProviderUnitCostOverride = chkProviderUnitCostOverride.CheckBox.Checked
                    End If

                    .AltReference = txtAltRef.Text
                    If cboContractType.DropDownList.SelectedValue = DomContractType.BlockPeriodic Then
                        'check if the contract framework has a rate category witht the measurement in money
                        msg = DomRateFrameworkBL.DoesRateFrameworkHaveDUMSystemType(conn:=Me.DbConnection, _
                                       tran:=Nothing, _
                                       domRateFrameworkID:=If(e.ItemID > 0, contract.DomRateFrameworkID, CInt(cboRateFramework.DropDownList.SelectedValue().ToString())), _
                                       systemType:=DomRateFrameworkBL.DomUnitOfMeasureSystemType.Money, _
                                       hasSystemType:=dumMoneyBased)

                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        If Not dumMoneyBased Then
                            lblError.Text = String.Format(ERROR_NO_MONEY_TYPE_DUM, cboContractType.DropDownList.SelectedItem.Text, _
                                     DomRateFrameworkBL.DomUnitOfMeasureSystemType.Money.ToString)
                            e.Cancel = True
                            Exit Sub
                        End If

                        'set service order maintained electronically
                        .DSOsMaintainedViaElectronicInterface = TriState.True
                    End If
                    If _copyFromID > 0 Then
                        ' copy existing contract
                        ' clear properties
                        .Unhook()
                        .EndDate = DataUtils.MAX_DATE
                        .ContractEndReasonID = Nothing
                        msg = DomContractBL.CopyContract(Me.DbConnection, _copyFromID, contract, Me.Settings)
                    Else
                        ' update or brand new
                        msg = DomContractBL.SaveContractHeader(Me.DbConnection, contract, Me.Settings)
                    End If
                    If Not msg.Success Then
                        If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_CONTRACT Then
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        e.ItemID = .ID
                        _contractID = .ID
                        txtNumber.Text = contract.Number
                        'FindClicked(e)
                        _refreshTree = True
                    End If

                End With
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub PopulateDropdowns(ByVal populateRateFramework As Boolean, Optional ByVal serviceGroupID As Integer = 0)

            Dim msg As ErrorMessage
            Dim groups As GenericContractGroupCollection = Nothing
            Dim frameworks As DomRateFrameworkCollection = Nothing
            Dim adminAreas As AdministrativeSectorCollection = Nothing
            Dim verificationtexts As VerificationTextCollection = Nothing

            ' contract type
            With cboContractType
                Dim itemList As New List(Of ListItem)
                For Each value As DomContractType In [Enum].GetValues(GetType(DomContractType))
                    Dim item As New ListItem

                    If value = DomContractType.Unknown Then
                        item.Text = String.Empty
                    Else
                        item.Text = Utils.SplitOnCapitals([Enum].GetName(GetType(DomContractType), value))
                    End If
                    item.Value = Convert.ToInt32(value).ToString
                    itemList.Add(item)
                Next
                itemList.Sort(New DropDownListEx.ListItemComparer)

                With .DropDownList.Items()
                    .Clear()
                    For Each item As ListItem In itemList
                        .Add(New ListItem(item.Text, item.Value))
                    Next
                End With
            End With

            With cboContractGroup
                ' get a list of non-redundant groups
                msg = GenericContractGroup.FetchList(Me.DbConnection, groups, String.Empty, String.Empty, TriState.False)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .SelectedIndex = -1
                    .SelectedValue = Nothing
                    .ClearSelection()
                    .DataSource = groups
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

            With cboAdministrativeArea
                ' get a list of non-redundant groups
                msg = AdministrativeAreasBL.FetchAdministrativeAreas(Nothing, Me.DbConnection, adminAreas)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .SelectedIndex = -1
                    .SelectedValue = Nothing
                    .ClearSelection()
                    .DataSource = adminAreas
                    .DataTextField = "Title"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

            With cboVerificationText
                ' get a list of non-redundant verification texts
                msg = VerificationText.FetchList(conn:=Me.DbConnection, list:=verificationtexts, isRedundant:=TriState.False, isForNonResidential:=TriState.True)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If _contractID <> 0 Then
                    Dim tempContract As DomContract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                    msg = New ErrorMessage()
                    msg = tempContract.Fetch(_contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    '' if current one is redundant but assigned then add that as well. 
                    If ((Utils.ToInt32(tempContract.VerificationTextID) <> 0) _
                          AndAlso _
                          (verificationtexts.ToArray().Where(Function(vt) vt.ID = tempContract.VerificationTextID).Count() <= 0)) Then
                        '' assigned one is made redundant so get that
                        Dim vtext As VerificationText = New VerificationText(Me.DbConnection)
                        msg = New ErrorMessage()
                        msg = vtext.Fetch(tempContract.VerificationTextID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        verificationtexts.Add(vtext)
                    End If
                End If

                With .DropDownList
                    .Items.Clear()
                    .SelectedIndex = -1
                    .SelectedValue = Nothing
                    .ClearSelection()
                    .DataSource = verificationtexts
                    .DataTextField = "Heading"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With

            End With


            If populateRateFramework Then
                With cboRateFramework
                    ' get a list of non-redundant frameworks
                    If serviceGroupID = 0 Then
                        msg = DomContractBL.FetchListRateFrameworksByServiceGroup(Me.DbConnection, Utils.ToInt32(Request.QueryString("svcGroupID")), frameworks)
                    Else
                        msg = DomContractBL.FetchListRateFrameworksByServiceGroup(Me.DbConnection, serviceGroupID, frameworks)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    With .DropDownList
                        .Items.Clear()
                        .SelectedIndex = -1
                        .SelectedValue = Nothing
                        .ClearSelection()
                        .DataSource = frameworks
                        .DataTextField = "Description"
                        .DataValueField = "ID"
                        .DataBind()
                        ' insert a blank at the top
                        .Items.Insert(0, New ListItem(String.Empty))
                    End With
                End With
            End If

        End Sub

        Private Sub SetupProviderSelector(ByVal providerID As Integer)
            With CType(Me.provider, InPlaceSelectors.InPlaceEstablishmentSelector)
                .EstablishmentID = providerID
                .Required = True
                .ValidationGroup = "Save"
            End With
        End Sub

        Private Sub SetupClientSelector(ByVal clientID As Integer)
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .ClientDetailID = clientID
                .Required = False
            End With
        End Sub

        Private Sub PopulateWarnings(ByVal contract As DomContract)

            Dim msg As ErrorMessage
            Dim warnings As List(Of String) = Nothing

            msg = DomContractBL.GetContractWarnings(Me.DbConnection, contract, warnings)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If warnings.Count = 0 Then
                fsWarnings.Visible = False
            Else
                With fsWarnings
                    .Visible = True
                    For index As Integer = 0 To warnings.Count - 1
                        Dim lit As Literal = New Literal()
                        lit.Text = warnings(index)
                        If index <> 0 Then .Controls.Add(New HtmlGenericControl("br"))
                        .Controls.Add(lit)
                    Next
                End With
            End If

        End Sub

        Private Sub PopulateCopyingFrom(ByVal number As String, ByVal title As String, ByVal description As String)
            Dim lit As Literal
            With fsCopyingFrom
                .Visible = True
                lit = New Literal()
                lit.Text = String.Format("{0}: {1}", number, title)
                .Controls.Add(lit)
                .Controls.Add(New HtmlGenericControl("br"))
                .Controls.Add(New HtmlGenericControl("br"))
                lit = New Literal()
                lit.Text = description
                .Controls.Add(lit)
            End With
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim currentMode As StdButtonsMode = CType(stdButtons1, StdButtonsBase).ButtonsMode
            Dim contractID As Integer = CType(stdButtons1, StdButtonsBase).SelectedItemID
            Dim msg As ErrorMessage
            Dim inUse As Boolean, hasVrcs As Boolean, inUseByDsoWithProvUnitCostOverrides As Boolean, hasVsts As Boolean
            Dim vrcs As DomContractVisitRateCategoryCollection = Nothing
            Dim vsts As DomContractVisitServiceTypeCollection = Nothing
            Dim startupJs As StringBuilder = New StringBuilder()
            Dim appSetting As ApplicationSettingCollection = Nothing

            ' is contract in use?
            msg = DomContractBL.ContractInUse(Me.DbConnection, contractID, inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' is contract in use by a DSO which has overridden provider unit costs?
            msg = DomContractBL.ContractInUseByDomServiceOrder(Me.DbConnection, contractID, True, inUseByDsoWithProvUnitCostOverrides)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' does it have vrcs or vsts?
            If contractID > 0 Then
                msg = DomContractVisitRateCategory.FetchList(Me.DbConnection, vrcs, String.Empty, String.Empty, contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                hasVrcs = (vrcs.Count > 0)

                msg = DomContractVisitServiceType.FetchList(Me.DbConnection, vsts, String.Empty, String.Empty, contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                hasVsts = (vsts.Count > 0)
            End If

            chkUseEnhancedRateDays.CheckBox.Enabled = False
            'chkDsoMaintExternal.CheckBox.Enabled = False

            If currentMode = StdButtonsMode.Edit Then
                chkChargeSU.CheckBox.Enabled = Not inUse
                chkBankHolidayCover.CheckBox.Enabled = chkUseEnhancedRateDays.CheckBox.Checked
            End If

            If Convert.ToBoolean(Me.Settings(ApplicationName.AbacusIntranet, DomContractBL.SETTING_AUTO_GENERATE_CONTRACT_NOS)) Then
                txtNumber.Enabled = False

                If currentMode = StdButtonsMode.AddNew Then
                    txtNumber.Text = "[Auto-Generated]"
                End If

            End If


            If inUse Then
                txtNumber.Enabled = False
                With CType(Me.provider, InPlaceEstablishmentSelector)
                    startupJs.AppendFormat("InPlaceEstablishmentSelector_Enabled('{0}', false);", .ClientID)
                End With
                With CType(Me.client, InPlaceClientSelector)
                    startupJs.AppendFormat("InPlaceClientSelector_Enabled('{0}', false);", .ClientID)
                End With
            End If

            If inUseByDsoWithProvUnitCostOverrides And chkProviderUnitCostOverride.CheckBox.Checked Then
                chkProviderUnitCostOverride.CheckBox.Enabled = False
            End If

            cboContractType.DropDownList.Attributes.Add("onchange", String.Format("cboContractType_Change('{0}');", client.ClientID))
            cboRateFramework.DropDownList.Attributes.Add("onchange", String.Format("cboRateFramework_Change();", client.ClientID))

            startupJs.Append("Init();")
            If _refreshTree Then
                startupJs.AppendFormat("window.parent.RefreshTree({0}, 'c', 0);", _contractID)
            End If

            startupJs.AppendFormat("cboContractType('{0}');", client.ClientID)

            If currentMode = StdButtonsMode.Edit Then startupJs.Append("cboRateFramework_Change();")
            startupJs.AppendFormat("contractHeader_InUse={0};", inUse.ToString().ToLower())

            _stdBut.Visible = Not _contractEnded

            ClientScript.RegisterStartupScript(Me.GetType, "Startup", startupJs.ToString(), True)

            'Controls Matrix - dependent on Framework Type
            If Not _frType Is Nothing Then
                Select Case _frType.ID
                    Case FrameworkTypes.ElectricMonitoring
                        pnlChargeSU.Visible = True
                    Case FrameworkTypes.ServiceRegister, FrameworkTypes.CommunityGeneral
                        pnlChargeSU.Visible = False
                End Select
            End If

        End Sub

    End Class
End Namespace