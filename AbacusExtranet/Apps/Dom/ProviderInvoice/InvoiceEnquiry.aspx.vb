Imports Target.Abacus.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Constants = Target.Library.Web


Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Wizard screen to allow the enquiry of domiciliary provider invoices.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF   17/12/2009  Allow for flexible back navigation (#5966)
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MvO     12/11/2008  Changed SU selector to use ClientsWithDomProviderInvoices mode.
    ''' </history>
    Partial Public Class InvoiceEnquiry
        Inherits Target.Web.Apps.BasePage

#Region " fields "
        Private Const _WebCmdRetractKey As String = "AbacusExtranet.WebNavMenuItemCommand.DomiciliaryCare.ProviderInvoices.RetractProviderInvoice"
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ProviderInvoiceEnquiry"), "Service User Payments")

            Dim estabStep As EstablishmentStep
            Dim contractStep As DomContractStep
            Dim svcUsrStep As ClientStep
            Dim filterStep As DomProviderInvoiceFilterStep = New DomProviderInvoiceFilterStep
            Dim results As DomProviderInvoiceEnquiryResultsStep = New DomProviderInvoiceEnquiryResultsStep
            results.EnableRetractFeature = True
            results.UserHasRetractCommand = UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdRetractKey))
            results.HideColumnContractNumber = False
            results.HideColumnProviderReference = False
            results.HideSUReference = True
            results.showPaymentSchedule = False

            estabStep = New EstablishmentStep()
            With estabStep
                .IsCareProvider = True
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
            End With

            svcUsrStep = New ClientStep
            With svcUsrStep
                .Mode = UserControls.ClientStepMode.ClientsWithDomProviderInvoices
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
            End With

            With SelectorWizard1
                .FinishButton.Style.Remove("width")
                .FinishButton.Style.Add("width", "4em")
                .BackButton.Style.Remove("margin-right")
                .BackButton.Style.Add("margin-right", "4px")
                .Steps.Add(estabStep)
                .Steps.Add(contractStep)
                .Steps.Add(svcUsrStep)
                .Steps.Add(filterStep)
                .Steps.Add(results)
                .InitControl()
            End With

            Me.CustomNavAdd(True)

        End Sub


    End Class
End Namespace