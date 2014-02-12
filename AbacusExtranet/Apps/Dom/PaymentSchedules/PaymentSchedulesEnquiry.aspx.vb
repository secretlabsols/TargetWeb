
Imports Target.Abacus.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Web.Apps.Security
Imports Constants = Target.Library.Web
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.UserControls

Namespace Apps.Dom.PaymentSchedules

    ''' <summary>
    ''' Screen that allows the enquiry of payment schedules.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     IHS         23/05/2011  D12084 - Created
    ''' </history>
    Partial Public Class PaymentSchedulesEnquiry
        Inherits Target.Web.Apps.BasePage

#Region " fields "
        Private Const _WebCmdRetractKey As String = "AbacusExtranet.WebNavMenuItemCommand.DomiciliaryCare.ProviderInvoices.RetractProviderInvoice"
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Payment Schedules")

            Dim estabStep As EstablishmentStep
            Dim contractStep As DomContractStep
            'Dim filterStep As DomProviderInvoiceFilterStep = New DomProviderInvoiceFilterStep
            Dim filterStep As New PaymentSchedulesFilterStep
            Dim results As PaymentScheduleEnquiryResultsStep = New PaymentScheduleEnquiryResultsStep
            results.EnableRetractFeature = True
            results.UserHasRetractCommand = UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdRetractKey))

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


            With CType(SelectorWizard1, SelectorWizardBase)
                .Steps.Add(estabStep)
                .Steps.Add(contractStep)
                .Steps.Add(filterStep)
                .Steps.Add(results)
                .InitControl()
            End With

            Me.CustomNavAdd(True)

        End Sub

    End Class

End Namespace