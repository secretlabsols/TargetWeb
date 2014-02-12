
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.SP.Web.Apps.UserControls.SelectorWizardSteps

Namespace Apps.Payments

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.Payments.PaymentInterface
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Wizard screen that allows users to view provider payment interface files.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      20/10/2006  Moved code to create steps to Page_Init().
    ''' 	[Mikevo]	11/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class PaymentInterface
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPPaymentsInterface"), "Payment Interface")

            Dim provStep As ProviderStep = New ProviderStep

            provStep.Required = True

            With SelectorWizard1
                .Steps.Add(provStep)
                .Steps.Add(New DateRangeStep)
                .Steps.Add(New ProviderPaymentsResultsStep)
                .InitControl()
            End With

        End Sub

    End Class

End Namespace

