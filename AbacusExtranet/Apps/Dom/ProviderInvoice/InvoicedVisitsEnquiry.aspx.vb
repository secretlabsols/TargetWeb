
Imports Target.Abacus.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library.Web.UserControls

Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Wizard screen to allow the enquiry of visit related to domiciliary provider invoices.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF   17/12/2009  Allow for flexible back navigation (#5966)
    ''' MikeVO  19/10/2009  D11546 - changed weekending filter to a date range.
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MvO     12/11/2008  Changed SU selector to use ClientsWithVisitBasedDomProviderInvoices mode.
    ''' </history>
    Partial Public Class InvoicedVisitsEnquiry
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.InvoicedVisitEnquiry"), "Invoiced Visits")

            Dim estabStep As EstablishmentStep
            Dim contractStep As DomContractStep
            Dim svcUsrStep As ClientStep
            Dim careWorkerStep As CareWorkerStep = New CareWorkerStep
            Dim dateStep As DateRangeStep = New DateRangeStep
            Dim results As DomProviderInvoiceVisitsEnquiryResultsStep = New DomProviderInvoiceVisitsEnquiryResultsStep

            estabStep = New EstablishmentStep()
            With estabStep
                .Mode = EstablishmentStep.EstablishmentStepMode.DomProviders
                .Required = True
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
                .Description = "Please select a provider from the list below and then click ""Next""."
            End With

            contractStep = New DomContractStep()
            With contractStep
                .ShowHeaderControls = True
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
                .ShowNewButton = False
                .ShowViewButton = True
                .Required = True
            End With

            svcUsrStep = New ClientStep
            With svcUsrStep
                .Mode = UserControls.ClientStepMode.ClientsWithVisitBasedDomProviderInvoices
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
            End With

            With dateStep
                .Required = True
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
            End With

            careWorkerStep = New CareWorkerStep
            With careWorkerStep
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
            End With

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(contractStep)
                .Steps.Add(svcUsrStep)
                .Steps.Add(careWorkerStep)
                .Steps.Add(dateStep)
                .Steps.Add(results)
                .InitControl()
            End With

            Me.CustomNavAdd(True)

        End Sub

    End Class
End Namespace