
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Abacus.Library.PaymentTolerance
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Admin page used to maintain domiciliary rate categories.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF   29/10/2013  Specify named parameters when fetching service types (#8286)
    ''' MoTahir 07/11/2012  D12343 - Remove Framework Type from Service Group.
    ''' ColinD  21/11/2011  I378 - Changes to control manual payment categories in a more graceful manner.
    ''' ColinD  21/11/2011  I377 - Changed SaveClicked to to use new bl function DomContractBL.SaveRateCategory when saving.
    ''' MoTahir 26/10/2011  Beta Testing Issue 237 - Behaviour of Rate Category Screen incorrect when attempting to amend Tolerance Group.
    ''' MoTahir 04/10/2011  Beta Testing Issue 120 - Domiciliary Rate Categories Error
    ''' ColinD  09/08/2011  D11965 - Numerous changes to the way in which the screen works dependant on Framework Type i.e. Electronic Monitoring or Service Registers.
    ''' Mo Tahir 04/08/2011 D11766 - Provider Invoice Tolerances
    ''' IHS     06/05/2011  A4WA#6544 - corrected SortOrder bug when saving Rate Category.
    ''' ColinD  05/01/2011  SDS427 - Altered js client side events for cboServiceType and chkVisitBasedReturns.
    ''' ColinD  13/10/2010  D11918 - altered service type drop down list to not include service types with a service category of cash or residential
    ''' Mo      19/05/2010  D11806 - Rate Category Ordering
    ''' ColinD  30/03/2010  AW4A#6184 - Prevent duplicate category descriptions being created against a framework
    ''' MikeVO  27/01/2010  D11649 - added FinanceCode fields.
    ''' MikeVO  07/12/2009  A4WA#5954 - fix to population of Service Location.
    ''' MoTahir 24/09/2009  A5803
    ''' MikeVO  10/09/2009  A4WA#5773 - corrected validation when saving in use visit-based rate category.
    ''' MoTahir 03/09/2009  D11675 - added concept of hidden fields based on framework type
    ''' MoTahir 03/09/2009  D11675 - added filtering to service types 
    ''' MoTahir 02/09/2009  D11675 - added service location and register abbreviation
    ''' MikeVO  08/01/2009  D11469 - support for units of measure.
    ''' MikeVO  06/01/2009  D11468 - support for non-visit-based services.
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  18/08/2008  D11398 - support for visit service types.
    ''' </history>
    Partial Public Class RateCategories
        Inherits BasePage

        ' constansts
        Private Const _RateFrameworkAttendance As String = "A"
        Private Const _RateFrameworkCommunityGeneral As String = "C"
        Private Const _RateFrameworkVisits As String = "V"


        ' locals
        Private _currentTimeBandID As Integer
        Private _currentDomServiceID As Integer
        Private _currentUomID As Integer
        Private _RateFramework As DomRateFramework = Nothing
        Private _RateFrameworkType As FrameworkType = Nothing
        Private _RateFrameworkTypes As FrameworkTypeCollection = Nothing
        Private _rateCategoryID As Integer
        Private _visitBasedReturns As Boolean
        Private _canAddRemove As Boolean
        Private _uomHoursId As Integer = 0
        Private _uomHoursMinsId As Integer = 0
        Private _IsRateFrameworkInUseByDomContracts As Nullable(Of Boolean) = Nothing
        Private _ptgcAllPaymentToleranceGroups As PaymentToleranceGroupCollection = Nothing
        Private _ptgType As String = Nothing
        Private _uoms As DomUnitsOfMeasureCollection = Nothing
        Private _jsoPaymentToleranceGroups As String = "[]"
        Private _jsoDomUnitsOfMeasure As String = "[]"
        Private _currentPaymentToleranceGroupID As Integer
        Private _canProviderContractOrServiceOrderBeInvalidated As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.RateCategories"), "Rate Categories")

            Dim msg As ErrorMessage

            msg = DomContractBL.CanEditRateFramework(Me.DbConnection, RateFrameworkId, _canAddRemove)

            If Not msg.Success Then WebUtils.DisplayError(msg)

            With StandardButtonsControl
                .AllowBack = True
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RateCategories.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RateCategories.Edit"))
                .AllowDelete = (Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RateCategories.Delete"))
                If Not _canAddRemove Then
                    .AllowNew = _canAddRemove
                    .AllowDelete = _canAddRemove
                End If
                .AllowFind = False
                .EditableControls.Add(fsControls.Controls)
                .AuditLogTableNames.Add("DomRateCategory")
                ' add handlers
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf EditClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
            End With

            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS
            Me.JsLinks.Add("RateCategories.js")

            ' add in the jquery library
            UseJQuery = True

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))

            ' add the payment tolerance group to use in javascript
            AjaxPro.Utility.RegisterEnumForAjax(GetType(PaymentToleranceGroupSystemTypes))

            cboServiceType.DropDownList.Attributes.Add("onchange", "cboServiceType_OnChange();")
            cboDayCategory.DropDownList.Attributes.Add("onchange", "FetchTimeBands();GetAbbreviation();PrimeValidators();")
            cboTimeBand.DropDownList.Attributes.Add("onchange", "GetAbbreviation();PrimeValidators();")
            txtConstructedAbbreviation.TextBox.ReadOnly = True
            cboUom.DropDownList.Attributes.Add("onchange", "cboUom_OnChange();")

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            PopulateDropdowns(0)
            _currentTimeBandID = 0
            _currentDomServiceID = 0
            _currentUomID = 0
            _currentPaymentToleranceGroupID = 0
            _rateCategoryID = 0
            _visitBasedReturns = False
            PopulateServiceTypes(0)
            PopulatePaymentToleranceGroups()
        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            FindClicked(e)
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim category As DomRateCategory
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim uom As DomUnitsOfMeasure = Nothing

            _rateCategoryID = e.ItemID

            category = New DomRateCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With category
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                txtRegisterGroup.Text = .RegisterGroup
                txtAbbreviationSuffix.Text = .AbbreviationSuffix
                chkManualPayments.CheckBox.Checked = .AllowUseWithManualPayments
                chkCareProviderNotPaid.CheckBox.Checked = .CareProviderNotPaid
                If RateFrameworkType.Abbreviation = _RateFrameworkCommunityGeneral Then
                    chkOneUnitPerOrder.CheckBox.Checked = .OneUnitPerOrder
                End If
                _currentDomServiceID = Utils.ToInt32(.DomServiceID)
                _currentUomID = Utils.ToInt32(.DomUnitsOfMeasureID)
                _currentPaymentToleranceGroupID = Utils.ToInt32(.PaymentToleranceGroupID)

                PopulateDropdowns(_currentUomID)

                'get the current dom unit of measure object if a Dom Unit of Measure is selected
                If _currentUomID > 0 Then
                    uom = (From u In _uoms.ToArray _
                    Where u.ID = _currentUomID _
                    Select u).FirstOrDefault()
                End If

                'set the payment tolerance group type
                If Not uom Is Nothing Then
                    If PaymentToleranceBL.IsUnitOfMeasureTimeBased(uom) Then
                        _ptgType = PaymentToleranceGroupSystemTypes.VisitBasedServiceToleranceGroup
                    Else
                        _ptgType = PaymentToleranceGroupSystemTypes.UserEnteredPaymentToleranceGroup
                    End If
                End If

                PopulatePaymentToleranceGroups()

                PopulateServiceTypes(.DomServiceTypeID)

                CType(txtService, InPlaceServiceSelector).ItemID = _currentDomServiceID
                cboUom.DropDownList.SelectedValue = IIf(_currentUomID = 0, String.Empty, _currentUomID)
                cboPaymentToleranceGroup.DropDownList.SelectedValue = IIf(_currentPaymentToleranceGroupID = 0, String.Empty, _currentPaymentToleranceGroupID)
                cboServiceType.DropDownList.SelectedValue = .DomServiceTypeID
                cboDayCategory.DropDownList.SelectedValue = .DomDayCategoryID
                _currentTimeBandID = Utils.ToInt32(.DomTimeBandID)
                cboTimeBand.DropDownList.SelectedValue = IIf(_currentTimeBandID = 0, String.Empty, .DomTimeBandID)

                CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode1
                CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode2

                'check if contracts or service orders can be invalidated by changing
                'current payment tolerance group id

                msg = PaymentToleranceBL.CanProviderContractOrServiceOrderBeInvalidatedByChangingPaymentToleranceGroup(Me.DbConnection, _
                                                                                                                       _currentPaymentToleranceGroupID, _
                                                                                                                       _canProviderContractOrServiceOrderBeInvalidated)

            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                txtAbbreviationSuffix.Text = String.Empty
                chkManualPayments.CheckBox.Checked = False
                chkCareProviderNotPaid.CheckBox.Checked = False
                CType(txtService, InPlaceServiceSelector).ItemID = 0
                cboServiceType.DropDownList.SelectedValue = String.Empty
                cboDayCategory.DropDownList.SelectedValue = String.Empty
                cboTimeBand.DropDownList.SelectedValue = String.Empty
                cboPaymentToleranceGroup.DropDownList.SelectedValue = String.Empty
                _currentTimeBandID = 0
                _currentDomServiceID = 0
                _currentUomID = 0
                _currentPaymentToleranceGroupID = 0
                _rateCategoryID = 0
                _visitBasedReturns = False
                CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
                CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            _rateCategoryID = e.ItemID

            If IsRateCategoryManualPayment AndAlso RateFramework.ManualPaymentRateCategoryID = e.ItemID Then
                ' unlink this rate category from the rate framework

                RateFramework.ManualPaymentRateCategoryID = 0
                msg = RateFramework.Save()
                If Not msg.Success Then WebUtils.DisplayError(msg)

            End If
            msg = DomRateCategory.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim category As DomRateCategory
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim inUse As Boolean, inUseByVrc As Boolean
            Dim rateCategoryVisitBased As Boolean = False

            _rateCategoryID = e.ItemID

            ' check if the rate category is in use
            inUse = RateCategoryInUse(_rateCategoryID)
            'If inUse Then
            ServiceControl.Enabled = False
            CType(txtService, InPlaceServiceSelector).Required = False
            'End If
            inUseByVrc = RateCategoryInUseByVrc(_rateCategoryID)
            If inUseByVrc Then
                cboServiceType.DropDownList.Enabled = False
                cboServiceType.RequiredValidator.Enabled = False
                cboDayCategory.DropDownList.Enabled = False
                cboDayCategory.RequiredValidator.Enabled = False
                cboTimeBand.DropDownList.Enabled = False
                cboTimeBand.RequiredValidator.Enabled = False
            End If

            If IsRateFrameworkInUseByDomContracts Then
                cboUom.DropDownList.Enabled = False
                cboServiceType.DropDownList.Enabled = False
                cboDayCategory.DropDownList.Enabled = False
                cboTimeBand.DropDownList.Enabled = False
            End If

            If IsRateCategoryManualPayment Then

                WebUtils.RecursiveDisable(fsControls.Controls, True)

                ' enable specific controls
                WebUtils.RecursiveDisable(pnlAbbreviationSuffix.Controls, False)
                'WebUtils.RecursiveDisable(pnlDescription.Controls, False)
                'WebUtils.RecursiveDisable(pnlUom.Controls, False)

                If inUse = False Then
                    WebUtils.RecursiveDisable(pnlDomService.Controls, False)
                End If
                WebUtils.RecursiveDisable(txtFinanceCode.Controls, False)
                WebUtils.RecursiveDisable(txtFinanceCode2.Controls, False)

            End If

            ' as we are not using viewstate because it fecks other things up, dropdowns need a bit of extra work to pass validation
            PopulateDropdowns(Utils.ToInt32(cboUom.GetPostBackValue()))
            PopulateServiceTypes(Utils.ToInt32(cboServiceType.GetPostBackValue()))
            PopulatePaymentToleranceGroups()

            cboServiceType.SelectPostBackValue()
            cboDayCategory.SelectPostBackValue()
            cboTimeBand.SelectPostBackValue()
            cboUom.SelectPostBackValue()
            cboPaymentToleranceGroup.SelectPostBackValue()
            category = New DomRateCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            If e.ItemID > 0 Then
                ' update
                With category
                    msg = .Fetch(e.ItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With
            End If

            rateCategoryVisitBased = False
            If Not RateFrameworkType Is Nothing Then
                rateCategoryVisitBased = (RateFrameworkType.Abbreviation = _RateFrameworkVisits)
            End If

            ' prime validators
            ' do not require any by default
            txtDescription.RequiredValidator.Enabled = False
            cboUom.RequiredValidator.Enabled = False
            cboServiceType.RequiredValidator.Enabled = False
            cboDayCategory.RequiredValidator.Enabled = False
            cboTimeBand.RequiredValidator.Enabled = False

            If Not RateFrameworkType Is Nothing Then

                Select Case RateFrameworkType.Abbreviation

                    Case _RateFrameworkVisits

                        txtDescription.RequiredValidator.Enabled = txtDescription.TextBox.Enabled
                        cboUom.RequiredValidator.Enabled = cboUom.DropDownList.Enabled
                        cboServiceType.RequiredValidator.Enabled = cboServiceType.DropDownList.Enabled
                        cboDayCategory.RequiredValidator.Enabled = cboDayCategory.DropDownList.Enabled
                        cboTimeBand.RequiredValidator.Enabled = cboTimeBand.DropDownList.Enabled

                    Case _RateFrameworkAttendance

                        cboUom.RequiredValidator.Enabled = cboUom.DropDownList.Enabled

                        cboServiceType.RequiredValidator.Enabled = cboServiceType.DropDownList.Enabled

                    Case _RateFrameworkCommunityGeneral

                        If IsRateFrameworkInUseByDomContracts Then

                            If category.DomUnitsOfMeasureID > 0 Then

                                Dim uom As New DomUnitsOfMeasure(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)

                                msg = uom.Fetch(category.DomUnitsOfMeasureID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)

                                If uom.MinutesPerUnit = 0 Then

                                    cboUom.DropDownList.Enabled = True
                                    cboUom.RequiredValidator.Enabled = True

                                ElseIf cboUom.Value <> String.Empty Then

                                    Dim selectedUom As New DomUnitsOfMeasure(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)

                                    msg = selectedUom.Fetch(Utils.ToInt32(cboUom.Value))
                                    If Not msg.Success Then WebUtils.DisplayError(msg)

                                    If uom.MinutesPerUnit = selectedUom.MinutesPerUnit Then

                                        cboUom.DropDownList.Enabled = True
                                        cboUom.RequiredValidator.Enabled = True
                                    End If

                                End If

                            End If

                        End If

                End Select

            End If

            Me.Validate("Save")

            If Me.IsValid Then

                ' check if use for manual payments and rate category is in use
                If _rateCategoryID > 0 And category.AllowUseWithManualPayments Then
                    inUse = RateCategoryInUseByManualPayment(_rateCategoryID)
                    If inUse Then chkManualPayments.CheckBox.Enabled = False
                End If

                With category
                    _rateCategoryID = e.ItemID
                    If txtDescription.TextBox.Enabled Then
                        .Description = txtDescription.Text
                    Else
                        txtDescription.Text = .Description
                    End If
                    .RegisterGroup = txtRegisterGroup.Text
                    .AbbreviationSuffix = txtAbbreviationSuffix.Text
                    If ServiceControl.Enabled Or ServiceControl.ItemID <> 0 And .DomServiceID <> ServiceControl.ItemID Then
                        .DomServiceID = ServiceControl.ItemID
                    Else
                        ServiceControl.ItemID = .DomServiceID
                    End If
                    If chkManualPayments.CheckBox.Enabled Then
                        .AllowUseWithManualPayments = chkManualPayments.CheckBox.Checked
                    Else
                        chkManualPayments.CheckBox.Checked = .AllowUseWithManualPayments
                    End If
                    If chkCareProviderNotPaid.CheckBox.Enabled Then
                        .CareProviderNotPaid = chkCareProviderNotPaid.CheckBox.Checked
                    Else
                        chkCareProviderNotPaid.CheckBox.Checked = .CareProviderNotPaid
                    End If
                    If cboServiceType.DropDownList.Enabled Then
                        .DomServiceTypeID = Utils.ToInt32(cboServiceType.DropDownList.SelectedValue)
                    ElseIf .DomServiceTypeID > 0 Then
                        cboServiceType.DropDownList.SelectedValue = .DomServiceTypeID
                    Else
                        cboServiceType.DropDownList.ClearSelection()
                    End If
                    .DomRateFrameworkID = RateFrameworkId
                    If cboDayCategory.DropDownList.Enabled Then
                        .DomDayCategoryID = Utils.ToInt32(cboDayCategory.DropDownList.SelectedValue)
                    ElseIf .DomDayCategoryID > 0 Then
                        cboDayCategory.DropDownList.SelectedValue = .DomDayCategoryID
                    Else
                        cboDayCategory.DropDownList.ClearSelection()
                    End If
                    If cboTimeBand.DropDownList.Enabled Then
                        .DomTimeBandID = Utils.ToInt32(cboTimeBand.DropDownList.SelectedValue)
                    ElseIf .DomTimeBandID > 0 Then
                        cboTimeBand.DropDownList.SelectedValue = .DomTimeBandID
                    Else
                        cboTimeBand.DropDownList.ClearSelection()
                    End If

                    If cboUom.DropDownList.Enabled = True Then
                        .DomUnitsOfMeasureID = Utils.ToInt32(cboUom.DropDownList.SelectedValue)
                    Else
                        cboUom.DropDownList.SelectedValue = .DomUnitsOfMeasureID
                    End If

                    'Payment Tolerance Groups
                    If cboPaymentToleranceGroup.DropDownList.Enabled Then
                        .PaymentToleranceGroupID = Utils.ToInt32(cboPaymentToleranceGroup.DropDownList.SelectedValue)
                    Else
                        cboPaymentToleranceGroup.DropDownList.SelectedValue = .PaymentToleranceGroupID
                    End If

                    _currentTimeBandID = .DomTimeBandID
                    _currentDomServiceID = .DomServiceID
                    _currentUomID = .DomUnitsOfMeasureID
                    _currentPaymentToleranceGroupID = .PaymentToleranceGroupID

                    .FinanceCode1 = CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText
                    .FinanceCode2 = CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText

                    ' save the rate category using bl
                    msg = DomContractBL.SaveRateCategory(DbConnection, category)
                    If Not msg.Success Then
                        ' 

                        If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_RATE_CATEGORY _
                            OrElse msg.Number = DomContractBL.ERR_RATE_CATEGORY_ABBREV_INVALID Then
                            ' if this is a known error then display on screen

                            lblError.Text = msg.Message
                            e.Cancel = True

                        Else
                            ' else this is an unknown error so throw a wobbly

                            WebUtils.DisplayError(msg)

                        End If

                    Else
                        ' if we have saved the item successfully then fetch back to interface again

                        e.ItemID = .ID
                        FindClicked(e)

                    End If

                End With

            Else

                e.Cancel = True

            End If

        End Sub

        Private Sub PopulateDropdowns(ByVal selectedUomId As Integer)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim services As DomServiceCollection = Nothing
            Dim types As DomServiceTypeCollection = Nothing
            Dim pairs As List(Of ViewablePair) = Nothing
            Dim timeBands As DomTimeBandCollection = Nothing

            ' For new rate categories filter on visit based returns.
            ' For existing it is too early to filter on visit based returns but upon edit the list is 
            ' re-populated from the web svc call anyway which is filtered properly.

            msg = DomService.FetchList(Me.DbConnection, services, TriState.False, TriState.True)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            '' service type
            'msg = DomServiceType.FetchList(Me.DbConnection, types, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), TriState.False, )
            'If Not msg.Success Then WebUtils.DisplayError(msg)
            'With cboServiceType.DropDownList
            '    .Items.Clear()
            '    .DataSource = types
            '    .DataTextField = "Description"
            '    .DataValueField = "ID"
            '    .DataBind()
            '    .Items.Insert(0, New ListItem(String.Empty))
            'End With

            ' day categories
            msg = DomContractBL.FetchDayCategoriesAvailableToRateFramework(Me.DbConnection, RateFrameworkId, pairs)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboDayCategory.DropDownList
                .Items.Clear()
                .DataSource = pairs
                .DataTextField = "Text"
                .DataValueField = "Value"
                .DataBind()
                .Items.Insert(0, New ListItem(String.Empty))
            End With

            ' time bands (fill with all time bands by default so saving on postback works, however, the javascript clears this dropdown and filters correctly)
            msg = DomTimeBand.FetchList(Me.DbConnection, timeBands, String.Empty, String.Empty)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboTimeBand.DropDownList
                .Items.Clear()
                .DataSource = timeBands
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, New ListItem(String.Empty))
            End With

            ' units of measure
            msg = DomUnitsOfMeasure.FetchList(Me.DbConnection, _uoms, String.Empty, String.Empty)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _uoms.Sort(New CollectionSorter("Description", SortDirection.Ascending))

            With cboUom.DropDownList
                .Items.Clear()
                .DataSource = _uoms
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, New ListItem(String.Empty))
            End With

            PopulateUnitsOfMeasure(selectedUomId)
        End Sub

        ''' <summary>
        ''' Populates the units of measure.
        ''' </summary>
        ''' <param name="selectedUomId">The selected uom id.</param>
        Private Sub PopulateUnitsOfMeasure(ByVal selectedUomId As Integer)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim uoms As DomUnitsOfMeasureCollection = Nothing
            Dim uomHours As DomUnitsOfMeasure = Nothing
            Dim uomHoursMins As DomUnitsOfMeasure = Nothing
            Dim selectedUom As DomUnitsOfMeasure = Nothing

            If selectedUomId > 0 Then
                ' if we have a selected uom then fetch from the db

                selectedUom = New DomUnitsOfMeasure(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                msg = selectedUom.Fetch(selectedUomId)
                If Not msg.Success Then WebUtils.DisplayError(msg)

            End If

            ' units of measure
            msg = DomUnitsOfMeasure.FetchList(Me.DbConnection, uoms, String.Empty, String.Empty)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            uoms.Sort(New CollectionSorter("Description", SortDirection.Ascending))

            ' clear the drop down and setup defaults
            With cboUom.DropDownList.Items
                .Clear()
                .Add(New ListItem(String.Empty))
            End With

            For Each uom As DomUnitsOfMeasure In uoms
                ' loop each uom

                If Not RateFrameworkType Is Nothing Then

                    Select Case RateFrameworkType.Abbreviation

                        Case _RateFrameworkVisits
                            ' visits/electronic monitoring

                            If uom.MinutesPerUnit <= 0 And Not IsRateCategoryManualPayment Then
                                Continue For
                            End If

                            If _rateCategoryID > 0 _
                                AndAlso IsRateCategoryInUse _
                                AndAlso Not selectedUom Is Nothing _
                                AndAlso selectedUom.MinutesPerUnit <> uom.MinutesPerUnit Then
                                Continue For
                            End If

                        Case _RateFrameworkAttendance
                            ' attendance/service registers

                            If uom.AllowUseWithServiceRegisters = TriState.False And Not IsRateCategoryManualPayment Then
                                Continue For
                            End If

                        Case _RateFrameworkCommunityGeneral

                            If _rateCategoryID > 0 _
                                AndAlso IsRateCategoryInUse _
                                AndAlso Not selectedUom Is Nothing _
                                AndAlso selectedUom.MinutesPerUnit <> 0 _
                                AndAlso selectedUom.MinutesPerUnit <> uom.MinutesPerUnit Then
                                Continue For
                            End If

                    End Select

                End If

                cboUom.DropDownList.Items.Add(New ListItem(uom.Description, uom.ID))

            Next

            ' get uoms to output to client in js
            uomHours = (From uom In uoms.ToArray() Where uom.SystemType = 1 Select uom).FirstOrDefault()
            uomHoursMins = (From uom In uoms.ToArray() Where uom.SystemType = 3 Select uom).FirstOrDefault()

            If Not uomHours Is Nothing Then
                ' if we have an hours uom set id for use later

                _uomHoursId = uomHours.ID

            End If

            If Not uomHoursMins Is Nothing Then
                ' if we have an hours/mins uom set id for use later

                _uomHoursMinsId = uomHoursMins.ID

            End If

        End Sub

        Private Function RateCategoryInUse(ByVal rateCategoryID As Integer) As Boolean

            Dim msg As ErrorMessage
            Dim inUse As Boolean

            msg = DomContractBL.RateCategoryInUseByDomServiceOrder(Me.DbConnection, rateCategoryID, inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not inUse Then
                msg = DomContractBL.RateCategoryInUseByProformaInvoice(Me.DbConnection, rateCategoryID, inUse)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If Not inUse Then
                    msg = DomContractBL.RateCategoryInUseByDomProviderInvoice(Me.DbConnection, rateCategoryID, inUse)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
            End If

            Return inUse

        End Function

        Private Function RateCategoryInUseByVrc(ByVal rateCategoryID As Integer) As Boolean

            Dim msg As ErrorMessage
            Dim inUse As Boolean

            msg = DomContractBL.RateCategoryInUseByVrc(Me.DbConnection, rateCategoryID, inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            Return inUse

        End Function

        Private Function RateCategoryInUseByManualPayment(ByVal rateCategoryID As Integer) As Boolean

            Dim msg As ErrorMessage
            Dim inUse As Boolean

            msg = DomContractBL.RateCategoryInUseByProformaInvoice(Me.DbConnection, rateCategoryID, DomProformaInvoiceBatchType.ManualPayment, inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not inUse Then
                msg = DomContractBL.RateCategoryInUseByDomProviderInvoice(Me.DbConnection, rateCategoryID, DomProviderInvoiceStyle.ManualPayment, inUse)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            Return inUse

        End Function

        Private Function RateCategoryDescriptionInUse(ByVal rateCategoryID As Integer, ByVal rateFrameworkID As Integer) As Boolean

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim domRateCategories As DomRateCategoryCollection = New DomRateCategoryCollection()
            Dim domRateCategoryDescriptionExists As Boolean = False
            Dim msg As ErrorMessage

            msg = DomRateCategory.FetchList(Me.DbConnection, domRateCategories, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), CType(txtService, InPlaceServiceSelector).ItemID, , rateFrameworkID, )

            If msg.Success Then

                If Not IsNothing(domRateCategories) AndAlso domRateCategories.Count > 0 Then

                    For Each drcategory As DomRateCategory In domRateCategories

                        If drcategory.ID <> rateCategoryID AndAlso String.Compare(drcategory.Description, txtDescription.Text.Trim(), True) = 0 Then

                            domRateCategoryDescriptionExists = True

                        End If

                    Next

                End If

                Return domRateCategoryDescriptionExists

            Else

                WebUtils.DisplayError(msg)

            End If

        End Function

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If IsRateCategoryManualPayment Then
                ' we cannot delete a manual payment from this interface

                StandardButtonsControl.AllowDelete = False

            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim inUse As Boolean, inUseByVrc As Boolean, inUseByManualPayment As Boolean
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim screenId As Integer
            Dim invisibleControls As WebNavMenuItem_ControlVisibilityCollection = Nothing
            Dim frameworkOnInvoice As Boolean = False
            Dim rateFrameWorkTypeDesc As String = String.Empty
            Dim jsStartupScript As New StringBuilder()
            Dim rateFrameWorkTypeAbvr As String = String.Empty
            Dim rateCat As DomRateCategory = Nothing

            ' disable the dom service dropdown if the rate category is in use
            If _rateCategoryID > 0 Then
                inUse = RateCategoryInUse(_rateCategoryID)
                If inUse Then
                    ServiceControl.Enabled = False
                    WebUtils.RecursiveDisable(pnlDomService.Controls, True)
                End If
                inUseByVrc = RateCategoryInUseByVrc(_rateCategoryID)
                If inUseByVrc Then
                    cboServiceType.DropDownList.Enabled = False
                    cboServiceType.RequiredValidator.Enabled = False
                    cboDayCategory.DropDownList.Enabled = False
                    cboDayCategory.RequiredValidator.Enabled = False
                    cboTimeBand.DropDownList.Enabled = False
                    cboTimeBand.RequiredValidator.Enabled = False
                End If
            End If

            ' disable use with manual payments if it is currently True and the rate category is in use by 
            ' one or more manual payment pro forma or provider invoice
            If _rateCategoryID > 0 And chkManualPayments.CheckBox.Checked Then
                inUseByManualPayment = RateCategoryInUseByManualPayment(_rateCategoryID)
                If inUseByManualPayment Then chkManualPayments.CheckBox.Enabled = False
            End If

            'Hide controls based on framework type
            screenId = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.RateCategories")

            'disable id the framework has been used to create provider invoices
            msg = DomContractBL.FrameworkInUseOnDomProviderInvoice(Me.DbConnection, RateFrameworkId, frameworkOnInvoice)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If frameworkOnInvoice Then
                chkCareProviderNotPaid.CheckBox.Enabled = False
            End If

            If Not RateFrameworkType Is Nothing Then
                rateFrameWorkTypeAbvr = RateFrameworkType.Abbreviation
            End If

            msg = WebNavMenuItem_ControlVisibility.FetchList(Me.DbConnection, invisibleControls, RateFramework.FrameworkTypeId, screenId)

            For Each ctrl As Control In Page.Controls
                HideControls(ctrl, invisibleControls)
            Next

            If Not RateFrameworkType Is Nothing Then

                Select Case RateFrameworkType.Abbreviation

                    Case _RateFrameworkVisits

                        If IsRateFrameworkInUseByDomContracts Then

                            cboServiceType.Enabled = False
                            cboDayCategory.Enabled = False
                            cboTimeBand.Enabled = False
                            WebUtils.RecursiveDisable(pnlDomService.Controls, True)

                        End If

                    Case _RateFrameworkAttendance

                        If IsRateFrameworkInUseByDomContracts Then

                            cboUom.Enabled = False
                            cboServiceType.Enabled = False
                            WebUtils.RecursiveDisable(pnlDomService.Controls, True)

                        End If

                    Case _RateFrameworkCommunityGeneral

                        If IsRateFrameworkInUseByDomContracts Then

                            If cboUom.Value <> String.Empty Then

                                Dim uom As New DomUnitsOfMeasure(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)

                                msg = uom.Fetch(cboUom.Value)
                                If Not msg.Success Then WebUtils.DisplayError(msg)

                                If uom.MinutesPerUnit = 0 Then

                                    cboUom.Enabled = False

                                End If

                            End If

                            cboServiceType.Enabled = False

                        End If

                        pnlDayCategory.Visible = False
                        pnlTimeBand.Visible = False
                        pnlRegisterGroup.Visible = False
                        pnlCareProviderNotPaid.Visible = False

                End Select

            End If

            If _rateCategoryID > 0 Then
                ' if we have a rate category and this form is editable

                If IsRateCategoryManualPayment Then

                    ' disable all controls
                    WebUtils.RecursiveDisable(fsControls.Controls, True)

                    If (StandardButtonsControl.ButtonsMode = StdButtonsMode.AddNew OrElse StandardButtonsControl.ButtonsMode = StdButtonsMode.Edit) Then

                        ' enable specific controls
                        WebUtils.RecursiveDisable(pnlAbbreviationSuffix.Controls, False)
                        If inUse = False Then
                            WebUtils.RecursiveDisable(pnlDomService.Controls, False)
                        End If
                        WebUtils.RecursiveDisable(txtFinanceCode.Controls, False)
                        WebUtils.RecursiveDisable(txtFinanceCode2.Controls, False)

                    End If

                End If

            End If

            jsStartupScript.AppendFormat("currentTimeBandID={0};", _currentTimeBandID)
            jsStartupScript.AppendFormat("currentDomServiceID={0};", _currentDomServiceID)
            jsStartupScript.AppendFormat("currentUomID={0};", _currentUomID)
            jsStartupScript.AppendFormat("rateFrameWorkTypeAbvr='{0}';", rateFrameWorkTypeAbvr)
            jsStartupScript.AppendFormat("uomHoursId={0};", _uomHoursId)
            jsStartupScript.AppendFormat("uomHoursMinsId={0};", _uomHoursMinsId)
            jsStartupScript.AppendFormat("inplaceServiceSelectorID='{0}';", txtService.ClientID)
            jsStartupScript.AppendFormat("mode={0};", Convert.ToInt32(StandardButtonsControl.ButtonsMode))
            jsStartupScript.AppendFormat("isRateCategoryManualPayment={0};", IsRateCategoryManualPayment.ToString().ToLower())
            jsStartupScript.AppendFormat("currentPTG_ID={0};", _currentPaymentToleranceGroupID)
            jsStartupScript.AppendFormat("ptgSystemType='{0}';", IIf(_ptgType = Nothing, -1, _ptgType))
            jsStartupScript.AppendFormat("var paymentToleranceGroupCollection = {0};", _jsoPaymentToleranceGroups)
            jsStartupScript.AppendFormat("var domUnitOfMeasureCollection = {0};", _jsoDomUnitsOfMeasure)
            jsStartupScript.AppendFormat("var canContractOrServiceOrderBeInvalidated = {0};", _canProviderContractOrServiceOrderBeInvalidated.ToString.ToLower)
            jsStartupScript.AppendFormat("Init();")

            If Not ClientScript.IsStartupScriptRegistered(Me.GetType(), "StartUp") Then
                ClientScript.RegisterStartupScript(Me.GetType(), "StartUp", _
                                    jsStartupScript.ToString(), _
                                    True)
            End If

        End Sub
        Public Sub HideControls(ByVal frmControl As Control, ByVal hiddenControls As WebNavMenuItem_ControlVisibilityCollection)
            Dim frmCtrl As Control
            For Each frmCtrl In frmControl.Controls
                For Each iControl As WebNavMenuItem_ControlVisibility In hiddenControls
                    If iControl.ControlName = frmCtrl.ID Then
                        frmCtrl.Visible = False
                    End If
                Next
                If frmCtrl.HasControls Then
                    HideControls(frmCtrl, hiddenControls)
                End If
            Next
        End Sub

#Region " PopulateServiceTypes "

        ''' <summary>
        ''' Populates the service types.
        ''' </summary>
        ''' <param name="selectedId">The selected id.</param>
        Private Sub PopulateServiceTypes(ByVal selectedId As Integer)

            Dim msg As ErrorMessage
            Dim service_Groups As ServiceGroupCollection = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim dom_ServiceTypes As DomServiceTypeCollection = Nothing
            Dim dom_ServiceTypesList As DomServiceTypeCollection = Nothing

            msg = ServiceGroup.FetchList(Me.DbConnection, service_Groups, String.Empty, String.Empty, , TriState.False)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            dom_ServiceTypesList = New DomServiceTypeCollection
            For Each sgroup As ServiceGroup In service_Groups
                msg = DomServiceType.FetchList(conn:=Me.DbConnection, list:=dom_ServiceTypes, auditUserName:=String.Empty, auditLogTitle:=String.Empty, redundant:=TriState.False, serviceGroupID:=sgroup.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                For Each stype As DomServiceType In dom_ServiceTypes
                    dom_ServiceTypesList.Add(stype)
                Next
            Next

            With cboServiceType.DropDownList.Items
                .Clear()
                .Add(New ListItem(String.Empty))
            End With

            If Not dom_ServiceTypesList Is Nothing AndAlso dom_ServiceTypesList.Count > 0 Then
                ' if we have some service types to output then add them
                ' filter results by service category to ensure cash and residential arent output

                For Each serviceType As DomServiceType In (From svt As DomServiceType In dom_ServiceTypesList.ToArray() _
                                                            Where CType(svt.ServiceCategory, ServiceCategory) <> ServiceCategory.Cash _
                                                                AndAlso CType(svt.ServiceCategory, ServiceCategory) <> ServiceCategory.Residential).ToList()

                    AddServiceTypeToDropDownList(serviceType, False)

                Next

            End If

            If selectedId > 0 AndAlso cboServiceType.DropDownList.Items.FindByValue(selectedId) Is Nothing Then
                ' if an id has been specified and the id isnt in the list then add manually

                Dim selectedServiceType As New DomServiceType(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)

                ' get the selected service type
                msg = selectedServiceType.Fetch(selectedId)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' add the selected serviice type to the drop down
                AddServiceTypeToDropDownList(selectedServiceType, True)

            End If

        End Sub

        ''' <summary>
        ''' Adds the service type to drop down list.
        ''' </summary>
        ''' <param name="serviceType">Type of the service.</param>
        Private Sub AddServiceTypeToDropDownList(ByVal serviceType As DomServiceType, ByVal ignoreRedundantFlag As Boolean)

            'check if it is redendant and if it can be ignored.
            If (serviceType.Redundant = TriState.True AndAlso ignoreRedundantFlag = False) Then
                Exit Sub
            End If

            cboServiceType.DropDownList.Items.Add(New ListItem(serviceType.Description, serviceType.ID))

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets a value indicating whether this instance is rate category in use.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance is rate category in use; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property IsRateCategoryInUse() As Boolean
            Get
                Return IIf(_rateCategoryID > 0, RateCategoryInUse(_rateCategoryID), False)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance is rate category manual payment.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance is rate category manual payment; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property IsRateCategoryManualPayment() As Boolean
            Get
                Return (_rateCategoryID > 0 _
                        AndAlso RateFramework.ManualPaymentRateCategoryID > 0 _
                        AndAlso _rateCategoryID = RateFramework.ManualPaymentRateCategoryID)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance is rate framework in use by DOM contracts.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance is rate framework in use by DOM contracts; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property IsRateFrameworkInUseByDomContracts() As Boolean
            Get
                If _IsRateFrameworkInUseByDomContracts Is Nothing _
                    OrElse _IsRateFrameworkInUseByDomContracts.HasValue = False Then
                    Dim msg As New ErrorMessage()
                    Dim tmpBool As Boolean = False
                    msg = DomContractBL.FrameworkInUseOnContract(DbConnection, RateFrameworkId, tmpBool)
                    _IsRateFrameworkInUseByDomContracts = tmpBool
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _IsRateFrameworkInUseByDomContracts.Value
            End Get
        End Property

        ''' <summary>
        ''' Gets the rate framework.
        ''' </summary>
        ''' <value>The rate framework.</value>
        Private ReadOnly Property RateFramework() As DomRateFramework
            Get
                If _RateFramework Is Nothing Then
                    ' if we havent fetched the framework then do so
                    Dim msg As New ErrorMessage()
                    Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                    _RateFramework = New DomRateFramework(conn:=DbConnection, auditLogTitle:=AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), auditUserName:=currentUser.ExternalUsername)
                    msg = _RateFramework.Fetch(RateFrameworkId)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _RateFramework
            End Get
        End Property

        ''' <summary>
        ''' Gets the rate frame work id.
        ''' </summary>
        ''' <value>The rate frame work id.</value>
        Private ReadOnly Property RateFrameworkId() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString("frameworkID"))
            End Get
        End Property

        ''' <summary>
        ''' Gets the type of the rate framework.
        ''' </summary>
        ''' <value>The type of the rate framework.</value>
        Private ReadOnly Property RateFrameworkType() As FrameworkType
            Get
                If _RateFrameworkType Is Nothing AndAlso RateFramework.FrameworkTypeId > 0 Then
                    ' if we havent fetched the framework type and one exists
                    Dim msg As New ErrorMessage()
                    _RateFrameworkType = New FrameworkType(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                    msg = _RateFrameworkType.Fetch(RateFramework.FrameworkTypeId)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _RateFrameworkType
            End Get
        End Property

        ''' <summary>
        ''' Gets the rate framework types.
        ''' </summary>
        ''' <value>The rate framework types.</value>
        Private ReadOnly Property RateFrameworkTypes() As FrameworkTypeCollection
            Get
                If _RateFrameworkTypes Is Nothing Then
                    Dim msg As New ErrorMessage()
                    msg = FrameworkType.FetchList(conn:=DbConnection, list:=_RateFrameworkTypes, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _RateFrameworkTypes
            End Get
        End Property
        ''' <summary>
        ''' Gets the service control.
        ''' </summary>
        ''' <value>The service control.</value>
        Private ReadOnly Property ServiceControl() As InPlaceServiceSelector
            Get
                Return CType(txtService, InPlaceServiceSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the standard buttons.
        ''' </summary>
        ''' <value>The standard buttons.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

#End Region

#Region "PopulatePaymentToleranceGroups"
        Private Sub PopulatePaymentToleranceGroups()

            Dim msg As ErrorMessage
            Dim jsSerializer As New JavaScriptSerializer()

            'get all payment tolerance groups
            msg = PaymentToleranceBL.FetchListPaymentToleranceGroupByPaymentToleranceGroupSystemType(Me.DbConnection, _ptgcAllPaymentToleranceGroups, Nothing)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'clear the combo box
            cboPaymentToleranceGroup.DropDownList.Items.Clear()

            ' add a blank item
            cboPaymentToleranceGroup.DropDownList.Items.Insert(cboPaymentToleranceGroup.DropDownList.Items.Count, _
                                                        New ListItem(String.Empty, 0))

            For Each ptg As PaymentToleranceGroup In _ptgcAllPaymentToleranceGroups
                '    If ptg.SystemType = _ptgType Then
                cboPaymentToleranceGroup.DropDownList.Items.Insert(cboPaymentToleranceGroup.DropDownList.Items.Count, _
                                                New ListItem(ptg.Description, ptg.ID))
                '    End If
            Next

            'populate the javascript arrays to use client side
            With jsSerializer
                If Not _ptgcAllPaymentToleranceGroups Is Nothing Then
                    _jsoPaymentToleranceGroups = .Serialize(_ptgcAllPaymentToleranceGroups.ToArray)
                End If
                If Not _uoms Is Nothing Then
                    _jsoDomUnitsOfMeasure = .Serialize(_uoms.ToArray)
                End If
            End With

        End Sub
#End Region

    End Class

End Namespace