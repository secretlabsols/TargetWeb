Imports Target.Web.Apps
Imports Target.Abacus.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library.Web.UserControls

Namespace Apps.ServiceOrders.NonRes.ServiceOrders

    Partial Public Class List
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ServiceOrders"), "Service Orders")

            Dim estabStep As EstablishmentStep
            Dim contractStep As DomContractStep
            Dim dateStep As New DateRangeDSOMovementStep
            Dim results As New GenericServiceOrderResultsStep
            Dim reports As New ServiceOrderHiddenReportsStep

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/date.js"))

            Me.UseJQuery = True
            Me.UseJqueryUI = True
            Me.UseJqueryTextboxClearer = True
            estabStep = New EstablishmentStep()
            With estabStep
                .Mode = EstablishmentStep.EstablishmentStepMode.DomProviders
                .IsCareProvider = True
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

            dateStep.UseJquery = True

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(contractStep)
                .Steps.Add(dateStep)
                .Steps.Add(results)
                .Steps.Add(reports)
                .InitControl()
            End With

            With CType(SelectorWizard1, SelectorWizardBase)
                ' default the Date Range step -> Date From to be empty, i.e. today
                .NewEnquiryDefaultQSParams.Add(DateRangeStep.QS_DATEFROM, Date.Today)
                .NewEnquiryDefaultQSParams.Add(DateRangeStep.QS_DATETO, Date.Today)
            End With

        End Sub

    End Class

End Namespace