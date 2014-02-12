Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.PaymentSchedules
Imports Target.Library
Imports Target.Library.Collections
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils



Namespace Apps.Dom.PaymentSchedules

    Partial Public Class PaymentSchedules
        Inherits Target.Web.Apps.BasePage

#Region " Private Variables "

        Private Const _AuditLogTable_PaymentSchedule As String = "PaymentSchedule"
        Private Const _AuditLogTable_DomProformaInvoice As String = "DomProformaInvoice"
        Private Const _AuditLogTable_DomProformaInvoiceBatch As String = "DomProformaInvoiceBatch"
        Private Const _AuditLogTable_DomProformaInvoiceDetail As String = "DomProformaInvoiceDetail"
        Private Const _AuditLogTable_DomProformaInvoiceDetailVisit As String = "DomProformaInvoiceDetailVisit"
        Private Const _AuditLogTable_DomProformaInvoiceVisit As String = "DomProformaInvoiceVisit"
        Private Const _AuditLogTable_DomProformaInvoiceVisit_CareWorker As String = "DomProformaInvoiceVisit_CareWorker"

        Private _stdBut As StdButtonsBase
        Private _mode As Integer
        Private _id As Integer = 0
        Private _providerId As Integer = 0
        Private _contractId As Integer = 0
        Private _genContractId As Integer = 0
        Private _UserHasEditInvoiceCommand As Boolean = False

        Private _ToolTipProvider As String = String.Empty
        Private _ToolTipContract As String = String.Empty
        Private _ToolTipContractID As Integer = 0
        Private _ToolTipPaymentFrom As String = String.Empty
        Private _ToolTipPaymentTo As String = String.Empty
        Private _auditUserName As String
        Private _auditLogTitle As String
        Private _UserHasVisitBasedAddCommand As Boolean
        Private _UserHasNonVisitBasedAddCommand As Boolean


#End Region

#Region " Properties "


        Dim _pScheduleResultStep As String
        Public Property pScheduleResultStep() As String
            Get
                If Not Session("pScheduleResultStep") Is Nothing Then
                    _pScheduleResultStep = Session("pScheduleResultStep")
                End If
                Return _pScheduleResultStep
            End Get
            Set(ByVal value As String)
                Session("pScheduleResultStep") = value
            End Set
        End Property


        Private _MaxNoWeeksRequestedPayments As String
        Public Property MaxNoWeeksRequestedPayments() As String
            Get
                Return _MaxNoWeeksRequestedPayments
            End Get
            Set(ByVal value As String)
                _MaxNoWeeksRequestedPayments = value
            End Set
        End Property


#End Region

#Region " Web Page Events "



        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Payment Schedules")
            _UserHasEditInvoiceCommand = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.EditInvoiceReferences"))
            _UserHasVisitBasedAddCommand = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentSchedules.AddCopyVisits"))
            _UserHasNonVisitBasedAddCommand = SecurityBL.UserHasMenuItem(Me.DbConnection, SecurityBL.GetCurrentUser.ID, Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.RequestPayments"), Me.Settings.CurrentApplicationID)
            'Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.RequestPayments")

            MaxNoWeeksRequestedPayments = Utils.ToNumeric(Me.Settings(ApplicationName.AbacusIntranet, "MaxNoWeeksRequestedPayments"))

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            _auditUserName = user.ExternalUsername
            _auditLogTitle = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)

            Me.JsLinks.Add("PaymentSchedules.js")
            _id = Utils.ToInt32(Request.QueryString("id"))
            _providerId = Utils.ToInt32(Request.QueryString("estabid"))
            _contractId = Utils.ToInt32(Request.QueryString("contractid"))
            _genContractId = Utils.ToInt32(Request.QueryString("gencontractid"))

            If _contractId = 0 And _genContractId <> 0 Then
                Dim genContract As GenericContract = New GenericContract(Me.DbConnection)
                Dim msg As ErrorMessage = Nothing
                msg = genContract.Fetch(_genContractId)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                _contractId = genContract.ChildID

            End If


            ' if  _id > 0 then fill tool tip values 
            If _id > 0 Then
                '' internally it will fill he tool tip values as well
                GetToolTipInformation(_id)
            End If
            ''

            SetbackUrl()
            StandardButtonSettings()
            IntialiseJsVariables()



        End Sub

        Private Sub SetbackUrl()
            Dim backUrl As String = String.Empty
            If Not Request.QueryString("backUrl") Is Nothing Then
                backUrl = Request.QueryString("backUrl")

                If Uri.IsWellFormedUriString(backUrl, UriKind.Absolute) Then

                    Dim url As Uri = New Uri(backUrl)
                    If url.AbsolutePath.ToString().Contains("Apps/Dom/PaymentSchedules/PaymentSchedulesEnquiry.aspx") Or _
                    url.AbsolutePath.ToString().Contains("Apps/Dom/ServiceDeliveryFile/ServiceDeliveryFilePaymentSchedule.aspx") Then
                        pScheduleResultStep = HttpUtility.UrlEncode(backUrl)
                    End If
                Else
                    pScheduleResultStep = HttpUtility.UrlEncode(backUrl)
                End If

            End If

        End Sub


        Private Sub Page_PreInit1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
            '' if there is no back url then get the url of payment scheduel result step and inject in current
            '' url as backUrl
            Dim url As String
            If Request.QueryString("backUrl") Is Nothing Then
                url = Request.Url.ToString()
                url = String.Format("{0}&backUrl={1}", url, pScheduleResultStep)
                Response.Redirect(url)
            End If
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim weekEndingDate As DateTime = DomContractBL.GetWeekEndingDate(DbConnection, Nothing)

            ' setup date from date picker
            With dtePaymentFrom
                .AllowableDays = weekEndingDate.AddDays(1).DayOfWeek
            End With

            ' setup date to date picker
            With dtePaymentTo
                .AllowableDays = weekEndingDate.DayOfWeek
            End With

            txtTotalValue.TextBox.Enabled = False
            ValidateModeRequest()
        End Sub

        Private Sub IntialiseJsVariables()


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

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.PaymentSchedule))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.RequestPayments))

            txtReference.TextBox.Attributes.Add("onchange", "OnOriginalValueChange();")

            ' get Payment schedule reference to display in dialog Box title
            Dim PaymetnScheduleTitle As String = "Reference = "
            PaymetnScheduleTitle += Target.Abacus.Library.InvoiceReferences.InvoiceReferencesBL.GetPaymentScheduleHeader(Me.DbConnection, _id)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.PaymentSchedules.Startup", _
              Target.Library.Web.Utils.WrapClientScript(String.Format("InPlaceContractSelectorID=""{0}""; " & _
                                                                      "InPlaceProviderSelectorID=""{1}""; " & _
                                                                      "txtProviderId=""{2}"";" & _
                                                                      "txtContractId=""{3}"";" & _
                                                                      "txtReferenceId=""{4}"";" & _
                                                                      "hidPaymentScheduleId=""{5}"";" & _
                                                                      "chkUnpaidId=""{6}"";" & _
                                                                      "chkSuspendedId=""{7}"";" & _
                                                                      "chkAuthorisedId=""{8}"";" & _
                                                                      "chkPaidId=""{9}"";" & _
                                                                      "chkAmendUnVerifiedId=""{10}"";" & _
                                                                      "chkAmendVerifiedId=""{11}"";" & _
                                                                      "chkAwaitingId=""{12}""; " & _
                                                                      "chkVerifiedId=""{13}""; " & _
                                                                      "OriginalValueChangedId=""{14}"";" & _
                                                                      "RequestModeId=""{15}"";" & _
                                                                      "dtePaymentToId=""{16}"";" & _
                                                                      "PaymetnScheduleTitle=""{17}"";" & _
                                                                      "ToolTipProvider=""{18}"";" & _
                                                                      "ToolTipContract=""{19}"";" & _
                                                                      "ToolTipPaymentFrom=""{20}"";" & _
                                                                      "ToolTipPaymentTo=""{21}"";" & _
                                                                      "PermitEditReference={22};" & _
                                                                      "ToolTipContractID=""{23}"";" & _
                                                                      "dtePaymentFromId=""{24}"";" & _
                                                                      "SelectedProviderId=""{25}"";" & _
                                                                      "chkVisitBasedId=""{26}"";" & _
                                                                      "UserHasVisitBasedAddCommand=""{27}"";" & _
                                                                      "UserHasNonVisitBasedAddCommand=""{28}"";" & _
                                                                      "MaxNoWeeksAllowed={29};", _
                                                                      ContractSelector.ClientID, _
                                                                      ProviderSelector.ClientID, _
                                                                      txtProviderId.ClientID, _
                                                                      txtContractId.ClientID, _
                                                                      txtReference.ClientID, _
                                                                      hidPaymentScheduleId.ClientID, _
                                                                      chkProviderInvoiceUnpaid.ClientID, _
                                                                      chkProviderInvoiceSuspended.ClientID, _
                                                                      chkProviderInvoiceAuthorised.ClientID, _
                                                                      chkProviderInvoicePaid.ClientID, _
                                                                      chkVisitAmendmentAwaitingVerification.ClientID, _
                                                                      chkVisitAmendmentVerified.ClientID, _
                                                                      chkProformaInvoiceAwaiting.ClientID, _
                                                                      chkProformaInvoiceVerified.ClientID, _
                                                                      OriginalValueChanged.ClientID, _
                                                                      RequestMode.ClientID, _
                                                                      dtePaymentTo.ClientID, _
                                                                      PaymetnScheduleTitle, _
                                                                      _ToolTipProvider, _
                                                                      _ToolTipContract, _
                                                                      _ToolTipPaymentFrom, _
                                                                      _ToolTipPaymentTo, _
                                                                      _UserHasEditInvoiceCommand.ToString.ToLower(), _
                                                                      _ToolTipContractID, _
                                                                      dtePaymentFrom.ClientID, _
                                                                      _providerId, _
                                                                      chkVisitBased.ClientID, _
                                                                    _UserHasVisitBasedAddCommand.ToString().ToLower(), _
                                                                    _UserHasNonVisitBasedAddCommand.ToString().ToLower(), _
                                                                      MaxNoWeeksRequestedPayments _
                                                         )) _
                                                    )

            'InPlaceProviderSelector.ClientID
        End Sub

        Private Sub Page_PreRenderComplete1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete


            If _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                If Utils.ToInt32(lblProformaInvoiceAwaitingCount.Text) > 0 _
                Or Utils.ToInt32(lblProformaInvoiceVerifiedCount.Text) > 0 _
                Or Utils.ToInt32(lblProviderInvoiceUnpaidCount.Text) > 0 _
                Or Utils.ToInt32(lblProviderInvoicesuspendedCount.Text) > 0 _
                Or Utils.ToInt32(lblProviderInvoiceAuthorisedCount.Text) > 0 _
                Or Utils.ToInt32(lblProviderInvoicePaidCount.Text) > 0 _
                Then
                    CType(ProviderSelector, InPlaceSelectors.InPlaceEstablishmentSelector).EstablishmentSelector_Enabled = False
                    CType(ContractSelector, InPlaceSelectors.InPlaceContractSelector).ContractSelector_Enabled = False

                    dtePaymentFrom.Enabled = False
                    dtePaymentTo.Enabled = False
                    txtTotalValue.Enabled = False
                End If
            ElseIf _stdBut.ButtonsMode = StdButtonsMode.AddNew Or _
                   _stdBut.ButtonsMode = StdButtonsMode.Fetched Then
                CType(ContractSelector, InPlaceSelectors.InPlaceContractSelector).ContractSelector_Enabled = False
            End If

            'If _stdBut.ButtonsMode = StdButtonsMode.AddNew Or _
            ' _stdBut.ButtonsMode = StdButtonsMode.Fetched Then
            '    CType(ContractSelector, InPlaceSelectors.InPlaceContractSelector).ContractSelector_Enabled = False

            'Else
            '    CType(ContractSelector, InPlaceSelectors.InPlaceContractSelector).ContractSelector_Enabled = True
            '    '    EnableDisableContractSelector.Value = "false"
            'End If

            If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                If _providerId > 0 Then
                    CType(ProviderSelector, InPlaceSelectors.InPlaceEstablishmentSelector).EstablishmentID = _providerId
                    txtProviderId.Value = _providerId
                End If
                If _providerId > 0 AndAlso _contractId > 0 Then
                    CType(ContractSelector, InPlaceSelectors.InPlaceContractSelector).ContractID = _contractId
                    txtContractId.Value = _contractId
                End If


            End If

            txtTotalValue.Enabled = False

            HandleReferencesVisibility()


        End Sub

       

#End Region

#Region " Edit References Visibility "
        Private Sub HandleReferencesVisibility()
            '' login of edit references
            If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                ' add new the button will not be visible
                pnlReferences.Visible = False
            ElseIf _stdBut.ButtonsMode = StdButtonsMode.Fetched Then
                ' button will be anabled in fetch mode
                pnlReferences.Visible = True
                btnEditInvoiceReferences.Disabled = False
            ElseIf _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                ' in edit mode button will be visible but disabled 
                pnlReferences.Visible = True
                btnEditInvoiceReferences.Disabled = True
            End If
        End Sub
#End Region

#Region " Validate Page mode "
        Private Sub ValidateModeRequest()
            '_mode = Utils.ToInt32(Request.QueryString("mode"))
            If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                pnlUnProcessedInvoices.Visible = False
                pnlProviderInvoices.Visible = False
                pnlUnProcessedVisitamendmentRequest.Visible = False
            Else
                pnlUnProcessedInvoices.Visible = True
                pnlProviderInvoices.Visible = True
                pnlUnProcessedVisitamendmentRequest.Visible = True
            End If

            RequestMode.Value = _stdBut.ButtonsMode
        End Sub
#End Region


#Region " standard button "

        Private Sub StandardButtonSettings()
            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .UseApplicationIdFilter = False
                .AllowBack = True
                .AllowNew = True
                .AllowEdit = True ' IsInvoiceBatchManuallyEntered(_originalInvoiceID)
                .AllowDelete = SetDeleteButtonVisibility(_id)
                .AllowFind = False
                .ShowNew = False
                .AuditLogTableNames.Add(_AuditLogTable_PaymentSchedule)
                .AuditLogTableNames.Add(_AuditLogTable_DomProformaInvoice)
                .AuditLogTableNames.Add(_AuditLogTable_DomProformaInvoiceBatch)
                .AuditLogTableNames.Add(_AuditLogTable_DomProformaInvoiceDetail)
                .AuditLogTableNames.Add(_AuditLogTable_DomProformaInvoiceDetailVisit)
                .AuditLogTableNames.Add(_AuditLogTable_DomProformaInvoiceVisit)
                .AuditLogTableNames.Add(_AuditLogTable_DomProformaInvoiceVisit_CareWorker)
                .EditableControls.Add(pnlPaymentSchedules.Controls)
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf EditClicked
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .CancelClicked, AddressOf CancelClick
                AddHandler .DeleteClicked, AddressOf DeleClick
            End With
        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            '''''''' start saving paymnet schedule 
            Dim pSchedules As DataClasses.PaymentSchedule = _
            New DataClasses.PaymentSchedule(Me.DbConnection, _
                                            _auditUserName, _
                                            _auditLogTitle)
            Dim msg As New Target.Library.ErrorMessage

            If e.ItemID > 0 Then
                msg = pSchedules.Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If
            pSchedules.ProviderID = txtProviderId.Value
            pSchedules.DomContractID = txtContractId.Value
            pSchedules.Reference = txtReference.TextBox.Text
            pSchedules.DateFrom = dtePaymentFrom.Value
            pSchedules.DateTo = dtePaymentTo.Value
            pSchedules.DateCreated = Date.Now
            Dim visitbased As TriState
            If hidVisitBased.Value.ToLower() = "true" Then
                visitbased = TriState.True
            ElseIf hidVisitBased.Value.ToLower() = "false" Then
                visitbased = TriState.False
            Else
                visitbased = TriState.UseDefault
            End If
            pSchedules.VisitBased = visitbased


            Dim validationMsg As New System.Text.StringBuilder
            Dim isPageValid As Boolean = True
            isPageValid = DomContractBL.ValidatePaymentScheduleSave(Me.DbConnection, _
                                                  Nothing, _
                                                  pSchedule:=pSchedules, _
                                                  msg:=validationMsg, _
                                                 ValidateForUpdate:=IIf(e.ItemID > 0, True, False), _
                                                 auditUserName:=_auditUserName, _
                                                 auditLogTitle:=_auditLogTitle)
            If Not isPageValid Then
                lblError.Text = validationMsg.ToString()
                OriginalValueChanged.Value = "false"
                e.Cancel = True
                Return
            End If
            ' saving payment schedule
            msg = pSchedules.Save()
            If Not msg.Success Then
                lblError.Text = msg.Message
                OriginalValueChanged.Value = "false"
                e.Cancel = True
                Return
            End If
            '' reload from database
            OriginalValueChanged.Value = "false"
            e.ItemID = pSchedules.ID

            'FindClicked(e)
            Dim backUrl As String = String.Empty
            If Not Request.QueryString("backUrl") Is Nothing Then
                backUrl = HttpUtility.UrlEncode(Request.QueryString("backUrl"))
            End If
            Response.Redirect(String.Format("PaymentSchedules.aspx?mode=1&id={0}&backUrl={1}", e.ItemID, backUrl))

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            PopulatePaymentSchedules(e.ItemID)
              End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            PopulatePaymentSchedules(e.ItemID)
        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            Dim totalValue As Decimal = 0.0
            txtTotalValue.Text = totalValue.ToString("C")
            ShowHideVoidPaymentDetails()
        End Sub

        Private Sub CancelClick(ByRef e As StdButtonEventArgs)
            If e.ItemID > 0 Then
                PopulatePaymentSchedules(e.ItemID)
            Else
                Response.Redirect(HttpUtility.UrlDecode(Request.QueryString("backUrl")))
            End If
        End Sub

        Private Sub DeleClick(ByRef e As StdButtonEventArgs)

            Dim pScheduleId As Integer = e.ItemID

            Dim validToDelete As Boolean = True
            Dim rtnMessage As String = String.Empty
            Dim msg As ErrorMessage = New ErrorMessage

            msg = DomContractBL.ValidatePaymentScheduleDelete(Me.DbConnection, Nothing, pScheduleId, False)
            If Not msg.Success Then
                lblError.Text = msg.Message
                Return
            End If

            msg = DomContractBL.DeletePaymentSchedule(Me.DbConnection, Nothing, pScheduleId, False)
            If Not msg.Success Then
                lblError.Text = msg.Message
                Return
            End If

            Dim url As String = Utils.ToString(Request.QueryString("backUrl"))
            url = HttpUtility.HtmlDecode(url)
            Response.Redirect(url)
        End Sub

#End Region

#Region " Populate Payment Schedules "
        Private Sub PopulatePaymentSchedules(ByVal pScheduleId As Integer)

            Dim pSchedules As DataClasses.PaymentSchedule = _
            New DataClasses.PaymentSchedule(Me.DbConnection, String.Empty, String.Empty)
            pSchedules.Fetch(pScheduleId)
            txtProviderId.Value = pSchedules.ProviderID

            CType(ProviderSelector, InPlaceSelectors.InPlaceEstablishmentSelector).EstablishmentID = _
            pSchedules.ProviderID

            txtContractId.Value = pSchedules.DomContractID
            CType(ContractSelector, InPlaceSelectors.InPlaceContractSelector).SelectedContractID = _
            pSchedules.DomContractID


            txtReference.Text = pSchedules.Reference
            dtePaymentFrom.Text = pSchedules.DateFrom
            dtePaymentTo.Text = pSchedules.DateTo
            If pSchedules.VisitBased = TriState.True Then
                chkVisitBased.Checked = True
                hidVisitBased.Value = "true"
            Else
                chkVisitBased.Checked = False
                hidVisitBased.Value = "false"
            End If

            hidPaymentScheduleId.Value = pSchedules.ID

            txtTotalValue.Text = (pSchedules.NetValue_Authorised + pSchedules.NetValue_AwaitingVerification + _
            pSchedules.NetValue_Paid + pSchedules.NetValue_Suspended + _
            pSchedules.NetValue_Unpaid + pSchedules.NetValue_Verified).ToString("c")


            lblProformaInvoiceAwaitingCount.Text = pSchedules.Count_AwaitingVerification
            lblProformaInvoiceAwaitingAmount.Text = pSchedules.NetValue_AwaitingVerification.ToString("0.00")
            lblProformaInvoiceVerifiedCount.Text = pSchedules.Count_Verified
            lblProformaInvoiceVerifiedAmount.Text = pSchedules.NetValue_Verified.ToString("0.00")
            '' provider invoices
            lblProviderInvoiceUnpaidCount.Text = pSchedules.Count_Unpaid
            lblProviderInvoiceUnpaidAmount.Text = pSchedules.NetValue_Unpaid.ToString("0.00")
            lblProviderInvoicesuspendedCount.Text = pSchedules.Count_Suspended
            lblProviderInvoiceSuspendedAmount.Text = pSchedules.NetValue_Suspended.ToString("0.00")
            lblProviderInvoiceAuthorisedCount.Text = pSchedules.Count_Authorised
            lblProviderInvoiceAuthorisedAmount.Text = pSchedules.NetValue_Authorised.ToString("0.00")
            lblProviderInvoicePaidCount.Text = pSchedules.Count_Paid
            lblProviderInvoicePaidAmount.Text = pSchedules.NetValue_Paid.ToString("0.00")
            '' Unprocessed visit amendment Requests
            lblVisitAmendmentAwaitingVerificationCount.Text = pSchedules.Count_UnverifiedVisitAmendmentRequests
            lblVisitAmendmentVerifiedCount.Text = pSchedules.Count_VerifiedVisitAmendmentRequests

            ShowHideVoidPaymentDetails(pSchedules)
        End Sub
#End Region

        Private Sub ShowHideVoidPaymentDetails(Optional pschedule As PaymentSchedule = Nothing)
            '' if no contract then just hide it
            If pschedule Is Nothing Then
                pnlVoidContract.Visible = False
                Return
            End If

            'If Weak have contract then process as below 

            Dim msg As New ErrorMessage()
            Dim contract As DomContract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = contract.Fetch(pschedule.DomContractID)
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
            End If

            If contract.ContractType <> [Enum].GetName(GetType(DomContractType), DomContractType.BlockGuarantee) Then
                pnlVoidContract.Visible = False
            Else
                ' populate information of void Payment
                Dim voidDetail As New VoidPaymentdetail()
                msg = VoidPaymentsBL.GetVoidPaymentDetails(Me.DbConnection, pschedule.ID, voidDetail)
                lblVoidPaymentMade.Text = voidDetail.VoidPaymentMade.ToString("C")
                lnkVoidPaymentDue.Text = voidDetail.VoidPaymentDue.ToString("C")
                lblAgreedAmount.Text = voidDetail.AgreedVoidPayment.ToString("C")
            End If

        End Sub




#Region " GettoolTipInformation "

        Private Sub GetToolTipInformation(ByVal id As Integer)
            If id = 0 Then
                Return
            End If

            Dim msg As ErrorMessage

            Dim pSchedules As DataClasses.PaymentSchedule = _
           New DataClasses.PaymentSchedule(Me.DbConnection, String.Empty, String.Empty)
            msg = New ErrorMessage
            msg = pSchedules.Fetch(id)

            If Not msg.Success Then WebUtils.DisplayError(msg)
            '' populate Tool tip values
            _ToolTipPaymentFrom = pSchedules.DateFrom
            _ToolTipPaymentTo = pSchedules.DateTo

            Dim contract As DataClasses.DomContract = _
            New DataClasses.DomContract(Me.DbConnection, "", "")

            msg = New ErrorMessage
            msg = contract.Fetch(pSchedules.DomContractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _ToolTipContract = String.Format("{0}: {1}", contract.Number, contract.Title)

            Dim estab As DataClasses.Establishment = _
            New DataClasses.Establishment(Me.DbConnection)

            msg = New ErrorMessage
            msg = estab.Fetch(pSchedules.ProviderID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _ToolTipProvider = String.Format("{0}: {1}", estab.AltReference, estab.Name)
            _ToolTipContractID = contract.ID

        End Sub
#End Region


#Region "Set Delete button SetDeleteButtonVisibility"
        Private Function SetDeleteButtonVisibility(ByVal pScheduleId As Integer) As Boolean
            Dim UserHasDeleteCommand As Boolean = False
            ' first of all check if current user have the access to delete payment schedule
            UserHasDeleteCommand = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.Delete"))
            ' if user has access rights then check either this paymetn schedule can be deleted
            If UserHasDeleteCommand = False Then
                Return UserHasDeleteCommand
            Else
                Dim pSchedules As DataClasses.PaymentSchedule = _
            New DataClasses.PaymentSchedule(Me.DbConnection, _auditUserName, _auditLogtitle)
                pSchedules.Fetch(pScheduleId)

                If pSchedules.Count_Unpaid > 0 Or pSchedules.Count_Suspended Or pSchedules.Count_Authorised > 0 Or pSchedules.Count_Paid > 0 Then
                    Return False
                Else
                    Return True
                End If
            End If
        End Function

#End Region

#Region " PageIsValid"



#End Region

        Private Function PaymentScheduleDefaultValue() As Boolean
            Dim msg As ErrorMessage
            Dim settings As New ApplicationSettingCollection()
            Dim settingKey As String = "PaymentScheduleDefaultStatusVisitbased"
            msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, _
                                                       applicationID:=2, _
                                                       auditLogTitle:=String.Empty, _
                                                       auditUserName:=String.Empty, _
                                                       list:=settings, _
                                                       settingKey:=settingKey)

            If Not msg.Success Then
                lblError.Text = msg.Message
                Return False
            Else
                Return settings(0).SettingValue
            End If
        End Function
    End Class

End Namespace