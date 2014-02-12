Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Abacus.Library.CreditorPayments
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.GCPaymentSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of SDS Contribution Levels.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     ColinDaly 01/11/2011 D12229 - Updated - Added FilterPaymentSuspendedPaymentStatusTypeOptions property.
    '''     ColinDaly 10/02/2010 D11874 - Complete overhaul for use as part of the new Generic Creditors Interface UI
    '''     [MoTahir] 02/09/2010 D11814
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class GenericCreditorPaymentSelector
        Inherits System.Web.UI.UserControl

#Region "Enums"

        ''' <summary>
        ''' Enumeration representing the mode in which this control opens other windows
        ''' </summary>
        Public Enum WindowOpenModes
            ''' <summary>
            ''' Redirects the browser to the other window
            ''' </summary>
            Redirect = 0
            ''' <summary>
            ''' Opens other windows in a popup window
            ''' </summary>
            Popup = 1
        End Enum

#End Region

#Region "Fields"

        Private _CurrentUser As WebSecurityUser = Nothing
        Private _FilterDateFrom As Nullable(Of DateTime) = Nothing
        Private _FilterDateTo As Nullable(Of DateTime) = Nothing
        Private _FilterGenericCreditorID As Integer = 0
        Private _FilterGenericContractID As Integer = 0
        Private _FilterIncludeDirectPayment As Boolean = True
        Private _FilterIncludeNonResidential As Boolean = True
        Private _FilterPaymentNumber As String = String.Empty
        Private _FilterPaymentStatusDateFrom As Nullable(Of DateTime) = Nothing
        Private _FilterPaymentStatusDateTo As Nullable(Of DateTime)
        Private _FilterPaymentStatusIncludeAuthorised As Boolean = True
        Private _FilterPaymentStatusIncludeExcludedFromCreditors As Nullable(Of Boolean) = Nothing
        Private _FilterPaymentStatusIncludePaid As Boolean = True
        Private _FilterPaymentStatusIncludeSuspended As Boolean = True
        Private _FilterPaymentStatusIncludeUnpaid As Boolean = True
        Private _FilterSelectedID As Integer = 0
        Private _FilterServiceUserID As Integer = 0
        Private _FilterNonResidentialSuspensionReason As CreditorPaymentsBL.GenericCreditorNonResidentialAdditionalFilter = CreditorPaymentsBL.GenericCreditorNonResidentialAdditionalFilter.NoAdditionalFiltering
        Private _FilterPaymentSuspendedPaymentStatusTypeOptions As WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions = WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Automatically Or WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Manually
        Private _ShowAuthoriseButton As Nullable(Of Boolean) = Nothing
        Private _ShowButtons As Boolean = True
        Private _ShowCreateBatchButton As Nullable(Of Boolean) = Nothing
        Private _ShowExcludeIncludeButton As Nullable(Of Boolean) = Nothing
        Private _ShowNewButton As Nullable(Of Boolean) = Nothing
        Private _ShowNotesButton As Nullable(Of Boolean) = Nothing
        Private _ShowSuspensionsButton As Nullable(Of Boolean) = Nothing
        Private _ShowServiceUserColumn As Boolean = False
        Private _WindowOpenMode As WindowOpenModes = WindowOpenModes.Redirect

        ' constants
        Private Const _JsLibrary As String = "Library/Javascript/"
        Private Const _SelectorName As String = "GenericCreditorPaymentSelector"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the care type selector control.
        ''' </summary>
        ''' <value>The care type selector control.</value>
        Private ReadOnly Property CareTypeSelectorControl() As CareTypeSelector
            Get
                Return CType(cTypeSelector, CareTypeSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the current page.
        ''' </summary>
        ''' <value>The current page.</value>
        Private ReadOnly Property CurrentPage() As Integer
            Get
                Dim page As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
                If page <= 0 Then page = 1
                Return page
            End Get
        End Property

        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>The current user.</value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the date from to filter on.
        ''' </summary>
        ''' <value>The filter date from.</value>
        Public Property FilterDateFrom() As Nullable(Of DateTime)
            Get
                Return _FilterDateFrom
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _FilterDateFrom = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets date to to filter on.
        ''' </summary>
        ''' <value>The filter date to.</value>
        Public Property FilterDateTo() As Nullable(Of DateTime)
            Get
                Return _FilterDateTo
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _FilterDateTo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter generic creditor ID.
        ''' </summary>
        ''' <value>The filter generic creditor ID.</value>
        Public Property FilterGenericCreditorID() As Integer
            Get
                Return _FilterGenericCreditorID
            End Get
            Set(ByVal value As Integer)
                _FilterGenericCreditorID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter generic contract ID.
        ''' </summary>
        ''' <value>The filter generic contract ID.</value>
        Public Property FilterGenericContractID() As Integer
            Get
                Return _FilterGenericContractID
            End Get
            Set(ByVal value As Integer)
                _FilterGenericContractID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [filter include direct payment].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [filter include direct payment]; otherwise, <c>false</c>.
        ''' </value>
        Public Property FilterIncludeDirectPayment() As Boolean
            Get
                Return _FilterIncludeDirectPayment
            End Get
            Set(ByVal value As Boolean)
                _FilterIncludeDirectPayment = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [include non residential].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [include non residential]; otherwise, <c>false</c>.
        ''' </value>
        Public Property FilterIncludeNonResidential() As Boolean
            Get
                Return _FilterIncludeNonResidential
            End Get
            Set(ByVal value As Boolean)
                _FilterIncludeNonResidential = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter payment number.
        ''' </summary>
        ''' <value>The filter payment number.</value>
        Public Property FilterPaymentNumber() As String
            Get
                Return _FilterPaymentNumber
            End Get
            Set(ByVal value As String)
                _FilterPaymentNumber = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter payment status date from.
        ''' </summary>
        ''' <value>The filter payment status date from.</value>
        Public Property FilterPaymentStatusDateFrom() As Nullable(Of DateTime)
            Get
                Return _FilterPaymentStatusDateFrom
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _FilterPaymentStatusDateFrom = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter payment status date to.
        ''' </summary>
        ''' <value>The filter payment status date to.</value>
        Public Property FilterPaymentStatusDateTo() As Nullable(Of DateTime)
            Get
                Return _FilterPaymentStatusDateTo
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _FilterPaymentStatusDateTo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [filter include authorised payment status].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [filter include authorised payment status]; otherwise, <c>false</c>.
        ''' </value>
        Public Property FilterPaymentStatusIncludeAuthorised() As Boolean
            Get
                Return _FilterPaymentStatusIncludeAuthorised
            End Get
            Set(ByVal value As Boolean)
                _FilterPaymentStatusIncludeAuthorised = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter payment status include excluded from creditors.
        ''' </summary>
        ''' <value>The filter payment status include excluded from creditors.</value>
        Public Property FilterPaymentStatusIncludeExcludedFromCreditors() As Nullable(Of Boolean)
            Get
                Return _FilterPaymentStatusIncludeExcludedFromCreditors
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                _FilterPaymentStatusIncludeExcludedFromCreditors = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [filter include paid payment status].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [filter include paid payment status]; otherwise, <c>false</c>.
        ''' </value>
        Public Property FilterPaymentStatusIncludePaid() As Boolean
            Get
                Return _FilterPaymentStatusIncludePaid
            End Get
            Set(ByVal value As Boolean)
                _FilterPaymentStatusIncludePaid = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [filter include suspended payment status].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [filter include suspended payment status]; otherwise, <c>false</c>.
        ''' </value>
        Public Property FilterPaymentStatusIncludeSuspended() As Boolean
            Get
                Return _FilterPaymentStatusIncludeSuspended
            End Get
            Set(ByVal value As Boolean)
                _FilterPaymentStatusIncludeSuspended = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [filter include unpaid payment status].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [filter include unpaid payment status]; otherwise, <c>false</c>.
        ''' </value>
        Public Property FilterPaymentStatusIncludeUnpaid() As Boolean
            Get
                Return _FilterPaymentStatusIncludeUnpaid
            End Get
            Set(ByVal value As Boolean)
                _FilterPaymentStatusIncludeUnpaid = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter selected ID.
        ''' </summary>
        ''' <value>The filter selected ID.</value>
        Public Property FilterSelectedID() As Integer
            Get
                Return _FilterSelectedID
            End Get
            Set(ByVal value As Integer)
                _FilterSelectedID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter service user ID.
        ''' </summary>
        ''' <value>The filter service user ID.</value>
        Public Property FilterServiceUserID() As String
            Get
                Return _FilterServiceUserID
            End Get
            Set(ByVal value As String)
                _FilterServiceUserID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the non residential suspension reason.
        ''' </summary>
        ''' <value>The non residential suspension reason.</value>
        Public Property FilterNonResidentialSuspensionReason() As CreditorPaymentsBL.GenericCreditorNonResidentialAdditionalFilter
            Get
                Return _FilterNonResidentialSuspensionReason
            End Get
            Set(ByVal value As CreditorPaymentsBL.GenericCreditorNonResidentialAdditionalFilter)
                _FilterNonResidentialSuspensionReason = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter payment suspended payment status type options.
        ''' </summary>
        ''' <value>
        ''' The filter payment suspended payment status type options.
        ''' </value>
        Public Property FilterPaymentSuspendedPaymentStatusTypeOptions() As WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions
            Get
                Return _FilterPaymentSuspendedPaymentStatusTypeOptions
            End Get
            Set(ByVal value As WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions)
                _FilterPaymentSuspendedPaymentStatusTypeOptions = value
            End Set
        End Property

        ''' <summary>
        ''' Gets base page.
        ''' </summary>
        ''' <value>Base page.</value>
        Private ReadOnly Property MyBasePage() As BasePage
            Get
                Return CType(Page, BasePage)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the authorise button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the authorise button]; otherwise, <c>false</c>.</value>
        Public Property ShowAuthoriseButton() As Boolean
            Get
                If _ShowAuthoriseButton Is Nothing OrElse _ShowAuthoriseButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowAuthoriseButton = SecurityBL.UserHasMenuItem(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.AuthoriseCreditorPayments"), MyBasePage.Settings.CurrentApplicationID)
                End If
                Return _ShowAuthoriseButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowAuthoriseButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [show buttons].
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if [show buttons]; otherwise, <c>false</c>.
        ''' </value>
        Public Property ShowButtons() As Boolean
            Get
                Return _ShowButtons
            End Get
            Set(ByVal value As Boolean)
                _ShowButtons = value
            End Set
        End Property


        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the create batch button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the create batch button]; otherwise, <c>false</c>.</value>
        Public Property ShowCreateBatchButton() As Boolean
            Get
                If _ShowCreateBatchButton Is Nothing OrElse _ShowCreateBatchButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowCreateBatchButton = SecurityBL.UserHasMenuItemCommand(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.CreditorPaymentBatches.CreatePaymentBatch"), 2)
                End If
                Return _ShowCreateBatchButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowCreateBatchButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the exclude\include button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the exclude\include button]; otherwise, <c>false</c>.</value>
        Public Property ShowExcludeIncludeButton() As Boolean
            Get
                If _ShowExcludeIncludeButton Is Nothing OrElse _ShowExcludeIncludeButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowExcludeIncludeButton = SecurityBL.UserHasMenuItemCommand(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.ExcludeInclude"), 2)
                End If
                Return _ShowExcludeIncludeButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowExcludeIncludeButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [show MRU list].
        ''' </summary>
        ''' <value><c>true</c> if [show MRU list]; otherwise, <c>false</c>.</value>
        Public Property ShowMruList() As Boolean
            Get
                Return mru.Visible
            End Get
            Set(ByVal value As Boolean)
                mru.Visible = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the new button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the new button]; otherwise, <c>false</c>.</value>
        Public Property ShowNewButton() As Boolean
            Get
                If _ShowNewButton Is Nothing OrElse _ShowNewButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowNewButton = SecurityBL.UserHasMenuItemCommand(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.AddNew"), 2)
                End If
                Return _ShowNewButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowNewButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the notes button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the notes button]; otherwise, <c>false</c>.</value>
        Public Property ShowNotesButton() As Boolean
            Get
                If _ShowNotesButton Is Nothing OrElse _ShowNotesButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowNotesButton = SecurityBL.UserHasMenuItem(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.Notes"), MyBasePage.Settings.CurrentApplicationID)
                End If
                Return _ShowNotesButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowNotesButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [to show the service user column].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [to show the service user column]; otherwise, <c>false</c>.
        ''' </value>
        Public Property ShowServiceUserColumn() As Boolean
            Get
                Return _ShowServiceUserColumn
            End Get
            Set(ByVal value As Boolean)
                _ShowServiceUserColumn = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [to show the suspensions button].
        ''' </summary>
        ''' <value><c>true</c> if [to show the suspensions button]; otherwise, <c>false</c>.</value>
        Public Property ShowSuspensionsButton() As Boolean
            Get
                If _ShowSuspensionsButton Is Nothing OrElse _ShowSuspensionsButton.HasValue = False Then
                    ' if we have no value set for this button yet then determine from menu command
                    _ShowSuspensionsButton = SecurityBL.UserHasMenuItem(MyBasePage.DbConnection, CurrentUser.ID, ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.Suspensions"), MyBasePage.Settings.CurrentApplicationID)
                End If
                Return _ShowSuspensionsButton.Value
            End Get
            Set(ByVal value As Boolean)
                _ShowSuspensionsButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the window open mode.
        ''' </summary>
        ''' <value>The window open mode.</value>
        Public Property WindowOpenMode() As WindowOpenModes
            Get
                Return _WindowOpenMode
            End Get
            Set(ByVal value As WindowOpenModes)
                _WindowOpenMode = value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        ''' <param name="wizard">The wizard.</param>
        Public Sub InitControl(ByVal wizard As WizardScreenParameters)

            SetFilterPropertiesFromWizard(wizard)
            InitControl()

        End Sub

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        Public Sub InitControl()

            SetupJavaScript()
            SetupCareTypeSelector()

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ' show or hide new button

            GenericCreditorPaymentSelector_btnNew.Visible = ShowNewButton
            GenericCreditorPaymentSelector_btnExcInc.Visible = ShowExcludeIncludeButton
            GenericCreditorPaymentSelector_btnCreateBatch.Visible = ShowCreateBatchButton
            GenericCreditorPaymentSelector_btnNotes.Visible = ShowNotesButton
            GenericCreditorPaymentSelector_btnSuspensions.Visible = ShowSuspensionsButton
            GenericCreditorPaymentSelector_btnAuthorise.Visible = ShowAuthoriseButton
            divButtons.Visible = ShowButtons

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Gets the nullable date time as javascipt string.
        ''' </summary>
        ''' <param name="value">The value.</param>
        ''' <returns></returns>
        Private Function GetNullableDateTimeAsJavasciptString(ByVal value As Nullable(Of DateTime)) As String

            If Not value Is Nothing AndAlso value.HasValue Then

                Return String.Format("new Date({0}, {1}, {2})", value.Value.Year, value.Value.Month - 1, value.Value.Day)

            Else

                Return "null"

            End If

        End Function

        ''' <summary>
        ''' Sets the controls filter properties from wizard.
        ''' </summary>
        ''' <param name="wizard">The wizard.</param>
        Public Sub SetFilterPropertiesFromWizard(ByVal wizard As WizardScreenParameters)

            With Me
                .FilterDateFrom = wizard.PaymentDateFrom
                .FilterDateTo = wizard.PaymentDateTo
                .FilterGenericContractID = wizard.GenericContractID
                .FilterGenericCreditorID = wizard.GenericCreditorID
                .FilterIncludeDirectPayment = wizard.PaymentTypesIncludeDirectPayment
                .FilterIncludeNonResidential = wizard.PaymentTypesIncludeNonResidential
                .FilterPaymentNumber = wizard.PaymentReference
                .FilterPaymentStatusDateFrom = wizard.PaymentStatusDateFrom
                .FilterPaymentStatusDateTo = wizard.PaymentStatusDateTo
                .FilterPaymentStatusIncludeAuthorised = wizard.PaymentStatusTypesIncludeAuthorised
                .FilterPaymentStatusIncludeExcludedFromCreditors = wizard.PaymentExcludeFromCreditors
                .FilterPaymentStatusIncludePaid = wizard.PaymentStatusTypesIncludePaid
                .FilterPaymentStatusIncludeSuspended = wizard.PaymentStatusTypesIncludeSuspended
                .FilterPaymentStatusIncludeUnpaid = wizard.PaymentStatusTypesIncludeUnpaid
                .FilterSelectedID = wizard.PaymentID
                .FilterNonResidentialSuspensionReason = wizard.NonResidentialSuspensionReason
                .FilterPaymentSuspendedPaymentStatusTypeOptions = wizard.PaymentSuspendedPaymentStatusTypeOptions
            End With

        End Sub

        ''' <summary>
        ''' Setups the care type selector.
        ''' </summary>
        Private Sub SetupCareTypeSelector()

            With CareTypeSelectorControl
                .InitControl(MyBasePage)
                .showRes = True
                .showNonRes = True
                .showDP = True
                .enableRes = False
                .enableNonRes = True
                .enableDP = False
                .defaultValue = CareTypeSelector.CareTypeType.NonResidential
            End With

        End Sub

        ''' <summary>
        ''' Setups the java script for this control.
        ''' </summary>
        Private Sub SetupJavaScript()

            Dim js As New StringBuilder()
            Dim childNonResContractID As Integer = 0
            Dim childNonResEstablismentID As Integer = 0
            Dim childNonResServiceUserID As Integer = 0
            Dim manuallySuspended As Nullable(Of Boolean) = Nothing

            ' add date utility JS
            MyBasePage.JsLinks.Add(WebUtils.GetVirtualPath(String.Format("{0}date.js", _JsLibrary)))

            ' add utility JS link
            MyBasePage.JsLinks.Add(WebUtils.GetVirtualPath(String.Format("{0}WebSvcUtils.js", _JsLibrary)))

            ' add dialog JS
            MyBasePage.JsLinks.Add(WebUtils.GetVirtualPath(String.Format("{0}Dialog.js", _JsLibrary)))

            ' add list filter JS
            MyBasePage.JsLinks.Add(WebUtils.GetVirtualPath(String.Format("{0}ListFilter.js", _JsLibrary)))

            ' add page JS
            MyBasePage.JsLinks.Add(WebUtils.GetVirtualPath(String.Format("AbacusWeb/Apps/UserControls/{0}.js", _SelectorName)))

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.CreditorPayments))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))

            If Convert.ToInt32(FilterPaymentSuspendedPaymentStatusTypeOptions) > 0 Then

                If (FilterPaymentSuspendedPaymentStatusTypeOptions And WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Automatically) = WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Automatically _
                    AndAlso (FilterPaymentSuspendedPaymentStatusTypeOptions And WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Manually) <> WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Manually Then

                    manuallySuspended = False

                ElseIf (FilterPaymentSuspendedPaymentStatusTypeOptions And WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Automatically) <> WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Automatically _
                    AndAlso (FilterPaymentSuspendedPaymentStatusTypeOptions And WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Manually) = WizardScreenParameters.SearchableSuspendedPaymentStatusTypeOptions.Manually Then

                    manuallySuspended = True

                End If

            End If

            js.AppendFormat("var {0}_CurrentPage = {1};", _SelectorName, CurrentPage)
            js.AppendFormat("var {0}_FilterClientID = {1};", _SelectorName, FilterServiceUserID)
            js.AppendFormat("var {0}_FilterDateFrom = {1};", _SelectorName, GetNullableDateTimeAsJavasciptString(FilterDateFrom))
            js.AppendFormat("var {0}_FilterDateTo = {1};", _SelectorName, GetNullableDateTimeAsJavasciptString(FilterDateTo))
            js.AppendFormat("var {0}_FilterGenericContractID = {1};", _SelectorName, FilterGenericContractID)
            js.AppendFormat("var {0}_FilterGenericCreditorID = {1};", _SelectorName, FilterGenericCreditorID)
            js.AppendFormat("var {0}_FilterIncludeNonResidential = {1};", _SelectorName, FilterIncludeNonResidential.ToString().ToLower())
            js.AppendFormat("var {0}_FilterIncludeDirectPayment = {1};", _SelectorName, FilterIncludeDirectPayment.ToString().ToLower())
            js.AppendFormat("var {0}_FilterPaymentStatusIncludeUnpaid = {1};", _SelectorName, FilterPaymentStatusIncludeUnpaid.ToString().ToLower())
            js.AppendFormat("var {0}_FilterPaymentStatusIncludeAuthorised = {1};", _SelectorName, FilterPaymentStatusIncludeAuthorised.ToString().ToLower())
            js.AppendFormat("var {0}_FilterPaymentStatusIncludePaid = {1};", _SelectorName, FilterPaymentStatusIncludePaid.ToString().ToLower())
            js.AppendFormat("var {0}_FilterPaymentStatusIncludeSuspended = {1};", _SelectorName, FilterPaymentStatusIncludeSuspended.ToString().ToLower())
            js.AppendFormat("var {0}_FilterPaymentStatusDateFrom = {1};", _SelectorName, GetNullableDateTimeAsJavasciptString(FilterPaymentStatusDateFrom))
            js.AppendFormat("var {0}_FilterPaymentStatusDateTo = {1};", _SelectorName, GetNullableDateTimeAsJavasciptString(FilterPaymentStatusDateTo))
            js.AppendFormat("var {0}_FilterManuallySuspended = {1};", _SelectorName, WebUtils.GetBooleanAsJavascriptString(manuallySuspended))
            If Not FilterPaymentStatusIncludeExcludedFromCreditors Is Nothing AndAlso FilterPaymentStatusIncludeExcludedFromCreditors.HasValue Then
                js.AppendFormat("var {0}_FilterPaymentStatusIncludeExcludedFromCreditors = {1};", _SelectorName, FilterPaymentStatusIncludeExcludedFromCreditors.Value.ToString().ToLower())
            Else
                js.AppendFormat("var {0}_FilterPaymentStatusIncludeExcludedFromCreditors = null;", _SelectorName)
            End If
            js.AppendFormat("var {0}_SelectedID = {1};", _SelectorName, FilterSelectedID)
            js.AppendFormat("var {0}_ShowServiceUserColumn = {1};", _SelectorName, ShowServiceUserColumn.ToString().ToLower())
            js.AppendFormat("var {0}_FilterPaymentNumber = '{1}';", _SelectorName, FilterPaymentNumber)
            js.AppendFormat("var {0}_FilterNonResidentialSuspensionReason = '{1}';", _SelectorName, CType(FilterNonResidentialSuspensionReason, Integer))
            js.AppendFormat("var {0}_QsPaymentID = '{1}';", _SelectorName, WizardScreenParameters.QsPaymentID)
            js.AppendFormat("var {0}_QsPaymentStatusTypes = '{1}';", _SelectorName, WizardScreenParameters.QsPaymentStatusTypes)
            js.AppendFormat("var {0}_QsPaymentStatusTypesAuthorised = '{1}';", _SelectorName, CType(WizardScreenParameters.SearchablePaymentStatusTypes.Authorised, Integer))
            js.AppendFormat("var {0}_QsPaymentStatusTypesUnpaid = '{1}';", _SelectorName, CType(WizardScreenParameters.SearchablePaymentStatusTypes.Unpaid, Integer))
            js.AppendFormat("var {0}_btnView = GetElement('{1}', true);", _SelectorName, GenericCreditorPaymentSelector_btnView.ClientID)
            js.AppendFormat("var {0}_btnNew = GetElement('{1}', true);", _SelectorName, GenericCreditorPaymentSelector_btnNew.ClientID)
            js.AppendFormat("var {0}_btnExcInc = GetElement('{1}', true);", _SelectorName, GenericCreditorPaymentSelector_btnExcInc.ClientID)
            js.AppendFormat("var {0}_btnCreateBatch = GetElement('{1}', true);", _SelectorName, GenericCreditorPaymentSelector_btnCreateBatch.ClientID)
            js.AppendFormat("var {0}_btnNotes = GetElement('{1}', true);", _SelectorName, GenericCreditorPaymentSelector_btnNotes.ClientID)
            js.AppendFormat("var {0}_btnSuspensions = GetElement('{1}', true);", _SelectorName, GenericCreditorPaymentSelector_btnSuspensions.ClientID)
            js.AppendFormat("var {0}_btnAuthorise = GetElement('{1}', true);", _SelectorName, GenericCreditorPaymentSelector_btnAuthorise.ClientID)
            js.AppendFormat("var {0}_WindowOpenMode = {1};", _SelectorName, CType(WindowOpenMode, Integer))
            js.AppendFormat("var {0}_QsFilterContractNumber = '{1}';", _SelectorName, WizardScreenParameters.QsFilterContractNumber)
            js.AppendFormat("var {0}_QsFilterCreditorName = '{1}';", _SelectorName, WizardScreenParameters.QsFilterCreditorName)
            js.AppendFormat("var {0}_QsFilterCreditorReference = '{1}';", _SelectorName, WizardScreenParameters.QsFilterCreditorReference)
            js.AppendFormat("var {0}_QsFilterPaymentReference = '{1}';", _SelectorName, WizardScreenParameters.QsFilterPaymentReference)
            js.AppendFormat("var {0}_QsFilterServiceUserName = '{1}';", _SelectorName, WizardScreenParameters.QsFilterServiceUserName)
            js.AppendFormat("var {0}_QsServiceUserID = '{1}';", _SelectorName, WizardScreenParameters.QsServiceUserID)
            js.AppendFormat("var {0}_QsNonResAdditionalFilter = '{1}';", _SelectorName, WizardScreenParameters.QsNonResAdditionalFilter)

            If FilterGenericContractID > 0 Then
                ' if we have a generic contract

                Dim contract As GenericContract = Nothing
                Dim msg As New ErrorMessage()
                Dim svcGroup As New ServiceGroup(auditUserName:=String.Empty, auditLogTitle:=String.Empty, conn:=MyBasePage.DbConnection)

                ' fetch the generic contract record
                msg = CreditorPaymentsBL.GetGenericContract(MyBasePage.DbConnection, FilterGenericContractID, contract)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' fetch the service group to determine
                msg = svcGroup.Fetch(contract.ServiceGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                Select Case CType(svcGroup.ServiceCategory, ServiceCategory)

                    Case Abacus.Library.ServiceCategory.NonResidential

                        childNonResContractID = contract.ChildID

                End Select

            End If

            If FilterGenericCreditorID > 0 Then
                ' if we have a generic creditor

                Dim creditor As GenericCreditor = Nothing
                Dim msg As New ErrorMessage()

                ' fetch the generic creditor record
                msg = CreditorPaymentsBL.GetGenericCreditor(MyBasePage.DbConnection, FilterGenericCreditorID, creditor)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                Select Case CType(creditor.Type, CreditorPaymentsBL.GenericCreditorType)

                    Case CreditorPaymentsBL.GenericCreditorType.Establishment

                        childNonResEstablismentID = creditor.ChildID

                    Case CreditorPaymentsBL.GenericCreditorType.Client

                        childNonResServiceUserID = creditor.ChildID

                End Select

            End If

            js.AppendFormat("var {0}_FilterNonResContractID = {1};", _SelectorName, childNonResContractID)
            js.AppendFormat("var {0}_FilterNonResEstablishmentID = {1};", _SelectorName, childNonResEstablismentID)
            js.AppendFormat("var {0}_FilterNonResServiceUserID = {1};", _SelectorName, childNonResServiceUserID)

            MyBasePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                            String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup", _SelectorName), _
                                                            Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

#End Region

    End Class
End Namespace
