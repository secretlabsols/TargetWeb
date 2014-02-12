
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.Sds

    ''' <summary>
    ''' Wizard scree to provide access to planned care packages.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Class PlannedCarePackageEnquiry
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.PlannedCarePackageEnquiry"), "Planned Care Package Enquiry")

            Dim suStep As ClientStep = New ClientStep()
            Dim dateStep As DateRangeStep = New DateRangeStep()
            Dim resultsStep As PlannedCarePackageEnquiryResultStep = New PlannedCarePackageEnquiryResultStep()

            With suStep
                .Required = True
            End With

            With dateStep
                .Description = "Please select a date range to filter the result on. The date range you select is compared to the effective date of the planned care package."
                .HeaderLabelWidth = New Unit(10, UnitType.Em)
            End With

            With resultsStep
                .ShowNewButton = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PlannedCarePackageEnquiry.AddNew"))
            End With

            With SelectorWizard1
                .Steps.Add(suStep)
                .Steps.Add(dateStep)
                .Steps.Add(resultsStep)
                .InitControl()
            End With

        End Sub

    End Class
End Namespace
