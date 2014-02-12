
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
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

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Admin page used to enter new manual payment domiciliary pro forma invoices.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  01/09/2010  Added validation summary.
    ''' CD   12/07/2010  A4WA#7864 - Prevent week ending date being overridden when click add button on details tab
    ''' Mo   19/05/2010  D11806 - Rate Category Ordering
    ''' MvO  09/12/2009  A4WA#5946 - various fixes found after testing of D11642.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' MvO  07/04/2008  Created
    ''' </history>
    Partial Public Class EnterManualPayment
        Inherits BasePage

        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER_NEW_LINES As String = "Counter"

        Const CTRL_RATE_CATEGORY As String = "cboRateCategory"
        Const CTRL_COMMENT As String = "txtComment"
        Const CTRL_UNITS As String = "txtUnits"
        Const CTRL_UNIT_COST As String = "txtUnitCost"
        Const CTRL_LINE_VALUE As String = "txtLineValue"
        Const CTRL_FINANCE_CODE As String = "txtFinanceCode"
        Const CTRL_REMOVE As String = "btnRemove"
        Const CTRL_ID As String = "hidID"

        Const UNIQUEID_PREFIX_NEW_DETAIL As String = "detailN"
        Const UNIQUEID_PREFIX_UPDATE_DETAIL As String = "detailU"
        Const UNIQUEID_PREFIX_DELETE_DETAIL As String = "detailD"

        Private _providerID As Integer
        Private _contractID As Integer
        Private _invoiceID As Integer
        Private _newInvoice As Boolean
        Private _periodID As Integer
        Private _systemAccountID As Integer
        Private _newDetailIDCounter As Integer
        Private _startupJS As StringBuilder = New StringBuilder()
        Private _stdBut As StdButtonsBase
        Private _idList As List(Of String)
        Private _canEdit As Boolean
        Private _canDelete As Boolean
        Private _btnAuditDetails As HtmlInputButton = New HtmlInputButton("button")
        Private _btnAddClicked As Boolean = False
        Private _auditUserName As String
        Private _auditLogTitle As String

#Region " Page_Load "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            With _stdBut
                AddHandler .AddCustomControls, AddressOf StdButtons_AddCustomControls
            End With
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim accounts As vwDomContractPeriodSystemAccountCollection = Nothing
            Dim msg As ErrorMessage

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ManualPayments"), "Manual Payment")
            Me.ShowValidationSummary = True

            _auditUserName = SecurityBL.GetCurrentUser().ExternalUsername
            _auditLogTitle = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)

            _invoiceID = Utils.ToInt32(Request.QueryString("id"))
            _newInvoice = (_invoiceID = 0)
            _providerID = Utils.ToInt32(Request.QueryString("estabID"))
            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _systemAccountID = Utils.ToInt32(Request.QueryString("sysAccID"))

            ' setup buttons
            With _stdBut
                .EditableControls.Add(fsHeader.Controls)
                .EditableControls.Add(fsDetails.Controls)
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManualPayments.AddNew"))
                .ShowNew = False
                .AllowFind = False
                .AllowBack = True
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManualPayments.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManualPayments.Delete"))
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            ' no invoiceID means we are creating a new manual payment, so check permissions
            If _invoiceID = 0 Then
                If Not Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManualPayments.AddNew")) Then
                    WebUtils.DisplayAccessDenied()
                End If
            End If


            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/WebSvcUtils.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))
            Me.JsLinks.Add("EnterManualPayment.js")

            If Me.IsPostBack Then
                ' grab posted back values
                If _invoiceID = 0 Then
                    _providerID = Utils.ToInt32(Request.Form(CType(provider, InPlaceEstablishmentSelector).HiddenFieldUniqueID))
                    _contractID = Utils.ToInt32(Request.Form(CType(domContract, InPlaceDomContractSelector).HiddenFieldUniqueID))
                    _periodID = Utils.ToInt32(cboPeriod.GetPostBackValue())
                    _systemAccountID = Utils.ToInt32(cboSystemAccount.GetPostBackValue())
                Else
                    Dim Invoice As DomProformaInvoice = New DomProformaInvoice(Me.DbConnection, _
                                                                               auditUserName:=_auditUserName, _
                                                                               auditLogTitle:=_auditLogTitle)
                    Dim batch As DomProformaInvoiceBatch = New DomProformaInvoiceBatch(Me.DbConnection, _
                                                                                       _auditUserName, _
                                                                                       _auditLogTitle)
                    With Invoice
                        msg = .Fetch(_invoiceID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        _systemAccountID = .ClientID
                    End With
                    With batch
                        msg = .Fetch(Invoice.DomProformaInvoiceBatchID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        _providerID = .ProviderID
                        _contractID = .DomContractID
                        _periodID = .DomContractPeriodID
                        PopulateAuditDetails(.CreatedBy, .DateCreated)
                    End With
                End If

                SetupProviderSelector(_providerID)
                SetupContractSelector(_contractID, _providerID)
                PopulateDropdowns()

                ' re-create the list of details (from view state)
                _idList = GetUniqueIDsFromViewState()
                For Each id As String In _idList
                    OutputDetailControls(id, Nothing)
                Next

            End If

            cboPeriod.DropDownList.Attributes.Add("onchange", "cboPeriod_OnChange()")

        End Sub

#End Region

#Region " StdButtons_AddCustomControls "

        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            With _btnAuditDetails
                .ID = "btnAuditDetails"
                .Value = "Audit Details"
            End With
            controls.Add(_btnAuditDetails)

            With CType(auditDetails, IBasicAuditDetails)
                .ToggleControlID = _btnAuditDetails.ClientID
            End With

        End Sub

#End Region

#Region " stdButton Events "

#Region "       FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim uniqueID As String
            Dim msg As ErrorMessage
            Dim batch As DomProformaInvoiceBatch
            Dim invoice As DomProformaInvoice
            Dim sysAccount As DomContractPeriodSystemAccount = Nothing
            Dim invoiceLines As vwDomProformaInvoiceDetailCollection = Nothing
            Dim client As ClientDetail
            Dim invoiceTotal As Decimal

            ' get the invoice
            invoice = New DomProformaInvoice(Me.DbConnection, _
                                             auditUserName:=_auditUserName, _
                                             auditLogTitle:=_auditLogTitle)
            With invoice
                msg = .Fetch(_invoiceID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _systemAccountID = .ClientID
            End With

            ' get the batch
            batch = New DomProformaInvoiceBatch(Me.DbConnection, _
                                                auditUserName:=_auditUserName, _
                                                auditLogTitle:=_auditLogTitle)
            With batch
                msg = .Fetch(invoice.DomProformaInvoiceBatchID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                _providerID = .ProviderID
                _contractID = .DomContractID
                _periodID = .DomContractPeriodID

                PopulateAuditDetails(.CreatedBy, .DateCreated)
            End With

            SetupProviderSelector(_providerID)
            SetupContractSelector(_contractID, _providerID)
            PopulateDropdowns()

            dteWeekending.Text = invoice.WETo

            ' get the invoice details
            msg = vwDomProformaInvoiceDetail.FetchList(Me.DbConnection, invoiceLines, _invoiceID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get the client/system account
            client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
            With client
                msg = .Fetch(invoice.ClientID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End With

            txtPaymentRef.Text = invoice.OurReference

            ' populate invoice lines
            ClearViewState()
            _idList = GetUniqueIDsFromViewState()

            invoiceTotal = 0
            For Each detail As vwDomProformaInvoiceDetail In invoiceLines
                invoiceTotal += detail.CalculatedPayment
                uniqueID = GetUniqueID(detail)
                _idList.Add(uniqueID)
                OutputDetailControls(uniqueID, detail)
            Next
            PersistUniqueIDsToViewState(_idList)

            txtInvoiceTotal.Text = invoiceTotal.ToString("C")
            lblLineValueTotal.Text = invoiceTotal.ToString("C")

            If batch.DomProformaInvoiceBatchStatusID = DomProformaInvoiceBatchStatus.Verified And _
                                                                batch.WorkInProgress = TriState.False Then
                _canDelete = True
                _canEdit = True
            End If


        End Sub

#End Region

#Region "       NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            PopulateDropdowns()
            SetupProviderSelector(_providerID)
            SetupContractSelector(_contractID, _providerID)
            PopulateAuditDetails(String.Empty, Nothing)
        End Sub

#End Region

#Region "       DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim invoice As DomProformaInvoice = New DomProformaInvoice(Me.DbConnection, _auditUserName, _auditLogTitle)

            msg = invoice.Fetch(_invoiceID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = DomContractBL.DeleteManualPaymentProformaInvoiceBatch(Me.DbConnection, currentUser.ExternalUsername, _
                                                                        AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                                        invoice.DomProformaInvoiceBatchID)
            If Not msg.Success Then
                If msg.Number = DomContractBL.ERR_COULD_NOT_DELETE_DOM_MANUAL_PAYMENT Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                Else
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
            Else
                _invoiceID = 0
                CancelClicked(e)
            End If

        End Sub

#End Region

#Region "       CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            ClearViewState()
            If _invoiceID = 0 Then
                _invoiceID = 0
                _providerID = 0
                _contractID = 0
                _periodID = 0
                _systemAccountID = 0

                SetupProviderSelector(_providerID)
                SetupContractSelector(_contractID, _providerID)
                PopulateDropdowns()

                dteWeekending.Text = String.Empty
                txtInvoiceTotal.Text = String.Empty
                txtPaymentRef.Text = String.Empty
                PopulateAuditDetails(String.Empty, Nothing)
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region "       SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim batch As DomProformaInvoiceBatch
            Dim invoice As DomProformaInvoice
            Dim invoiceDetail As DomProformaInvoiceDetail
            Dim invoiceDetails As DomProformaInvoiceDetailCollection
            Dim client As ClientDetail
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim idList As List(Of String)
            Dim uniqueID As String
            Dim invDetToDelete As List(Of String)




            Me.Validate("Save")

            If Me.IsValid() Then

                If Convert.ToDateTime(dteWeekending.Text) <> DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing, dteWeekending.Text) Then
                    lblError.Text = "The weekending date is not a valid weekending date."
                    e.Cancel = True
                    Exit Sub
                End If


                batch = New DomProformaInvoiceBatch(Me.DbConnection, _
                                                    currentUser.ExternalUsername, _
                                                    AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                invoice = New DomProformaInvoice(Me.DbConnection, _
                                                 currentUser.ExternalUsername, _
                                                 AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                If _newInvoice Then
                    ' batch
                    With batch
                        .ProviderID = _providerID
                        .DomContractID = _contractID
                        .DomContractPeriodID = cboPeriod.DropDownList.SelectedValue
                        .DomProformaInvoiceBatchTypeID = Convert.ToInt32(DomProformaInvoiceBatchType.ManualPayment)
                        .VisitBasedReturn = TriState.False
                        .UserID = currentUser.ExternalUserID
                        .DateCreated = DateTime.Now
                        .CreatedBy = currentUser.ExternalUsername
                        .DomProformaInvoiceBatchStatusID = Convert.ToInt32(DomProformaInvoiceBatchStatus.Verified)
                        .StatusDate = .DateCreated
                        .StatusChangedBy = .CreatedBy
                    End With
                Else
                    msg = invoice.Fetch(_invoiceID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    msg = batch.Fetch(invoice.DomProformaInvoiceBatchID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                ' invoice
                With invoice
                    .ClientID = Utils.ToInt32(cboSystemAccount.GetPostBackValue())
                    ' get client name
                    client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
                    msg = client.Fetch(.ClientID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    .ServiceUserDetails = client.Name
                    .WETo = dteWeekending.Text 'DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing)
                    .WEFrom = .WETo
                    .OurReference = txtPaymentRef.Text
                    .InvoiceDate = batch.DateCreated.Date
                    .DateReceived = batch.DateCreated.Date
                    .ServiceUserContribution = 0
                    .PaymentClaimed = 0
                    .Query = TriState.False
                End With

                'details
                invDetToDelete = New List(Of String)
                invoiceDetails = New DomProformaInvoiceDetailCollection
                idList = GetUniqueIDsFromViewState()
                For Each uniqueID In idList
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_DETAIL) Then
                        ' we are deleting
                        invDetToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE_DETAIL, String.Empty))
                    Else
                        'If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_DETAIL) Then
                        ' create empty detail
                        invoiceDetail = New DomProformaInvoiceDetail(currentUser.ExternalUsername, _
                                                                     AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        With invoiceDetail
                            If Utils.ToInt32(CType(phDetails.FindControl(CTRL_ID & uniqueID), HiddenField).Value) <> 0 Then
                                invoiceDetail.DbConnection = Me.DbConnection
                                msg = invoiceDetail.Fetch(Utils.ToInt32(CType(phDetails.FindControl(CTRL_ID & uniqueID), HiddenField).Value))
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                            .DomRateCategoryID = Utils.ToInt32( _
                                CType(phDetails.FindControl(CTRL_RATE_CATEGORY & uniqueID), DropDownListEx).GetPostBackValue() _
                            )
                            ' unit cost
                            msg = DomContractBL.GetUnitCost(Me.DbConnection, _
                                                            Convert.ToInt32(cboPeriod.DropDownList.SelectedValue), _
                                                            .DomRateCategoryID, _
                                                            .UnitCost)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            .ItemCount = 1
                            .NetUnitsPaid = Convert.ToDecimal( _
                                CType(phDetails.FindControl(CTRL_UNITS & uniqueID), TextBoxEx).Text _
                            )
                            .CalculatedPayment = (.UnitCost * .NetUnitsPaid).ToString("F2")
                            .NetChargeableUnits = 0
                            .Comment = CType(phDetails.FindControl(CTRL_COMMENT & uniqueID), TextBoxEx).Text
                            .ManualFinanceCode = CType(phDetails.FindControl(CTRL_FINANCE_CODE & uniqueID), InPlaceFinanceCodeSelector).FinanceCodeText
                        End With
                        invoiceDetails.Add(invoiceDetail)
                    End If
                Next

                ' save
                msg = DomContractBL.SaveManualPaymentProformaInvoice(Me.DbConnection, _
                                currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                batch, invoice, invoiceDetails, invDetToDelete)
                If Not msg.Success Then
                    If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_PROFORMA_INVOICE Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                    Else
                        e.Cancel = True
                        WebUtils.DisplayError(msg)
                    End If
                Else
                    _invoiceID = invoice.ID

                    Dim uri As Target.Library.Web.UriBuilder = New Target.Library.Web.UriBuilder(Request.Url)
                    uri.QueryItems.Remove("id")
                    uri.QueryItems.Add("id", _invoiceID)
                    uri.QueryItems.Remove("mode")
                    uri.QueryItems.Add("mode", StdButtonsMode.Fetched)
                    Response.Redirect(String.Format("EnterManualPayment.aspx{0}", uri.Query))
                    'FindClicked(e)
                End If

            End If

        End Sub

#End Region

#End Region

#Region " SetupInPlaceSelectors "

        Private Sub SetupProviderSelector(ByVal providerID As Integer)
            With CType(Me.provider, InPlaceSelectors.InPlaceEstablishmentSelector)
                .EstablishmentID = providerID
                .Required = True
                .ValidationGroup = "Save"
            End With
        End Sub

        Private Sub SetupContractSelector(ByVal contractID As Integer, ByVal providerID As Integer)
            With CType(Me.domContract, InPlaceSelectors.InPlaceDomContractSelector)
                .ContractID = contractID
                .Required = True
                .ValidationGroup = "Save"
                If providerID > 0 Then
                    .Enabled = True
                Else
                    .Enabled = False
                End If
            End With
        End Sub

#End Region

#Region " PopulateDropdowns "

        Private Sub PopulateDropdowns()

            Dim msg As ErrorMessage
            Dim periods As DomContractPeriodCollection = Nothing
            Dim accounts As vwDomContractPeriodSystemAccountCollection = Nothing
            Dim selectedPeriodID As Integer = 0
            Dim currentWeekEndingDate As Date = Nothing
            Dim periodExisits As Boolean = False
            Dim sysAccountExists As Boolean = False

            cboPeriod.DropDownList.Items.Clear()
            cboSystemAccount.DropDownList.Items.Clear()

            If _contractID > 0 Then

                ' get periods
                currentWeekEndingDate = DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing)

                msg = DomContractPeriod.FetchList(Me.DbConnection, periods, String.Empty, String.Empty, _contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                periods.Sort(New CollectionSorter("DateFrom", SortDirection.Descending))

                With cboPeriod.DropDownList.Items
                    .Clear()
                    For Each p As DomContractPeriod In periods
                        If p.DateFrom <= currentWeekEndingDate Or p.ID = _periodID Then
                            .Add(New ListItem(String.Format("{0} - {1}", p.DateFrom.ToString("dd/MM/yyyy"), p.DateTo.ToString("dd/MM/yyyy")), p.ID))
                            If p.ID = _periodID Then periodExisits = True 'this checks that the period on the query string can be used
                        End If
                    Next
                    .Insert(0, String.Empty)
                End With

                'get systemaccounts
                If _periodID > 0 Then
                    'If _systemAccountID > 0 Then
                    msg = vwDomContractPeriodSystemAccount.FetchList(Me.DbConnection, accounts, _periodID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    accounts.Sort(New CollectionSorter("ClientName", SortDirection.Ascending))

                    With cboSystemAccount.DropDownList.Items
                        .Clear()
                        For Each sa As vwDomContractPeriodSystemAccount In accounts
                            .Add(New ListItem(sa.ClientName, sa.ClientID))
                            If sa.ClientID = _systemAccountID Then sysAccountExists = True
                        Next
                        .Insert(0, String.Empty)
                    End With
                End If

                If periodExisits Then
                    cboPeriod.DropDownList.SelectedValue = _periodID
                End If

                If sysAccountExists Then
                    cboSystemAccount.DropDownList.SelectedValue = _systemAccountID
                End If

            End If
        End Sub

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState()
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            ViewState.Remove(VIEWSTATE_KEY_COUNTER_NEW_LINES)
            phDetails.Controls.Clear()
        End Sub

#End Region

#Region " btnAdd_Click "

        Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click

            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()

            ' add a new row to the detail list
            id = GetUniqueID(New vwDomProformaInvoiceDetail())
            ' create the controls
            OutputDetailControls(id, Nothing)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState(list)
            _btnAddClicked = True

        End Sub

#End Region

#Region " btnRemove_Click "

        Private Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_REMOVE, String.Empty)

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_DETAIL) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_DETAIL, UNIQUEID_PREFIX_DELETE_DETAIL)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phDetails.Controls.Count - 1
                If phDetails.Controls(index).ID = id Then
                    phDetails.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " OutputDetailControls "

        Private Sub OutputDetailControls(ByVal uniqueID As String, ByVal invoiceLine As vwDomProformaInvoiceDetail)

            Dim row As TableRow
            Dim cell As TableCell
            Dim rateCategory As DropDownListEx
            Dim comment As TextBoxEx
            Dim units As TextBoxEx
            Dim unitCost As TextBoxEx
            Dim lineValue As TextBoxEx
            Dim financeCode As InPlaceFinanceCodeSelector
            Dim btnRemove As HtmlInputImage
            Dim hidField As HiddenField

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_DETAIL) Then

                row = New TableRow()
                row.ID = uniqueID
                phDetails.Controls.Add(row)

                ' rate category
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")

                hidField = New HiddenField
                With hidField
                    .ID = CTRL_ID & uniqueID
                    If Not invoiceLine Is Nothing Then .Value = invoiceLine.ID
                End With
                cell.Controls.Add(hidField)

                rateCategory = New DropDownListEx()
                With rateCategory
                    .ID = String.Format("{0}{1}", CTRL_RATE_CATEGORY, uniqueID)
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    LoadRateCategoryDropdown(rateCategory)
                    If Not invoiceLine Is Nothing Then .DropDownList.SelectedValue = invoiceLine.DomRateCategoryID
                    .DropDownList.Attributes.Add("onchange", String.Format("cboRateCategory_OnChange('{0}')", row.ClientID))
                    .DropDownList.Enabled = _newInvoice
                End With
                cell.Controls.Add(rateCategory)

                ' comment
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                comment = New TextBoxEx()
                With comment
                    .ID = String.Format("{0}{1}", CTRL_COMMENT, uniqueID)
                    .MaxLength = 255
                    .Required = True
                    .Width = New Unit(98, UnitType.Percentage)
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    If Not invoiceLine Is Nothing Then .Text = invoiceLine.Comment
                    .TextBox.Enabled = _newInvoice
                End With
                cell.Controls.Add(comment)

                ' units
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                units = New TextBoxEx()
                With units
                    .ID = String.Format("{0}{1}", CTRL_UNITS, uniqueID)
                    .Width = New Unit(5, UnitType.Em)
                    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    If Not invoiceLine Is Nothing Then .Text = invoiceLine.NetUnitsPaid.ToString("F2")
                    _startupJS.AppendFormat("function {0}_Changed(id) {{ txtUnits_Changed(id) }}{1}", .ID, vbCrLf)
                    .TextBox.Enabled = _newInvoice
                End With
                cell.Controls.Add(units)

                ' unit cost
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                unitCost = New TextBoxEx()
                With unitCost
                    .ID = String.Format("{0}{1}", CTRL_UNIT_COST, uniqueID)
                    .Width = New Unit(5, UnitType.Em)
                    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                    .Enabled = False
                    If Not invoiceLine Is Nothing Then .Text = invoiceLine.UnitCost.ToString("F2")
                    _startupJS.AppendFormat("function {0}_Changed(id) {{ txtUnitCost_Changed(id) }}{1}", .ID, vbCrLf)
                End With
                cell.Controls.Add(unitCost)

                ' line value
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                lineValue = New TextBoxEx()
                With lineValue
                    .ID = String.Format("{0}{1}", CTRL_LINE_VALUE, uniqueID)
                    .Width = New Unit(5, UnitType.Em)
                    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                    .Enabled = False
                    If Not invoiceLine Is Nothing Then
                        .Text = invoiceLine.CalculatedPayment.ToString("F2")
                    End If

                End With
                cell.Controls.Add(lineValue)

                ' finance code
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                financeCode = LoadControl("~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx")
                With financeCode
                    .ID = String.Format("{0}{1}", CTRL_FINANCE_CODE, uniqueID)
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    If Not invoiceLine Is Nothing Then .FinanceCodeText = invoiceLine.ManualFinanceCode
                    .Enabled = _newInvoice
                End With
                cell.Controls.Add(financeCode)

                ' remove button
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "right"
                btnRemove = New HtmlInputImage()
                With btnRemove
                    .ID = String.Format("{0}{1}", CTRL_REMOVE, uniqueID)
                    .Src = WebUtils.GetVirtualPath("images/delete.png")
                    .Alt = "Remove this detail line"
                    AddHandler .ServerClick, AddressOf btnRemove_Click
                    .Attributes.Add("onclick", "return btnRemoveDetail_Click();")
                End With
                cell.Controls.Add(btnRemove)


            End If

        End Sub

#End Region

#Region " GetUniqueIDsFromViewState "

        Private Function GetUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_LINES) Is Nothing Then
                _newDetailIDCounter = 0
            Else
                _newDetailIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_LINES), Integer)
            End If

            Return list

        End Function

#End Region

#Region " GetUniqueID "

        Private Function GetUniqueID(ByVal invoiceLine As vwDomProformaInvoiceDetail) As String

            Dim id As String

            If invoiceLine.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_DETAIL & _newDetailIDCounter
                _newDetailIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_DETAIL & invoiceLine.ID
            End If

            Return id

        End Function

#End Region

#Region " PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_NEW_LINES, _newDetailIDCounter)
        End Sub

#End Region

#Region " LoadRateCategoryDropdown "

        Private Sub LoadRateCategoryDropdown(ByVal dropdown As DropDownListEx)

            ' load the dropdown with rate categories that are available to the contract and for manual payments

            Dim msg As ErrorMessage
            Dim rateCategories As DomRateCategoryCollection = Nothing

            msg = DomContractBL.FetchRateCategoriesAvailableForManualPayment(Me.DbConnection, _contractID, rateCategories)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            rateCategories.Sort(New CollectionSorter("SortOrder", SortDirection.Ascending))

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = rateCategories
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region " PopulateAuditDetails "

        Private Sub PopulateAuditDetails(ByVal createdBy As String, ByVal dateCreated As Date)

            If _invoiceID > 0 Then
                With CType(auditDetails, IBasicAuditDetails)
                    .Collapsed = True
                    .EnteredBy = createdBy
                    .DateEntered = dateCreated
                    .AmendedVisible = False
                End With
            Else
                _btnAuditDetails.Visible = False
                auditDetails.Visible = False
            End If

        End Sub

#End Region

#Region " Page_PreRenderComplete "

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            _startupJS.AppendFormat("contractPeriodID='{0}';systemAccountID='{1}';selectedPeriodID={2};selectedSystemAccountID={3};addID='{4}';invoiceTotalID='{5}';invoiceID={6};manualPayment_domContractID='{7}';providerID={8};contractID={9};weekendingID='{10}'; btnAddClicked={11};", _
                                        cboPeriod.ClientID, cboSystemAccount.ClientID, _periodID, _systemAccountID, btnAdd.ClientID, txtInvoiceTotal.ClientID, _invoiceID, domContract.ClientID, _providerID, _contractID, dteWeekending.ClientID, _btnAddClicked.ToString().ToLower())

            _startupJS.AppendFormat("manualPayment_mode={0};", Convert.ToInt32(_stdBut.ButtonsMode))
            _startupJS.AppendFormat("manualPayment_providerID='{0}';", provider.ClientID)
            _startupJS.AppendFormat("lblLineValueTotalID='{0}';", lblLineValueTotal.ClientID)
            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _startupJS.ToString(), True)

            If _stdBut.ButtonsMode = StdButtonsMode.Fetched Then
                WebUtils.RecursiveDisable(fsDetails.Controls, True)
            End If

            _stdBut.AllowEdit = (Me.UserHasMenuItemCommand( _
                                            Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManualPayments.Edit")) And _
                                            _canEdit)
            _stdBut.AllowDelete = (Me.UserHasMenuItemCommand( _
                                                Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManualPayments.Delete")) And _
                                                _canDelete)

        End Sub

#End Region

    End Class

End Namespace
