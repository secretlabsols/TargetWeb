
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.ContractPeriodPaymentPlan
Imports Target.Abacus.Library.ContractPeriodPaymentPlan.Messages
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Screen used to maintain Period Payment Plan.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir     18/09/2013  D12459 Block Contract Payment Plan
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class PaymentPlan
        Inherits BasePage

#Region " Fields "

        Private _contract As DomContract = Nothing
        Private _contractID As Integer
        Private _periodID As Integer
        Private _periodPeriodPaymentPlanRec As Target.Abacus.Library.DataClasses.DomContractPeriod
        Private _refreshTree As Boolean
        Private _stdBut As StdButtonsBase
        Private _contractEnded As Boolean
        Private Const CONST_DATE_FORMAT As String = "dd/MM/yyyy"
        Private Const END_DATE As String = "31/12/9999 00:00:00"
        Private Const CONST_JS_DATE_FORMAT As String = "MM/dd/yyyy"
        Private _paymentPlanPresent As Boolean
        Private _paymentConfigPresent As Boolean
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"
        Private Const _WebNavMenuItemCommandEditKey As String = "AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"
        Private Const MANUAL_PAYMENT_DESCRIPTION As String = "Manual Payment"

#End Region

#Region " Properties "

        Public Property ExpenditureAccountID() As Integer
            Get
                Return CType(Me.expenditureAccount, InPlaceSelectors.InPlaceClientSelector).ClientDetailID
            End Get
            Set(ByVal value As Integer)
                With CType(Me.expenditureAccount, InPlaceSelectors.InPlaceClientSelector)
                    .ClientDetailID = value
                End With
            End Set
        End Property

        Public Property ClientGroupID() As Integer
            Get
                Return CType(Me.clientGroup, InPlaceSelectors.InPlaceClientGroupSelector).ClientGroupID
            End Get
            Set(ByVal value As Integer)
                CType(Me.clientGroup, InPlaceSelectors.InPlaceClientGroupSelector).ClientGroupID = value
            End Set
        End Property

        Public Property ClientSubGroupID() As Integer
            Get
                Return CType(Me.clientSubGroup, InPlaceSelectors.InPlaceClientSubGroupSelector).ClientSubGroupID
            End Get
            Set(ByVal value As Integer)
                CType(Me.clientSubGroup, InPlaceSelectors.InPlaceClientSubGroupSelector).ClientSubGroupID = value
            End Set
        End Property

        Public Property TeamID() As Integer
            Get
                Return CType(Me.team, InPlaceSelectors.InPlaceTeamSelector).TeamID
            End Get
            Set(ByVal value As Integer)
                CType(Me.team, InPlaceSelectors.InPlaceTeamSelector).TeamID = value
            End Set
        End Property

        Public Property ContractValue() As Decimal
            Get
                Return Utils.ToDecimal(txtContractValue.Text)
            End Get
            Set(ByVal value As Decimal)
                With txtContractValue
                    .Text = If(value = 0, String.Empty, value.ToString("0.00"))
                End With
            End Set
        End Property

        Public Property RateCategoryID() As Integer
            Get
                Return Utils.ToInt32(cboRateCategory.DropDownList.SelectedValue)
            End Get
            Set(ByVal value As Integer)
                With cboRateCategory.DropDownList
                    .SelectedValue = IIf(value = 0, String.Empty, value)
                End With
            End Set
        End Property

        Private Property EndDate() As Nullable(Of DateTime)
            Get
                If dteEndDate.Value Is Nothing Then
                    Return New Date
                Else
                    Return Target.Library.Utils.ToDateTime(dteEndDate.Value)
                End If
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If value.HasValue And value <> New Date And value.ToString() <> END_DATE Then
                    dteEndDate.Text = value.Value.ToString(CONST_DATE_FORMAT)
                Else
                    dteEndDate.Text = String.Empty
                End If
            End Set
        End Property

        Private Property FirstPaymentDate() As Nullable(Of DateTime)
            Get
                If dteFirstPaymentDate.Value Is Nothing Then
                    Return New Date
                Else
                    Return Target.Library.Utils.ToDateTime(dteFirstPaymentDate.Value)
                End If
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If value.HasValue And value <> New Date And value.ToString() <> END_DATE Then
                    dteFirstPaymentDate.Text = value.Value.ToString(CONST_DATE_FORMAT)
                Else
                    dteFirstPaymentDate.Text = String.Empty
                End If
            End Set
        End Property

        Public Property PaymentFrequencyID() As Integer
            Get
                Return Utils.ToInt32(cboFrequency.DropDownList.SelectedValue)
            End Get
            Set(ByVal value As Integer)
                With cboFrequency.DropDownList
                    .SelectedValue = IIf(value = 0, String.Empty, value)
                End With
            End Set
        End Property

        'Public Property OptFirst() As Boolean
        '    Get
        '        Return optFirstPayment.Checked
        '    End Get
        '    Set(ByVal value As Boolean)
        '        optFirstPayment.Checked = value
        '    End Set
        'End Property

        'Public Property OptLast() As Boolean
        '    Get
        '        Return optLastPayment.Checked
        '    End Get
        '    Set(ByVal value As Boolean)
        '        optLastPayment.Checked = value
        '    End Set
        'End Property

        Public Property RunningTotal() As Decimal
            Get
                Return Utils.ToDecimal(lblRunningTotal.Text)
            End Get
            Set(ByVal value As Decimal)
                With lblRunningTotal
                    .Text = If(value = 0, String.Empty, value.ToString("0.00"))
                End With
            End Set
        End Property

        Public ReadOnly Property ContractID() As Integer
            Get
                _contractID = Utils.ToInt32(Request.QueryString("contractID"))
                Return _contractID
            End Get
        End Property

        Public ReadOnly Property PeriodID() As Integer
            Get
                _periodID = Utils.ToInt32(Request.QueryString("periodID"))
                Return _periodID
            End Get
        End Property

        Public Property FinanceCodeID() As Integer
            Get
                Return CType(Me.financeCode, InPlaceSelectors.InPlaceFinanceCodeSelector).FinanceCodeID
            End Get
            Set(ByVal value As Integer)
                CType(Me.financeCode, InPlaceSelectors.InPlaceFinanceCodeSelector).FinanceCodeID = value
            End Set
        End Property

#End Region

#Region " Even Handlers "
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Domiciliary Contract Block Contract Payment Plan")

            SetUpStdButtonsConfig()

            GetHeadearData()

            SetFlags()

            If Me.IsPostBack Then RefreshControls()

            If Not Me.IsPostBack Then LoadPeriodPaymentPlanDetailTemp()

            AddJavaScriptDependencies()

            AddSelectorControl()

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            GetHeadearData()

            SetFlags()

            ManagePostBack()

            SetUpStdButtons()

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            _stdBut.Visible = Not _contractEnded

            pnlFirstPaymentDate.Visible = Not _paymentConfigPresent
            pnlFrequency.Visible = Not _paymentConfigPresent

            SetUpJavascriptVariables()

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

            If e.ItemID > 0 Then

                SetFieldsOnScreen(e.ItemID)
                LoadPeriodPaymentPlanDetailTemp()

            Else
                ResetFields()
            End If

            SetValidDateRange()

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

            If e.ItemID = 0 Then
                ResetFields()
            Else
                FindClicked(e)
            End If

        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            If PeriodID > 0 Then

                msg = PeriodPaymentPlanBL.DeletePeriodPaymentPlan(Me.DbConnection, PeriodID)
                If Not msg.Success Then
                    If msg.Number = PeriodPaymentPlanBL.ERR_LINKED_PERIOD_PAYMENT_PLAN Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        Exit Sub
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                End If

                _stdBut.SelectedItemID = 0
                ResetFields()
                _refreshTree = True
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing

            RefreshControls()

            Me.Validate("Save")

            If Me.IsValid Then
                Try
                    If Utils.ToInt32(cboRateCategory.DropDownList.SelectedValue) = 0 Then
                        lblError.Text = "A rate category must be selected."
                    End If

                    If ExpenditureAccountID = 0 Then
                        If lblError.Text <> String.Empty Then lblError.Text = lblError.Text & "<br/>"
                        lblError.Text = lblError.Text & "An expenditure account must be selected."
                    End If

                    If ContractValue < 0 Then
                        If lblError.Text <> String.Empty Then lblError.Text = lblError.Text & "<br/>"
                        lblError.Text = lblError.Text & "Contract value amount can not be negative"
                    End If

                    If lblError.Text <> String.Empty Then
                        e.Cancel = True
                        Exit Sub
                    Else
                        If e.ItemID > 0 And _paymentPlanPresent Then
                            If txtContractValue.Text <> hidRunningTotal.Value.ToString Then
                                lblError.Text = lblError.Text & "Contract Value and Total sum of Period Payment Plan Detail lines must be the same"
                                e.Cancel = True
                                Exit Sub
                            End If
                        End If
                    End If

                    If e.ItemID > 0 Then
                        'Update of existing record..
                        msg = DomContractBL.FetchDomContractPeriod(Me.DbConnection, Nothing, e.ItemID, _periodPeriodPaymentPlanRec)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If

                    With _periodPeriodPaymentPlanRec

                        .ContractValue = Utils.ToDecimal(txtContractValue.Text)
                        .ExpenditureAccountID = ExpenditureAccountID
                        .DomRateCategoryID = RateCategoryID
                        .ClientGroupID = ClientGroupID
                        .ClientSubGroupID = ClientSubGroupID
                        .TeamID = TeamID
                        .DateTo = EndDate
                        .FinanceCodeID = FinanceCodeID

                        msg = PeriodPaymentPlanBL.SaveDomContractPeriodPaymentPlan(Me.DbConnection, _periodPeriodPaymentPlanRec, _
                                                                                  PeriodPaymentPlanBL.PaymentRemainder.FirstPayment, _
                                                                                   FirstPaymentDate, PaymentFrequencyID, _paymentPlanPresent)
                        If Not msg.Success Then
                            If msg.Number = PeriodPaymentPlanBL.ERR_MAX_NO_PAYMENTS_EXCEEDED Then
                                lblError.Text = msg.Message
                                e.Cancel = True
                                SetValidDateRange()
                                Exit Sub
                            ElseIf msg.Number = PeriodPaymentPlanBL.ERR_PERIOD_PAYMENT_LINES Then
                                lblError.Text = msg.Message
                                e.Cancel = True
                                SetValidDateRange()
                                Exit Sub
                            ElseIf msg.Number = PeriodPaymentPlanBL.ERR_PAYMENT_PLAN_CHANGED Then
                                lblError.Text = msg.Message
                            Else
                                WebUtils.DisplayError(msg)
                            End If
                        End If

                        'load newly created data in to temp table
                        LoadPeriodPaymentPlanDetailTemp()

                        e.ItemID = .ID
                        _refreshTree = True

                    End With

                Catch ex As Exception
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
                End Try
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            ResetFields()
            SetNewDefaults()
        End Sub
#End Region

#Region " Helpers "
        Private Sub PopulateRateCategories(ByVal selectedID As Integer)
            Dim uoms As DomUnitsOfMeasureCollection = Nothing
            Dim rateCategories As DomRateCategoryCollection = Nothing
            Dim msg As ErrorMessage

            cboRateCategory.DropDownList.Items.Clear()
            If _contract IsNot Nothing Then
                'Fetch all money-type rate categories for this contract's framework..

                msg = DomContractBL.FetchListDomUnitsOfMeasure(connection:=Me.DbConnection,
                                                               transaction:=Nothing,
                                                               items:=uoms,
                                                               systemType:=DomRateFrameworkBL.DomUnitOfMeasureSystemType.Money)

                If Not msg.Success Then WebUtils.DisplayError(msg)

                If uoms IsNot Nothing AndAlso uoms.Count > 0 Then

                    msg = DomContractBL.FetchListDomRateCategory(connection:=Me.DbConnection,
                                                                 transaction:=Nothing,
                                                                 items:=rateCategories,
                                                                 domServiceID:=Nothing,
                                                                 domServiceTypeID:=Nothing,
                                                                 domRateFrameworkID:=_contract.DomRateFrameworkID,
                                                                 allowUseWithManualPayments:=TriState.UseDefault,
                                                                 domUnitsOfMeasureID:=uoms(0).ID,
                                                                 registerGroup:=Nothing,
                                                                 paymentToleranceGroupID:=Nothing)

                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                Dim itemList As New List(Of ListItem)
                itemList.Add(New ListItem(String.Empty, 0))
                For Each rateCateg As DomRateCategory In rateCategories
                    Dim item As New ListItem

                    item.Text = rateCateg.Description
                    item.Value = rateCateg.ID.ToString
                    If item.Text <> MANUAL_PAYMENT_DESCRIPTION Then
                        itemList.Add(item)
                    End If
                Next
                itemList.Sort(New DropDownListEx.ListItemComparer)

                With cboRateCategory.DropDownList.Items()
                    For Each item As ListItem In itemList
                        .Add(New ListItem(item.Text, item.Value))

                        If item.Value = selectedID.ToString Then
                            cboRateCategory.DropDownList.SelectedValue = item.Value
                        End If
                    Next
                End With
            End If
        End Sub

        Private Sub PopulateFrequencies(ByVal selectedID As Integer)
            Dim freqCollection As FrequencyCollection = Nothing
            Dim msg As ErrorMessage

            If Not _paymentConfigPresent Then

                msg = PeriodPaymentPlanBL.FetchFrequencyList(freqCollection)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If freqCollection IsNot Nothing AndAlso freqCollection.Count > 0 Then

                    With cboFrequency.DropDownList.Items
                        .Clear()
                        For Each freq As FrequencyItem In freqCollection.Items
                            .Add(New ListItem(Utils.SplitOnCapitals(freq.Description), freq.ID))
                        Next
                        .Insert(0, String.Empty)
                    End With

                End If

                cboFrequency.DropDownList.SelectedValue = selectedID

            End If
        End Sub

        Private Sub ResetFields()
            ContractValue = 0
            ExpenditureAccountID = 0
            PopulateRateCategories(0)
            PopulateFrequencies(0)
            ClientGroupID = 0
            ClientSubGroupID = 0
            FinanceCodeID = 0
            TeamID = 0
            _stdBut.AllowDelete = False
            RunningTotal = 0
            SetValidDateRange()
            EndDate = END_DATE
            FirstPaymentDate = END_DATE
        End Sub

        Private Sub RefreshControls()
            ContractValue = Utils.ToDecimal(txtContractValue.GetPostBackValue)
            ExpenditureAccountID = Utils.ToInt32(Request.Form(CType(Me.expenditureAccount, InPlaceClientSelector).HiddenFieldUniqueID))
            PopulateRateCategories(cboRateCategory.GetPostBackValue())
            PopulateFrequencies(cboFrequency.GetPostBackValue())
            ClientGroupID = Utils.ToInt32(Request.Form(CType(Me.clientGroup, InPlaceClientGroupSelector).HiddenFieldUniqueID))
            ClientSubGroupID = Utils.ToInt32(Request.Form(CType(Me.clientSubGroup, InPlaceClientSubGroupSelector).HiddenFieldUniqueID))
            FinanceCodeID = Utils.ToInt32(Request.Form(CType(Me.financeCode, InPlaceFinanceCodeSelector).HiddenFieldUniqueID))
            TeamID = Utils.ToInt32(Request.Form(CType(Me.team, InPlaceTeamSelector).HiddenFieldUniqueID))
            EndDate = Utils.ToDateTime(dteEndDate.GetPostBackValue)
        End Sub

        Private Sub AddJavaScriptDependencies()
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

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

            ' add in page script
            JsLinks.Add("PaymentPlan.js")

        End Sub

        Private Sub SetUpJavascriptVariables()

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage = Nothing
            Dim lastPaymentToDate As Nullable(Of Date) = Nothing

            msg = PeriodPaymentPlanBL.getPaidPeriodPaymentPlanLastPaymentToDate(Nothing, Me.DbConnection, _periodPeriodPaymentPlanRec.ID, lastPaymentToDate)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            txtContractValue.TextBox.Attributes.Add("onchange", _
                             "javascript:txtContractValue_Changed();")

            Dim jsStartupScript As New StringBuilder()
            jsStartupScript.AppendFormat("domContractPeriodID={0};", PeriodID)
            jsStartupScript.AppendFormat("periodFrom='{0}';", If(Not _periodPeriodPaymentPlanRec Is Nothing, _periodPeriodPaymentPlanRec.DateFrom.ToString(CONST_JS_DATE_FORMAT), ""))
            jsStartupScript.AppendFormat("periodTo='{0}';", If(Not _periodPeriodPaymentPlanRec Is Nothing, _periodPeriodPaymentPlanRec.DateTo.ToString(CONST_JS_DATE_FORMAT), ""))
            jsStartupScript.AppendFormat("window.parent.RefreshTree({0}, 'pp', {1});", ContractID, PeriodID)
            jsStartupScript.AppendFormat("lblTotal=GetElement('{0}');", lblRunningTotal.ClientID)
            jsStartupScript.AppendFormat("txtContractValue=GetElement('{0}_txtTextBox');", txtContractValue.ClientID)
            jsStartupScript.AppendFormat("stdButtonMode='{0}';", CType(_stdBut.ButtonsMode, Integer))
            jsStartupScript.AppendFormat("hidTotal=GetElement('{0}');", hidRunningTotal.ClientID)
            jsStartupScript.AppendFormat("lblError=GetElement('{0}');", lblError.ClientID)
            jsStartupScript.AppendFormat("dteEndDate=GetElement('{0}_txtTextBox');", dteEndDate.ClientID)
            jsStartupScript.AppendFormat("paymentConfigPresent={0};", _paymentConfigPresent.ToString.ToLower)
            jsStartupScript.AppendFormat("userID={0};", currentUser.ID)
            jsStartupScript.AppendFormat("lastPaymentToDate='{0}';", If(lastPaymentToDate.HasValue, CDate(lastPaymentToDate).AddDays(1).ToString(CONST_JS_DATE_FORMAT), ""))

            If Not ClientScript.IsStartupScriptRegistered(Me.GetType(), "StartUp") Then
                ClientScript.RegisterStartupScript(Me.GetType(), "StartUp", _
                                    jsStartupScript.ToString(), _
                                    True)
            End If
        End Sub

        Private Sub AddSelectorControl()
            Dim resultSettings As New PeriodPaymentPlanDetailSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "pppdResultSettings")
            End With
        End Sub

        Private Sub GetHeadearData()
            Dim msg As ErrorMessage
            'Get the contract..
            _contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = _contract.Fetch(ContractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _contractEnded = (_contract.EndDate <> DataUtils.MAX_DATE)

            'Get the contract period's block contract agreed cost details.
            If PeriodID > 0 Then
                msg = DomContractBL.FetchDomContractPeriod(Me.DbConnection, Nothing, PeriodID, _periodPeriodPaymentPlanRec)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If
        End Sub

        Private Sub LoadPeriodPaymentPlanDetailTemp()
            Dim msg As ErrorMessage

            'Load period payment plan details in to temp table.
            If PeriodID > 0 Then
                msg = PeriodPaymentPlanBL.LoadPeriodPaymentPlanDetailTemp(Me.DbConnection, PeriodID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If
        End Sub

        Private Sub SetFlags()
            Dim msg As ErrorMessage
            msg = PeriodPaymentPlanBL.PaymentPlanPresent(Me.DbConnection, Nothing, _periodPeriodPaymentPlanRec.ID, _paymentPlanPresent)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _paymentConfigPresent = PeriodPaymentPlanBL.PaymentConfigPresent(_periodPeriodPaymentPlanRec)
        End Sub

        Private Sub SetUpStdButtonsConfig()

            Dim msg As ErrorMessage = Nothing
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant(_WebNavMenuItemCommandEditKey))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = canEdit
                .ShowNew = canEdit
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

        End Sub

        Private Sub SetFieldsOnScreen(ByVal periodID As Integer)

            Dim msg As ErrorMessage = Nothing
            msg = DomContractBL.FetchDomContractPeriod(Me.DbConnection, Nothing, periodID, _periodPeriodPaymentPlanRec)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With _periodPeriodPaymentPlanRec
                If _stdBut.ButtonsMode <> StdButtonsMode.Edit Then
                    ContractValue = .ContractValue
                    ExpenditureAccountID = .ExpenditureAccountID
                    RateCategoryID = .DomRateCategoryID
                    ClientGroupID = .ClientGroupID
                    ClientSubGroupID = .ClientSubGroupID
                    FinanceCodeID = .FinanceCodeID
                    TeamID = .TeamID
                    EndDate = .DateTo
                End If
                msg = PeriodPaymentPlanBL.getPeriodPaymentPlanDetailTotal(Nothing, Me.DbConnection, periodID, RunningTotal)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                hidRunningTotal.Value = RunningTotal.ToString("0.00")
            End With

        End Sub

        Private Sub SetUpStdButtons()
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

            With _stdBut
                .AllowNew = (canEdit And (Not _paymentConfigPresent))
                .ShowNew = canEdit
                .AllowFind = False
                .AllowEdit = (canEdit And (Not .AllowNew))
                .AllowDelete = .AllowEdit
            End With
        End Sub

        Private Sub ManagePostBack()
            If Me.IsPostBack AndAlso (_stdBut.ButtonsMode = StdButtonsMode.AddNew) Then
                RefreshControls()
                If _periodPeriodPaymentPlanRec.DateTo <> END_DATE Then
                    EndDate = _periodPeriodPaymentPlanRec.DateTo.ToString(CONST_DATE_FORMAT)
                End If
            Else

                If _paymentConfigPresent Then
                    _stdBut.SelectedItemID = Utils.ToInt32(PeriodID)
                Else
                    _stdBut.SelectedItemID = 0
                End If

                If PeriodID > 0 Then
                    SetFieldsOnScreen(PeriodID)
                Else
                    ResetFields()
                End If
            End If
        End Sub

        Private Sub SetNewDefaults()
            'expand collapsible panel
            cpe.Collapsed = False
            cpe.ClientState = "false"

            'set date picker selected date
            dteFirstPaymentDate.Text = _periodPeriodPaymentPlanRec.DateFrom.ToString(CONST_DATE_FORMAT)
        End Sub

        Private Sub SetValidDateRange()
            Dim msg As ErrorMessage = Nothing
            Dim hasFuturePeriods As Boolean
            Dim lastPaymentToDate As Nullable(Of Date) = Nothing

            With dteEndDate
                .MinimumValue = _periodPeriodPaymentPlanRec.DateFrom.ToString(CONST_DATE_FORMAT)
                msg = DomContractBL.CurrentPeriodHasFuturePeriods(Me.DbConnection, Nothing, _periodPeriodPaymentPlanRec, hasFuturePeriods)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If hasFuturePeriods Then
                    .MaximumValue = _periodPeriodPaymentPlanRec.DateTo.ToString(CONST_DATE_FORMAT)
                    .MinimumValue = _periodPeriodPaymentPlanRec.DateTo.ToString(CONST_DATE_FORMAT)
                End If
                msg = PeriodPaymentPlanBL.getPeriodPaymentPlanLastPaymentToDate(Nothing, Me.DbConnection, _periodPeriodPaymentPlanRec.ID, lastPaymentToDate)
                If lastPaymentToDate.HasValue Then .MinimumValue = lastPaymentToDate.ToString
                .SetupRangeValidator()
            End With
            With dteFirstPaymentDate
                .MinimumValue = _periodPeriodPaymentPlanRec.DateFrom.ToString(CONST_DATE_FORMAT)
                .MaximumValue = _periodPeriodPaymentPlanRec.DateTo.ToString(CONST_DATE_FORMAT)
                .SetupRangeValidator()
            End With
        End Sub

#End Region

    End Class

End Namespace