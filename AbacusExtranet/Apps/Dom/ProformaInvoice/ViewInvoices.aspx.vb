Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Namespace Apps.Dom.ProformaInvoice

    ''' <summary>
    ''' Screen to allow a user to view the invoices within a domiciliary pro forma invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      16/10/2009  D11546 - added payment mismatch tolerance and S/U Ref/Name filters.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewInvoices
        Inherits Target.Web.Apps.BasePage

#Region " Private variables "
        Dim _pScheduleId As Integer = 0
        Dim _selectedInvoiceId As Integer = 0
        Dim _estabId As Integer = 0
        Dim _contractId As Integer = 0
        Dim _clientId As Integer = 0
        Dim _batchstatus As String = String.Empty
        Dim _filterPanelExpanded As String
        Dim _canVerifyUnverify As Boolean
        Dim _canDeleteInvoice As Boolean
        Dim _queried As String
        Dim _payment As String
        Dim _dcrFilter As String
        Dim _tolerance As String
        Dim _await As String
        Dim _ver As String

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim paymentScheduleId As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim style As New StringBuilder
            _pScheduleId = Utils.ToInt32(Request.QueryString("pScheduleId"))
            _selectedInvoiceId = Utils.ToInt32(Request.QueryString("selectedInvoiceID"))
            _filterPanelExpanded = Utils.ToString(Request.QueryString("filter"))
            _queried = Utils.ToString(Request.QueryString("q"))
            _payment = Utils.ToString(Request.QueryString("m"))
            _tolerance = Utils.ToString(Request.QueryString("t"))
            _await = Utils.ToString(Request.QueryString("await"))
            _ver = Utils.ToString(Request.QueryString("ver"))

            If Not Request.QueryString("dcrFilter") Is Nothing Then
                _dcrFilter = Utils.ToInt32(Request.QueryString("dcrFilter"))
            Else
                _dcrFilter = -2
            End If


            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Pro forma Invoices")
            Me.UseJQuery = True
            Me.UseExt = True
            If Not Me.IsPostBack Then
                Me.CustomNavAdd(True)
            End If
            LoadDetails()

            btnRecalculate.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.Recalculate"))
            _canVerifyUnverify = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.ProformaInvoice.Verify"))
            btnVerify.Visible = _canVerifyUnverify
            btnUnVerify.Visible = _canVerifyUnverify
            _canDeleteInvoice = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.ProformaInvoice.Delete"))
            btnDelete.Visible = _canDeleteInvoice

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProformaInvoice/ViewInvoices.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProformaInvoice/CommentDialog.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Library.ReferenceDataCommon.Services.VerificationTextSvc))
            style.Append("label.label { float:left; width:9em; font-weight:bold; }")
            style.Append("span.label { float:left; width:9em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            'Set breadcrumb
            'With CType(breadCrumb, InvoiceBatchBreadcrumb)
            '    .BatchID = domProformaBatchID
            'End With
            'set a fixed width on the 2 combos
            cboMismatches.DropDownList.Width = New Unit(8, UnitType.Em)
            cboQueried.DropDownList.Width = New Unit(8, UnitType.Em)

            'Populate Filter Drop downs
            cboQueried.DropDownList.Items.Add("")
            cboQueried.DropDownList.Items.Add("Any Note")
            cboQueried.DropDownList.Items.Add("Private Note")
            cboQueried.DropDownList.Items.Add("Invoice Note")
            cboQueried.DropDownList.Items.Add("No Note")
            cboMismatches.DropDownList.Items.Add("")
            cboMismatches.DropDownList.Items.Add("Mismatched")
            cboMismatches.DropDownList.Items.Add("Matched")
            cboQueried.DropDownList.Attributes.Add("onchange", "cboQueried_Click();")
            cboMismatches.DropDownList.Attributes.Add("onchange", "cboMismatch_Click();")
            rdbDcrFilterDefault.Attributes.Add("onclick", "GetRDBValue();")
            rdbDcrFilterNo.Attributes.Add("onclick", "GetRDBValue();")
            rdbDcrFilterYes.Attributes.Add("onclick", "GetRDBValue();")

            CType(psHeader1, Apps.UserControls.PaymentScheduleHeader).SingleLiner = False
            CType(psHeader1, Apps.UserControls.PaymentScheduleHeader).PaymentScheduleId = _pScheduleId

            SetReports()

            If _filterPanelExpanded.Length > 0 AndAlso Boolean.Parse(_filterPanelExpanded) Then
                cpe.Expanded = True
            End If

        End Sub

#Region " Disabled in D12084 Remove Invoice "
        'Removed in Dev D12084
        'Private Sub btnRemove_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemove.ServerClick

        '    Dim msg As ErrorMessage
        '    Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
        '    Dim batch As DomProformaInvoiceBatch
        '    Dim selectedInvoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("selectedInvoiceID"))
        '    'Dim originalBatchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
        '    Dim originalBatch As DomProformaInvoiceBatch = Nothing
        '    Dim invoice As DomProformaInvoice = Nothing
        '    Dim trans As SqlTransaction = Nothing

        '    Try
        '        'Start a transaction
        '        trans = SqlHelper.GetTransaction(Me.DbConnection)

        '        'Fetch the details of the invoice we want to remove
        '        invoice = New DomProformaInvoice(trans)
        '        msg = invoice.Fetch(selectedInvoiceID)
        '        If Not msg.Success Then
        '            'Rollback transaction and display error
        '            SqlHelper.RollbackTransaction(trans)
        '            WebUtils.DisplayError(msg)
        '        End If

        '        'Fetch the details of the batch we wish to remove the invoice from.
        '        originalBatch = New DomProformaInvoiceBatch(trans)
        '        msg = originalBatch.Fetch(invoice.DomProformaInvoiceBatchID)
        '        If Not msg.Success Then
        '            'Rollback transaction and display error
        '            SqlHelper.RollbackTransaction(trans)
        '            WebUtils.DisplayError(msg)
        '        End If

        '        'Create a new batch to place the removed proforma invoice
        '        batch = New DomProformaInvoiceBatch(trans)
        '        With batch
        '            .ProviderID = originalBatch.ProviderID
        '            .DomContractID = originalBatch.DomContractID
        '            .UserID = currentUser.ExternalUserID
        '            .DateCreated = DateTime.Now
        '            .CreatedBy = String.Format("{0} {1}", currentUser.FirstName, currentUser.Surname)
        '            If .CreatedBy.Length > 50 Then .CreatedBy = .CreatedBy.Substring(0, 50)
        '            .DomProformaInvoiceBatchTypeID = DomProformaInvoiceBatchType.ManuallyEntered
        '            .VisitBasedReturn = originalBatch.VisitBasedReturn
        '            .DomProformaInvoiceBatchStatusID = DomProformaInvoiceBatchStatus.AwaitingVerification
        '            .StatusDate = .DateCreated
        '            .StatusChangedBy = .CreatedBy
        '            'Set Item count to 1 to reflect the invoice we are moving
        '            .ItemCount = 1
        '            'set the total payment amount to that of the Proforma invoice amount.
        '            .TotalPayment = invoice.CalculatedPayment
        '            msg = .Save
        '            If Not msg.Success Then
        '                'Rollback transaction and display error
        '                SqlHelper.RollbackTransaction(trans)
        '                WebUtils.DisplayError(msg)
        '            End If
        '        End With
        '        'Update the original batch, with updated totals
        '        With originalBatch
        '            'decrease Item count by one
        '            .ItemCount = .ItemCount - 1
        '            'Remove value of invoice from the Total Payment on the batch.
        '            .TotalPayment = .TotalPayment - invoice.CalculatedPayment
        '            msg = .Save
        '            If Not msg.Success Then
        '                'Rollback transaction and display error
        '                SqlHelper.RollbackTransaction(trans)
        '                WebUtils.DisplayError(msg)
        '            End If
        '        End With

        '        'Update the invoice setting the invoice batch ID equal to the new batch created above.
        '        With invoice
        '            .DomProformaInvoiceBatchID = batch.ID
        '            msg = .Save
        '            If Not msg.Success Then
        '                'Rollback transaction and display error
        '                SqlHelper.RollbackTransaction(trans)
        '                WebUtils.DisplayError(msg)
        '            End If
        '        End With

        '        'Commit Database transaction
        '        trans.Commit()


        '    Catch ex As Exception
        '        SqlHelper.RollbackTransaction(trans)
        '        WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))
        '    End Try

        'End Sub
#End Region


        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim paymentScheduleId As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim batchStatus As Short
            'Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            'Dim style As New StringBuilder
            Dim js As String
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))

            If currentPage <= 0 Then currentPage = 1

            msg = New ErrorMessage
            Dim invoice As DataClasses.DomProformaInvoice = New DataClasses.DomProformaInvoice(Me.DbConnection, String.Empty, String.Empty)

            batchStatus = GetBatchStatus(_selectedInvoiceId, invoice, _batchstatus)
            'If Not invoice Is Nothing Then
            '    _clientId = invoice.ClientID
            'End If

            msg = New ErrorMessage
            Dim pSchedule As DataClasses.PaymentSchedule = New DataClasses.PaymentSchedule(Me.DbConnection, String.Empty, String.Empty)
            msg = pSchedule.Fetch(_pScheduleId)
            If Not msg.Success Then Return
            If Not pSchedule Is Nothing Then
                _estabId = pSchedule.ProviderID
                _contractId = pSchedule.DomContractID
            End If


            js = String.Format("currentPage={0}; " & _
                               "paymentScheduleId={1};" & _
                               "batchStatus={2};" & _
                               "btnRecalculateID=""{3}"";" & _
                               "rdbDcrFilterDefaultID=""{4}""; " & _
                               "rdbDcrFilterNoID=""{5}""; " & _
                               "rdbDcrFilterYesID=""{6}"";" & _
                               "estabId={7};" & _
                               "contractId={8};" & _
                               "batchStatusDesc=""{9}"";" & _
                               "canVerifyUnverify=""{10}"";" & _
                               "canDeleteInvoice=""{11}"";" & _
                               "selectedNote=""{12}"";" & _
                               "paymentMismatch=""{13}"";" & _
                               "dcrFilter=""{14}"";" & _
                               "mismatchtolerance=""{15}"";", _
                               currentPage, _
                               paymentScheduleId, _
                               batchStatus, _
                               btnRecalculate.ClientID, _
                               rdbDcrFilterDefault.ClientID, _
                               rdbDcrFilterNo.ClientID, _
                               rdbDcrFilterYes.ClientID, _
                               _estabId, _
                               _contractId, _
                               _batchstatus, _
                               _canVerifyUnverify.ToString().ToLower(), _
                               _canDeleteInvoice.ToString.ToLower(), _
                               _queried, _
                               _payment, _
                               _dcrFilter, _
                               _tolerance _
                                )

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))


        End Sub

        Private Sub LoadDetails()

            cboMismatches = cpe.FindControl("cboMismatches")
            cboQueried = cpe.FindControl("cboQueried")
            txtTolerance = cpe.FindControl("txtTolerance")

        End Sub

        Private Function GetBatchStatus(ByVal invoiceId As Integer, _
                                        ByRef invoice As DataClasses.DomProformaInvoice, _
                                        ByRef batchStatusDesc As String) As Integer
            Try
                invoice.Fetch(invoiceId)
                Dim batch As DataClasses.DomProformaInvoiceBatch = New DomProformaInvoiceBatch(Me.DbConnection, String.Empty, String.Empty)
                batch.Fetch(invoice.DomProformaInvoiceBatchID)
                If batch.DomProformaInvoiceBatchStatusID = DomProformaInvoiceBatchStatus.Verified Then
                    batchStatusDesc = "Verified"
                ElseIf batch.DomProformaInvoiceBatchStatusID = DomProformaInvoiceBatchStatus.AwaitingVerification Then
                    batchStatusDesc = "Awaiting Verification"
                End If
                Return batch.DomProformaInvoiceBatchStatusID
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Private Sub SetReports()

            With CType(rptPrint, Target.Library.Web.UserControls.IReportsButton)
                .Position = Target.Library.Web.Controls.SearchableMenu.SearchableMenuPosition.TopRight
                .Enabled = True
                .ButtonText = "Print"
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebReport.UnProcessedProformaInvoiceList")
                If _pScheduleId > 0 Then .Parameters.Add("intPScheduleID", _pScheduleId)

                If _queried = "No Note" Then
                    .Parameters.Add("intQueried", 0)
                ElseIf _queried = "Private Note" Then
                    .Parameters.Add("intQueried", 1)
                ElseIf _queried = "Invoice Note" Then
                    .Parameters.Add("intQueried", 2)
                ElseIf _queried = "Any Note" Then
                    .Parameters.Add("intQueried", 3)
                End If

                If _payment = "Mismatched" Then
                    .Parameters.Add("blnMismatch", True)
                ElseIf _payment = "No Mismatch" Then
                    .Parameters.Add("blnMismatch", False)
                End If


                If txtTolerance.Value <> "" Then
                    .Parameters.Add("mnyTolerance", txtTolerance.Value)
                End If


                If _dcrFilter = 0 Then
                    .Parameters.Add("dcrFilter", False)
                ElseIf _dcrFilter = -1 Then
                    .Parameters.Add("dcrFilter", True)
                End If

                ' awaiting and verified filters
                .Parameters.Add("await", _await)
                .Parameters.Add("ver", _ver)

            End With

        End Sub
    End Class

End Namespace