Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports Target.SP.Web.Apps.UserControls.SelectorWizardSteps

Namespace Apps.ListSubsidies

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.ListSubsidies.ListSubsidies
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Wizard to allows users to view subsidy information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      18/12/2006  Changed New Enquiry button to default dateFrom to today.
    ''' 	[Mikevo]	15/11/2006	Changed SelectorWizard ID to allow JS scripts to function.
    '''                             Not the nicest solution but will do for now.
    '''     [Mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ListSubsidies
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim servStep As ServiceStep = New ServiceStep
            Dim strStyle As New StringBuilder

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPSubsidyList"), "Subsidies")

            strStyle.Append("label.label { float:left; width:8em; padding-right:1em; font-weight: bold; }")
            strStyle.Append(".chkBoxStyle { float:left; width:6%; }")
            Me.AddExtraCssStyle(strStyle.ToString)

            'Service Step is Mandatory
            servStep.Required = True

            With SelectorWizard1
                .Steps.Add(New ProviderStep)
                .Steps.Add(servStep)
                .Steps.Add(New ClientStep)
                .Steps.Add(New SubsidyFilterStep)
                .Steps.Add(New SubsidyEnquiryResultsStep)
                .InitControl()
                .NewButton.Attributes("onclick") = "document.location.href='ListSubsidies.aspx?dateFrom=';"
            End With
        End Sub

    End Class

End Namespace
