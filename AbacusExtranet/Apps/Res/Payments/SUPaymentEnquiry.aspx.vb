
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.Res.Payments

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.Res.Payments.SUPaymentEnquiry
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Wizard screen that allows users to view payment information for an individual 
    '''     service user.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	12/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class SUPaymentEnquiry
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ServiceUserPaymentEnquiry"), "Residential Service User Payment Enquiry")

            Dim estabStep As EstablishmentStep = New EstablishmentStep
            Dim suStep As ClientStep = New ClientStep
            Dim dateStep As DateRangeStep = New DateRangeStep

            estabStep.Description = "Please select a residential home from the list below and then click ""Next""."
            estabStep.Mode = EstablishmentStep.EstablishmentStepMode.ResidentialHomes
            estabStep.Required = True

            suStep.Required = True
            suStep.Mode = UserControls.ClientStepMode.ResidentialClients

            dateStep.Description = "Please enter a date range to filter the results on. The dates entered will be applied to the Paid Date of the payment."
            dateStep.HeaderLabelWidth = New Unit(8, UnitType.Em)
            dateStep.UseJquery = True

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(suStep)
                .Steps.Add(dateStep)
                .Steps.Add(New ResSUPaymentEnquiryResultsStep)
                .InitControl()
            End With


            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in the table filter library 
            UseJqueryTableFilter = True

            ' add the table scroller library as we might have large amounts of data
            UseJqueryTableScroller = True

            ' add the searchable menu
            UseJquerySearchableMenu = True

            ' add the jquery tooltip
            UseJqueryTooltip = True

            UseJqueryTemplates = True


        End Sub


    End Class

End Namespace