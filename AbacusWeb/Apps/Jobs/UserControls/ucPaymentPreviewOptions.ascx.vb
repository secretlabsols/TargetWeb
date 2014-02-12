Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Web.Apps
Imports Target.Library.Web

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control which provides the ability to control whether payments are generated or previewed. 
    ''' </summary>
    ''' <history>
    ''' Colin D   D12436E Updated 09/05/2013 - Added ability to control if Report or Generate options are available i.e. set the options as read only if required.
    ''' Mo Tahir  D12104  Created 30/06/2011
    ''' </history>
    Partial Public Class ucPaymentPreviewOptions
        Inherits System.Web.UI.UserControl

#Region "Enums"

        ''' <summary>
        ''' Represents the current mode of this control
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum GenerateMode
            ''' <summary>
            ''' ReportOnly selected
            ''' </summary>
            ''' <remarks></remarks>
            ReportOnlySelected = 1
            ''' <summary>
            ''' GeneratePayments selected
            ''' </summary>
            ''' <remarks></remarks>
            GeneratePaymentsSelected = 2
            ''' <summary>
            ''' No options selected
            ''' </summary>
            ''' <remarks></remarks>
            NoOptionsSelected = 3
        End Enum

#End Region

#Region "Fields"

        ' Constants
        Private Const _JavascriptPath As String = "AbacusWeb/Apps/Jobs/UserControls/ucPaymentPreviewOptions.js"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            SetupJavascript()

            If Not IsPostBack Then
                ' if first time hit this uc then set defaults

                SetupDefaults()

            End If

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

            Dim startupJavascript As String

            ' create a script tht sets ids of controls for this user control
            startupJavascript = String.Format("rdoReportOnly=GetElement('{0}');" _
                                              & "rdoGeneratePayments=GetElement('{1}');" _
                                              & "allowGenerate={2};" _
                                              & "allowReport={3};" _
                                               , _
                                              optReportOnly.ClientID, _
                                              optGeneratePayments.ClientID,
                                              Utils.GetBooleanAsJavascriptString(AllowGenerate), _
                                              Utils.GetBooleanAsJavascriptString(AllowReport))

            ' register startup script for this user control
            BaseWebPage.ClientScript.RegisterStartupScript(BaseWebPage.GetType(), _
                                                           "ucPaymentPreviewOptions.Init", _
                                                           Utils.WrapClientScript(startupJavascript))

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets a value indicating whether [allow generate].
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if [allow generate]; otherwise, <c>false</c>.
        ''' </value>
        Public Property AllowGenerate As Boolean

        ''' <summary>
        ''' Gets or sets a value indicating whether [allow report].
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if [allow report]; otherwise, <c>false</c>.
        ''' </value>
        Public Property AllowReport As Boolean

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
        ''' Gets or sets the create job generate mode.
        ''' </summary>
        ''' <value>The create job generate mode. maintained in hidGenerateMode</value>
        Public Property CreateJobGenerateMode() As GenerateMode
            Get
                Return CType(Integer.Parse(hidGenerateMode.Value), GenerateMode)
            End Get
            Set(ByVal value As GenerateMode)
                hidGenerateMode.Value = CType(value, Integer).ToString()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether to [generate payments].
        ''' </summary>
        ''' <value><c>true</c> if [generate payments]; otherwise, <c>false</c>.</value>
        Public Property GeneratePayments() As Boolean
            Get
                Return optGeneratePayments.Checked
            End Get
            Set(ByVal value As Boolean)

                ' reset checked status of both radios
                optReportOnly.Checked = False
                optGeneratePayments.Checked = False

                If value Then
                    ' if true then check generate payments

                    optGeneratePayments.Checked = True

                Else
                    ' else false so check report only

                    optReportOnly.Checked = True

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
        ''' Setups the defaults for job options.
        ''' </summary>
        Private Sub SetupDefaults()

            CreateJobGenerateMode = GenerateMode.NoOptionsSelected

        End Sub

        ''' <summary>
        ''' Setups the javascript.
        ''' </summary>
        Private Sub SetupJavascript()

            ' register javascript file for this user control
            BaseWebPage.JsLinks.Add(JavascriptPath)

        End Sub
        ''' <summary>
        ''' Server Side cutom validator.
        ''' </summary>
        Protected Sub CustomValidatorPaymentPreviewOptions_ServerValidate(ByVal source As Object, ByVal args As ServerValidateEventArgs)
            args.IsValid = optReportOnly.Checked OrElse optGeneratePayments.Checked
        End Sub

#End Region

    End Class

End Namespace

