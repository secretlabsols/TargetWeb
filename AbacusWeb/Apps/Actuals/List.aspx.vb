
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Wizard screen to search for a view domiciliary contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  02/07/2009  A4WA#5562 - show header controls so a contract can be selected for the report hidden step.
    ''' MvO  19/05/2009  D11549 - added reporting support.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Class List
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryContracts"), "Domiciliary Contracts")

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim estabStep As EstablishmentStep = New EstablishmentStep()
            Dim conStep As DomContractStep = New DomContractStep()
            Dim reportsStep As DomContractHiddenReportsStep = New DomContractHiddenReportsStep()

            With estabStep
                .Description = "Please select a domiciliary provider from the list below and then click ""Next""."
                .Mode = UserControls.EstablishmentSelectorMode.DomProviders
            End With
            
            With conStep
                .ShowNewButton = Me.UserHasMenuItemCommand(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.AddNew"))
                .ShowCopyButton = Me.UserHasMenuItemCommand(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Copy"))
                .ShowReinstateButton = SecurityBL.UserHasMenuItem(Me.DbConnection, currentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryContractsTerminate"), Me.Settings.CurrentApplicationID)
                .ShowTerminateButton = SecurityBL.UserHasMenuItem(Me.DbConnection, currentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryContractsReinstate"), Me.Settings.CurrentApplicationID)
                .ShowHeaderControls = True
            End With

            With reportsStep
                .ShowCapacityReport = Me.UserHasMenuItemCommand(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Reports.Capacity"))
                .ShowSvcDelSummaryReport = Me.UserHasMenuItemCommand(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Reports.ServiceDeliverySummary"))
            End With

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(New DomContractFilterStep)
                .Steps.Add(conStep)
                .Steps.Add(reportsStep)
                .InitControl()
            End With

        End Sub

    End Class
End Namespace
