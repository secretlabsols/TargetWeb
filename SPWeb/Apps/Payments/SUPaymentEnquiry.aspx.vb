
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.SP.Web.Apps.UserControls.SelectorWizardSteps

Namespace Apps.Payments

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.Payments.SUPaymentEnquiry
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Wizard screen that allows users to view payment information for a specific service user.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      09/03/2007  Changes to support service user statement.
    '''     MikeVO      05/03/2007  Changed date range step description.
    '''     MikeVO      20/10/2006  Moved code to create steps to Page_Init().
    ''' 	[Mikevo]	10/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class SUPaymentEnquiry
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPPaymentsSUPaymentEnquiry"), "Service User Payment Enquiry")

            Dim svcStep As ServiceStep = New ServiceStep
            svcStep.Required = True

            Dim suStep As ClientStep = New ClientStep
            suStep.Required = True

            Dim dateStep As DateRangeStep = New DateRangeStep
            dateStep.Description = "Please enter a date range to filter the results on. The dates entered will be applied to the period covered by the remittances that contain the relevant payment lines."

            With SelectorWizard1
                .Steps.Add(New ProviderStep)
                .Steps.Add(svcStep)
                .Steps.Add(suStep)
                .Steps.Add(dateStep)
                .Steps.Add(New SUPaymentEnquiryResultsStep)
                .InitControl()
            End With

        End Sub

    End Class

End Namespace

