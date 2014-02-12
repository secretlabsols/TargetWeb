
Imports Target.Abacus.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Web.Apps.Security

Namespace Apps.Dom.ProformaInvoice
    Partial Public Class InvoiceEnquiry
        Inherits Target.Web.Apps.BasePage

        ''' <summary>
        ''' Screen that allows the enquiry of interface domiciliary proforma invoices.
        ''' </summary>
        ''' <remarks></remarks>
        ''' <history>
        '''     Waqas       17/03/2011  D12081 
        '''     MikeVO      01/12/2008  D11444 - security overhaul.
        '''     MikeVO      ??/??/????  Created.
        ''' </history>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Domiciliary Pro forma Invoice Enquiry")

            Dim estabStep As EstablishmentStep
            Dim contractStep As DomContractStep
            Dim filterStep As DomProformaInvoiceBatchFilterStep
            Dim results As DomProformaInvoiceBatchEnquiryResultsStep
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
                .ShowViewButton = True
                .Required = False
            End With

            filterStep = New DomProformaInvoiceBatchFilterStep()
            With filterStep
                ' all expect ManualPayment
                .VisibleBatchType = DomProformaInvoiceBatchType.InterfaceFile + _
                                    DomProformaInvoiceBatchType.ManuallyEntered + _
                                    DomProformaInvoiceBatchType.RecalculationAdjustment + _
                                    DomProformaInvoiceBatchType.VisitAmendment
                ' only tick ManualllyEntered by default
                .TickedBatchType = DomProformaInvoiceBatchType.InterfaceFile
                ' all status checkboxes are visible, all expect Deleted ticked by default
                .TickedBatchStatus = DomProformaInvoiceBatchStatus.AwaitingVerification + _
                                    DomProformaInvoiceBatchStatus.Invoiced + _
                                    DomProformaInvoiceBatchStatus.Verified
            End With

            results = New DomProformaInvoiceBatchEnquiryResultsStep()
            With results
                .DefaultBatchType = filterStep.TickedBatchType
                .DefaultBatchStatus = filterStep.TickedBatchStatus
                With .DisplayOptions
                    .ShowViewContractButton = _
                        SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                                   currentUser.ID, _
                                                   Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), _
                                                   Me.Settings.CurrentApplicationID)
                    .ShowViewBatchButton = _
                        SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                                   currentUser.ID, _
                                                   Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), _
                                                   Me.Settings.CurrentApplicationID)
                    .ShowViewInvoicesButton = _
                        SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                                   currentUser.ID, _
                                                   Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), _
                                                   Me.Settings.CurrentApplicationID)
                End With
            End With

            With CType(SelectorWizard1, Target.Library.Web.UserControls.SelectorWizardBase)
                .Steps.Add(estabStep)
                .Steps.Add(contractStep)
                .Steps.Add(filterStep)
                .Steps.Add(results)
                .InitControl()
            End With


        End Sub

    End Class

End Namespace