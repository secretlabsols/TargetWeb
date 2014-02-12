Imports Constants = Target.Library.Web
Imports Target.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Abacus.Library
Imports Target.Web.Apps.Security

Namespace Apps.Dom.ProviderInvoice
    Partial Public Class VisitAmendmentRequestList
        Inherits Target.Web.Apps.BasePage

#Region " Fields "
        Private _pScheduleId As Integer
        Private _contractId As Integer
        Private _providerId As Integer
        Private _backUrl As String
        Private _currentstep As Integer

        Private _statusAwait As Boolean = False
        Private _statusVer As Boolean = False

#End Region

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.VisitAmendmentRequestEnquiry"), "Visit Amendment Requests")
            Me.JsLinks.Add("VisitAmendmentRequestList.js")


            _pScheduleId = Utils.ToInt32(Request.QueryString("pScheduleId"))
            _backUrl = Utils.ToString(Request.QueryString("backUrl"))
            _currentstep = Utils.ToInt32(Request.QueryString("currentstep"))

            _statusAwait = Utils.ToBoolean(Request.QueryString("await"))
            _statusVer = Utils.ToBoolean(Request.QueryString("ver"))

            Dim estabStep As EstablishmentStep
            Dim contractStep As DomContractStep
            Dim svcUsrStep As ClientStep
            Dim filterStep As VisitAmendmentRequestEnquiryFilterStep
            Dim resultsStep As VisitAmendmentRequestEnquiryResultsStep = New VisitAmendmentRequestEnquiryResultsStep
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Security.WebSvc.Security))


            estabStep = New EstablishmentStep()
            With estabStep
                .Mode = EstablishmentStep.EstablishmentStepMode.DomProviders
                .Required = False
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
                .Description = "Please select a provider from the list below and then click ""Next""."
            End With

            contractStep = New DomContractStep()
            With contractStep
                .ShowHeaderControls = True
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
                .ShowNewButton = False
                .ShowViewButton = True
                .Required = False
            End With

            svcUsrStep = New ClientStep
            With svcUsrStep
                .Mode = UserControls.ClientStepMode.ClientsWithDomProviderInvoices
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
            End With

            filterStep = New VisitAmendmentRequestEnquiryFilterStep
            With filterStep
                .IsCouncilUser = SecurityBL.IsCouncilUser(Me.DbConnection, Me.Settings, currentUser.ExternalUserID)
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
                If (_statusAwait And _statusVer) Then
                    .TickedStatus = DomProviderInvoiceVisitAmendmentStatus.AwaitingVerification + _
                                    DomProviderInvoiceVisitAmendmentStatus.Verified
                End If
                If (_statusAwait And Not _statusVer) Then
                    .TickedStatus = DomProviderInvoiceVisitAmendmentStatus.AwaitingVerification
                End If
                If (Not _statusAwait And _statusVer) Then
                    .TickedStatus = DomProviderInvoiceVisitAmendmentStatus.Verified
                End If

                .VisibleStatus = DomProviderInvoiceVisitAmendmentStatus.AwaitingVerification + _
                                 DomProviderInvoiceVisitAmendmentStatus.Verified
            End With

            '.Steps.Add(estabStep)
            '.Steps.Add(contractStep)
            '.Steps.Add(svcUsrStep)
            CType(pSchedules, Apps.UserControls.PaymentScheduleHeader).PaymentScheduleId = _pScheduleId
            _contractId = CType(pSchedules, Apps.UserControls.PaymentScheduleHeader).ContractId
            _providerId = CType(pSchedules, Apps.UserControls.PaymentScheduleHeader).EstabId

            With SelectorWizard1
                .NewEnquiryDefaultQSParams.Add("pScheduleId", _pScheduleId)
                .NewEnquiryDefaultQSParams.Add("contractId", _contractId)
                .NewEnquiryDefaultQSParams.Add("estabid", _providerId)
                .NewEnquiryDefaultQSParams.Add("backUrl", _backUrl)
                .NewEnquiryDefaultQSParams.Add("await", _statusAwait.ToString().ToLower())
                .NewEnquiryDefaultQSParams.Add("ver", _statusVer.ToString().ToLower())
                .ShowBackButtonOnFirststep = True
                .Steps.Add(filterStep)
                .Steps.Add(resultsStep)
                .InitControl()
            End With

            
            Me.CustomNavAdd(True)

        End Sub

     
    End Class
End Namespace