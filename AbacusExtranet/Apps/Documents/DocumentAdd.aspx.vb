Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports System.Data.SqlClient
Imports Target.Web.Apps.FileStore
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.Documents
Imports System.Configuration.ConfigurationManager
Imports System.Text

Namespace Apps.Documents

    ''' <summary>
    ''' Uploads a user-specified document and adds it to the document storage
    ''' </summary>
    ''' <history>
    ''' [PaulW]  27/10/2011  Created (D11945A)
    ''' </history>
    Partial Public Class DocumentAdd
        Inherits BasePage

#Region "Fields"

        Private Const _NavigationItemKey As String = "AbacusExtranet.WebNavMenuItem.AddDocuments"
        Private Const _PageTitle As String = "Add Document"
        Private Const _documentID As String = "documentid"
        Private Const _qryClientParamName As String = "clientID"
        Private Const _qryGSOParamName As String = "gsoID"

        Private Const _serviceUserType As String = "serviceusertype"
        Private Const _documentAssociatorID As String = "documentassociatorid"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        ''' <history>
        ''' IHS  11/03/2011  D11915 - added comments
        ''' </history>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(_NavigationItemKey), _PageTitle)
            Me.Form.Enctype = "multipart/form-data"
            Me.JsLinks.Add("DocumentAdd.js")

        End Sub

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        ''' <history>
        ''' IHS  11/03/2011  D11915 - added comments
        ''' </history>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.UseJQuery = True
            Me.UseJqueryUI = True
            Me.UseJquerySearchableMenu = True

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.Documents))

            If Not Page.IsPostBack Then

                BindDocumentTypes()

            Else
                UploadFile()
            End If

            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.Documents.DocumentAdd.cboDocumentTypeID", _
                                                       String.Format("cboDocumentTypeID = ""{0}""; ", cboDocumentType.DropDownList.ClientID), True)
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the Document.ID from the query string. Null if not specified or has no value.
        ''' </summary>
        ''' <value>The DocumentID.</value>
        Private ReadOnly Property DocumentID() As Nullable(Of Integer)
            Get
                Return GetIntegerFromQueryString(_documentID)
            End Get
        End Property

        ''' <summary>
        ''' Gets the client.ID from the query string. Null if not specified or has no value.
        ''' </summary>
        ''' <value>The DocumentID.</value>
        Private ReadOnly Property ServiceUserID() As Nullable(Of Integer)
            Get
                Return GetIntegerFromQueryString(_qryClientParamName)
            End Get
        End Property

        ''' <summary>
        ''' Gets the GenericServiceOrder.ID from the query string. Null if not specified or has no value.
        ''' </summary>
        ''' <value>The DocumentID.</value>
        Private ReadOnly Property GenericServiceOrderID() As Nullable(Of Integer)
            Get
                Return GetIntegerFromQueryString(_qryGSOParamName)
            End Get
        End Property

#End Region

#Region "Methods"

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

        Public Sub BindDocumentTypes()
            Dim list As New DataClasses.Collections.DocumentTypeCollection
            Dim conn As SqlConnection = Me.DbConnection

            DataClasses.DocumentType.FetchList(conn:=conn, auditLogTitle:=String.Empty, _
                                               auditUserName:=String.Empty, list:=list, _
                                               redundant:=False, uploadViaExtranet:=True)

            cboDocumentType.DropDownList.Items.Clear()
            cboDocumentType.DropDownList.DataSource = list
            cboDocumentType.DropDownList.DataValueField = "ID"
            cboDocumentType.DropDownList.DataTextField = "Description"
            cboDocumentType.DataBind()

            ' insert empty item at top - to enforce document type selection
            cboDocumentType.DropDownList.Items.Insert(0, New ListItem(String.Empty, String.Empty))
        End Sub

        Private Function SetDocumentProperties(ByVal trans As SqlTransaction, _
                                               ByVal docFile As DocumentFileBase) As ErrorMessage
            Dim msg As ErrorMessage = Nothing

            Try
                Dim fileData As Byte() = Nothing, fileName As String = String.Empty

                msg = Me.GetDataFromTempTable(trans, fileData, fileName)
                If Not msg.Success Then Return msg

                docFile.Document.CreatedBy = SecurityBL.GetCurrentUser().ExternalUsername
                docFile.SetFileData(fileData)
                docFile.FileExtension = IO.Path.GetExtension(fileName)
                docFile.Document.Description = txtDescription.Text
                docFile.Document.DocumentTypeID = CType(cboDocumentType.DropDownList.SelectedValue, Integer)
                docFile.Document.PublishToExtranet = TriState.True

                'Associate file to the generic Service Order
                If Not GenericServiceOrderID Is Nothing AndAlso GenericServiceOrderID > 0 Then
                    Dim gso As New GenericServiceOrder(trans)
                    msg = gso.Fetch(GenericServiceOrderID)
                    If Not msg.Success Then Return msg

                    'Add service User Association
                    docFile.AddAssociation(False, DocumentAssociationType.ServiceUser, gso.ClientID, msg)
                    'Add Generic Service Order Association
                    docFile.AddAssociation(False, DocumentAssociationType.ServiceOrder, gso.ID, msg)
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg
        End Function


        Private Function GetDataFromTempTable(ByVal trans As SqlTransaction, ByRef fileData As Byte(), ByRef fileName As String) As ErrorMessage
            Dim msg As ErrorMessage = Nothing
            Dim tempData As WebFileStoreTempUpload = Nothing
            Dim scannedDocIDs As Integer() = Nothing

            Try
                ' get the uploaded doc ID
                scannedDocIDs = FileStoreBL.GetUploadedTempFileIDs()

                tempData = New WebFileStoreTempUpload(trans)
                If scannedDocIDs.Length > 0 Then
                    msg = tempData.Fetch(scannedDocIDs(0))
                    If Not msg.Success Then Return msg
                End If

                If tempData.Data Is Nothing Then
                    msg = New ErrorMessage
                    msg.Number = "E0517"
                    msg.Message = "File has no data"
                    tempData.Delete()
                    Return msg
                End If

                fileData = tempData.Data
                fileName = tempData.Filename

                ' delete the temp file
                msg = WebFileStoreTempUpload.Delete(trans, scannedDocIDs(0))
                If Not msg.Success Then Return msg

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg
        End Function
#End Region

#Region "  UploadFile "

        ''' <summary>
        ''' On clicking Add this method uploads selected document and stores it to the document store.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub UploadFile()
            Dim msg As ErrorMessage = Nothing, fileData As Byte() = Nothing, fileName As String = String.Empty
            Dim docFile As DocumentFileBase, trans As SqlTransaction = Nothing
            Dim DocumentRef As String

            Try

                Me.Validate()
                If Not Me.IsValid Then Return

                trans = SqlHelper.GetTransaction(MyBase.DbConnection)

                docFile = DocumentFactory.GetFileObject(trans, DocumentRepositoryType.InternalDatabase)

                msg = Me.SetDocumentProperties(trans, docFile)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                DocumentRef = docFile.DocumentAssociations(0).Reference
                msg = docFile.Save(New FileNameHelper(serviceUserRef:=DocumentRef))
                If Not msg.Success Then WebUtils.DisplayError(msg)

                trans.Commit()

                ' close dialog and refresh 'parent' page
                Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.Documents.DocumentAdded", _
                                                           "RefreshParentAndCloseDialog();", True)
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                WebUtils.DisplayError(msg)
            Finally
                If Not msg Is Nothing AndAlso Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                End If
            End Try

        End Sub

#End Region


    End Class

End Namespace