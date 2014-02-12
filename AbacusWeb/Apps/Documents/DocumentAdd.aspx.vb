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
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Abacus.Library.Documents
Imports System.Configuration.ConfigurationManager
Imports System.Text

Namespace Apps.Documents

    ''' <summary>
    ''' Uploads a user-specified document and adds it to the document storage
    ''' </summary>
    ''' <history>
    ''' Mo Tahir 08/11/2011 BTI260 Document Types duplicated in drop downs 
    ''' IHS  11/03/2011  D11915 - added comments
    ''' IHS  02/02/2011  Created (D11915)
    ''' </history>
    Partial Public Class DocumentAdd
        Inherits BasePage

#Region "Fields"

        Private Const _NavigationItemKey As String = "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"
        Private Const _PageTitle As String = "Add Document"
        Private Const _documentID As String = "documentid"

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

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Documents))

            If Not Page.IsPostBack Then

                InitialiseAssociation1()

                BindDocumentTypes()
            Else
                UploadFile()
            End If

            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.Documents.DocumentAdd.cboDocumentTypeID", _
                                                       String.Format("cboDocumentTypeID = '{0}'; ServiceUserTypeID = {1};clientSelector1ID='{2}';budgetholderSelector1ID='{3}';creditorSelector1ID='{4}';serviceOrderSelector1ID='{5}';" + _
                                                                     "clientSelector2ID='{6}';budgetholderSelector2ID='{7}';creditorSelector2ID='{8}';serviceOrderSelector2ID='{9}';" + _
                                                                     "clientSelector3ID='{10}';budgetholderSelector3ID='{11}';creditorSelector3ID='{12}';serviceOrderSelector3ID='{13}';" + _
                                                                     "clientSelector4ID='{14}';budgetholderSelector4ID='{15}';creditorSelector4ID='{16}';serviceOrderSelector4ID='{17}';", _
                                                                     cboDocumentType.DropDownList.ClientID, Convert.ToInt32(ServiceUserType), _
                                                                     clientSelector1.ClientID, budgetholderSelector1.ClientID, creditorSelector1.ClientID, serviceOrderSelector1.ClientID, _
                                                                     clientSelector2.ClientID, budgetholderSelector2.ClientID, creditorSelector2.ClientID, serviceOrderSelector2.ClientID, _
                                                                     clientSelector3.ClientID, budgetholderSelector3.ClientID, creditorSelector3.ClientID, serviceOrderSelector3.ClientID, _
                                                                     clientSelector4.ClientID, budgetholderSelector4.ClientID, creditorSelector4.ClientID, serviceOrderSelector4.ClientID), True)
        End Sub

        Private Sub InitialiseAssociation1()

            ' if DocumentAssociatorID (service user's ID e.g. ClientID) is provided 
            ' then prime 'InPlaceSelector' control
            If DocumentAssociatorID > 0 Then
                Select Case ServiceUserType
                    Case DocumentAssociationType.ServiceUser
                        CType(clientSelector1, InPlaceClientSelector).ClientDetailID = DocumentAssociatorID
                        CType(serviceOrderSelector1, InPlaceGenericServiceOrderSelector).selectedClientID = DocumentAssociatorID
                        CType(serviceOrderSelector2, InPlaceGenericServiceOrderSelector).selectedClientID = DocumentAssociatorID
                        CType(serviceOrderSelector3, InPlaceGenericServiceOrderSelector).selectedClientID = DocumentAssociatorID
                        CType(serviceOrderSelector4, InPlaceGenericServiceOrderSelector).selectedClientID = DocumentAssociatorID
                    Case DocumentAssociationType.Creditor
                        CType(creditorSelector1, InPlaceGenericCreditorSelector).ItemID = DocumentAssociatorID
                    Case DocumentAssociationType.BudgetHolder
                        CType(budgetholderSelector1, InPlaceBudgetHolderSelector).BudgetHolderID = DocumentAssociatorID
                End Select
            End If
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

        Private ReadOnly Property ServiceUserType() As DocumentAssociationType
            Get
                Dim intTmp As Nullable(Of Integer) = GetIntegerFromQueryString(_serviceUserType)

                If intTmp Is Nothing Then Return DocumentAssociationType.None

                Return CType(intTmp.Value, DocumentAssociationType)
            End Get
        End Property

        Private ReadOnly Property DocumentAssociatorID() As Integer
            Get
                Dim intTmp As Nullable(Of Integer) = GetIntegerFromQueryString(_documentAssociatorID)

                If intTmp Is Nothing Then Return 0

                Return intTmp.Value
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
            Dim webSecurityUserID As Integer = SecurityBL.GetCurrentUser().ID
            Dim list As New DataClasses.Collections.vwDocumentTypesForWebSecurityUserCollection
            Dim conn As SqlConnection = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
            DocumentTypeBL.FetchDistinctList(conn:=conn, list:=list, webSecurityUserID:=webSecurityUserID, _
                                     systemType:=Nothing, redundant:=TriState.False)

            cboDocumentType.DropDownList.Items.Clear()
            cboDocumentType.DropDownList.DataSource = list
            cboDocumentType.DropDownList.DataValueField = "DocumentTypeID"
            cboDocumentType.DropDownList.DataTextField = "DocumentType"
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
                If chkPublish.Visible Then
                    docFile.Document.PublishToExtranet = (chkPublish.CheckBox.Checked)
                Else
                    docFile.Document.PublishToExtranet = False
                End If
                If hidAssoc1TypeID.Value <> "" And hidAssoc1TypeID.Value <> "99" Then
                    msg = AddDocumentAssociation(docFile, hidAssoc1TypeID.Value, clientSelector1, _
                                                 creditorSelector1, budgetholderSelector1, serviceOrderSelector1)
                    If Not msg.Success Then Return msg
                End If

                If hidAssoc2TypeID.Value <> "" And hidAssoc2TypeID.Value <> "99" Then
                    msg = AddDocumentAssociation(docFile, hidAssoc2TypeID.Value, clientSelector2, _
                                                 creditorSelector2, budgetholderSelector2, serviceOrderSelector2)
                    If Not msg.Success Then Return msg
                End If

                If hidAssoc3TypeID.Value <> "" And hidAssoc3TypeID.Value <> "99" Then
                    msg = AddDocumentAssociation(docFile, hidAssoc3TypeID.Value, clientSelector3, _
                                                 creditorSelector3, budgetholderSelector3, serviceOrderSelector3)
                    If Not msg.Success Then Return msg
                End If

                If hidAssoc4TypeID.Value <> "" And hidAssoc4TypeID.Value <> "99" Then
                    msg = AddDocumentAssociation(docFile, hidAssoc4TypeID.Value, clientSelector4, _
                                                 creditorSelector4, budgetholderSelector4, serviceOrderSelector4)
                    If Not msg.Success Then Return msg
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg
        End Function

        Private Function AddDocumentAssociation(ByRef docFile As DocumentFileBase, _
                                                ByVal enumDocAssocType As DocumentAssociationType, _
                                                ByVal clientSelector As InPlaceClientSelector, _
                                                ByVal creditorSelector As InPlaceGenericCreditorSelector, _
                                                ByVal budgetholderSelector As InPlaceBudgetHolderSelector, _
                                                ByVal serviceOrderSelector As InPlaceGenericServiceOrderSelector) As ErrorMessage
            Try
                Dim msg As New ErrorMessage
                Dim selectedID As Integer = -1

                Select Case enumDocAssocType
                    Case DocumentAssociationType.ServiceUser
                        selectedID = Utils.ToInt32(Request.Form(CType(clientSelector, InPlaceClientSelector).HiddenFieldUniqueID))
                    Case DocumentAssociationType.Creditor
                        selectedID = Utils.ToInt32(Request.Form(CType(creditorSelector, InPlaceGenericCreditorSelector).HiddenFieldUniqueID))
                    Case DocumentAssociationType.BudgetHolder
                        selectedID = Utils.ToInt32(Request.Form(CType(budgetholderSelector, InPlaceBudgetHolderSelector).HiddenFieldUniqueID))
                    Case DocumentAssociationType.ServiceOrder
                        selectedID = Utils.ToInt32(Request.Form(CType(serviceOrderSelector, InPlaceGenericServiceOrderSelector).HiddenFieldUniqueID))
                End Select

                If selectedID > 0 Then
                    docFile.AddAssociation(False, enumDocAssocType, selectedID, msg)
                Else
                    msg.Success = True
                End If

                Return msg
            Catch ex As Exception
                Return Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try
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

#Region "  btnAdd_Click "

        ''' <summary>
        ''' On clicking Add this method uploads selected document and stores it to the document store.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub UploadFile()
            Dim msg As ErrorMessage = Nothing, fileData As Byte() = Nothing, fileName As String = String.Empty
            Dim docFile As DocumentFileBase, trans As SqlTransaction = Nothing
            Dim serviceUserRef As String

            Try
                ResetValidators()

                Me.Validate()
                If Not Me.IsValid Then Return

                trans = SqlHelper.GetTransaction(MyBase.DbConnection)

                docFile = DocumentFactory.GetFileObject(trans, DocumentRepositoryType.InternalDatabase)

                msg = Me.SetDocumentProperties(trans, docFile)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                serviceUserRef = docFile.DocumentAssociations(0).Reference
                msg = docFile.Save(New FileNameHelper(serviceUserRef:=serviceUserRef))
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

        Private Sub ResetValidators()
            CType(clientSelector1, InPlaceClientSelector).Required = False
            CType(budgetholderSelector1, InPlaceBudgetHolderSelector).Required = False
            CType(creditorSelector1, InPlaceGenericCreditorSelector).Required = False
            CType(serviceOrderSelector1, InPlaceGenericServiceOrderSelector).Required = False

            CType(clientSelector2, InPlaceClientSelector).Required = False
            CType(budgetholderSelector2, InPlaceBudgetHolderSelector).Required = False
            CType(creditorSelector2, InPlaceGenericCreditorSelector).Required = False
            CType(serviceOrderSelector2, InPlaceGenericServiceOrderSelector).Required = False

            CType(clientSelector3, InPlaceClientSelector).Required = False
            CType(budgetholderSelector3, InPlaceBudgetHolderSelector).Required = False
            CType(creditorSelector3, InPlaceGenericCreditorSelector).Required = False
            CType(serviceOrderSelector3, InPlaceGenericServiceOrderSelector).Required = False

            CType(clientSelector4, InPlaceClientSelector).Required = False
            CType(budgetholderSelector4, InPlaceBudgetHolderSelector).Required = False
            CType(creditorSelector4, InPlaceGenericCreditorSelector).Required = False
            CType(serviceOrderSelector4, InPlaceGenericServiceOrderSelector).Required = False

        End Sub


#End Region

        Private Sub Page_PreRenderComplete1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            'only show publish checkbox if the user has the relevant security item
            chkPublish.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DocumentsTab.PublishToExtranet"))

        End Sub

    End Class

End Namespace