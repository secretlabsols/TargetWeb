Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Library.Web.UserControls
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.SDS
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' <summary>
    ''' User control representing contribution summary information
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   22/12/2010 D11852C - Created
    ''' </history>
    Partial Public Class SdsContributionSummary
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' locals
        Private _ContributionSummary As ViewableContributionSummary = Nothing
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _ImgStartCollectingUrl As String = Target.Library.Web.Utils.GetVirtualPath("Images/WarningHS.png")
        Private _ImgStopCollectingUrl As String = Target.Library.Web.Utils.GetVirtualPath("Images/info18_blue.png")

        ' constants        
        Private Const _CurrencyFormat As String = "C"
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber
        Private Const _StartCollectingButtonText As String = "Start Collecting"
        Private Const _StopCollectingButtonText As String = "Stop Collecting"
        Private Const _ViewStateClientIdKey As String = "SdsContributionSummary_ClientID"
        Private Const _WebNavMenuItemCommandSuppressKey As String = "AbacusIntranet.WebNavMenuItemCommand.ServiceUserEnquiry.SuppressOrUnsuppressContributions"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the audit log title.
        ''' </summary>
        ''' <value>The audit log title.</value>
        Private ReadOnly Property AuditLogTitle() As String
            Get
                Return AuditLogging.GetAuditLogTitle(BasePage.PageTitle, BasePage.Settings)
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the audit log user.
        ''' </summary>
        ''' <value>The name of the audit log user.</value>
        Private ReadOnly Property AuditLogUserName() As String
            Get
                Return CurrentUser.ExternalUsername
            End Get
        End Property

        ''' <summary>
        ''' Gets the base page.
        ''' </summary>
        ''' <value>The base page.</value>
        Private ReadOnly Property BasePage() As BasePage
            Get
                Return CType(Page, BasePage)
            End Get
        End Property

        ''' <summary>
        ''' Gets the contribution summary.
        ''' </summary>
        ''' <value>The contribution summary.</value>
        Private ReadOnly Property ContributionSummary() As ViewableContributionSummary
            Get
                Dim msg As New ErrorMessage()

                If _ContributionSummary Is Nothing Then
                    ' if we havent fetched the contrib summary

                    Try

                        ' fetch contribution summary and if unsuccessful display error
                        msg = ServiceUserBL.GetContributionSummary(BasePage.DbConnection, ContributionSummaryClientID, _ContributionSummary)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                    Catch ex As Exception
                        ' catch exception and display

                        _ContributionSummary = Nothing
                        If Not msg.Success Then WebUtils.DisplayError(Target.Library.Utils.CatchError(ex, _GeneralErrorNumber))

                    End Try

                End If

                Return _ContributionSummary
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the client ID.
        ''' </summary>
        ''' <value>The client ID.</value>
        Public Property ContributionSummaryClientID() As Integer
            Get
                Return Target.Library.Utils.ToInt32(ViewState(_ViewStateClientIdKey))
            End Get
            Set(ByVal value As Integer)
                ViewState(_ViewStateClientIdKey) = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>The current user.</value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    ' if current user not fetched then get current user
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance has suppress or unsuppress permission.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance has suppress or unsuppress permission; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property HasSuppressUnsuppressPermission() As Boolean
            Get
                Return BasePage.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant(_WebNavMenuItemCommandSuppressKey))
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Click event of the btnStartStopCollection control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub btnStartStopCollection_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStartStopCollection.Click

            SaveClientDetail()

        End Sub

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not IsPostBack Then
                ' if first hit then populate form

                ResetControlVisibility()
                PopulateContributionSummary(True, False)

            End If

        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Displays the error.
        ''' </summary>
        ''' <param name="message">The message.</param>
        Private Sub DisplayError(ByVal message As String)

            ResetControlVisibility()
            divError.Visible = True
            lblError.Text = message

        End Sub

        ''' <summary>
        ''' Populates the contribution summary.
        ''' </summary>
        ''' <param name="forceFromDb">if set to <c>true</c> [force from db] i.e. do not use cached copy of data.</param>
        ''' <param name="displayActiveServiceUserLevelWarning">if set to <c>true</c> [display active service user level warning].</param>
        Private Sub PopulateContributionSummary(ByVal forceFromDb As Boolean, ByVal displayActiveServiceUserLevelWarning As Boolean)

            Dim result As ViewableContributionSummary = ContributionSummary

            If forceFromDb Then
                ' if force get summary from db i.e. fresh copy of data set var to null

                _ContributionSummary = Nothing

            End If

            result = ContributionSummary

            If Not result Is Nothing Then
                ' if we have a summary to work with

                If result.SucConsideredUpToDate.HasValue Then
                    ' if we have a considered to date for sucw to display 

                    lblConsideredUpToValue.Text = result.SucConsideredUpToDate.Value.ToString(_DateFormat)

                End If

                lblNettedOffDirectPaymentsValue.Text = result.TotalNettedOffDirectPayments.ToString(_CurrencyFormat)
                lblNotYetCollectedConsideredValue.Text = result.TotalSucwNotCollectedConsideredTo.ToString(_CurrencyFormat)

                If result.SdsInvoicedUpToDate.HasValue Then
                    ' if we have a invoice up to date to display 

                    lblInvoicedUpToValue.Text = result.SdsInvoicedUpToDate.Value.ToString(_DateFormat)

                End If

                lblRaisedViaInvoiceValue.Text = result.TotalRaisedViaInvoice.ToString(_CurrencyFormat)
                lblNotYetCollectedInvoicedValue.Text = result.TotalSucwNotCollectedInvoicedTo.ToString(_CurrencyFormat)
                lblNotYetPaidValue.Text = result.TotalNotBeenPaidInvoice.ToString(_CurrencyFormat)

                ' control visibility of suppress button using command item
                btnStartStopCollection.Visible = HasSuppressUnsuppressPermission

                If result.SdsSuppressCollectionOfContributions Then
                    ' we are not collecting contributions, so offer start options

                    imgStartStopCollectionInfo.ImageUrl = _ImgStartCollectingUrl
                    With lblStartStopCollectionInfo
                        .Text = "<span style=""color : red; font-weight : bold;"">Contributions are <u>not</u> being collected</span>"
                    End With
                    btnStartStopCollection.Text = _StartCollectingButtonText

                Else
                    ' we are collecting contributions, so offer stop options

                    imgStartStopCollectionInfo.ImageUrl = _ImgStopCollectingUrl
                    With lblStartStopCollectionInfo
                        .Text = "<span style=""color : black; font-weight : normal"">Contributions are being collected</span>"
                    End With
                    btnStartStopCollection.Text = _StopCollectingButtonText

                End If

                If displayActiveServiceUserLevelWarning AndAlso result.HasActiveServiceUserContributionLevel = False Then
                    ' if we should display warning if no service user contribution levels exist and none exist

                    divStartStopCollectionInfoWarning.Visible = True
                    lblStartStopCollectionInfoWarning.Text = "This service user has no 'active' SDS Contribution Levels and, thus, no contribution will be collected."

                Else
                    ' else do not display warning

                    divStartStopCollectionInfoWarning.Visible = False
                    lblStartStopCollectionInfoWarning.Text = ""

                End If

                divForm.Visible = True

            Else
                ' else no client id so display an error

                DisplayError("Could not locate a contribution summary for specified client id.")

            End If

            With CType(ctlNettedOffDP, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.SDSContributionsNettedOffDirectPayments")
                If ContributionSummaryClientID > 0 Then .Parameters.Add("intClientID", ContributionSummaryClientID)
            End With

            With CType(ctlNotYetCollectedConsidered, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.UncollectedSDSContributions")
                If ContributionSummaryClientID > 0 Then .Parameters.Add("intClientID", ContributionSummaryClientID)
                If result.SucConsideredUpToDate.HasValue Then
                    .Parameters.Add("dteFilterDate", result.SucConsideredUpToDate.Value)
                    .Parameters.Add("considered", True)
                End If


            End With

            With CType(ctlRaisedViaInvoice, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.SDSInvoiceSummary")
                If ContributionSummaryClientID > 0 Then .Parameters.Add("intClientID", ContributionSummaryClientID)
            End With

            With CType(ctlNotYetCollectedInvoiced, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.UncollectedSDSContributions")
                If ContributionSummaryClientID > 0 Then .Parameters.Add("intClientID", ContributionSummaryClientID)
                If result.SdsInvoicedUpToDate.HasValue Then
                    .Parameters.Add("dteFilterDate", result.SdsInvoicedUpToDate.Value)
                    .Parameters.Add("considered", False)
                End If

            End With

            With CType(ctlNotYetPaid, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.SDSInvoiceSummaryUnsatisfiedDebt")
                If ContributionSummaryClientID > 0 Then .Parameters.Add("intClientID", ContributionSummaryClientID)
            End With

        End Sub

        ''' <summary>
        ''' Resets the control visibility.
        ''' </summary>
        Private Sub ResetControlVisibility()

            divError.Visible = False
            divForm.Visible = False

        End Sub

        ''' <summary>
        ''' Saves the client detail.
        ''' </summary>
        Private Sub SaveClientDetail()

            Dim client As ClientDetail = Nothing
            Dim msg As ErrorMessage = Nothing

            ' get the service user so we can update the suppress flag
            msg = ServiceUserBL.GetServiceUser(BasePage.DbConnection, client, ContributionSummaryClientID, AuditLogUserName, AuditLogTitle)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If client.SDSSuppressCollectionOfContributions = TriState.True Then
                ' if we should start collection

                client.SDSSuppressCollectionOfContributions = TriState.False

            Else
                ' else we should stop collection

                client.SDSSuppressCollectionOfContributions = TriState.True

            End If

            ' try to save the changes to db
            msg = client.Save()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' repopulate contribution summary, fetch from db again
            PopulateContributionSummary(True, IIf(client.SDSSuppressCollectionOfContributions = TriState.False, True, False))

        End Sub

#End Region

    End Class

End Namespace
