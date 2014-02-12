
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library
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
Imports Target.Abacus.Library.PaymentTolerance
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Abacus.Library.DomProviderInvoice

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Screen used to maintain payment tolerances.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Mo Tahir  A4WA#7549 When removing tolerance from a contract it does not mark associated invoices as tolerances changed.
    ''' Mo Tahir  BTI-366 Payment Tolerances - Rate Category / Rate Framework  (not filtering by rate framework)
    ''' Mo Tahir  20/10/2011  BTI235 Overlapping text on Payment Tolerance node on contract screen. 
    ''' Mo Tahir  09/08/2011  D11766 Provider Invoice Tolerances - Initial version
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class PaymentTolerances
        Inherits BasePage

#Region " Constants"

        'constants
        Const CTRL_PREFIX_PAYMENT_TOLERANCE_GROUP As String = "pTolGroup"
        Const CTRL_PREFIX_PAYMENT_TOLERANCE_EXISTING As String = "pTolExisting"
        Const CTRL_PREFIX_PAYMENT_TOLERANCE_NEW As String = "pTolNew"
        Const RATE_CATEGORY_DESCRIPTIONS_DELIMETER As String = ", "
        Const RATE_CATEGORY_DESCRIPTIONS_OPEN_TAG As String = "("
        Const RATE_CATEGORY_DESCRIPTIONS_CLOSE_TAG As String = ")"
        Const RATE_CATEGORY_DESCRIPTIONS_PREFIX As String = "Associated Rate Categories: "
        Const OPT1_SERVICE_ORDER_TEXT As String = "Use payment tolerances recorded on the contract"
        Const OPT2_SERVICE_ORDER_TEXT As String = "Override payment tolerances recorded on the contract"
        Const OPT1_CONTRACT_PERIOD_TEXT As String = "Suspend provider invoices as indicated in System Settings </br> (overrriden on a Service User-by-Service User basis)"
        Const OPT2_CONTRACT_PERIOD_TEXT As String = "Tolerate Provider Invoices that exceed the plan/commitment using the following rules:"
        Const SERVICEORDER_PAYMENT_TOLERANCES_FRAME_DESCRIPTION As String = "Overridden Payment Tolerances"
        Const CONTRACT_PAYMENT_TOLERANCES_FRAME_DESCRIPTION As String = "Payment Tolerances"

#End Region

#Region " Private_Variables "

        'locals
        Private _contractID As Nullable(Of Integer) = Nothing
        Private _periodID As Nullable(Of Integer) = Nothing
        Private _refreshTree As Boolean
        Private _stdBut As StdButtonsBase
        Private _contractEnded As Boolean
        Private _toleranceGroups As PaymentToleranceGroupCollection = Nothing
        Private _domContractPeriodPaymentTolerances As DomContractPeriodPaymentToleranceCollection = Nothing
        Private _domServiceOrderPaymentTolerances As DomServiceOrderPaymentToleranceCollection = Nothing
        Private _paymentToleranceDisplayMode As Nullable(Of PaymentToleranceDisplayMode) = Nothing
        Private _haveTolerancesBeenSet As Boolean
        Private _serviceOrderID As Nullable(Of Integer) = Nothing
        Private _clientID As Nullable(Of Integer) = Nothing
        Private _dsoDateFrom As Nullable(Of Date) = Nothing
        Private _dsDateTo As Nullable(Of Date) = Nothing
        Private _rateFrameWorkID As Nullable(Of Integer) = Nothing

#End Region

#Region " Page_Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Payment Tolerance Groups")

            Dim msg As ErrorMessage = Nothing
            Dim contract As DomContract = Nothing
            Dim domservorder As DomServiceOrder = Nothing

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            If Not Request.QueryString("contractID") Is Nothing Then _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            If Not Request.QueryString("periodID") Is Nothing Then _periodID = Utils.ToInt32(Request.QueryString("periodID"))
            If Not Request.QueryString("serviceOrderID") Is Nothing Then _serviceOrderID = Utils.ToInt32(Request.QueryString("serviceOrderID"))
            If Not Request.QueryString("clientID") Is Nothing Then _clientID = Utils.ToInt32(Request.QueryString("clientID"))

            _dsoDateFrom = Utils.ToDateTime(Request.QueryString("dsoDateFrom"))
            _dsDateTo = Utils.ToDateTime(Request.QueryString("dsoDateTo"))

            'set the screen display mode
            SetDisplayMode()

            With _stdBut
                .AllowNew = False
                .AllowDelete = False
                .AllowFind = False
                If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract Then
                    .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))
                ElseIf _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then
                    .AllowEdit = Me.UserHasMenuItemCommandInAnyMenuItem(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.OverridePaymentTolerances"))
                Else
                    .AllowEdit = False
                End If
                .EditableControls.Add(fsControls.Controls)
                .EditableControls.Add(fsSettings.Controls)
            End With
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.CancelClicked, AddressOf FindClicked

            ' get the contract
            If _contractID > 0 Then

                contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                With contract
                    msg = .Fetch(_contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    _contractEnded = (.EndDate <> DataUtils.MAX_DATE)
                    _rateFrameWorkID = .DomRateFrameworkID
                End With
            End If

            'if contract id is not present then get the contract ID from service order
            If _serviceOrderID > 0 Then

                domservorder = New DomServiceOrder(Me.DbConnection, String.Empty, String.Empty)
                msg = domservorder.Fetch(_serviceOrderID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                _contractID = domservorder.DomContractID

                contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                With contract
                    msg = .Fetch(_contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    _contractEnded = (.EndDate <> DataUtils.MAX_DATE)
                    _rateFrameWorkID = .DomRateFrameworkID
                End With
            End If

            ' add the payment tolerance group enum to use in javascript
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.PaymentTolerance.PaymentToleranceDisplayMode))

            ' add page JS
            Me.JsLinks.Add("PaymentTolerances.js")

            ' add in the jquery library
            UseJQuery = True

            ' get the distinct list of payment tolerance groups and related payment tolerance records
            GetData()

            'create the controls
            OutputPaymentToleranceControls()

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim jsStartupScript As New StringBuilder()

            If _refreshTree Then _
                ClientScript.RegisterStartupScript(Me.GetType(), "Startup", String.Format("window.parent.RefreshTree({0}, 'pt', {1});", _contractID, _periodID), True)

            SetUpPanelsAndOptionControls()

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            SetUpJavaScriptVariables()

        End Sub

#End Region

#Region " StdButtons_Events "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim cPanel As CollapsiblePanel = Nothing
            Dim pTolControl As PaymentToleranceControl = Nothing
            Dim domConPerPayTolerance As DomContractPeriodPaymentTolerance = Nothing
            Dim domSerOrdPayTolerance As DomServiceOrderPaymentTolerance = Nothing
            Dim msg As ErrorMessage = Nothing
            Dim periodID As Nullable(Of Integer) = Nothing

            'if we are in the service order screen get the payment tolerance data from domcontractperiodpaymenttolerances, so that
            'they can be primed if they dont have the a record in domserviceorderpaymenttolerance with a matching paymenttolerancegroupid
            If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then

                msg = PaymentToleranceBL.GetDomContractPeriodIDByDSODateFrom(Me.DbConnection, _dsoDateFrom, _contractID, periodID)

                If Not msg.Success Then WebUtils.DisplayError(msg)

                If periodID.HasValue Then
                    GetDomContractPeriodPaymentTolerances(periodID)
                End If

            End If

            'find each payment tolerance group collapsible panel
            For Each tolGroup As Target.Abacus.Library.DataClasses.PaymentToleranceGroup In _toleranceGroups

                'find the collapsible panel
                cPanel = CType(phPaymentTolerances.FindControl(CTRL_PREFIX_PAYMENT_TOLERANCE_GROUP & tolGroup.ID), CollapsiblePanel)

                'then find the payment tolerance control in the panel
                pTolControl = CType(cPanel.FindControl(CTRL_PREFIX_PAYMENT_TOLERANCE_EXISTING & tolGroup.ID), PaymentToleranceControl)


                If pTolControl Is Nothing Then
                    'find the payment tolerance control in the panel
                    pTolControl = CType(cPanel.FindControl(CTRL_PREFIX_PAYMENT_TOLERANCE_NEW & tolGroup.ID), PaymentToleranceControl)
                End If

                'get the related payment tolerance object
                msg = PaymentToleranceBL.GetPaymentToleranceFromCollectionByPaymentToleranceGroupID(tolGroup.ID, _
                                                                                                    _domContractPeriodPaymentTolerances, _
                                                                                                    _domServiceOrderPaymentTolerances, _
                                                                                                    domConPerPayTolerance, _
                                                                                                    domSerOrdPayTolerance)

                If Not msg.Success Then WebUtils.DisplayError(msg)

                'populate the payment tolerance control
                PopulatePaymentToleranceControl(pTolControl, tolGroup, domConPerPayTolerance, domSerOrdPayTolerance)

                'End If

                ' reset to nothing
                pTolControl = Nothing
                cPanel = Nothing
                domConPerPayTolerance = Nothing
                domSerOrdPayTolerance = Nothing

            Next

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim cPanel As CollapsiblePanel = Nothing
            Dim pTolControl As PaymentToleranceControl = Nothing
            Dim domConPerPayTolerance As DomContractPeriodPaymentTolerance = Nothing
            Dim domSerOrdPayTolerance As DomServiceOrderPaymentTolerance = Nothing

            If Me.IsValid Then

                Try
                    'check if deleting
                    If optSuspendAsIndicatedInSystemSetting.Checked Then

                        If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract Then

                            msg = PaymentToleranceBL.DeletePaymentTolerances(Me.DbConnection, Nothing, _periodID)

                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            msg = DomProviderInvoiceBL.SetProviderInvoiceFlagTolerancesChanged(Me.DbConnection, _
                                            _contractID, _
                                            _clientID, _
                                            _dsoDateFrom, _
                                            _dsDateTo, _
                                            _periodID, _
                                            True)

                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            Exit Sub

                        ElseIf _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then

                            msg = PaymentToleranceBL.DeletePaymentTolerances(Me.DbConnection, _serviceOrderID, Nothing)

                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            msg = DomProviderInvoiceBL.SetProviderInvoiceFlagTolerancesChanged(Me.DbConnection, _
                                            _contractID, _
                                            _clientID, _
                                            _dsoDateFrom, _
                                            _dsDateTo, _
                                            _periodID, _
                                            True)

                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            Exit Sub

                        End If

                    End If

                        ' loop round payment tolerance group panels
                    For Each tolGroup As Target.Abacus.Library.DataClasses.PaymentToleranceGroup In _toleranceGroups

                        'instantiate the dom contract period payment tolerance object
                        If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract Then

                            domConPerPayTolerance = New DomContractPeriodPaymentTolerance(currentUser.ExternalUsername, _
                                                                                                  AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                            'instantiate the dom service order payment tolerance object
                        ElseIf _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then

                            domSerOrdPayTolerance = New DomServiceOrderPaymentTolerance(currentUser.ExternalUsername, _
                                                                                                  AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                        End If

                        'find the collapsible panel
                        cPanel = CType(phPaymentTolerances.FindControl(CTRL_PREFIX_PAYMENT_TOLERANCE_GROUP & tolGroup.ID), CollapsiblePanel)

                        'then find the existing payment tolerance control in the panel
                        pTolControl = CType(cPanel.FindControl(CTRL_PREFIX_PAYMENT_TOLERANCE_EXISTING & tolGroup.ID), PaymentToleranceControl)

                        'not found existing so must be new so find the new payment tolerance control
                        If pTolControl Is Nothing Then

                            pTolControl = CType(cPanel.FindControl(CTRL_PREFIX_PAYMENT_TOLERANCE_NEW & tolGroup.ID), PaymentToleranceControl)

                        End If

                        'retrieve information from control and update dom contract period payment tolerance object
                        PopulatePaymentToleranceFromPaymentToleranceControl(pTolControl, _
                                                      tolGroup, _
                                                      domConPerPayTolerance, _
                                                      domSerOrdPayTolerance)

                        'update the database
                        msg = PaymentToleranceBL.SavePaymentTolerance(Me.DbConnection, _
                                                                      domConPerPayTolerance, _
                                                                      domSerOrdPayTolerance)

                        If Not msg.Success Then WebUtils.DisplayError(msg)


                        msg = DomProviderInvoiceBL.SetProviderInvoiceFlagTolerancesChanged(Me.DbConnection, _
                                                                                            _contractID, _
                                                                                            _clientID, _
                                                                                            _dsoDateFrom, _
                                                                                            _dsDateTo, _
                                                                                            _periodID, _
                                                                                            True)

                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        'reset variables
                        pTolControl = Nothing
                        domConPerPayTolerance = Nothing
                        domSerOrdPayTolerance = Nothing
                        cPanel = Nothing

                    Next

                        'set the variable if in contract period screen
                        If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract Then _refreshTree = True

                        msg = New ErrorMessage()
                        msg.Success = True

                Catch ex As Exception
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
                End Try

            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " Methods "

        Private Sub OutputPaymentToleranceControls()

            Dim msg As ErrorMessage = Nothing
            Dim cPanel As CollapsiblePanel = Nothing
            Dim pTolControl As PaymentToleranceControl = Nothing
            Dim domConPerPayTolerance As DomContractPeriodPaymentTolerance = Nothing
            Dim domSerOrdPayTolerance As DomServiceOrderPaymentTolerance = Nothing
            Dim controlPrefix As String
            Dim rateCategoryDescriptions As String = Nothing
            Dim rateCategoryCollection As DomRateCategoryCollection = Nothing

            'first create the payment tolerance group collapsible panels
            For Each tolGroup As Target.Abacus.Library.DataClasses.PaymentToleranceGroup In _toleranceGroups

                cPanel = New CollapsiblePanel

                With cPanel
                    .ID = EncodeControlID(CTRL_PREFIX_PAYMENT_TOLERANCE_GROUP, tolGroup.ID)
                    phPaymentTolerances.Controls.Add(cPanel)
                    .EnsureChildControls()
                    .HeaderLinkText = tolGroup.Description
                    'only add the follwoing if in the dso screen
                    If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then
                        .ExpandedJS = "parent.resizeIframe(document.body.scrollHeight, 'ifrPaymentTolerances');"
                        .CollapsedJS = "parent.resizeIframe(document.body.scrollHeight, 'ifrPaymentTolerances');"
                    End If
                End With

                'get the payment tolerance
                msg = PaymentToleranceBL.GetPaymentToleranceFromCollectionByPaymentToleranceGroupID(tolGroup.ID, _
                                                                                                    _domContractPeriodPaymentTolerances, _
                                                                                                    _domServiceOrderPaymentTolerances, _
                                                                                                    domConPerPayTolerance, _
                                                                                                    domSerOrdPayTolerance)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                'check if existing or new based on the screen this page is in
                If Not domConPerPayTolerance Is Nothing _
                                And _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract _
                                OrElse Not domSerOrdPayTolerance Is Nothing _
                                And _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then

                    controlPrefix = CTRL_PREFIX_PAYMENT_TOLERANCE_EXISTING

                Else

                    controlPrefix = CTRL_PREFIX_PAYMENT_TOLERANCE_NEW

                End If

                'get a handle to created payment tolerance control
                pTolControl = CreatePaymentToleranceControl(controlPrefix, tolGroup)

                'add control to panel
                With cPanel.ContentPanel
                    .Controls.Add(pTolControl)
                    .Controls.Add(CreateBr())
                End With

            Next

        End Sub

        Private Function EncodeControlID(ByVal controlPrefix As String, ByVal controlUniqueID As Integer) As String

            Return controlPrefix & controlUniqueID

        End Function

        Private Function CreateBr() As Literal

            Dim literal1 As Literal

            literal1 = New Literal()
            literal1.Text = "<br>"

            Return literal1

        End Function

        Private Function CreatePaymentToleranceControl(ByVal controlPrefix As String, _
                                                      ByVal payToleranceGroup As Target.Abacus.Library.DataClasses.PaymentToleranceGroup) As PaymentToleranceControl

            ' load payment tolerance control
            Dim payToleranceControl As UserControls.PaymentToleranceControl = LoadControl(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/PaymentToleranceControl.ascx"))

            With payToleranceControl
                .ID = EncodeControlID(controlPrefix, payToleranceGroup.ID)
            End With

            CreatePaymentToleranceControl = payToleranceControl

        End Function

        Private Sub PopulatePaymentToleranceControl(ByRef PTolControl As PaymentToleranceControl, _
                                                    ByVal payToleranceGroup As Target.Abacus.Library.DataClasses.PaymentToleranceGroup, _
                                                    Optional ByVal domConPeriodPayTolerance As DomContractPeriodPaymentTolerance = Nothing, _
                                                    Optional ByVal domSerOrdPayTolerance As DomServiceOrderPaymentTolerance = Nothing)

            Dim msg As ErrorMessage = Nothing
            Dim rateCategoryCollection As DomRateCategoryCollection = Nothing
            Dim rateCategoryDescriptions As String = Nothing


            'get the rate category objects that have this payment tolerance group id specified on them
            msg = PaymentToleranceBL.FetchListDomRateCategoryByPaymentToleranceGroupID(Me.DbConnection, _
                                                                                       rateCategoryCollection, _
                                                                                       payToleranceGroup.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get the formatted string of related rate categories
            msg = PaymentToleranceBL.GetFormattedStringOfDomRateCategoryDescriptionFromDomRateCategoryCollection(rateCategoryCollection, _
                                                                                                                RATE_CATEGORY_DESCRIPTIONS_PREFIX, _
                                                                                                                RATE_CATEGORY_DESCRIPTIONS_DELIMETER, _
                                                                                                                RATE_CATEGORY_DESCRIPTIONS_OPEN_TAG, _
                                                                                                                RATE_CATEGORY_DESCRIPTIONS_CLOSE_TAG, _
                                                                                                                rateCategoryDescriptions, _
                                                                                                                _rateFrameWorkID)
            If Not msg.Success Then WebUtils.DisplayError(msg)



            With PTolControl

                If Not domConPeriodPayTolerance Is Nothing AndAlso _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract _
                Or _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder And domSerOrdPayTolerance Is Nothing Then

                    If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract And Not domConPeriodPayTolerance Is Nothing Then
                        .ID = EncodeControlID(CTRL_PREFIX_PAYMENT_TOLERANCE_EXISTING, payToleranceGroup.ID)
                    ElseIf _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder And domSerOrdPayTolerance Is Nothing Then
                        .ID = EncodeControlID(CTRL_PREFIX_PAYMENT_TOLERANCE_NEW, payToleranceGroup.ID)
                    End If

                    If Not domConPeriodPayTolerance Is Nothing Then

                        .PaymentToleranceID = IIf(_paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract, domConPeriodPayTolerance.ID, 0)
                        .PaymentToleranceDisplayMode = _paymentToleranceDisplayMode
                        '.PaymentToleranceGroupSystemType = payToleranceGroup.SystemType
                        .AcceptableAdditionalUnits = domConPeriodPayTolerance.AdditionalHours
                        .AcceptableAdditionalUnitsAsPercentage = domConPeriodPayTolerance.AdditionalHoursPercentage
                        .UnitsOfServiceCappedAt = domConPeriodPayTolerance.AdditionalHoursPercentageCap
                        .SuspendInvoiceWhenPlannedUnitsExceeded = domConPeriodPayTolerance.SuspendWhenUnitsExceeded
                        .AcceptableAdditionalPayment = domConPeriodPayTolerance.AdditionalPayment
                        .AcceptableAdditionalPaymentAsPercentage = domConPeriodPayTolerance.AdditionalPaymentPercentage
                        .CostOfServiceCappedAt = domConPeriodPayTolerance.AdditionalPaymentPercentageCap
                        .ToleranceCombiMethod = domConPeriodPayTolerance.ToleranceCombinationMethod

                        'set this flag indicating tolerances have been set.
                        If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract Then _haveTolerancesBeenSet = True

                    End If


                End If

                If Not domSerOrdPayTolerance Is Nothing AndAlso _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then

                    .ID = EncodeControlID(CTRL_PREFIX_PAYMENT_TOLERANCE_EXISTING, payToleranceGroup.ID)
                    .PaymentToleranceID = domSerOrdPayTolerance.ID
                    .PaymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract
                    '.PaymentToleranceGroupSystemType = payToleranceGroup.SystemType
                    .AcceptableAdditionalUnits = domSerOrdPayTolerance.AdditionalHours
                    .AcceptableAdditionalUnitsAsPercentage = domSerOrdPayTolerance.AdditionalHoursPercentage
                    .UnitsOfServiceCappedAt = domSerOrdPayTolerance.AdditionalHoursPercentageCap
                    .SuspendInvoiceWhenPlannedUnitsExceeded = domSerOrdPayTolerance.SuspendWhenUnitsExceeded
                    .AcceptableAdditionalPayment = domSerOrdPayTolerance.AdditionalPayment
                    .AcceptableAdditionalPaymentAsPercentage = domSerOrdPayTolerance.AdditionalPaymentPercentage
                    .CostOfServiceCappedAt = domSerOrdPayTolerance.AdditionalPaymentPercentageCap
                    .ToleranceCombiMethod = domSerOrdPayTolerance.ToleranceCombinationMethod

                    'set this flag indicating tolerances have been set.
                    _haveTolerancesBeenSet = True

                End If

                'set Payment Tolerance Group system type
                .PaymentToleranceGroupSystemType = payToleranceGroup.SystemType

            End With


            'set the rate category descriptions
            If Not rateCategoryDescriptions Is Nothing Then

                With PTolControl
                    .RateCategoryDescriptions = rateCategoryDescriptions
                End With

            End If

        End Sub

        Private Sub GetData()

            Dim msg As ErrorMessage

            ' get the distinct list of payment tolerance groups
            msg = PaymentToleranceBL.FetchListDistinctPaymentToleranceGroupsByContractID(Me.DbConnection, _
                                                                                         _toleranceGroups, _
                                                                                         _contractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract Then
                GetDomContractPeriodPaymentTolerances(_periodID)
            ElseIf _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then
                GetDomServiceOrderPaymentTolerances()
            End If

        End Sub

        Private Sub PopulatePaymentToleranceFromPaymentToleranceControl(ByRef PTolControl As PaymentToleranceControl, _
                                                    ByVal payToleranceGroup As Target.Abacus.Library.DataClasses.PaymentToleranceGroup, _
                                                    ByVal domConPeriodPayTolerance As DomContractPeriodPaymentTolerance, _
                                                    ByVal domSerOrdPayTolerance As DomServiceOrderPaymentTolerance)

            If Not domConPeriodPayTolerance Is Nothing Then

                With domConPeriodPayTolerance

                    .ID = PTolControl.PaymentToleranceID
                    .DomContractPeriodID = _periodID
                    .PaymentToleranceGroupID = payToleranceGroup.ID
                    .AdditionalHours = PTolControl.AcceptableAdditionalUnits
                    .AdditionalHoursPercentage = PTolControl.AcceptableAdditionalUnitsAsPercentage
                    .AdditionalHoursPercentageCap = PTolControl.UnitsOfServiceCappedAt
                    .AdditionalPayment = PTolControl.AcceptableAdditionalPayment
                    .AdditionalPaymentPercentage = PTolControl.AcceptableAdditionalPaymentAsPercentage
                    .AdditionalPaymentPercentageCap = PTolControl.CostOfServiceCappedAt
                    .SuspendWhenUnitsExceeded = PTolControl.SuspendInvoiceWhenPlannedUnitsExceeded
                    .ToleranceCombinationMethod = PTolControl.ToleranceCombiMethod

                End With

            ElseIf Not domSerOrdPayTolerance Is Nothing Then

                With domSerOrdPayTolerance

                    .ID = PTolControl.PaymentToleranceID
                    .DomServiceOrderID = _serviceOrderID
                    .PaymentToleranceGroupID = payToleranceGroup.ID
                    .AdditionalHours = PTolControl.AcceptableAdditionalUnits
                    .AdditionalHoursPercentage = PTolControl.AcceptableAdditionalUnitsAsPercentage
                    .AdditionalHoursPercentageCap = PTolControl.UnitsOfServiceCappedAt
                    .AdditionalPayment = PTolControl.AcceptableAdditionalPayment
                    .AdditionalPaymentPercentage = PTolControl.AcceptableAdditionalPaymentAsPercentage
                    .AdditionalPaymentPercentageCap = PTolControl.CostOfServiceCappedAt
                    .SuspendWhenUnitsExceeded = PTolControl.SuspendInvoiceWhenPlannedUnitsExceeded
                    .ToleranceCombinationMethod = PTolControl.ToleranceCombiMethod

                End With

            End If


        End Sub

        Private Sub SetUpPanelsAndOptionControls()

            Dim cPanel As CollapsiblePanel = Nothing
            Dim pTolControl As PaymentToleranceControl = Nothing

            'set the options text
            If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract Then
                optSuspendAsIndicatedInSystemSetting.Text = OPT1_CONTRACT_PERIOD_TEXT
                optTolerateRules.Text = OPT2_CONTRACT_PERIOD_TEXT
            ElseIf _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then
                optSuspendAsIndicatedInSystemSetting.Text = OPT1_SERVICE_ORDER_TEXT
                optTolerateRules.Text = OPT2_SERVICE_ORDER_TEXT
            End If

            'If tolerances have not been set then set the options
            If Not _haveTolerancesBeenSet Then
                optSuspendAsIndicatedInSystemSetting.Checked = Not _haveTolerancesBeenSet

                'find each payment tolerance group collapsible panel
                For Each tolGroup As Target.Abacus.Library.DataClasses.PaymentToleranceGroup In _toleranceGroups

                    'find the collapsible panel and disable it
                    cPanel = CType(phPaymentTolerances.FindControl(CTRL_PREFIX_PAYMENT_TOLERANCE_GROUP & tolGroup.ID), CollapsiblePanel)
                    cPanel.Expanded = _haveTolerancesBeenSet
                    cPanel.Enabled = _haveTolerancesBeenSet
                    cPanel = Nothing

                Next

                'if in the dom service order screen hide the payment tolerance control
                If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then
                    fsControlsLegend.InnerHtml = SERVICEORDER_PAYMENT_TOLERANCES_FRAME_DESCRIPTION
                ElseIf _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract Then
                    fsControlsLegend.InnerHtml = CONTRACT_PAYMENT_TOLERANCES_FRAME_DESCRIPTION
                End If

            Else
                optTolerateRules.Checked = _haveTolerancesBeenSet
            End If

        End Sub

        Private Sub SetUpJavaScriptVariables()

            Dim jsStartupScript As New StringBuilder()

            jsStartupScript.AppendFormat("var mode={0};", Convert.ToInt32(CType(stdButtons1, StdButtonsBase).ButtonsMode))
            jsStartupScript.AppendFormat("var divPlaceHolderPaymentTolerances = GetElement('{0}', true);", divPlaceHolderPaymentTolerances.ClientID)
            jsStartupScript.AppendFormat("var rdoSuspendAsIndicatedInSystemSetting = GetElement('{0}', true);", optSuspendAsIndicatedInSystemSetting.ClientID)
            jsStartupScript.AppendFormat("var rdoTolerateRules = GetElement('{0}', true);", optTolerateRules.ClientID)
            If _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder Then
                jsStartupScript.AppendFormat("var iFrameName='{0}';", "ifrPaymentTolerances")
            End If
            jsStartupScript.AppendFormat("var paymentToleranceDisplayMode={0};", CInt(_paymentToleranceDisplayMode))

            ClientScript.RegisterStartupScript(Me.GetType(), "PaymentTolerances.Init", _
                                jsStartupScript.ToString(), _
                                True)

        End Sub

        Private Sub GetDomServiceOrderPaymentTolerances()

            Dim msg As ErrorMessage

            'get the list of payment tolerances for this service order
            msg = PaymentToleranceBL.FetchListDomServiceOrderPaymentTolerancesByServiceOrderID(Me.DbConnection, _
                                                                    _domServiceOrderPaymentTolerances, _
                                                                       _serviceOrderID)

            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

        Private Sub GetDomContractPeriodPaymentTolerances(ByVal periodID As Nullable(Of Integer))

            Dim msg As ErrorMessage

            'get the list of payment tolerances for this contract period
            msg = PaymentToleranceBL.FetchListDomContractPeriodPaymentToleranceByPeriodID(Me.DbConnection, _
                                                                       _domContractPeriodPaymentTolerances, _
                                                                       periodID)

            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

        Private Sub SetDisplayMode()
            If _periodID > 0 Then
                _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.Contract
            ElseIf _serviceOrderID > 0 Then
                _paymentToleranceDisplayMode = PaymentToleranceDisplayMode.ServiceOrder
            End If
        End Sub

#End Region

    End Class

End Namespace