
Imports Target.Abacus.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Web.Apps.Security

Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Screen to allow a user to a user to search for and view visit amendment requests.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Waqas       18/03/2011  D12082
    '''     Waqas       17/03/2011  D12081
    '''     JohnF       17/12/2009  Allow for flexible back navigation (#5966)
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Public Class VisitAmendReqEnquiry
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.VisitAmendmentRequestEnquiry"), "Visit Amendment Requests")

        End Sub

      
        Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load


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
                .Mode = UserControls.ClientStepMode.ClientsWithVisitBasedDomProviderInvoices
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
            End With

            filterStep = New VisitAmendmentRequestEnquiryFilterStep
            With filterStep
                .IsCouncilUser = SecurityBL.IsCouncilUser(Me.DbConnection, Me.Settings, currentUser.ExternalUserID)
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
                .TickedStatus = DomProviderInvoiceVisitAmendmentStatus.AwaitingVerification + _
                                DomProviderInvoiceVisitAmendmentStatus.Declined + _
                                DomProviderInvoiceVisitAmendmentStatus.Invoiced + _
                                DomProviderInvoiceVisitAmendmentStatus.Processed + _
                                DomProviderInvoiceVisitAmendmentStatus.Verified

                .VisibleStatus = DomProviderInvoiceVisitAmendmentStatus.AwaitingVerification + _
                                 DomProviderInvoiceVisitAmendmentStatus.Declined + _
                                 DomProviderInvoiceVisitAmendmentStatus.Invoiced + _
                                 DomProviderInvoiceVisitAmendmentStatus.Processed + _
                                 DomProviderInvoiceVisitAmendmentStatus.Verified
            End With

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(contractStep)
                .Steps.Add(svcUsrStep)
                .Steps.Add(filterStep)
                .Steps.Add(resultsStep)
                .InitControl()
            End With

            Me.CustomNavAdd(True)
        End Sub
    End Class

End Namespace