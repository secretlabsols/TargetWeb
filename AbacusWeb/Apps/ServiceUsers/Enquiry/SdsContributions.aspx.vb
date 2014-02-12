﻿Imports System.Collections.Generic
Imports System.Text
Imports Target.Web.Apps
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.SDS
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.ServiceUsers.Enquiry

    ''' <summary>
    ''' Screen used to maintain service user sds contributions.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD      10/12/2010  D11852C - Created
    ''' </history>
    Partial Public Class SdsContributions
        Inherits BasePage

#Region "Fields"

        ' locals
        Private _ServiceUserID As Nullable(Of Integer) = Nothing

        ' constants
        Private Const _PageTitle As String = "SDS Contributions"
        Private Const _QsClientIdKey As String = "clientid"
        Private Const _QsExpandedKey As String = "expanded"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.ServiceUserEnquiry"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the financial assessment selector control.
        ''' </summary>
        ''' <value>The financial assessment selector control.</value>
        Private ReadOnly Property FinancialAssessmentSelectorControl() As Apps.UserControls.FinancialAssessmentSelector
            Get
                Return CType(cpFinancialAssessments.FindControl("financialAssessmentSelector1"),  _
                                Apps.UserControls.FinancialAssessmentSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the pending sucl changes control.
        ''' </summary>
        ''' <value>The pending sucl changes control.</value>
        Private ReadOnly Property PendingSuclChangesControl() As Apps.UserControls.PendingSuclChangesSelector
            Get
                Return CType(cpPendingContributionLevels.FindControl("pendingServiceUserContributionLevel1"),  _
                                Apps.UserControls.PendingSuclChangesSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the SDS contribution summary control.
        ''' </summary>
        ''' <value>The SDS contribution summary control.</value>
        Private ReadOnly Property SdsContributionSummaryControl() As Apps.UserControls.SdsContributionSummary
            Get
                Return CType(cpContributionSummary.FindControl("sdsContributionSummary1"),  _
                                Apps.UserControls.SdsContributionSummary)
            End Get
        End Property

        ''' <summary>
        ''' Gets the service user contribution level control.
        ''' </summary>
        ''' <value>The service user contribution level control.</value>
        Private ReadOnly Property ServiceUserContributionLevelControl() As Apps.UserControls.SUCLevelSelector
            Get
                Return CType(cpActiveContributionLevels.FindControl("serviceUserContributionLevel1"),  _
                                Apps.UserControls.SUCLevelSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the service user contribution level control.
        ''' </summary>
        ''' <value>The service user contribution level control.</value>
        Private ReadOnly Property HistoryNotificationLettersControl() As Apps.UserControls.HistoricNotificationLetters
            Get
                Return CType(cpHistoricNotificationLetters.FindControl("historicNotificationLetters1"),  _
                                Apps.UserControls.HistoricNotificationLetters)
            End Get
        End Property

        ''' <summary>
        ''' Gets the server control identifier generated by ASP.NET.
        ''' </summary>
        ''' <value></value>
        ''' <returns>
        ''' The server control identifier generated by ASP.NET.
        ''' </returns>
        Private ReadOnly Property ServiceUserID() As Nullable(Of Integer)
            Get
                If _ServiceUserID.HasValue = False Then
                    ' if we have not fetched service user id then fetch from qs
                    _ServiceUserID = Utils.ToInt32(Request.QueryString(_QsClientIdKey))
                End If
                Return _ServiceUserID
            End Get
        End Property

        ''' <summary>
        ''' Gets if all panels should be expanded.
        ''' </summary>
        ''' <value>Gets if all panels should be expanded.</value>
        Private ReadOnly Property ExpandAllPanels() As Boolean
            Get
                Dim expand As Nullable(Of Boolean) = Utils.ToBoolean(Request.QueryString(_QsExpandedKey))
                If Not expand Is Nothing AndAlso expand.HasValue AndAlso expand.Value = True Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Setups the financial assessment selector control.
        ''' </summary>
        Private Sub SetupFinancialAssessmentSelectorControl()

            ' setup financial control selector
            With FinancialAssessmentSelectorControl
                .FilterClientID = ServiceUserID
                .InitControl(Me.Page)
            End With

            If ExpandAllPanels = True Then
                ' if we should expand all panels then expand this one

                cpFinancialAssessments.Expanded = True

            End If

        End Sub

        ''' <summary>
        ''' Setups the pending sucl changes control.
        ''' </summary>
        Private Sub SetupPendingSuclChangesControl()

            Dim deferContributionLevelsUntilNotificationLetter As Boolean = False
            Dim deferContributionLevelsUntilNotificationLetterKey As String = "DeferChangesToContributionLevels"
            Dim msg As New ErrorMessage()
            Dim numberOfPendingChanges As Integer = 0

            If Settings.SettingExists(ApplicationName.AbacusIntranet, deferContributionLevelsUntilNotificationLetterKey) Then

                If Settings(ApplicationName.AbacusIntranet, deferContributionLevelsUntilNotificationLetterKey) = 1 Then

                    deferContributionLevelsUntilNotificationLetter = True

                End If

            End If

            ' get if we have any pending changes to display so we can control visibility of control
            msg = ServiceUserContributionLevelBL.FetchPendingServiceUserContributionLevelPendingChanges(DbConnection, 0, 1, ServiceUserID, numberOfPendingChanges, New List(Of ViewablePendingServiceUserContributionLevel))
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If deferContributionLevelsUntilNotificationLetter = False Then

                cpPendingContributionLevels.Visible = False

            Else

                cpPendingContributionLevels.Visible = True

                If numberOfPendingChanges > 0 Then
                    ' if we have some pending changes then setup control

                    ' setup pending sucl changes control
                    With PendingSuclChangesControl
                        .FilterClientID = ServiceUserID
                        .InitControl(Me.Page)
                    End With

                Else
                    ' else no pending changes so hide control

                    PendingSuclChangesControl.Visible = False
                    cpPendingContributionLevels.HeaderLinkTextCSS = "colPanelHeaderLinkEmpty"

                End If

            End If

            If ExpandAllPanels = True Then
                ' if we should expand all panels then expand this one

                cpPendingContributionLevels.Expanded = True

            End If

        End Sub

        ''' <summary>
        ''' Setups the SDS contribution summary control.
        ''' </summary>
        Private Sub SetupSdsContributionSummaryControl()

            ' setup contribution summary control
            With SdsContributionSummaryControl
                .ContributionSummaryClientID = ServiceUserID
            End With

            If ExpandAllPanels = True Then
                ' if we should expand all panels then expand this one

                cpContributionSummary.Expanded = True

            End If

        End Sub

        ''' <summary>
        ''' Setups the service user contribution level control.
        ''' </summary>
        Private Sub SetupServiceUserContributionLevelControl()

            ' setup service user contribution level controls
            With ServiceUserContributionLevelControl
                .InitControl(Me.Page, Nothing, ServiceUserID, True, False)
            End With

            If ExpandAllPanels = True Then
                ' if we should expand all panels then expand this one

                cpActiveContributionLevels.Expanded = True

            End If

        End Sub

        Private Sub SetUpHistoricNotificationLettersControl()
            Dim deferContributionLevelsUntilNotificationLetter As Boolean = False
            Dim deferContributionLevelsUntilNotificationLetterKey As String = "DeferChangesToContributionLevels"
            Dim msg As New ErrorMessage()

            If Settings.SettingExists(ApplicationName.AbacusIntranet, deferContributionLevelsUntilNotificationLetterKey) Then

                If Settings(ApplicationName.AbacusIntranet, deferContributionLevelsUntilNotificationLetterKey) = 1 Then

                    deferContributionLevelsUntilNotificationLetter = True

                End If

            End If


            If deferContributionLevelsUntilNotificationLetter = False Then

                cpHistoricNotificationLetters.Visible = False

            Else

                cpHistoricNotificationLetters.Visible = True

                ' if we have some pending changes then setup control

                ' setup pending sucl changes control
                With HistoryNotificationLettersControl
                    .FilterClientID = ServiceUserID
                    .InitControl(Me.Page)
                End With



            End If

            If ExpandAllPanels = True Then
                ' if we should expand all panels then expand this one

                cpHistoricNotificationLetters.Expanded = True

            End If

        End Sub

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            InitPage(Target.Library.Web.ConstantsManager.GetConstant(_WebNavMenuItemKey), _PageTitle)
            Me.UseJQuery = True
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = MenuItemID

            If ServiceUserID.HasValue = False OrElse ServiceUserID.Value = 0 Then
                ' if we have no user id then advise user

                lblError.Text = "Required parameter Client Id must be specified."

            Else
                ' we have a service user id, setup controls 

                SetupSdsContributionSummaryControl()
                SetupServiceUserContributionLevelControl()
                SetupPendingSuclChangesControl()
                SetupFinancialAssessmentSelectorControl()
                SetUpHistoricNotificationLettersControl()
            End If

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim js As New StringBuilder()

            JsLinks.Add("SdsContributions.js")

            ' add filter properties to page in js format
            js.AppendFormat("SdsContributions_cpPendingContributionLevelsID = '{0}';", cpPendingContributionLevels.ClientID)

            ClientScript.RegisterStartupScript(Me.GetType(), _
                                               "Target.Abacus.Web.Apps.ServiceUsers.Enquiry.SdsContributions", _
                                               Target.Library.Web.Utils.WrapClientScript(js.ToString()))


        End Sub

#End Region

    End Class

End Namespace