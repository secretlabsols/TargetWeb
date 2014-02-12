
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security

Namespace Apps.Sds.DPContracts

    ''' <summary>
    ''' Wizard screen to search for and view Direct Payment Contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir 24/10/2012  D12399 Copy Function For Direct Payment Contracts.
    ''' MikeVO  13/04/2011  SDS issue #415 - corrected default DateFrom behaviour (reversing SDS issue #322).
    ''' MikeVO  22/03/2011 SDS issue #524
    ''' Waqas   14/02/2011 D12009 - Updated - Changed the dates Step to call DP contract Filter step for SDS
    ''' ColinD  23/11/2010  SDS345 - Updated - Configured to use DefaultDateFrom property of ClientStep
    ''' JohnF   15/07/2010  D11801 - Created (D11801)
    ''' </history>
    Partial Class List
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Dim clientStep As New ClientStep()
            ' Call SDS filter step
            Dim datesStep As DPContractFilterStep = New DPContractFilterStep()
            Dim dpcStep As DPContractsResultsStep = New DPContractsResultsStep()

            Me.InitPage(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DirectPaymentContracts"), "Direct Payments")

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            With CType(SelectorWizard1, SelectorWizardBase)
                ' default the Date Range step -> Date From to be empty, i.e. today
                .NewEnquiryDefaultQSParams.Add(DateRangeStep.QS_DATEFROM, String.Empty)
            End With

            With dpcStep
                .ShowNewButton = Me.UserHasMenuItemCommand(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.AddNew"))
                .ShowReinstateButton = Me.UserHasMenuItemCommand(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.Reinstate"))
                .ShowTerminateButton = Me.UserHasMenuItemCommand(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.Terminate"))
                .ShowViewButton = True
                .ShowCreatePaymentsButton = SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                                                       currentUser.ID, _
                                                                       ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Administration.CreateDirectPayments"), _
                                                                       Me.Settings.CurrentApplicationID)
                .ShowCopyButton = True
            End With

            With SelectorWizard1
                .Steps.Add(clientStep)
                .Steps.Add(New BudgetHolderStep)
                .Steps.Add(datesStep)
                .Steps.Add(dpcStep)
                .Steps.Add(New DPContractHiddenReportsStep)
                .InitControl()
            End With

        End Sub

    End Class
End Namespace
