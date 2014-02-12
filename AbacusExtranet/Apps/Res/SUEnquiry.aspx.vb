Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.Res

    ''' <summary>
    ''' Screen to allow a user search for and view residential service users.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class SUEnquiry
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ServiceUserEnquiry"), "Residential Service User Enquiry")

            Dim estabStep As EstablishmentStep = New EstablishmentStep
            Dim dateStep As DateRangeStep = New DateRangeStep
            Dim suStep As ClientStep = New ClientStep
            Dim strStyle As New StringBuilder

            strStyle.Append("label.label { width:7em; padding-right:1em; font-weight: bold; }")
            strStyle.Append(".chkBoxStyle { width:65em; padding-right:1em; font-weight: bold;}")
            Me.AddExtraCssStyle(strStyle.ToString)

            estabStep.Description = "Please select a residential home from the list below and then click ""Next""."
            estabStep.Mode = EstablishmentStep.EstablishmentStepMode.ResidentialHomes

            dateStep.Description = "Please enter a date range to filter the results on. The results will be filtered to show only service users who have care that overlap with these dates."
            dateStep.HeaderLabelWidth = New Unit(8, UnitType.Em)

            suStep.Mode = UserControls.ClientStepMode.ResidentialClients
            suStep.ViewClientBaseUrl = "ViewServiceUser.aspx?id="
            suStep.AddHeaderControls = False

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(dateStep)
                .Steps.Add(suStep)
                .InitControl()
            End With

        End Sub

    End Class
End Namespace
