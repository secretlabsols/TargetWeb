
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Screen to allow the enquiry of manual domiciliary payments, i.e. manual pro forma invoices.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Class ManualPayments
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ManualPayments"), "Manual Payments")

            Dim estabStep As EstablishmentStep = New EstablishmentStep()
            Dim contractStep As DomContractStep = New DomContractStep()
            Dim manPaymentStep As ManualPaymentDomProformaInvoiceStep = New ManualPaymentDomProformaInvoiceStep()
            Dim reportsStep As ManualPaymentDomProformaInvoiceHiddenReportsStep = New ManualPaymentDomProformaInvoiceHiddenReportsStep()

            With estabStep
                .Description = "Please select a domiciliary provider from the list below and then click ""Next""."
                .Mode = UserControls.EstablishmentSelectorMode.DomProviders
            End With

            With contractStep
                .Description = "Please select a domiciliary contract from the list below and then click ""Next""."
                .ShowHeaderControls = True
                .ShowNewButton = False
                .ShowViewButton = False
                .ShowCopyButton = False
                .ShowServiceUserColumn = True
                .SetTitle("Select a Domiciliary Contract")
            End With

            With manPaymentStep
                .ShowNewButton = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ManualPayments.AddNew"))
            End With

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(contractStep)
                .Steps.Add(New DomContractPeriodSysAccFilterStep)
                .Steps.Add(manPaymentStep)
                .Steps.Add(reportsStep)
                .InitControl()
            End With

        End Sub

    End Class
End Namespace
