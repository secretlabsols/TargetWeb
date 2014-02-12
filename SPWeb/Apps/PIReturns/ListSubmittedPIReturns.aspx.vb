Imports System.Collections.Specialized
Imports Target.Web.Apps.Security
Imports Target.SP.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.SP.Library
Imports Target.SP.Library.SPClassesBL
Imports Target.Library.Web
Imports Target.Library
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Web.Apps.FileStore
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.PIReturns

    Partial Class ListSubmittedPIReturns
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_FINANCIALYEAR As String = "finYr"
        Const QS_SERVICEID As String = "serviceID"
        Const QS_QUARTER As String = "qtr"
        Const STEP_FIN_YR_QTR As Integer = 2
        Const STEP_RESULTS As Integer = 3

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemPIReturnsListSubmitted"), "List Submitted PI Returns")

            Dim currentStep As Integer = Target.Library.Utils.ToInt32(Request.QueryString(QS_CURRENTSTEP))
            Dim provStep As ProviderStep
            Dim svcStep As ServiceStep
            Dim finYrQtrStep As FinancialYrQtrPIStatusStep

            ' setup the steps
            provStep = New ProviderStep
            provStep.Required = False
            svcStep = New ServiceStep
            finYrQtrStep = New FinancialYrQtrPIStatusStep

            With SelectorWizard1
                ' add the steps
                .Steps.Add(provStep)
                .Steps.Add(svcStep)
                .Steps.Add(finYrQtrStep)
                .Steps.Add(New SubmittedPIReturnsResultsStep)

                ' set the display of the new button
                .NewButton.Value = "New Enquiry"
                .NewButton.Attributes("title") = "Click here to list Submitted PI Returns."

                ' hide the finish button when not on the attach doc step
                If currentStep <> STEP_FIN_YR_QTR Then .FinishButton.Visible = False

                .InitControl()
            End With

        End Sub

    End Class
End Namespace
