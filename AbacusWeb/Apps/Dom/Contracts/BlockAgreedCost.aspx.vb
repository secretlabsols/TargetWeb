
Imports System.Collections.Generic
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

Namespace Apps.Dom.Contracts

	''' <summary>
	''' Screen used to maintain a domiciliary contract header.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JF      29/08/2013  Initial version (D12503)
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class BlockAgreedCost
        Inherits BasePage

        Private _contract As DomContract = Nothing
        Private _contractID As Integer
        Private _periodID As Integer
        Private _agreedCostRec As Target.Abacus.Library.DataClasses.BlockAgreedCost
        Private _agreedCostRecID As Integer
        Private _refreshTree As Boolean
        Private _stdBut As StdButtonsBase
        Private _contractEnded As Boolean
        Private _failedValidation As Boolean

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .hideCreditorRef = True
                .hideDebtorRef = True
            End With
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim periodRec As DomContractPeriod = Nothing

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Period Block Agreed Cost")

            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _periodID = Utils.ToInt32(Request.QueryString("periodID"))

            Dim msg As ErrorMessage
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

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

            '++ Get the contract..
            _contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = _contract.Fetch(_contractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _contractEnded = (_contract.EndDate <> DataUtils.MAX_DATE)
            '++ ..then get the contract period..
            periodRec = New DomContractPeriod(Me.DbConnection, String.Empty, String.Empty)
            msg = periodRec.Fetch(_periodID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            '++ ..then get the contract period's block agreed cost details.
            _agreedCostRec = New Target.Abacus.Library.DataClasses.BlockAgreedCost(Me.DbConnection, String.Empty, String.Empty)
            If Utils.ToInt32(periodRec.BlockAgreedCostID) > 0 Then
                With _agreedCostRec
                    msg = .Fetch(periodRec.BlockAgreedCostID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    _agreedCostRecID = .ID
                End With
            Else
                _agreedCostRecID = 0
            End If

            If Me.IsPostBack Then RefreshControls()
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim msg As ErrorMessage
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

            If Me.IsPostBack AndAlso (_stdBut.ButtonsMode = StdButtonsMode.AddNew) Then
                RefreshControls()
            Else
                If Not _failedValidation Then
                    _stdBut.SelectedItemID = Utils.ToInt32(_agreedCostRecID)
                    If _agreedCostRecID > 0 Then
                        _agreedCostRec = New Target.Abacus.Library.DataClasses.BlockAgreedCost(Me.DbConnection, String.Empty, String.Empty)
                        With _agreedCostRec
                            msg = .Fetch(_agreedCostRecID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            txtAgreedPayment.Text = Utils.ToDecimal(.AgreedWeeklyPayment.ToString("0.00"))
                            SetupClientSelector(.SystemAccountID, False)
                            PopulateRateCategories(.RateCategoryID)
                            SetupFinanceCodeSelector(.FinanceCodeID, False)
                        End With
                    Else
                        ResetFields()
                    End If

                    _failedValidation = False
                End If
            End If

            With _stdBut
                .AllowNew = (canEdit And (_agreedCostRecID = 0))
                .ShowNew = canEdit
                .AllowFind = False
                .AllowEdit = (canEdit And (Not .AllowNew))
                .AllowDelete = .AllowEdit
            End With

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            _stdBut.Visible = Not _contractEnded
        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            ResetFields()
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

            If e.ItemID > 0 Then
                _agreedCostRec = New Target.Abacus.Library.DataClasses.BlockAgreedCost(Me.DbConnection, String.Empty, String.Empty)
                With _agreedCostRec
                    msg = .Fetch(e.ItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtAgreedPayment.Text = Utils.ToDecimal(.AgreedWeeklyPayment.ToString("0.00"))
                    SetupClientSelector(.SystemAccountID, False)
                    PopulateRateCategories(.RateCategoryID)
                    SetupFinanceCodeSelector(.FinanceCodeID, False)
                End With
            Else
                ResetFields()
            End If

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
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim periodRec As DomContractPeriod = Nothing

            If _agreedCostRecID > 0 AndAlso _periodID > 0 Then
                periodRec = New DomContractPeriod(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                msg = periodRec.Fetch(_periodID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With periodRec
                    .BlockAgreedCostID = Nothing
                    msg = .Save()
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    msg = Target.Abacus.Library.DataClasses.BlockAgreedCost.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _agreedCostRecID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With

                _agreedCostRecID = 0
                _stdBut.SelectedItemID = 0
                ResetFields()
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
                        _failedValidation = True
                        lblError.Text = "A rate category must be selected."
                        e.Cancel = True
                        Exit Sub
                    End If

                    If Not (Utils.ToDecimal(txtAgreedPayment.Text) > 0) Then
                        _failedValidation = True
                        lblError.Text = "The Agreed Weekly Payment entry must be a positive amount."
                        e.Cancel = True
                        Exit Sub
                    End If

                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    _agreedCostRec = New Target.Abacus.Library.DataClasses.BlockAgreedCost(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    If e.ItemID > 0 Then
                        '++ Update of existing record..
                        With _agreedCostRec
                            msg = .Fetch(e.ItemID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End With
                    Else
                        '++ New record..
                        _agreedCostRec.Unhook()
                    End If
                    With _agreedCostRec
                        .AgreedWeeklyPayment = Utils.ToDecimal(txtAgreedPayment.Text)
                        .SystemAccountID = Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID))
                        .RateCategoryID = Utils.ToInt32(cboRateCategory.DropDownList.SelectedValue)
                        .FinanceCodeID = Utils.ToInt32(Request.Form(CType(financeCode, InPlaceFinanceCodeSelector).HiddenFieldUniqueID))
                        .AuditLogOverriddenParentID = _contractID
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        e.ItemID = .ID
                        _agreedCostRecID = .ID
                        _refreshTree = True

                        If _periodID > 0 Then
                            Dim periodRecs As DomContractPeriodCollection = Nothing
                            Dim idList As New Generic.List(Of Integer)
                            idList.Add(_periodID)
                            msg = DomContractPeriod.FetchListByIDS(trans, idList, periodRecs, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            If periodRecs IsNot Nothing AndAlso periodRecs.Count > 0 Then
                                With periodRecs(0)
                                    .DbTransaction = trans
                                    .BlockAgreedCostID = _agreedCostRecID
                                    msg = .Save()
                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                End With
                            End If
                        End If
                    End With

                    trans.Commit()

                    msg = New ErrorMessage()
                    msg.Success = True

                Catch ex As Exception
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub SetupClientSelector(ByVal clientID As Integer, ByVal allowBlanks As Boolean)
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .ClientDetailID = clientID
                If allowBlanks Then
                    .Required = False
                    .ValidationGroup = ""
                Else
                    .Required = True
                    .ValidationGroup = "Save"
                End If
            End With
        End Sub

        Private Sub SetupFinanceCodeSelector(ByVal finCodeID As Integer, ByVal allowBlanks As Boolean)
            With CType(Me.financeCode, InPlaceSelectors.InPlaceFinanceCodeSelector)
                .FinanceCodeID = finCodeID
                If allowBlanks Then
                    .Required = False
                    .ValidationGroup = ""
                Else
                    .Required = True
                    .ValidationGroup = "Save"
                End If
            End With
        End Sub

        Private Sub PopulateRateCategories(ByVal selectedID As Integer)
            Dim uoms As DomUnitsOfMeasureCollection = Nothing
            Dim rateCategories As DomRateCategoryCollection = Nothing
            Dim msg As ErrorMessage

            cboRateCategory.DropDownList.Items.Clear()
            If _contract IsNot Nothing Then
                '++ Fetch all money-type rate categories for this contract's framework..
                msg = DomUnitsOfMeasure.FetchList(conn:=Me.DbConnection, list:=uoms, _
                                              auditLogTitle:=String.Empty, auditUserName:=String.Empty, _
                                              systemType:=2)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If uoms IsNot Nothing AndAlso uoms.Count > 0 Then
                    msg = DomRateCategory.FetchList(conn:=Me.DbConnection, list:=rateCategories, _
                                                  auditLogTitle:=String.Empty, auditUserName:=String.Empty, _
                                                  domRateFrameworkID:=_contract.DomRateFrameworkID, _
                                                  domUnitsOfMeasureID:=uoms(0).ID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                Dim itemList As New List(Of ListItem)
                itemList.Add(New ListItem(String.Empty, 0))
                For Each rateCateg As DomRateCategory In rateCategories
                    Dim item As New ListItem

                    item.Text = rateCateg.Description
                    item.Value = rateCateg.ID.ToString
                    itemList.Add(item)
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

        Private Sub ResetFields()
            txtAgreedPayment.Text = String.Empty
            SetupClientSelector(0, False)
            PopulateRateCategories(0)
            SetupFinanceCodeSelector(0, False)

            _stdBut.AllowDelete = False
        End Sub

        Private Sub RefreshControls()
            Dim sysAccID As Integer = Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID))
            Dim financeCodeID As Integer = Utils.ToInt32(Request.Form(CType(financeCode, InPlaceFinanceCodeSelector).HiddenFieldUniqueID))
            Dim rateCategoryID As Integer = Utils.ToInt32(cboRateCategory.GetPostBackValue())
            Dim wkylAmount As String = txtAgreedPayment.GetPostBackValue

            txtAgreedPayment.Text = wkylAmount
            SetupClientSelector(sysAccID, False)
            PopulateRateCategories(rateCategoryID)
            SetupFinanceCodeSelector(financeCodeID, False)
        End Sub
    End Class

End Namespace