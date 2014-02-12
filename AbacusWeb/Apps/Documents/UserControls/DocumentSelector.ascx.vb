Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Abacus.Library
Imports Target.Library.Web.Controls
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Configuration.ConfigurationManager
Imports System.Text
Imports Target.Abacus.Library.Documents
Imports Target.Web.Apps.Security
Imports System.Data.SqlClient
Imports Target.Abacus.Library.DataClasses

Namespace Apps.Documents.UserControls

    ''' <summary>
    ''' This User Control provides a paginated list of documents
    ''' along with functionality to filter the list, view a document, add
    ''' new document and view properties of a document
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Mo Tahir 08/11/2011 BTI260 Document Types duplicated in drop downs 
    ''' IHS  30/03/2011  D11960 - added "Print" and "Print All" buttons
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Partial Public Class DocumentSelector
        Inherits System.Web.UI.UserControl

#Region " Private Members "

        Private _serviceUserType As DocumentAssociationType = DocumentAssociationType.None
        Private _documentAssociatorID As Integer = -1
        Private _documentPrintQueueBatchID As Integer = -1
        Private _showFilters As ShowFilters = ShowFilters.None
        Private _showButtons As ShowButtons = ShowButtons.None
        Private _parentPage As BasePage = Nothing
        Private _printStatusLabel As String = Nothing

        Dim objFilterHelper As DocumentFilterHelper = Nothing
        Dim _conn As SqlConnection = Nothing

        Private Const DateFormat As String = "dd/MM/yyyy"
        Private Const WebNavMenuItemAddNew As String = "AbacusIntranet.WebNavMenuItemCommand.DocumentsTab.AddNew"
        Private Const WebNavMenuItemDequeue As String = "AbacusIntranet.WebNavMenuItemCommand.DocumentsTab.Dequeue"

        Private ReadOnly Property filterHelper() As DocumentFilterHelper
            Get
                If objFilterHelper Is Nothing Then
                    objFilterHelper = New DocumentFilterHelper(Request.QueryString)
                End If

                Return objFilterHelper
            End Get
        End Property

        Private ReadOnly Property conn() As SqlConnection
            Get
                If _conn Is Nothing Then
                    _conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                End If

                Return _conn
            End Get
        End Property

#End Region

#Region " Properties "

        ''' <summary>
        ''' Service User e.g. client, generic creditor, 3rd party budget holder
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' IHS  11/03/2011  D11915 - added comments
        ''' </history>
        Public Property ServiceUserType() As DocumentAssociationType
            Get
                Return _serviceUserType
            End Get
            Set(ByVal value As DocumentAssociationType)
                _serviceUserType = value
            End Set
        End Property

        ''' <summary>
        ''' Service User's ID e.g. ClientID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' IHS  11/03/2011  D11915 - added comments
        ''' </history>
        Public Property DocumentAssociatorID() As Integer
            Get
                Return _documentAssociatorID
            End Get
            Set(ByVal value As Integer)
                _documentAssociatorID = value
            End Set
        End Property

        Public Property DocumentPrintQueueBatchID() As Integer
            Get
                Return _documentPrintQueueBatchID
            End Get
            Set(ByVal value As Integer)
                _documentPrintQueueBatchID = value
            End Set
        End Property

        Public ReadOnly Property isPrintQueueBatchDocumentsScreen() As Boolean
            Get
                Return (DocumentPrintQueueBatchID > 0)
            End Get
        End Property

        Public Property Show_Filters() As ShowFilters
            Get
                Return _showFilters
            End Get
            Set(ByVal value As ShowFilters)
                _showFilters = value
            End Set
        End Property

        Public Property Show_Buttons() As ShowButtons
            Get
                Return _showButtons
            End Get
            Set(ByVal value As ShowButtons)
                _showButtons = value
            End Set
        End Property

        Public Property PrintStatusLabel() As String
            Get
                Return _printStatusLabel
            End Get
            Set(ByVal value As String)
                _printStatusLabel = value
            End Set
        End Property

        Public ReadOnly Property IframeID() As String
            Get
                Return Target.Library.Utils.ToString(Request.QueryString("iframeid"))
            End Get
        End Property
#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Primes control with DocumentAssociatorID (i.e. Service User's ID e.g. ClientID)
        ''' and initialises JavaScript values
        ''' </summary>
        ''' <param name="thePage"></param>
        ''' <param name="document_AssociatorID"></param>
        ''' <remarks></remarks>
        ''' <history>
        ''' MvO  20/04/2011  SDS issue #514 - corrected behaviour when screen posts back.
        ''' IHS  11/03/2011  D11915 - added comments
        ''' </history>
        Public Sub InitControl(ByVal thePage As BasePage, ByVal document_AssociatorID As Integer)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            DocumentAssociatorID = document_AssociatorID
            _parentPage = thePage

            thePage.UseJQuery = True

            Me.Initialise()

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Documents/UserControls/DocumentSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Documents))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))
            ' set button IDs and misc. variables
            Dim jsIframeID As String = IIf(String.IsNullOrEmpty(IframeID), "null", String.Format("'{0}'", IframeID))
            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.Documents.UserControls.DocumentSelector.Startup", _
                                                       String.Format("currentPage=1; serviceUserType={0}; documentAssociatorID={1}; btnNewID='{2}'; btnViewID='{3}'; btnPropertiesID='{4}'; IframeID={5};", _
                                                       CType(ServiceUserType, Integer), DocumentAssociatorID, btnNew.ClientID, btnView.ClientID, btnProperties.ClientID, jsIframeID), True)
            ' set filters
            Dim strJSvars As String = filterHelper.GetJavaScriptFilterVars()
            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                       "Target.Abacus.Web.Apps.Documents.UserControls.DocumentSelector.SetFilters", _
                                                       strJSvars, True)
            Me.SetJSfilterControlsIDs(thePage)

        End Sub

#End Region

#Region " Private Methods "

        Private Sub Initialise()
            If IframeID <> String.Empty Then
                cpFilters.ExpandedJS = String.Format("parent.resizeIframe(document.body.scrollHeight, '{0}')", IframeID)
                cpFilters.CollapsedJS = cpFilters.ExpandedJS
            End If

            Me.InitialisePrintStatusCheckBoxes()

            Me.BindDocumentTypes()

            Me.ShowHideButtons()

            Me.ShowHideFilters()

            If (Not String.IsNullOrEmpty(PrintStatusLabel)) Then lgdPrintStatus.InnerHtml = PrintStatusLabel

            ApplyFiltersFromQueryString()
        End Sub

        Private Sub ShowHideButtons()
            btnNew.Visible = (Show_Buttons And ShowButtons.New) AndAlso _
                              _parentPage.UserHasMenuItemCommandInAnyMenuItem( _
                               ConstantsManager.GetConstant(WebNavMenuItemAddNew))

            btnView.Visible = (Show_Buttons And ShowButtons.View)
            btnProperties.Visible = (Show_Buttons And ShowButtons.Properties)

            btnRemove.Visible = (Show_Buttons And ShowButtons.Remove) AndAlso _
                                 _parentPage.UserHasMenuItemCommandInAnyMenuItem( _
                                 ConstantsManager.GetConstant(WebNavMenuItemDequeue))
            btnCreateBatch.Visible = (Show_Buttons And ShowButtons.CreateBatch)

            btnPrint.Visible = (Show_Buttons And ShowButtons.Print)
            btnPrintAll.Visible = (Show_Buttons And ShowButtons.Print)
        End Sub

        Private Sub ShowHideFilters()
            fsDocumentType.Visible = (Show_Filters And ShowFilters.DocumentType)

            fsOrigin.Visible = (Show_Filters And ShowFilters.Origin)

            fsCreated.Visible = (Show_Filters And ShowFilters.Created)
            dteCreatedFrom.Visible = (Show_Filters And ShowFilters.CreatedFrom)
            dteCreatedTo.Visible = (Show_Filters And ShowFilters.CreatedTo)

            fsRecipient.Visible = (Show_Filters And ShowFilters.Recipient)

            fsPrintStatus.Visible = (Show_Filters And ShowFilters.PrintStatus)

            chkNeverQueued.Visible = (Show_Filters And ShowFilters.PrintStatusCheckBoxes)
            chkQueued.Visible = (Show_Filters And ShowFilters.PrintStatusCheckBoxes)
            chkBatched.Visible = (Show_Filters And ShowFilters.PrintStatusCheckBoxes)
            chkSentToPrinter.Visible = (Show_Filters And ShowFilters.PrintStatusCheckBoxes)
            chkRemovedFromQueue.Visible = (Show_Filters And ShowFilters.PrintStatusCheckBoxes)

            dtePrintedFrom.Visible = (Show_Filters And ShowFilters.PrintStatusFrom)
            dtePrintedTo.Visible = (Show_Filters And ShowFilters.PrintStatusTo)
            txtPrintedBy.Visible = (Show_Filters And ShowFilters.PrintStatusBy)

            If isPrintQueueBatchDocumentsScreen Then
                spanQueuedFlags.Visible = False
                fsPrintQueueBatch.Visible = True
                PopulateFsPrintQueueBatch()
            End If
        End Sub

        Private Sub PopulateFsPrintQueueBatch()
            If Not isPrintQueueBatchDocumentsScreen Then Exit Sub

            Dim objBatch As DocumentPrintQueueBatch = Nothing
            Dim objPrinter As New DocumentPrinter(conn, String.Empty, String.Empty)
            Dim msg As ErrorMessage

            msg = DocumentPrintQueueBatchBL.Fetch(DocumentPrintQueueBatchID, conn, objBatch)
            If Not msg.Success Then Exit Sub

            txtCreated.Text = String.Format("{0} by {1}", objBatch.CreatedDate, objBatch.CreatedBy)
            txtDocumentCount.Text = objBatch.DocumentCount.ToString()
            txtComment.Text = objBatch.Comment

            msg = DocumentPrinterBL.Fetch(objBatch.DocumentPrinterID, objPrinter)
            If Not msg.Success Then Exit Sub

            txtPrinter.Text = objPrinter.PrinterName
        End Sub

        Private Sub BindDocumentTypes()
            Dim webSecurityUserID As Integer = SecurityBL.GetCurrentUser().ID
            Dim list As New DataClasses.Collections.vwDocumentTypesForWebSecurityUserCollection

            DocumentTypeBL.FetchDistinctList(conn:=conn, list:=list, _
                                             webSecurityUserID:=webSecurityUserID, _
                                             systemType:=Nothing, redundant:=Nothing)

            cboDocumentTypes = cpFilters.FindControl("cboDocumentTypes")

            With cboDocumentTypes
                .Items.Clear()
                .DataSource = list
                .DataValueField = "DocumentTypeID"
                .DataTextField = "DocumentType"
                .DataBind()
            End With

            ' select all document types
            For Each li As ListItem In cboDocumentTypes.Items
                li.Selected = True
            Next
        End Sub

        Private Sub InitialisePrintStatusCheckBoxes()
            ' right align the checkbox label
            CType(cpFilters.FindControl("chkNeverQueued"), CheckBoxEx).CheckBox.TextAlign = TextAlign.Right
            CType(cpFilters.FindControl("chkQueued"), CheckBoxEx).CheckBox.TextAlign = TextAlign.Right
            CType(cpFilters.FindControl("chkBatched"), CheckBoxEx).CheckBox.TextAlign = TextAlign.Right
            CType(cpFilters.FindControl("chkSentToPrinter"), CheckBoxEx).CheckBox.TextAlign = TextAlign.Right
            CType(cpFilters.FindControl("chkRemovedFromQueue"), CheckBoxEx).CheckBox.TextAlign = TextAlign.Right

            ' tick all checkboxes
            CType(cpFilters.FindControl("chkNeverQueued"), CheckBoxEx).CheckBox.Checked = True
            CType(cpFilters.FindControl("chkQueued"), CheckBoxEx).CheckBox.Checked = True
            CType(cpFilters.FindControl("chkBatched"), CheckBoxEx).CheckBox.Checked = True
            CType(cpFilters.FindControl("chkSentToPrinter"), CheckBoxEx).CheckBox.Checked = True
            CType(cpFilters.FindControl("chkRemovedFromQueue"), CheckBoxEx).CheckBox.Checked = True
        End Sub

        Private Sub SetJSfilterControlsIDs(ByVal thePage As BasePage)
            Dim strJS As New StringBuilder
            Dim strSetVar As String = "  list{0}ID = '{1}';" & Environment.NewLine
            Dim strSetVarEx As String = "  list{0}ID = '{1}_{2}';" & Environment.NewLine

            strJS.AppendFormat(strSetVar, DocumentFilterList.FilterDocumentTypes, cboDocumentTypes.ClientID)

            strJS.AppendFormat(strSetVar, DocumentFilterList.FilterOrigin, rboOrigin.ClientID)

            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterDescription, txtDescription.ClientID, "txtTextBox")

            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterCreatedFromDate, dteCreatedFrom.ClientID, "txtTextBox")
            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterCreatedToDate, dteCreatedTo.ClientID, "txtTextBox")
            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterCreatedBy, txtCreatedBy.ClientID, "txtTextBox")

            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterRecipientReference, txtRecipientReference.ClientID, "txtTextBox")
            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterRecipientName, txtRecipientName.ClientID, "txtTextBox")

            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterNeverQueued, chkNeverQueued.ClientID, "chkCheckbox")
            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterQueued, chkQueued.ClientID, "chkCheckbox")
            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterBatched, chkBatched.ClientID, "chkCheckbox")
            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterSentToPrinter, chkSentToPrinter.ClientID, "chkCheckbox")
            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterRemovedFromQueue, chkRemovedFromQueue.ClientID, "chkCheckbox")

            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterPrintStatusFromDate, dtePrintedFrom.ClientID, "txtTextBox")
            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterPrintStatusToDate, dtePrintedTo.ClientID, "txtTextBox")
            strJS.AppendFormat(strSetVarEx, DocumentFilterList.FilterPrintStatusBy, txtPrintedBy.ClientID, "txtTextBox")

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                       "Target.Abacus.Web.Apps.Documents.UserControls.DocumentSelector.SetFilterControlsIDs", _
                                                       strJS.ToString(), True)
        End Sub

        Private Sub PopulateFilterHelperObject(ByRef objFilterHelper As DocumentFilterHelper)
            If Not cboDocumentTypes Is Nothing Then
                For Each item As ListItem In cboDocumentTypes.Items
                    If item.Selected Then objFilterHelper.DocumentTypes.Add(item.Value)
                Next
            End If

            objFilterHelper.Origin = [Enum].Parse(GetType(DocumentOrigin), rboOrigin.SelectedValue)

            objFilterHelper.Description = txtDescription.Text

            If Not String.IsNullOrEmpty(dteCreatedFrom.Text) Then
                objFilterHelper.CreatedFromDate = CType(dteCreatedFrom.Value, DateTime)
            End If

            If Not String.IsNullOrEmpty(dteCreatedTo.Text) Then
                objFilterHelper.CreatedToDate = CType(dteCreatedTo.Value, DateTime)
            End If

            objFilterHelper.CreatedBy = txtCreatedBy.Text

            objFilterHelper.RecipientReference = txtRecipientReference.Text
            objFilterHelper.RecipientName = txtRecipientName.Text

            objFilterHelper.NeverQueued = chkNeverQueued.CheckBox.Checked
            objFilterHelper.Queued = chkQueued.CheckBox.Checked
            objFilterHelper.Batched = chkBatched.CheckBox.Checked
            objFilterHelper.SentToprinter = chkSentToPrinter.CheckBox.Checked

            If Not String.IsNullOrEmpty(dtePrintedFrom.Text) Then
                objFilterHelper.PrintStatusFromDate = CType(dtePrintedFrom.Value, DateTime)
            End If

            If Not String.IsNullOrEmpty(dtePrintedTo.Text) Then
                objFilterHelper.PrintStatusToDate = CType(dtePrintedTo.Value, DateTime)
            End If

            objFilterHelper.PrintStatusBy = txtPrintedBy.Text
        End Sub

        Private Sub ApplyFiltersFromQueryString()

            filterHelper.SetFiltersFromQueryString()

            If filterHelper.DocumentTypes.Count > 0 Then
                Dim docType As ListItem

                For Each docType In cboDocumentTypes.Items
                    docType.Selected = False
                Next

                For Each docTypeID As Integer In filterHelper.DocumentTypes
                    docType = cboDocumentTypes.Items.FindByValue(docTypeID.ToString())

                    If docType IsNot Nothing Then docType.Selected = True
                Next
            End If

            txtDescription.Text = filterHelper.Description

            txtRecipientReference.Text = filterHelper.RecipientReference
            txtRecipientName.Text = filterHelper.RecipientName

            txtPrintedBy.Text = filterHelper.PrintStatusBy

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim enquiry As Integer = Target.Library.Utils.ToInt32(Request.QueryString("enquiry"))

        End Sub

        Protected Sub cboDocumentTypes_DataBound(ByVal sender As Object, ByVal e As EventArgs) Handles cboDocumentTypes.DataBound
            ' add each document type's value to the corresponding SPAN's "alt" attribute
            ' (a crude way to provide JavaScript access to document type values for filtering purpose)
            For Each item As ListItem In cboDocumentTypes.Items
                item.Attributes.Add("alt", item.Value)
            Next
        End Sub

#End Region

    End Class

End Namespace