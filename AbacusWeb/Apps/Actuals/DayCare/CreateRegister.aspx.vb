Imports System.Collections.Generic
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.ServiceRegisters
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Library
Imports System.Text
Imports Target.Abacus.Library.DataClasses.Collections
Imports System.Web.Script.Serialization
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DomProviderInvoice

Namespace Apps.Actuals.DayCare

    ''' <summary>
    ''' Web Form for Creating Service Registers
    ''' </summary>
    ''' <history>
    ''' JohnF    15/06/2012 Allow creation of future-dated registers (#7400)
    ''' ColinD   09/08/2011 D11965 - Updated - Altered screen to use new Jquery Date Selector which only allows week ending dates to be selected and, where applicable, prevent selection of future dates.
    ''' ColinD   05/07/2011 D11240 - Created
    ''' </history>
    Partial Public Class CreateRegister
        Inherits BasePage

#Region "Fields"

        ' constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _JsIdentifier As String = "Startup"
        Private Const _PageTitle As String = "Create Service Register"
        Private Const _QsBackUrl As String = "backUrl"
        Private Const _QsEstablishmentID As String = "estabID"
        Private Const _QsContractID As String = "contractID"
        Private Const _QsWeekEndingDate As String = "wedate"
        Private Const _WebCmdAddNewKey As String = "AbacusIntranet.WebNavMenuItemCommand.DayCare.AddNew"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.DayCare"

        ' locals 
        Private _CreateButton As HtmlInputButton = Nothing

#End Region

#Region "Properties"

#Region "Authorisation Properties"

        ''' <summary>
        ''' Gets a value indicating whether [user has add command].
        ''' </summary>
        ''' <value><c>true</c> if [user has add command]; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasAddCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(WebUtils.ConstantsManager.GetConstant(_WebCmdAddNewKey))
            End Get
        End Property

#End Region

#Region "QueryString Properties"

        ''' <summary>
        ''' Gets the back URL.
        ''' </summary>
        ''' <value>The back URL.</value>
        Private ReadOnly Property BackUrl() As String
            Get
                Return HttpUtility.UrlEncode(Utils.ToString(Request.QueryString(_QsBackUrl)))
            End Get
        End Property

        ''' <summary>
        ''' Gets the contract ID.
        ''' </summary>
        ''' <value>The contract ID.</value>
        Private ReadOnly Property ContractID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString(_QsContractID))
            End Get
        End Property

        ''' <summary>
        ''' Gets the establishment ID.
        ''' </summary>
        ''' <value>The establishment ID.</value>
        Private ReadOnly Property EstablishmentID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString(_QsEstablishmentID))
            End Get
        End Property

        ''' <summary>
        ''' Gets the week ending date.
        ''' </summary>
        ''' <value>The week ending date.</value>
        Private ReadOnly Property WeekEndingDate() As Nullable(Of DateTime)
            Get
                Return Utils.ToDateTime(Request.QueryString(_QsWeekEndingDate))
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Gets the create button.
        ''' </summary>
        ''' <value>The create button.</value>
        Private ReadOnly Property CreateButton() As HtmlInputButton
            Get
                If _CreateButton Is Nothing Then
                    ' setup the create button
                    _CreateButton = New HtmlInputButton()
                    With _CreateButton
                        .Value = "Create"
                        .Style.Add("padding-right", "1em")
                        .Style.Add("padding-left", "1em")
                        .ValidationGroup = "Save"
                        .Attributes.Add("onclick", "CreateServiceRegister();")
                        .Disabled = Not UserHasAddCommand
                        .CausesValidation = True
                        .Attributes.Add("title", "Create new Service Register?")
                    End With
                End If
                Return _CreateButton
            End Get
        End Property

        ''' <summary>
        ''' Gets the standard buttons control.
        ''' </summary>
        ''' <value>The standard buttons control.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            AddHandler StandardButtonsControl.AddCustomControls, AddressOf StdButtons_AddCustomControls

        End Sub

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), _PageTitle)

            ' setup the standard buttons control
            With StandardButtonsControl
                .AllowBack = True
                .AllowNew = False
                .AllowEdit = False
                .AllowDelete = False
                .AllowFind = False
                .InitialMode = StdButtonsMode.Fetched
            End With

            If Not IsPostBack Then
                ' setup the selectors with data from query string

                SetupContractSelector()
                SetupEstablishmentSelector()
                SetupWeekEndingDateSelector()

            End If

            ' setup js/reports
            SetupJavaScript()

        End Sub

#End Region

#Region "Functions/Methods"

        ''' <summary>
        ''' Set up the contract selector.
        ''' </summary>
        Private Sub SetupContractSelector()

            With CType(domContract, InPlaceSelectors.InPlaceDomContractSelector)
                .ValidationGroup = "Save"
                .Required = True
                If ContractID > 0 AndAlso EstablishmentID > 0 Then
                    .ContractID = ContractID
                End If
                If EstablishmentID > 0 Then
                    .Enabled = True
                Else
                    .Enabled = False
                End If
                .FrameworkTypeID = FrameworkTypes.ServiceRegister
            End With

        End Sub

        ''' <summary>
        ''' Set up the establishment selector.
        ''' </summary>
        Private Sub SetupEstablishmentSelector()

            With CType(provider, InPlaceSelectors.InPlaceEstablishmentSelector)
                .ValidationGroup = "Save"
                .Required = True
                If EstablishmentID > 0 Then
                    .EstablishmentID = EstablishmentID
                Else
                    .EstablishmentID = 0
                End If
            End With

        End Sub

        ''' <summary>
        ''' Setups the week ending date selector.
        ''' </summary>
        Private Sub SetupWeekEndingDateSelector()

            If WeekEndingDate.HasValue() Then

                dteDatesEffectiveDate.Text = WeekEndingDate.Value.ToString(_DateFormat)

            Else

                dteDatesEffectiveDate.Text = ""

            End If

            SetupWeekEndingDateSelectorValidation()

        End Sub

        ''' <summary>
        ''' Setups the week ending date selector validation.
        ''' </summary>
        Private Sub SetupWeekEndingDateSelectorValidation()

            Dim msg As New ErrorMessage()
            Dim weekEndingDate As DateTime = DomContractBL.GetWeekEndingDate(CType(Me.Page, BasePage).DbConnection, Nothing)

            With dteDatesEffectiveDate
                ' set the allowable day of the week to select based on the weekending date
                .AllowableDays = Integer.Parse(weekEndingDate.DayOfWeek)

                .MaximumValue = DateTime.MaxValue.ToString(_DateFormat)
                .MinimumValue = DateTime.MinValue.ToString(_DateFormat)
            End With

        End Sub

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            ' add in js link for handlers
            JsLinks.Add("CreateRegister.js")

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog js
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add in the jquery library
            UseJQuery = True
            UseJqueryUI = True
            UseJqueryTextboxClearer = True

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.ServiceRegisters))

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), _JsIdentifier) Then

                Dim js As New StringBuilder()

                js.AppendFormat("contractSelectorClientID = '{0}';", domContract.ClientID)
                js.AppendFormat("selectedContractID = {0};", ContractID)
                js.AppendFormat("selectedEstablishmentID = {0};", EstablishmentID)
                js.AppendFormat("weekEndingClientID = '{0}_txtTextBox';", dteDatesEffectiveDate.ClientID)
                js.AppendFormat("backUrl = '{0}';", BackUrl)

                Page.ClientScript.RegisterStartupScript(Me.GetType(), _JsIdentifier, js.ToString(), True)

            End If

        End Sub

        ''' <summary>
        ''' Adds custom controls to the standard buttons control
        ''' </summary>
        ''' <param name="controls">The controls.</param>
        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            controls.Add(CreateButton)

        End Sub

#End Region

    End Class

End Namespace
