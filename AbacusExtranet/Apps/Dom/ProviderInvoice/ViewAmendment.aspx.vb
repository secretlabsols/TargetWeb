
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Screen to allow the creation and verification/declination of visit amendment requests.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD 14/05/2010  D11739 - added actual duration
    ''' MikeVO 22/02/2010  A4WA#6106 - cannot create new amendment request if the invoice the visit belongs to 
    '''                    has a pending retraction.
    ''' MikeVO 20/10/2009  D11546 - ensure edit/delete is only possible by same type of user as the originator.
    '''                    Added service type.
    ''' MikeVO 01/12/2008  D11492 - tweak to std buttons.
    ''' MikeVO 01/12/2008  D11444 - security overhaul.
    ''' MikeVO 07/10/2008  Fix to filter the list of visit codes by visit code group.
    ''' PaulW  ??/??/????  Created.
    ''' </history>
    Partial Public Class ViewAmendment
        Inherits Target.Web.Apps.BasePage

        Const SP_NAME_FETCH_AMENDMENT As String = "spxDomProviderInvoiceVisitAmendment_FetchForView"
        Const ALREADY_AMENDED_MESSAGE As String = "Visit has already been amended."

        Private _stdBut As StdButtonsBase
        Private _councilUser As Boolean
        Private _amendmentRequestExists As Boolean
        Private _amendmentOriginator As String = String.Empty
        Private _invoiced As Boolean
        Private _amendmentID As Integer
        Private _amendmentVerified As Boolean
        Private _weekending As Date
        Private _retractionPending As Boolean
        Private _visitCategory As String = String.Empty
        Private _notOnProformaInvoice As Boolean
        Private _currentStatus As String = String.Empty
        Private _canEdit As Boolean
        Private _pScheduleId As Integer

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.VisitAmendmentRequest"), "View/Amend Visit Amendment Request")

            GetPaymentScheduleId()

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            'Determine if the current user is a council or Provider User 
            _councilUser = SecurityBL.IsCouncilUser(Me.DbConnection, Me.Settings, user.ExternalUserID)

            With _stdBut
                .AllowFind = False
                .AllowBack = False
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.VisitAmendmentRequest.Delete"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.VisitAmendmentRequest.Edit"))
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.VisitAmendmentRequest.AddNew"))
                .ShowNew = False
                .EditableControls.Add(pnlRequest.Controls)
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.CancelClicked, AddressOf FindClicked

            If Not Me.IsPostBack Then
                Me.CustomNavAdd(False)
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim style As New StringBuilder
            Dim js As String

            style.Append("label.label { float:left; width:12em; font-weight:bold; }")
            style.Append("span.label { float:left; width:12em; padding-right:1em; font-weight:bold; }")
            style.Append(".Amendment {padding-left:2em; color:red; font-style:italic; )")
            Me.AddExtraCssStyle(style.ToString)

            js = String.Format("var providerFeedbackID; providerFeedbackID=""{0}"";", txtFeedback.ClientID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

            CType(pSchedule, Apps.UserControls.PaymentScheduleHeader).SingleLiner = True
            CType(pSchedule, Apps.UserControls.PaymentScheduleHeader).LabelWidth = "11.5em"

            If Not Me.IsPostBack Then
                Me.CustomNavAdd(False)
            End If
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim visitID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Dim domContractWeekOpen As Boolean

            PopulateDropdowns(visitID)

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_AMENDMENT, False)
                spParams(0).Value = visitID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_AMENDMENT, spParams)

                reader.Read()

                If IsDBNull(reader("ID")) Then
                    hidAmendmentID.Value = 0
                Else
                    hidAmendmentID.Value = Utils.ToInt32(reader("ID"))
                End If

                hidContractID.Value = Utils.ToInt32(reader("DomContractID"))
                hidVisitDate.Value = reader("VisitDate")
                lblProvider.Text = String.Format("{0}/{1}", reader("ProviderReference"), reader("ProviderName"))
                lblContract.Text = String.Format("{0}/{1}", reader("ContractNumber"), reader("ContractTitle"))
                If Not IsDBNull(reader("ClientName")) Then
                    lblServiceUser.Text = String.Format("{0}/{1}", reader("ClientReference"), reader("ClientName"))
                End If
                If Not IsDBNull(reader("VisitDateOriginal")) Then lblVisitDate.Text = Convert.ToDateTime(reader("VisitDateOriginal")).ToString("dd MMM yyyy")

                If Not IsDBNull(reader("RequestDate")) Then lblRequestDate.Text = Convert.ToDateTime(reader("RequestDate")).ToString("dd MMM yyyy")
                If Not IsDBNull(reader("RequestBy")) Then lblRequestBy.Text = reader("RequestBy")
                If Not IsDBNull(reader("Originator")) Then
                    _amendmentOriginator = reader("Originator")
                    lblOriginator.Text = _amendmentOriginator
                End If

                'Original Values
                If Not IsDBNull(reader("StartTimeClaimedOriginal")) Then lblStartTimeClaimedOriginal.Text = Convert.ToDateTime(reader("StartTimeClaimedOriginal")).ToString("HH:mm")
                If Not IsDBNull(reader("DurationClaimedOriginal")) Then lblDurationClaimedOriginal.Text = Convert.ToDateTime(reader("DurationClaimedOriginal")).ToString("H\h m\m")
                If Not IsDBNull(reader("PreRoundedDurationClaimed")) Then lblPreRoundedDurationClaimed.Text = "[" & Convert.ToDateTime(reader("PreRoundedDurationClaimed")).ToString("H\h m\m") & "]"
                If Not IsDBNull(reader("ActualDurationOriginal")) Then lblActualDurationOriginal.Text = Convert.ToDateTime(reader("ActualDurationOriginal")).ToString("H\h m\m")
                If Not IsDBNull(reader("VisitCodeOriginal")) Then lblVisitCodeOriginal.Text = reader("VisitCodeOriginal")
                If Not IsDBNull(reader("DomServiceTypeOriginal")) Then lblServiceTypeOriginal.Text = reader("DomServiceTypeOriginal")
                If Not IsDBNull(reader("SecondaryVisitOriginal")) Then lblActualSecondaryVisit.Text = IIf(Convert.ToBoolean(reader("SecondaryVisitOriginal")), "Yes", "No")

                'Amended values
                If Not IsDBNull(reader("StartTimeClaimed")) Then
                    txtStartTimeClaimedAmendment.Hours = Convert.ToDateTime(reader("StartTimeClaimed")).ToString("HH")
                    txtStartTimeClaimedAmendment.Minutes = Convert.ToDateTime(reader("StartTimeClaimed")).ToString("mm")
                Else
                    'an amendment does not currently exist so initially set the values to that of the original values
                    txtStartTimeClaimedAmendment.Hours = Convert.ToDateTime(reader("StartTimeClaimedOriginal")).ToString("HH")
                    txtStartTimeClaimedAmendment.Minutes = Convert.ToDateTime(reader("StartTimeClaimedOriginal")).ToString("mm")
                End If

                If Not IsDBNull(reader("DurationClaimed")) Then
                    txtDurationClaimedAmendment.Hours = Convert.ToDateTime(reader("DurationClaimed")).ToString("HH")
                    txtDurationClaimedAmendment.Minutes = Convert.ToDateTime(reader("DurationClaimed")).ToString("mm")
                Else
                    'an amendment does not currently exist so initially set the values to that of the original values
                    txtDurationClaimedAmendment.Hours = Convert.ToDateTime(reader("DurationClaimedOriginal")).ToString("HH")
                    txtDurationClaimedAmendment.Minutes = Convert.ToDateTime(reader("DurationClaimedOriginal")).ToString("mm")
                End If

                If Not IsDBNull(reader("ActualDuration")) Then
                    txtActualDurationAmendment.Hours = Convert.ToDateTime(reader("ActualDuration")).ToString("HH")
                    txtActualDurationAmendment.Minutes = Convert.ToDateTime(reader("ActualDuration")).ToString("mm")
                Else
                    'an amendment does not currently exist so initially set the values to that of the original values
                    txtActualDurationAmendment.Hours = Convert.ToDateTime(reader("ActualDurationOriginal")).ToString("HH")
                    txtActualDurationAmendment.Minutes = Convert.ToDateTime(reader("ActualDurationOriginal")).ToString("mm")
                End If

                If Not IsDBNull(reader("SecondaryVisit")) Then
                    cboSecondaryVisit.DropDownList.SelectedValue = Convert.ToBoolean(reader("SecondaryVisit"))
                Else
                    cboSecondaryVisit.DropDownList.SelectedValue = Convert.ToBoolean(reader("SecondaryVisitOriginal"))
                End If

                If Not IsDBNull(reader("VisitCode")) Then
                    cboVisitCodeAmendment.DropDownList.SelectedValue = Convert.ToInt32(reader("DomVisitCodeID"))
                Else
                    cboVisitCodeAmendment.DropDownList.SelectedValue = Convert.ToInt32(reader("DomVisitCodeIDOriginal"))
                End If

                If Not IsDBNull(reader("ServiceType")) Then
                    cboServiceTypeAmendment.DropDownList.SelectedValue = Convert.ToInt32(reader("DomServiceTypeID"))
                Else
                    cboServiceTypeAmendment.DropDownList.SelectedValue = Convert.ToInt32(reader("DomServiceTypeIDOriginal"))
                End If

                If Not IsDBNull(reader("AmendmentReason")) Then txtReason.Text = reader("AmendmentReason")
                If Not IsDBNull(reader("Status")) Then
                    lblStatus.Text = Utils.SplitOnCapitals([Enum].GetName(GetType(DomProviderInvoiceVisitAmendmentStatus), Convert.ToInt32(reader("Status"))))
                    If reader("Status") = DomProviderInvoiceVisitAmendmentStatus.Declined Then
                        optDecline.Checked = True
                    ElseIf reader("Status") = DomProviderInvoiceVisitAmendmentStatus.Verified Then
                        optVerify.Checked = True
                    End If
                End If

                If Not IsDBNull(reader("ProviderFeedback")) Then txtFeedback.Text = reader("ProviderFeedback")
                If Not IsDBNull(reader("VerificationDate")) Then lblVerificationDate.Text = reader("VerificationDate")
                If Not IsDBNull(reader("VerificationBy")) Then lblVerifiedBy.Text = reader("VerificationBy")
                If Not IsDBNull(reader("invoicenumber")) Then lblInvoiceNo.Text = reader("invoicenumber")

                If Not IsDBNull(reader("InvoiceNumber")) Then
                    'Visit is linked to an invoice
                    _invoiced = True
                Else
                    'Not linked to an invoice so hide the invoice number panel
                    _invoiced = False
                End If

                If IsDBNull(reader("RequestDate")) Then
                    'No amendment request currently exists for this visit
                    _amendmentRequestExists = False
                Else 'Amendment request exists
                    _amendmentRequestExists = True
                    '_stdBut.InitialMode = StdButtonsMode.Fetched
                    _currentStatus = Utils.SplitOnCapitals([Enum].GetName(GetType(DomProviderInvoiceVisitAmendmentStatus), Convert.ToInt32(reader("Status"))))
                    _amendmentVerified = Not IsDBNull(reader("VerificationDate"))
                End If

                _weekending = reader("Weekending")
                _visitCategory = reader("VisitCategory").ToString.ToUpper
                _retractionPending = Convert.ToBoolean(reader("RetractionPending"))
                _notOnProformaInvoice = IsDBNull(reader("ProformaInvoiceID"))

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_AMENDMENT, "ViewAmendment.FindClicked")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            'Populate Carers Table
            PopulateCarersTable(visitID)

            'Check see if the contrat is open for this weekending date
            msg = DomContractBL.IsContractWeekOpen(Me.DbConnection, Utils.ToInt32(hidContractID.Value), _
                                                                    _weekending, domContractWeekOpen)
            If _visitCategory = "STANDARD" And _
                    _retractionPending = False And _
                    _notOnProformaInvoice And _
                    Not _amendmentVerified And _
                    domContractWeekOpen Then
                If _councilUser Then
                    If _amendmentOriginator.ToString.ToUpper = "COUNCIL" Then
                        'The council Created the amendment so they can amend and delete the amendment
                        _canEdit = True
                    End If
                Else
                    'Must be a provider
                    If _amendmentOriginator.ToString.ToUpper = "PROVIDER" Then
                        'The provider Created the amendment so they can amend and delete the amendment
                        _canEdit = True
                    End If
                End If
            End If

            'Enable items for edit?
            With _stdBut
                .AllowEdit = _canEdit
                .AllowDelete = _canEdit
            End With

            ' if a retraction is pending...
            IsRetractionPending()

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim visitID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim originator As String
            Dim msg As ErrorMessage
            Dim trans As SqlTransaction = Nothing
            Dim indicator As DataClasses.DomManuallyAmendedIndicator = Nothing

            PopulateDropdowns(visitID)
            cboVisitCodeAmendment.SelectPostBackValue()
            cboServiceTypeAmendment.SelectPostBackValue()

            '' validate if record has not already been changed
            If (ChangeRequestExists(visitID)) Then
                'Re-find/populate the screen
                FindClicked(e)
                lblError.Text = ALREADY_AMENDED_MESSAGE
                Return
            End If

            Me.Validate("Save")

            If Me.IsValid Then

                If _councilUser Then
                    originator = "Council"
                Else
                    originator = "Provider"
                End If
                'Get the manually amended indicator
                msg = DomContractBL.GetManuallyAmendedIndicatorForManualVisit(Me.DbConnection, hidContractID.Value, hidVisitDate.Value, indicator)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                'Start transaction and save details
                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)
                    msg = DomContractBL.SaveDomProviderInvoiceVisitAmendment( _
                            trans, _
                            Utils.ToInt32(hidAmendmentID.Value), _
                            Target.Library.Utils.ToInt32(Request.QueryString("id")), _
                            originator, _
                            DateTime.Parse(txtStartTimeClaimedAmendment.ToString(DomContractBL.TIME_ONLY_DATE)), _
                            DateTime.Parse(txtDurationClaimedAmendment.ToString(DomContractBL.TIME_ONLY_DATE)), _
                            DateTime.Parse(txtActualDurationAmendment.ToString(DomContractBL.TIME_ONLY_DATE)), _
                            Utils.ToInt32(cboVisitCodeAmendment.GetPostBackValue), _
                            Utils.ToInt32(cboServiceTypeAmendment.GetPostBackValue), _
                            txtReason.Text, _
                            indicator.ID, _
                            user.ID, _
                            Convert.ToBoolean(cboSecondaryVisit.GetPostBackValue) _
                    )
                    If Not msg.Success Then
                        'Rollback transaction and display error
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                        e.Cancel = True
                    End If
                    '' recalculate payment schedule
                    msg = New ErrorMessage
                    msg = DomContractBL.PaymentscheduleRecalculateCountsAttributesAndNetValues(trans, _pScheduleId)
                    If Not msg.Success Then
                        'Rollback transaction and display error
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                        e.Cancel = True
                    End If
                    'Commit Database transaction
                    trans.Commit()
                    'Re-find/populate the screen
                    FindClicked(e)

                Catch ex As Exception
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))
                End Try
            Else
                e.Cancel = True
                NewClicked(e)
            End If
        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            'Re-find/populate the screen
            FindClicked(e)
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim trans As SqlTransaction = Nothing
            Dim msg As ErrorMessage

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)
                msg = DataClasses.DomProviderInvoiceVisitAmendment.Delete(trans, Utils.ToInt32(hidAmendmentID.Value))
                If Not msg.Success Then
                    trans.Rollback()
                    WebUtils.DisplayError(msg)
                    e.Cancel = True
                End If

                '' recalculate payment schedule
                msg = New ErrorMessage
                msg = DomContractBL.PaymentscheduleRecalculateCountsAttributesAndNetValues(trans, _pScheduleId)
                If Not msg.Success Then
                    'Rollback transaction and display error
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                    e.Cancel = True
                End If

                'Commit the transaction
                trans.Commit()

                Me.CustomNavRemoveLast()
                Me.CustomNavGoBack()

                'Response.Redirect(HttpUtility.UrlDecode(Request.QueryString("backUrl")))
            Catch ex As Exception
                SqlHelper.RollbackTransaction(trans)
                WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))
            End Try
        End Sub

        Private Sub btnVerification_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVerification.ServerClick
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim trans As SqlTransaction = Nothing
            Dim verify As Boolean
            Dim postBackVerify As String

            If Me.IsValid Then

                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    postBackVerify = Request.Form("ctl00$MPContent$Verification")
                    If postBackVerify = optDecline.ID Then
                        verify = False
                    ElseIf postBackVerify = optVerify.ID Then
                        verify = True
                    End If
                    'verify/decline the amendment
                    msg = DomContractBL.VerifyVisitAmendment(trans, _
                                                             Utils.ToInt32(hidAmendmentID.Value), _
                                                             verify, _
                                                             txtFeedback.Text, _
                                                             user.ExternalUsername)
                    If Not msg.Success Then
                        'Rollback transaction and display error
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If
                    '' after verify the amendment . Recalculate PS
                    msg = New ErrorMessage
                    msg = DomContractBL.PaymentscheduleRecalculateCountsAttributesAndNetValues(trans, _pScheduleId)
                    If Not msg.Success Then
                        'Rollback transaction and display error
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If

                    'Commit Database transaction
                    trans.Commit()
                    'Re-find/populate the screen
                    Dim f As StdButtonEventArgs = New StdButtonEventArgs(False, 0, Nothing)
                    FindClicked(f)
                Catch ex As Exception
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))
                End Try
            End If
        End Sub

        Private Sub PopulateDropdowns(ByVal visitID As Integer)

            Dim codes As DataClasses.Collections.DomVisitCodeCollection = Nothing
            Dim visitCode As DataClasses.DomVisitCode
            Dim visit As DataClasses.DomProviderInvoiceVisit
            Dim msg As ErrorMessage
            Dim serviceTypes As List(Of ViewablePair) = Nothing
            Dim invoice As DataClasses.DomProviderInvoice

            ' get the original visit
            visit = New DataClasses.DomProviderInvoiceVisit(Me.DbConnection)
            msg = visit.Fetch(visitID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get the invoice
            invoice = New DataClasses.DomProviderInvoice(Me.DbConnection)
            msg = invoice.Fetch(visit.DomProviderInvoiceID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' visit codes
            visitCode = New DataClasses.DomVisitCode(Me.DbConnection, String.Empty, String.Empty)
            msg = visitCode.Fetch(visit.DomVisitCodeID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = DataClasses.DomVisitCode.FetchList(Me.DbConnection, codes, String.Empty, String.Empty, visitCode.DomVisitCodeGroupID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboVisitCodeAmendment.DropDownList
                .DataSource = codes
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

            ' service types
            msg = DomContractBL.FetchServiceTypesAvailableToContract(Me.DbConnection, invoice.DomContractID, TriState.True, serviceTypes)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With cboServiceTypeAmendment.DropDownList
                .DataSource = serviceTypes
                .DataTextField = "Text"
                .DataValueField = "Value"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

            ' Secondary Visit
            If Not Me.IsPostBack Then
                With cboSecondaryVisit.DropDownList
                    .Items.Add(New ListItem("Yes", "True"))
                    .Items.Add(New ListItem("No", "False"))
                End With
            End If
        End Sub

        Private Sub PopulateCarersTable(ByVal VisitID As Long)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_CARERS As String = "spxDomProviderInvoiceVisit_CareWorker_FetchListForView"
            ' grab the list of titles
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_CARERS, False)
                spParams(0).Value = VisitID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_CARERS, spParams)
                rptCarers.DataSource = reader
                rptCarers.DataBind()

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_CARERS, "ViewAmendment.PopulateCarersTable")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            'Display and enable items as required
            If _amendmentRequestExists Then
                pnlAmendmentPanel.Visible = True
                pnlAmendmentDetailsPanel.Visible = True
                pnlVerification.Visible = _amendmentVerified Or Not _councilUser
                pnlVerificationDetails.Visible = _amendmentVerified

                If _currentStatus = _
                        Utils.SplitOnCapitals( _
                            [Enum].GetName(GetType(DomProviderInvoiceVisitAmendmentStatus), DomProviderInvoiceVisitAmendmentStatus.AwaitingVerification) _
                        ) And Not _councilUser Then
                    txtFeedback.Enabled = True
                    optDecline.Disabled = False
                    optVerify.Disabled = False
                    optVerify.Checked = True
                    btnVerification.Disabled = False
                    btnVerification.Visible = True
                Else
                    txtFeedback.Enabled = False
                    optDecline.Disabled = True
                    optVerify.Disabled = True
                    btnVerification.Disabled = True
                    btnVerification.Visible = False
                End If

            Else
                'No amendment request currently exists for this visit
                pnlAmendmentPanel.Visible = True
                pnlAmendmentDetailsPanel.Visible = False
                pnlVerification.Visible = False
                If _invoiced Then
                    'Not linked to an invoice so hide the invoice number panel
                    pnlInvoicedPanel.Visible = False
                Else
                    'Visit is linked to an invoice
                    pnlInvoicedPanel.Visible = True
                End If
            End If

            ' if a retraction is pending...
            IsRetractionPending()

        End Sub

        Private Sub IsRetractionPending()
            ' if a retraction is pending, disable amendment details and show warnings message
            If _retractionPending Then
                pnlAmendmentPanel.Enabled = False
                pnlRetractionPending.Visible = True
                litRetractionPendingInvoiceNumber.Text = lblInvoiceNo.Text
                With _stdBut
                    .AllowNew = False
                    .AllowEdit = False
                    .AllowDelete = False
                    .ShowNew = False
                    .ShowSave = False
                End With
            End If
        End Sub

        Private Sub GetPaymentScheduleId()
            Dim msg As ErrorMessage
            '' try to get Paymentschedule from querystring
            _pScheduleId = Utils.ToInt32(Request.QueryString("pScheduleId"))
            '' try to get from the visit amendment
            If _pScheduleId = 0 Then
                ' get visit id
                Dim invVisitId As Integer = 0
                invVisitId = Utils.ToInt32(Request.QueryString("id"))
                If invVisitId > 0 Then
                    msg = New ErrorMessage
                    Dim invVisit As DataClasses.DomProviderInvoiceVisit = _
                        New DataClasses.DomProviderInvoiceVisit(Me.DbConnection)
                    msg = invVisit.Fetch(invVisitId)
                    If Not msg.Success Then Return
                    ' get invoiceid
                    Dim invId As Integer = 0
                    invId = invVisit.DomProviderInvoiceID
                    '' get paymentscheduleId
                    If invId > 0 Then
                        msg = New ErrorMessage
                        Dim inv As DataClasses.DomProviderInvoice = _
                        New DataClasses.DomProviderInvoice(Me.DbConnection)
                        msg = inv.Fetch(invId)
                        If Not msg.Success Then Return
                        ' set payment scheduleid 
                        _pScheduleId = inv.PaymentScheduleID
                    End If
                End If
            End If

        End Sub

        ''' <summary>
        ''' Verify before saving amendment , if record has already been changed
        ''' </summary>
        ''' <param name="visitId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ChangeRequestExists(visitId As Integer) As Boolean
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim reader As SqlDataReader = Nothing
            Dim requestExists As Boolean
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_AMENDMENT, False)
                spParams(0).Value = visitId
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_AMENDMENT, spParams)

                reader.Read()
                If IsDBNull(reader("RequestDate")) Then
                    requestExists = False
                Else 'Amendment request exists
                    requestExists = True
                End If
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_AMENDMENT, "ViewAmendment.FindClicked")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
            Return requestExists
        End Function


        Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.ServerClick
            Me.CustomNavRemoveLast()
            Me.CustomNavGoBack()
        End Sub


    End Class

End Namespace