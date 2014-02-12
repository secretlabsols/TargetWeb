Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.Res

    ''' <summary>
    ''' Screen to allow a user to enquire upon residential home occupancy.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class OccupancyEnquiry
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.OccupancyEnquiry"), "Residential Occupancy Enquiry")

            IntialiseJsVariables()

            Dim estabStep As EstablishmentStep = New EstablishmentStep
            Dim dateStep As DateRangeMovementStep = New DateRangeMovementStep
            Dim strStyle As New StringBuilder


            strStyle.Append("label.label { width:7em; padding-right:1em; font-weight: bold; }")
            strStyle.Append(".chkBoxStyle { padding-right:1em; font-weight: bold;}")
            strStyle.Append(".chkBoxStyle input, label { white-space:nowrap; float:left; }")

            Me.AddExtraCssStyle(strStyle.ToString)

            estabStep.Description = "Please select a residential home from the list below and then click ""Next""."
            estabStep.Mode = EstablishmentStep.EstablishmentStepMode.ResidentialHomes
            estabStep.Required = True

            dateStep.Description = "Please enter a date range and movement type to filter the results on."
            dateStep.Required = True
            dateStep.HeaderLabelWidth = New Unit(8, UnitType.Em)

            With SelectorWizard1
                .Steps.Add(estabStep)
                .Steps.Add(dateStep)
                .Steps.Add(New ResOccupancyEnquiryResultsStep)
                .InitControl()
            End With

        End Sub

        Private Sub IntialiseJsVariables()


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

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))


        End Sub
    End Class
End Namespace
