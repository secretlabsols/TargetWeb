
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.SP.Web.Apps.UserControls.SelectorWizardSteps

Namespace Apps.Payments

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.Payments.PaymentEnquiry
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Wizard screen that allows users to view payment information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      20/10/2006  Moved code to create steps to Page_Init().
    ''' 	[Mikevo]	05/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class PaymentEnquiry
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPPaymentsEnquiry"), "Payment Enquiry")

            Dim dateStep As DateRangeStep = New DateRangeStep
            dateStep.Description = "Please enter a date range to filter the results on. The dates entered will be applied to the period covered by the remittance."

            With SelectorWizard1
                .Steps.Add(New ProviderStep)
                .Steps.Add(New ServiceStep)
                .Steps.Add(dateStep)
                .Steps.Add(New PaymentEnquiryResultsStep)
                .InitControl()
            End With


        End Sub

    End Class

End Namespace

