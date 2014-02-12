Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.Dom.SvcOrders

    ''' <summary>
    ''' Wizard screen used to search for and view and create Commissioned service Suspensions.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     PaulW     02/07/2010  D11795 - SDS, Generic Contracts and Service Orders
    '''     MikeVO    26/05/2009  D11549 - addedd reporting support.
    '''     Paul      14/01/2009  D11472 - Service Order Suspensions.
    ''' </history>
    Partial Class Suspensions
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrderSuspensions"), "Commissioned Service Suspensions")

            Dim resultsStep As ServiceOrderSuspensionPeriodStep = New ServiceOrderSuspensionPeriodStep
            Dim sUserStep As ClientStep = New ClientStep

            sUserStep.Required = False
            sUserStep.HeaderLabelWidth = New Unit(12, UnitType.Em)

            With resultsStep
                .ShowNewButton = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderSuspensions.AddNew"))
            End With


            With SelectorWizard1
                .Steps.Add(sUserStep)
                .Steps.Add(New ServiceOrderSuspensionFilterStep)
                .Steps.Add(resultsStep)
                .Steps.Add(New ServiceOrderSuspensionHiddenReportStep())
                .InitControl()
            End With

        End Sub

    End Class
End Namespace