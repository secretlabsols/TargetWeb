Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.Msg
Imports Target.Web.Apps.Security
Imports Target.SP.Web.Apps.UserControls.SelectorWizardSteps

Namespace Apps
    Partial Class ListServices
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPServiceList"), "Services")

            Me.JsLinks.Add("ListServices.js")

            Dim provStep As ProviderStep = New ProviderStep
            provStep.AllowViewProvider = True
            provStep.Description = "Please select a provider from the list below and then click ""Finish""."

            Dim svcStep As ServiceStep = New ServiceStep
            svcStep.AllowViewService = True
            svcStep.Description = "Please select a service from the list below."

            With SelectorWizard1
                .Steps.Add(provStep)
                .Steps.Add(svcStep)
                .InitControl()
            End With

            btnBack.Visible = (Not Request.QueryString("backUrl") Is Nothing)

        End Sub

    End Class

End Namespace
