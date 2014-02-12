Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Utils = Target.Library.Utils
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.Documents
Imports Target.Abacus.Web.Apps.Documents.UserControls
Imports Target.Web.Library.UserControls

Namespace Apps.Documents

    ''' <summary>
    ''' Displays properties for a document as well as functionality to delete a document.
    ''' </summary>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' IHS  01/02/2011  Created (D11915)
    ''' </history>
    Partial Public Class DocumentProperties
        Inherits BasePage

#Region "Fields"

        ' Constants
        Private Const DATE_FORMAT As String = "dd/MM/yyyy HH:mm"
        Private Const _ErrorNoDocumentID As String = "DocumentID must be specified."
        Private Const _NavigationItemKey As String = "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"
        Private Const _WebNavMenuItemDelete As String = "AbacusIntranet.WebNavMenuItemCommand.DocumentsTab.Delete"
        Private Const _PageTitle As String = "View Document Properties"
        Private Const _documentID As String = "documentid"
        Private _showPublishToExtranetButton As Boolean = False

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Me.InitPage(ConstantsManager.GetConstant(_NavigationItemKey), _PageTitle)
            MyBase.UseJQuery = True
        End Sub

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If ValidateRequest() Then PopulateTabs()

        End Sub

        Protected Sub gvPrintHistory_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) _
                                                       Handles gvPrintHistory.PageIndexChanging
            gvPrintHistory.PageIndex = e.NewPageIndex
            PopulatePrintHistory()
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the DocumentID from the query string. Null if not specified or has no value.
        ''' </summary>
        ''' <value>The budget holder ID.</value>
        Public ReadOnly Property DocumentID() As Nullable(Of Integer)
            Get
                Return GetIntegerFromQueryString(_documentID)
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Displays an error message
        ''' </summary>
        ''' <param name="errorMessage">The error message to display</param>
        ''' <remarks></remarks>
        Private Sub DisplayError(ByVal errorMessage As String)

            lblError.Text = errorMessage
            pnlForm.Visible = False

        End Sub

        ''' <summary>
        ''' Gets an integer from query string.
        ''' </summary>
        ''' <param name="key">The key.</param>
        ''' <returns></returns>
        Private Function GetIntegerFromQueryString(ByVal key As String) As Nullable(Of Integer)
            Dim qsInt As String = Request.QueryString(key)
            Dim tmpInt As Integer = Target.Library.Utils.ToInt32(qsInt)

            ' if the value is larger than 0 return
            If tmpInt > 0 Then Return tmpInt

            ' else return nothing
            Return Nothing
        End Function

        ''' <summary>
        ''' Populates the document tabs.
        ''' </summary>
        Private Sub PopulateTabs()
            If DocumentID.HasValue Then
                PopulateProperties()
                PopulatePrintHistory()
            Else
                DisplayError(_ErrorNoDocumentID)
            End If
        End Sub

        ''' <summary>
        ''' Populates the Properties tab
        ''' </summary>
        Private Sub PopulateProperties()

            Dim objDocument As Document = New Document(DbConnection)

            DocumentBL.Fetch(DocumentID, objDocument)

            ' Document
            txtDescription.Text = objDocument.Description
            CType(lnkIconAndDownload, DocumentDownloadLink).InitControl(DocumentID)
            txtType.Text = DocumentTypeBL.GetDocumentType(objDocument.DocumentTypeID, DbConnection)
            txtCreated.Text = objDocument.CreatedDate.ToString(DATE_FORMAT)
            txtCreatedBy.Text = objDocument.CreatedBy

            ' Repository
            txtName.Text = DocumentRepositoryBL.GetDocumentRepository _
                                (objDocument.DocumentRepositoryID, DbConnection)
            txtReference.Text = objDocument.RepositoryReference

            ' show/hide Delete button
            btnDelete.Visible = MyBase.UserHasMenuItemCommand(ConstantsManager.GetConstant(_WebNavMenuItemDelete)) _
                                AndAlso (objDocument.Origin <> DocumentOrigin.SystemGenerated)



            If Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DocumentsTab.PublishToExtranet")) Then
                btnPublish.Visible = True
                If objDocument.PublishToExtranet Then
                    btnPublish.Text = "UnPublish on Extranet"
                Else
                    btnPublish.Text = "Publish to Extranet"
                End If
            Else
                btnPublish.Visible = False
            End If



            ' Associations
            PopulateAssociations()

            'detail tab
            PopulatePrintHistory()

        End Sub

        ''' <summary>
        ''' Populates the document's associations.
        ''' </summary>
        Private Sub PopulateAssociations()
            Dim listDocAssoc As DocumentAssociationCollection = New DocumentAssociationCollection

            DocumentAssociationBL.FetchListByDocument(DbConnection, listDocAssoc, DocumentID)

            gvAssociations.DataSource = listDocAssoc
            gvAssociations.DataBind()

            For Each docAssoc As DocumentAssociation In listDocAssoc
                If GetTypeDesc(docAssoc.Type) = "ServiceOrder" Then
                    _showPublishToExtranetButton = True
                    Exit For
                End If
            Next

        End Sub

        ''' <summary>
        ''' Populates the document's print history tab
        ''' </summary>
        Private Sub PopulatePrintHistory()
            Dim listPrintHistory As New vwDocumentPrintHistoryCollection

            DocumentPrinterBL.FetchPrintHistoryByDocument(Me.DbConnection, listPrintHistory, DocumentID)

            gvPrintHistory.DataSource = listPrintHistory
            gvPrintHistory.DataBind()
        End Sub

        ''' <summary>
        ''' Validates the request i.e. has the correct query string params.
        ''' </summary>
        ''' <returns></returns>
        Private Function ValidateRequest() As Boolean
            ' if we have payment id to work with
            If DocumentID.HasValue Then Return True

            'else: no payment id to work with so advise
            DisplayError(_ErrorNoDocumentID)
            Return False
        End Function

        Public Function GetTypeDesc(ByVal intType As Byte) As String
            Return [Enum].GetName(GetType(DocumentAssociationType), intType)
        End Function

#End Region

#Region "  btnDelete_Click "

        Public Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
            Dim msg As New ErrorMessage
            Dim docFile As DocumentFileBase = Nothing
            Dim trans As SqlTransaction = Nothing

            Try
                trans = SqlHelper.GetTransaction(MyBase.DbConnection)

                docFile = DocumentFactory.GetFileObject(trans, DocumentRepositoryType.InternalDatabase)

                docFile.FetchMetaData(DocumentID)

                msg = docFile.Delete()
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' close dialog and refresh 'parent' page
                Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.Documents.DocumentDeleted", _
                                                           WebUtils.WrapClientScript("RefreshParentAndClose();"))
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                WebUtils.DisplayError(msg)
            Finally
                If Not msg Is Nothing AndAlso Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                Else
                    If Me.IsValid Then trans.Commit()
                End If
            End Try
        End Sub

#End Region

        Private Sub btnPublish_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPublish.Click
            Dim doc As New Document(Me.DbConnection)
            Dim msg As ErrorMessage

            msg = doc.Fetch(DocumentID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With doc
                If .PublishToExtranet Then
                    .PublishToExtranet = False
                    btnPublish.Text = "Publish to Extranet"
                Else
                    .PublishToExtranet = True
                    btnPublish.Text = "UnPublish on Extranet"
                End If
                msg = .Save()
                If Not msg.Success Then WebUtils.DisplayError(msg)

            End With

            PopulateTabs()

        End Sub

        Private Sub Page_PreRenderComplete1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            btnPublish.Visible = _showPublishToExtranetButton

        End Sub
    End Class

End Namespace