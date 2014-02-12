Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Web.Apps
Imports Target.Library.Web

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control which provides the ability to define when a job should be
    ''' executed i.e. execute now or defer until a later date. Control has various display methods
    ''' defined in the enumeration <see cref="ucCreateJobPointInTime.DisplayMode" />.
    ''' </summary>
    ''' <history>
    ''' Colin Daly   D11802 Created 02/08/2010
    ''' </history>
    Partial Public Class ucCreateJobPointInTime
        Inherits System.Web.UI.UserControl

#Region "Enums"

        ''' <summary>
        ''' Represents the display mode of this control
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum DisplayMode
            ''' <summary>
            ''' Disables the create later functionality on click
            ''' </summary>
            ''' <remarks></remarks>
            DisableCreateLaterOnClick = 1
            ''' <summary>
            ''' Hides the create later functionality on click
            ''' </summary>
            ''' <remarks></remarks>
            HideCreateLaterOnClick = 2
        End Enum

#End Region

#Region "Fields"

        ' Constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _DateTooltip As String = "The date must be between {0} and {1}."
        Private Const _DateValidationError As String = "The date must be between {0} and {1}"
        Private Const _JavascriptPath As String = "AbacusWeb/Apps/Jobs/UserControls/ucCreateJobPointInTime.js"
        Private Const _TimeFormat As String = "HH:mm:ss"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            SetupDateValidation()
            SetupJavascript()

            If Not IsPostBack Then
                ' if first time hit this uc then set defaults

                SetupDefaults()

            End If

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the base web page.
        ''' </summary>
        ''' <value>The base web page.</value>
        Private ReadOnly Property BaseWebPage() As BasePage
            Get
                Return CType(Me.Page, BasePage)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the create job display mode.
        ''' </summary>
        ''' <value>The create job display mode.</value>
        Public Property CreateJobDisplayMode() As DisplayMode
            Get
                Return CType(Integer.Parse(hidDisplayMode.Value), DisplayMode)
            End Get
            Set(ByVal value As DisplayMode)
                hidDisplayMode.Value = CType(value, Integer).ToString()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the create job date time. Setting this value will
        ''' automatically set the flag CreateJobNow to true.
        ''' </summary>
        ''' <value>The create job date time.</value>
        Public Property CreateJobDateTime() As DateTime
            Get

                If CreateJobNow Then
                    ' if we create the job now then return 
                    ' current date and time

                    Return DateTime.Now()

                Else
                    ' else return date and time from fields

                    Dim tmpDate As DateTime = DateTime.Now

                    If DateTime.TryParse(String.Format("{0} {1}", dteStartDate.Text, tmeStartDate.Text), tmpDate) Then
                        ' if we can cast date and time selected to a datetime

                        Return tmpDate

                    Else
                        ' else return now

                        Return DateTime.Now

                    End If

                End If

            End Get
            Set(ByVal value As DateTime)

                ' always set the CreateJobNow flag to false if calls this setter
                CreateJobNow = False
                dteStartDate.Text = value.ToString(_DateFormat)
                tmeStartDate.Text = value.ToString(_TimeFormat)

            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether to [create job now].
        ''' </summary>
        ''' <value><c>true</c> if [create job now]; otherwise, <c>false</c>.</value>
        Public Property CreateJobNow() As Boolean
            Get
                Return optCreateJobNow.Checked
            End Get
            Set(ByVal value As Boolean)

                ' reset checked status of both radios
                optCreateJobNow.Checked = False
                optCreateJobLater.Checked = False

                If value Then
                    ' if true then check now

                    optCreateJobNow.Checked = True

                Else
                    ' else false so check later

                    optCreateJobLater.Checked = True

                End If

            End Set
        End Property

        ''' <summary>
        ''' Gets the javascript path for this user control.
        ''' </summary>
        ''' <value>The javascript path.</value>
        Private ReadOnly Property JavascriptPath() As String
            Get
                Return Target.Library.Web.Utils.GetVirtualPath(_JavascriptPath)
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Setups the defaults for job creation.
        ''' </summary>
        Private Sub SetupDefaults()

            CreateJobDateTime = DateTime.Now
            CreateJobNow = True
            CreateJobDisplayMode = DisplayMode.HideCreateLaterOnClick

        End Sub

        ''' <summary>
        ''' Setups the date validation for job creation time.
        ''' </summary>
        Private Sub SetupDateValidation()

            Dim maxDate As DateTime = DateTime.MaxValue
            Dim minDate As DateTime = DateTime.Now

            dteStartDate.TextBox.ToolTip = String.Format(_DateTooltip, _
                                                       minDate.ToString(_DateFormat), _
                                                       maxDate.ToString(_DateFormat))

            ' set up validation of the start date so its today or later
            With valDates
                .MinimumValue = minDate.ToString(_DateFormat)
                .MaximumValue = maxDate.ToString(_DateFormat)
                .ErrorMessage = String.Format(_DateValidationError, _
                                              .MinimumValue, _
                                              .MaximumValue)
                .SetFocusOnError = True
                .Type = ValidationDataType.Date
            End With

        End Sub

        ''' <summary>
        ''' Setups the javascript.
        ''' </summary>
        Private Sub SetupJavascript()

            Dim startupJavascript As String

            ' create a script tht sets ids of controls for this user control
            startupJavascript = String.Format("rdoCreateNow=GetElement('{0}');" _
                                              & "rdoCreateLater=GetElement('{1}');" _
                                              & "divCreateJobLater=GetElement('{2}');" _
                                              & "hidDisplayMode=GetElement('{3}');" _
                                               , _
                                              optCreateJobNow.ClientID, _
                                              optCreateJobLater.ClientID, _
                                              divCreateJobLater.ClientID, _
                                              hidDisplayMode.ClientID)

            ' register javascript file for this user control
            BaseWebPage.JsLinks.Add(JavascriptPath)

            ' register startup script for this user control
            BaseWebPage.ClientScript.RegisterStartupScript(BaseWebPage.GetType(), _
                                                           "ucCreateJobPointInTime.Init", _
                                                           Utils.WrapClientScript(startupJavascript))

        End Sub

#End Region
        
    End Class

End Namespace

