Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.FinanceCodes
Imports Target.Abacus.Library.FinanceCodes.Services
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.FinanceCodeMatrix.Enquiry
    ''' <summary>
    ''' Admin page used to maintain finance code matrices.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Motahir 29/08/2013 A8158 Unfriendly message if option for matrix is False
    ''' MoTahir 09/07/2013 A7975 Issues arising in testing D12487
    ''' MoTahir 19/06/2013 D12487 - Finance Code Componenet Matrix - Reference Data.
    ''' </history>
    Partial Public Class Edit
        Inherits BasePage

#Region "Fields"

        Dim _fcMatrixConfigurations As FinanceCodeMatrixConfiguration = Nothing
        Private Const CONST_DATE_FORMAT As String = "dd/MM/yyyy"
        Private _expenditureAccountGroupID As Integer
        Private Const CONST_MATRIX_ACCESS_DENIED As String = "System setting for Finance Coding – Derive from matrix is not set which bars access to the Finance Code Matrix."

#End Region

#Region "Properties"

        ''' <summary>
        ''' Get the current financeCodeLengthValidator RegEx String.
        ''' </summary>
        ''' <value>The standard buttons.</value>
        Public ReadOnly Property FinanceCodeMinLenghtValidatorRegExString() As String
            Get
                Return ".{" & FinanceCodeMatrixConfiguration.FinanceCodeLength.ToString & "}.*"
            End Get
        End Property
        ''' <summary>
        ''' Get the current financeCodeLengthValidator Error Msg.
        ''' </summary>
        ''' <value>The standard buttons.</value>
        Public ReadOnly Property FinanceCodeMinLenghtValidatorErrMsg() As String
            Get
                Return String.Format("Minimum finance code length required is {0}", FinanceCodeMatrixConfiguration.FinanceCodeLength)
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

        ''' <summary>
        ''' Gets the client group control.
        ''' </summary>
        ''' <value>The client group control.</value>
        Private ReadOnly Property ClientGroupControl() As InPlaceClientGroupSelector
            Get
                Return CType(txtClientGroup, InPlaceClientGroupSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the client sub group control.
        ''' </summary>
        ''' <value>The client sub group control.</value>
        Private ReadOnly Property ClientSubGroupControl() As InPlaceClientSubGroupSelector
            Get
                Return CType(txtClientSubGroup, InPlaceClientSubGroupSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the team control.
        ''' </summary>
        ''' <value>The team control.</value>
        Private ReadOnly Property TeamControl() As InPlaceTeamSelector
            Get
                Return CType(txtTeam, InPlaceTeamSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the provider control.
        ''' </summary>
        ''' <value>The provider control.</value>
        Private ReadOnly Property ProviderControl() As InPlaceEstablishmentSelector
            Get
                Return CType(txtProvider, InPlaceEstablishmentSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the service type control.
        ''' </summary>
        ''' <value>The service type control.</value>
        Private ReadOnly Property ServiceTypeControl() As InPlaceServiceTypeSelector
            Get
                Return CType(txtServiceType, InPlaceServiceTypeSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the contract control.
        ''' </summary>
        ''' <value>The contract control.</value>
        Private ReadOnly Property ContractControl() As InPlaceDomContractSelector
            Get
                Return CType(txtContract, InPlaceDomContractSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the contract control.
        ''' </summary>
        ''' <value>The contract control.</value>
        Private ReadOnly Property FinanceCodeMatrixConfiguration() As FinanceCodeMatrixConfiguration
            Get
                If _fcMatrixConfigurations Is Nothing Then
                    Dim msg As ErrorMessage
                    msg = FinanceCodeMatrixBL.GetFinanceCodeMatrixConfiguration(Me.DbConnection, Nothing, _fcMatrixConfigurations)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _fcMatrixConfigurations
            End Get
        End Property

        ''' <summary>
        ''' Gets the lower age band control.
        ''' </summary>
        ''' <value>The lower age band control.</value>
        Private Property LowerAgeBand() As Nullable(Of Integer)
            Get
                Dim value As Integer = Target.Library.Utils.ToInt32(txtAgeRangeFrom.Value)

                If value = 0 Then
                    value = 1
                End If
                Return value
            End Get
            Set(value As Nullable(Of Integer))
                If value.HasValue AndAlso value > 0 Then
                    txtAgeRangeFrom.Text = value.ToString
                Else
                    txtAgeRangeFrom.Text = "1"
                End If

            End Set
        End Property

        ''' <summary>
        ''' Gets the upper age band control.
        ''' </summary>
        ''' <value>The upper age band control.</value>
        Private Property UpperAgeBand() As Nullable(Of Integer)
            Get
                Dim value As Integer = Target.Library.Utils.ToInt32(txtAgeRangeTo.Value)

                If value = 0 Then
                    value = 999
                End If
                Return value
            End Get
            Set(value As Nullable(Of Integer))
                If value.HasValue AndAlso value > 0 Then
                    txtAgeRangeTo.Text = value.ToString
                Else
                    txtAgeRangeTo.Text = "999"
                End If

            End Set
        End Property

        Private Property EffectiveTo() As Nullable(Of DateTime)
            Get
                If dteEffectiveDateTo.Value Is Nothing Then
                    Return New Date
                Else
                    Return Target.Library.Utils.ToDateTime(dteEffectiveDateTo.Value)
                End If
                'Return Target.Library.Utils.ToDateTime(dteEffectiveDateTo.Value)
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If value.HasValue And value <> New Date Then
                    dteEffectiveDateTo.Text = value.Value.ToString(CONST_DATE_FORMAT)
                Else
                    dteEffectiveDateTo.Text = String.Empty
                End If
            End Set
        End Property

        Private Property EffectiveFrom() As Nullable(Of DateTime)
            Get
                If dteEffectiveDateFrom.Value Is Nothing Then
                    Return New Date
                Else
                    Return Target.Library.Utils.ToDateTime(dteEffectiveDateFrom.Value)
                End If
                'Return Target.Library.Utils.ToDateTime(dteEffectiveDateTo.Value)
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If value.HasValue And value <> New Date Then
                    dteEffectiveDateFrom.Text = value.Value.ToString(CONST_DATE_FORMAT)
                Else
                    dteEffectiveDateFrom.Text = String.Empty
                End If
            End Set
        End Property

        Public Property ExpenditureAccountGroupID() As Integer
            Get
                Return _expenditureAccountGroupID
            End Get
            Set(ByVal value As Integer)
                _expenditureAccountGroupID = value
            End Set
        End Property



#End Region

#Region "Standard Button Events"

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            EffectiveFrom = FinanceCodeMatrixConfiguration.EffectiveDate
            optExpenditure.Checked = True
            If pnlPrivateOrLA.Visible Then optBoth.Checked = True
            dteEffectiveDateFrom.MinimumValue = FinanceCodeMatrixConfiguration.EffectiveDate
            dteEffectiveDateTo.MinimumValue = FinanceCodeMatrixConfiguration.EffectiveDate
        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            FindClicked(e)
            dteEffectiveDateFrom.MinimumValue = FinanceCodeMatrixConfiguration.EffectiveDate
            dteEffectiveDateTo.MinimumValue = FinanceCodeMatrixConfiguration.EffectiveDate
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim fcMatrix As DataClasses.FinanceCodeMatrix = Nothing

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = FinanceCodeMatrixBL.GetFinanceCodeMatrix(Me.DbConnection, Nothing, e.ItemID, fcMatrix)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With fcMatrix
                If FinanceCodeMatrixConfiguration.ClientGroupVisible Then ClientGroupControl.ClientGroupID = .ClientGroupID
                If FinanceCodeMatrixConfiguration.ClientSubGroupVisible Then ClientSubGroupControl.ClientSubGroupID = .ClientSubGroupID

                If FinanceCodeMatrixConfiguration.AgeRangeVisible Then
                    LowerAgeBand = .LowerAgeBand
                    UpperAgeBand = .UpperAgeBand
                End If

                If FinanceCodeMatrixConfiguration.TeamVisible Then TeamControl.TeamID = .TeamID
                If FinanceCodeMatrixConfiguration.ProviderVisible Then ProviderControl.EstablishmentID = .ProviderID
                If FinanceCodeMatrixConfiguration.PrivateOrLAVisible Then
                    If .PrivateLAIndicator = FinanceCodeMatrixBL.PrivateOrLocalAuthority.IsPrivate Then
                        optPrivate.Checked = True
                    ElseIf .PrivateLAIndicator = FinanceCodeMatrixBL.PrivateOrLocalAuthority.IsLocal Then
                        optLA.Checked = True
                    Else
                        optBoth.Checked = True
                    End If
                End If
                If FinanceCodeMatrixConfiguration.ServiceTypeVisible Then ServiceTypeControl.ItemID = .ServiceTypeID
                If FinanceCodeMatrixConfiguration.ContractNumberVisible Then ContractControl.ContractID = .ContractID
                If FinanceCodeMatrixConfiguration.ServiceOrderVisible Then txtServiceOrderRef.Text = .ServiceOrderReference

                Dim expAccount As ExpenditureAccount = New ExpenditureAccount(DbConnection)
                msg = expAccount.Fetch(.ExpenditureAccountID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If Not expAccount Is Nothing Then
                    CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeID = expAccount.FinanceCodeID_Expenditure
                End If

                EffectiveFrom = .DateFrom
                EffectiveTo = .DateTo

                If .IncomeOrExpenditureFlag = FinanceCodeMatrixBL.CONST_EXPENDITURE_FLAG Then
                    optExpenditure.Checked = True
                Else
                    optIncome.Checked = True
                End If

            End With

        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = FinanceCodeMatrixBL.DeleteFinanceCodeMatrix(Me.DbConnection, e.ItemID)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing
            Dim fcm As DataClasses.FinanceCodeMatrix = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If e.ItemID > 0 Then
                ' update
                msg = FinanceCodeMatrixBL.GetFinanceCodeMatrix(Me.DbConnection, Nothing, e.ItemID, fcm)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            Else
                fcm = New DataClasses.FinanceCodeMatrix(String.Empty, String.Empty)
            End If

            Me.Validate("Save")

            If Me.IsValid Then

                With fcm

                    .DateFrom = FinanceCodeMatrixConfiguration.EffectiveDate

                    'If Not IsNothing(EffectiveFrom) Then .DateFrom = EffectiveFrom
                    'If Not IsNothing(EffectiveTo) Then .DateTo = EffectiveTo

                    Dim finCodeID As Integer = CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeID


                    msg = FinanceCodeBL.MaintainMatrixExpenditureFinanceCodes(DbConnection, _
                                                                              ExpenditureAccountGroupID, _
                                                                              finCodeID, _
                                                                              .ExpenditureAccountID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)


                    If optExpenditure.Checked Then
                        .IncomeOrExpenditureFlag = FinanceCodeMatrixBL.CONST_EXPENDITURE_FLAG
                    ElseIf optIncome.Checked Then
                        .IncomeOrExpenditureFlag = FinanceCodeMatrixBL.CONST_INCOME_FLAG
                    End If
                    If pnlClientGroup.Visible Then .ClientGroupID = Utils.ToInt32(Request.Form(ClientGroupControl.HiddenFieldUniqueID))
                    If pnlClientSubGroup.Visible Then .ClientSubGroupID = Utils.ToInt32(Request.Form(ClientSubGroupControl.HiddenFieldUniqueID))
                    'If pnlAgeRange.Visible Then
                    If Not UpperAgeBand Is Nothing Then
                        .UpperAgeBand = UpperAgeBand
                    Else
                        .UpperAgeBand = 999
                    End If

                    If Not LowerAgeBand Is Nothing Then
                        .LowerAgeBand = LowerAgeBand
                    Else
                        .LowerAgeBand = 1
                    End If

                    'End If
                    If pnlTeam.Visible Then .TeamID = Utils.ToInt32(Request.Form(TeamControl.HiddenFieldUniqueID))
                    If pnlProvider.Visible Then .ProviderID = Utils.ToInt32(Request.Form(ProviderControl.HiddenFieldUniqueID))
                    If pnlPrivateOrLA.Visible Then
                        If optPrivate.Checked Then
                            .PrivateLAIndicator = FinanceCodeMatrixBL.PrivateOrLocalAuthority.IsPrivate
                        ElseIf optLA.Checked Then
                            .PrivateLAIndicator = FinanceCodeMatrixBL.PrivateOrLocalAuthority.IsLocal
                        ElseIf optBoth.Checked Then
                            .PrivateLAIndicator = FinanceCodeMatrixBL.PrivateOrLocalAuthority.NoneSelected
                        End If
                    End If
                    If pnlServiceType.Visible Then .ServiceTypeID = Utils.ToInt32(Request.Form(ServiceTypeControl.HiddenFieldUniqueID))
                    If pnlContractNo.Visible Then .ContractID = Utils.ToInt32(Request.Form(ContractControl.HiddenFieldUniqueID))
                    If pnlServiceOrderRef.Visible Then .ServiceOrderReference = txtServiceOrderRef.Text

                    ' save the finance code matrix  using bl
                    msg = FinanceCodeMatrixBL.SaveFinanceCodeMatrix(DbConnection, fcm)
                    If Not msg.Success Then
                        ' 
                        If msg.Number = FinanceCodeMatrixBL.Error_FinanceCodeMatrixInvalid Then
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

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                EffectiveFrom = Nothing
                EffectiveTo = Nothing
                CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeID = 0
                optIncome.Checked = False
                optExpenditure.Checked = False
                If pnlClientGroup.Visible Then ClientGroupControl.ClientGroupID = 0
                If pnlClientSubGroup.Visible Then ClientSubGroupControl.ClientSubGroupID = 0
                If pnlAgeRange.Visible Then
                    txtAgeRangeFrom.Text = String.Empty
                    txtAgeRangeTo.Text = String.Empty
                End If
                If pnlTeam.Visible Then TeamControl.TeamID = 0
                If pnlProvider.Visible Then ProviderControl.EstablishmentID = 0
                If pnlPrivateOrLA.Visible Then
                    optPrivate.Checked = False
                    optLA.Checked = False
                    optBoth.Checked = False
                End If
                If pnlServiceType.Visible Then ServiceTypeControl.ItemID = 0
                If pnlContractNo.Visible Then ContractControl.ContractID = 0
                If pnlServiceOrderRef.Visible Then txtServiceOrderRef.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region "Page Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Commitments.ReferenceDate.FinanceCodeMatrix"), "Finance Code Matrix")
            Dim msg As ErrorMessage = Nothing
            Dim deriveFromMatrix As Boolean = False
            Dim expAccountGroupList As ExpenditureAccountGroupCollection = Nothing

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Library.FinanceCodes.Services.FinanceCodeMatrixService))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Library.FinanceCodes.Services.FinanceCodeService))

            'check if derive from matrix aplication setting present and configured.
            msg = FinanceCodeMatrixBL.GetDeriveFromMatrixApplicationSetting(Me.DbConnection, Nothing, deriveFromMatrix)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not deriveFromMatrix Then
                msg = New ErrorMessage
                msg.Message = CONST_MATRIX_ACCESS_DENIED
                WebUtils.DisplayError(msg)
            End If

            msg = DataClasses.ExpenditureAccountGroup.FetchList(conn:=Me.DbConnection, systemType:=1, list:=expAccountGroupList)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not expAccountGroupList Is Nothing AndAlso expAccountGroupList.Count > 0 Then
                ExpenditureAccountGroupID = expAccountGroupList(0).ID
            End If

            CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeCategoryID = 1 'Expenditure

            With StandardButtonsControl
                .AllowBack = True
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Commitments.ReferenceDate.FinanceCodeMatrix.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Commitments.ReferenceDate.FinanceCodeMatrix.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Commitments.ReferenceDate.FinanceCodeMatrix.Delete"))
                .AllowFind = False
                .EditableControls.Add(fsControls.Controls)
                .AuditLogTableNames.Add("FinanceCodeMatrix")
                ' add handlers
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf EditClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
            End With

            ' add utility JS link
            With Me.JsLinks
                .Add("Edit.js")
                .Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
                .Add(WebUtils.GetVirtualPath("Library/Javascript/Dialog.js"))
                .Add(WebUtils.GetVirtualPath("Library/JavaScript/date.js"))
                .Add(WebUtils.GetVirtualPath("Library/JavaScript/Utils.js"))
            End With

            ' add in the jquery library
            UseJQuery = True
            UseJqueryUI = True
            UseJqueryTextboxClearer = True
            UseExt = True


            SetUpPanelVisibility()
            SetupValidators()

        End Sub

        Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

            Dim msg As ErrorMessage = Nothing
            Dim serviceOrderCount As Integer = 0

            If StandardButtonsControl.SelectedItemID > 0 Then

                If Not (IsNothing(EffectiveTo) And EffectiveTo <> New Date) And (EffectiveTo < FinanceCodeMatrixConfiguration.EffectiveDate) Then
                    StandardButtonsControl.AllowEdit = False
                    StandardButtonsControl.AllowDelete = False

                    With (lblError)

                        If .Text = String.Empty Then
                            .Text = "This records effective to date is less than finance code matrix configuration effective date. You can not edit this record<br /><br />"
                            .CssClass = "warningText"
                        End If

                    End With

                End If

                msg = FinanceCodeMatrixBL.GetNumberOfServiceOrdersAssociatedToFinanceCodeMatrix(Me.DbConnection, Nothing, StandardButtonsControl.SelectedItemID, serviceOrderCount)

                If serviceOrderCount > 0 Then
                    With (lblError)
                        If .Text = String.Empty Then
                            .Text = serviceOrderCount.ToString() _
                            & " service order funding records have been allocated this finance code from this matrix entry.  Editing or deleting this entry will cause them to be reconsidered<br />" _
                            & "which may result in them being recoded differently.  This will not cause automatic adjustments to the ledger. If adjustments are required they must be done manually<br /><br />" _
                            & "Service orders will be reconsidered when the service order import job is next run."
                            .CssClass = "warningText"
                        End If

                    End With
                End If

                'dteEffectiveDateFrom.Enabled = False
                'dteEffectiveDateTo.Enabled = False
                'optIncome.Visible = False
            End If

        End Sub

        Private Sub Page_PreRenderComplete(sender As Object, e As System.EventArgs) Handles Me.PreRenderComplete

            Const SCRIPT_STARTUP As String = "Startup"
            Dim js As StringBuilder = New StringBuilder()

            'AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Library.FinanceCodes.Services.FinanceCodeMatrixService))

            dteEffectiveDateFrom.TextBox.Attributes.Add("onchange", _
                                         "javascript:dteEffectiveDateFrom_Changed();")

            SetupValidators()

            js.AppendFormat("Edit_dteEffectiveDateFromID='{0}';", dteEffectiveDateFrom.ClientID)
            js.AppendFormat("Edit_dteEffectiveDateToID='{0}';", dteEffectiveDateTo.ClientID)
            js.AppendFormat("Edit_txtClientSubGroupID='{0}';", txtClientSubGroup.ClientID)
            js.AppendFormat("Edit_txtClientGroupID='{0}';", txtClientGroup.ClientID)
            js.AppendFormat("Edit_txtAgeRangeFromID='{0}';", txtAgeRangeFrom.ClientID)
            js.AppendFormat("Edit_txtAgeRangeToID='{0}';", txtAgeRangeTo.ClientID)
            js.AppendFormat("Edit_txtTeamID='{0}';", txtTeam.ClientID)
            js.AppendFormat("Edit_txtProviderID='{0}';", txtProvider.ClientID)
            js.AppendFormat("Edit_txtServiceTypeID='{0}';", txtServiceType.ClientID)
            js.AppendFormat("Edit_txtContractID='{0}';", txtContract.ClientID)
            js.AppendFormat("Edit_txtServiceOrderRefID='{0}';", txtServiceOrderRef.ClientID)
            js.AppendFormat("InPlaceContractSelectorVisible={0};", txtContract.Visible.ToString.ToLower)
            js.AppendFormat("Init();")

            If Not ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                                    js.ToString(), _
                                    True)
            End If

            If StandardButtonsControl.ButtonsMode = StdButtonsMode.Edit Or StandardButtonsControl.ButtonsMode = StdButtonsMode.AddNew Then
                btnNewFinanceCode.Visible = True
            Else
                btnNewFinanceCode.Visible = False
            End If


            dteEffectiveDateFrom.Enabled = False
            dteEffectiveDateTo.Enabled = False
            optIncome.Visible = False
            lblIncome.Visible = False

        End Sub

#End Region

#Region "Helpers"

        Private Sub SetupValidators()

            'txtFinanceCode.MaxLength = FinanceCodeMatrixConfiguration.FinanceCodeLength
            'txtFinanceCodeMinLenghtValidator.ErrorMessage = String.Format("Minimum finance code length required is {0}", FinanceCodeMatrixConfiguration.FinanceCodeLength)
            'txtFinanceCodeMinLenghtValidator.ValidationExpression = ".{" & FinanceCodeMatrixConfiguration.FinanceCodeLength.ToString & "}.*"

        End Sub

        Private Sub SetUpPanelVisibility()
            'show or hide panels dependent on configuration settings
            With FinanceCodeMatrixConfiguration
                pnlClientGroup.Visible = .ClientGroupVisible
                pnlClientSubGroup.Visible = .ClientSubGroupVisible
                pnlAgeRange.Visible = .AgeRangeVisible
                pnlTeam.Visible = .TeamVisible
                pnlProvider.Visible = .ProviderVisible
                pnlPrivateOrLA.Visible = .PrivateOrLAVisible
                pnlServiceType.Visible = .ServiceTypeVisible
                pnlContractNo.Visible = .ContractNumberVisible
                pnlServiceOrderRef.Visible = .ServiceOrderVisible
            End With

            If (pnlClientGroup.Visible Or _
               pnlClientSubGroup.Visible Or _
               pnlAgeRange.Visible Or _
               pnlTeam.Visible Or _
                pnlProvider.Visible Or _
                pnlPrivateOrLA.Visible Or _
                pnlServiceType.Visible Or _
                pnlContractNo.Visible Or _
                pnlServiceOrderRef.Visible) Then
                fsFactors.Visible = True
            Else
                fsFactors.Visible = False
            End If
        End Sub

#End Region

    End Class
End Namespace
