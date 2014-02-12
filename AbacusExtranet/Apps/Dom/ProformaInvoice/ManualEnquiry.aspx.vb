
Imports Target.Abacus.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Web.Apps.Security
Imports Target.Library.Web.Utils
Imports Target.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections


Namespace Apps.Dom.ProformaInvoice

    ''' <summary>
    ''' Screen that allows the enquiry of manually entered domiciliary proforma invoices.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Public Class ManualEnquiry
        Inherits Target.Web.Apps.BasePage

        Const QS_PSCHEDULEID As String = "pscheduleid"
        Const QS_backUrl As String = "backUrl"
        Const QS_pSWE As String = "pSWE"

#Region " Properties  "

        Private _backUrl As String
        Public Property backUrl() As String
            Get
                Return _backUrl
            End Get
            Set(ByVal value As String)
                _backUrl = value
            End Set
        End Property


        Private _pSWE As String
        Public Property pSWE() As String
            Get
                Return _pSWE
            End Get
            Set(ByVal value As String)
                _pSWE = value
            End Set
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Manually Entered Visits")
            Me.JsLinks.Add("ManualEnquiry.js")
            DefaultParameterHandler()


            Dim estabStep As EstablishmentStep
            Dim contractStep As DomContractStep
            Dim clientStep As ClientStep
            Dim filterStep As DomProformaInvoiceBatchFilterStep
            Dim results As ManualDomProformaInvoiceBatchEnquiryResultsStep
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            estabStep = New EstablishmentStep()
            With estabStep
                .Mode = EstablishmentStep.EstablishmentStepMode.DomProviders
                .Required = True
                .Description = "Please select a provider from the list below and then click ""Next""."
            End With

            contractStep = New DomContractStep()
            With contractStep
                .ShowHeaderControls = True
                .HeaderLabelWidth = New Unit(8, UnitType.Em)
                .ShowNewButton = False
                .ShowViewButton = _
                    SecurityBL.UserHasMenuItem( _
                        Me.DbConnection, _
                        currentUser.ID, _
                        Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ViewDomiciliaryContract"), _
                        Me.Settings.CurrentApplicationID)

                .Required = True
            End With

            clientStep = New ClientStep()
            With clientStep
                .Mode = UserControls.ClientStepMode.ClientsWithDomSvcOrders
                .Required = True
            End With

            filterStep = New DomProformaInvoiceBatchFilterStep()
            With filterStep
                ' all expect ManualPayment
                .VisibleBatchType = DomProformaInvoiceBatchType.InterfaceFile + _
                                    DomProformaInvoiceBatchType.ManuallyEntered + _
                                    DomProformaInvoiceBatchType.RecalculationAdjustment + _
                                    DomProformaInvoiceBatchType.VisitAmendment
                ' only tick InterfaceFile and ManualllyEntered by default
                .TickedBatchType = DomProformaInvoiceBatchType.InterfaceFile + _
                                    DomProformaInvoiceBatchType.ManuallyEntered
                ' all status checkboxes are visible, all except Deleted ticked by default
                .TickedBatchStatus = DomProformaInvoiceBatchStatus.AwaitingVerification + _
                                    DomProformaInvoiceBatchStatus.Invoiced + _
                                    DomProformaInvoiceBatchStatus.Verified
            End With

            results = New ManualDomProformaInvoiceBatchEnquiryResultsStep()
            With results
                .DefaultBatchType = filterStep.TickedBatchType
                .DefaultBatchStatus = filterStep.TickedBatchStatus
                With .DisplayOptions
                    .ShowNewButton = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentSchedules.AddCopyVisits"))
                    .ShowCopyButton = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentSchedules.AddCopyVisits"))
                    .ShowVerifyButton = Me.UserHasMenuItemCommandInAnyMenuItem(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.ProformaInvoice.Verify"))
                    .ShowUnVerifyButton = .ShowVerifyButton
                    .ShowDeleteButton = Me.UserHasMenuItemCommandInAnyMenuItem(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.ProformaInvoice.Delete"))
                    .ShowViewButton = _
                        SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                                    currentUser.ID, _
                                                    Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), _
                                                     Me.Settings.CurrentApplicationID)
                End With
            End With

            '.Steps.Add(estabStep)
            '.Steps.Add(contractStep)pScheduleId
            With CType(SelectorWizard1, Target.Library.Web.UserControls.SelectorWizardBase)
                .NewEnquiryDefaultQSParams.Add(EstablishmentStep.QS_ESTABLISHMENTID, Request.QueryString(EstablishmentStep.QS_ESTABLISHMENTID))
                .NewEnquiryDefaultQSParams.Add(DomContractStep.QS_CONTRACTID, Request.QueryString(DomContractStep.QS_CONTRACTID))
                .NewEnquiryDefaultQSParams.Add(clientStep.QS_PSCHEDULE_ID, Request.QueryString(clientStep.QS_PSCHEDULE_ID))
                .NewEnquiryDefaultQSParams.Add(QS_pSWE, pSWE)
                .NewEnquiryDefaultQSParams.Add(QS_backUrl, backUrl)
                .ShowBackButtonOnFirststep = True
                .Steps.Add(clientStep)
                .Steps.Add(filterStep)
                .Steps.Add(results)
                .InitControl()
            End With


        End Sub


        Private Sub DefaultParameterHandler()
            If Not Request.QueryString(QS_backUrl) Is Nothing Then
                backUrl = Request.QueryString(QS_backUrl)
            End If
            If Not Request.QueryString(QS_pSWE) Is Nothing Then
                pSWE = HttpUtility.UrlDecode(Request.QueryString(QS_pSWE))
            End If
        End Sub

    End Class

End Namespace