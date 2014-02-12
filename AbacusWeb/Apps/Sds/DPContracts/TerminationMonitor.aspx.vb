Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web

Namespace Apps.Sds.DPContracts

    ''' <summary>
    ''' User Interface for monitoring Direct Payment Contract that have been terminated
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD  08/09/2011   Created
    ''' </history>
    Partial Class TerminationMonitor
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        ' constants
        Private Const _PageTitle As String = "Direct Payments Monitor"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.CreditorPayments.DirectPaymentsMonitor"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the selector wizard control.
        ''' </summary>
        ''' <value>The selector wizard control.</value>
        Private ReadOnly Property SelectorWizardControl() As SelectorWizardBase
            Get
                Return CType(SelectorWizard1, SelectorWizardBase)
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), _PageTitle)

            ' setup javascript
            SetupJavaScript()

            ' setup steps
            SetupDpContractTerminationMonitorFilterStep()
            SetupDpContractTerminationMonitorResultsStep()

            ' init the selector control init
            SelectorWizardControl.InitControl()

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Setup the dp contract termination monitor filter step.
        ''' </summary>
        Private Sub SetupDpContractTerminationMonitorFilterStep()

            SelectorWizardControl.Steps.Add(New DpContractTerminationMonitorFilterStep())

        End Sub

        ''' <summary>
        ''' Setups the dp contract termination monitor results step.
        ''' </summary>
        Private Sub SetupDpContractTerminationMonitorResultsStep()

            SelectorWizardControl.Steps.Add(New DpContractTerminationMonitorResultsStep())


        End Sub

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            UseJQuery = True
            UseJqueryUI = True
            UseJqueryTextboxClearer = True

        End Sub

#End Region

    End Class

End Namespace