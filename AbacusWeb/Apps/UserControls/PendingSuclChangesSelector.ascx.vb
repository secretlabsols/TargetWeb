Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.UserLetters
Imports Target.Abacus.Library.SDS
Imports Target.Abacus.Web.Apps.WebSvc
Imports Target.Abacus.Library.Documents
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.UserControls

    ''' <summary>
    ''' Class representing a selector tool for PendingServiceUserContributionLevel records
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   14/12/2010 D11852C - Created
    ''' </history>
    Partial Public Class PendingSuclChangesSelector
        Inherits System.Web.UI.UserControl

#Region "Fields"

        Private _FilterClientID As Integer = 0
        Private _FilterSelectedID As Integer = 0

        ' constants
        Private Const _SelectorName As String = "PendingSuclChangesSelector"
        Private Const _WebNavMenuItemCommandGenerateNotificationsKey As String = "AbacusIntranet.WebNavMenuItemCommand.ServiceUserEnquiry.GenerateSdsNotifications"

#End Region

#Region "Properties"

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
        ''' Gets or sets the client ID to filter by.
        ''' </summary>
        ''' <value>The filter client ID.</value>
        Public Property FilterClientID() As Integer
            Get
                Return _FilterClientID
            End Get
            Set(ByVal value As Integer)
                _FilterClientID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets selected id.
        ''' </summary>
        ''' <value>The selected id.</value>
        Public Property FilterSelectedID() As Integer
            Get
                Return _FilterSelectedID
            End Get
            Set(ByVal value As Integer)
                _FilterSelectedID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance has permission to generate SDS contribution notifications.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance has can permission to generate SDS contribution notifications; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property HasCanGenerateSdsContributionNotificationsPermission() As Boolean
            Get
                Return BasePage.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant(_WebNavMenuItemCommandGenerateNotificationsKey))
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Click event of the btnNotify control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnNotify_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNotify.Click
            Dim DocumentID As Integer
            Dim trans As SqlTransaction = Nothing
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Dim msg As New ErrorMessage
            Dim UserName As String = Target.Web.Apps.Security.SecurityBL.GetCurrentUser().ExternalUsername

            trans = SqlHelper.GetTransaction(thePage.DbConnection)
            Try
                'Create the document  first 
                Dim notificationLetter As New SdsContributionNotificationUserLetter()
                '' 
                '
                Dim doc As New System.IO.MemoryStream
                ' set letters properties, merge and then output to client
                Dim cDetail As ClientDetail = New ClientDetail(trans, String.Empty, String.Empty)
                msg = cDetail.Fetch(FilterClientID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                With notificationLetter
                    .ClientID = FilterClientID
                    .DbTransaction = trans
                    msg = .Validate()
                    If msg.Success Then
                        msg = .Merge()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        .MergedUserLetter.Save(doc, Aspose.Words.SaveFormat.Pdf)
                        doc.Position = 0
                    Else
                        PendingSuclChangesSelector_Error.InnerHtml = String.Format("Could not create document : {0}", _
                                                                                   msg.Message.Replace(Environment.NewLine, "<br />"))
                        Return
                    End If
                End With
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' save the document 
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Dim data() As Byte = New Byte() {}
                Dim objDocument As DocumentFileBase = Nothing
                objDocument = DocumentFactory.GetFileObject(trans, DocumentRepositoryType.InternalDatabase)
                objDocument.FileExtension = ".pdf"
                'String.Format("HNL_{0}_{1}.pdf", _
                'cDetail.Reference, Date.Now.ToString("yyyyMMddhhmmss"))
                objDocument.Document.DocumentTypeID = DocumentTypeBL.SU_CONTRIBUTION_NOTIFICATION
                objDocument.Document.Origin = DocumentOrigin.SystemGenerated
                objDocument.Document.CreatedBy = UserName
                objDocument.AddAssociation(False, DocumentAssociationType.ServiceUser, FilterClientID, msg)
                ''If Not msg.Success Then Return msg
                objDocument.SetFileData(doc.ToArray())
                Dim serviceUserReference As String = objDocument.DocumentAssociations(0).Reference
                msg = objDocument.Save(New FileNameHelper(serviceUserRef:=serviceUserReference))
                If Not msg.Success Then
                    PendingSuclChangesSelector_Error.InnerHtml = String.Format("Could not create document : {0}", _
                                                                               msg.Message.Replace(Environment.NewLine, "<br />"))
                    Return
                End If
                DocumentID = objDocument.Document.ID
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' Document print queue
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '' create the document for the print Queue
                Dim documentPrintQueueID As Integer = 0
                msg = DocumentPrinterBL.QueueSingleDocumentForBatchPrinting(trans, _
                                                                            DocumentID, _
                                                                            DateTime.Now, _
                                                                            UserName, _
                                                                            documentPrintQueueID)

                If Not msg.Success Then
                    PendingSuclChangesSelector_Error.InnerHtml = String.Format("Could not create document : {0}", _
                                                                               msg.Message.Replace(Environment.NewLine, "<br />"))
                    Return
                End If
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Create history notification Letters entries
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Dim svc As SUCLevels = New SUCLevels
                Dim svcResults As SucLevels_FetchPendingServiceUserContributionLevelPendingChangesResult = _
                New SucLevels_FetchPendingServiceUserContributionLevelPendingChangesResult()
                svcResults = svc.FetchPendingServiceUserContributionLevelPendingChanges(-1, FilterSelectedID, FilterClientID)
                'Dim listHistoricNotificationLetters As New List(Of HistoricNotificationLetters)
                Dim listHistoricNotificationLetters As NotifiedContributionLevelHistoryCollection
                listHistoricNotificationLetters = New NotifiedContributionLevelHistoryCollection()
                For Each pendingConLevel As ViewablePendingServiceUserContributionLevel In svcResults.Results
                    Dim historicNotificationLetters As New NotifiedContributionLevelHistory(trans)
                    historicNotificationLetters.DateFrom = pendingConLevel.DateFrom
                    historicNotificationLetters.DateTo = pendingConLevel.DateTo
                    historicNotificationLetters.AssessmentType = pendingConLevel.AssessmentType
                    historicNotificationLetters.AssessedCharge = pendingConLevel.AssessedCharge
                    historicNotificationLetters.ChargeableSpendPlanCost = pendingConLevel.ChargeableSpendPlanCost
                    historicNotificationLetters.ContributionLevel = pendingConLevel.ContributionLevel
                    historicNotificationLetters.PlannedAdditionalCost = pendingConLevel.PlannedAdditionalCost
                    historicNotificationLetters.ClientID = pendingConLevel.ClientID
                    '' assign the ID of new document for history notification 
                    historicNotificationLetters.DocumentID = DocumentID
                    '' save the historic notification history
                    msg = historicNotificationLetters.Save()
                    If Not msg.Success Then
                        PendingSuclChangesSelector_Error.InnerHtml = String.Format("Could not create document : {0}", _
                                                                                   msg.Message.Replace(Environment.NewLine, "<br />"))
                        Return
                    End If
                Next
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Syncronise contribution level
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                msg = ServiceUserContributionLevelBL.SynchroniseContributionLevels(trans, FilterClientID)
                If Not msg.Success Then
                    PendingSuclChangesSelector_Error.InnerHtml = String.Format("Could not create document : {0}", _
                                                                               msg.Message.Replace(Environment.NewLine, "<br />"))
                    Return
                End If
            Catch ex As Exception
                trans.Rollback()
            Finally
                If msg.Success Then
                    trans.Commit()
                Else
                    trans.Rollback()
                End If
            End Try
        End Sub

        ''' <summary>
        ''' Handles the Click event of the btnPreview control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnPreview_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPreview.Click

            Dim msg As New ErrorMessage()
            Dim notificationLetter As New SdsContributionNotificationUserLetter()

            ' set letters properties, merge and then output to client
            With notificationLetter
                .ClientID = FilterClientID
                .DbConnection = BasePage.DbConnection
                msg = .Validate()
                If msg.Success Then
                    msg = .Merge()
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    .MergedUserLetter.Save(Response, _
                                           "SdsContributionNotificationUserLetter.pdf", _
                                           Aspose.Words.ContentDisposition.Attachment, _
                                           Aspose.Words.Saving.SaveOptions.CreateSaveOptions(Aspose.Words.SaveFormat.Pdf))

                Else
                    PendingSuclChangesSelector_Error.InnerHtml = String.Format("Could not create document : {0}", _
                                                                               msg.Message.Replace(Environment.NewLine, "<br />"))
                End If
            End With

        End Sub

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        ''' <param name="thePage">The page.</param>
        Public Sub InitControl(ByVal thePage As BasePage)

            Dim js As New StringBuilder()

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))

            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/UserControls/{0}.js", _SelectorName)))

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.SUCLevels))

            ' add filter properties to page in js format
            js.AppendFormat("{1}_FilterClientID = {0};", FilterClientID, _SelectorName)
            js.AppendFormat("{1}_SelectedID = {0};", FilterSelectedID, _SelectorName)
            js.AppendFormat("{1}_CurrentPage = {0};", CurrentPage, _SelectorName)
            js.AppendFormat("{1}_TickImg = '{0}';", Target.Library.Web.Utils.GetVirtualPath("Images/Complete.png"), _SelectorName)
            js.AppendFormat("{1}_CrossImg = '{0}';", Target.Library.Web.Utils.GetVirtualPath("Images/CriticalError.png"), _SelectorName)
            js.AppendFormat("{1}_WarningImg = '{0}';", Target.Library.Web.Utils.GetVirtualPath("Images/WarningHS.png"), _SelectorName)
            js.AppendFormat("{1}_BtnNotifyID = '{0}';", btnNotify.ClientID, _SelectorName)
            js.AppendFormat("{1}_BtnPreviewID = '{0}';", btnPreview.ClientID, _SelectorName)
            js.AppendFormat("{1}_HasCanGenerateSdsContributionNotificationsPermission = {0};", HasCanGenerateSdsContributionNotificationsPermission.ToString().ToLower(), _SelectorName)

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                       String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup", _SelectorName), _
                                                       Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

#End Region


    End Class

End Namespace
