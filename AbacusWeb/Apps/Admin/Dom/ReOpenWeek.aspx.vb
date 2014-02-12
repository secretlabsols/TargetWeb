
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Screen to allow the enquiry of re-opened domiciliary contract weeks.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Class ReOpenWeek
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Re-openedWeeks"), "Re-Opened Weeks")

            Dim estabStep As EstablishmentStep = New EstablishmentStep()
            Dim contractStep As DomContractStep = New DomContractStep()
            Dim weeksResultStep As DomContractReOpenWeeksResultStep = New DomContractReOpenWeeksResultStep()

            With estabStep
                .Description = "Please select a domiciliary provider from the list below and then click ""Next""."
                .HeaderLabelWidth = New Unit(18, UnitType.Em)
                .Mode = UserControls.EstablishmentSelectorMode.DomProviders
            End With

            With contractStep
                .Description = "Please select a domiciliary contract from the list below and then click ""Next""."
                .HeaderLabelWidth = New Unit(18, UnitType.Em)
                .ShowHeaderControls = True
                .ShowNewButton = False
                .ShowViewButton = False
                .ShowCopyButton = False
                .ShowServiceUserColumn = True
            End With

            With weeksResultStep
                .ShowNewButton = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Re-openedWeeks.AddNew"))
            End With

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(contractStep)
                .Steps.Add(New DomContractReOpenWeeksFilterStep)
                .Steps.Add(weeksResultStep)
                .InitControl()
            End With

        End Sub

    End Class
End Namespace
