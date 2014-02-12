
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.CreditorPayments
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections

Namespace Apps.UserControls.SelectorWizardSteps

    ''' <summary>
    ''' Class used to represent a wizard step for selecting generic creditors
    ''' </summary>
    ''' <history>
    ''' MikeVO      A4WA#7014 09/09/2011 Corrected rendering of header controls.
    ''' ColinDaly   D11874 Created 10/02/2010
    ''' </history>
    Friend Class GenericCreditorStep
        Implements ISelectorWizardStep

#Region "Fields"

        ' locals
        Private _baseUrl As String = String.Empty
        Private _currentStep As Integer
        Private _dbConnection As SqlConnection = Nothing
        Private _description As String = "Please select a creditor and then click ""Next""."
        Private _GenericCreditorId As Integer = 0
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _isCurrentStep As Boolean = False
        Private _queryString As NameValueCollection = Nothing
        Private _required As Boolean = False
        Private _stepIndex As Integer

        ' constants
        Private Const QsGenericCreditorIdKey As String = Target.Abacus.Library.CreditorPayments.WizardScreenParameters.QsGenericCreditorID
        Private Const QsCurrentStep As String = "currentStep"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or set the base portion of the current Url, upto the "?" character
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the Javascript to execute before navigating forwards from the step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "GenericCreditorSelector_BeforeNavigate()"
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the database connection to use.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Property DbConnection() As System.Data.SqlClient.SqlConnection Implements ISelectorWizardStep.DbConnection
            Get
                Return _dbConnection
            End Get
            Set(ByVal Value As System.Data.SqlClient.SqlConnection)
                _dbConnection = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the description text to display for the step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Property Description() As String Implements ISelectorWizardStep.Description
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the width of the header label.
        ''' </summary>
        ''' <value>The width of the header label.</value>
        Public Property HeaderLabelWidth() As Unit
            Get
                Return _headerLabelWidth
            End Get
            Set(ByVal value As Unit)
                _headerLabelWidth = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if the step is the current step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Property IsCurrentStep() As Boolean Implements ISelectorWizardStep.IsCurrentStep
            Get
                Return _isCurrentStep
            End Get
            Set(ByVal Value As Boolean)
                _isCurrentStep = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the current querystring values for the step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _currentStep = Target.Library.Utils.ToInt32(_queryString(QsCurrentStep))
                _GenericCreditorId = Target.Library.Utils.ToInt32(_queryString(QsGenericCreditorIdKey))
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if the step must be cimpleted before moving onto the next step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets where in the series of steps the step appears.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Property StepIndex() As Integer Implements ISelectorWizardStep.StepIndex
            Get
                Return _stepIndex
            End Get
            Set(ByVal Value As Integer)
                _stepIndex = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the title text to display for the step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public ReadOnly Property Title() As String Implements ISelectorWizardStep.Title
            Get
                Return "Select a Creditor"
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Adds a header control line.
        ''' </summary>
        ''' <param name="controls">The controls.</param>
        ''' <param name="keyText">The key text.</param>
        ''' <param name="valueText">The value text.</param>
        ''' <param name="changeLinkText">The change link text.</param>
        ''' <param name="changeLinkUrl">The change link URL.</param>
        Public Sub AddHeaderControlLine(ByRef controls As System.Web.UI.ControlCollection, _
                                       ByVal keyText As String, _
                                       ByVal valueText As String, _
                                       ByVal changeLinkText As String, _
                                       ByVal changeLinkUrl As String)

            Dim keyLabel As New Label()
            Dim valueLabel As New Label()
            Dim changeLink As New HyperLink()
            Dim container As New HtmlGenericControl("div")

            With container
                .Style.Add("width", "100%")
            End With

            With keyLabel
                .Text = keyText
                .Style.Add("float", "left")
                .Style.Add("font-weight", "bold")
                .Style.Add("width", _headerLabelWidth.ToString())
                .Style.Add("display", "inline")
                container.Controls.Add(keyLabel)
            End With

            With valueLabel
                .Text = valueText
                .Style.Add("float", "left")
                .Style.Add("display", "inline")
                container.Controls.Add(valueLabel)
            End With

            With changeLink
                .Text = changeLinkText
                .NavigateUrl = changeLinkUrl
                .Style.Add("float", "right")
                .Style.Add("display", "inline")
                .Style.Add("clear", "right")
                container.Controls.Add(changeLink)
            End With

            controls.Add(container)

        End Sub

        ''' <summary>
        ''' Initialises the step.
        ''' </summary>
        ''' <param name="wizard"></param>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        ''' <summary>
        ''' Called during the page PreRender event.
        ''' Can be used to output step specific javascript.
        ''' </summary>
        ''' <param name="wizard">A SelectorWizard instance.</param>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        ''' <summary>
        ''' Renders the header controls relevant to this step.
        ''' </summary>
        ''' <param name="controls">The control collection to add the new controls to.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage

            Try

                Dim genericCreditorKeyText As String = String.Empty
                Dim genericCreditorValueText As String = " "
                Dim genericCreditorLinkText As String = String.Empty
                Dim genericCreditorLinkUrl As String = String.Empty
                Dim genericCreditorQs As NameValueCollection = Me.QueryString

                ' setup the key text
                genericCreditorKeyText = "Creditor"

                If _GenericCreditorId > 0 Then
                    ' we have a creditor so fetch info from db

                    Dim creditor As GenericCreditor = Nothing
                    Dim mruManager As New MostRecentlyUsedManager(HttpContext.Current)
                    Dim wizardParams As New WizardScreenParameters(DbConnection, genericCreditorQs)

                    creditor = wizardParams.GenericCreditor
                    genericCreditorValueText = wizardParams.GenericCreditorReference

                    ' add the creditor into the mru list
                    mruManager("GENERIC_CREDITORS")(creditor.ID) = genericCreditorValueText
                    mruManager.Save(HttpContext.Current)

                Else
                    ' we have no creditor 

                    genericCreditorValueText = "All"

                End If

                If Me.IsCurrentStep Then
                    ' if the current step

                    If _GenericCreditorId <= 0 Then
                        ' if we have no creditor then display nowt

                        genericCreditorLinkText = ""

                    Else

                        genericCreditorLinkText = "All Creditors"
                        genericCreditorQs(QsGenericCreditorIdKey) = ""

                    End If

                    genericCreditorLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(genericCreditorQs))

                ElseIf _currentStep > Me.StepIndex Then
                    ' else we are on another step 

                    genericCreditorLinkText = "Change Creditor"
                    genericCreditorQs.Remove(QsCurrentStep)
                    genericCreditorQs.Add(QsCurrentStep, Me.StepIndex)
                    genericCreditorLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(genericCreditorQs))

                End If

                ' add collection of contributions link
                AddHeaderControlLine(controls, genericCreditorKeyText, genericCreditorValueText, genericCreditorLinkText, genericCreditorLinkUrl)

                ' setup success msg
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                ' catch the excpetion and wrap it

                msg = Target.Library.Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            End Try

            Return msg

        End Function

        ''' <summary>
        ''' Renders the content controls relevant for the step.
        ''' </summary>
        ''' <param name="wizard">A reference to the wizard user control itself.</param>
        ''' <param name="controls">The control collection to add the new controls to.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As New ErrorMessage()
            Dim filterControl As GenericCreditorSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/GenericCreditorSelector.ascx")

            ' setup the filter control
            With filterControl
                .FilterSelectedID = _GenericCreditorId
                .InitControl(wizard.Page, True)
            End With

            ' add the control to be used
            controls.Add(filterControl)

            msg = New ErrorMessage()
            msg.Success = True

            Return msg

        End Function

#End Region

    End Class

End Namespace