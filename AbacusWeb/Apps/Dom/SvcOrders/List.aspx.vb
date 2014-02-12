
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.Dom.SvcOrders

    ''' <summary>
    ''' Wizard screen used to search for and view domiciliary service orders.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Paul W      29/06/2010  D11795 - SDS, Generic Contracts and Service Orders
    '''     ColinD      23/04/2010  D11807 - addded support for copying service order at DomServiceOrderStep
    '''     MikeVO      23/09/2009  D11701 - added support for order-level reporting.
    '''     MikeVO      22/05/2009  D11549 - added reporting support.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Class List
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrders"), "Service Orders")

            Dim estabStep As EstablishmentStep = New EstablishmentStep()
            Dim contrStep As ContractStep = New ContractStep()
            Dim dateStep As ServiceOrderFilterStep = New ServiceOrderFilterStep()
            Dim dsoStep As ServiceOrderStep = New ServiceOrderStep()
            Dim canCreateServiceOrders As Boolean = False

            canCreateServiceOrders = Me.UserHasMenuItemCommandInAnyMenuItem(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.AddNew"))

            With estabStep
                .Description = "Please select a provider from the list below and then click ""Next""."
                .Mode = UserControls.EstablishmentSelectorMode.Establishments
            End With

            With contrStep
                .Description = "Please select a contract from the list below and then click ""Next""."
                .SetTitle("Select a Contract")
                .ShowHeaderControls = True
                .ShowNewButton = False
                .ShowViewButton = False
                .ShowCopyButton = False
                .ShowReinstateButton = False
                .ShowTerminateButton = False
            End With

            With dateStep
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
            End With

            With dsoStep
                .ShowCopyButton = canCreateServiceOrders
                .ShowNewButton = canCreateServiceOrders
                .ShowHeaderControls = True
            End With

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(contrStep)
                .Steps.Add(New ClientStep)
                .Steps.Add(dateStep)
                .Steps.Add(dsoStep)
                .Steps.Add(New ServiceOrderHiddenReportsStep())
                .InitControl()
            End With

        End Sub

    End Class
End Namespace
